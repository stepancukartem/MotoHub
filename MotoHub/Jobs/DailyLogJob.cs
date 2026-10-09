using Quartz;

namespace MotoHub.Jobs
{
    public class DailyLogJob : IJob
    {
        private readonly ILogger<DailyLogJob> _logger;

        public DailyLogJob(ILogger<DailyLogJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation(
                "Фонове завдання MotoHub виконано о {Time}",
                DateTimeOffset.Now);

            return Task.CompletedTask;
        }
    }
}

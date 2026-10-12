
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

        public Task Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Фонове завдання MotoHub виконано о {Time}",
                DateTimeOffset.Now);

            return Task.CompletedTask;
        }

        ValueTask IJob.Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

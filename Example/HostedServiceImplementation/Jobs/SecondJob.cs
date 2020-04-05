using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace HostedServiceImplementation.Jobs
{
    public class SecondJob : IJob
    {
        private readonly ILogger _logger;

        public SecondJob(ILogger<SecondJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("SecondJob is running...");

            return Task.CompletedTask;
        }
    }
}
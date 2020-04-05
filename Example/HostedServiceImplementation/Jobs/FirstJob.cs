using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace HostedServiceImplementation.Jobs
{
    public class FirstJob : IJob
    {
        private readonly ILogger _logger;

        public FirstJob(ILogger<FirstJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("FirstJob is running...");

            return Task.CompletedTask;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

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
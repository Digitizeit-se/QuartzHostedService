using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;

namespace Digitizeit.Quartz.HostedService
{
    public class QuartzHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;

        public QuartzHostedService(IScheduler scheduler, ILogger<QuartzHostedService> logger = null)
        {
            _logger = logger ?? new NullLogger<QuartzHostedService>();
            _scheduler = scheduler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Quartz started...");
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Quartz stopped...");
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}
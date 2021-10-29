using Digitizeit.Quartz.HostedService.Interfaces;
using Digitizeit.Quartz.HostedService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Specialized;

namespace Digitizeit.Quartz.HostedService.Options
{
    public class QuartzBaseOptions
    {
        private readonly IDatabaseCreatorFactory _dbCreatorFactory;
        private readonly ILogger<QuartzBaseOptions> _logger;
        public Scheduler Scheduler { get; set; }
        public ThreadPool ThreadPool { get; set; }
        public Plugin Plugin { get; set; }
        public JobStore JobStore { get; set; }
        public Serializer Serializer { get; set; }
        private NameValueCollection _providerCollection;

        public QuartzBaseOptions(IConfiguration config, IServiceCollection services, IDatabaseCreatorFactory dbCreatorFactory, ILogger<QuartzBaseOptions> logger = null)
        {
            _dbCreatorFactory = dbCreatorFactory;
            _logger = logger ?? new NullLogger<QuartzBaseOptions>();
            if (config == null)
            {
                _logger.LogCritical("IConfiguration is null");
                throw new ArgumentNullException(nameof(config));
            }

            if (services == null)
            {
                _logger.LogCritical("IServiceCollection is null");
                throw new ArgumentNullException(nameof(services));
            }

            var section = config.GetSection("quartz");
            section.Bind(this);

            SetupProvider();
        }

        private void SetupProvider()
        {
            var databaseCreator = _dbCreatorFactory.GetDatabaseCreator(JobStore);
            if (databaseCreator == null)
            {
                _logger.LogDebug("using Provider InMemory.");
                return;
            }

            _providerCollection = databaseCreator.Init();
            _logger.LogDebug($"using Provider {_providerCollection["quartz.dataSource.default.provider"]}.");
        }

        public NameValueCollection ToProperties()
        {
            var properties = new NameValueCollection
            {
                ["quartz.scheduler.instanceName"] = Scheduler?.InstanceName ?? "defaultInstanceName" + Guid.NewGuid(),
                ["quartz.scheduler.instanceId"] = Scheduler?.InstanceId ?? Guid.NewGuid().ToString(),
                ["quartz.threadPool.type"] = ThreadPool?.Type ?? "Quartz.Simpl.SimpleThreadPool, Quartz",
                //["quartz.threadPool.threadPriority"] = ThreadPool?.ThreadPriority ?? "Normal",
                ["quartz.threadPool.threadCount"] = ThreadPool?.ThreadCount.ToString() ?? "10",
                ["quartz.plugin.jobInitializer.type"] = Plugin?.JobInitializer?.Type ?? "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins",
                ["quartz.plugin.jobInitializer.fileNames"] = Plugin?.JobInitializer?.FileNames ?? "quartz_jobs.xml",
                ["quartz.plugin.jobInitializer.scanInterval"] = Plugin?.JobInitializer?.ScanInterval ?? "0",
                ["quartz.plugin.triggHistory.type"] = "Quartz.Plugin.History.LoggingJobHistoryPlugin, Quartz.Plugins",
                ["quartz.serializer.type"] = Serializer?.Type ?? "json"
            };
            if (_providerCollection != null)
            {
                properties.Add(_providerCollection);
            }

            return properties;
        }
    }
}
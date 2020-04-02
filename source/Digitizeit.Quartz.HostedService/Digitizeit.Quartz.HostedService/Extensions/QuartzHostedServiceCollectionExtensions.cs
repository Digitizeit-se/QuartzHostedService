using Digitizeit.Quartz.HostedService.Factory;
using Digitizeit.Quartz.HostedService.Interfaces;
using Digitizeit.Quartz.HostedService.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz.Impl;
using Quartz.Spi;
using System;

namespace Digitizeit.Quartz.HostedService.Extensions
{
    public static class QuartzHostedServiceCollectionExtensions
    {
        public static IServiceCollection AddQuartzHostedService(this IServiceCollection services, IConfiguration config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            services.AddOptions();

            //database services
            services.AddTransient<QuartzSqlServerOption>();
            services.AddTransient<QuartzSqliteOptions>();
            services.AddTransient<QuartzMySqlOptions>();
            services.AddTransient<IDatabaseCreatorFactory, DatabaseCreatorFactory>();

            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton(provider =>
            {
                var option = new QuartzBaseOptions(config, services, provider.GetService<IDatabaseCreatorFactory>(), provider.GetService<ILogger<QuartzBaseOptions>>());
                var sf = new StdSchedulerFactory(option.ToProperties());
                var scheduler = sf.GetScheduler().Result;
                scheduler.JobFactory = provider.GetService<IJobFactory>();
                return scheduler;
            });

            services.AddHostedService<QuartzHostedService>();

            return services;
        }
    }
}
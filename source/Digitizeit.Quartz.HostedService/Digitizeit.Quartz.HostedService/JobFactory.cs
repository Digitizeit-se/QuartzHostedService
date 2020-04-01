﻿using Quartz;
using Quartz.Spi;
using System;

namespace Digitizeit.Quartz.HostedService
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var job = _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            return job;
        }

        public void ReturnJob(IJob job)
        {
            var obj = (IDisposable)job;
            obj?.Dispose();
        }
    }
}
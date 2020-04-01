using Digitizeit.Quartz.HostedService.Interfaces;
using Digitizeit.Quartz.HostedService.MySql;
using Digitizeit.Quartz.HostedService.SqLite;
using Digitizeit.Quartz.HostedService.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Digitizeit.Quartz.HostedService.Models;
using Digitizeit.Quartz.HostedService.Options;

namespace Digitizeit.Quartz.HostedService.Factory
{
    public class DatabaseCreatorFactory : IDatabaseCreatorFactory
    {
        private readonly IServiceProvider _services;

        public DatabaseCreatorFactory(IServiceProvider services)
        {
            _services = services;
        }

        public ICreateDatabase GetDatabaseCreator(JobStoreSettings jobStoreSettings)
        {
            ICreateDatabase createDb;
            switch (jobStoreSettings.Provider)
            {
                case "SqlServer":
                    createDb = GetSqlCreator(jobStoreSettings);
                    break;

                case "sqlite-custom":
                    createDb = GetSqliteCreator(jobStoreSettings);
                    break;

                case "MySql-50":
                    createDb = GetMySqlCreator(jobStoreSettings);
                    break;

                default:
                    createDb = null;
                    break;
            }

            return createDb;
        }

        private ICreateDatabase GetMySqlCreator(JobStoreSettings jobStoreSettings)
        {
            return new CreateMySqlDatabase(_services.GetService<ILogger<CreateMySqlDatabase>>(), _services.GetService<QuartzMySqlOptions>(), jobStoreSettings);
        }

        private ICreateDatabase GetSqlCreator(JobStoreSettings jobStoreSettings)
        {
            return new CreateSqlDatabase(_services.GetService<ILogger<CreateSqlDatabase>>(), _services.GetService<QuartzSqlServerOption>(), jobStoreSettings);
        }

        private ICreateDatabase GetSqliteCreator(JobStoreSettings jobStoreSettings)
        {
            return new CreateSqLiteDatabase(_services.GetService<ILogger<CreateSqLiteDatabase>>(), _services.GetService<QuartzSqliteOptions>(), jobStoreSettings);
        }
    }
}
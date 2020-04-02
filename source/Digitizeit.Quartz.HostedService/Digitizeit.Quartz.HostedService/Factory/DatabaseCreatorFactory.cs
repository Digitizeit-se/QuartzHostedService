using Digitizeit.Quartz.HostedService.Interfaces;
using Digitizeit.Quartz.HostedService.MySql;
using Digitizeit.Quartz.HostedService.SqLite;
using Digitizeit.Quartz.HostedService.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Digitizeit.Quartz.HostedService.Models;
using Digitizeit.Quartz.HostedService.Options;
using Digitizeit.Quartz.HostedService.Postgres;

namespace Digitizeit.Quartz.HostedService.Factory
{
    public class DatabaseCreatorFactory : IDatabaseCreatorFactory
    {
        private readonly IServiceProvider _services;

        public DatabaseCreatorFactory(IServiceProvider services)
        {
            _services = services;
        }

        public ICreateDatabase GetDatabaseCreator(JobStore jobStore)
        {
            ICreateDatabase createDb = null;
            if (jobStore == null) return createDb;

            switch (jobStore.Provider)
            {
                case "SqlServer":
                    createDb = GetSqlCreator(jobStore);
                    break;

                case "sqlite-custom":
                    createDb = GetSqliteCreator(jobStore);
                    break;

                case "MySql-50":
                    createDb = GetMySqlCreator(jobStore);
                    break;

                case "MySql-51":
                    createDb = GetMySqlCreator(jobStore);
                    break;

                case "MySql-65":
                    createDb = GetMySqlCreator(jobStore);
                    break;

                case "Npgsql-20":
                    createDb = GetPostgresCreator(jobStore);
                    break;

                default:
                    createDb = null;
                    break;
            }

            return createDb;
        }

        private ICreateDatabase GetMySqlCreator(JobStore jobStoreSettings)
        {
            return new CreateMySqlDatabase(_services.GetService<ILogger<CreateMySqlDatabase>>(), _services.GetService<QuartzMySqlOptions>(), jobStoreSettings);
        }

        private ICreateDatabase GetSqlCreator(JobStore jobStoreSettings)
        {
            return new CreateSqlDatabase(_services.GetService<ILogger<CreateSqlDatabase>>(), _services.GetService<QuartzSqlServerOption>(), jobStoreSettings);
        }

        private ICreateDatabase GetSqliteCreator(JobStore jobStoreSettings)
        {
            return new CreateSqLiteDatabase(_services.GetService<ILogger<CreateSqLiteDatabase>>(), _services.GetService<QuartzSqliteOptions>(), jobStoreSettings);
        }

        private ICreateDatabase GetPostgresCreator(JobStore jobStoreSettings)
        {
            return new CreatePostgresDatabase(_services.GetService<ILogger<CreatePostgresDatabase>>(), _services.GetService<QuartzPostgresOptions>(), jobStoreSettings);
        }
    }
}
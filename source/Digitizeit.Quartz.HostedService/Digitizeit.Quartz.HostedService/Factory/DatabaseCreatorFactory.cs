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
            if (jobStore == null) return null;

            var createDb = jobStore.Provider switch
            {
                "SqlServer" => GetSqlCreator(jobStore),
                "sqlite-custom" => GetSqliteCreator(jobStore),
                "MySql-50" => GetMySqlCreator(jobStore),
                "MySql-51" => GetMySqlCreator(jobStore),
                "MySql-65" => GetMySqlCreator(jobStore),
                "Npgsql-20" => GetPostgresCreator(jobStore),
                _ => null
            };

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
﻿using Digitizeit.Quartz.HostedService.Extensions;
using Digitizeit.Quartz.HostedService.Models;
using Microsoft.Data.Sqlite;
using Quartz.Impl.AdoJobStore.Common;
using System.Collections.Specialized;
using System.Data;

namespace Digitizeit.Quartz.HostedService.Options
{
    public class QuartzSqliteOptions
    {
        private NameValueCollection _dataBaseProperties;

        public NameValueCollection GetDatabaseProperties(JobStore jobStore)
        {
            SetProperties(jobStore);
            return _dataBaseProperties;
        }

        private void SetProperties(JobStore jobStore)
        {
            SetDbMetaData();
            _dataBaseProperties = new NameValueCollection
            {
                ["quartz.jobStore.misfireThreshold"] = jobStore?.MisfireThreshold ?? "600000",
                ["quartz.jobStore.type"] = jobStore?.Type ?? "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.jobStore.useProperties"] = jobStore?.UseProperties ?? "true",
                ["quartz.jobStore.dataSource"] = jobStore?.DataSource ?? "default",
                ["quartz.jobStore.tablePrefix"] = jobStore?.TablePrefix ?? "QRTZ_",
                ["quartz.dataSource.default.connectionString"] = jobStore?.ConnectionString.GetSqliteConnectionString(),
                ["quartz.dataSource.default.provider"] = jobStore?.Provider,
                ["quartz.jobStore.driverDelegateType"] = jobStore?.DriverDelegate ?? "Quartz.Impl.AdoJobStore.SQLiteDelegate, Quartz"

                //["quartz.dataSource.default.provider"] = "sqlite-custom",
            };
        }

        private static void SetDbMetaData()
        {
            DbProvider.RegisterDbMetadata("sqlite-custom", new DbMetadata()
            {
                AssemblyName = typeof(SqliteConnection).Assembly.GetName().Name,
                ConnectionType = typeof(SqliteConnection),
                CommandType = typeof(SqliteCommand),
                ParameterType = typeof(SqliteParameter),
                ParameterDbType = typeof(DbType),
                ParameterDbTypePropertyName = "DbType",
                ParameterNamePrefix = "@",
                ExceptionType = typeof(SqliteException),
                BindByName = true
            });
        }
    }
}
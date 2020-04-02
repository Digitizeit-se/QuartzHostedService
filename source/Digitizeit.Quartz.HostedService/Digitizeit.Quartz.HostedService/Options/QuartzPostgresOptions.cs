using Digitizeit.Quartz.HostedService.Models;
using Quartz.Impl.AdoJobStore.Common;
using System.Collections.Specialized;
using System.Data;
using MySql.Data.MySqlClient;
using Npgsql;

namespace Digitizeit.Quartz.HostedService.Options
{
    public class QuartzPostgresOptions
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
                ["quartz.dataSource.default.connectionString"] = jobStore?.ConnectionString,
                ["quartz.dataSource.default.provider"] = jobStore?.Provider ?? "Npgsql-20",
                ["quartz.jobStore.driverDelegateType"] = jobStore?.DriverDelegate ?? "Quartz.Impl.AdoJobStore.PostgreSQLDelegate, Quartz"
            };
        }

        private static void SetDbMetaData()
        {
            DbProvider.RegisterDbMetadata("Npgsql-20", new DbMetadata()
            {
                AssemblyName = typeof(NpgsqlConnection).Assembly.GetName().Name,
                ConnectionType = typeof(NpgsqlConnection),
                CommandType = typeof(NpgsqlCommand),
                ParameterType = typeof(NpgsqlParameter),
                ParameterDbType = typeof(DbType),
                ParameterDbTypePropertyName = "DbType",
                ParameterNamePrefix = "@",
                ExceptionType = typeof(NpgsqlException),
                BindByName = true
            });
        }
    }
}
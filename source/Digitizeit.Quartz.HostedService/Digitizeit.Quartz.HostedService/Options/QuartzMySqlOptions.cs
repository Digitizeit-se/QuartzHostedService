using Digitizeit.Quartz.HostedService.Models;
using MySql.Data.MySqlClient;
using Quartz.Impl.AdoJobStore.Common;
using System.Collections.Specialized;
using System.Data;

namespace Digitizeit.Quartz.HostedService.Options
{
    public class QuartzMySqlOptions
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
                ["quartz.dataSource.default.provider"] = jobStore?.Provider ?? "MySql-50",
                ["quartz.jobStore.driverDelegateType"] = jobStore?.DriverDelegate ?? "Quartz.Impl.AdoJobStore.MySQLDelegate, Quartz"
            };
        }

        private static void SetDbMetaData()
        {
            DbProvider.RegisterDbMetadata("MySql-50", new DbMetadata()
            {
                AssemblyName = typeof(MySqlConnection).Assembly.GetName().Name,
                ConnectionType = typeof(MySqlConnection),
                CommandType = typeof(MySqlCommand),
                ParameterType = typeof(MySqlParameter),
                ParameterDbType = typeof(DbType),
                ParameterDbTypePropertyName = "DbType",
                ParameterNamePrefix = "@",
                ExceptionType = typeof(MySqlException),
                BindByName = true
            });
        }
    }
}
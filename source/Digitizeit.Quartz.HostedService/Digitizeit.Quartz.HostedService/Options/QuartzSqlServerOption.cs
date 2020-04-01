using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Digitizeit.Quartz.HostedService.Models;
using Digitizeit.Quartz.HostedService.SqlServer;

namespace Digitizeit.Quartz.HostedService.Options
{
    public class QuartzSqlServerOption
    {
        private NameValueCollection _dataBaseProperties;
        private readonly CreateSqlDatabase _createSqlDatabase;

        public QuartzSqlServerOption(CreateSqlDatabase createSqlDatabase)
        {
            _createSqlDatabase = createSqlDatabase;
        }

        public NameValueCollection GetDatabaseProperties(JobStoreSettings jobStore)
        {
            SetProperties(jobStore);
            //_createSqlDatabase.Init(jobStore.ConnectionString);
            return _dataBaseProperties;
        }

        private void SetProperties(JobStoreSettings jobStore)
        {
            _dataBaseProperties = new NameValueCollection
            {
                ["quartz.jobStore.misfireThreshold"] = jobStore?.MisfireThreshold ?? "600000",
                ["quartz.jobStore.type"] = jobStore?.Type ?? "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.jobStore.useProperties"] = jobStore?.UseProperties ?? "true",
                ["quartz.jobStore.dataSource"] = jobStore?.DataSource ?? "default",
                ["quartz.jobStore.tablePrefix"] = jobStore?.TablePrefix ?? "QRTZ_",
                // ["quartz.jobStore.lockHandler.type"] = jobStore?.LockHandler ?? "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz",
                ["quartz.dataSource.default.connectionString"] = jobStore?.ConnectionString,
                ["quartz.dataSource.default.provider"] = jobStore?.Provider,
                ["quartz.jobStore.driverDelegateType"] = jobStore?.DriverDelegate ?? "Quartz.Impl.AdoJobStore.SqlServerDelegate,Quartz"
            };
        }
    }
}
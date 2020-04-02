using Digitizeit.Quartz.HostedService.Models;
using System.Collections.Specialized;

namespace Digitizeit.Quartz.HostedService.Options
{
    public class QuartzSqlServerOption
    {
        private NameValueCollection _dataBaseProperties;

        public NameValueCollection GetDatabaseProperties(JobStore jobStore)
        {
            SetProperties(jobStore);
            return _dataBaseProperties;
        }

        private void SetProperties(JobStore jobStore)
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
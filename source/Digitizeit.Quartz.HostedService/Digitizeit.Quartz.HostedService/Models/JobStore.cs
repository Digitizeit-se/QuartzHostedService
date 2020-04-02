namespace Digitizeit.Quartz.HostedService.Models
{
    public class JobStore
    {
        public string MisfireThreshold { get; set; }
        public string Type { get; set; }
        public string UseProperties { get; set; }
        public string DataSource { get; }
        public string TablePrefix { get; set; }
        public string LockHandler { get; set; }
        public string ConnectionString { get; set; }
        public string Provider { get; set; }
        public string DriverDelegate { get; set; }
    }
}
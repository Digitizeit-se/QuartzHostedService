namespace Digitizeit.Quartz.HostedService.Models
{
    public class JobInitializer
    {
        public string Type { get; set; }
        public string FileNames { get; set; }
        public string ScanInterval { get; set; }
    }
}
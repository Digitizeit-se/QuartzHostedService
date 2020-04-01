namespace Digitizeit.Quartz.HostedService.Models
{
    public class ThreadPoolSettings
    {
        public string Type { get; set; }
        public string ThreadPriority { get; set; }
        public int ThreadCount { get; set; }
    }
}
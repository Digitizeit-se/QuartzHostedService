namespace Digitizeit.Quartz.HostedService.Models
{
    public class JobSettings
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public string JobType { get; set; }
        public bool Durable { get; set; } = true;
        public bool Recover { get; set; } = true;
    }
}
namespace Digitizeit.Quartz.HostedService.Interfaces
{
    public interface ICreateDatabase
    {
        void Init(string connectionString);
    }
}
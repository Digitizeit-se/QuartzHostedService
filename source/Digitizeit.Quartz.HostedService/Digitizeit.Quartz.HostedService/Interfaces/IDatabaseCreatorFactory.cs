using Digitizeit.Quartz.HostedService.Models;

namespace Digitizeit.Quartz.HostedService.Interfaces
{
    public interface IDatabaseCreatorFactory
    {
        ICreateDatabase GetDatabaseCreator(JobStoreSettings jobStoreSettings);
    }
}
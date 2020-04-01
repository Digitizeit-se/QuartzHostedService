using System.Collections.Specialized;

namespace Digitizeit.Quartz.HostedService.Interfaces
{
    public interface ICreateDatabase
    {
        NameValueCollection Init();
    }
}
using Microsoft.Azure.Management.AppService.Fluent;

namespace OrchardCore.LetsEncrypt.Services
{
    public interface IAzureServiceManager
    {
        IAppServiceManager GetAppServiceManager();
    }
}

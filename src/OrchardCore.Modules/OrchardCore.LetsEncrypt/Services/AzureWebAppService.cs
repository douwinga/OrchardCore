using System.Threading.Tasks;

namespace OrchardCore.LetsEncrypt.Services
{
    public class AzureWebAppService : IAzureWebAppService
    {
        private readonly IAzureServiceManager _azureServiceManager;

        public AzureWebAppService(IAzureServiceManager azureServiceManager)
        {
            _azureServiceManager = azureServiceManager;
        }

        public Task InstallCertificate()
        {
            var appServiceManager = _azureServiceManager.GetAppServiceManager();

            // TODO: Add function to get site or slot to app service manager

            return Task.CompletedTask;
        }
    }
}

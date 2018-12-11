using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Options;
using OrchardCore.LetsEncrypt.Settings;

namespace OrchardCore.LetsEncrypt.Services
{
    public class AzureServiceManager : IAzureServiceManager
    {
        private readonly LetsEncryptAzureAuthSettings _azureAuthSettings;

        public AzureServiceManager(IOptions<LetsEncryptAzureAuthSettings> azureAuthSettings)
        {
            _azureAuthSettings = azureAuthSettings.Value;
        }

        public IAppServiceManager GetAppServiceManager()
        {
            return AppServiceManager.Authenticate(GetAzureCredentials(), _azureAuthSettings.SubscriptionId);
        }

        private AzureCredentials GetAzureCredentials()
        {
            var servicePrincipalLoginInformation = new ServicePrincipalLoginInformation
            {
                ClientId = _azureAuthSettings.ClientId,
                ClientSecret = _azureAuthSettings.ClientSecret
            };

            return new AzureCredentials(servicePrincipalLoginInformation, _azureAuthSettings.Tenant, AzureEnvironment.AzureGlobalCloud);
        }
    }
}

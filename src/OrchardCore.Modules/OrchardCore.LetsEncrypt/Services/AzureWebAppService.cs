using System.Threading.Tasks;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;
using OrchardCore.LetsEncrypt.Settings;

namespace OrchardCore.LetsEncrypt.Services
{
    public class AzureWebAppService : IAzureWebAppService
    {
        private readonly IShellHost _orchardHost;
        private readonly LetsEncryptAzureAuthSettings _azureAuthSettings;
        private readonly IAppServiceManager _appServiceManager;

        public AzureWebAppService(IShellHost orchardHost)
        {
            _orchardHost = orchardHost;
            _azureAuthSettings = GetAzureAuthSettings().GetAwaiter().GetResult();
            _appServiceManager = GetAppServiceManager();
        }

        public IWebAppBase GetWebApp()
        {
            var site = _appServiceManager.WebApps.GetByResourceGroup(_azureAuthSettings.ResourceGroupName, _azureAuthSettings.WebAppName);
            var siteOrSlot = (IWebAppBase)site;

            if (!string.IsNullOrEmpty(_azureAuthSettings.SiteSlotName))
            {
                var slot = site.DeploymentSlots.GetByName(_azureAuthSettings.SiteSlotName);
                siteOrSlot = slot;
            }

            return siteOrSlot;
        }

        public async Task<IPagedCollection<IAppServiceCertificate>> GetAppServiceCertificatesAsync()
        {
            return await _appServiceManager.AppServiceCertificates
                .ListByResourceGroupAsync(_azureAuthSettings.ServicePlanResourceGroupName ?? _azureAuthSettings.ResourceGroupName);
        }

        public Task InstallCertificateAsync()
        {
            // TODO: Add function to get site or slot to app service manager

            return Task.CompletedTask;
        }

        private async Task<LetsEncryptAzureAuthSettings> GetAzureAuthSettings()
        {
            // Get the azure auth settings from the default tenant
            var shellSettings = _orchardHost.GetSettings(ShellHelper.DefaultShellName);

            using (var scope = await _orchardHost.GetScopeAsync(shellSettings))
            {
                return scope.ServiceProvider.GetRequiredService<IOptions<LetsEncryptAzureAuthSettings>>().Value;
            }
        }

        private IAppServiceManager GetAppServiceManager()
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

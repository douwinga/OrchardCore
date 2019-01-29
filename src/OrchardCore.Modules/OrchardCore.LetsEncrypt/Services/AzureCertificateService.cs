using System.Threading.Tasks;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using OrchardCore.LetsEncrypt.Models;

namespace OrchardCore.LetsEncrypt.Services
{
    public class AzureCertificateService : IAzureCertificateService
    {
        private readonly IAzureWebAppService _azureWebAppService;
        private readonly IAzureAuthSettingsService _azureAuthSettingsService;

        public AzureCertificateService(IAzureWebAppService azureWebAppService, IAzureAuthSettingsService azureAuthSettingsService)
        {
            _azureWebAppService = azureWebAppService;
            _azureAuthSettingsService = azureAuthSettingsService;
        }


        public async Task InstallAsync(CertificateInstallModel certInstallModel)
        {
            var azureAuthSettings = await _azureAuthSettingsService.GetAzureAuthSettingsAsync();
            var webApp = await _azureWebAppService.GetWebAppAsync();

            var cert = new CertificateInner()
            {
                PfxBlob = certInstallModel.PfxCertificate,
                Password = certInstallModel.PfxPassword,
                Location = webApp.Inner.Location,
                ServerFarmId = webApp.Inner.ServerFarmId
            };

            await webApp.Manager.AppServiceCertificates.Inner.CreateOrUpdateWithHttpMessagesAsync(azureAuthSettings.ServicePlanResourceGroupName, certInstallModel.CertInfo.CommonName, cert);
        }
    }
}

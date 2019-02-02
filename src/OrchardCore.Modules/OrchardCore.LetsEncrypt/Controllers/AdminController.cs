using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.LetsEncrypt.Services;
using OrchardCore.LetsEncrypt.Settings;
using OrchardCore.LetsEncrypt.ViewModels;

namespace OrchardCore.LetsEncrypt.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAzureWebAppService _azureWebAppService;
        private readonly IAzureCertificateService _azureCertificateService;
        private readonly ILetsEncryptService _letsEncryptService;
        private readonly LetsEncryptAzureAuthSettings _azureAuthSettings;
        private readonly INotifier _notifier;

        public AdminController(
            IAzureWebAppService azureWebAppService,
            IAzureCertificateService azureCertificateService,
            ILetsEncryptService letsEncryptService,
            IOptions<LetsEncryptAzureAuthSettings> azureAuthSettings,
            INotifier notifier,
            IHtmlLocalizer<AdminController> localizer
            )
        {
            _azureWebAppService = azureWebAppService;
            _azureCertificateService = azureCertificateService;
            _letsEncryptService = letsEncryptService;
            _azureAuthSettings = azureAuthSettings.Value;
            _notifier = notifier;

            T = localizer;
        }

        public IHtmlLocalizer T { get; }

        public async Task<IActionResult> InstallAzureCertificate()
        {
            return View(await BuildInstallAzureCertificateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> InstallAzureCertificate(InstallAzureCertificateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await BuildInstallAzureCertificateViewModel(model);
                return View(model);
            }

            var certInstallModel = await _letsEncryptService.RequestHttpChallengeCertificate(model.RegistrationEmail, model.Hostnames, model.UseStaging);

            await _azureCertificateService.InstallAsync(certInstallModel);

            _notifier.Success(T["Successfully installed Let's Encrypt certificate!"]);

            return RedirectToAction("InstallAzureCertificate");
        }

        // TODO: Need to change this as we don't need this much info on tenants
        private async Task<InstallAzureCertificateViewModel> BuildInstallAzureCertificateViewModel(InstallAzureCertificateViewModel model = null)
        {
            if (model == null)
            {
                model = new InstallAzureCertificateViewModel();
            }

            var webApp = await _azureWebAppService.GetWebAppAsync();

            model.AvailableHostNames = webApp.HostNames;
            model.HostNameSslStates = webApp.HostNameSslStates;
            model.InstalledCertificates = await _azureWebAppService.GetAppServiceCertificatesAsync();

            return model;
        }
    }
}

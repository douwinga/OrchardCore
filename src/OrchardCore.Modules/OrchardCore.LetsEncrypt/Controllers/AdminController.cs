using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.LetsEncrypt.Services;
using OrchardCore.LetsEncrypt.Settings;
using OrchardCore.LetsEncrypt.ViewModels;

namespace OrchardCore.LetsEncrypt.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAzureServiceManager _azureServiceManager;
        private readonly ILetsEncryptService _letsEncryptService;
        private readonly LetsEncryptAzureAuthSettings _azureAuthSettings;
        private readonly INotifier _notifier;

        public AdminController(
            IAzureServiceManager azureServiceManager,
            ILetsEncryptService letsEncryptService,
            IOptions<LetsEncryptAzureAuthSettings> azureAuthSettings,
            INotifier notifier,
            IHtmlLocalizer<AdminController> localizer
            )
        {
            _azureServiceManager = azureServiceManager;
            _letsEncryptService = letsEncryptService;
            _azureAuthSettings = azureAuthSettings.Value;
            _notifier = notifier;

            T = localizer;
        }

        public IHtmlLocalizer T { get; }

        public async Task<IActionResult> AzureInstallCertificate()
        {
            //var shells = await GetShellsAsync();
            //var dataProtector = _dataProtectorProvider.CreateProtector("Tokens").ToTimeLimitedDataProtector();

            //var model = new AdminIndexViewModel
            //{
            //    ShellSettingsEntries = shells.Select(x =>
            //    {
            //        var entry = new ShellSettingsEntry
            //        {
            //            Name = x.Settings.Name,
            //            ShellSettings = x.Settings,
            //            IsDefaultTenant = string.Equals(x.Settings.Name, ShellHelper.DefaultShellName, StringComparison.OrdinalIgnoreCase)
            //        };

            //        if (x.Settings.State == TenantState.Uninitialized && !string.IsNullOrEmpty(x.Settings.Secret))
            //        {
            //            entry.Token = dataProtector.Protect(x.Settings.Secret, _clock.UtcNow.Add(new TimeSpan(24, 0, 0)));
            //        }

            //        return entry;
            //    }).ToList()
            //};

            return View(await BuildAzureInstallCertificateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AzureInstallCertificate(AzureInstallCertificateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await BuildAzureInstallCertificateViewModel(model);
                return View("AzureInstallCertificate", model);
            }

            await _letsEncryptService.RequestHttpChallengeCertificate(model.RegistrationEmail, model.Hostnames, model.UseStaging);

            _notifier.Success(T["Successfully installed Let's Encrypt certificate!"]);

            return RedirectToAction("AzureInstallCertificate");
        }

        private async Task<AzureInstallCertificateViewModel> BuildAzureInstallCertificateViewModel(AzureInstallCertificateViewModel model = null)
        {
            if (model == null)
            {
                model = new AzureInstallCertificateViewModel();
            }
            var appServiceManager = _azureServiceManager.GetAppServiceManager();

            var site = appServiceManager.WebApps.GetByResourceGroup(_azureAuthSettings.ResourceGroupName, _azureAuthSettings.WebAppName);
            var siteOrSlot = (IWebAppBase)site;

            if (!string.IsNullOrEmpty(_azureAuthSettings.SiteSlotName))
            {
                var slot = site.DeploymentSlots.GetByName(_azureAuthSettings.SiteSlotName);
                siteOrSlot = slot;
            }

            model.AvailableHostNames = siteOrSlot.HostNames;
            model.HostNameSslStates = siteOrSlot.HostNameSslStates;
            model.InstalledCertificates = await appServiceManager.AppServiceCertificates
                .ListByResourceGroupAsync(_azureAuthSettings.ServicePlanResourceGroupName ?? _azureAuthSettings.ResourceGroupName);

            return model;
        }
    }
}

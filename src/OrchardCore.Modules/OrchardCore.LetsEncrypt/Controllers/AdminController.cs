using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Extensions.Options;
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

        public AdminController(
            IAzureServiceManager azureServiceManager,
            ILetsEncryptService letsEncryptService,
            IOptions<LetsEncryptAzureAuthSettings> azureAuthSettings)
        {
            _azureServiceManager = azureServiceManager;
            _letsEncryptService = letsEncryptService;
            _azureAuthSettings = azureAuthSettings.Value;
        }

        public async Task<IActionResult> AzureSettings()
        {
            var appServiceManager = _azureServiceManager.GetAppServiceManager();

            var site = appServiceManager.WebApps.GetByResourceGroup(_azureAuthSettings.ResourceGroupName, _azureAuthSettings.WebAppName);
            IWebAppBase siteOrSlot = site;

            if (!string.IsNullOrEmpty(_azureAuthSettings.SiteSlotName))
            {
                var slot = site.DeploymentSlots.GetByName(_azureAuthSettings.SiteSlotName);
                siteOrSlot = slot;
            }

            var model = new AzureSettingsViewModel
            {
                HostNames = siteOrSlot.HostNames,
                HostNameSslStates = siteOrSlot.HostNameSslStates,
                Certificates = await appServiceManager.AppServiceCertificates.ListByResourceGroupAsync(_azureAuthSettings.ServicePlanResourceGroupName ?? _azureAuthSettings.ResourceGroupName)
            };

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

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Install()
        {
            await _letsEncryptService.RequestDnsChallengeCertificate("test@example.com", "example.com", true);
            return View();
        }
    }
}

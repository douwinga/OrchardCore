using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Extensions.Options;
using OrchardCore.LetsEncrypt.Extensions;
using OrchardCore.LetsEncrypt.Services;
using OrchardCore.LetsEncrypt.Settings;
using OrchardCore.LetsEncrypt.ViewModels;

namespace OrchardCore.LetsEncrypt.Controllers
{
    public class AdminController : Controller
    {
        private readonly IArmService _armService;
        private readonly LetsEncryptAzureAuthSettings _azureAuthSettings;

        public AdminController(IArmService armService, IOptions<LetsEncryptAzureAuthSettings> azureAuthSettings)
        {
            _armService = armService;
            _azureAuthSettings = azureAuthSettings.Value;
        }

        public async Task<IActionResult> AzureSettings()
        {
            var client = await _armService.GetWebSiteManagementClient();

            var site = client.WebApps.GetSiteOrSlot(_azureAuthSettings);

            var model = new AzureSettingsViewModel
            {
                HostNames = site.HostNames,
                HostNameSslStates = site.HostNameSslStates,
                Certificates = client.Certificates.ListByResourceGroup(_azureAuthSettings.ServicePlanResourceGroupName).ToList()
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
    }
}

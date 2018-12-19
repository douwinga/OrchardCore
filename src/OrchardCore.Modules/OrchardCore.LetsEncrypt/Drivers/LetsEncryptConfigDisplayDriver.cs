using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Environment.Shell;
using OrchardCore.LetsEncrypt.Configuration;
using OrchardCore.LetsEncrypt.Settings;
using OrchardCore.Settings;

namespace OrchardCore.LetsEncrypt.Drivers
{
    public class LetsEncryptConfigSettingsDisplayDriver : SectionDisplayDriver<ISite, LetsEncryptConfigSettings>
    {
        private const string SettingsGroupId = "OrchardCore.LetsEncrypt";

        private readonly IAuthorizationService _authorizationService;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IShellHost _shellHost;
        private readonly ShellSettings _shellSettings;

        public LetsEncryptConfigSettingsDisplayDriver(
            IAuthorizationService authorizationService,
            IDataProtectionProvider dataProtectionProvider,
            IHttpContextAccessor httpContextAccessor,
            IShellHost shellHost,
            ShellSettings shellSettings
            )
        {
            _authorizationService = authorizationService;
            _dataProtectionProvider = dataProtectionProvider;
            _httpContextAccessor = httpContextAccessor;
            _shellHost = shellHost;
            _shellSettings = shellSettings;
        }

        public override async Task<IDisplayResult> EditAsync(LetsEncryptConfigSettings settings, BuildEditorContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !await _authorizationService.AuthorizeAsync(user, Permissions.ManageLetsEncryptSettings))
            {
                return null;
            }

            return Initialize<LetsEncryptConfigSettings>("LetsEncryptConfigSettings_Edit", model =>
            {
                model.PfxPassword = settings.PfxPassword;
                model.Country = settings.Country;
                model.StateOrProvince = settings.StateOrProvince;
                model.Locality = settings.Locality;
                model.Organization = settings.Organization;
                model.OrganizationUnit = settings.OrganizationUnit;
            }).Location("Content:2").OnGroup(SettingsGroupId);
        }

        public override async Task<IDisplayResult> UpdateAsync(LetsEncryptConfigSettings settings, BuildEditorContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !await _authorizationService.AuthorizeAsync(user, Permissions.ManageLetsEncryptSettings))
            {
                return null;
            }

            if (context.GroupId == SettingsGroupId)
            {
                var previousPfxPassword = settings.PfxPassword;
                await context.Updater.TryUpdateModelAsync(settings, Prefix);

                // Restore pfx password if the input is empty.
                if (string.IsNullOrWhiteSpace(settings.PfxPassword))
                {
                    settings.PfxPassword = previousPfxPassword;
                }
                else
                {
                    // encrypt the client secret
                    var protector = _dataProtectionProvider.CreateProtector(nameof(LetsEncryptConfigSettingsConfiguration));
                    settings.PfxPassword = protector.Protect(settings.PfxPassword);
                }

                // Reload the tenant to apply the settings
                await _shellHost.ReloadShellContextAsync(_shellSettings);
            }

            return await EditAsync(settings, context);
        }
    }
}
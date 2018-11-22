using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Environment.Shell;
using OrchardCore.Navigation;

namespace OrchardCore.LetsEncrypt
{
    public class AdminMenu : INavigationProvider
    {
        private readonly IStringLocalizer<AdminMenu> T;
        private readonly ShellSettings _shellSettings;

        public AdminMenu(IStringLocalizer<AdminMenu> localizer, ShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
            T = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            // Don't add the menu item on non-default tenants
            if (_shellSettings.Name != ShellHelper.DefaultShellName)
            {
                return Task.CompletedTask;
            }

            builder
                .Add(T["Configuration"], configuration => configuration
                    .Add(T["Let's Encrypt Azure Auth"], T["Let's Encrypt Azure Auth"], entry => entry
                        .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = "OrchardCore.LetsEncrypt.Azure.Auth" })
                        .Permission(Permissions.ManageLetsEncryptAzureAuthSettings)
                        .LocalNav()
                ));

            return Task.CompletedTask;
        }
    }
}

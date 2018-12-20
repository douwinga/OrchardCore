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

            builder.Add(T["Let's Encrypt"], "16", category =>
            {
                category.AddClass("lock").Id("lock");

                if (_shellSettings.Name == ShellHelper.DefaultShellName)
                {
                    category.Add(T["Azure Authentication"], T["Azure Authentication"], client => client
                        .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = "OrchardCore.LetsEncrypt.Azure.Auth" })
                        .Permission(Permissions.ManageLetsEncryptAzureAuthSettings)
                        .LocalNav()
                    );
                }

                category.Add(T["Cert Configuration"], T["Cert Configuration"], client => client
                    .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = "OrchardCore.LetsEncrypt.Cert" })
                    .Permission(Permissions.ManageLetsEncryptSettings)
                    .LocalNav()
                );

                category.Add(T["Install Azure Cert"], T["Install Azure Cert"], azureEntry => azureEntry
                    .Action("InstallAzureCertificate", "Admin", new { area = "OrchardCore.LetsEncrypt", groupId = "OrchardCore.LetsEncrypt.AzureInstall" })
                    .Permission(Permissions.ManageLetsEncryptSettings)
                    .LocalNav()
                );
            });

            return Task.CompletedTask;
        }
    }
}

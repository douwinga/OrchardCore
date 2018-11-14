using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace OrchardCore.LetsEncrypt
{
    public class AdminMenu : INavigationProvider
    {
        private readonly IStringLocalizer<AdminMenu> T;

        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;

            builder
                .Add(T["Configuration"], configuration => configuration
                    .Add(T["Let's Encrypt"], T["Let's Encrypt"], entry => entry
                        .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = "LetsEncrypt" })
                        .Permission(Permissions.ManageLetsEncryptSettings)
                        .LocalNav()
                ));

            return Task.CompletedTask;
        }
    }
}

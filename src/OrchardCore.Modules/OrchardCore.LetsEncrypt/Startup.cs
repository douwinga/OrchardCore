using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.LetsEncrypt.Configuration;
using OrchardCore.LetsEncrypt.Drivers;
using OrchardCore.LetsEncrypt.Services;
using OrchardCore.LetsEncrypt.Settings;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;

namespace OrchardCore.LetsEncrypt
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<IDisplayDriver<ISite>, LetsEncryptAzureAuthSettingsDisplayDriver>();
            services.AddScoped<INavigationProvider, AdminMenu>();

            services.AddTransient<IConfigureOptions<LetsEncryptAzureAuthSettings>, LetsEncryptAzureAuthSettingsConfiguration>();
            services.AddScoped<IAzureServiceManager, AzureServiceManager>();
            services.AddScoped<ILetsEncryptService, LetsEncryptService>();
        }
    }
}

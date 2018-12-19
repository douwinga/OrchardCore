using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
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
            services.AddScoped<IDisplayDriver<ISite>, LetsEncryptConfigSettingsDisplayDriver>();
            services.AddScoped<INavigationProvider, AdminMenu>();

            services.AddTransient<IConfigureOptions<LetsEncryptAzureAuthSettings>, LetsEncryptAzureAuthSettingsConfiguration>();
            services.AddTransient<IConfigureOptions<LetsEncryptConfigSettings>, LetsEncryptConfigSettingsConfiguration>();
            services.AddScoped<IAzureServiceManager, AzureServiceManager>();
            services.AddScoped<ILetsEncryptService, LetsEncryptService>();
        }

        public override void Configure(IApplicationBuilder builder, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaRoute(
                name: "AcmeChallenge",
                areaName: "OrchardCore.LetsEncrypt",
                template: ".well-known/acme-challenge/{token}",
                defaults: new { controller = "Acme", action = "Challenge" }
            );
        }
    }
}

